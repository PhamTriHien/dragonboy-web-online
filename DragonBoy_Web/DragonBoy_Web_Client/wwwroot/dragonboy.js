/**
 * Module trình điều khiển phía trình duyệt cho DragonBoy WebAssembly.
 * Quản lý HTML5 Canvas 2D renderer, kho lưu trữ ảo IndexedDB/localStorage, sự kiện bàn phím/chuột/cảm ứng,
 * điều phối đa Tab qua iframe và nạp runtime .NET 8 WebAssembly.
 */
(function () {
    "use strict";

    const urlParams = new URLSearchParams(window.location.search);
    const initialTabId = parseInt(urlParams.get("tab") || "1", 10) || 1;
    const isEmbeddedTab = urlParams.get("embed") === "1";

    const canvas = document.getElementById("game-canvas");
    const ctx = canvas.getContext("2d", { alpha: false, desynchronized: true });
    const measureCanvas = document.createElement("canvas");
    const measureCtx = measureCanvas.getContext("2d");
    if (ctx && "textRendering" in ctx) ctx.textRendering = "optimizeLegibility";
    if (measureCtx && "textRendering" in measureCtx) measureCtx.textRendering = "optimizeLegibility";

    const loadingOverlay = document.getElementById("loading-overlay");
    const loadingBar = document.getElementById("loading-bar");
    const loadingText = document.getElementById("loading-text");
    const kbModal = document.getElementById("mobile-keyboard-modal");
    const kbInput = document.getElementById("mobile-keyboard-input");
    const kbOkBtn = document.getElementById("mobile-keyboard-ok");
    const kbCancelBtn = document.getElementById("mobile-keyboard-cancel");
    const tabIframesContainer = document.getElementById("tab-iframes-container");

    let virtualW = 1024;
    let virtualH = 600;
    let targetBaseW = 1024;
    let targetBaseH = 600;
    let isMobileOrTablet = false;
    let isPortraitRotated = false;
    let safeAreaPacked = 0;
    let lastRenderMode = -1;
    let clipDepth = 0;
    let matrixDepth = 0;
    let clipboardCache = "";
    let wasmExports = null;
    let activeVisibleTabId = initialTabId;
    const childTabIframes = new Map();

    // Texture storage: texId -> { img, canvas, ctx, width, height, loaded, tintCache }
    const textures = new Map();

    // Persisted VFS storage in memory + batched IndexedDB + localStorage + BroadcastChannel 0ms sync
    const persistedFiles = new Map();
    const pendingIdbOps = new Map();
    const DB_NAME = "DragonBoyWebVFS";
    const DB_STORE = "files";
    const LS_PREFIX = "db_vfs:";
    const instanceUid = `${initialTabId}_${Math.random().toString(36).slice(2)}_${Date.now()}`;
    const vfsChannel = (typeof BroadcastChannel !== "undefined") ? new BroadcastChannel("dragonboy_vfs_sync") : null;
    let idbInstance = null;
    let idbFlushTimer = 0;
    let lastHealthPingMs = Date.now();
    const HEALTH_KEEPALIVE_INTERVAL_MS = 180000;

    function pingRenderHealthKeepAlive(force) {
        if (isEmbeddedTab) return;
        const now = Date.now();
        if (!force && now - lastHealthPingMs < HEALTH_KEEPALIVE_INTERVAL_MS) return;
        lastHealthPingMs = now;
        try {
            fetch(`/healthz?ka=${now}`, {
                method: "GET",
                cache: "no-store",
                keepalive: true
            }).catch(() => { });
        } catch (_) { }
    }

    function flushIndexedDB() {
        if (idbFlushTimer) {
            clearTimeout(idbFlushTimer);
            idbFlushTimer = 0;
        }
        if (!idbInstance || pendingIdbOps.size === 0) return;
        const ops = Array.from(pendingIdbOps.entries());
        pendingIdbOps.clear();
        try {
            const tx = idbInstance.transaction(DB_STORE, "readwrite");
            const store = tx.objectStore(DB_STORE);
            for (let i = 0; i < ops.length; i++) {
                const [k, val] = ops[i];
                if (val === null) {
                    store.delete(k);
                } else {
                    store.put(val, k);
                }
            }
        } catch (_) { }
    }

    function scheduleIdbFlush() {
        if (idbFlushTimer || !idbInstance) return;
        idbFlushTimer = setTimeout(flushIndexedDB, 120);
    }

    if (vfsChannel) {
        vfsChannel.onmessage = (ev) => {
            const msg = ev && ev.data;
            if (!msg || msg.sender === instanceUid || !msg.vfsPath) return;
            if (msg.type === "put" && typeof msg.encodedLine === "string") {
                persistedFiles.set(msg.vfsPath, msg.encodedLine);
                if (wasmExports) {
                    try {
                        wasmExports.DragonBoy_Web_Host.Program.OnBroadcastFileUpdated(msg.encodedLine);
                    } catch (_) { }
                }
            } else if (msg.type === "del") {
                persistedFiles.delete(msg.vfsPath);
                if (wasmExports) {
                    try {
                        wasmExports.DragonBoy_Web_Host.Program.OnBroadcastFileDeleted(msg.vfsPath);
                    } catch (_) { }
                }
            }
        };
    }

    window.addEventListener("storage", (ev) => {
        if (!ev || !ev.key || !ev.key.startsWith(LS_PREFIX)) return;
        const vfsPath = ev.key.substring(LS_PREFIX.length);
        if (!isSharedVfsPath(vfsPath)) return;
        if (ev.newValue) {
            persistedFiles.set(vfsPath, ev.newValue);
            if (wasmExports && !vfsChannel) {
                try {
                    wasmExports.DragonBoy_Web_Host.Program.OnBroadcastFileUpdated(ev.newValue);
                } catch (_) { }
            }
        } else {
            persistedFiles.delete(vfsPath);
            if (wasmExports && !vfsChannel) {
                try {
                    wasmExports.DragonBoy_Web_Host.Program.OnBroadcastFileDeleted(vfsPath);
                } catch (_) { }
            }
        }
    });

    window.addEventListener("pagehide", flushIndexedDB);
    window.addEventListener("beforeunload", flushIndexedDB);
    document.addEventListener("visibilitychange", () => {
        if (document.visibilityState === "hidden") {
            flushIndexedDB();
        } else if (document.visibilityState === "visible") {
            pingRenderHealthKeepAlive(false);
        }
    });

    function openIndexedDB() {
        return new Promise((resolve) => {
            try {
                const req = indexedDB.open(DB_NAME, 1);
                req.onupgradeneeded = (e) => {
                    const db = e.target.result;
                    if (!db.objectStoreNames.contains(DB_STORE)) {
                        db.createObjectStore(DB_STORE);
                    }
                };
                req.onsuccess = (e) => {
                    idbInstance = e.target.result;
                    const tx = idbInstance.transaction(DB_STORE, "readonly");
                    const store = tx.objectStore(DB_STORE);
                    const keysReq = store.getAllKeys();
                    const valsReq = store.getAll();
                    tx.oncomplete = () => {
                        const keys = keysReq.result || [];
                        const vals = valsReq.result || [];
                        for (let i = 0; i < keys.length; i++) {
                            persistedFiles.set(keys[i], vals[i]);
                        }
                        syncFromLocalStorage(false);
                        resolve();
                    };
                    tx.onerror = () => {
                        syncFromLocalStorage(false);
                        resolve();
                    };
                };
                req.onerror = () => {
                    syncFromLocalStorage(false);
                    resolve();
                };
            } catch (_) {
                syncFromLocalStorage(false);
                resolve();
            }
        });
    }

    function syncFromLocalStorage(sharedOnly) {
        try {
            for (let i = 0; i < localStorage.length; i++) {
                const k = localStorage.key(i);
                if (!k || !k.startsWith(LS_PREFIX)) continue;
                const vfsPath = k.substring(LS_PREFIX.length);
                if (sharedOnly && !isSharedVfsPath(vfsPath)) continue;
                const val = localStorage.getItem(k);
                if (val) {
                    persistedFiles.set(vfsPath, val);
                }
            }
        } catch (_) { }
    }

    function isSharedVfsPath(vfsPath) {
        return vfsPath === "/rms/tabs.xml" ||
            vfsPath === "/rms/mod_config.ini" ||
            vfsPath.indexOf("/rms/tab_status_") === 0 ||
            vfsPath.indexOf("/rms/stop_tab_") === 0;
    }

    function writeToStorage(vfsPath, encodedLine) {
        persistedFiles.set(vfsPath, encodedLine);
        const shared = isSharedVfsPath(vfsPath);
        try {
            if (shared || encodedLine.length < 32768) {
                localStorage.setItem(LS_PREFIX + vfsPath, encodedLine);
            }
        } catch (_) { }
        if (shared && vfsChannel) {
            try {
                vfsChannel.postMessage({
                    type: "put",
                    vfsPath,
                    encodedLine,
                    sender: instanceUid
                });
            } catch (_) { }
        }
        pendingIdbOps.set(vfsPath, encodedLine);
        scheduleIdbFlush();
    }

    function removeFromStorage(vfsPath) {
        persistedFiles.delete(vfsPath);
        const shared = isSharedVfsPath(vfsPath);
        try {
            localStorage.removeItem(LS_PREFIX + vfsPath);
        } catch (_) { }
        if (shared && vfsChannel) {
            try {
                vfsChannel.postMessage({
                    type: "del",
                    vfsPath,
                    sender: instanceUid
                });
            } catch (_) { }
        }
        pendingIdbOps.set(vfsPath, null);
        scheduleIdbFlush();
    }

    function utf8ToBase64(str) {
        const bytes = new TextEncoder().encode(str);
        let bin = "";
        for (let i = 0; i < bytes.length; i++) {
            bin += String.fromCharCode(bytes[i]);
        }
        return btoa(bin);
    }

    function detectMobileOrTabletDevice() {
        const ua = navigator.userAgent || "";
        if (/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(ua)) {
            return true;
        }
        if (navigator.userAgentData && navigator.userAgentData.mobile) {
            return true;
        }
        if (/Macintosh/i.test(ua) && navigator.maxTouchPoints && navigator.maxTouchPoints > 1) {
            return true;
        }
        if (window.matchMedia && window.matchMedia("(pointer: coarse)").matches && (("ontouchstart" in window) || navigator.maxTouchPoints > 0)) {
            return true;
        }
        return false;
    }

    function parseCssPx(val) {
        const n = parseFloat(val);
        return Number.isFinite(n) && n > 0 ? n : 0;
    }

    function updateCanvasViewportSize() {
        if (kbModal && kbModal.classList.contains("visible")) {
            return;
        }
        isMobileOrTablet = detectMobileOrTabletDevice();
        const rect = canvas.getBoundingClientRect();
        const vv = window.visualViewport;
        const fallbackW = (vv && vv.width > 0) ? vv.width : (window.innerWidth || document.documentElement.clientWidth || 1024);
        const fallbackH = (vv && vv.height > 0) ? vv.height : (window.innerHeight || document.documentElement.clientHeight || 600);
        const rawCssW = Math.max(240, (rect && rect.width > 0) ? rect.width : fallbackW);
        const rawCssH = Math.max(240, (rect && rect.height > 0) ? rect.height : fallbackH);
        const cssW = Math.round(rawCssW);
        const cssH = Math.round(rawCssH);

        isPortraitRotated = isMobileOrTablet && (cssW < cssH);
        const landscapeW = isPortraitRotated ? cssH : cssW;
        const landscapeH = isPortraitRotated ? cssW : cssH;
        const shortSide = Math.min(landscapeW, landscapeH);

        let baseH;
        if (isMobileOrTablet) {
            if (shortSide < 600) {
                baseH = Math.min(Math.max(targetBaseH || 600, 540), 720);
            } else {
                baseH = Math.min(Math.max(targetBaseH || 600, 600), 900);
            }
        } else {
            baseH = Math.min(Math.max(targetBaseH || 600, 600), 1080);
        }

        const aspect = landscapeW / Math.max(1, landscapeH);
        let vh = baseH;
        let vw = Math.round(vh * aspect);

        const minW = Math.max(960, Math.round(baseH * 1.6));
        const maxW = Math.round(baseH * 2.4);
        if (vw < minW) {
            vw = minW;
            vh = Math.round(minW / Math.max(0.5, aspect));
        } else if (vw > maxW) {
            vw = maxW;
            vh = Math.round(maxW / Math.max(0.5, aspect));
        }

        vw = (vw + 1) & ~1;
        vh = (vh + 1) & ~1;
        virtualW = vw;
        virtualH = vh;

        const dpr = Math.min(Math.max(window.devicePixelRatio || 1, 1), 4);
        const backingW = Math.max(320, Math.round(rawCssW * dpr));
        const backingH = Math.max(240, Math.round(rawCssH * dpr));
        if (canvas.width !== backingW || canvas.height !== backingH) {
            canvas.width = backingW;
            canvas.height = backingH;
        }

        try {
            const cs = getComputedStyle(document.documentElement);
            const rawTop = parseCssPx(cs.getPropertyValue("--sat"));
            const rawRight = parseCssPx(cs.getPropertyValue("--sar"));
            const rawBottom = parseCssPx(cs.getPropertyValue("--sab"));
            const rawLeft = parseCssPx(cs.getPropertyValue("--sal"));

            let effLeft = rawLeft;
            let effTop = rawTop;
            let effRight = rawRight;
            let effBottom = rawBottom;
            if (isPortraitRotated) {
                effLeft = rawTop;
                effTop = rawRight;
                effRight = rawBottom;
                effBottom = rawLeft;
            }

            const zoom = 2;
            const scaleX = landscapeW / Math.max(1, virtualW);
            const scaleY = landscapeH / Math.max(1, virtualH);
            const vLeft = Math.min(120, Math.max(0, Math.round(effLeft / (scaleX * zoom))));
            const vTop = Math.min(120, Math.max(0, Math.round(effTop / (scaleY * zoom))));
            const vRight = Math.min(120, Math.max(0, Math.round(effRight / (scaleX * zoom))));
            const vBottom = Math.min(120, Math.max(0, Math.round(effBottom / (scaleY * zoom))));
            safeAreaPacked = (vLeft & 0xFF) | ((vTop & 0xFF) << 8) | ((vRight & 0xFF) << 16) | ((vBottom & 0xFF) << 24);
        } catch (_) {
            safeAreaPacked = 0;
        }
    }

    function applyBaseTransform() {
        if (isPortraitRotated) {
            const scaleX = canvas.height / Math.max(1, virtualW);
            const scaleY = canvas.width / Math.max(1, virtualH);
            ctx.setTransform(0, scaleX, -scaleY, 0, canvas.width, 0);
        } else {
            const scaleX = canvas.width / Math.max(1, virtualW);
            const scaleY = canvas.height / Math.max(1, virtualH);
            ctx.setTransform(scaleX, 0, 0, scaleY, 0, 0);
        }
    }

    let pseudoFullscreen = false;
    let pendingFullscreenAction = 0;

    function getRootDocument() {
        try {
            if (window.top && window.top.document) return window.top.document;
        } catch (_) { }
        return document;
    }

    function getFullscreenElement() {
        const doc = getRootDocument();
        return doc.fullscreenElement || doc.webkitFullscreenElement || doc.mozFullScreenElement || doc.msFullscreenElement || null;
    }

    function isDocumentFullscreen() {
        if (getFullscreenElement()) return true;
        if (pseudoFullscreen) return true;
        if (!isMobileOrTablet && window.screen && window.screen.width > 0 && window.screen.height > 0) {
            let winW = window.innerWidth;
            let winH = window.innerHeight;
            try {
                if (window.top) {
                    winW = window.top.innerWidth || winW;
                    winH = window.top.innerHeight || winH;
                }
            } catch (_) { }
            if (Math.abs(winW - window.screen.width) <= 2 && Math.abs(winH - window.screen.height) <= 2) {
                return true;
            }
        }
        return false;
    }

    function applyFullscreenChange(wantFullscreen) {
        const doc = getRootDocument();
        const el = doc.documentElement;
        const curEl = getFullscreenElement();
        if (wantFullscreen) {
            if (curEl) {
                pendingFullscreenAction = 0;
                return;
            }
            const reqFn = el.requestFullscreen || el.webkitRequestFullscreen || el.mozRequestFullScreen || el.msRequestFullscreen;
            if (reqFn) {
                try {
                    const res = reqFn.call(el);
                    if (res && typeof res.then === "function") {
                        res.then(() => {
                            pendingFullscreenAction = 0;
                            if (isMobileOrTablet && screen.orientation && screen.orientation.lock) {
                                screen.orientation.lock("landscape").catch(() => { });
                            }
                            updateCanvasViewportSize();
                        }).catch(() => {
                            pendingFullscreenAction = 1;
                        });
                    } else {
                        pendingFullscreenAction = 0;
                        updateCanvasViewportSize();
                    }
                } catch (_) {
                    pendingFullscreenAction = 1;
                }
            } else {
                pseudoFullscreen = true;
                pendingFullscreenAction = 0;
                try { window.scrollTo(0, 1); } catch (_) { }
                updateCanvasViewportSize();
            }
        } else {
            pendingFullscreenAction = 0;
            pseudoFullscreen = false;
            if (curEl) {
                const exitFn = doc.exitFullscreen || doc.webkitExitFullscreen || doc.mozCancelFullScreen || doc.msExitFullscreen;
                if (exitFn) {
                    try {
                        const res = exitFn.call(doc);
                        if (res && typeof res.catch === "function") res.catch(() => { });
                    } catch (_) { }
                }
            }
            updateCanvasViewportSize();
        }
    }

    window.addEventListener("resize", updateCanvasViewportSize);
    window.addEventListener("orientationchange", () => {
        updateCanvasViewportSize();
        setTimeout(updateCanvasViewportSize, 150);
    });
    document.addEventListener("fullscreenchange", updateCanvasViewportSize);
    document.addEventListener("webkitfullscreenchange", updateCanvasViewportSize);
    try {
        if (window.top && window.top.document && window.top.document !== document) {
            window.top.document.addEventListener("fullscreenchange", updateCanvasViewportSize);
            window.top.document.addEventListener("webkitfullscreenchange", updateCanvasViewportSize);
        }
    } catch (_) { }
    if (window.visualViewport) {
        window.visualViewport.addEventListener("resize", updateCanvasViewportSize);
        window.visualViewport.addEventListener("scroll", updateCanvasViewportSize);
    }
    if (typeof ResizeObserver !== "undefined") {
        try {
            const canvasResizeObserver = new ResizeObserver(() => updateCanvasViewportSize());
            canvasResizeObserver.observe(canvas);
        } catch (_) { }
    }
    updateCanvasViewportSize();

    function buildFontSpec(fontSize, fontCode) {
        switch (fontCode) {
            case 1: return `bold ${fontSize}px Tahoma, Verdana, sans-serif`;
            case 11: return `normal ${fontSize}px Tahoma, Verdana, sans-serif`;
            case 2: return `600 ${fontSize}px 'Segoe UI', system-ui, sans-serif`;
            case 12: return `normal ${fontSize}px 'Segoe UI', system-ui, sans-serif`;
            case 3: return `bold ${fontSize}px Consolas, 'Courier New', monospace`;
            case 13: return `normal ${fontSize}px Consolas, 'Courier New', monospace`;
            case 10: return `normal ${fontSize}px 'DragonBoyFont1', Tahoma, sans-serif`;
            case 20: return `bold ${fontSize}px 'DragonBoyFont2', Tahoma, sans-serif`;
            default: return `bold ${fontSize}px 'DragonBoyFont0', Tahoma, sans-serif`;
        }
    }

    function ensureTextureCanvas(entry) {
        if (entry.canvas) return entry.canvas;
        if (!entry.loaded || !entry.img) return null;
        const c = document.createElement("canvas");
        c.width = entry.width;
        c.height = entry.height;
        const cctx = c.getContext("2d", { willReadFrequently: true });
        cctx.imageSmoothingEnabled = false;
        cctx.drawImage(entry.img, 0, 0, entry.width, entry.height);
        entry.canvas = c;
        entry.ctx = cctx;
        return c;
    }

    function getTintedSource(entry, r, g, b) {
        if (!entry.loaded) return null;
        const source = entry.canvas || entry.img;
        if (!source) return null;
        if (r >= 0.99 && g >= 0.99 && b >= 0.99) return source;

        const rq = Math.round(r * 32);
        const gq = Math.round(g * 32);
        const bq = Math.round(b * 32);
        const key = (rq << 12) | (gq << 6) | bq;
        if (!entry.tintCache) entry.tintCache = new Map();
        let tc = entry.tintCache.get(key);
        if (tc) return tc;

        if (entry.tintCache.size > 8) entry.tintCache.clear();
        tc = document.createElement("canvas");
        tc.width = entry.width;
        tc.height = entry.height;
        const tctx = tc.getContext("2d");
        tctx.imageSmoothingEnabled = false;
        tctx.drawImage(source, 0, 0, entry.width, entry.height);
        tctx.globalCompositeOperation = "multiply";
        tctx.fillStyle = `rgb(${Math.round(r * 255)},${Math.round(g * 255)},${Math.round(b * 255)})`;
        tctx.fillRect(0, 0, entry.width, entry.height);
        tctx.globalCompositeOperation = "destination-in";
        tctx.drawImage(source, 0, 0, entry.width, entry.height);
        entry.tintCache.set(key, tc);
        return tc;
    }

    function toVirtualCoords(clientX, clientY) {
        const rect = canvas.getBoundingClientRect();
        const rw = Math.max(1, rect.width);
        const rh = Math.max(1, rect.height);
        const relX = clientX - rect.left;
        const relY = clientY - rect.top;
        if (isPortraitRotated) {
            return {
                x: (relY / rh) * virtualW,
                y: ((rw - relX) / rw) * virtualH
            };
        }
        return {
            x: (relX / rw) * virtualW,
            y: (relY / rh) * virtualH
        };
    }

    function setupInputListeners() {
        canvas.addEventListener("pointerdown", (e) => {
            if (kbModal.classList.contains("visible")) return;
            canvas.focus();
            try { canvas.setPointerCapture(e.pointerId); } catch (_) { }
            const pt = toVirtualCoords(e.clientX, e.clientY);
            if (wasmExports) {
                wasmExports.DragonBoy_Web_Host.WebInputBridge.OnPointerEvent(0, pt.x, pt.y, e.button || 0);
            }
            e.preventDefault();
        }, { passive: false });

        canvas.addEventListener("pointermove", (e) => {
            if (kbModal.classList.contains("visible")) return;
            const pt = toVirtualCoords(e.clientX, e.clientY);
            if (wasmExports) {
                wasmExports.DragonBoy_Web_Host.WebInputBridge.OnPointerEvent(2, pt.x, pt.y, e.button || 0);
            }
            e.preventDefault();
        }, { passive: false });

        const onUp = (e) => {
            if (kbModal.classList.contains("visible")) return;
            const pt = toVirtualCoords(e.clientX, e.clientY);
            if (wasmExports) {
                wasmExports.DragonBoy_Web_Host.WebInputBridge.OnPointerEvent(1, pt.x, pt.y, e.button || 0);
                wasmExports.DragonBoy_Web_Host.Program.OnInputImmediate(performance.now());
            }
            if (pendingFullscreenAction !== 0) {
                const want = pendingFullscreenAction > 0;
                pendingFullscreenAction = 0;
                applyFullscreenChange(want);
            }
        };
        window.addEventListener("pointerup", onUp, { passive: true });
        window.addEventListener("pointercancel", onUp, { passive: true });

        window.addEventListener("blur", () => {
            if (wasmExports) {
                wasmExports.DragonBoy_Web_Host.WebInputBridge.OnBlurReset();
            }
        });

        document.addEventListener("visibilitychange", () => {
            if (document.hidden && wasmExports) {
                wasmExports.DragonBoy_Web_Host.WebInputBridge.OnBlurReset();
            }
        });

        canvas.addEventListener("contextmenu", (e) => e.preventDefault());

        canvas.addEventListener("wheel", (e) => {
            if (wasmExports) {
                wasmExports.DragonBoy_Web_Host.WebInputBridge.OnWheelEvent(e.deltaY);
            }
            e.preventDefault();
        }, { passive: false });

        window.addEventListener("keydown", (e) => {
            if (document.activeElement === kbInput) return;
            if (e.code === "F11" || (e.altKey && (e.code === "Enter" || e.code === "NumpadEnter"))) {
                e.preventDefault();
                if (!e.repeat) {
                    applyFullscreenChange(!isDocumentFullscreen());
                }
                return;
            }
            if (e.ctrlKey && (e.code === "KeyV" || e.code === "KeyC")) {
                if (navigator.clipboard && navigator.clipboard.readText && e.code === "KeyV") {
                    navigator.clipboard.readText().then(t => { clipboardCache = t || ""; }).catch(() => { });
                }
            }
            if (wasmExports) {
                wasmExports.DragonBoy_Web_Host.WebInputBridge.OnKeyEvent(1, e.code || "", e.key || "", e.ctrlKey ? 1 : 0, e.shiftKey ? 1 : 0, e.altKey ? 1 : 0);
            }
            if (["ArrowUp", "ArrowDown", "ArrowLeft", "ArrowRight", "Tab", "Space", "F1", "F2", "F3", "F4", "F5", "F6", "F7", "F8", "F9", "F10", "F11"].includes(e.code)) {
                e.preventDefault();
            }
        });

        window.addEventListener("keyup", (e) => {
            if (document.activeElement === kbInput) return;
            if (e.code === "F11" || (e.altKey && (e.code === "Enter" || e.code === "NumpadEnter"))) {
                e.preventDefault();
                return;
            }
            if (wasmExports) {
                wasmExports.DragonBoy_Web_Host.WebInputBridge.OnKeyEvent(0, e.code || "", e.key || "", e.ctrlKey ? 1 : 0, e.shiftKey ? 1 : 0, e.altKey ? 1 : 0);
            }
        });

        kbInput.addEventListener("input", () => {
            if (wasmExports) {
                wasmExports.DragonBoy_Web_Host.WebInputBridge.OnSoftKeyboardCommit(kbInput.value);
            }
        });

        const submitSoftKeyboard = () => {
            const val = kbInput.value;
            kbModal.classList.remove("visible");
            kbInput.blur();
            canvas.focus();
            if (wasmExports) {
                wasmExports.DragonBoy_Web_Host.WebInputBridge.OnSoftKeyboardSubmit(val, 1);
            }
            setTimeout(updateCanvasViewportSize, 80);
        };

        const cancelSoftKeyboard = () => {
            const val = kbInput.value;
            kbModal.classList.remove("visible");
            kbInput.blur();
            canvas.focus();
            if (wasmExports) {
                wasmExports.DragonBoy_Web_Host.WebInputBridge.OnSoftKeyboardSubmit(val, -1);
            }
            setTimeout(updateCanvasViewportSize, 80);
        };

        kbInput.addEventListener("keydown", (e) => {
            if (e.key === "Enter") {
                submitSoftKeyboard();
                e.preventDefault();
            } else if (e.key === "Escape") {
                cancelSoftKeyboard();
                e.preventDefault();
            }
        });

        kbOkBtn.addEventListener("click", submitSoftKeyboard);
        if (kbCancelBtn) {
            kbCancelBtn.addEventListener("click", cancelSoftKeyboard);
        }
    }

    // Multi-Tab Iframe coordination (Parent Window <-> Child Tab Iframes)
    function ensureChildTabIframe(tabId) {
        if (tabId <= 1) return null;
        if (isEmbeddedTab) {
            window.parent.postMessage({ type: "db_prewarm_tab", tabId }, "*");
            return null;
        }
        let iframe = childTabIframes.get(tabId);
        if (!iframe) {
            iframe = document.createElement("iframe");
            iframe.className = "tab-iframe";
            iframe.allow = "fullscreen; clipboard-read; clipboard-write";
            iframe.allowFullscreen = true;
            iframe.src = `${window.location.pathname}?tab=${tabId}&embed=1`;
            tabIframesContainer.appendChild(iframe);
            childTabIframes.set(tabId, iframe);
        }
        return iframe;
    }

    function activateTabInHost(tabId, keepHubOpen, bx, by) {
        if (isEmbeddedTab) {
            window.parent.postMessage({ type: "db_switch_tab", tabId, keepHubOpen, bx, by }, "*");
            return;
        }
        activeVisibleTabId = tabId;
        for (const [id, frame] of childTabIframes.entries()) {
            if (id === tabId) {
                frame.classList.add("active");
                try {
                    frame.contentWindow.postMessage({ type: "db_tab_activated", keepHubOpen: keepHubOpen ? 1 : 0, bx, by }, "*");
                    frame.contentWindow.focus();
                } catch (_) { }
            } else {
                frame.classList.remove("active");
            }
        }
        if (tabId === 1) {
            if (wasmExports) {
                wasmExports.DragonBoy_Web_Host.Program.OnTabActivated(keepHubOpen ? 1 : 0, bx, by);
            }
            canvas.focus();
        } else {
            const targetFrame = ensureChildTabIframe(tabId);
            if (targetFrame) {
                targetFrame.classList.add("active");
                setTimeout(() => {
                    try {
                        targetFrame.contentWindow.postMessage({ type: "db_tab_activated", keepHubOpen: keepHubOpen ? 1 : 0, bx, by }, "*");
                    } catch (_) { }
                }, 120);
            }
        }
    }

    window.addEventListener("message", (e) => {
        const d = e.data;
        if (!d || typeof d.type !== "string") return;
        if (d.type === "db_switch_tab") {
            activateTabInHost(d.tabId, d.keepHubOpen, d.bx, d.by);
        } else if (d.type === "db_prewarm_tab") {
            ensureChildTabIframe(d.tabId);
        } else if (d.type === "db_stop_tab") {
            if (!isEmbeddedTab) {
                const frame = childTabIframes.get(d.tabId);
                if (frame && frame.contentWindow) {
                    frame.contentWindow.postMessage({ type: "db_do_stop_self" }, "*");
                }
            }
        } else if (d.type === "db_delete_tab") {
            if (!isEmbeddedTab) {
                const frame = childTabIframes.get(d.tabId);
                if (frame) {
                    frame.remove();
                    childTabIframes.delete(d.tabId);
                }
                if (activeVisibleTabId === d.tabId) {
                    activateTabInHost(1, true, -1, -1);
                }
            }
        } else if (d.type === "db_tab_activated") {
            updateCanvasViewportSize();
            if (wasmExports) {
                wasmExports.DragonBoy_Web_Host.Program.OnTabActivated(d.keepHubOpen || 0, d.bx ?? -1, d.by ?? -1);
            }
        } else if (d.type === "db_do_stop_self") {
            if (wasmExports) {
                wasmExports.DragonBoy_Web_Host.Program.OnTabStopRequested();
            }
        }
    });

    function startGameLoops() {
        let lastRafTime = 0;
        let lastRenderTime = 0;

        function onRaf(timestamp) {
            lastRafTime = performance.now();
            let isChildActive = true;
            if (isEmbeddedTab) {
                try {
                    if (window.frameElement) {
                        isChildActive = window.frameElement.classList.contains("active");
                    }
                } catch (_) { }
            }
            const isTabVisible = !document.hidden && (isEmbeddedTab ? isChildActive : activeVisibleTabId === 1);
            if (wasmExports) {
                let shouldRender = isTabVisible;
                if (shouldRender) {
                    const targetFps = wasmExports.DragonBoy_Web_Host.Program.GetTargetFrameRate() || 60;
                    if (targetFps > 0 && targetFps < 58) {
                        const minFrameInterval = (1000.0 / targetFps) - 2.0;
                        if (lastRafTime - lastRenderTime < minFrameInterval) {
                            shouldRender = false;
                        }
                    }
                    if (shouldRender) {
                        lastRenderTime = lastRafTime;
                    }
                }
                wasmExports.DragonBoy_Web_Host.Program.OnBrowserFrame(lastRafTime, shouldRender ? 1 : 0);
            }
            requestAnimationFrame(onRaf);
        }
        requestAnimationFrame(onRaf);

        // Background Web Worker heartbeat so Auto-Play, Socket Keep-Alive & Render Anti-Sleep never pause when browser tab is minimized
        try {
            const workerBlob = new Blob([
                "setInterval(function(){ postMessage(Date.now()); }, 25);"
            ], { type: "application/javascript" });
            const worker = new Worker(URL.createObjectURL(workerBlob));
            worker.onmessage = function () {
                const now = performance.now();
                if (now - lastRafTime >= 40 && wasmExports) {
                    wasmExports.DragonBoy_Web_Host.Program.OnBrowserFrame(now, 0);
                }
                pingRenderHealthKeepAlive(false);
            };
        } catch (_) {
            if (!isEmbeddedTab) {
                setInterval(() => pingRenderHealthKeepAlive(false), 60000);
            }
        }
    }

    window.DragonBoyWeb = {
        getCanvasWidth: () => virtualW,
        getCanvasHeight: () => virtualH,
        isFullScreen: () => isDocumentFullscreen(),
        setResolution: (w, h, fullscreen) => {
            if (w > 0 && h > 0) {
                targetBaseW = w;
                targetBaseH = h;
                updateCanvasViewportSize();
            }
            if (Boolean(fullscreen) !== isDocumentFullscreen()) {
                applyFullscreenChange(Boolean(fullscreen));
            }
        },
        setWindowTitle: (title) => {
            if (!isEmbeddedTab && title) document.title = title;
        },
        isTouchDevice: () => isMobileOrTablet,
        getSafeAreaInsetsPacked: () => safeAreaPacked,

        isTextureLoaded: (texId) => {
            const entry = textures.get(texId);
            return !!(entry && entry.loaded);
        },

        registerLazyTexture: (texId, url, width, height) => {
            const entry = {
                img: null,
                canvas: null,
                ctx: null,
                width: width || 1,
                height: height || 1,
                loaded: false,
                url: url,
                isPixelArt: typeof url === "string" && url.toLowerCase().indexOf("/myfont/") >= 0,
                pendingScales: []
            };
            const img = new Image();
            img.decoding = "async";
            img.onload = () => {
                entry.loaded = true;
                if ((!entry.width || entry.width <= 1) && img.naturalWidth > 0) entry.width = img.naturalWidth;
                if ((!entry.height || entry.height <= 1) && img.naturalHeight > 0) entry.height = img.naturalHeight;
                if (entry.pendingScales.length > 0) {
                    const tasks = entry.pendingScales.slice();
                    entry.pendingScales.length = 0;
                    for (const fn of tasks) fn();
                }
            };
            img.src = url;
            entry.img = img;
            textures.set(texId, entry);
        },

        createTextureFromBytes: (texId, imageBytes, width, height) => {
            const entry = {
                img: null,
                canvas: null,
                ctx: null,
                width: width || 1,
                height: height || 1,
                loaded: false,
                pendingScales: []
            };
            const blob = new Blob([imageBytes]);
            const blobUrl = URL.createObjectURL(blob);
            const img = new Image();
            img.onload = () => {
                entry.loaded = true;
                if ((!entry.width || entry.width <= 1) && img.naturalWidth > 0) entry.width = img.naturalWidth;
                if ((!entry.height || entry.height <= 1) && img.naturalHeight > 0) entry.height = img.naturalHeight;
                URL.revokeObjectURL(blobUrl);
                if (entry.pendingScales.length > 0) {
                    const tasks = entry.pendingScales.slice();
                    entry.pendingScales.length = 0;
                    for (const fn of tasks) fn();
                }
            };
            img.src = blobUrl;
            entry.img = img;
            textures.set(texId, entry);
        },

        scaleTextureNN: (dstTexId, srcTexId, srcW, srcH, targetW, targetH) => {
            const dstCanvas = document.createElement("canvas");
            dstCanvas.width = targetW;
            dstCanvas.height = targetH;
            const dstCtx = dstCanvas.getContext("2d");
            dstCtx.imageSmoothingEnabled = false;

            const dstEntry = {
                img: null,
                canvas: dstCanvas,
                ctx: dstCtx,
                width: targetW,
                height: targetH,
                loaded: false,
                pendingScales: []
            };
            textures.set(dstTexId, dstEntry);

            const srcEntry = textures.get(srcTexId);
            if (!srcEntry) return;
            dstEntry.isPixelArt = Boolean(srcEntry.isPixelArt);

            const performScale = () => {
                const srcDrawable = srcEntry.canvas || srcEntry.img;
                if (srcDrawable) {
                    const sw = srcDrawable.naturalWidth || srcDrawable.width || srcEntry.width;
                    const sh = srcDrawable.naturalHeight || srcDrawable.height || srcEntry.height;
                    dstCtx.clearRect(0, 0, targetW, targetH);
                    dstCtx.imageSmoothingEnabled = false;
                    dstCtx.drawImage(srcDrawable, 0, 0, sw, sh, 0, 0, targetW, targetH);
                    dstEntry.loaded = true;
                    if (dstEntry.tintCache) dstEntry.tintCache.clear();
                    if (dstEntry.pendingScales.length > 0) {
                        const tasks = dstEntry.pendingScales.slice();
                        dstEntry.pendingScales.length = 0;
                        for (const fn of tasks) fn();
                    }
                }
            };

            if (srcEntry.loaded) {
                performScale();
            } else if (srcEntry.pendingScales) {
                srcEntry.pendingScales.push(performScale);
            }
        },

        quantizeTextureJava2x2: (dstTexId, srcTexId, targetW, targetH) => {
            const dstCanvas = document.createElement("canvas");
            dstCanvas.width = targetW;
            dstCanvas.height = targetH;
            const dstCtx = dstCanvas.getContext("2d", { willReadFrequently: true });
            dstCtx.imageSmoothingEnabled = false;

            const dstEntry = {
                img: null,
                canvas: dstCanvas,
                ctx: dstCtx,
                width: targetW,
                height: targetH,
                loaded: false,
                pendingScales: []
            };
            textures.set(dstTexId, dstEntry);

            const srcEntry = textures.get(srcTexId);
            if (!srcEntry) return;

            const performQuantize = () => {
                const srcDrawable = srcEntry.canvas || srcEntry.img;
                if (!srcDrawable) return;
                const sw = srcDrawable.naturalWidth || srcDrawable.width || srcEntry.width || targetW;
                const sh = srcDrawable.naturalHeight || srcDrawable.height || srcEntry.height || targetH;
                if (dstCanvas.width !== sw || dstCanvas.height !== sh) {
                    dstCanvas.width = sw;
                    dstCanvas.height = sh;
                    dstEntry.width = sw;
                    dstEntry.height = sh;
                }
                dstCtx.clearRect(0, 0, sw, sh);
                dstCtx.imageSmoothingEnabled = false;
                dstCtx.drawImage(srcDrawable, 0, 0, sw, sh, 0, 0, sw, sh);
                if (sw >= 2 && sh >= 2) {
                    const imgData = dstCtx.getImageData(0, 0, sw, sh);
                    const d = imgData.data;
                    const stride = sw * 4;
                    for (let y = 0; y + 1 < sh; y += 2) {
                        const row0 = y * stride;
                        const row1 = (y + 1) * stride;
                        for (let x = 0; x + 1 < sw; x += 2) {
                            const i0 = row0 + x * 4;
                            const i1 = i0 + 4;
                            const i2 = row1 + x * 4;
                            const i3 = i2 + 4;
                            const a0 = d[i0 + 3], a1 = d[i1 + 3], a2 = d[i2 + 3], a3 = d[i3 + 3];
                            const sumA = a0 + a1 + a2 + a3;
                            let br = 0, bg = 0, bb = 0, ba = 0;
                            if (sumA >= 256) {
                                const r0 = d[i0], g0 = d[i0 + 1], b0 = d[i0 + 2];
                                const r1 = d[i1], g1 = d[i1 + 1], b1 = d[i1 + 2];
                                const r2 = d[i2], g2 = d[i2 + 1], b2 = d[i2 + 2];
                                const r3 = d[i3], g3 = d[i3 + 1], b3 = d[i3 + 2];
                                const ar = ((r0 * a0 + r1 * a1 + r2 * a2 + r3 * a3) / sumA) | 0;
                                const ag = ((g0 * a0 + g1 * a1 + g2 * a2 + g3 * a3) / sumA) | 0;
                                const ab = ((b0 * a0 + b1 * a1 + b2 * a2 + b3 * a3) / sumA) | 0;
                                br = ar; bg = ag; bb = ab; ba = 255;
                                let bestDist = 999999;
                                if (a0 >= 128) { const dist = Math.abs(r0 - ar) + Math.abs(g0 - ag) + Math.abs(b0 - ab); if (dist < bestDist) { bestDist = dist; br = r0; bg = g0; bb = b0; } }
                                if (a1 >= 128) { const dist = Math.abs(r1 - ar) + Math.abs(g1 - ag) + Math.abs(b1 - ab); if (dist < bestDist) { bestDist = dist; br = r1; bg = g1; bb = b1; } }
                                if (a2 >= 128) { const dist = Math.abs(r2 - ar) + Math.abs(g2 - ag) + Math.abs(b2 - ab); if (dist < bestDist) { bestDist = dist; br = r2; bg = g2; bb = b2; } }
                                if (a3 >= 128) { const dist = Math.abs(r3 - ar) + Math.abs(g3 - ag) + Math.abs(b3 - ab); if (dist < bestDist) { bestDist = dist; br = r3; bg = g3; bb = b3; } }
                            }
                            d[i0] = br; d[i0 + 1] = bg; d[i0 + 2] = bb; d[i0 + 3] = ba;
                            d[i1] = br; d[i1 + 1] = bg; d[i1 + 2] = bb; d[i1 + 3] = ba;
                            d[i2] = br; d[i2 + 1] = bg; d[i2 + 2] = bb; d[i2 + 3] = ba;
                            d[i3] = br; d[i3 + 1] = bg; d[i3 + 2] = bb; d[i3 + 3] = ba;
                        }
                    }
                    dstCtx.putImageData(imgData, 0, 0);
                }
                dstEntry.loaded = true;
                if (dstEntry.tintCache) dstEntry.tintCache.clear();
                if (dstEntry.pendingScales.length > 0) {
                    const tasks = dstEntry.pendingScales.slice();
                    dstEntry.pendingScales.length = 0;
                    for (const fn of tasks) fn();
                }
            };

            if (srcEntry.loaded) {
                performQuantize();
            } else if (srcEntry.pendingScales) {
                srcEntry.pendingScales.push(performQuantize);
            }
        },

        blendTexture: (dstTexId, srcTexId, targetW, targetH, level, r, g, b) => {
            const dstCanvas = document.createElement("canvas");
            dstCanvas.width = targetW;
            dstCanvas.height = targetH;
            const dstCtx = dstCanvas.getContext("2d");
            dstCtx.imageSmoothingEnabled = false;

            const dstEntry = {
                img: null,
                canvas: dstCanvas,
                ctx: dstCtx,
                width: targetW,
                height: targetH,
                loaded: false,
                pendingScales: []
            };
            textures.set(dstTexId, dstEntry);

            const srcEntry = textures.get(srcTexId);
            if (!srcEntry) return;
            dstEntry.isPixelArt = Boolean(srcEntry.isPixelArt);

            const performBlend = () => {
                const srcDrawable = srcEntry.canvas || srcEntry.img;
                if (srcDrawable) {
                    const sw = srcDrawable.naturalWidth || srcDrawable.width || srcEntry.width;
                    const sh = srcDrawable.naturalHeight || srcDrawable.height || srcEntry.height;
                    dstCtx.clearRect(0, 0, targetW, targetH);
                    dstCtx.imageSmoothingEnabled = false;
                    dstCtx.globalCompositeOperation = "source-over";
                    dstCtx.drawImage(srcDrawable, 0, 0, sw, sh, 0, 0, targetW, targetH);
                    if (level > 0.001) {
                        dstCtx.globalCompositeOperation = "source-atop";
                        dstCtx.fillStyle = `rgba(${r},${g},${b},${Math.min(1, Math.max(0, level))})`;
                        dstCtx.fillRect(0, 0, targetW, targetH);
                        dstCtx.globalCompositeOperation = "source-over";
                    }
                    dstEntry.loaded = true;
                    if (dstEntry.tintCache) dstEntry.tintCache.clear();
                    if (dstEntry.pendingScales.length > 0) {
                        const tasks = dstEntry.pendingScales.slice();
                        dstEntry.pendingScales.length = 0;
                        for (const fn of tasks) fn();
                    }
                }
            };

            if (srcEntry.loaded) {
                performBlend();
            } else if (srcEntry.pendingScales) {
                srcEntry.pendingScales.push(performBlend);
            }
        },

        uploadRgbaPixels: (texId, rgbaBytes, width, height) => {
            let entry = textures.get(texId);
            if (!entry || !entry.canvas || entry.width !== width || entry.height !== height) {
                const c = document.createElement("canvas");
                c.width = width;
                c.height = height;
                const cctx = c.getContext("2d");
                cctx.imageSmoothingEnabled = false;
                entry = {
                    img: null,
                    canvas: c,
                    ctx: cctx,
                    width: width,
                    height: height,
                    loaded: true,
                    pendingScales: []
                };
                textures.set(texId, entry);
            }
            const clamped = new Uint8ClampedArray(rgbaBytes.buffer, rgbaBytes.byteOffset, rgbaBytes.byteLength);
            const imgData = new ImageData(clamped, width, height);
            entry.ctx.putImageData(imgData, 0, 0);
            entry.loaded = true;
            if (entry.tintCache) entry.tintCache.clear();
        },

        readTexturePixels: (texId, width, height) => {
            const entry = textures.get(texId);
            if (!entry || !entry.loaded) return new Uint8Array(width * height * 4);
            const c = ensureTextureCanvas(entry);
            if (!c || !entry.ctx) return new Uint8Array(width * height * 4);
            const imgData = entry.ctx.getImageData(0, 0, width, height);
            return new Uint8Array(imgData.data.buffer);
        },

        beginFrame: (vw, vh, renderMode) => {
            while (clipDepth > 0) {
                ctx.restore();
                clipDepth--;
            }
            while (matrixDepth > 0) {
                ctx.restore();
                matrixDepth--;
            }
            ctx.setTransform(1, 0, 0, 1, 0, 0);
            ctx.globalAlpha = 1.0;
            ctx.fillStyle = "#000000";
            ctx.fillRect(0, 0, canvas.width, canvas.height);

            applyBaseTransform();
            const smooth = (renderMode === 1);
            ctx.imageSmoothingEnabled = smooth;
            if (smooth) {
                ctx.imageSmoothingQuality = "high";
            }
            if (lastRenderMode !== renderMode) {
                lastRenderMode = renderMode;
                canvas.style.imageRendering = smooth ? "auto" : "pixelated";
            }
        },

        endFrame: () => {
            while (clipDepth > 0) {
                ctx.restore();
                clipDepth--;
            }
            while (matrixDepth > 0) {
                ctx.restore();
                matrixDepth--;
            }
            ctx.globalAlpha = 1.0;
        },

        drawTexture: (texId, dstX, dstY, dstW, dstH, srcX, srcY, srcW, srcH, r, g, b, a) => {
            const entry = textures.get(texId);
            if (!entry || !entry.loaded) return;
            const drawable = getTintedSource(entry, r, g, b);
            if (!drawable) return;

            const tw = drawable.naturalWidth || drawable.width || entry.width;
            const th = drawable.naturalHeight || drawable.height || entry.height;
            const flipX = srcW < 0;
            const flipY = srcH < 0;
            const absSrcW = Math.abs(srcW);
            const absSrcH = Math.abs(srcH);

            const sx = Math.round(srcX * tw);
            const sy = Math.round((1.0 - srcY - absSrcH) * th);
            const sw = Math.round(absSrcW * tw);
            const sh = Math.round(absSrcH * th);
            dstX = Math.round(dstX);
            dstY = Math.round(dstY);
            dstW = Math.round(dstW);
            dstH = Math.round(dstH);
            if (sw <= 0 || sh <= 0 || dstW <= 0 || dstH <= 0) return;

            const prevAlpha = ctx.globalAlpha;
            const prevSmooth = ctx.imageSmoothingEnabled;
            ctx.globalAlpha = a;
            if (entry.isPixelArt && prevSmooth) {
                ctx.imageSmoothingEnabled = false;
            }

            if (flipX || flipY) {
                ctx.save();
                ctx.translate(dstX + (flipX ? dstW : 0), dstY + (flipY ? dstH : 0));
                ctx.scale(flipX ? -1 : 1, flipY ? -1 : 1);
                ctx.drawImage(drawable, sx, sy, sw, sh, 0, 0, dstW, dstH);
                ctx.restore();
            } else {
                ctx.drawImage(drawable, sx, sy, sw, sh, dstX, dstY, dstW, dstH);
            }

            if (entry.isPixelArt && prevSmooth) {
                ctx.imageSmoothingEnabled = true;
            }
            ctx.globalAlpha = prevAlpha;
        },

        drawSolidRect: (dstX, dstY, dstW, dstH, r, g, b, a) => {
            ctx.fillStyle = `rgba(${Math.round(r * 255)},${Math.round(g * 255)},${Math.round(b * 255)},${a})`;
            ctx.fillRect(dstX, dstY, dstW, dstH);
        },

        drawCircle: (cx, cy, radius, r, g, b, a, fill) => {
            ctx.beginPath();
            ctx.arc(cx, cy, radius, 0, Math.PI * 2);
            const style = `rgba(${Math.round(r * 255)},${Math.round(g * 255)},${Math.round(b * 255)},${a})`;
            if (fill) {
                ctx.fillStyle = style;
                ctx.fill();
            } else {
                ctx.strokeStyle = style;
                ctx.lineWidth = 1.5;
                ctx.stroke();
            }
        },

        measureTextWidth: (text, fontSize, fontCode) => {
            measureCtx.font = buildFontSpec(fontSize, fontCode);
            return measureCtx.measureText(text).width;
        },

        drawText: (text, x, y, w, h, fontSize, fontCode, r, g, b, a, align) => {
            ctx.fillStyle = `rgba(${Math.round(r * 255)},${Math.round(g * 255)},${Math.round(b * 255)},${a})`;
            ctx.textBaseline = "top";
            let drawX = x;
            if (align === 1) {
                ctx.textAlign = "center";
                drawX = x + w * 0.5;
            } else if (align === 2) {
                ctx.textAlign = "right";
                drawX = x + w;
            } else {
                ctx.textAlign = "left";
            }

            const drawY = y + 1;
            if (matrixDepth === 0) {
                if (isPortraitRotated) {
                    const scaleX = canvas.height / Math.max(1, virtualW);
                    const scaleY = canvas.width / Math.max(1, virtualH);
                    const physSize = Math.max(8, Math.round(fontSize * scaleY));
                    const physX = Math.round(drawX * scaleX);
                    const physY = Math.round(drawY * scaleY);
                    ctx.setTransform(0, 1, -1, 0, canvas.width, 0);
                    ctx.font = buildFontSpec(physSize, fontCode);
                    ctx.fillText(text, physX, physY);
                } else {
                    const scaleX = canvas.width / Math.max(1, virtualW);
                    const scaleY = canvas.height / Math.max(1, virtualH);
                    const physSize = Math.max(8, Math.round(fontSize * scaleY));
                    const physX = Math.round(drawX * scaleX);
                    const physY = Math.round(drawY * scaleY);
                    ctx.setTransform(1, 0, 0, 1, 0, 0);
                    ctx.font = buildFontSpec(physSize, fontCode);
                    ctx.fillText(text, physX, physY);
                }
                applyBaseTransform();
            } else {
                ctx.font = buildFontSpec(fontSize, fontCode);
                ctx.fillText(text, Math.round(drawX), Math.round(drawY));
            }
        },

        pushClipRect: (x, y, w, h) => {
            ctx.save();
            clipDepth++;
            ctx.beginPath();
            ctx.rect(x, y, Math.max(0, w), Math.max(0, h));
            ctx.clip();
        },

        popClipRect: () => {
            if (clipDepth > 0) {
                ctx.restore();
                clipDepth--;
            }
        },

        rotateAroundPivot: (angleDeg, pivotX, pivotY) => {
            ctx.save();
            matrixDepth++;
            const rad = (angleDeg * Math.PI) / 180.0;
            ctx.translate(pivotX, pivotY);
            ctx.rotate(rad);
            ctx.translate(-pivotX, -pivotY);
        },

        restoreMatrix: () => {
            if (matrixDepth > 0) {
                ctx.restore();
                matrixDepth--;
            }
        },

        resetMatrixStack: () => {
            while (matrixDepth > 0) {
                ctx.restore();
                matrixDepth--;
            }
            if (clipDepth === 0) {
                applyBaseTransform();
            }
        },

        saveStorageFile: (vfsPath, base64Data) => {
            writeToStorage(vfsPath, `B64|${vfsPath}|${base64Data}`);
        },

        saveStorageText: (vfsPath, textContent) => {
            writeToStorage(vfsPath, `TXT|${vfsPath}|${utf8ToBase64(textContent)}`);
        },

        deleteStorageFile: (vfsPath) => {
            removeFromStorage(vfsPath);
        },

        getSharedFilesPayload: () => {
            syncFromLocalStorage(true);
            const lines = [];
            for (const [k, v] of persistedFiles.entries()) {
                if (isSharedVfsPath(k)) lines.push(v);
            }
            return lines.join("\n");
        },

        getAllPersistedFilesPayload: () => {
            syncFromLocalStorage(false);
            const lines = [];
            for (const v of persistedFiles.values()) {
                lines.push(v);
            }
            return lines.join("\n");
        },

        openUrl: (url) => {
            window.open(url, "_blank", "noopener,noreferrer");
        },

        getClipboardText: () => clipboardCache || "",

        setClipboardText: (text) => {
            clipboardCache = text || "";
            if (navigator.clipboard && navigator.clipboard.writeText) {
                navigator.clipboard.writeText(clipboardCache).catch(() => { });
            }
        },

        showSoftKeyboard: (initialText) => {
            if (wasmExports) {
                wasmExports.DragonBoy_Web_Host.WebInputBridge.OnBlurReset();
            }
            kbInput.value = initialText || "";
            kbModal.classList.add("visible");
            try {
                kbInput.focus();
                kbInput.setSelectionRange(kbInput.value.length, kbInput.value.length);
            } catch (_) { }
            setTimeout(() => {
                if (kbModal.classList.contains("visible")) {
                    try {
                        kbInput.focus();
                        kbInput.setSelectionRange(kbInput.value.length, kbInput.value.length);
                    } catch (_) { }
                }
            }, 30);
        },

        hideSoftKeyboard: () => {
            kbModal.classList.remove("visible");
            kbInput.blur();
            canvas.focus();
            setTimeout(updateCanvasViewportSize, 80);
        },

        getRelayWsUrl: (targetHost, targetPort) => {
            const proto = window.location.protocol === "https:" ? "wss:" : "ws:";
            return `${proto}//${window.location.host}/ws/tcp?host=${encodeURIComponent(targetHost)}&port=${targetPort}`;
        },

        getHttpBaseUrl: () => window.location.origin,

        getInitialTabId: () => initialTabId,

        hostSwitchTab: (tabId, keepHubOpen, bx, by) => {
            activateTabInHost(tabId, keepHubOpen, bx, by);
        },

        hostPrewarmTab: (tabId) => {
            ensureChildTabIframe(tabId);
        },

        hostStopTab: (tabId) => {
            if (isEmbeddedTab) {
                window.parent.postMessage({ type: "db_stop_tab", tabId }, "*");
            } else {
                const frame = childTabIframes.get(tabId);
                if (frame && frame.contentWindow) {
                    frame.contentWindow.postMessage({ type: "db_do_stop_self" }, "*");
                }
            }
        },

        hostDeleteTab: (tabId) => {
            if (isEmbeddedTab) {
                window.parent.postMessage({ type: "db_delete_tab", tabId }, "*");
            } else {
                const frame = childTabIframes.get(tabId);
                if (frame) {
                    frame.remove();
                    childTabIframes.delete(tabId);
                }
            }
        },

        setLoadingProgress: (statusText, percent) => {
            if (loadingText && statusText) loadingText.textContent = statusText;
            if (loadingBar && typeof percent === "number") {
                loadingBar.style.width = `${Math.max(5, Math.min(100, percent))}%`;
            }
        },

        hideLoadingOverlay: () => {
            if (loadingOverlay) {
                loadingOverlay.classList.add("hidden");
                setTimeout(() => { loadingOverlay.style.display = "none"; }, 350);
            }
            canvas.focus();
        },

        bootBlazor: async () => {
            await openIndexedDB();
            if (document.fonts && document.fonts.ready) {
                try { await document.fonts.ready; } catch (_) { }
            }
            await Blazor.start();
            const runtime = (window.Blazor && window.Blazor.runtime)
                ? window.Blazor.runtime
                : (typeof window.getDotnetRuntime === "function" ? await window.getDotnetRuntime(0) : null);
            try {
                wasmExports = await runtime.getAssemblyExports("DragonBoy_Web_Client.dll");
            } catch (_) {
                wasmExports = await runtime.getAssemblyExports("DragonBoy_Web_Client");
            }
            if (vfsChannel && wasmExports) {
                try {
                    const sharedInit = persistedFiles.get("/rms/tabs.xml");
                    if (sharedInit) {
                        wasmExports.DragonBoy_Web_Host.Program.OnBroadcastFileUpdated(sharedInit);
                    }
                } catch (_) { }
            }
            setupInputListeners();
            startGameLoops();
            pingRenderHealthKeepAlive(false);
        }
    };
})();

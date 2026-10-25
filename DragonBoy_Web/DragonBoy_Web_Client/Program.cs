using System;
using System.Runtime.InteropServices.JavaScript;
using System.Threading;
using System.Threading.Tasks;
using DragonBoy_Net8_Native.Engine.Platform;
using UnityEngine;

namespace DragonBoy_Web_Host
{
    /// <summary>
    /// Điểm vào chính và bộ điều phối vòng lặp khung hình cho phiên bản trình duyệt (.NET 8 WebAssembly).
    /// </summary>
    public static partial class Program
    {
        private static bool _isInitialized;
        private static double _lastTimestampMs;
        private static double _fixedAccumulatorMs;

        /// <summary>
        /// Khởi tạo VFS từ bộ nhớ trình duyệt, tải gói tài nguyên khởi động và khởi chạy DragonBoy Engine.
        /// </summary>
        public static async System.Threading.Tasks.Task Main(string[] args)
        {
            try
            {
                Thread.CurrentThread.Name = global::Main.mainThreadName;
            }
            catch { }

            try
            {
                int initialTabId = WebInterop.GetInitialTabId();
                if (initialTabId < 1) initialTabId = 1;
                Application.CurrentTabId = initialTabId;

                PlatformService.Current = new WebPlatformBridge();
                DefaultDesktopBridge.HostSwitchToTab = (tabId, keepHubOpen, bx, by) =>
                {
                    WebInterop.HostSwitchTab(tabId, keepHubOpen, bx, by);
                };
                DefaultDesktopBridge.HostPrewarmTab = tabId =>
                {
                    WebInterop.HostPrewarmTab(tabId);
                };
                DefaultDesktopBridge.HostStopTab = tabId =>
                {
                    if (tabId == Application.CurrentTabId)
                    {
                        GameTabManager.StopTab(tabId);
                    }
                    else
                    {
                        WebInterop.HostStopTab(tabId);
                    }
                };
                DefaultDesktopBridge.HostDeleteTab = tabId =>
                {
                    WebInterop.HostDeleteTab(tabId);
                    GameTabRegistry.DeleteTab(tabId);
                };

                TouchScreenKeyboard.OpenHandler = (text, t, b1, b2, type, b3, caption) =>
                {
                    if (WebInterop.IsTouchDevice())
                    {
                        TouchScreenKeyboard.visible = true;
                        WebInterop.ShowSoftKeyboard(text ?? string.Empty);
                    }
                    return new TouchScreenKeyboard
                    {
                        active = true,
                        done = false,
                        text = text ?? string.Empty
                    };
                };
                TouchScreenKeyboard.ClearHandler = () =>
                {
                    TouchScreenKeyboard.visible = false;
                    if (WebInterop.IsTouchDevice())
                    {
                        WebInterop.HideSoftKeyboard();
                    }
                };

                WebInterop.SetLoadingProgress("Đang nạp dữ liệu đã lưu trên trình duyệt...", 10);
                WebStorageSync.HydrateFromBrowserStorage();

                await WebAssetLoader.InitializeStartupPackAsync();

                WebInterop.SetLoadingProgress("Đang khởi tạo DragonBoy Engine...", 85);
                WebGraphicsBackend.UpdateDynamicViewport();

                if (global::Main.main == null)
                {
                    global::Main.main = new global::Main();
                }

                global::Main.main.Start();
                WebGraphicsBackend.UpdateDynamicViewport();

                if (Application.CurrentTabId > 1)
                {
                    WebInterop.SetWindowTitle($"DragonBoy (Tab {Application.CurrentTabId}) - Mod TriHienKun [Web]");
                }
                else
                {
                    WebInterop.SetWindowTitle("DragonBoy - Mod TriHienKun [Web]");
                }

                _isInitialized = true;
                WebInterop.SetLoadingProgress("Hoàn tất!", 100);
                WebInterop.HideLoadingOverlay();
            }
            catch (Exception ex)
            {
                Console.WriteLine("[Program.Main] Fatal Init Error: " + ex);
                WebInterop.SetLoadingProgress("Lỗi khởi tạo: " + ex.Message, 100);
            }

            await System.Threading.Tasks.Task.Delay(Timeout.Infinite);
        }

        /// <summary>
        /// Thực thi một chu kỳ mô phỏng vật lý, cập nhật logic và kết xuất đồ họa được gọi từ requestAnimationFrame hoặc Web Worker.
        /// </summary>
        [JSExport]
        public static void OnBrowserFrame(double timestampMs, int renderEnabled)
        {
            if (!_isInitialized || global::Main.main == null) return;

            try
            {
                if (_lastTimestampMs <= 0)
                {
                    _lastTimestampMs = timestampMs;
                }
                double dtMs = timestampMs - _lastTimestampMs;
                _lastTimestampMs = timestampMs;
                if (dtMs < 0) dtMs = 0;
                if (dtMs > 250) dtMs = 250;

                Time.deltaTime = (float)(dtMs / 1000.0);

                WebGraphicsBackend.UpdateDynamicViewport();
                if (GameMidlet.gameCanvas != null)
                {
                    ModGraphics.UpdateResolutionWatcher();
                }

                EventPump.ProcessInput(global::Main.main);

                _fixedAccumulatorMs += dtMs;
                int maxSteps = 5;
                while (_fixedAccumulatorMs >= 16.0 && maxSteps-- > 0)
                {
                    global::Main.main.FixedUpdate();
                    WebInputBridge.ResetKeyDownFlags();
                    _fixedAccumulatorMs -= 16.0;
                }
                if (_fixedAccumulatorMs > 32.0) _fixedAccumulatorMs = 0;

                global::Main.main.Update();

                if (renderEnabled != 0)
                {
                    WebGraphicsBackend.BeginFrame();
                    Event.current.type = EventType.Repaint;
                    global::Main.main.OnGUI();
                    WebGraphicsBackend.EndFrame();
                }

                WebInputBridge.EndFrameInputReset();
            }
            catch (Exception ex)
            {
                Console.WriteLine("[OnBrowserFrame] Error: " + ex.Message);
            }
        }

        /// <summary>
        /// Trả về mức giới hạn FPS mục tiêu hiện tại để vòng lặp trình duyệt điều tiết nhịp vẽ.
        /// </summary>
        [JSExport]
        public static int GetTargetFrameRate() => WebGraphicsBackend.TargetFrameRate;

        /// <summary>
        /// Thực thi tức thời chu kỳ xử lý đầu vào ngay trong sự kiện User Gesture của trình duyệt (kích hoạt đồng bộ Bàn phím ảo & Fullscreen trên thiết bị di động).
        /// </summary>
        [JSExport]
        public static void OnInputImmediate(double timestampMs)
        {
            if (!_isInitialized || global::Main.main == null) return;
            try
            {
                WebGraphicsBackend.UpdateDynamicViewport();
                if (GameMidlet.gameCanvas != null)
                {
                    ModGraphics.UpdateResolutionWatcher();
                }
                EventPump.ProcessInput(global::Main.main);
                global::Main.main.FixedUpdate();
                WebInputBridge.ResetKeyDownFlags();
                _fixedAccumulatorMs = System.Math.Max(-16.0, _fixedAccumulatorMs - 16.0);
                global::Main.main.Update();
                WebInputBridge.EndFrameInputReset();
            }
            catch { }
        }

        /// <summary>
        /// Đồng bộ trạng thái HUB và các tệp dùng chung khi tab trình duyệt được kích hoạt hiển thị.
        /// </summary>
        [JSExport]
        public static void OnTabActivated(int keepHubOpen, int bx, int by)
        {
            if (!_isInitialized) return;
            try
            {
                WebGraphicsBackend.UpdateDynamicViewport();
                if (GameMidlet.gameCanvas != null)
                {
                    ModGraphics.UpdateResolutionWatcher();
                }
                WebStorageSync.RefreshSharedFilesFromLocalStorage();
                GameTabStatus.RefreshAllTabs(force: true);
                ModBackground.Update();
                if (keepHubOpen != 0)
                {
                    ModFloatingHUD.isShowFloatingHUD = true;
                    ModFloatingHUD.isExpanded = true;
                    if (bx >= 0 && by >= 0)
                    {
                        ModFloatingHUD.bubbleX = bx;
                        ModFloatingHUD.bubbleY = by;
                    }
                }
                else
                {
                    ModFloatingHUD.isExpanded = false;
                }
            }
            catch { }
        }

        /// <summary>
        /// Xử lý yêu cầu dừng hoạt động của tab hiện tại từ cửa sổ quản lý đa tab.
        /// </summary>
        [JSExport]
        public static void OnTabStopRequested()
        {
            if (!_isInitialized) return;
            try
            {
                GameTabManager.StopTab(Application.CurrentTabId);
            }
            catch { }
        }
    }
}

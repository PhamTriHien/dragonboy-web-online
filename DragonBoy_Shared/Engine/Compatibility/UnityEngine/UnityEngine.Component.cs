using System;

namespace UnityEngine
{
    /// <summary>
    /// Thành phần phân mảnh (partial) của lớp Component: Đảm nhiệm phân hệ chuyên trách Component ().
    /// </summary>
    public class Component
    {
        public GameObject gameObject { get; set; }
        public Transform transform { get; set; }
        public string tag { get; set; } = "Untagged";
        public string name { get; set; } = string.Empty;

        public T GetComponent<T>() where T : class
        {
            if (this is T t) return t;
            if (gameObject != null) return gameObject.GetComponent<T>();
            if (typeof(T) == typeof(Camera)) return Camera.main as T;
            return null;
        }
    }

    /// <summary>
    /// Thành phần phân mảnh (partial) của lớp Transform: Đảm nhiệm phân hệ chuyên trách Component ().
    /// </summary>
    public class Transform : Component
    {
        public Vector3 position { get; set; } = Vector3.zero;
        public Vector3 localPosition { get; set; } = Vector3.zero;
        public Vector3 localScale { get; set; } = Vector3.one;
    }

    /// <summary>
    /// Thành phần phân mảnh (partial) của lớp GameObject: Đảm nhiệm phân hệ chuyên trách Component ().
    /// </summary>
    public class GameObject
    {
        public string name { get; set; } = "GameObject";
        public Transform transform { get; set; } = new Transform();
        private System.Collections.Generic.List<Component> _components = new System.Collections.Generic.List<Component>();

        private static GameObject _mainCamera;
        /// <summary>
        /// Thực thi phương thức nghiệp vụ Find.
        /// </summary>
        public static GameObject Find(string name)
        {
            if (name == "Main Camera")
            {
                if (_mainCamera == null)
                {
                    _mainCamera = new GameObject { name = "Main Camera" };
                }
                return _mainCamera;
            }
            return new GameObject { name = name };
        }

        public T AddComponent<T>() where T : Component, new()
        {
            T comp = new T();
            comp.gameObject = this;
            comp.transform = this.transform;
            _components.Add(comp);
            return comp;
        }

        public T GetComponent<T>() where T : class
        {
            if (typeof(T) == typeof(Camera)) return Camera.main as T;
            for (int i = 0; i < _components.Count; i++)
            {
                if (_components[i] is T t) return t;
            }
            if (typeof(T) == typeof(AudioSource))
            {
                return AddComponent<AudioSource>() as T;
            }
            return null;
        }
    }

    /// <summary>
    /// Thành phần phân mảnh (partial) của lớp MonoBehaviour: Đảm nhiệm phân hệ chuyên trách Component ().
    /// </summary>
    public class MonoBehaviour : Component
    {
        public bool enabled { get; set; } = true;
        public bool useGUILayout { get; set; } = false;

        /// <summary>
        /// Thực thi phương thức nghiệp vụ Awake.
        /// </summary>
        public virtual void Awake() { }
        /// <summary>
        /// Bắt đầu khởi chạy tiến trình hoặc luồng xử lý nền.
        /// </summary>
        public virtual void Start() { }
        /// <summary>
        /// Cập nhật trạng thái logic của đối tượng theo từng chu kỳ khung hình.
        /// </summary>
        public virtual void Update() { }
        /// <summary>
        /// Thực thi phương thức nghiệp vụ FixedUpdate.
        /// </summary>
        public virtual void FixedUpdate() { }
        /// <summary>
        /// Bộ xử lý sự kiện tiếp nhận và phản hồi OnGUI.
        /// </summary>
        public virtual void OnGUI() { }
        /// <summary>
        /// Bộ xử lý sự kiện tiếp nhận và phản hồi OnApplicationQuit.
        /// </summary>
        public virtual void OnApplicationQuit() { }
    }

    /// <summary>
    /// Thành phần phân mảnh (partial) của lớp Camera: Đảm nhiệm phân hệ chuyên trách Component ().
    /// </summary>
    public class Camera : Component
    {
        public static Camera main { get; set; } = new Camera();
        public Color backgroundColor { get; set; } = Color.black;
    }

    /// <summary>
    /// Thành phần phân mảnh (partial) của lớp TextAsset: Đảm nhiệm phân hệ chuyên trách Component ().
    /// </summary>
    public class TextAsset
    {
        public byte[] bytes { get; set; }
        public string text => bytes != null ? System.Text.Encoding.UTF8.GetString(bytes) : string.Empty;

        /// <summary>
        /// Thực thi phương thức nghiệp vụ TextAsset.
        /// </summary>
        public TextAsset(byte[] data)
        {
            bytes = data;
        }
    }

    /// <summary>
    /// Thành phần phân mảnh (partial) của lớp Resources: Đảm nhiệm phân hệ chuyên trách Component ().
    /// </summary>
    public static class Resources
    {
        private const string SharedTexSlot = "DragonBoy.SharedFileTextures.v2";
        private const string SharedTextAssetSlot = "DragonBoy.SharedTextAssets.v1";

        private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, Texture2D> _tabTextureCache =
            new System.Collections.Concurrent.ConcurrentDictionary<string, Texture2D>(StringComparer.OrdinalIgnoreCase);

        private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, TextAsset> _tabTextAssetCache =
            new System.Collections.Concurrent.ConcurrentDictionary<string, TextAsset>(StringComparer.OrdinalIgnoreCase);

        private static System.Collections.Concurrent.ConcurrentDictionary<string, (Raylib_cs.Texture2D tex, int w, int h, string path, bool isFallbackFromX1)> GetSharedTexturePool()
        {
            if (AppDomain.CurrentDomain.GetData(SharedTexSlot) is System.Collections.Concurrent.ConcurrentDictionary<string, (Raylib_cs.Texture2D tex, int w, int h, string path, bool isFallbackFromX1)> dict)
            {
                return dict;
            }
            lock (typeof(AppDomain))
            {
                if (AppDomain.CurrentDomain.GetData(SharedTexSlot) is System.Collections.Concurrent.ConcurrentDictionary<string, (Raylib_cs.Texture2D tex, int w, int h, string path, bool isFallbackFromX1)> existing)
                {
                    return existing;
                }
                var created = new System.Collections.Concurrent.ConcurrentDictionary<string, (Raylib_cs.Texture2D tex, int w, int h, string path, bool isFallbackFromX1)>(StringComparer.OrdinalIgnoreCase);
                AppDomain.CurrentDomain.SetData(SharedTexSlot, created);
                return created;
            }
        }

        private static System.Collections.Concurrent.ConcurrentDictionary<string, byte[]> GetSharedTextAssetPool()
        {
            if (AppDomain.CurrentDomain.GetData(SharedTextAssetSlot) is System.Collections.Concurrent.ConcurrentDictionary<string, byte[]> dict)
            {
                return dict;
            }
            lock (typeof(AppDomain))
            {
                if (AppDomain.CurrentDomain.GetData(SharedTextAssetSlot) is System.Collections.Concurrent.ConcurrentDictionary<string, byte[]> existing)
                {
                    return existing;
                }
                var created = new System.Collections.Concurrent.ConcurrentDictionary<string, byte[]>(StringComparer.OrdinalIgnoreCase);
                AppDomain.CurrentDomain.SetData(SharedTextAssetSlot, created);
                return created;
            }
        }

        public static object Load(string path) => Load(path, typeof(object));

        public static T Load<T>(string path) where T : class
        {
            return Load(path, typeof(T)) as T;
        }

        /// <summary>
        /// Khởi tạo và nạp dữ liệu cấu hình cho Load.
        /// </summary>
        public static object Load(string path, Type type)
        {
            if (string.IsNullOrEmpty(path)) return null;

            string cleanPath = path.Replace('\\', '/').TrimStart('/');
            string withoutRes = cleanPath;
            if (withoutRes.StartsWith("res/")) withoutRes = withoutRes.Substring(4);

#if BROWSER
            if (OperatingSystem.IsBrowser())
            {
                return DragonBoy_Web_Host.WebAssetLoader.Load(cleanPath, withoutRes, type);
            }
#endif

#if ANDROID || __ANDROID__
            if (OperatingSystem.IsAndroid())
            {
                var androidObj = DragonBoy_Android_Host.AndroidAssetLoader.Load(cleanPath, withoutRes, type);
                if (androidObj != null) return androidObj;
            }
#endif

            if (type == typeof(Font) ||
                cleanPath.StartsWith("FontSys/", StringComparison.OrdinalIgnoreCase) ||
                cleanPath.EndsWith(".ttf", StringComparison.OrdinalIgnoreCase) ||
                cleanPath.EndsWith(".otf", StringComparison.OrdinalIgnoreCase))
            {
                return new Font { name = System.IO.Path.GetFileNameWithoutExtension(cleanPath) };
            }

            string withoutZoom = withoutRes;
            if (withoutZoom.StartsWith("x1/") || withoutZoom.StartsWith("x2/") ||
                withoutZoom.StartsWith("x3/") || withoutZoom.StartsWith("x4/"))
            {
                withoutZoom = withoutZoom.Substring(3);
            }
            bool isJavaMode = ModGraphics.IsJavaGraphics || withoutZoom.StartsWith("myfont/", StringComparison.OrdinalIgnoreCase);

            if (type == typeof(TextAsset))
            {
                if (_tabTextAssetCache.TryGetValue(cleanPath, out var cachedTa))
                {
                    return cachedTa;
                }

                var sharedBytesPool = GetSharedTextAssetPool();
                if (sharedBytesPool.TryGetValue(cleanPath, out var sharedBytes))
                {
                    var ta = new TextAsset(sharedBytes);
                    _tabTextAssetCache[cleanPath] = ta;
                    return ta;
                }
            }
            else
            {
                string cacheKey = cleanPath + (isJavaMode ? "@java1" : ("@" + mGraphics.zoomLevel));
                if (_tabTextureCache.TryGetValue(cacheKey, out var cachedTex) && cachedTex != null)
                {
                    return cachedTex;
                }

                var sharedTexPool = GetSharedTexturePool();
                if (sharedTexPool.TryGetValue(cacheKey, out var sharedEntry) && sharedEntry.tex.Id != 0)
                {
                    var t2d = new Texture2D(sharedEntry.w, sharedEntry.h, allocatePixelBuffer: false)
                    {
                        RaylibTexture = sharedEntry.tex,
                        HasRaylibTexture = true,
                        IsSharedGpuTexture = true,
                        IsFallbackFromX1 = sharedEntry.isFallbackFromX1,
                        _sourceFilePath = sharedEntry.path
                    };
                    _tabTextureCache[cacheKey] = t2d;
                    return t2d;
                }
            }

            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string[] searchRoots = new string[]
            {
                System.IO.Path.Combine(baseDir, "Assets"),
                System.IO.Path.Combine(baseDir, "Assets", "x2"),
                System.IO.Path.Combine(baseDir, "Assets", "data"),
                System.IO.Path.Combine(baseDir, "Assets", "x4"),
                System.IO.Path.Combine(baseDir, "Assets", "x3"),
                System.IO.Path.Combine(baseDir, "Assets", "x1"),
                baseDir,
                System.IO.Path.Combine(baseDir, "res"),
                @"C:\ModNRO\DragonBoy_Shared\Assets",
                @"C:\ModNRO\DragonBoy_Shared\Assets\x2",
                @"C:\ModNRO\DragonBoy_Shared\Assets\data",
                @"C:\ModNRO\DragonBoy_Shared\Assets\x4",
                @"C:\ModNRO\DragonBoy_Shared\Assets\x3",
                @"C:\ModNRO\DragonBoy_Shared\Assets\x1",
                @"C:\ModNRO\DragonBoy_Net8_Native\Assets",
                @"C:\ModNRO\DragonBoy_Net8_Native\Assets\x2",
                @"C:\ModNRO\DragonBoy_Net8_Native\Assets\data",
                @"C:\ModNRO\DragonBoy_Net8_Native\Assets\x4",
                @"C:\ModNRO\DragonBoy_Net8_Native\Assets\x3",
                @"C:\ModNRO\DragonBoy_Net8_Native\Assets\x1",
                @"C:\ModNRO\ModNRO_Tools\Decompiled\APK_apktool\assets",
                @"C:\ModNRO\ModNRO_Tools\Decompiled\APK_apktool\assets\x2",
                @"C:\ModNRO\DragonBoy250_Assets\textures",
                @"C:\ModNRO\DragonBoy250_Assets\textassets",
                @"C:\ModNRO\DragonBoy250_Assets"
            };

            string[] subPaths = isJavaMode
                ? new string[]
                {
                    "x1/" + withoutZoom,
                    "x1/" + withoutRes,
                    "x1/" + cleanPath,
                    cleanPath,
                    withoutRes,
                    "x2/" + withoutZoom,
                    "x2/" + withoutRes,
                    "x2/" + cleanPath,
                    withoutZoom,
                    "data/" + cleanPath,
                    "data/" + withoutRes,
                    "data/" + withoutZoom,
                    "x3/" + withoutZoom,
                    "x4/" + withoutZoom,
                    "x3/" + withoutRes,
                    "x4/" + withoutRes,
                    "x3/" + cleanPath,
                    "x4/" + cleanPath
                }
                : new string[]
                {
                    "x2/" + withoutZoom,
                    cleanPath,
                    withoutRes,
                    "x2/" + withoutRes,
                    "x2/" + cleanPath,
                    withoutZoom,
                    "data/" + cleanPath,
                    "data/" + withoutRes,
                    "data/" + withoutZoom,
                    "x4/" + withoutRes,
                    "x3/" + withoutRes,
                    "x1/" + withoutRes,
                    "x4/" + cleanPath,
                    "x3/" + cleanPath,
                    "x1/" + cleanPath,
                    "x4/" + withoutZoom,
                    "x3/" + withoutZoom,
                    "x1/" + withoutZoom
                };

            if (type == typeof(TextAsset))
            {
                foreach (var root in searchRoots)
                {
                    if (!System.IO.Directory.Exists(root)) continue;
                    foreach (var sub in subPaths)
                    {
                        string full = System.IO.Path.Combine(root, sub).Replace('\\', '/');
                        string matchedPath = null;
                        if (System.IO.File.Exists(full)) matchedPath = full;
                        else if (System.IO.File.Exists(full + ".bytes")) matchedPath = full + ".bytes";
                        else if (System.IO.File.Exists(full + ".txt")) matchedPath = full + ".txt";

                        if (matchedPath != null)
                        {
                            byte[] bytes = System.IO.File.ReadAllBytes(matchedPath);
                            GetSharedTextAssetPool()[cleanPath] = bytes;
                            var ta = new TextAsset(bytes);
                            _tabTextAssetCache[cleanPath] = ta;
                            return ta;
                        }
                    }
                }
                return null;
            }
            else
            {
                string cacheKey = cleanPath + (isJavaMode ? "@java1" : ("@" + mGraphics.zoomLevel));
                foreach (var root in searchRoots)
                {
                    if (!System.IO.Directory.Exists(root)) continue;
                    foreach (var sub in subPaths)
                    {
                        string full = System.IO.Path.Combine(root, sub).Replace('\\', '/');
                        string matchedPath = null;
                        if (System.IO.File.Exists(full + ".png")) matchedPath = full + ".png";
                        else if (System.IO.File.Exists(full)) matchedPath = full;
                        else if (System.IO.File.Exists(full + ".jpg")) matchedPath = full + ".jpg";

                        if (matchedPath != null)
                        {
                            var t = LoadTextureFromFile(matchedPath, isJavaMode);
                            if (t != null)
                            {
                                _tabTextureCache[cacheKey] = t;
                                if (t.HasRaylibTexture && t.RaylibTexture.Id != 0)
                                {
                                    GetSharedTexturePool()[cacheKey] = (t.RaylibTexture, t.width, t.height, matchedPath, t.IsFallbackFromX1);
                                }
                                return t;
                            }
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Khởi tạo và nạp dữ liệu cấu hình cho LoadTextureFromFile.
        /// </summary>
        private static Texture2D LoadTextureFromFile(string fullPath, bool isJavaMode)
        {
            try
            {
                Raylib_cs.Image img = Raylib_cs.Raylib.LoadImage(fullPath);
                if (img.Width > 0 && img.Height > 0)
                {
                    string norm = fullPath.Replace('\\', '/');
                    bool isFontAsset = norm.IndexOf("/myfont/", StringComparison.OrdinalIgnoreCase) >= 0;
                    bool isFallbackFromX1 = false;
                    if (mGraphics.zoomLevel == 2)
                    {
                        if (norm.Contains("/x1/") || norm.StartsWith("x1/"))
                        {
                            if (isJavaMode || isFontAsset)
                            {
                                Raylib_cs.Raylib.ImageResizeNN(ref img, img.Width * 2, img.Height * 2);
                            }
                            else
                            {
                                UpscaleSmart2x(ref img);
                                isFallbackFromX1 = true;
                            }
                        }
                        else if (norm.Contains("/x3/") || norm.StartsWith("x3/"))
                        {
                            int tw = (img.Width * 2 + 1) / 3;
                            int th = (img.Height * 2 + 1) / 3;
                            if (tw > 0 && th > 0)
                            {
                                Raylib_cs.Raylib.ImageResizeNN(ref img, tw, th);
                            }
                            if (isJavaMode && !isFontAsset)
                            {
                                Texture2D.QuantizeRaylibImageJava2x2(ref img);
                            }
                        }
                        else if (norm.Contains("/x4/") || norm.StartsWith("x4/"))
                        {
                            int tw = (img.Width + 1) / 2;
                            int th = (img.Height + 1) / 2;
                            if (tw > 0 && th > 0)
                            {
                                Raylib_cs.Raylib.ImageResizeNN(ref img, tw, th);
                            }
                            if (isJavaMode && !isFontAsset)
                            {
                                Texture2D.QuantizeRaylibImageJava2x2(ref img);
                            }
                        }
                        else if (isJavaMode && !isFontAsset)
                        {
                            Texture2D.QuantizeRaylibImageJava2x2(ref img);
                        }
                    }
                    Texture2D t2d = new Texture2D(img.Width, img.Height, allocatePixelBuffer: false);
                    t2d.RaylibTexture = Raylib_cs.Raylib.LoadTextureFromImage(img);
                    Raylib_cs.Raylib.SetTextureWrap(t2d.RaylibTexture, Raylib_cs.TextureWrap.Clamp);
                    Raylib_cs.TextureFilter desiredFilter = (ModGraphics.renderMode == 0 || isFontAsset) ? Raylib_cs.TextureFilter.Point : Raylib_cs.TextureFilter.Bilinear;
                    Raylib_cs.Raylib.SetTextureFilter(t2d.RaylibTexture, desiredFilter);
                    t2d.currentRaylibFilter = desiredFilter;
                    t2d.HasRaylibTexture = true;
                    t2d.IsSharedGpuTexture = true;
                    t2d.IsFallbackFromX1 = isFallbackFromX1;
                    t2d._sourceFilePath = fullPath;
                    Raylib_cs.Raylib.UnloadImage(img);
                    return t2d;
                }
            }
            catch
            {
            }
            return null;
        }

        /// <summary>
        /// Thực thi phương thức nghiệp vụ UpscaleSmart2x.
        /// </summary>
        private static unsafe void UpscaleSmart2x(ref Raylib_cs.Image img)
        {
            if (img.Width <= 0 || img.Height <= 0) return;
            if (img.Width > 512 || img.Height > 512)
            {
                Raylib_cs.Raylib.ImageResizeNN(ref img, img.Width * 2, img.Height * 2);
                return;
            }

            if (img.Format != Raylib_cs.PixelFormat.UncompressedR8G8B8A8)
            {
                Raylib_cs.Raylib.ImageFormat(ref img, Raylib_cs.PixelFormat.UncompressedR8G8B8A8);
            }

            int w = img.Width;
            int h = img.Height;
            Raylib_cs.Color* src = (Raylib_cs.Color*)img.Data;
            if (src == null)
            {
                Raylib_cs.Raylib.ImageResizeNN(ref img, w * 2, h * 2);
                return;
            }

            Raylib_cs.Image dst = Raylib_cs.Raylib.GenImageColor(w * 2, h * 2, Raylib_cs.Color.Blank);
            Raylib_cs.Color* dstPixels = (Raylib_cs.Color*)dst.Data;
            if (dstPixels == null)
            {
                Raylib_cs.Raylib.UnloadImage(dst);
                Raylib_cs.Raylib.ImageResizeNN(ref img, w * 2, h * 2);
                return;
            }

            /// <summary>
            /// Truy xuất và trả về dữ liệu của GetPix.
            /// </summary>
            Raylib_cs.Color GetPix(int px, int py)
            {
                int cx = px < 0 ? 0 : (px >= w ? w - 1 : px);
                int cy = py < 0 ? 0 : (py >= h ? h - 1 : py);
                return src[cy * w + cx];
            }

            /// <summary>
            /// Thực thi phương thức nghiệp vụ Df.
            /// </summary>
            float Df(Raylib_cs.Color c1, Raylib_cs.Color c2)
            {
                if (c1.A == 0 && c2.A == 0) return 0f;
                if (c1.A == 0 || c2.A == 0) return 1000f;
                float y1 = 0.2126f * c1.R + 0.7152f * c1.G + 0.0722f * c1.B;
                float y2 = 0.2126f * c2.R + 0.7152f * c2.G + 0.0722f * c2.B;
                return MathF.Abs(y1 - y2) + MathF.Abs(c1.A - c2.A) * 2f;
            }

            /// <summary>
            /// Thực thi phương thức nghiệp vụ Wd.
            /// </summary>
            float Wd(Raylib_cs.Color a, Raylib_cs.Color b, Raylib_cs.Color c, Raylib_cs.Color d, Raylib_cs.Color e, Raylib_cs.Color f, Raylib_cs.Color g, Raylib_cs.Color h)
            {
                return Df(a, b) + Df(a, c) + Df(d, e) + Df(d, f) + 4.0f * Df(g, h);
            }

            byte ClampByte(int v) => (byte)(v < 0 ? 0 : (v > 255 ? 255 : v));

            /// <summary>
            /// Thực thi phương thức nghiệp vụ Blend.
            /// </summary>
            Raylib_cs.Color Blend(Raylib_cs.Color c1, Raylib_cs.Color c2, float t)
            {
                return new Raylib_cs.Color(
                    ClampByte((int)(c1.R * (1f - t) + c2.R * t + 0.5f)),
                    ClampByte((int)(c1.G * (1f - t) + c2.G * t + 0.5f)),
                    ClampByte((int)(c1.B * (1f - t) + c2.B * t + 0.5f)),
                    ClampByte((int)(c1.A * (1f - t) + c2.A * t + 0.5f))
                );
            }

            int outW = w * 2;
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    Raylib_cs.Color E = GetPix(x, y);
                    Raylib_cs.Color B = GetPix(x, y - 1);
                    Raylib_cs.Color D = GetPix(x - 1, y);
                    Raylib_cs.Color F = GetPix(x + 1, y);
                    Raylib_cs.Color H = GetPix(x, y + 1);

                    Raylib_cs.Color C = GetPix(x + 1, y - 1);
                    Raylib_cs.Color I = GetPix(x + 1, y + 1);
                    Raylib_cs.Color G = GetPix(x - 1, y + 1);
                    Raylib_cs.Color A = GetPix(x - 1, y - 1);

                    Raylib_cs.Color e0 = E, e1 = E, e2 = E, e3 = E;

                    Raylib_cs.Color F4 = GetPix(x + 2, y);
                    Raylib_cs.Color H5 = GetPix(x, y + 2);
                    Raylib_cs.Color I4 = GetPix(x + 2, y + 1);
                    Raylib_cs.Color I5 = GetPix(x + 1, y + 2);
                    float wd1_br = Wd(G, H, H, I5, C, F, F, I4);
                    float wd2_br = Wd(D, E, E, H5, B, E, E, F4) + 4f * Df(E, I) - Df(G, H);
                    if (wd1_br < wd2_br && (Df(E, F) > 10f || Df(E, H) > 10f))
                    {
                        Raylib_cs.Color blend = Df(E, F) < Df(E, H) ? F : H;
                        e3 = Blend(E, blend, 0.5f);
                    }

                    Raylib_cs.Color G5 = GetPix(x - 1, y + 2);
                    Raylib_cs.Color D4 = GetPix(x - 2, y);
                    Raylib_cs.Color G4 = GetPix(x - 2, y + 1);
                    float wd1_bl = Wd(I, H, H, G5, A, D, D, G4);
                    float wd2_bl = Wd(F, E, E, H5, B, E, E, D4) + 4f * Df(E, G) - Df(I, H);
                    if (wd1_bl < wd2_bl && (Df(E, D) > 10f || Df(E, H) > 10f))
                    {
                        Raylib_cs.Color blend = Df(E, D) < Df(E, H) ? D : H;
                        e2 = Blend(E, blend, 0.5f);
                    }

                    Raylib_cs.Color C1 = GetPix(x + 1, y - 2);
                    Raylib_cs.Color B1 = GetPix(x, y - 2);
                    float wd1_tr = Wd(A, B, B, C1, I, F, F, I4);
                    float wd2_tr = Wd(D, E, E, B1, H, E, E, F4) + 4f * Df(E, C) - Df(A, B);
                    if (wd1_tr < wd2_tr && (Df(E, F) > 10f || Df(E, B) > 10f))
                    {
                        Raylib_cs.Color blend = Df(E, F) < Df(E, B) ? F : B;
                        e1 = Blend(E, blend, 0.5f);
                    }

                    Raylib_cs.Color A1 = GetPix(x - 1, y - 2);
                    float wd1_tl = Wd(C, B, B, A1, G, D, D, G4);
                    float wd2_tl = Wd(F, E, E, B1, H, E, E, D4) + 4f * Df(E, A) - Df(C, B);
                    if (wd1_tl < wd2_tl && (Df(E, D) > 10f || Df(E, B) > 10f))
                    {
                        Raylib_cs.Color blend = Df(E, D) < Df(E, B) ? D : B;
                        e0 = Blend(E, blend, 0.5f);
                    }

                    dstPixels[(y * 2) * outW + (x * 2)] = e0;
                    dstPixels[(y * 2) * outW + (x * 2 + 1)] = e1;
                    dstPixels[(y * 2 + 1) * outW + (x * 2)] = e2;
                    dstPixels[(y * 2 + 1) * outW + (x * 2 + 1)] = e3;
                }
            }

            Raylib_cs.Raylib.UnloadImage(img);
            img = dst;
        }

        /// <summary>
        /// Thực thi phương thức nghiệp vụ UnloadUnusedAssets.
        /// </summary>
        public static void UnloadUnusedAssets() { }
    }
}

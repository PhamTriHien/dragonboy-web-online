using System;
using Raylib_cs;

namespace UnityEngine
{
    /// <summary>
    /// =========================================================================================================
    /// TẦNG TƯƠNG THÍCH ĐỒ HỌA UNITY ENGINE TRÊN .NET 8 (UNITY GRAPHICS COMPATIBILITY LAYER)
    /// =========================================================================================================
    /// Tái hiện toàn bộ API đồ họa của Unity (Texture2D, Graphics, Color, Matrix4x4, Rect, Screen, Vector2/3)
    /// bằng các cấu trúc native hiệu năng cao, ánh xạ trực tiếp sang Raylib GPU rendering mà không cần Unity runtime:
    /// - Texture2D / Image: Tải và nạp PNG/JPG từ byte[] vào Raylib GPU Texture2D với bộ đệm bộ nhớ an toàn.
    /// - Graphics.DrawTexture: Render texture với clipping rect, matrix transform, color tint và alpha blending.
    /// - Screen / QualitySettings: Đồng bộ kích thước cửa sổ và quản lý tốc độ khung hình.
    /// =========================================================================================================
    /// </summary>
    public enum AnisotropicFiltering
    {
        Disable = 0,
        Enable = 1,
        ForceEnable = 2
    }

    /// <summary>
    /// Thành phần phân mảnh (partial) của lớp FilterMode: Đảm nhiệm phân hệ chuyên trách Graphics ().
    /// </summary>
    public enum FilterMode
    {
        Point = 0,
        Bilinear = 1,
        Trilinear = 2
    }

    /// <summary>
    /// Thành phần phân mảnh (partial) của lớp TextureWrapMode: Đảm nhiệm phân hệ chuyên trách Graphics ().
    /// </summary>
    public enum TextureWrapMode
    {
        Repeat = 0,
        Clamp = 1
    }

    [Flags]
    /// <summary>
    /// Thành phần phân mảnh (partial) của lớp HideFlags: Đảm nhiệm phân hệ chuyên trách Graphics ().
    /// </summary>
    public enum HideFlags
    {
        None = 0,
        HideInHierarchy = 1,
        HideInInspector = 2,
        DontSave = 4,
        NotEditable = 8,
        HideAndDontSave = 13
    }

    /// <summary>
    /// Thành phần phân mảnh (partial) của lớp ScreenOrientation: Đảm nhiệm phân hệ chuyên trách Graphics ().
    /// </summary>
    public enum ScreenOrientation
    {
        Unknown = 0,
        Portrait = 1,
        PortraitUpsideDown = 2,
        LandscapeLeft = 3,
        LandscapeRight = 4,
        AutoRotation = 5
    }

    /// <summary>
    /// Thành phần phân mảnh (partial) của lớp Resolution: Đảm nhiệm phân hệ chuyên trách Graphics ().
    /// </summary>
    public struct Resolution
    {
        public int width;
        public int height;
        public int refreshRate;
    }

    /// <summary>
    /// Thành phần phân mảnh (partial) của lớp Color: Đảm nhiệm phân hệ chuyên trách Graphics ().
    /// </summary>
    public struct Color
    {
        public float r, g, b, a;

        /// <summary>
        /// Thực thi phương thức nghiệp vụ Color.
        /// </summary>
        public Color(float r, float g, float b, float a = 1f)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }

        public static Color white => new Color(1f, 1f, 1f, 1f);
        public static Color black => new Color(0f, 0f, 0f, 1f);
        public static Color red => new Color(1f, 0f, 0f, 1f);
        public static Color green => new Color(0f, 1f, 0f, 1f);
        public static Color blue => new Color(0f, 0f, 1f, 1f);
        public static Color yellow => new Color(1f, 0.92f, 0.016f, 1f);
        public static Color clear => new Color(0f, 0f, 0f, 0f);
        public static Color gray => new Color(0.5f, 0.5f, 0.5f, 1f);

        public static bool operator ==(Color a, Color b) => a.r == b.r && a.g == b.g && a.b == b.b && a.a == b.a;
        public static bool operator !=(Color a, Color b) => !(a == b);
        public override bool Equals(object obj) => obj is Color c && this == c;
        public override int GetHashCode() => HashCode.Combine(r, g, b, a);

        public static implicit operator Raylib_cs.Color(Color c)
        {
            byte r = (byte)(System.Math.Clamp(c.r, 0f, 1f) * 255);
            byte g = (byte)(System.Math.Clamp(c.g, 0f, 1f) * 255);
            byte b = (byte)(System.Math.Clamp(c.b, 0f, 1f) * 255);
            byte a = (byte)(System.Math.Clamp(c.a, 0f, 1f) * 255);
            return new Raylib_cs.Color(r, g, b, a);
        }

        /// <summary>
        /// Thực thi phương thức nghiệp vụ Color.
        /// </summary>
        public static implicit operator Color(Raylib_cs.Color rc)
        {
            return new Color(rc.R / 255f, rc.G / 255f, rc.B / 255f, rc.A / 255f);
        }
    }

    /// <summary>
    /// Thành phần phân mảnh (partial) của lớp Color32: Đảm nhiệm phân hệ chuyên trách Graphics ().
    /// </summary>
    public struct Color32
    {
        public byte r, g, b, a;

        /// <summary>
        /// Thực thi phương thức nghiệp vụ Color32.
        /// </summary>
        public Color32(byte r, byte g, byte b, byte a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }

        public static implicit operator Color(Color32 c) => new Color(c.r / 255f, c.g / 255f, c.b / 255f, c.a / 255f);
        public static implicit operator Color32(Color c) => new Color32((byte)(c.r * 255), (byte)(c.g * 255), (byte)(c.b * 255), (byte)(c.a * 255));
    }

    /// <summary>
    /// Thành phần phân mảnh (partial) của lớp Texture: Đảm nhiệm phân hệ chuyên trách Graphics ().
    /// </summary>
    public class Texture
    {
        public int width { get; set; }
        public int height { get; set; }
        public FilterMode filterMode { get; set; } = FilterMode.Point;
        public int anisoLevel { get; set; } = 0;
        public float mipMapBias { get; set; } = 0f;
        public TextureWrapMode wrapMode { get; set; } = TextureWrapMode.Clamp;
    }

    /// <summary>
    /// Thành phần phân mảnh (partial) của lớp Texture2D: Đảm nhiệm phân hệ chuyên trách Graphics ().
    /// </summary>
    public class Texture2D : Texture
    {
        public Raylib_cs.Texture2D RaylibTexture;
        public Raylib_cs.Image RaylibImage;
        public bool HasRaylibTexture = false;
        public bool IsSharedGpuTexture = false;
        public bool IsFallbackFromX1 = false;
        public Color[] pixelBuffer;
        public TextureFilter currentRaylibFilter = TextureFilter.Point;
        internal byte[] _compressedData;
        internal string _sourceFilePath;
        private ulong _dataHash;

#if ANDROID || __ANDROID__
        public Android.Graphics.Bitmap AndroidBitmap;
#endif
#if BROWSER
        public int WebTextureId;
#endif

        private const string SharedByteTexSlot = "DragonBoy.SharedByteTextures.v1";

        private static System.Collections.Concurrent.ConcurrentDictionary<(ulong, int, int), (Raylib_cs.Texture2D tex, int w, int h)> GetSharedByteTexturePool()
        {
            if (AppDomain.CurrentDomain.GetData(SharedByteTexSlot) is System.Collections.Concurrent.ConcurrentDictionary<(ulong, int, int), (Raylib_cs.Texture2D tex, int w, int h)> dict)
            {
                return dict;
            }
            lock (typeof(AppDomain))
            {
                if (AppDomain.CurrentDomain.GetData(SharedByteTexSlot) is System.Collections.Concurrent.ConcurrentDictionary<(ulong, int, int), (Raylib_cs.Texture2D tex, int w, int h)> existing)
                {
                    return existing;
                }
                var created = new System.Collections.Concurrent.ConcurrentDictionary<(ulong, int, int), (Raylib_cs.Texture2D tex, int w, int h)>();
                AppDomain.CurrentDomain.SetData(SharedByteTexSlot, created);
                return created;
            }
        }

        private static ulong ComputeHash(byte[] data)
        {
            ulong hash = 14695981039346656037UL;
            for (int i = 0; i < data.Length; i++)
            {
                hash ^= data[i];
                hash *= 1099511628211UL;
            }
            return hash;
        }

        private static Texture2D _whiteTexture;
        public static Texture2D whiteTexture
        {
            get
            {
                if (_whiteTexture == null)
                {
                    _whiteTexture = new Texture2D(1, 1);
                    _whiteTexture.SetPixel(0, 0, Color.white);
                    _whiteTexture.Apply();
                }
                return _whiteTexture;
            }
        }

        /// <summary>
        /// Thực thi phương thức nghiệp vụ Texture2D.
        /// </summary>
        public Texture2D(int width, int height) : this(width, height, allocatePixelBuffer: true)
        {
        }

        public Texture2D(int width, int height, bool allocatePixelBuffer)
        {
            this.width = width;
            this.height = height;
            if (allocatePixelBuffer && width > 0 && height > 0)
            {
                pixelBuffer = new Color[width * height];
            }
        }

        /// <summary>
        /// Thực thi phương thức nghiệp vụ Texture2D.
        /// </summary>
        public Texture2D(int width, int height, TextureFormat format = TextureFormat.RGBA32, bool mipmap = false) : this(width, height, allocatePixelBuffer: true)
        {
        }

        private void EnsurePixelBufferLoaded()
        {
            if (pixelBuffer != null && pixelBuffer.Length == width * height)
            {
                return;
            }
            if (width <= 0 || height <= 0)
            {
                pixelBuffer = Array.Empty<Color>();
                return;
            }
#if BROWSER
            if (OperatingSystem.IsBrowser())
            {
                DragonBoy_Web_Host.WebGraphicsBackend.EnsurePixelBufferLoaded(this);
                return;
            }
#endif
            if (!OperatingSystem.IsAndroid() && !OperatingSystem.IsIOS() && !OperatingSystem.IsBrowser())
            {
                try
                {
                    Raylib_cs.Image img = default;
                    bool shouldUnload = false;
                    if (HasRaylibTexture && RaylibTexture.Id != 0)
                    {
                        img = Raylib.LoadImageFromTexture(RaylibTexture);
                        shouldUnload = true;
                    }
                    else if (_compressedData != null && _compressedData.Length > 0)
                    {
                        img = Raylib.LoadImageFromMemory(".png", _compressedData);
                        shouldUnload = true;
                    }
                    else if (!string.IsNullOrEmpty(_sourceFilePath) && System.IO.File.Exists(_sourceFilePath))
                    {
                        img = Raylib.LoadImage(_sourceFilePath);
                        shouldUnload = true;
                    }

                    if (img.Width > 0 && img.Height > 0)
                    {
                        if (img.Width != width || img.Height != height)
                        {
                            Raylib.ImageResizeNN(ref img, width, height);
                        }
                        if (img.Format != PixelFormat.UncompressedR8G8B8A8)
                        {
                            Raylib.ImageFormat(ref img, PixelFormat.UncompressedR8G8B8A8);
                        }
                        unsafe
                        {
                            Raylib_cs.Color* pColors = Raylib.LoadImageColors(img);
                            if (pColors != null)
                            {
                                pixelBuffer = new Color[width * height];
                                for (int i = 0; i < pixelBuffer.Length; i++)
                                {
                                    pixelBuffer[i] = pColors[i];
                                }
                                Raylib.UnloadImageColors(pColors);
                            }
                        }
                        if (shouldUnload)
                        {
                            Raylib.UnloadImage(img);
                        }
                    }
                }
                catch { }
            }
            if (pixelBuffer == null || pixelBuffer.Length != width * height)
            {
                pixelBuffer = new Color[width * height];
            }
        }

        /// <summary>
        /// Thiết lập giá trị hoặc trạng thái cho SetPixel.
        /// </summary>
        public void SetPixel(int x, int y, Color color)
        {
            if (x >= 0 && x < width && y >= 0 && y < height)
            {
                if (pixelBuffer == null || pixelBuffer.Length != width * height)
                {
                    pixelBuffer = new Color[width * height];
                }
                pixelBuffer[y * width + x] = color;
            }
        }

        /// <summary>
        /// Truy xuất và trả về dữ liệu của GetPixel.
        /// </summary>
        public Color GetPixel(int x, int y)
        {
            if (x >= 0 && x < width && y >= 0 && y < height)
            {
#if ANDROID || __ANDROID__
                if (AndroidBitmap != null && !AndroidBitmap.IsRecycled)
                {
                    int colorInt = AndroidBitmap.GetPixel(x, y);
                    float a = ((colorInt >> 24) & 0xFF) / 255f;
                    float r = ((colorInt >> 16) & 0xFF) / 255f;
                    float g = ((colorInt >> 8) & 0xFF) / 255f;
                    float b = (colorInt & 0xFF) / 255f;
                    return new Color(r, g, b, a);
                }
#endif
                EnsurePixelBufferLoaded();
                if (pixelBuffer != null && pixelBuffer.Length == width * height)
                {
                    return pixelBuffer[y * width + x];
                }
            }
            return Color.clear;
        }

        /// <summary>
        /// Truy xuất và trả về dữ liệu của GetPixels.
        /// </summary>
        public Color[] GetPixels()
        {
#if ANDROID || __ANDROID__
            if (AndroidBitmap != null && !AndroidBitmap.IsRecycled && width > 0 && height > 0)
            {
                int[] pixels = new int[width * height];
                AndroidBitmap.GetPixels(pixels, 0, width, 0, 0, width, height);
                pixelBuffer = new Color[width * height];
                for (int i = 0; i < pixels.Length; i++)
                {
                    int c = pixels[i];
                    pixelBuffer[i] = new Color(
                        ((c >> 16) & 0xFF) / 255f,
                        ((c >> 8) & 0xFF) / 255f,
                        (c & 0xFF) / 255f,
                        ((c >> 24) & 0xFF) / 255f
                    );
                }
                return pixelBuffer;
            }
#endif
            EnsurePixelBufferLoaded();
            if (pixelBuffer != null && pixelBuffer.Length == width * height)
                return pixelBuffer;
            return pixelBuffer ?? Array.Empty<Color>();
        }

        /// <summary>
        /// Truy xuất và trả về dữ liệu của GetPixels.
        /// </summary>
        public Color[] GetPixels(int x, int y, int blockWidth, int blockHeight)
        {
            Color[] block = new Color[blockWidth * blockHeight];
#if ANDROID || __ANDROID__
            if (AndroidBitmap != null && !AndroidBitmap.IsRecycled)
            {
                for (int r = 0; r < blockHeight; r++)
                {
                    for (int c = 0; c < blockWidth; c++)
                    {
                        int px = x + c;
                        int py = y + r;
                        if (px >= 0 && px < width && py >= 0 && py < height)
                        {
                            int colorInt = AndroidBitmap.GetPixel(px, py);
                            float a = ((colorInt >> 24) & 0xFF) / 255f;
                            float rCol = ((colorInt >> 16) & 0xFF) / 255f;
                            float gCol = ((colorInt >> 8) & 0xFF) / 255f;
                            float bCol = (colorInt & 0xFF) / 255f;
                            block[r * blockWidth + c] = new Color(rCol, gCol, bCol, a);
                        }
                    }
                }
                return block;
            }
#endif
            EnsurePixelBufferLoaded();
            for (int r = 0; r < blockHeight; r++)
            {
                for (int c = 0; c < blockWidth; c++)
                {
                    int px = x + c;
                    int py = y + r;
                    if (px >= 0 && px < width && py >= 0 && py < height)
                    {
                        block[r * blockWidth + c] = GetPixel(px, py);
                    }
                }
            }
            return block;
        }

        /// <summary>
        /// Thiết lập giá trị hoặc trạng thái cho SetPixels.
        /// </summary>
        public void SetPixels(Color[] colors)
        {
            if (colors == null) return;
            if (pixelBuffer == null || pixelBuffer.Length != width * height)
            {
                pixelBuffer = new Color[width * height];
            }
            int count = System.Math.Min(colors.Length, pixelBuffer.Length);
            Array.Copy(colors, pixelBuffer, count);
        }

        /// <summary>
        /// Thiết lập giá trị hoặc trạng thái cho SetPixels.
        /// </summary>
        public void SetPixels(int x, int y, int blockWidth, int blockHeight, Color[] colors)
        {
            if (colors == null) return;
            if (pixelBuffer == null || pixelBuffer.Length != width * height)
            {
                pixelBuffer = new Color[width * height];
            }
            int idx = 0;
            for (int r = 0; r < blockHeight; r++)
            {
                for (int c = 0; c < blockWidth; c++)
                {
                    if (idx >= colors.Length) break;
                    int px = x + c;
                    int py = y + r;
                    if (px >= 0 && px < width && py >= 0 && py < height)
                    {
                        pixelBuffer[py * width + px] = colors[idx];
                    }
                    idx++;
                }
            }
        }

        /// <summary>
        /// Khởi tạo và nạp dữ liệu cấu hình cho LoadImage.
        /// </summary>
        public bool LoadImage(byte[] data)
        {
            if (data == null || data.Length == 0) return false;
            try
            {
                _compressedData = data;
#if BROWSER
                if (OperatingSystem.IsBrowser())
                {
                    return DragonBoy_Web_Host.WebGraphicsBackend.LoadImageFromBytes(this, data);
                }
#endif
                if (!OperatingSystem.IsAndroid() && !OperatingSystem.IsIOS())
                {
                    _dataHash = ComputeHash(data);
                    var pool = GetSharedByteTexturePool();
                    if (pool.TryGetValue((_dataHash, data.Length, 1), out var cached) && cached.tex.Id != 0)
                    {
                        this.width = cached.w;
                        this.height = cached.h;
                        this.RaylibTexture = cached.tex;
                        this.HasRaylibTexture = true;
                        this.IsSharedGpuTexture = true;
                        this.pixelBuffer = null;
                        return true;
                    }

                    if (HasRaylibTexture && !IsSharedGpuTexture)
                    {
                        Raylib.UnloadTexture(RaylibTexture);
                        HasRaylibTexture = false;
                    }
                    Raylib_cs.Image img = Raylib.LoadImageFromMemory(".png", data);
                    if (img.Width > 0 && img.Height > 0)
                    {
                        this.width = img.Width;
                        this.height = img.Height;
                        this.RaylibTexture = Raylib.LoadTextureFromImage(img);
                        Raylib.UnloadImage(img);
                        this.RaylibImage = default;
                        TextureFilter desiredFilter = (ModGraphics.renderMode == 0) ? TextureFilter.Point : TextureFilter.Bilinear;
                        Raylib.SetTextureWrap(this.RaylibTexture, TextureWrap.Clamp);
                        Raylib.SetTextureFilter(this.RaylibTexture, desiredFilter);
                        this.currentRaylibFilter = desiredFilter;
                        this.HasRaylibTexture = true;
                        this.IsSharedGpuTexture = true;
                        this.pixelBuffer = null;
                        pool[(_dataHash, data.Length, 1)] = (this.RaylibTexture, this.width, this.height);
                        return true;
                    }
                }
                else
                {
#if ANDROID || __ANDROID__
                    if (OperatingSystem.IsAndroid())
                    {
                        var bmp = Android.Graphics.BitmapFactory.DecodeByteArray(data, 0, data.Length);
                        if (bmp != null)
                        {
                            this.width = bmp.Width;
                            this.height = bmp.Height;
                            this.AndroidBitmap = bmp;
                            this.pixelBuffer = null;
                            return true;
                        }
                    }
#endif
                    if (data.Length >= 24 && data[0] == 0x89 && data[1] == 0x50 && data[2] == 0x4E && data[3] == 0x47)
                    {
                        int w = (data[16] << 24) | (data[17] << 16) | (data[18] << 8) | data[19];
                        int h = (data[20] << 24) | (data[21] << 16) | (data[22] << 8) | data[23];
                        this.width = w > 0 ? w : 1;
                        this.height = h > 0 ? h : 1;
                    }
                    else
                    {
                        this.width = 1;
                        this.height = 1;
                    }
                    this.pixelBuffer = null;
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Texture2D.LoadImage] Exception: {ex.Message}");
            }
            return false;
        }

        /// <summary>
        /// Thực thi phương thức nghiệp vụ ScaleNN.
        /// </summary>
        public void ScaleNN(int factor)
        {
            if (factor <= 1 || width <= 0 || height <= 0) return;
            int newWidth = width * factor;
            int newHeight = height * factor;

#if BROWSER
            if (OperatingSystem.IsBrowser())
            {
                DragonBoy_Web_Host.WebGraphicsBackend.ScaleTextureNN(this, newWidth, newHeight);
                return;
            }
#endif

            if (!OperatingSystem.IsAndroid() && !OperatingSystem.IsIOS())
            {
                if (_compressedData != null && _compressedData.Length > 0)
                {
                    if (_dataHash == 0) _dataHash = ComputeHash(_compressedData);
                    var pool = GetSharedByteTexturePool();
                    if (pool.TryGetValue((_dataHash, _compressedData.Length, factor), out var cached) && cached.tex.Id != 0)
                    {
                        this.width = cached.w;
                        this.height = cached.h;
                        this.RaylibTexture = cached.tex;
                        this.HasRaylibTexture = true;
                        this.IsSharedGpuTexture = true;
                        this.pixelBuffer = null;
                        return;
                    }

                    Raylib_cs.Image img = Raylib.LoadImageFromMemory(".png", _compressedData);
                    if (img.Width > 0 && img.Height > 0)
                    {
                        Raylib.ImageResizeNN(ref img, newWidth, newHeight);
                        if (HasRaylibTexture && !IsSharedGpuTexture)
                        {
                            Raylib.UnloadTexture(RaylibTexture);
                            HasRaylibTexture = false;
                        }
                        RaylibTexture = Raylib.LoadTextureFromImage(img);
                        Raylib.UnloadImage(img);
                        this.RaylibImage = default;
                        TextureFilter desiredFilter = (ModGraphics.renderMode == 0) ? TextureFilter.Point : TextureFilter.Bilinear;
                        Raylib.SetTextureWrap(RaylibTexture, TextureWrap.Clamp); Raylib.SetTextureFilter(RaylibTexture, desiredFilter);
                        this.currentRaylibFilter = desiredFilter;
                        HasRaylibTexture = true;
                        IsSharedGpuTexture = true;
                        this.width = newWidth;
                        this.height = newHeight;
                        this.pixelBuffer = null;
                        pool[(_dataHash, _compressedData.Length, factor)] = (this.RaylibTexture, this.width, this.height);
                        return;
                    }
                }
                else if (HasRaylibTexture && RaylibTexture.Id != 0)
                {
                    Raylib_cs.Image img = Raylib.LoadImageFromTexture(RaylibTexture);
                    if (img.Width > 0 && img.Height > 0)
                    {
                        Raylib.ImageResizeNN(ref img, newWidth, newHeight);
                        if (!IsSharedGpuTexture)
                        {
                            Raylib.UnloadTexture(RaylibTexture);
                        }
                        RaylibTexture = Raylib.LoadTextureFromImage(img);
                        Raylib.UnloadImage(img);
                        TextureFilter desiredFilter = (ModGraphics.renderMode == 0) ? TextureFilter.Point : TextureFilter.Bilinear;
                        Raylib.SetTextureWrap(RaylibTexture, TextureWrap.Clamp); Raylib.SetTextureFilter(RaylibTexture, desiredFilter);
                        this.currentRaylibFilter = desiredFilter;
                        HasRaylibTexture = true;
                        IsSharedGpuTexture = false;
                        this.width = newWidth;
                        this.height = newHeight;
                        this.pixelBuffer = null;
                        return;
                    }
                }
            }

#if ANDROID || __ANDROID__
            if (OperatingSystem.IsAndroid() && AndroidBitmap != null && !AndroidBitmap.IsRecycled)
            {
                try
                {
                    var scaled = Android.Graphics.Bitmap.CreateScaledBitmap(AndroidBitmap, newWidth, newHeight, false);
                    if (scaled != null)
                    {
                        AndroidBitmap.Recycle();
                        AndroidBitmap = scaled;
                        this.width = newWidth;
                        this.height = newHeight;
                        this.pixelBuffer = null;
                        return;
                    }
                }
                catch { }
            }
#endif

            if (pixelBuffer != null && pixelBuffer.Length == width * height)
            {
                Color[] newBuf = new Color[newWidth * newHeight];
                for (int y = 0; y < newHeight; y++)
                {
                    int srcY = y / factor;
                    for (int x = 0; x < newWidth; x++)
                    {
                        int srcX = x / factor;
                        newBuf[y * newWidth + x] = pixelBuffer[srcY * width + srcX];
                    }
                }
                pixelBuffer = newBuf;
            }
            this.width = newWidth;
            this.height = newHeight;

            if (!OperatingSystem.IsAndroid() && !OperatingSystem.IsIOS() && !OperatingSystem.IsBrowser() && pixelBuffer != null)
            {
                if (HasRaylibTexture && !IsSharedGpuTexture)
                {
                    Raylib.UnloadTexture(RaylibTexture);
                    HasRaylibTexture = false;
                }
                Apply();
            }
        }

        /// <summary>
        /// Thực thi phương thức nghiệp vụ EncodeToPNG.
        /// </summary>
        private static unsafe void QuantizeRgbaBufferJava2x2(Raylib_cs.Color* p, int w, int h)
        {
            for (int y = 0; y + 1 < h; y += 2)
            {
                int row0 = y * w;
                int row1 = (y + 1) * w;
                for (int x = 0; x + 1 < w; x += 2)
                {
                    Raylib_cs.Color c0 = p[row0 + x];
                    Raylib_cs.Color c1 = p[row0 + x + 1];
                    Raylib_cs.Color c2 = p[row1 + x];
                    Raylib_cs.Color c3 = p[row1 + x + 1];

                    int sumA = c0.A + c1.A + c2.A + c3.A;
                    Raylib_cs.Color outCol;
                    if (sumA >= 256)
                    {
                        int ar = (c0.R * c0.A + c1.R * c1.A + c2.R * c2.A + c3.R * c3.A) / sumA;
                        int ag = (c0.G * c0.A + c1.G * c1.A + c2.G * c2.A + c3.G * c3.A) / sumA;
                        int ab = (c0.B * c0.A + c1.B * c1.A + c2.B * c2.A + c3.B * c3.A) / sumA;

                        Raylib_cs.Color best = default;
                        int bestDist = int.MaxValue;
                        if (c0.A >= 128) { int d = System.Math.Abs(c0.R - ar) + System.Math.Abs(c0.G - ag) + System.Math.Abs(c0.B - ab); if (d < bestDist) { bestDist = d; best = c0; } }
                        if (c1.A >= 128) { int d = System.Math.Abs(c1.R - ar) + System.Math.Abs(c1.G - ag) + System.Math.Abs(c1.B - ab); if (d < bestDist) { bestDist = d; best = c1; } }
                        if (c2.A >= 128) { int d = System.Math.Abs(c2.R - ar) + System.Math.Abs(c2.G - ag) + System.Math.Abs(c2.B - ab); if (d < bestDist) { bestDist = d; best = c2; } }
                        if (c3.A >= 128) { int d = System.Math.Abs(c3.R - ar) + System.Math.Abs(c3.G - ag) + System.Math.Abs(c3.B - ab); if (d < bestDist) { bestDist = d; best = c3; } }
                        if (bestDist == int.MaxValue)
                        {
                            outCol = new Raylib_cs.Color((byte)ar, (byte)ag, (byte)ab, (byte)255);
                        }
                        else
                        {
                            outCol = new Raylib_cs.Color(best.R, best.G, best.B, (byte)255);
                        }
                    }
                    else
                    {
                        outCol = new Raylib_cs.Color((byte)0, (byte)0, (byte)0, (byte)0);
                    }

                    p[row0 + x] = outCol;
                    p[row0 + x + 1] = outCol;
                    p[row1 + x] = outCol;
                    p[row1 + x + 1] = outCol;
                }
            }
        }

        public static unsafe void QuantizeRaylibImageJava2x2(ref Raylib_cs.Image img)
        {
            if (img.Width < 2 || img.Height < 2) return;
            if (img.Format != PixelFormat.UncompressedR8G8B8A8)
            {
                Raylib.ImageFormat(ref img, PixelFormat.UncompressedR8G8B8A8);
            }
            if (img.Data != null)
            {
                QuantizeRgbaBufferJava2x2((Raylib_cs.Color*)img.Data, img.Width, img.Height);
            }
        }

        public unsafe void QuantizeJava2x2()
        {
            if (width < 2 || height < 2) return;

#if BROWSER
            if (OperatingSystem.IsBrowser())
            {
                DragonBoy_Web_Host.WebGraphicsBackend.QuantizeTextureJava2x2(this);
                return;
            }
#endif

            if (!OperatingSystem.IsAndroid() && !OperatingSystem.IsIOS())
            {
                if (_compressedData != null && _compressedData.Length > 0)
                {
                    if (_dataHash == 0) _dataHash = ComputeHash(_compressedData);
                    var pool = GetSharedByteTexturePool();
                    if (pool.TryGetValue((_dataHash, _compressedData.Length, -2), out var cached) && cached.tex.Id != 0)
                    {
                        this.width = cached.w;
                        this.height = cached.h;
                        this.RaylibTexture = cached.tex;
                        this.HasRaylibTexture = true;
                        this.IsSharedGpuTexture = true;
                        this.pixelBuffer = null;
                        return;
                    }

                    Raylib_cs.Image img = Raylib.LoadImageFromMemory(".png", _compressedData);
                    if (img.Width >= 2 && img.Height >= 2)
                    {
                        if (img.Format != PixelFormat.UncompressedR8G8B8A8)
                        {
                            Raylib.ImageFormat(ref img, PixelFormat.UncompressedR8G8B8A8);
                        }
                        if (img.Data != null)
                        {
                            QuantizeRgbaBufferJava2x2((Raylib_cs.Color*)img.Data, img.Width, img.Height);
                        }
                        if (HasRaylibTexture && !IsSharedGpuTexture)
                        {
                            Raylib.UnloadTexture(RaylibTexture);
                            HasRaylibTexture = false;
                        }
                        RaylibTexture = Raylib.LoadTextureFromImage(img);
                        this.width = img.Width;
                        this.height = img.Height;
                        Raylib.UnloadImage(img);
                        this.RaylibImage = default;
                        Raylib.SetTextureWrap(RaylibTexture, TextureWrap.Clamp);
                        Raylib.SetTextureFilter(RaylibTexture, TextureFilter.Point);
                        this.currentRaylibFilter = TextureFilter.Point;
                        HasRaylibTexture = true;
                        IsSharedGpuTexture = true;
                        this.pixelBuffer = null;
                        pool[(_dataHash, _compressedData.Length, -2)] = (this.RaylibTexture, this.width, this.height);
                        return;
                    }
                    if (img.Width > 0 && img.Height > 0)
                    {
                        Raylib.UnloadImage(img);
                    }
                }
                return;
            }

#if ANDROID || __ANDROID__
            if (OperatingSystem.IsAndroid() && AndroidBitmap != null && !AndroidBitmap.IsRecycled)
            {
                try
                {
                    int w = AndroidBitmap.Width;
                    int h = AndroidBitmap.Height;
                    if (w < 2 || h < 2) return;
                    int[] pixels = new int[w * h];
                    AndroidBitmap.GetPixels(pixels, 0, w, 0, 0, w, h);
                    for (int y = 0; y + 1 < h; y += 2)
                    {
                        int row0 = y * w;
                        int row1 = (y + 1) * w;
                        for (int x = 0; x + 1 < w; x += 2)
                        {
                            int p0 = pixels[row0 + x];
                            int p1 = pixels[row0 + x + 1];
                            int p2 = pixels[row1 + x];
                            int p3 = pixels[row1 + x + 1];

                            int a0 = (p0 >> 24) & 0xFF, a1 = (p1 >> 24) & 0xFF, a2 = (p2 >> 24) & 0xFF, a3 = (p3 >> 24) & 0xFF;
                            int sumA = a0 + a1 + a2 + a3;
                            int outCol = 0;
                            if (sumA >= 256)
                            {
                                int r0 = (p0 >> 16) & 0xFF, g0 = (p0 >> 8) & 0xFF, b0 = p0 & 0xFF;
                                int r1 = (p1 >> 16) & 0xFF, g1 = (p1 >> 8) & 0xFF, b1 = p1 & 0xFF;
                                int r2 = (p2 >> 16) & 0xFF, g2 = (p2 >> 8) & 0xFF, b2 = p2 & 0xFF;
                                int r3 = (p3 >> 16) & 0xFF, g3 = (p3 >> 8) & 0xFF, b3 = p3 & 0xFF;

                                int ar = (r0 * a0 + r1 * a1 + r2 * a2 + r3 * a3) / sumA;
                                int ag = (g0 * a0 + g1 * a1 + g2 * a2 + g3 * a3) / sumA;
                                int ab = (b0 * a0 + b1 * a1 + b2 * a2 + b3 * a3) / sumA;

                                int bestDist = int.MaxValue;
                                int br = ar, bg = ag, bb = ab;
                                if (a0 >= 128) { int d = System.Math.Abs(r0 - ar) + System.Math.Abs(g0 - ag) + System.Math.Abs(b0 - ab); if (d < bestDist) { bestDist = d; br = r0; bg = g0; bb = b0; } }
                                if (a1 >= 128) { int d = System.Math.Abs(r1 - ar) + System.Math.Abs(g1 - ag) + System.Math.Abs(b1 - ab); if (d < bestDist) { bestDist = d; br = r1; bg = g1; bb = b1; } }
                                if (a2 >= 128) { int d = System.Math.Abs(r2 - ar) + System.Math.Abs(g2 - ag) + System.Math.Abs(b2 - ab); if (d < bestDist) { bestDist = d; br = r2; bg = g2; bb = b2; } }
                                if (a3 >= 128) { int d = System.Math.Abs(r3 - ar) + System.Math.Abs(g3 - ag) + System.Math.Abs(b3 - ab); if (d < bestDist) { bestDist = d; br = r3; bg = g3; bb = b3; } }

                                outCol = unchecked((int)0xFF000000) | (br << 16) | (bg << 8) | bb;
                            }

                            pixels[row0 + x] = outCol;
                            pixels[row0 + x + 1] = outCol;
                            pixels[row1 + x] = outCol;
                            pixels[row1 + x + 1] = outCol;
                        }
                    }
                    var quantized = Android.Graphics.Bitmap.CreateBitmap(pixels, w, h, Android.Graphics.Bitmap.Config.Argb8888);
                    if (quantized != null)
                    {
                        AndroidBitmap.Recycle();
                        AndroidBitmap = quantized;
                        this.pixelBuffer = null;
                    }
                }
                catch { }
            }
#endif
        }

        public byte[] EncodeToPNG()
        {
#if ANDROID || __ANDROID__
            if (OperatingSystem.IsAndroid())
            {
                try
                {
                    if (AndroidBitmap != null && !AndroidBitmap.IsRecycled)
                    {
                        using var ms = new System.IO.MemoryStream();
                        AndroidBitmap.Compress(Android.Graphics.Bitmap.CompressFormat.Png, 100, ms);
                        return ms.ToArray();
                    }
                }
                catch { }
            }
#endif
            return Array.Empty<byte>();
        }

        /// <summary>
        /// Áp dụng các thay đổi cấu hình hoặc hiệu ứng của Apply.
        /// </summary>
        public unsafe void Apply()
        {
            if (width <= 0 || height <= 0 || pixelBuffer == null) return;
#if BROWSER
            if (OperatingSystem.IsBrowser())
            {
                DragonBoy_Web_Host.WebGraphicsBackend.ApplyPixelBuffer(this);
                return;
            }
#endif
#if ANDROID || __ANDROID__
            if (OperatingSystem.IsAndroid())
            {
                try
                {
                    int[] intColors = new int[pixelBuffer.Length];
                    for (int i = 0; i < pixelBuffer.Length; i++)
                    {
                        Color c = pixelBuffer[i];
                        int a = (int)(System.Math.Clamp(c.a, 0f, 1f) * 255);
                        int r = (int)(System.Math.Clamp(c.r, 0f, 1f) * 255);
                        int g = (int)(System.Math.Clamp(c.g, 0f, 1f) * 255);
                        int b = (int)(System.Math.Clamp(c.b, 0f, 1f) * 255);
                        intColors[i] = (a << 24) | (r << 16) | (g << 8) | b;
                    }
                    if (AndroidBitmap != null && !AndroidBitmap.IsRecycled)
                    {
                        AndroidBitmap.Recycle();
                    }
                    AndroidBitmap = Android.Graphics.Bitmap.CreateBitmap(intColors, width, height, Android.Graphics.Bitmap.Config.Argb8888);
                }
                catch { }
                return;
            }
#endif
            if (OperatingSystem.IsAndroid() || OperatingSystem.IsIOS() || OperatingSystem.IsBrowser()) return;
            try
            {
                if (HasRaylibTexture && !IsSharedGpuTexture)
                {
                    Raylib.UnloadTexture(RaylibTexture);
                    HasRaylibTexture = false;
                }
                Raylib_cs.Image img = Raylib.GenImageColor(width, height, Raylib_cs.Color.Blank);
                Raylib_cs.Color* p = (Raylib_cs.Color*)img.Data;
                for (int i = 0; i < pixelBuffer.Length && i < width * height; i++)
                {
                    p[i] = pixelBuffer[i];
                }
                RaylibTexture = Raylib.LoadTextureFromImage(img);
                TextureFilter desiredFilter = (ModGraphics.renderMode == 0) ? TextureFilter.Point : TextureFilter.Bilinear;
                Raylib.SetTextureWrap(RaylibTexture, TextureWrap.Clamp); Raylib.SetTextureFilter(RaylibTexture, desiredFilter);
                this.currentRaylibFilter = desiredFilter;
                Raylib.UnloadImage(img);
                HasRaylibTexture = true;
                IsSharedGpuTexture = false;
            }
            catch { }
        }
    }

    /// <summary>
    /// Thành phần phân mảnh (partial) của lớp TextureFormat: Đảm nhiệm phân hệ chuyên trách Graphics ().
    /// </summary>
    public enum TextureFormat
    {
        Alpha8 = 1,
        ARGB4444 = 2,
        RGB24 = 3,
        RGBA32 = 4,
        ARGB32 = 5,
        RGB565 = 7,
        R16 = 9,
        DXT1 = 10,
        DXT5 = 12,
        RGBA4444 = 13,
        BGRA32 = 14
    }

    /// <summary>
    /// Thành phần phân mảnh (partial) của lớp Sprite: Đảm nhiệm phân hệ chuyên trách Graphics ().
    /// </summary>
    public class Sprite
    {
    }

    /// <summary>
    /// Thành phần phân mảnh (partial) của lớp Screen: Đảm nhiệm phân hệ chuyên trách Graphics ().
    /// </summary>
    public static class Screen
    {
        public static int customWidth { get; set; } = 1024;
        public static int customHeight { get; set; } = 600;

        public static int width
        {
            get
            {
#if BROWSER
                if (OperatingSystem.IsBrowser())
                    return DragonBoy_Web_Host.WebGraphicsBackend.ScreenWidth;
#endif
                if (OperatingSystem.IsAndroid() || OperatingSystem.IsIOS() || OperatingSystem.IsBrowser())
                    return customWidth;
                try { return Raylib.GetScreenWidth(); } catch { return customWidth; }
            }
            set => customWidth = value;
        }

        public static int height
        {
            get
            {
#if BROWSER
                if (OperatingSystem.IsBrowser())
                    return DragonBoy_Web_Host.WebGraphicsBackend.ScreenHeight;
#endif
                if (OperatingSystem.IsAndroid() || OperatingSystem.IsIOS() || OperatingSystem.IsBrowser())
                    return customHeight;
                try { return Raylib.GetScreenHeight(); } catch { return customHeight; }
            }
            set => customHeight = value;
        }

        public static int w => width;
        public static int h => height;

        public static ScreenOrientation orientation { get; set; } = ScreenOrientation.LandscapeLeft;

        public static bool fullScreen
        {
            get
            {
                if (OperatingSystem.IsAndroid() || OperatingSystem.IsIOS())
                    return true;
#if BROWSER
                if (OperatingSystem.IsBrowser())
                    return DragonBoy_Web_Host.WebGraphicsBackend.IsFullScreen;
#endif
                if (OperatingSystem.IsBrowser())
                    return ModGraphics.isFullscreen;
                try { return Raylib.IsWindowFullscreen(); } catch { return false; }
            }
            set
            {
                if (!Application.IsActiveTab || OperatingSystem.IsAndroid() || OperatingSystem.IsIOS()) return;
#if BROWSER
                if (OperatingSystem.IsBrowser())
                {
                    if (value != DragonBoy_Web_Host.WebGraphicsBackend.IsFullScreen)
                    {
                        DragonBoy_Web_Host.WebGraphicsBackend.SetResolution(DragonBoy_Web_Host.WebGraphicsBackend.ScreenWidth, DragonBoy_Web_Host.WebGraphicsBackend.ScreenHeight, value);
                    }
                    return;
                }
#endif
                if (OperatingSystem.IsBrowser()) return;
                try
                {
                    if (value != Raylib.IsWindowFullscreen())
                    {
                        Raylib.ToggleFullscreen();
                    }
                }
                catch { }
            }
        }

        public static Resolution currentResolution
        {
            get
            {
                Resolution r = default;
                if (OperatingSystem.IsAndroid() || OperatingSystem.IsIOS() || OperatingSystem.IsBrowser())
                {
                    r.width = width;
                    r.height = height;
                    r.refreshRate = 60;
                    return r;
                }
                try
                {
                    r.width = Raylib.GetMonitorWidth(Raylib.GetCurrentMonitor());
                    r.height = Raylib.GetMonitorHeight(Raylib.GetCurrentMonitor());
                    r.refreshRate = Raylib.GetMonitorRefreshRate(Raylib.GetCurrentMonitor());
                }
                catch
                {
                    r.width = customWidth;
                    r.height = customHeight;
                    r.refreshRate = 60;
                }
                return r;
            }
        }

        /// <summary>
        /// Thiết lập giá trị hoặc trạng thái cho SetResolution.
        /// </summary>
        public static void SetResolution(int width, int height, bool fullscreen)
        {
            customWidth = width;
            customHeight = height;
#if BROWSER
            if (OperatingSystem.IsBrowser())
            {
                DragonBoy_Web_Host.WebGraphicsBackend.OnSetResolution(width, height, fullscreen);
                return;
            }
#endif
            if (!Application.IsActiveTab || OperatingSystem.IsAndroid() || OperatingSystem.IsIOS()) return;
            try
            {
                Raylib.SetWindowSize(width, height);
                if (fullscreen != Raylib.IsWindowFullscreen())
                {
                    Raylib.ToggleFullscreen();
                }
                if (!fullscreen)
                {
                    DragonBoy_Net8_Native.Engine.Graphics.RenderManager.CenterWindow();
                }
            }
            catch { }
        }
    }

    /// <summary>
    /// Thành phần phân mảnh (partial) của lớp QualitySettings: Đảm nhiệm phân hệ chuyên trách Graphics ().
    /// </summary>
    public static class QualitySettings
    {
        public static AnisotropicFiltering anisotropicFiltering { get; set; } = AnisotropicFiltering.ForceEnable;
        public static int antiAliasing { get; set; } = 4;
        public static int masterTextureLimit { get; set; } = 0;
        public static int vSyncCount { get; set; } = 0;
    }

    /// <summary>
    /// Thành phần phân mảnh (partial) của lớp Graphics: Đảm nhiệm phân hệ chuyên trách Graphics ().
    /// </summary>
    public static class Graphics
    {
        /// <summary>
        /// Thực hiện vẽ và hiển thị thành phần DrawTexture lên canvas đồ họa.
        /// </summary>
        public static void DrawTexture(Rect screenRect, Texture texture)
        {
            DrawTexture(screenRect, texture, new Rect(0, 0, 1, 1), 0, 0, 0, 0);
        }

        /// <summary>
        /// Thực hiện vẽ và hiển thị thành phần DrawTexture lên canvas đồ họa.
        /// </summary>
        public static void DrawTexture(Rect screenRect, Texture texture, Rect sourceRect, int leftBorder, int rightBorder, int topBorder, int bottomBorder)
        {
#if BROWSER
            if (OperatingSystem.IsBrowser())
            {
                DragonBoy_Web_Host.WebGraphicsBackend.DrawTexture(screenRect, texture, sourceRect, GUI.color);
                return;
            }
#endif
#if ANDROID || __ANDROID__
            if (OperatingSystem.IsAndroid())
            {
                DragonBoy_Android_Host.AndroidGraphicsBackend.DrawTexture(screenRect, texture, sourceRect, GUI.color);
                return;
            }
#endif
            if (OperatingSystem.IsAndroid() || OperatingSystem.IsIOS()) return;
            try
            {
                float destX = screenRect.x;
                float destY = screenRect.y;
                float destW = screenRect.width;
                float destH = screenRect.height;

                if (texture is Texture2D t2d && t2d.HasRaylibTexture)
                {
                    bool isFontTex = !string.IsNullOrEmpty(t2d._sourceFilePath) && t2d._sourceFilePath.IndexOf("myfont", StringComparison.OrdinalIgnoreCase) >= 0;
                    TextureFilter desiredFilter = (ModGraphics.renderMode == 0 || isFontTex) ? TextureFilter.Point : TextureFilter.Bilinear;
                    if (t2d.currentRaylibFilter != desiredFilter)
                    {
                        Raylib.SetTextureFilter(t2d.RaylibTexture, desiredFilter);
                        t2d.currentRaylibFilter = desiredFilter;
                    }

                    float srcX = MathF.Round(sourceRect.x * texture.width);
                    float srcY = MathF.Round((1f - sourceRect.y - sourceRect.height) * texture.height);
                    float srcW = MathF.Round(sourceRect.width * texture.width);
                    float srcH = MathF.Round(sourceRect.height * texture.height);

                    if (destW < 0)
                    {
                        destX += destW;
                        destW = -destW;
                        srcW = -srcW;
                    }
                    if (destH < 0)
                    {
                        destY += destH;
                        destH = -destH;
                        srcH = -srcH;
                    }

                    destX = MathF.Round(destX);
                    destY = MathF.Round(destY);
                    destW = MathF.Round(destW);
                    destH = MathF.Round(destH);

                    Raylib_cs.Rectangle dest = new Raylib_cs.Rectangle(destX, destY, destW, destH);
                    Raylib_cs.Rectangle src = new Raylib_cs.Rectangle(srcX, srcY, srcW, srcH);
                    Raylib_cs.Color tint = GUI.color;
                    Raylib.DrawTexturePro(t2d.RaylibTexture, src, dest, System.Numerics.Vector2.Zero, 0f, tint);
                }
                else
                {
                    if (destW < 0) { destX += destW; destW = -destW; }
                    if (destH < 0) { destY += destH; destH = -destH; }
                    Raylib.DrawRectangle((int)MathF.Round(destX), (int)MathF.Round(destY), (int)MathF.Round(destW), (int)MathF.Round(destH), GUI.color);
                }
            }
            catch { }
        }
    }

    /// <summary>
    /// Thành phần phân mảnh (partial) của lớp TextAnchor: Đảm nhiệm phân hệ chuyên trách Graphics ().
    /// </summary>
    public enum TextAnchor
    {
        UpperLeft = 0,
        UpperCenter = 1,
        UpperRight = 2,
        MiddleLeft = 3,
        MiddleCenter = 4,
        MiddleRight = 5,
        LowerLeft = 6,
        LowerCenter = 7,
        LowerRight = 8
    }

    /// <summary>
    /// Thành phần phân mảnh (partial) của lớp GUIStyleState: Đảm nhiệm phân hệ chuyên trách Graphics ().
    /// </summary>
    public class GUIStyleState
    {
        public Color textColor { get; set; } = Color.white;
    }

    /// <summary>
    /// Thành phần phân mảnh (partial) của lớp GUIStyle: Đảm nhiệm phân hệ chuyên trách Graphics ().
    /// </summary>
    public class GUIStyle
    {
        public int fontSize { get; set; } = 0;
        public Font font { get; set; }
        public TextAnchor alignment { get; set; } = TextAnchor.UpperLeft;
        public GUIStyleState normal { get; set; } = new GUIStyleState();

        /// <summary>
        /// Thực thi phương thức nghiệp vụ GUIStyle.
        /// </summary>
        public GUIStyle() { }

        /// <summary>
        /// Thực thi phương thức nghiệp vụ GUIStyle.
        /// </summary>
        public GUIStyle(GUIStyle other)
        {
            if (other != null)
            {
                fontSize = other.fontSize;
                font = other.font;
                alignment = other.alignment;
                normal = new GUIStyleState { textColor = other.normal.textColor };
            }
        }

        /// <summary>
        /// Thực thi phương thức nghiệp vụ CalcSize.
        /// </summary>
        public Vector2 CalcSize(GUIContent content)
        {
            if (content == null || string.IsNullOrEmpty(content.text)) return Vector2.zero;
#if BROWSER
            if (OperatingSystem.IsBrowser())
            {
                return DragonBoy_Web_Host.WebGraphicsBackend.MeasureText(content.text, this);
            }
#endif
#if ANDROID || __ANDROID__
            if (OperatingSystem.IsAndroid())
            {
                return DragonBoy_Android_Host.AndroidGraphicsBackend.MeasureText(content.text, this);
            }
#endif
            int fs = (fontSize > 0) ? (fontSize * mGraphics.zoomLevel) : Font.GetDefaultFontSize(this.font);
            Raylib_cs.Font rayFont = Font.GetRaylibFont(this.font, fs);
            if (rayFont.Texture.Id != 0)
            {
                System.Numerics.Vector2 size = Raylib.MeasureTextEx(rayFont, content.text, fs, 0f);
                return new Vector2(size.X, size.Y);
            }
            return new Vector2(content.text.Length * (fs / 2), fs);
        }
    }

    /// <summary>
    /// Thành phần phân mảnh (partial) của lớp GUISkin: Đảm nhiệm phân hệ chuyên trách Graphics ().
    /// </summary>
    public class GUISkin
    {
        public GUIStyle label { get; set; } = new GUIStyle();
    }

    /// <summary>
    /// Thành phần phân mảnh (partial) của lớp GUIContent: Đảm nhiệm phân hệ chuyên trách Graphics ().
    /// </summary>
    public class GUIContent
    {
        public string text { get; set; } = string.Empty;
        /// <summary>
        /// Thực thi phương thức nghiệp vụ GUIContent.
        /// </summary>
        public GUIContent(string text) { this.text = text; }
    }

    /// <summary>
    /// Thành phần phân mảnh (partial) của lớp Font: Đảm nhiệm phân hệ chuyên trách Graphics ().
    /// </summary>
    public class Font
    {
        public string name { get; set; } = "DefaultFont";

        private static readonly string[,] _resolvedPaths = new string[4, 3];
        private static bool _fontPathsResolved = false;
        private static int[] _cachedCodepoints;
        private static readonly object _lock = new object();
        private const string SharedFontSlot = "DragonBoy.SharedExactFonts.v3";
        private const string EmbeddedPrefix = "embedded:";
        private static readonly byte[] s_FontGammaLut = BuildFontGammaLut();

        private static byte[] BuildFontGammaLut()
        {
            byte[] lut = new byte[256];
            lut[0] = 0;
            lut[255] = 255;
            for (int i = 1; i < 255; i++)
            {
                float corrected = MathF.Pow(i / 255f, 0.68f) * 255f;
                lut[i] = (byte)System.Math.Clamp((int)MathF.Round(corrected), 0, 255);
            }
            return lut;
        }

        private static System.Collections.Concurrent.ConcurrentDictionary<int, Raylib_cs.Font> GetSharedFontPool()
        {
            if (AppDomain.CurrentDomain.GetData(SharedFontSlot) is System.Collections.Concurrent.ConcurrentDictionary<int, Raylib_cs.Font> dict)
            {
                return dict;
            }
            lock (typeof(AppDomain))
            {
                if (AppDomain.CurrentDomain.GetData(SharedFontSlot) is System.Collections.Concurrent.ConcurrentDictionary<int, Raylib_cs.Font> existing)
                {
                    return existing;
                }
                var created = new System.Collections.Concurrent.ConcurrentDictionary<int, Raylib_cs.Font>();
                AppDomain.CurrentDomain.SetData(SharedFontSlot, created);
                return created;
            }
        }

        /// <summary>
        /// Thực thi phương thức nghiệp vụ ResolveFontPath.
        /// </summary>
        private static string ResolveFontPath(string fileName, params string[] windowsFallbacks)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string[] searchPaths = new string[]
                {
                    System.IO.Path.Combine(baseDir, "Assets", "myfont", fileName),
                    System.IO.Path.Combine(baseDir, "myfont", fileName),
                    System.IO.Path.Combine(@"C:\ModNRO\DragonBoy_Shared\Assets\myfont", fileName),
                    System.IO.Path.Combine(@"C:\ModNRO\DragonBoy_Net8_Native\Assets\myfont", fileName),
                    System.IO.Path.Combine(@"C:\ModNRO\ModNRO_Tools\Decompiled\APK_apktool\assets\myfont", fileName),
                    System.IO.Path.Combine(baseDir, fileName)
                };

                foreach (var path in searchPaths)
                {
                    if (System.IO.File.Exists(path)) return path;
                }

                string resourceName = "myfont/" + fileName;
                var entryAsm = System.Reflection.Assembly.GetEntryAssembly();
                if (entryAsm != null && entryAsm.GetManifestResourceInfo(resourceName) != null)
                {
                    return EmbeddedPrefix + resourceName;
                }
                var selfAsm = typeof(Font).Assembly;
                if (selfAsm.GetManifestResourceInfo(resourceName) != null)
                {
                    return EmbeddedPrefix + resourceName;
                }
            }

            if (windowsFallbacks != null)
            {
                foreach (var fb in windowsFallbacks)
                {
                    if (string.IsNullOrEmpty(fb)) continue;
                    if (fb.StartsWith(EmbeddedPrefix, StringComparison.Ordinal) || System.IO.File.Exists(fb))
                    {
                        return fb;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Thực thi phương thức nghiệp vụ BuildCodepoints.
        /// </summary>
        private static int[] BuildCodepoints()
        {
            System.Collections.Generic.HashSet<int> codepoints = new System.Collections.Generic.HashSet<int>();
            for (int i = 32; i <= 255; i++) codepoints.Add(i);

            string vietnameseChars = "áàảãạăắằẳẵặâấầẩẫậéèẻẽẹêếềểễệíìỉĩịóòỏõọôốồổỗộơớờởỡợúùủũụưứừửữựýỳỷỹỵđ" +
                                     "ÁÀẢÃẠĂẮẰẲẴẶÂẤẦẨẪẬÉÈẺẼẸÊẾỀỂỄỆÍÌỈĨỊÓÒỎÕỌÔỐỒỔỖỘƠỚỜỞỠỢÚÙỦŨỤƯỨỪỬỮỰÝỲỶỸỴĐ" +
                                     "₫•…°±×÷—–'\"«»™©®⭐⚡🔄📌🗑️📂⚙️−+✓✕";
            foreach (char c in vietnameseChars) codepoints.Add((int)c);

            // Latin Extended-A & B
            for (int i = 0x0100; i <= 0x024F; i++) codepoints.Add(i);
            // Vietnamese Precomposed Tone Marks (0x1EA0 - 0x1EF9)
            for (int i = 0x1EA0; i <= 0x1EF9; i++) codepoints.Add(i);
            // General Punctuation (0x2000 - 0x206F)
            for (int i = 0x2000; i <= 0x206F; i++) codepoints.Add(i);
            // Currency Symbols (0x20A0 - 0x20CF)
            for (int i = 0x20A0; i <= 0x20CF; i++) codepoints.Add(i);
            // Letterlike Symbols & Arrows (0x2190 - 0x21FF)
            for (int i = 0x2190; i <= 0x21FF; i++) codepoints.Add(i);
            // Miscellaneous Symbols (0x2600 - 0x26FF)
            for (int i = 0x2600; i <= 0x26FF; i++) codepoints.Add(i);

            int[] cpArray = new int[codepoints.Count];
            codepoints.CopyTo(cpArray);
            return cpArray;
        }

        private static unsafe void SharpenFontAtlasAlpha(ref Raylib_cs.Font f)
        {
            if (f.Texture.Id == 0) return;
            try
            {
                Raylib_cs.Image atlas = Raylib.LoadImageFromTexture(f.Texture);
                if (atlas.Width > 0 && atlas.Height > 0 && atlas.Data != null)
                {
                    if (atlas.Format != PixelFormat.UncompressedR8G8B8A8)
                    {
                        Raylib.ImageFormat(ref atlas, PixelFormat.UncompressedR8G8B8A8);
                    }
                    Raylib_cs.Color* pixels = (Raylib_cs.Color*)atlas.Data;
                    if (pixels != null)
                    {
                        int total = atlas.Width * atlas.Height;
                        for (int i = 0; i < total; i++)
                        {
                            byte a = pixels[i].A;
                            pixels[i] = new Raylib_cs.Color((byte)255, (byte)255, (byte)255, s_FontGammaLut[a]);
                        }
                        Raylib_cs.Texture2D crispTex = Raylib.LoadTextureFromImage(atlas);
                        if (crispTex.Id != 0)
                        {
                            Raylib.UnloadTexture(f.Texture);
                            f.Texture = crispTex;
                        }
                    }
                    Raylib.UnloadImage(atlas);
                }
            }
            catch { }
            Raylib.SetTextureWrap(f.Texture, TextureWrap.Clamp);
            Raylib.SetTextureFilter(f.Texture, TextureFilter.Point);
        }

        private static byte[] ReadEmbeddedResourceBytes(string resourceName)
        {
            var entryAsm = System.Reflection.Assembly.GetEntryAssembly();
            using var s1 = entryAsm?.GetManifestResourceStream(resourceName);
            if (s1 != null)
            {
                using var ms = new System.IO.MemoryStream((int)s1.Length);
                s1.CopyTo(ms);
                return ms.ToArray();
            }
            using var s2 = typeof(Font).Assembly.GetManifestResourceStream(resourceName);
            if (s2 != null)
            {
                using var ms = new System.IO.MemoryStream((int)s2.Length);
                s2.CopyTo(ms);
                return ms.ToArray();
            }
            return null;
        }

        /// <summary>
        /// Khởi tạo và nạp dữ liệu cấu hình cho LoadCustomFont.
        /// </summary>
        private static Raylib_cs.Font LoadCustomFont(string path, int size, int[] codepoints)
        {
            if (string.IsNullOrEmpty(path)) return default;
            try
            {
                Raylib_cs.Font f = default;
                if (path.StartsWith(EmbeddedPrefix, StringComparison.Ordinal))
                {
                    string resName = path.Substring(EmbeddedPrefix.Length);
                    byte[] fontBytes = ReadEmbeddedResourceBytes(resName);
                    if (fontBytes != null && fontBytes.Length > 0)
                    {
                        f = Raylib.LoadFontFromMemory(".ttf", fontBytes, size, codepoints, codepoints.Length);
                    }
                }
                else if (System.IO.File.Exists(path))
                {
                    f = Raylib.LoadFontEx(path, size, codepoints, codepoints.Length);
                }

                if (f.Texture.Id != 0)
                {
                    SharpenFontAtlasAlpha(ref f);
                    return f;
                }
            }
            catch { }
            return default;
        }

#if ANDROID || __ANDROID__
        private static bool _androidFontsLoaded = false;
        private static Android.Graphics.Typeface _fontBarmenebTypeface;
        private static Android.Graphics.Typeface _fontChelthmTypeface;
        private static Android.Graphics.Typeface _fontStaccatoTypeface;
        private static Android.Graphics.Typeface _androidDefaultTypeface;
        private static Android.Graphics.Typeface _tahomaBoldTypeface;
        private static Android.Graphics.Typeface _tahomaRegTypeface;
        private static Android.Graphics.Typeface _segoeBoldTypeface;
        private static Android.Graphics.Typeface _segoeRegTypeface;
        private static Android.Graphics.Typeface _monoBoldTypeface;
        private static Android.Graphics.Typeface _monoRegTypeface;

        /// <summary>
        /// Truy xuất và trả về dữ liệu của GetAndroidTypeface.
        /// </summary>
        public static Android.Graphics.Typeface GetAndroidTypeface(Font font = null)
        {
            EnsureFontsLoaded();
            int theme = System.Math.Clamp(global::ModGraphics.fontTheme, 0, 3);
            bool isRegular = font != null && !string.IsNullOrEmpty(font.name) && font.name.ToLowerInvariant().Contains("chelthm");
            bool isStaccato = font != null && !string.IsNullOrEmpty(font.name) && font.name.ToLowerInvariant().Contains("staccato");

            if (theme == 1)
            {
                return isRegular ? (_tahomaRegTypeface ?? _fontChelthmTypeface) : (_tahomaBoldTypeface ?? _fontBarmenebTypeface);
            }
            if (theme == 2)
            {
                return isRegular ? (_segoeRegTypeface ?? _fontChelthmTypeface) : (_segoeBoldTypeface ?? _fontBarmenebTypeface);
            }
            if (theme == 3)
            {
                return isRegular ? (_monoRegTypeface ?? _fontChelthmTypeface) : (_monoBoldTypeface ?? _fontBarmenebTypeface);
            }

            if (isRegular && _fontChelthmTypeface != null) return _fontChelthmTypeface;
            if (isStaccato && _fontStaccatoTypeface != null) return _fontStaccatoTypeface;
            if (_fontBarmenebTypeface != null) return _fontBarmenebTypeface;
            return _androidDefaultTypeface ?? Android.Graphics.Typeface.DefaultBold;
        }
#endif

        private static int ResolveFontKind(Font font)
        {
            if (font != null && !string.IsNullOrEmpty(font.name))
            {
                string fn = font.name.ToLowerInvariant();
                if (fn.Contains("chelthm")) return 1;
                if (fn.Contains("staccato")) return 2;
            }
            return 0;
        }

        private static void EnsurePathsResolved()
        {
            if (_fontPathsResolved) return;
            string defaultBarmeneb = ResolveFontPath("barmeneb.ttf", @"C:\Windows\Fonts\tahomabd.ttf", @"C:\Windows\Fonts\tahoma.ttf", @"C:\Windows\Fonts\arialbd.ttf");
            string defaultChelthm = ResolveFontPath("chelthm.ttf", @"C:\Windows\Fonts\tahoma.ttf", @"C:\Windows\Fonts\arial.ttf");
            string defaultStaccato = ResolveFontPath("staccato.ttf", defaultBarmeneb);

            _resolvedPaths[0, 0] = defaultBarmeneb;
            _resolvedPaths[0, 1] = defaultChelthm;
            _resolvedPaths[0, 2] = defaultStaccato;

            string tahomaBold = ResolveFontPath(null, @"C:\Windows\Fonts\tahomabd.ttf", @"C:\Windows\Fonts\tahoma.ttf", defaultBarmeneb);
            _resolvedPaths[1, 0] = tahomaBold;
            _resolvedPaths[1, 1] = ResolveFontPath(null, @"C:\Windows\Fonts\tahoma.ttf", defaultChelthm);
            _resolvedPaths[1, 2] = tahomaBold;

            string segoeBold = ResolveFontPath(null, @"C:\Windows\Fonts\segoeuib.ttf", @"C:\Windows\Fonts\segoeui.ttf", @"C:\Windows\Fonts\arialbd.ttf", defaultBarmeneb);
            _resolvedPaths[2, 0] = segoeBold;
            _resolvedPaths[2, 1] = ResolveFontPath(null, @"C:\Windows\Fonts\segoeui.ttf", @"C:\Windows\Fonts\arial.ttf", defaultChelthm);
            _resolvedPaths[2, 2] = segoeBold;

            string consolaBold = ResolveFontPath(null, @"C:\Windows\Fonts\consolab.ttf", @"C:\Windows\Fonts\consola.ttf", defaultBarmeneb);
            _resolvedPaths[3, 0] = consolaBold;
            _resolvedPaths[3, 1] = ResolveFontPath(null, @"C:\Windows\Fonts\consola.ttf", defaultChelthm);
            _resolvedPaths[3, 2] = consolaBold;

            _fontPathsResolved = true;
        }

        /// <summary>
        /// Thực thi phương thức nghiệp vụ EnsureFontsLoaded.
        /// </summary>
        public static void EnsureFontsLoaded()
        {
            if (OperatingSystem.IsBrowser()) return;
#if ANDROID || __ANDROID__
            if (OperatingSystem.IsAndroid())
            {
                if (!_androidFontsLoaded)
                {
                    lock (_lock)
                    {
                        if (!_androidFontsLoaded)
                        {
                            _androidFontsLoaded = true;
                            try
                            {
                                var assets = Android.App.Application.Context?.Assets;
                                if (assets != null)
                                {
                                    try { _fontBarmenebTypeface = Android.Graphics.Typeface.CreateFromAsset(assets, "myfont/barmeneb.ttf"); } catch { }
                                    try { _fontChelthmTypeface = Android.Graphics.Typeface.CreateFromAsset(assets, "myfont/chelthm.ttf"); } catch { }
                                    try { _fontStaccatoTypeface = Android.Graphics.Typeface.CreateFromAsset(assets, "myfont/staccato.ttf"); } catch { }
                                }
                                try { _tahomaBoldTypeface = Android.Graphics.Typeface.Create("sans-serif", Android.Graphics.TypefaceStyle.Bold); } catch { }
                                try { _tahomaRegTypeface = Android.Graphics.Typeface.Create("sans-serif", Android.Graphics.TypefaceStyle.Normal); } catch { }
                                try { _segoeBoldTypeface = Android.Graphics.Typeface.Create("sans-serif-medium", Android.Graphics.TypefaceStyle.Bold); } catch { }
                                try { _segoeRegTypeface = Android.Graphics.Typeface.Create("sans-serif-light", Android.Graphics.TypefaceStyle.Normal); } catch { }
                                try { _monoBoldTypeface = Android.Graphics.Typeface.Create(Android.Graphics.Typeface.Monospace, Android.Graphics.TypefaceStyle.Bold); } catch { }
                                try { _monoRegTypeface = Android.Graphics.Typeface.Monospace; } catch { }
                            }
                            catch { }
                            _androidDefaultTypeface = _fontBarmenebTypeface ?? Android.Graphics.Typeface.DefaultBold;
                        }
                    }
                }
                return;
            }
#endif
            if (OperatingSystem.IsIOS()) return;
            lock (_lock)
            {
                EnsurePathsResolved();
                _cachedCodepoints ??= BuildCodepoints();
            }
        }

        /// <summary>
        /// Truy xuất kích thước tham chiếu bố cục mặc định (100% NRO Gốc) để căn giữa dòng chính xác.
        /// </summary>
        public static int GetLayoutReferenceFontSize(Font font = null)
        {
            int baseSize = 13;
            if (font != null && !string.IsNullOrEmpty(font.name))
            {
                string fn = font.name.ToLowerInvariant();
                if (fn.Contains("barmeneb")) baseSize = 14;
                else if (fn.Contains("chelthm")) baseSize = 12;
                else if (fn.Contains("staccato")) baseSize = 13;
            }
            int zoom = (mGraphics.zoomLevel > 0) ? mGraphics.zoomLevel : 2;
            return baseSize * zoom;
        }

        /// <summary>
        /// Truy xuất và trả về dữ liệu của GetDefaultFontSize.
        /// </summary>
        public static int GetDefaultFontSize(Font font = null)
        {
            int theme = System.Math.Clamp(global::ModGraphics.fontTheme, 0, 3);
            int baseSize = (theme == 0) ? 13 : 12;
            if (font != null && !string.IsNullOrEmpty(font.name))
            {
                string fn = font.name.ToLowerInvariant();
                if (fn.Contains("barmeneb")) baseSize = (theme == 0) ? 14 : 12;
                else if (fn.Contains("chelthm")) baseSize = (theme == 0) ? 12 : 11;
                else if (fn.Contains("staccato")) baseSize = (theme == 0) ? 13 : 12;
            }
            int zoom = (mGraphics.zoomLevel > 0) ? mGraphics.zoomLevel : 2;
            float scale = global::ModGraphics.GetFontSizeScale();
            return System.Math.Max(7, (int)MathF.Round(baseSize * zoom * scale));
        }

        /// <summary>
        /// Truy xuất và trả về dữ liệu của GetRaylibFont.
        /// </summary>
        public static Raylib_cs.Font GetRaylibFont(Font font, int targetPixelSize)
        {
            if (OperatingSystem.IsAndroid() || OperatingSystem.IsIOS() || OperatingSystem.IsBrowser()) return default;
            int theme = System.Math.Clamp(global::ModGraphics.fontTheme, 0, 3);
            int kind = ResolveFontKind(font);
            int pxSize = (targetPixelSize > 0) ? System.Math.Clamp(targetPixelSize, 7, 72) : GetDefaultFontSize(font);
            int key = (theme << 16) | (kind << 8) | pxSize;

            var pool = GetSharedFontPool();
            if (pool.TryGetValue(key, out Raylib_cs.Font existing) && existing.Texture.Id != 0)
            {
                return existing;
            }

            lock (_lock)
            {
                if (pool.TryGetValue(key, out existing) && existing.Texture.Id != 0)
                {
                    return existing;
                }
                EnsurePathsResolved();
                _cachedCodepoints ??= BuildCodepoints();
                string path = _resolvedPaths[theme, kind] ?? _resolvedPaths[theme, 0] ?? _resolvedPaths[0, 0];
                Raylib_cs.Font loaded = LoadCustomFont(path, pxSize, _cachedCodepoints);
                if (loaded.Texture.Id == 0 && path != _resolvedPaths[0, 0])
                {
                    loaded = LoadCustomFont(_resolvedPaths[0, 0], pxSize, _cachedCodepoints);
                }
                if (loaded.Texture.Id != 0)
                {
                    pool[key] = loaded;
                }
                return loaded;
            }
        }

        public static Raylib_cs.Font GetRaylibFont(Font font) => GetRaylibFont(font, 0);

        public static Raylib_cs.Font GetRaylibFont() => GetRaylibFont(null, 0);
    }

    /// <summary>
    /// Thành phần phân mảnh (partial) của lớp Material: Đảm nhiệm phân hệ chuyên trách Graphics ().
    /// </summary>
    public class Material
    {
        public HideFlags hideFlags { get; set; } = HideFlags.None;
        /// <summary>
        /// Thực thi phương thức nghiệp vụ Material.
        /// </summary>
        public Material(string s) { }
        /// <summary>
        /// Thực thi phương thức nghiệp vụ Material.
        /// </summary>
        public Material(Shader shader) { }
        /// <summary>
        /// Thiết lập giá trị hoặc trạng thái cho SetPass.
        /// </summary>
        public void SetPass(int pass) { }

        public static bool operator !(Material m) => m is null;
        public static bool operator true(Material m) => m is not null;
        public static bool operator false(Material m) => m is null;
        public static implicit operator bool(Material m) => m is not null;
    }

    /// <summary>
    /// Thành phần phân mảnh (partial) của lớp Shader: Đảm nhiệm phân hệ chuyên trách Graphics ().
    /// </summary>
    public class Shader
    {
        public static Shader Find(string name) => new Shader();
        public static int globalMaximumLOD { get; set; } = 1000;
    }

    /// <summary>
    /// Thành phần phân mảnh (partial) của lớp GL: Đảm nhiệm phân hệ chuyên trách Graphics ().
    /// </summary>
    public static class GL
    {
        public const int LINES = 1;
        public const int LINE_STRIP = 2;
        public const int TRIANGLES = 4;
        public const int TRIANGLE_STRIP = 5;
        public const int QUADS = 7;

        /// <summary>
        /// Thực thi phương thức nghiệp vụ PushMatrix.
        /// </summary>
        public static void PushMatrix() { }
        /// <summary>
        /// Thực thi phương thức nghiệp vụ PopMatrix.
        /// </summary>
        public static void PopMatrix() { }
        /// <summary>
        /// Thực thi phương thức nghiệp vụ Begin.
        /// </summary>
        public static void Begin(int mode) { }
        /// <summary>
        /// Thực thi phương thức nghiệp vụ End.
        /// </summary>
        public static void End() { }
        /// <summary>
        /// Thực thi phương thức nghiệp vụ Vertex.
        /// </summary>
        public static void Vertex(Vector2 v) { }
        /// <summary>
        /// Thực thi phương thức nghiệp vụ Vertex.
        /// </summary>
        public static void Vertex(Vector3 v) { }
        /// <summary>
        /// Thực thi phương thức nghiệp vụ Vertex.
        /// </summary>
        public static void Vertex(float x, float y, float z = 0f) { }
        /// <summary>
        /// Thực thi phương thức nghiệp vụ Vertex3.
        /// </summary>
        public static void Vertex3(float x, float y, float z) { }
        /// <summary>
        /// Thực thi phương thức nghiệp vụ Color.
        /// </summary>
        public static void Color(Color c) { }
        /// <summary>
        /// Khởi tạo và nạp dữ liệu cấu hình cho LoadOrtho.
        /// </summary>
        public static void LoadOrtho() { }
        /// <summary>
        /// Khởi tạo và nạp dữ liệu cấu hình cho LoadIdentity.
        /// </summary>
        public static void LoadIdentity() { }
        /// <summary>
        /// Thực thi phương thức nghiệp vụ MultMatrix.
        /// </summary>
        public static void MultMatrix(Matrix4x4 m) { }
    }
}

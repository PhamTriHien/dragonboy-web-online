using System;
using System.Numerics;
using Raylib_cs;

namespace DragonBoy_Net8_Native.Engine.Graphics
{
    /// <summary>
    /// =========================================================================================================
    /// QUẢN LÝ ĐỒ HỌA & HIỂN THỊ NATIVE RAYLIB (RENDER MANAGER)
    /// =========================================================================================================
    /// Đảm nhiệm toàn bộ vòng đời cửa sổ, Render Target ảo và xuất hình ảnh ra màn hình:
    /// 1. Quản lý cửa sổ: Khởi tạo Raylib Window, icon tùy chỉnh, căn giữa màn hình (CenterWindow), BringToFront.
    /// 2. Virtual Canvas: Vẽ toàn bộ UI/Game vào RenderTexture2D ảo với kích thước đồng bộ screen size.
    /// 3. Chế độ render: Đọc ModGraphics.renderMode mỗi khung hình (đổi từ menu Mod hoặc phím F10):
    ///    Java Gốc = shader pixel to + RGB565 + lọc Point, Bản HD = nguyên độ phân giải + lọc Bilinear.
    /// 4. Screenshot: Hỗ trợ phím tắt F12 chụp màn hình trực tiếp từ GPU Texture.
    /// =========================================================================================================
    /// </summary>
    public static class RenderManager
    {
        public static int WindowWidth = 1024;
        public static int WindowHeight = 600;
        public static string WindowTitle = "DragonBoy - Mod TriHienKun [.NET 8 Native]";

        public static int VirtualWidth = 1024;
        public static int VirtualHeight = 600;

        public static RenderTexture2D VirtualTarget;
        public static bool IsInitialized = false;
        public static int TargetFps = 240;

        private static TextureFilter? _appliedFilter;

        /// <summary>
        /// Khởi tạo và nạp dữ liệu cấu hình cho Init.
        /// </summary>
        public static void Init(int width = 1024, int height = 600, string title = "DragonBoy - Mod TriHienKun [.NET 8 Native]")
        {
            WindowWidth = width;
            WindowHeight = height;
            VirtualWidth = width;
            VirtualHeight = height;
            WindowTitle = title;

            Raylib.SetConfigFlags(ConfigFlags.ResizableWindow | ConfigFlags.VSyncHint | ConfigFlags.Msaa4xHint);
            Raylib.InitWindow(WindowWidth, WindowHeight, WindowTitle);
            Raylib.SetExitKey(KeyboardKey.Null);

            try
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string iconPath = System.IO.Path.Combine(baseDir, "custom_logo.png");
                if (!System.IO.File.Exists(iconPath))
                {
                    iconPath = System.IO.Path.Combine(baseDir, "Assets", "custom_logo.png");
                }
                if (!System.IO.File.Exists(iconPath))
                {
                    iconPath = "custom_logo.png";
                }
                if (System.IO.File.Exists(iconPath))
                {
                    Raylib_cs.Image icon = Raylib.LoadImage(iconPath);
                    if (icon.Width > 0 && icon.Height > 0)
                    {
                        Raylib.SetWindowIcon(icon);
                        Raylib.UnloadImage(icon);
                    }
                }
            }
            catch { }

            Raylib.SetWindowMinSize(640, 360);
            Raylib.SetTargetFPS(TargetFps);

            CenterWindow();
            BringToFront();

            UpdateVirtualTarget();
            InitShaders();
            IsInitialized = true;
        }

        private static Shader _javaRetroShader;
        private static bool _shaderInitialized = false;
        private static int _blockSizeLoc = -1;

        private const string JavaRetroFragShader = @"#version 330
in vec2 fragTexCoord;
in vec4 fragColor;
out vec4 finalColor;
uniform sampler2D texture0;
uniform vec4 colDiffuse;
uniform float blockSize;

void main()
{
    vec2 res = vec2(textureSize(texture0, 0));
    vec2 snapped = (floor(fragTexCoord * res / blockSize) * blockSize + 0.5) / res;
    vec4 tex = texture(texture0, snapped);
    float r = floor(tex.r * 31.0 + 0.5) / 31.0;
    float g = floor(tex.g * 63.0 + 0.5) / 63.0;
    float b = floor(tex.b * 31.0 + 0.5) / 31.0;
    finalColor = vec4(r, g, b, tex.a) * fragColor;
}
";

        /// <summary>
        /// Nạp shader Java Gốc: gom mỗi khối pixel thành một pixel to rồi lượng tử hóa màu RGB565 (5-6-5 bit).
        /// </summary>
        private static void InitShaders()
        {
            try
            {
                if (Raylib.IsWindowReady())
                {
                    _javaRetroShader = Raylib.LoadShaderFromMemory(null, JavaRetroFragShader);
                    _shaderInitialized = (_javaRetroShader.Id != 0);
                    if (_shaderInitialized)
                    {
                        _blockSizeLoc = Raylib.GetShaderLocation(_javaRetroShader, "blockSize");
                    }
                }
            }
            catch
            {
                _shaderInitialized = false;
            }
        }

        [System.Runtime.InteropServices.DllImport("user32.dll", ExactSpelling = true)]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [System.Runtime.InteropServices.DllImport("user32.dll", ExactSpelling = true)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [System.Runtime.InteropServices.DllImport("user32.dll", ExactSpelling = true)]
        private static extern bool BringWindowToTop(IntPtr hWnd);

        /// <summary>
        /// Cưỡng chế hiển thị cửa sổ game nổi lên phía trước màn hình desktop.
        /// </summary>
        public static unsafe void BringToFront()
        {
            if (!OperatingSystem.IsWindows()) return;
            try
            {
                IntPtr handle = (IntPtr)Raylib.GetWindowHandle();
                if (handle != IntPtr.Zero)
                {
                    ShowWindow(handle, 9); // SW_RESTORE
                    ShowWindow(handle, 5); // SW_SHOW
                    BringWindowToTop(handle);
                    SetForegroundWindow(handle);
                }
            }
            catch { }
        }

        /// <summary>
        /// Căn chỉnh vị trí cửa sổ game nằm chính giữa màn hình máy tính.
        /// </summary>
        public static void CenterWindow()
        {
            try
            {
                int monitor = Raylib.GetCurrentMonitor();
                int monitorWidth = Raylib.GetMonitorWidth(monitor);
                int monitorHeight = Raylib.GetMonitorHeight(monitor);
                Vector2 monitorPos = Raylib.GetMonitorPosition(monitor);
                int screenW = Raylib.GetScreenWidth();
                int screenH = Raylib.GetScreenHeight();

                int posX = (int)monitorPos.X + (monitorWidth - screenW) / 2;
                int posY = (int)monitorPos.Y + (monitorHeight - screenH) / 2;

                if (posX < (int)monitorPos.X) posX = (int)monitorPos.X;
                if (posY < (int)monitorPos.Y) posY = (int)monitorPos.Y;

                Raylib.SetWindowPosition(posX, posY);
            }
            catch { }
        }

        /// <summary>
        /// Tái tạo lại Virtual Render Target khi kích thước cửa sổ game thay đổi.
        /// </summary>
        public static void UpdateVirtualTarget()
        {
            if (VirtualTarget.Id != 0)
            {
                Raylib.UnloadRenderTexture(VirtualTarget);
            }

            int w = (VirtualWidth > 0) ? VirtualWidth : 1024;
            int h = (VirtualHeight > 0) ? VirtualHeight : 600;
            VirtualTarget = Raylib.LoadRenderTexture(w, h);
            _appliedFilter = null;
            ApplyTextureFilter();
        }

        /// <summary>
        /// Đồng bộ bộ lọc texture của Virtual Target theo chế độ render (Java Gốc: Point, Bản HD: Bilinear).
        /// Chỉ gọi xuống GPU khi bộ lọc thực sự thay đổi.
        /// </summary>
        public static void ApplyTextureFilter(int renderModeOverride = -1)
        {
            if (VirtualTarget.Id == 0) return;

            int mode = (renderModeOverride >= 0) ? renderModeOverride : ModGraphics.renderMode;
            TextureFilter filter = (mode == 0)
                ? TextureFilter.Point
                : TextureFilter.Bilinear;

            if (_appliedFilter == filter) return;

            Raylib.SetTextureFilter(VirtualTarget.Texture, filter);
            _appliedFilter = filter;
        }

        /// <summary>
        /// Bật hoặc tắt chế độ hiển thị toàn màn hình (Fullscreen).
        /// </summary>
        public static void ToggleFullscreen()
        {
            try
            {
                Raylib.ToggleFullscreen();
                if (!Raylib.IsWindowFullscreen())
                {
                    int idx = ModGraphics.resolutionIndex;
                    if (idx < 0 || idx >= ModGraphics.resolutionNames.Length) idx = 0;
                    int w = ModGraphics.resolutionList[idx, 0];
                    int h = ModGraphics.resolutionList[idx, 1];
                    Raylib.SetWindowSize(w, h);
                    CenterWindow();
                }
            }
            catch { }
        }

        /// <summary>
        /// Xử lý nghiệp vụ logic chuyên trách cho HandleInput.
        /// </summary>
        public static void HandleInput()
        {
            // F12 để chụp ảnh màn hình
            if (Raylib.IsKeyPressed(KeyboardKey.F12))
            {
                Raylib.TakeScreenshot("screenshot.png");
                Console.WriteLine("[RenderManager] Đã chụp ảnh màn hình vào screenshot.png");
            }
        }

        /// <summary>
        /// Bắt đầu chu kỳ vẽ vào Virtual Render Target ảo.
        /// </summary>
        public static void BeginVirtualRender()
        {
            UnityEngine.GUIUtility.ResetMatrixStack();

            int screenW = Raylib.GetScreenWidth();
            int screenH = Raylib.GetScreenHeight();
            if (screenW > 0 && screenH > 0 && (screenW != VirtualWidth || screenH != VirtualHeight || VirtualTarget.Id == 0))
            {
                VirtualWidth = screenW;
                VirtualHeight = screenH;
                UpdateVirtualTarget();
            }

            Raylib.BeginTextureMode(VirtualTarget);
            Raylib.ClearBackground(Raylib_cs.Color.Black);
        }

        /// <summary>
        /// Kết thúc chu kỳ vẽ và xuất khung hình từ Virtual Target ra màn hình thật theo chế độ render hiện tại:
        /// Java Gốc đi qua shader pixel to + RGB565, Bản HD hiển thị nguyên độ phân giải.
        /// </summary>
        public static void EndVirtualRender(int activeRenderState = -1)
        {
            int renderMode = (activeRenderState >= 0) ? (activeRenderState & 0xFF) : ModGraphics.renderMode;
            int zoomLevel = (activeRenderState >= 0) ? System.Math.Max(1, activeRenderState >> 8) : System.Math.Max(1, mGraphics.zoomLevel);

            Raylib.EndTextureMode();
            ApplyTextureFilter(renderMode);

            Raylib.BeginDrawing();
            Raylib.ClearBackground(Raylib_cs.Color.Black);

            int screenW = Raylib.GetScreenWidth();
            int screenH = Raylib.GetScreenHeight();

            Rectangle src = new Rectangle(0, 0, VirtualWidth, -VirtualHeight);
            Rectangle dst = new Rectangle(0, 0, screenW, screenH);

            bool useJavaShader = (renderMode == 0 && _shaderInitialized);
            if (useJavaShader)
            {
                Raylib.BeginShaderMode(_javaRetroShader);
                Raylib.SetShaderValue(_javaRetroShader, _blockSizeLoc, (float)zoomLevel, ShaderUniformDataType.Float);
            }

            Raylib.DrawTexturePro(VirtualTarget.Texture, src, dst, Vector2.Zero, 0f, Raylib_cs.Color.White);

            if (useJavaShader)
            {
                Raylib.EndShaderMode();
            }

            Raylib.EndDrawing();

            _frameCounter++;
        }

        /// <summary>
        /// Truy xuất và trả về dữ liệu của GetVirtualMousePosition.
        /// </summary>
        public static Vector2 GetVirtualMousePosition()
        {
            return Raylib.GetMousePosition();
        }

        private static int _frameCounter = 0;
        public static int FrameCount => _frameCounter;

        /// <summary>
        /// Dọn dẹp bộ nhớ đệm và giải phóng các tài nguyên đã cấp phát.
        /// </summary>
        public static void Cleanup()
        {
            if (_shaderInitialized && _javaRetroShader.Id != 0)
            {
                Raylib.UnloadShader(_javaRetroShader);
                _shaderInitialized = false;
            }
            if (VirtualTarget.Id != 0)
            {
                Raylib.UnloadRenderTexture(VirtualTarget);
            }
            if (Raylib.IsWindowReady())
            {
                Raylib.CloseWindow();
            }
            IsInitialized = false;
        }
    }
}

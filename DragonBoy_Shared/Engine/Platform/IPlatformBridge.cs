using System;

namespace DragonBoy_Net8_Native.Engine.Platform
{
    /// <summary>
    /// ENUM PlatformType: Thuộc phân hệ Tầng tương thích động cơ nền tảng Native.
    /// </summary>
    public enum PlatformType
    {
        DesktopWindows,
        Android,
        iOS
    }

    /// <summary>
    /// Giao diện giao tiếp nền tảng: Định nghĩa các chức năng đặc thù cho từng hệ điều hành (Windows, Android, iOS).
    /// </summary>
    public interface IPlatformBridge
    {
        PlatformType CurrentPlatform { get; }
        bool IsTouchDevice { get; }
        string StoragePath { get; }
        void KeepAlive(bool enable);
        void OpenUrl(string url);
        void RequestQuit();

        // Clipboard Management
        string GetClipboardText();
        void SetClipboardText(string text);

        // Safe Area Insets (Left, Top, Right, Bottom in virtual coordinates)
        (int left, int top, int right, int bottom) GetSafeAreaInsets();

        // Soft Keyboard Control
        void ShowSoftKeyboard(string initialText = "");
        void HideSoftKeyboard();

        // Display & Frame Rate Control
        int GetMaxRefreshRate();
        void SetTargetFrameRate(int fps);
        void SetResolution(int width, int height, bool fullscreen);
        void ToggleFullscreen();

        // Multi-Tab & Clone Management
        int CurrentTabId { get; }
        void SwitchToTab(int tabId);
        void CycleNextTab();
        void OpenAllTabs(int maxTabs = 0);
        void RequestPip();
        void OpenDataFolder(int tabId);
        void ClearTabData(int tabId);
    }

    /// <summary>
    /// Dịch vụ cầu nối nền tảng: Điều phối thao tác toàn màn hình, độ phân giải và clipboard trên các hệ điều hành.
    /// </summary>
    public static class PlatformService
    {
        public static IPlatformBridge Current { get; set; } = new DefaultDesktopBridge();
    }

    /// <summary>
    /// CLASS DefaultDesktopBridge: Thuộc phân hệ Tầng tương thích động cơ nền tảng Native.
    /// </summary>
    public class DefaultDesktopBridge : IPlatformBridge
    {
        public PlatformType CurrentPlatform => PlatformType.DesktopWindows;
        public bool IsTouchDevice => false;
        public string StoragePath => AppDomain.CurrentDomain.BaseDirectory;

        /// <summary>
        /// Thực thi phương thức nghiệp vụ KeepAlive.
        /// </summary>
        public void KeepAlive(bool enable) { }

        /// <summary>
        /// Thực thi phương thức nghiệp vụ OpenUrl.
        /// </summary>
        public void OpenUrl(string url)
        {
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            catch { }
        }

        /// <summary>
        /// Thực thi phương thức nghiệp vụ RequestQuit.
        /// </summary>
        public void RequestQuit()
        {
            Environment.Exit(0);
        }

        /// <summary>
        /// Truy xuất và trả về dữ liệu của GetClipboardText.
        /// </summary>
        public string GetClipboardText()
        {
            try
            {
                string txt = Raylib_cs.Raylib.GetClipboardText_();
                if (!string.IsNullOrEmpty(txt)) return txt;
            }
            catch { }

            if (OperatingSystem.IsWindows())
            {
                try { return WindowsClipboardHelper.GetText(); } catch { }
            }
            return string.Empty;
        }

        /// <summary>
        /// Thiết lập giá trị hoặc trạng thái cho SetClipboardText.
        /// </summary>
        public void SetClipboardText(string text)
        {
            if (text == null) text = string.Empty;
            try
            {
                Raylib_cs.Raylib.SetClipboardText(text);
            }
            catch { }

            if (OperatingSystem.IsWindows())
            {
                try { WindowsClipboardHelper.SetText(text); } catch { }
            }
        }

        public (int left, int top, int right, int bottom) GetSafeAreaInsets()
        {
            return (0, 0, 0, 0);
        }

        /// <summary>
        /// Thực thi phương thức nghiệp vụ ShowSoftKeyboard.
        /// </summary>
        public void ShowSoftKeyboard(string initialText = "") { }
        /// <summary>
        /// Thực thi phương thức nghiệp vụ HideSoftKeyboard.
        /// </summary>
        public void HideSoftKeyboard() { }

        /// <summary>
        /// Truy xuất và trả về dữ liệu của GetMaxRefreshRate.
        /// </summary>
        public int GetMaxRefreshRate()
        {
            try
            {
                int refresh = Raylib_cs.Raylib.GetMonitorRefreshRate(Raylib_cs.Raylib.GetCurrentMonitor());
                return refresh > 0 ? refresh : 60;
            }
            catch
            {
                return 60;
            }
        }

        /// <summary>
        /// Thiết lập giá trị hoặc trạng thái cho SetTargetFrameRate.
        /// </summary>
        public void SetTargetFrameRate(int fps)
        {
            try
            {
                Raylib_cs.Raylib.SetTargetFPS(fps);
            }
            catch { }
        }

        /// <summary>
        /// Thiết lập giá trị hoặc trạng thái cho SetResolution.
        /// </summary>
        public void SetResolution(int width, int height, bool fullscreen)
        {
            try
            {
                if (Raylib_cs.Raylib.IsWindowFullscreen() != fullscreen)
                {
                    Raylib_cs.Raylib.ToggleFullscreen();
                }
                Raylib_cs.Raylib.SetWindowSize(width, height);
            }
            catch { }
        }

        /// <summary>
        /// Bật hoặc tắt chế độ hiển thị toàn màn hình (Fullscreen).
        /// </summary>
        public void ToggleFullscreen()
        {
            try
            {
                Raylib_cs.Raylib.ToggleFullscreen();
            }
            catch { }
        }

        public int CurrentTabId => UnityEngine.Application.CurrentTabId;

        public static Action<int, bool, int, int> HostSwitchToTab { get; set; }
        public static Action<int> HostStopTab { get; set; }
        public static Action<int> HostDeleteTab { get; set; }
        public static Action<int> HostPrewarmTab { get; set; }

        public static string GetTabWindowTitle(int tabId)
        {
            return (tabId > 1)
                ? $"DragonBoy (Tab {tabId}) - Mod TriHienKun [.NET 8 Native]"
                : "DragonBoy - Mod TriHienKun [.NET 8 Native]";
        }

        public static void RequestSwitchTab(int tabId, bool closeHub)
        {
            if (tabId < 1) tabId = 1;
            GameTabRegistry.EnsureTab(tabId);

            if (tabId == UnityEngine.Application.CurrentTabId)
            {
                if (closeHub)
                {
                    ModFloatingHUD.isExpanded = false;
                    ModUI.uiCustomOpen = false;
                }
                GameScr.info1?.addInfo("Đang ở Tab " + tabId + "!", 0);
                return;
            }

            if (HostSwitchToTab != null)
            {
                bool keepHubOpen = !closeHub && ModFloatingHUD.isExpanded;
                int bx = ModFloatingHUD.bubbleX;
                int by = ModFloatingHUD.bubbleY;
                if (closeHub)
                {
                    ModFloatingHUD.isExpanded = false;
                    ModUI.uiCustomOpen = false;
                }
                HostSwitchToTab(tabId, keepHubOpen, bx, by);
                return;
            }

            GameTabManager.SwitchToTab(tabId);
        }

        /// <summary>
        /// Thực thi phương thức nghiệp vụ SwitchToTab.
        /// </summary>
        public void SwitchToTab(int tabId)
        {
            RequestSwitchTab(tabId, closeHub: true);
        }

        /// <summary>
        /// Thực thi phương thức nghiệp vụ CycleNextTab.
        /// </summary>
        public void CycleNextTab()
        {
            GameTabManager.CycleNextTab();
        }

        /// <summary>
        /// Thực thi phương thức nghiệp vụ OpenAllTabs.
        /// </summary>
        public void OpenAllTabs(int maxTabs = 0)
        {
            foreach (int id in GameTabRegistry.GetTabs()) SwitchToTab(id);
        }

        private static bool _isMiniPipMode = false;
        private static int _savedPipWidth = 1024;
        private static int _savedPipHeight = 600;

        /// <summary>
        /// Thực thi phương thức nghiệp vụ RequestPip.
        /// </summary>
        public void RequestPip()
        {
            try
            {
                _isMiniPipMode = !_isMiniPipMode;
                if (_isMiniPipMode)
                {
                    _savedPipWidth = Raylib_cs.Raylib.GetScreenWidth();
                    _savedPipHeight = Raylib_cs.Raylib.GetScreenHeight();
                    if (_savedPipWidth <= 640) _savedPipWidth = 1024;
                    if (_savedPipHeight <= 375) _savedPipHeight = 600;
                    SetResolution(640, 375, false);
                }
                else
                {
                    SetResolution(_savedPipWidth, _savedPipHeight, false);
                }
            }
            catch { }
        }

        /// <summary>
        /// Thực thi phương thức nghiệp vụ OpenDataFolder.
        /// </summary>
        public void OpenDataFolder(int tabId)
        {
            try
            {
                string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                string unityPath = System.IO.Path.Combine(userProfile, "AppData", "LocalLow", "Team", "DragonBoy250");
                string root = System.IO.Directory.Exists(unityPath)
                    ? System.IO.Path.Combine(unityPath, $"tab{tabId}")
                    : System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "rms", $"tab{tabId}");

                if (!System.IO.Directory.Exists(root))
                {
                    System.IO.Directory.CreateDirectory(root);
                }
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = root,
                    UseShellExecute = true
                });
            }
            catch { }
        }

        /// <summary>
        /// Dọn dẹp và đặt lại trạng thái ban đầu cho ClearTabData.
        /// </summary>
        public void ClearTabData(int tabId)
        {
            try
            {
                GameTabManager.RequestStopTab(tabId);
                string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                string unityPath = System.IO.Path.Combine(userProfile, "AppData", "LocalLow", "Team", "DragonBoy250");
                string root = System.IO.Directory.Exists(unityPath)
                    ? System.IO.Path.Combine(unityPath, $"tab{tabId}")
                    : System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "rms", $"tab{tabId}");

                if (System.IO.Directory.Exists(root))
                {
                    System.IO.Directory.Delete(root, true);
                    System.IO.Directory.CreateDirectory(root);
                }
                GameScr.info1?.addInfo("Đã reset dữ liệu Tab " + tabId + "!", 0);
            }
            catch { }
        }

        /// <summary>
        /// CLASS WindowsClipboardHelper: Thuộc phân hệ Tầng tương thích động cơ nền tảng Native.
        /// </summary>
        private static class WindowsClipboardHelper
        {
            [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
            private static extern bool OpenClipboard(IntPtr hWndNewOwner);

            [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
            private static extern bool CloseClipboard();

            [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
            private static extern IntPtr GetClipboardData(uint uFormat);

            [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
            private static extern bool SetClipboardData(uint uFormat, IntPtr hMem);

            [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
            private static extern bool EmptyClipboard();

            [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
            private static extern IntPtr GlobalLock(IntPtr hMem);

            [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
            private static extern bool GlobalUnlock(IntPtr hMem);

            [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
            private static extern IntPtr GlobalAlloc(uint uFlags, UIntPtr dwBytes);

            private const uint CF_UNICODETEXT = 13;
            private const uint GMEM_MOVEABLE = 0x0002;

            /// <summary>
            /// Truy xuất và trả về dữ liệu của GetText.
            /// </summary>
            public static string GetText()
            {
                for (int attempt = 0; attempt < 5; attempt++)
                {
                    if (OpenClipboard(IntPtr.Zero))
                    {
                        try
                        {
                            IntPtr handle = GetClipboardData(CF_UNICODETEXT);
                            if (handle != IntPtr.Zero)
                            {
                                IntPtr ptr = GlobalLock(handle);
                                if (ptr != IntPtr.Zero)
                                {
                                    try
                                    {
                                        return System.Runtime.InteropServices.Marshal.PtrToStringUni(ptr) ?? string.Empty;
                                    }
                                    finally
                                    {
                                        GlobalUnlock(handle);
                                    }
                                }
                            }
                        }
                        finally
                        {
                            CloseClipboard();
                        }
                        break;
                    }
                    System.Threading.Thread.Sleep(10);
                }
                return string.Empty;
            }

            /// <summary>
            /// Thiết lập giá trị hoặc trạng thái cho SetText.
            /// </summary>
            public static void SetText(string text)
            {
                if (text == null) text = string.Empty;
                for (int attempt = 0; attempt < 5; attempt++)
                {
                    if (OpenClipboard(IntPtr.Zero))
                    {
                        try
                        {
                            EmptyClipboard();
                            int bytes = (text.Length + 1) * 2;
                            IntPtr hMem = GlobalAlloc(GMEM_MOVEABLE, (UIntPtr)bytes);
                            if (hMem != IntPtr.Zero)
                            {
                                IntPtr ptr = GlobalLock(hMem);
                                if (ptr != IntPtr.Zero)
                                {
                                    try
                                    {
                                        System.Runtime.InteropServices.Marshal.Copy(text.ToCharArray(), 0, ptr, text.Length);
                                        System.Runtime.InteropServices.Marshal.WriteInt16(ptr, text.Length * 2, 0);
                                    }
                                    finally
                                    {
                                        GlobalUnlock(hMem);
                                    }
                                    SetClipboardData(CF_UNICODETEXT, hMem);
                                }
                            }
                        }
                        finally
                        {
                            CloseClipboard();
                        }
                        break;
                    }
                    System.Threading.Thread.Sleep(10);
                }
            }
        }
    }
}

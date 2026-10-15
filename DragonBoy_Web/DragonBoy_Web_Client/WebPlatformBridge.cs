using System;
using System.IO;
using DragonBoy_Net8_Native.Engine.Platform;
using UnityEngine;

namespace DragonBoy_Web_Host
{
    /// <summary>
    /// Triển khai cầu nối nền tảng IPlatformBridge cho môi trường trình duyệt WebAssembly.
    /// </summary>
    public sealed class WebPlatformBridge : IPlatformBridge
    {
        public PlatformType CurrentPlatform => WebInterop.IsTouchDevice() ? PlatformType.Android : PlatformType.DesktopWindows;
        public bool IsTouchDevice => WebInterop.IsTouchDevice();
        public string StoragePath => "/rms";

        /// <summary>
        /// Duy trì trạng thái hoạt động nền cho tab trình duyệt.
        /// </summary>
        public void KeepAlive(bool enable) { }

        /// <summary>
        /// Mở liên kết URL trong tab trình duyệt mới.
        /// </summary>
        public void OpenUrl(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                WebInterop.OpenUrl(url);
            }
        }

        /// <summary>
        /// Đánh dấu dừng phiên hoạt động của tab hiện tại.
        /// </summary>
        public void RequestQuit()
        {
            GameTabStatus.MarkTabStopped(Application.CurrentTabId);
        }

        /// <summary>
        /// Đọc văn bản hiện tại từ bộ nhớ tạm (Clipboard).
        /// </summary>
        public string GetClipboardText()
        {
            return WebInterop.GetClipboardText() ?? string.Empty;
        }

        /// <summary>
        /// Ghi văn bản vào bộ nhớ tạm (Clipboard) của trình duyệt.
        /// </summary>
        public void SetClipboardText(string text)
        {
            WebInterop.SetClipboardText(text ?? string.Empty);
        }

        /// <summary>
        /// Truy xuất vùng lề an toàn của màn hình.
        /// </summary>
        public (int left, int top, int right, int bottom) GetSafeAreaInsets()
        {
            return (0, 0, 0, 0);
        }

        /// <summary>
        /// Hiển thị hộp thoại bàn phím ảo cho thiết bị cảm ứng trên trình duyệt.
        /// </summary>
        public void ShowSoftKeyboard(string initialText = "")
        {
            WebInterop.ShowSoftKeyboard(initialText ?? string.Empty);
        }

        /// <summary>
        /// Ẩn hộp thoại bàn phím ảo trên trình duyệt.
        /// </summary>
        public void HideSoftKeyboard()
        {
            WebInterop.HideSoftKeyboard();
        }

        /// <summary>
        /// Trả về tần số làm tươi mặc định của trình duyệt.
        /// </summary>
        public int GetMaxRefreshRate()
        {
            return 60;
        }

        /// <summary>
        /// Thiết lập mức FPS mục tiêu cho bộ kết xuất Web.
        /// </summary>
        public void SetTargetFrameRate(int fps)
        {
            WebGraphicsBackend.TargetFrameRate = fps;
        }

        /// <summary>
        /// Cập nhật độ phân giải khung vẽ và trạng thái toàn màn hình.
        /// </summary>
        public void SetResolution(int width, int height, bool fullscreen)
        {
            WebGraphicsBackend.SetResolution(width, height, fullscreen);
        }

        /// <summary>
        /// Chuyển đổi trạng thái toàn màn hình (Fullscreen API) trên trình duyệt.
        /// </summary>
        public void ToggleFullscreen()
        {
            bool next = !WebGraphicsBackend.IsFullScreen;
            WebGraphicsBackend.SetResolution(WebGraphicsBackend.ScreenWidth, WebGraphicsBackend.ScreenHeight, next);
        }

        public int CurrentTabId => Application.CurrentTabId;

        /// <summary>
        /// Chuyển sang tab game chỉ định ngay trong giao diện trình duyệt.
        /// </summary>
        public void SwitchToTab(int tabId)
        {
            DefaultDesktopBridge.RequestSwitchTab(tabId, closeHub: true);
        }

        /// <summary>
        /// Chuyển tuần tự sang tab kế tiếp.
        /// </summary>
        public void CycleNextTab()
        {
            GameTabManager.CycleNextTab();
        }

        /// <summary>
        /// Khởi tạo sẵn các iframe tab phụ trên trình duyệt.
        /// </summary>
        public void OpenAllTabs(int maxTabs = 0)
        {
            foreach (int id in GameTabRegistry.GetTabs())
            {
                if (id > 1) WebInterop.HostPrewarmTab(id);
            }
        }

        /// <summary>
        /// Chuyển đổi chế độ hiển thị toàn màn hình.
        /// </summary>
        public void RequestPip()
        {
            ToggleFullscreen();
        }

        /// <summary>
        /// Hiển thị thông báo vị trí lưu trữ dữ liệu của tab trên trình duyệt.
        /// </summary>
        public void OpenDataFolder(int tabId)
        {
            GameScr.info1?.addInfo("Dữ liệu Tab " + tabId + " được lưu an toàn trên trình duyệt (IndexedDB/LocalStorage)!", 0);
        }

        /// <summary>
        /// Dừng tab và xóa toàn bộ dữ liệu RMS của tab chỉ định khỏi VFS và bộ nhớ trình duyệt.
        /// </summary>
        public void ClearTabData(int tabId)
        {
            try
            {
                GameTabManager.RequestStopTab(tabId);
                string root = $"/rms/tab{tabId}";
                if (Directory.Exists(root))
                {
                    foreach (string f in Directory.GetFiles(root))
                    {
                        File.Delete(f);
                        WebStorageSync.OnFileDeleted(f);
                    }
                }
                GameScr.info1?.addInfo("Đã xóa dữ liệu Tab " + tabId + " trên trình duyệt!", 0);
            }
            catch { }
        }
    }
}

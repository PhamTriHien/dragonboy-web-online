using System;
using System.IO;
using DragonBoy_Net8_Native.Engine.Platform;
using UnityEngine;

namespace DragonBoy_Web_Host
{
    public sealed class WebPlatformBridge : IPlatformBridge
    {
        public PlatformType CurrentPlatform => WebInterop.IsTouchDevice() ? PlatformType.Android : PlatformType.DesktopWindows;
        public bool IsTouchDevice => WebInterop.IsTouchDevice();
        public string StoragePath => "/rms";

        public void KeepAlive(bool enable) { }

        public void OpenUrl(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                WebInterop.OpenUrl(url);
            }
        }

        public void RequestQuit()
        {
            GameTabStatus.MarkTabStopped(Application.CurrentTabId);
        }

        public string GetClipboardText()
        {
            return WebInterop.GetClipboardText() ?? string.Empty;
        }

        public void SetClipboardText(string text)
        {
            WebInterop.SetClipboardText(text ?? string.Empty);
        }

        public (int left, int top, int right, int bottom) GetSafeAreaInsets()
        {
            return (0, 0, 0, 0);
        }

        public void ShowSoftKeyboard(string initialText = "")
        {
            WebInterop.ShowSoftKeyboard(initialText ?? string.Empty);
        }

        public void HideSoftKeyboard()
        {
            WebInterop.HideSoftKeyboard();
        }

        public int GetMaxRefreshRate()
        {
            return 60;
        }

        public void SetTargetFrameRate(int fps)
        {
            WebGraphicsBackend.TargetFrameRate = fps;
        }

        public void SetResolution(int width, int height, bool fullscreen)
        {
            WebGraphicsBackend.SetResolution(width, height, fullscreen);
        }

        public void ToggleFullscreen()
        {
            bool next = !WebGraphicsBackend.IsFullScreen;
            WebGraphicsBackend.SetResolution(WebGraphicsBackend.ScreenWidth, WebGraphicsBackend.ScreenHeight, next);
        }

        public int CurrentTabId => Application.CurrentTabId;

        public void SwitchToTab(int tabId)
        {
            DefaultDesktopBridge.RequestSwitchTab(tabId, closeHub: true);
        }

        public void CycleNextTab()
        {
            GameTabManager.CycleNextTab();
        }

        public void OpenAllTabs(int maxTabs = 0)
        {
            foreach (int id in GameTabRegistry.GetTabs())
            {
                if (id > 1) WebInterop.HostPrewarmTab(id);
            }
        }

        public void RequestPip()
        {
            ToggleFullscreen();
        }

        public void OpenDataFolder(int tabId)
        {
            GameScr.info1?.addInfo("Dữ liệu Tab " + tabId + " được lưu an toàn trên trình duyệt (IndexedDB/LocalStorage)!", 0);
        }

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

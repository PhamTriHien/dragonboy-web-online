using System;
using System.IO;
using System.Text;

namespace DragonBoy_Web_Host
{
    /// <summary>
    /// Đồng bộ hệ thống tệp ảo (VFS) của WebAssembly với IndexedDB và localStorage trên trình duyệt.
    /// </summary>
    public static class WebStorageSync
    {
        private static bool _isHydrating;
        public static bool IsBroadcastActive;

        /// <summary>
        /// Khôi phục toàn bộ dữ liệu RMS và cấu hình đã lưu từ bộ nhớ trình duyệt vào VFS khi khởi động.
        /// </summary>
        public static void HydrateFromBrowserStorage()
        {
            _isHydrating = true;
            try
            {
                Directory.CreateDirectory("/rms");
                Directory.CreateDirectory("/rms/tab1");

                string payload = WebInterop.GetAllPersistedFilesPayload();
                ApplyPayloadToVfs(payload);
            }
            catch (Exception ex)
            {
                Console.WriteLine("[WebStorageSync] Hydrate error: " + ex.Message);
            }
            finally
            {
                _isHydrating = false;
            }
        }

        /// <summary>
        /// Áp dụng tức thì 1 tệp dùng chung vừa nhận qua BroadcastChannel vào VFS mà không ghi ngược lại JS.
        /// </summary>
        public static void ApplyExternalUpdate(string encodedLine)
        {
            if (string.IsNullOrEmpty(encodedLine)) return;
            IsBroadcastActive = true;
            bool prev = _isHydrating;
            _isHydrating = true;
            try
            {
                ApplyEncodedLineToVfs(encodedLine);
            }
            catch { }
            finally
            {
                _isHydrating = prev;
            }
        }

        /// <summary>
        /// Xóa tức thì 1 tệp dùng chung vừa nhận sự kiện xóa qua BroadcastChannel khỏi VFS.
        /// </summary>
        public static void ApplyExternalDelete(string vfsPath)
        {
            if (string.IsNullOrEmpty(vfsPath)) return;
            IsBroadcastActive = true;
            bool prev = _isHydrating;
            _isHydrating = true;
            try
            {
                string norm = NormalizeVfsPath(vfsPath);
                if (File.Exists(norm))
                {
                    File.Delete(norm);
                }
            }
            catch { }
            finally
            {
                _isHydrating = prev;
            }
        }

        /// <summary>
        /// Làm mới các tệp trạng thái đa tab dùng chung từ localStorage.
        /// </summary>
        public static void RefreshSharedFilesFromLocalStorage()
        {
            if (_isHydrating) return;
            _isHydrating = true;
            try
            {
                string payload = WebInterop.GetSharedFilesPayload();
                ApplyPayloadToVfs(payload);
            }
            catch { }
            finally
            {
                _isHydrating = false;
            }
        }

        /// <summary>
        /// Giải mã chuỗi dữ liệu lưu trữ và ghi vào hệ thống tệp ảo trong bộ nhớ.
        /// </summary>
        private static void ApplyPayloadToVfs(string payload)
        {
            if (string.IsNullOrEmpty(payload)) return;
            string[] lines = payload.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < lines.Length; i++)
            {
                ApplyEncodedLineToVfs(lines[i]);
            }
        }

        private static void ApplyEncodedLineToVfs(string line)
        {
            if (string.IsNullOrEmpty(line)) return;
            int sep1 = line.IndexOf('|');
            if (sep1 <= 0) return;
            int sep2 = line.IndexOf('|', sep1 + 1);
            if (sep2 <= sep1) return;

            string mode = line.Substring(0, sep1);
            string vfsPath = NormalizeVfsPath(line.Substring(sep1 + 1, sep2 - sep1 - 1));
            string dataPart = line.Substring(sep2 + 1);

            try
            {
                string dir = Path.GetDirectoryName(vfsPath);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                if (mode == "B64")
                {
                    byte[] bytes = Convert.FromBase64String(dataPart);
                    File.WriteAllBytes(vfsPath, bytes);
                }
                else if (mode == "TXT")
                {
                    byte[] bytes = Convert.FromBase64String(dataPart);
                    File.WriteAllText(vfsPath, Encoding.UTF8.GetString(bytes));
                }
            }
            catch { }
        }

        /// <summary>
        /// Đồng bộ tệp nhị phân vừa ghi trong VFS xuống bộ nhớ trình duyệt.
        /// </summary>
        public static void OnFileSaved(string vfsPath, byte[] data)
        {
            if (_isHydrating || string.IsNullOrEmpty(vfsPath) || data == null) return;
            try
            {
                string norm = NormalizeVfsPath(vfsPath);
                WebInterop.SaveStorageFile(norm, Convert.ToBase64String(data));
            }
            catch { }
        }

        /// <summary>
        /// Đồng bộ tệp văn bản vừa ghi trong VFS xuống bộ nhớ trình duyệt.
        /// </summary>
        public static void OnTextFileSaved(string vfsPath, string content)
        {
            if (_isHydrating || string.IsNullOrEmpty(vfsPath) || content == null) return;
            try
            {
                string norm = NormalizeVfsPath(vfsPath);
                WebInterop.SaveStorageText(norm, content);
            }
            catch { }
        }

        /// <summary>
        /// Xóa tệp tương ứng khỏi bộ nhớ trình duyệt khi tệp trong VFS bị xóa.
        /// </summary>
        public static void OnFileDeleted(string vfsPath)
        {
            if (_isHydrating || string.IsNullOrEmpty(vfsPath)) return;
            try
            {
                string norm = NormalizeVfsPath(vfsPath);
                WebInterop.DeleteStorageFile(norm);
            }
            catch { }
        }

        /// <summary>
        /// Chuẩn hóa đường dẫn tệp trong VFS về định dạng dấu gạch chéo chuẩn.
        /// </summary>
        private static string NormalizeVfsPath(string path)
        {
            if (string.IsNullOrEmpty(path)) return "/rms";
            string norm = path.Replace('\\', '/');
            if (!norm.StartsWith('/')) norm = "/" + norm;
            while (norm.Contains("//")) norm = norm.Replace("//", "/");
            return norm;
        }
    }
}

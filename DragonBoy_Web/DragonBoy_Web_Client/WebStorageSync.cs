using System;
using System.IO;
using System.Text;

namespace DragonBoy_Web_Host
{
    public static class WebStorageSync
    {
        private static bool _isHydrating;

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

        private static void ApplyPayloadToVfs(string payload)
        {
            if (string.IsNullOrEmpty(payload)) return;
            string[] lines = payload.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                int sep1 = line.IndexOf('|');
                if (sep1 <= 0) continue;
                int sep2 = line.IndexOf('|', sep1 + 1);
                if (sep2 <= sep1) continue;

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
        }

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

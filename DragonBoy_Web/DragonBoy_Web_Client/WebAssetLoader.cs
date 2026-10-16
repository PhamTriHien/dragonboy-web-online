using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using UnityEngine;

namespace DragonBoy_Web_Host
{
    /// <summary>
    /// Quản lý nạp tài nguyên đồ họa, phông chữ và dữ liệu nhị phân cho phiên bản trình duyệt WebAssembly.
    /// </summary>
    public static class WebAssetLoader
    {
        private static readonly HttpClient _http = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
        private static readonly Dictionary<string, (int width, int height)> _imageDimensions = new Dictionary<string, (int, int)>(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<string, byte[]> _binaryAssets = new Dictionary<string, byte[]>(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<string, Texture2D> _textureCache = new Dictionary<string, Texture2D>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Tải và giải nén gói tài nguyên khởi động (/api/startup-pack) vào bộ nhớ đệm và hệ thống tệp ảo VFS.
        /// </summary>
        public static async System.Threading.Tasks.Task InitializeStartupPackAsync()
        {
            string baseUrl = WebInterop.GetHttpBaseUrl().TrimEnd('/');
            _http.BaseAddress = new Uri(baseUrl + "/");

            try
            {
                WebInterop.SetLoadingProgress("Đang đồng bộ danh mục tài nguyên đồ họa...", 25);
                byte[] packBytes = await _http.GetByteArrayAsync("api/startup-pack");
                using (var ms = new MemoryStream(packBytes))
                using (var br = new BinaryReader(ms))
                {
                    int imgCount = br.ReadInt32();
                    for (int i = 0; i < imgCount; i++)
                    {
                        string relPath = br.ReadString();
                        int w = br.ReadInt16();
                        int h = br.ReadInt16();
                        _imageDimensions[relPath] = (w, h);
                    }

                    int fileCount = br.ReadInt32();
                    for (int i = 0; i < fileCount; i++)
                    {
                        string vfsRelPath = br.ReadString();
                        int len = br.ReadInt32();
                        byte[] bytes = br.ReadBytes(len);
                        _binaryAssets[vfsRelPath] = bytes;

                        string vfsFullPath = "/Assets/" + vfsRelPath;
                        string dir = Path.GetDirectoryName(vfsFullPath);
                        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                        {
                            Directory.CreateDirectory(dir);
                        }
                        File.WriteAllBytes(vfsFullPath, bytes);
                        if (vfsRelPath.Equals("custom_logo.png", StringComparison.OrdinalIgnoreCase))
                        {
                            File.WriteAllBytes("/custom_logo.png", bytes);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[WebAssetLoader] Startup pack error: " + ex.Message);
            }
        }

        /// <summary>
        /// Nạp đối tượng tài nguyên (Texture2D, TextAsset, Font) với cơ chế fallback đa mức zoom (x2 -> x1 -> x3 -> x4).
        /// </summary>
        public static object Load(string cleanPath, string withoutRes, Type type)
        {
            if (string.IsNullOrEmpty(cleanPath)) return null;

            if (type == typeof(Font) ||
                cleanPath.StartsWith("FontSys/", StringComparison.OrdinalIgnoreCase) ||
                cleanPath.EndsWith(".ttf", StringComparison.OrdinalIgnoreCase) ||
                cleanPath.EndsWith(".otf", StringComparison.OrdinalIgnoreCase))
            {
                return new Font { name = Path.GetFileNameWithoutExtension(cleanPath) };
            }

            string baseClean = cleanPath.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
                ? cleanPath.Substring(0, cleanPath.Length - 4)
                : cleanPath;
            string baseNoRes = withoutRes.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
                ? withoutRes.Substring(0, withoutRes.Length - 4)
                : withoutRes;
            string withoutZoom = baseNoRes;
            if (withoutZoom.StartsWith("x1/", StringComparison.OrdinalIgnoreCase) ||
                withoutZoom.StartsWith("x2/", StringComparison.OrdinalIgnoreCase) ||
                withoutZoom.StartsWith("x3/", StringComparison.OrdinalIgnoreCase) ||
                withoutZoom.StartsWith("x4/", StringComparison.OrdinalIgnoreCase))
            {
                withoutZoom = withoutZoom.Substring(3);
            }

            if (type == typeof(TextAsset) || cleanPath.StartsWith("data/", StringComparison.OrdinalIgnoreCase))
            {
                string[] textCandidates =
                {
                    cleanPath,
                    withoutRes,
                    withoutZoom,
                    "x2/" + withoutZoom,
                    "x1/" + withoutZoom,
                    "x3/" + withoutZoom,
                    "x4/" + withoutZoom,
                    "data/" + cleanPath,
                    "data/" + withoutRes,
                    "data/" + withoutZoom
                };

                for (int i = 0; i < textCandidates.Length; i++)
                {
                    string tc = textCandidates[i];
                    if (_binaryAssets.TryGetValue(tc, out byte[] dataBytes))
                    {
                        return new TextAsset(dataBytes);
                    }
                    if (_binaryAssets.TryGetValue(tc + ".bytes", out byte[] dataBytesExt))
                    {
                        return new TextAsset(dataBytesExt);
                    }
                    string vfsCandidate = "/Assets/" + tc;
                    if (File.Exists(vfsCandidate))
                    {
                        return new TextAsset(File.ReadAllBytes(vfsCandidate));
                    }
                }
                return null;
            }

            string cacheKey = cleanPath + "@" + mGraphics.zoomLevel;
            if (_textureCache.TryGetValue(cacheKey, out Texture2D cachedTex))
            {
                return cachedTex;
            }

            string preferredZoom = "x" + mGraphics.zoomLevel + "/";
            string[] candidates =
            {
                preferredZoom + withoutZoom + ".png",
                baseNoRes + ".png",
                baseClean + ".png",
                "res/" + baseNoRes + ".png",
                "x2/" + withoutZoom + ".png",
                "x1/" + withoutZoom + ".png",
                "x3/" + withoutZoom + ".png",
                "x4/" + withoutZoom + ".png",
                withoutZoom + ".png"
            };

            for (int i = 0; i < candidates.Length; i++)
            {
                string key = candidates[i];
                if (_imageDimensions.TryGetValue(key, out var dims))
                {
                    int targetW = dims.width;
                    int targetH = dims.height;
                    if (mGraphics.zoomLevel == 2)
                    {
                        if (key.StartsWith("x1/", StringComparison.OrdinalIgnoreCase))
                        {
                            targetW = dims.width * 2;
                            targetH = dims.height * 2;
                        }
                        else if (key.StartsWith("x3/", StringComparison.OrdinalIgnoreCase))
                        {
                            targetW = System.Math.Max(1, (dims.width * 2 + 1) / 3);
                            targetH = System.Math.Max(1, (dims.height * 2 + 1) / 3);
                        }
                        else if (key.StartsWith("x4/", StringComparison.OrdinalIgnoreCase))
                        {
                            targetW = System.Math.Max(1, (dims.width + 1) / 2);
                            targetH = System.Math.Max(1, (dims.height + 1) / 2);
                        }
                    }

                    int texId = WebGraphicsBackend.RegisterLazyTexture("/game-assets/" + key, targetW, targetH);
                    var tex = new Texture2D(targetW, targetH, allocatePixelBuffer: false)
                    {
                        WebTextureId = texId
                    };
                    _textureCache[cacheKey] = tex;
                    return tex;
                }
            }

            if (_binaryAssets.TryGetValue(cleanPath, out byte[] rawBytes))
            {
                return new TextAsset(rawBytes);
            }

            return null;
        }

        /// <summary>
        /// Thực hiện yêu cầu HTTP bất đồng bộ phục vụ lớp tương thích WWW trên trình duyệt.
        /// </summary>
        public static void FetchWwwAsync(string url, Action<byte[], string, string> onComplete)
        {
            _ = FetchWwwInternalAsync(url, onComplete);
        }

        /// <summary>
        /// Tải nội dung từ URL qua cổng proxy CORS và trả kết quả về callback.
        /// </summary>
        private static async System.Threading.Tasks.Task FetchWwwInternalAsync(string url, Action<byte[], string, string> onComplete)
        {
            try
            {
                string targetUrl = url;
                if (url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                    url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    targetUrl = "api/proxy?url=" + Uri.EscapeDataString(url);
                }
                byte[] bytes = await _http.GetByteArrayAsync(targetUrl);
                string text = Encoding.UTF8.GetString(bytes);
                onComplete?.Invoke(bytes, text, null);
            }
            catch (Exception ex)
            {
                onComplete?.Invoke(Array.Empty<byte>(), string.Empty, ex.Message);
            }
        }

        /// <summary>
        /// Tải tệp từ URL về hệ thống tệp ảo VFS và đồng bộ xuống bộ nhớ trình duyệt kèm tiến trình phần trăm.
        /// </summary>
        public static async System.Threading.Tasks.Task DownloadFileToVfsAsync(string url, string targetVfsPath, Action<int> onProgress, Action<bool> onComplete)
        {
            try
            {
                string targetUrl = url;
                if (url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                    url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    targetUrl = "api/proxy?url=" + Uri.EscapeDataString(url);
                }

                using (HttpResponseMessage resp = await _http.GetAsync(targetUrl, HttpCompletionOption.ResponseHeadersRead))
                {
                    resp.EnsureSuccessStatusCode();
                    long total = resp.Content.Headers.ContentLength ?? 0;
                    using (Stream stream = await resp.Content.ReadAsStreamAsync())
                    using (MemoryStream ms = new MemoryStream())
                    {
                        byte[] buf = new byte[16384];
                        long downloaded = 0;
                        int read;
                        while ((read = await stream.ReadAsync(buf, 0, buf.Length)) > 0)
                        {
                            ms.Write(buf, 0, read);
                            downloaded += read;
                            if (total > 0)
                            {
                                onProgress?.Invoke(System.Math.Clamp((int)((downloaded * 100) / total), 0, 100));
                            }
                        }

                        byte[] finalBytes = ms.ToArray();
                        string dir = Path.GetDirectoryName(targetVfsPath);
                        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                        {
                            Directory.CreateDirectory(dir);
                        }
                        File.WriteAllBytes(targetVfsPath, finalBytes);
                        WebStorageSync.OnFileSaved(targetVfsPath, finalBytes);
                        onProgress?.Invoke(100);
                        onComplete?.Invoke(true);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[WebAssetLoader] DownloadFileToVfsAsync failed: " + ex.Message);
                onComplete?.Invoke(false);
            }
        }
    }
}

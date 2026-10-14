using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using UnityEngine;

namespace DragonBoy_Web_Host
{
    public static class WebAssetLoader
    {
        private static readonly HttpClient _http = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
        private static readonly Dictionary<string, (int width, int height)> _imageDimensions = new Dictionary<string, (int, int)>(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<string, byte[]> _binaryAssets = new Dictionary<string, byte[]>(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<string, Texture2D> _textureCache = new Dictionary<string, Texture2D>(StringComparer.OrdinalIgnoreCase);

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

        public static object Load(string cleanPath, string withoutRes, Type type)
        {
            if (string.IsNullOrEmpty(cleanPath)) return null;

            if (type == typeof(Font))
            {
                return new Font { name = Path.GetFileNameWithoutExtension(cleanPath) };
            }

            if (_textureCache.TryGetValue(cleanPath, out Texture2D cachedTex))
            {
                return cachedTex;
            }

            if (type == typeof(TextAsset) || cleanPath.StartsWith("data/", StringComparison.OrdinalIgnoreCase))
            {
                if (_binaryAssets.TryGetValue(cleanPath, out byte[] dataBytes))
                {
                    return new TextAsset(dataBytes);
                }
                if (_binaryAssets.TryGetValue(withoutRes, out byte[] dataBytes2))
                {
                    return new TextAsset(dataBytes2);
                }
                string vfsCandidate = "/Assets/" + cleanPath;
                if (File.Exists(vfsCandidate))
                {
                    return new TextAsset(File.ReadAllBytes(vfsCandidate));
                }
            }

            string[] candidates =
            {
                cleanPath.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ? cleanPath : cleanPath + ".png",
                withoutRes.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ? withoutRes : withoutRes + ".png",
                "res/" + (withoutRes.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ? withoutRes : withoutRes + ".png")
            };

            for (int i = 0; i < candidates.Length; i++)
            {
                string key = candidates[i];
                if (_imageDimensions.TryGetValue(key, out var dims))
                {
                    int texId = WebGraphicsBackend.RegisterLazyTexture("/game-assets/" + key, dims.width, dims.height);
                    var tex = new Texture2D(dims.width, dims.height, allocatePixelBuffer: false)
                    {
                        WebTextureId = texId
                    };
                    _textureCache[cleanPath] = tex;
                    return tex;
                }
            }

            if (_binaryAssets.TryGetValue(cleanPath, out byte[] rawBytes))
            {
                return new TextAsset(rawBytes);
            }

            return null;
        }

        public static void FetchWwwAsync(string url, Action<byte[], string, string> onComplete)
        {
            _ = FetchWwwInternalAsync(url, onComplete);
        }

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

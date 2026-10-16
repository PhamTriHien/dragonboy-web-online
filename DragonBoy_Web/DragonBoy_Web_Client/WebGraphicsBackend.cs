using System;
using System.Collections.Generic;
using UnityEngine;

namespace DragonBoy_Web_Host
{
    /// <summary>
    /// Hạ tầng kết xuất đồ họa 2D chuyển tiếp các lệnh vẽ của Engine sang HTML5 Canvas 2D qua WebInterop.
    /// </summary>
    public static class WebGraphicsBackend
    {
        private static int _nextTextureId = 1;
        private static int _targetFrameRate = 60;
        private static int _virtualW = 1024;
        private static int _virtualH = 600;
        private static readonly Stack<Vector2> _clipOffsetStack = new Stack<Vector2>();
        private static Vector2 _currentOffset = Vector2.zero;
        private static readonly Dictionary<(int theme, int size, string text), float> _textWidthCache = new Dictionary<(int, int, string), float>(2048);

        public static int ScreenWidth => _virtualW > 0 ? _virtualW : 1024;
        public static int ScreenHeight => _virtualH > 0 ? _virtualH : 600;
        public static bool IsFullScreen => WebInterop.IsFullScreen();

        public static int TargetFrameRate
        {
            get => _targetFrameRate;
            set => _targetFrameRate = value > 0 ? value : 60;
        }

        /// <summary>
        /// Thiết lập độ phân giải khung vẽ và trạng thái toàn màn hình.
        /// </summary>
        public static void SetResolution(int width, int height, bool fullscreen)
        {
            OnSetResolution(width, height, fullscreen);
        }

        /// <summary>
        /// Cập nhật kích thước khung vẽ ảo và đồng bộ với HTML5 Canvas.
        /// </summary>
        public static void OnSetResolution(int width, int height, bool fullscreen)
        {
            WebInterop.SetResolution(width, height, fullscreen);
            UpdateDynamicViewport();
        }

        /// <summary>
        /// Đồng bộ kích thước khung vẽ theo kích thước hiển thị thực tế của cửa sổ trình duyệt.
        /// </summary>
        public static void UpdateDynamicViewport()
        {
            int cw = WebInterop.GetCanvasWidth();
            int ch = WebInterop.GetCanvasHeight();
            if (cw > 0 && ch > 0 && (_virtualW != cw || _virtualH != ch))
            {
                _virtualW = cw;
                _virtualH = ch;
                Screen.customWidth = cw;
                Screen.customHeight = ch;
            }
        }

        /// <summary>
        /// Khởi đầu khung hình vẽ mới trên HTML5 Canvas.
        /// </summary>
        public static void BeginFrame()
        {
            _clipOffsetStack.Clear();
            _currentOffset = Vector2.zero;
            WebInterop.BeginFrame(_virtualW, _virtualH, ModGraphics.renderMode);
        }

        /// <summary>
        /// Kết thúc khung hình vẽ và khôi phục trạng thái ngữ cảnh Canvas.
        /// </summary>
        public static void EndFrame()
        {
            WebInterop.EndFrame();
        }

        /// <summary>
        /// Đăng ký texture tải bất đồng bộ theo URL với kích thước logic xác định trước.
        /// </summary>
        public static int RegisterLazyTexture(string url, int width, int height)
        {
            int texId = _nextTextureId++;
            WebInterop.RegisterLazyTexture(texId, url, width, height);
            return texId;
        }

        /// <summary>
        /// Tạo texture trên trình duyệt từ mảng byte PNG/JPEG và trích xuất kích thước ảnh.
        /// </summary>
        public static bool LoadImageFromBytes(Texture2D tex, byte[] data)
        {
            if (tex == null || data == null || data.Length < 8) return false;
            ParseImageDimensions(data, out int w, out int h);
            int texId = _nextTextureId++;
            WebInterop.CreateTextureFromBytes(texId, data, w, h);
            tex.width = w;
            tex.height = h;
            tex.WebTextureId = texId;
            return true;
        }

        /// <summary>
        /// Kiểm tra xem texture trên trình duyệt đã tải hoàn tất dữ liệu hình ảnh hay chưa.
        /// </summary>
        public static bool IsTextureLoaded(Texture2D tex)
        {
            if (tex == null || tex.WebTextureId <= 0) return false;
            return WebInterop.IsTextureLoaded(tex.WebTextureId);
        }

        /// <summary>
        /// Phóng đại hoặc thu nhỏ texture theo thuật toán láng giềng gần nhất (Nearest-Neighbor) trên Canvas.
        /// </summary>
        public static void ScaleTextureNN(Texture2D tex, int newWidth, int newHeight)
        {
            if (tex == null || newWidth <= 0 || newHeight <= 0) return;
            int srcId = tex.WebTextureId;
            if (srcId > 0)
            {
                int dstId = _nextTextureId++;
                WebInterop.ScaleTextureNN(dstId, srcId, tex.width, tex.height, newWidth, newHeight);
                tex.WebTextureId = dstId;
            }
            tex.width = newWidth;
            tex.height = newHeight;
        }

        /// <summary>
        /// Tạo texture mới đã hòa trộn màu (source-atop) trực tiếp trên GPU Canvas2D.
        /// </summary>
        public static Texture2D BlendTexture(Texture2D srcTex, float level, int rgb)
        {
            if (srcTex == null || srcTex.WebTextureId <= 0 || srcTex.width <= 0 || srcTex.height <= 0)
            {
                return null;
            }
            int b = rgb & 0xFF;
            int g = (rgb >> 8) & 0xFF;
            int r = (rgb >> 16) & 0xFF;
            int dstId = _nextTextureId++;
            WebInterop.BlendTexture(dstId, srcTex.WebTextureId, srcTex.width, srcTex.height, level, r, g, b);
            return new Texture2D(srcTex.width, srcTex.height, allocatePixelBuffer: false)
            {
                WebTextureId = dstId
            };
        }

        /// <summary>
        /// Đẩy mảng điểm ảnh RGBA từ bộ đệm CPU của Texture2D lên Canvas 2D trên trình duyệt.
        /// </summary>
        public static void ApplyPixelBuffer(Texture2D tex)
        {
            if (tex == null || tex.width <= 0 || tex.height <= 0) return;
            Color[] pixels = tex.GetPixels();
            if (pixels == null || pixels.Length < tex.width * tex.height) return;
            int width = tex.width;
            int height = tex.height;
            int total = width * height;
            int texId = tex.WebTextureId > 0 ? tex.WebTextureId : _nextTextureId++;
            byte[] rgba = new byte[total * 4];
            for (int i = 0; i < total; i++)
            {
                Color c = pixels[i];
                int idx = i << 2;
                rgba[idx] = (byte)System.Math.Clamp((int)(c.r * 255f), 0, 255);
                rgba[idx + 1] = (byte)System.Math.Clamp((int)(c.g * 255f), 0, 255);
                rgba[idx + 2] = (byte)System.Math.Clamp((int)(c.b * 255f), 0, 255);
                rgba[idx + 3] = (byte)System.Math.Clamp((int)(c.a * 255f), 0, 255);
            }
            WebInterop.UploadRgbaPixels(texId, rgba, width, height);
            tex.WebTextureId = texId;
        }

        /// <summary>
        /// Đọc dữ liệu điểm ảnh RGBA từ Canvas 2D trên trình duyệt về bộ đệm pixelBuffer của Texture2D khi cần lấy mẫu màu.
        /// </summary>
        public static void EnsurePixelBufferLoaded(Texture2D tex)
        {
            if (tex == null || tex.width <= 0 || tex.height <= 0) return;
            if (tex.WebTextureId <= 0 || !WebInterop.IsTextureLoaded(tex.WebTextureId)) return;
            int width = tex.width;
            int height = tex.height;
            int total = width * height;
            byte[] rgba = WebInterop.ReadTexturePixels(tex.WebTextureId, width, height);
            if (rgba != null && rgba.Length >= total * 4)
            {
                Color[] destPixels = new Color[total];
                for (int i = 0; i < total; i++)
                {
                    int idx = i << 2;
                    destPixels[i] = new Color(
                        rgba[idx] / 255f,
                        rgba[idx + 1] / 255f,
                        rgba[idx + 2] / 255f,
                        rgba[idx + 3] / 255f);
                }
                tex.SetPixels(destPixels);
            }
        }

        /// <summary>
        /// Vẽ một vùng cắt từ Texture2D lên vùng chữ nhật trên màn hình với hệ số nhân màu tint.
        /// </summary>
        public static void DrawTexture(Rect screenRect, Texture texture, Rect sourceRect, Color tint)
        {
            float destX = screenRect.x;
            float destY = screenRect.y;
            float destW = screenRect.width;
            float destH = screenRect.height;

            if (texture is Texture2D t2d && t2d.WebTextureId > 0)
            {
                float srcX = sourceRect.x;
                float srcY = sourceRect.y;
                float srcW = sourceRect.width;
                float srcH = sourceRect.height;

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

                WebInterop.DrawTexture(
                    t2d.WebTextureId,
                    destX,
                    destY,
                    destW,
                    destH,
                    srcX,
                    srcY,
                    srcW,
                    srcH,
                    tint.r, tint.g, tint.b, tint.a);
            }
            else
            {
                if (destW < 0) { destX += destW; destW = -destW; }
                if (destH < 0) { destY += destH; destH = -destH; }
                WebInterop.DrawSolidRect(destX, destY, destW, destH, tint.r, tint.g, tint.b, tint.a);
            }
        }

        /// <summary>
        /// Vẽ hình chữ nhật đặc màu hoặc phủ Texture2D với tọa độ cộng dồn theo nhóm GUI hiện tại.
        /// </summary>
        public static void DrawTextureSolid(Rect position, Texture image, Color color, float groupOffsetX, float groupOffsetY)
        {
            float dx = position.x + groupOffsetX;
            float dy = position.y + groupOffsetY;
            float dw = position.width;
            float dh = position.height;

            if (image == Texture2D.whiteTexture || image == null)
            {
                WebInterop.DrawSolidRect(dx, dy, dw, dh, color.r, color.g, color.b, color.a);
            }
            else if (image is Texture2D t2d && t2d.WebTextureId > 0)
            {
                WebInterop.DrawTexture(t2d.WebTextureId, dx, dy, dw, dh, 0f, 0f, 1f, 1f, color.r, color.g, color.b, color.a);
            }
        }

        /// <summary>
        /// Vẽ hình tròn đặc màu tại tâm và bán kính chỉ định.
        /// </summary>
        public static void DrawCircle(float centerX, float centerY, float radius, Color c)
        {
            WebInterop.DrawCircle(centerX, centerY, radius, c.r, c.g, c.b, c.a, fill: true);
        }

        /// <summary>
        /// Vẽ đường viền hình tròn tại tâm và bán kính chỉ định.
        /// </summary>
        public static void DrawCircleOutline(float centerX, float centerY, float radius, Color c)
        {
            WebInterop.DrawCircle(centerX, centerY, radius, c.r, c.g, c.b, c.a, fill: false);
        }

        private static int ResolveEffectiveFontCode(GUIStyle style)
        {
            int theme = ModGraphics.fontTheme;
            if (theme == 1) return 1;
            if (theme == 2) return 2;
            if (theme == 3) return 3;
            string fname = style?.font?.name;
            if (!string.IsNullOrEmpty(fname))
            {
                if (fname.IndexOf("staccato", StringComparison.OrdinalIgnoreCase) >= 0) return 20;
                if (fname.IndexOf("chelthm", StringComparison.OrdinalIgnoreCase) >= 0) return 10;
            }
            return 0;
        }

        /// <summary>
        /// Đo kích thước chiều rộng và chiều cao của chuỗi văn bản theo GUIStyle và bộ đệm kích thước chữ.
        /// </summary>
        public static Vector2 MeasureText(string text, GUIStyle style)
        {
            if (string.IsNullOrEmpty(text)) return Vector2.zero;
            int fontSize = (style != null && style.fontSize > 0)
                ? (style.fontSize * mGraphics.zoomLevel)
                : Font.GetDefaultFontSize(style?.font);
            int fontCode = ResolveEffectiveFontCode(style);
            var key = (fontCode, fontSize, text);
            if (!_textWidthCache.TryGetValue(key, out float w))
            {
                w = (float)WebInterop.MeasureTextWidth(text, fontSize, fontCode);
                if (_textWidthCache.Count > 4096) _textWidthCache.Clear();
                _textWidthCache[key] = w;
            }
            return new Vector2(w, fontSize + 4f);
        }

        /// <summary>
        /// Vẽ nhãn văn bản lên Canvas 2D theo định dạng phông chữ, màu sắc và căn lề của GUIStyle.
        /// </summary>
        public static void DrawLabel(Rect position, string text, GUIStyle style, float groupOffsetX, float groupOffsetY, Color defaultColor)
        {
            if (string.IsNullOrEmpty(text)) return;
            float px = position.x + groupOffsetX;
            float py = position.y + groupOffsetY;

            int fontSize;
            if (style != null && style.fontSize > 0)
            {
                fontSize = style.fontSize * mGraphics.zoomLevel;
            }
            else
            {
                fontSize = Font.GetDefaultFontSize(style?.font);
                int refSize = Font.GetLayoutReferenceFontSize(style?.font);
                py += (refSize - fontSize) / 2f;
            }

            Color col = style?.normal?.textColor ?? defaultColor;
            int alignCode = 0;
            if (style != null)
            {
                if (style.alignment == TextAnchor.UpperCenter || style.alignment == TextAnchor.MiddleCenter || style.alignment == TextAnchor.LowerCenter)
                {
                    alignCode = 1;
                }
                else if (style.alignment == TextAnchor.UpperRight || style.alignment == TextAnchor.MiddleRight || style.alignment == TextAnchor.LowerRight)
                {
                    alignCode = 2;
                }
            }

            int fontCode = ResolveEffectiveFontCode(style);
            WebInterop.DrawText(
                text,
                px,
                py,
                position.width,
                position.height,
                fontSize,
                fontCode,
                col.r, col.g, col.b, col.a,
                alignCode);
        }

        /// <summary>
        /// Bắt đầu một vùng cắt (clipping group) mới trên Canvas 2D và lưu trạng thái dịch chuyển tọa độ.
        /// </summary>
        public static void BeginGroup(Rect position)
        {
            _clipOffsetStack.Push(_currentOffset);
            float sx = position.x;
            float sy = position.y;
            float sw = position.width;
            float sh = position.height;
            WebInterop.PushClipRect(sx, sy, sw, sh);
        }

        /// <summary>
        /// Kết thúc vùng cắt hiện tại và khôi phục trạng thái dịch chuyển tọa độ trước đó.
        /// </summary>
        public static void EndGroup()
        {
            if (_clipOffsetStack.Count > 0)
            {
                _currentOffset = _clipOffsetStack.Pop();
            }
            else
            {
                _currentOffset = Vector2.zero;
            }
            WebInterop.PopClipRect();
        }

        /// <summary>
        /// Xoay ma trận biến đổi 2D trên Canvas quanh tâm chỉ định một góc tính bằng độ.
        /// </summary>
        public static void RotateAroundPivot(float angle, Vector2 pivot)
        {
            WebInterop.RotateAroundPivot(angle, pivot.x, pivot.y);
        }

        /// <summary>
        /// Khôi phục ma trận biến đổi gần nhất trên ngăn xếp Canvas 2D.
        /// </summary>
        public static void RestoreMatrix()
        {
            WebInterop.RestoreMatrix();
        }

        /// <summary>
        /// Đặt lại toàn bộ ngăn xếp ma trận biến đổi về trạng thái gốc.
        /// </summary>
        public static void ResetMatrixStack()
        {
            WebInterop.ResetMatrixStack();
        }

        /// <summary>
        /// Xóa bộ nhớ đệm đo kích thước chuỗi văn bản khi đổi bộ phông chữ hoặc độ phóng đại.
        /// </summary>
        public static void InvalidateFontCache()
        {
            _textWidthCache.Clear();
        }

        /// <summary>
        /// Trích xuất nhanh chiều rộng và chiều cao từ header nhị phân của ảnh PNG hoặc JPEG.
        /// </summary>
        public static void ParseImageDimensions(byte[] data, out int width, out int height)
        {
            width = 1;
            height = 1;
            if (data == null || data.Length < 24) return;

            if (data[0] == 0x89 && data[1] == 0x50 && data[2] == 0x4E && data[3] == 0x47)
            {
                width = (data[16] << 24) | (data[17] << 16) | (data[18] << 8) | data[19];
                height = (data[20] << 24) | (data[21] << 16) | (data[22] << 8) | data[23];
                if (width <= 0) width = 1;
                if (height <= 0) height = 1;
                return;
            }

            if (data[0] == 0xFF && data[1] == 0xD8)
            {
                int idx = 2;
                while (idx + 9 < data.Length)
                {
                    if (data[idx] != 0xFF)
                    {
                        idx++;
                        continue;
                    }
                    byte marker = data[idx + 1];
                    if (marker == 0xFF)
                    {
                        idx++;
                        continue;
                    }
                    int len = (data[idx + 2] << 8) | data[idx + 3];
                    if (marker >= 0xC0 && marker <= 0xC3)
                    {
                        height = (data[idx + 5] << 8) | data[idx + 6];
                        width = (data[idx + 7] << 8) | data[idx + 8];
                        if (width <= 0) width = 1;
                        if (height <= 0) height = 1;
                        return;
                    }
                    if (len < 2) break;
                    idx += 2 + len;
                }
            }
        }
    }
}

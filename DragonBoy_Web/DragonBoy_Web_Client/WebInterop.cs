using System.Runtime.InteropServices.JavaScript;

namespace DragonBoy_Web_Host
{
    /// <summary>
    /// Khai báo các phương thức cầu nối JSImport giao tiếp trực tiếp giữa .NET 8 WebAssembly và đối tượng globalThis.DragonBoyWeb trên trình duyệt.
    /// </summary>
    public static partial class WebInterop
    {
        [JSImport("globalThis.DragonBoyWeb.getCanvasWidth")]
        internal static partial int GetCanvasWidth();

        [JSImport("globalThis.DragonBoyWeb.getCanvasHeight")]
        internal static partial int GetCanvasHeight();

        [JSImport("globalThis.DragonBoyWeb.isFullScreen")]
        internal static partial bool IsFullScreen();

        [JSImport("globalThis.DragonBoyWeb.setResolution")]
        internal static partial void SetResolution(int width, int height, bool fullscreen);

        [JSImport("globalThis.DragonBoyWeb.setWindowTitle")]
        internal static partial void SetWindowTitle(string title);

        [JSImport("globalThis.DragonBoyWeb.isTouchDevice")]
        internal static partial bool IsTouchDevice();

        [JSImport("globalThis.DragonBoyWeb.isTextureLoaded")]
        internal static partial bool IsTextureLoaded(int texId);

        [JSImport("globalThis.DragonBoyWeb.registerLazyTexture")]
        internal static partial void RegisterLazyTexture(int texId, string url, int width, int height);

        [JSImport("globalThis.DragonBoyWeb.createTextureFromBytes")]
        internal static partial void CreateTextureFromBytes(int texId, byte[] imageBytes, int width, int height);

        [JSImport("globalThis.DragonBoyWeb.scaleTextureNN")]
        internal static partial void ScaleTextureNN(int dstTexId, int srcTexId, int srcW, int srcH, int targetW, int targetH);

        [JSImport("globalThis.DragonBoyWeb.blendTexture")]
        internal static partial void BlendTexture(int dstTexId, int srcTexId, int targetW, int targetH, double level, int r, int g, int b);

        [JSImport("globalThis.DragonBoyWeb.uploadRgbaPixels")]
        internal static partial void UploadRgbaPixels(int texId, byte[] rgbaBytes, int width, int height);

        [JSImport("globalThis.DragonBoyWeb.readTexturePixels")]
        internal static partial byte[] ReadTexturePixels(int texId, int width, int height);

        [JSImport("globalThis.DragonBoyWeb.beginFrame")]
        internal static partial void BeginFrame(int virtualW, int virtualH);

        [JSImport("globalThis.DragonBoyWeb.endFrame")]
        internal static partial void EndFrame();

        [JSImport("globalThis.DragonBoyWeb.drawTexture")]
        internal static partial void DrawTexture(int texId, double dstX, double dstY, double dstW, double dstH, double srcX, double srcY, double srcW, double srcH, double r, double g, double b, double a);

        [JSImport("globalThis.DragonBoyWeb.drawSolidRect")]
        internal static partial void DrawSolidRect(double dstX, double dstY, double dstW, double dstH, double r, double g, double b, double a);

        [JSImport("globalThis.DragonBoyWeb.drawCircle")]
        internal static partial void DrawCircle(double cx, double cy, double radius, double r, double g, double b, double a, bool fill);

        [JSImport("globalThis.DragonBoyWeb.measureTextWidth")]
        internal static partial double MeasureTextWidth(string text, int fontSize, int fontTheme);

        [JSImport("globalThis.DragonBoyWeb.drawText")]
        internal static partial void DrawText(string text, double x, double y, double w, double h, int fontSize, int fontTheme, double r, double g, double b, double a, int align);

        [JSImport("globalThis.DragonBoyWeb.pushClipRect")]
        internal static partial void PushClipRect(double x, double y, double w, double h);

        [JSImport("globalThis.DragonBoyWeb.popClipRect")]
        internal static partial void PopClipRect();

        [JSImport("globalThis.DragonBoyWeb.rotateAroundPivot")]
        internal static partial void RotateAroundPivot(double angleDeg, double pivotX, double pivotY);

        [JSImport("globalThis.DragonBoyWeb.restoreMatrix")]
        internal static partial void RestoreMatrix();

        [JSImport("globalThis.DragonBoyWeb.resetMatrixStack")]
        internal static partial void ResetMatrixStack();

        [JSImport("globalThis.DragonBoyWeb.saveStorageFile")]
        internal static partial void SaveStorageFile(string vfsPath, string base64Data);

        [JSImport("globalThis.DragonBoyWeb.saveStorageText")]
        internal static partial void SaveStorageText(string vfsPath, string textContent);

        [JSImport("globalThis.DragonBoyWeb.deleteStorageFile")]
        internal static partial void DeleteStorageFile(string vfsPath);

        [JSImport("globalThis.DragonBoyWeb.getSharedFilesPayload")]
        internal static partial string GetSharedFilesPayload();

        [JSImport("globalThis.DragonBoyWeb.getAllPersistedFilesPayload")]
        internal static partial string GetAllPersistedFilesPayload();

        [JSImport("globalThis.DragonBoyWeb.openUrl")]
        internal static partial void OpenUrl(string url);

        [JSImport("globalThis.DragonBoyWeb.getClipboardText")]
        internal static partial string GetClipboardText();

        [JSImport("globalThis.DragonBoyWeb.setClipboardText")]
        internal static partial void SetClipboardText(string text);

        [JSImport("globalThis.DragonBoyWeb.showSoftKeyboard")]
        internal static partial void ShowSoftKeyboard(string initialText);

        [JSImport("globalThis.DragonBoyWeb.hideSoftKeyboard")]
        internal static partial void HideSoftKeyboard();

        [JSImport("globalThis.DragonBoyWeb.getRelayWsUrl")]
        internal static partial string GetRelayWsUrl(string targetHost, int targetPort);

        [JSImport("globalThis.DragonBoyWeb.getHttpBaseUrl")]
        internal static partial string GetHttpBaseUrl();

        [JSImport("globalThis.DragonBoyWeb.getInitialTabId")]
        internal static partial int GetInitialTabId();

        [JSImport("globalThis.DragonBoyWeb.hostSwitchTab")]
        internal static partial void HostSwitchTab(int tabId, bool keepHubOpen, int bx, int by);

        [JSImport("globalThis.DragonBoyWeb.hostPrewarmTab")]
        internal static partial void HostPrewarmTab(int tabId);

        [JSImport("globalThis.DragonBoyWeb.hostStopTab")]
        internal static partial void HostStopTab(int tabId);

        [JSImport("globalThis.DragonBoyWeb.hostDeleteTab")]
        internal static partial void HostDeleteTab(int tabId);

        [JSImport("globalThis.DragonBoyWeb.setLoadingProgress")]
        internal static partial void SetLoadingProgress(string statusText, int percent);

        [JSImport("globalThis.DragonBoyWeb.hideLoadingOverlay")]
        internal static partial void HideLoadingOverlay();
    }
}

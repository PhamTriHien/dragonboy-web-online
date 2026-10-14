using System;
using DragonBoy_Net8_Native.Engine.Graphics;

namespace DragonBoy_Net8_Native.Engine.Platform
{
    /// <summary>
    /// =========================================================================================================
    /// DRIVER ĐỒ HỌA RAYLIB NATIVE (RAYLIB GRAPHICS DRIVER)
    /// =========================================================================================================
    /// Hiện thực hóa IGraphicsDriver sử dụng engine đồ họa Raylib_cs:
    /// Chuyển tiếp (delegate) trực tiếp các lệnh vòng đời đồ họa và input tới RenderManager tĩnh.
    /// =========================================================================================================
    /// </summary>
    public class RaylibGraphicsDriver : IGraphicsDriver
    {
        public int WindowWidth => RenderManager.WindowWidth;
        public int WindowHeight => RenderManager.WindowHeight;
        public int VirtualWidth => RenderManager.VirtualWidth;
        public int VirtualHeight => RenderManager.VirtualHeight;
        public bool IsInitialized => RenderManager.IsInitialized;

        public void Init(int width, int height, string title) => RenderManager.Init(width, height, title);
        public void BeginVirtualRender() => RenderManager.BeginVirtualRender();
        public void EndVirtualRender() => RenderManager.EndVirtualRender();
        public void HandleInput() => RenderManager.HandleInput();
        public void Cleanup() => RenderManager.Cleanup();
    }
}

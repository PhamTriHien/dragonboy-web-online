using System;

namespace DragonBoy_Net8_Native.Engine.Platform
{
    /// <summary>
    /// =========================================================================================================
    /// GIAO DIỆN TRỪU TƯỢNG HÓA DRIVER ĐỒ HỌA (GRAPHICS DRIVER INTERFACE)
    /// =========================================================================================================
    /// Định nghĩa hợp đồng chung cho các backend đồ họa (Raylib, SDL, Metal, Vulkan, v.v.):
    /// - Quản lý kích thước cửa sổ thực (WindowWidth, WindowHeight) và Canvas ảo (VirtualWidth, VirtualHeight).
    /// - Điều phối vòng đời render: Init -> BeginVirtualRender -> EndVirtualRender -> Cleanup.
    /// =========================================================================================================
    /// </summary>
    public interface IGraphicsDriver
    {
        int WindowWidth { get; }
        int WindowHeight { get; }
        int VirtualWidth { get; }
        int VirtualHeight { get; }
        bool IsInitialized { get; }

        void Init(int width, int height, string title);
        void BeginVirtualRender();
        void EndVirtualRender();
        void HandleInput();
        void Cleanup();
    }
}

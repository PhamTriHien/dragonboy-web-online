using System;

namespace DragonBoy_Net8_Native.Engine.Platform
{
    /// <summary>
    /// INTERFACE IInputDriver: Thuộc phân hệ Tầng tương thích động cơ nền tảng Native.
    /// </summary>
    public interface IInputDriver
    {
        void ProcessInput(global::Main mainGame);
        void OnPointerDown(int pointerId, float x, float y);
        void OnPointerMove(int pointerId, float x, float y);
        void OnPointerUp(int pointerId, float x, float y);
    }
}

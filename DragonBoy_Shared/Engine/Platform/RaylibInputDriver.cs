using System;
using UnityEngine;

namespace DragonBoy_Net8_Native.Engine.Platform
{
    /// <summary>
    /// Trình điều khiển sự kiện nhập liệu Raylib: Chuyển tiếp tọa độ chuột, chạm cảm ứng và phím bấm vào GameCanvas.
    /// </summary>
    public class RaylibInputDriver : IInputDriver
    {
        /// <summary>
        /// Xử lý nghiệp vụ logic chuyên trách cho ProcessInput.
        /// </summary>
        public void ProcessInput(global::Main mainGame)
        {
            EventPump.ProcessInput(mainGame);
        }

        /// <summary>
        /// Bộ xử lý sự kiện tiếp nhận và phản hồi OnPointerDown.
        /// </summary>
        public void OnPointerDown(int pointerId, float x, float y)
        {
            if (global::GameMidlet.gameCanvas != null)
            {
                global::GameMidlet.gameCanvas.pointerPressed((int)(x / (float)mGraphics.zoomLevel), (int)(y / (float)mGraphics.zoomLevel));
            }
        }

        /// <summary>
        /// Bộ xử lý sự kiện tiếp nhận và phản hồi OnPointerMove.
        /// </summary>
        public void OnPointerMove(int pointerId, float x, float y)
        {
            if (global::GameMidlet.gameCanvas != null)
            {
                global::GameMidlet.gameCanvas.pointerDragged((int)(x / (float)mGraphics.zoomLevel), (int)(y / (float)mGraphics.zoomLevel));
            }
        }

        /// <summary>
        /// Bộ xử lý sự kiện tiếp nhận và phản hồi OnPointerUp.
        /// </summary>
        public void OnPointerUp(int pointerId, float x, float y)
        {
            if (global::GameMidlet.gameCanvas != null)
            {
                global::GameMidlet.gameCanvas.pointerReleased((int)(x / (float)mGraphics.zoomLevel), (int)(y / (float)mGraphics.zoomLevel));
            }
        }
    }
}

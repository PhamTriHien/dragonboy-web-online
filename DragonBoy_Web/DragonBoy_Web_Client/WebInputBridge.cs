using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.JavaScript;
using UnityEngine;

namespace DragonBoy_Web_Host
{
    /// <summary>
    /// Cầu nối tiếp nhận và ánh xạ sự kiện chuột, cảm ứng, con lăn và bàn phím từ trình duyệt sang hệ thống Input của Engine.
    /// </summary>
    public static partial class WebInputBridge
    {
        private static Vector3 _mousePos = Vector3.zero;
        private static readonly bool[] _mouseHeld = new bool[3];
        private static readonly bool[] _mouseDown = new bool[3];
        private static readonly bool[] _mouseUp = new bool[3];
        private static float _wheelDelta;

        private static readonly HashSet<KeyCode> _keysHeld = new HashSet<KeyCode>();
        private static readonly HashSet<KeyCode> _keysDown = new HashSet<KeyCode>();

        private struct QueuedKeyEvent
        {
            public EventType Type;
            public KeyCode Key;
            public char Character;
            public bool Ctrl;
            public bool Shift;
            public bool Alt;
        }

        private static readonly Queue<QueuedKeyEvent> _keyQueue = new Queue<QueuedKeyEvent>();

        public static Vector3 GetMousePosition() => _mousePos;
        public static bool GetMouseButton(int button) => button >= 0 && button < 3 && _mouseHeld[button];
        public static bool GetMouseButtonDown(int button) => button >= 0 && button < 3 && _mouseDown[button];
        public static bool GetMouseButtonUp(int button) => button >= 0 && button < 3 && _mouseUp[button];
        public static bool GetKey(KeyCode key) => _keysHeld.Contains(key);
        public static bool GetKeyDown(KeyCode key) => _keysDown.Contains(key);

        /// <summary>
        /// Đọc giá trị trục con lăn chuột trong khung hình hiện tại.
        /// </summary>
        public static float GetAxis(string axisName)
        {
            if (axisName == "Mouse ScrollWheel")
            {
                return _wheelDelta;
            }
            return 0f;
        }

        /// <summary>
        /// Đặt lại trạng thái nhấn/nhả chuột và phím tức thời sau khi kết thúc mỗi khung hình.
        /// </summary>
        public static void EndFrameInputReset()
        {
            for (int i = 0; i < 3; i++)
            {
                _mouseDown[i] = false;
                _mouseUp[i] = false;
            }
            _keysDown.Clear();
            _wheelDelta = 0f;
        }

        /// <summary>
        /// Phát các sự kiện bàn phím và chuột đang chờ vào vòng lặp OnGUI của Main trước khi chạy FixedUpdate.
        /// </summary>
        public static void ProcessInput(global::Main mainGame)
        {
            if (mainGame == null) return;

            bool pointerHandled = false;
            while (_keyQueue.Count > 0)
            {
                QueuedKeyEvent ev = _keyQueue.Dequeue();
                Event.current.type = ev.Type;
                Event.current.keyCode = ev.Key;
                Event.current.character = ev.Character;
                Event.current.control = ev.Ctrl;
                Event.current.shift = ev.Shift;
                Event.current.alt = ev.Alt;
                mainGame.OnGUI();
                if (!pointerHandled)
                {
                    pointerHandled = true;
                    EndFrameInputReset();
                }
            }

            if (!pointerHandled && (_mouseDown[0] || _mouseUp[0] || _mouseHeld[0] || _wheelDelta != 0f))
            {
                Event.current.type = EventType.Layout;
                Event.current.keyCode = KeyCode.None;
                Event.current.character = '\0';
                mainGame.OnGUI();
                EndFrameInputReset();
            }
        }

        /// <summary>
        /// Tiếp nhận sự kiện con trỏ chuột hoặc chạm cảm ứng từ trình duyệt.
        /// </summary>
        [JSExport]
        public static void OnPointerEvent(int type, double virtualX, double virtualY, int button)
        {
            float vx = (float)virtualX;
            float vy = (float)virtualY;
            int sh = Screen.height;
            _mousePos = new Vector3(vx, sh - vy, 0f);

            int btn = (button >= 0 && button < 3) ? button : 0;
            if (type == 0)
            {
                _mouseHeld[btn] = true;
                _mouseDown[btn] = true;
            }
            else if (type == 1)
            {
                _mouseHeld[btn] = false;
                _mouseUp[btn] = true;
            }
        }

        /// <summary>
        /// Tiếp nhận sự kiện cuộn con lăn chuột từ trình duyệt.
        /// </summary>
        [JSExport]
        public static void OnWheelEvent(double deltaY)
        {
            _wheelDelta = (float)(-deltaY * 0.01);
        }

        /// <summary>
        /// Tiếp nhận sự kiện bàn phím vật lý từ trình duyệt và chuyển đổi sang KeyCode / ký tự Unicode.
        /// </summary>
        [JSExport]
        public static void OnKeyEvent(int isDown, string code, string keyStr, int ctrl, int shift, int alt)
        {
            KeyCode kc = MapBrowserKey(code, keyStr);
            bool isCtrl = ctrl != 0;
            bool isShift = shift != 0;
            bool isAlt = alt != 0;

            if (isDown != 0)
            {
                bool wasHeld = kc != KeyCode.None && _keysHeld.Contains(kc);
                if (kc != KeyCode.None)
                {
                    _keysHeld.Add(kc);
                    if (!wasHeld)
                    {
                        _keysDown.Add(kc);
                    }
                }

                bool isTextFocused = ChatTextField.gI().isShow || TField.GetActive() != null;
                if (wasHeld && !isTextFocused)
                {
                    return;
                }

                char ch = '\0';
                if (code == "Backspace") ch = '\b';
                else if (code == "Delete") ch = (char)127;
                else if (code == "Enter" || code == "NumpadEnter") ch = '\n';
                else if (code == "Escape") ch = (char)27;
                else if (code == "Tab") ch = '\t';
                else if (isCtrl)
                {
                    if (code == "KeyV") ch = (char)22;
                    else if (code == "KeyC") ch = (char)3;
                    else if (code == "KeyA") ch = (char)1;
                    else if (code == "KeyX") ch = (char)24;
                }
                else if (!string.IsNullOrEmpty(keyStr) && keyStr.Length == 1 && keyStr[0] >= 32)
                {
                    ch = keyStr[0];
                }

                if (kc != KeyCode.None || ch != '\0')
                {
                    _keyQueue.Enqueue(new QueuedKeyEvent
                    {
                        Type = EventType.KeyDown,
                        Key = kc,
                        Character = ch,
                        Ctrl = isCtrl,
                        Shift = isShift,
                        Alt = isAlt
                    });
                }
            }
            else
            {
                if (kc != KeyCode.None)
                {
                    _keysHeld.Remove(kc);
                    _keyQueue.Enqueue(new QueuedKeyEvent
                    {
                        Type = EventType.KeyUp,
                        Key = kc,
                        Character = '\0',
                        Ctrl = isCtrl,
                        Shift = isShift,
                        Alt = isAlt
                    });
                }
            }
        }

        /// <summary>
        /// Giải phóng toàn bộ trạng thái phím giữ và chuột khi cửa sổ trình duyệt mất tiêu điểm (blur).
        /// </summary>
        [JSExport]
        public static void OnBlurReset()
        {
            for (int i = 0; i < 3; i++)
            {
                _mouseHeld[i] = false;
                _mouseDown[i] = false;
                _mouseUp[i] = false;
            }
            _keysHeld.Clear();
            _keysDown.Clear();
            _keyQueue.Clear();
            _wheelDelta = 0f;
            try
            {
                GameCanvas.clearKeyHold();
                GameCanvas.clearKeyPressed();
                GameCanvas.clearAllPointerEvent();
            }
            catch { }
        }

        /// <summary>
        /// Đồng bộ nội dung văn bản từ bàn phím ảo trên thiết bị di động vào ô nhập liệu TField đang mở.
        /// </summary>
        [JSExport]
        public static void OnSoftKeyboardCommit(string text)
        {
            if (text == null) return;
            var activeField = TField.GetActive();
            if (activeField != null)
            {
                activeField.setText(text);
            }
        }

        /// <summary>
        /// Ánh xạ mã phím KeyboardEvent.code của trình duyệt sang enum KeyCode của Engine.
        /// </summary>
        private static KeyCode MapBrowserKey(string code, string keyStr)
        {
            switch (code)
            {
                case "ArrowUp": return KeyCode.UpArrow;
                case "ArrowDown": return KeyCode.DownArrow;
                case "ArrowLeft": return KeyCode.LeftArrow;
                case "ArrowRight": return KeyCode.RightArrow;
                case "Enter": return KeyCode.Return;
                case "NumpadEnter": return KeyCode.KeypadEnter;
                case "Backspace": return KeyCode.Backspace;
                case "Delete": return KeyCode.Delete;
                case "Tab": return KeyCode.Tab;
                case "Escape": return KeyCode.Escape;
                case "Space": return KeyCode.Space;
                case "Backquote": return KeyCode.BackQuote;
                case "Home": return KeyCode.Home;
                case "AltLeft": return KeyCode.LeftAlt;
                case "AltRight": return KeyCode.RightAlt;
                case "F1": return KeyCode.F1;
                case "F2": return KeyCode.F2;
                case "F3": return KeyCode.F3;
                case "F10": return KeyCode.F10;
                case "F11": return KeyCode.F11;
                case "Digit0": return KeyCode.Alpha0;
                case "Digit1": return KeyCode.Alpha1;
                case "Digit2": return KeyCode.Alpha2;
                case "Digit3": return KeyCode.Alpha3;
                case "Digit4": return KeyCode.Alpha4;
                case "Digit5": return KeyCode.Alpha5;
                case "Digit6": return KeyCode.Alpha6;
                case "Digit7": return KeyCode.Alpha7;
                case "Digit8": return KeyCode.Alpha8;
                case "Digit9": return KeyCode.Alpha9;
                case "Numpad0": return KeyCode.Keypad0;
                case "Numpad1": return KeyCode.Keypad1;
                case "Numpad2": return KeyCode.Keypad2;
                case "Numpad3": return KeyCode.Keypad3;
                case "Numpad4": return KeyCode.Keypad4;
                case "Numpad5": return KeyCode.Keypad5;
                case "Numpad6": return KeyCode.Keypad6;
                case "Numpad7": return KeyCode.Keypad7;
                case "Numpad8": return KeyCode.Keypad8;
                case "Numpad9": return KeyCode.Keypad9;
            }

            if (!string.IsNullOrEmpty(code) && code.StartsWith("Key") && code.Length == 4)
            {
                char c = char.ToLowerInvariant(code[3]);
                if (c >= 'a' && c <= 'z')
                {
                    return (KeyCode)((int)KeyCode.A + (c - 'a'));
                }
            }

            if (keyStr == "-") return KeyCode.Minus;
            if (keyStr == "=" || keyStr == "+") return KeyCode.Equals;
            return KeyCode.None;
        }
    }
}

using System;
using System.Reflection;
using Raylib_cs;

namespace UnityEngine
{
    /// <summary>
    /// =========================================================================================================
    /// CẦU NỐI SỰ KIỆN ĐẦU VÀO RAYLIB -> UNITY EVENT (EVENT PUMP)
    /// =========================================================================================================
    /// Chuyển đổi và bơm (pump) tất cả sự kiện chuột, bàn phím và ký tự từ Raylib vào Unity Compatibility Layer:
    /// 1. KeyDown / KeyUp Mapping: Ánh xạ mã phím Raylib (KeyboardKey) sang Unity KeyCode (`MapToKeyCode`).
    /// 2. Text Input & Hotkeys: Nhận diện phím điều hướng, phím hành động (Enter, Tab, Backspace, Esc, F1-F12),
    ///    tổ hợp phím Ctrl+A/C/V/X, và các ký tự gõ tiếng Việt / UTF-8 gửi tới `TField` / `ChatTextField`.
    /// 3. Mouse / Pointer Events: Xử lý tọa độ chuột, drag chuột, nút cuộn chuột và click events.
    /// =========================================================================================================
    /// </summary>
    public static class EventPump
    {
        private static readonly KeyboardKey[] MonitoredKeys = new KeyboardKey[]
        {
            KeyboardKey.A, KeyboardKey.B, KeyboardKey.C, KeyboardKey.D, KeyboardKey.E,
            KeyboardKey.F, KeyboardKey.G, KeyboardKey.H, KeyboardKey.I, KeyboardKey.J,
            KeyboardKey.K, KeyboardKey.L, KeyboardKey.M, KeyboardKey.N, KeyboardKey.O,
            KeyboardKey.P, KeyboardKey.Q, KeyboardKey.R, KeyboardKey.S, KeyboardKey.T,
            KeyboardKey.U, KeyboardKey.V, KeyboardKey.W, KeyboardKey.X, KeyboardKey.Y, KeyboardKey.Z,
            KeyboardKey.Zero, KeyboardKey.One, KeyboardKey.Two, KeyboardKey.Three, KeyboardKey.Four,
            KeyboardKey.Five, KeyboardKey.Six, KeyboardKey.Seven, KeyboardKey.Eight, KeyboardKey.Nine,
            KeyboardKey.Space, KeyboardKey.Enter, KeyboardKey.Escape, KeyboardKey.Tab, KeyboardKey.Backspace, KeyboardKey.Delete,
            KeyboardKey.Up, KeyboardKey.Down, KeyboardKey.Left, KeyboardKey.Right,
            KeyboardKey.F1, KeyboardKey.F2, KeyboardKey.F3, KeyboardKey.F10, KeyboardKey.F11,
            KeyboardKey.Minus, KeyboardKey.Equal, KeyboardKey.LeftBracket, KeyboardKey.RightBracket,
            KeyboardKey.Semicolon, KeyboardKey.Apostrophe, KeyboardKey.Comma, KeyboardKey.Period, KeyboardKey.Slash, KeyboardKey.Backslash, KeyboardKey.Grave,
            KeyboardKey.Kp0, KeyboardKey.Kp1, KeyboardKey.Kp2, KeyboardKey.Kp3, KeyboardKey.Kp4,
            KeyboardKey.Kp5, KeyboardKey.Kp6, KeyboardKey.Kp7, KeyboardKey.Kp8, KeyboardKey.Kp9,
            KeyboardKey.KpEnter
        };

        /// <summary>
        /// Thực thi phương thức nghiệp vụ MapToKeyCode.
        /// </summary>
        public static KeyCode MapToKeyCode(KeyboardKey rk)
        {
            return rk switch
            {
                KeyboardKey.A => KeyCode.A,
                KeyboardKey.B => KeyCode.B,
                KeyboardKey.C => KeyCode.C,
                KeyboardKey.D => KeyCode.D,
                KeyboardKey.E => KeyCode.E,
                KeyboardKey.F => KeyCode.F,
                KeyboardKey.G => KeyCode.G,
                KeyboardKey.H => KeyCode.H,
                KeyboardKey.I => KeyCode.I,
                KeyboardKey.J => KeyCode.J,
                KeyboardKey.K => KeyCode.K,
                KeyboardKey.L => KeyCode.L,
                KeyboardKey.M => KeyCode.M,
                KeyboardKey.N => KeyCode.N,
                KeyboardKey.O => KeyCode.O,
                KeyboardKey.P => KeyCode.P,
                KeyboardKey.Q => KeyCode.Q,
                KeyboardKey.R => KeyCode.R,
                KeyboardKey.S => KeyCode.S,
                KeyboardKey.T => KeyCode.T,
                KeyboardKey.U => KeyCode.U,
                KeyboardKey.V => KeyCode.V,
                KeyboardKey.W => KeyCode.W,
                KeyboardKey.X => KeyCode.X,
                KeyboardKey.Y => KeyCode.Y,
                KeyboardKey.Z => KeyCode.Z,
                KeyboardKey.Zero => KeyCode.Alpha0,
                KeyboardKey.One => KeyCode.Alpha1,
                KeyboardKey.Two => KeyCode.Alpha2,
                KeyboardKey.Three => KeyCode.Alpha3,
                KeyboardKey.Four => KeyCode.Alpha4,
                KeyboardKey.Five => KeyCode.Alpha5,
                KeyboardKey.Six => KeyCode.Alpha6,
                KeyboardKey.Seven => KeyCode.Alpha7,
                KeyboardKey.Eight => KeyCode.Alpha8,
                KeyboardKey.Nine => KeyCode.Alpha9,
                KeyboardKey.Space => KeyCode.Space,
                KeyboardKey.Enter => KeyCode.Return,
                KeyboardKey.Escape => KeyCode.Escape,
                KeyboardKey.Tab => KeyCode.Tab,
                KeyboardKey.Backspace => KeyCode.Backspace,
                KeyboardKey.Delete => KeyCode.Delete,
                KeyboardKey.Up => KeyCode.UpArrow,
                KeyboardKey.Down => KeyCode.DownArrow,
                KeyboardKey.Left => KeyCode.LeftArrow,
                KeyboardKey.Right => KeyCode.RightArrow,
                KeyboardKey.F1 => KeyCode.F1,
                KeyboardKey.F2 => KeyCode.F2,
                KeyboardKey.F3 => KeyCode.F3,
                KeyboardKey.F10 => KeyCode.F10,
                KeyboardKey.F11 => KeyCode.F11,
                KeyboardKey.Minus => KeyCode.Minus,
                KeyboardKey.Equal => KeyCode.Equals,
                KeyboardKey.LeftBracket => KeyCode.LeftBracket,
                KeyboardKey.RightBracket => KeyCode.RightBracket,
                KeyboardKey.Semicolon => KeyCode.Semicolon,
                KeyboardKey.Apostrophe => KeyCode.Quote,
                KeyboardKey.Comma => KeyCode.Comma,
                KeyboardKey.Period => KeyCode.Period,
                KeyboardKey.Slash => KeyCode.Slash,
                KeyboardKey.Backslash => KeyCode.Backslash,
                KeyboardKey.Grave => KeyCode.BackQuote,
                KeyboardKey.Kp0 => KeyCode.Keypad0,
                KeyboardKey.Kp1 => KeyCode.Keypad1,
                KeyboardKey.Kp2 => KeyCode.Keypad2,
                KeyboardKey.Kp3 => KeyCode.Keypad3,
                KeyboardKey.Kp4 => KeyCode.Keypad4,
                KeyboardKey.Kp5 => KeyCode.Keypad5,
                KeyboardKey.Kp6 => KeyCode.Keypad6,
                KeyboardKey.Kp7 => KeyCode.Keypad7,
                KeyboardKey.Kp8 => KeyCode.Keypad8,
                KeyboardKey.Kp9 => KeyCode.Keypad9,
                KeyboardKey.KpEnter => KeyCode.KeypadEnter,
                _ => KeyCode.None
            };
        }

        /// <summary>
        /// Xử lý nghiệp vụ logic chuyên trách cho ProcessInput.
        /// </summary>
        public static void ProcessInput(global::Main mainGame)
        {
            if (mainGame == null) return;
#if BROWSER
            if (OperatingSystem.IsBrowser())
            {
                DragonBoy_Web_Host.WebInputBridge.ProcessInput(mainGame);
                return;
            }
#endif

            bool ctrl = Raylib.IsKeyDown(KeyboardKey.LeftControl) || Raylib.IsKeyDown(KeyboardKey.RightControl);
            bool shift = Raylib.IsKeyDown(KeyboardKey.LeftShift) || Raylib.IsKeyDown(KeyboardKey.RightShift);
            bool alt = Raylib.IsKeyDown(KeyboardKey.LeftAlt) || Raylib.IsKeyDown(KeyboardKey.RightAlt);

            bool isTextFocused = (ChatTextField.gI().isShow || TField.GetActive() != null);

            // 1. Process KeyDown events from Raylib key queue
            while (true)
            {
                int key = Raylib.GetKeyPressed();
                if (key <= 0) break;

                KeyboardKey rk = (KeyboardKey)key;
                KeyCode kc = MapToKeyCode(rk);

                char ch = '\0';
                bool isActionKey = false;

                if (rk == KeyboardKey.Backspace)
                {
                    ch = '\b';
                    isActionKey = true;
                }
                else if (rk == KeyboardKey.Delete)
                {
                    ch = (char)127;
                    isActionKey = true;
                }
                else if (rk == KeyboardKey.Enter || rk == KeyboardKey.KpEnter)
                {
                    ch = '\n';
                    isActionKey = true;
                }
                else if (rk == KeyboardKey.Escape)
                {
                    ch = (char)27;
                    isActionKey = true;
                }
                else if (rk == KeyboardKey.Tab)
                {
                    ch = '\t';
                    isActionKey = true;
                }
                else if (rk == KeyboardKey.Up || rk == KeyboardKey.Down || rk == KeyboardKey.Left || rk == KeyboardKey.Right)
                {
                    ch = '\0';
                    isActionKey = true;
                }
                else if (rk >= KeyboardKey.F1 && rk <= KeyboardKey.F12)
                {
                    ch = '\0';
                    isActionKey = true;
                }
                else if (ctrl)
                {
                    if (rk == KeyboardKey.V) ch = (char)22;
                    else if (rk == KeyboardKey.C) ch = (char)3;
                    else if (rk == KeyboardKey.A) ch = (char)1;
                    else if (rk == KeyboardKey.X) ch = (char)24;
                    else ch = '\0';
                    isActionKey = true;
                }

                if (isActionKey)
                {
                    Event.current.type = EventType.KeyDown;
                    Event.current.keyCode = kc;
                    Event.current.character = ch;
                    Event.current.control = ctrl;
                    Event.current.shift = shift;
                    Event.current.alt = alt;

                    mainGame.OnGUI();
                }
                else if (!isTextFocused)
                {
                    char defaultChar = '\0';
                    if (kc >= KeyCode.A && kc <= KeyCode.Z)
                    {
                        defaultChar = shift ? (char)('A' + (kc - KeyCode.A)) : (char)('a' + (kc - KeyCode.A));
                    }
                    else if (kc >= KeyCode.Alpha0 && kc <= KeyCode.Alpha9)
                    {
                        defaultChar = (char)('0' + (kc - KeyCode.Alpha0));
                    }
                    else if (rk == KeyboardKey.Space)
                    {
                        defaultChar = ' ';
                    }

                    Event.current.type = EventType.KeyDown;
                    Event.current.keyCode = kc;
                    Event.current.character = defaultChar;
                    Event.current.control = ctrl;
                    Event.current.shift = shift;
                    Event.current.alt = alt;

                    mainGame.OnGUI();
                }
            }

            // 2. Process Unicode character events from Raylib char queue
            // (Provides 100% accurate Unicode diacritics for all languages, including Vietnamese Telex / IME)
            while (true)
            {
                int charCode = Raylib.GetCharPressed();
                if (charCode <= 0) break;

                // If text field is focused, dispatch character to onGuiMethod
                if (isTextFocused)
                {
                    char ch = (char)charCode;
                    if (ch < ' ' && ch != '\n' && ch != '\t') continue;
                    if (ch == 127) continue;

                    Event.current.type = EventType.KeyDown;
                    Event.current.keyCode = KeyCode.None;
                    Event.current.character = ch;
                    Event.current.control = ctrl;
                    Event.current.shift = shift;
                    Event.current.alt = alt;

                    mainGame.OnGUI();
                }
            }

            // 3. Process KeyUp events
            for (int i = 0; i < MonitoredKeys.Length; i++)
            {
                KeyboardKey rk = MonitoredKeys[i];
                if (Raylib.IsKeyReleased(rk))
                {
                    KeyCode kc = MapToKeyCode(rk);
                    Event.current.type = EventType.KeyUp;
                    Event.current.keyCode = kc;
                    Event.current.character = '\0';
                    Event.current.control = ctrl;
                    Event.current.shift = shift;
                    Event.current.alt = alt;

                    mainGame.OnGUI();
                }
            }
        }
    }
}

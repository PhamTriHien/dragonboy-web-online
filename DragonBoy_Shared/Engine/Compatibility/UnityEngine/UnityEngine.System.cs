using System;
using System.IO;
using System.Numerics;
using System.Diagnostics;
using Raylib_cs;

namespace UnityEngine
{
    /// <summary>
    /// =========================================================================================================
    /// TẦNG TƯƠNG THÍCH HỆ THỐNG VÀ RUNTIME UNITY (UNITY SYSTEM COMPATIBILITY LAYER)
    /// =========================================================================================================
    /// Cung cấp các module cốt lõi tương đương Unity trên môi trường thuần .NET 8:
    /// - Time: Đồng hồ thời gian chính xác cao qua Stopwatch, deltaTime, fixedDeltaTime (0.02s / 50Hz).
    /// - Application: Quản lý platform (PC/Windows), phiên bản, đường dẫn dữ liệu (persistentDataPath), Application.Quit.
    /// - PlayerPrefs: Lưu trữ cấu hình dạng Key-Value được đồng bộ vào file cấu hình trên đĩa.
    /// - Debug / Mathf: Logging có phân cấp và các hàm toán học tối ưu hóa bằng phần cứng CPU.
    /// =========================================================================================================
    /// </summary>
    public static class Time
    {
        private static readonly System.Diagnostics.Stopwatch _sw = System.Diagnostics.Stopwatch.StartNew();
        private static float _deltaTime = 0.016f;

        public static float time => (float)_sw.Elapsed.TotalSeconds;
        public static float deltaTime
        {
            get => _deltaTime;
            set => _deltaTime = value;
        }
        public static float unscaledDeltaTime => deltaTime;
        public static float fixedDeltaTime { get; set; } = 0.02f;
        public static float maximumDeltaTime { get; set; } = 0.1f;
        public static float timeScale { get; set; } = 1.0f;
        public static int frameCount { get; set; } = 1;
    }

    /// <summary>
    /// Thành phần phân mảnh (partial) của lớp KeyCode: Đảm nhiệm phân hệ chuyên trách System ().
    /// </summary>
    public enum KeyCode
    {
        None = 0,
        Backspace = 8,
        Tab = 9,
        Return = 13,
        Escape = 27,
        Space = 32,
        Quote = 39,
        Comma = 44,
        Minus = 45,
        Period = 46,
        Slash = 47,
        Alpha0 = 48,
        Alpha1 = 49,
        Alpha2 = 50,
        Alpha3 = 51,
        Alpha4 = 52,
        Alpha5 = 53,
        Alpha6 = 54,
        Alpha7 = 55,
        Alpha8 = 56,
        Alpha9 = 57,
        Semicolon = 59,
        Equals = 61,
        LeftBracket = 91,
        Backslash = 92,
        RightBracket = 93,
        BackQuote = 96,
        A = 97,
        B = 98,
        C = 99,
        D = 100,
        E = 101,
        F = 102,
        G = 103,
        H = 104,
        I = 105,
        J = 106,
        K = 107,
        L = 108,
        M = 109,
        N = 110,
        O = 111,
        P = 112,
        Q = 113,
        R = 114,
        S = 115,
        T = 116,
        U = 117,
        V = 118,
        W = 119,
        X = 120,
        Y = 121,
        Z = 122,
        Delete = 127,
        Keypad0 = 256,
        Keypad1 = 257,
        Keypad2 = 258,
        Keypad3 = 259,
        Keypad4 = 260,
        Keypad5 = 261,
        Keypad6 = 262,
        Keypad7 = 263,
        Keypad8 = 264,
        Keypad9 = 265,
        KeypadPeriod = 266,
        KeypadDivide = 267,
        KeypadMultiply = 268,
        KeypadMinus = 269,
        KeypadPlus = 270,
        KeypadEnter = 271,
        KeypadEquals = 272,
        UpArrow = 273,
        DownArrow = 274,
        RightArrow = 275,
        LeftArrow = 276,
        Home = 278,
        F1 = 282,
        F2 = 283,
        F3 = 284,
        F10 = 291,
        F11 = 292,
        LeftAlt = 308,
        RightAlt = 307,
        At = 64
    }

    /// <summary>
    /// Thành phần phân mảnh (partial) của lớp Input: Đảm nhiệm phân hệ chuyên trách System ().
    /// </summary>
    public static class Input
    {
        public static Vector3 mousePosition
        {
            get
            {
#if BROWSER
                if (OperatingSystem.IsBrowser()) return DragonBoy_Web_Host.WebInputBridge.GetMousePosition();
#endif
                if (OperatingSystem.IsAndroid() || OperatingSystem.IsIOS()) return Vector3.zero;
                try
                {
                    System.Numerics.Vector2 mp = Raylib.GetMousePosition();
                    return new Vector3(mp.X, Screen.height - mp.Y, 0f);
                }
                catch { return Vector3.zero; }
            }
        }

        /// <summary>
        /// Truy xuất và trả về dữ liệu của GetMouseButton.
        /// </summary>
        public static bool GetMouseButton(int button)
        {
            if (!Application.IsActiveTab || OperatingSystem.IsAndroid() || OperatingSystem.IsIOS()) return false;
#if BROWSER
            if (OperatingSystem.IsBrowser()) return DragonBoy_Web_Host.WebInputBridge.GetMouseButton(button);
#endif
            try
            {
                MouseButton mb = (button == 0) ? MouseButton.Left : (button == 1 ? MouseButton.Right : MouseButton.Middle);
                return Raylib.IsMouseButtonDown(mb);
            }
            catch { return false; }
        }

        /// <summary>
        /// Truy xuất và trả về dữ liệu của GetMouseButtonDown.
        /// </summary>
        public static bool GetMouseButtonDown(int button)
        {
            if (!Application.IsActiveTab || OperatingSystem.IsAndroid() || OperatingSystem.IsIOS()) return false;
#if BROWSER
            if (OperatingSystem.IsBrowser()) return DragonBoy_Web_Host.WebInputBridge.GetMouseButtonDown(button);
#endif
            try
            {
                MouseButton mb = (button == 0) ? MouseButton.Left : (button == 1 ? MouseButton.Right : MouseButton.Middle);
                return Raylib.IsMouseButtonPressed(mb);
            }
            catch { return false; }
        }

        /// <summary>
        /// Truy xuất và trả về dữ liệu của GetMouseButtonUp.
        /// </summary>
        public static bool GetMouseButtonUp(int button)
        {
            if (!Application.IsActiveTab || OperatingSystem.IsAndroid() || OperatingSystem.IsIOS()) return false;
#if BROWSER
            if (OperatingSystem.IsBrowser()) return DragonBoy_Web_Host.WebInputBridge.GetMouseButtonUp(button);
#endif
            try
            {
                MouseButton mb = (button == 0) ? MouseButton.Left : (button == 1 ? MouseButton.Right : MouseButton.Middle);
                return Raylib.IsMouseButtonReleased(mb);
            }
            catch { return false; }
        }

        /// <summary>
        /// Truy xuất và trả về dữ liệu của GetKey.
        /// </summary>
        public static bool GetKey(KeyCode key)
        {
            if (!Application.IsActiveTab || OperatingSystem.IsAndroid() || OperatingSystem.IsIOS()) return false;
#if BROWSER
            if (OperatingSystem.IsBrowser()) return DragonBoy_Web_Host.WebInputBridge.GetKey(key);
#endif
            try
            {
                KeyboardKey rk = MapKey(key);
                return rk != 0 && Raylib.IsKeyDown(rk);
            }
            catch { return false; }
        }

        /// <summary>
        /// Truy xuất và trả về dữ liệu của GetKeyDown.
        /// </summary>
        public static bool GetKeyDown(KeyCode key)
        {
            if (!Application.IsActiveTab || OperatingSystem.IsAndroid() || OperatingSystem.IsIOS()) return false;
#if BROWSER
            if (OperatingSystem.IsBrowser()) return DragonBoy_Web_Host.WebInputBridge.GetKeyDown(key);
#endif
            try
            {
                KeyboardKey rk = MapKey(key);
                return rk != 0 && Raylib.IsKeyPressed(rk);
            }
            catch { return false; }
        }

        /// <summary>
        /// Truy xuất và trả về dữ liệu của GetAxis.
        /// </summary>
        public static float GetAxis(string axisName)
        {
            if (!Application.IsActiveTab || OperatingSystem.IsAndroid() || OperatingSystem.IsIOS()) return 0f;
#if BROWSER
            if (OperatingSystem.IsBrowser()) return DragonBoy_Web_Host.WebInputBridge.GetAxis(axisName);
#endif
            try
            {
                if (axisName == "Horizontal")
                {
                    if (Raylib.IsKeyDown(KeyboardKey.Right) || Raylib.IsKeyDown(KeyboardKey.D)) return 1f;
                    if (Raylib.IsKeyDown(KeyboardKey.Left) || Raylib.IsKeyDown(KeyboardKey.A)) return -1f;
                }
                else if (axisName == "Vertical")
                {
                    if (Raylib.IsKeyDown(KeyboardKey.Up) || Raylib.IsKeyDown(KeyboardKey.W)) return 1f;
                    if (Raylib.IsKeyDown(KeyboardKey.Down) || Raylib.IsKeyDown(KeyboardKey.S)) return -1f;
                }
                else if (axisName == "Mouse ScrollWheel")
                {
                    return Raylib.GetMouseWheelMove();
                }
            }
            catch { }
            return 0f;
        }

        /// <summary>
        /// Thực thi phương thức nghiệp vụ MapKey.
        /// </summary>
        private static KeyboardKey MapKey(KeyCode k)
        {
            switch (k)
            {
                case KeyCode.A: return KeyboardKey.A;
                case KeyCode.B: return KeyboardKey.B;
                case KeyCode.C: return KeyboardKey.C;
                case KeyCode.D: return KeyboardKey.D;
                case KeyCode.E: return KeyboardKey.E;
                case KeyCode.F: return KeyboardKey.F;
                case KeyCode.G: return KeyboardKey.G;
                case KeyCode.H: return KeyboardKey.H;
                case KeyCode.I: return KeyboardKey.I;
                case KeyCode.J: return KeyboardKey.J;
                case KeyCode.K: return KeyboardKey.K;
                case KeyCode.L: return KeyboardKey.L;
                case KeyCode.M: return KeyboardKey.M;
                case KeyCode.N: return KeyboardKey.N;
                case KeyCode.O: return KeyboardKey.O;
                case KeyCode.P: return KeyboardKey.P;
                case KeyCode.Q: return KeyboardKey.Q;
                case KeyCode.R: return KeyboardKey.R;
                case KeyCode.S: return KeyboardKey.S;
                case KeyCode.T: return KeyboardKey.T;
                case KeyCode.U: return KeyboardKey.U;
                case KeyCode.V: return KeyboardKey.V;
                case KeyCode.W: return KeyboardKey.W;
                case KeyCode.X: return KeyboardKey.X;
                case KeyCode.Y: return KeyboardKey.Y;
                case KeyCode.Z: return KeyboardKey.Z;
                case KeyCode.Alpha0: return KeyboardKey.Zero;
                case KeyCode.Alpha1: return KeyboardKey.One;
                case KeyCode.Alpha2: return KeyboardKey.Two;
                case KeyCode.Alpha3: return KeyboardKey.Three;
                case KeyCode.Alpha4: return KeyboardKey.Four;
                case KeyCode.Alpha5: return KeyboardKey.Five;
                case KeyCode.Alpha6: return KeyboardKey.Six;
                case KeyCode.Alpha7: return KeyboardKey.Seven;
                case KeyCode.Alpha8: return KeyboardKey.Eight;
                case KeyCode.Alpha9: return KeyboardKey.Nine;
                case KeyCode.Space: return KeyboardKey.Space;
                case KeyCode.Return: return KeyboardKey.Enter;
                case KeyCode.Escape: return KeyboardKey.Escape;
                case KeyCode.Tab: return KeyboardKey.Tab;
                case KeyCode.Backspace: return KeyboardKey.Backspace;
                case KeyCode.Delete: return KeyboardKey.Delete;
                case KeyCode.UpArrow: return KeyboardKey.Up;
                case KeyCode.DownArrow: return KeyboardKey.Down;
                case KeyCode.LeftArrow: return KeyboardKey.Left;
                case KeyCode.RightArrow: return KeyboardKey.Right;
                case KeyCode.F1: return KeyboardKey.F1;
                case KeyCode.F2: return KeyboardKey.F2;
                case KeyCode.F3: return KeyboardKey.F3;
                case KeyCode.F10: return KeyboardKey.F10;
                case KeyCode.F11: return KeyboardKey.F11;
                case KeyCode.LeftAlt: return KeyboardKey.LeftAlt;
                case KeyCode.RightAlt: return KeyboardKey.RightAlt;
                case KeyCode.BackQuote: return KeyboardKey.Grave;
                case KeyCode.Home: return KeyboardKey.Home;
                default: return 0;
            }
        }
    }

    /// <summary>
    /// Thành phần phân mảnh (partial) của lớp Application: Đảm nhiệm phân hệ chuyên trách System ().
    /// </summary>
    public static class Application
    {
        public static bool IsActiveTab { get; set; } = true;
        private static int _currentTabId = 1;
        public static int CurrentTabId
        {
            get => _currentTabId;
            set
            {
                _currentTabId = value;
                _persistentDataPath = null;
            }
        }

        private static string _persistentDataPath;
        public static string persistentDataPath
        {
            get
            {
                if (_persistentDataPath == null)
                {
                    string rootPath;
                    if (OperatingSystem.IsBrowser())
                    {
                        rootPath = "/rms";
                    }
                    else if (OperatingSystem.IsAndroid())
                    {
                        rootPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "DragonBoy");
                    }
                    else
                    {
                        string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                        string unityPath = System.IO.Path.Combine(userProfile, "AppData", "LocalLow", "Team", "DragonBoy250");
                        if (System.IO.Directory.Exists(unityPath))
                        {
                            rootPath = unityPath;
                        }
                        else
                        {
                            rootPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "rms");
                        }
                    }

                    _persistentDataPath = (_currentTabId > 1)
                        ? System.IO.Path.Combine(rootPath, $"tab{_currentTabId}")
                        : System.IO.Path.Combine(rootPath, "tab1");

                    if (!System.IO.Directory.Exists(_persistentDataPath))
                    {
                        try { System.IO.Directory.CreateDirectory(_persistentDataPath); } catch { }
                    }
                }
                return _persistentDataPath;
            }
        }
        public static string dataPath => AppDomain.CurrentDomain.BaseDirectory;
        public static RuntimePlatform platform
        {
            get
            {
                if (OperatingSystem.IsAndroid()) return RuntimePlatform.Android;
                if (OperatingSystem.IsIOS()) return RuntimePlatform.IPhonePlayer;
                return RuntimePlatform.WindowsPlayer;
            }
        }
        public static bool runInBackground { get; set; } = true;
        private static int _targetFrameRate = 60;
        public static int targetFrameRate
        {
            get => _targetFrameRate;
            set
            {
                _targetFrameRate = value;
                try { DragonBoy_Net8_Native.Engine.Platform.PlatformService.Current?.SetTargetFrameRate(value); } catch { }
                if (IsActiveTab && !OperatingSystem.IsAndroid() && !OperatingSystem.IsIOS() && !OperatingSystem.IsBrowser())
                {
                    try { Raylib.SetTargetFPS(value); } catch { }
                }
            }
        }

        public static Action<string> InstallApkHandler;
        public static Action<string> OpenURLHandler;

        /// <summary>
        /// Thực thi phương thức nghiệp vụ OpenURL.
        /// </summary>
        public static void OpenURL(string url)
        {
            if (OpenURLHandler != null)
            {
                try { OpenURLHandler(url); return; } catch { }
            }
            try
            {
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            catch
            {
            }
        }
    }

    /// <summary>
    /// Thành phần phân mảnh (partial) của lớp RuntimePlatform: Đảm nhiệm phân hệ chuyên trách System ().
    /// </summary>
    public enum RuntimePlatform
    {
        OSXEditor = 0,
        OSXPlayer = 1,
        WindowsPlayer = 2,
        OSXDashboardPlayer = 3,
        WindowsEditor = 7,
        IPhonePlayer = 8,
        Android = 11
    }

    /// <summary>
    /// Thành phần phân mảnh (partial) của lớp SystemInfo: Đảm nhiệm phân hệ chuyên trách System ().
    /// </summary>
    public static class SystemInfo
    {
        public static string deviceUniqueIdentifier => Environment.MachineName + "_" + Environment.UserName;
        public static string operatingSystem => Environment.OSVersion.ToString();
    }

    /// <summary>
    /// Thành phần phân mảnh (partial) của lớp Debug: Đảm nhiệm phân hệ chuyên trách System ().
    /// </summary>
    public static class Debug
    {
        public static void Log(object message) => Console.WriteLine("[LOG] " + message);
        public static void LogWarning(object message) => Console.WriteLine("[WARN] " + message);
        public static void LogError(object message) => Console.Error.WriteLine("[ERR] " + message);
    }

    /// <summary>
    /// Thành phần phân mảnh (partial) của lớp PlayerPrefs: Đảm nhiệm phân hệ chuyên trách System ().
    /// </summary>
    public static class PlayerPrefs
    {
        /// <summary>
        /// Thực thi phương thức nghiệp vụ DeleteKey.
        /// </summary>
        public static void DeleteKey(string key) { }
    }

    /// <summary>
    /// Thành phần phân mảnh (partial) của lớp EventType: Đảm nhiệm phân hệ chuyên trách System ().
    /// </summary>
    public enum EventType
    {
        MouseDown = 0,
        MouseUp = 1,
        MouseMove = 2,
        MouseDrag = 3,
        KeyDown = 4,
        KeyUp = 5,
        Repaint = 7,
        Layout = 8
    }

    /// <summary>
    /// Thành phần phân mảnh (partial) của lớp Event: Đảm nhiệm phân hệ chuyên trách System ().
    /// </summary>
    public class Event
    {
        private static Event currentEvent = new Event();
        public static Event current => currentEvent;

        public EventType type { get; set; } = EventType.Repaint;
        public KeyCode keyCode { get; set; } = KeyCode.None;
        public char character { get; set; } = '\0';
        public bool control { get; set; } = false;
        public bool shift { get; set; } = false;
        public bool alt { get; set; } = false;
    }

    /// <summary>
    /// Thành phần phân mảnh (partial) của lớp GUIUtility: Đảm nhiệm phân hệ chuyên trách System ().
    /// </summary>
    public static class GUIUtility
    {
        public static System.Func<string> GetClipboardHandler;
        public static System.Action<string> SetClipboardHandler;

        public static string systemCopyBuffer
        {
            get
            {
                if (GetClipboardHandler != null)
                {
                    try
                    {
                        string txt = GetClipboardHandler();
                        if (!string.IsNullOrEmpty(txt)) return txt;
                    }
                    catch
                    {
                    }
                }

                try
                {
                    return DragonBoy_Net8_Native.Engine.Platform.PlatformService.Current.GetClipboardText();
                }
                catch
                {
                    return string.Empty;
                }
            }
            set
            {
                if (SetClipboardHandler != null)
                {
                    try
                    {
                        SetClipboardHandler(value);
                    }
                    catch
                    {
                    }
                }

                try
                {
                    DragonBoy_Net8_Native.Engine.Platform.PlatformService.Current.SetClipboardText(value);
                }
                catch
                {
                }
            }
        }

        private static int s_RaylibMatrixSaveCount = 0;

        /// <summary>
        /// Xoay hệ tọa độ một góc angle quanh điểm tựa pivot.
        /// </summary>
        public static void RotateAroundPivot(float angle, Vector2 pivot)
        {
#if BROWSER
            if (OperatingSystem.IsBrowser())
            {
                DragonBoy_Web_Host.WebGraphicsBackend.RotateAroundPivot(angle, pivot);
                return;
            }
#endif
#if ANDROID || __ANDROID__
            if (OperatingSystem.IsAndroid())
            {
                DragonBoy_Android_Host.AndroidGraphicsBackend.RotateAroundPivot(angle, pivot);
                return;
            }
#endif
            if (!OperatingSystem.IsAndroid() && !OperatingSystem.IsIOS())
            {
                try
                {
                    Raylib_cs.Rlgl.PushMatrix();
                    Raylib_cs.Rlgl.Translatef(pivot.x, pivot.y, 0f);
                    Raylib_cs.Rlgl.Rotatef(angle, 0f, 0f, 1f);
                    Raylib_cs.Rlgl.Translatef(-pivot.x, -pivot.y, 0f);
                    s_RaylibMatrixSaveCount++;
                }
                catch { }
            }
        }

        /// <summary>
        /// Phục hồi lại trạng thái ma trận trước khi xoay.
        /// </summary>
        public static void RestoreMatrix()
        {
#if BROWSER
            if (OperatingSystem.IsBrowser())
            {
                DragonBoy_Web_Host.WebGraphicsBackend.RestoreMatrix();
                return;
            }
#endif
#if ANDROID || __ANDROID__
            if (OperatingSystem.IsAndroid())
            {
                DragonBoy_Android_Host.AndroidGraphicsBackend.RestoreMatrix();
                return;
            }
#endif
            if (!OperatingSystem.IsAndroid() && !OperatingSystem.IsIOS())
            {
                try
                {
                    if (s_RaylibMatrixSaveCount > 0)
                    {
                        Raylib_cs.Rlgl.PopMatrix();
                        s_RaylibMatrixSaveCount--;
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// Dọn sạch ngăn xếp ma trận xoay khi bắt đầu chu kỳ vẽ mới.
        /// </summary>
        public static void ResetMatrixStack()
        {
#if BROWSER
            if (OperatingSystem.IsBrowser())
            {
                DragonBoy_Web_Host.WebGraphicsBackend.ResetMatrixStack();
                return;
            }
#endif
#if ANDROID || __ANDROID__
            if (OperatingSystem.IsAndroid())
            {
                DragonBoy_Android_Host.AndroidGraphicsBackend.ResetMatrixStack();
                return;
            }
#endif
            if (!OperatingSystem.IsAndroid() && !OperatingSystem.IsIOS())
            {
                try
                {
                    while (s_RaylibMatrixSaveCount > 0)
                    {
                        Raylib_cs.Rlgl.PopMatrix();
                        s_RaylibMatrixSaveCount--;
                    }
                }
                catch { }
            }
        }
    }

    /// <summary>
    /// Thành phần phân mảnh (partial) của lớp GUI: Đảm nhiệm phân hệ chuyên trách System ().
    /// </summary>
    public static class GUI
    {
        public static Color color { get; set; } = Color.white;
        private static Matrix4x4 _matrix = Matrix4x4.identity;
        public static Matrix4x4 matrix
        {
            get => _matrix;
            set
            {
                _matrix = value;
                GUIUtility.RestoreMatrix();
            }
        }
        public static GUISkin skin { get; set; } = new GUISkin();
        private static float groupOffsetX = 0;
        private static float groupOffsetY = 0;
        private static bool isScissorActive = false;
        private static readonly System.Collections.Generic.Stack<(float x, float y)> _groupOffsetStack = new System.Collections.Generic.Stack<(float, float)>();

        /// <summary>
        /// Thực thi phương thức nghiệp vụ BeginGroup.
        /// </summary>
        public static void BeginGroup(Rect position)
        {
            _groupOffsetStack.Push((groupOffsetX, groupOffsetY));
            groupOffsetX += position.x;
            groupOffsetY += position.y;
#if BROWSER
            if (OperatingSystem.IsBrowser())
            {
                DragonBoy_Web_Host.WebGraphicsBackend.BeginGroup(position);
                return;
            }
#endif
#if ANDROID || __ANDROID__
            if (OperatingSystem.IsAndroid())
            {
                DragonBoy_Android_Host.AndroidGraphicsBackend.BeginGroup(position);
                return;
            }
#endif
            if (!OperatingSystem.IsAndroid() && !OperatingSystem.IsIOS())
            {
                try
                {
                    int sx = (int)position.x;
                    int sy = (int)position.y;
                    int sw = (int)position.width;
                    int sh = (int)position.height;

                    if (sx < 0) { sw += sx; sx = 0; }
                    if (sy < 0) { sh += sy; sy = 0; }
                    int screenW = Raylib.GetScreenWidth();
                    int screenH = Raylib.GetScreenHeight();
                    if (sx + sw > screenW) sw = screenW - sx;
                    if (sy + sh > screenH) sh = screenH - sy;

                    if (sw > 0 && sh > 0)
                    {
                        Raylib.BeginScissorMode(sx, sy, sw, sh);
                        isScissorActive = true;
                    }
                    else
                    {
                        isScissorActive = false;
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// Thực thi phương thức nghiệp vụ EndGroup.
        /// </summary>
        public static void EndGroup()
        {
            if (_groupOffsetStack.Count > 0)
            {
                var prev = _groupOffsetStack.Pop();
                groupOffsetX = prev.x;
                groupOffsetY = prev.y;
            }
            else
            {
                groupOffsetX = 0f;
                groupOffsetY = 0f;
            }
#if BROWSER
            if (OperatingSystem.IsBrowser())
            {
                DragonBoy_Web_Host.WebGraphicsBackend.EndGroup();
                return;
            }
#endif
#if ANDROID || __ANDROID__
            if (OperatingSystem.IsAndroid())
            {
                DragonBoy_Android_Host.AndroidGraphicsBackend.EndGroup();
                return;
            }
#endif
            if (!OperatingSystem.IsAndroid() && !OperatingSystem.IsIOS())
            {
                if (isScissorActive)
                {
                    try { Raylib.EndScissorMode(); } catch { }
                    isScissorActive = false;
                }
            }
        }

        /// <summary>
        /// Thực thi phương thức nghiệp vụ Label.
        /// </summary>
        public static void Label(Rect position, string text)
        {
            Label(position, text, skin.label);
        }

        /// <summary>
        /// Thực thi phương thức nghiệp vụ Label.
        /// </summary>
        public static void Label(Rect position, string text, GUIStyle style)
        {
            if (string.IsNullOrEmpty(text)) return;
#if BROWSER
            if (OperatingSystem.IsBrowser())
            {
                DragonBoy_Web_Host.WebGraphicsBackend.DrawLabel(position, text, style, groupOffsetX, groupOffsetY, color);
                return;
            }
#endif
#if ANDROID || __ANDROID__
            if (OperatingSystem.IsAndroid())
            {
                DragonBoy_Android_Host.AndroidGraphicsBackend.DrawLabel(position, text, style, groupOffsetX, groupOffsetY, color);
                return;
            }
#endif
            if (OperatingSystem.IsAndroid() || OperatingSystem.IsIOS()) return;
            try
            {
                int px = (int)(position.x + groupOffsetX);
                int py = (int)(position.y + groupOffsetY);

                int fontSize;
                if (style != null && style.fontSize > 0)
                {
                    fontSize = style.fontSize * mGraphics.zoomLevel;
                }
                else
                {
                    fontSize = Font.GetDefaultFontSize(style?.font);
                    int refSize = Font.GetLayoutReferenceFontSize(style?.font);
                    py += (refSize - fontSize) / 2;
                }

                Raylib_cs.Font font = Font.GetRaylibFont(style?.font, fontSize);
                Raylib_cs.Color col = style?.normal?.textColor ?? color;

                if (font.Texture.Id != 0)
                {
                    System.Numerics.Vector2 textSize = Raylib.MeasureTextEx(font, text, fontSize, 0f);
                    if (style != null)
                    {
                        if (style.alignment == TextAnchor.UpperCenter || style.alignment == TextAnchor.MiddleCenter || style.alignment == TextAnchor.LowerCenter)
                        {
                            px += (int)((position.width - textSize.X) / 2);
                        }
                        else if (style.alignment == TextAnchor.UpperRight || style.alignment == TextAnchor.MiddleRight || style.alignment == TextAnchor.LowerRight)
                        {
                            px += (int)(position.width - textSize.X);
                        }
                    }
                    Raylib.DrawTextEx(font, text, new System.Numerics.Vector2(px, py), fontSize, 0f, col);
                }
                else
                {
                    if (style != null)
                    {
                        int textWidth = Raylib.MeasureText(text, fontSize);
                        if (style.alignment == TextAnchor.UpperCenter || style.alignment == TextAnchor.MiddleCenter || style.alignment == TextAnchor.LowerCenter)
                        {
                            px += (int)((position.width - textWidth) / 2);
                        }
                        else if (style.alignment == TextAnchor.UpperRight || style.alignment == TextAnchor.MiddleRight || style.alignment == TextAnchor.LowerRight)
                        {
                            px += (int)(position.width - textWidth);
                        }
                    }
                    Raylib.DrawText(text, px, py, fontSize, col);
                }
            }
            catch { }
        }

        /// <summary>
        /// Thực hiện vẽ và hiển thị thành phần DrawTexture lên canvas đồ họa.
        /// </summary>
        public static void DrawTexture(Rect position, Texture image)
        {
#if BROWSER
            if (OperatingSystem.IsBrowser())
            {
                DragonBoy_Web_Host.WebGraphicsBackend.DrawTextureSolid(position, image, color, groupOffsetX, groupOffsetY);
                return;
            }
#endif
#if ANDROID || __ANDROID__
            if (OperatingSystem.IsAndroid())
            {
                DragonBoy_Android_Host.AndroidGraphicsBackend.DrawTextureSolid(position, image, color, groupOffsetX, groupOffsetY);
                return;
            }
#endif
            if (OperatingSystem.IsAndroid() || OperatingSystem.IsIOS()) return;
            try
            {
                int dx = (int)(position.x + groupOffsetX);
                int dy = (int)(position.y + groupOffsetY);
                int dw = (int)position.width;
                int dh = (int)position.height;

                if (image == Texture2D.whiteTexture || image == null)
                {
                    Raylib.DrawRectangle(dx, dy, dw, dh, color);
                }
                else if (image is Texture2D t2d && t2d.HasRaylibTexture)
                {
                    TextureFilter desiredFilter = (ModGraphics.renderMode == 0) ? TextureFilter.Point : TextureFilter.Bilinear;
                    if (t2d.currentRaylibFilter != desiredFilter)
                    {
                        Raylib.SetTextureFilter(t2d.RaylibTexture, desiredFilter);
                        t2d.currentRaylibFilter = desiredFilter;
                    }
                    Raylib.DrawTexturePro(
                        t2d.RaylibTexture,
                        new Raylib_cs.Rectangle(0, 0, t2d.width, t2d.height),
                        new Raylib_cs.Rectangle(dx, dy, dw, dh),
                        System.Numerics.Vector2.Zero,
                        0f,
                        color
                    );
                }
            }
            catch { }
        }

        /// <summary>
        /// Thực hiện vẽ và hiển thị thành phần DrawCircle lên canvas đồ họa.
        /// </summary>
        public static void DrawCircle(int cx, int cy, int radius, Color fillColor)
        {
#if BROWSER
            if (OperatingSystem.IsBrowser())
            {
                DragonBoy_Web_Host.WebGraphicsBackend.DrawCircle(cx + groupOffsetX, cy + groupOffsetY, radius, fillColor);
                return;
            }
#endif
#if ANDROID || __ANDROID__
            if (OperatingSystem.IsAndroid())
            {
                DragonBoy_Android_Host.AndroidGraphicsBackend.DrawCircle(cx + groupOffsetX, cy + groupOffsetY, radius, fillColor);
                return;
            }
#endif
            if (OperatingSystem.IsAndroid() || OperatingSystem.IsIOS()) return;
            try
            {
                int px = (int)(cx + groupOffsetX);
                int py = (int)(cy + groupOffsetY);
                Raylib.DrawCircle(px, py, radius, fillColor);
            }
            catch { }
        }

        /// <summary>
        /// Thực hiện vẽ và hiển thị thành phần DrawCircleOutline lên canvas đồ họa.
        /// </summary>
        public static void DrawCircleOutline(int cx, int cy, int radius, Color strokeColor)
        {
#if BROWSER
            if (OperatingSystem.IsBrowser())
            {
                DragonBoy_Web_Host.WebGraphicsBackend.DrawCircleOutline(cx + groupOffsetX, cy + groupOffsetY, radius, strokeColor);
                return;
            }
#endif
#if ANDROID || __ANDROID__
            if (OperatingSystem.IsAndroid())
            {
                DragonBoy_Android_Host.AndroidGraphicsBackend.DrawCircleOutline(cx + groupOffsetX, cy + groupOffsetY, radius, strokeColor);
                return;
            }
#endif
            if (OperatingSystem.IsAndroid() || OperatingSystem.IsIOS()) return;
            try
            {
                int px = (int)(cx + groupOffsetX);
                int py = (int)(cy + groupOffsetY);
                Raylib.DrawCircleLines(px, py, radius, strokeColor);
            }
            catch { }
        }
    }

    /// <summary>
    /// Thành phần phân mảnh (partial) của lớp WWW: Đảm nhiệm phân hệ chuyên trách System ().
    /// </summary>
    public class WWW
    {
        public bool isDone { get; private set; } = false;
        public string error { get; private set; } = null;
        public string text { get; private set; } = string.Empty;
        public byte[] bytes { get; private set; } = Array.Empty<byte>();

        /// <summary>
        /// Thực thi phương thức nghiệp vụ WWW.
        /// </summary>
        public WWW(string url)
        {
#if BROWSER
            if (OperatingSystem.IsBrowser())
            {
                DragonBoy_Web_Host.WebAssetLoader.FetchWwwAsync(url, (b, t, err) =>
                {
                    bytes = b ?? Array.Empty<byte>();
                    text = t ?? string.Empty;
                    error = err;
                    isDone = true;
                });
                return;
            }
#endif
            System.Threading.Tasks.Task.Run(async () =>
            {
                try
                {
                    using (var client = new System.Net.Http.HttpClient())
                    {
                        bytes = await client.GetByteArrayAsync(url);
                        text = System.Text.Encoding.UTF8.GetString(bytes);
                    }
                }
                catch (Exception ex)
                {
                    error = ex.Message;
                }
                finally
                {
                    isDone = true;
                }
            });
        }
    }
}

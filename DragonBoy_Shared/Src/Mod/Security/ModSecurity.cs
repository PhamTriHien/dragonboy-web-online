using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace DragonBoy_Net8_Native.Src.Mod.Security
{
    /// <summary>
    /// CLASS ModSecurity: Định nghĩa và điều phối logic trong hệ thống game.
    /// </summary>
    public static class ModSecurity
    {
        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
        private static extern bool IsDebuggerPresent();

        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
        private static extern bool CheckRemoteDebuggerPresent(IntPtr hProcess, ref bool isDebuggerPresent);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetCurrentProcess();

        private static Thread _watchdogThread = null;
        private static volatile bool _isRunning = false;
        private static uint _baselineCodeChecksum = 0;
        private static IntPtr _moduleBase = IntPtr.Zero;
        private static int _moduleScanSize = 65536; // Scan first 64KB of executable code in RAM

        private static readonly string[] BlacklistedTools = new string[]
        {
            "x64dbg",
            "x32dbg",
            "cheatengine",
            "cheatengine-x86_64",
            "cheatengine-i386",
            "ida64",
            "ida",
            "ollydbg",
            "processhacker",
            "scylla",
            "dnspy",
            "de4dot"
        };

        /// <summary>
        /// Khởi tạo và nạp dữ liệu cấu hình cho Initialize.
        /// </summary>
        public static void Initialize()
        {
            if (!OperatingSystem.IsWindows()) return;

            try
            {
                // Initial immediate check
                if (DetectDebugger())
                {
                    TerminateProcess("Debugger detected on startup.");
                }

                // Baseline memory checksum
                try
                {
                    Process cur = Process.GetCurrentProcess();
                    _moduleBase = cur.MainModule.BaseAddress;
                    _baselineCodeChecksum = ComputeMemoryChecksum(_moduleBase, _moduleScanSize);
                }
                catch { }

                Console.WriteLine("[SECURITY] Anti-Debug & Memory Protection initialized.");
            }
            catch { }
        }

        /// <summary>
        /// Thực thi phương thức nghiệp vụ StartWatchdog.
        /// </summary>
        public static void StartWatchdog()
        {
            if (_isRunning || !OperatingSystem.IsWindows()) return;

            _isRunning = true;
            /// <summary>
            /// Thực thi phương thức nghiệp vụ Thread.
            /// </summary>
            _watchdogThread = new Thread(WatchdogLoop)
            {
                Name = "NRO_SecurityWatchdog",
                IsBackground = true,
                Priority = ThreadPriority.Lowest
            };
            _watchdogThread.Start();
            Console.WriteLine("[SECURITY] Security Watchdog active (polling every 2000ms).");
        }

        /// <summary>
        /// Thực thi phương thức nghiệp vụ StopWatchdog.
        /// </summary>
        public static void StopWatchdog()
        {
            _isRunning = false;
        }

        /// <summary>
        /// Thực thi phương thức nghiệp vụ WatchdogLoop.
        /// </summary>
        private static void WatchdogLoop()
        {
            while (_isRunning)
            {
                try
                {
                    Thread.Sleep(2000);
                    if (!_isRunning) break;

                    // 1. Check Debugger
                    if (DetectDebugger())
                    {
                        TerminateProcess("Debugger attached.");
                    }

                    // 2. Check Blacklisted Reversing Tools
                    if (DetectBlacklistedProcesses())
                    {
                        TerminateProcess("Reverse engineering tool detected.");
                    }

                    // 3. Check Memory Code Integrity
                    if (_baselineCodeChecksum != 0 && _moduleBase != IntPtr.Zero)
                    {
                        uint currentChecksum = ComputeMemoryChecksum(_moduleBase, _moduleScanSize);
                        if (currentChecksum != _baselineCodeChecksum)
                        {
                            TerminateProcess("Memory code tampering detected.");
                        }
                    }
                }
                catch
                {
                    // Fail silently, keep watchdog running
                }
            }
        }

        /// <summary>
        /// Thực thi phương thức nghiệp vụ DetectDebugger.
        /// </summary>
        private static bool DetectDebugger()
        {
            try
            {
                if (IsDebuggerPresent())
                {
                    return true;
                }

                bool isRemote = false;
                if (CheckRemoteDebuggerPresent(GetCurrentProcess(), ref isRemote) && isRemote)
                {
                    return true;
                }

                if (Debugger.IsAttached)
                {
                    return true;
                }
            }
            catch { }

            return false;
        }

        /// <summary>
        /// Thực thi phương thức nghiệp vụ DetectBlacklistedProcesses.
        /// </summary>
        private static bool DetectBlacklistedProcesses()
        {
            try
            {
                Process[] processes = Process.GetProcesses();
                foreach (Process p in processes)
                {
                    try
                    {
                        string name = p.ProcessName.ToLowerInvariant();
                        for (int i = 0; i < BlacklistedTools.Length; i++)
                        {
                            if (name.Contains(BlacklistedTools[i]))
                            {
                                return true;
                            }
                        }
                    }
                    catch { }
                }
            }
            catch { }

            return false;
        }

        /// <summary>
        /// Thực thi phương thức nghiệp vụ ComputeMemoryChecksum.
        /// </summary>
        private static unsafe uint ComputeMemoryChecksum(IntPtr baseAddr, int size)
        {
            uint hash = 2166136261;
            try
            {
                byte* ptr = (byte*)baseAddr.ToPointer();
                // Sample every 16 bytes for ultra-fast FNV-1a hashing
                for (int i = 0; i < size; i += 16)
                {
                    hash ^= ptr[i];
                    hash *= 16777619;
                }
            }
            catch
            {
                return 0;
            }
            return hash;
        }

        /// <summary>
        /// Thực thi phương thức nghiệp vụ TerminateProcess.
        /// </summary>
        private static void TerminateProcess(string reason)
        {
            try
            {
                Console.WriteLine($"[SECURITY ALERT] {reason}");
                Process.GetCurrentProcess().Kill();
            }
            catch
            {
                Environment.Exit(0);
            }
        }
    }
}

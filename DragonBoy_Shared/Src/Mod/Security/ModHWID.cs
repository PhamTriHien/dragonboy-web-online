using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Win32;

namespace DragonBoy_Net8_Native.Src.Mod.Security
{
    /// <summary>
    /// CLASS ModHWID: Định nghĩa và điều phối logic trong hệ thống game.
    /// </summary>
    public static class ModHWID
    {
        private const string MasterSecret = "DragonBoy_NativeAOT_Extreme_Security_Key_Salt_2026";
        private static string _cachedHWID = null;

        /// <summary>
        /// Truy xuất và trả về dữ liệu của GetHWID.
        /// </summary>
        public static string GetHWID()
        {
            if (!string.IsNullOrEmpty(_cachedHWID))
            {
                return _cachedHWID;
            }

            StringBuilder sb = new StringBuilder();

            // 1. Windows MachineGuid
            if (OperatingSystem.IsWindows())
            {
                try
                {
                    using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Cryptography"))
                    {
                        if (key != null)
                        {
                            object val = key.GetValue("MachineGuid");
                            if (val != null) sb.Append(val.ToString());
                        }
                    }
                }
                catch { }
            }

            // 2. Processor Identifier
            try
            {
                string procId = Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER");
                if (!string.IsNullOrEmpty(procId)) sb.Append(procId);
                sb.Append(Environment.ProcessorCount);
            }
            catch { }

            // 3. System Drive Volume Serial
            try
            {
                string winDir = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
                string root = Path.GetPathRoot(winDir) ?? "C:\\";
                DriveInfo d = new DriveInfo(root);
                sb.Append(d.TotalSize);
            }
            catch { }

            // Fallback if empty
            if (sb.Length == 0)
            {
                sb.Append(Environment.MachineName).Append(Environment.UserName);
            }

            // Compute SHA256
            using (SHA256 sha = SHA256.Create())
            {
                byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(sb.ToString()));
                string hex = Convert.ToHexString(hash).ToUpperInvariant();
                // Format: DB8-XXXX-XXXX-XXXX-XXXX (16 chars)
                _cachedHWID = $"DB8-{hex.Substring(0, 4)}-{hex.Substring(4, 4)}-{hex.Substring(8, 4)}-{hex.Substring(12, 4)}";
            }

            return _cachedHWID;
        }

        /// <summary>
        /// Thực thi phương thức nghiệp vụ GenerateLicenseKey.
        /// </summary>
        public static string GenerateLicenseKey(string hwid)
        {
            using (HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(MasterSecret)))
            {
                byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(hwid));
                string hex = Convert.ToHexString(hash).ToUpperInvariant();
                return $"LIC-{hex.Substring(0, 4)}-{hex.Substring(4, 4)}-{hex.Substring(8, 4)}-{hex.Substring(12, 4)}";
            }
        }

        /// <summary>
        /// Thực thi phương thức nghiệp vụ ValidateLicense.
        /// </summary>
        public static bool ValidateLicense()
        {
            try
            {
                string hwid = GetHWID();
                string expectedKey = GenerateLicenseKey(hwid);
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string licPath = Path.Combine(baseDir, "license.key");

                // Auto-create valid license for current owner on first run
                if (!File.Exists(licPath))
                {
                    File.WriteAllText(licPath, $"# DragonBoy Native AOT License\n# HWID: {hwid}\nLICENSE_KEY={expectedKey}\n");
                    Console.WriteLine($"[SECURITY] Auto-activated license for current hardware ({hwid}).");
                    return true;
                }

                string[] lines = File.ReadAllLines(licPath);
                foreach (string line in lines)
                {
                    string trimmed = line.Trim();
                    if (trimmed.StartsWith("LICENSE_KEY=", StringComparison.OrdinalIgnoreCase))
                    {
                        string userKey = trimmed.Substring("LICENSE_KEY=".Length).Trim();
                        if (string.Equals(userKey, expectedKey, StringComparison.OrdinalIgnoreCase))
                        {
                            Console.WriteLine($"[SECURITY] License verified successfully. HWID: {hwid}");
                            return true;
                        }
                    }
                }

                Console.WriteLine($"[SECURITY ERROR] Invalid license key! HWID: {hwid}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SECURITY EX] License validation error: {ex.Message}");
                return false;
            }
        }
    }
}

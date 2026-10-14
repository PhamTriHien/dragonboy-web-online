using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

/// <summary>
/// CLASS GameTabRegistry: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
public static class GameTabRegistry
{
    private static readonly object sync = new object();
    private static List<int> ids = new List<int> { 1 };
    private static string loadedPath;
    private static long lastLoad;
    public static string Root => Path.GetDirectoryName(UnityEngine.Application.persistentDataPath);
    private static string FilePath => Path.Combine(Root, "tabs.xml");

    /// <summary>
    /// Truy xuất và trả về dữ liệu của GetTabs.
    /// </summary>
    public static int[] GetTabs()
    {
        lock (sync)
        {
            Load();
            return ids.ToArray();
        }
    }

    public static void Reload()
    {
        lock (sync)
        {
            Load(true);
        }
    }

    /// <summary>
    /// Tìm số thứ tự Tab nhỏ nhất còn trống (bắt đầu từ 2 trở đi: 2, 3, 4...).
    /// </summary>
    private static int GetSmallestAvailableId()
    {
        int candidate = 2;
        while (ids.Contains(candidate))
        {
            candidate = checked(candidate + 1);
        }
        return candidate;
    }

    /// <summary>
    /// Khởi tạo và nạp dữ liệu cấu hình cho Load.
    /// </summary>
    private static void Load(bool force = false)
    {
        if (loadedPath != FilePath)
        {
            ids = new List<int> { 1 };
            loadedPath = FilePath;
            lastLoad = 0;
        }
        if (!force && Environment.TickCount64 - lastLoad < 1000) return;
        lastLoad = Environment.TickCount64;
#if BROWSER
        if (OperatingSystem.IsBrowser()) DragonBoy_Web_Host.WebStorageSync.RefreshSharedFilesFromLocalStorage();
#endif
        if (!File.Exists(FilePath)) return;
        try
        {
            var root = XElement.Load(FilePath);
            if (root.Name != "tabs") return;
            var loaded = root.Elements("tab")
                .Select(e => int.TryParse((string)e.Attribute("id"), out int id) ? id : 0)
                .Where(id => id > 0 && id < int.MaxValue)
                .Distinct()
                .OrderBy(id => id)
                .ToList();
            if (!loaded.Contains(1)) loaded.Insert(0, 1);
            ids = loaded;
        }
        catch (Exception ex) when (ex is IOException || ex is System.Xml.XmlException || ex is FormatException || ex is OverflowException)
        {
            UnityEngine.Debug.LogWarning("Tab registry: " + ex.Message);
        }
    }

    /// <summary>
    /// Thực thi phương thức nghiệp vụ LockFile.
    /// </summary>
    private static FileStream LockFile()
    {
#if BROWSER
        if (OperatingSystem.IsBrowser()) return null;
#endif
        Directory.CreateDirectory(Root);
        for (int attempt = 0; ; attempt++)
        {
            try { return new FileStream(FilePath + ".lock", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None); }
            catch (IOException) when (attempt < 20) { System.Threading.Thread.Sleep(10); }
        }
    }

    /// <summary>
    /// Thực thi phương thức nghiệp vụ Save.
    /// </summary>
    private static void Save()
    {
        Directory.CreateDirectory(Root);
        ids.Sort();
        int nextId = GetSmallestAvailableId();
        var root = new XElement("tabs", new XAttribute("nextId", nextId),
            ids.Select(id => new XElement("tab", new XAttribute("id", id))));
        string temp = FilePath + "." + Environment.ProcessId + ".tmp";
        root.Save(temp);
        File.Move(temp, FilePath, true);
#if BROWSER
        if (OperatingSystem.IsBrowser() && File.Exists(FilePath)) DragonBoy_Web_Host.WebStorageSync.OnTextFileSaved(FilePath, File.ReadAllText(FilePath));
#endif
    }

    /// <summary>
    /// Dọn dẹp thông tin tài khoản cũ trong thư mục tab phụ khi tạo mới hoặc xóa tab.
    /// </summary>
    private static void ResetTabCredentials(int id)
    {
        if (id <= 1) return;
        try
        {
            string tabDir = Path.Combine(Root, "tab" + id);
            if (Directory.Exists(tabDir))
            {
                string[] exactFiles = { "acc", "pass", "check", "sys_dev_id" };
                foreach (string name in exactFiles)
                {
                    string p = Path.Combine(tabDir, name);
                    if (File.Exists(p)) File.Delete(p);
                }
                foreach (string f in Directory.GetFiles(tabDir))
                {
                    string fileName = Path.GetFileName(f);
                    if (fileName.StartsWith("userAo", StringComparison.OrdinalIgnoreCase) ||
                        fileName.StartsWith("passAo", StringComparison.OrdinalIgnoreCase))
                    {
                        File.Delete(f);
                    }
                }
            }
            string statusFile = Path.Combine(Root, $"tab_status_{id}.txt");
            if (File.Exists(statusFile)) File.Delete(statusFile);
            string stopFile = Path.Combine(Root, $"stop_tab_{id}");
            if (File.Exists(stopFile)) File.Delete(stopFile);
        }
        catch { }
    }

    /// <summary>
    /// Thực thi phương thức nghiệp vụ AddTab.
    /// </summary>
    public static int AddTab()
    {
        lock (sync)
        {
            using var fileLock = LockFile();
            Load(true);
            int id = GetSmallestAvailableId();
            ResetTabCredentials(id);
            ids.Add(id);
            ids.Sort();
            Save();
            return id;
        }
    }

    /// <summary>
    /// Thực thi phương thức nghiệp vụ EnsureTab.
    /// </summary>
    public static void EnsureTab(int id)
    {
        if (id < 1) return;
        lock (sync)
        {
            using var fileLock = LockFile();
            Load(true);
            if (ids.Contains(id)) return;
            ids.Add(id);
            ids.Sort();
            Save();
        }
    }

    /// <summary>
    /// Thực thi phương thức nghiệp vụ DeleteTab.
    /// </summary>
    public static bool DeleteTab(int id)
    {
        if (id <= 1) return false;
        lock (sync)
        {
            using var fileLock = LockFile();
            Load(true);
            if (!ids.Remove(id)) return false;
            ResetTabCredentials(id);
            ids.Sort();
            Save();
            return true;
        }
    }

    /// <summary>
    /// Thực thi phương thức nghiệp vụ NextTab.
    /// </summary>
    public static int NextTab(int current)
    {
        int[] tabs = GetTabs();
        int index = Array.IndexOf(tabs, current);
        return tabs[(index + 1) % tabs.Length];
    }
}

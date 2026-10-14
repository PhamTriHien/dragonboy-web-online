using System;
using System.IO;
using System.Collections.Generic;
using DragonBoy_Net8_Native.Engine.Platform;
using UnityEngine;

/// <summary>
/// CLASS GameTabStatus: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
public static class GameTabStatus
{
	/// <summary>
	/// CLASS TabInfo: Định nghĩa và điều phối logic trong hệ thống game.
	/// </summary>
	public class TabInfo
	{
		public int tabId;
		public bool isOnline;
		public bool isConnecting;
		public bool isAutoRunning;
		public string charName = string.Empty;
		public string mapName = string.Empty;
		public int zoneId = 0;
		public long currentHp = 0;
		public long maxHp = 0;
		public string autoSummary = "Không";
		public long lastTick = 0;

		public bool IsAlive => (isOnline || isConnecting) && (mSystem.currentTimeMillis() - lastTick < 10000);
	}

	private static readonly Dictionary<int, TabInfo> _cachedTabs = new Dictionary<int, TabInfo>();
	private static long _lastDiskUpdate = 0;
	private static long _lastDiskRead = 0;

	/// <summary>
	/// Thực thi phương thức nghiệp vụ GameTabStatus.
	/// </summary>
	static GameTabStatus()
	{
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của GetStatusDirectory.
	/// </summary>
	private static string GetStatusDirectory()
	{
		try
		{
			string root = Path.GetDirectoryName(Application.persistentDataPath);

			if (!Directory.Exists(root))
			{
				Directory.CreateDirectory(root);
			}
			return root;
		}
		catch
		{
			return AppDomain.CurrentDomain.BaseDirectory;
		}
	}

	/// <summary>
	/// Cập nhật trạng thái logic của UpdateCurrentTab theo chu kỳ khung hình.
	/// </summary>
	public static void UpdateCurrentTab()
	{
		long now = mSystem.currentTimeMillis();
		int curTab = Application.CurrentTabId;
		if (curTab < 1) return;

		TabInfo info = GetTabInfo(curTab);
		info.tabId = curTab;
		info.lastTick = now;

		bool inGame = ModMenu.IsInGame();
		Char me = inGame ? Char.myCharz() : null;

		if (inGame && me != null)
		{
			GameTabManager.isTabExplicitlyStopped = false;
			info.isOnline = true;
			info.isConnecting = false;
			info.charName = me.cName ?? "Nhân vật";
			info.currentHp = me.cHP;
			info.maxHp = (me.cHPFull > 0) ? me.cHPFull : me.cHP;
			info.zoneId = TileMap.zoneID;

			string mName = (TileMap.mapNames != null && TileMap.mapID >= 0 && TileMap.mapID < TileMap.mapNames.Length)
				? TileMap.mapNames[TileMap.mapID]
				: ("Map " + TileMap.mapID);
			info.mapName = mName;

			// Kiểm tra trạng thái Auto đang chạy
			bool isAuto = ModTanSat.autoTanSat || ModSetActivator.isActive || ModAutoPick.autoPick || GameScr.isAutoPlay;
			info.isAutoRunning = isAuto;

			if (ModTanSat.autoTanSat) info.autoSummary = "Tàn Sát";
			else if (ModSetActivator.isActive) info.autoSummary = "Úp Set KH";
			else if (ModAutoPick.autoPick) info.autoSummary = "Tự Nhặt";
			else if (GameScr.isAutoPlay) info.autoSummary = "Auto Đánh";
			else info.autoSummary = "Đang Chơi";
		}
		else if (DragonBoy_Net8_Native.Src.Mod.Security.ModCredentialSecurity.isLoggingIn || ServerListScreen.waitToLogin || LoginScr.isContinueToLogin)
		{
			GameTabManager.isTabExplicitlyStopped = false;
			info.isOnline = false;
			info.isConnecting = true;
			info.isAutoRunning = false;
			info.autoSummary = "Đang đăng nhập...";
		}
		else if (GameTabManager.isTabExplicitlyStopped)
		{
			info.isOnline = false;
			info.isConnecting = false;
			info.isAutoRunning = false;
			info.autoSummary = "Đã dừng (Chưa vào game)";
		}
		else
		{
			info.isOnline = false;
			info.isConnecting = true;
			info.isAutoRunning = false;
			info.autoSummary = "Đang ở sảnh chờ";
		}

		// Ghi trạng thái xuống ổ đĩa mỗi 1.5 giây để các tab khác đọc được
		if (now - _lastDiskUpdate > 1500)
		{
			SaveCurrentTabToDisk(curTab, info, now);
		}
	}

	public static void ForceSaveCurrentTab()
	{
		UpdateCurrentTab();
		int curTab = Application.CurrentTabId;
		if (curTab < 1) return;
		SaveCurrentTabToDisk(curTab, GetTabInfo(curTab), mSystem.currentTimeMillis());
	}

	private static void SaveCurrentTabToDisk(int curTab, TabInfo info, long now)
	{
		_lastDiskUpdate = now;
		try
		{
			string dir = GetStatusDirectory();
			string filePath = Path.Combine(dir, $"tab_status_{curTab}.txt");
			string content = $"{info.tabId}|{info.isOnline}|{info.isConnecting}|{info.isAutoRunning}|{info.charName}|{info.mapName}|{info.zoneId}|{info.currentHp}|{info.maxHp}|{info.autoSummary}|{info.lastTick}";
			File.WriteAllText(filePath, content);
#if BROWSER
			if (OperatingSystem.IsBrowser()) DragonBoy_Web_Host.WebStorageSync.OnTextFileSaved(filePath, content);
#endif
		}
		catch { }
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ RefreshAllTabs.
	/// </summary>
	public static void RefreshAllTabs(bool force = false)
	{
		long now = mSystem.currentTimeMillis();
		if (!force && now - _lastDiskRead < 1000) return;
		_lastDiskRead = now;

#if BROWSER
		if (OperatingSystem.IsBrowser()) DragonBoy_Web_Host.WebStorageSync.RefreshSharedFilesFromLocalStorage();
#endif

		int curTab = Application.CurrentTabId;
		string dir = GetStatusDirectory();

		foreach (int t in GameTabRegistry.GetTabs())
		{
			if (t == curTab) continue; // Tab hiện tại lấy từ bộ nhớ trong

			try
			{
				string filePath = Path.Combine(dir, $"tab_status_{t}.txt");
				if (File.Exists(filePath))
				{
					string text = File.ReadAllText(filePath);
					if (!string.IsNullOrEmpty(text))
					{
						string[] parts = text.Split('|');
						if (parts.Length >= 11)
						{
							TabInfo tab = GetTabInfo(t);
							int.TryParse(parts[0], out tab.tabId);
							bool.TryParse(parts[1], out tab.isOnline);
							bool.TryParse(parts[2], out tab.isConnecting);
							bool.TryParse(parts[3], out tab.isAutoRunning);
							tab.charName = parts[4];
							tab.mapName = parts[5];
							int.TryParse(parts[6], out tab.zoneId);
							long.TryParse(parts[7], out tab.currentHp);
							long.TryParse(parts[8], out tab.maxHp);
							tab.autoSummary = parts[9];
							long.TryParse(parts[10], out tab.lastTick);
						}
					}
				}
				else
				{
					TabInfo tab = GetTabInfo(t);
					tab.isOnline = false;
					tab.isConnecting = false;
					tab.isAutoRunning = false;
				}
			}
			catch { }
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ MarkTabStopped.
	/// </summary>
	public static void MarkTabStopped(int tabId)
	{
		if (tabId < 1) return;
		try
		{
			TabInfo tab = GetTabInfo(tabId);
			tab.isOnline = false;
			tab.isConnecting = false;
			tab.isAutoRunning = false;
			tab.autoSummary = "Đã dừng (Offline)";
			tab.lastTick = 0;

			string dir = GetStatusDirectory();
			string filePath = Path.Combine(dir, $"tab_status_{tabId}.txt");
			if (File.Exists(filePath))
			{
				File.Delete(filePath);
#if BROWSER
				if (OperatingSystem.IsBrowser()) DragonBoy_Web_Host.WebStorageSync.OnFileDeleted(filePath);
#endif
			}
		}
		catch { }
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của GetTabInfo.
	/// </summary>
	public static TabInfo GetTabInfo(int tabId)
	{
		if (tabId < 1) return new TabInfo { tabId = tabId };
		if (!_cachedTabs.TryGetValue(tabId, out TabInfo tab))
		{
			tab = new TabInfo { tabId = tabId };
			_cachedTabs.Add(tabId, tab);
		}
		return tab;
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của GetStatusColor.
	/// </summary>
	public static int GetStatusColor(int tabId)
	{
		TabInfo tab = GetTabInfo(tabId);
		if (tab.tabId == Application.CurrentTabId && tab.isOnline)
		{
			return 0x00e676; // Xanh lá sáng (Đang chạy & Đang xem)
		}
		if (tab.IsAlive)
		{
			if (tab.isAutoRunning) return 0x00e676; // Xanh lá (Đang Auto)
			if (tab.isOnline) return 0x29b6f6;     // Xanh dương (Đang online)
			if (tab.isConnecting) return 0xffb300; // Vàng (Đang kết nối)
		}
		return 0x888888; // Xám (Đã dừng / Offline)
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của GetStatusText.
	/// </summary>
	public static string GetStatusText(int tabId)
	{
		TabInfo tab = GetTabInfo(tabId);
		if (tab.IsAlive && tab.isOnline)
		{
			string name = !string.IsNullOrEmpty(tab.charName) ? tab.charName : "Online";
			string map = !string.IsNullOrEmpty(tab.mapName) ? $"{tab.mapName} (K{tab.zoneId})" : "";
			string auto = tab.autoSummary;
			return $"{name} | {map} | {auto}";
		}
		if (tab.IsAlive && tab.isConnecting)
		{
			return !string.IsNullOrEmpty(tab.autoSummary) ? tab.autoSummary : "Đang ở sảnh chờ";
		}
		return "Đã dừng (Chưa vào game)";
	}
}

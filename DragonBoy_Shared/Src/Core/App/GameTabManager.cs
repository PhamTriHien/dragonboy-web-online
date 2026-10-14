using System;
using System.IO;
using DragonBoy_Net8_Native.Engine.Platform;
using UnityEngine;

/// <summary>
/// CLASS GameTabManager: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
public static class GameTabManager
{
	public static int CurrentTab => Application.CurrentTabId;
	public static bool isTabExplicitlyStopped;
	private static long lastCommandCheck;

	/// <summary>
	/// Thực thi phương thức nghiệp vụ OpenTab.
	/// </summary>
	public static void OpenTab(int tabId, bool closeHub = true)
	{
		if (tabId < 1) return;
		if (tabId == CurrentTab)
		{
			isTabExplicitlyStopped = false;
			ServerListScreen.isAutoConect = true;
		}
		GameTabRegistry.EnsureTab(tabId);
		string stopPath = Path.Combine(GameTabRegistry.Root, "stop_tab_" + tabId);
		if (File.Exists(stopPath)) File.Delete(stopPath);
		DefaultDesktopBridge.RequestSwitchTab(tabId, closeHub);
	}

	public static void RequestPrewarmTab(int tabId)
	{
		if (tabId <= 1) return;
		GameTabRegistry.EnsureTab(tabId);
		DefaultDesktopBridge.HostPrewarmTab?.Invoke(tabId);
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ RequestStopTab.
	/// </summary>
	public static void RequestStopTab(int tabId)
	{
		if (tabId < 1) return;
		if (DefaultDesktopBridge.HostStopTab != null)
		{
			DefaultDesktopBridge.HostStopTab(tabId);
			return;
		}
		if (tabId == CurrentTab)
		{
			StopTab(tabId);
		}
		else
		{
			try
			{
				Directory.CreateDirectory(GameTabRegistry.Root);
				File.WriteAllText(Path.Combine(GameTabRegistry.Root, "stop_tab_" + tabId), "stop");
				GameTabStatus.MarkTabStopped(tabId);
				GameScr.info1?.addInfo("Đã dừng Tab " + tabId + "!", 0);
			}
			catch { }
		}
	}

	public static void RequestDeleteTab(int tabId)
	{
		if (tabId <= 1) return;
		if (DefaultDesktopBridge.HostDeleteTab != null)
		{
			DefaultDesktopBridge.HostDeleteTab(tabId);
			return;
		}
		RequestStopTab(tabId);
		GameTabRegistry.DeleteTab(tabId);
	}

	/// <summary>
	/// Cập nhật trạng thái logic của UpdateCommands theo chu kỳ khung hình.
	/// </summary>
	public static void UpdateCommands()
	{
		long now = mSystem.currentTimeMillis();
		if (now - lastCommandCheck < 250) return;
		lastCommandCheck = now;
		string path = Path.Combine(GameTabRegistry.Root, "stop_tab_" + CurrentTab);
		if (!File.Exists(path)) return;
		File.Delete(path);
		StopTab(CurrentTab);
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ SwitchToTab.
	/// </summary>
	public static void SwitchToTab(int targetTab)
	{
		if (targetTab < 1) targetTab = 1;
		GameTabRegistry.EnsureTab(targetTab);

		if (Application.CurrentTabId == targetTab)
		{
			GameScr.info1?.addInfo("Đang ở Tab " + targetTab + "!", 0);
			return;
		}

		try
		{
			ModAutoLogin.ResetSession();
			GameTabStatus.MarkTabStopped(Application.CurrentTabId);
			ServerListScreen.waitToLogin = false;
			LoginScr.isContinueToLogin = false;
			Application.CurrentTabId = targetTab;
			isTabExplicitlyStopped = false;
			DragonBoy_Net8_Native.Src.Mod.Security.ModCredentialSecurity.ResetForTab();

			if (!OperatingSystem.IsAndroid() && !OperatingSystem.IsIOS() && !OperatingSystem.IsBrowser())
			{
				try
				{
					string windowTitle = (targetTab > 1)
						? "DragonBoy (Tab " + targetTab + ") - Mod TriHienKun [.NET 8 Native]"
						: "DragonBoy - Mod TriHienKun [.NET 8 Native]";
					Raylib_cs.Raylib.SetWindowTitle(windowTitle);
				}
				catch { }
			}

			try
			{
				Session_ME.gI()?.close();
				Session_ME2.gI()?.close();
			}
			catch { }

			GameScr.isAutoPlay = false;
			GameScr.canAutoPlay = false;
			ModTanSat.autoTanSat = false;
			ModAutoPick.autoPick = false;
			ModAutoHeal.autoPean = false;
			ModGoBack.isGoBackActive = false;
			ModSetActivator.isActive = false;

			try
			{
				ModConfig.LoadConfig();
			}
			catch { }

			GameCanvas.clearAllPointerEvent();
			GameCanvas.clearKeyPressed();
			GameCanvas.clearKeyHold();

			try
			{
				ServerListScreen.loadIP();
				SplashScr.loadIP();

				if (GameCanvas.loginScr != null)
				{
					GameCanvas.loginScr.setUserPass();
				}

				if (GameCanvas.serverScreen != null)
				{
					GameCanvas.serverScreen.switchToMe();
				}
				else if (GameCanvas.loginScr != null)
				{
					GameCanvas.loginScr.switchToMe();
				}
			}
			catch { }

			SoundMn.gI()?.buttonClick();
			GameScr.info1?.addInfo("Đã chuyển sang Tab " + targetTab + " (Dữ liệu: tab" + targetTab + ")", 0);
		}
		catch (Exception ex)
		{
			Debug.LogWarning("SwitchToTab error: " + ex.Message);
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ StopTab.
	/// </summary>
	public static void StopTab(int targetTab)
	{
		if (targetTab < 1) targetTab = 1;
		if (targetTab != Application.CurrentTabId) return;

		try
		{
			isTabExplicitlyStopped = true;
			ModAutoLogin.ResetSession();
			DragonBoy_Net8_Native.Src.Mod.Security.ModCredentialSecurity.OnLoginFinished();
			ServerListScreen.waitToLogin = false;
			ServerListScreen.isAutoLogin = false;
			ServerListScreen.isAutoConect = false;
			LoginScr.isContinueToLogin = false;

			GameScr.isAutoPlay = false;
			GameScr.canAutoPlay = false;
			ModTanSat.autoTanSat = false;
			ModAutoPick.autoPick = false;
			ModAutoHeal.autoPean = false;
			ModGoBack.isGoBackActive = false;
			ModSetActivator.isActive = false;
			ModNextMap.StopNextMap();
			ModConfig.SaveConfig();

			try
			{
				Session_ME.gI()?.close();
				Session_ME2.gI()?.close();
			}
			catch { }

			try
			{
				if (GameCanvas.serverScreen != null)
				{
					GameCanvas.serverScreen.switchToMe2();
				}
				else if (GameCanvas.loginScr != null)
				{
					GameCanvas.loginScr.switchToMe();
				}
			}
			catch { }

			GameTabStatus.MarkTabStopped(targetTab);
			SoundMn.gI()?.buttonClose();
			GameScr.info1?.addInfo("ĐÃ DỪNG TAB " + targetTab + "!", 0);
		}
		catch (Exception ex)
		{
			Debug.LogWarning("StopTab error: " + ex.Message);
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ CycleNextTab.
	/// </summary>
	public static void CycleNextTab()
	{
		int nextTab = GameTabRegistry.NextTab(Application.CurrentTabId);
		PlatformService.Current.SwitchToTab(nextTab);
	}
}

using System;
using UnityEngine;

/// <summary>
/// Module Điều Tốc FPS: Đồng bộ tần số quét màn hình (Auto FPS) và khóa cứng các mốc FPS ổn định.
/// </summary>
public static class ModFps
{
	public static int targetFps = 144;
	public static bool isAutoFps = true;
	public static int pingMs = 25;

	private static GUIStyle pingFpsStyle;
	private static GUIStyle pingFpsStyleShadow;

	/// <summary>
	/// Truy xuất và trả về dữ liệu của GetDeviceMaxRefreshRate.
	/// </summary>
	public static int GetDeviceMaxRefreshRate()
	{
		try
		{
			int rate = DragonBoy_Net8_Native.Engine.Platform.PlatformService.Current.GetMaxRefreshRate();
			if (rate <= 0)
			{
				rate = 60;
			}
			return rate;
		}
		catch
		{
			return 60;
		}
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của GetBestFpsForDevice.
	/// </summary>
	public static int GetBestFpsForDevice()
	{
		int max = GetDeviceMaxRefreshRate();
		if (max >= 240) return 240;
		if (max >= 185) return 185;
		if (max >= 165) return 165;
		if (max >= 144) return 144;
		if (max >= 120) return 120;
		if (max >= 90) return 90;
		return 60;
	}

	/// <summary>
	/// Khởi tạo và nạp dữ liệu cấu hình cho LoadFPS.
	/// </summary>
	public static void LoadFPS()
	{
		try
		{
			if (isAutoFps)
			{
				targetFps = GetBestFpsForDevice();
			}
			ApplyFPS();
		}
		catch
		{
			targetFps = 144;
			ApplyFPS();
		}
	}

	/// <summary>
	/// Áp dụng các thay đổi cấu hình hoặc hiệu ứng của ApplyFPS.
	/// </summary>
	public static void ApplyFPS()
	{
		try
		{
			QualitySettings.vSyncCount = 0;
			Application.targetFrameRate = targetFps;
		}
		catch
		{
		}
	}

	/// <summary>
	/// Thiết lập giá trị hoặc trạng thái cho SetAutoFPS.
	/// </summary>
	public static void SetAutoFPS()
	{
		isAutoFps = true;
		targetFps = GetBestFpsForDevice();
		ApplyFPS();
		ModConfig.SaveConfig();
	}

	/// <summary>
	/// Thiết lập giá trị hoặc trạng thái cho SetFPS.
	/// </summary>
	public static void SetFPS(int fps)
	{
		isAutoFps = false;
		targetFps = fps;
		ApplyFPS();
		ModConfig.SaveConfig();
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ CycleFPS.
	/// </summary>
	public static void CycleFPS()
	{
		int[] steps = new int[8] { 30, 60, 90, 120, 144, 165, 185, 240 };
		int idx = Array.IndexOf(steps, targetFps);
		if (idx == -1)
		{
			targetFps = 144;
			isAutoFps = false;
		}
		else if (idx >= steps.Length - 1)
		{
			SetAutoFPS();
			return;
		}
		else
		{
			isAutoFps = false;
			targetFps = steps[idx + 1];
		}
		ApplyFPS();
		ModConfig.SaveConfig();
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của GetFpsCaption.
	/// </summary>
	public static string GetFpsCaption()
	{
		if (isAutoFps)
		{
			return "FPS: " + targetFps + " (Auto " + GetDeviceMaxRefreshRate() + "Hz)";
		}
		return "FPS: " + targetFps;
	}

	/// <summary>
	/// Thực hiện vẽ và hiển thị thành phần PaintFPS lên canvas đồ họa.
	/// </summary>
	public static void PaintFPS(mGraphics g)
	{
		try
		{
			if (g == null)
			{
				return;
			}

			// Tự động ẩn FPS khi mở Hành trang (Panel), Menu, Hộp thoại hoặc giao diện Mod
			if ((GameCanvas.panel != null && GameCanvas.panel.isShow) ||
			    (GameCanvas.panel2 != null && GameCanvas.panel2.isShow) ||
			    (GameCanvas.menu != null && GameCanvas.menu.showMenu) ||
			    GameCanvas.currentDialog != null ||
			    ModUI.uiCustomOpen)
			{
				return;
			}

			g.translate(-g.getTranslateX(), -g.getTranslateY());
			g.setClip(0, 0, GameCanvas.w, GameCanvas.h);

			int curFps = (Main.realFPS > 0) ? Main.realFPS : targetFps;
			if (curFps > 999) curFps = 999;
			if (curFps < 0) curFps = 0;

			int drawX;
			int drawY;
			if (ModMenu.IsInGame())
			{
				drawX = 160;
				drawY = 4;
			}
			else
			{
				drawX = 8;
				drawY = 6;
			}

			string text = curFps + "fps - " + pingMs + "ms";
			if (mFont.tahoma_7_grey != null)
			{
				mFont.tahoma_7_grey.drawString(g, text, drawX + 1, drawY + 1, mFont.LEFT);
			}
			if (mFont.tahoma_7_green != null)
			{
				mFont.tahoma_7_green.drawString(g, text, drawX, drawY, mFont.LEFT);
			}
			else if (mFont.tahoma_7_white != null)
			{
				mFont.tahoma_7_white.drawString(g, text, drawX, drawY, mFont.LEFT);
			}
		}
		catch
		{
		}
	}
}

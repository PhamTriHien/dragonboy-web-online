using System;
using UnityEngine;

/// <summary>
/// Bộ quản lý đồ họa Mod: Điều phối độ phân giải cửa sổ, khử răng cưa GPU và 2 chế độ render (Java Gốc vs Bản HD).
/// </summary>
public static class ModGraphics
{
	// 0: Ultra (mặc định), 1: Medium (xóa hiệu ứng động), 2: Low (xóa bg), 3: Super Low
	public static int graphicsQuality = 0;
	public static readonly string[] graphicsNames = new string[4] { "Ultra", "Medium", "Low", "Super Low" };

	// Chế độ Treo Giảm CPU (giới hạn 15 FPS + tối giản hiệu ứng nền)
	public static bool isAfkLowCpu = false;

	public static void ToggleAfkLowCpu()
	{
		isAfkLowCpu = !isAfkLowCpu;
		if (isAfkLowCpu)
		{
			Application.targetFrameRate = 15;
		}
		else
		{
			ModFps.ApplyFPS();
		}
		ModConfig.SaveConfig();
		GameScr.info1?.addInfo("Chế độ Treo Giảm CPU (15 FPS): " + (isAfkLowCpu ? "BẬT" : "TẮT"), 0);
		SoundMn.gI().buttonClick();
	}

	// 2 chế độ đồ hoạ: 0: Đồ Hoạ Java (tài nguyên x1 + font bitmap từ src Java gốc), 1: Đồ Hoạ HD (tài nguyên x2 lọc Bilinear + font hệ thống)
	public static int renderMode = 0;
	public static bool IsJavaGraphics => renderMode == 0;
	public static readonly string[] renderModeNames = new string[2] { "Đồ Hoạ Java", "Đồ Hoạ HD" };

	public static int renderBitDepth
	{
		get => renderMode;
		set => renderMode = (value >= 1) ? 1 : 0;
	}
	public static string[] bitDepthNames => renderModeNames;

	// 0: 1024x600 (Gốc), 1: 1280x720 (HD 16:9), 2: 1600x900 (HD+), 3: 1920x1080 (Full HD)
	public static int resolutionIndex = 0;
	public static readonly int[,] resolutionList = new int[4, 2]
	{
		{ 1024, 600 },
		{ 1280, 720 },
		{ 1600, 900 },
		{ 1920, 1080 }
	};
	public static readonly string[] resolutionNames = new string[4] { "1024x600", "1280x720", "1600x900", "1920x1080" };
	public static bool isFullscreen = false;

	// Chủ đề Font chữ: 0: NRO Gốc, 1: Tahoma, 2: Segoe UI, 3: Consolas
	public static int fontTheme = 0;
	public static readonly string[] fontThemeNames = new string[4] { "NRO Gốc", "Tahoma", "Segoe UI", "Consolas" };

	// Kích thước Font chữ: 0: Chuẩn 60%, 1: Vừa 75%, 2: Lớn 90%, 3: Gốc 100%
	public static int fontSizeIndex = 0;
	public static readonly string[] fontSizeNames = new string[4] { "Chuẩn 60%", "Vừa 75%", "Lớn 90%", "Gốc 100%" };
	public static readonly float[] fontSizeScales = new float[4] { 0.60f, 0.75f, 0.90f, 1.0f };

	private static int lastScreenWidth = 0;
	private static int lastScreenHeight = 0;
	private static bool lastFullscreen = false;

	/// <summary>
	/// Khởi tạo hệ thống đồ họa, thiết lập khử răng cưa GPU và kích thước cửa sổ.
	/// </summary>
	public static void InitGraphics()
	{
		try
		{
			// Kích hoạt bộ lọc nét đồ họa phần cứng GPU trong Unity, chống nhòe mờ
			QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;
			QualitySettings.antiAliasing = 4;
			QualitySettings.masterTextureLimit = 0;
			QualitySettings.vSyncCount = 0;

			// Mặc định game luôn mở ở dạng cửa sổ nhỏ hiển thị giữa màn hình desktop
			isFullscreen = false;
			if (resolutionIndex < 0 || resolutionIndex >= resolutionNames.Length)
			{
				resolutionIndex = 0;
			}
			if (Application.CurrentTabId == 1 || OperatingSystem.IsBrowser())
			{
				ApplyResolution(resolutionIndex);
			}
			ApplyBitDepth();
		}
		catch
		{
		}
	}

	/// <summary>
	/// Bật hoặc tắt chế độ hiển thị toàn màn hình (Fullscreen).
	/// </summary>
	public static void ToggleFullscreen()
	{
		try
		{
			bool target = !Screen.fullScreen;
			DragonBoy_Net8_Native.Engine.Platform.PlatformService.Current.ToggleFullscreen();
			isFullscreen = OperatingSystem.IsBrowser() ? target : Screen.fullScreen;
			ModConfig.SaveConfig();
		}
		catch
		{
		}
	}

	/// <summary>
	/// Áp dụng độ phân giải cửa sổ hiển thị theo chỉ số cấu hình được chọn.
	/// </summary>
	public static void ApplyResolution(int index)
	{
		try
		{
			if (index < 0 || index >= resolutionNames.Length) index = 0;
			resolutionIndex = index;
			int w = resolutionList[index, 0];
			int h = resolutionList[index, 1];
			bool wantFs = isFullscreen && Application.CurrentTabId == 1;
			Screen.SetResolution(w, h, wantFs);
			ModConfig.SaveConfig();
		}
		catch
		{
		}
	}

	/// <summary>
	/// Cập nhật trạng thái logic của UpdateResolutionWatcher theo chu kỳ khung hình.
	/// </summary>
	public static void UpdateResolutionWatcher()
	{
		try
		{
			if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight || Screen.fullScreen != lastFullscreen)
			{
				lastScreenWidth = Screen.width;
				lastScreenHeight = Screen.height;
				lastFullscreen = Screen.fullScreen;
				isFullscreen = Screen.fullScreen;

				ScaleGUI.WIDTH = Screen.width;
				ScaleGUI.HEIGHT = Screen.height;
				if (MotherCanvas.instance != null)
				{
					MotherCanvas.instance.checkZoomLevel(Screen.width, Screen.height);
				}
				if (GameMidlet.gameCanvas != null)
				{
					GameMidlet.gameCanvas.initGameCanvas();
				}
				if (TileMap.tileID > 0)
				{
					TileMap.loadTileImage();
					TileMap.loadMainTile();
				}
				if (GameScr.instance != null && GameCanvas.currentScreen == GameScr.instance && TileMap.tmw > 0 && TileMap.tmh > 0)
				{
					GameScr.loadCamera(true, -1, -1);
				}
				else
				{
					GameScr.gW = GameCanvas.w;
					GameScr.gH = GameCanvas.h;
					GameScr.gW2 = GameScr.gW >> 1;
					GameScr.gH2 = GameScr.gH >> 1;
					GameScr.gH23 = GameScr.gH * 2 / 3;
					if (GameScr.cmy < 0)
					{
						GameScr.cmy = (GameScr.cmtoY = 0);
					}
					if (GameScr.cmx < 0)
					{
						GameScr.cmx = (GameScr.cmtoX = 0);
					}
				}
				GameCanvas.getYBackground(GameCanvas.typeBg);
				if (GameScr.gamePad != null)
				{
					GameScr.gamePad.updateZone();
				}
				GameScr.setSkillBarPosition();
				if (GameCanvas.loginScr != null)
				{
					GameCanvas.loginScr.updatePosition();
				}
				if (GameCanvas.serverScreen != null && GameCanvas.currentScreen == GameCanvas.serverScreen)
				{
					ServerListScreen.createDeleteRMS();
					GameCanvas.serverScreen.initCommand();
				}
			}
		}
		catch
		{
		}
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của GetCurrentQualityName.
	/// </summary>
	public static string GetCurrentQualityName()
	{
		if (graphicsQuality >= 0 && graphicsQuality < graphicsNames.Length)
		{
			return graphicsNames[graphicsQuality];
		}
		return graphicsNames[0];
	}

	/// <summary>
	/// Thiết lập giá trị hoặc trạng thái cho SetQuality.
	/// </summary>
	public static void SetQuality(int quality)
	{
		if (quality >= 0 && quality < graphicsNames.Length)
		{
			graphicsQuality = quality;
			ModConfig.SaveConfig();
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ CycleQuality.
	/// </summary>
	public static void CycleQuality()
	{
		graphicsQuality = (graphicsQuality + 1) % graphicsNames.Length;
		ModConfig.SaveConfig();
	}

	/// <summary>
	/// Đổi chế độ đồ hoạ (0: Đồ Hoạ Java, 1: Đồ Hoạ HD), lưu cấu hình và nạp lại tài nguyên trực tiếp.
	/// </summary>
	public static void SetRenderMode(int mode, bool refreshAssets = true)
	{
		int normalized = (mode >= 1) ? 1 : 0;
		bool changed = (renderMode != normalized);
		renderMode = normalized;
		ApplyRenderMode();
		if (refreshAssets)
		{
			ModConfig.SaveConfig();
			if (changed && GameMidlet.gameCanvas != null)
			{
				RefreshGraphicsAssets();
			}
		}
	}

	public static void SetBitDepth(int depth) => SetRenderMode(depth);

	public static void RefreshGraphicsAssets()
	{
		try
		{
			if (GameMidlet.gameCanvas == null)
			{
				return;
			}
			if (mGraphics.cachedTextures != null)
			{
				mGraphics.cachedTextures.Clear();
			}
			mSystem.ClearImageCache();
			mFont.ReloadJavaFontImages();
			mFont.InvalidateFontMetrics();
			Paint.loadbg();
			Paint.imgCheck = GameCanvas.loadImage("/mainImage/myTexture2dcheck.png");
			for (int i = 0; i < 3; i++)
			{
				GameCanvas.imgBorder[i] = GameCanvas.loadImage("/mainImage/myTexture2dbd" + i + ".png");
			}
			GameCanvas.borderConnerW = mGraphics.getImageWidth(GameCanvas.imgBorder[0]);
			GameCanvas.borderConnerH = mGraphics.getImageHeight(GameCanvas.imgBorder[0]);
			GameCanvas.borderCenterW = mGraphics.getImageWidth(GameCanvas.imgBorder[1]);
			GameCanvas.borderCenterH = mGraphics.getImageHeight(GameCanvas.imgBorder[1]);
			GameCanvas.imgShuriken = GameCanvas.loadImage("/mainImage/myTexture2df.png");
			GameCanvas.imgClear = GameCanvas.loadImage("/mainImage/myTexture2der.png");
			Command.btn0left = GameCanvas.loadImage("/mainImage/btn0left.png");
			Command.btn0mid = GameCanvas.loadImage("/mainImage/btn0mid.png");
			Command.btn0right = GameCanvas.loadImage("/mainImage/btn0right.png");
			Command.btn1left = GameCanvas.loadImage("/mainImage/btn1left.png");
			Command.btn1mid = GameCanvas.loadImage("/mainImage/btn1mid.png");
			Command.btn1right = GameCanvas.loadImage("/mainImage/btn1right.png");
			if (GameCanvas.instance != null)
			{
				GameCanvas.instance.loadDust();
				GameCanvas.instance.loadWaterSplash();
			}
			GameScr.ReloadHudImages();
			SmallImage.ReloadForGraphicsMode();
			TileMap.loadBg();
			if (TileMap.tileID > 0)
			{
				TileMap.loadTileImage();
				TileMap.loadMainTile();
			}
			BgItem.ReloadCurrMapItems();
			BackgroudEffect.initCloud();
			if (GameCanvas.typeBg >= 0)
			{
				int curBg = GameCanvas.typeBg;
				TileMap.lastBgID = -1;
				GameCanvas.loadBG(curBg);
			}
			mScreen.ITEM_HEIGHT = mFont.tahoma_8b.getHeight() + 8;
		}
		catch
		{
		}
	}

	/// <summary>
	/// Truy xuất hệ số tỷ lệ kích thước font chữ hiện tại.
	/// </summary>
	public static float GetFontSizeScale()
	{
		if (fontSizeIndex >= 0 && fontSizeIndex < fontSizeScales.Length)
		{
			return fontSizeScales[fontSizeIndex];
		}
		return 0.60f;
	}

	/// <summary>
	/// Thiết lập chủ đề font chữ và làm mới bộ đệm đo đạc văn bản.
	/// </summary>
	public static void SetFontTheme(int theme)
	{
		if (theme < 0 || theme >= fontThemeNames.Length) theme = 0;
		fontTheme = theme;
		mFont.InvalidateFontMetrics();
		ModConfig.SaveConfig();
	}

	/// <summary>
	/// Thiết lập mức kích thước font chữ và làm mới bộ đệm đo đạc văn bản.
	/// </summary>
	public static void SetFontSize(int index)
	{
		if (index < 0 || index >= fontSizeNames.Length) index = 0;
		fontSizeIndex = index;
		mFont.InvalidateFontMetrics();
		ModConfig.SaveConfig();
	}

	/// <summary>
	/// Đồng bộ chế độ render xuống nền tảng. Desktop (Raylib) đọc renderMode ở mỗi khung hình trong RenderManager;
	/// Android cần đổi định dạng Surface phần cứng nên được đẩy chủ động qua GameView.
	/// </summary>
	public static void ApplyRenderMode()
	{
#if ANDROID || __ANDROID__
		try
		{
			Type gameViewType = Type.GetType("DragonBoy_Android_Host.GameView, DragonBoy_Android");
			if (gameViewType != null)
			{
				var method = gameViewType.GetMethod("ApplyHardwareBitDepth", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
				method?.Invoke(null, new object[] { renderMode });
			}
		}
		catch { }
#endif
	}

	public static void ApplyBitDepth() => ApplyRenderMode();
}

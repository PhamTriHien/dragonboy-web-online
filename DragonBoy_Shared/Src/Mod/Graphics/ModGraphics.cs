using System;
using UnityEngine;

/// <summary>
/// Bộ quản lý đồ họa Mod: Điều phối độ phân giải cửa sổ, khử răng cưa GPU và 2 chế độ render (Java Gốc vs Bản HD).
/// </summary>
public static class ModGraphics
{
	// 0: Ultra (mặc định), 1: Medium (xóa hiệu ứng động), 2: Low (xóa bg), 3: Super Low, 4: Hình Học (Wireframe)
	public static int graphicsQuality = 0;
	public static readonly string[] graphicsNames = new string[5] { "Ultra", "Medium", "Low", "Super Low", "Hình Học" };

	// Chế độ Treo Giảm CPU (giới hạn 15 FPS + tối giản hiệu ứng nền) & Treo Màn Đen (10 FPS, tắt render thế giới)
	public static bool isAfkLowCpu = false;
	public static bool isAfkBlackScreen = false;
	public static bool hideOtherPlayers = false;
	private static long s_LastBlackScreenTapMs = 0L;

	public static void ToggleAfkBlackScreen()
	{
		isAfkBlackScreen = !isAfkBlackScreen;
		s_LastBlackScreenTapMs = 0L;
		ModFps.ApplyFPS();
		if (!isAfkBlackScreen && GameScr.info1 != null && ModMenu.IsInGame())
		{
			GameScr.info1.addInfo("Đã tắt Treo Màn Đen, khôi phục hình ảnh.", 0);
		}
		SoundMn.gI().buttonClick();
	}

	public static bool ShouldRenderAfkBlackScreen()
	{
		if (!isAfkBlackScreen || !ModMenu.IsInGame())
		{
			return false;
		}
		if (ModUI.uiCustomOpen || GameCanvas.currentDialog != null || ModAutoCaptcha.IsCaptchaActive())
		{
			return false;
		}
		return true;
	}

	public static bool CheckWakeFromBlackScreenTap()
	{
		if (!ShouldRenderAfkBlackScreen())
		{
			return false;
		}
		if (GameCanvas.isPointerJustRelease)
		{
			long now = mSystem.currentTimeMillis();
			if (now - s_LastBlackScreenTapMs <= 450L)
			{
				s_LastBlackScreenTapMs = 0L;
				ToggleAfkBlackScreen();
			}
			else
			{
				s_LastBlackScreenTapMs = now;
			}
			GameCanvas.clearAllPointerEvent();
			return true;
		}
		if (GameCanvas.isPointerDown || GameCanvas.isPointerClick)
		{
			GameCanvas.clearAllPointerEvent();
			return true;
		}
		return false;
	}

	public static void PaintAfkBlackScreen(mGraphics g)
	{
		if (g == null) return;
		g.translate(-g.getTranslateX(), -g.getTranslateY());
		g.setClip(0, 0, GameCanvas.w, GameCanvas.h);
		g.setColor(0);
		g.fillRect(0, 0, GameCanvas.w, GameCanvas.h);

		int cx = GameCanvas.w / 2;
		int cy = GameCanvas.h / 2 - 34;
		int fps = (Main.realFPS > 0) ? Main.realFPS : 10;

		(mFont.tahoma_7b_yellow ?? mFont.tahoma_7_yellow).drawString(g, "[ TREO MÀN ĐEN TIẾT KIỆM GPU - " + fps + " FPS ]", cx, cy, mFont.CENTER);
		cy += 14;

		string mapLine = "Map: [" + TileMap.mapID + "] " + ModNextMap.GetMapName(TileMap.mapID) + " - Khu " + TileMap.zoneID;
		(mFont.tahoma_7_white ?? mFont.tahoma_7_yellow).drawString(g, mapLine, cx, cy, mFont.CENTER);
		cy += 13;

		Char me = Char.myCharz();
		if (me != null)
		{
			string statLine = me.cName + " | HP: " + ModMapEntityHUD.FormatHp(me.cHP) + "/" + ModMapEntityHUD.FormatHp(me.cHPFull) + " | KI: " + ModMapEntityHUD.FormatHp(me.cMP) + "/" + ModMapEntityHUD.FormatHp(me.cMPFull);
			(mFont.tahoma_7_yellow ?? mFont.tahoma_7_white).drawString(g, statLine, cx, cy, mFont.CENTER);
			cy += 13;

			string autoLine = "Tàn Sát: " + (ModTanSat.autoTanSat ? "BẬT" : "TẮT") + " | Tự Đánh: " + (ModAutoSkill.isAutoAttackInPlace ? "BẬT" : "TẮT") + " | Vàng: " + ModMapEntityHUD.FormatHp(me.xu);
			(mFont.tahoma_7_white ?? mFont.tahoma_7_yellow).drawString(g, autoLine, cx, cy, mFont.CENTER);
			cy += 16;
		}

		(mFont.tahoma_7_yellow ?? mFont.tahoma_7_white).drawString(g, "Chạm đúp màn hình hoặc bấm F9 để mở lại hình ảnh", cx, cy, mFont.CENTER);
	}

	public static void PaintTilemapWireframe(mGraphics g)
	{
		if (Char.isLoadingMap || TileMap.maps == null || g == null)
		{
			return;
		}
		int tmw = TileMap.tmw;
		int tmh = TileMap.tmh;
		int sz = TileMap.size;
		int startX = System.Math.Max(0, GameScr.gssx);
		int endX = System.Math.Min(tmw, GameScr.gssxe);
		int startY = System.Math.Max(0, GameScr.gssy);
		int endY = System.Math.Min(tmh, GameScr.gssye);

		for (int i = startX; i < endX; i++)
		{
			for (int j = startY; j < endY; j++)
			{
				int idx = j * tmw + i;
				if (idx < 0 || idx >= TileMap.maps.Length)
				{
					continue;
				}
				int type = TileMap.tileTypeAt(i, j);
				bool isSolidTop = (type & 2) == 2;
				bool isWallBlock = (type & (4 | 8 | 8192)) != 0;
				if (!isSolidTop && !isWallBlock)
				{
					continue;
				}
				int px = i * sz;
				int py = j * sz;
				if (isSolidTop)
				{
					g.setColor(0x14331E);
					g.fillRect(px, py, sz, sz);
					g.setColor(0x2EA043);
					g.drawRect(px, py, sz - 1, sz - 1);
				}
				else
				{
					g.setColor(0x1C2128);
					g.fillRect(px, py, sz, sz);
					g.setColor(0x484F58);
					g.drawRect(px, py, sz - 1, sz - 1);
				}
			}
		}

		if (TileMap.vGo != null)
		{
			for (int w = 0; w < TileMap.vGo.size(); w++)
			{
				Waypoint wp = (Waypoint)TileMap.vGo.elementAt(w);
				if (wp == null) continue;
				int wx = wp.minX;
				int wy = wp.minY;
				int ww = System.Math.Max(16, wp.maxX - wp.minX);
				int wh = System.Math.Max(16, wp.maxY - wp.minY);
				g.setColor(0x00BCD4);
				g.drawRect(wx, wy, ww, wh);
				if (wp.popup != null && wp.popup.command != null && !string.IsNullOrEmpty(wp.popup.command.caption))
				{
					(mFont.tahoma_7_yellow ?? mFont.tahoma_7_white).drawString(g, wp.popup.command.caption, wx + ww / 2, wy - 12, mFont.CENTER);
				}
			}
		}
	}

	public static void PaintMobWireframe(Mob mob, mGraphics g)
	{
		if (mob == null || g == null || mob.hp <= 0 || mob.status == 0 || mob.status == 1)
		{
			return;
		}
		if (mob.x < GameScr.cmx - 40 || mob.x > GameScr.cmx + GameScr.gW + 40 ||
		    mob.y < GameScr.cmy - 40 || mob.y > GameScr.cmy + GameScr.gH + 60)
		{
			return;
		}
		int mw = mob.w > 12 ? (mob.w * 2 / 3) : 16;
		int mh = mob.h > 12 ? (mob.h * 2 / 3) : 16;
		int bx = mob.x - mw / 2;
		int by = mob.y - mh;
		bool focused = (Char.myCharz() != null && Char.myCharz().mobFocus == mob);

		g.setColor(focused ? 0x9E2A2B : 0x4A2525);
		g.fillRect(bx, by, mw, mh);
		g.setColor(focused ? 0xFFD600 : 0xFF5252);
		g.drawRect(bx, by, mw - 1, mh - 1);

		string mobName = (mob.getTemplate() != null && !string.IsNullOrEmpty(mob.getTemplate().name))
			? mob.getTemplate().name
			: ("Mob " + mob.templateId);
		mFont nf = focused ? (mFont.tahoma_7b_yellow ?? mFont.tahoma_7_yellow) : (mFont.tahoma_7_white ?? mFont.tahoma_7_yellow);
		nf.drawString(g, mobName, mob.x, by - 12, mFont.CENTER);

		if (mob.maxHp > 0)
		{
			int barW = 28;
			int fillW = (int)(mob.hp * barW / mob.maxHp);
			if (fillW < 1 && mob.hp > 0) fillW = 1;
			if (fillW > barW) fillW = barW;
			int barX = mob.x - barW / 2;
			int barY = by - 3;
			g.setColor(0x222222);
			g.fillRect(barX, barY, barW, 3);
			g.setColor(focused ? 0x00E676 : 0xFF5252);
			g.fillRect(barX, barY, fillW, 3);
		}
	}

	public static void PaintItemMapWireframe(ItemMap item, mGraphics g)
	{
		if (item == null || g == null || item.template == null)
		{
			return;
		}
		g.setColor(0xFFD54F);
		g.fillRect(item.x - 4, item.y - 8, 8, 8);
		g.setColor(0xFF6F00);
		g.drawRect(item.x - 4, item.y - 8, 7, 7);
		string label = ModSetActivator.showItemId ? ("[" + item.template.id + "] " + item.template.name) : item.template.name;
		(mFont.tahoma_7_yellow ?? mFont.tahoma_7_white).drawString(g, label, item.x, item.y - 19, mFont.CENTER);
	}

	public static void ToggleHideOtherPlayers()
	{
		hideOtherPlayers = !hideOtherPlayers;
		ModConfig.SaveConfig();
		GameScr.info1?.addInfo("Xóa người chơi khác: " + (hideOtherPlayers ? "BẬT" : "TẮT"), 0);
		SoundMn.gI().buttonClick();
	}

	public static bool ShouldHideChar(Char c)
	{
		if (!hideOtherPlayers || c == null) return false;
		Char me = Char.myCharz();
		if (c == me || c.me) return false;
		if (me != null && (c.charID == -me.charID || me.charFocus == c)) return false;
		if (c.head == 31) return false;
		if (c.cTypePk == 5 || c.cTypePk == 3) return false;
		if (c.charID < 0 && !c.isPet && !c.isMiniPet) return false;
		if (!string.IsNullOrEmpty(c.cName) && ModBossNotice.IsBossName(c.cName)) return false;
		return true;
	}

	public static void ToggleAfkLowCpu()
	{
		isAfkLowCpu = !isAfkLowCpu;
		ModFps.ApplyFPS();
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

	// Kích thước Font chữ: 0: Nhỏ 65%, 1: Chuẩn 75%, 2: Lớn 90%, 3: Gốc 100%
	public static int fontSizeIndex = 1;
	public static readonly string[] fontSizeNames = new string[4] { "Nhỏ 65%", "Chuẩn 75%", "Lớn 90%", "Gốc 100%" };
	public static readonly float[] fontSizeScales = new float[4] { 0.65f, 0.75f, 0.90f, 1.0f };

	// Tốc độ hoạt ảnh game (nhân hệ số chu kỳ mô phỏng 50Hz gốc): 0: x1.0 Gốc .. 7: x5.0
	public static int animSpeedIndex = 0;
	public static readonly float[] animSpeedValues = new float[8] { 1.0f, 1.25f, 1.5f, 2.0f, 2.5f, 3.0f, 4.0f, 5.0f };
	public static readonly string[] animSpeedNames = new string[8] { "x1.0 Gốc", "x1.25", "x1.5", "x2.0", "x2.5", "x3.0", "x4.0", "x5.0" };

	public static float GetAnimSpeed()
	{
		if (animSpeedIndex >= 0 && animSpeedIndex < animSpeedValues.Length)
		{
			return animSpeedValues[animSpeedIndex];
		}
		return 1.0f;
	}

	public static void ApplyAnimSpeed()
	{
		try
		{
			float speed = GetAnimSpeed();
			Time.timeScale = speed;
			Time.fixedDeltaTime = 0.02f;
			Time.maximumDeltaTime = (speed > 1.0f) ? (0.1f * speed) : 0.1f;
		}
		catch
		{
		}
	}

	public static void SetAnimSpeed(int index, bool save = true)
	{
		if (index < 0 || index >= animSpeedValues.Length)
		{
			index = 0;
		}
		animSpeedIndex = index;
		ApplyAnimSpeed();
		if (save)
		{
			ModConfig.SaveConfig();
		}
	}

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
			ApplyAnimSpeed();
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
			for (int j = 0; j < 7; j++)
			{
				GameCanvas.imgBlue[j] = GameCanvas.loadImage("/effectdata/blue/" + j + ".png");
				GameCanvas.imgViolet[j] = GameCanvas.loadImage("/effectdata/violet/" + j + ".png");
			}
			GameScr.ReloadHudImages();
			Panel.loadBg();
			Menu.loadBg();
			PopUp.goc = null;
			PopUp.imgPopUp = null;
			PopUp.imgPopUp2 = null;
			PopUp.loadBg();
			Info.gocnhon = GameCanvas.loadImage("/mainImage/myTexture2dgocnhon.png");
			TField.imgTf = GameCanvas.loadImage("/mainImage/myTexture2dtf.png");
			ModUI.ReloadForGraphicsMode();
			SmallImage.ReloadForGraphicsMode();
			Mob.ReloadForGraphicsMode();
			Effect.ReloadForGraphicsMode();
			ImgByName.ReloadForGraphicsMode();
			Char.ReloadForGraphicsMode();
			ItemMap.imageFlare = GameCanvas.loadImage("/mainImage/myTexture2dflare.png");
			ItemMap.imageAuraItem1 = GameCanvas.loadImage("/mainImage/myTexture2ditemaura1.png");
			ItemMap.imageAuraItem2 = GameCanvas.loadImage("/mainImage/myTexture2ditemaura2.png");
			ItemMap.imageAuraItem3 = GameCanvas.loadImage("/mainImage/myTexture2ditemaura3.png");
			MagicTree.pea = GameCanvas.loadImage("/mainImage/myTexture2dhatdau.png");
			EffectFeet.imgFeet1 = GameCanvas.loadImage("/mainImage/myTexture2dmove-1.png");
			EffectFeet.imgFeet3 = GameCanvas.loadImage("/mainImage/myTexture2dmove-3.png");
			if (Teleport.maybay != null)
			{
				for (int m = 0; m < 3 && m < Teleport.maybay.Length; m++)
				{
					if (Teleport.maybay[m] != null)
					{
						Teleport.maybay[m] = GameCanvas.loadImage("/mainImage/myTexture2dmaybay" + (m + 1) + ".png");
					}
				}
				if (Teleport.maybay.Length > 3 && Teleport.maybay[3] != null)
				{
					Teleport.maybay[3] = GameCanvas.loadImage("/mainImage/myTexture2dmaybay4a.png");
				}
				if (Teleport.maybay.Length > 4 && Teleport.maybay[4] != null)
				{
					Teleport.maybay[4] = GameCanvas.loadImage("/mainImage/myTexture2dmaybay4b.png");
				}
			}
			if (Teleport.hole != null)
			{
				Teleport.hole = GameCanvas.loadImage("/mainImage/hole.png");
			}
			if (TransportScr.ship != null)
			{
				TransportScr.ship = GameCanvas.loadImage("/mainImage/myTexture2dfutherShip.png");
			}
			if (TransportScr.taungam != null)
			{
				TransportScr.taungam = GameCanvas.loadImage("/mainImage/taungam.png");
			}
			RadarScr.ReloadForGraphicsMode();
			CrackBallScr.ReloadForGraphicsMode();
			TileMap.loadBg();
			if (TileMap.tileID > 0)
			{
				TileMap.loadTileImage();
				TileMap.loadMainTile();
			}
			BgItem.ReloadCurrMapItems();
			BackgroudEffect.ReloadForGraphicsMode();
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
		return 0.75f;
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
		if (index < 0 || index >= fontSizeNames.Length) index = 1;
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

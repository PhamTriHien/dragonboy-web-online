using System;
using UnityEngine;

/// <summary>
/// Giao diện cài đặt đồ họa Mod: Cung cấp các nút bấm tùy chỉnh độ phân giải, FPS, khử răng cưa và chế độ render.
/// </summary>
public static class ModUIGraphics
{
	public const int ContentHeight = 524;
	private static readonly int[] fpsOptions = new int[8] { 30, 60, 90, 120, 144, 165, 185, 240 };

	/// <summary>
	/// Thực hiện vẽ toàn bộ đối tượng và các lớp hiển thị lên canvas mGraphics.
	/// </summary>
	public static void Paint(int uiX, int uiY, int uiW, int uiH, mGraphics g)
	{
		// 1. Toàn Màn Hình
		mFont.tahoma_7_grey.drawString(g, "Toàn màn hình:", uiX + 6, uiY + 7, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 88, uiY + 4, 44, 16, ModGraphics.isFullscreen ? "BẬT" : "TẮT", ModGraphics.isFullscreen, g);
		mFont.tahoma_7_grey.drawString(g, "(F11 / Alt+Enter)", uiX + 138, uiY + 7, mFont.LEFT);

		// 2. Độ Phân Giải Cửa Sổ
		mFont.tahoma_7_grey.drawString(g, "Độ phân giải cửa sổ:", uiX + 6, uiY + 28, mFont.LEFT);
		int rBtnW = (uiW - 18) / 4;
		for (int r = 0; r < 4; r++)
		{
			int rx = uiX + 5 + r * (rBtnW + 3);
			bool isSelRes = (!ModGraphics.isFullscreen && ModGraphics.resolutionIndex == r);
			ModUI.PaintNativeButton(rx, uiY + 43, rBtnW, 16, ModGraphics.resolutionNames[r], isSelRes, g);
		}

		// 3. Chế độ đồ họa & Khử răng cưa GPU (5 mức)
		mFont.tahoma_7_grey.drawString(g, "Chất lượng đồ họa (GPU Khử răng cưa):", uiX + 6, uiY + 66, mFont.LEFT);
		int qCount = ModGraphics.graphicsNames.Length;
		int qBtnW = (uiW - 10 - (qCount - 1) * 3) / qCount;
		for (int q = 0; q < qCount; q++)
		{
			int qx = uiX + 5 + q * (qBtnW + 3);
			ModUI.PaintNativeButton(qx, uiY + 81, qBtnW, 16, ModGraphics.graphicsNames[q], ModGraphics.graphicsQuality == q, g);
		}

		// Mô tả chi tiết mức đồ họa
		int descY = uiY + 101;
		if (ModGraphics.graphicsQuality == 0)
		{
			mFont.tahoma_7_grey.drawString(g, "* Ultra: GPU khử răng cưa 4x, texture sắc nét.", uiX + 6, descY, mFont.LEFT);
		}
		else if (ModGraphics.graphicsQuality == 1)
		{
			mFont.tahoma_7_grey.drawString(g, "* Medium: Tối ưu mượt, xóa hiệu ứng chiêu & thời tiết.", uiX + 6, descY, mFont.LEFT);
		}
		else if (ModGraphics.graphicsQuality == 2)
		{
			mFont.tahoma_7_grey.drawString(g, "* Low: Xóa background, phông nền sáng siêu nhẹ.", uiX + 6, descY, mFont.LEFT);
		}
		else if (ModGraphics.graphicsQuality == 3)
		{
			mFont.tahoma_7_green2.drawString(g, "* Super Low: Xóa cây cỏ trang trí, chỉ giữ base map.", uiX + 6, descY, mFont.LEFT);
		}
		else if (ModGraphics.graphicsQuality == 4)
		{
			mFont.tahoma_7_green2.drawString(g, "* Hình Học: Khung lưới địa hình & hitbox quái siêu nhẹ.", uiX + 6, descY, mFont.LEFT);
		}

		// 4. 2 Chế Độ Đồ Hoạ (0: Đồ Hoạ Java, 1: Đồ Hoạ HD)
		mFont.tahoma_7_grey.drawString(g, "Chế Độ Đồ Hoạ (F10):", uiX + 6, uiY + 116, mFont.LEFT);
		int mBtnW = (uiW - 15) / 2;
		for (int m = 0; m < 2; m++)
		{
			int mx = uiX + 5 + m * (mBtnW + 5);
			ModUI.PaintNativeButton(mx, uiY + 131, mBtnW, 16, ModGraphics.renderModeNames[m], ModGraphics.renderMode == m, g);
		}

		int bdDescY = uiY + 151;
		if (ModGraphics.renderMode == 0)
		{
			mFont.tahoma_7_green2.drawString(g, "* Đồ Hoạ Java: Tài nguyên x1 & font bitmap từ src Java gốc.", uiX + 6, bdDescY, mFont.LEFT);
		}
		else
		{
			mFont.tahoma_7_green2.drawString(g, "* Đồ Hoạ HD: Tài nguyên x2 sắc nét, lọc mượt & font hệ thống.", uiX + 6, bdDescY, mFont.LEFT);
		}

		// 5. Phần FPS
		mFont.tahoma_7_grey.drawString(g, "Auto FPS:", uiX + 6, uiY + 168, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 62, uiY + 165, 44, 16, ModFps.isAutoFps ? "BẬT" : "TẮT", ModFps.isAutoFps, g);
		mFont.tahoma_7_grey.drawString(g, "(Khớp tần số quét)", uiX + 112, uiY + 168, mFont.LEFT);

		// Mốc FPS cố định (2 hàng x 4 nút)
		for (int f = 0; f < fpsOptions.Length; f++)
		{
			int col = f % 4;
			int row = f / 4;
			int fx = uiX + 5 + col * (rBtnW + 3);
			int fy = uiY + 187 + row * 20;
			bool isSel = (!ModFps.isAutoFps && ModFps.targetFps == fpsOptions[f]);
			ModUI.PaintNativeButton(fx, fy, rBtnW, 16, fpsOptions[f] + " FPS", isSel, g);
		}

		mFont.tahoma_7_green2.drawString(g, "FPS: " + Main.realFPS + " | Màn: " + ModFps.GetDeviceMaxRefreshRate() + "Hz | Cửa sổ: " + Screen.width + "x" + Screen.height, uiX + uiW / 2, uiY + 233, mFont.CENTER);

		// 6. Tốc độ hoạt ảnh game
		int curAnimIdx = (ModGraphics.animSpeedIndex >= 0 && ModGraphics.animSpeedIndex < ModGraphics.animSpeedNames.Length) ? ModGraphics.animSpeedIndex : 0;
		mFont.tahoma_7_grey.drawString(g, "Tốc độ hoạt ảnh game:", uiX + 6, uiY + 250, mFont.LEFT);
		mFont.tahoma_7_green2.drawString(g, ModGraphics.animSpeedNames[curAnimIdx] + " (" + (int)(50f * ModGraphics.GetAnimSpeed() + 0.5f) + "Hz)", uiX + 122, uiY + 250, mFont.LEFT);
		for (int a = 0; a < ModGraphics.animSpeedNames.Length; a++)
		{
			int col = a % 4;
			int row = a / 4;
			int ax = uiX + 5 + col * (rBtnW + 3);
			int ay = uiY + 265 + row * 20;
			ModUI.PaintNativeButton(ax, ay, rBtnW, 16, ModGraphics.animSpeedNames[a], curAnimIdx == a, g);
		}

		// 7. Việt Hoá Server Data & Logo
		mFont.tahoma_7_grey.drawString(g, "Việt Hoá:", uiX + 6, uiY + 310, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 62, uiY + 307, 38, 16, ModConfig.isTranslate ? "BẬT" : "TẮT", ModConfig.isTranslate, g);

		mFont.tahoma_7_grey.drawString(g, "Logo:", uiX + 108, uiY + 310, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 144, uiY + 307, 38, 16, ModLogo.isShowLogoInGame ? "BẬT" : "TẮT", ModLogo.isShowLogoInGame, g);
		ModUI.PaintNativeButton(uiX + 186, uiY + 307, 60, 16, ModLogo.logoPosition == 0 ? "Góc Phải" : "Ở Giữa", ModLogo.isShowLogoInGame, g);

		// 8. Menu HUB Nổi, Bàn phím ảo Analog & Treo Giảm CPU
		mFont.tahoma_7_grey.drawString(g, "HUB Nổi:", uiX + 6, uiY + 332, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 54, uiY + 329, 34, 16, ModFloatingHUD.isShowFloatingHUD ? "BẬT" : "TẮT", ModFloatingHUD.isShowFloatingHUD, g);

		mFont.tahoma_7_grey.drawString(g, "Analog:", uiX + 92, uiY + 332, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 132, uiY + 329, 34, 16, (GameScr.isAnalog == 1) ? "BẬT" : "TẮT", GameScr.isAnalog == 1, g);

		ModUI.PaintNativeButton(uiX + 170, uiY + 329, 76, 16, "Giảm CPU: " + (ModGraphics.isAfkLowCpu ? "BẬT" : "TẮT"), ModGraphics.isAfkLowCpu, g);

		// 9. Chủ Đề Font Chữ & Kích Thước Font Chữ
		mFont.tahoma_7_grey.drawString(g, "Chủ đề Font chữ:", uiX + 6, uiY + 351, mFont.LEFT);
		for (int ft = 0; ft < ModGraphics.fontThemeNames.Length; ft++)
		{
			int ftx = uiX + 5 + ft * (rBtnW + 3);
			ModUI.PaintNativeButton(ftx, uiY + 366, rBtnW, 16, ModGraphics.fontThemeNames[ft], ModGraphics.fontTheme == ft, g);
		}

		mFont.tahoma_7_grey.drawString(g, "Kích thước Font chữ:", uiX + 6, uiY + 388, mFont.LEFT);
		for (int fs = 0; fs < ModGraphics.fontSizeNames.Length; fs++)
		{
			int fsx = uiX + 5 + fs * (rBtnW + 3);
			ModUI.PaintNativeButton(fsx, uiY + 403, rBtnW, 16, ModGraphics.fontSizeNames[fs], ModGraphics.fontSizeIndex == fs, g);
		}

		// 10. Phong Cảnh Nền Game (Git)
		string bgBtnText = "HÌNH NỀN PHONG CẢNH (GIT)" + (ModBackground.isCustomBGActive ? " [ĐANG BẬT]" : "");
		ModUI.PaintNativeButton(uiX + 6, uiY + 426, uiW - 12, 18, bgBtnText, ModBackground.isCustomBGActive, g);

		// 11. Hiển thị & Cụm Nút Nhanh / Phím Tắt PC
		int halfBtnW = (uiW - 16) / 2;
		ModUI.PaintNativeButton(uiX + 6, uiY + 450, halfBtnW, 18, "Xóa Người: " + (ModGraphics.hideOtherPlayers ? "BẬT" : "TẮT"), ModGraphics.hideOtherPlayers, g);
		ModUI.PaintNativeButton(uiX + 10 + halfBtnW, uiY + 450, halfBtnW, 18, "HUD Sư Phụ: " + (ModMapEntityHUD.isShowMasterHUD ? "BẬT" : "TẮT"), ModMapEntityHUD.isShowMasterHUD, g);

		ModUI.PaintNativeButton(uiX + 6, uiY + 472, halfBtnW, 18, "Nút Nhanh: " + (ModArrowButton.showExtraQuickButtons ? "BẬT" : "TẮT"), ModArrowButton.showExtraQuickButtons, g);
		ModUI.PaintNativeButton(uiX + 10 + halfBtnW, uiY + 472, halfBtnW, 18, "Phím Tắt PC: " + (ModHotkey.enableQuickPcHotkeys ? "BẬT" : "TẮT"), ModHotkey.enableQuickPcHotkeys, g);

		ModUI.PaintNativeButton(uiX + 6, uiY + 494, uiW - 12, 18, "TREO MÀN ĐEN TIẾT KIỆM GPU (F9): " + (ModGraphics.isAfkBlackScreen ? "BẬT" : "TẮT"), ModGraphics.isAfkBlackScreen, g);
	}

	/// <summary>
	/// Xử lý nghiệp vụ logic chuyên trách cho HandleTap.
	/// </summary>
	public static bool HandleTap(int px, int py, int uiX, int uiY, int uiW, int uiH)
	{
		// 1. Bật / Tắt Toàn Màn Hình
		if (px >= uiX + 88 && px <= uiX + 132 && py >= uiY + 3 && py <= uiY + 21)
		{
			ModGraphics.ToggleFullscreen();
			SoundMn.gI().buttonClick();
			return true;
		}

		// 2. Chọn Độ Phân Giải
		int rBtnW = (uiW - 18) / 4;
		for (int r = 0; r < 4; r++)
		{
			int rx = uiX + 5 + r * (rBtnW + 3);
			if (px >= rx && px <= rx + rBtnW && py >= uiY + 42 && py <= uiY + 60)
			{
				ModGraphics.ApplyResolution(r);
				SoundMn.gI().buttonClick();
				return true;
			}
		}

		// 3. Chọn Chất Lượng Đồ Họa
		int qCount = ModGraphics.graphicsNames.Length;
		int qBtnW = (uiW - 10 - (qCount - 1) * 3) / qCount;
		for (int q = 0; q < qCount; q++)
		{
			int qx = uiX + 5 + q * (qBtnW + 3);
			if (px >= qx && px <= qx + qBtnW && py >= uiY + 80 && py <= uiY + 98)
			{
				ModGraphics.SetQuality(q);
				SoundMn.gI().buttonClick();
				return true;
			}
		}

		// 4. Chọn Chế Độ Đồ Hoạ (0: Đồ Hoạ Java, 1: Đồ Hoạ HD)
		int mBtnW = (uiW - 15) / 2;
		for (int m = 0; m < 2; m++)
		{
			int mx = uiX + 5 + m * (mBtnW + 5);
			if (px >= mx && px <= mx + mBtnW && py >= uiY + 130 && py <= uiY + 148)
			{
				ModGraphics.SetRenderMode(m);
				GameScr.info1.addInfo("Chế độ đồ hoạ: " + ModGraphics.renderModeNames[m], 0);
				SoundMn.gI().buttonClick();
				return true;
			}
		}

		// 5. Auto FPS
		if (px >= uiX + 62 && px <= uiX + 106 && py >= uiY + 164 && py <= uiY + 182)
		{
			if (ModFps.isAutoFps)
			{
				ModFps.SetFPS(144);
			}
			else
			{
				ModFps.SetAutoFPS();
			}
			SoundMn.gI().buttonClick();
			return true;
		}

		// 6. Chọn Mốc FPS
		for (int f = 0; f < fpsOptions.Length; f++)
		{
			int col = f % 4;
			int row = f / 4;
			int fx = uiX + 5 + col * (rBtnW + 3);
			int fy = uiY + 187 + row * 20;
			if (px >= fx && px <= fx + rBtnW && py >= fy && py <= fy + 18)
			{
				ModFps.SetFPS(fpsOptions[f]);
				SoundMn.gI().buttonClick();
				return true;
			}
		}

		// 6b. Chọn Tốc Độ Hoạt Ảnh Game
		for (int a = 0; a < ModGraphics.animSpeedNames.Length; a++)
		{
			int col = a % 4;
			int row = a / 4;
			int ax = uiX + 5 + col * (rBtnW + 3);
			int ay = uiY + 265 + row * 20;
			if (px >= ax && px <= ax + rBtnW && py >= ay && py <= ay + 18)
			{
				ModGraphics.SetAnimSpeed(a);
				GameScr.info1?.addInfo("Tốc độ hoạt ảnh game: " + ModGraphics.animSpeedNames[a], 0);
				SoundMn.gI().buttonClick();
				return true;
			}
		}

		// 7. Bật / Tắt Việt Hoá Server Data
		if (px >= uiX + 62 && px <= uiX + 100 && py >= uiY + 306 && py <= uiY + 324)
		{
			ModConfig.isTranslate = !ModConfig.isTranslate;
			ModConfig.SaveConfig();
			ModTranslate.ApplyAllTranslations();
			GameScr.info1.addInfo("Dịch Việt Hoá: " + (ModConfig.isTranslate ? "BẬT" : "TẮT (Gốc Server)"), 0);
			SoundMn.gI().buttonClick();
			return true;
		}

		// 8. Bật / Tắt Logo TriHienKun trong game
		if (px >= uiX + 144 && px <= uiX + 182 && py >= uiY + 306 && py <= uiY + 324)
		{
			ModLogo.isShowLogoInGame = !ModLogo.isShowLogoInGame;
			ModConfig.SaveConfig();
			SoundMn.gI().buttonClick();
			return true;
		}

		// 9. Vị trí Logo (Góc Phải / Ở Giữa)
		if (px >= uiX + 186 && px <= uiX + 246 && py >= uiY + 306 && py <= uiY + 324)
		{
			ModLogo.logoPosition = (ModLogo.logoPosition == 0) ? 1 : 0;
			ModConfig.SaveConfig();
			GameScr.info1.addInfo("Vị trí Logo: " + (ModLogo.logoPosition == 0 ? "Góc Phải" : "Ở Giữa"), 0);
			SoundMn.gI().buttonClick();
			return true;
		}

		// 10. Bật / Tắt Menu HUB Nổi
		if (px >= uiX + 54 && px <= uiX + 88 && py >= uiY + 328 && py <= uiY + 346)
		{
			ModFloatingHUD.isShowFloatingHUD = !ModFloatingHUD.isShowFloatingHUD;
			ModConfig.SaveConfig();
			GameScr.info1.addInfo("Menu HUB Nổi: " + (ModFloatingHUD.isShowFloatingHUD ? "BẬT" : "TẮT"), 0);
			SoundMn.gI().buttonClick();
			return true;
		}

		// 11. Bật / Tắt Bàn Phím Ảo & Analog
		if (px >= uiX + 132 && px <= uiX + 166 && py >= uiY + 328 && py <= uiY + 346)
		{
			GameScr.isAnalog = (GameScr.isAnalog == 1) ? 0 : 1;
			GameScr.setSkillBarPosition();
			Rms.saveRMSInt("analog", GameScr.isAnalog);
			ModConfig.SaveConfig();
			GameScr.info1.addInfo("Bàn phím ảo & Analog: " + ((GameScr.isAnalog == 1) ? "BẬT" : "TẮT"), 0);
			SoundMn.gI().buttonClick();
			return true;
		}

		// 11b. Bật / Tắt Treo Giảm CPU
		if (px >= uiX + 170 && px <= uiX + 246 && py >= uiY + 328 && py <= uiY + 346)
		{
			ModGraphics.ToggleAfkLowCpu();
			return true;
		}

		// 12. Chọn Chủ Đề Font Chữ
		for (int ft = 0; ft < ModGraphics.fontThemeNames.Length; ft++)
		{
			int ftx = uiX + 5 + ft * (rBtnW + 3);
			if (px >= ftx && px <= ftx + rBtnW && py >= uiY + 365 && py <= uiY + 383)
			{
				ModGraphics.SetFontTheme(ft);
				GameScr.info1.addInfo("Font chữ: " + ModGraphics.fontThemeNames[ft], 0);
				SoundMn.gI().buttonClick();
				return true;
			}
		}

		// 13. Chọn Kích Thước Font Chữ
		for (int fs = 0; fs < ModGraphics.fontSizeNames.Length; fs++)
		{
			int fsx = uiX + 5 + fs * (rBtnW + 3);
			if (px >= fsx && px <= fsx + rBtnW && py >= uiY + 402 && py <= uiY + 420)
			{
				ModGraphics.SetFontSize(fs);
				GameScr.info1.addInfo("Cỡ chữ: " + ModGraphics.fontSizeNames[fs], 0);
				SoundMn.gI().buttonClick();
				return true;
			}
		}

		// 14. Mở Giao diện Hình Nền Phong Cảnh
		if (px >= uiX + 6 && px <= uiX + uiW - 6 && py >= uiY + 425 && py <= uiY + 446)
		{
			ModUIBackground.isOpen = true;
			ModUI.detailScrollY = 0;
			SoundMn.gI().buttonClick();
			return true;
		}

		int halfBtnW = (uiW - 16) / 2;
		if (px >= uiX + 6 && px <= uiX + 6 + halfBtnW && py >= uiY + 449 && py <= uiY + 469)
		{
			ModGraphics.ToggleHideOtherPlayers();
			return true;
		}
		if (px >= uiX + 10 + halfBtnW && px <= uiX + 10 + halfBtnW * 2 && py >= uiY + 449 && py <= uiY + 469)
		{
			ModMapEntityHUD.ToggleMasterHUD();
			return true;
		}
		if (px >= uiX + 6 && px <= uiX + 6 + halfBtnW && py >= uiY + 471 && py <= uiY + 491)
		{
			ModArrowButton.showExtraQuickButtons = !ModArrowButton.showExtraQuickButtons;
			ModConfig.SaveConfig();
			GameScr.info1.addInfo("Cụm nút nhanh phụ: " + (ModArrowButton.showExtraQuickButtons ? "BẬT" : "TẮT"), 0);
			SoundMn.gI().buttonClick();
			return true;
		}
		if (px >= uiX + 10 + halfBtnW && px <= uiX + 10 + halfBtnW * 2 && py >= uiY + 471 && py <= uiY + 491)
		{
			ModHotkey.enableQuickPcHotkeys = !ModHotkey.enableQuickPcHotkeys;
			ModConfig.SaveConfig();
			GameScr.info1.addInfo("Phím tắt nhanh PC: " + (ModHotkey.enableQuickPcHotkeys ? "BẬT" : "TẮT"), 0);
			SoundMn.gI().buttonClick();
			return true;
		}
		if (px >= uiX + 6 && px <= uiX + uiW - 6 && py >= uiY + 493 && py <= uiY + 513)
		{
			ModGraphics.ToggleAfkBlackScreen();
			if (ModGraphics.isAfkBlackScreen)
			{
				ModMenu.CloseMenu();
			}
			return true;
		}

		return false;
	}
}

using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Thư viện giao diện Mod tùy biến: Cung cấp các hàm vẽ khung viền (PaintNativeBorder), nút bấm (PaintNativeButton) và tab.
/// </summary>
public static class ModUI
{
	public static bool uiCustomOpen = false;
	public static bool uiTanSatOpen
	{
		get { return uiCustomOpen; }
		set { uiCustomOpen = value; }
	}
	public static int selectedTab = 0;
	public static int tanSatTab = 0;

	public static readonly string[] tabNames = new string[12]
	{
		"Tàn Sát",
		"Tự Nhặt",
		"Tốc Độ",
		"Hồi Máu",
		"Tự Động",
		"Đồ Họa",
		"Báo Boss",
		"Qua Map",
		"GoBack",
		"Úp Set KH",
		"Nhân Bản",
		"Lệnh & Phím"
	};

	// Cột Danh Mục Trái (Left Sidebar) - Chạm kéo trực tiếp, không thanh cuộn
	public static int colScrollY = 0;
	private static bool isColDragging = false;
	private static bool hasColDragged = false;
	private static int startColDragY = 0;
	private static int lastColDragY = 0;

	// Khung Nội Dung Phải (Right Detail Panel) - Chạm kéo trực tiếp, không thanh cuộn
	public static int detailScrollY = 0;
	private static bool isDetailDragging = false;
	private static bool hasDetailDragged = false;
	private static int startDetailDragY = 0;
	private static int lastDetailDragY = 0;

	private static Image imgBtX;

	public static void ReloadForGraphicsMode()
	{
		imgBtX = null;
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của GetUniqueMobTemplateIds.
	/// </summary>
	public static List<int> GetUniqueMobTemplateIds()
	{
		List<int> list = new List<int>();
		if (GameScr.vMob != null)
		{
			for (int i = 0; i < GameScr.vMob.size(); i++)
			{
				Mob m = (Mob)GameScr.vMob.elementAt(i);
				if (m != null && !list.Contains(m.templateId))
				{
					list.Add(m.templateId);
				}
			}
		}
		return list;
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của GetPlayerAttackSkills.
	/// </summary>
	public static List<Skill> GetPlayerAttackSkills()
	{
		List<Skill> list = new List<Skill>();
		List<int> addedIds = new List<int>();
		Char me = Char.myCharz();

		if (GameScr.keySkill != null)
		{
			for (int i = 0; i < GameScr.keySkill.Length; i++)
			{
				Skill s = GameScr.keySkill[i];
				if (s != null && s.template != null && ModTanSatFilter.IsAttackSkill(s) && !addedIds.Contains(s.template.id))
				{
					list.Add(s);
					addedIds.Add(s.template.id);
				}
			}
		}

		if (GameScr.onScreenSkill != null)
		{
			for (int j = 0; j < GameScr.onScreenSkill.Length; j++)
			{
				Skill s2 = GameScr.onScreenSkill[j];
				if (s2 != null && s2.template != null && ModTanSatFilter.IsAttackSkill(s2) && !addedIds.Contains(s2.template.id))
				{
					list.Add(s2);
					addedIds.Add(s2.template.id);
				}
			}
		}

		if (me != null && me.myskill != null && me.myskill.template != null && ModTanSatFilter.IsAttackSkill(me.myskill) && !addedIds.Contains(me.myskill.template.id))
		{
			list.Add(me.myskill);
			addedIds.Add(me.myskill.template.id);
		}

		if (me != null && me.vSkillFight != null)
		{
			for (int k = 0; k < me.vSkillFight.size(); k++)
			{
				Skill s3 = (Skill)me.vSkillFight.elementAt(k);
				if (s3 != null && s3.template != null && ModTanSatFilter.IsAttackSkill(s3) && !addedIds.Contains(s3.template.id))
				{
					list.Add(s3);
					addedIds.Add(s3.template.id);
				}
			}
		}

		return list;
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của GetSkillShortName.
	/// </summary>
	public static string GetSkillShortName(Skill s)
	{
		if (s == null || s.template == null)
		{
			return string.Empty;
		}
		string n = s.template.name;
		if (string.IsNullOrEmpty(n))
		{
			return "Chiêu " + s.template.id;
		}
		if (n.Length > 12)
		{
			return n.Substring(0, 10) + "..";
		}
		return n;
	}

	/// <summary>
	/// Thực hiện vẽ và hiển thị thành phần DrawCheckbox lên canvas đồ họa.
	/// </summary>
	public static void DrawCheckbox(int bx, int by, bool isChecked, mGraphics g)
	{
		if (Paint.imgCheck == null)
		{
			try { Paint.imgCheck = GameCanvas.loadImage("/mainImage/myTexture2dcheck.png"); } catch { }
		}
		if (Paint.imgCheck != null)
		{
			g.drawRegion(Paint.imgCheck, 0, (isChecked ? 2 : 0) * 18, 20, 18, 0, bx, by, 0);
		}
		else
		{
			GameCanvas.paintz.paintCheckPass(g, bx, by, isChecked, false);
		}
	}

	/// <summary>
	/// Thực hiện vẽ và hiển thị thành phần PaintNativeButton lên canvas đồ họa.
	/// </summary>
	public static void PaintNativeButton(int x, int y, int w, int h, string text, bool isFocus, mGraphics g)
	{
		try
		{
			if (Command.btn0left == null)
			{
				try
				{
					Command.btn0left = GameCanvas.loadImage("/mainImage/btn0left.png");
					Command.btn0mid = GameCanvas.loadImage("/mainImage/btn0mid.png");
					Command.btn0right = GameCanvas.loadImage("/mainImage/btn0right.png");
					Command.btn1left = GameCanvas.loadImage("/mainImage/btn1left.png");
					Command.btn1mid = GameCanvas.loadImage("/mainImage/btn1mid.png");
					Command.btn1right = GameCanvas.loadImage("/mainImage/btn1right.png");
				}
				catch { }
			}

			if (Command.btn0left != null && Command.btn0mid != null && Command.btn0right != null)
			{
				Image bLeft = isFocus ? Command.btn1left : Command.btn0left;
				Image bMid = isFocus ? Command.btn1mid : Command.btn0mid;
				Image bRight = isFocus ? Command.btn1right : Command.btn0right;

				if (h >= 24 && w >= 20)
				{
					Command.paintOngMau(bLeft, bMid, bRight, x, y, w, g);
				}
				else
				{
					int topH = h / 2;
					int botH = h - topH;
					int srcBotY = 24 - botH;

					if (w >= 20)
					{
						// Nửa trên (Top cap)
						g.drawRegion(bLeft, 0, 0, 10, topH, 0, x, y, 0);
						for (int i = 10; i <= w - 20; i += 10)
						{
							g.drawRegion(bMid, 0, 0, 10, topH, 0, x + i, y, 0);
						}
						int rem = w % 10;
						if (rem > 0)
						{
							g.drawRegion(bMid, 0, 0, rem, topH, 0, x + w - 10 - rem, y, 0);
						}
						g.drawRegion(bRight, 0, 0, 10, topH, 0, x + w - 10, y, 0);

						// Nửa dưới (Bottom cap bo tròn & đổ bóng chuẩn)
						g.drawRegion(bLeft, 0, srcBotY, 10, botH, 0, x, y + topH, 0);
						for (int i = 10; i <= w - 20; i += 10)
						{
							g.drawRegion(bMid, 0, srcBotY, 10, botH, 0, x + i, y + topH, 0);
						}
						if (rem > 0)
						{
							g.drawRegion(bMid, 0, srcBotY, rem, botH, 0, x + w - 10 - rem, y + topH, 0);
						}
						g.drawRegion(bRight, 0, srcBotY, 10, botH, 0, x + w - 10, y + topH, 0);
					}
					else
					{
						int halfW = w / 2;
						int remW = w - halfW;
						// Top
						g.drawRegion(bLeft, 0, 0, halfW, topH, 0, x, y, 0);
						g.drawRegion(bRight, 10 - remW, 0, remW, topH, 0, x + halfW, y, 0);
						// Bottom
						g.drawRegion(bLeft, 0, srcBotY, halfW, botH, 0, x, y + topH, 0);
						g.drawRegion(bRight, 10 - remW, srcBotY, remW, botH, 0, x + halfW, y + topH, 0);
					}
				}
			}
			else
			{
				g.setColor(isFocus ? 16383818 : 14338484);
				g.fillRect(x + 1, y + 1, w - 2, h - 2);
				g.setColor(6702080);
				g.drawRect(x, y, w - 1, h - 1);
			}

			int textY = y + (h - 10) / 2;
			mFont font = isFocus ? (mFont.tahoma_7b_yellow ?? mFont.tahoma_7_yellow) : (mFont.tahoma_7b_white ?? mFont.tahoma_7_white);
			if (font != null && w > 24 && !string.IsNullOrEmpty(text) && font.getWidth(text) > w - 4)
			{
				font = isFocus ? (mFont.tahoma_7_yellow ?? font) : (mFont.tahoma_7_white ?? font);
			}
			font.drawString(g, text, x + w / 2, textY, mFont.CENTER);
		}
		catch
		{
		}
	}

	/// <summary>
	/// Thực hiện vẽ và hiển thị thành phần PaintNativeButton lên canvas đồ họa.
	/// </summary>
	public static void PaintNativeButton(int x, int y, int w, string text, bool isFocus, mGraphics g)
	{
		PaintNativeButton(x, y, w, 18, text, isFocus, g);
	}

	/// <summary>
	/// Thực hiện vẽ và hiển thị thành phần PaintArrowButton lên canvas đồ họa.
	/// </summary>
	public static void PaintArrowButton(int x, int y, int w, int h, bool isUp, bool isFocus, mGraphics g)
	{
		PaintNativeButton(x, y, w, h, string.Empty, isFocus, g);
		if (Mob.imgHP != null)
		{
			int arrowX = x + (w - 9) / 2;
			int arrowY = y + (h - 6) / 2;
			g.drawRegion(Mob.imgHP, 0, 0, 9, 6, isUp ? 1 : 0, arrowX, arrowY, 0);
		}
		else
		{
			int cx = x + w / 2;
			int cy = y + h / 2;
			g.setColor(isFocus ? 0x00783e : 0x3c1400);
			if (isUp)
			{
				for (int r = 0; r < 4; r++)
				{
					g.fillRect(cx - r, cy - 2 + r, r * 2 + 1, 1);
				}
			}
			else
			{
				for (int r = 0; r < 4; r++)
				{
					g.fillRect(cx - (3 - r), cy - 1 + r, (3 - r) * 2 + 1, 1);
				}
			}
		}
	}

	/// <summary>
	/// Thực hiện vẽ nút đóng [X] bằng asset gốc /mainImage/myTexture2dbtX.png.
	/// </summary>
	public static void PaintCloseButton(int x, int y, mGraphics g)
	{
		if (imgBtX == null)
		{
			try
			{
				imgBtX = GameCanvas.loadImage("/mainImage/myTexture2dbtX.png");
			}
			catch
			{
			}
		}
		if (imgBtX != null)
		{
			g.drawImage(imgBtX, x, y, 0);
		}
		else
		{
			PaintNativeButton(x, y, 18, 17, "X", false, g);
		}
	}

	/// <summary>
	/// Thực hiện vẽ và hiển thị thành phần PaintTanSatUI lên canvas đồ họa.
	/// </summary>
	public static void PaintTanSatUI(mGraphics g)
	{
		if (!uiCustomOpen)
		{
			return;
		}
		try
		{
			// Tự động scale tương thích mọi tỷ lệ và độ phân giải màn hình + Safe-Area
			int safeLeft = SafeArea.Left;
			int safeTop = SafeArea.Top;
			int safeRight = SafeArea.Right;
			int safeBottom = SafeArea.Bottom;

			int maxW = GameCanvas.w - 12 - safeLeft - safeRight;
			int maxH = GameCanvas.h - 12 - safeTop - safeBottom;
			int uiW = (maxW > 450) ? 450 : maxW;
			int uiH = (maxH > 260) ? 260 : maxH;
			if (uiW < 280) uiW = GameCanvas.w - safeLeft - safeRight;
			if (uiH < 200) uiH = GameCanvas.h - safeTop - safeBottom;
			int uiX = safeLeft + (GameCanvas.w - safeLeft - safeRight - uiW) / 2;
			int uiY = safeTop + (GameCanvas.h - safeTop - safeBottom - uiH) / 2;
			if (uiX < safeLeft) uiX = safeLeft;
			if (uiY < safeTop) uiY = safeTop;

			// 1. Khung Dialog chính: Asset chuẩn NRO
			GameCanvas.paintz.paintFrame(uiX, uiY, uiW, uiH, g);
			GameCanvas.paintz.paintFrameInside(uiX + 6, uiY + 6, uiW - 12, uiH - 12, g);

			// Tiêu đề
			string currentTabName = (selectedTab >= 0 && selectedTab < tabNames.Length) ? tabNames[selectedTab] : "CÀI ĐẶT";
			string title = "MENU MOD - " + currentTabName.ToUpper();
			mFont.tahoma_7b_dark.drawString(g, title, uiX + uiW / 2, uiY + 8, mFont.CENTER);

			// Nút [X] đóng góc phải
			PaintCloseButton(uiX + uiW - 25, uiY + 6, g);

			// 2. Cột Danh Mục Bên Trái (Chạm kéo trực tiếp - Không thanh cuộn)
			int colW = (uiW > 380) ? 80 : 72;
			int colX = uiX + 6;
			int colY = uiY + 24;
			int colH = uiH - 30;

			int closeBtnH = 17;
			int closeBtnY = colY + colH - closeBtnH - 2;
			int tabListH = closeBtnY - colY - 3;
			int itemH = 17;
			int itemStep = 19;
			int colContentH = tabNames.Length * itemStep + 2;
			int maxColScroll = (colContentH > tabListH) ? (colContentH - tabListH) : 0;
			if (colScrollY > maxColScroll) colScrollY = maxColScroll;
			if (colScrollY < 0) colScrollY = 0;

			GameCanvas.paintz.paintFrameSimple(colX, colY, colW, colH, g);
			g.setColor(15196114);
			g.fillRect(colX + 2, colY + 2, colW - 4, colH - 4);

			g.setClip(colX + 2, colY + 2, colW - 4, tabListH);
			for (int t = 0; t < tabNames.Length; t++)
			{
				int btnY = colY + 2 + t * itemStep - colScrollY;
				if (btnY + itemH < colY || btnY > colY + tabListH) continue;
				bool isSel = (selectedTab == t);
				PaintNativeButton(colX + 2, btnY, colW - 4, itemH, tabNames[t], isSel, g);
			}
			g.setClip(0, 0, GameCanvas.w, GameCanvas.h);

			// Nút ĐÓNG cố định ở đáy Cột Trái (nằm trọn vẹn bên trong khung)
			PaintNativeButton(colX + 2, closeBtnY, colW - 4, closeBtnH, "ĐÓNG", false, g);

			// 3. Vùng Nội Dung Chi Tiết Bên Phải (Chạm kéo trực tiếp - Không thanh cuộn)
			int detailX = colX + colW + 5;
			int detailY = uiY + 24;
			int detailW = uiW - (colW + 17);
			int detailH = uiH - 30;

			GameCanvas.paintz.paintFrameSimple(detailX, detailY, detailW, detailH, g);
			g.setColor(15787715);
			g.fillRect(detailX + 2, detailY + 2, detailW - 4, detailH - 4);

			g.setClip(detailX + 2, detailY + 2, detailW - 4, detailH - 4);

			// Áp dụng trượt ảo cho các tab có nội dung dài
			bool useDetailScroll = (selectedTab == 5 && !ModUIBackground.isOpen);
			if (useDetailScroll)
			{
				int maxPaintDetailScroll = (ModUIGraphics.ContentHeight - (detailH - 4) > 0) ? (ModUIGraphics.ContentHeight - (detailH - 4)) : 0;
				if (detailScrollY > maxPaintDetailScroll) detailScrollY = maxPaintDetailScroll;
				if (detailScrollY < 0) detailScrollY = 0;
				g.translate(0, -detailScrollY);
			}

			// Render Sub-Panel tương ứng
			switch (selectedTab)
			{
				case 0:
					ModUITanSat.Paint(detailX, detailY, detailW, detailH, g);
					break;
				case 1:
					ModUIAutoPick.Paint(detailX, detailY, detailW, detailH, g);
					break;
				case 2:
					ModUISpeed.Paint(detailX, detailY, detailW, detailH, g);
					break;
				case 3:
					ModUIAutoHeal.Paint(detailX, detailY, detailW, detailH, g);
					break;
				case 4:
					ModUIAutomation.Paint(detailX, detailY, detailW, detailH, g);
					break;
				case 5:
					if (ModUIBackground.isOpen)
					{
						ModUIBackground.Paint(detailX, detailY, detailW, detailH, g);
					}
					else
					{
						ModUIGraphics.Paint(detailX, detailY, detailW, detailH, g);
					}
					break;
				case 6:
					ModUIBoss.Paint(detailX, detailY, detailW, detailH, g);
					break;
				case 7:
					ModUINextMap.Paint(detailX, detailY, detailW, detailH, g);
					break;
				case 8:
					ModUIGoBack.Paint(detailX, detailY, detailW, detailH, g);
					break;
				case 9:
					ModUISetActivator.Paint(detailX, detailY, detailW, detailH, g);
					break;
				case 10:
					ModUIMultiTab.Paint(detailX, detailY, detailW, detailH, g);
					break;
				case 11:
					ModUIHelp.Paint(detailX, detailY, detailW, detailH, g);
					break;
			}

			if (useDetailScroll)
			{
				g.translate(0, detailScrollY);
			}

			g.setClip(0, 0, GameCanvas.w, GameCanvas.h);
		}
		catch
		{
		}
	}

	/// <summary>
	/// Xử lý nghiệp vụ logic chuyên trách cho HandleTap.
	/// </summary>
	public static void HandleTap()
	{
		try
		{

			// Kiểm tra click vào nút mũi tên Mod Menu
			if (ModArrowButton.CheckClick())
			{
				return;
			}

			if (!ModMenu.IsInGame() && !uiCustomOpen)
			{
				return;
			}

			int safeLeft = SafeArea.Left;
			int safeTop = SafeArea.Top;
			int safeRight = SafeArea.Right;
			int safeBottom = SafeArea.Bottom;

			int maxW = GameCanvas.w - 12 - safeLeft - safeRight;
			int maxH = GameCanvas.h - 12 - safeTop - safeBottom;
			int uiW = (maxW > 450) ? 450 : maxW;
			int uiH = (maxH > 260) ? 260 : maxH;
			if (uiW < 280) uiW = GameCanvas.w - safeLeft - safeRight;
			if (uiH < 200) uiH = GameCanvas.h - safeTop - safeBottom;
			int uiX = safeLeft + (GameCanvas.w - safeLeft - safeRight - uiW) / 2;
			int uiY = safeTop + (GameCanvas.h - safeTop - safeBottom - uiH) / 2;
			if (uiX < safeLeft) uiX = safeLeft;
			if (uiY < safeTop) uiY = safeTop;

			if (uiCustomOpen)
			{
				int px = GameCanvas.px;
				int py = GameCanvas.py;

				int colW = (uiW > 380) ? 80 : 72;
				int colX = uiX + 6;
				int colY = uiY + 24;
				int colH = uiH - 30;

				int closeBtnH = 17;
				int closeBtnY = colY + colH - closeBtnH - 2;
				int tabListH = closeBtnY - colY - 3;
				int itemH = 17;
				int itemStep = 19;
				int colContentH = tabNames.Length * itemStep + 2;
				int maxColScroll = (colContentH > tabListH) ? (colContentH - tabListH) : 0;

				int detailX = colX + colW + 5;
				int detailY = uiY + 24;
				int detailW = uiW - (colW + 17);
				int detailH = uiH - 30;
				if (selectedTab == 10 && ModUIMultiTab.HandlePointer(detailX, detailY, detailW, detailH))
				{
					GameCanvas.clearAllPointerEvent();
					return;
				}
				int maxDetailScroll = (ModUIGraphics.ContentHeight - (detailH - 4) > 0) ? (ModUIGraphics.ContentHeight - (detailH - 4)) : 0;

				// Đọc trạng thái chuột & cảm ứng chuẩn hóa qua GameCanvas
				bool isDown = GameCanvas.isPointerDown;
				bool isJustRelease = GameCanvas.isPointerJustRelease;

				// 1. Xử lý con lăn chuột (Mouse ScrollWheel)
				float wheel = GameCanvas.pXYScrollMouse;
				if (wheel != 0)
				{
					GameCanvas.pXYScrollMouse = 0;
				}
				if (wheel != 0)
				{
					// Con lăn chuột trên Cột Trái
					if (px >= colX && px <= colX + colW && py >= colY && py <= colY + tabListH)
					{
						int step = 28;
						if (wheel > 0) colScrollY -= step;
						else if (wheel < 0) colScrollY += step;
						if (colScrollY < 0) colScrollY = 0;
						if (colScrollY > maxColScroll) colScrollY = maxColScroll;
					}
					// Con lăn chuột trên Khung Phải
					else if (px >= detailX && px <= detailX + detailW && py >= detailY && py <= detailY + detailH)
					{
						if (selectedTab == 5 && !ModUIBackground.isOpen)
						{
							int step = 28;
							if (wheel > 0) detailScrollY -= step;
							else if (wheel < 0) detailScrollY += step;
							if (detailScrollY < 0) detailScrollY = 0;
							if (detailScrollY > maxDetailScroll) detailScrollY = maxDetailScroll;
						}
						else if (selectedTab == 5 && ModUIBackground.isOpen)
						{
							ModUIBackground.OnMouseScroll(wheel);
						}
						else if (selectedTab == 4)
						{
							ModUIAutomation.OnMouseScroll(wheel);
						}
						else if (selectedTab == 11)
						{
							ModUIHelp.OnMouseScroll(wheel);
						}
						else if (selectedTab == 0)
						{
							ModUITanSat.OnMouseScroll(wheel);
						}
						else if (selectedTab == 7)
						{
							ModUINextMap.OnMouseScroll(wheel);
						}
						else if (selectedTab == 9)
						{
							ModUISetActivator.OnMouseScroll(wheel);
						}
					}
				}

				// 2. Chạm kéo trượt trực tiếp (Direct Touch/Drag)
				if (isDown)
				{
					if (!isColDragging && !isDetailDragging)
					{
						if (px >= colX && px <= colX + colW && py >= colY && py <= colY + tabListH)
						{
							isColDragging = true;
							hasColDragged = false;
							startColDragY = py;
							lastColDragY = py;
						}
						else if (px >= detailX && px <= detailX + detailW && py >= detailY && py <= detailY + detailH)
						{
							if (selectedTab == 5 && !ModUIBackground.isOpen)
							{
								isDetailDragging = true;
								hasDetailDragged = false;
								startDetailDragY = py;
								lastDetailDragY = py;
							}
						}
					}

					if (isColDragging)
					{
						int moveY = py - lastColDragY;
						lastColDragY = py;
						if (Res.abs(py - startColDragY) > 5)
						{
							hasColDragged = true;
						}
						if (hasColDragged && maxColScroll > 0)
						{
							colScrollY -= moveY;
							if (colScrollY < 0) colScrollY = 0;
							if (colScrollY > maxColScroll) colScrollY = maxColScroll;
						}
					}
					else if (isDetailDragging)
					{
						int moveY = py - lastDetailDragY;
						lastDetailDragY = py;
						if (Res.abs(py - startDetailDragY) > 5)
						{
							hasDetailDragged = true;
						}
						if (hasDetailDragged && maxDetailScroll > 0)
						{
							detailScrollY -= moveY;
							if (detailScrollY < 0) detailScrollY = 0;
							if (detailScrollY > maxDetailScroll) detailScrollY = maxDetailScroll;
						}
					}
				}

				// Xử lý kéo thả cho các Sub-Panel chuyên dụng
				if (selectedTab == 11)
				{
					ModUIHelp.UpdateDragScroll(px, py, detailX, detailY, detailW, detailH);
				}
				else if (selectedTab == 4)
				{
					ModUIAutomation.UpdateDragScroll(px, py, detailX, detailY, detailW, detailH);
				}
				else if (selectedTab == 0)
				{
					ModUITanSat.UpdateDragScroll(px, py, detailX, detailY, detailW, detailH);
				}
				else if (selectedTab == 7)
				{
					ModUINextMap.UpdateDragScroll(px, py, detailX, detailY, detailW, detailH);
				}
				else if (selectedTab == 9)
				{
					ModUISetActivator.UpdateDragScroll(px, py, detailX, detailY, detailW, detailH);
				}
				else if (selectedTab == 5 && ModUIBackground.isOpen)
				{
					ModUIBackground.UpdateDragScroll(px, py, detailX, detailY, detailW, detailH);
				}

				// 3. Xử lý click/chạm khi nhấc tay (Release / Click)
				if (isJustRelease)
				{
					// Nếu click hoặc nhấc tay trên tab list cột danh mục trái
					if (isColDragging || (px >= colX && px <= colX + colW && py >= colY && py <= colY + tabListH))
					{
						isColDragging = false;
						if (!hasColDragged && px >= colX + 2 && px <= colX + colW - 2 && py >= colY + 2 && py <= colY + tabListH)
						{
							int relY = py - (colY + 2) + colScrollY;
							if (relY >= 0)
							{
								int clickedIdx = relY / itemStep;
								if (clickedIdx >= 0 && clickedIdx < tabNames.Length)
								{
									selectedTab = clickedIdx;
									detailScrollY = 0;
									ModUIBackground.isOpen = false;
									ModConfig.SaveConfig();
									SoundMn.gI().buttonClick();
									GameCanvas.isPointerJustRelease = GameCanvas.isPointerClick = false;
									return;
								}
							}
						}
						hasColDragged = false;
					}

					// Nút [ĐÓNG] ở đáy Cột Trái
					if (px >= colX && px <= colX + colW && py >= closeBtnY - 2 && py <= closeBtnY + closeBtnH + 2)
					{
						uiCustomOpen = false;
						ModUIBackground.isOpen = false;
						ModConfig.SaveConfig();
						SoundMn.gI().buttonClose();
						GameCanvas.isPointerJustRelease = GameCanvas.isPointerClick = false;
						return;
					}

					// Nếu click hoặc nhấc tay trên khung nội dung phải (Tab Đồ Họa)
					if (isDetailDragging || (px >= detailX && px <= detailX + detailW && py >= detailY && py <= detailY + detailH))
					{
						if (selectedTab == 5 && !ModUIBackground.isOpen && !hasDetailDragged)
						{
							if (ModUIGraphics.HandleTap(px, py + detailScrollY, detailX, detailY, detailW, detailH))
							{
								isDetailDragging = false;
								hasDetailDragged = false;
								GameCanvas.isPointerJustRelease = GameCanvas.isPointerClick = false;
								return;
							}
						}
						isDetailDragging = false;
						hasDetailDragged = false;
					}

					// Nút [X] đóng góc phải
					if (px >= uiX + uiW - 32 && px <= uiX + uiW - 4 && py >= uiY + 4 && py <= uiY + 28)
					{
						uiCustomOpen = false;
						ModUIBackground.isOpen = false;
						ModConfig.SaveConfig();
						SoundMn.gI().buttonClose();
						GameCanvas.isPointerJustRelease = GameCanvas.isPointerClick = false;
						return;
					}

					// Xử lý click trong Vùng Chi Tiết bên phải cho các Tab khác
					if (px >= detailX && px <= detailX + detailW && py >= detailY && py <= detailY + detailH)
					{
						bool handled = false;
						switch (selectedTab)
						{
							case 0:
								handled = ModUITanSat.HandleTap(px, py, detailX, detailY, detailW, detailH);
								break;
							case 1:
								handled = ModUIAutoPick.HandleTap(px, py, detailX, detailY, detailW, detailH);
								break;
							case 2:
								handled = ModUISpeed.HandleTap(px, py, detailX, detailY, detailW, detailH);
								break;
							case 3:
								handled = ModUIAutoHeal.HandleTap(px, py, detailX, detailY, detailW, detailH);
								break;
							case 4:
								handled = ModUIAutomation.HandleTap(px, py, detailX, detailY, detailW, detailH);
								break;
							case 5:
								if (ModUIBackground.isOpen)
								{
									handled = ModUIBackground.HandleTap(px, py, detailX, detailY, detailW, detailH);
								}
								break;
							case 6:
								handled = ModUIBoss.HandleTap(px, py, detailX, detailY, detailW, detailH);
								break;
							case 7:
								handled = ModUINextMap.HandleTap(px, py, detailX, detailY, detailW, detailH);
								break;
							case 8:
								handled = ModUIGoBack.HandleTap(px, py, detailX, detailY, detailW, detailH);
								break;
							case 9:
								handled = ModUISetActivator.HandleTap(px, py, detailX, detailY, detailW, detailH);
								break;
							case 10:
								handled = ModUIMultiTab.HandleTap(px, py, detailX, detailY, detailW, detailH);
								break;
							case 11:
								handled = ModUIHelp.HandleTap(px, py, detailX, detailY, detailW, detailH);
								break;
						}
						if (handled)
						{
							GameCanvas.isPointerJustRelease = GameCanvas.isPointerClick = false;
							return;
						}
					}

					// Chặn sự kiện click xuyên xuống thế giới game
					if (px >= uiX && px <= uiX + uiW && py >= uiY && py <= uiY + uiH)
					{
						GameCanvas.isPointerJustRelease = GameCanvas.isPointerClick = false;
					}
				}

				if (px >= uiX && px <= uiX + uiW && py >= uiY && py <= uiY + uiH)
				{
					GameCanvas.isPointerJustDown = false;
				}

				if (!isDown)
				{
					isColDragging = false;
					hasColDragged = false;
					isDetailDragging = false;
					hasDetailDragged = false;
				}
			}
		}
		catch
		{
		}
	}
}

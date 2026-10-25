using System;
using UnityEngine;

/// <summary>
/// CLASS ModUIHelp: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
public static class ModUIHelp
{
	/// <summary>
	/// STRUCT CommandInfo: Định nghĩa và điều phối logic trong hệ thống game.
	/// </summary>
	public struct CommandInfo
	{
		public string cmd;
		public string desc;
		public string tag;
		public int tagColor;

		/// <summary>
		/// Thực thi phương thức nghiệp vụ CommandInfo.
		/// </summary>
		public CommandInfo(string cmd, string desc, string tag, int tagColor)
		{
			this.cmd = cmd;
			this.desc = desc;
			this.tag = tag;
			this.tagColor = tagColor;
		}
	}

	public static int scrollY = 0;

	public static readonly CommandInfo[] commands = new CommandInfo[]
	{
		// Nhóm Úp Set & Nhặt Đồ
		new CommandInfo("upset", "Bật / Tắt Auto Úp Set Kích Hoạt", "Chat", 0x00e676),
		/// <summary>
		/// Thực thi phương thức nghiệp vụ CommandInfo.
		/// </summary>
		new CommandInfo("banrac", "Tự về Urôn bán rác rồi quay lại farm", "Chat", 0x00e676),
		/// <summary>
		/// Thực thi phương thức nghiệp vụ CommandInfo.
		/// </summary>
		new CommandInfo("iditem", "Bật / Tắt hiện Tên & ID trên ô item", "Chat", 0x00e676),
		/// <summary>
		/// Thực thi phương thức nghiệp vụ CommandInfo.
		/// </summary>
		new CommandInfo("nhat_id", "Cài đặt lọc ID nhặt (nhập ID)", "Chat", 0x00e676),
		/// <summary>
		/// Thực thi phương thức nghiệp vụ CommandInfo.
		/// </summary>
		new CommandInfo("nhat_ten", "Cài đặt lọc Tên nhặt (nhập từ khóa)", "Chat", 0x00e676),
		/// <summary>
		/// Thực thi phương thức nghiệp vụ CommandInfo.
		/// </summary>
		new CommandInfo("xnhat", "Xóa toàn bộ lọc ID và Tên nhặt", "Chat", 0x00e676),
		/// <summary>
		/// Thực thi phương thức nghiệp vụ CommandInfo.
		/// </summary>
		new CommandInfo("roido", "Bật / Tắt Hút Đồ Tức Thì (Instant Pick)", "Chat", 0x00e676),
		/// <summary>
		/// Thực thi phương thức nghiệp vụ CommandInfo.
		/// </summary>
		new CommandInfo("tkrd", "Xem thống kê quái diệt & tỉ lệ rơi đồ", "Chat", 0x00e676),

		// Nhóm Boss & Broly
		new CommandInfo("kbroly", "Bật / Tắt Auto Né Broly (Kite áp sát)", "Chat", 0xff9800),
		/// <summary>
		/// Thực thi phương thức nghiệp vụ CommandInfo.
		/// </summary>
		new CommandInfo("kc", "Bật / Tắt Khinh công Broly (bay lơ lửng)", "Chat", 0xff9800),

		// Nhóm Bùa & Tiện Ích
		new CommandInfo("muabua", "Tự đến Bà Hạt Mít mua bùa đã chọn", "Chat", 0x29b6f6),
		/// <summary>
		/// Thực thi phương thức nghiệp vụ CommandInfo.
		/// </summary>
		new CommandInfo("autobua", "Bật / Tắt tự động gia hạn bùa khi hết", "Chat", 0x29b6f6),
		/// <summary>
		/// Thực thi phương thức nghiệp vụ CommandInfo.
		/// </summary>
		new CommandInfo("gb", "Bật / Tắt Auto GoBack về chỗ cũ khi chết", "Chat", 0x29b6f6),
		/// <summary>
		/// Thực thi phương thức nghiệp vụ CommandInfo.
		/// </summary>
		new CommandInfo("nextmap=...", "safe:ID (an toàn) hoặc fast:ID (nhanh)", "Chat", 0x29b6f6),
		/// <summary>
		/// Thực thi phương thức nghiệp vụ CommandInfo.
		/// </summary>
		new CommandInfo("stopmap", "Dừng Next Map đang chạy", "Chat", 0x29b6f6),
		/// <summary>
		/// Thực thi phương thức nghiệp vụ CommandInfo.
		/// </summary>
		new CommandInfo("td", "Thu hoạch Đậu Thần trên cây ngay", "Chat", 0x29b6f6),
		/// <summary>
		/// Thực thi phương thức nghiệp vụ CommandInfo.
		/// </summary>
		new CommandInfo("cd", "Tự động xin đậu / cho đậu trong bang", "Chat", 0x29b6f6),
		/// <summary>
		/// Thực thi phương thức nghiệp vụ CommandInfo.
		/// </summary>
		new CommandInfo("cde", "Bật / Tắt tự cho đệ tử ăn đậu khi kêu", "Chat", 0x29b6f6),

		// Nhóm Ngôn Ngữ & Dữ Liệu
		new CommandInfo("dich", "Đổi ngôn ngữ Việt Hoá / Gốc Server", "Chat", 0xab47bc),
		/// <summary>
		/// Thực thi phương thức nghiệp vụ CommandInfo.
		/// </summary>
		new CommandInfo("bg", "Mở Quản Lý Hình Nền Phong Cảnh (Git)", "Chat", 0xab47bc),

		// Nhóm Nhân Bản & Đa Tab
		new CommandInfo("nhanban", "Mở Menu Quản Lý Tab Nhân Bản", "Chat", 0x7e57c2),
		/// <summary>
		/// Thực thi phương thức nghiệp vụ CommandInfo.
		/// </summary>
		new CommandInfo("hub / hud", "Bật / Tắt Menu HUB Bong Bóng Nổi", "Chat", 0x7e57c2),
		/// <summary>
		/// Thực thi phương thức nghiệp vụ CommandInfo.
		/// </summary>
		new CommandInfo("tabN", "Mở Tab theo số thứ tự", "Chat", 0x7e57c2),
		/// <summary>
		/// Thực thi phương thức nghiệp vụ CommandInfo.
		/// </summary>
		new CommandInfo("tab", "Luân chuyển sang Tab tiếp theo trong game", "Chat", 0x7e57c2),
		/// <summary>
		/// Thực thi phương thức nghiệp vụ CommandInfo.
		/// </summary>
		new CommandInfo("autochat", "Bật/tắt Auto Chat; autochat=Nội dung|ms", "Chat", 0x00e676),
		/// <summary>
		/// Thực thi phương thức nghiệp vụ CommandInfo.
		/// </summary>
		new CommandInfo("boxsort", "Sắp xếp rương ngay", "Chat", 0x00e676),
		/// <summary>
		/// Thực thi phương thức nghiệp vụ CommandInfo.
		/// </summary>
		new CommandInfo("autoboxsort", "Bật/tắt tự động sắp xếp rương", "Chat", 0x00e676),
		/// <summary>
		/// Thực thi phương thức nghiệp vụ CommandInfo.
		/// </summary>
		new CommandInfo("autopoint", "Bật/tắt Auto cộng điểm tiềm năng", "Chat", 0x00e676),
		/// <summary>
		/// Thực thi phương thức nghiệp vụ CommandInfo.
		/// </summary>
		new CommandInfo("autopoint=...", "Cấu hình: autopoint=type|điểm|ms", "Chat", 0x00e676),
		/// <summary>
		/// Thực thi phương thức nghiệp vụ CommandInfo.
		/// </summary>
		new CommandInfo("autofreeze", "Bật/tắt đóng băng hồi chiêu; autofreeze=ID theo chiêu", "Chat", 0x00e676),
		/// <summary>
		/// Thực thi phương thức nghiệp vụ CommandInfo.
		/// </summary>
		new CommandInfo("autoitem=...", "Cấu hình: autoitem=ID1,ID2|chu_kỳ_ms", "Chat", 0x00e676),
		/// <summary>
		/// Thực thi phương thức nghiệp vụ CommandInfo.
		/// </summary>
		new CommandInfo("autoitemhp", "Cấu hình: autoitemhp=ID|%_HP|ms", "Chat", 0x00e676),
		/// <summary>
		/// Thực thi phương thức nghiệp vụ CommandInfo.
		/// </summary>
		new CommandInfo("autoitemmp", "Cấu hình: autoitemmp=ID|%_MP|ms", "Chat", 0x00e676),
		/// <summary>
		/// Thực thi phương thức nghiệp vụ CommandInfo.
		/// </summary>
		new CommandInfo("schedule=...", "Hẹn giờ: schedule=08:00-18:00", "Chat", 0x00e676),
		/// <summary>
		/// Thực thi phương thức nghiệp vụ CommandInfo.
		/// </summary>
		new CommandInfo("autorevive", "Bật/tắt tự hồi sinh bằng packet chuẩn", "Chat", 0xff9800),
		/// <summary>
		/// Thực thi phương thức nghiệp vụ CommandInfo.
		/// </summary>
		new CommandInfo("autopet=...", "Cấu hình: autopet=mã_trạng_thái|ms", "Chat", 0xff9800),
		/// <summary>
		/// Thực thi phương thức nghiệp vụ CommandInfo.
		/// </summary>
		new CommandInfo("autofusion", "Cấu hình: autofusion=type|chu_kỳ_ms", "Chat", 0xff9800),

		// Nhóm Phím Tắt PC
		new CommandInfo("Phím ~ / F2", "Bật / Tắt Giao diện Menu Mod", "Phím", 0xffca28),
		/// <summary>
		/// Thực thi phương thức nghiệp vụ CommandInfo.
		/// </summary>
		new CommandInfo("F11", "Bật / Tắt Toàn Màn Hình (Fullscreen)", "Phím", 0xffca28),
		/// <summary>
		/// Thực thi phương thức nghiệp vụ CommandInfo.
		/// </summary>
		new CommandInfo("Phím Home", "Giải kẹt nhân vật khẩn cấp (Unstuck)", "Phím", 0xffca28),
		/// <summary>
		/// Thực thi phương thức nghiệp vụ CommandInfo.
		/// </summary>
		new CommandInfo("Click Logo", "Bấm Logo TriHienKun để mở Menu Mod", "Chuột", 0x26a69a),
		/// <summary>
		/// Thực thi phương thức nghiệp vụ CommandInfo.
		/// </summary>
		new CommandInfo("set1 / set2 / xoaset", "Mặc nhanh Set Đồ 1 / Set Đồ 2 / Xóa Set", "Chat", 0x00e676),
		new CommandInfo("xoanguoi / ttsp", "Xóa người chơi khác / Bật HUD Sư Phụ", "Chat", 0x29b6f6),
		new CommandInfo("tusat / captcha", "Tự sát nhanh / Bật tắt Auto Giải Captcha", "Chat", 0xff9800),
		new CommandInfo("Phím E / N / Z", "Tự đánh / Auto Nhặt / Bật tắt Cờ đen", "Phím", 0xffca28),
		new CommandInfo("Phím C / F / T", "Dùng Capsule / Bông tai / Tự Động Luyện Tập", "Phím", 0xffca28),
		new CommandInfo("Phím J / K / L", "Qua nhanh cổng Map Trái / Giữa / Phải", "Phím", 0xffca28),
		new CommandInfo("Phím B / G / I / M / X", "Bạn bè / Giao dịch / Khóa mục tiêu / Khu / Menu Mod", "Phím", 0xffca28),
		new CommandInfo("update", "Kiểm tra & Tải cập nhật mới (GitHub)", "Hệ thống", 0x00e676),
	};

	private static bool isDragging = false;
	private static bool hasDragged = false;
	private static int startDragY = 0;
	private static int lastDragY = 0;

	/// <summary>
	/// Bộ xử lý sự kiện tiếp nhận và phản hồi OnMouseScroll.
	/// </summary>
	public static void OnMouseScroll(float wheel)
	{
		int listH = 186;
		int itemH = 20;
		int contentH = commands.Length * itemH + 4;
		int maxScroll = (contentH > listH - 4) ? (contentH - (listH - 4)) : 0;
		if (maxScroll <= 0) return;

		int step = 24;
		if (wheel > 0)
		{
			scrollY -= step;
			if (scrollY < 0) scrollY = 0;
		}
		else if (wheel < 0)
		{
			scrollY += step;
			if (scrollY > maxScroll) scrollY = maxScroll;
		}
	}

	/// <summary>
	/// Cập nhật trạng thái logic của UpdateDragScroll theo chu kỳ khung hình.
	/// </summary>
	public static void UpdateDragScroll(int px, int py, int uiX, int uiY, int uiW, int uiH)
	{
		int listX = uiX + 5;
		int listY = uiY + 20;
		int listW = uiW - 10;
		int listH = uiH - 24;

		int itemH = 20;
		int contentH = commands.Length * itemH + 4;
		int maxScroll = (contentH > listH - 4) ? (contentH - (listH - 4)) : 0;

		bool isDown = GameCanvas.isPointerDown;

		if (isDown)
		{
			if (!isDragging)
			{
				if (px >= listX && px <= listX + listW && py >= listY && py <= listY + listH)
				{
					isDragging = true;
					hasDragged = false;
					startDragY = py;
					lastDragY = py;
				}
			}
			else
			{
				int moveY = py - lastDragY;
				lastDragY = py;
				if (Res.abs(py - startDragY) > 5)
				{
					hasDragged = true;
				}
				if (hasDragged && maxScroll > 0)
				{
					scrollY -= moveY;
					if (scrollY < 0) scrollY = 0;
					if (scrollY > maxScroll) scrollY = maxScroll;
				}
			}
		}
		else
		{
			isDragging = false;
		}
	}

	/// <summary>
	/// Thực hiện vẽ toàn bộ đối tượng và các lớp hiển thị lên canvas mGraphics.
	/// </summary>
	public static void Paint(int uiX, int uiY, int uiW, int uiH, mGraphics g)
	{
		int listX = uiX + 5;
		int listY = uiY + 20;
		int listW = uiW - 10;
		int listH = uiH - 24;

		// Tiêu đề danh sách
		mFont.tahoma_7_grey.drawString(g, "LỆNH CHAT & PHÍM TẮT (" + commands.Length + " mục - Chạm kéo trượt):", listX, uiY + 6, mFont.LEFT);

		// Khung chứa danh sách
		GameCanvas.paintz.paintFrameSimple(listX, listY, listW, listH, g);
		g.setColor(15196114);
		g.fillRect(listX + 2, listY + 2, listW - 4, listH - 4);

		int itemH = 20;
		int contentH = commands.Length * itemH + 4;
		int maxScroll = (contentH > listH - 4) ? (contentH - (listH - 4)) : 0;
		if (scrollY > maxScroll) scrollY = maxScroll;
		if (scrollY < 0) scrollY = 0;

		g.setClip(listX + 2, listY + 2, listW - 4, listH - 4);

		for (int i = 0; i < commands.Length; i++)
		{
			CommandInfo info = commands[i];
			int rowY = listY + 3 + i * itemH - scrollY;

			if (rowY + itemH < listY || rowY > listY + listH)
			{
				continue;
			}

			// Nền xen kẽ
			if (i % 2 == 1)
			{
				g.setColor(15724527);
				g.fillRect(listX + 3, rowY, listW - 6, itemH - 1);
			}

			bool hasTryBtn = info.tag.Equals("Chat") || info.tag.Equals("Hệ thống");
			int textClipW = hasTryBtn ? (listW - 48) : (listW - 6);
			g.setClip(listX + 3, rowY, textClipW, itemH);

			// Tag loại lệnh
			g.setColor(info.tagColor);
			g.fillRect(listX + 5, rowY + 3, 3, 12);

			// Tên lệnh / Phím tắt
			mFont.tahoma_7_green2.drawString(g, info.cmd, listX + 12, rowY + 2, mFont.LEFT);

			// Nhãn loại
			mFont.tahoma_7_grey.drawString(g, "[" + info.tag + "]", listX + 78, rowY + 2, mFont.LEFT);

			// Mô tả chức năng
			mFont.tahoma_7_grey.drawString(g, info.desc, listX + 122, rowY + 2, mFont.LEFT);

			g.setClip(listX + 2, listY + 2, listW - 4, listH - 4);

			// Phím thử ngay
			if (hasTryBtn)
			{
				ModUI.PaintNativeButton(listX + listW - 44, rowY + 2, 38, 15, "Thử", false, g);
			}
		}

		g.setClip(0, 0, GameCanvas.w, GameCanvas.h);
	}

	/// <summary>
	/// Xử lý nghiệp vụ logic chuyên trách cho HandleTap.
	/// </summary>
	public static bool HandleTap(int px, int py, int uiX, int uiY, int uiW, int uiH)
	{
		if (hasDragged)
		{
			hasDragged = false;
			return false;
		}

		int listX = uiX + 5;
		int listY = uiY + 20;
		int listW = uiW - 10;
		int listH = uiH - 24;

		if (px < listX || px > listX + listW || py < listY || py > listY + listH)
		{
			return false;
		}

		int itemH = 20;

		for (int i = 0; i < commands.Length; i++)
		{
			int rowY = listY + 3 + i * itemH - scrollY;
			if (rowY + itemH < listY || rowY > listY + listH)
			{
				continue;
			}

			int btnX = listX + listW - 46;
			if (px >= btnX && px <= btnX + 42 && py >= rowY + 1 && py <= rowY + 18)
			{
				ExecuteCommand(commands[i].cmd);
				SoundMn.gI().buttonClick();
				return true;
			}
		}

		return false;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ ExecuteCommand.
	/// </summary>
	private static void ExecuteCommand(string cmd)
	{
		try
		{
			if (cmd.Equals("upset"))
			{
				ModSetActivator.ToggleActive();
			}
			else if (cmd.Equals("banrac"))
			{
				ModSetActivator.StartGoSellJunkNow();
			}
			else if (cmd.Equals("iditem"))
			{
				ModSetActivator.showItemId = !ModSetActivator.showItemId;
				ModConfig.SaveConfig();
				GameScr.info1.addInfo("Hiện ID & Tên Item: " + (ModSetActivator.showItemId ? "BẬT" : "TẮT"), 0);
			}
			else if (cmd.Equals("xnhat"))
			{
				ModAutoPick.ClearFilters();
				ModConfig.SaveConfig();
			}
			else if (cmd.Equals("roido"))
			{
				ModDropRate.isInstantPick = !ModDropRate.isInstantPick;
				ModConfig.SaveConfig();
				GameScr.info1.addInfo("Hút Đồ Tức Thì: " + (ModDropRate.isInstantPick ? "BẬT" : "TẮT"), 0);
			}
			else if (cmd.Equals("tkrd"))
			{
				GameScr.info1.addInfo("TK Rơi Đồ: " + ModDropRate.totalItemsDropped + "/" + ModDropRate.totalMobsKilled + " (" + ModDropRate.GetDropRatePercent() + "%), " + ModDropRate.GetMobsPerMinute() + " quái/ph", 0);
			}
			else if (cmd.Equals("kbroly"))
			{
				ModKiteBroly.ToggleAutoKite();
			}
			else if (cmd.Equals("kc"))
			{
				ModKiteBroly.ToggleKhinhCong();
			}
			else if (cmd.Equals("muabua"))
			{
				ModAutoBuyBua.StartBuyBuaNow();
			}
			else if (cmd.Equals("autobua"))
			{
				ModAutoBuyBua.ToggleAutoRebuy();
			}
			else if (cmd.StartsWith("gb"))
			{
				ModGoBack.ToggleGoBack();
			}
			else if (cmd.Equals("td"))
			{
				ModAutoHeal.HarvestMagicTreeNow();
			}
			else if (cmd.Equals("cd"))
			{
				ModAutoHeal.DonateClanNow();
			}
			else if (cmd.Equals("cde"))
			{
				ModAutoHeal.autoFeedPetOnAsk = !ModAutoHeal.autoFeedPetOnAsk;
				ModConfig.SaveConfig();
				GameScr.info1.addInfo("Ăn đậu cho đệ tử: " + (ModAutoHeal.autoFeedPetOnAsk ? "BẬT" : "TẮT"), 0);
			}
			else if (cmd.Equals("dich"))
			{
				ModConfig.isTranslate = !ModConfig.isTranslate;
				ModConfig.SaveConfig();
				ModTranslate.ApplyAllTranslations();
				GameScr.info1.addInfo("Dịch Việt Hoá: " + (ModConfig.isTranslate ? "BẬT" : "TẮT (Gốc Server)"), 0);
			}
			else if (cmd.Equals("bg"))
			{
				ModUI.selectedTab = 5;
				ModUIBackground.isOpen = true;
				ModUI.uiCustomOpen = true;
			}
			else if (cmd.Equals("nhanban"))
			{
				ModUI.selectedTab = 10;
				ModUI.uiCustomOpen = true;
			}
			else if (cmd.Contains("hub") || cmd.Contains("hud"))
			{
				ModFloatingHUD.isShowFloatingHUD = !ModFloatingHUD.isShowFloatingHUD;
				ModConfig.SaveConfig();
				GameScr.info1.addInfo("Menu HUB Nổi: " + (ModFloatingHUD.isShowFloatingHUD ? "BẬT" : "TẮT"), 0);
			}
			else if (cmd.Equals("tab"))
			{
				DragonBoy_Net8_Native.Engine.Platform.PlatformService.Current.CycleNextTab();
			}
			else if (cmd.Equals("alltab"))
			{
				DragonBoy_Net8_Native.Engine.Platform.PlatformService.Current.OpenAllTabs();
			}
			else if (cmd.StartsWith("nhat_id"))
			{
				ModAutoPick.ShowInputFilterId();
			}
			else if (cmd.StartsWith("nhat_ten"))
			{
				ModAutoPick.ShowInputFilterName();
			}
			else if (cmd.StartsWith("nextmap="))
			{
				ModUI.selectedTab = 7;
				ModUI.uiCustomOpen = true;
			}
			else if (cmd.Equals("stopmap"))
			{
				ModNextMap.StopNextMap();
			}
			else if (cmd.Equals("tabN"))
			{
				DragonBoy_Net8_Native.Engine.Platform.PlatformService.Current.CycleNextTab();
			}
			else if (cmd.StartsWith("autochat"))
			{
				ModAutoChat.Toggle();
			}
			else if (cmd.Equals("boxsort"))
			{
				ModAutoBoxSort.SortNow();
				GameScr.info1?.addInfo("Đã gửi lệnh sắp xếp rương.", 0);
			}
			else if (cmd.Equals("autoboxsort"))
			{
				ModAutoBoxSort.Toggle();
			}
			else if (cmd.StartsWith("autopoint"))
			{
				ModAutoPoint.Toggle();
			}
			else if (cmd.Equals("autofreeze"))
			{
				ModTanSat.freezeSkillCooldown = !ModTanSat.freezeSkillCooldown;
				ModConfig.SaveConfig();
				GameScr.info1?.addInfo("Đóng băng hồi chiêu tất cả chiêu: " + (ModTanSat.freezeSkillCooldown ? "BẬT" : "TẮT"), 0);
			}
			else if (cmd.StartsWith("set1"))
			{
				ModInventoryExt.EquipSet(1);
			}
			else if (cmd.StartsWith("xoanguoi"))
			{
				ModGraphics.ToggleHideOtherPlayers();
			}
			else if (cmd.StartsWith("tusat"))
			{
				ModSelfKill.Toggle();
			}
			else if (cmd.Equals("autoitemhp"))
			{
				ModAutoItem.conditionType = 1;
				ModAutoItem.thresholdPercent = 50;
				ModConfig.SaveConfig();
				GameScr.info1?.addInfo("Auto Item: Đã đặt điều kiện dùng khi HP < 50%", 0);
			}
			else if (cmd.Equals("autoitemmp"))
			{
				ModAutoItem.conditionType = 2;
				ModAutoItem.thresholdPercent = 50;
				ModConfig.SaveConfig();
				GameScr.info1?.addInfo("Auto Item: Đã đặt điều kiện dùng khi KI < 50%", 0);
			}
			else if (cmd.StartsWith("autoitem"))
			{
				if (ModAutoItem.itemIds.Count == 0) ModAutoItem.ShowInputItemIds();
				else ModAutoItem.Toggle();
			}
			else if (cmd.StartsWith("schedule"))
			{
				ModSchedule.Toggle();
			}
			else if (cmd.Equals("autorevive"))
			{
				ModAutoRevive.Toggle();
			}
			else if (cmd.StartsWith("autopet"))
			{
				ModAutoPet.Toggle();
			}
			else if (cmd.StartsWith("autofusion"))
			{
				ModAutoFusion.Toggle();
			}
			else if (cmd.Contains("F11"))
			{
				ModGraphics.ToggleFullscreen();
			}
			else if (cmd.Contains("Home"))
			{
				ModHotkey.EmergencyUnstuck();
			}
			else if (cmd.Equals("update"))
			{
				ModAutoUpdate.CheckManual();
			}
		}
		catch
		{
		}
	}
}

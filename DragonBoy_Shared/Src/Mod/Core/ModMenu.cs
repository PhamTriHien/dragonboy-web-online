using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// CLASS ModMenu: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
public static class ModMenu
{
	#region Backward Compatibility Aliases

	// Tàn Sát
	public static bool autoTanSat
	{
		get { return ModTanSat.autoTanSat; }
		set { ModTanSat.autoTanSat = value; }
	}
	public static bool autoAttack
	{
		get { return ModTanSat.autoAttack; }
		set { ModTanSat.autoAttack = value; }
	}
	public static bool useTeleport
	{
		get { return ModTanSat.useTeleport; }
		set { ModTanSat.useTeleport = value; }
	}
	public static Mob currentFarmTarget
	{
		get { return ModTanSat.currentFarmTarget; }
		set { ModTanSat.currentFarmTarget = value; }
	}
	public static int selectedMobTemplateId
	{
		get { return ModTanSat.selectedMobTemplateId; }
		set { ModTanSat.selectedMobTemplateId = value; }
	}
	public static bool selectAllMobs
	{
		get { return ModTanSat.selectAllMobs; }
		set { ModTanSat.selectAllMobs = value; }
	}
	public static List<int> tickedMobTemplateIds
	{
		get { return ModTanSat.tickedMobTemplateIds; }
	}
	public static bool selectAllSkills
	{
		get { return ModTanSat.selectAllSkills; }
		set { ModTanSat.selectAllSkills = value; }
	}
	public static List<int> tickedSkillTemplateIds
	{
		get { return ModTanSat.tickedSkillTemplateIds; }
	}

	// Tự Nhặt
	public static bool autoPick
	{
		get { return ModAutoPick.autoPick; }
		set { ModAutoPick.autoPick = value; }
	}
	public static bool pickAll
	{
		get { return ModAutoPick.pickAll; }
		set { ModAutoPick.pickAll = value; }
	}
	public static bool pickGold
	{
		get { return ModAutoPick.pickGold; }
		set { ModAutoPick.pickGold = value; }
	}
	public static bool pickEquip
	{
		get { return ModAutoPick.pickEquip; }
		set { ModAutoPick.pickEquip = value; }
	}
	public static bool pickGem
	{
		get { return ModAutoPick.pickGem; }
		set { ModAutoPick.pickGem = value; }
	}

	// Tốc Chạy
	public static bool speedHack
	{
		get { return ModSpeed.speedHack; }
		set { ModSpeed.speedHack = value; }
	}
	public static float speedMult
	{
		get { return ModSpeed.speedMult; }
		set { ModSpeed.speedMult = value; }
	}

	// Bơm Đậu & Khóa HP
	public static bool autoPean
	{
		get { return ModAutoHeal.autoPean; }
		set { ModAutoHeal.autoPean = value; }
	}
	public static int autoPeanHpPercent
	{
		get { return ModAutoHeal.autoPeanHpPercent; }
		set { ModAutoHeal.autoPeanHpPercent = value; }
	}
	public static bool lockHPMP
	{
		get { return ModAutoHeal.lockHPMP; }
		set { ModAutoHeal.lockHPMP = value; }
	}

	// Đồ Họa & FPS
	public static int graphicsQuality
	{
		get { return ModGraphics.graphicsQuality; }
		set { ModGraphics.graphicsQuality = value; }
	}
	public static string[] graphicsNames
	{
		get { return ModGraphics.graphicsNames; }
	}
	public static int targetFps
	{
		get { return ModFps.targetFps; }
		set { ModFps.targetFps = value; }
	}
	public static bool isAutoFps
	{
		get { return ModFps.isAutoFps; }
		set { ModFps.isAutoFps = value; }
	}

	public static int pingMs
	{
		get { return ModFps.pingMs; }
		set { ModFps.pingMs = value; }
	}

	// Thông Báo Boss
	public static bool isShowBossNotice
	{
		get { return ModBossNotice.isShowBossNotice; }
		set { ModBossNotice.isShowBossNotice = value; }
	}
	public static List<ModBossNotice.BossNoticeEntry> listBossNotices
	{
		get { return ModBossNotice.listBossNotices; }
	}
	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của IsBossName.
	/// </summary>
	public static bool IsBossName(string name)
	{
		return ModBossNotice.IsBossName(name);
	}
	/// <summary>
	/// Thực thi phương thức nghiệp vụ AddBossNotice.
	/// </summary>
	public static void AddBossNotice(string bossName, string mapName, string timeStr)
	{
		ModBossNotice.AddBossNotice(bossName, mapName, timeStr);
	}
	/// <summary>
	/// Xử lý nghiệp vụ logic chuyên trách cho ProcessServerBossNotice.
	/// </summary>
	public static void ProcessServerBossNotice(string raw)
	{
		ModBossNotice.ProcessServerBossNotice(raw);
	}

	// Next Map
	public static bool isNextMapActive
	{
		get { return ModNextMap.isNextMapActive; }
		set { ModNextMap.isNextMapActive = value; }
	}
	public static int nextMapTargetId
	{
		get { return ModNextMap.nextMapTargetId; }
		set { ModNextMap.nextMapTargetId = value; }
	}
	/// <summary>
	/// Truy xuất và trả về dữ liệu của GetMapName.
	/// </summary>
	public static string GetMapName(int id)
	{
		return ModNextMap.GetMapName(id);
	}
	/// <summary>
	/// Thực thi phương thức nghiệp vụ StartNextMap.
	/// </summary>
	public static void StartNextMap(int targetId)
	{
		ModNextMap.StartNextMap(targetId);
	}
	/// <summary>
	/// Thực thi phương thức nghiệp vụ StopNextMap.
	/// </summary>
	public static void StopNextMap()
	{
		ModNextMap.StopNextMap();
	}

	// UI
	public static bool uiCustomOpen
	{
		get { return ModUI.uiCustomOpen; }
		set { ModUI.uiCustomOpen = value; }
	}
	public static bool uiTanSatOpen
	{
		get { return ModUI.uiTanSatOpen; }
		set { ModUI.uiTanSatOpen = value; }
	}
	public static int selectedTab
	{
		get { return ModUI.selectedTab; }
		set { ModUI.selectedTab = value; }
	}
	public static int tanSatTab
	{
		get { return ModUI.tanSatTab; }
		set { ModUI.tanSatTab = value; }
	}

	// Config
	public static void SaveConfig()
	{
		ModConfig.SaveConfig();
	}
	/// <summary>
	/// Đọc và áp dụng toàn bộ giá trị cài đặt cấu hình từ tệp mod_config.ini.
	/// </summary>
	public static void LoadConfig()
	{
		ModConfig.LoadConfig();
	}

	#endregion

	#region Core State & Lifecycle

	private static bool inited = false;
	private static bool _wasInGameLastTick = false;
	private static int _lastUiInputFrame = -1;
	private static long mapChangeWatchdogTime = 0;
	public static bool modMenuOpen = false;
	private static Command[] cmds;
	private static readonly ModAction act = new ModAction();

	/// <summary>
	/// CLASS ModAction: Định nghĩa và điều phối logic trong hệ thống game.
	/// </summary>
	private class ModAction : IActionListener
	{
		/// <summary>
		/// Thực thi hành động tương tác khi người dùng kích hoạt lệnh hoặc nút bấm.
		/// </summary>
		public void perform(int idAction, object p)
		{
			ModMenu.OnAction(idAction);
		}
	}

	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của IsInGame.
	/// </summary>
	public static bool IsInGame()
	{
		try
		{
			if (GameCanvas.currentScreen == null || GameScr.instance == null)
			{
				return false;
			}
			if (GameCanvas.currentScreen != GameScr.instance &&
			    !(Assets.src.g.ClientInput.instance != null && GameCanvas.currentScreen == Assets.src.g.ClientInput.instance))
			{
				return false;
			}
			if (Char.myCharz() == null)
			{
				return false;
			}
			return Session_ME.gI().isConnected();
		}
		catch
		{
			return false;
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ OpenMenu.
	/// </summary>
	public static void OpenMenu()
	{
		try
		{
			ModUI.uiCustomOpen = true;
			modMenuOpen = true;
			SoundMn.gI().buttonClick();
		}
		catch
		{
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ CloseMenu.
	/// </summary>
	public static void CloseMenu()
	{
		try
		{
			ModUI.uiCustomOpen = false;
			modMenuOpen = false;
			ModConfig.SaveConfig();
			SoundMn.gI().buttonClose();
		}
		catch
		{
		}
	}

	/// <summary>
	/// Bộ xử lý sự kiện tiếp nhận và phản hồi OnAction.
	/// </summary>
	public static void OnAction(int id)
	{
		try
		{
			if (id >= 1 && id <= 7)
			{
				ModUI.selectedTab = id - 1;
				ModUI.uiCustomOpen = true;
				SoundMn.gI().buttonClick();
			}
		}
		catch
		{
		}
	}

	/// <summary>
	/// Cập nhật trạng thái logic của đối tượng theo từng chu kỳ khung hình.
	/// </summary>
	public static void Update()
	{
		try
		{
			GameTabManager.UpdateCommands();
			if (!inited)
			{
				inited = true;
				ModConfig.LoadConfig();
				ModFps.LoadFPS();
			}

			GameTabStatus.UpdateCurrentTab();
			ModGraphics.UpdateResolutionWatcher();
			ModBackground.Update();

			if (Application.IsActiveTab)
			{
				if (ModFps.isAutoFps)
				{
					int best = ModFps.GetBestFpsForDevice();
					if (best != ModFps.targetFps)
					{
						ModFps.targetFps = best;
						ModFps.ApplyFPS();
					}
				}

				ModHotkey.UpdateHotkeys();
				int curFrame = Time.frameCount;
				if ( curFrame <= 0 || curFrame != _lastUiInputFrame)
				{
					_lastUiInputFrame = curFrame;
					if (!ModGraphics.CheckWakeFromBlackScreenTap())
					{
						ModUI.HandleTap();
						if (GameCanvas.isPointerJustRelease && ModLogo.IsPointerInsideLogo(GameCanvas.px, GameCanvas.py))
						{
							OpenMenu();
						}
					}
				}
				GameTabStatus.RefreshAllTabs();
			}

			// Tự động kết nối lại & đăng nhập lại khi mất kết nối mạng
			ModAutoLogin.Update();
			ModAutoChat.Update();
			ModAutoBoxSort.Update();
			ModAutoPoint.Update();
			ModAutoItem.Update();
			ModSchedule.Update();
			ModAutoRevive.Update();
			ModAutoPet.Update();
			ModAutoFusion.Update();
			ModAutoNewAccount.Update();
			ModInventoryExt.Update();
			ModAutoCaptcha.Update();
			ModSelfKill.Update();

			if (!IsInGame())
			{
				if (_wasInGameLastTick)
				{
					_wasInGameLastTick = false;
					ModAccountManager.NotifyLeftGame();
				}
				GameScr.isAutoPlay = false;
				GameScr.canAutoPlay = false;
				ModUI.uiCustomOpen = false;
				modMenuOpen = false;
				ModTeleport.ResetSessionColdCounters();
				ModGoBack.OnOutOfGame();
				return;
			}
			_wasInGameLastTick = true;

			// Ghi nhận trạng thái Auto khi đang trong game ổn định
			ModAutoLogin.SnapshotAutoState();

			// Keep-Alive Heartbeat: Giữ kết nối socket liên tục
			long now = mSystem.currentTimeMillis();
			if (now - Session_ME.lastSendTime > 15000)
			{
				Session_ME.lastSendTime = now;
				Service.gI().clientOk();
			}

			Char me = Char.myCharz();
			if (me == null)
			{
				return;
			}

			// Universal Map-Change Watchdog: Tự động chống kẹt map / kẹt khóa phím khi chuyển map
			if (Char.ischangingMap || (Char.isLockKey && !GameCanvas.panel.isShow && GameCanvas.currentScreen == GameScr.gI()) || Controller.isStopReadMessage)
			{
				if (Char.isLoadingMap)
				{
					mapChangeWatchdogTime = 0;
				}
				else
				{
					if (mapChangeWatchdogTime == 0)
					{
						mapChangeWatchdogTime = now;
					}
					else
					{
						int timeout = (Teleport.vTeleport != null && Teleport.vTeleport.size() > 0) ? 5500 : 3000;
						if (now - mapChangeWatchdogTime > timeout)
						{
							Char.ischangingMap = false;
							Char.isLockKey = false;
							me.isLockAttack = false;
							me.isLockMove = false;
							me.isTeleport = false;
							Controller.isStopReadMessage = false;
							GameScr.lockTick = 0;
							if (Teleport.vTeleport != null)
							{
								Teleport.vTeleport.removeAllElements();
							}
							InfoDlg.hide();
							GameCanvas.endDlg();
							GameCanvas.clearKeyHold();
							GameCanvas.clearKeyPressed();
							mapChangeWatchdogTime = 0;

						// Đặt cờ entranceWaypoint nếu nhân vật vẫn đứng trong cổng để không bị re-trigger lặp lại (chỉ áp dụng cho di chuyển thủ công, không can thiệp Next Map / GoBack)
						if (!ModNextMap.isNextMapActive && !ModGoBack.isReturning && TileMap.vGo != null)
						{
							for (int w = 0; w < TileMap.vGo.size(); w++)
							{
								Waypoint wp = (Waypoint)TileMap.vGo.elementAt(w);
								if (wp != null && !wp.isEnter && me.cx >= wp.minX && me.cx <= wp.maxX && me.cy >= wp.minY && me.cy <= wp.maxY)
								{
									Char.entranceWaypoint = wp;
									break;
								}
							}
						}
					}
				}
			}
		}
		else
		{
			mapChangeWatchdogTime = 0;
		}

			if (me.charID != ModSpeed.lastCharId)
			{
				ModSpeed.lastCharId = me.charID;
				ModSpeed.originalSpeed = -1;
			}

			// Xử lý tốc chạy
			if (ModSpeed.speedHack)
			{
				if (ModSpeed.originalSpeed == -1)
				{
					ModSpeed.originalSpeed = me.cspeed;
				}
				me.cspeed = (int)(ModSpeed.originalSpeed * ModSpeed.speedMult);
			}
			else if (ModSpeed.originalSpeed != -1)
			{
				me.cspeed = ModSpeed.originalSpeed;
				ModSpeed.originalSpeed = -1;
			}

			// Tự động bơm đậu & khóa HP/MP
			ModAutoHeal.DoRealAutoHeal();

			// Cập nhật khóa mục tiêu & thống kê vàng treo máy
			ModMapEntityHUD.UpdateLockTarget();
			ModTanSat.UpdateGoldTrain(me, now);

			// Tự động chuyển map
			ModNextMap.UpdateNextMap();

			// Tự động về chỗ cũ khi chết (GoBack)
			ModGoBack.Update();

			// Tự động úp set kích hoạt & bán đồ rác / cất rương
			ModSetActivator.Update();

			// Tự động né Broly & khinh công
			ModKiteBroly.Update();

			// Tự động dò khu Boss & săn Boss
			ModBossNotice.UpdateAutoBoss();

			// Tối ưu hóa tỉ lệ rơi đồ
			ModDropRate.Update();

			// Tự động mua bùa Bà Hạt Mít
			ModAutoBuyBua.Update();

			// Tự động nhặt đồ & vứt đồ
			ModAutoPick.RunRealAutoPick();

			// Tự động Tàn Sát (hoặc đóng băng hồi chiêu / buff hỗ trợ khi đánh tay)
			if (!ModTanSat.autoTanSat && (ModTanSat.freezeSkillCooldown || ModTanSat.frozenSkillIds.Count > 0 || ModTanSat.autoSupportSkills) && !me.isDie && me.cHP > 0)
			{
				ModTanSat.UpdateSupportAndFreezeSkills(me, now);
			}
			ModAutoSkill.Update();
			ModTanSat.RunTanSat();
		}
		catch
		{
		}
	}

	/// <summary>
	/// Thực hiện vẽ toàn bộ đối tượng và các lớp hiển thị lên canvas mGraphics.
	/// </summary>
	public static void Paint(mGraphics g)
	{
		if (g == null)
		{
			return;
		}
		try
		{
			// 1. Menu HUB nổi đa nhiệm trên màn hình (Bong bóng / Panel chuyển 6 Tab - hiển thị trên tất cả màn hình)
			ModFloatingHUD.Paint(g);

			// 2. Giao diện Modal Cài Đặt (khi mở menu mod)
			ModUI.PaintTanSatUI(g);

			// 3. Hiển thị FPS & Ping
			ModFps.PaintFPS(g);

			// 4. Hiển thị Logo TriHienKun
			ModLogo.Paint(g);

			// Các HUD dưới đây chỉ hiển thị khi đã vào game thế giới
			if (IsInGame())
			{
				// Vòng bán kính tự nhặt (vẽ trên mặt đất)
				ModAutoPick.PaintPickRadius(g);

				// Nút mũi tên mở Mod Menu góc phải
				ModArrowButton.Paint(g);

				// HUD Thông báo Boss
				ModBossNotice.PaintBossNotice(g);

				// HUD Radar thực thể Boss & Player trong map
				ModMapEntityHUD.Paint(g);

				// HUD Thông số Đệ Tử
				ModAutoPet.PaintPetHUD(g);

				// HUD Thống kê Vàng, SKH & Bùa
				ModTanSat.PaintGoldTrainHUD(g);
				ModSetActivator.PaintSkhHUD(g);
				ModAutoBuyBua.PaintCharmHUD(g);

				// HUD Tên Map & Khu vực
				ModNextMap.PaintHUDMapTag(g);
			}
		}
		catch
		{
		}
	}

	/// <summary>
	/// Thực hiện vẽ và hiển thị thành phần PaintFPS lên canvas đồ họa.
	/// </summary>
	public static void PaintFPS(mGraphics g)
	{
		ModFps.PaintFPS(g);
	}

	/// <summary>
	/// Thực hiện vẽ và hiển thị thành phần PaintBossNotice lên canvas đồ họa.
	/// </summary>
	public static void PaintBossNotice(mGraphics g)
	{
		ModBossNotice.PaintBossNotice(g);
	}

	/// <summary>
	/// Thực hiện vẽ và hiển thị thành phần PaintTanSatUI lên canvas đồ họa.
	/// </summary>
	public static void PaintTanSatUI(mGraphics g)
	{
		ModUI.PaintTanSatUI(g);
	}

	#endregion
}

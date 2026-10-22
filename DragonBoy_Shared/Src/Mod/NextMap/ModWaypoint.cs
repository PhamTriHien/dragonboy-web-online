using System;

/// <summary>
/// Module Tọa Độ Cổng Chuyển Map: Quản lý danh sách các Waypoint kết nối giữa các bản đồ trong thế giới game.
/// </summary>
public static class ModWaypoint
{
	public static int edgeWalkAttempts = 0;
	public static bool isWaitingShipMenu;
	public static int pendingTargetPlanetMapId = -1;
	public static long lastShipOpenMenuTime;
	private static Waypoint activeWp;
	private static long wpInitTime;
	private static bool hasTeleportedToWp;
	private static long lastTeleportTime;
	private static long lastWalkTime;

	/// <summary>
	/// Đặt lại trạng thái theo dõi Waypoint khi đổi map hoặc khởi động lại lộ trình.
	/// </summary>
	public static void ResetWaypointState()
	{
		activeWp = null;
		wpInitTime = 0;
		hasTeleportedToWp = false;
		lastTeleportTime = 0;
		lastWalkTime = 0;
		edgeWalkAttempts = 0;
		isWaitingShipMenu = false;
		pendingTargetPlanetMapId = -1;
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của GetGroundY.
	/// </summary>
	public static int GetGroundY(int x, int minY, int maxY)
	{
		try
		{
			if (TileMap.isInAirMap() || TileMap.mapID == 47 || TileMap.mapID == 45 || TileMap.mapID == 46 || TileMap.mapID == 48)
			{
				return (minY + maxY) / 2;
			}

			int startY = System.Math.Max(24, minY);
			int endY = System.Math.Min(TileMap.pxh - 12, maxY);
			for (int y = startY; y <= endY; y += 12)
			{
				if (TileMap.tileTypeAt(x, y, 2))
				{
					int gy = TileMap.tileYofPixel(y);
					if (gy >= minY && gy <= maxY)
					{
						return gy;
					}
					return (gy <= maxY) ? gy : maxY;
				}
			}

			for (int y = 50; y < TileMap.pxh - 24; y += 24)
			{
				if (TileMap.tileTypeAt(x, y, 2))
				{
					return TileMap.tileYofPixel(y);
				}
			}

			return maxY;
		}
		catch
		{
			return maxY;
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ TeleportTo.
	/// </summary>
	public static void TeleportTo(int x, int y)
	{
		try
		{
			Char me = Char.myCharz();
			if (me == null)
			{
				return;
			}
			me.vMovePoints.removeAllElements();
			me.currentMovePoint = null;
			me.endMovePointCommand = null;
			me.cvx = 0;
			me.cvy = 0;
			me.statusMe = 1;
			me.delayFall = 0;
			me.cyStartFall = 0;
			me.isFlyUp = false;

			me.cx = x;
			me.cy = y;
			Service.gI().charMove();
			if (!ItemTime.isExistItem(4387))
			{
				me.cx = x;
				me.cy = y + 1;
				Service.gI().charMove();
				me.cx = x;
				me.cy = y;
				Service.gI().charMove();
			}
		}
		catch
		{
		}
	}

	/// <summary>
	/// Dịch chuyển theo từng nhịp mặt đất an toàn đến tọa độ đích khi bật chế độ an toàn hoặc sau khi bị server reset point.
	/// </summary>
	public static bool MoveSteppedTo(int targetX, int targetY)
	{
		Char me = Char.myCharz();
		if (me == null)
		{
			return false;
		}

		int dx = Res.abs(me.cx - targetX);
		int dy = Res.abs(me.cy - targetY);
		int maxStepX = 160;

		if ((ModNextMap.useSafeMovement || ModNextMap.resetPenaltyCount > 0) && dx > maxStepX)
		{
			int nextX = me.cx + ((targetX > me.cx) ? maxStepX : (-maxStepX));
			int nextY;
			if (TileMap.isInAirMap() || TileMap.mapID == 45 || TileMap.mapID == 46 || TileMap.mapID == 47 || TileMap.mapID == 48)
			{
				nextY = targetY;
			}
			else
			{
				int minSearchY = System.Math.Max(24, System.Math.Min(me.cy, targetY) - 96);
				int maxSearchY = System.Math.Min(TileMap.pxh - 12, System.Math.Max(me.cy, targetY) + 96);
				nextY = GetGroundY(nextX, minSearchY, maxSearchY);
			}
			me.cdir = (targetX >= me.cx) ? 1 : (-1);
			TeleportTo(nextX, nextY);
			return false;
		}

		if (dx > 20 || dy > 20)
		{
			me.cdir = (targetX >= me.cx) ? 1 : (-1);
			TeleportTo(targetX, targetY);
			return false;
		}

		if (me.cx != targetX || me.cy != targetY || me.cxSend != targetX || me.cySend != targetY)
		{
			TeleportTo(targetX, targetY);
		}
		return true;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ StepToWaypoint.
	/// </summary>
	public static bool StepToWaypoint(Waypoint wp)
	{
		try
		{
			Char me = Char.myCharz();
			if (me == null || wp == null)
			{
				return false;
			}

			if (GameCanvas.menu != null && GameCanvas.menu.showMenu)
			{
				GameCanvas.menu.showMenu = false;
			}

			if (Char.entranceWaypoint == wp)
			{
				Char.entranceWaypoint = null;
			}

			long now = mSystem.currentTimeMillis();
			if (activeWp != wp)
			{
				activeWp = wp;
				wpInitTime = now;
				hasTeleportedToWp = false;
				lastTeleportTime = 0;
				lastWalkTime = 0;
				edgeWalkAttempts = 0;
			}

			if (hasTeleportedToWp && now - lastTeleportTime < 120)
			{
				return false;
			}

			bool isLeftEdge = wp.maxX < 60;
			bool isRightEdge = !isLeftEdge && wp.minX > TileMap.pxw - 60;
			bool isInterior = !isLeftEdge && !isRightEdge;

			int wpTargetX;
			if (isLeftEdge)
			{
				wpTargetX = 15;
			}
			else if (isInterior)
			{
				wpTargetX = (wp.minX + wp.maxX) / 2;
			}
			else
			{
				wpTargetX = TileMap.pxw - 15;
			}

			bool isAirOrTower = TileMap.isInAirMap() || TileMap.mapID == 45 || TileMap.mapID == 46 || TileMap.mapID == 47 || TileMap.mapID == 48;
			int wpTargetY;
			if (isAirOrTower && wp.maxY > wp.minY + 24)
			{
				wpTargetY = (wp.minY + wp.maxY) / 2;
			}
			else if (!isAirOrTower)
			{
				wpTargetY = GetGroundY(wpTargetX, wp.minY, wp.maxY);
			}
			else
			{
				wpTargetY = wp.maxY;
			}

			int dx = Res.abs(me.cx - wpTargetX);
			int dy = Res.abs(me.cy - wpTargetY);
			int teleportX = (isInterior || isAirOrTower) ? wpTargetX : (isLeftEdge ? (wpTargetX + 20) : (wpTargetX - 20));
			me.cdir = isLeftEdge ? (-1) : (isRightEdge ? 1 : ((me.cx <= wpTargetX) ? 1 : (-1)));

			if (!hasTeleportedToWp || dx > 30 || dy > 30)
			{
				if (ModNextMap.useSafeMovement || ModNextMap.resetPenaltyCount > 0)
				{
					if (MoveSteppedTo(teleportX, wpTargetY))
					{
						hasTeleportedToWp = true;
					}
				}
				else
				{
					TeleportTo(teleportX, wpTargetY);
					hasTeleportedToWp = true;
				}
				lastTeleportTime = now;
				wpInitTime = now;
				return false;
			}

			if (!isInterior && !isAirOrTower)
			{
				if (edgeWalkAttempts >= 2 || (edgeWalkAttempts > 0 && now - lastWalkTime >= 350))
				{
					TeleportTo(wpTargetX, wpTargetY);
					if (wp.isOffline || TileMap.isOfflineMap() || TileMap.isTrainingMap())
					{
						Char.ischangingMap = true;
						Service.gI().getMapOffline();
					}
					else
					{
						Char.ischangingMap = true;
						Service.gI().requestChangeMap();
					}
					ResetWaypointState();
					return true;
				}

				if (me.currentMovePoint == null && now - lastWalkTime >= 180)
				{
					int walkTargetX = isLeftEdge ? 5 : (TileMap.pxw - 5);
					me.currentMovePoint = new MovePoint(walkTargetX, wpTargetY);
					edgeWalkAttempts++;
					lastWalkTime = now;
				}
				return false;
			}

			if (me.currentMovePoint == null)
			{
				if (me.cx != wpTargetX || me.cy != wpTargetY || me.cxSend != wpTargetX || me.cySend != wpTargetY)
				{
					TeleportTo(wpTargetX, wpTargetY);
				}
				if (wp.isOffline || TileMap.isOfflineMap() || TileMap.isTrainingMap())
				{
					Char.ischangingMap = true;
					Service.gI().getMapOffline();
				}
				else
				{
					Char.ischangingMap = true;
					Service.gI().requestChangeMap();
				}
				ResetWaypointState();
				return true;
			}

			return false;
		}
		catch
		{
			return false;
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ UseSpaceShip.
	/// </summary>
	public static bool UseSpaceShip(Npc shipNpc, int targetPlanetMapId = -1)
	{
		try
		{
			Char me = Char.myCharz();
			if (me == null || shipNpc == null)
			{
				return false;
			}

			if (!MoveSteppedTo(shipNpc.cx, shipNpc.cy))
			{
				return false;
			}

			me.focusManualTo(shipNpc);
			int npcTemplateId = (shipNpc.template != null) ? shipNpc.template.npcTemplateId : shipNpc.npcId;

			long now = mSystem.currentTimeMillis();
			if (isWaitingShipMenu && now - lastShipOpenMenuTime < 2500)
			{
				return true;
			}

			isWaitingShipMenu = true;
			pendingTargetPlanetMapId = targetPlanetMapId;
			lastShipOpenMenuTime = now;

			Service.gI().openMenu(npcTemplateId);
			return true;
		}
		catch
		{
			return false;
		}
	}

	/// <summary>
	/// Bộ xử lý sự kiện tiếp nhận và phản hồi OnReceiveShipMenu.
	/// </summary>
	public static void OnReceiveShipMenu(string[] menuItems, Npc npc)
	{
		try
		{
			if (!isWaitingShipMenu || menuItems == null || menuItems.Length == 0)
			{
				return;
			}

			if (mSystem.currentTimeMillis() - lastShipOpenMenuTime > 5000)
			{
				isWaitingShipMenu = false;
				pendingTargetPlanetMapId = -1;
				return;
			}

			isWaitingShipMenu = false;
			int targetPlanet = pendingTargetPlanetMapId;
			int selectedIndex = -1;

			for (int i = 0; i < menuItems.Length; i++)
			{
				string text = menuItems[i].ToLower();
				if (targetPlanet == 25 && (text.Contains("nam") || text.Contains("namec")))
				{
					selectedIndex = i;
					break;
				}
				if (targetPlanet == 26 && (text.Contains("xay") || text.Contains("say") || text.Contains("sai")))
				{
					selectedIndex = i;
					break;
				}
				if (targetPlanet == 24 && (text.Contains("trái") || text.Contains("trai") || text.Contains("earth") || text.Contains("đất")))
				{
					selectedIndex = i;
					break;
				}
			}

			if (selectedIndex == -1)
			{
				if (TileMap.mapID == 24)
				{
					selectedIndex = (targetPlanet == 26) ? 1 : 0;
				}
				else if (TileMap.mapID == 25)
				{
					selectedIndex = (targetPlanet == 26) ? 1 : 0;
				}
				else if (TileMap.mapID == 26)
				{
					selectedIndex = (targetPlanet == 25) ? 1 : 0;
				}
				else
				{
					selectedIndex = 0;
				}
			}

			if (selectedIndex >= menuItems.Length)
			{
				selectedIndex = 0;
			}

			int npcTemplateId = (npc != null && npc.template != null) ? npc.template.npcTemplateId : ((npc != null) ? npc.npcId : 10);
			Service.gI().confirmMenu((short)npcTemplateId, (sbyte)selectedIndex);

			GameCanvas.menu.showMenu = false;
		}
		catch
		{
			isWaitingShipMenu = false;
		}
	}
}

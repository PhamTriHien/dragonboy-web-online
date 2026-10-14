using System;

/// <summary>
/// CLASS ModTeleport: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
public static class ModTeleport
{
	/// <summary>
	/// Thực thi phương thức nghiệp vụ TeleportTo.
	/// </summary>
	public static void TeleportTo(int targetX, int targetY)
	{
		try
		{
			Char me = Char.myCharz();
			if (me == null)
			{
				return;
			}

			// Neu nhan vat da o rat gan diem den (<= 5px), khong can dich chuyen lai tranh spam packet
			if (Res.abs(me.cx - targetX) <= 5 && Res.abs(me.cy - targetY) <= 5)
			{
				return;
			}

			// Xoa cac diem di chuyen trung gian
			me.vMovePoints.removeAllElements();
			me.currentMovePoint = null;
			me.endMovePointCommand = null;
			me.cvx = 0;
			me.cvy = 0;
			me.delayFall = 0;
			me.cyStartFall = 0;
			me.isFlyUp = false;

			me.cx = targetX;
			me.cy = targetY;
			Service.gI().charMove();
			if (!ItemTime.isExistItem(4387))
			{
				me.cx = targetX;
				me.cy = targetY + 1;
				Service.gI().charMove();
				me.cx = targetX;
				me.cy = targetY;
				Service.gI().charMove();
			}

			// Tao hieu ung dich chuyen visual
			ServerEffect.addServerEffect(1, targetX, targetY, 1);
		}
		catch
		{
		}
	}
}

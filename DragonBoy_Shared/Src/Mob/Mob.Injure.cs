using System;
using Assets.src.g;

/// <summary>
/// Thành phần phân mảnh (partial) của lớp Mob: Đảm nhiệm phân hệ chuyên trách Injure ().
/// </summary>
public partial class Mob : IMapObject
{
	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của isBigBoss.
	/// </summary>
	public bool isBigBoss()
		{
			return this is BachTuoc || this is BigBoss2 || this is BigBoss || this is NewBoss;
		}

	/// <summary>
	/// Thiết lập giá trị hoặc trạng thái cho setInjure.
	/// </summary>
	public void setInjure()
		{
			if (hp > 0 && status != 3 && status != 7)
			{
				timeStatus = 4;
				status = 7;
			}
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ startDie.
	/// </summary>
	public void startDie()
		{
			hp = 0L;
			injureThenDie = true;
			hp = 0L;
			status = 1;
			Res.outz("MOB DIEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEe");
			p1 = -3;
			p2 = -dir;
			p3 = 0;
			ModDropRate.OnMobDied(this);
		}

}

using System;
using Assets.src.g;

/// <summary>
/// Thành phần phân mảnh (partial) của lớp Mob: Đảm nhiệm kết xuất đồ họa, vẽ giao diện, hiệu ứng và nhân vật ().
/// </summary>
public partial class Mob : IMapObject
{
	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của isPaint.
	/// </summary>
	public bool isPaint()
		{
			if (x < GameScr.cmx)
			{
				return false;
			}
			if (x > GameScr.cmx + GameScr.gW)
			{
				return false;
			}
			if (y < GameScr.cmy)
			{
				return false;
			}
			if (y > GameScr.cmy + GameScr.gH + 30)
			{
				return false;
			}
			if (arrMobTemplate[templateId] == null)
			{
				return false;
			}
			if (arrMobTemplate[templateId].data == null)
			{
				return false;
			}
			if (arrMobTemplate[templateId].data.img == null)
			{
				return false;
			}
			if (status == 0)
			{
				return false;
			}
			return true;
		}

	/// <summary>
	/// Cập nhật trạng thái logic của updateHp_bar theo chu kỳ khung hình.
	/// </summary>
	public void updateHp_bar()
		{
			len = (int)(hp * 100 / maxHp * w_hp_bar) / 100;
			per = (int)(hp * 100 / maxHp);
			if (per == 100)
			{
				per_tem = per;
			}
			if (per >= 100)
			{
				per_tem = per;
			}
			offset = 0;
			if (per < 30)
			{
				color = 15473700;
				imgHPtem = GameScr.imgHP_tm_do;
			}
			else if (per < 60)
			{
				color = 16744448;
				imgHPtem = GameScr.imgHP_tm_vang;
			}
			else
			{
				color = 11992374;
				imgHPtem = GameScr.imgHP_tm_xanh;
			}
		}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của getHPColor.
	/// </summary>
	public int getHPColor()
		{
			return 16711680;
		}

}

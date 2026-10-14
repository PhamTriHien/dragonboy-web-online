using System;
using Assets.src.e;
using Assets.src.g;
using UnityEngine;

/// <summary>
/// Thành phần phân mảnh (partial) của lớp Char: Đảm nhiệm phân hệ chuyên trách Skills ().
/// </summary>
public partial class Char : IMapObject
{
	/// <summary>
	/// Áp dụng các thay đổi cấu hình hoặc hiệu ứng của applyCharLevelPercent.
	/// </summary>
	public void applyCharLevelPercent()
			{
				try
				{
					long num = 1L;
					long num2 = 0L;
					int num3 = 0;
					for (int num4 = GameScr.exps.Length - 1; num4 >= 0; num4--)
					{
						if (cPower >= GameScr.exps[num4])
						{
							num = ((num4 != GameScr.exps.Length - 1) ? (GameScr.exps[num4 + 1] - GameScr.exps[num4]) : 1);
							num2 = cPower - GameScr.exps[num4];
							num3 = num4;
							break;
						}
					}
					clevel = num3;
					cLevelPercent = (int)(num2 * 10000 / num);
				}
				catch (Exception ex)
				{
					Cout.LogError("Loi char level percent: " + ex.ToString());
				}
			}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của getStrLevel.
	/// </summary>
	public string getStrLevel()
			{
				string text = strLevel[clevel] + "+" + cLevelPercent / 100 + "." + cLevelPercent % 100 + "%";
				if (text.Length > 23 && text.IndexOf("cấp ") >= 0)
				{
					text = Res.replace(text, "cấp ", "c");
				}
				return text;
			}

	/// <summary>
	/// Thiết lập giá trị hoặc trạng thái cho setPowerInfo.
	/// </summary>
	public void setPowerInfo(string info, short p, short maxP, short sc)
			{
				powerPoint = p;
				strInfo = info;
				maxPowerPoint = maxP;
				secondPower = sc;
				lastS = (currS = mSystem.currentTimeMillis());
			}

}

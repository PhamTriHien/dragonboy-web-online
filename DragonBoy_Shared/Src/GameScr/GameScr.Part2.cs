using System;
using Assets.src.g;
/// <summary>
/// Thành phần phân mảnh (partial) của lớp GameScr: Đảm nhiệm phân hệ chuyên trách Part2 ().
/// </summary>
public partial class GameScr : mScreen, IChatable
{
	/// <summary>
	/// Thực thi phương thức nghiệp vụ testAct.
	/// </summary>
	public bool testAct()
		{
			for (sbyte b = 2; b < 9; b += 2)
			{
				if (GameCanvas.keyHold[b])
				{
					return false;
				}
			}
			return true;
		}
	/// <summary>
	/// Thực thi phương thức nghiệp vụ clanInvite.
	/// </summary>
	public void clanInvite(string strInvite, int clanID, int code)
		{
			ClanObject clanObject = new ClanObject();
			clanObject.code = code;
			clanObject.clanID = clanID;
			startYesNoPopUp(strInvite, new Command(mResources.YES, 12002, clanObject), new Command(mResources.NO, 12003, clanObject));
		}

}

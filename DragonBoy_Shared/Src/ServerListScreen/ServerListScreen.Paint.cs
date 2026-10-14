using System;
using UnityEngine;

/// <summary>
/// Thành phần phân mảnh (partial) của lớp ServerListScreen: Đảm nhiệm kết xuất đồ họa, vẽ giao diện, hiệu ứng và nhân vật ().
/// </summary>
public partial class ServerListScreen : mScreen, IActionListener
{
	/// <summary>
	/// Thực hiện vẽ toàn bộ đối tượng và các lớp hiển thị lên canvas mGraphics.
	/// </summary>
	public override void paint(mGraphics g)
	{
		if (!loadScreen)
		{
			g.setColor(0);
			g.fillRect(0, 0, GameCanvas.w, GameCanvas.h);
		}
		else
		{
			GameCanvas.paintBGGameScr(g);
		}

		int num2 = ((GameCanvas.w < 200) ? 160 : 180);
		paintDeleteData(g);
		if (!loadScreen)
		{
			if (!bigOk)
			{
				g.drawImage(LoginScr.imgTitle, GameCanvas.hw, GameCanvas.hh - 32, 3);
				if (!isGetData)
				{
					mFont.tahoma_7b_white.drawString(g, mResources.taidulieudechoi, GameCanvas.hw, GameCanvas.hh + 20, 2);
					if (cmdDownload != null)
					{
						cmdDownload.paint(g);
					}
				}
				else
				{
					mFont.tahoma_7b_white.drawString(g, mResources.downloading_data + percent + "%", GameCanvas.w / 2, GameCanvas.hh + 20, 2);
					GameScr.paintOngMauPercent(GameScr.frBarPow20, GameScr.frBarPow21, GameScr.frBarPow22, GameCanvas.w / 2 - 50, GameCanvas.hh + 36, 100, 100f, g);
					GameScr.paintOngMauPercent(GameScr.frBarPow0, GameScr.frBarPow1, GameScr.frBarPow2, GameCanvas.w / 2 - 50, GameCanvas.hh + 36, 100, percent, g);
					if (cmdDownload != null)
					{
						cmdDownload.paint(g);
					}
				}
			}
		}
		else
		{
			int num3 = GameCanvas.hh - 15 * cmd.Length - 15;
			if (num3 < 25)
			{
				num3 = 25;
			}
			if (LoginScr.imgTitle != null)
			{
				g.drawImage(LoginScr.imgTitle, GameCanvas.hw, num3, 3);
			}
			if (isNewUI)
			{
				paint_UI_New(g);
			}
			else
			{
				int num4 = cmd.Length;
				int numY = GameCanvas.hh - 15 * cmd.Length + 28;
				for (int i = 0; i < num4; i++)
				{
					if (cmd[i] != null)
					{
						cmd[i].x = (GameCanvas.w - cmd[i].w) / 2;
						cmd[i].y = numY + i * 30;
						cmd[i].paint(g);
					}
				}
				g.setClip(0, 0, GameCanvas.w, GameCanvas.h);
				if (mGraphics.zoomLevel == 1)
				{
					if (testConnect == -1)
					{
						if (GameCanvas.gameTick % 20 > 10)
						{
							g.drawRegion(GameScr.imgRoomStat, 0, 14, 7, 7, 0, (GameCanvas.w - mFont.tahoma_7b_dark.getWidth(cmd[2 + nCmdPlay].caption) >> 1) - 10, cmd[2 + nCmdPlay].y + 10, 0);
						}
					}
					else
					{
						g.drawRegion(GameScr.imgRoomStat, 0, testConnect * 7, 7, 7, 0, (GameCanvas.w - mFont.tahoma_7b_dark.getWidth(cmd[2 + nCmdPlay].caption) >> 1) - 10, cmd[2 + nCmdPlay].y + 9, 0);
					}
				}
			}
		}
		base.paint(g);
		ModAutoUpdate.PaintLobbyUI(g);
	}

	/// <summary>
	/// Thực hiện vẽ và hiển thị thành phần paintDeleteData lên canvas đồ họa.
	/// </summary>
	public static void paintDeleteData(mGraphics g)
	{
		if (cmdDeleteRMS != null)
		{
			cmdDeleteRMS.x = GameCanvas.w - 78;
			cmdDeleteRMS.y = GameCanvas.h - 26;
			mFont.tahoma_7_white.drawString(g, mResources.xoadulieu, GameCanvas.w - 2, GameCanvas.h - 15, 1, mFont.tahoma_7_grey);
		}
	}

	/// <summary>
	/// Thực hiện vẽ và hiển thị thành phần paint_UI_New lên canvas đồ họa.
	/// </summary>
	public void paint_UI_New(mGraphics g)
	{
		if (isNewUI)
		{
			for (int i = 0; i < cmd_New_Ui.Length; i++)
			{
				cmd_New_Ui[i].paint(g);
			}
		}
	}
}

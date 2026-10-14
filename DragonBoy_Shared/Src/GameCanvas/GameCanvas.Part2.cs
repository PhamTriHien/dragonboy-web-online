using System;
using Assets.src.e;
using Assets.src.g;
using UnityEngine;
/// <summary>
/// Thành phần phân mảnh (partial) của lớp GameCanvas: Đảm nhiệm phân hệ chuyên trách Part2 ().
/// </summary>
public partial class GameCanvas : IActionListener
{
	/// <summary>
	/// Thực thi phương thức nghiệp vụ doResetToLoginScr.
	/// </summary>
	public void doResetToLoginScr(mScreen screen)
		{
			try
			{
				SoundMn.gI().stopAll();
				LoginScr.isContinueToLogin = false;
				TileMap.lastType = (TileMap.bgType = 0);
				Char.clearMyChar();
				GameScr.clearGameScr();
				GameScr.resetAllvector();
				InfoDlg.hide();
				GameScr.info1.hide();
				GameScr.info2.hide();
				GameScr.info2.cmdChat = null;
				Hint.isShow = false;
				ChatPopup.currChatPopup = null;
				Controller.isStopReadMessage = false;
				GameScr.loadCamera(fullmScreen: true, -1, -1);
				GameScr.cmx = 100;
				panel.currentTabIndex = 0;
				panel.selected = (isTouch ? (-1) : 0);
				panel.init();
				panel2 = null;
				GameScr.isPaint = true;
				ClanMessage.vMessage.removeAllElements();
				GameScr.textTime.removeAllElements();
				GameScr.vClan.removeAllElements();
				GameScr.vFriend.removeAllElements();
				GameScr.vEnemies.removeAllElements();
				TileMap.vCurrItem.removeAllElements();
				BackgroudEffect.vBgEffect.removeAllElements();
				EffecMn.vEff.removeAllElements();
				Effect.newEff.removeAllElements();
				menu.showMenu = false;
				panel.vItemCombine.removeAllElements();
				panel.isShow = false;
				if (panel.tabIcon != null)
				{
					panel.tabIcon.isShow = false;
				}
				if (mGraphics.zoomLevel == 1)
				{
					SmallImage.clearHastable();
				}
				Session_ME.gI().close();
				Session_ME2.gI().close();
			}
			catch (Exception ex)
			{
				Cout.println("Loi tai doResetToLoginScr " + ex.ToString());
			}
			ServerListScreen.isAutoConect = true;
			ServerListScreen.countDieConnect = 0;
			ServerListScreen.testConnect = -1;
			ServerListScreen.loadScreen = true;
			ServerListScreen.count_reConnect = 0;
			ServerListScreen.waitToLogin = false;
			ServerListScreen.tWaitToLogin = 0;
			ServerListScreen.isWait = false;
			LoginScr.timeLogin = 0;
			if (ServerListScreen.ipSelect == -1)
			{
				serverScr.switchToMe();
				return;
			}
			if (serverScreen == null)
			{
				serverScreen = new ServerListScreen();
			}
			serverScreen.switchToMe();
		}
	/// <summary>
	/// Thực thi phương thức nghiệp vụ fillRect.
	/// </summary>
	public static void fillRect(mGraphics g, int color, int x, int y, int w, int h, int detalY)
		{
			g.setColor(color);
			int cmy = GameScr.cmy;
			if (cmy > GameCanvas.h)
			{
				cmy = GameCanvas.h;
			}
			if (cmy < 0)
			{
				cmy = 0;
			}
			g.fillRect(x, y - ((detalY != 0) ? (cmy >> detalY) : 0), w, h + ((detalY != 0) ? (cmy >> detalY) : 0));
		}
	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của isHDVersion.
	/// </summary>
	public static bool isHDVersion()
		{
			if (mGraphics.zoomLevel > 1)
			{
				return true;
			}
			return false;
		}
	/// <summary>
	/// Truy xuất và trả về dữ liệu của getYBackground.
	/// </summary>
	public static void getYBackground(int typeBg)
		{
			try
			{
				if (imgBG != null && bgW != null && bgH != null)
				{
					for (int i = 0; i < imgBG.Length && i < bgW.Length; i++)
					{
						if (imgBG[i] != null)
						{
							bgW[i] = mGraphics.getImageWidth(imgBG[i]);
							bgH[i] = mGraphics.getImageHeight(imgBG[i]);
						}
					}
				}
				int gH = GameScr.gH23;
				switch (typeBg)
				{
				case 0:
					yb[0] = gH - bgH[0] + 70;
					yb[1] = yb[0] - bgH[1] + 20;
					yb[2] = yb[1] - bgH[2] + 30;
					yb[3] = yb[2] - bgH[3] + 50;
					break;
				case 1:
					yb[0] = gH - bgH[0] + 120;
					yb[1] = yb[0] - bgH[1] + 40;
					yb[2] = yb[1] - 90;
					yb[3] = yb[2] - 25;
					break;
				case 2:
					yb[0] = gH - bgH[0] + 150;
					yb[1] = yb[0] - bgH[1] - 60;
					yb[2] = yb[1] - bgH[2] - 40;
					yb[3] = yb[2] - bgH[3] - 10;
					yb[4] = yb[3] - bgH[4];
					break;
				case 3:
					yb[0] = gH - bgH[0] + 10;
					yb[1] = yb[0] + 80;
					yb[2] = yb[1] - bgH[2] - 10;
					break;
				case 4:
					yb[0] = gH - bgH[0] + 130;
					yb[1] = yb[0] - bgH[1];
					yb[2] = yb[1] - bgH[2] - 20;
					yb[3] = yb[1] - bgH[2] - 80;
					break;
				case 5:
					yb[0] = gH - bgH[0] + 40;
					yb[1] = yb[0] - bgH[1] + 10;
					yb[2] = yb[1] - bgH[2] + 15;
					yb[3] = yb[2] - bgH[3] + 50;
					break;
				case 6:
					yb[0] = gH - bgH[0] + 100;
					yb[1] = yb[0] - bgH[1] - 30;
					yb[2] = yb[1] - bgH[2] + 10;
					yb[3] = yb[2] - bgH[3] + 15;
					yb[4] = yb[3] - bgH[4] + 15;
					break;
				case 7:
					yb[0] = gH - bgH[0] + 20;
					yb[1] = yb[0] - bgH[1] + 15;
					yb[2] = yb[1] - bgH[2] + 20;
					yb[3] = yb[1] - bgH[2] - 10;
					break;
				case 8:
					yb[0] = gH - 103 + 150;
					if (TileMap.mapID == 103)
					{
						yb[0] -= 100;
					}
					yb[1] = yb[0] - bgH[1] - 10;
					yb[2] = yb[1] - bgH[2] + 40;
					yb[3] = yb[2] - bgH[3] + 10;
					break;
				case 9:
					yb[0] = gH - bgH[0] + 100;
					yb[1] = yb[0] - bgH[1] + 22;
					yb[2] = yb[1] - bgH[2] + 50;
					yb[3] = yb[2] - bgH[3];
					break;
				case 10:
					yb[0] = gH - bgH[0] - 45;
					yb[1] = yb[0] - bgH[1] - 10;
					break;
				case 11:
					yb[0] = gH - bgH[0] + 60;
					yb[1] = yb[0] - bgH[1] + 5;
					yb[2] = yb[1] - bgH[2] - 15;
					break;
				case 12:
					yb[0] = gH + 40;
					yb[1] = yb[0] - 40;
					yb[2] = yb[1] - 40;
					break;
				case 13:
					yb[0] = gH - 80;
					yb[1] = yb[0];
					break;
				case 15:
					yb[0] = gH - 20;
					yb[1] = yb[0] - 80;
					break;
				case 16:
					yb[0] = gH - bgH[0] + 75;
					yb[1] = yb[0] - bgH[1] + 50;
					yb[2] = yb[1] - bgH[2] + 50;
					yb[3] = yb[2] - bgH[3] + 90;
					break;
				case 19:
					yb[0] = gH - bgH[0] + 150;
					yb[1] = yb[0] - bgH[1] - 60;
					yb[2] = yb[1] - bgH[2] - 40;
					yb[3] = yb[2] - bgH[3] - 10;
					yb[4] = yb[3] - bgH[4];
					break;
				default:
					yb[0] = gH - bgH[0] + 75;
					yb[1] = yb[0] - bgH[1] + 50;
					yb[2] = yb[1] - bgH[2] + 50;
					yb[3] = yb[2] - bgH[3] + 90;
					break;
				}
			}
			catch (Exception)
			{
				int gH2 = GameScr.gH23;
				for (int i = 0; i < yb.Length; i++)
				{
					yb[i] = 1;
				}
			}
		}
	/// <summary>
	/// Thực thi phương thức nghiệp vụ randomRaintEff.
	/// </summary>
	private static void randomRaintEff(int typeBG)
		{
			for (int i = 0; i < bgRain.Length; i++)
			{
				if (typeBG == bgRain[i] && Res.random(0, 2) == 0)
				{
					BackgroudEffect.addEffect(0);
					break;
				}
			}
		}
	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của isPointSelect.
	/// </summary>
	public static bool isPointSelect(int x, int y, int w, int h)
		{
			if (!isPointerSelect)
			{
				return false;
			}
			if (px >= x && px <= x + w && py >= y && py <= y + h)
			{
				return true;
			}
			return false;
		}
	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của checkBackButton.
	/// </summary>
	public static void checkBackButton()
		{
			if (ChatPopup.serverChatPopUp == null && ChatPopup.currChatPopup == null)
			{
				startYesNoDlg(mResources.DOYOUWANTEXIT, new Command(mResources.YES, instance, 8885, null), new Command(mResources.NO, instance, 8882, null));
			}
		}
	/// <summary>
	/// Thực thi phương thức nghiệp vụ startOKDlg.
	/// </summary>
	public static void startOKDlg(string info)
		{
			DragonBoy_Net8_Native.Src.Mod.Security.ModCredentialSecurity.OnLoginFinished();
			info = Res.changeString(info);
			closeKeyBoard();
			msgdlg.setInfo(info, null, new Command(mResources.OK, instance, 8882, null), null);
			currentDialog = msgdlg;
		}
	/// <summary>
	/// Thực thi phương thức nghiệp vụ startWaitDlg.
	/// </summary>
	public static void startWaitDlg(string info)
		{
			info = Res.changeString(info);
			closeKeyBoard();
			msgdlg.setInfo(info, null, new Command(mResources.CANCEL, instance, 8882, null), null);
			currentDialog = msgdlg;
			msgdlg.isWait = true;
		}
	/// <summary>
	/// Thực thi phương thức nghiệp vụ startOKDlg.
	/// </summary>
	public static void startOKDlg(string info, bool isError)
		{
			DragonBoy_Net8_Native.Src.Mod.Security.ModCredentialSecurity.OnLoginFinished();
			info = Res.changeString(info);
			closeKeyBoard();
			msgdlg.setInfo(info, null, new Command(mResources.CANCEL, instance, 8882, null), null);
			currentDialog = msgdlg;
			msgdlg.isWait = true;
		}
	/// <summary>
	/// Thực thi phương thức nghiệp vụ startWaitDlg.
	/// </summary>
	public static void startWaitDlg()
		{
			closeKeyBoard();
			msgdlg.setInfo(mResources.PLEASEWAIT, null, new Command(mResources.CANCEL, instance, 8882, null), null);
			currentDialog = msgdlg;
			msgdlg.isWait = true;
		}
	/// <summary>
	/// Thực thi phương thức nghiệp vụ openWeb.
	/// </summary>
	public void openWeb(string strLeft, string strRight, string url, string str)
		{
			str = Res.changeString(str);
			msgdlg.setInfo(str, new Command(strLeft, this, 8881, url), null, new Command(strRight, this, 8882, null));
			currentDialog = msgdlg;
		}
	/// <summary>
	/// Thực thi phương thức nghiệp vụ startOK.
	/// </summary>
	public static void startOK(string info, int actionID, object p)
		{
			DragonBoy_Net8_Native.Src.Mod.Security.ModCredentialSecurity.OnLoginFinished();
			info = Res.changeString(info);
			closeKeyBoard();
			msgdlg.setInfo(info, null, new Command(mResources.OK, instance, actionID, p), null);
			msgdlg.show();
		}
	/// <summary>
	/// Thực thi phương thức nghiệp vụ startserverThongBao.
	/// </summary>
	public static void startserverThongBao(string msgSv)
		{
			msgSv = Res.changeString(msgSv);
			thongBaoTest = msgSv;
			xThongBaoTranslate = w - 60;
			dir_ = -1;
			ModBossNotice.LogBossDebug("RAW-TICKER", msgSv);
			ModBossNotice.ProcessServerBossNotice(msgSv);
		}
	/// <summary>
	/// Truy xuất và trả về dữ liệu của getMoneys.
	/// </summary>
	public static string getMoneys(int m)
		{
			string text = string.Empty;
			int num = m / 1000 + 1;
			for (int i = 0; i < num; i++)
			{
				if (m >= 1000)
				{
					int num2 = m % 1000;
					text = ((num2 != 0) ? ((num2 >= 10) ? ((num2 >= 100) ? ("." + num2 + text) : (".0" + num2 + text)) : (".00" + num2 + text)) : (".000" + text));
					m /= 1000;
					continue;
				}
				text = m + text;
				break;
			}
			return text;
		}
	/// <summary>
	/// Truy xuất và trả về dữ liệu của getX.
	/// </summary>
	public static int getX(int start, int w)
		{
			return (px - start) / w;
		}
	/// <summary>
	/// Truy xuất và trả về dữ liệu của getY.
	/// </summary>
	public static int getY(int start, int w)
		{
			return (py - start) / w;
		}
	/// <summary>
	/// Thực thi phương thức nghiệp vụ sizeChanged.
	/// </summary>
	protected void sizeChanged(int w, int h)
		{
		}
	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của isGetResourceFromServer.
	/// </summary>
	public static bool isGetResourceFromServer()
		{
			return true;
		}

}

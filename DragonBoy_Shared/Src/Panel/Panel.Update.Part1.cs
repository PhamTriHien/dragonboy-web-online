using System;
using System.Collections.Generic;
using Assets.src.g;
using UnityEngine;
/// <summary>
/// Thành phần phân mảnh (partial) của lớp Panel: Đảm nhiệm vòng lặp cập nhật trạng thái logic theo từng khung hình (Part1).
/// </summary>
public partial class Panel : IActionListener, IChatable
{
	/// <summary>
	/// Cập nhật trạng thái logic của updateKey theo chu kỳ khung hình.
	/// </summary>
	public void updateKey()
		{
			if ((chatTField != null && chatTField.isShow) || !GameCanvas.panel.isDoneCombine || InfoDlg.isShow)
			{
				return;
			}
			if (tabIcon != null && tabIcon.isShow)
			{
				tabIcon.updateKey();
			}
			else
			{
				if (isClose || !isShow)
				{
					return;
				}
				if (cmdClose.isPointerPressInside())
				{
					cmdClose.performAction();
					return;
				}
				if (GameCanvas.keyPressed[13])
				{
					if (type != 4)
					{
						hide();
						return;
					}
					setTypeMain();
					cmx = (cmtoX = 0);
				}
				if (GameCanvas.keyPressed[12] || GameCanvas.keyPressed[(!Main.isPC) ? 5 : 25])
				{
					if (left.idAction > 0)
					{
						perform(left.idAction, left.p);
					}
					else
					{
						waitToPerform = 2;
					}
				}
				if (Equals(GameCanvas.panel) && GameCanvas.panel2 == null && GameCanvas.isPointerJustRelease && !GameCanvas.isPointer(X, Y, W, H) && !pointerIsDowning)
				{
					hide();
					return;
				}
				if (!isClanOption)
				{
					updateKeyInTabBar();
				}
				switch (type)
				{
				case 23:
				case 24:
					updateKeyScrollView();
					break;
				case 21:
					if (currentTabIndex == 0)
					{
						updateKeyScrollView();
					}
					if (currentTabIndex == 1)
					{
						updateKeyPetStatus();
					}
					if (currentTabIndex == 2)
					{
						updateKeyScrollView();
					}
					break;
				case 0:
					if (currentTabIndex == 0)
					{
						updateKeyQuest();
						GameCanvas.clearKeyPressed();
						return;
					}
					if (currentTabIndex == 1)
					{
						updateKeyInventory();
					}
					if (currentTabIndex == 2)
					{
						updateKeySkill();
					}
					if (currentTabIndex == 3)
					{
						if (mainTabName.Length == 4)
						{
							updateKeyTool();
						}
						else
						{
							updateKeyClans();
						}
					}
					if (currentTabIndex == 4)
					{
						updateKeyTool();
					}
					break;
				case 2:
					updateKeyInventory();
					break;
				case 3:
					updateKeyScrollView();
					break;
				case 14:
					updateKeyScrollView();
					break;
				case 1:
				case 17:
				case 25:
					if (currentTabIndex < currentTabName.Length - ((GameCanvas.panel2 == null) ? 1 : 0) && type != 17)
					{
						updateKeyScrollView();
					}
					else if (typeShop == 0)
					{
						updateKeyInventory();
					}
					else
					{
						updateKeyScrollView();
					}
					break;
				case 4:
					updateKeyMap();
					GameCanvas.clearKeyPressed();
					return;
				case 7:
					updateKeyInventory();
					break;
				case 8:
					updateKeyScrollView();
					break;
				case 9:
					updateKeyScrollView();
					break;
				case 10:
					updateKeyScrollView();
					break;
				case 11:
				case 16:
					updateKeyScrollView();
					break;
				case 15:
					updateKeyScrollView();
					break;
				case 12:
					updateKeyCombine();
					break;
				case 13:
					updateKeyGiaoDich();
					break;
				case 18:
					updateKeyScrollView();
					break;
				case 19:
					updateKeyOption();
					break;
				case 20:
					updateKeyOption();
					break;
				case 22:
					updateKeyAuto();
					break;
				}
				GameCanvas.clearKeyHold();
				for (int i = 0; i < GameCanvas.keyPressed.Length; i++)
				{
					GameCanvas.keyPressed[i] = false;
				}
			}
		}
	/// <summary>
	/// Cập nhật trạng thái logic của updateKeyAuto theo chu kỳ khung hình.
	/// </summary>
	private void updateKeyAuto()
		{
		}
	/// <summary>
	/// Thực thi phương thức nghiệp vụ keyGiaodich.
	/// </summary>
	private void keyGiaodich()
		{
			updateKeyScrollView();
		}
	/// <summary>
	/// Cập nhật trạng thái logic của updateKeyGiaoDich theo chu kỳ khung hình.
	/// </summary>
	private void updateKeyGiaoDich()
		{
			if (currentTabIndex == 0)
			{
				if (Equals(GameCanvas.panel))
				{
					updateKeyInventory();
				}
				if (Equals(GameCanvas.panel2))
				{
					keyGiaodich();
				}
			}
			if (currentTabIndex == 1 || currentTabIndex == 2)
			{
				keyGiaodich();
			}
		}
	/// <summary>
	/// Cập nhật trạng thái logic của updateKeyTool theo chu kỳ khung hình.
	/// </summary>
	private void updateKeyTool()
		{
			updateKeyScrollView();
		}
	/// <summary>
	/// Cập nhật trạng thái logic của updateKeySkill theo chu kỳ khung hình.
	/// </summary>
	private void updateKeySkill()
		{
			updateKeyScrollView();
		}
	/// <summary>
	/// Cập nhật trạng thái logic của updateKeyQuest theo chu kỳ khung hình.
	/// </summary>
	private void updateKeyQuest()
		{
			if (GameCanvas.keyHold[(!Main.isPC) ? 2 : 21])
			{
				cmyQuest -= 5;
			}
			if (GameCanvas.keyHold[(!Main.isPC) ? 8 : 22])
			{
				cmyQuest += 5;
			}
			if (cmyQuest < 0)
			{
				cmyQuest = 0;
			}
			int num = indexRowMax * 12 - (hScroll - 60);
			if (num < 0)
			{
				num = 0;
			}
			if (cmyQuest > num)
			{
				cmyQuest = num;
			}
			if (scroll != null)
			{
				if (!GameCanvas.isTouch)
				{
					scroll.cmy = cmyQuest;
				}
				scroll.updateKey();
			}
			int num2 = xScroll + wScroll / 2 - 35;
			int num3 = ((GameCanvas.h <= 300) ? 15 : 20);
			int num4 = yScroll + hScroll - num3 - 15;
			int px = GameCanvas.px;
			int py = GameCanvas.py;
			keyTouchMapButton = -1;
			if (isPaintMap && !GameScr.gI().isMapDocNhan() && px >= num2 && px <= num2 + 70 && py >= num4 && py <= num4 + 30 && (scroll == null || !scroll.pointerIsDowning))
			{
				keyTouchMapButton = 1;
				if (GameCanvas.isPointerJustRelease)
				{
					SoundMn.gI().buttonClick();
					waitToPerform = 2;
					GameCanvas.clearAllPointerEvent();
				}
			}
		}
	/// <summary>
	/// Cập nhật trạng thái logic của updateScroolMouse theo chu kỳ khung hình.
	/// </summary>
	public void updateScroolMouse(int a)
		{
			if (a == 0) return;
			if (scroll != null && GameCanvas.pxMouse >= scroll.xPos && GameCanvas.pxMouse <= scroll.xPos + scroll.width)
			{
				scroll.cmtoY -= a * 12;
				if (scroll.cmtoY > scroll.cmyLim) scroll.cmtoY = scroll.cmyLim;
				if (scroll.cmtoY < 0) scroll.cmtoY = 0;
				return;
			}
			if (GameCanvas.pxMouse < xScroll || GameCanvas.pxMouse > xScroll + wScroll)
			{
				return;
			}
			bool flag = false;
			if (indexMouse == -1)
			{
				indexMouse = selected >= 0 ? selected : (cmtoY / 12);
			}
			if (a > 0)
			{
				indexMouse -= a;
				flag = true;
			}
			else if (a < 0)
			{
				indexMouse += -a;
				flag = true;
			}
			int maxIndex = System.Math.Max(0, (cmyLim + 11) / 12);
			if (indexMouse > maxIndex)
			{
				indexMouse = maxIndex;
			}
			if (indexMouse < 0)
			{
				indexMouse = 0;
			}
			if (flag)
			{
				cmtoY = indexMouse * 12;
				if (cmtoY > cmyLim)
				{
					cmtoY = cmyLim;
				}
				if (cmtoY < 0)
				{
					cmtoY = 0;
				}
			}
		}

}

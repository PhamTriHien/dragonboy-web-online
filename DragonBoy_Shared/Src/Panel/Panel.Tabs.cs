using System;
using Assets.src.g;
using UnityEngine;

/// <summary>
/// Thành phần phân mảnh (partial) của lớp Panel: Đảm nhiệm phân hệ chuyên trách Tabs ().
/// </summary>
public partial class Panel : IActionListener
{
	/// <summary>
	/// Thiết lập giá trị hoặc trạng thái cho setType.
	/// </summary>
	private void setType(int position)
		{
			typeShop = -1;
			W = GetScaledPanelWidth();
			WIDTH_PANEL = W;
			H = GameCanvas.h;
			X = 0;
			Y = 0;
			ITEM_HEIGHT = 24;
			this.position = position;
			switch (position)
			{
			case 0:
				xScroll = 2;
				yScroll = 80;
				wScroll = W - 4;
				hScroll = H - 96;
				cmx = wScroll;
				cmtoX = 0;
				X = 0;
				break;
		case 1:
			wScroll = W - 4;
			xScroll = GameCanvas.w - wScroll;
			yScroll = 80;
			hScroll = H - 96;
			X = xScroll - 2;
			cmx = -(GameCanvas.w + W);
			cmtoX = GameCanvas.w - W;
			break;
		default:
			xScroll = 2;
			yScroll = 80;
			wScroll = W - 4;
			hScroll = H - 96;
			cmx = wScroll;
			cmtoX = 0;
			X = 0;
			break;
		}
			if (cmdClose != null)
			{
				cmdClose.x = X + W - 26;
				cmdClose.y = 3;
			}
			TAB_W = W / 5 - 1;
			currentTabIndex = 0;
			currentTabName = tabName[type];
			if (currentTabName.Length < 5)
			{
				TAB_W += 5;
			}
			startTabPos = xScroll + wScroll / 2 - currentTabName.Length * TAB_W / 2;
			lastSelect = new int[currentTabName.Length];
			cmyLast = new int[currentTabName.Length];
			for (int i = 0; i < currentTabName.Length; i++)
			{
				lastSelect[i] = (GameCanvas.isTouch ? (-1) : 0);
			}
			if (lastTabIndex[type] != -1)
			{
				currentTabIndex = lastTabIndex[type];
			}
			if (currentTabIndex < 0)
			{
				currentTabIndex = 0;
			}
			if (currentTabIndex > currentTabName.Length - 1)
			{
				currentTabIndex = currentTabName.Length - 1;
			}
			scroll = null;
			if (type == 0)
			{
				EnsurePetTabLayout();
			}
		}

	/// <summary>
	/// Thiết lập giá trị hoặc trạng thái cho setTypeInfomatioin.
	/// </summary>
	public void setTypeInfomatioin()
		{
			type = 6;
			cmx = wScroll;
			cmtoX = 0;
		}

	/// <summary>
	/// Thiết lập giá trị hoặc trạng thái cho setTypeArchivement.
	/// </summary>
	public void setTypeArchivement()
		{
			currentListLength = Char.myCharz().arrArchive.Length;
			setType(0);
			type = 9;
			cmyLim = currentListLength * ITEM_HEIGHT - hScroll;
			cmy = (cmtoY = cmyLast[currentTabIndex]);
			if (cmyLim < 0)
			{
				cmyLim = 0;
			}
			if (cmy < 0)
			{
				cmy = (cmtoY = 0);
			}
			if (cmy > cmyLim)
			{
				cmy = (cmtoY = 0);
			}
			selected = (GameCanvas.isTouch ? (-1) : 0);
		}

	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của IsNewMessage.
	/// </summary>
	private bool IsNewMessage(string name)
		{
			return false;
		}

	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của IsHaveNewMessage.
	/// </summary>
	public bool IsHaveNewMessage()
		{
			return false;
		}

	/// <summary>
	/// Dọn dẹp và đặt lại trạng thái ban đầu cho ClearNewMessage.
	/// </summary>
	private void ClearNewMessage(string name)
		{
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ addPlayerMenu.
	/// </summary>
	public void addPlayerMenu(Command pm)
		{
			vPlayerMenu.addElement(pm);
		}

	/// <summary>
	/// Thiết lập giá trị hoặc trạng thái cho setTabPlayerMenu.
	/// </summary>
	public void setTabPlayerMenu()
		{
			ITEM_HEIGHT = 24;
			currentListLength = vPlayerMenu.size();
			cmyLim = currentListLength * ITEM_HEIGHT - hScroll;
			if (cmyLim < 0)
			{
				cmyLim = 0;
			}
			cmy = (cmtoY = cmyLast[currentTabIndex]);
			if (cmy < 0)
			{
				cmy = (cmtoY = 0);
			}
			if (cmy > cmyLim)
			{
				cmy = (cmtoY = cmyLim);
			}
			selected = (GameCanvas.isTouch ? (-1) : 0);
		}

	/// <summary>
	/// Thiết lập giá trị hoặc trạng thái cho setTypeFlag.
	/// </summary>
	public void setTypeFlag()
		{
			type = 18;
			setType(0);
			ITEM_HEIGHT = 24;
			selected = (GameCanvas.isTouch ? (-1) : 0);
			setTabFlag();
		}

	/// <summary>
	/// Thiết lập giá trị hoặc trạng thái cho setTabFlag.
	/// </summary>
	public void setTabFlag()
		{
			currentListLength = vFlag.size();
			cmyLim = currentListLength * ITEM_HEIGHT - hScroll;
			if (cmyLim < 0)
			{
				cmyLim = 0;
			}
			cmy = (cmtoY = cmyLast[currentTabIndex]);
			if (cmy < 0)
			{
				cmy = (cmtoY = 0);
			}
			if (cmy > cmyLim)
			{
				cmy = (cmtoY = cmyLim);
			}
			if (selected > currentListLength - 1)
			{
				selected = currentListLength - 1;
			}
			cmx = (cmtoX = 0);
		}

	/// <summary>
	/// Thiết lập giá trị hoặc trạng thái cho setTypePlayerMenu.
	/// </summary>
	public void setTypePlayerMenu(Char c)
		{
			type = 10;
			charMenu = c;
			setType(0);
			setTabPlayerMenu();
		}

	/// <summary>
	/// Thiết lập giá trị hoặc trạng thái cho setTypeFriend.
	/// </summary>
	public void setTypeFriend()
		{
			type = 11;
			setType(0);
			ITEM_HEIGHT = 24;
			selected = (GameCanvas.isTouch ? (-1) : 0);
			setTabFriend();
		}

	/// <summary>
	/// Thiết lập giá trị hoặc trạng thái cho setTypeEnemy.
	/// </summary>
	public void setTypeEnemy()
		{
			type = 16;
			setType(0);
			ITEM_HEIGHT = 24;
			selected = (GameCanvas.isTouch ? (-1) : 0);
			setTabEnemy();
		}

	/// <summary>
	/// Thiết lập giá trị hoặc trạng thái cho setTabTop.
	/// </summary>
	public void setTabTop()
		{
			currentListLength = vTop.size();
			cmyLim = currentListLength * ITEM_HEIGHT - hScroll;
			if (cmyLim < 0)
			{
				cmyLim = 0;
			}
			cmy = (cmtoY = cmyLast[currentTabIndex]);
			if (cmy < 0)
			{
				cmy = (cmtoY = 0);
			}
			if (cmy > cmyLim)
			{
				cmy = (cmtoY = cmyLim);
			}
			if (selected > currentListLength - 1)
			{
				selected = currentListLength - 1;
			}
			cmx = (cmtoX = 0);
		}

	/// <summary>
	/// Thiết lập giá trị hoặc trạng thái cho setTabFriend.
	/// </summary>
	public void setTabFriend()
		{
			currentListLength = vFriend.size();
			cmyLim = currentListLength * ITEM_HEIGHT - hScroll;
			if (cmyLim < 0)
			{
				cmyLim = 0;
			}
			cmy = (cmtoY = cmyLast[currentTabIndex]);
			if (cmy < 0)
			{
				cmy = (cmtoY = 0);
			}
			if (cmy > cmyLim)
			{
				cmy = (cmtoY = cmyLim);
			}
			if (selected > currentListLength - 1)
			{
				selected = currentListLength - 1;
			}
			cmx = (cmtoX = 0);
		}

	/// <summary>
	/// Thiết lập giá trị hoặc trạng thái cho setTabEnemy.
	/// </summary>
	public void setTabEnemy()
		{
			currentListLength = vEnemy.size();
			cmyLim = currentListLength * ITEM_HEIGHT - hScroll;
			if (cmyLim < 0)
			{
				cmyLim = 0;
			}
			cmy = (cmtoY = cmyLast[currentTabIndex]);
			if (cmy < 0)
			{
				cmy = (cmtoY = 0);
			}
			if (cmy > cmyLim)
			{
				cmy = (cmtoY = cmyLim);
			}
			if (selected > currentListLength - 1)
			{
				selected = currentListLength - 1;
			}
			cmx = (cmtoX = 0);
		}

	/// <summary>
	/// Thiết lập giá trị hoặc trạng thái cho setTypeMessage.
	/// </summary>
	public void setTypeMessage()
		{
			type = 8;
			setType(0);
			setTabMessage();
			currentTabIndex = 0;
		}

	/// <summary>
	/// Thiết lập giá trị hoặc trạng thái cho setTypeAuto.
	/// </summary>
	public void setTypeAuto()
		{
			type = 22;
			setType(0);
			setTabAuto();
			cmx = (cmtoX = 0);
		}

	/// <summary>
	/// Thiết lập giá trị hoặc trạng thái cho setTabAuto.
	/// </summary>
	private void setTabAuto()
		{
			currentListLength = strAuto.Length;
			ITEM_HEIGHT = 24;
			selected = (GameCanvas.isTouch ? (-1) : 0);
			cmyLim = currentListLength * ITEM_HEIGHT - hScroll;
			if (cmyLim < 0)
			{
				cmyLim = 0;
			}
			cmy = (cmtoY = cmyLast[currentTabIndex]);
			if (cmy < 0)
			{
				cmy = (cmtoY = 0);
			}
			if (cmy > cmyLim)
			{
				cmy = (cmtoY = cmyLim);
			}
		}

	/// <summary>
	/// Thiết lập giá trị hoặc trạng thái cho setTypeMain.
	/// </summary>
	public void setTypeMain()
		{
			type = 0;
			setType(0);
			if (currentTabIndex == 1)
			{
				setTabInventory(resetSelect: true);
			}
			if (currentTabIndex == 2)
			{
				setTabSkill();
			}
			if (currentTabIndex == 3)
			{
				if (mainTabName.Length == 4)
				{
					setTabTool();
				}
				else
				{
					setTabClans();
				}
			}
			if (currentTabIndex == 4)
			{
				setTabTool();
			}
		}


	/// <summary>
	/// Thiết lập giá trị hoặc trạng thái cho setTabGiaoDich.
	/// </summary>
	public void setTabGiaoDich(bool isMe)
		{
			currentListLength = ((!isMe) ? (vFriendGD.size() + 3) : (vMyGD.size() + 3));
			ITEM_HEIGHT = 24;
			selected = (GameCanvas.isTouch ? (-1) : 0);
			cmyLim = currentListLength * ITEM_HEIGHT - hScroll;
			if (cmyLim < 0)
			{
				cmyLim = 0;
			}
			cmy = (cmtoY = cmyLast[currentTabIndex]);
			if (cmy < 0)
			{
				cmy = (cmtoY = 0);
			}
			if (cmy > cmyLim)
			{
				cmy = (cmtoY = cmyLim);
			}
		}

	/// <summary>
	/// Thiết lập giá trị hoặc trạng thái cho setTypeGiaoDich.
	/// </summary>
	public void setTypeGiaoDich(Char cGD)
		{
			type = 13;
			tabName[type] = boxGD;
			isAccept = false;
			isLock = false;
			isFriendLock = false;
			vMyGD.removeAllElements();
			vFriendGD.removeAllElements();
			moneyGD = 0;
			friendMoneyGD = 0;
			if (GameCanvas.w > 2 * WIDTH_PANEL)
			{
				GameCanvas.panel2 = new Panel();
				GameCanvas.panel2.type = 13;
				GameCanvas.panel2.tabName[type] = new string[1][] { mResources.item_receive };
				GameCanvas.panel2.setType(1);
				GameCanvas.panel2.setTabGiaoDich(isMe: false);
				GameCanvas.panel.tabName[type] = new string[2][]
				{
					mResources.inventory,
					mResources.item_give
				};
				GameCanvas.panel2.show();
				GameCanvas.panel2.charMenu = cGD;
			}
			if (Equals(GameCanvas.panel))
			{
				setType(0);
			}
			if (currentTabIndex == 0)
			{
				setTabInventory(resetSelect: true);
			}
			if (currentTabIndex == 1)
			{
				setTabGiaoDich(isMe: true);
			}
			if (currentTabIndex == 2)
			{
				setTabGiaoDich(isMe: false);
			}
			charMenu = cGD;
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ subArray.
	/// </summary>
	public string subArray(string[] str)
		{
			return null;
		}

	/// <summary>
	/// Thiết lập giá trị hoặc trạng thái cho setTabTool.
	/// </summary>
	private void setTabTool()
		{
			SoundMn.gI().getSoundOption();
			currentListLength = strTool.Length;
			ITEM_HEIGHT = 24;
			selected = (GameCanvas.isTouch ? (-1) : 0);
			cmyLim = currentListLength * ITEM_HEIGHT - hScroll;
			if (cmyLim < 0)
			{
				cmyLim = 0;
			}
			cmy = (cmtoY = cmyLast[currentTabIndex]);
			if (cmy < 0)
			{
				cmy = (cmtoY = 0);
			}
			if (cmy > cmyLim)
			{
				cmy = (cmtoY = cmyLim);
			}
		}

	/// <summary>
	/// Khởi tạo và nạp dữ liệu cấu hình cho initLogMessage.
	/// </summary>
	public void initLogMessage()
		{
			currentListLength = logChat.size() + 1;
			cmyLim = currentListLength * ITEM_HEIGHT - hScroll;
			if (cmyLim < 0)
			{
				cmyLim = 0;
			}
			cmy = (cmtoY = cmyLast[currentTabIndex]);
			if (cmy < 0)
			{
				cmy = (cmtoY = 0);
			}
			if (cmy > cmyLim)
			{
				cmy = (cmtoY = cmyLim);
			}
			cmx = (cmtoX = 0);
		}

	/// <summary>
	/// Thiết lập giá trị hoặc trạng thái cho setTabMessage.
	/// </summary>
	private void setTabMessage()
		{
			ITEM_HEIGHT = 24;
			initLogMessage();
			selected = (GameCanvas.isTouch ? (-1) : 0);
		}

	/// <summary>
	/// Thiết lập giá trị hoặc trạng thái cho setTabSkill.
	/// </summary>
	private void setTabSkill()
		{
			ITEM_HEIGHT = 30;
			currentListLength = Char.myCharz().nClass.skillTemplates.Length + 6;
			cmyLim = currentListLength * ITEM_HEIGHT - hScroll;
			if (cmyLim < 0)
			{
				cmyLim = 0;
			}
			cmy = (cmtoY = cmyLast[currentTabIndex]);
			if (cmy < 0)
			{
				cmy = (cmtoY = 0);
			}
			if (cmy > cmyLim)
			{
				cmy = cmyLim;
			}
			selected = (GameCanvas.isTouch ? (-1) : 0);
		}

	/// <summary>
	/// Thiết lập giá trị hoặc trạng thái cho setTabTask.
	/// </summary>
	private void setTabTask()
		{
			cmyQuest = 0;
		}


	/// <summary>
	/// Thiết lập giá trị hoặc trạng thái cho setTypeGameSubInfo.
	/// </summary>
	private void setTypeGameSubInfo()
		{
			string content = ((GameInfo)vGameInfo.elementAt(infoSelect)).content;
			contenInfo = mFont.tahoma_7_grey.splitFontArray(content, wScroll - 40);
			currentListLength = contenInfo.Length;
			ITEM_HEIGHT = 16;
			selected = (GameCanvas.isTouch ? (-1) : 0);
			cmyLim = currentListLength * ITEM_HEIGHT - hScroll;
			if (cmyLim < 0)
			{
				cmyLim = 0;
			}
			if (cmy < 0)
			{
				cmy = (cmtoY = 0);
			}
			if (cmy > cmyLim)
			{
				cmy = (cmtoY = cmyLim);
			}
			type = 24;
			setType(0);
		}

	/// <summary>
	/// Thiết lập giá trị hoặc trạng thái cho setTypeGameInfo.
	/// </summary>
	private void setTypeGameInfo()
		{
			currentListLength = vGameInfo.size();
			ITEM_HEIGHT = 24;
			selected = (GameCanvas.isTouch ? (-1) : 0);
			cmyLim = currentListLength * ITEM_HEIGHT - hScroll;
			if (cmyLim < 0)
			{
				cmyLim = 0;
			}
			if (cmy < 0)
			{
				cmy = (cmtoY = 0);
			}
			if (cmy > cmyLim)
			{
				cmy = (cmtoY = cmyLim);
			}
			type = 23;
			setType(0);
		}


	/// <summary>
	/// Thiết lập giá trị hoặc trạng thái cho setTypeOption.
	/// </summary>
	public void setTypeOption()
		{
			type = 19;
			setType(0);
			setTabOption();
			cmx = (cmtoX = 0);
		}

	/// <summary>
	/// Thiết lập giá trị hoặc trạng thái cho setTabOption.
	/// </summary>
	private void setTabOption()
		{
			SoundMn.gI().getStrOption();
			currentListLength = strCauhinh.Length;
			ITEM_HEIGHT = 24;
			selected = (GameCanvas.isTouch ? (-1) : 0);
			cmyLim = currentListLength * ITEM_HEIGHT - hScroll;
			if (cmyLim < 0)
			{
				cmyLim = 0;
			}
			cmy = (cmtoY = cmyLast[currentTabIndex]);
			if (cmy < 0)
			{
				cmy = (cmtoY = 0);
			}
			if (cmy > cmyLim)
			{
				cmy = (cmtoY = cmyLim);
			}
		}

	/// <summary>
	/// Thiết lập giá trị hoặc trạng thái cho setTypeAccount.
	/// </summary>
	public void setTypeAccount()
		{
			type = 20;
			setType(0);
			setTabAccount();
			cmx = (cmtoX = 0);
		}

	/// <summary>
	/// Thiết lập giá trị hoặc trạng thái cho setTabAccount.
	/// </summary>
	private void setTabAccount()
		{
			if (Main.IphoneVersionApp)
			{
				strAccount = new string[4]
				{
					mResources.inventory_Pass,
					mResources.friend,
					mResources.enemy,
					mResources.msg
				};
				if (GameScr.canAutoPlay)
				{
					strAccount = new string[5]
					{
						mResources.inventory_Pass,
						mResources.friend,
						mResources.enemy,
						mResources.msg,
						mResources.autoFunction
					};
				}
			}
			else
			{
				strAccount = new string[5]
				{
					mResources.inventory_Pass,
					mResources.friend,
					mResources.enemy,
					mResources.msg,
					mResources.charger
				};
				if (GameScr.canAutoPlay)
				{
					strAccount = new string[6]
					{
						mResources.inventory_Pass,
						mResources.friend,
						mResources.enemy,
						mResources.msg,
						mResources.charger,
						mResources.autoFunction
					};
				}
				if ((mSystem.clientType == 2 || mSystem.clientType == 7) && mResources.language != 2)
				{
					strAccount = new string[5]
					{
						mResources.inventory_Pass,
						mResources.friend,
						mResources.enemy,
						mResources.msg,
						mResources.charger
					};
					if (GameScr.canAutoPlay)
					{
						strAccount = new string[6]
						{
							mResources.inventory_Pass,
							mResources.friend,
							mResources.enemy,
							mResources.msg,
							mResources.charger,
							mResources.autoFunction
						};
					}
				}
			}
			currentListLength = strAccount.Length;
			ITEM_HEIGHT = 24;
			selected = (GameCanvas.isTouch ? (-1) : 0);
			cmyLim = currentListLength * ITEM_HEIGHT - hScroll;
			if (cmyLim < 0)
			{
				cmyLim = 0;
			}
			cmy = (cmtoY = cmyLast[currentTabIndex]);
			if (cmy < 0)
			{
				cmy = (cmtoY = 0);
			}
			if (cmy > cmyLim)
			{
				cmy = (cmtoY = cmyLim);
			}
		}

	/// <summary>
	/// Thiết lập giá trị hoặc trạng thái cho setTypeSpeacialSkill.
	/// </summary>
	public void setTypeSpeacialSkill()
		{
			type = 25;
			setType(0);
			setTabSpeacialSkill();
			currentTabIndex = 0;
		}

	/// <summary>
	/// Thiết lập giá trị hoặc trạng thái cho setTabSpeacialSkill.
	/// </summary>
	private void setTabSpeacialSkill()
		{
			ITEM_HEIGHT = 24;
			currentListLength = Char.myCharz().infoSpeacialSkill[currentTabIndex].Length;
			cmyLim = currentListLength * ITEM_HEIGHT - hScroll;
			if (cmyLim < 0)
			{
				cmyLim = 0;
			}
			cmy = (cmtoY = cmyLast[currentTabIndex]);
			if (cmy < 0)
			{
				cmy = (cmtoY = 0);
			}
			if (cmy > cmyLim)
			{
				cmy = (cmtoY = cmyLim);
			}
			selected = (GameCanvas.isTouch ? (-1) : 0);
		}


}

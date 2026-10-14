using System;
using Assets.src.e;
using Assets.src.g;
using UnityEngine;
/// <summary>
/// Thành phần phân mảnh (partial) của lớp Char: Đảm nhiệm phân hệ chuyên trách Part1 ().
/// </summary>
public partial class Char : IMapObject
{
	/// <summary>
	/// Thực thi phương thức nghiệp vụ Char.
	/// </summary>
	public Char()
			{
				statusMe = 6;
			}
	/// <summary>
	/// Thực thi phương thức nghiệp vụ taskAction.
	/// </summary>
	public static void taskAction(bool isNextStep)
			{
				Task task = myCharz().taskMaint;
				if (task.index > task.contentInfo.Length - 1)
				{
					task.index = task.contentInfo.Length - 1;
				}
				string text = task.contentInfo[task.index];
				if (text != null && !text.Equals(string.Empty))
				{
					if (text.StartsWith("#"))
					{
						text = NinjaUtil.replace(text, "#", string.Empty);
						Npc npc = new Npc(5, 0, -100, -100, 5, GameScr.info1.charId[myCharz().cgender][2]);
						npc.cx = (npc.cy = -100);
						npc.avatar = GameScr.info1.charId[myCharz().cgender][2];
						npc.charID = 5;
						if (GameCanvas.currentScreen == GameScr.instance)
						{
							ChatPopup.addNextPopUpMultiLine(text, npc);
						}
					}
					else if (isNextStep)
					{
						GameScr.info1.addInfo(text, 0);
					}
				}
				GameScr.isHaveSelectSkill = true;
				Cout.println("TASKx " + myCharz().taskMaint.taskId);
				if (myCharz().taskMaint.taskId <= 2)
				{
					myCharz().canFly = false;
				}
				else
				{
					myCharz().canFly = true;
				}
				GameScr.gI().left = null;
				if (task.taskId == 0)
				{
					Hint.isViewMap = false;
					Hint.isViewPotential = false;
					GameScr.gI().right = null;
					GameScr.isHaveSelectSkill = false;
					GameScr.gI().left = null;
					if (task.index < 4)
					{
						MagicTree.isPaint = false;
						GameScr.isPaintRada = -1;
					}
					if (task.index == 4)
					{
						GameScr.isPaintRada = 1;
						MagicTree.isPaint = true;
					}
					if (task.index >= 5)
					{
						GameScr.gI().right = GameScr.gI().cmdFocus;
					}
				}
				if (task.taskId == 1)
				{
					GameScr.isHaveSelectSkill = true;
				}
				if (task.taskId >= 1)
				{
					GameScr.gI().right = GameScr.gI().cmdFocus;
					GameScr.gI().left = GameScr.gI().cmdMenu;
				}
				if (task.taskId >= 0)
				{
					Panel.isPaintMap = true;
				}
				else
				{
					Panel.isPaintMap = false;
				}
				if (task.taskId < 12)
				{
					GameCanvas.panel.mainTabName = mResources.mainTab1;
				}
				else
				{
					GameCanvas.panel.mainTabName = mResources.mainTab2;
				}
				GameCanvas.panel.tabName[0] = GameCanvas.panel.mainTabName;
				if (myChar.taskMaint.taskId > 10)
				{
					Rms.saveRMSString("fake", "aa");
				}
			}
	/// <summary>
	/// Thực thi phương thức nghiệp vụ avatarz.
	/// </summary>
	public int avatarz()
			{
				return getAvatar(head);
			}
	/// <summary>
	/// Thêm thông điệp thông báo nổi (toast notice) vào hàng đợi hiển thị.
	/// </summary>
	public void addInfo(string info)
			{
				if (chatInfo == null)
				{
					chatInfo = new Info();
				}
				Char cInfo = null;
				chatInfo.addInfo(info, 0, cInfo, isChatServer: false);
			}
	/// <summary>
	/// Thực thi phương thức nghiệp vụ myCharz.
	/// </summary>
	public static Char myCharz()
			{
				if (myChar == null)
				{
					myChar = new Char();
					myChar.me = true;
					myChar.cmtoChar = true;
				}
				return myChar;
			}
	/// <summary>
	/// Thực thi phương thức nghiệp vụ myPetz.
	/// </summary>
	public static Char myPetz()
			{
				if (myPet == null)
				{
					myPet = new Char();
					myPet.me = false;
				}
				return myPet;
			}
	/// <summary>
	/// Dọn dẹp và đặt lại trạng thái ban đầu cho clearMyChar.
	/// </summary>
	public static void clearMyChar()
			{
				myChar = null;
			}
	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của isInEnterOfflinePoint.
	/// </summary>
	public Waypoint isInEnterOfflinePoint()
			{
				Task task = myChar.taskMaint;
				if (task != null && task.taskId == 0 && task.index < 6)
				{
					return null;
				}
				int num = TileMap.vGo.size();
				for (sbyte b = 0; b < num; b++)
				{
					Waypoint waypoint = (Waypoint)TileMap.vGo.elementAt(b);
					if (PopUp.vPopups.size() >= num)
					{
						PopUp popUp = (PopUp)PopUp.vPopups.elementAt(b);
						if (!popUp.isPaint)
						{
							return null;
						}
					}
					if (cx >= waypoint.minX && cx <= waypoint.maxX && cy >= waypoint.minY && cy <= waypoint.maxY && waypoint.isEnter && waypoint.isOffline)
					{
						return waypoint;
					}
				}
				return null;
			}
	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của isInEnterOnlinePoint.
	/// </summary>
	public Waypoint isInEnterOnlinePoint()
			{
				Task task = myChar.taskMaint;
				if (task != null && task.taskId == 0 && task.index < 6)
				{
					return null;
				}
				int num = TileMap.vGo.size();
				for (sbyte b = 0; b < num; b++)
				{
					Waypoint waypoint = (Waypoint)TileMap.vGo.elementAt(b);
					if (PopUp.vPopups.size() >= num)
					{
						PopUp popUp = (PopUp)PopUp.vPopups.elementAt(b);
						if (!popUp.isPaint)
						{
							return null;
						}
					}
					if (cx >= waypoint.minX && cx <= waypoint.maxX && cy >= waypoint.minY && cy <= waypoint.maxY && waypoint.isEnter && !waypoint.isOffline)
					{
						return waypoint;
					}
				}
				return null;
			}
	/// <summary>
	/// Ẩn thành phần giao diện khỏi màn hình hiển thị.
	/// </summary>
	public void hide()
			{
				isHide = true;
				EffecMn.addEff(new Effect(107, cx, cy + 25, 3, 15, 1));
			}
	/// <summary>
	/// Hiển thị thành phần giao diện hoặc bảng thông báo lên màn hình.
	/// </summary>
	public void show()
			{
				isHide = false;
				EffecMn.addEff(new Effect(107, cx, cy + 25, 3, 10, 1));
			}
	/// <summary>
	/// Thực thi phương thức nghiệp vụ returnAct.
	/// </summary>
	public int returnAct(int xFirst, int yFirst, int xEnd, int yEnd)
			{
				int num = xEnd - xFirst;
				int num2 = yEnd - yFirst;
				if (num == 0 && num2 == 0)
				{
					return 1;
				}
				if (num2 == 0 && yFirst % 24 == 0 && TileMap.tileTypeAt(xFirst, yFirst, 2))
				{
					return 2;
				}
				if (num2 > 0 && (yFirst % 24 != 0 || !TileMap.tileTypeAt(xFirst, yFirst, 2)))
				{
					return 4;
				}
				cvy = -10;
				cp1 = 0;
				cdir = ((num > 0) ? 1 : (-1));
				if (num <= 5)
				{
					cvx = 0;
				}
				else if (num <= 10)
				{
					cvx = 3;
				}
				else
				{
					cvx = 5;
				}
				return 9;
			}
	/// <summary>
	/// Truy xuất và trả về dữ liệu của getSoundVolumn.
	/// </summary>
	public float getSoundVolumn()
			{
				if (me)
				{
					return 0.1f;
				}
				int num = Res.abs(myChar.cx - cx);
				if (num >= 0 && num <= 50)
				{
					return 0.1f;
				}
				return 0.05f;
			}
	/// <summary>
	/// Dừng tiến trình hoặc luồng xử lý nền.
	/// </summary>
	private void stop()
			{
				statusMe = 6;
				cp3 = 0;
				cvx = 0;
				cvy = 0;
				cp1 = (cp2 = 0);
				if (me && (cx != cxSend || cy != cySend))
				{
					Service.gI().charMove();
				}
			}
	/// <summary>
	/// Thực thi phương thức nghiệp vụ abs.
	/// </summary>
	public static int abs(int i)
			{
				return (i <= 0) ? (-i) : i;
			}
	/// <summary>
	/// Thực thi phương thức nghiệp vụ skillInfoPaint.
	/// </summary>
	public SkillInfoPaint[] skillInfoPaint()
			{
				if (skillPaint == null || skillPaintRandomPaint == null)
				{
					return null;
				}
				if (sType == 1 && skillPaintRandomPaint.skillfly != null && skillPaintRandomPaint.skillfly.Length > 0)
				{
					return skillPaintRandomPaint.skillfly;
				}
				return skillPaintRandomPaint.skillStand;
			}

}

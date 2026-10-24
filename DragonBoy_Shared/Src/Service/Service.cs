using System;
using Assets.src.g;
using UnityEngine;

/// <summary>
/// Dịch vụ mạng gửi đi (Client -> Server): Đóng gói dữ liệu nhị phân thành Message gửi lên Game Server.
/// </summary>
public partial class Service
{
	private ISession session = Session_ME.gI();

	protected static Service instance;

	public static long curCheckController;

	public static long curCheckMap;

	public static long logController;

	public static long logMap;

	public int demGui;

	public static bool reciveFromMainSession;

	/// <summary>
	/// Thực thi phương thức nghiệp vụ gI.
	/// </summary>
	public static Service gI()
		{
			if (instance == null)
			{
				instance = new Service();
			}
			return instance;
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ charInfo.
	/// </summary>
	public void charInfo(string day, string month, string year, string address, string cmnd, string dayCmnd, string noiCapCmnd, string sdt, string name)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)42);
				message.writer().writeUTF(day);
				message.writer().writeUTF(month);
				message.writer().writeUTF(year);
				message.writer().writeUTF(address);
				message.writer().writeUTF(cmnd);
				message.writer().writeUTF(dayCmnd);
				message.writer().writeUTF(noiCapCmnd);
				message.writer().writeUTF(sdt);
				message.writer().writeUTF(name);
				session.sendMessage(message);
			}
			catch (Exception ex)
			{
				ex.StackTrace.ToString();
			}
			finally
			{
				message.cleanup();
			}
		}

	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của checkAd.
	/// </summary>
	public void checkAd(sbyte status)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)(-44));
				message.writer().writeByte(status);
				session.sendMessage(message);
			}
			catch (Exception ex)
			{
				ex.StackTrace.ToString();
			}
			finally
			{
				message.cleanup();
			}
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ test.
	/// </summary>
	public void test(short x, short y)
		{
			Res.outz("gui x= " + x + " y= " + y);
			Message message = null;
			try
			{
				message = new Message(0);
				message.writer().writeShort(x);
				message.writer().writeShort(y);
				session.sendMessage(message);
			}
			catch (Exception ex)
			{
				ex.StackTrace.ToString();
			}
			finally
			{
				message.cleanup();
			}
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ test2.
	/// </summary>
	public void test2()
		{
			Res.outz("gui test1");
			Message message = null;
			try
			{
				message = new Message(1);
				session.sendMessage(message);
			}
			catch (Exception ex)
			{
				ex.StackTrace.ToString();
			}
			finally
			{
				message.cleanup();
			}
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ testJoint.
	/// </summary>
	public void testJoint()
		{
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ mobCapcha.
	/// </summary>
	public void mobCapcha(char ch)
		{
			Res.outz("cap char c= " + ch);
			Message message = null;
			try
			{
				message = new Message((sbyte)(-85));
				message.writer().writeChar(ch);
				session.sendMessage(message);
			}
			catch (Exception)
			{
			}
			finally
			{
				message.cleanup();
			}
		}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của getArchivemnt.
	/// </summary>
	public void getArchivemnt(int index)
		{
			Res.outz("get ngoc");
			Message message = null;
			try
			{
				message = new Message((sbyte)(-76));
				message.writer().writeByte(index);
				session.sendMessage(message);
			}
			catch (Exception ex)
			{
				Cout.println(ex.Message + ex.StackTrace);
			}
			finally
			{
				message.cleanup();
			}
		}

	/// <summary>
	/// Cập nhật trạng thái logic của updateCaption theo chu kỳ khung hình.
	/// </summary>
	public void updateCaption(sbyte gender)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)(-41));
				message.writer().writeByte(gender);
				session.sendMessage(message);
			}
			catch (Exception ex)
			{
				Cout.println(ex.Message + ex.StackTrace);
			}
			finally
			{
				message.cleanup();
			}
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ messageNotMap.
	/// </summary>
	public Message messageNotMap(sbyte command)
		{
			Message message = new Message((sbyte)(-28));
			message.writer().writeByte(command);
			return message;
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ messageSubCommand.
	/// </summary>
	public static Message messageSubCommand(sbyte command)
		{
			Message message = new Message((sbyte)(-30));
			message.writer().writeByte(command);
			return message;
		}

	/// <summary>
	/// Đóng gói và gửi thông điệp mạng sendCheckController lên máy chủ.
	/// </summary>
	public void sendCheckController()
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)(-120));
				session.sendMessage(message);
			}
			catch (Exception)
			{
			}
			finally
			{
				curCheckController = mSystem.currentTimeMillis();
				message.cleanup();
			}
		}

	/// <summary>
	/// Đóng gói và gửi thông điệp mạng sendCheckMap lên máy chủ.
	/// </summary>
	public void sendCheckMap()
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)(-121));
				session.sendMessage(message);
			}
			catch (Exception)
			{
			}
			finally
			{
				curCheckMap = mSystem.currentTimeMillis();
				message.cleanup();
			}
		}

	public static long lastCharMoveTime = 0;

	/// <summary>
	/// Thực thi phương thức nghiệp vụ createChar.
	/// </summary>
	public void createChar(string name, int gender, int hair)
		{
			Message message = new Message((sbyte)(-28));
			try
			{
				message.writer().writeByte((sbyte)2);
				message.writer().writeUTF(name);
				message.writer().writeByte(gender);
				message.writer().writeByte(hair);
			}
			catch (Exception ex)
			{
				Cout.println(ex.Message + ex.StackTrace);
			}
			session.sendMessage(message);
		}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của getEffData.
	/// </summary>
	public void getEffData(short id)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)(-66));
				message.writer().writeShort(id);
				session.sendMessage(message);
			}
			catch (Exception ex)
			{
				Cout.println(ex.Message + ex.StackTrace);
			}
			finally
			{
				message.cleanup();
			}
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ confirmMenu.
	/// </summary>
	public void confirmMenu(short npcID, sbyte select)
		{
			Res.outz("confirme menu" + select);
			Message message = null;
			try
			{
				message = new Message((sbyte)32);
				message.writer().writeShort(npcID);
				message.writer().writeByte(select);
				session.sendMessage(message);
			}
			catch (Exception ex)
			{
				Cout.println(ex.Message + ex.StackTrace);
			}
			finally
			{
				message.cleanup();
			}
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ openMenu.
	/// </summary>
	public void openMenu(int npcId)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)33);
				message.writer().writeShort(npcId);
				session.sendMessage(message);
			}
			catch (Exception ex)
			{
				Cout.println(ex.Message + ex.StackTrace);
			}
			finally
			{
				message.cleanup();
			}
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ menu.
	/// </summary>
	public void menu(int npcId, int menuId, int optionId)
		{
			Cout.println("menuid: " + menuId);
			Message message = null;
			try
			{
				message = new Message((sbyte)22);
				message.writer().writeByte(npcId);
				message.writer().writeByte(menuId);
				message.writer().writeByte(optionId);
				session.sendMessage(message);
			}
			catch (Exception ex)
			{
				Cout.println(ex.Message + ex.StackTrace);
			}
			finally
			{
				message.cleanup();
			}
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ menuId.
	/// </summary>
	public void menuId(short menuId)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)27);
				message.writer().writeShort(menuId);
				session.sendMessage(message);
			}
			catch (Exception ex)
			{
				Cout.println(ex.Message + ex.StackTrace);
			}
			finally
			{
				message.cleanup();
			}
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ textBoxId.
	/// </summary>
	public void textBoxId(short menuId, string str)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)88);
				message.writer().writeShort(menuId);
				message.writer().writeUTF(str);
				session.sendMessage(message);
			}
			catch (Exception ex)
			{
				Cout.println(ex.Message + ex.StackTrace);
			}
			finally
			{
				message.cleanup();
			}
		}

	/// <summary>
	/// Tự động sắp xếp và dồn các vật phẩm cùng loại trong rương đồ cá nhân.
	/// </summary>
	public void boxSort()
		{
			Message message = null;
			try
			{
				message = messageSubCommand(19);
				session.sendMessage(message);
			}
			catch (Exception ex)
			{
				Cout.println(ex.Message + ex.StackTrace);
			}
			finally
			{
				message.cleanup();
			}
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ boxCoinOut.
	/// </summary>
	public void boxCoinOut(int coinOut)
		{
			Message message = null;
			try
			{
				message = messageSubCommand(21);
				message.writer().writeInt(coinOut);
				session.sendMessage(message);
			}
			catch (Exception ex)
			{
				Cout.println(ex.Message + ex.StackTrace);
			}
			finally
			{
				message.cleanup();
			}
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ returnTownFromDead.
	/// </summary>
	public void returnTownFromDead()
		{
			ModTdltLogger.Log("RETURN_TOWN_SEND", $"Packet -15 sent -> Map='{TileMap.mapName}' (ID:{TileMap.mapID}, Zone:{TileMap.zoneID}), Pos=({Char.myCharz()?.cx},{Char.myCharz()?.cy}), HP={Char.myCharz()?.cHP}/{Char.myCharz()?.cHPFull}");
			Message message = null;
			try
			{
				message = new Message((sbyte)(-15));
				session.sendMessage(message);
			}
			catch (Exception ex)
			{
				Cout.println(ex.Message + ex.StackTrace);
			}
			finally
			{
				message.cleanup();
			}
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ wakeUpFromDead.
	/// </summary>
	public void wakeUpFromDead()
		{
			ModTdltLogger.Log("WAKEUP_SEND", $"Packet -16 sent -> Map='{TileMap.mapName}' (ID:{TileMap.mapID}, Zone:{TileMap.zoneID}), Pos=({Char.myCharz()?.cx},{Char.myCharz()?.cy}), HP={Char.myCharz()?.cHP}/{Char.myCharz()?.cHPFull}");
			Message message = null;
			try
			{
				message = new Message((sbyte)(-16));
				session.sendMessage(message);
			}
			catch (Exception ex)
			{
				Cout.println(ex.Message + ex.StackTrace);
			}
			finally
			{
				message.cleanup();
			}
		}

	/// <summary>
	/// Cập nhật trạng thái logic của updateData theo chu kỳ khung hình.
	/// </summary>
	public void updateData()
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)(-87));
				if (Session_ME2.gI().isConnected() && !Session_ME2.connecting)
				{
					session = Session_ME2.gI();
				}
				else
				{
					session = Session_ME.gI();
				}
				session.sendMessage(message);
				session = Session_ME.gI();
			}
			catch (Exception ex)
			{
				Cout.println(ex.Message + ex.StackTrace);
			}
			finally
			{
				message.cleanup();
			}
		}

	/// <summary>
	/// Cập nhật trạng thái logic của updateMap theo chu kỳ khung hình.
	/// </summary>
	public void updateMap()
		{
			Message message = null;
			try
			{
				message = messageNotMap(6);
				if (Session_ME2.gI().isConnected() && !Session_ME2.connecting)
				{
					session = Session_ME2.gI();
				}
				else
				{
					session = Session_ME.gI();
				}
				session.sendMessage(message);
				session = Session_ME.gI();
			}
			catch (Exception ex)
			{
				Cout.println(ex.Message + ex.StackTrace);
			}
			finally
			{
				message.cleanup();
			}
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ testInvite.
	/// </summary>
	public void testInvite(int charId)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)59);
				message.writer().writeInt(charId);
				session.sendMessage(message);
			}
			catch (Exception ex)
			{
				Cout.println(ex.Message + ex.StackTrace);
			}
			finally
			{
				message.cleanup();
			}
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ addCuuSat.
	/// </summary>
	public void addCuuSat(int charId)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)62);
				message.writer().writeInt(charId);
				session.sendMessage(message);
			}
			catch (Exception ex)
			{
				Cout.println(ex.Message + ex.StackTrace);
			}
			finally
			{
				message.cleanup();
			}
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ finishUpdate.
	/// </summary>
	public void finishUpdate()
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)(-38));
				session.sendMessage(message);
			}
			catch (Exception ex)
			{
				Cout.println(ex.Message + ex.StackTrace);
			}
			finally
			{
				message.cleanup();
			}
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ finishUpdate.
	/// </summary>
	public void finishUpdate(int playerID)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)(-38));
				message.writer().writeInt(playerID);
				session.sendMessage(message);
			}
			catch (Exception)
			{
			}
			finally
			{
				message.cleanup();
			}
		}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của getResource.
	/// </summary>
	public void getResource(sbyte action, MyVector vResourceIndex)
		{
			Res.outz("request resource action= " + action);
			Message message = null;
			try
			{
				message = new Message((sbyte)(-74));
				message.writer().writeByte(action);
				if (action == 2 && vResourceIndex != null)
				{
					message.writer().writeShort(vResourceIndex.size());
					for (int i = 0; i < vResourceIndex.size(); i++)
					{
						message.writer().writeShort(short.Parse((string)vResourceIndex.elementAt(i)));
					}
				}
				if (Session_ME2.gI().isConnected() && !Session_ME2.connecting)
				{
					session = Session_ME2.gI();
				}
				else
				{
					reciveFromMainSession = true;
					session = Session_ME.gI();
				}
				session.sendMessage(message);
				session = Session_ME.gI();
			}
			catch (Exception ex)
			{
				Cout.println(ex.Message + ex.StackTrace);
			}
			finally
			{
				message.cleanup();
			}
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ imageSource.
	/// </summary>
	public void imageSource(MyVector vID)
		{
			Message message = null;
			try
			{
				Res.outz("IMAGE SOURCE size= " + vID.size());
				message = new Message((sbyte)(-111));
				message.writer().writeShort(vID.size());
				if (vID.size() > 0)
				{
					for (int i = 0; i < vID.size(); i++)
					{
						Res.outz("gui len str " + ((ImageSource)vID.elementAt(i)).id);
						message.writer().writeUTF(((ImageSource)vID.elementAt(i)).id);
					}
				}
				if (Session_ME2.gI().isConnected() && !Session_ME2.connecting)
				{
					session = Session_ME2.gI();
				}
				else
				{
					session = Session_ME.gI();
					reciveFromMainSession = true;
				}
				session.sendMessage(message);
				session = Session_ME.gI();
			}
			catch (Exception ex)
			{
				ex.StackTrace.ToString();
			}
			finally
			{
				message.cleanup();
			}
		}

	/// <summary>
	/// Đóng gói và gửi thông điệp mạng sendServerData lên máy chủ.
	/// </summary>
	public void sendServerData(sbyte action, int id, sbyte[] data)
		{
			Message message = null;
			try
			{
				Res.outz("SERVER DATA");
				message = new Message((sbyte)(-110));
				message.writer().writeByte(action);
				if (action == 1)
				{
					message.writer().writeInt(id);
					if (data != null)
					{
						int num = data.Length;
						message.writer().writeShort(num);
						message.writer().write(ref data, 0, num);
					}
				}
				session.sendMessage(message);
			}
			catch (Exception)
			{
			}
			finally
			{
				message.cleanup();
			}
		}

	/// <summary>
	/// Đóng gói và gửi thông điệp mạng sendCmdExtra lên máy chủ.
	/// </summary>
	public void sendCmdExtra(sbyte sub, string user, string pass)
		{
			Message message = new Message((sbyte)24);
			try
			{
				message.writer().writeByte(sub);
				if (sub == sbyte.MaxValue)
				{
					message.writer().writeUTF(user);
					message.writer().writeUTF(pass);
					Controller.isEXTRA_LINK = false;
					Res.err(" =====> SEND EXTRA_LINK " + sub + " user:" + user + " pass:" + pass);
				}
				session.sendMessage(message);
			}
			catch (Exception)
			{
			}
			finally
			{
				message.cleanup();
			}
		}

}

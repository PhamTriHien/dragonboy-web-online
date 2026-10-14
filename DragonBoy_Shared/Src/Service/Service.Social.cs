using System;
using Assets.src.g;
using UnityEngine;

/// <summary>
/// Thành phần phân mảnh (partial) của lớp Service: Đảm nhiệm phân hệ chuyên trách Social ().
/// </summary>
public partial class Service
{
	/// <summary>
	/// Thực thi phương thức nghiệp vụ giaodich.
	/// </summary>
	public void giaodich(sbyte action, int playerID, sbyte index, int num)
		{
			Res.outz2("giao dich action = " + action);
			Message message = null;
			try
			{
				message = new Message((sbyte)(-86));
				message.writer().writeByte(action);
				if (action == 0 || action == 1)
				{
					Res.outz2(">>>> len playerID =" + playerID);
					message.writer().writeInt(playerID);
				}
				if (action == 2)
				{
					Res.outz2("gui len index =" + index + " num= " + num);
					message.writer().writeByte(index);
					message.writer().writeInt(num);
				}
				if (action == 4)
				{
					Res.outz2(">>>> len index =" + index);
					message.writer().writeByte(index);
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
	/// Đóng gói và gửi thông điệp mạng sendClientInput lên máy chủ.
	/// </summary>
	public void sendClientInput(TField[] t)
		{
			Message message = null;
			try
			{
				Res.outz(" gui input ");
				message = new Message((sbyte)(-125));
				Res.outz("byte lent = " + t.Length);
				message.writer().writeByte(t.Length);
				for (int i = 0; i < t.Length; i++)
				{
					message.writer().writeUTF(t[i].getText());
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
	/// Thực thi phương thức nghiệp vụ friend.
	/// </summary>
	public void friend(sbyte action, int playerId)
		{
			Res.outz("add friend");
			Message message = null;
			try
			{
				message = new Message((sbyte)(-80));
				message.writer().writeByte(action);
				if (playerId != -1)
				{
					message.writer().writeInt(playerId);
				}
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
	/// Truy xuất và trả về dữ liệu của getPlayerMenu.
	/// </summary>
	public void getPlayerMenu(int playerID)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)(-79));
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
	/// Thực thi phương thức nghiệp vụ clanDonate.
	/// </summary>
	public void clanDonate(int id)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)(-54));
				message.writer().writeInt(id);
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
	/// Thực thi phương thức nghiệp vụ clanMessage.
	/// </summary>
	public void clanMessage(int type, string text, int clanID)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)(-51));
				message.writer().writeByte(type);
				if (type == 0)
				{
					message.writer().writeUTF(text);
				}
				if (type == 2)
				{
					message.writer().writeInt(clanID);
				}
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
	/// Thực thi phương thức nghiệp vụ joinClan.
	/// </summary>
	public void joinClan(int id, sbyte action)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)(-49));
				message.writer().writeInt(id);
				message.writer().writeByte(action);
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
	/// Thực thi phương thức nghiệp vụ clanMember.
	/// </summary>
	public void clanMember(int id)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)(-50));
				message.writer().writeInt(id);
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
	/// Thực thi phương thức nghiệp vụ searchClan.
	/// </summary>
	public void searchClan(string text)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)(-47));
				message.writer().writeUTF(text);
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
	/// Thực thi phương thức nghiệp vụ clanRemote.
	/// </summary>
	public void clanRemote(int id, sbyte role)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)(-56));
				message.writer().writeInt(id);
				message.writer().writeByte(role);
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
	/// Thực thi phương thức nghiệp vụ leaveClan.
	/// </summary>
	public void leaveClan()
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)(-55));
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
	/// Thực thi phương thức nghiệp vụ clanInvite.
	/// </summary>
	public void clanInvite(sbyte action, int playerID, int clanID, int code)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)(-57));
				message.writer().writeByte(action);
				if (action == 0)
				{
					message.writer().writeInt(playerID);
				}
				if (action == 1 || action == 2)
				{
					message.writer().writeInt(clanID);
					message.writer().writeInt(code);
				}
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
	/// Truy xuất và trả về dữ liệu của getClan.
	/// </summary>
	public void getClan(sbyte action, int id, string text)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)(-46));
				message.writer().writeByte(action);
				if (action == 2 || action == 4)
				{
					message.writer().writeShort((short)id);
					message.writer().writeUTF(text);
				}
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
	/// Thực thi phương thức nghiệp vụ selectCharToPlay.
	/// </summary>
	public void selectCharToPlay(string charname)
		{
			Message message = new Message((sbyte)(-28));
			try
			{
				message.writer().writeByte((sbyte)1);
				message.writer().writeUTF(charname);
			}
			catch (Exception ex)
			{
				Cout.println(ex.Message + ex.StackTrace);
			}
			session.sendMessage(message);
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ acceptInviteTrade.
	/// </summary>
	public void acceptInviteTrade(int playerMapId)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)37);
				message.writer().writeInt(playerMapId);
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
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của cancelInviteTrade.
	/// </summary>
	public void cancelInviteTrade()
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)50);
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
	/// Thực thi phương thức nghiệp vụ tradeAccept.
	/// </summary>
	public void tradeAccept()
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)39);
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
	/// Thực thi phương thức nghiệp vụ tradeInvite.
	/// </summary>
	public void tradeInvite(int charId)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)36);
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
	/// Thực thi phương thức nghiệp vụ addFriend.
	/// </summary>
	public void addFriend(string name)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)53);
				message.writer().writeUTF(name);
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
	/// Thực thi phương thức nghiệp vụ addPartyAccept.
	/// </summary>
	public void addPartyAccept(int charId)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)76);
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
	/// Thực thi phương thức nghiệp vụ addPartyCancel.
	/// </summary>
	public void addPartyCancel(int charId)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)77);
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
	/// Thực thi phương thức nghiệp vụ addParty.
	/// </summary>
	public void addParty(string name)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)75);
				message.writer().writeUTF(name);
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
	/// Thực thi phương thức nghiệp vụ player_vs_player.
	/// </summary>
	public void player_vs_player(sbyte action, sbyte type, int playerId)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)(-59));
				message.writer().writeByte(action);
				message.writer().writeByte(type);
				message.writer().writeInt(playerId);
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
	/// Thực thi phương thức nghiệp vụ outParty.
	/// </summary>
	public void outParty()
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)79);
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
	/// Thực thi phương thức nghiệp vụ pleaseInputParty.
	/// </summary>
	public void pleaseInputParty(string str)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)16);
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
	/// Thực thi phương thức nghiệp vụ acceptPleaseParty.
	/// </summary>
	public void acceptPleaseParty(string str)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)17);
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
	/// Đóng gói và gửi thông điệp mạng sendCardInfo lên máy chủ.
	/// </summary>
	public void sendCardInfo(string NAP, string PIN)
		{
			Message message = null;
			try
			{
				message = messageNotMap(16);
				message.writer().writeUTF(NAP);
				message.writer().writeUTF(PIN);
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
	/// Thực thi phương thức nghiệp vụ saveRms.
	/// </summary>
	public void saveRms(string key, sbyte[] data)
		{
			Message message = null;
			try
			{
				message = messageSubCommand(60);
				message.writer().writeUTF(key);
				message.writer().writeInt(data.Length);
				message.writer().write(data);
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
	/// Khởi tạo và nạp dữ liệu cấu hình cho loadRMS.
	/// </summary>
	public void loadRMS(string key)
		{
			Cout.println("REQUEST RMS");
			Message message = null;
			try
			{
				message = messageSubCommand(61);
				message.writer().writeUTF(key);
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
	/// Thực thi phương thức nghiệp vụ changeName.
	/// </summary>
	public void changeName(string name, int id)
		{
			Message message = null;
			try
			{
				message = messageNotMap(18);
				message.writer().writeInt(id);
				message.writer().writeUTF(name);
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
	/// Thực thi phương thức nghiệp vụ inviteClanDun.
	/// </summary>
	public void inviteClanDun(string name)
		{
			Message message = null;
			try
			{
				message = messageNotMap(34);
				message.writer().writeUTF(name);
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
	/// Thực thi phương thức nghiệp vụ activeAccProtect.
	/// </summary>
	public void activeAccProtect(int pass)
		{
			Message message = null;
			try
			{
				message = messageNotMap(37);
				message.writer().writeInt(pass);
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
	/// Dọn dẹp và đặt lại trạng thái ban đầu cho clearAccProtect.
	/// </summary>
	public void clearAccProtect(int pass)
		{
			Message message = null;
			try
			{
				message = messageNotMap(41);
				message.writer().writeInt(pass);
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
	/// Cập nhật trạng thái logic của updateActive theo chu kỳ khung hình.
	/// </summary>
	public void updateActive(int passOld, int passNew)
		{
			Message message = null;
			try
			{
				message = messageNotMap(38);
				message.writer().writeInt(passOld);
				message.writer().writeInt(passNew);
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
	/// Thực thi phương thức nghiệp vụ openLockAccProtect.
	/// </summary>
	public void openLockAccProtect(int pass2)
		{
			Message message = null;
			try
			{
				message = messageNotMap(39);
				message.writer().writeInt(pass2);
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
	/// Đóng gói và gửi thông điệp mạng sendTop lên máy chủ.
	/// </summary>
	public void sendTop(string topName, sbyte selected)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)(-96));
				message.writer().writeUTF(topName);
				message.writer().writeByte(selected);
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
	/// Thực thi phương thức nghiệp vụ enemy.
	/// </summary>
	public void enemy(sbyte b, int charID)
		{
			Message message = null;
			Res.outz("add enemy");
			try
			{
				message = new Message((sbyte)(-99));
				message.writer().writeByte(b);
				if (b == 1 || b == 2)
				{
					message.writer().writeInt(charID);
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
	/// Truy xuất và trả về dữ liệu của getFlag.
	/// </summary>
	public void getFlag(sbyte action, sbyte flagType)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)(-103));
				message.writer().writeByte(action);
				Res.outz("------------service--  " + action + "   " + flagType);
				if (action != 0)
				{
					message.writer().writeByte(flagType);
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
	/// Thực thi phương thức nghiệp vụ messagePlayerMenu.
	/// </summary>
	public void messagePlayerMenu(int charId)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)(-30));
				message.writer().writeByte((sbyte)63);
				message.writer().writeInt(charId);
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
	/// Thực thi phương thức nghiệp vụ playerMenuAction.
	/// </summary>
	public void playerMenuAction(int charId, short select)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)(-30));
				message.writer().writeByte((sbyte)64);
				message.writer().writeInt(charId);
				message.writer().writeShort(select);
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
	/// Đóng gói và gửi thông điệp mạng sendDelAcc lên máy chủ.
	/// </summary>
	public void sendDelAcc()
		{
			Message message = new Message((sbyte)69);
			try
			{
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

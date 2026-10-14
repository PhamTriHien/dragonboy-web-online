using System;
using Assets.src.g;
using UnityEngine;

/// <summary>
/// Thành phần phân mảnh (partial) của lớp Service: Đảm nhiệm phân hệ chuyên trách Quest ().
/// </summary>
public partial class Service
{
	/// <summary>
	/// Thực thi phương thức nghiệp vụ requestClan.
	/// </summary>
	public void requestClan(short id)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)(-53));
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
	/// Truy xuất và trả về dữ liệu của getTask.
	/// </summary>
	public void getTask(int npcTemplateId, int menuId, int optionId)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)40);
				message.writer().writeByte(npcTemplateId);
				message.writer().writeByte(menuId);
				if (optionId >= 0)
				{
					message.writer().writeByte(optionId);
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
	/// Thực thi phương thức nghiệp vụ requestRegister.
	/// </summary>
	public void requestRegister(string username, string pass, string usernameAo, string passAo, string version)
		{
			try
			{
				Message message = messageNotLogin(1);
				message.writer().writeUTF(username);
				message.writer().writeUTF(pass);
				if (usernameAo != null && !usernameAo.Equals(string.Empty))
				{
					message.writer().writeUTF(usernameAo);
					message.writer().writeUTF("a");
				}
				session.sendMessage(message);
				message.cleanup();
			}
			catch (Exception ex)
			{
				Cout.println(ex.Message + ex.StackTrace);
			}
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ requestModTemplate.
	/// </summary>
	public void requestModTemplate(int modTemplateId)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)11);
				message.writer().writeShort(modTemplateId);
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
	/// Thực thi phương thức nghiệp vụ requestPlayerInfo.
	/// </summary>
	public void requestPlayerInfo(MyVector chars)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)18);
				message.writer().writeByte(chars.size());
				for (int i = 0; i < chars.size(); i++)
				{
					Char @char = (Char)chars.elementAt(i);
					message.writer().writeInt(@char.charID);
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
	/// Dọn dẹp và đặt lại trạng thái ban đầu cho clearTask.
	/// </summary>
	public void clearTask()
		{
			Message message = null;
			try
			{
				message = messageNotMap(17);
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
	/// Thực thi phương thức nghiệp vụ requestMapSelect.
	/// </summary>
	public void requestMapSelect(int selected)
		{
			Res.outz("request magic tree");
			Message message = null;
			try
			{
				message = new Message((sbyte)(-91));
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

}

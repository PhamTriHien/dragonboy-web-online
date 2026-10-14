using System;
using Assets.src.g;

namespace Assets.src.f;

/// <summary>
/// CLASS Controller2: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
public partial class Controller2
{
	/// <summary>
	/// Thực thi phương thức nghiệp vụ readMessage.
	/// </summary>
	public static void readMessage(Message msg)
		{
			try
			{
				if (readMessage_Part1(msg.command, msg)) return;
				if (readMessage_Part2(msg.command, msg)) return;
			}
			catch (Exception ex4)
			{
				Res.outz("=====> Controller2 " + ex4.StackTrace);
			}
		}

}

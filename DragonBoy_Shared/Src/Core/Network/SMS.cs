using System;
using System.Threading;
using UnityEngine;

/// <summary>
/// CLASS SMS: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
public class SMS
{
	private const int INTERVAL = 5;

	private const int MAXTIME = 500;

	private static int status;

	private static int _result;

	private static string _to;

	private static string _content;

	private static bool f;

	private static int time;

	public static bool sendEnable;

	private static int time0;

	/// <summary>
	/// Đóng gói và gửi thông điệp mạng send lên máy chủ.
	/// </summary>
	public static int send(string content, string to)
	{
		return __send(content, to);
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ _send.
	/// </summary>
	private static int _send(string content, string to)
	{
		if (status != 0)
		{
			for (int i = 0; i < 500; i++)
			{
				Thread.Sleep(5);
				if (status == 0)
				{
					break;
				}
			}
			if (status != 0)
			{
				Cout.LogError("CANNOT SEND SMS " + content + " WHEN SENDING " + _content);
				return -1;
			}
		}
		_content = content;
		_to = to;
		_result = -1;
		status = 2;
		int j;
		for (j = 0; j < 500; j++)
		{
			Thread.Sleep(5);
			if (status == 0)
			{
				break;
			}
		}
		if (j == 500)
		{
			Debug.LogError("TOO LONG FOR SEND SMS " + content);
			status = 0;
		}
		else
		{
			Debug.Log("Send SMS " + content + " done in " + j * 5 + "ms");
		}
		return _result;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ __send.
	/// </summary>
	private static int __send(string content, string to)
	{
		int num = iOSPlugins.Check();
		Cout.println("vao sms ko " + num);
		if (num >= 0)
		{
			f = true;
			sendEnable = true;
			iOSPlugins.SMSsend(to, content, num);
			Screen.orientation = ScreenOrientation.AutoRotation;
		}
		return num;
	}

	/// <summary>
	/// Cập nhật trạng thái logic của đối tượng theo từng chu kỳ khung hình.
	/// </summary>
	public static void update()
	{
		float num = Time.time;
		if (num - (float)time > 1f)
		{
			time++;
		}
		if (f)
		{
			OnSMS();
		}
		if (status == 2)
		{
			status = 1;
			try
			{
				_result = __send(_content, _to);
			}
			catch (Exception)
			{
				Debug.Log("CANNOT SEND SMS");
			}
			status = 0;
		}
	}

	/// <summary>
	/// Bộ xử lý sự kiện tiếp nhận và phản hồi OnSMS.
	/// </summary>
	private static void OnSMS()
	{
		if (sendEnable)
		{
			if (iOSPlugins.checkRotation() == 1)
			{
				Screen.orientation = ScreenOrientation.LandscapeLeft;
			}
			else if (iOSPlugins.checkRotation() == -1)
			{
				Screen.orientation = ScreenOrientation.Portrait;
			}
			else if (iOSPlugins.checkRotation() == 0)
			{
				Screen.orientation = ScreenOrientation.AutoRotation;
			}
			else if (iOSPlugins.checkRotation() == 2)
			{
				Screen.orientation = ScreenOrientation.LandscapeRight;
			}
			else if (iOSPlugins.checkRotation() == 3)
			{
				Screen.orientation = ScreenOrientation.PortraitUpsideDown;
			}
			if (time0 < 5)
			{
				time0++;
			}
			else
			{
				iOSPlugins.Send();
				sendEnable = false;
				time0 = 0;
			}
		}
		if (iOSPlugins.unpause() == 1)
		{
			Screen.orientation = ScreenOrientation.LandscapeLeft;
			if (time0 < 5)
			{
				time0++;
				return;
			}
			f = false;
			iOSPlugins.back();
			time0 = 0;
		}
	}
}

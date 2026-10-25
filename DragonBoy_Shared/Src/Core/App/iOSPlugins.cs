using System.Runtime.InteropServices;
using UnityEngine;

/// <summary>
/// CLASS iOSPlugins: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
public class iOSPlugins
{
	public static string devide;

	public static string Myname;

#if BROWSER
	private static void _SMSsend(string tophone, string withtext, int n) { }
	private static int _unpause() => 0;
	private static int _checkRotation() => 0;
	private static int _back() => 0;
	private static int _Send() => 0;
	private static void _purchaseItem(string itemID, string userName, string gameID) { }
#else

	[DllImport("__Internal")]
	private static extern void _SMSsend(string tophone, string withtext, int n);

	[DllImport("__Internal")]
	private static extern int _unpause();

	[DllImport("__Internal")]
	private static extern int _checkRotation();

	[DllImport("__Internal")]
	private static extern int _back();

	[DllImport("__Internal")]
	private static extern int _Send();

	[DllImport("__Internal")]
	private static extern void _purchaseItem(string itemID, string userName, string gameID);
#endif

	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của Check.
	/// </summary>
	public static int Check()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			return checkCanSendSMS();
		}
		devide = iPhoneSettings.generation.ToString();
		string text = string.Empty + devide[2];
		if (text == "h" && devide.Length > 6)
		{
			Myname = SystemInfo.operatingSystem.ToString();
			string text2 = string.Empty + Myname[10];
			if (text2 != "2" && text2 != "3")
			{
				return 0;
			}
			return 1;
		}
		Cout.println(devide + "  loai");
		if (devide == "Unknown" && ScaleGUI.WIDTH * ScaleGUI.HEIGHT < 786432f)
		{
			return 0;
		}
		return -1;
	}

	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của checkCanSendSMS.
	/// </summary>
	public static int checkCanSendSMS()
	{
		if (iPhoneSettings.generation == iPhoneGeneration.iPhone3GS || iPhoneSettings.generation == iPhoneGeneration.iPhone4 || iPhoneSettings.generation == iPhoneGeneration.iPhone4S || iPhoneSettings.generation == iPhoneGeneration.iPhone5)
		{
			return 0;
		}
		return -1;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ SMSsend.
	/// </summary>
	public static void SMSsend(string phonenumber, string bodytext, int n)
	{
		if (Application.platform != 0)
		{
			_SMSsend(phonenumber, bodytext, n);
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ back.
	/// </summary>
	public static void back()
	{
		if (Application.platform != 0)
		{
			_back();
		}
	}

	/// <summary>
	/// Đóng gói và gửi thông điệp mạng Send lên máy chủ.
	/// </summary>
	public static void Send()
	{
		if (Application.platform != 0)
		{
			_Send();
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ unpause.
	/// </summary>
	public static int unpause()
	{
		if (Application.platform != 0)
		{
			return _unpause();
		}
		return 0;
	}

	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của checkRotation.
	/// </summary>
	public static int checkRotation()
	{
		if (Application.platform != 0)
		{
			return _checkRotation();
		}
		return 0;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ purchaseItem.
	/// </summary>
	public static void purchaseItem(string itemID, string userName, string gameID)
	{
		if (Application.platform != 0)
		{
			_purchaseItem(itemID, userName, gameID);
		}
	}
}

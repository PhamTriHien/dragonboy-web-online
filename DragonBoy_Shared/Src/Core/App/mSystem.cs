using System;
using System.Text;
using UnityEngine;

/// <summary>
/// Lớp cầu nối hệ thống: Cung cấp các hàm kiểm tra nền tảng phần cứng, rung, mở trình duyệt và xử lý chuỗi ký tự.
/// </summary>
public class mSystem
{
	public static bool isTest;

	public static string strAdmob;

	public static bool loadAdOk;

	public static string publicID;

	public static string android_pack;

	public static int clientType = 4;

	public static sbyte LANGUAGE;

	public static sbyte curINAPP;

	public static sbyte maxINAPP = 5;

	public const int JAVA = 1;

	public const int ANDROID = 2;

	public const int IP_JB = 3;

	public const int PC = 4;

	public const int IP_APPSTORE = 5;

	public const int WINDOWS_PHONE = 6;

	public const int GOOGLE_PLAY = 7;

	public static mSystem instance;

	internal static bool isANDROID;

	/// <summary>
	/// Thực thi phương thức nghiệp vụ AddIpTest.
	/// </summary>
	public static void AddIpTest()
	{
		ModProxyServer.InjectCustomServer();
	}

	/// <summary>
	/// Dọn dẹp và đặt lại trạng thái ban đầu cho resetCurInapp.
	/// </summary>
	public static void resetCurInapp()
	{
		curINAPP = 0;
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của getWidth.
	/// </summary>
	public static int getWidth(Image img)
	{
		if (clientType == 5)
		{
			return img.getWidth();
		}
		return img.getWidth();
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của getHeight.
	/// </summary>
	public static int getHeight(Image img)
	{
		if (clientType == 5)
		{
			return img.getHeight();
		}
		return img.getWidth();
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của getTimeCountDown.
	/// </summary>
	public static string getTimeCountDown(long timeStart, int secondCount, bool isOnlySecond, bool isShortText)
	{
		string result = string.Empty;
		long num = (timeStart + secondCount * 1000 - currentTimeMillis()) / 1000;
		if (num <= 0)
		{
			return string.Empty;
		}
		long num2 = 0L;
		long num3 = 0L;
		long num4 = num / 60;
		long num5 = num;
		if (isOnlySecond)
		{
			return num5 + string.Empty;
		}
		if (num >= 86400)
		{
			num2 = num / 86400;
			num3 = num % 86400 / 3600;
		}
		else if (num >= 3600)
		{
			num3 = num / 3600;
			num4 = num % 3600 / 60;
		}
		else if (num >= 60)
		{
			num4 = num / 60;
			num5 = num % 60;
		}
		else
		{
			num5 = num;
		}
		if (isShortText)
		{
			if (num2 > 0)
			{
				return num2 + "d";
			}
			if (num3 > 0)
			{
				return num3 + "h";
			}
			if (num4 > 0)
			{
				return num4 + "m";
			}
			if (num5 > 0)
			{
				return num5 + "s";
			}
		}
		if (num2 > 0)
		{
			if (num2 >= 10)
			{
				result = ((num3 < 1) ? (num2 + "d") : ((num3 >= 10) ? (num2 + "d" + num3 + "h") : (num2 + "d0" + num3 + "h")));
			}
			else if (num2 < 10)
			{
				result = ((num3 < 1) ? (num2 + "d") : ((num3 >= 10) ? (num2 + "d" + num3 + "h") : (num2 + "d0" + num3 + "h")));
			}
		}
		else if (num3 > 0)
		{
			if (num3 >= 10)
			{
				result = ((num4 < 1) ? (num3 + "h") : ((num4 >= 10) ? (num3 + "h" + num4 + "m") : (num3 + "h0" + num4 + "m")));
			}
			else if (num3 < 10)
			{
				result = ((num4 < 1) ? (num3 + "h") : ((num4 >= 10) ? (num3 + "h" + num4 + "m") : (num3 + "h0" + num4 + "m")));
			}
		}
		else if (num4 > 0)
		{
			if (num4 >= 10)
			{
				if (num5 >= 10)
				{
					result = num4 + "m" + num5 + string.Empty;
				}
				else if (num5 < 10)
				{
					result = num4 + "m0" + num5 + string.Empty;
				}
			}
			else if (num4 < 10)
			{
				if (num5 >= 10)
				{
					result = num4 + "m" + num5 + string.Empty;
				}
				else if (num5 < 10)
				{
					result = num4 + "m0" + num5 + string.Empty;
				}
			}
		}
		else
		{
			result = ((num5 >= 10) ? (num5 + string.Empty) : ("0" + num5 + string.Empty));
		}
		return result;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ numberTostring2.
	/// </summary>
	public static string numberTostring2(int aa)
	{
		try
		{
			string text = string.Empty;
			string text2 = string.Empty;
			string text3 = aa + string.Empty;
			if (text3.Equals(string.Empty))
			{
				return text;
			}
			if (text3[0] == '-')
			{
				text2 = "-";
				text3 = text3.Substring(1);
			}
			for (int num = text3.Length - 1; num >= 0; num--)
			{
				text = (((text3.Length - 1 - num) % 3 != 0 || text3.Length - 1 - num <= 0) ? (text3[num] + text) : (text3[num] + "." + text));
			}
			return text2 + text;
		}
		catch (Exception)
		{
			return aa + string.Empty;
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ numberTostring.
	/// </summary>
	public static string numberTostring(long number)
	{
		string text = string.Empty + number;
		bool flag = false;
		try
		{
			string empty = string.Empty;
			if (number < 0)
			{
				flag = true;
				number = -number;
				text = string.Empty + number;
			}
			int num = 0;
			if (number >= 1000000000)
			{
				empty = "b";
				number /= 1000000000;
				num = (string.Empty + number).Length;
			}
			else if (number >= 1000000)
			{
				empty = "m";
				number /= 1000000;
				num = (string.Empty + number).Length;
			}
			else
			{
				if (number < 1000)
				{
					if (flag)
					{
						return "-" + text;
					}
					return text;
				}
				empty = "k";
				number /= 1000;
				num = (string.Empty + number).Length;
			}
			int num2 = int.Parse(text.Substring(num, 2));
			text = ((num2 == 0) ? (text.Substring(0, num) + empty) : ((num2 % 10 != 0) ? (text.Substring(0, num) + "," + text.Substring(num, 2) + empty) : (text.Substring(0, num) + "," + text.Substring(num, 1) + empty)));
		}
		catch (Exception)
		{
		}
		if (flag)
		{
			return "-" + text;
		}
		return text;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ callHotlinePC.
	/// </summary>
	public static void callHotlinePC()
	{
		Application.OpenURL("http://ngocrongonline.com/");
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ callHotlineJava.
	/// </summary>
	public static void callHotlineJava()
	{
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ callHotlineIphone.
	/// </summary>
	public static void callHotlineIphone()
	{
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ callHotlineWindowsPhone.
	/// </summary>
	public static void callHotlineWindowsPhone()
	{
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ closeBanner.
	/// </summary>
	public static void closeBanner()
	{
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ showBanner.
	/// </summary>
	public static void showBanner()
	{
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ createAdmob.
	/// </summary>
	public static void createAdmob()
	{
	}

	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của checkAdComlete.
	/// </summary>
	public static void checkAdComlete()
	{
	}

	/// <summary>
	/// Thực hiện vẽ và hiển thị thành phần paintPopUp2 lên canvas đồ họa.
	/// </summary>
	public static void paintPopUp2(mGraphics g, int x, int y, int w, int h)
	{
		g.fillRect(x, y, w + 10, h, 0, 90);
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ arraycopy.
	/// </summary>
	public static void arraycopy(sbyte[] scr, int scrPos, sbyte[] dest, int destPos, int lenght)
	{
		Array.Copy(scr, scrPos, dest, destPos, lenght);
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ arrayReplace.
	/// </summary>
	public static void arrayReplace(sbyte[] scr, int scrPos, ref sbyte[] dest, int destPos, int lenght)
	{
		if (scr != null && dest != null && scrPos + lenght <= scr.Length)
		{
			sbyte[] array = new sbyte[dest.Length + lenght];
			for (int i = 0; i < destPos; i++)
			{
				array[i] = dest[i];
			}
			for (int j = destPos; j < destPos + lenght; j++)
			{
				array[j] = scr[scrPos + j - destPos];
			}
			for (int k = destPos + lenght; k < array.Length; k++)
			{
				array[k] = dest[destPos + k - lenght];
			}
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ currentTimeMillis.
	/// </summary>
	public static long currentTimeMillis()
	{
		DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
		return (DateTime.UtcNow.Ticks - dateTime.Ticks) / 10000;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ freeData.
	/// </summary>
	public static void freeData()
	{
		Resources.UnloadUnusedAssets();
		GC.Collect();
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ convertToSbyte.
	/// </summary>
	public static sbyte[] convertToSbyte(byte[] scr)
	{
		sbyte[] array = new sbyte[scr.Length];
		for (int i = 0; i < scr.Length; i++)
		{
			array[i] = (sbyte)scr[i];
		}
		return array;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ convertToSbyte.
	/// </summary>
	public static sbyte[] convertToSbyte(string scr)
	{
		ASCIIEncoding aSCIIEncoding = new ASCIIEncoding();
		byte[] bytes = aSCIIEncoding.GetBytes(scr);
		return convertToSbyte(bytes);
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ convetToByte.
	/// </summary>
	public static byte[] convetToByte(sbyte[] scr)
	{
		byte[] array = new byte[scr.Length];
		for (int i = 0; i < scr.Length; i++)
		{
			if (scr[i] > 0)
			{
				array[i] = (byte)scr[i];
			}
			else
			{
				array[i] = (byte)(scr[i] + 256);
			}
		}
		return array;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ ToCharArray.
	/// </summary>
	public static char[] ToCharArray(sbyte[] scr)
	{
		char[] array = new char[scr.Length];
		for (int i = 0; i < scr.Length; i++)
		{
			array[i] = (char)scr[i];
		}
		return array;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ currentHour.
	/// </summary>
	public static int currentHour()
	{
		return DateTime.Now.Hour;
	}

	public static bool isPC => Main.isPC;

	/// <summary>
	/// Thực thi phương thức nghiệp vụ println.
	/// </summary>
	public static void println(object str)
	{
		Debug.Log(str);
	}

	public static void println(string str)
	{
		Res.outz(str);
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ gcc.
	/// </summary>
	public static void gcc()
	{
		Resources.UnloadUnusedAssets();
		GC.Collect();
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ gI.
	/// </summary>
	public static mSystem gI()
	{
		if (instance == null)
		{
			instance = new mSystem();
		}
		return instance;
	}

	/// <summary>
	/// Bộ xử lý sự kiện tiếp nhận và phản hồi onConnectOK.
	/// </summary>
	public static void onConnectOK()
	{
		Controller.isConnectOK = true;
	}

	/// <summary>
	/// Bộ xử lý sự kiện tiếp nhận và phản hồi onConnectionFail.
	/// </summary>
	public static void onConnectionFail()
	{
		Controller.isConnectionFail = true;
		Debug.LogError(">>>>>>>> Controller.isConnectionFail = true;");
	}

	/// <summary>
	/// Bộ xử lý sự kiện tiếp nhận và phản hồi onDisconnected.
	/// </summary>
	public static void onDisconnected()
	{
		Controller.isDisconnected = true;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ exitWP.
	/// </summary>
	public static void exitWP()
	{
	}

	/// <summary>
	/// Thực hiện vẽ và hiển thị thành phần paintFlyText lên canvas đồ họa.
	/// </summary>
	public static void paintFlyText(mGraphics g)
	{
		for (int i = 0; i < 5; i++)
		{
			if (GameScr.flyTextState[i] != -1 && GameCanvas.isPaint(GameScr.flyTextX[i], GameScr.flyTextY[i]))
			{
				if (GameScr.flyTextColor[i] == mFont.RED)
				{
					mFont.bigNumber_red.drawStringBorder(g, GameScr.flyTextString[i], GameScr.flyTextX[i], GameScr.flyTextY[i], mFont.CENTER);
				}
				else if (GameScr.flyTextColor[i] == mFont.YELLOW)
				{
					mFont.bigNumber_yellow.drawStringBorder(g, GameScr.flyTextString[i], GameScr.flyTextX[i], GameScr.flyTextY[i], mFont.CENTER);
				}
				else if (GameScr.flyTextColor[i] == mFont.GREEN)
				{
					mFont.bigNumber_green.drawStringBorder(g, GameScr.flyTextString[i], GameScr.flyTextX[i], GameScr.flyTextY[i], mFont.CENTER);
				}
				else if (GameScr.flyTextColor[i] == mFont.FATAL)
				{
					mFont.bigNumber_yellow.drawStringBorder(g, GameScr.flyTextString[i], GameScr.flyTextX[i], GameScr.flyTextY[i], mFont.CENTER, mFont.bigNumber_black);
				}
				else if (GameScr.flyTextColor[i] == mFont.FATAL_ME)
				{
					mFont.bigNumber_green.drawStringBorder(g, GameScr.flyTextString[i], GameScr.flyTextX[i], GameScr.flyTextY[i], mFont.CENTER, mFont.bigNumber_black);
				}
				else if (GameScr.flyTextColor[i] == mFont.MISS)
				{
					mFont.bigNumber_While.drawStringBorder(g, GameScr.flyTextString[i], GameScr.flyTextX[i], GameScr.flyTextY[i], mFont.CENTER, mFont.tahoma_7_grey);
				}
				else if (GameScr.flyTextColor[i] == mFont.ORANGE)
				{
					mFont.bigNumber_orange.drawStringBorder(g, GameScr.flyTextString[i], GameScr.flyTextX[i], GameScr.flyTextY[i], mFont.CENTER);
				}
				else if (GameScr.flyTextColor[i] == mFont.ADDMONEY)
				{
					mFont.bigNumber_yellow.drawStringBorder(g, GameScr.flyTextString[i], GameScr.flyTextX[i], GameScr.flyTextY[i], mFont.CENTER, mFont.bigNumber_black);
				}
				else if (GameScr.flyTextColor[i] == mFont.MISS_ME)
				{
					mFont.bigNumber_While.drawStringBorder(g, GameScr.flyTextString[i], GameScr.flyTextX[i], GameScr.flyTextY[i], mFont.CENTER, mFont.bigNumber_black);
				}
				else if (GameScr.flyTextColor[i] == mFont.HP)
				{
					mFont.bigNumber_red.drawStringBorder(g, GameScr.flyTextString[i], GameScr.flyTextX[i], GameScr.flyTextY[i], mFont.CENTER, mFont.bigNumber_black);
				}
				else if (GameScr.flyTextColor[i] == mFont.MP)
				{
					mFont.bigNumber_blue.drawStringBorder(g, GameScr.flyTextString[i], GameScr.flyTextX[i], GameScr.flyTextY[i], mFont.CENTER, mFont.bigNumber_black);
				}
			}
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ endKey.
	/// </summary>
	public static void endKey()
	{
	}

	private static readonly MyHashTable hashFraImage = new MyHashTable();

	public static void ClearImageCache()
	{
		hashFraImage.clear();
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của getFraImage.
	/// </summary>
	public static FrameImage getFraImage(string nameImg)
	{
		if (string.IsNullOrEmpty(nameImg))
		{
			return null;
		}
		FrameImage cached = (FrameImage)hashFraImage.get(nameImg);
		if (cached != null)
		{
			return cached;
		}
		MainImage mainImage = ImgByName.getImagePath(nameImg, ImgByName.hashImagePath);
		if (mainImage != null && mainImage.img != null)
		{
			int num = mainImage.img.getHeight() / (mainImage.nFrame > 0 ? mainImage.nFrame : 1);
			if (num < 1)
			{
				num = 1;
			}
			cached = new FrameImage(mainImage.img, mainImage.img.getWidth(), num);
			hashFraImage.put(nameImg, cached);
			return cached;
		}
		return null;
	}

	/// <summary>
	/// Khởi tạo và nạp dữ liệu cấu hình cho loadImage.
	/// </summary>
	public static Image loadImage(string path)
	{
		return GameCanvas.loadImage(path);
	}
}

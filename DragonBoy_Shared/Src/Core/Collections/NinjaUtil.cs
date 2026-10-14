using System;

/// <summary>
/// CLASS NinjaUtil: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
public class NinjaUtil
{
	/// <summary>
	/// Bộ xử lý sự kiện tiếp nhận và phản hồi onLoadMapComplete.
	/// </summary>
	public static void onLoadMapComplete()
	{
		GameCanvas.endDlg();
	}

	/// <summary>
	/// Bộ xử lý sự kiện tiếp nhận và phản hồi onLoading.
	/// </summary>
	public void onLoading()
	{
		GameCanvas.startWaitDlg(mResources.downloading_data);
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ randomNumber.
	/// </summary>
	public static int randomNumber(int max)
	{
		MyRandom myRandom = new MyRandom();
		return myRandom.nextInt(max);
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ readByteArray.
	/// </summary>
	public static sbyte[] readByteArray(Message msg)
	{
		try
		{
			int num = msg.reader().readInt();
			if (num > 1)
			{
				sbyte[] data = new sbyte[num];
				msg.reader().read(ref data);
				return data;
			}
		}
		catch (Exception)
		{
		}
		return null;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ readByteArray.
	/// </summary>
	public static sbyte[] readByteArray(myReader dos)
	{
		try
		{
			int num = dos.readInt();
			sbyte[] data = new sbyte[num];
			dos.read(ref data);
			return data;
		}
		catch (Exception)
		{
			Cout.LogError("LOI DOC readByteArray dos  NINJAUTIL");
		}
		return null;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ replace.
	/// </summary>
	public static string replace(string text, string regex, string replacement)
	{
		return text.Replace(regex, replacement);
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ numberTostring.
	/// </summary>
	public static string numberTostring(string number)
	{
		string text = string.Empty;
		string text2 = string.Empty;
		if (number.Equals(string.Empty))
		{
			return text;
		}
		if (number[0] == '-')
		{
			text2 = "-";
			number = number.Substring(1);
		}
		for (int num = number.Length - 1; num >= 0; num--)
		{
			text = (((number.Length - 1 - num) % 3 != 0 || number.Length - 1 - num <= 0) ? (number[num] + text) : (number[num] + "." + text));
		}
		return text2 + text;
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của getDate.
	/// </summary>
	public static string getDate(int second)
	{
		long num = (long)second * 1000L;
		DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Add(new TimeSpan(num * 10000)).ToUniversalTime();
		int hour = dateTime.Hour;
		int minute = dateTime.Minute;
		int day = dateTime.Day;
		int month = dateTime.Month;
		int year = dateTime.Year;
		return day + "/" + month + "/" + year + " " + hour + "h";
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của getDate2.
	/// </summary>
	public static string getDate2(long second)
	{
		long num = second + 25200000;
		DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Add(new TimeSpan(num * 10000)).ToUniversalTime();
		int hour = dateTime.Hour;
		int minute = dateTime.Minute;
		return hour + "h" + minute + "m";
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của getTime.
	/// </summary>
	public static string getTime(int timeRemainS)
	{
		int num = 0;
		if (timeRemainS > 60)
		{
			num = timeRemainS / 60;
			timeRemainS %= 60;
		}
		int num2 = 0;
		if (num > 60)
		{
			num2 = num / 60;
			num %= 60;
		}
		int num3 = 0;
		if (num2 > 24)
		{
			num3 = num2 / 24;
			num2 %= 24;
		}
		string empty = string.Empty;
		if (num3 > 0)
		{
			empty += num3;
			empty += "d";
			return empty + num2 + "h";
		}
		if (num2 > 0)
		{
			empty += num2;
			empty += "h";
			return empty + num + "'";
		}
		empty = ((num <= 9) ? (empty + "0" + num) : (empty + num));
		empty += ":";
		if (timeRemainS > 9)
		{
			return empty + timeRemainS;
		}
		return empty + "0" + timeRemainS;
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của getMoneys.
	/// </summary>
	public static string getMoneys(long m)
	{
		string text = string.Empty;
		long num = m / 1000 + 1;
		for (int i = 0; i < num; i++)
		{
			if (m >= 1000)
			{
				long num2 = m % 1000;
				text = ((num2 != 0) ? ((num2 >= 10) ? ((num2 >= 100) ? ("." + num2 + text) : (".0" + num2 + text)) : (".00" + num2 + text)) : (".000" + text));
				m /= 1000;
				continue;
			}
			text = m + text;
			break;
		}
		return text;
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của getTimeAgo.
	/// </summary>
	public static string getTimeAgo(int timeRemainS)
	{
		int num = 0;
		if (timeRemainS > 60)
		{
			num = timeRemainS / 60;
			timeRemainS %= 60;
		}
		int num2 = 0;
		if (num > 60)
		{
			num2 = num / 60;
			num %= 60;
		}
		int num3 = 0;
		if (num2 > 24)
		{
			num3 = num2 / 24;
			num2 %= 24;
		}
		string empty = string.Empty;
		if (num3 > 0)
		{
			empty += num3;
			empty += "d";
			return empty + num2 + "h";
		}
		if (num2 > 0)
		{
			empty += num2;
			empty += "h";
			return empty + num + "'";
		}
		if (num == 0)
		{
			num = 1;
		}
		empty += num;
		return empty + "ph";
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ split.
	/// </summary>
	public static string[] split(string original, string separator)
	{
		MyVector myVector = new MyVector();
		for (int num = original.IndexOf(separator); num >= 0; num = original.IndexOf(separator))
		{
			myVector.addElement(original.Substring(0, num));
			original = original.Substring(num + separator.Length);
		}
		myVector.addElement(original);
		string[] array = new string[myVector.size()];
		if (myVector.size() > 0)
		{
			for (int i = 0; i < myVector.size(); i++)
			{
				array[i] = (string)myVector.elementAt(i);
			}
		}
		return array;
	}

	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của checkNumber.
	/// </summary>
	public static bool checkNumber(string numberStr)
	{
		try
		{
			int.Parse(numberStr);
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}
}

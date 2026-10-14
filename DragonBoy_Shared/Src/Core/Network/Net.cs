using UnityEngine;

/// <summary>
/// CLASS Net: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
internal class Net
{
	public static WWW www;

	public static Command h;

	/// <summary>
	/// Cập nhật trạng thái logic của đối tượng theo từng chu kỳ khung hình.
	/// </summary>
	public static void update()
	{
		if (www != null && www.isDone)
		{
			string str = string.Empty;
			if (www.error == null || www.error.Equals(string.Empty))
			{
				str = www.text;
			}
			www = null;
			if (h != null)
			{
				h.perform(str);
			}
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ connectHTTP.
	/// </summary>
	public static void connectHTTP(string link, Command h)
	{
		if (www != null)
		{
			Cout.LogError("GET HTTP BUSY");
		}
		www = new WWW(link);
		Net.h = h;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ connectHTTP2.
	/// </summary>
	public static void connectHTTP2(string link, Command h)
	{
		Net.h = h;
		if (link != null)
		{
			h.perform(link);
		}
	}
}

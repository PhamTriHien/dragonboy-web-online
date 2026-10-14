using UnityEngine;

/// <summary>
/// CLASS Cout: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
public class Cout
{
	public static int count;

	/// <summary>
	/// Thực thi phương thức nghiệp vụ println.
	/// </summary>
	public static void println(string s)
	{
		if (mSystem.isTest)
		{
			Debug.Log(((count % 2 != 0) ? "***--- " : ">>>--- ") + s);
			count++;
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ Log.
	/// </summary>
	public static void Log(string str)
	{
		if (mSystem.isTest)
		{
			Debug.Log(str);
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ LogError.
	/// </summary>
	public static void LogError(string str)
	{
		if (mSystem.isTest)
		{
			Debug.LogError(str);
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ LogError2.
	/// </summary>
	public static void LogError2(string str)
	{
		if (!mSystem.isTest)
		{
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ LogError3.
	/// </summary>
	public static void LogError3(string str)
	{
		if (!mSystem.isTest)
		{
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ LogWarning.
	/// </summary>
	public static void LogWarning(string str)
	{
		if (mSystem.isTest)
		{
			Debug.LogWarning(str);
		}
	}
}

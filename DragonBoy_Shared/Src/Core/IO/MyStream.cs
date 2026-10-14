using System;

/// <summary>
/// CLASS MyStream: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
public class MyStream
{
	/// <summary>
	/// Thực thi phương thức nghiệp vụ readFile.
	/// </summary>
	public static DataInputStream readFile(string path)
	{
		path = Main.res + path;
		try
		{
			return DataInputStream.getResourceAsStream(path);
		}
		catch (Exception)
		{
			return null;
		}
	}
}

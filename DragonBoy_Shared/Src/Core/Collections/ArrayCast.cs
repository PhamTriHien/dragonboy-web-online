/// <summary>
/// CLASS ArrayCast: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
public class ArrayCast
{
	/// <summary>
	/// Thực thi phương thức nghiệp vụ cast.
	/// </summary>
	public static sbyte[] cast(byte[] data)
	{
		sbyte[] array = new sbyte[data.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = (sbyte)data[i];
		}
		return array;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ cast.
	/// </summary>
	public static byte[] cast(sbyte[] data)
	{
		byte[] array = new byte[data.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = (byte)data[i];
		}
		return array;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ ToCharArray.
	/// </summary>
	public static char[] ToCharArray(sbyte[] data)
	{
		char[] array = new char[data.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = (char)data[i];
		}
		return array;
	}
}

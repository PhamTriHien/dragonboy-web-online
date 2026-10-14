/// <summary>
/// CLASS InputStream: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
public class InputStream : myReader
{
	/// <summary>
	/// Thực thi phương thức nghiệp vụ InputStream.
	/// </summary>
	public InputStream()
	{
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ InputStream.
	/// </summary>
	public InputStream(sbyte[] data)
	{
		buffer = data;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ InputStream.
	/// </summary>
	public InputStream(string filename)
		: base(filename)
	{
	}
}

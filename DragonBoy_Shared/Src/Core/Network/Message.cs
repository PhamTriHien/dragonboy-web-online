/// <summary>
/// CLASS Message: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
public class Message
{
	public sbyte command;

	private myReader dis;

	private myWriter dos;

	/// <summary>
	/// Thực thi phương thức nghiệp vụ Message.
	/// </summary>
	public Message(int command)
	{
		this.command = (sbyte)command;
		dos = new myWriter();
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ Message.
	/// </summary>
	public Message()
	{
		dos = new myWriter();
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ Message.
	/// </summary>
	public Message(sbyte command)
	{
		this.command = command;
		dos = new myWriter();
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ Message.
	/// </summary>
	public Message(sbyte command, sbyte[] data)
	{
		this.command = command;
		dis = new myReader(data);
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của getData.
	/// </summary>
	public sbyte[] getData()
	{
		return dos.getData();
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ reader.
	/// </summary>
	public myReader reader()
	{
		return dis;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ writer.
	/// </summary>
	public myWriter writer()
	{
		return dos;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ readInt3Byte.
	/// </summary>
	public int readInt3Byte()
	{
		return dis.readInt();
	}

	/// <summary>
	/// Dọn dẹp bộ nhớ đệm và giải phóng các tài nguyên đã cấp phát.
	/// </summary>
	public void cleanup()
	{
	}
}

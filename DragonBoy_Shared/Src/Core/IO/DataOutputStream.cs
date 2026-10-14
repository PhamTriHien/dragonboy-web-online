/// <summary>
/// CLASS DataOutputStream: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
public class DataOutputStream
{
	private myWriter w = new myWriter();

	/// <summary>
	/// Thực thi phương thức nghiệp vụ DataOutputStream.
	/// </summary>
	public DataOutputStream()
	{
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ DataOutputStream.
	/// </summary>
	public DataOutputStream(int len)
	{
		w = new myWriter(len);
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ writeShort.
	/// </summary>
	public void writeShort(short i)
	{
		w.writeShort(i);
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ writeInt.
	/// </summary>
	public void writeInt(int i)
	{
		w.writeInt(i);
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ write.
	/// </summary>
	public void write(sbyte[] data)
	{
		w.writeSByte(data);
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ toByteArray.
	/// </summary>
	public sbyte[] toByteArray()
	{
		return w.getData();
	}

	/// <summary>
	/// Đóng cửa sổ hoặc giải phóng phiên làm việc hiện tại.
	/// </summary>
	public void close()
	{
		w.Close();
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ writeByte.
	/// </summary>
	public void writeByte(sbyte b)
	{
		w.writeByte(b);
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ writeUTF.
	/// </summary>
	public void writeUTF(string name)
	{
		w.writeUTF(name);
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ writeBoolean.
	/// </summary>
	public void writeBoolean(bool b)
	{
		w.writeBoolean(b);
	}
}

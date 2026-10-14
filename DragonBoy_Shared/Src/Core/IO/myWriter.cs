using System.Text;

/// <summary>
/// CLASS myWriter: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
public class myWriter
{
	public sbyte[] buffer = new sbyte[2048];

	private int posWrite;

	private int lenght = 2048;

	/// <summary>
	/// Thực thi phương thức nghiệp vụ myWriter.
	/// </summary>
	public myWriter()
	{
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ myWriter.
	/// </summary>
	public myWriter(int len)
	{
		buffer = new sbyte[len];
		lenght = len;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ writeSByte.
	/// </summary>
	public void writeSByte(sbyte value)
	{
		checkLenght(0);
		buffer[posWrite++] = value;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ writeSByteUncheck.
	/// </summary>
	public void writeSByteUncheck(sbyte value)
	{
		buffer[posWrite++] = value;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ writeByte.
	/// </summary>
	public void writeByte(sbyte value)
	{
		writeSByte(value);
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ writeByte.
	/// </summary>
	public void writeByte(int value)
	{
		writeSByte((sbyte)value);
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ writeChar.
	/// </summary>
	public void writeChar(char value)
	{
		writeSByte(0);
		writeSByte((sbyte)value);
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ writeUnsignedByte.
	/// </summary>
	public void writeUnsignedByte(byte value)
	{
		writeSByte((sbyte)value);
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ writeUnsignedByte.
	/// </summary>
	public void writeUnsignedByte(byte[] value)
	{
		checkLenght(value.Length);
		for (int i = 0; i < value.Length; i++)
		{
			writeSByteUncheck((sbyte)value[i]);
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ writeSByte.
	/// </summary>
	public void writeSByte(sbyte[] value)
	{
		checkLenght(value.Length);
		for (int i = 0; i < value.Length; i++)
		{
			writeSByteUncheck(value[i]);
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ writeShort.
	/// </summary>
	public void writeShort(short value)
	{
		checkLenght(2);
		for (int num = 1; num >= 0; num--)
		{
			writeSByteUncheck((sbyte)(value >> num * 8));
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ writeShort.
	/// </summary>
	public void writeShort(int value)
	{
		checkLenght(2);
		short num = (short)value;
		for (int num2 = 1; num2 >= 0; num2--)
		{
			writeSByteUncheck((sbyte)(num >> num2 * 8));
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ writeUnsignedShort.
	/// </summary>
	public void writeUnsignedShort(ushort value)
	{
		checkLenght(2);
		for (int num = 1; num >= 0; num--)
		{
			writeSByteUncheck((sbyte)(value >> num * 8));
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ writeInt.
	/// </summary>
	public void writeInt(int value)
	{
		checkLenght(4);
		for (int num = 3; num >= 0; num--)
		{
			writeSByteUncheck((sbyte)(value >> num * 8));
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ writeLong.
	/// </summary>
	public void writeLong(long value)
	{
		checkLenght(8);
		for (int num = 7; num >= 0; num--)
		{
			writeSByteUncheck((sbyte)(value >> num * 8));
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ writeBoolean.
	/// </summary>
	public void writeBoolean(bool value)
	{
		writeSByte((sbyte)(value ? 1 : 0));
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ writeBool.
	/// </summary>
	public void writeBool(bool value)
	{
		writeSByte((sbyte)(value ? 1 : 0));
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ writeString.
	/// </summary>
	public void writeString(string value)
	{
		char[] array = value.ToCharArray();
		writeShort((short)array.Length);
		checkLenght(array.Length);
		for (int i = 0; i < array.Length; i++)
		{
			writeSByteUncheck((sbyte)array[i]);
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ writeUTF.
	/// </summary>
	public void writeUTF(string value)
	{
		Encoding unicode = Encoding.Unicode;
		Encoding encoding = Encoding.GetEncoding(65001);
		byte[] bytes = unicode.GetBytes(value);
		byte[] array = Encoding.Convert(unicode, encoding, bytes);
		writeShort((short)array.Length);
		checkLenght(array.Length);
		for (int i = 0; i < array.Length; i++)
		{
			sbyte value2 = (sbyte)array[i];
			writeSByteUncheck(value2);
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ write.
	/// </summary>
	public void write(ref sbyte[] data, int arg1, int arg2)
	{
		if (data == null)
		{
			return;
		}
		for (int i = 0; i < arg2; i++)
		{
			writeSByte(data[i + arg1]);
			if (posWrite > buffer.Length)
			{
				break;
			}
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ write.
	/// </summary>
	public void write(sbyte[] value)
	{
		writeSByte(value);
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của getData.
	/// </summary>
	public sbyte[] getData()
	{
		if (posWrite <= 0)
		{
			return null;
		}
		sbyte[] array = new sbyte[posWrite];
		for (int i = 0; i < posWrite; i++)
		{
			array[i] = buffer[i];
		}
		return array;
	}

	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của checkLenght.
	/// </summary>
	public void checkLenght(int ltemp)
	{
		if (posWrite + ltemp > lenght)
		{
			sbyte[] array = new sbyte[lenght + 1024 + ltemp];
			for (int i = 0; i < lenght; i++)
			{
				array[i] = buffer[i];
			}
			buffer = null;
			buffer = array;
			lenght += 1024 + ltemp;
		}
	}

	/// <summary>
	/// Đóng cửa sổ hoặc giải phóng phiên làm việc hiện tại.
	/// </summary>
	public void Close()
	{
		buffer = null;
	}

	/// <summary>
	/// Đóng cửa sổ hoặc giải phóng phiên làm việc hiện tại.
	/// </summary>
	public void close()
	{
		buffer = null;
	}
}

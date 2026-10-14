using System;
using System.Text;
using UnityEngine;

/// <summary>
/// CLASS myReader: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
public class myReader
{
	public sbyte[] buffer;

	private int posRead;

	private int posMark;

	private static string fileName;

	private static int status;

	/// <summary>
	/// Thực thi phương thức nghiệp vụ myReader.
	/// </summary>
	public myReader()
	{
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ myReader.
	/// </summary>
	public myReader(sbyte[] data)
	{
		buffer = data;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ myReader.
	/// </summary>
	public myReader(string filename)
	{
		TextAsset textAsset = (TextAsset)Resources.Load(filename, typeof(TextAsset));
		buffer = mSystem.convertToSbyte(textAsset.bytes);
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ readSByte.
	/// </summary>
	public sbyte readSByte()
	{
		if (posRead < buffer.Length)
		{
			return buffer[posRead++];
		}
		posRead = buffer.Length;
		throw new Exception(" loi doc sbyte eof ");
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ readByte.
	/// </summary>
	public sbyte readByte()
	{
		return readSByte();
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ mark.
	/// </summary>
	public void mark(int readlimit)
	{
		posMark = posRead;
	}

	/// <summary>
	/// Dọn dẹp và đặt lại trạng thái ban đầu cho reset.
	/// </summary>
	public void reset()
	{
		posRead = posMark;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ readUnsignedByte.
	/// </summary>
	public byte readUnsignedByte()
	{
		return convertSbyteToByte(readSByte());
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ readShort.
	/// </summary>
	public short readShort()
	{
		short num = 0;
		for (int i = 0; i < 2; i++)
		{
			num <<= 8;
			num |= (short)(0xFF & buffer[posRead++]);
		}
		return num;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ readUnsignedShort.
	/// </summary>
	public ushort readUnsignedShort()
	{
		ushort num = 0;
		for (int i = 0; i < 2; i++)
		{
			num <<= 8;
			num |= (ushort)(0xFF & buffer[posRead++]);
		}
		return num;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ readInt.
	/// </summary>
	public int readInt()
	{
		int num = 0;
		for (int i = 0; i < 4; i++)
		{
			num <<= 8;
			num |= 0xFF & buffer[posRead++];
		}
		return num;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ readLong.
	/// </summary>
	public long readLong()
	{
		long num = 0L;
		for (int i = 0; i < 8; i++)
		{
			num <<= 8;
			num |= 0xFF & buffer[posRead++];
		}
		return num;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ readBool.
	/// </summary>
	public bool readBool()
	{
		return readSByte() > 0;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ readBoolean.
	/// </summary>
	public bool readBoolean()
	{
		return readSByte() > 0;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ readString.
	/// </summary>
	public string readString()
	{
		short num = readShort();
		byte[] array = new byte[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = convertSbyteToByte(readSByte());
		}
		UTF8Encoding uTF8Encoding = new UTF8Encoding();
		return uTF8Encoding.GetString(array);
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ readStringUTF.
	/// </summary>
	public string readStringUTF()
	{
		short num = readShort();
		byte[] array = new byte[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = convertSbyteToByte(readSByte());
		}
		UTF8Encoding uTF8Encoding = new UTF8Encoding();
		return uTF8Encoding.GetString(array);
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ readUTF.
	/// </summary>
	public string readUTF()
	{
		return readStringUTF();
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ read.
	/// </summary>
	public int read()
	{
		if (posRead < buffer.Length)
		{
			return readSByte();
		}
		return -1;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ read.
	/// </summary>
	public int read(ref sbyte[] data)
	{
		if (data == null)
		{
			return 0;
		}
		int num = 0;
		for (int i = 0; i < data.Length; i++)
		{
			data[i] = readSByte();
			if (posRead > buffer.Length)
			{
				return -1;
			}
			num++;
		}
		return num;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ readFully.
	/// </summary>
	public void readFully(ref sbyte[] data)
	{
		if (data != null && data.Length + posRead <= buffer.Length)
		{
			for (int i = 0; i < data.Length; i++)
			{
				data[i] = readSByte();
			}
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ available.
	/// </summary>
	public int available()
	{
		return buffer.Length - posRead;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ convertSbyteToByte.
	/// </summary>
	public static byte convertSbyteToByte(sbyte var)
	{
		if (var > 0)
		{
			return (byte)var;
		}
		return (byte)(var + 256);
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ convertSbyteToByte.
	/// </summary>
	public static byte[] convertSbyteToByte(sbyte[] var)
	{
		byte[] array = new byte[var.Length];
		for (int i = 0; i < var.Length; i++)
		{
			if (var[i] > 0)
			{
				array[i] = (byte)var[i];
			}
			else
			{
				array[i] = (byte)(var[i] + 256);
			}
		}
		return array;
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

	/// <summary>
	/// Thực thi phương thức nghiệp vụ read.
	/// </summary>
	public void read(ref sbyte[] data, int arg1, int arg2)
	{
		if (data == null)
		{
			return;
		}
		for (int i = 0; i < arg2; i++)
		{
			data[i + arg1] = readSByte();
			if (posRead > buffer.Length)
			{
				break;
			}
		}
	}
}

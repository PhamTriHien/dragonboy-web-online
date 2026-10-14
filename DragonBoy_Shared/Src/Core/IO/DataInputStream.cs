using System;
using System.Threading;
using UnityEngine;

/// <summary>
/// CLASS DataInputStream: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
public class DataInputStream
{
	public myReader r;

	private const int INTERVAL = 5;

	private const int MAXTIME = 500;

	public static DataInputStream istemp;

	private static int status;

	private static string filenametemp;

	/// <summary>
	/// Thực thi phương thức nghiệp vụ DataInputStream.
	/// </summary>
	public DataInputStream(string filename)
	{
		TextAsset textAsset = (TextAsset)Resources.Load(filename, typeof(TextAsset));
		r = new myReader(ArrayCast.cast(textAsset.bytes));
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ DataInputStream.
	/// </summary>
	public DataInputStream(sbyte[] data)
	{
		r = new myReader(data);
	}

	/// <summary>
	/// Cập nhật trạng thái logic của đối tượng theo từng chu kỳ khung hình.
	/// </summary>
	public static void update()
	{
		if (status == 2)
		{
			status = 1;
			istemp = __getResourceAsStream(filenametemp);
			status = 0;
		}
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của getResourceAsStream.
	/// </summary>
	public static DataInputStream getResourceAsStream(string filename)
	{
		return __getResourceAsStream(filename);
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ _getResourceAsStream.
	/// </summary>
	private static DataInputStream _getResourceAsStream(string filename)
	{
		return __getResourceAsStream(filename);
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ __getResourceAsStream.
	/// </summary>
	private static DataInputStream __getResourceAsStream(string filename)
	{
		try
		{
			return new DataInputStream(filename);
		}
		catch (Exception)
		{
			return null;
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ readShort.
	/// </summary>
	public short readShort()
	{
		return r.readShort();
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ readInt.
	/// </summary>
	public int readInt()
	{
		return r.readInt();
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ read.
	/// </summary>
	public int read()
	{
		return r.readUnsignedByte();
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ read.
	/// </summary>
	public void read(ref sbyte[] data)
	{
		r.read(ref data);
	}

	/// <summary>
	/// Đóng cửa sổ hoặc giải phóng phiên làm việc hiện tại.
	/// </summary>
	public void close()
	{
		r.Close();
	}

	/// <summary>
	/// Đóng cửa sổ hoặc giải phóng phiên làm việc hiện tại.
	/// </summary>
	public void Close()
	{
		r.Close();
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ readUTF.
	/// </summary>
	public string readUTF()
	{
		return r.readUTF();
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ readByte.
	/// </summary>
	public sbyte readByte()
	{
		return r.readByte();
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ readLong.
	/// </summary>
	public long readLong()
	{
		return r.readLong();
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ readBoolean.
	/// </summary>
	public bool readBoolean()
	{
		return r.readBoolean();
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ readUnsignedByte.
	/// </summary>
	public int readUnsignedByte()
	{
		return (byte)r.readByte();
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ readUnsignedShort.
	/// </summary>
	public int readUnsignedShort()
	{
		return r.readUnsignedShort();
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ readFully.
	/// </summary>
	public void readFully(ref sbyte[] data)
	{
		r.read(ref data);
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ available.
	/// </summary>
	public int available()
	{
		return r.available();
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ read.
	/// </summary>
	internal void read(ref sbyte[] byteData, int p, int size)
	{
		throw new NotImplementedException();
	}
}

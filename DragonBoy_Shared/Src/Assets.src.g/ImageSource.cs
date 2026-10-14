using System;

namespace Assets.src.g;

/// <summary>
/// CLASS ImageSource: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
internal class ImageSource
{
	public sbyte version;

	public string id;

	public static MyVector vSource = new MyVector();

	public static MyVector vRms = new MyVector();

	/// <summary>
	/// Thực thi phương thức nghiệp vụ ImageSource.
	/// </summary>
	public ImageSource(string ID, sbyte version)
	{
		id = ID;
		this.version = version;
	}

	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của checkRMS.
	/// </summary>
	public static void checkRMS()
	{
		MyVector myVector = new MyVector();
		sbyte[] array = Rms.loadRMS("ImageSource");
		if (array == null)
		{
			Service.gI().imageSource(myVector);
			return;
		}
		vRms = new MyVector();
		DataInputStream dataInputStream = new DataInputStream(array);
		if (dataInputStream == null)
		{
			return;
		}
		try
		{
			short num = dataInputStream.readShort();
			string[] array2 = new string[num];
			sbyte[] array3 = new sbyte[num];
			for (int i = 0; i < num; i++)
			{
				array2[i] = dataInputStream.readUTF();
				array3[i] = dataInputStream.readByte();
				vRms.addElement(new ImageSource(array2[i], array3[i]));
			}
			dataInputStream.close();
		}
		catch (Exception ex)
		{
			ex.StackTrace.ToString();
		}
		Res.outz("vS size= " + vSource.size() + " vRMS size= " + vRms.size());
		if (false)
		{
			for (int j = 0; j < vSource.size(); j++)
			{
				ImageSource imageSource = (ImageSource)vSource.elementAt(j);
				if (!isExistID(imageSource.id))
				{
					myVector.addElement(imageSource);
				}
			}
			for (int k = 0; k < vRms.size(); k++)
			{
				ImageSource imageSource2 = (ImageSource)vRms.elementAt(k);
				if (getVersionRMSByID(imageSource2.id) != getCurrVersionByID(imageSource2.id))
				{
					myVector.addElement(imageSource2);
				}
			}
		}
		Service.gI().imageSource(myVector);
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của getVersionRMSByID.
	/// </summary>
	public static sbyte getVersionRMSByID(string id)
	{
		for (int i = 0; i < vRms.size(); i++)
		{
			if (id.Equals(((ImageSource)vRms.elementAt(i)).id))
			{
				return ((ImageSource)vRms.elementAt(i)).version;
			}
		}
		return -1;
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của getCurrVersionByID.
	/// </summary>
	public static sbyte getCurrVersionByID(string id)
	{
		for (int i = 0; i < vSource.size(); i++)
		{
			if (id.Equals(((ImageSource)vSource.elementAt(i)).id))
			{
				return ((ImageSource)vSource.elementAt(i)).version;
			}
		}
		return -1;
	}

	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của isExistID.
	/// </summary>
	public static bool isExistID(string id)
	{
		for (int i = 0; i < vRms.size(); i++)
		{
			if (id.Equals(((ImageSource)vRms.elementAt(i)).id))
			{
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ saveRMS.
	/// </summary>
	public static void saveRMS()
	{
		DataOutputStream dataOutputStream = new DataOutputStream();
		try
		{
			dataOutputStream.writeShort((short)vSource.size());
			for (int i = 0; i < vSource.size(); i++)
			{
				dataOutputStream.writeUTF(((ImageSource)vSource.elementAt(i)).id);
				dataOutputStream.writeByte(((ImageSource)vSource.elementAt(i)).version);
			}
			Rms.saveRMS("ImageSource", dataOutputStream.toByteArray());
			dataOutputStream.close();
		}
		catch (Exception ex)
		{
			ex.StackTrace.ToString();
		}
	}
}

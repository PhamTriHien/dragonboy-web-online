/// <summary>
/// CLASS ClanImage: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
public class ClanImage
{
	public int ID;

	public string name;

	public short[] idImage;

	public int xu;

	public int luong;

	public static MyVector vClanImage = new MyVector();

	public static MyHashTable idImages = new MyHashTable();

	/// <summary>
	/// Thực thi phương thức nghiệp vụ addClanImage.
	/// </summary>
	public static void addClanImage(ClanImage cm)
	{
		Service.gI().clanImage((sbyte)cm.ID);
		vClanImage.addElement(cm);
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của getClanImage.
	/// </summary>
	public static ClanImage getClanImage(short ID)
	{
		for (int i = 0; i < vClanImage.size(); i++)
		{
			ClanImage clanImage = (ClanImage)vClanImage.elementAt(i);
			if (clanImage.ID == ID)
			{
				return clanImage;
			}
		}
		return null;
	}

	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của isExistClanImage.
	/// </summary>
	public static bool isExistClanImage(int ID)
	{
		for (int i = 0; i < vClanImage.size(); i++)
		{
			ClanImage clanImage = (ClanImage)vClanImage.elementAt(i);
			if (clanImage.ID == ID)
			{
				return true;
			}
		}
		return false;
	}
}

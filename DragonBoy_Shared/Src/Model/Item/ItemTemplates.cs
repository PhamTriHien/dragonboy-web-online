/// <summary>
/// CLASS ItemTemplates: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
public class ItemTemplates
{
	public static MyHashTable itemTemplates = new MyHashTable();

	/// <summary>
	/// Thực thi phương thức nghiệp vụ add.
	/// </summary>
	public static void add(ItemTemplate it)
	{
		itemTemplates.put(it.id, it);
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của get.
	/// </summary>
	public static ItemTemplate get(short id)
	{
		return (ItemTemplate)itemTemplates.get(id);
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của getPart.
	/// </summary>
	public static short getPart(short itemTemplateID)
	{
		return get(itemTemplateID).part;
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của getIcon.
	/// </summary>
	public static short getIcon(short itemTemplateID)
	{
		return get(itemTemplateID).iconID;
	}
}

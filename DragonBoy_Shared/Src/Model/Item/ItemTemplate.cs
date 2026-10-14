/// <summary>
/// CLASS ItemTemplate: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
public class ItemTemplate
{
	public short id;

	public sbyte type;

	public sbyte gender;

	public string name;

	public string[] subName;

	public string description;

	public sbyte level;

	public short iconID;

	public short part;

	public bool isUpToUp;

	public int w;

	public int h;

	public int strRequire;

	public string rawName;

	public string rawDescription;

	/// <summary>
	/// Thực thi phương thức nghiệp vụ ItemTemplate.
	/// </summary>
	public ItemTemplate(short templateID, sbyte type, sbyte gender, string name, string description, sbyte level, int strRequire, short iconID, short part, bool isUpToUp)
	{
		id = templateID;
		this.type = type;
		this.gender = gender;
		this.rawName = name;
		this.rawDescription = description;
		this.name = ModTranslate.GetItemName(templateID, name, description);
		this.description = ModTranslate.GetItemDescription(templateID, description);
		this.level = level;
		this.strRequire = strRequire;
		this.iconID = iconID;
		this.part = part;
		this.isUpToUp = isUpToUp;
	}
}

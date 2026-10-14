/// <summary>
/// CLASS ItemOption: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
public class ItemOption
{
	public int param;

	public sbyte active;

	public sbyte activeCard;

	public ItemOptionTemplate optionTemplate;

	/// <summary>
	/// Thực thi phương thức nghiệp vụ ItemOption.
	/// </summary>
	public ItemOption()
	{
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ ItemOption.
	/// </summary>
	public ItemOption(int optionTemplateId, int param)
	{
		if (optionTemplateId == 22)
		{
			optionTemplateId = 6;
			param *= 1000;
		}
		if (optionTemplateId == 23)
		{
			optionTemplateId = 7;
			param *= 1000;
		}
		this.param = param;
		optionTemplate = GameScr.gI().iOptionTemplates[optionTemplateId];
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của getOptionString.
	/// </summary>
	public string getOptionString()
	{
		string text = (optionTemplate != null) ? Res.GetVietnameseOptionTemplate(optionTemplate.id, optionTemplate.name) : string.Empty;
		return NinjaUtil.replace(text, "#", param + string.Empty);
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của getOptionName.
	/// </summary>
	public string getOptionName()
	{
		string text = (optionTemplate != null) ? Res.GetVietnameseOptionTemplate(optionTemplate.id, optionTemplate.name) : string.Empty;
		return NinjaUtil.replace(text, "+#", string.Empty);
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của getOptiongColor.
	/// </summary>
	public string getOptiongColor()
	{
		string text = (optionTemplate != null) ? Res.GetVietnameseOptionTemplate(optionTemplate.id, optionTemplate.name) : string.Empty;
		return NinjaUtil.replace(text, "$", string.Empty);
	}
}

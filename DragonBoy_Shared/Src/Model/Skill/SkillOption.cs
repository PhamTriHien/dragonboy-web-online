/// <summary>
/// CLASS SkillOption: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
public class SkillOption
{
	public int param;

	public SkillOptionTemplate optionTemplate;

	public string optionString;

	/// <summary>
	/// Truy xuất và trả về dữ liệu của getOptionString.
	/// </summary>
	public string getOptionString()
	{
		if (optionString == null)
		{
			optionString = NinjaUtil.replace(optionTemplate.name, "#", string.Empty + param);
		}
		return optionString;
	}
}

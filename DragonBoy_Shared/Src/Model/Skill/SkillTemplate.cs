/// <summary>
/// CLASS SkillTemplate: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
public class SkillTemplate
{
	public sbyte id;

	public int classId;

	public string name;

	public int maxPoint;

	public int manaUseType;

	public int type;

	public int iconId;

	public string[] description;

	public Skill[] skills;

	public string damInfo;

	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của isBuffToPlayer.
	/// </summary>
	public bool isBuffToPlayer()
	{
		if (type == 2)
		{
			return true;
		}
		return false;
	}

	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của isUseAlone.
	/// </summary>
	public bool isUseAlone()
	{
		if (type == 3)
		{
			return true;
		}
		return false;
	}

	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của isAttackSkill.
	/// </summary>
	public bool isAttackSkill()
	{
		if (type == 1)
		{
			return true;
		}
		return false;
	}

	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của isSkillSpec.
	/// </summary>
	public bool isSkillSpec()
	{
		if (type == 4)
		{
			return true;
		}
		return false;
	}
}

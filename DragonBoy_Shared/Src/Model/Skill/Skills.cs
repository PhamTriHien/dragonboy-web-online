/// <summary>
/// CLASS Skills: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
public class Skills
{
	public static MyHashTable skills = new MyHashTable();

	/// <summary>
	/// Thực thi phương thức nghiệp vụ add.
	/// </summary>
	public static void add(Skill skill)
	{
		skills.put(skill.skillId, skill);
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của get.
	/// </summary>
	public static Skill get(short skillId)
	{
		return (Skill)skills.get(skillId);
	}
}

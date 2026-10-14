/// <summary>
/// CLASS Friend: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
public class Friend
{
	public string friendName;

	public sbyte type;

	/// <summary>
	/// Thực thi phương thức nghiệp vụ Friend.
	/// </summary>
	public Friend(string friendName, sbyte type)
	{
		this.friendName = friendName;
		this.type = type;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ Friend.
	/// </summary>
	public Friend(string friendName)
	{
		this.friendName = friendName;
		type = 2;
	}
}

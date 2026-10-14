/// <summary>
/// CLASS PlayerData: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
public class PlayerData
{
	public int playerID;

	public string name;

	public short head;

	public short body;

	public short leg;

	public long powpoint;

	/// <summary>
	/// Thực thi phương thức nghiệp vụ PlayerData.
	/// </summary>
	public PlayerData(int playerID, string name, short head, short body, short leg, long ppoint)
	{
		this.playerID = playerID;
		this.name = name;
		this.head = head;
		this.body = body;
		this.leg = leg;
		powpoint = ppoint;
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của getInfo.
	/// </summary>
	public string getInfo()
	{
		return name + "\n" + mResources.power_point + " " + powpoint;
	}
}

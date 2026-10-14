/// <summary>
/// CLASS PlayerInfo: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
public class PlayerInfo
{
	public string name;

	public string showName;

	public string status;

	public int IDDB;

	private int exp;

	public bool isReady;

	public int xu;

	public int gold;

	public string strMoney = string.Empty;

	public sbyte finishPosition;

	public bool isMaster;

	public static Image[] imgStart;

	public sbyte[] indexLv;

	public int onlineTime;

	/// <summary>
	/// Truy xuất và trả về dữ liệu của getName.
	/// </summary>
	public string getName()
	{
		return name;
	}

	/// <summary>
	/// Thiết lập giá trị hoặc trạng thái cho setMoney.
	/// </summary>
	public void setMoney(int m)
	{
		xu = m;
		strMoney = GameCanvas.getMoneys(xu);
	}

	/// <summary>
	/// Thiết lập giá trị hoặc trạng thái cho setName.
	/// </summary>
	public void setName(string name)
	{
		this.name = name;
		if (name.Length > 9)
		{
			showName = name.Substring(0, 8);
		}
		else
		{
			showName = name;
		}
	}

	/// <summary>
	/// Thực hiện vẽ toàn bộ đối tượng và các lớp hiển thị lên canvas mGraphics.
	/// </summary>
	public void paint(mGraphics g, int x, int y)
	{
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của getExp.
	/// </summary>
	public int getExp()
	{
		return exp;
	}
}

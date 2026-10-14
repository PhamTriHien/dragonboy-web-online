/// <summary>
/// CLASS InfoItem: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
public class InfoItem
{
	public string s;

	private mFont f;

	public int speed = 70;

	public Char charInfo;

	public bool isChatServer;

	public bool isOnline;

	public int timeCount;

	public int maxTime;

	public long last;

	public long curr;

	/// <summary>
	/// Thực thi phương thức nghiệp vụ InfoItem.
	/// </summary>
	public InfoItem(string s)
	{
		f = mFont.tahoma_7_green2;
		this.s = s;
		speed = 20;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ InfoItem.
	/// </summary>
	public InfoItem(string s, mFont f, int speed)
	{
		this.f = f;
		this.s = s;
		this.speed = speed;
	}
}

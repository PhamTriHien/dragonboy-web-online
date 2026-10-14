/// <summary>
/// CLASS BallInfo: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
public class BallInfo
{
	public int x;

	public int y;

	public int xTo = -999;

	public int yTo = -999;

	public int count;

	public int vy;

	public int vx;

	public int dir;

	public int idImg;

	public bool isPaint = true;

	public bool isDone;

	public bool isSetImg;

	public Char cFocus;

	/// <summary>
	/// Thiết lập giá trị hoặc trạng thái cho SetChar.
	/// </summary>
	public void SetChar()
	{
		cFocus = new Char();
		cFocus.charID = Res.random(-999, -800);
		cFocus.head = -1;
		cFocus.body = -1;
		cFocus.leg = -1;
		cFocus.bag = -1;
		cFocus.cName = string.Empty;
		cFocus.cHP = (cFocus.cHPFull = 20L);
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ UpdChar.
	/// </summary>
	public void UpdChar()
	{
		cFocus.cx = x;
		cFocus.cy = y;
	}
}

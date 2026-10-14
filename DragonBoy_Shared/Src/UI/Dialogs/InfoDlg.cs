/// <summary>
/// CLASS InfoDlg: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
public class InfoDlg
{
	public static bool isShow;

	private static string title;

	private static string subtitke;

	public static int delay;

	public static bool isLock;

	/// <summary>
	/// Hiển thị thành phần giao diện hoặc bảng thông báo lên màn hình.
	/// </summary>
	public static void show(string title, string subtitle, int delay)
	{
		if (title != null)
		{
			isShow = true;
			InfoDlg.title = title;
			subtitke = subtitle;
			InfoDlg.delay = delay;
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ showWait.
	/// </summary>
	public static void showWait()
	{
		show(mResources.PLEASEWAIT, null, 1000);
		isLock = true;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ showWait.
	/// </summary>
	public static void showWait(string str)
	{
		show(str, null, 700);
		isLock = true;
	}

	/// <summary>
	/// Thực hiện vẽ toàn bộ đối tượng và các lớp hiển thị lên canvas mGraphics.
	/// </summary>
	public static void paint(mGraphics g)
	{
		if (isShow && (!isLock || delay <= 4990) && !GameScr.isPaintAlert)
		{
			int num = 10;
			GameCanvas.paintz.paintPopUp(GameCanvas.hw - 75, num, 150, 55, g);
			if (isLock)
			{
				GameCanvas.paintShukiren(GameCanvas.hw - mFont.tahoma_8b.getWidth(title) / 2 - 10, num + 28, g);
				mFont.tahoma_8b.drawString(g, title, GameCanvas.hw + 5, num + 21, 2);
			}
			else if (subtitke != null)
			{
				mFont.tahoma_8b.drawString(g, title, GameCanvas.hw, num + 13, 2);
				mFont.tahoma_7_green2.drawString(g, subtitke, GameCanvas.hw, num + 30, 2);
			}
			else
			{
				mFont.tahoma_8b.drawString(g, title, GameCanvas.hw, num + 21, 2);
			}
		}
	}

	/// <summary>
	/// Cập nhật trạng thái logic của đối tượng theo từng chu kỳ khung hình.
	/// </summary>
	public static void update()
	{
		if (isShow && delay > 0)
		{
			delay--;
			if (delay <= 0)
			{
				hide();
			}
		}
	}

	/// <summary>
	/// Ẩn thành phần giao diện khỏi màn hình hiển thị.
	/// </summary>
	public static void hide()
	{
		title = string.Empty;
		subtitke = null;
		isLock = false;
		delay = 0;
		isShow = false;
	}
}

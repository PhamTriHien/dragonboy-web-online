/// <summary>
/// CLASS ItemTime: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
public class ItemTime
{
	public short idIcon;

	public int second;

	public int minute;

	private long curr;

	private long last;

	private bool isText;

	private bool dontClear;

	private string text;

	private bool isPaint_coolDownBar;

	public int time;

	public int coutTime;

	private int per = 100;

	/// <summary>
	/// Thực thi phương thức nghiệp vụ ItemTime.
	/// </summary>
	public ItemTime()
	{
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ ItemTime.
	/// </summary>
	public ItemTime(short idIcon, int s)
	{
		this.idIcon = idIcon;
		minute = s / 60;
		second = s % 60;
		time = s;
		coutTime = s;
		curr = (last = mSystem.currentTimeMillis());
		isPaint_coolDownBar = idIcon == 14;
	}

	/// <summary>
	/// Khởi tạo và nạp dữ liệu cấu hình cho initTimeText.
	/// </summary>
	public void initTimeText(sbyte id, string text, int time)
	{
		if (time == -1)
		{
			dontClear = true;
		}
		else
		{
			dontClear = false;
		}
		isText = true;
		minute = time / 60;
		second = time % 60;
		idIcon = id;
		this.time = time;
		coutTime = time;
		this.text = text;
		curr = (last = mSystem.currentTimeMillis());
		isPaint_coolDownBar = idIcon == 14;
	}

	/// <summary>
	/// Khởi tạo và nạp dữ liệu cấu hình cho initTime.
	/// </summary>
	public void initTime(int time, bool isText)
	{
		minute = time / 60;
		second = time % 60;
		this.time = time;
		coutTime = time;
		this.isText = isText;
		curr = (last = mSystem.currentTimeMillis());
	}

	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của isExistItem.
	/// </summary>
	public static bool isExistItem(int id)
	{
		for (int i = 0; i < Char.vItemTime.size(); i++)
		{
			ItemTime itemTime = (ItemTime)Char.vItemTime.elementAt(i);
			if (itemTime.idIcon == id)
			{
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của getMessageById.
	/// </summary>
	public static ItemTime getMessageById(int id)
	{
		for (int i = 0; i < GameScr.textTime.size(); i++)
		{
			ItemTime itemTime = (ItemTime)GameScr.textTime.elementAt(i);
			if (itemTime.idIcon == id)
			{
				return itemTime;
			}
		}
		return null;
	}

	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của isExistMessage.
	/// </summary>
	public static bool isExistMessage(int id)
	{
		for (int i = 0; i < GameScr.textTime.size(); i++)
		{
			ItemTime itemTime = (ItemTime)GameScr.textTime.elementAt(i);
			if (itemTime.idIcon == id)
			{
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của getItemById.
	/// </summary>
	public static ItemTime getItemById(int id)
	{
		for (int i = 0; i < Char.vItemTime.size(); i++)
		{
			ItemTime itemTime = (ItemTime)Char.vItemTime.elementAt(i);
			if (itemTime.idIcon == id)
			{
				return itemTime;
			}
		}
		return null;
	}

	/// <summary>
	/// Khởi tạo và nạp dữ liệu cấu hình cho initTime.
	/// </summary>
	public void initTime(int time)
	{
		minute = time / 60;
		second = time % 60;
		coutTime = time;
		curr = (last = mSystem.currentTimeMillis());
	}

	/// <summary>
	/// Thực hiện vẽ toàn bộ đối tượng và các lớp hiển thị lên canvas mGraphics.
	/// </summary>
	public void paint(mGraphics g, int x, int y)
	{
		SmallImage.drawSmallImage(g, idIcon, x, y, 0, 3);
		string empty = string.Empty;
		empty = minute + "'";
		if (minute == 0)
		{
			empty = second + "s";
		}
		mFont.tahoma_7b_white.drawString(g, empty, x, y + 15, 2, mFont.tahoma_7b_dark);
	}

	/// <summary>
	/// Thực hiện vẽ và hiển thị thành phần paintText lên canvas đồ họa.
	/// </summary>
	public void paintText(mGraphics g, int x, int y)
	{
		if (isPaint_coolDownBar)
		{
			if (Char.myCharz() != null)
			{
				int num = 80;
				int x2 = GameCanvas.w / 2 - num / 2;
				int y2 = GameCanvas.h - 80;
				g.setColor(8421504);
				g.fillRect(x2, y2, num, 2);
				g.setColor(16777215);
				if (per > 0)
				{
					g.fillRect(x2, y2, num * per / 100, 2);
				}
			}
			return;
		}
		string empty = string.Empty;
		empty = minute + "'";
		if (minute < 1)
		{
			empty = second + "s";
		}
		if (minute < 0)
		{
			empty = string.Empty;
		}
		if (dontClear)
		{
			empty = string.Empty;
		}
		mFont.tahoma_7b_white.drawString(g, text + " " + empty, x, y, 0, mFont.tahoma_7b_dark);
	}

	/// <summary>
	/// Cập nhật trạng thái logic của đối tượng theo từng chu kỳ khung hình.
	/// </summary>
	public void update()
	{
		curr = mSystem.currentTimeMillis();
		if (curr - last >= 1000)
		{
			last = mSystem.currentTimeMillis();
			second--;
			coutTime--;
			if (second <= 0)
			{
				second = 60;
				minute--;
			}
			if (time > 0)
			{
				per = coutTime * 100 / time;
			}
		}
		if (minute < 0 && !isText)
		{
			Char.vItemTime.removeElement(this);
		}
		if (minute < 0 && isText && !dontClear)
		{
			GameScr.textTime.removeElement(this);
		}
	}
}

/// <summary>
/// CLASS Position: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
public class Position
{
	public int x;

	public int y;

	public int anchor;

	public int g;

	public int v;

	public int w;

	public int h;

	public int color;

	public int limitY;

	public Layer layer;

	public short yTo;

	public short xTo;

	public short distant;

	/// <summary>
	/// Thực thi phương thức nghiệp vụ Position.
	/// </summary>
	public Position()
	{
		x = 0;
		y = 0;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ Position.
	/// </summary>
	public Position(int x, int y, int anchor)
	{
		this.x = x;
		this.y = y;
		this.anchor = anchor;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ Position.
	/// </summary>
	public Position(int x, int y)
	{
		this.x = x;
		this.y = y;
	}

	/// <summary>
	/// Thiết lập giá trị hoặc trạng thái cho setPosTo.
	/// </summary>
	public void setPosTo(int xT, int yT)
	{
		xTo = (short)xT;
		yTo = (short)yT;
		distant = (short)Res.distance(x, y, xTo, yTo);
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ translate.
	/// </summary>
	public int translate()
	{
		if (x == xTo && y == yTo)
		{
			return -1;
		}
		if (Math.abs((xTo - x) / 2) <= 1 && Math.abs((yTo - y) / 2) <= 1)
		{
			x = xTo;
			y = yTo;
			return 0;
		}
		if (x != xTo)
		{
			x += (xTo - x) / 2;
		}
		if (y != yTo)
		{
			y += (yTo - y) / 2;
		}
		if (Res.distance(x, y, xTo, yTo) <= distant / 5)
		{
			return 2;
		}
		return 1;
	}

	/// <summary>
	/// Cập nhật trạng thái logic của đối tượng theo từng chu kỳ khung hình.
	/// </summary>
	public void update()
	{
		layer.update();
	}

	/// <summary>
	/// Thực hiện vẽ toàn bộ đối tượng và các lớp hiển thị lên canvas mGraphics.
	/// </summary>
	public void paint(mGraphics g)
	{
		layer.paint(g, x, y);
	}
}

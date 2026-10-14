/// <summary>
/// CLASS Line: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
public class Line
{
	public int x0;

	public int y0;

	public int x1;

	public int y1;

	public int vx;

	public int vy;

	public int f;

	public int fRe;

	public int idColor;

	public int type;

	public bool is2Line;

	public FrameImage fraImgEff;

	public int[] frame;

	/// <summary>
	/// Thiết lập giá trị hoặc trạng thái cho setLine.
	/// </summary>
	public void setLine(int x0, int y0, int x1, int y1, int vx, int vy, bool is2Line)
	{
		this.x0 = x0;
		this.y0 = y0;
		this.x1 = x1;
		this.y1 = y1;
		this.vx = vx;
		this.vy = vy;
		this.is2Line = is2Line;
	}

	/// <summary>
	/// Cập nhật trạng thái logic của đối tượng theo từng chu kỳ khung hình.
	/// </summary>
	public void update()
	{
		x0 += vx;
		x1 += vx;
		y0 += vy;
		y1 += vy;
		f++;
	}

	/// <summary>
	/// Cập nhật trạng thái logic của update_not_F theo chu kỳ khung hình.
	/// </summary>
	public void update_not_F()
	{
		x0 += vx;
		x1 += vx;
		y0 += vy;
		y1 += vy;
	}
}

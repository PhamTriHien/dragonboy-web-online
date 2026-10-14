/// <summary>
/// CLASS MovePoint: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
public class MovePoint
{
	public int xEnd;

	public int yEnd;

	public int dir;

	public int cvx;

	public int cvy;

	public int status;

	/// <summary>
	/// Thực thi phương thức nghiệp vụ MovePoint.
	/// </summary>
	public MovePoint(int xEnd, int yEnd, int act, int dir)
	{
		this.xEnd = xEnd;
		this.yEnd = yEnd;
		this.dir = dir;
		status = act;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ MovePoint.
	/// </summary>
	public MovePoint(int xEnd, int yEnd)
	{
		this.xEnd = xEnd;
		this.yEnd = yEnd;
	}
}

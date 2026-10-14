/// <summary>
/// CLASS EPosition: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
public class EPosition
{
	public int x;

	public int y;

	public int anchor;

	public sbyte follow;

	public sbyte count;

	public sbyte dir = 1;

	public short index = -1;

	/// <summary>
	/// Thực thi phương thức nghiệp vụ EPosition.
	/// </summary>
	public EPosition(int x, int y)
	{
		this.x = x;
		this.y = y;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ EPosition.
	/// </summary>
	public EPosition(int x, int y, int fol)
	{
		this.x = x;
		this.y = y;
		follow = (sbyte)fol;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ EPosition.
	/// </summary>
	public EPosition()
	{
	}
}

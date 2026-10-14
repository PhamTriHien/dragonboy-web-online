/// <summary>
/// CLASS Part: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
public class Part
{
	public int type;

	public PartImage[] pi;

	/// <summary>
	/// Thực thi phương thức nghiệp vụ Part.
	/// </summary>
	public Part(int type)
	{
		this.type = type;
		if (type == 0)
		{
			pi = new PartImage[3];
		}
		if (type == 1)
		{
			pi = new PartImage[17];
		}
		if (type == 2)
		{
			pi = new PartImage[14];
		}
		if (type == 3)
		{
			pi = new PartImage[2];
		}
	}
}

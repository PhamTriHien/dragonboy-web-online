/// <summary>
/// CLASS MainImage: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
public class MainImage
{
	public Image img;

	public long count = -1L;

	public int timeImageNull;

	public int idImage;

	public long timerequest;

	public sbyte nFrame = 1;

	public long timeUse = mSystem.currentTimeMillis();

	/// <summary>
	/// Thực thi phương thức nghiệp vụ MainImage.
	/// </summary>
	public MainImage()
	{
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ MainImage.
	/// </summary>
	public MainImage(Image im, sbyte nFrame)
	{
		img = im;
		count = 0L;
		this.nFrame = nFrame;
	}
}

namespace Assets.src.e;

/// <summary>
/// CLASS Small: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
public class Small
{
	public Image img;

	public int id;

	public int timePaint;

	public int timeUpdate;

	/// <summary>
	/// Thực thi phương thức nghiệp vụ Small.
	/// </summary>
	public Small(Image img, int id)
	{
		this.img = img;
		this.id = id;
		timePaint = 0;
		timeUpdate = 0;
		ensureScaled();
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ ensureScaled.
	/// </summary>
	public void ensureScaled()
	{
		if (mGraphics.zoomLevel > 1 && img != null && img != SmallImage.imgEmpty && !img.isZoomScaled)
		{
			if (img.w <= 256 && img.h <= 256)
			{
				img.scaleNN(mGraphics.zoomLevel);
			}
		}
	}

	public void clean()
	{
		if (img != null && img != SmallImage.imgEmpty)
		{
			if (img.texture != null)
			{
#if !ANDROID && !__ANDROID__ && !BROWSER
				if (img.texture.HasRaylibTexture && !img.texture.IsSharedGpuTexture)
				{
					try { Raylib_cs.Raylib.UnloadTexture(img.texture.RaylibTexture); } catch { }
					img.texture.HasRaylibTexture = false;
				}
#endif
				img.texture = null;
			}
		}
		img = null;
	}

	/// <summary>
	/// Thực hiện vẽ toàn bộ đối tượng và các lớp hiển thị lên canvas mGraphics.
	/// </summary>
	public void paint(mGraphics g, int transform, int x, int y, int anchor)
	{
		if (img == null || img == SmallImage.imgEmpty)
		{
			return;
		}
		ensureScaled();
		if (mGraphics.getImageWidth(img) <= 1)
		{
			return;
		}
		g.drawRegion(img, 0, 0, mGraphics.getImageWidth(img), mGraphics.getImageHeight(img), transform, x, y, anchor);
		if (GameCanvas.gameTick % 1000 == 0)
		{
			timePaint++;
			timeUpdate = timePaint;
		}
	}

	/// <summary>
	/// Thực hiện vẽ toàn bộ đối tượng và các lớp hiển thị lên canvas mGraphics.
	/// </summary>
	public void paint(mGraphics g, int transform, int f, int x, int y, int w, int h, int anchor)
	{
		paint(g, transform, f, x, y, w, h, anchor, isClip: false);
	}

	/// <summary>
	/// Thực hiện vẽ toàn bộ đối tượng và các lớp hiển thị lên canvas mGraphics.
	/// </summary>
	public void paint(mGraphics g, int transform, int f, int x, int y, int w, int h, int anchor, bool isClip)
	{
		if (img == null || img == SmallImage.imgEmpty)
		{
			return;
		}
		ensureScaled();
		if (mGraphics.getImageWidth(img) <= 1)
		{
			return;
		}
		g.drawRegion(img, 0, f * h, w, h, transform, x, y, anchor, isClip);
		if (GameCanvas.gameTick % 1000 == 0)
		{
			timePaint++;
			timeUpdate = timePaint;
		}
	}

	/// <summary>
	/// Cập nhật trạng thái logic của đối tượng theo từng chu kỳ khung hình.
	/// </summary>
	public void update()
	{
		timeUpdate++;
		if (timeUpdate - timePaint > 300 && !isUsedInMap(id))
		{
			clean();
			SmallImage.setSmall(id, null);
		}
	}

	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của isUsedInMap.
	/// </summary>
	private static bool isUsedInMap(int imgId)
	{
		try
		{
			if (Char.myCharz() != null && Char.myCharz().isCharBodyImageID(imgId))
			{
				return true;
			}
			if (GameScr.vCharInMap != null)
			{
				for (int i = 0; i < GameScr.vCharInMap.size(); i++)
				{
					if (GameScr.vCharInMap.elementAt(i) is Char c && c.isCharBodyImageID(imgId))
					{
						return true;
					}
				}
			}
		}
		catch { }
		return false;
	}
}

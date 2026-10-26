using System;
using Assets.src.e;

/// <summary>
/// CLASS SmallImage: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
public class SmallImage
{
	public static int[][] smallImg;

	public static SmallImage instance;

	public static Image[] imgbig;

	public static Small[] imgNew;

	public static MyVector vKeys = new MyVector();

	public static Image imgEmpty = null;

	public static sbyte[] newSmallVersion;

	public static MyVector vt_images_watingDowload = new MyVector();

	public static int smallCount;

	public static short maxSmall;

	/// <summary>
	/// Thực thi phương thức nghiệp vụ SmallImage.
	/// </summary>
	public SmallImage()
	{
		readImage();
	}

	/// <summary>
	/// Khởi tạo và nạp dữ liệu cấu hình cho loadBigRMS.
	/// </summary>
	public static void loadBigRMS()
	{
		if (imgbig == null || imgbig[0] == null)
		{
			imgbig = new Image[5];
			for (int i = 0; i < 5; i++)
			{
				try
				{
					imgbig[i] = GameCanvas.loadImageRMS("/img/Big" + i + ".png");
					if (imgbig[i] == null)
					{
						imgbig[i] = GameCanvas.loadImage("/img/Big" + i + ".png");
					}
					if (imgbig[i] == null)
					{
						imgbig[i] = GameCanvas.loadImage("/x1/img/Big" + i + ".png");
					}
					if (imgbig[i] == null)
					{
						imgbig[i] = Image.createImage(Main.res + "/x1/img/Big" + i + ".png");
					}
				}
				catch (Exception ex)
				{
					Cout.LogError("Loi loadBigRMS " + i + ": " + ex.Message);
				}
			}
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ freeBig.
	/// </summary>
	public static void freeBig()
	{
		imgbig = null;
		mSystem.gcc();
	}

	public static void ReloadForGraphicsMode()
	{
		imgbig = null;
		if (imgNew != null)
		{
			bool isJava = ModGraphics.IsJavaGraphics;
			for (int i = 0; i < imgNew.Length; i++)
			{
				Small s = imgNew[i];
				if (s == null)
				{
					continue;
				}
				if (isJava)
				{
					Image localJava = GameCanvas.loadImage("/SmallImage/Small" + i + ".png");
					if (localJava != null)
					{
						imgNew[i] = new Small(localJava, i);
					}
					else if (s.img != null && s.img.rawServerPngBytes != null)
					{
						s.img = Image.refreshServerZoomImage(s.img);
					}
					else
					{
						imgNew[i] = null;
					}
				}
				else
				{
					if (s.img != null && s.img.rawServerPngBytes != null)
					{
						s.img = Image.refreshServerZoomImage(s.img);
					}
					else
					{
						imgNew[i] = null;
					}
				}
			}
		}
		loadBigRMS();
	}

	/// <summary>
	/// Khởi tạo và nạp dữ liệu cấu hình cho loadBigImage.
	/// </summary>
	public static void loadBigImage()
	{
		imgEmpty = Image.createRGBImage(new int[1], 1, 1, bl: true);
	}

	/// <summary>
	/// Khởi tạo và nạp dữ liệu cấu hình cho init.
	/// </summary>
	public static void init()
	{
		instance = null;
		instance = new SmallImage();
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ readData.
	/// </summary>
	public void readData(byte[] data)
	{
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ readImage.
	/// </summary>
	public void readImage()
	{
		int num = 0;
		try
		{
			DataInputStream dataInputStream = new DataInputStream(Rms.loadRMS("NR_image"));
			short num2 = dataInputStream.readShort();
			smallImg = new int[num2][];
			for (int i = 0; i < smallImg.Length; i++)
			{
				smallImg[i] = new int[5];
			}
			for (int j = 0; j < num2; j++)
			{
				num++;
				smallImg[j][0] = dataInputStream.readUnsignedByte();
				smallImg[j][1] = dataInputStream.readShort();
				smallImg[j][2] = dataInputStream.readShort();
				smallImg[j][3] = dataInputStream.readShort();
				smallImg[j][4] = dataInputStream.readShort();
			}
		}
		catch (Exception ex)
		{
			Cout.LogError3("Loi readImage: " + ex.ToString() + "i= " + num);
		}
	}

	/// <summary>
	/// Dọn dẹp và đặt lại trạng thái ban đầu cho clearHastable.
	/// </summary>
	public static void clearHastable()
	{
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ ensureImgNew.
	/// </summary>
	public static void ensureImgNew(int id)
	{
		if (imgNew == null)
		{
			imgNew = new Small[System.Math.Max(10000, id + 500)];
		}
		else if (id >= imgNew.Length)
		{
			Array.Resize(ref imgNew, System.Math.Max(imgNew.Length * 2, id + 500));
		}
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của getSmall.
	/// </summary>
	public static Small getSmall(int id)
	{
		if (id < 0) return null;
		ensureImgNew(id);
		return imgNew[id];
	}

	/// <summary>
	/// Thiết lập giá trị hoặc trạng thái cho setSmall.
	/// </summary>
	public static void setSmall(int id, Small s)
	{
		if (id < 0) return;
		ensureImgNew(id);
		imgNew[id] = s;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ queueDownload.
	/// </summary>
	public static void queueDownload(int id)
	{
		if (id < 0) return;
		Small s = getSmall(id);
		if (s == null)
		{
			setSmall(id, new Small(imgEmpty, id));
			s = getSmall(id);
		}
		if (GameCanvas.currentScreen == GameCanvas._SelectCharScr)
		{
			Service.gI().requestIcon(id);
			return;
		}
		if (vt_images_watingDowload == null)
		{
			vt_images_watingDowload = new MyVector();
		}
		for (int i = 0; i < vt_images_watingDowload.size(); i++)
		{
			if (vt_images_watingDowload.elementAt(i) is Small existing && existing.id == id)
			{
				return;
			}
		}
		vt_images_watingDowload.addElement(s);
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ createImage.
	/// </summary>
	public static void createImage(int id)
	{
		if (id < 0) return;
		ensureImgNew(id);
		if (imgEmpty == null)
		{
			loadBigImage();
		}
		try
		{
			if (mGraphics.zoomLevel == 1)
			{
				Image image = GameCanvas.loadImage("/SmallImage/Small" + id + ".png");
				if (image == null)
				{
					image = GameCanvas.loadImageRMS("/x1/SmallImage/Small" + id + ".png");
				}
				if (image == null)
				{
					try { image = Image.createImage("x1/SmallImage/Small" + id + ".png"); } catch { }
				}
				if (image != null)
				{
					setSmall(id, new Small(image, id));
					return;
				}
				setSmall(id, new Small(imgEmpty, id));
				queueDownload(id);
				return;
			}
			Image image2 = GameCanvas.loadImage("/SmallImage/Small" + id + ".png");
			if (image2 != null && (ModGraphics.IsJavaGraphics || image2.texture == null || !image2.texture.IsFallbackFromX1))
			{
				setSmall(id, new Small(image2, id));
				return;
			}

			bool flag = false;
			sbyte[] array = Rms.loadRMS(mGraphics.zoomLevel + "Small" + id);
			if (array != null)
			{
				if (newSmallVersion != null && id < newSmallVersion.Length && array.Length % 127 != newSmallVersion[id])
				{
					flag = true;
				}
				if (!flag)
				{
					Image image3 = null;
					try { image3 = (mGraphics.zoomLevel > 1) ? Image.createServerZoomImage(array, 0, array.Length) : Image.createImage(array, 0, array.Length); } catch { }
					if (image3 != null)
					{
						Small smallRms = new Small(image3, id);
						setSmall(id, smallRms);
						return;
					}
					else
					{
						flag = true;
					}
				}
			}
			else
			{
				flag = true;
			}

			if (image2 != null)
			{
				setSmall(id, new Small(image2, id));
				if (flag)
				{
					queueDownload(id);
				}
				return;
			}

			// Fallback ve asset x1 neu chua co x2
			Image imageFallback = null;
			try { imageFallback = Image.createImage("x1/SmallImage/Small" + id + ".png"); } catch { }
			if (imageFallback == null)
			{
				try { imageFallback = Image.createImage("SmallImage/Small" + id + ".png"); } catch { }
			}
			if (imageFallback != null)
			{
				Small smallFallback = new Small(imageFallback, id);
				smallFallback.ensureScaled();
				setSmall(id, smallFallback);
			}
			else
			{
				setSmall(id, new Small(imgEmpty, id));
			}

			if (flag)
			{
				queueDownload(id);
			}
		}
		catch (Exception)
		{
			setSmall(id, new Small(imgEmpty, id));
		}
	}

	/// <summary>
	/// Thực hiện vẽ và hiển thị thành phần drawSmallImage lên canvas đồ họa.
	/// </summary>
	public static void drawSmallImage(mGraphics g, int id, int x, int y, int transform, int anchor)
	{
		if (id < 0) return;
		if (smallImg == null)
		{
			if (instance == null) instance = new SmallImage();
			else instance.readImage();
		}
		if (imgbig == null || imgbig[0] == null)
		{
			loadBigRMS();
		}

		if (imgbig == null || imgbig[0] == null)
		{
			Small small = getSmall(id);
			if (small == null)
			{
				createImage(id);
			}
			else
			{
				small.paint(g, transform, x, y, anchor);
			}
		}
		else if (smallImg != null)
		{
			if (id >= smallImg.Length || smallImg[id] == null || smallImg[id][1] >= 256 || smallImg[id][3] >= 256 || smallImg[id][2] >= 256 || smallImg[id][4] >= 256)
			{
				Small small2 = getSmall(id);
				if (small2 == null)
				{
					createImage(id);
				}
				else
				{
					small2.paint(g, transform, x, y, anchor);
				}
			}
			else if (smallImg[id][0] < imgbig.Length && imgbig[smallImg[id][0]] != null)
			{
				g.drawRegion(imgbig[smallImg[id][0]], smallImg[id][1], smallImg[id][2], smallImg[id][3], smallImg[id][4], transform, x, y, anchor);
			}
			else
			{
				Small small2 = getSmall(id);
				if (small2 == null)
				{
					createImage(id);
				}
				else
				{
					small2.paint(g, transform, x, y, anchor);
				}
			}
		}
		else if (GameCanvas.currentScreen != GameScr.gI())
		{
			Small small3 = getSmall(id);
			if (small3 == null)
			{
				createImage(id);
			}
			else
			{
				small3.paint(g, transform, x, y, anchor);
			}
		}
	}

	/// <summary>
	/// Thực hiện vẽ và hiển thị thành phần drawSmallImage lên canvas đồ họa.
	/// </summary>
	public static void drawSmallImage(mGraphics g, int id, int f, int x, int y, int w, int h, int transform, int anchor)
	{
		if (id < 0) return;
		if (smallImg == null)
		{
			if (instance == null) instance = new SmallImage();
			else instance.readImage();
		}
		if (imgbig == null || imgbig[0] == null)
		{
			loadBigRMS();
		}

		if (imgbig == null || imgbig[0] == null)
		{
			Small small = getSmall(id);
			if (small == null)
			{
				createImage(id);
			}
			else
			{
				small.paint(g, transform, f, x, y, w, h, anchor);
			}
		}
		else if (smallImg != null)
		{
			if (id >= smallImg.Length || smallImg[id] == null || smallImg[id][1] >= 256 || smallImg[id][3] >= 256 || smallImg[id][2] >= 256 || smallImg[id][4] >= 256)
			{
				Small small2 = getSmall(id);
				if (small2 == null)
				{
					createImage(id);
				}
				else
				{
					small2.paint(g, transform, f, x, y, w, h, anchor);
				}
			}
			else if (smallImg[id][0] != 4 && smallImg[id][0] < imgbig.Length && imgbig[smallImg[id][0]] != null)
			{
				g.drawRegion(imgbig[smallImg[id][0]], smallImg[id][1], smallImg[id][2] + f * h, w, h, transform, x, y, anchor);
			}
			else
			{
				Small small3 = getSmall(id);
				if (small3 == null)
				{
					createImage(id);
				}
				else
				{
					small3.paint(g, transform, f, x, y, w, h, anchor);
				}
			}
		}
		else if (GameCanvas.currentScreen != GameScr.gI())
		{
			Small small4 = getSmall(id);
			if (small4 == null)
			{
				createImage(id);
			}
			else
			{
				small4.paint(g, transform, f, x, y, w, h, anchor);
			}
		}
	}

	/// <summary>
	/// Cập nhật trạng thái logic của đối tượng theo từng chu kỳ khung hình.
	/// </summary>
	public static void update()
	{
		int num = 0;
		if (GameCanvas.gameTick % 1000 != 0 || imgNew == null)
		{
			return;
		}
		for (int i = 0; i < imgNew.Length; i++)
		{
			if (imgNew[i] != null)
			{
				num++;
				imgNew[i].update();
				smallCount++;
			}
		}
		if (num > 200 && GameCanvas.lowGraphic)
		{
			imgNew = new Small[System.Math.Max((int)maxSmall, 10000)];
		}
	}
}

/// <summary>
/// CLASS PopUp: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
public class PopUp
{
	public static MyVector vPopups = new MyVector();

	public int sayWidth;

	public int sayRun;

	public string[] says;

	public int cx;

	public int cy;

	public int cw;

	public int ch;

	public static int f;

	public static int tF;

	public static int dir;

	public bool isWayPoint;

	public int tDelay;

	private int timeDelay;

	public Command command;

	public bool isPaint = true;

	public bool isHide;

	public static Image goc;

	public static Image imgPopUp;

	public static Image imgPopUp2;

	public Image imgFocus;

	public Image imgUnFocus;

	/// <summary>
	/// Thực thi phương thức nghiệp vụ PopUp.
	/// </summary>
	public PopUp(string info, int x, int y)
	{
		sayWidth = 100;
		if (info.Length < 10)
		{
			sayWidth = 60;
		}
		if (GameCanvas.w == 128)
		{
			sayWidth = 128;
		}
		says = mFont.tahoma_7b_dark.splitFontArray(info, sayWidth - 10);
		sayRun = 7;
		cx = x - sayWidth / 2 - 1;
		cy = y - 15 + sayRun - says.Length * 12 - 10;
		cw = sayWidth + 2;
		ch = (says.Length + 1) * 12 + 1;
		while (cw % 10 != 0)
		{
			cw++;
		}
		while (ch % 10 != 0)
		{
			ch++;
		}
		if (x >= 0 && x <= 24)
		{
			cx += cw / 2 + 30;
		}
		if (x <= TileMap.tmw * 24 && x >= TileMap.tmw * 24 - 24)
		{
			cx -= cw / 2 + 6;
		}
		while (cx <= 30)
		{
			cx += 2;
		}
		while (cx + cw >= TileMap.tmw * 24 - 30)
		{
			cx -= 2;
		}
	}

	/// <summary>
	/// Khởi tạo và nạp dữ liệu cấu hình cho loadBg.
	/// </summary>
	public static void loadBg()
	{
		if (goc == null)
		{
			goc = GameCanvas.loadImage("/mainImage/myTexture2dbd3.png");
		}
		if (imgPopUp == null)
		{
			imgPopUp = GameCanvas.loadImage("/mainImage/myTexture2dimgPopup.png");
		}
		if (imgPopUp2 == null)
		{
			imgPopUp2 = GameCanvas.loadImage("/mainImage/myTexture2dimgPopup2.png");
		}
	}

	/// <summary>
	/// Cập nhật trạng thái logic của updateXYWH theo chu kỳ khung hình.
	/// </summary>
	public void updateXYWH(string[] info, int x, int y)
	{
		sayWidth = 0;
		for (int i = 0; i < info.Length; i++)
		{
			if (sayWidth < mFont.tahoma_7b_dark.getWidth(info[i]))
			{
				sayWidth = mFont.tahoma_7b_dark.getWidth(info[i]);
			}
		}
		sayWidth += 20;
		says = info;
		sayRun = 7;
		cx = x - sayWidth / 2 - 1;
		cy = y - 15 + sayRun - says.Length * 12 - 10;
		cw = sayWidth + 2;
		ch = (says.Length + 1) * 12 + 1;
		while (cw % 10 != 0)
		{
			cw++;
		}
		while (ch % 10 != 0)
		{
			ch++;
		}
		if (x >= 0 && x <= 24)
		{
			cx += cw / 2 + 30;
		}
		if (x <= TileMap.tmw * 24 && x >= TileMap.tmw * 24 - 24)
		{
			cx -= cw / 2 + 6;
		}
		while (cx <= 30)
		{
			cx += 2;
		}
		while (cx + cw >= TileMap.tmw * 24 - 30)
		{
			cx -= 2;
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ addPopUp.
	/// </summary>
	public static void addPopUp(int x, int y, string info)
	{
		vPopups.addElement(new PopUp(info, x, y));
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ addPopUp.
	/// </summary>
	public static void addPopUp(PopUp p)
	{
		vPopups.addElement(p);
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ removePopUp.
	/// </summary>
	public static void removePopUp(PopUp p)
	{
		vPopups.removeElement(p);
	}

	/// <summary>
	/// Thực hiện vẽ và hiển thị thành phần paintClipPopUp lên canvas đồ họa.
	/// </summary>
	public void paintClipPopUp(mGraphics g, int x, int y, int w, int h, int color, bool isFocus)
	{
		if (color == 1)
		{
			g.fillRect(x, y, w, h, 16777215, 90);
		}
		else
		{
			g.fillRect(x, y, w, h, 0, 77);
		}
	}

	/// <summary>
	/// Thực hiện vẽ và hiển thị thành phần paintPopUp lên canvas đồ họa.
	/// </summary>
	public static void paintPopUp(mGraphics g, int x, int y, int w, int h, int color, bool isButton)
	{
		if (!isButton)
		{
			if (goc == null)
			{
				try
				{
					goc = GameCanvas.loadImage("/mainImage/myTexture2dbd3.png");
				}
				catch { }
			}

			// 1. Viền ngoài đen (Khung chữ nhật bo góc)
			g.setColor(0);
			g.fillRect(x + 6, y, w - 14 + 1, h);
			g.fillRect(x, y + 6, w, h - 12 + 1);

			// 2. Ruột nền bên trong
			g.setColor(color);
			g.fillRect(x + 6, y + 1, w - 12, h - 2);
			g.fillRect(x + 1, y + 6, w - 2, h - 12);

			// 3. 4 Góc bo tròn 7x6 px
			if (goc != null)
			{
				g.drawRegion(goc, 0, 0, 7, 6, 0, x, y, 0);
				g.drawRegion(goc, 0, 0, 7, 6, 2, x + w - 7, y, 0);
				g.drawRegion(goc, 0, 0, 7, 6, 1, x, y + h - 6, 0);
				g.drawRegion(goc, 0, 0, 7, 6, 3, x + w - 7, y + h - 6, 0);
			}
			return;
		}

		if (imgPopUp == null)
		{
			try { imgPopUp = GameCanvas.loadImage("/mainImage/myTexture2dimgPopup.png"); } catch { }
		}
		if (imgPopUp2 == null)
		{
			try { imgPopUp2 = GameCanvas.loadImage("/mainImage/myTexture2dimgPopup2.png"); } catch { }
		}

		Image arg = ((color != 1) ? imgPopUp : imgPopUp2);
		if (arg != null)
		{
			// 4 góc 10x10 px
			g.drawRegion(arg, 0, 0, 10, 10, 0, x, y, 0);
			g.drawRegion(arg, 0, 20, 10, 10, 0, x + w - 10, y, 0);
			g.drawRegion(arg, 0, 50, 10, 10, 0, x, y + h - 10, 0);
			g.drawRegion(arg, 0, 70, 10, 10, 0, x + w - 10, y + h - 10, 0);

			int remW = w - 20;
			int remH = h - 20;
			int num = (remW % 10 != 0) ? (remW / 10 + 1) : (remW / 10);
			int num2 = (remH % 10 != 0) ? (remH / 10 + 1) : (remH / 10);

			for (int i = 0; i < num; i++)
			{
				int segW = (i == num - 1 && remW % 10 != 0) ? (remW % 10) : 10;
				g.drawRegion(arg, 0, 10, segW, 10, 0, x + 10 + i * 10, y, 0);
				g.drawRegion(arg, 0, 60, segW, 10, 0, x + 10 + i * 10, y + h - 10, 0);
			}
			for (int j = 0; j < num2; j++)
			{
				int segH = (j == num2 - 1 && remH % 10 != 0) ? (remH % 10) : 10;
				g.drawRegion(arg, 0, 30, 10, segH, 0, x, y + 10 + j * 10, 0);
				g.drawRegion(arg, 0, 40, 10, segH, 0, x + w - 10, y + 10 + j * 10, 0);
			}
			g.setColor((color != 1) ? 16770503 : 12052656);
			g.fillRect(x + 10, y + 10, w - 20, h - 20);
		}
	}

	/// <summary>
	/// Thực hiện vẽ toàn bộ đối tượng và các lớp hiển thị lên canvas mGraphics.
	/// </summary>
	public void paint(mGraphics g)
	{
		if (isPaint && says != null && ChatPopup.currChatPopup == null && !isHide)
		{
			paintClipPopUp(g, cx, cy - GameCanvas.transY, cw, ch, (timeDelay != 0) ? 1 : 0, isFocus: true);
			mFont font = (timeDelay != 0) ? mFont.tahoma_7b_green2 : mFont.tahoma_7b_white;
			int fontH = (font != null) ? font.getHeight() : 12;
			int startY = cy + (ch - says.Length * fontH) / 2 - GameCanvas.transY;
			for (int i = 0; i < says.Length; i++)
			{
				font.drawString(g, says[i], cx + cw / 2, startY + i * fontH, mFont.CENTER);
			}
		}
	}

	/// <summary>
	/// Cập nhật trạng thái logic của đối tượng theo từng chu kỳ khung hình.
	/// </summary>
	private void update()
	{
		if (Char.myCharz().taskMaint != null && Char.myCharz().taskMaint.taskId == 0)
		{
			if (cx + cw >= GameScr.cmx && cx <= GameCanvas.w + GameScr.cmx && cy + ch >= GameScr.cmy && cy <= GameCanvas.h + GameScr.cmy)
			{
				isHide = false;
			}
			else
			{
				isHide = true;
			}
		}
		if (Char.myCharz().taskMaint == null || (Char.myCharz().taskMaint != null && Char.myCharz().taskMaint.taskId != 0))
		{
			if (cx + cw / 2 >= Char.myCharz().cx - 100 && cx + cw / 2 <= Char.myCharz().cx + 100 && cy + ch >= GameScr.cmy && cy <= GameCanvas.h + GameScr.cmy)
			{
				isHide = false;
			}
			else
			{
				isHide = true;
			}
		}
		if (timeDelay > 0)
		{
			timeDelay--;
			if (timeDelay == 0 && command != null)
			{
				command.performAction();
			}
		}
		if (!isWayPoint)
		{
			return;
		}
		if (Char.myCharz().taskMaint != null)
		{
			if (Char.myCharz().taskMaint.taskId == 0)
			{
				if (Char.myCharz().taskMaint.index == 0)
				{
					isPaint = false;
				}
				if (Char.myCharz().taskMaint.index == 1)
				{
					isPaint = true;
				}
				if (Char.myCharz().taskMaint.index > 1 && Char.myCharz().taskMaint.index < 6)
				{
					isPaint = false;
				}
			}
			else if (!isPaint)
			{
				tDelay++;
				if (tDelay == 50)
				{
					isPaint = true;
				}
			}
		}
		else if (!isPaint)
		{
			Hint.isPaint = false;
			tDelay++;
			if (tDelay == 50)
			{
				isPaint = true;
				Hint.isPaint = true;
			}
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ doClick.
	/// </summary>
	public void doClick(int timeDelay)
	{
		this.timeDelay = timeDelay;
	}

	/// <summary>
	/// Thực hiện vẽ và hiển thị thành phần paintAll lên canvas đồ họa.
	/// </summary>
	public static void paintAll(mGraphics g)
	{
		for (int i = 0; i < vPopups.size(); i++)
		{
			((PopUp)vPopups.elementAt(i)).paint(g);
		}
	}

	/// <summary>
	/// Cập nhật trạng thái logic của updateAll theo chu kỳ khung hình.
	/// </summary>
	public static void updateAll()
	{
		for (int i = 0; i < vPopups.size(); i++)
		{
			((PopUp)vPopups.elementAt(i)).update();
		}
	}
}

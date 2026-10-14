/// <summary>
/// Đối tượng lệnh tương tác: Gán hành động xử lý (actionListener) cho các nút bấm menu và phím tắt mềm.
/// </summary>
public class Command
{
	public bool isDisplay;

	public ActionChat actionChat;

	public string caption;

	public string[] subCaption;

	public IActionListener actionListener;

	public int idAction;

	public bool isPlaySoundButton = true;

	public Image img;

	public Image imgFocus;

	public Image imgBtn;

	public int x;

	public int y;

	public int w = mScreen.cmdW;

	public int h = mScreen.cmdH;

	public int hw;

	private int lenCaption;

	public bool isFocus;

	public object p;

	public int type;

	public string caption2 = string.Empty;

	public static Image btn0left;

	public static Image btn0mid;

	public static Image btn0right;

	public static Image btn1left;

	public static Image btn1mid;

	public static Image btn1right;

	public bool cmdClosePanel;

	public bool isPaintNew;

	/// <summary>
	/// Thực thi phương thức nghiệp vụ Command.
	/// </summary>
	public Command(string caption, IActionListener actionListener, int action, object p, int x, int y)
	{
		this.caption = caption;
		idAction = action;
		this.actionListener = actionListener;
		this.p = p;
		this.x = x;
		this.y = y;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ Command.
	/// </summary>
	public Command()
	{
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ Command.
	/// </summary>
	public Command(string caption, IActionListener actionListener, int action, object p)
	{
		this.caption = caption;
		idAction = action;
		this.actionListener = actionListener;
		this.p = p;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ Command.
	/// </summary>
	public Command(string caption, int action, object p)
	{
		this.caption = caption;
		idAction = action;
		this.p = p;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ Command.
	/// </summary>
	public Command(string caption, int action)
	{
		this.caption = caption;
		idAction = action;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ Command.
	/// </summary>
	public Command(string caption, int action, int x, int y)
	{
		this.caption = caption;
		idAction = action;
		this.x = x;
		this.y = y;
	}

	/// <summary>
	/// Thực thi hành động tương tác khi người dùng kích hoạt lệnh hoặc nút bấm.
	/// </summary>
	public void perform(string str)
	{
		if (actionChat != null)
		{
			actionChat(str);
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ performAction.
	/// </summary>
	public void performAction()
	{
		GameCanvas.clearAllPointerEvent();
		if (isPlaySoundButton && ((caption != null && !caption.Equals(string.Empty) && !caption.Equals(mResources.saying)) || img != null))
		{
			SoundMn.gI().buttonClick();
		}
		if (idAction > 0)
		{
			if (actionListener != null)
			{
				actionListener.perform(idAction, p);
			}
			else
			{
				GameScr.gI().actionPerform(idAction, p);
			}
		}
	}

	/// <summary>
	/// Thiết lập giá trị hoặc trạng thái cho setType.
	/// </summary>
	public void setType()
	{
		type = 1;
		w = 160;
		hw = 80;
	}

	/// <summary>
	/// Thực hiện vẽ toàn bộ đối tượng và các lớp hiển thị lên canvas mGraphics.
	/// </summary>
	public void paint(mGraphics g)
	{
		int btnW = (w > 0) ? w : mScreen.cmdW;
		int btnH = (h > 0) ? h : 24;

		if (img != null)
		{
			int imgW = mGraphics.getImageWidth(img);
			int imgH = mGraphics.getImageHeight(img);
			g.drawImage(img, x, y + mGraphics.addYWhenOpenKeyBoard, 0);
			if (isFocus)
			{
				if (imgFocus == null)
				{
					if (cmdClosePanel)
					{
						g.drawImage(ItemMap.imageFlare, x + 8, y + mGraphics.addYWhenOpenKeyBoard + 8, 3);
					}
					else
					{
						g.drawImage(ItemMap.imageFlare, x - (img.Equals(GameScr.imgMenu) ? 10 : 0), y + mGraphics.addYWhenOpenKeyBoard, 0);
					}
				}
				else
				{
					g.drawImage(imgFocus, x, y + mGraphics.addYWhenOpenKeyBoard, 0);
				}
			}
			if (caption != "menu" && caption != null && caption != string.Empty)
			{
				mFont font = isFocus ? mFont.tahoma_7b_green2 : mFont.tahoma_7b_dark;
				int fontH = (font != null) ? font.getHeight() : 12;
				int textY = y + mGraphics.addYWhenOpenKeyBoard + (imgH - fontH) / 2;
				font.drawString(g, caption, x + imgW / 2, textY, mFont.CENTER);
			}
			return;
		}

		if (caption != null && caption != string.Empty)
		{
			if (btn0left == null)
			{
				try
				{
					btn0left = GameCanvas.loadImage("/mainImage/btn0left.png");
					btn0mid = GameCanvas.loadImage("/mainImage/btn0mid.png");
					btn0right = GameCanvas.loadImage("/mainImage/btn0right.png");
					btn1left = GameCanvas.loadImage("/mainImage/btn1left.png");
					btn1mid = GameCanvas.loadImage("/mainImage/btn1mid.png");
					btn1right = GameCanvas.loadImage("/mainImage/btn1right.png");
				}
				catch { }
			}

			if (btn0left != null && btn0mid != null && btn0right != null)
			{
				if (!isFocus)
				{
					paintOngMau(btn0left, btn0mid, btn0right, x, y, btnW, g);
				}
				else
				{
					paintOngMau(btn1left, btn1mid, btn1right, x, y, btnW, g);
				}
			}
		}

		int num = 0;
		int num2 = x + btnW / 2;
		if (imgBtn != null)
		{
			num = imgBtn.getWidth();
			num2 = x + num;
			if (!isFocus)
			{
				g.drawImage(imgBtn, x, y, 0);
			}
			else
			{
				g.drawImage(imgBtn, x, y + 1, 0);
			}
		}

		if (caption != null && caption != string.Empty)
		{
			mFont font = isFocus ? mFont.tahoma_7b_green2 : mFont.tahoma_7b_dark;
			int fontH = (font != null) ? font.getHeight() : 12;
			int textY = y + (btnH - fontH) / 2;
			font.drawString(g, caption, num2, textY, (num == 0) ? mFont.CENTER : mFont.LEFT);
		}
	}

	/// <summary>
	/// Thực hiện vẽ và hiển thị thành phần paintOngMau lên canvas đồ họa.
	/// </summary>
	public static void paintOngMau(Image img0, Image img1, Image img2, int x, int y, int size, mGraphics g)
	{
		for (int i = 10; i <= size - 20; i += 10)
		{
			g.drawImage(img1, x + i, y, 0);
		}
		int num = size % 10;
		if (num > 0)
		{
			g.drawRegion(img1, 0, 0, num, 24, 0, x + size - 10 - num, y, 0);
		}
		g.drawImage(img0, x, y, 0);
		g.drawImage(img2, x + size - 10, y, 0);
	}

	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của isPointerPressInside.
	/// </summary>
	public bool isPointerPressInside()
	{
		isFocus = false;
		int cw = (w > 0) ? w : mScreen.cmdW;
		int ch = (h > 0) ? h : ((img != null) ? mGraphics.getImageHeight(img) : 24);
		if (GameCanvas.isPointerHoldIn(x, y, cw, ch) || GameCanvas.isPointer(x, y, cw, ch))
		{
			if (GameCanvas.isPointerDown)
			{
				isFocus = true;
			}
			if (GameCanvas.isPointerJustRelease || GameCanvas.isPointerClick)
			{
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của isPointerPressInsideCamera.
	/// </summary>
	public bool isPointerPressInsideCamera(int cmx, int cmy)
	{
		isFocus = false;
		int cw = (w > 0) ? w : mScreen.cmdW;
		int ch = (h > 0) ? h : mScreen.cmdH;
		if (GameCanvas.isPointerHoldIn(x - cmx, y - cmy, cw, ch) || GameCanvas.isPointer(x - cmx, y - cmy, cw, ch))
		{
			if (GameCanvas.isPointerDown)
			{
				isFocus = true;
			}
			if (GameCanvas.isPointerJustRelease || GameCanvas.isPointerClick)
			{
				return true;
			}
		}
		return false;
	}
}

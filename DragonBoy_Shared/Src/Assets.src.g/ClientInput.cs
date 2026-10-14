namespace Assets.src.g;

/// <summary>
/// CLASS ClientInput: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
public class ClientInput : mScreen, IActionListener
{
	public static ClientInput instance;

	public TField[] tf;

	private int x;

	private int y;

	private int w;

	private int h;

	private string[] strPaint;

	private int focus;

	private int nTf;

	/// <summary>
	/// Khởi tạo và nạp dữ liệu cấu hình cho init.
	/// </summary>
	private void init(string t)
	{
		w = GameCanvas.w - 20;
		if (w > 320)
		{
			w = 320;
		}
		Res.outz("title= " + t);
		strPaint = mFont.tahoma_7b_dark.splitFontArray(t, w - 20);
		x = (GameCanvas.w - w) / 2;
		tf = new TField[nTf];
		h = tf.Length * 35 + (strPaint.Length - 1) * 20 + 40;
		y = GameCanvas.h - h - 40 - (strPaint.Length - 1) * 20;
		for (int i = 0; i < tf.Length; i++)
		{
			tf[i] = new TField();
			tf[i].name = string.Empty;
			tf[i].x = x + 10;
			tf[i].y = y + 35 + (strPaint.Length - 1) * 20 + i * 35;
			tf[i].width = w - 20;
			tf[i].height = mScreen.ITEM_HEIGHT + 2;
			if (GameCanvas.isTouch)
			{
				tf[0].isFocus = false;
			}
			else
			{
				tf[0].isFocus = true;
			}
			if (!GameCanvas.isTouch)
			{
				right = tf[0].cmdClear;
			}
		}
		left = new Command(mResources.CLOSE, this, 1, null);
		center = new Command(mResources.OK, this, 2, null);
		if (GameCanvas.isTouch)
		{
			center.x = GameCanvas.w / 2 + 18;
			left.x = GameCanvas.w / 2 - 85;
			center.y = (left.y = y + h + 5);
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ gI.
	/// </summary>
	public static ClientInput gI()
	{
		if (instance == null)
		{
			instance = new ClientInput();
		}
		return instance;
	}

	/// <summary>
	/// Chuyển đổi màn hình hiện tại sang màn hình này làm màn hình hiển thị chính.
	/// </summary>
	public override void switchToMe()
	{
		focus = 0;
		base.switchToMe();
	}

	/// <summary>
	/// Thiết lập giá trị hoặc trạng thái cho setInput.
	/// </summary>
	public void setInput(int type, string title)
	{
		nTf = type;
		init(title);
		switchToMe();
	}

	/// <summary>
	/// Thực hiện vẽ toàn bộ đối tượng và các lớp hiển thị lên canvas mGraphics.
	/// </summary>
	public override void paint(mGraphics g)
	{
		GameScr.gI().paint(g);
		PopUp.paintPopUp(g, x, y, w, h, -1, isButton: true);
		for (int i = 0; i < strPaint.Length; i++)
		{
			mFont.tahoma_7b_green2.drawString(g, strPaint[i], GameCanvas.w / 2, y + 15 + i * 20, mFont.CENTER);
		}
		for (int j = 0; j < tf.Length; j++)
		{
			tf[j].paint(g);
		}
		base.paint(g);
	}

	/// <summary>
	/// Cập nhật trạng thái logic của đối tượng theo từng chu kỳ khung hình.
	/// </summary>
	public override void update()
	{
		GameScr.gI().update();
		for (int i = 0; i < tf.Length; i++)
		{
			tf[i].update();
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ keyPress.
	/// </summary>
	public override void keyPress(int keyCode)
	{
		for (int i = 0; i < tf.Length; i++)
		{
			if (tf[i].isFocus)
			{
				tf[i].keyPressed(keyCode);
				break;
			}
		}
		base.keyPress(keyCode);
	}

	/// <summary>
	/// Cập nhật trạng thái logic của updateKey theo chu kỳ khung hình.
	/// </summary>
	public override void updateKey()
	{
		if (GameCanvas.keyPressed[2])
		{
			focus--;
			if (focus < 0)
			{
				focus = tf.Length - 1;
			}
		}
		else if (GameCanvas.keyPressed[8])
		{
			focus++;
			if (focus > tf.Length - 1)
			{
				focus = 0;
			}
		}
		if (GameCanvas.keyPressed[2] || GameCanvas.keyPressed[8])
		{
			GameCanvas.clearKeyPressed();
			for (int i = 0; i < tf.Length; i++)
			{
				if (focus == i)
				{
					tf[i].isFocus = true;
					if (!GameCanvas.isTouch)
					{
						right = tf[i].cmdClear;
					}
				}
				else
				{
					tf[i].isFocus = false;
				}
				if (GameCanvas.isPointerJustRelease && GameCanvas.isPointerHoldIn(tf[i].x, tf[i].y, tf[i].width, tf[i].height))
				{
					focus = i;
					break;
				}
			}
		}
		base.updateKey();
		GameCanvas.clearKeyPressed();
	}

	/// <summary>
	/// Dọn dẹp và đặt lại trạng thái ban đầu cho clearScreen.
	/// </summary>
	public void clearScreen()
	{
		instance = null;
	}

	/// <summary>
	/// Thực thi hành động tương tác khi người dùng kích hoạt lệnh hoặc nút bấm.
	/// </summary>
	public void perform(int idAction, object p)
	{
		if (idAction == 1)
		{
			GameScr.instance.switchToMe();
			clearScreen();
		}
		if (idAction != 2)
		{
			return;
		}
		for (int i = 0; i < tf.Length; i++)
		{
			if (tf[i].getText() == null || tf[i].getText().Equals(string.Empty))
			{
				GameCanvas.startOKDlg(mResources.vuilongnhapduthongtin);
				return;
			}
		}
		Service.gI().sendClientInput(tf);
		GameScr.instance.switchToMe();
	}
}

/// <summary>
/// CLASS ChatTextField: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
public class ChatTextField : IActionListener
{
	private static ChatTextField instance;

	public TField tfChat;

	public bool isShow;

	public IChatable parentScreen;

	private long lastChatTime;

	public Command left;

	public Command cmdChat;

	public Command right;

	public Command center;

	private int x;

	private int y;

	private int w;

	private int h;

	private bool isPublic;

	public Command cmdChat2;

	public int yBegin;

	public int yUp;

	public int KC;

	public string to;

	public string strChat = "Chat ";

	/// <summary>
	/// Thực thi phương thức nghiệp vụ ChatTextField.
	/// </summary>
	public ChatTextField()
	{
		tfChat = new TField();
		if (Main.isWindowsPhone)
		{
			tfChat.showSubTextField = false;
		}
		if (Main.isIPhone)
		{
			tfChat.isPaintMouse = false;
		}
		tfChat.name = "chat";
		if (Main.isWindowsPhone)
		{
			tfChat.strInfo = tfChat.name;
		}
		tfChat.width = GameCanvas.w - 6;
		if (Main.isPC && tfChat.width > 300)
		{
			tfChat.width = 300;
		}
		tfChat.height = mScreen.ITEM_HEIGHT + 2;
		tfChat.x = GameCanvas.w / 2 - tfChat.width / 2;
		tfChat.isFocus = true;
		tfChat.setMaxTextLenght(250);
	}

	/// <summary>
	/// Khởi tạo và nạp dữ liệu cấu hình cho initChatTextField.
	/// </summary>
	public void initChatTextField()
	{
		left = new Command(mResources.OK, this, 8000, null, 1, GameCanvas.h - mScreen.cmdH + 1);
		right = new Command(mResources.DELETE, this, 8001, null, GameCanvas.w - 70, GameCanvas.h - mScreen.cmdH + 1);
		center = null;
		w = tfChat.width + 20;
		h = tfChat.height + 26;
		x = GameCanvas.w / 2 - w / 2;
		y = tfChat.y - 18;
		if (Main.isPC && w > 340)
		{
			w = 340;
		}
		left.x = x;
		right.x = x + w - 68;
		if (GameCanvas.isTouch)
		{
			tfChat.y -= 5;
			y -= 20;
			h += 30;
			left.x = GameCanvas.w / 2 - 68 - 5;
			right.x = GameCanvas.w / 2 + 5;
			left.y = GameCanvas.h - 30;
			right.y = GameCanvas.h - 30;
		}
		cmdChat = new Command();
		ActionChat actionChat = delegate(string str)
		{
			tfChat.justReturnFromTextBox = false;
			if (parentScreen != null)
			{
				tfChat.setText(str);
				parentScreen.onChatFromMe(str, to);
				tfChat.setText(string.Empty);
				tfChat.clearKb();
				right.caption = mResources.CLOSE;
			}
			isShow = false;
		};
		cmdChat.actionChat = actionChat;
		cmdChat2 = new Command();
		cmdChat2.actionChat = delegate(string str)
		{
			tfChat.justReturnFromTextBox = false;
			if (parentScreen != null)
			{
				tfChat.setText(str);
				parentScreen.onChatFromMe(str, to);
				tfChat.setText(string.Empty);
				tfChat.clearKb();
				if (right != null)
				{
					right.performAction();
				}
			}
			isShow = false;
		};
		yBegin = tfChat.y;
		yUp = GameCanvas.h / 2 - 2 * tfChat.height;
		if (Main.isWindowsPhone)
		{
			tfChat.showSubTextField = false;
		}
		if (Main.isIPhone)
		{
			tfChat.isPaintMouse = false;
		}
	}

	/// <summary>
	/// Cập nhật trạng thái logic của updateWhenKeyBoardVisible theo chu kỳ khung hình.
	/// </summary>
	public void updateWhenKeyBoardVisible()
	{
	}

	/// <summary>
	/// Tiếp nhận và xử lý sự kiện khi một phím trên bàn phím được nhấn xuống.
	/// </summary>
	public void keyPressed(int keyCode)
	{
		if (isShow)
		{
			tfChat.keyPressed(keyCode);
		}
		if (tfChat.getText().Equals(string.Empty))
		{
			right.caption = mResources.CLOSE;
		}
		else
		{
			right.caption = mResources.DELETE;
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ gI.
	/// </summary>
	public static ChatTextField gI()
	{
		return (instance != null) ? instance : (instance = new ChatTextField());
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ startChat.
	/// </summary>
	public void startChat(int firstCharacter, IChatable parentScreen, string to)
	{
		right.caption = mResources.CLOSE;
		this.to = to;
		if (Main.isWindowsPhone)
		{
			tfChat.showSubTextField = false;
		}
		if (Main.isIPhone)
		{
			tfChat.isPaintMouse = false;
		}
		tfChat.keyPressed(firstCharacter);
		if (!tfChat.getText().Equals(string.Empty) && GameCanvas.currentDialog == null)
		{
			this.parentScreen = parentScreen;
			isShow = true;
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ startChat.
	/// </summary>
	public void startChat(IChatable parentScreen, string to)
	{
		right.caption = mResources.CLOSE;
		this.to = to;
		this.parentScreen = parentScreen;
		if (Main.isWindowsPhone)
		{
			tfChat.showSubTextField = false;
		}
		if (Main.isIPhone)
		{
			tfChat.isPaintMouse = false;
		}
		tfChat.setText(string.Empty);
		tfChat.clearAll();
		if (GameCanvas.currentDialog == null)
		{
			isShow = true;
			tfChat.isFocus = true;
			if (!Main.isPC)
			{
				ipKeyboard.openKeyBoard(strChat, ipKeyboard.TEXT, string.Empty, cmdChat);
				tfChat.setFocusWithKb(isFocus: true);
			}
			else
			{
				tfChat.setFocusWithKb(isFocus: true);
			}
		}
		isPublic = false;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ startChat2.
	/// </summary>
	public void startChat2(IChatable parentScreen, string to)
	{
		this.to = to;
		this.parentScreen = parentScreen;
		if (Main.isWindowsPhone)
		{
			tfChat.showSubTextField = false;
		}
		if (Main.isIPhone)
		{
			tfChat.isPaintMouse = false;
		}
		tfChat.setText(string.Empty);
		tfChat.clearAll();
		if (GameCanvas.currentDialog == null)
		{
			isShow = true;
			if (!Main.isPC)
			{
				ipKeyboard.openKeyBoard(strChat, ipKeyboard.TEXT, string.Empty, cmdChat2);
				tfChat.setFocusWithKb(isFocus: true);
			}
			else
			{
				tfChat.setFocusWithKb(isFocus: true);
			}
		}
		isPublic = false;
	}

	/// <summary>
	/// Cập nhật trạng thái logic của updateKey theo chu kỳ khung hình.
	/// </summary>
	public void updateKey()
	{
	}

	/// <summary>
	/// Cập nhật trạng thái logic của đối tượng theo từng chu kỳ khung hình.
	/// </summary>
	public void update()
	{
		if (!isShow)
		{
			return;
		}
		tfChat.update();
		if (Main.isWindowsPhone)
		{
			updateWhenKeyBoardVisible();
		}
		if (tfChat.justReturnFromTextBox)
		{
			tfChat.justReturnFromTextBox = false;
			parentScreen.onChatFromMe(tfChat.getText(), to);
			tfChat.setText(string.Empty);
			right.caption = mResources.CLOSE;
		}
		if (!Main.isPC)
		{
			return;
		}
		if (GameCanvas.keyPressed[15])
		{
			if (left != null && tfChat.getText() != string.Empty)
			{
				left.performAction();
			}
			GameCanvas.keyPressed[15] = false;
			GameCanvas.keyPressed[(!Main.isPC) ? 5 : 25] = false;
		}
		if (GameCanvas.keyPressed[14])
		{
			if (right != null)
			{
				right.performAction();
			}
			GameCanvas.keyPressed[14] = false;
		}
	}

	/// <summary>
	/// Đóng cửa sổ hoặc giải phóng phiên làm việc hiện tại.
	/// </summary>
	public void close()
	{
		tfChat.setText(string.Empty);
		isShow = false;
		TouchScreenKeyboard.Clear();
		if (parentScreen != null)
		{
			parentScreen.onCancelChat();
		}
	}

	/// <summary>
	/// Đóng gói và gửi thông điệp mạng sendChat lên máy chủ.
	/// </summary>
	public void sendChat()
	{
		if (left != null && !tfChat.getText().Equals(string.Empty))
		{
			left.performAction();
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ pasteText.
	/// </summary>
	public void pasteText(string clip)
	{
		if (string.IsNullOrEmpty(clip))
		{
			return;
		}
		tfChat.suspendTelex = true;
		try
		{
			for (int i = 0; i < clip.Length; i++)
			{
				char c = clip[i];
				if (c >= ' ' && c != 127)
				{
					tfChat.keyPressed((int)c);
				}
			}
		}
		finally
		{
			tfChat.suspendTelex = false;
		}
		if (tfChat.getText().Equals(string.Empty))
		{
			right.caption = mResources.CLOSE;
		}
		else
		{
			right.caption = mResources.DELETE;
		}
	}

	/// <summary>
	/// Thực hiện vẽ toàn bộ đối tượng và các lớp hiển thị lên canvas mGraphics.
	/// </summary>
	public void paint(mGraphics g)
	{
		if (isShow && !Main.isIPhone)
		{
			int num = ((!Main.isWindowsPhone) ? (y - KC) : (tfChat.y - 5));
			int num2 = ((!Main.isWindowsPhone) ? x : 0);
			int num3 = ((!Main.isWindowsPhone) ? w : GameCanvas.w);
			PopUp.paintPopUp(g, num2, num, num3, h, -1, isButton: true);
			mFont.tahoma_7b_green2.drawString(g, strChat + to, tfChat.x, tfChat.y - ((!GameCanvas.isTouch) ? 12 : 17), 0);
			GameCanvas.paintz.paintCmdBar(g, left, center, right);
			tfChat.paint(g);
		}
	}

	/// <summary>
	/// Thực thi hành động tương tác khi người dùng kích hoạt lệnh hoặc nút bấm.
	/// </summary>
	public void perform(int idAction, object p)
	{
		switch (idAction)
		{
		case 8000:
			Cout.LogError("perform chat 8000");
			if (parentScreen != null)
			{
				long num = mSystem.currentTimeMillis();
				if (num - lastChatTime >= 1000)
				{
					lastChatTime = num;
					parentScreen.onChatFromMe(tfChat.getText(), to);
					tfChat.setText(string.Empty);
					right.caption = mResources.CLOSE;
					tfChat.clearKb();
					TouchScreenKeyboard.Clear();
					if (!Main.isPC)
					{
						isShow = false;
					}
				}
			}
			break;
		case 8001:
			Cout.LogError("perform chat 8001");
			if (tfChat.getText().Equals(string.Empty))
			{
				isShow = false;
				TouchScreenKeyboard.Clear();
				parentScreen.onCancelChat();
			}
			tfChat.clear();
			break;
		case 8002:
			break;
		}
	}
}

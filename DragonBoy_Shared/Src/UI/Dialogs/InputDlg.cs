/// <summary>
/// CLASS InputDlg: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
public class InputDlg : Dialog
{
	protected string[] info;

	public TField tfInput;

	private int padLeft;

	/// <summary>
	/// Thực thi phương thức nghiệp vụ InputDlg.
	/// </summary>
	public InputDlg()
	{
		padLeft = 40;
		if (GameCanvas.w <= 176)
		{
			padLeft = 10;
		}
		tfInput = new TField();
		tfInput.x = padLeft + 10;
		tfInput.y = GameCanvas.h - mScreen.ITEM_HEIGHT - 43;
		tfInput.width = GameCanvas.w - 2 * (padLeft + 10);
		tfInput.height = mScreen.ITEM_HEIGHT + 2;
		tfInput.isFocus = true;
		right = tfInput.cmdClear;
	}

	/// <summary>
	/// Hiển thị thành phần giao diện hoặc bảng thông báo lên màn hình.
	/// </summary>
	public void show(string info, Command ok, int type)
	{
		tfInput.setText(string.Empty);
		tfInput.setIputType(type);
		this.info = mFont.tahoma_8b.splitFontArray(info, GameCanvas.w - padLeft * 2);
		left = new Command(mResources.CLOSE, GameCanvas.gI(), 8882, null);
		center = ok;
		show();
		if (!Main.isPC)
		{
			tfInput.setFocusWithKb(isFocus: true);
		}
		else
		{
			tfInput.setFocus(isFocus: true);
		}
	}

	/// <summary>
	/// Thực hiện vẽ toàn bộ đối tượng và các lớp hiển thị lên canvas mGraphics.
	/// </summary>
	public override void paint(mGraphics g)
	{
		GameCanvas.paintz.paintInputDlg(g, padLeft, GameCanvas.h - 77 - mScreen.cmdH, GameCanvas.w - padLeft * 2, 69, info);
		tfInput.paint(g);
		base.paint(g);
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ keyPress.
	/// </summary>
	public override void keyPress(int keyCode)
	{
		tfInput.keyPressed(keyCode);
		base.keyPress(keyCode);
	}

	/// <summary>
	/// Cập nhật trạng thái logic của đối tượng theo từng chu kỳ khung hình.
	/// </summary>
	public override void update()
	{
		tfInput.update();
		base.update();
	}

	/// <summary>
	/// Hiển thị thành phần giao diện hoặc bảng thông báo lên màn hình.
	/// </summary>
	public override void show()
	{
		GameCanvas.currentDialog = this;
	}

	/// <summary>
	/// Ẩn thành phần giao diện khỏi màn hình hiển thị.
	/// </summary>
	public void hide()
	{
		GameCanvas.endDlg();
	}
}

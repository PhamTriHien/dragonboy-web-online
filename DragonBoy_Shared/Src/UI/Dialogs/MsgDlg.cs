/// <summary>
/// CLASS MsgDlg: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
public class MsgDlg : Dialog
{
	public string[] info;

	public bool isWait;

	private int h;

	private int padLeft;

	private long time = -1L;

	/// <summary>
	/// Thực thi phương thức nghiệp vụ MsgDlg.
	/// </summary>
	public MsgDlg()
	{
		padLeft = 35;
		if (GameCanvas.w <= 176)
		{
			padLeft = 10;
		}
		if (GameCanvas.w > 320)
		{
			padLeft = 80;
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ pleasewait.
	/// </summary>
	public void pleasewait()
	{
		setInfo(mResources.PLEASEWAIT, null, null, null);
		GameCanvas.currentDialog = this;
		time = mSystem.currentTimeMillis() + 5000;
	}

	/// <summary>
	/// Hiển thị thành phần giao diện hoặc bảng thông báo lên màn hình.
	/// </summary>
	public override void show()
	{
		GameCanvas.currentDialog = this;
		time = -1L;
	}

	/// <summary>
	/// Thiết lập giá trị hoặc trạng thái cho setInfo.
	/// </summary>
	public void setInfo(string info)
	{
		try
		{
			ModBossNotice.ProcessServerBossNotice(info);
		}
		catch
		{
		}
		this.info = mFont.tahoma_8b.splitFontArray(info, GameCanvas.w - (padLeft * 2 + 20));
		h = 80;
		if (this.info.Length >= 5)
		{
			h = this.info.Length * mFont.tahoma_8b.getHeight() + 20;
		}
	}

	/// <summary>
	/// Thiết lập giá trị hoặc trạng thái cho setInfo.
	/// </summary>
	public void setInfo(string info, Command left, Command center, Command right)
	{
		try
		{
			ModBossNotice.ProcessServerBossNotice(info);
		}
		catch
		{
		}
		this.info = mFont.tahoma_8b.splitFontArray(info, GameCanvas.w - (padLeft * 2 + 20));
		base.left = left;
		base.center = center;
		base.right = right;
		h = 80;
		if (this.info.Length >= 5)
		{
			h = this.info.Length * mFont.tahoma_8b.getHeight() + 20;
		}
		if (GameCanvas.isTouch)
		{
			if (left != null)
			{
				base.left.x = GameCanvas.w / 2 - 68 - 5;
				base.left.y = GameCanvas.h - 50;
			}
			if (right != null)
			{
				base.right.x = GameCanvas.w / 2 + 5;
				base.right.y = GameCanvas.h - 50;
			}
			if (center != null)
			{
				base.center.x = GameCanvas.w / 2 - 35;
				base.center.y = GameCanvas.h - 50;
			}
		}
		isWait = false;
		time = -1L;
	}

	/// <summary>
	/// Thực hiện vẽ toàn bộ đối tượng và các lớp hiển thị lên canvas mGraphics.
	/// </summary>
	public override void paint(mGraphics g)
	{
		g.setClip(0, 0, GameCanvas.w, GameCanvas.h);
		if (!LoginScr.isContinueToLogin)
		{
			int num = GameCanvas.h - h - 38;
			int w = GameCanvas.w - padLeft * 2;
			GameCanvas.paintz.paintPopUp(padLeft, num, w, h, g);
			int num2 = num + (h - info.Length * mFont.tahoma_8b.getHeight()) / 2 - 2;
			if (isWait)
			{
				num2 += 8;
				GameCanvas.paintShukiren(GameCanvas.hw, num2 - 12, g);
			}
			int num3 = 0;
			int num4 = num2;
			while (num3 < info.Length)
			{
				mFont.tahoma_7b_dark.drawString(g, info[num3], GameCanvas.hw, num4, 2);
				num3++;
				num4 += mFont.tahoma_8b.getHeight();
			}
			base.paint(g);
		}
	}

	/// <summary>
	/// Cập nhật trạng thái logic của đối tượng theo từng chu kỳ khung hình.
	/// </summary>
	public override void update()
	{
		base.update();
		if (time != -1 && mSystem.currentTimeMillis() > time)
		{
			GameCanvas.endDlg();
		}
	}
}

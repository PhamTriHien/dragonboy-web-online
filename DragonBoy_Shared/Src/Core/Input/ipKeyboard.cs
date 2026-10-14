using System;

/// <summary>
/// CLASS ipKeyboard: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
public class ipKeyboard
{
	private static TouchScreenKeyboard tk;

	public static int TEXT;

	public static int NUMBERIC = 1;

	public static int PASS = 2;

	private static Command act;

	/// <summary>
	/// Thực thi phương thức nghiệp vụ openKeyBoard.
	/// </summary>
	public static void openKeyBoard(string caption, int type, string text, Command action)
	{
		act = action;
		TouchScreenKeyboardType t = ((type == 0 || type == 2) ? TouchScreenKeyboardType.ASCIICapable : TouchScreenKeyboardType.NumberPad);
		TouchScreenKeyboard.hideInput = false;
		tk = TouchScreenKeyboard.Open(text, t, b1: false, b2: false, type == 2, b3: false, caption);
	}

	/// <summary>
	/// Cập nhật trạng thái logic của đối tượng theo từng chu kỳ khung hình.
	/// </summary>
	public static void update()
	{
		try
		{
			if (tk != null && tk.done)
			{
				if (act != null)
				{
					act.perform(tk.text);
				}
				tk.text = string.Empty;
				tk = null;
			}
		}
		catch (Exception)
		{
		}
	}
}

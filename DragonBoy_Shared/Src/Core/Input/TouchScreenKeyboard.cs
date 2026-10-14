using System;

/// <summary>
/// CLASS TouchScreenKeyboard: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
public class TouchScreenKeyboard
{
	public static bool hideInput;

	public static bool visible;

	public bool done;

	public bool active;

	public string text = string.Empty;

	public static Func<string, TouchScreenKeyboardType, bool, bool, bool, bool, string, TouchScreenKeyboard> OpenHandler;

	public static Action ClearHandler;

	/// <summary>
	/// Thực thi phương thức nghiệp vụ Open.
	/// </summary>
	public static TouchScreenKeyboard Open(string text, TouchScreenKeyboardType t, bool b1, bool b2, bool type, bool b3, string caption)
	{
		if (OpenHandler != null)
		{
			return OpenHandler(text, t, b1, b2, type, b3, caption);
		}
		visible = false;
		return new TouchScreenKeyboard
		{
			active = true,
			done = false,
			text = text ?? string.Empty
		};
	}

	/// <summary>
	/// Dọn dẹp và đặt lại trạng thái ban đầu cho Clear.
	/// </summary>
	public static void Clear()
	{
		visible = false;
		ClearHandler?.Invoke();
	}
}


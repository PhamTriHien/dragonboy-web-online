using System;

/// <summary>
/// CLASS Timer: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
public class Timer
{
	public static IActionListener timeListener;

	public static int idAction;

	public static long timeExecute;

	public static bool isON;

	/// <summary>
	/// Thiết lập giá trị hoặc trạng thái cho setTimer.
	/// </summary>
	public static void setTimer(IActionListener actionListener, int action, long timeEllapse)
	{
		timeListener = actionListener;
		idAction = action;
		timeExecute = mSystem.currentTimeMillis() + timeEllapse;
		isON = true;
	}

	/// <summary>
	/// Cập nhật trạng thái logic của đối tượng theo từng chu kỳ khung hình.
	/// </summary>
	public static void update()
	{
		long num = mSystem.currentTimeMillis();
		if (!isON || num <= timeExecute)
		{
			return;
		}
		isON = false;
		try
		{
			if (idAction > 0)
			{
				GameScr.gI().actionPerform(idAction, null);
			}
		}
		catch (Exception)
		{
		}
	}
}

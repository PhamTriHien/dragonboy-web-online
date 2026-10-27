using UnityEngine;

/// <summary>
/// CLASS GameMidlet: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
public class GameMidlet
{
	public static string IP = "112.213.94.23";

	public static int PORT = 14445;

	public static string IP2;

	public static int PORT2;

	public static sbyte PROVIDER;

	public static int LANGUAGE;

	public static string VERSION = "2.5.2";

	public static int intVERSION = 252;

	public static GameCanvas gameCanvas;

	public static GameMidlet instance;

	public static bool isConnect2;

	public static bool isBackWindowsPhone;

	/// <summary>
	/// Thực thi phương thức nghiệp vụ GameMidlet.
	/// </summary>
	public GameMidlet()
	{
		initGame();
	}

	/// <summary>
	/// Khởi tạo và nạp dữ liệu cấu hình cho initGame.
	/// </summary>
	public void initGame()
	{
		instance = this;
		MotherCanvas.instance = new MotherCanvas();
		Session_ME.gI().setHandler(Controller.gI());
		Session_ME2.gI().setHandler(Controller.gI());
		Session_ME2.isMainSession = false;
		instance = this;
		gameCanvas = new GameCanvas();
		gameCanvas.start();
		SplashScr.loadImg();
		SplashScr.loadSplashScr();
		GameCanvas.currentScreen = new SplashScr();
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ exit.
	/// </summary>
	public void exit()
	{
		GameCanvas.bRun = false;
		mSystem.gcc();
		notifyDestroyed();
	}

	/// <summary>
	/// Đóng gói và gửi thông điệp mạng sendSMS lên máy chủ.
	/// </summary>
	public static void sendSMS(string data, string to, Command successAction, Command failAction)
	{
		Cout.println("SEND SMS");
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ flatForm.
	/// </summary>
	public static void flatForm(string url)
	{
		Cout.LogWarning("PLATFORM REQUEST: " + url);
		Application.OpenURL(url);
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ notifyDestroyed.
	/// </summary>
	public void notifyDestroyed()
	{
		Main.exit();
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ platformRequest.
	/// </summary>
	public void platformRequest(string url)
	{
		Cout.LogWarning("PLATFORM REQUEST: " + url);
		Application.OpenURL(url);
	}
}

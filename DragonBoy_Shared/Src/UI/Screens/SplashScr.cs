/// <summary>
/// CLASS SplashScr: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
public class SplashScr : mScreen
{
	public static int splashScrStat;

	private bool isCheckConnect;

	private bool isSwitchToLogin;

	public bool isDoneSplash;

	public static int nData = -1;

	public static int maxData = -1;

	public static SplashScr instance;

	public static Image imgLogo;

	private int timeLoading = 10;

	public long TIMEOUT;

	/// <summary>
	/// Thực thi phương thức nghiệp vụ SplashScr.
	/// </summary>
	public SplashScr()
	{
		instance = this;
	}

	/// <summary>
	/// Khởi tạo và nạp dữ liệu cấu hình cho loadSplashScr.
	/// </summary>
	public static void loadSplashScr()
	{
		splashScrStat = 0;
		if (instance != null)
		{
			instance.isDoneSplash = false;
		}
	}

	/// <summary>
	/// Cập nhật trạng thái logic của đối tượng theo từng chu kỳ khung hình.
	/// </summary>
	public override void update()
	{
		splashScrStat++;
		if (splashScrStat == 10 && !isCheckConnect)
		{
			isCheckConnect = true;
			if (Rms.loadRMSInt("serverchat") != -1)
			{
				GameScr.isPaintChatVip = Rms.loadRMSInt("serverchat") == 0;
			}
			if (Rms.loadRMSInt("isPlaySound") != -1)
			{
				GameCanvas.isPlaySound = Rms.loadRMSInt("isPlaySound") == 1;
			}
			if (GameCanvas.isPlaySound)
			{
				SoundMn.gI().loadSound(TileMap.mapID);
			}
			SoundMn.gI().getStrOption();
			ServerListScreen.loadIP();
			if (UnityEngine.Application.CurrentTabId == 1)
			{
				ModAutoUpdate.ResetAndCheckOnLaunch();
			}
		}
		if (splashScrStat >= 25 && !isSwitchToLogin)
		{
			isSwitchToLogin = true;
			isDoneSplash = true;
			if (GameCanvas.serverScreen == null)
			{
				GameCanvas.serverScreen = new ServerListScreen();
			}
			ServerListScreen.loadScreen = true;
			ServerListScreen.bigOk = true;
			GameCanvas.serverScreen.switchToMe();
		}
		ServerListScreen.updateDeleteData();
	}

	/// <summary>
	/// Khởi tạo và nạp dữ liệu cấu hình cho loadIP.
	/// </summary>
	public static void loadIP()
	{
		ModProxyServer.InjectCustomServer();
		int sv = Rms.loadRMSInt(ServerListScreen.RMS_svselect);
		Res.err(">>>>>loadIP:  svselect == " + sv + "  clientType:" + mSystem.clientType);
		if (sv >= 0 && (ServerListScreen.nameServer == null || sv < ServerListScreen.nameServer.Length))
		{
			ServerListScreen.SetIpSelect(sv, issave: false);
		}
		else
		{
			int fallback = (ServerListScreen.serverPriority >= 0 && ServerListScreen.nameServer != null && ServerListScreen.serverPriority < ServerListScreen.nameServer.Length)
				? ServerListScreen.serverPriority : 0;
			ServerListScreen.SetIpSelect(fallback, issave: true);
		}
		if (ServerListScreen.isGetData || ServerListScreen.waitToLogin ||
			DragonBoy_Net8_Native.Src.Mod.Security.ModCredentialSecurity.isLoggingIn)
		{
			ServerListScreen.ConnectIP();
		}
	}

	/// <summary>
	/// Thực hiện vẽ toàn bộ đối tượng và các lớp hiển thị lên canvas mGraphics.
	/// </summary>
	public override void paint(mGraphics g)
	{
		if (imgLogo != null)
		{
			g.setColor(16777215);
			g.fillRect(0, 0, GameCanvas.w, GameCanvas.h);
			g.drawImage(imgLogo, GameCanvas.w / 2, GameCanvas.h / 2, 3);
		}
		else
		{
			g.setColor(0);
			g.fillRect(0, 0, GameCanvas.w, GameCanvas.h);
			GameCanvas.paintShukiren(GameCanvas.hw, GameCanvas.hh, g);
			ServerListScreen.paintDeleteData(g);
		}
	}

	/// <summary>
	/// Khởi tạo và nạp dữ liệu cấu hình cho loadImg.
	/// </summary>
	public static void loadImg()
	{
		if (imgLogo == null)
		{
			Image customLogo = GameCanvas.loadCustomImage("custom_logo.png");
			if (customLogo != null)
			{
				imgLogo = customLogo;
			}
			else
			{
				imgLogo = GameCanvas.loadImage("/gamelogo.png");
			}
		}
	}
}

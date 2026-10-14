using System;
using Assets.src.e;
using Assets.src.g;
using UnityEngine;
/// <summary>
/// Thành phần phân mảnh (partial) của lớp GameCanvas: Đảm nhiệm phân hệ chuyên trách Part1 ().
/// </summary>
public partial class GameCanvas : IActionListener
{
	/// <summary>
	/// Thực thi phương thức nghiệp vụ GameCanvas.
	/// </summary>
	public GameCanvas()
		{
			switch (Rms.loadRMSInt("languageVersion"))
			{
			case -1:
				Rms.saveRMSInt("languageVersion", 2);
				break;
			default:
				Main.main.doClearRMS();
				Rms.saveRMSInt("languageVersion", 2);
				break;
			case 2:
				break;
			}
			clearOldData = Rms.loadRMSInt(GameMidlet.VERSION);
			if (clearOldData != 1)
			{
				Main.main.doClearRMS();
				Rms.saveRMSInt(GameMidlet.VERSION, 1);
			}
			initGame();
		}
	/// <summary>
	/// Truy xuất và trả về dữ liệu của getPlatformName.
	/// </summary>
	public static string getPlatformName()
	{
		return "Pc platform xxx";
	}
	/// <summary>
	/// Khởi tạo và nạp dữ liệu cấu hình cho initGame.
	/// </summary>
	public void initGame()
		{
			try
			{
				MotherCanvas.instance.setChildCanvas(this);
				w = MotherCanvas.instance.getWidthz();
				h = MotherCanvas.instance.getHeightz();
				hw = w / 2;
				hh = h / 2;
				isTouch = true;
				if (w >= 240)
				{
					isTouchControl = true;
				}
				if (w < 320)
				{
					isTouchControlSmallScreen = true;
				}
				if (w >= 320)
				{
					isTouchControlLargeScreen = true;
				}
				msgdlg = new MsgDlg();
				if (h <= 160)
				{
					Paint.hTab = 15;
					mScreen.cmdH = 17;
				}
				GameScr.d = ((w <= h) ? h : w) + 20;
				instance = this;
				mFont.init();
				mScreen.ITEM_HEIGHT = mFont.tahoma_8b.getHeight() + 8;
				initPaint();
				loadDust();
				loadWaterSplash();
				panel = new Panel();
				imgShuriken = loadImage("/mainImage/myTexture2df.png");
				int num = Rms.loadRMSInt(Rms.RMS_clienttype);
				if (num != -1)
				{
					mSystem.clientType = num;
				}
				else if (!Main.isPC)
				{
					mSystem.clientType = 2;
				}
				if (mSystem.clientType == 7 && (Rms.loadRMSString("fake") == null || Rms.loadRMSString("fake") == string.Empty))
				{
					imgShuriken = loadImage("/mainImage/wait.png");
				}
				imgClear = loadImage("/mainImage/myTexture2der.png");
				img18 = loadImage("/mainImage/18+.png");
				debugUpdate = new MyVector();
				debugPaint = new MyVector();
				debugSession = new MyVector();
				for (int i = 0; i < 3; i++)
				{
					imgBorder[i] = loadImage("/mainImage/myTexture2dbd" + i + ".png");
				}
				borderConnerW = mGraphics.getImageWidth(imgBorder[0]);
				borderConnerH = mGraphics.getImageHeight(imgBorder[0]);
				borderCenterW = mGraphics.getImageWidth(imgBorder[1]);
				borderCenterH = mGraphics.getImageHeight(imgBorder[1]);
				Panel.graphics = Rms.loadRMSInt("lowGraphic");
				lowGraphic = Rms.loadRMSInt("lowGraphic") == 1;
				GameScr.isPaintChatVip = Rms.loadRMSInt("serverchat") != 1;
				Char.isPaintAura = Rms.loadRMSInt("isPaintAura") == 1;
				Char.isPaintAura2 = Rms.loadRMSInt("isPaintAura2") == 1;
				Res.init();
				SmallImage.loadBigImage();
				Panel.WIDTH_PANEL = 176;
				if (Panel.WIDTH_PANEL > w)
				{
					Panel.WIDTH_PANEL = w;
				}
				InfoMe.gI().loadCharId();
				Command.btn0left = loadImage("/mainImage/btn0left.png");
				Command.btn0mid = loadImage("/mainImage/btn0mid.png");
				Command.btn0right = loadImage("/mainImage/btn0right.png");
				Command.btn1left = loadImage("/mainImage/btn1left.png");
				Command.btn1mid = loadImage("/mainImage/btn1mid.png");
				Command.btn1right = loadImage("/mainImage/btn1right.png");
				serverScreen = new ServerListScreen();
				img18 = loadImage("/mainImage/18+.png");
				for (int j = 0; j < 7; j++)
				{
					imgBlue[j] = loadImage("/effectdata/blue/" + j + ".png");
					imgViolet[j] = loadImage("/effectdata/violet/" + j + ".png");
				}
				ServerListScreen.createDeleteRMS();
				serverScr = new ServerScr();
				loginScr = new LoginScr();
				_SelectCharScr = new SelectCharScr();
			}
			catch (Exception)
			{
				Debug.LogError("----------------->>>>>>>>>>errr");
			}
		}
	/// <summary>
	/// Thực thi phương thức nghiệp vụ gI.
	/// </summary>
	public static GameCanvas gI()
		{
			return instance;
		}
	/// <summary>
	/// Khởi tạo và nạp dữ liệu cấu hình cho initPaint.
	/// </summary>
	public void initPaint()
		{
			paintz = new Paint();
		}
	/// <summary>
	/// Bộ xử lý sự kiện tiếp nhận và phản hồi onDisconnected.
	/// </summary>
	public void onDisconnected()
		{
			if (Controller.isConnectionFail)
			{
				Controller.isConnectionFail = false;
			}
			isResume = true;
			Session_ME.gI().clearSendingMessage();
			Session_ME2.gI().clearSendingMessage();
			Session_ME.gI().close();
			Session_ME2.gI().close();
			if (Controller.isLoadingData)
			{
				startOK(mResources.pls_restart_game_error, 8885, null);
				Controller.isDisconnected = false;
				return;
			}
			Debug.LogError(">>>>onDisconnected");
			ModAutoLogin.OnDisconnected();
			if (currentScreen != serverScreen)
			{
				DragonBoy_Net8_Native.Src.Mod.Security.ModCredentialSecurity.OnLoginFinished();
				serverScreen.switchToMe();
				startOK(mResources.maychutathoacmatsong + " [4]", 8884, null);
				ServerListScreen.waitToLogin = true;
				ServerListScreen.tWaitToLogin = 0;
			}
			else
			{
				ServerListScreen.count_reConnect = mSystem.currentTimeMillis() + 5000;
				if (DragonBoy_Net8_Native.Src.Mod.Security.ModCredentialSecurity.isLoggingIn)
				{
					DragonBoy_Net8_Native.Src.Mod.Security.ModCredentialSecurity.OnLoginFinished();
					startOK(mResources.maychutathoacmatsong + " [4]", 8884, null);
				}
				else
				{
					endDlg();
				}
			}
			Char.isLoadingMap = false;
			if (Controller.isMain)
			{
				ServerListScreen.testConnect = 0;
			}
			mSystem.endKey();
		}
	/// <summary>
	/// Bộ xử lý sự kiện tiếp nhận và phản hồi onConnectionFail.
	/// </summary>
	public void onConnectionFail()
		{
			if (currentScreen.Equals(SplashScr.instance))
			{
				startOK(mResources.maychutathoacmatsong + " [1]", 8884, null);
				return;
			}
			Session_ME.gI().clearSendingMessage();
			Session_ME2.gI().clearSendingMessage();
			ServerListScreen.isWait = false;
			if (Controller.isLoadingData)
			{
				DragonBoy_Net8_Native.Src.Mod.Security.ModCredentialSecurity.OnLoginFinished();
				startOK(mResources.maychutathoacmatsong + " [2]", 8884, null);
				Controller.isConnectionFail = false;
				return;
			}
			isResume = true;
			ModAutoLogin.OnDisconnected();
			LoginScr.isContinueToLogin = false;
			if (ServerListScreen.nameServer != null && ServerListScreen.ipSelect >= 0 && ServerListScreen.ipSelect < ServerListScreen.nameServer.Length)
			{
				LoginScr.serverName = ServerListScreen.nameServer[ServerListScreen.ipSelect];
			}
			if (currentScreen != serverScreen)
			{
				ServerListScreen.countDieConnect = 0;
				if (DragonBoy_Net8_Native.Src.Mod.Security.ModCredentialSecurity.isLoggingIn)
				{
					DragonBoy_Net8_Native.Src.Mod.Security.ModCredentialSecurity.OnLoginFinished();
					startOK(mResources.maychutathoacmatsong + " [3]", 8884, null);
				}
			}
			else
			{
				ServerListScreen.loadScreen = true;
				ServerListScreen.count_reConnect = mSystem.currentTimeMillis() + 5000;
				if (DragonBoy_Net8_Native.Src.Mod.Security.ModCredentialSecurity.isLoggingIn)
				{
					DragonBoy_Net8_Native.Src.Mod.Security.ModCredentialSecurity.OnLoginFinished();
					startOK(mResources.maychutathoacmatsong + " [3]", 8884, null);
				}
				else
				{
					endDlg();
				}
			}
			Char.isLoadingMap = false;
			if (Controller.isMain)
			{
				ServerListScreen.testConnect = 0;
			}
			mSystem.endKey();
		}
	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của isWaiting.
	/// </summary>
	public static bool isWaiting()
		{
			if (InfoDlg.isShow || (msgdlg != null && msgdlg.info.Equals(mResources.PLEASEWAIT)) || Char.isLoadingMap || LoginScr.isContinueToLogin)
			{
				return true;
			}
			return false;
		}
	/// <summary>
	/// Khởi tạo và thiết lập kết nối socket TCP tới máy chủ trò chơi.
	/// </summary>
	public static void connect()
		{
			if (ServerListScreen.address != null && ServerListScreen.ipSelect >= 0 && ServerListScreen.ipSelect < ServerListScreen.address.Length)
			{
				GameMidlet.IP = ServerListScreen.address[ServerListScreen.ipSelect];
				GameMidlet.PORT = ServerListScreen.port[ServerListScreen.ipSelect];
			}
			if (!Session_ME.gI().isConnected())
			{
				Session_ME.gI().connect(GameMidlet.IP, GameMidlet.PORT);
			}
		}
	/// <summary>
	/// Thực thi phương thức nghiệp vụ connect2.
	/// </summary>
	public static void connect2()
		{
			if (!Session_ME2.gI().isConnected())
			{
				Res.outz("IP2= " + GameMidlet.IP2 + " PORT 2= " + GameMidlet.PORT2);
				Session_ME2.gI().connect(GameMidlet.IP2, GameMidlet.PORT2);
			}
		}
	/// <summary>
	/// Dọn dẹp và đặt lại trạng thái ban đầu cho resetTrans.
	/// </summary>
	public static void resetTrans(mGraphics g)
		{
			g.translate(-g.getTranslateX(), -g.getTranslateY());
			g.setClip(0, 0, w, h);
		}
	/// <summary>
	/// Dọn dẹp và đặt lại trạng thái ban đầu cho resetTransGameScr.
	/// </summary>
	public static void resetTransGameScr(mGraphics g)
		{
			g.translate(-g.getTranslateX(), -g.getTranslateY());
			g.translate(0, 0);
			g.setClip(0, 0, w, h);
			g.translate(-GameScr.cmx, -GameScr.cmy);
		}
	/// <summary>
	/// Khởi tạo các thành phần giao diện và kích thước hiển thị của canvas game.
	/// </summary>
	public void initGameCanvas()
		{
			debug("SP2i1", 0);
			w = MotherCanvas.instance.getWidthz();
			h = MotherCanvas.instance.getHeightz();
			debug("SP2i2", 0);
			hw = w / 2;
			hh = h / 2;
			wd3 = w / 3;
			hd3 = h / 3;
			w2d3 = 2 * w / 3;
			h2d3 = 2 * h / 3;
			w3d4 = 3 * w / 4;
			h3d4 = 3 * h / 4;
			wd6 = w / 6;
			hd6 = h / 6;
			debug("SP2i3", 0);
			mScreen.initPos();
			debug("SP2i4", 0);
			debug("SP2i5", 0);
			inputDlg = new InputDlg();
			debug("SP2i6", 0);
			listPoint = new MyVector();
			debug("SP2i7", 0);
		}
	/// <summary>
	/// Bắt đầu khởi chạy tiến trình hoặc luồng xử lý nền.
	/// </summary>
	public void start()
		{
		}
	/// <summary>
	/// Truy xuất và trả về dữ liệu của getWidth.
	/// </summary>
	public int getWidth()
		{
			return (int)ScaleGUI.WIDTH;
		}
	/// <summary>
	/// Truy xuất và trả về dữ liệu của getHeight.
	/// </summary>
	public int getHeight()
		{
			return (int)ScaleGUI.HEIGHT;
		}
	/// <summary>
	/// Thực thi phương thức nghiệp vụ debug.
	/// </summary>
	public static void debug(string s, int type)
		{
		}

}

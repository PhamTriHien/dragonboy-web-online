using System.Net.NetworkInformation;
using System.Threading;
using UnityEngine;

/// <summary>
/// Lớp khởi chạy vòng đời ứng dụng trên Unity/Host: Cấu hình FPS, kiểm tra nền tảng và khởi tạo MotherCanvas.
/// </summary>
public class Main : MonoBehaviour
{
	public static Main main;

	public static mGraphics g;

	public static GameMidlet midlet;

	public static string res = "res";

	public static string mainThreadName;

	public static bool started;

	public static bool isIpod;

	public static bool isIphone4;

	public static bool isPC = true;

	public static bool isWindowsPhone;

	public static bool isIPhone;

	public static bool IphoneVersionApp;

	public static string IMEI;

	public static int versionIp;

	public static int numberQuit = 1;

	private int level;

	private int updateCount;

	private int paintCount;

	private int count;

	private int fps;

	private int max;
	public static int realFPS = 60;

	private int up;

	private int upmax;

	private long timefps;

	private long timeup;

	private bool isRun;

	public static int waitTick;

	public static int f;

	public static bool isResume;

	public static bool isMiniApp = true;

	public static bool isQuitApp;

	private Vector2 lastMousePos = default(Vector2);

	public static int a = 1;

	public static bool isCompactDevice = true;

	/// <summary>
	/// Bắt đầu khởi chạy tiến trình hoặc luồng xử lý nền.
	/// </summary>
	public void Start()
	{
		Debug.LogWarning("MAIN.START CALLED");
		mSystem.isTest = true; // Kích hoạt log Engine & Network gốc vào output_log.txt
		Application.runInBackground = true; // DUY TRÌ GAME CHẠY LIÊN TỤC 24/7 KHI THU NHỎ HOẶC CHẠY DƯỚI NỀN
		GameTabRegistry.EnsureTab(Application.CurrentTabId);
		ModConfig.LoadConfig();
		ModTdltLogger.InitSession();
		ModFps.LoadFPS();
		if (started)
		{
			return;
		}
		try
		{
			if (Thread.CurrentThread.Name != "Main")
			{
				Thread.CurrentThread.Name = "Main";
			}
		}
		catch
		{
		}
		mainThreadName = Thread.CurrentThread.Name;
#if ANDROID || __ANDROID__
		isPC = false;
#else
		isPC = !System.OperatingSystem.IsAndroid() && !System.OperatingSystem.IsIOS() && !(DragonBoy_Net8_Native.Engine.Platform.PlatformService.Current?.IsTouchDevice ?? false);
#endif
		if (!isPC)
		{
			GameCanvas.isTouch = true;
			GameCanvas.isTouchControl = true;
			mSystem.clientType = 2;
		}
		started = true;
		if (isPC)
		{
			try
			{
				ModGraphics.InitGraphics();
			}
			catch (System.Exception ex)
			{
				Debug.LogWarning("ModGraphics.InitGraphics error: " + ex.Message);
			}
		}
		try
		{
			ModBackground.Init();
		}
		catch (System.Exception ex)
		{
			Debug.LogWarning("ModBackground.Init error: " + ex.Message);
		}
		Debug.LogWarning("MAIN.START FINISHED");
	}

	/// <summary>
	/// Thiết lập giá trị hoặc trạng thái cho SetInit.
	/// </summary>
	private void SetInit()
	{
		base.enabled = true;
	}

	/// <summary>
	/// Bộ xử lý sự kiện tiếp nhận và phản hồi OnHideUnity.
	/// </summary>
	private void OnHideUnity(bool isGameShown)
	{
		// Luôn duy trì Time.timeScale = 1f để nhân vật tiếp tục di chuyển và nhận gói tin mạng
		Time.timeScale = 1f;
	}

	/// <summary>
	/// Bộ xử lý sự kiện tiếp nhận và phản hồi OnApplicationFocus.
	/// </summary>
	private void OnApplicationFocus(bool hasFocus)
	{
		Application.runInBackground = true;
		if (!hasFocus)
		{
			// Tiết kiệm CPU/GPU khi thu nhỏ hoặc mất focus xuống 20 FPS
			Application.targetFrameRate = 20;
		}
		else
		{
			// Khôi phục FPS đầy đủ khi quay lại game
			ModFps.ApplyFPS();
		}
	}

	private static int _suppressInputFrames;

	/// <summary>
	/// Bộ xử lý sự kiện tiếp nhận và phản hồi OnGUI.
	/// </summary>
	public void OnGUI()
	{
		if (count >= 10)
		{
			if (_suppressInputFrames > 0)
			{
				_suppressInputFrames--;
			}
			else if (!System.OperatingSystem.IsBrowser() || !Event.current.type.Equals(EventType.Repaint))
			{
				checkInput();
			}
			Session_ME.update();
			Session_ME2.update();
			if (Event.current.type.Equals(EventType.Repaint))
			{
				if (fps == 0)
				{
					timefps = mSystem.currentTimeMillis();
				}
				else if (mSystem.currentTimeMillis() - timefps > 1000)
				{
					max = fps;
					realFPS = fps;
					fps = 0;
					timefps = mSystem.currentTimeMillis();
				}
				fps++;
				GameMidlet.gameCanvas.paint(g);
				paintCount++;
				g.reset();
			}
		}
	}



	/// <summary>
	/// Thiết lập giá trị hoặc trạng thái cho setsizeChange.
	/// </summary>
	public void setsizeChange()
	{
		if (!isRun)
		{
			Screen.orientation = ScreenOrientation.LandscapeLeft;
			Application.runInBackground = true;
			try
			{
				System.Net.ServicePointManager.DefaultConnectionLimit = 512;
				System.Net.ServicePointManager.Expect100Continue = false;
				System.Net.ServicePointManager.UseNagleAlgorithm = false;
			}
			catch (System.Exception)
			{
			}
			base.useGUILayout = false;
			isCompactDevice = detectCompactDevice();
			if (main == null)
			{
				main = this;
			}
			isRun = true;
			ScaleGUI.initScaleGUI();
			if (isPC)
			{
				IMEI = SystemInfo.deviceUniqueIdentifier;
			}
			else
			{
				IMEI = GetMacAddress();
			}
#if ANDROID || __ANDROID__
			isPC = false;
#else
			isPC = !System.OperatingSystem.IsAndroid() && !System.OperatingSystem.IsIOS() && !(DragonBoy_Net8_Native.Engine.Platform.PlatformService.Current?.IsTouchDevice ?? false);
#endif
			if (!isPC)
			{
				GameCanvas.isTouch = true;
				GameCanvas.isTouchControl = true;
				mSystem.clientType = 2;
			}
			if (isPC && Application.CurrentTabId == 1)
			{
				Screen.fullScreen = ModGraphics.isFullscreen;
			}
			if (iPhoneSettings.generation == iPhoneGeneration.iPodTouch4Gen)
			{
				isIpod = true;
			}
			if (iPhoneSettings.generation == iPhoneGeneration.iPhone4)
			{
				isIphone4 = true;
			}
			g = new mGraphics();
			midlet = new GameMidlet();
			TileMap.loadBg();
			Paint.loadbg();
			PopUp.loadBg();
			GameScr.loadBg();
			InfoMe.gI().loadCharId();
			Panel.loadBg();
			Menu.loadBg();
			Key.mapKeyPC();
			SoundMn.gI().loadSound(TileMap.mapID);
			g.CreateLineMaterial();
		}
	}

	/// <summary>
	/// Thiết lập giá trị hoặc trạng thái cho setBackupIcloud.
	/// </summary>
	public static void setBackupIcloud(string path)
	{
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của GetMacAddress.
	/// </summary>
	public string GetMacAddress()
	{
		if (System.OperatingSystem.IsBrowser())
		{
			return SystemInfo.deviceUniqueIdentifier;
		}
		string empty = string.Empty;
		NetworkInterface[] allNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
		for (int i = 0; i < allNetworkInterfaces.Length; i++)
		{
			PhysicalAddress physicalAddress = allNetworkInterfaces[i].GetPhysicalAddress();
			if (physicalAddress.ToString() != string.Empty)
			{
				return physicalAddress.ToString();
			}
		}
		return string.Empty;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ doClearRMS.
	/// </summary>
	public void doClearRMS()
	{
		if (isPC)
		{
			int num = Rms.loadRMSInt(Rms.RMS_lastZoomlevel);
			if (num != mGraphics.zoomLevel)
			{
				Rms.clearAll();
				Rms.saveRMSInt(Rms.RMS_lastZoomlevel, mGraphics.zoomLevel);
				Rms.saveRMSInt("levelScreenKN", level);
			}
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ closeKeyBoard.
	/// </summary>
	public static void closeKeyBoard()
	{
		try
		{
			TouchScreenKeyboard.Clear();
			if (TField.kb != null)
			{
				TField.kb.active = false;
				TField.kb = null;
			}
		}
		catch { }
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ FixedUpdate.
	/// </summary>
	public void FixedUpdate()
	{
		if (count == 0)
		{
			Debug.LogWarning("MAIN.FIXEDUPDATE FIRST TICK");
			mainThreadName = Thread.CurrentThread.Name;
		}
		Rms.update();
		count++;
		if (count >= 10)
		{
			if (up == 0)
			{
				timeup = mSystem.currentTimeMillis();
			}
			else if (mSystem.currentTimeMillis() - timeup > 1000)
			{
				upmax = up;
				up = 0;
				timeup = mSystem.currentTimeMillis();
			}
			up++;
			setsizeChange();
			updateCount++;
			ipKeyboard.update();
			GameMidlet.gameCanvas.update();
			ModMenu.Update();
			Image.update();
			DataInputStream.update();
			SMS.update();
			Net.update();
			f++;
			if (f > 8)
			{
				f = 0;
			}

		}
	}

	/// <summary>
	/// Cập nhật trạng thái logic của đối tượng theo từng chu kỳ khung hình.
	/// </summary>
	public void Update()
	{
		if (Time.fixedDeltaTime != 0.02f)
		{
			Time.fixedDeltaTime = 0.02f;
			Time.maximumDeltaTime = 0.1f;
		}
	}

	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của checkInput.
	/// </summary>
	private void checkInput()
	{
		if (Event.current.type != EventType.KeyDown && Event.current.type != EventType.KeyUp)
		{
			if (Input.GetMouseButtonDown(0))
			{
				Vector3 mousePosition = Input.mousePosition;
				GameMidlet.gameCanvas.pointerPressed((int)(mousePosition.x / (float)mGraphics.zoomLevel), (int)(((float)Screen.height - mousePosition.y) / (float)mGraphics.zoomLevel) + mGraphics.addYWhenOpenKeyBoard);
				lastMousePos.x = mousePosition.x / (float)mGraphics.zoomLevel;
				lastMousePos.y = mousePosition.y / (float)mGraphics.zoomLevel + (float)mGraphics.addYWhenOpenKeyBoard;
			}
			else if (Input.GetMouseButton(0))
			{
				Vector3 mousePosition2 = Input.mousePosition;
				GameMidlet.gameCanvas.pointerDragged((int)(mousePosition2.x / (float)mGraphics.zoomLevel), (int)(((float)Screen.height - mousePosition2.y) / (float)mGraphics.zoomLevel) + mGraphics.addYWhenOpenKeyBoard);
				lastMousePos.x = mousePosition2.x / (float)mGraphics.zoomLevel;
				lastMousePos.y = mousePosition2.y / (float)mGraphics.zoomLevel + (float)mGraphics.addYWhenOpenKeyBoard;
			}
			if (Input.GetMouseButtonUp(0))
			{
				Vector3 mousePosition3 = Input.mousePosition;
				lastMousePos.x = mousePosition3.x / (float)mGraphics.zoomLevel;
				lastMousePos.y = mousePosition3.y / (float)mGraphics.zoomLevel + (float)mGraphics.addYWhenOpenKeyBoard;
				GameMidlet.gameCanvas.pointerReleased((int)(mousePosition3.x / (float)mGraphics.zoomLevel), (int)(((float)Screen.height - mousePosition3.y) / (float)mGraphics.zoomLevel) + mGraphics.addYWhenOpenKeyBoard);
			}
			float wheel = Input.GetAxis("Mouse ScrollWheel");
			if (isPC || wheel != 0f)
			{
				GameMidlet.gameCanvas.scrollMouse((int)(wheel * 10f));
			}
			if (isPC)
			{
				float x = Input.mousePosition.x;
				float y = Input.mousePosition.y;
				int x2 = (int)x / mGraphics.zoomLevel;
				int y2 = (Screen.height - (int)y) / mGraphics.zoomLevel;
				GameMidlet.gameCanvas.pointerMouse(x2, y2);
			}
			return;
		}
		if (Event.current.type == EventType.KeyDown)
		{
			char character = Event.current.character;
			int num = 0;

			// 1. Phím tắt Ctrl (Ctrl+V dán, Ctrl+C sao chép, Ctrl+A xóa toàn bộ, Ctrl+X cắt)
			if (Event.current.control || character == 22 || character == 3 || character == 1 || character == 24)
			{
				TField activeTf = TField.GetActive();
				if (Event.current.keyCode == KeyCode.V || character == 22)
				{
					try
					{
						string clip = GUIUtility.systemCopyBuffer;
						if (!string.IsNullOrEmpty(clip))
						{
							if (ChatTextField.gI().isShow)
							{
								ChatTextField.gI().pasteText(clip);
							}
							else if (activeTf != null)
							{
								activeTf.paste(clip);
							}
						}
					}
					catch
					{
					}
					return;
				}
				if (Event.current.keyCode == KeyCode.C || character == 3)
				{
					try
					{
						if (ChatTextField.gI().isShow)
						{
							GUIUtility.systemCopyBuffer = ChatTextField.gI().tfChat.getText();
						}
						else if (activeTf != null)
						{
							GUIUtility.systemCopyBuffer = activeTf.getText();
						}
					}
					catch
					{
					}
					return;
				}
				if (Event.current.keyCode == KeyCode.A || character == 1)
				{
					try
					{
						if (ChatTextField.gI().isShow)
						{
							ChatTextField.gI().tfChat.clearAllText();
						}
						else if (activeTf != null)
						{
							activeTf.clearAllText();
						}
					}
					catch
					{
					}
					return;
				}
				if (Event.current.keyCode == KeyCode.X || character == 24)
				{
					try
					{
						if (ChatTextField.gI().isShow)
						{
							GUIUtility.systemCopyBuffer = ChatTextField.gI().tfChat.getText();
							ChatTextField.gI().tfChat.clearAllText();
						}
						else if (activeTf != null)
						{
							GUIUtility.systemCopyBuffer = activeTf.getText();
							activeTf.clearAllText();
						}
					}
					catch
					{
					}
					return;
				}
				return;
			}

			// 2. Phím chức năng & điều hướng (Ưu tiên tuyệt đối trước khi xử lý ký tự)
			if (character == '\b' || Event.current.keyCode == KeyCode.Backspace)
			{
				if (ChatTextField.gI().isShow)
				{
					ChatTextField.gI().keyPressed(-8);
					return;
				}
				num = -8;
			}
			else if (character == 127 || Event.current.keyCode == KeyCode.Delete)
			{
				if (ChatTextField.gI().isShow)
				{
					ChatTextField.gI().keyPressed(-9);
					return;
				}
				num = -9;
			}
			else if (character == '\n' || character == '\r' || Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter)
			{
				if (ChatTextField.gI().isShow)
				{
					ChatTextField.gI().sendChat();
					return;
				}
				num = -5;
			}
			else if (character == 27 || Event.current.keyCode == KeyCode.Escape)
			{
				if (ChatTextField.gI().isShow)
				{
					ChatTextField.gI().close();
					return;
				}
				if (ModUI.uiCustomOpen)
				{
					if (ModUIBackground.isOpen)
					{
						ModUIBackground.isOpen = false;
						SoundMn.gI().buttonClose();
					}
					else
					{
						ModMenu.CloseMenu();
					}
					return;
				}
				num = -7;
			}
			else if (character == '\t' || Event.current.keyCode == KeyCode.Tab)
			{
				num = -26;
			}
			else if (Event.current.keyCode == KeyCode.LeftArrow)
			{
				if (ChatTextField.gI().isShow)
				{
					ChatTextField.gI().keyPressed(-3);
					return;
				}
				num = -3;
			}
			else if (Event.current.keyCode == KeyCode.RightArrow)
			{
				if (ChatTextField.gI().isShow)
				{
					ChatTextField.gI().keyPressed(-4);
					return;
				}
				num = -4;
			}
			else if (Event.current.keyCode == KeyCode.UpArrow)
			{
				num = -1;
			}
			else if (Event.current.keyCode == KeyCode.DownArrow)
			{
				num = -2;
			}
			else if (character >= ' ' && character != 127)
			{
				if (ChatTextField.gI().isShow)
				{
					ChatTextField.gI().keyPressed((int)character);
					return;
				}
				num = (int)character;
			}
			else
			{
				num = MyKeyMap.map(Event.current.keyCode);
			}

			if (num != 0)
			{
				GameMidlet.gameCanvas.keyPressedz(num);
			}
		}
		if (Event.current.type == EventType.KeyUp)
		{
			int num2 = MyKeyMap.map(Event.current.keyCode);
			if (num2 != 0)
			{
				GameMidlet.gameCanvas.keyReleasedz(num2);
			}
		}
	}

	/// <summary>
	/// Bộ xử lý sự kiện tiếp nhận và phản hồi OnApplicationQuit.
	/// </summary>
	public override void OnApplicationQuit()
	{
		try
		{
			Debug.LogWarning("MAIN.ONAPPLICATIONQUIT STACK: " + System.Environment.StackTrace);
			GameTabStatus.MarkTabStopped(Application.CurrentTabId);
			ModMenu.SaveConfig();
			GameCanvas.bRun = false;
			Session_ME.gI().close();
			Session_ME2.gI().close();
		}
		catch
		{
		}
	}

	/// <summary>
	/// Bộ xử lý sự kiện tiếp nhận và phản hồi OnApplicationPause.
	/// </summary>
	private void OnApplicationPause(bool paused)
	{
		Debug.LogWarning("MAIN.ONAPPLICATIONPAUSE: " + paused);
		isResume = false;
		if (!paused)
		{
			isResume = true;
		}
		try
		{
			TouchScreenKeyboard.Clear();
			if (TField.kb != null)
			{
				TField.kb.active = false;
				TField.kb = null;
			}
		}
		catch { }
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ exit.
	/// </summary>
	public static void exit()
	{
		try
		{
			main?.OnApplicationQuit();
#if ANDROID || __ANDROID__
			Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
#else
			if (!System.OperatingSystem.IsBrowser())
			{
				System.Environment.Exit(0);
			}
#endif
		}
		catch
		{
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ detectCompactDevice.
	/// </summary>
	public static bool detectCompactDevice()
	{
		if (iPhoneSettings.generation == iPhoneGeneration.iPhone || iPhoneSettings.generation == iPhoneGeneration.iPhone3G || iPhoneSettings.generation == iPhoneGeneration.iPodTouch1Gen || iPhoneSettings.generation == iPhoneGeneration.iPodTouch2Gen)
		{
			return false;
		}
		return true;
	}

	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của checkCanSendSMS.
	/// </summary>
	public static bool checkCanSendSMS()
	{
		if (iPhoneSettings.generation == iPhoneGeneration.iPhone3GS || iPhoneSettings.generation == iPhoneGeneration.iPhone4 || iPhoneSettings.generation > iPhoneGeneration.iPodTouch4Gen)
		{
			return true;
		}
		return false;
	}

	private int _idleBgTick;

	public void BackgroundUpdate()
	{
		Rms.update();
		count++;
		if (count < 10)
		{
			return;
		}

		bool isIdleLobby = GameCanvas.currentScreen != GameScr.instance &&
			!Session_ME.gI().isConnected() &&
			!Session_ME.connecting &&
			!ServerListScreen.waitToLogin &&
			!ServerListScreen.isGetData &&
			!DragonBoy_Net8_Native.Src.Mod.Security.ModCredentialSecurity.isLoggingIn;

		if (isIdleLobby)
		{
			_idleBgTick++;
			if (_idleBgTick < 10)
			{
				return;
			}
			_idleBgTick = 0;
		}

		setsizeChange();
		Session_ME.update();
		Session_ME2.update();
		updateCount++;
		GameMidlet.gameCanvas.update();
		ModMenu.Update();
		Image.update();
		DataInputStream.update();
		SMS.update();
		Net.update();
		f++;
		if (f > 8)
		{
			f = 0;
		}
	}

	private static void SyncVirtualSize(int virtualW, int virtualH)
	{
		if (virtualW <= 0 || virtualH <= 0) return;
		if (DragonBoy_Net8_Native.Engine.Graphics.RenderManager.VirtualWidth != virtualW ||
			DragonBoy_Net8_Native.Engine.Graphics.RenderManager.VirtualHeight != virtualH)
		{
			DragonBoy_Net8_Native.Engine.Graphics.RenderManager.VirtualWidth = virtualW;
			DragonBoy_Net8_Native.Engine.Graphics.RenderManager.VirtualHeight = virtualH;
		}
	}

	public static void HostInitTab(
		int tabId,
		int virtualW,
		int virtualH,
		System.Action<int, bool, int, int> onSwitchTab,
		System.Action<int> onStopTab,
		System.Action<int> onDeleteTab,
		System.Action<int> onPrewarmTab)
	{
		Application.CurrentTabId = tabId;
		Application.IsActiveTab = false;
		DragonBoy_Net8_Native.Engine.Platform.DefaultDesktopBridge.HostSwitchToTab = onSwitchTab;
		DragonBoy_Net8_Native.Engine.Platform.DefaultDesktopBridge.HostStopTab = onStopTab;
		DragonBoy_Net8_Native.Engine.Platform.DefaultDesktopBridge.HostDeleteTab = onDeleteTab;
		DragonBoy_Net8_Native.Engine.Platform.DefaultDesktopBridge.HostPrewarmTab = onPrewarmTab;
		SyncVirtualSize(virtualW, virtualH);

		main = new Main();
		main.Start();
		main.setsizeChange();

		if (tabId > 1)
		{
			main.count = 10;
			SplashScr.splashScrStat = 9;
			main.FixedUpdate();
			SplashScr.splashScrStat = 24;
			main.FixedUpdate();
			main.FixedUpdate();
			main.Update();
		}
		GameTabStatus.ForceSaveCurrentTab();
	}

	public static void HostSetActive(bool active, bool syncHub, bool hubExpanded, int hubX, int hubY, string notification)
	{
		Application.IsActiveTab = active;
		_suppressInputFrames = active ? 2 : 0;
		try
		{
			GameCanvas.clearAllPointerEvent();
			GameCanvas.clearKeyPressed();
			GameCanvas.clearKeyHold();
		}
		catch { }

		if (!active)
		{
			GameTabStatus.ForceSaveCurrentTab();
			return;
		}

		GameTabManager.isTabExplicitlyStopped = false;
		ServerListScreen.isAutoConect = true;
		if (syncHub)
		{
			ModFloatingHUD.SyncState(hubExpanded, hubX, hubY);
		}
		GameTabRegistry.Reload();
		GameTabStatus.RefreshAllTabs(force: true);
		if (!string.IsNullOrEmpty(notification))
		{
			GameScr.info1?.addInfo(notification, 0);
		}
	}

	public static void HostStepActive(double frameTime, int subSteps, int virtualW, int virtualH)
	{
		if (main == null) return;
		Application.IsActiveTab = true;
		SyncVirtualSize(virtualW, virtualH);
		Time.deltaTime = (float)frameTime;
		if (_suppressInputFrames == 0)
		{
			EventPump.ProcessInput(main);
		}
		for (int i = 0; i < subSteps; i++)
		{
			main.FixedUpdate();
			main.Update();
		}
	}

	public static void HostStepBackground(double frameTime, int subSteps, int virtualW, int virtualH)
	{
		if (main == null || subSteps <= 0) return;
		Application.IsActiveTab = false;
		SyncVirtualSize(virtualW, virtualH);
		Time.deltaTime = (float)frameTime;
		for (int i = 0; i < subSteps; i++)
		{
			main.BackgroundUpdate();
			main.Update();
		}
	}

	public static int HostRenderActive()
	{
		if (main == null) return (2 << 8) | 1;
		Application.IsActiveTab = true;
		UnityEngine.GUIUtility.ResetMatrixStack();
		Event.current.type = EventType.Repaint;
		main.OnGUI();
		int zoom = System.Math.Max(1, mGraphics.zoomLevel);
		int mode = ModGraphics.renderMode & 0xFF;
		return (zoom << 8) | mode;
	}

	public static void HostStopInstance()
	{
		GameTabManager.StopTab(Application.CurrentTabId);
	}

	public static void HostNotify(string message)
	{
		if (!string.IsNullOrEmpty(message))
		{
			GameScr.info1?.addInfo(message, 0);
		}
	}

	public static void HostQuitInstance()
	{
		main?.OnApplicationQuit();
	}
}

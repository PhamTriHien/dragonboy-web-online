using System;
using System.Security.Cryptography;
using System.Text;

namespace DragonBoy_Net8_Native.Src.Mod.Security
{
	/// <summary>
	/// Module Bảo Mật Tài Khoản: Quản lý mã hóa thông tin tài khoản, tự động dọn dẹp bộ nhớ nhạy cảm khi thoát.
	/// </summary>
	public static class ModCredentialSecurity
	{
		private const string ENC_PREFIX = "ENC_V1:";
		private static string _cachedDeviceKey = null;

		// Login Watchdog State
		public static bool isLoggingIn = false;
		public static long loginStartTime = 0;
		public static long lastLoginAttemptTime = 0;
		private const long LOGIN_TIMEOUT_MS = 12000; // 12 seconds max wait for server login response

		// Zone Change Watchdog State
		public static bool isWaitingZoneChange = false;
		public static long zoneChangeStartTime = 0;
		public static int targetZone = -1;
		private const long ZONE_TIMEOUT_MS = 5000; // 5 seconds max wait for zone change response

		/// <summary>
		/// Truy xuất và trả về dữ liệu của GetDeviceKey.
		/// </summary>
		public static string GetDeviceKey()
		{
			if (!string.IsNullOrEmpty(_cachedDeviceKey))
			{
				return _cachedDeviceKey;
			}
			try
			{
				string devId = Rms.loadRMSString("sys_dev_id");
				if (string.IsNullOrEmpty(devId))
				{
					devId = Guid.NewGuid().ToString("N");
					Rms.saveRMSString("sys_dev_id", devId);
				}
				_cachedDeviceKey = "NRO_KEY_" + devId;
			}
			catch
			{
				_cachedDeviceKey = "NRO_KEY_DEFAULT_MOD_2026";
			}
			return _cachedDeviceKey;
		}

		/// <summary>
		/// Thực thi phương thức nghiệp vụ ObfuscatePassword.
		/// </summary>
		public static string ObfuscatePassword(string plainPassword)
		{
			if (string.IsNullOrEmpty(plainPassword))
			{
				return string.Empty;
			}
			try
			{
				byte[] plainBytes = Encoding.UTF8.GetBytes(plainPassword);
				byte[] keyBytes;
				using (SHA256 sha = SHA256.Create())
				{
					keyBytes = sha.ComputeHash(Encoding.UTF8.GetBytes(GetDeviceKey()));
				}

				byte[] cipherBytes = new byte[plainBytes.Length];
				for (int i = 0; i < plainBytes.Length; i++)
				{
					cipherBytes[i] = (byte)(plainBytes[i] ^ keyBytes[i % keyBytes.Length] ^ (0x5A + (i & 0x0F)));
				}
				return ENC_PREFIX + Convert.ToBase64String(cipherBytes);
			}
			catch
			{
				return plainPassword;
			}
		}

		/// <summary>
		/// Thực thi phương thức nghiệp vụ DeobfuscatePassword.
		/// </summary>
		public static string DeobfuscatePassword(string storedPassword)
		{
			if (string.IsNullOrEmpty(storedPassword))
			{
				return string.Empty;
			}
			// Nếu là mật khẩu thô từ phiên bản cũ (không có tiền tố ENC), giữ nguyên
			if (!storedPassword.StartsWith(ENC_PREFIX))
			{
				return storedPassword;
			}
			try
			{
				string b64 = storedPassword.Substring(ENC_PREFIX.Length);
				byte[] cipherBytes = Convert.FromBase64String(b64);
				byte[] keyBytes;
				using (SHA256 sha = SHA256.Create())
				{
					keyBytes = sha.ComputeHash(Encoding.UTF8.GetBytes(GetDeviceKey()));
				}

				byte[] plainBytes = new byte[cipherBytes.Length];
				for (int i = 0; i < cipherBytes.Length; i++)
				{
					plainBytes[i] = (byte)(cipherBytes[i] ^ keyBytes[i % keyBytes.Length] ^ (0x5A + (i & 0x0F)));
				}
				return Encoding.UTF8.GetString(plainBytes);
			}
			catch
			{
				return storedPassword;
			}
		}

		public static string activePassword = string.Empty;

		/// <summary>
		/// Dọn dẹp và đặt lại trạng thái ban đầu cho ResetForTab.
		/// </summary>
		public static void ResetForTab()
		{
			activePassword = string.Empty;
			_cachedDeviceKey = null;
			isLoggingIn = false;
			loginStartTime = 0;
			lastLoginAttemptTime = 0;
			isWaitingZoneChange = false;
			zoneChangeStartTime = 0;
			targetZone = -1;
		}

		/// <summary>
		/// Thực thi phương thức nghiệp vụ SaveCredentials.
		/// </summary>
		public static void SaveCredentials(string username, string password, bool remember)
		{
			try
			{
				activePassword = password ?? string.Empty;
				Rms.saveRMSString(Rms.RMS_acc, username != null ? username.Trim() : string.Empty);
				if (remember)
				{
					Rms.saveRMSInt(Rms.RMS_check, 1);
					Rms.saveRMSString(Rms.RMS_pass, !string.IsNullOrEmpty(password) ? ObfuscatePassword(password) : string.Empty);
				}
				else
				{
					Rms.saveRMSInt(Rms.RMS_check, 2);
					Rms.saveRMSString(Rms.RMS_pass, string.Empty);
				}
			}
			catch { }
		}

		/// <summary>
		/// Khởi tạo và nạp dữ liệu cấu hình cho LoadSavedPassword.
		/// </summary>
		public static string LoadSavedPassword()
		{
			try
			{
				if (!string.IsNullOrEmpty(activePassword))
				{
					return activePassword;
				}
				if (Rms.loadRMSInt(Rms.RMS_check) == 2)
				{
					return string.Empty;
				}
				string raw = Rms.loadRMSString(Rms.RMS_pass);
				return DeobfuscatePassword(raw);
			}
			catch
			{
				return string.Empty;
			}
		}

		/// <summary>
		/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của CanAttemptLogin.
		/// </summary>
		public static bool CanAttemptLogin()
		{
			long now = mSystem.currentTimeMillis();
			if (now - lastLoginAttemptTime < 800)
			{
				return false;
			}
			return true;
		}

		/// <summary>
		/// Bộ xử lý sự kiện tiếp nhận và phản hồi OnLoginStarted.
		/// </summary>
		public static void OnLoginStarted()
		{
			isLoggingIn = true;
			loginStartTime = mSystem.currentTimeMillis();
			lastLoginAttemptTime = loginStartTime;
		}

		/// <summary>
		/// Bộ xử lý sự kiện tiếp nhận và phản hồi OnLoginFinished.
		/// </summary>
		public static void OnLoginFinished()
		{
			isLoggingIn = false;
			loginStartTime = 0;
		}

		/// <summary>
		/// Cập nhật trạng thái logic của UpdateLoginWatchdog theo chu kỳ khung hình.
		/// </summary>
		public static void UpdateLoginWatchdog()
		{
			if (isLoggingIn && loginStartTime > 0)
			{
				if (GameCanvas.currentDialog == null)
				{
					isLoggingIn = false;
					loginStartTime = 0;
					return;
				}
				// Nếu dialog hiện tại không phải là dialog chờ ("Xin chờ"), tức là đã nhận phản hồi / thông báo từ server
				if (GameCanvas.currentDialog == GameCanvas.msgdlg && GameCanvas.msgdlg != null && !GameCanvas.msgdlg.isWait)
				{
					isLoggingIn = false;
					loginStartTime = 0;
					return;
				}
				long now = mSystem.currentTimeMillis();
				if (now - loginStartTime > LOGIN_TIMEOUT_MS)
				{
					isLoggingIn = false;
					loginStartTime = 0;
					GameCanvas.endDlg();
					GameCanvas.startOK("Máy chủ không phản hồi (Timeout), vui lòng thử lại!", 8884, null);
				}
			}
		}

		/// <summary>
		/// Thực thi phương thức nghiệp vụ StartZoneChangeWatchdog.
		/// </summary>
		public static void StartZoneChangeWatchdog(int zone)
		{
			isWaitingZoneChange = true;
			zoneChangeStartTime = mSystem.currentTimeMillis();
			targetZone = zone;
		}

		/// <summary>
		/// Bộ xử lý sự kiện tiếp nhận và phản hồi OnZoneChangeSuccess.
		/// </summary>
		public static void OnZoneChangeSuccess()
		{
			isWaitingZoneChange = false;
			zoneChangeStartTime = 0;
			targetZone = -1;
		}

		/// <summary>
		/// Cập nhật trạng thái logic của UpdateZoneWatchdog theo chu kỳ khung hình.
		/// </summary>
		public static void UpdateZoneWatchdog()
		{
			if (isWaitingZoneChange && zoneChangeStartTime > 0)
			{
				long now = mSystem.currentTimeMillis();
				if (now - zoneChangeStartTime > ZONE_TIMEOUT_MS)
				{
					isWaitingZoneChange = false;
					zoneChangeStartTime = 0;
					targetZone = -1;
					InfoDlg.hide();
					GameScr.info1?.addInfo("Khu vực đầy hoặc đổi khu thất bại!", 0);
				}
			}
		}

		/// <summary>
		/// Thực thi phương thức nghiệp vụ SwitchServerCleanly.
		/// </summary>
		public static void SwitchServerCleanly(int newIpSelect)
		{
			if (newIpSelect < 0 || ServerListScreen.address == null ||
				newIpSelect >= ServerListScreen.address.Length ||
				string.IsNullOrWhiteSpace(ServerListScreen.address[newIpSelect]) ||
				ServerListScreen.port == null || newIpSelect >= ServerListScreen.port.Length ||
				ServerListScreen.port[newIpSelect] <= 0)
			{
				return;
			}

			try
			{
				Session_ME.gI().close();
				ServerListScreen.SetIpSelect(newIpSelect, issave: true);

				if (ServerListScreen.address != null && newIpSelect >= 0 && newIpSelect < ServerListScreen.address.Length)
				{
					GameMidlet.IP = ServerListScreen.address[newIpSelect];
					GameMidlet.PORT = ServerListScreen.port[newIpSelect];
					if (ServerListScreen.language != null && newIpSelect < ServerListScreen.language.Length)
					{
						GameMidlet.LANGUAGE = ServerListScreen.language[newIpSelect];
						if (ServerListScreen.language[newIpSelect] != mResources.language)
						{
							mResources.loadLanguague(ServerListScreen.language[newIpSelect]);
						}
					}
					if (ServerListScreen.nameServer != null && newIpSelect < ServerListScreen.nameServer.Length)
					{
						LoginScr.serverName = ServerListScreen.nameServer[newIpSelect];
					}
					sbyte area = (sbyte)(GameMidlet.LANGUAGE == 0 ? 0 : 1);
					Rms.saveRMS("area_select", new sbyte[2] { area, 0 });
				}

				if (GameCanvas.serverScreen == null)
				{
					GameCanvas.serverScreen = new ServerListScreen();
				}
				ServerListScreen.loadScreen = true;
				ServerListScreen.isAutoConect = true;
				GameCanvas.serverScreen.switchToMe();
				ServerListScreen.ConnectIP();
			}
			catch (Exception ex)
			{
				Cout.LogError("Loi SwitchServerCleanly: " + ex.Message);
			}
		}
	}
}

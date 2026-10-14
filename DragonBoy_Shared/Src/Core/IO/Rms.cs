using System;
using System.IO;
using System.Threading;
using UnityEngine;

/// <summary>
/// CLASS Rms: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
public class Rms
{
	public static int status;

	public static sbyte[] data;

	public static string filename;

	private const int INTERVAL = 5;

	private const int MAXTIME = 500;

	public static string RMS_ResVersion = "ResVersion";

	public static string RMS_clienttype = "clienttype";

	public static string RMS_lastZoomlevel = "lastZoomlevel";

	public static string RMS_userAo = "userAo";

	public static string RMS_acc = "acc";

	public static string RMS_pass = "pass";

	public static string RMS_check = "check";

	private static readonly object rmsLock = new object();

	/// <summary>
	/// Thực thi phương thức nghiệp vụ saveRMS.
	/// </summary>
	public static void saveRMS(string filename, sbyte[] data)
	{
		__saveRMS(filename, data);
	}

	/// <summary>
	/// Khởi tạo và nạp dữ liệu cấu hình cho loadRMS.
	/// </summary>
	public static sbyte[] loadRMS(string filename)
	{
		return __loadRMS(filename);
	}

	/// <summary>
	/// Khởi tạo và nạp dữ liệu cấu hình cho loadRMSString.
	/// </summary>
	public static string loadRMSString(string fileName)
	{
		sbyte[] array = loadRMS(fileName);
		if (array == null)
		{
			return null;
		}
		DataInputStream dataInputStream = new DataInputStream(array);
		try
		{
			string result = dataInputStream.readUTF();
			dataInputStream.close();
			if (fileName == RMS_pass && result != null && result.StartsWith("ENC_V1:"))
			{
				result = DragonBoy_Net8_Native.Src.Mod.Security.ModCredentialSecurity.DeobfuscatePassword(result);
			}
			return result;
		}
		catch (Exception ex)
		{
			Cout.println(ex.StackTrace);
		}
		return null;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ convertSbyteToByte.
	/// </summary>
	public static byte[] convertSbyteToByte(sbyte[] var)
	{
		byte[] array = new byte[var.Length];
		for (int i = 0; i < var.Length; i++)
		{
			if (var[i] > 0)
			{
				array[i] = (byte)var[i];
			}
			else
			{
				array[i] = (byte)(var[i] + 256);
			}
		}
		return array;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ saveRMSString.
	/// </summary>
	public static void saveRMSString(string filename, string data)
	{
		DataOutputStream dataOutputStream = new DataOutputStream();
		try
		{
			dataOutputStream.writeUTF(data);
			saveRMS(filename, dataOutputStream.toByteArray());
			dataOutputStream.close();
		}
		catch (Exception ex)
		{
			Cout.println(ex.StackTrace);
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ _saveRMS.
	/// </summary>
	private static void _saveRMS(string filename, sbyte[] data)
	{
		__saveRMS(filename, data);
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ _loadRMS.
	/// </summary>
	private static sbyte[] _loadRMS(string filename)
	{
		return __loadRMS(filename);
	}

	/// <summary>
	/// Cập nhật trạng thái logic của đối tượng theo từng chu kỳ khung hình.
	/// </summary>
	public static void update()
	{
		if (status == 2)
		{
			status = 1;
			__saveRMS(filename, data);
			status = 0;
		}
		else if (status == 3)
		{
			status = 1;
			data = __loadRMS(filename);
			status = 0;
		}
	}

	/// <summary>
	/// Khởi tạo và nạp dữ liệu cấu hình cho loadRMSInt.
	/// </summary>
	public static int loadRMSInt(string file)
	{
		sbyte[] array = loadRMS(file);
		return (array != null && array.Length > 0) ? array[0] : (-1);
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ saveRMSInt.
	/// </summary>
	public static void saveRMSInt(string file, int x)
	{
		try
		{
			saveRMS(file, new sbyte[1] { (sbyte)x });
			if (file == ServerListScreen.RMS_svselect)
			{
				Debug.LogError(">>>>>>>>Save saveRMSInt: " + file + "  index:" + x);
			}
		}
		catch (Exception)
		{
		}
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của GetiPhoneDocumentsPath.
	/// </summary>
	public static string GetiPhoneDocumentsPath()
	{
		return Application.persistentDataPath;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ __saveRMS.
	/// </summary>
	private static void __saveRMS(string filename, sbyte[] data)
	{
		if (string.IsNullOrEmpty(filename) || data == null) return;
		lock (rmsLock)
		{
			try
			{
				string text = GetiPhoneDocumentsPath() + "/" + filename;
				string dir = Path.GetDirectoryName(text);
				if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
				{
					Directory.CreateDirectory(dir);
				}
				using (FileStream fileStream = new FileStream(text, FileMode.Create, FileAccess.Write, FileShare.None))
				{
					fileStream.Write(ArrayCast.cast(data), 0, data.Length);
					fileStream.Flush();
				}
#if BROWSER
				if (OperatingSystem.IsBrowser()) DragonBoy_Web_Host.WebStorageSync.OnFileSaved(text, ArrayCast.cast(data));
#endif
				Main.setBackupIcloud(text);
			}
			catch (Exception ex)
			{
				Cout.LogError("Loi __saveRMS " + filename + ": " + ex.Message);
			}
		}
	}

	private static bool IsPerTabIsolatedFile(string filename)
	{
		if (string.IsNullOrEmpty(filename)) return true;
		if (string.Equals(filename, RMS_acc, StringComparison.OrdinalIgnoreCase) ||
			string.Equals(filename, RMS_pass, StringComparison.OrdinalIgnoreCase) ||
			string.Equals(filename, RMS_check, StringComparison.OrdinalIgnoreCase) ||
			string.Equals(filename, "sys_dev_id", StringComparison.OrdinalIgnoreCase) ||
			string.Equals(filename, "mod_config.ini", StringComparison.OrdinalIgnoreCase))
		{
			return true;
		}
		if (filename.StartsWith("userAo", StringComparison.OrdinalIgnoreCase) ||
			filename.StartsWith("passAo", StringComparison.OrdinalIgnoreCase))
		{
			return true;
		}
		return false;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ __loadRMS.
	/// </summary>
	private static sbyte[] __loadRMS(string filename)
	{
		if (string.IsNullOrEmpty(filename)) return null;
		lock (rmsLock)
		{
			try
			{
				string docsPath = GetiPhoneDocumentsPath();
				string fullPath = docsPath + "/" + filename;
				if (!File.Exists(fullPath) && Application.CurrentTabId > 1 && !IsPerTabIsolatedFile(filename))
				{
					string rootDir = Path.GetDirectoryName(docsPath);
					if (!string.IsNullOrEmpty(rootDir))
					{
						string tab1Path = Path.Combine(rootDir, "tab1", filename);
						if (File.Exists(tab1Path))
						{
							fullPath = tab1Path;
						}
						else
						{
							string sharedRootPath = Path.Combine(rootDir, filename);
							if (File.Exists(sharedRootPath))
							{
								fullPath = sharedRootPath;
							}
						}
					}
				}
				if (File.Exists(fullPath))
				{
					using (FileStream fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
					{
						byte[] array = new byte[fileStream.Length];
						fileStream.Read(array, 0, array.Length);
						return ArrayCast.cast(array);
					}
				}
			}
			catch (Exception)
			{
			}

			// Fallback to bundled data templates (e.g. NR_image, NR_part, NR_dart, etc.)
			try
			{
				var textAsset = Resources.Load("data/" + filename) as TextAsset;
				if (textAsset != null && textAsset.bytes != null && textAsset.bytes.Length > 0)
				{
					return ArrayCast.cast(textAsset.bytes);
				}
			}
			catch (Exception)
			{
			}

			try
			{
				string baseDir = AppDomain.CurrentDomain.BaseDirectory;
				string[] paths = new string[]
				{
					Path.Combine(Path.Combine(baseDir, "Assets"), Path.Combine("data", filename)),
					Path.Combine(Path.Combine(baseDir, "data"), filename),
					Path.Combine(@"C:\ModNRO\DragonBoy_Net8_Native\Assets\data", filename)
				};
				foreach (string p in paths)
				{
					if (File.Exists(p))
					{
						byte[] bytes = File.ReadAllBytes(p);
						return ArrayCast.cast(bytes);
					}
				}
			}
			catch (Exception)
			{
			}

			return null;
		}
	}

	/// <summary>
	/// Dọn dẹp và đặt lại trạng thái ban đầu cho clearAll.
	/// </summary>
	public static void clearAll()
	{
		try
		{
			string dirPath = GetiPhoneDocumentsPath();
			if (string.IsNullOrEmpty(dirPath) || !Directory.Exists(dirPath)) return;
			DirectoryInfo di = new DirectoryInfo(dirPath);
			FileInfo[] files = di.GetFiles();
			foreach (FileInfo fileInfo in files)
			{
				if (!(fileInfo.Name == GameMidlet.VERSION) && !(fileInfo.Name == ServerListScreen.RMS_svselect) && !(fileInfo.Name == RMS_acc) && !(fileInfo.Name == RMS_pass) && !(fileInfo.Name == RMS_check))
				{
					try
					{
						string fullName = fileInfo.FullName;
						fileInfo.Delete();
#if BROWSER
						if (OperatingSystem.IsBrowser()) DragonBoy_Web_Host.WebStorageSync.OnFileDeleted(fullName);
#endif
					}
					catch { }
				}
			}
		}
		catch { }
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ DeleteStorage.
	/// </summary>
	public static void DeleteStorage(string path)
	{
		try
		{
			string fullPath = GetiPhoneDocumentsPath() + "/" + path;
			File.Delete(fullPath);
#if BROWSER
			if (OperatingSystem.IsBrowser()) DragonBoy_Web_Host.WebStorageSync.OnFileDeleted(fullPath);
#endif
		}
		catch (Exception)
		{
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ ByteArrayToString.
	/// </summary>
	public static string ByteArrayToString(byte[] ba)
	{
		string text = BitConverter.ToString(ba);
		return text.Replace("-", string.Empty);
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ StringToByteArray.
	/// </summary>
	public static byte[] StringToByteArray(string hex)
	{
		int length = hex.Length;
		byte[] array = new byte[length / 2];
		for (int i = 0; i < length; i += 2)
		{
			array[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
		}
		return array;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ deleteRecord.
	/// </summary>
	public static void deleteRecord(string name)
	{
		if (string.IsNullOrEmpty(name)) return;
		lock (rmsLock)
		{
			try
			{
				PlayerPrefs.DeleteKey(name);
				string text = GetiPhoneDocumentsPath() + "/" + name;
				if (File.Exists(text))
				{
					File.Delete(text);
#if BROWSER
					if (OperatingSystem.IsBrowser()) DragonBoy_Web_Host.WebStorageSync.OnFileDeleted(text);
#endif
				}
			}
			catch (Exception ex)
			{
				Cout.println("loi xoa RMS --------------------------" + ex.ToString());
			}
		}
	}

	/// <summary>
	/// Dọn dẹp và đặt lại trạng thái ban đầu cho clearRMS.
	/// </summary>
	public static void clearRMS()
	{
		deleteRecord("data");
		deleteRecord("dataVersion");
		deleteRecord("map");
		deleteRecord("mapVersion");
		deleteRecord("skill");
		deleteRecord("killVersion");
		deleteRecord("item");
		deleteRecord("itemVersion");
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ saveIP.
	/// </summary>
	public static void saveIP(string strID)
	{
		saveRMSString("NRIPlink", strID);
	}

	/// <summary>
	/// Khởi tạo và nạp dữ liệu cấu hình cho loadIP.
	/// </summary>
	public static string loadIP()
	{
		string text = loadRMSString("NRIPlink");
		if (text == null)
		{
			return null;
		}
		return text;
	}
}

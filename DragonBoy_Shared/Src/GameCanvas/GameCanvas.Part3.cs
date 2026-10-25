using System;
using Assets.src.e;
using Assets.src.g;
using UnityEngine;
/// <summary>
/// Thành phần phân mảnh (partial) của lớp GameCanvas: Đảm nhiệm phân hệ chuyên trách Part3 ().
/// </summary>
public partial class GameCanvas : IActionListener
{
	/// <summary>
	/// Khởi tạo và nạp dữ liệu cấu hình cho loadImageRMS.
	/// </summary>
	public static Image loadImageRMS(string path)
		{
			string rawPath = path;
			Image result = null;
			if (ModGraphics.IsJavaGraphics)
			{
				try
				{
					result = Image.createImage(cutPng(Main.res + "/x1" + rawPath));
				}
				catch (Exception)
				{
				}
			}
			path = cutPng(Main.res + "/x" + mGraphics.zoomLevel + rawPath);
			if (result == null)
			{
				try
				{
					result = Image.createImage(path);
				}
				catch (Exception)
				{
				}
			}
			if (result == null)
			{
				try
				{
					string[] array = Res.split(path, "/", 0);
					string filename = "x" + mGraphics.zoomLevel + array[array.Length - 1];
					sbyte[] array2 = Rms.loadRMS(filename);
					if (array2 != null && array2.Length > 0)
					{
						result = (mGraphics.zoomLevel > 1) ? Image.createServerZoomImage(array2, 0, array2.Length) : Image.createImage(array2, 0, array2.Length);
					}
				}
				catch (Exception ex)
				{
					Cout.LogError("Loi ham khong tim thay a: " + ex.ToString());
				}
			}
			return result;
		}
	/// <summary>
	/// Khởi tạo và nạp dữ liệu cấu hình cho loadImage.
	/// </summary>
	public static Image loadImage(string path)
		{
			string rawPath = path;
			Image result = null;
			if (ModGraphics.IsJavaGraphics)
			{
				try
				{
					result = Image.createImage(cutPng(Main.res + "/x1" + rawPath));
				}
				catch (Exception)
				{
				}
			}
			if (result == null)
			{
				path = cutPng(Main.res + "/x" + mGraphics.zoomLevel + rawPath);
				try
				{
					result = Image.createImage(path);
				}
				catch (Exception)
				{
				}
			}
			return result;
		}
	/// <summary>
	/// Khởi tạo và nạp dữ liệu cấu hình cho loadCustomImage.
	/// </summary>
	public static Image loadCustomImage(string filename)
		{
			try
			{
				if (cachedCustomLogo != null)
				{
					return cachedCustomLogo;
				}
				string baseDir = AppDomain.CurrentDomain.BaseDirectory;
				string currentDir = Environment.CurrentDirectory;
				string fname = string.IsNullOrEmpty(filename) ? "custom_logo.png" : filename;

#if BROWSER
				if (OperatingSystem.IsBrowser())
				{
					var webLogo = Image.createImage(fname.Replace(".png", string.Empty));
					if (webLogo != null)
					{
						cachedCustomLogo = webLogo;
						return cachedCustomLogo;
					}
				}
#endif

				string[] candidatePaths = new string[]
				{
					System.IO.Path.Combine(baseDir, fname),
					System.IO.Path.Combine(baseDir, "Assets", fname),
					System.IO.Path.Combine(baseDir, "Assets", "x2", fname),
					System.IO.Path.Combine(baseDir, "Assets", "x2", "mainImage", fname),
					System.IO.Path.Combine(currentDir, fname),
					System.IO.Path.Combine(currentDir, "Assets", fname),
					System.IO.Path.Combine(currentDir, "Assets", "x2", fname),
					fname,
					@"C:\ModNRO\DragonBoy_Net8_Native\Assets\" + fname,
					@"C:\ModNRO\DragonBoy_Net8_Native\Assets\x2\" + fname,
					@"C:\ModNRO\DragonBoy_Net8_Native\" + fname,
					@"C:\ModNRO\DragonBoy250_Assets\" + fname
				};

				for (int p = 0; p < candidatePaths.Length; p++)
				{
					string path = candidatePaths[p];
					if (System.IO.File.Exists(path))
					{
						byte[] array = System.IO.File.ReadAllBytes(path);
						if (array != null && array.Length > 0)
						{
							cachedCustomLogo = Image.createImage(array);
							if (cachedCustomLogo != null)
							{
								return cachedCustomLogo;
							}
						}
					}
				}
			}
			catch (Exception)
			{
			}
			return null;
		}
	/// <summary>
	/// Thực thi phương thức nghiệp vụ cutPng.
	/// </summary>
	public static string cutPng(string str)
		{
			string result = str;
			if (str.Contains(".png"))
			{
				result = str.Replace(".png", string.Empty);
			}
			return result;
		}
	/// <summary>
	/// Thực thi phương thức nghiệp vụ random.
	/// </summary>
	public static int random(int a, int b)
		{
			return a + r.nextInt(b - a);
		}
	/// <summary>
	/// Khởi tạo và nạp dữ liệu cấu hình cho loadWaterSplash.
	/// </summary>
	public void loadWaterSplash()
		{
			if (!lowGraphic)
			{
				imgWS = new Image[3];
				for (int i = 0; i < 3; i++)
				{
					imgWS[i] = loadImage("/e/w" + i + ".png");
				}
				wsX = new int[2];
				wsY = new int[2];
				wsState = new int[2];
				wsF = new int[2];
				wsState[0] = (wsState[1] = -1);
			}
		}
	/// <summary>
	/// Thực thi phương thức nghiệp vụ startWaterSplash.
	/// </summary>
	public bool startWaterSplash(int x, int y)
		{
			if (lowGraphic)
			{
				return false;
			}
			int num = ((wsState[0] != -1) ? 1 : 0);
			if (wsState[num] != -1)
			{
				return false;
			}
			wsState[num] = 0;
			wsX[num] = x;
			wsY[num] = y;
			return true;
		}
	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của isPaint.
	/// </summary>
	public static bool isPaint(int x, int y)
		{
			if (x < GameScr.cmx)
			{
				return false;
			}
			if (x > GameScr.cmx + GameScr.gW)
			{
				return false;
			}
			if (y < GameScr.cmy)
			{
				return false;
			}
			if (y > GameScr.cmy + GameScr.gH + 30)
			{
				return false;
			}
			return true;
		}
	/// <summary>
	/// Dọn dẹp và đặt lại trạng thái ban đầu cho resetToLoginScrz.
	/// </summary>
	public void resetToLoginScrz()
		{
			resetToLoginScr = true;
		}

}

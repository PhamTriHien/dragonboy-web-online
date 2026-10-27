using System;
using System.Collections;
using Assets.src.e;
using UnityEngine;

/// <summary>
/// Tầng đồ họa trung gian trừu tượng: Cung cấp các thao tác vẽ hình học, cắt vùng (clip), vẽ ảnh sprite và quản lý màu sắc.
/// </summary>
public partial class mGraphics
{
	public static int HCENTER = 1;

	public static int VCENTER = 2;

	public static int LEFT = 4;

	public static int RIGHT = 8;

	public static int TOP = 16;

	public static int BOTTOM = 32;

	private float r;

	private float g;

	private float b;

	private float a;

	public int clipX;

	public int clipY;

	public int clipW;

	public int clipH;

	public bool isClip;

	public bool isTranslate = true;

	public int translateX;

	public int translateY;

	private float translateXf;

	private float translateYf;

	public static int zoomLevel = 2;

	public const int BASELINE = 64;

	public const int SOLID = 0;

	public const int DOTTED = 1;

	public const int TRANS_MIRROR = 2;

	public const int TRANS_MIRROR_ROT180 = 1;

	public const int TRANS_MIRROR_ROT270 = 4;

	public const int TRANS_MIRROR_ROT90 = 7;

	public const int TRANS_NONE = 0;

	public const int TRANS_ROT180 = 3;

	public const int TRANS_ROT270 = 6;

	public const int TRANS_ROT90 = 5;

	public static Hashtable cachedTextures = new Hashtable();

	public static int addYWhenOpenKeyBoard;

	public int clipTX;

	public int clipTY;

	private int currentBGColor;

	private Vector2 pos = new Vector2(0f, 0f);

	private Rect rect;

	private Matrix4x4 matrixBackup;

	private Vector2 pivot;

	public Vector2 size = new Vector2(128f, 128f);

	public Vector2 relativePosition = new Vector2(0f, 0f);

	public Color clTrans;

	public static Color transParentColor = new Color(1f, 1f, 1f, 0f);

	private Material lineMaterial;

	public static void clearCached()
		{
			try
			{
				foreach (System.Collections.DictionaryEntry item in cachedTextures)
				{
#if !ANDROID && !__ANDROID__ && !BROWSER
					if (item.Value is Texture2D tex2D && tex2D.HasRaylibTexture && !tex2D.IsSharedGpuTexture)
					{
						try { Raylib_cs.Raylib.UnloadTexture(tex2D.RaylibTexture); } catch { }
						tex2D.HasRaylibTexture = false;
					}
#endif
				}
			}
			catch (Exception)
			{
			}
			cachedTextures.Clear();
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ cache.
	/// </summary>
	private void cache(string key, Texture value)
		{
			if (cachedTextures.Count > 400)
			{
				clearCached();
			}
			if (value.width * value.height < GameCanvas.w * GameCanvas.h)
			{
				cachedTextures.Add(key, value);
			}
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ translate.
	/// </summary>
	public void translate(int tx, int ty)
		{
			tx *= zoomLevel;
			ty *= zoomLevel;
			translateX += tx;
			translateY += ty;
			isTranslate = true;
			if (translateX == 0 && translateY == 0)
			{
				isTranslate = false;
			}
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ translate.
	/// </summary>
	public void translate(float x, float y)
		{
			translateXf += x;
			translateYf += y;
			isTranslate = true;
			if (translateXf == 0f && translateYf == 0f)
			{
				isTranslate = false;
			}
		}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của getTranslateX.
	/// </summary>
	public int getTranslateX()
		{
			return translateX / zoomLevel;
		}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của getTranslateY.
	/// </summary>
	public int getTranslateY()
		{
			return translateY / zoomLevel + addYWhenOpenKeyBoard;
		}

	/// <summary>
	/// Thiết lập giá trị hoặc trạng thái cho setClip.
	/// </summary>
	public void setClip(int x, int y, int w, int h)
		{
			if (x <= 0 && y <= 0 && w >= GameCanvas.w && h >= GameCanvas.h)
			{
				isClip = false;
				clipTX = 0;
				clipTY = 0;
				clipX = 0;
				clipY = 0;
				clipW = GameCanvas.w * zoomLevel;
				clipH = GameCanvas.h * zoomLevel;
				return;
			}
			x *= zoomLevel;
			y *= zoomLevel;
			w *= zoomLevel;
			h *= zoomLevel;
			clipTX = translateX;
			clipTY = translateY;
			clipX = x;
			clipY = y;
			clipW = w;
			clipH = h;
			isClip = true;

			int screenClipX = clipX + clipTX;
			int screenClipY = clipY + clipTY;
			if (screenClipX <= 0 && screenClipY <= 0 &&
				screenClipX + clipW >= GameCanvas.w * zoomLevel &&
				screenClipY + clipH >= GameCanvas.h * zoomLevel)
			{
				isClip = false;
				clipTX = 0;
				clipTY = 0;
				clipX = 0;
				clipY = 0;
				clipW = GameCanvas.w * zoomLevel;
				clipH = GameCanvas.h * zoomLevel;
			}
		}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của getClipX.
	/// </summary>
	public int getClipX()
		{
			return GameScr.cmx;
		}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của getClipY.
	/// </summary>
	public int getClipY()
		{
			return GameScr.cmy;
		}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của getClipWidth.
	/// </summary>
	public int getClipWidth()
		{
			return GameScr.gW;
		}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của getClipHeight.
	/// </summary>
	public int getClipHeight()
		{
			return GameScr.gH;
		}

	/// <summary>
	/// Cập nhật trạng thái logic của UpdatePos theo chu kỳ khung hình.
	/// </summary>
	private void UpdatePos(int anchor)
		{
			Vector2 vector = new Vector2(0f, 0f);
			switch (anchor)
			{
			case 3:
				vector = new Vector2(size.x / 2f, size.y / 2f);
				break;
			case 20:
				vector = new Vector2(0f, 0f);
				break;
			case 17:
				vector = new Vector2(Screen.width / 2, 0f);
				break;
			case 24:
				vector = new Vector2(Screen.width, 0f);
				break;
			case 6:
				vector = new Vector2(0f, Screen.height / 2);
				break;
			case 10:
				vector = new Vector2(Screen.width, Screen.height / 2);
				break;
			case 36:
				vector = new Vector2(0f, Screen.height);
				break;
			case 33:
				vector = new Vector2(Screen.width / 2, Screen.height);
				break;
			case 40:
				vector = new Vector2(Screen.width, Screen.height);
				break;
			}
			pos = vector + relativePosition;
			rect = new Rect(pos.x - size.x * 0.5f, pos.y - size.y * 0.5f, size.x, size.y);
			pivot = new Vector2(rect.xMin + rect.width * 0.5f, rect.yMin + rect.height * 0.5f);
		}

	/// <summary>
	/// Dọn dẹp và đặt lại trạng thái ban đầu cho reset.
	/// </summary>
	public void reset()
		{
			isClip = false;
			isTranslate = false;
			translateX = 0;
			translateY = 0;
			GUIUtility.ResetMatrixStack();
		}

}

using System;
using System.Collections;
using Assets.src.e;
using UnityEngine;

/// <summary>
/// Thành phần phân mảnh (partial) của lớp mGraphics: Triển khai kết xuất chuỗi văn bản, căn lề và định dạng màu chữ.
/// </summary>
public partial class mGraphics
{
	/// <summary>
	/// Thiết lập giá trị hoặc trạng thái cho setColorMiniMap.
	/// </summary>
	public Color setColorMiniMap(int rgb)
		{
			int num = rgb & 0xFF;
			int num2 = (rgb >> 8) & 0xFF;
			int num3 = (rgb >> 16) & 0xFF;
			float num4 = (float)num / 256f;
			float num5 = (float)num2 / 256f;
			float num6 = (float)num3 / 256f;
			return new Color(num6, num5, num4);
		}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của getRGB.
	/// </summary>
	public float[] getRGB(Color cl)
		{
			float num = 256f * cl.r;
			float num2 = 256f * cl.g;
			float num3 = 256f * cl.b;
			return new float[3] { num, num2, num3 };
		}

	/// <summary>
	/// Thiết lập giá trị hoặc trạng thái cho setColor.
	/// </summary>
	public void setColor(int rgb)
		{
			int num = rgb & 0xFF;
			int num2 = (rgb >> 8) & 0xFF;
			int num3 = (rgb >> 16) & 0xFF;
			b = (float)num / 256f;
			g = (float)num2 / 256f;
			r = (float)num3 / 256f;
			a = 1f;
		}

	/// <summary>
	/// Thiết lập giá trị hoặc trạng thái cho setColor.
	/// </summary>
	public void setColor(Color color)
		{
			b = color.b;
			g = color.g;
			r = color.r;
			a = (color.a > 1f) ? (color.a / 255f) : color.a;
		}

	/// <summary>
	/// Thiết lập giá trị hoặc trạng thái cho setBgColor.
	/// </summary>
	public void setBgColor(int rgb)
		{
			if (rgb != currentBGColor)
			{
				currentBGColor = rgb;
				int num = rgb & 0xFF;
				int num2 = (rgb >> 8) & 0xFF;
				int num3 = (rgb >> 16) & 0xFF;
				b = (float)num / 256f;
				g = (float)num2 / 256f;
				r = (float)num3 / 256f;
				Main.main.GetComponent<Camera>().backgroundColor = new Color(r, g, b);
			}
		}

	/// <summary>
	/// Thực hiện vẽ và hiển thị thành phần drawString lên canvas đồ họa.
	/// </summary>
	public void drawString(string s, int x, int y, GUIStyle style)
		{
			x *= zoomLevel;
			y *= zoomLevel;
			if (isTranslate)
			{
				x += translateX;
				y += translateY;
			}
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			if (isClip)
			{
				num = clipX;
				num2 = clipY;
				num3 = clipW;
				num4 = clipH;
				if (isTranslate)
				{
					num += clipTX;
					num2 += clipTY;
				}
			}
			if (isClip)
			{
				GUI.BeginGroup(new Rect(num, num2, num3, num4));
			}
			GUI.Label(new Rect(x - num, y - num2, ScaleGUI.WIDTH, 100f), s, style);
			if (isClip)
			{
				GUI.EndGroup();
			}
		}

	/// <summary>
	/// Thiết lập giá trị hoặc trạng thái cho setColor.
	/// </summary>
	public void setColor(int rgb, float alpha)
		{
			int num = rgb & 0xFF;
			int num2 = (rgb >> 8) & 0xFF;
			int num3 = (rgb >> 16) & 0xFF;
			b = (float)num / 256f;
			g = (float)num2 / 256f;
			r = (float)num3 / 256f;
			a = (alpha > 1f) ? (alpha / 255f) : alpha;
		}

	/// <summary>
	/// Thực hiện vẽ và hiển thị thành phần drawString lên canvas đồ họa.
	/// </summary>
	public void drawString(string s, int x, int y, GUIStyle style, int w)
		{
			x *= zoomLevel;
			y *= zoomLevel;
			if (isTranslate)
			{
				x += translateX;
				y += translateY;
			}
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			if (isClip)
			{
				num = clipX;
				num2 = clipY;
				num3 = clipW;
				num4 = clipH;
				if (isTranslate)
				{
					num += clipTX;
					num2 += clipTY;
				}
			}
			if (isClip)
			{
				GUI.BeginGroup(new Rect(num, num2, num3, num4));
			}
			GUI.Label(new Rect(x - num, y - num2 - 4, w, 100f), s, style);
			if (isClip)
			{
				GUI.EndGroup();
			}
		}

	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của isNotTranColor.
	/// </summary>
	public static bool isNotTranColor(Color color)
		{
			if (color.a <= 0.05f || color == Color.clear || color == transParentColor)
			{
				return false;
			}
			return true;
		}

	/// <summary>
	/// Thiết lập giá trị hoặc trạng thái cho setColorObj.
	/// </summary>
	public static Color setColorObj(int rgb)
		{
			int num = rgb & 0xFF;
			int num2 = (rgb >> 8) & 0xFF;
			int num3 = (rgb >> 16) & 0xFF;
			float num4 = (float)num / 256f;
			float num5 = (float)num2 / 256f;
			float num6 = (float)num3 / 256f;
			return new Color(num6, num5, num4);
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ fillTrans.
	/// </summary>
	public void fillTrans(Image imgTrans, int x, int y, int w, int h)
		{
			setColor(0, 0.5f);
			fillRect(x * zoomLevel, y * zoomLevel, w * zoomLevel, h * zoomLevel);
		}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của getIntByColor.
	/// </summary>
	public static int getIntByColor(Color cl)
		{
			float num = cl.r * 255f;
			float num2 = cl.b * 255f;
			float num3 = cl.g * 255f;
			return (((int)num & 0xFF) << 16) | (((int)num3 & 0xFF) << 8) | ((int)num2 & 0xFF);
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ fillArg.
	/// </summary>
	public void fillArg(int i, int j, int k, int l, int m, int n)
		{
			fillRect(i * zoomLevel, j * zoomLevel, k * zoomLevel, l * zoomLevel);
		}

}

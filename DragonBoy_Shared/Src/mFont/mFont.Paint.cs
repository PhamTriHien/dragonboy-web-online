using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Thành phần phân mảnh (partial) của lớp mFont: Đảm nhiệm kết xuất đồ họa, vẽ giao diện, hiệu ứng và nhân vật ().
/// </summary>
public partial class mFont
{
	/// <summary>
	/// Thiết lập giá trị hoặc trạng thái cho setTypePaint.
	/// </summary>
	public void setTypePaint(mGraphics g, string st, int x, int y, int align, sbyte idFont)
		{
			sbyte b = id;
			if (idFont > 0)
			{
				b = idFont;
			}
			x--;
			if (id > 24)
			{
				Color[] array = new Color[7]
				{
					setColor(6029312),
					setColor(7169025),
					setColor(7680),
					setColor(0),
					setColor(9264),
					setColor(6029312),
					setColor(5197647)
				};
				int idx = System.Math.Clamp(id - 25, 0, array.Length - 1);
				color1 = array[idx];
				color2 = array[idx];
				_drawString(g, st, x + 1, y, align);
				_drawString(g, st, x - 1, y, align);
				_drawString(g, st, x, y - 1, align);
				_drawString(g, st, x, y + 1, align);
				_drawString(g, st, x + 1, y + 1, align);
				_drawString(g, st, x + 1, y - 1, align);
				_drawString(g, st, x - 1, y - 1, align);
				_drawString(g, st, x - 1, y + 1, align);
				color1 = bigColor(id);
				color2 = bigColor(id);
			}
			else
			{
				setColorByID(b);
			}
			_drawString(g, st, x, y - yAdd, align);
		}

	/// <summary>
	/// Thực hiện vẽ và hiển thị thành phần drawString lên canvas đồ họa.
	/// </summary>
	public void drawString(mGraphics g, string st, int x, int y, int align)
		{
			setTypePaint(g, st, x, y, align, 0);
		}

	/// <summary>
	/// Thực hiện vẽ và hiển thị thành phần drawStringBorder lên canvas đồ họa.
	/// </summary>
	public void drawStringBorder(mGraphics g, string st, int x, int y, int align)
		{
			setTypePaint(g, st, x, y, align, 0);
		}

	/// <summary>
	/// Thực hiện vẽ và hiển thị thành phần drawStringBorder lên canvas đồ họa.
	/// </summary>
	public void drawStringBorder(mGraphics g, string st, int x, int y, int align, mFont font2)
		{
			drawStringBd(g, st, x, y, align, font2);
		}

	/// <summary>
	/// Thực hiện vẽ và hiển thị thành phần drawStringBd lên canvas đồ họa.
	/// </summary>
	public void drawStringBd(mGraphics g, string st, int x, int y, int align, mFont font)
		{
			sbyte b = (font != null) ? font.id : (sbyte)0;
			setTypePaint(g, st, x - 1, y - 1, align, b);
			setTypePaint(g, st, x - 1, y + 1, align, b);
			setTypePaint(g, st, x + 1, y - 1, align, b);
			setTypePaint(g, st, x + 1, y + 1, align, b);
			setTypePaint(g, st, x, y - 1, align, b);
			setTypePaint(g, st, x, y + 1, align, b);
			setTypePaint(g, st, x + 1, y, align, b);
			setTypePaint(g, st, x - 1, y, align, b);
			setTypePaint(g, st, x, y, align, 0);
		}

	/// <summary>
	/// Thực hiện vẽ và hiển thị thành phần drawString lên canvas đồ họa.
	/// </summary>
	public void drawString(mGraphics g, string st, int x, int y, int align, mFont font)
		{
			if (font != null)
			{
				setTypePaint(g, st, x, y + 1, align, font.id);
			}
			setTypePaint(g, st, x, y, align, 0);
		}

	private static readonly GUIStyle s_StyleLeft = new GUIStyle { alignment = TextAnchor.UpperLeft };
	private static readonly GUIStyle s_StyleRight = new GUIStyle { alignment = TextAnchor.UpperRight };
	private static readonly GUIStyle s_StyleCenter = new GUIStyle { alignment = TextAnchor.UpperCenter };

	/// <summary>
	/// Thực thi phương thức nghiệp vụ _drawString.
	/// </summary>
	public void _drawString(mGraphics g, string st, int x0, int y0, int align)
		{
			if (string.IsNullOrEmpty(st)) return;
			y0 += yAddFont;
			GUIStyle gUIStyle;
			float num = 0f;
			float num2 = y0;
			switch (align)
			{
			case 0:
				num = x0;
				gUIStyle = s_StyleLeft;
				break;
			case 1:
				num = x0 - GameCanvas.w;
				gUIStyle = s_StyleRight;
				break;
			default:
				num = x0 - GameCanvas.w / 2;
				gUIStyle = s_StyleCenter;
				break;
			}
			gUIStyle.font = myFont;
			gUIStyle.normal.textColor = color1;
			g.drawString(st, (int)num, (int)num2, gUIStyle);
		}

}

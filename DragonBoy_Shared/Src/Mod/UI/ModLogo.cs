using System;
using UnityEngine;

/// <summary>
/// Module Logo Bản Quyền: Hiển thị logo thương hiệu Mod TriHienKun ở góc màn hình hoặc vị trí tùy chỉnh.
/// </summary>
public static class ModLogo
{
	public static bool isShowLogoInGame = true;
	public static int logoPosition = 1; // 0: Góc phải trên (Top-Right), 1: Giữa trên sát mép (Top-Center)

	private static Image inGameLogo = null;
	private static bool isLoaded = false;

	/// <summary>
	/// Khởi tạo và nạp dữ liệu cấu hình cho LoadLogo.
	/// </summary>
	public static void LoadLogo()
	{
		try
		{
			if (inGameLogo != null)
			{
				return;
			}
			inGameLogo = GameCanvas.loadCustomImage("custom_logo.png");
			isLoaded = (inGameLogo != null);
		}
		catch
		{
		}
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của GetLogo.
	/// </summary>
	public static Image GetLogo()
	{
		if (inGameLogo == null && !isLoaded)
		{
			LoadLogo();
		}
		return inGameLogo;
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của GetBounds.
	/// </summary>
	public static void GetBounds(out int x, out int y, out int w, out int h)
	{
		Image img = GetLogo();
		w = (img != null) ? mGraphics.getImageWidth(img) : 140;
		h = (img != null) ? mGraphics.getImageHeight(img) : 76;

		if (logoPosition == 0)
		{
			// Góc phải trên (Top-Right)
			x = GameCanvas.w - w - 8 - SafeArea.Right;
			y = SafeArea.Top;
		}
		else
		{
			// Giữa game, sát phía trên cùng màn hình (Top-Center)
			x = GameCanvas.hw - w / 2;
			y = SafeArea.Top;
		}
	}

	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của IsPointerInsideLogo.
	/// </summary>
	public static bool IsPointerInsideLogo(int px, int py)
	{
		if (!isShowLogoInGame || !ModMenu.IsInGame())
		{
			return false;
		}
		GetBounds(out int lx, out int ly, out int lw, out int lh);
		return (px >= lx && px <= lx + lw && py >= ly && py <= ly + lh);
	}

	/// <summary>
	/// Thực hiện vẽ toàn bộ đối tượng và các lớp hiển thị lên canvas mGraphics.
	/// </summary>
	public static void Paint(mGraphics g)
	{
		try
		{
			if (!isShowLogoInGame || !ModMenu.IsInGame() || g == null)
			{
				return;
			}

			// Tự động ẩn khi đang mở Panel, Menu, Hộp thoại hoặc Giao diện Mod
			if ((GameCanvas.panel != null && GameCanvas.panel.isShow) ||
			    (GameCanvas.panel2 != null && GameCanvas.panel2.isShow) ||
			    (GameCanvas.menu != null && GameCanvas.menu.showMenu) ||
			    GameCanvas.currentDialog != null ||
			    ModUI.uiCustomOpen)
			{
				return;
			}

			Image img = GetLogo();
			if (img == null)
			{
				return;
			}

			g.translate(-g.getTranslateX(), -g.getTranslateY());
			g.setClip(0, 0, GameCanvas.w, GameCanvas.h);

			GetBounds(out int drawX, out int drawY, out int drawW, out int drawH);
			g.drawImage(img, drawX, drawY, mGraphics.TOP | mGraphics.LEFT);
		}
		catch
		{
		}
	}
}

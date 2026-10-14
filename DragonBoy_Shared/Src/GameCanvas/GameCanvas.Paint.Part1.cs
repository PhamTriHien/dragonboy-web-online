using System;
using Assets.src.e;
using Assets.src.g;
using UnityEngine;
/// <summary>
/// Thành phần phân mảnh (partial) của lớp GameCanvas: Đảm nhiệm kết xuất đồ họa, vẽ giao diện, hiệu ứng và nhân vật (Part1).
/// </summary>
public partial class GameCanvas : IActionListener
{
	/// <summary>
	/// Thực hiện vẽ và hiển thị thành phần paintCloud lên canvas đồ họa.
	/// </summary>
	public static void paintCloud(mGraphics g)
		{
		}
	/// <summary>
	/// Cập nhật trạng thái logic của updateBG theo chu kỳ khung hình.
	/// </summary>
	public static void updateBG()
		{
		}
	/// <summary>
	/// Thực hiện vẽ và hiển thị thành phần paintBackgroundtLayer lên canvas đồ họa.
	/// </summary>
	public static void paintBackgroundtLayer(mGraphics g, int layer, int deltaY, int color1, int color2)
		{
			try
			{
				int num = layer - 1;
				if (num == imgBG.Length - 1 && (GameScr.gI().isRongThanXuatHien || GameScr.gI().isFireWorks))
				{
					g.setColor(GameScr.gI().mautroi);
					g.fillRect(0, 0, w, h);
					if (typeBg == 2 || typeBg == 4 || typeBg == 7)
					{
						drawSun1(g);
						drawSun2(g);
					}
					if (GameScr.gI().isFireWorks && !lowGraphic)
					{
						FireWorkEff.paint(g);
					}
				}
				else
				{
					if (imgBG == null || imgBG[num] == null || bgW[num] <= 0)
					{
						return;
					}
					if (moveX[num] != 0)
					{
						moveX[num] += moveXSpeed[num];
					}
					int cmy = GameScr.cmy;
					if (cmy > h)
					{
						cmy = h;
					}
					if (cmy < 0)
					{
						cmy = 0;
					}
					int maxDrawW = (GameScr.gW > w) ? GameScr.gW : w;
					int maxDrawH = (GameScr.gH > h) ? GameScr.gH : h;
					if (layerSpeed[num] != 0)
					{
						int move = (GameScr.cmx + moveX[num]) >> layerSpeed[num];
						int offset = (move % bgW[num] + bgW[num]) % bgW[num];
						for (int i = -offset; i < maxDrawW + bgW[num]; i += bgW[num])
						{
							g.drawImage(imgBG[num], i, yb[num] - ((deltaY > 0) ? (cmy >> deltaY) : 0), 0);
						}
					}
					else
					{
						for (int j = 0; j < maxDrawW + bgW[num]; j += bgW[num])
						{
							g.drawImage(imgBG[num], j, yb[num] - ((deltaY > 0) ? (cmy >> deltaY) : 0), 0);
						}
					}
					if (color1 != -1)
					{
						if (num == nBg - 1)
						{
							fillRect(g, color1, 0, -(cmy >> deltaY), maxDrawW, yb[num], deltaY);
						}
						else
						{
							fillRect(g, color1, 0, yb[num - 1] + bgH[num - 1], maxDrawW, yb[num] - (yb[num - 1] + bgH[num - 1]), deltaY);
						}
					}
					if (color2 != -1)
					{
						if (num == 0)
						{
							fillRect(g, color2, 0, yb[num] + bgH[num], maxDrawW, maxDrawH - (yb[num] + bgH[num]), deltaY);
						}
						else
						{
							fillRect(g, color2, 0, yb[num] + bgH[num], maxDrawW, yb[num - 1] - (yb[num] + bgH[num]) + 80, deltaY);
						}
					}
					if (currentScreen == GameScr.instance)
					{
						if (layer == 1 && typeBg == 11)
						{
							g.drawImage(imgSun2, -(GameScr.cmx >> layerSpeed[0]) + 400, yb[0] + 30 - (cmy >> 2), StaticObj.BOTTOM_HCENTER);
						}
						if (layer == 1 && typeBg == 13)
						{
							g.drawImage(imgBG[1], -(GameScr.cmx >> layerSpeed[0]) + TileMap.tmw * 24 / 4, yb[0] - (cmy >> 3) + 30, 0);
							g.drawRegion(imgBG[1], 0, 0, bgW[1], bgH[1], 2, -(GameScr.cmx >> layerSpeed[0]) + TileMap.tmw * 24 / 4 + bgW[1], yb[0] - (cmy >> 3) + 30, 0);
						}
						if (layer == 3 && TileMap.mapID == 1)
						{
							for (int k = 0; k < TileMap.pxh / mGraphics.getImageHeight(imgCaycot); k++)
							{
								g.drawImage(imgCaycot, -(GameScr.cmx >> layerSpeed[2]) + 300, k * mGraphics.getImageHeight(imgCaycot) - (cmy >> 3), 0);
							}
						}
					}
					int x = -(GameScr.cmx + moveX[num] >> layerSpeed[num]);
					EffecMn.paintBackGroundUnderLayer(g, x, yb[num] + bgH[num] - (cmy >> deltaY), num);
				}
			}
			catch (Exception ex)
			{
				Cout.LogError("Loi ham paint bground: " + ex.ToString());
			}
		}
	/// <summary>
	/// Thực hiện vẽ và hiển thị thành phần drawSun1 lên canvas đồ họa.
	/// </summary>
	public static void drawSun1(mGraphics g)
		{
			if (imgSun != null)
			{
				g.drawImage(imgSun, sunX, sunY, 0);
			}
			if (!isBoltEff)
			{
				return;
			}
			if (gameTick % 200 == 0)
			{
				boltActive = true;
			}
			if (boltActive)
			{
				tBolt++;
				if (tBolt == 10)
				{
					tBolt = 0;
					boltActive = false;
				}
				if (tBolt % 2 == 0)
				{
					g.setColor(16777215);
					g.fillRect(0, 0, w, h);
				}
			}
		}
	/// <summary>
	/// Thực hiện vẽ và hiển thị thành phần drawSun2 lên canvas đồ họa.
	/// </summary>
	public static void drawSun2(mGraphics g)
		{
			if (imgSun2 != null)
			{
				g.drawImage(imgSun2, sunX2, sunY2, 0);
			}
		}
	/// <summary>
	/// Thực hiện vẽ và hiển thị thành phần paint_ios_bg lên canvas đồ họa.
	/// </summary>
	public static void paint_ios_bg(mGraphics g)
		{
			if (mSystem.clientType != 5)
			{
				return;
			}
			if (imgBgIOS != null)
			{
				g.setColor(0);
				g.fillRect(0, 0, w, h);
				for (int i = 0; i < 3; i++)
				{
					g.drawImage(imgBgIOS, imgBgIOS.getWidth() * i, h / 2, mGraphics.VCENTER | mGraphics.HCENTER);
				}
			}
			else
			{
				int num = ((TileMap.bgID % 2 != 0) ? 1 : 2);
				imgBgIOS = mSystem.loadImage("/bg/bg_ios_" + num + ".png");
			}
		}

}

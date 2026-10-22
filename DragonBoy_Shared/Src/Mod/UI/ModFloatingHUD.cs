using System;
using UnityEngine;

/// <summary>
/// Module Menu Nổi (Floating HUD): Cung cấp bong bóng phím ảo nổi trên màn hình để mở nhanh Menu Quản Lý Tab (HUB).
/// </summary>
public static class ModFloatingHUD
{
    public static bool isShowFloatingHUD = true;
    public static bool isExpanded;
    public static int bubbleX = 10;
    public static int bubbleY = 46;
    public const int BUBBLE_RADIUS = 16;
    public const int BUBBLE_SIZE = 32;
    private static bool dragging;
    private static bool panelHeaderDragging;
    private static bool moved;
    private static int startX;
    private static int startY;
    private static int offsetX;
    private static int offsetY;
    private static bool panelPointer;
    private static bool closePressed;
    private static bool outsidePressed;
    private static readonly ModTabPanel tabs = new ModTabPanel();

    public static void SyncState(bool expanded, int x, int y)
    {
        isExpanded = expanded;
        bubbleX = x;
        bubbleY = y;
        dragging = panelPointer = panelHeaderDragging = false;
        tabs.SelectTab(Application.CurrentTabId);
        tabs.CancelPointer();
    }

    private static bool Visible => isShowFloatingHUD &&
        !(GameCanvas.panel?.isShow ?? false) && !(GameCanvas.panel2?.isShow ?? false) && !ModUI.uiCustomOpen;

    /// <summary>
    /// Nạp và trả về nút tròn gốc NRO (/mainImage/myTexture2dnut.png & myTexture2dnutF.png).
    /// </summary>
    private static Image GetNativeBubbleImage(bool isFocus)
    {
        try
        {
            if (GameScr.imgNut == null)
            {
                GameScr.imgNut = GameCanvas.loadImage("/mainImage/myTexture2dnut.png");
            }
            if (GameScr.imgNutF == null)
            {
                GameScr.imgNutF = GameCanvas.loadImage("/mainImage/myTexture2dnutF.png");
            }
        }
        catch
        {
        }
        return isFocus ? (GameScr.imgNutF ?? GameScr.imgNut) : (GameScr.imgNut ?? GameScr.imgNutF);
    }

    /// <summary>
    /// Thực thi phương thức nghiệp vụ ClampBubble.
    /// </summary>
    private static void ClampBubble()
    {
        int minX = SafeArea.Left + 2;
        int minY = SafeArea.Top + 2;
        bubbleX = System.Math.Clamp(bubbleX, minX, System.Math.Max(minX, GameCanvas.w - SafeArea.Right - BUBBLE_SIZE - 2));
        bubbleY = System.Math.Clamp(bubbleY, minY, System.Math.Max(minY, GameCanvas.h - SafeArea.Bottom - BUBBLE_SIZE - 2));
    }

    /// <summary>
    /// Tính toán tọa độ khung cửa sổ HUB khi mở rộng.
    /// </summary>
    private static (int x, int y, int width, int height) PanelBounds()
    {
        int width = System.Math.Min(236, System.Math.Max(190, GameCanvas.w - SafeArea.Left - SafeArea.Right - 8));
        int height = 110;
        int x = System.Math.Clamp(bubbleX, SafeArea.Left + 4,
            System.Math.Max(SafeArea.Left + 4, GameCanvas.w - SafeArea.Right - width - 4));
        int y = System.Math.Clamp(bubbleY, SafeArea.Top + 4,
            System.Math.Max(SafeArea.Top + 4, GameCanvas.h - SafeArea.Bottom - height - 4));
        return (x, y, width, height);
    }

    /// <summary>
    /// Thực thi phương thức nghiệp vụ Inside.
    /// </summary>
    private static bool Inside(int px, int py, int x, int y, int w, int h)
        => px >= x && px < x + w && py >= y && py < y + h;

    /// <summary>
    /// Dọn dẹp và đặt lại trạng thái ban đầu cho ClearPointer.
    /// </summary>
    private static void ClearPointer()
    {
        GameCanvas.clearAllPointerEvent();
        if (GameScr.instance != null) GameScr.instance.clickMoving = false;
        Char me = Char.myCharz();
        if (me != null)
        {
            me.currentMovePoint = null;
            me.vMovePoints?.removeAllElements();
        }
    }

    /// <summary>
    /// Thực hiện vẽ toàn bộ đối tượng và các lớp hiển thị lên canvas mGraphics.
    /// </summary>
    public static void Paint(mGraphics g)
    {
        if (!Visible || g == null) return;
        ClampBubble();
        g.translate(-g.getTranslateX(), -g.getTranslateY());
        g.setClip(0, 0, GameCanvas.w, GameCanvas.h);

        if (isExpanded)
        {
            var b = PanelBounds();
            GameCanvas.paintz.paintFrame(b.x, b.y, b.width, b.height, g);
            GameCanvas.paintz.paintFrameInside(b.x + 6, b.y + 6, b.width - 12, b.height - 12, g);
            mFont.tahoma_7b_dark.drawString(g, "HUB - TAB " + Application.CurrentTabId, b.x + b.width / 2, b.y + 7, mFont.CENTER);
            ModUI.PaintCloseButton(b.x + b.width - 23, b.y + 6, g);
            tabs.Paint(b.x + 8, b.y + 23, b.width - 16, b.height - 29, g);
        }
        else
        {
            int cx = bubbleX + BUBBLE_RADIUS;
            int cy = bubbleY + BUBBLE_RADIUS;
            bool isHover = Inside(GameCanvas.px, GameCanvas.py, bubbleX, bubbleY, BUBBLE_SIZE, BUBBLE_SIZE);
            Image nutImg = GetNativeBubbleImage(dragging || isHover);
            if (nutImg != null)
            {
                g.drawImage(nutImg, cx, cy, mGraphics.HCENTER | mGraphics.VCENTER);
            }
            else
            {
                ModUI.PaintNativeButton(bubbleX, bubbleY, BUBBLE_SIZE, BUBBLE_SIZE, string.Empty, dragging || isHover, g);
            }
            mFont.tahoma_7b_white.drawString(g, "HUB", cx, cy - 5, mFont.CENTER);
        }

        g.setClip(0, 0, GameCanvas.w, GameCanvas.h);
    }

    /// <summary>
    /// Kiểm tra điều kiện hợp lệ hoặc trạng thái của CheckClick.
    /// </summary>
    public static bool CheckClick()
    {
        if (!Visible)
        {
            dragging = panelPointer = panelHeaderDragging = false;
            tabs.CancelPointer();
            return false;
        }
        ClampBubble();
        int px = GameCanvas.px;
        int py = GameCanvas.py;
        bool press = GameCanvas.isPointerJustDown;
        bool release = GameCanvas.isPointerJustRelease || GameCanvas.isPointerClick;

        if (!isExpanded)
        {
            int dx = px - bubbleX - BUBBLE_RADIUS;
            int dy = py - bubbleY - BUBBLE_RADIUS;
            if (press && dx * dx + dy * dy <= (BUBBLE_RADIUS + 3) * (BUBBLE_RADIUS + 3))
            {
                dragging = true;
                moved = false;
                startX = px;
                startY = py;
                offsetX = px - bubbleX;
                offsetY = py - bubbleY;
            }
            if (!dragging) return false;
            if (System.Math.Abs(px - startX) > 4 || System.Math.Abs(py - startY) > 4) moved = true;
            if (moved)
            {
                bubbleX = px - offsetX;
                bubbleY = py - offsetY;
                ClampBubble();
            }
            if (release)
            {
                dragging = false;
                if (moved)
                {
                    ModConfig.SaveConfig();
                }
                else
                {
                    isExpanded = true;
                    SoundMn.gI()?.buttonClick();
                }
            }
            ClearPointer();
            return true;
        }

        var b = PanelBounds();
        bool overPanel = Inside(px, py, b.x, b.y, b.width, b.height);
        bool overClose = Inside(px, py, b.x + b.width - 26, b.y + 4, 22, 19);
        bool overHeader = Inside(px, py, b.x, b.y, b.width - 26, 22);

        if (press)
        {
            panelPointer = true;
            closePressed = overClose;
            outsidePressed = !overPanel;
            if (overHeader)
            {
                panelHeaderDragging = true;
                moved = false;
                startX = px;
                startY = py;
                offsetX = px - b.x;
                offsetY = py - b.y;
            }
        }

        if (panelHeaderDragging && GameCanvas.isPointerDown)
        {
            if (System.Math.Abs(px - startX) > 4 || System.Math.Abs(py - startY) > 4) moved = true;
            if (moved)
            {
                bubbleX = px - offsetX;
                bubbleY = py - offsetY;
                ClampBubble();
            }
        }

        bool handled = !panelHeaderDragging && tabs.HandlePointer(b.x + 8, b.y + 23, b.width - 16, b.height - 29);

        if (release)
        {
            if (panelHeaderDragging && moved)
            {
                ModConfig.SaveConfig();
            }
            if ((panelPointer && closePressed && overClose) || (panelPointer && outsidePressed && !overPanel))
            {
                isExpanded = false;
                SoundMn.gI()?.buttonClose();
            }
            panelPointer = false;
            panelHeaderDragging = false;
            tabs.CancelPointer();
        }
        if (handled || panelPointer || press || release || GameCanvas.isPointerDown)
        {
            ClearPointer();
            return true;
        }
        return false;
    }
}

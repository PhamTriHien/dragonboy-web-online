using System;
using DragonBoy_Net8_Native.Engine.Platform;
using UnityEngine;

/// <summary>
/// CLASS ModTabPanel: Định nghĩa và điều phối giao diện Quản Lý Tab (HUB & Menu Nhân Bản) đồng bộ 100% asset UI gốc NRO.
/// </summary>
public sealed class ModTabPanel
{
    public int SelectedTab { get; private set; } = 1;
    private bool initializedSelection;
    private int scrollX;
    private bool pressed;
    private bool dragged;
    private bool stripPressed;
    private int startX;
    private int startY;
    private int initialScroll;
    private const int TabWidth = 68;
    private const int TabStep = 71;
    private const int StripHeight = 19;
    private const int AddBtnWidth = 22;
    private const int ActionBtnHeight = 18;
    private readonly bool showDataActions;

    /// <summary>
    /// Thực thi phương thức nghiệp vụ ModTabPanel.
    /// </summary>
    public ModTabPanel(bool showDataActions = false)
    {
        this.showDataActions = showDataActions;
    }

    /// <summary>
    /// Thực thi phương thức nghiệp vụ Inside.
    /// </summary>
    private static bool Inside(int px, int py, int x, int y, int w, int h)
        => px >= x && px < x + w && py >= y && py < y + h;

    /// <summary>
    /// Thực thi phương thức nghiệp vụ Tabs.
    /// </summary>
    private int[] Tabs(int width)
    {
        int[] tabs = GameTabRegistry.GetTabs();
        if (!initializedSelection)
        {
            initializedSelection = true;
            SelectedTab = System.Math.Max(1, Application.CurrentTabId);
        }
        if (Array.IndexOf(tabs, SelectedTab) < 0) SelectedTab = tabs[0];
        scrollX = System.Math.Clamp(scrollX, 0, System.Math.Max(0, tabs.Length * TabStep - width));
        return tabs;
    }

    /// <summary>
    /// Cắt chuỗi vừa khít độ rộng hiển thị.
    /// </summary>
    private static string FitText(mFont font, string text, int maxWidth)
    {
        if (string.IsNullOrEmpty(text) || font == null || maxWidth <= 0) return string.Empty;
        if (font.getWidth(text) <= maxWidth) return text;
        string trimmed = text;
        while (trimmed.Length > 0 && font.getWidth(trimmed + "..") > maxWidth)
        {
            trimmed = trimmed.Substring(0, trimmed.Length - 1);
        }
        return trimmed + "..";
    }

    /// <summary>
    /// Thực hiện vẽ toàn bộ đối tượng và các lớp hiển thị lên canvas mGraphics.
    /// </summary>
    public void Paint(int x, int y, int width, int height, mGraphics g)
    {
        int rowWidth = System.Math.Max(1, width - AddBtnWidth - 3);
        int addX = x + width - AddBtnWidth;
        int bw = (width - 4) / 2;
        int rightBtnX = x + bw + 4;

        int[] tabs = Tabs(rowWidth);
        GameTabStatus.RefreshAllTabs();

        g.setClip(x, y, rowWidth, StripHeight + 2);
        for (int i = 0; i < tabs.Length; i++)
        {
            int tx = x + i * TabStep - scrollX;
            if (tx + TabWidth <= x || tx >= x + rowWidth) continue;

            int tabId = tabs[i];
            string label = (tabId == Application.CurrentTabId) ? ("*Tab " + tabId) : ("Tab " + tabId);
            ModUI.PaintNativeButton(tx, y, TabWidth, StripHeight, label, tabId == SelectedTab, g);
        }

        g.setClip(x, y, width, height);
        ModUI.PaintNativeButton(addX, y, AddBtnWidth, StripHeight, "+", false, g);

        ModUI.PaintNativeButton(x, y + 23, bw, ActionBtnHeight, "Mở", SelectedTab == Application.CurrentTabId, g);
        ModUI.PaintNativeButton(rightBtnX, y + 23, bw, ActionBtnHeight, "Dừng", false, g);
        ModUI.PaintNativeButton(x, y + 44, bw, ActionBtnHeight, "Xóa", false, g);
        string pipLabel = OperatingSystem.IsBrowser()
            ? (ModGraphics.isFullscreen ? "Thoát Full" : "Full màn")
            : "PiP";
        bool pipActive = OperatingSystem.IsBrowser() && ModGraphics.isFullscreen;
        ModUI.PaintNativeButton(rightBtnX, y + 44, bw, ActionBtnHeight, pipLabel, pipActive, g);

        GameTabStatus.TabInfo selInfo = GameTabStatus.GetTabInfo(SelectedTab);
        bool selOnline = selInfo.IsAlive && selInfo.isOnline;
        string status = FitText(mFont.tahoma_7_grey, "Tab " + SelectedTab + ": " + GameTabStatus.GetStatusText(SelectedTab), width);
        (selOnline ? mFont.tahoma_7_green2 : mFont.tahoma_7_grey).drawString(g, status, x, y + 66, mFont.LEFT);

        if (showDataActions && height >= 102)
        {
            ModUI.PaintNativeButton(x, y + 82, bw, ActionBtnHeight, "Thư mục RMS", false, g);
            ModUI.PaintNativeButton(rightBtnX, y + 82, bw, ActionBtnHeight, "Reset dữ liệu", false, g);
        }
    }

    /// <summary>
    /// Xử lý nghiệp vụ logic chuyên trách cho HandlePointer.
    /// </summary>
    public bool HandlePointer(int x, int y, int width, int height)
    {
        int px = GameCanvas.px;
        int py = GameCanvas.py;
        bool release = GameCanvas.isPointerJustRelease || GameCanvas.isPointerClick;

        int rowWidth = System.Math.Max(1, width - AddBtnWidth - 3);
        int addX = x + width - AddBtnWidth;
        int bw = (width - 4) / 2;
        int rightBtnX = x + bw + 4;

        int[] tabs = Tabs(rowWidth);
        if (GameCanvas.pXYScrollMouse != 0 && Inside(px, py, x, y, rowWidth, StripHeight + 2))
        {
            int delta = GameCanvas.pXYScrollMouse > 0 ? -TabStep : TabStep;
            scrollX = System.Math.Clamp(scrollX + delta, 0, System.Math.Max(0, tabs.Length * TabStep - rowWidth));
            GameCanvas.pXYScrollMouse = 0;
            return true;
        }
        if (GameCanvas.isPointerJustDown && Inside(px, py, x, y, width, height))
        {
            pressed = true;
            dragged = false;
            stripPressed = Inside(px, py, x, y, rowWidth, StripHeight + 2);
            startX = px;
            startY = py;
            initialScroll = scrollX;
        }
        if (!pressed) return false;
        if (System.Math.Abs(px - startX) > 4 || System.Math.Abs(py - startY) > 4) dragged = true;
        if (stripPressed && dragged)
            scrollX = System.Math.Clamp(initialScroll + startX - px, 0, System.Math.Max(0, tabs.Length * TabStep - rowWidth));
        if (!release) return true;
        pressed = false;
        if (dragged) return true;
        if (stripPressed && Inside(px, py, x, y, rowWidth, StripHeight + 2))
        {
            int index = (px - x + scrollX) / TabStep;
            if (index >= 0 && index < tabs.Length && (px - x + scrollX) % TabStep < TabWidth)
            {
                int clickedTab = tabs[index];
                SelectedTab = clickedTab;
                SoundMn.gI()?.buttonClick();
            }
            return true;
        }
        if (Hit(addX, y, AddBtnWidth, StripHeight))
        {
            SelectedTab = GameTabRegistry.AddTab();
            GameTabManager.RequestPrewarmTab(SelectedTab);
            int[] updatedTabs = GameTabRegistry.GetTabs();
            int selIdx = Array.IndexOf(updatedTabs, SelectedTab);
            if (selIdx >= 0)
            {
                int targetX = selIdx * TabStep;
                if (targetX < scrollX) scrollX = targetX;
                else if (targetX + TabWidth > scrollX + rowWidth)
                    scrollX = System.Math.Max(0, targetX + TabWidth - rowWidth);
            }
            SoundMn.gI()?.buttonClick();
            return true;
        }

        if (Hit(x, y + 23, bw, ActionBtnHeight))
        {
            SoundMn.gI()?.buttonClick();
            GameTabManager.OpenTab(SelectedTab, closeHub: true);
        }
        else if (Hit(rightBtnX, y + 23, bw, ActionBtnHeight))
        {
            SoundMn.gI()?.buttonClick();
            GameTabManager.RequestStopTab(SelectedTab);
        }
        else if (Hit(x, y + 44, bw, ActionBtnHeight))
        {
            SoundMn.gI()?.buttonClick();
            if (SelectedTab != 1)
            {
                int deletingTab = SelectedTab;
                SelectedTab = 1;
                GameTabManager.RequestDeleteTab(deletingTab);
            }
            else
            {
                GameScr.info1?.addInfo("Tab 1 là Tab gốc, không thể xóa!", 0);
            }
        }
        else if (Hit(rightBtnX, y + 44, bw, ActionBtnHeight))
        {
            SoundMn.gI()?.buttonClick();
            if (OperatingSystem.IsBrowser())
            {
                ModGraphics.ToggleFullscreen();
            }
            else
            {
                PlatformService.Current.RequestPip();
            }
        }
        else if (showDataActions && height >= 102 && Hit(x, y + 82, bw, ActionBtnHeight))
        {
            SoundMn.gI()?.buttonClick();
            PlatformService.Current.OpenDataFolder(SelectedTab);
        }
        else if (showDataActions && height >= 102 && Hit(rightBtnX, y + 82, bw, ActionBtnHeight))
        {
            SoundMn.gI()?.buttonClick();
            PlatformService.Current.ClearTabData(SelectedTab);
        }
        return true;
    }

    public void SelectTab(int tabId)
    {
        initializedSelection = true;
        SelectedTab = System.Math.Max(1, tabId);
    }

    /// <summary>
    /// Thực thi phương thức nghiệp vụ Hit.
    /// </summary>
    private bool Hit(int x, int y, int w, int h)
        => Inside(startX, startY, x, y, w, h) && Inside(GameCanvas.px, GameCanvas.py, x, y, w, h);

    public void CancelPointer() => pressed = false;
}

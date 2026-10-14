/// <summary>
/// CLASS ModUIMultiTab: Định nghĩa và điều phối giao diện Tab Nhân Bản trong Menu Mod.
/// </summary>
public static class ModUIMultiTab
{
    private static readonly ModTabPanel tabs = new ModTabPanel(true);

    /// <summary>
    /// Thực hiện vẽ toàn bộ đối tượng và các lớp hiển thị lên canvas mGraphics.
    /// </summary>
    public static void Paint(int x, int y, int width, int height, mGraphics g)
    {
        tabs.Paint(x + 4, y + 4, width - 8, height - 8, g);
    }

    /// <summary>
    /// Xử lý nghiệp vụ logic chuyên trách cho HandlePointer.
    /// </summary>
    public static bool HandlePointer(int x, int y, int width, int height)
        => tabs.HandlePointer(x + 4, y + 4, width - 8, height - 8);

    /// <summary>
    /// Xử lý nghiệp vụ logic chuyên trách cho HandleTap.
    /// </summary>
    public static bool HandleTap(int px, int py, int x, int y, int width, int height)
        => HandlePointer(x, y, width, height);
}

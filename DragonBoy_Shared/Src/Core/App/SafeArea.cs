using DragonBoy_Net8_Native.Engine.Platform;

/// <summary>
/// CLASS SafeArea: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
public static class SafeArea
{
    public static int Left => PlatformService.Current.GetSafeAreaInsets().left;
    public static int Top => PlatformService.Current.GetSafeAreaInsets().top;
    public static int Right => PlatformService.Current.GetSafeAreaInsets().right;
    public static int Bottom => PlatformService.Current.GetSafeAreaInsets().bottom;
}

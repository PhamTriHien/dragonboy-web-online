using System;

/// <summary>
/// Module Tốc Chạy & Tốc Độ Game: Tinh chỉnh hệ số tốc độ di chuyển và điều tốc nhịp game mượt mà.
/// </summary>
public static class ModSpeed
{
	public static bool speedHack = false;
	public static float speedMult = 2f;
	public static int originalSpeed = -1;
	public static int lastCharId = -1;

	/// <summary>
	/// Thiết lập giá trị hoặc trạng thái cho SetSpeedHack.
	/// </summary>
	public static void SetSpeedHack(bool enabled)
	{
		speedHack = enabled;
		ModConfig.SaveConfig();
	}

	/// <summary>
	/// Thiết lập giá trị hoặc trạng thái cho SetSpeedMult.
	/// </summary>
	public static void SetSpeedMult(float mult)
	{
		speedMult = mult;
		ModConfig.SaveConfig();
	}
}

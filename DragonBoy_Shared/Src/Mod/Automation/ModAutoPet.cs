/// <summary>
/// Module Auto Đệ Tử: Quản lý trạng thái đệ tử (đi theo, bảo vệ, tấn công, về nhà) và tự động cho ăn đậu khi đệ xin.
/// </summary>
public static class ModAutoPet
{
	public static bool isEnabled;
	public static int status = 5;
	public static int intervalMs = 5000;
	private static long nextAt;
	/// <summary>
	/// Cấu hình tham số và thiết lập hoạt động cho module.
	/// </summary>
	public static void Configure(int value, int interval)
	{
		status = value < 0 ? 0 : (value > 20 ? 20 : value);
		intervalMs = interval < 1000 ? 1000 : (interval > 3600000 ? 3600000 : interval);
		isEnabled = true; nextAt = 0; ModConfig.SaveConfig();
	}
	/// <summary>
	/// Bật hoặc tắt trạng thái kích hoạt của Toggle.
	/// </summary>
	public static void Toggle()
	{
		isEnabled = !isEnabled; nextAt = 0; ModConfig.SaveConfig();
		GameScr.info1.addInfo("Auto Pet: " + (isEnabled ? "BẬT" : "TẮT"), 0);
	}
	/// <summary>
	/// Cập nhật trạng thái logic của đối tượng theo từng chu kỳ khung hình.
	/// </summary>
	public static void Update()
	{
		if (!isEnabled || !ModMenu.IsInGame() || Char.myCharz() == null || !Char.myCharz().havePet) return;
		long now = mSystem.currentTimeMillis();
		if (now < nextAt) return;
		Service.gI().petStatus((sbyte)status);
		nextAt = now + intervalMs;
	}
}

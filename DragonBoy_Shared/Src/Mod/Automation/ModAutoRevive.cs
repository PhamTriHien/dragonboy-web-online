/// <summary>
/// Module Auto Hồi Sinh: Tự động hồi sinh nhân vật tại chỗ hoặc về nhà khi bị tiêu diệt.
/// </summary>
public static class ModAutoRevive
{
	public static bool isEnabled;
	public static int intervalMs = 10000;
	private static long nextAt;

	/// <summary>
	/// Bật hoặc tắt trạng thái kích hoạt của Toggle.
	/// </summary>
	public static void Toggle()
	{
		isEnabled = !isEnabled;
		nextAt = 0;
		ModConfig.SaveConfig();
		GameScr.info1.addInfo("Auto hồi sinh: " + (isEnabled ? "BẬT" : "TẮT"), 0);
	}

	/// <summary>
	/// Cập nhật trạng thái logic của đối tượng theo từng chu kỳ khung hình.
	/// </summary>
	public static void Update()
	{
		if (!isEnabled || !ModMenu.IsInGame() || Char.myCharz() == null) return;
		Char me = Char.myCharz();
		if (me.statusMe != 14) return;
		long now = mSystem.currentTimeMillis();
		if (now < nextAt) return;
		Service.gI().wakeUpFromDead();
		nextAt = now + intervalMs;
	}
}

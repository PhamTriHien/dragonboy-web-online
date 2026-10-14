using System;

/// <summary>
/// Module Auto Cộng Tiềm Năng: Tự động cộng chỉ số sức mạnh, HP, KI, giáp, chí mạng theo thiết lập.
/// </summary>
public static class ModAutoPoint
{
	public static bool isEnabled;
	public static int targetType = 0;
	public static int pointsPerTick = 1;
	public static int intervalMs = 1000;
	private static long nextAt;

	/// <summary>
	/// Cấu hình tham số và thiết lập hoạt động cho module.
	/// </summary>
	public static void Configure(int type, int amount, int interval)
	{
		targetType = type < 0 ? 0 : (type > 3 ? 3 : type);
		pointsPerTick = amount < 1 ? 1 : (amount > 100 ? 100 : amount);
		intervalMs = interval < 500 ? 500 : (interval > 60000 ? 60000 : interval);
	}

	/// <summary>
	/// Bật hoặc tắt trạng thái kích hoạt của Toggle.
	/// </summary>
	public static void Toggle()
	{
		isEnabled = !isEnabled;
		nextAt = 0;
		GameScr.info1.addInfo("Auto cộng điểm: " + (isEnabled ? "BẬT" : "TẮT"), 0);
		ModConfig.SaveConfig();
	}

	/// <summary>
	/// Cập nhật trạng thái logic của đối tượng theo từng chu kỳ khung hình.
	/// </summary>
	public static void Update()
	{
		if (!isEnabled || !ModMenu.IsInGame() || Char.myCharz() == null || GameScr.gI() == null || GameCanvas.gameTick % 2 != 0) return;
		Char me = Char.myCharz();
		if (me.powerPoint <= 0) return;
		long now = mSystem.currentTimeMillis();
		if (now < nextAt) return;
		int amount = pointsPerTick < me.powerPoint ? pointsPerTick : me.powerPoint;
		if (amount <= 0) return;
		Service.gI().upPotential(targetType, amount);
		nextAt = now + intervalMs;
	}
}

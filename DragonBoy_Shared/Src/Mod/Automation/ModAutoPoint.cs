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
		pointsPerTick = (amount >= 100) ? 100 : ((amount >= 10) ? 10 : 1);
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

	private static long GetRequiredPotential(Char me, int type, int num)
	{
		if (me == null || num <= 0) return long.MaxValue;
		switch (type)
		{
			case 0: // HP Gốc
			{
				long baseHp = me.cHPGoc + 1000L;
				if (num == 1) return baseHp;
				if (num == 10) return 10L * (2L * baseHp + 180L) / 2L;
				if (num == 100) return 100L * (2L * baseHp + 1980L) / 2L;
				return long.MaxValue;
			}
			case 1: // KI Gốc
			{
				long baseMp = me.cMPGoc + 1000L;
				if (num == 1) return baseMp;
				if (num == 10) return 10L * (2L * baseMp + 180L) / 2L;
				if (num == 100) return 100L * (2L * baseMp + 1980L) / 2L;
				return long.MaxValue;
			}
			case 2: // Sức Đánh Gốc
			{
				long expForOne = me.expForOneAdd > 0 ? me.expForOneAdd : 100L;
				long baseDam = me.cDamGoc;
				if (num == 1) return baseDam * expForOne;
				if (num == 10) return 10L * (2L * baseDam + 9L) / 2L * expForOne;
				if (num == 100) return 100L * (2L * baseDam + 99L) / 2L * expForOne;
				return long.MaxValue;
			}
			case 3: // Giáp Gốc
			{
				long baseDef = me.cDefGoc + 5L;
				if (num == 1) return (2L * baseDef) / 2L * 100000L;
				if (num == 10) return 10L * (2L * baseDef + 9L) / 2L * 100000L;
				if (num == 100) return 100L * (2L * baseDef + 99L) / 2L * 100000L;
				return long.MaxValue;
			}
			default:
				return long.MaxValue;
		}
	}

	/// <summary>
	/// Cập nhật trạng thái logic của đối tượng theo từng chu kỳ khung hình.
	/// </summary>
	public static void Update()
	{
		if (!isEnabled || !ModMenu.IsInGame() || Char.myCharz() == null || GameScr.gI() == null || GameCanvas.gameTick % 2 != 0) return;
		if (Char.isLoadingMap || Char.ischangingMap) return;
		Char me = Char.myCharz();
		if (me.isDie || me.cHP <= 0 || me.statusMe == 14 || me.statusMe == 5 || me.cTiemNang <= 0) return;
		long now = mSystem.currentTimeMillis();
		if (now < nextAt) return;

		int[] tiers = (pointsPerTick >= 100) ? new int[] { 100, 10, 1 } : ((pointsPerTick >= 10) ? new int[] { 10, 1 } : new int[] { 1 });
		int chosenAmount = 0;
		for (int i = 0; i < tiers.Length; i++)
		{
			long cost = GetRequiredPotential(me, targetType, tiers[i]);
			if (me.cTiemNang >= cost)
			{
				chosenAmount = tiers[i];
				break;
			}
		}
		if (chosenAmount <= 0) return;
		Service.gI().upPotential(targetType, chosenAmount);
		nextAt = now + intervalMs;
	}
}

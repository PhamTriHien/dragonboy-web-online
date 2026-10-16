using System;

/// <summary>
/// Module Lập Lịch Treo Game: Quản lý thời gian tự động bật và tắt các tác vụ tự động theo mốc giờ trong ngày.
/// </summary>
public static class ModSchedule
{
	public static bool isEnabled;
	public static int startMinute = 0;
	public static int endMinute = 0;
	private static long nextCheck;
	private static bool lastWindow;
	private static bool hasEvaluatedWindow;

	/// <summary>
	/// Cấu hình tham số và thiết lập hoạt động cho module.
	/// </summary>
	public static bool Configure(string value)
	{
		string[] parts = (value ?? string.Empty).Split('-');
		int start, end;
		if (parts.Length != 2 || !TryMinute(parts[0], out start) || !TryMinute(parts[1], out end)) return false;
		ApplyPreset(start, end);
		return true;
	}

	/// <summary>
	/// Thiết lập mốc phút bắt đầu và kết thúc cho lịch tự động và buộc kiểm tra lại ngay lập tức.
	/// </summary>
	public static void ApplyPreset(int start, int end)
	{
		startMinute = start;
		endMinute = end;
		isEnabled = true;
		hasEvaluatedWindow = false;
		nextCheck = 0;
		ModConfig.SaveConfig();
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ TryMinute.
	/// </summary>
	private static bool TryMinute(string text, out int minute)
	{
		minute = 0;
		string[] p = (text ?? string.Empty).Trim().Split(':');
		int hour, min;
		if (p.Length != 2 || !int.TryParse(p[0], out hour) || !int.TryParse(p[1], out min) || hour < 0 || hour > 23 || min < 0 || min > 59) return false;
		minute = hour * 60 + min;
		return true;
	}

	/// <summary>
	/// Bật hoặc tắt trạng thái kích hoạt của Toggle.
	/// </summary>
	public static void Toggle()
	{
		isEnabled = !isEnabled;
		hasEvaluatedWindow = false;
		nextCheck = 0;
		ModConfig.SaveConfig();
		GameScr.info1.addInfo("Lịch Auto: " + (isEnabled ? "BẬT" : "TẮT"), 0);
	}

	/// <summary>
	/// Cập nhật trạng thái logic của đối tượng theo từng chu kỳ khung hình.
	/// </summary>
	public static void Update()
	{
		if (!isEnabled) return;
		long now = mSystem.currentTimeMillis();
		if (now < nextCheck) return;
		nextCheck = now + 1000;
		int current = DateTime.Now.Hour * 60 + DateTime.Now.Minute;
		bool inWindow = startMinute == endMinute ? true : (startMinute < endMinute ? current >= startMinute && current < endMinute : current >= startMinute || current < endMinute);
		if (hasEvaluatedWindow && inWindow == lastWindow) return;
		hasEvaluatedWindow = true;
		lastWindow = inWindow;
		ModTanSat.autoTanSat = inWindow;
		ModAutoPick.autoPick = inWindow;
		ModAutoItem.isEnabled = inWindow && ModAutoItem.itemIds.Count > 0;
		GameScr.info1.addInfo("Lịch Auto: " + (inWindow ? "ĐÃ BẬT" : "ĐÃ TẮT"), 0);
	}
}

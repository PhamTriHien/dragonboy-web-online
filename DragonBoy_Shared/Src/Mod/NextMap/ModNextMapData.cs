using System;
using System.Collections.Generic;

/// <summary>
/// CLASS ModNextMapData: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
public static class ModNextMapData
{
	// Danh sách ID bản đồ theo 3 hành tinh: Trái Đất, Namếc, Xayda
	public static readonly int[][] planetMapIds = new int[3][]
	{
		new int[] { 21, 0, 1, 2, 24, 3, 4, 5, 6, 27, 28, 29, 30, 42, 47, 46, 45, 48 },
		new int[] { 22, 7, 8, 9, 25, 11, 12, 13, 10, 31, 32, 33, 34, 43 },
		new int[] { 23, 14, 15, 16, 26, 17, 18, 20, 19, 35, 36, 37, 38, 44 }
	};

	// Đồ thị kết nối bản đồ
	public static readonly Dictionary<int, List<int>> mapWaypoints = new Dictionary<int, List<int>>
	{
		{ 0, new List<int> { 21, 1, 24, 42 } },
		{ 21, new List<int> { 0 } },
		{ 1, new List<int> { 0, 2, 27 } },
		{ 2, new List<int> { 1, 3 } },
		{ 3, new List<int> { 2, 4 } },
		{ 4, new List<int> { 3, 5 } },
		{ 5, new List<int> { 4, 6 } },
		{ 6, new List<int> { 5 } },
		{ 27, new List<int> { 1, 28 } },
		{ 28, new List<int> { 27, 29 } },
		{ 29, new List<int> { 28, 30 } },
		{ 30, new List<int> { 29 } },
		{ 42, new List<int> { 0, 47 } },
		{ 47, new List<int> { 42, 46 } },
		{ 46, new List<int> { 47, 45 } },
		{ 45, new List<int> { 46, 48 } },
		{ 48, new List<int> { 45 } },
		{ 24, new List<int> { 0, 25, 26 } },

		{ 7, new List<int> { 22, 8, 25, 43 } },
		{ 22, new List<int> { 7 } },
		{ 8, new List<int> { 7, 9, 31 } },
		{ 9, new List<int> { 8, 11, 10 } },
		{ 10, new List<int> { 9 } },
		{ 11, new List<int> { 9, 12 } },
		{ 12, new List<int> { 11, 13 } },
		{ 13, new List<int> { 12 } },
		{ 31, new List<int> { 8, 32 } },
		{ 32, new List<int> { 31, 33 } },
		{ 33, new List<int> { 32, 34 } },
		{ 34, new List<int> { 33 } },
		{ 43, new List<int> { 7 } },
		{ 25, new List<int> { 7, 24, 26 } },

		{ 14, new List<int> { 23, 15, 26, 44 } },
		{ 23, new List<int> { 14 } },
		{ 15, new List<int> { 14, 16, 35 } },
		{ 16, new List<int> { 15, 17 } },
		{ 17, new List<int> { 16, 18 } },
		{ 18, new List<int> { 17, 20 } },
		{ 20, new List<int> { 18, 19 } },
		{ 19, new List<int> { 20 } },
		{ 35, new List<int> { 15, 36 } },
		{ 36, new List<int> { 35, 37 } },
		{ 37, new List<int> { 36, 38 } },
		{ 38, new List<int> { 37 } },
		{ 44, new List<int> { 14 } },
		{ 26, new List<int> { 14, 24, 25 } }
	};

	/// <summary>
	/// Truy xuất và trả về dữ liệu của GetMapName.
	/// </summary>
	public static string GetMapName(int id)
	{
		switch (id)
		{
			case 0: return "Làng Aru";
			case 1: return "Đồi hoa cúc";
			case 2: return "Thung lũng tre";
			case 3: return "Rừng nấm";
			case 4: return "Rừng xương";
			case 5: return "Đảo Kamê";
			case 6: return "Đông Nam Kamê";
			case 7: return "Làng Mori";
			case 8: return "Đồi nấm";
			case 9: return "Thung lũng Maima";
			case 10: return "Thung lũng đá";
			case 11: return "Vực cấm";
			case 12: return "Núi Appule";
			case 13: return "Căn cứ Fide";
			case 14: return "Làng Kakarot";
			case 15: return "Đồi hoang";
			case 16: return "Làng Plant";
			case 17: return "Rừng nguyên sinh";
			case 18: return "Rừng cọ";
			case 19: return "Thành phố Vegeta";
			case 20: return "Vách núi đen";
			case 21: return "Nhà Gôhan";
			case 22: return "Nhà Moori";
			case 23: return "Nhà Broly";
			case 24: return "Trạm tàu T.Đất";
			case 25: return "Trạm tàu Namếc";
			case 26: return "Trạm tàu Xayda";
			case 27: return "Rừng bamboo";
			case 28: return "Rừng dương xỉ";
			case 29: return "Nam Kamê";
			case 30: return "Đảo Kamê 2";
			case 31: return "Núi hoa vàng";
			case 32: return "Núi hoa tím";
			case 33: return "Nam Guru";
			case 34: return "Đền Guru";
			case 35: return "Rừng đá";
			case 36: return "Thung lũng đen";
			case 37: return "Bờ vực đen";
			case 38: return "Căn cứ Fide 2";
			case 42: return "Vách Aru";
			case 43: return "Vách Moori";
			case 44: return "Vách Kakarot";
			case 45: return "Thần điện";
			case 46: return "Tháp Karin";
			case 47: return "Rừng Karin";
			case 48: return "Thánh địa Kaio";
			default: return "Map " + id;
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ RemoveAccents.
	/// </summary>
	public static string RemoveAccents(string text)
	{
		if (string.IsNullOrEmpty(text)) return string.Empty;
		string[] arr1 = new string[] {
			"á","à","ả","ã","ạ","â","ấ","ầ","ẩ","ẫ","ậ","ă","ắ","ằ","ẳ","ẵ","ặ",
			"đ",
			"é","è","ẻ","ẽ","ẹ","ê","ế","ề","ể","ễ","ệ",
			"í","ì","ỉ","ĩ","ị",
			"ó","ò","ỏ","õ","ọ","ô","ố","ồ","ổ","ỗ","ộ","ơ","ớ","ờ","ở","ỡ","ợ",
			"ú","ù","ủ","ũ","ụ","ư","ứ","ừ","ử","ữ","ự",
			"ý","ỳ","ỷ","ỹ","ỵ"
		};
		string[] arr2 = new string[] {
			"a","a","a","a","a","a","a","a","a","a","a","a","a","a","a","a","a",
			"d",
			"e","e","e","e","e","e","e","e","e","e","e",
			"i","i","i","i","i",
			"o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o",
			"u","u","u","u","u","u","u","u","u","u","u",
			"y","y","y","y","y"
		};
		text = text.ToLower().Trim();
		for (int i = 0; i < arr1.Length; i++)
		{
			text = text.Replace(arr1[i], arr2[i]);
		}
		return text;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ CleanName.
	/// </summary>
	public static string CleanName(string s)
	{
		if (string.IsNullOrEmpty(s)) return string.Empty;
		s = RemoveAccents(s);
		string res = string.Empty;
		for (int i = 0; i < s.Length; i++)
		{
			char c = s[i];
			if (char.IsLetterOrDigit(c))
			{
				res += c;
			}
		}
		return res;
	}

	private static readonly Dictionary<int, string[]> s_MapMultilingualAliases = new Dictionary<int, string[]>
	{
		{ 0, new string[] { "Lang Aru", "Desa Aru" } },
		{ 1, new string[] { "Hoa cuc", "Doi hoa cuc", "Krisan", "Chrysanthemum" } },
		{ 2, new string[] { "Thung lung tre", "Lembah bambu" } },
		{ 3, new string[] { "Rung nam", "Hutan jamur", "Mushroom" } },
		{ 4, new string[] { "Rung xuong", "Hutan tulang", "Bone forest" } },
		{ 5, new string[] { "Dao Kame", "Pulau Kame" } },
		{ 6, new string[] { "Dong Nam Kame", "Tenggara Kame" } },
		{ 7, new string[] { "Lang Mori", "Lang Moori", "Desa Mori", "Desa Moori" } },
		{ 8, new string[] { "Doi nam", "Doi nam tim", "Bukit jamur" } },
		{ 9, new string[] { "Maima", "Lembah Maima" } },
		{ 10, new string[] { "Thung lung da", "Lembah batu" } },
		{ 11, new string[] { "Vuc cam", "Jurang terlarang" } },
		{ 12, new string[] { "Appule", "Gunung Appule" } },
		{ 13, new string[] { "Can cu Fide", "Pangkalan Frieza" } },
		{ 14, new string[] { "Lang Kakarot", "Desa Kakarot", "Kakarot village" } },
		{ 15, new string[] { "Doi hoang", "Abadon", "Bukit liar" } },
		{ 16, new string[] { "Lang Plant", "Desa Plant", "Plant village" } },
		{ 17, new string[] { "Nguyen sinh", "Rung nguyen sinh", "Hutan perawan", "Primeval" } },
		{ 18, new string[] { "Rung co", "Rung thong", "Hutan palem" } },
		{ 19, new string[] { "Vegeta", "Kota Vegeta" } },
		{ 20, new string[] { "Vach nui den", "Tebing hitam" } },
		{ 21, new string[] { "Nha Gohan", "Rumah Gohan" } },
		{ 22, new string[] { "Nha Moori", "Nha Mori", "Rumah Moori" } },
		{ 23, new string[] { "Nha Broly", "Rumah Broly" } },
		{ 24, new string[] { "Tram tau", "Tau vu tru", "Stasiun", "Spaceship" } },
		{ 25, new string[] { "Tram tau", "Tau vu tru", "Stasiun Namec", "Spaceship" } },
		{ 26, new string[] { "Tram tau", "Tau vu tru", "Stasiun Saiya", "Spaceship" } },
		{ 27, new string[] { "Rung bamboo", "Rung truc", "Hutan bambu" } },
		{ 28, new string[] { "Duong xi", "Rung duong xi", "Hutan pakis" } },
		{ 29, new string[] { "Nam Kame", "Selatan Kame" } },
		{ 30, new string[] { "Dao Bulong", "Bulong", "Dao Kame 2", "Kame 2" } },
		{ 31, new string[] { "Nui hoa vang", "Bunga kuning" } },
		{ 32, new string[] { "Nui hoa tim", "Bunga ungu" } },
		{ 33, new string[] { "Nam Guru", "Selatan Guru" } },
		{ 34, new string[] { "Den Guru", "Kuil Guru" } },
		{ 35, new string[] { "Rung da", "Hutan batu" } },
		{ 36, new string[] { "Thung lung den", "Lembah hitam" } },
		{ 37, new string[] { "Bo vuc den", "Jurang hitam" } },
		{ 38, new string[] { "Can cu Fide 2", "Fide 2", "Frieza 2" } },
		{ 42, new string[] { "Vach Aru", "Vach nui Aru", "Tebing Aru" } },
		{ 43, new string[] { "Vach Moori", "Vach Mori", "Vach nui Moori", "Vach nui Mori", "Tebing Moori" } },
		{ 44, new string[] { "Vach Kakarot", "Vach nui Kakarot", "Tebing Kakarot" } },
		{ 45, new string[] { "Than dien", "Kuil Dewa", "Kami" } },
		{ 46, new string[] { "Thap Karin", "Menara Karin", "Korin" } },
		{ 47, new string[] { "Rung Karin", "Hutan Karin" } },
		{ 48, new string[] { "Kaio", "Thanh dia Kaio", "Planet Kaio" } }
	};

	/// <summary>
	/// Thực thi phương thức nghiệp vụ MatchMapId.
	/// </summary>
	public static bool MatchMapId(string wpName, int targetMapId)
	{
		if (string.IsNullOrEmpty(wpName)) return false;

		string mName = GetMapName(targetMapId);
		if (MatchMapName(wpName, mName)) return true;

		if (s_MapMultilingualAliases.TryGetValue(targetMapId, out string[] aliases))
		{
			for (int i = 0; i < aliases.Length; i++)
			{
				if (MatchMapName(wpName, aliases[i]))
				{
					return true;
				}
			}
		}

		return false;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ MatchMapName.
	/// </summary>
	public static bool MatchMapName(string wpName, string mapName)
	{
		if (string.IsNullOrEmpty(wpName) || string.IsNullOrEmpty(mapName)) return false;
		string w = CleanName(wpName);
		string m = CleanName(mapName);
		if (string.IsNullOrEmpty(w) || string.IsNullOrEmpty(m)) return false;

		if (w.Equals(m, StringComparison.OrdinalIgnoreCase)) return true;

		// Tránh khớp nhầm giữa bản đồ gốc và bản đồ hậu tố số (ví dụ: Đảo Kamê vs Đảo Kamê 2, Căn cứ Fide vs Căn cứ Fide 2)
		bool wpHas2 = w.Contains("2");
		bool mapHas2 = m.Contains("2");
		if (wpHas2 != mapHas2) return false;

		string normW = RemoveAccents(wpName).ToLower();
		string normM = RemoveAccents(mapName).ToLower();

		// Tránh khớp nhầm giữa "Đông Nam Kamê" và "Nam Kamê"
		if (normW.Contains("dong nam") != normM.Contains("dong nam"))
		{
			return false;
		}

		if (w.Contains(m))
		{
			return true;
		}

		char[] sep = new char[] { ' ', '-', '_', '(', ')', ',', '.', '/', '\\', ':' };
		string[] wWords = normW.Split(sep, StringSplitOptions.RemoveEmptyEntries);
		string[] mWords = normM.Split(sep, StringSplitOptions.RemoveEmptyEntries);
		if (mWords.Length == 0 || wWords.Length == 0) return false;

		int requiredCount = 0;
		int matchedCount = 0;
		for (int i = 0; i < mWords.Length; i++)
		{
			string kw = mWords[i];
			if (kw == "t" || kw == "dat" && normM.Contains("tram tau"))
			{
				continue;
			}
			requiredCount++;
			bool tokenFound = false;
			for (int j = 0; j < wWords.Length; j++)
			{
				if (wWords[j].Equals(kw, StringComparison.OrdinalIgnoreCase))
				{
					tokenFound = true;
					break;
				}
			}
			if (tokenFound)
			{
				matchedCount++;
			}
		}

		return requiredCount > 0 && matchedCount == requiredCount;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ FindMapIdByName.
	/// </summary>
	public static int FindMapIdByName(string name)
	{
		if (string.IsNullOrEmpty(name)) return -1;
		string cleanTarget = CleanName(name);

		// 1. So khớp chính xác theo danh sách ID chuẩn
		for (int id = 0; id <= 48; id++)
		{
			string mName = GetMapName(id);
			if (CleanName(mName).Equals(cleanTarget))
			{
				return id;
			}
		}

		// 2. So khớp với TileMap.mapNames
		if (TileMap.mapNames != null)
		{
			for (int i = 0; i < TileMap.mapNames.Length; i++)
			{
				if (TileMap.mapNames[i] != null && CleanName(TileMap.mapNames[i]).Equals(cleanTarget))
				{
					return i;
				}
			}
		}

		// 3. So khớp theo từ khóa đầy đủ
		for (int id = 0; id <= 48; id++)
		{
			if (MatchMapId(name, id))
			{
				return id;
			}
		}

		if (TileMap.mapNames != null)
		{
			for (int i = 0; i < TileMap.mapNames.Length; i++)
			{
				if (TileMap.mapNames[i] != null && MatchMapName(name, TileMap.mapNames[i]))
				{
					return i;
				}
			}
		}

		return -1;
	}
}

using System;
using UnityEngine;

/// <summary>
/// Thành phần phân mảnh (partial) của lớp Res: Đảm nhiệm phân hệ chuyên trách String ().
/// </summary>
public partial class Res
{
	/// <summary>
	/// Truy xuất và trả về dữ liệu của GetVietnameseOptionTemplate.
	/// </summary>
	public static string GetVietnameseOptionTemplate(int id, string defaultName)
	{
		return ModTranslate.GetOptionTemplate(id, defaultName);
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ changeString.
	/// </summary>
	public static string changeString(string str)
	{
		return ModTranslate.TranslateString(str);
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ replace.
	/// </summary>
	public static string replace(string _text, string _searchStr, string _replacementStr)
	{
		return _text.Replace(_searchStr, _replacementStr);
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ split.
	/// </summary>
	public static string[] split(string original, string separator, int count)
	{
		int num = original.IndexOf(separator);
		string[] array;
		if (num >= 0)
		{
			array = split(original.Substring(num + separator.Length), separator, count + 1);
		}
		else
		{
			array = new string[count + 1];
			num = original.Length;
		}
		array[count] = original.Substring(0, num);
		return array;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ formatNumber.
	/// </summary>
	public static string formatNumber(long number)
	{
		string empty = string.Empty;
		string empty2 = string.Empty;
		empty = string.Empty;
		if (number >= 1000000000)
		{
			empty2 = mResources.billion;
			long num = number % 1000000000 / 100000000;
			number /= 1000000000;
			empty = number + string.Empty;
			if (num > 0)
			{
				string text = empty;
				return text + "," + num + empty2;
			}
			return empty + empty2;
		}
		if (number >= 1000000)
		{
			empty2 = mResources.million;
			long num2 = number % 1000000 / 100000;
			number /= 1000000;
			empty = number + string.Empty;
			if (num2 > 0)
			{
				string text = empty;
				return text + "," + num2 + empty2;
			}
			return empty + empty2;
		}
		return number + string.Empty;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ formatNumber2.
	/// </summary>
	public static string formatNumber2(long number)
	{
		string empty = string.Empty;
		string empty2 = string.Empty;
		empty = string.Empty;
		if (number >= 1000000000)
		{
			empty2 = mResources.billion;
			long num = number % 1000000000 / 10000000;
			number /= 1000000000;
			empty = number + string.Empty;
			if (num >= 10)
			{
				if (num % 10 == 0)
				{
					num /= 10;
				}
				string text = empty;
				return text + "," + num + empty2;
			}
			if (num > 0)
			{
				string text = empty;
				return text + ",0" + num + empty2;
			}
			return empty + empty2;
		}
		if (number >= 1000000)
		{
			empty2 = mResources.million;
			long num2 = number % 1000000 / 10000;
			number /= 1000000;
			empty = number + string.Empty;
			if (num2 >= 10)
			{
				if (num2 % 10 == 0)
				{
					num2 /= 10;
				}
				string text = empty;
				return text + "," + num2 + empty2;
			}
			if (num2 > 0)
			{
				string text = empty;
				return text + ",0" + num2 + empty2;
			}
			return empty + empty2;
		}
		if (number >= 10000)
		{
			empty2 = "k";
			long num3 = number % 1000 / 10;
			number /= 1000;
			empty = number + string.Empty;
			if (num3 >= 10)
			{
				if (num3 % 10 == 0)
				{
					num3 /= 10;
				}
				string text = empty;
				return text + "," + num3 + empty2;
			}
			if (num3 > 0)
			{
				string text = empty;
				return text + ",0" + num3 + empty2;
			}
			return empty + empty2;
		}
		return number + string.Empty;
	}
}

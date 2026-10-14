using System;

/// <summary>
/// CLASS Math: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
public class Math
{
	public const double PI = System.Math.PI;

	/// <summary>
	/// Thực thi phương thức nghiệp vụ abs.
	/// </summary>
	public static int abs(int i)
	{
		return (i <= 0) ? (-i) : i;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ min.
	/// </summary>
	public static int min(int x, int y)
	{
		return (x >= y) ? y : x;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ max.
	/// </summary>
	public static int max(int x, int y)
	{
		return (x <= y) ? y : x;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ pow.
	/// </summary>
	public static int pow(int data, int x)
	{
		int num = 1;
		for (int i = 0; i < x; i++)
		{
			num *= data;
		}
		return num;
	}
}

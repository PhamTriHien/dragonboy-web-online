using System;

/// <summary>
/// CLASS MyRandom: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
public class MyRandom
{
	public Random r;

	/// <summary>
	/// Thực thi phương thức nghiệp vụ MyRandom.
	/// </summary>
	public MyRandom()
	{
		r = new Random();
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ nextInt.
	/// </summary>
	public int nextInt()
	{
		return r.Next();
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ nextInt.
	/// </summary>
	public int nextInt(int a)
	{
		return r.Next(a);
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ nextInt.
	/// </summary>
	public int nextInt(int a, int b)
	{
		return r.Next(a, b);
	}
}

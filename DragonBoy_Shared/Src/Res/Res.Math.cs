using System;
using UnityEngine;

/// <summary>
/// Thành phần phân mảnh (partial) của lớp Res: Đảm nhiệm phân hệ chuyên trách Math ().
/// </summary>
public partial class Res
{
	/// <summary>
	/// Thực thi phương thức nghiệp vụ sin.
	/// </summary>
	public static int sin(int a)
		{
			a = fixangle(a);
			if (a >= 0 && a < 90)
			{
				return sinz[a];
			}
			if (a >= 90 && a < 180)
			{
				return sinz[180 - a];
			}
			if (a >= 180 && a < 270)
			{
				return -sinz[a - 180];
			}
			return -sinz[360 - a];
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ cos.
	/// </summary>
	public static int cos(int a)
		{
			a = fixangle(a);
			if (a >= 0 && a < 90)
			{
				return cosz[a];
			}
			if (a >= 90 && a < 180)
			{
				return -cosz[180 - a];
			}
			if (a >= 180 && a < 270)
			{
				return -cosz[a - 180];
			}
			return cosz[360 - a];
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ tan.
	/// </summary>
	public static int tan(int a)
		{
			a = fixangle(a);
			if (a >= 0 && a < 90)
			{
				return tanz[a];
			}
			if (a >= 90 && a < 180)
			{
				return -tanz[180 - a];
			}
			if (a >= 180 && a < 270)
			{
				return tanz[a - 180];
			}
			return -tanz[360 - a];
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ atan.
	/// </summary>
	public static int atan(int a)
		{
			for (int i = 0; i <= 90; i++)
			{
				if (tanz[i] >= a)
				{
					return i;
				}
			}
			return 0;
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ angle.
	/// </summary>
	public static int angle(int dx, int dy)
		{
			int num;
			if (dx != 0)
			{
				int a = Math.abs((dy << 10) / dx);
				num = atan(a);
				if (dy >= 0 && dx < 0)
				{
					num = 180 - num;
				}
				if (dy < 0 && dx < 0)
				{
					num = 180 + num;
				}
				if (dy < 0 && dx >= 0)
				{
					num = 360 - num;
				}
			}
			else
			{
				num = ((dy <= 0) ? 270 : 90);
			}
			return num;
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ fixangle.
	/// </summary>
	public static int fixangle(int angle)
		{
			if (angle >= 360)
			{
				angle -= 360;
			}
			if (angle < 0)
			{
				angle += 360;
			}
			return angle;
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ xetVX.
	/// </summary>
	public static int xetVX(int goc, int d)
		{
			return cos(fixangle(goc)) * d >> 10;
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ xetVY.
	/// </summary>
	public static int xetVY(int goc, int d)
		{
			return sin(fixangle(goc)) * d >> 10;
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ random.
	/// </summary>
	public static int random(int a, int b)
		{
			if (a == b)
			{
				return a;
			}
			return a + r.nextInt(b - a);
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ random.
	/// </summary>
	public static int random(int a)
		{
			return r.nextInt(a);
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ random_Am.
	/// </summary>
	public static int random_Am(int a, int b)
		{
			int num = a + r.nextInt(b - a);
			if (random(2) == 0)
			{
				num = -num;
			}
			return num;
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ random_Am_0.
	/// </summary>
	public static int random_Am_0(int a)
		{
			int num;
			for (num = 0; num == 0; num = r.nextInt() % a)
			{
			}
			return num;
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ s2tick.
	/// </summary>
	public static int s2tick(int currentTimeMillis)
		{
			int num = 0;
			num = currentTimeMillis * 16 / 1000;
			if (currentTimeMillis * 16 % 1000 >= 5)
			{
				num++;
			}
			return num;
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ distance.
	/// </summary>
	public static int distance(int x1, int y1, int x2, int y2)
		{
			return sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
		}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của getDistance.
	/// </summary>
	public static int getDistance(int x, int y)
		{
			return sqrt(x * x + y * y);
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ sqrt.
	/// </summary>
	public static int sqrt(int a)
		{
			if (a <= 0)
			{
				return 0;
			}
			int num = (a + 1) / 2;
			int num2;
			do
			{
				num2 = num;
				num = num / 2 + a / (2 * num);
			}
			while (Math.abs(num2 - num) > 1);
			return num;
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ rnd.
	/// </summary>
	public static int rnd(int a)
		{
			return r.nextInt(a);
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ abs.
	/// </summary>
	public static int abs(int i)
		{
			return (i <= 0) ? (-i) : i;
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ inRect.
	/// </summary>
	public static bool inRect(int x1, int y1, int width, int height, int x2, int y2)
		{
			return x2 >= x1 && x2 <= x1 + width && y2 >= y1 && y2 <= y1 + height;
		}

}

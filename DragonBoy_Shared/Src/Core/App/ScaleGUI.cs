using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Hệ thống quản lý tỉ lệ co giãn giao diện: Chuẩn hóa kích thước tọa độ hiển thị trên các độ phân giải màn hình khác nhau.
/// </summary>
public class ScaleGUI
{
	public static bool scaleScreen;

	public static float WIDTH;

	public static float HEIGHT;

	private static List<Matrix4x4> stack = new List<Matrix4x4>();

	/// <summary>
	/// Khởi tạo và nạp dữ liệu cấu hình cho initScaleGUI.
	/// </summary>
	public static void initScaleGUI()
	{
		Cout.println("Init Scale GUI: Screen.w=" + Screen.width + " Screen.h=" + Screen.height);
		WIDTH = Screen.width;
		HEIGHT = Screen.height;
		scaleScreen = false;
		if (Screen.width <= 1200)
		{
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ BeginGUI.
	/// </summary>
	public static void BeginGUI()
	{
		if (scaleScreen)
		{
			stack.Add(GUI.matrix);
			Matrix4x4 matrix4x = default(Matrix4x4);
			float num = Screen.width;
			float num2 = Screen.height;
			float num3 = num / num2;
			float num4 = 1f;
			Vector3 zero = Vector3.zero;
			num4 = ((!(num3 < WIDTH / HEIGHT)) ? ((float)Screen.height / HEIGHT) : ((float)Screen.width / WIDTH));
			matrix4x.SetTRS(zero, Quaternion.identity, Vector3.one * num4);
			GUI.matrix *= matrix4x;
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ EndGUI.
	/// </summary>
	public static void EndGUI()
	{
		if (scaleScreen)
		{
			GUI.matrix = stack[stack.Count - 1];
			stack.RemoveAt(stack.Count - 1);
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ scaleX.
	/// </summary>
	public static float scaleX(float x)
	{
		if (!scaleScreen)
		{
			return x;
		}
		x = x * WIDTH / (float)Screen.width;
		return x;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ scaleY.
	/// </summary>
	public static float scaleY(float y)
	{
		if (!scaleScreen)
		{
			return y;
		}
		y = y * HEIGHT / (float)Screen.height;
		return y;
	}
}

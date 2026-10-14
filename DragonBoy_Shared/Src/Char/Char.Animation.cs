using System;
using Assets.src.e;
using Assets.src.g;
using UnityEngine;

/// <summary>
/// Thành phần phân mảnh (partial) của lớp Char: Đảm nhiệm phân hệ chuyên trách Animation ().
/// </summary>
public partial class Char : IMapObject
{
	/// <summary>
	/// Thực thi phương thức nghiệp vụ createShadow.
	/// </summary>
	public void createShadow(int x, int y, int life)
			{
				shadowX = x;
				shadowY = y;
				shadowLife = life;
			}

}

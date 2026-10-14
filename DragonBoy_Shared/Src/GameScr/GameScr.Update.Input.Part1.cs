using System;
using Assets.src.g;
/// <summary>
/// Thành phần phân mảnh (partial) của lớp GameScr: Đảm nhiệm vòng lặp cập nhật trạng thái logic theo từng khung hình (Input).
/// </summary>
public partial class GameScr : mScreen, IChatable
{
	/// <summary>
	/// Thực thi phương thức nghiệp vụ keyPress.
	/// </summary>
	public override void keyPress(int keyCode)
			{
				base.keyPress(keyCode);
			}

}

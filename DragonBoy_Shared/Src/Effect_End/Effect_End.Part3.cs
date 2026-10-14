using System;
/// <summary>
/// Thành phần phân mảnh (partial) của lớp Effect_End: Đảm nhiệm phân hệ chuyên trách Part3 ().
/// </summary>
public partial class Effect_End
{
	/// <summary>
	/// Thực thi phương thức nghiệp vụ pnt_Pow.
	/// </summary>
	private void pnt_Pow(mGraphics g, int anchor)
			{
				if (fra_skill[0] != null)
				{
					if (nFrame != null)
					{
						fra_skill[0].drawFrame(nFrame[f % nFrame.Length], x, y, dir_nguoc, anchor, g);
					}
					else
					{
						fra_skill[0].drawFrame(f / n_frame % fra_skill[0].nFrame, x, y, dir_nguoc, anchor, g);
					}
				}
			}
	/// <summary>
	/// Thực thi phương thức nghiệp vụ upd_Sub.
	/// </summary>
	private void upd_Sub()
			{
				if (timeRemove > 0)
				{
					if (GameCanvas.timeNow - time >= timeRemove)
					{
						removeEff();
					}
				}
				else if (f >= fra_skill[0].nFrame * n_frame)
				{
					removeEff();
				}
			}
	/// <summary>
	/// Thực thi phương thức nghiệp vụ pnt_Sub.
	/// </summary>
	private void pnt_Sub(mGraphics g, int anchor)
			{
				fra_skill[0].drawFrame(f / n_frame % fra_skill[0].nFrame, x, y, dir, anchor, g);
			}
	/// <summary>
	/// Thực thi phương thức nghiệp vụ upd_.
	/// </summary>
	private void upd_()
			{
			}
	/// <summary>
	/// Thực thi phương thức nghiệp vụ pnt_.
	/// </summary>
	private void pnt_(mGraphics g)
			{
			}

}

/// <summary>
/// CLASS EffectPaint: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
public class EffectPaint
{
	public int index;

	public Mob eMob;

	public Char eChar;

	public EffectCharPaint effCharPaint;

	public bool isFly;

	/// <summary>
	/// Truy xuất và trả về dữ liệu của getImgId.
	/// </summary>
	public int getImgId()
	{
		return effCharPaint.arrEfInfo[index].idImg;
	}
}

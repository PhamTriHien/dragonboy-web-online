/// <summary>
/// CLASS Effect2: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
public abstract class Effect2
{
	public static MyVector vEffect3 = new MyVector();

	public static MyVector vEffect2 = new MyVector();

	public static MyVector vRemoveEffect2 = new MyVector();

	public static MyVector vEffect2Outside = new MyVector();

	public static MyVector vAnimateEffect = new MyVector();

	public static MyVector vEffectFeet = new MyVector();

	/// <summary>
	/// Cập nhật trạng thái logic của đối tượng theo từng chu kỳ khung hình.
	/// </summary>
	public virtual void update()
	{
	}

	/// <summary>
	/// Thực hiện vẽ toàn bộ đối tượng và các lớp hiển thị lên canvas mGraphics.
	/// </summary>
	public virtual void paint(mGraphics g)
	{
	}
}

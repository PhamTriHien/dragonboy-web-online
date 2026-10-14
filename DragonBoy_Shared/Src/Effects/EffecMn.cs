/// <summary>
/// CLASS EffecMn: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
public class EffecMn
{
	public static MyVector vEff = new MyVector();

	/// <summary>
	/// Thực thi phương thức nghiệp vụ addEff.
	/// </summary>
	public static void addEff(Effect me)
	{
		vEff.addElement(me);
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ removeEff.
	/// </summary>
	public static void removeEff(int id)
	{
		if (getEffById(id) != null)
		{
			vEff.removeElement(getEffById(id));
		}
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của getEffById.
	/// </summary>
	public static Effect getEffById(int id)
	{
		for (int i = 0; i < vEff.size(); i++)
		{
			Effect effect = (Effect)vEff.elementAt(i);
			if (effect.effId == id)
			{
				return effect;
			}
		}
		return null;
	}

	/// <summary>
	/// Thực hiện vẽ và hiển thị thành phần paintBackGroundUnderLayer lên canvas đồ họa.
	/// </summary>
	public static void paintBackGroundUnderLayer(mGraphics g, int x, int y, int layer)
	{
		for (int i = 0; i < vEff.size(); i++)
		{
			if (((Effect)vEff.elementAt(i)).layer == -layer)
			{
				((Effect)vEff.elementAt(i)).paintUnderBackground(g, x, y);
			}
		}
	}

	/// <summary>
	/// Thực hiện vẽ và hiển thị thành phần paintLayer1 lên canvas đồ họa.
	/// </summary>
	public static void paintLayer1(mGraphics g)
	{
		for (int i = 0; i < vEff.size(); i++)
		{
			if (((Effect)vEff.elementAt(i)).layer == 1)
			{
				((Effect)vEff.elementAt(i)).paint(g);
			}
		}
	}

	/// <summary>
	/// Thực hiện vẽ và hiển thị thành phần paintLayer2 lên canvas đồ họa.
	/// </summary>
	public static void paintLayer2(mGraphics g)
	{
		for (int i = 0; i < vEff.size(); i++)
		{
			if (((Effect)vEff.elementAt(i)).layer == 2)
			{
				((Effect)vEff.elementAt(i)).paint(g);
			}
		}
	}

	/// <summary>
	/// Thực hiện vẽ và hiển thị thành phần paintLayer3 lên canvas đồ họa.
	/// </summary>
	public static void paintLayer3(mGraphics g)
	{
		for (int i = 0; i < vEff.size(); i++)
		{
			if (((Effect)vEff.elementAt(i)).layer == 3)
			{
				((Effect)vEff.elementAt(i)).paint(g);
			}
		}
	}

	/// <summary>
	/// Thực hiện vẽ và hiển thị thành phần paintLayer4 lên canvas đồ họa.
	/// </summary>
	public static void paintLayer4(mGraphics g)
	{
		for (int i = 0; i < vEff.size(); i++)
		{
			if (((Effect)vEff.elementAt(i)).layer == 4)
			{
				((Effect)vEff.elementAt(i)).paint(g);
			}
		}
	}

	/// <summary>
	/// Cập nhật trạng thái logic của đối tượng theo từng chu kỳ khung hình.
	/// </summary>
	public static void update()
	{
		for (int i = 0; i < vEff.size(); i++)
		{
			((Effect)vEff.elementAt(i)).update();
		}
	}
}

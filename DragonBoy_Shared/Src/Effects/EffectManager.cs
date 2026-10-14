/// <summary>
/// CLASS EffectManager: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
public class EffectManager : MyVector
{
	public static EffectManager lowEffects = new EffectManager();

	public static EffectManager mid_2Effects = new EffectManager();

	public static EffectManager midEffects = new EffectManager();

	public static EffectManager hiEffects = new EffectManager();

	/// <summary>
	/// Cập nhật trạng thái logic của updateAll theo chu kỳ khung hình.
	/// </summary>
	public void updateAll()
	{
		for (int num = size() - 1; num >= 0; num--)
		{
			Effect_End effect_End = (Effect_End)elementAt(num);
			if (effect_End != null)
			{
				effect_End.update();
				if (effect_End.isRemove)
				{
					removeElementAt(num);
				}
			}
		}
	}

	/// <summary>
	/// Cập nhật trạng thái logic của đối tượng theo từng chu kỳ khung hình.
	/// </summary>
	public static void update()
	{
		hiEffects.updateAll();
		mid_2Effects.updateAll();
		midEffects.updateAll();
		lowEffects.updateAll();
	}

	/// <summary>
	/// Thực hiện vẽ và hiển thị thành phần paintAll lên canvas đồ họa.
	/// </summary>
	public void paintAll(mGraphics g)
	{
		for (int i = 0; i < size(); i++)
		{
			Effect_End effect_End = (Effect_End)elementAt(i);
			if (effect_End != null && !effect_End.isRemove)
			{
				((Effect_End)elementAt(i)).paint(g);
			}
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ removeAll.
	/// </summary>
	public void removeAll()
	{
		for (int num = size() - 1; num >= 0; num--)
		{
			Effect_End effect_End = (Effect_End)elementAt(num);
			if (effect_End != null)
			{
				effect_End.isRemove = true;
				removeElementAt(num);
			}
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ remove.
	/// </summary>
	public static void remove()
	{
		hiEffects.removeAll();
		lowEffects.removeAll();
		midEffects.removeAll();
		mid_2Effects.removeAll();
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ addHiEffect.
	/// </summary>
	public static void addHiEffect(Effect_End eff)
	{
		hiEffects.addElement(eff);
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ addMidEffects.
	/// </summary>
	public static void addMidEffects(Effect_End eff)
	{
		midEffects.addElement(eff);
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ addMid_2Effects.
	/// </summary>
	public static void addMid_2Effects(Effect_End eff)
	{
		mid_2Effects.addElement(eff);
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ addLowEffect.
	/// </summary>
	public static void addLowEffect(Effect_End eff)
	{
		lowEffects.addElement(eff);
	}
}

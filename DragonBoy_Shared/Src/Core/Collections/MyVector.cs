using System.Collections;

/// <summary>
/// CLASS MyVector: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
public class MyVector
{
	private ArrayList a;

	/// <summary>
	/// Thực thi phương thức nghiệp vụ MyVector.
	/// </summary>
	public MyVector()
	{
		a = new ArrayList();
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ MyVector.
	/// </summary>
	public MyVector(string s)
	{
		a = new ArrayList();
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ MyVector.
	/// </summary>
	public MyVector(ArrayList a)
	{
		this.a = a;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ addElement.
	/// </summary>
	public void addElement(object o)
	{
		a.Add(o);
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ contains.
	/// </summary>
	public bool contains(object o)
	{
		if (a.Contains(o))
		{
			return true;
		}
		return false;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ size.
	/// </summary>
	public int size()
	{
		if (a == null)
		{
			return 0;
		}
		return a.Count;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ elementAt.
	/// </summary>
	public object elementAt(int index)
	{
		if (index > -1 && index < a.Count)
		{
			return a[index];
		}
		return null;
	}

	/// <summary>
	/// Thiết lập giá trị hoặc trạng thái cho set.
	/// </summary>
	public void set(int index, object obj)
	{
		if (index > -1 && index < a.Count)
		{
			a[index] = obj;
		}
	}

	/// <summary>
	/// Thiết lập giá trị hoặc trạng thái cho setElementAt.
	/// </summary>
	public void setElementAt(object obj, int index)
	{
		if (index > -1 && index < a.Count)
		{
			a[index] = obj;
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ indexOf.
	/// </summary>
	public int indexOf(object o)
	{
		return a.IndexOf(o);
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ removeElementAt.
	/// </summary>
	public void removeElementAt(int index)
	{
		if (index > -1 && index < a.Count)
		{
			a.RemoveAt(index);
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ removeElement.
	/// </summary>
	public void removeElement(object o)
	{
		a.Remove(o);
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ removeAllElements.
	/// </summary>
	public void removeAllElements()
	{
		a.Clear();
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ insertElementAt.
	/// </summary>
	public void insertElementAt(object o, int i)
	{
		a.Insert(i, o);
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ firstElement.
	/// </summary>
	public object firstElement()
	{
		return a[0];
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ lastElement.
	/// </summary>
	public object lastElement()
	{
		return a[a.Count - 1];
	}
}

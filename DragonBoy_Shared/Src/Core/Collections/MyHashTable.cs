using System.Collections;

/// <summary>
/// CLASS MyHashTable: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
public class MyHashTable
{
	public Hashtable h = new Hashtable();

	/// <summary>
	/// Truy xuất và trả về dữ liệu của get.
	/// </summary>
	public object get(object k)
	{
		return h[k];
	}

	/// <summary>
	/// Dọn dẹp và đặt lại trạng thái ban đầu cho clear.
	/// </summary>
	public void clear()
	{
		h.Clear();
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của GetEnumerator.
	/// </summary>
	public IDictionaryEnumerator GetEnumerator()
	{
		return h.GetEnumerator();
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ size.
	/// </summary>
	public int size()
	{
		return h.Count;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ put.
	/// </summary>
	public void put(object k, object v)
	{
		if (h.ContainsKey(k))
		{
			h.Remove(k);
		}
		h.Add(k, v);
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ remove.
	/// </summary>
	public void remove(object k)
	{
		h.Remove(k);
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ Remove.
	/// </summary>
	public void Remove(string key)
	{
		h.Remove(key);
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ containsKey.
	/// </summary>
	public bool containsKey(object key)
	{
		return h.ContainsKey(key);
	}
}

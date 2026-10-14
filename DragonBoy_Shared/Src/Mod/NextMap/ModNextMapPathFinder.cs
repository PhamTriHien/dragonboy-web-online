using System;
using System.Collections.Generic;

/// <summary>
/// CLASS ModNextMapPathFinder: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
public static class ModNextMapPathFinder
{
	private static readonly Dictionary<long, List<int>> s_PathCache = new Dictionary<long, List<int>>();

	/// <summary>
	/// Thực thi phương thức nghiệp vụ FindPath.
	/// </summary>
	public static List<int> FindPath(int startMapId, int targetMapId)
	{
		if (startMapId == targetMapId)
		{
			return new List<int>();
		}

		// Tuyến đường phụ thuộc giới tính vì các map nhà riêng 21/22/23 bị lọc.
		Char meForKey = Char.myCharz();
		int gender = meForKey != null ? meForKey.cgender : -1;
		long cacheKey = ((long)startMapId << 40) | ((long)targetMapId << 8) | (byte)(gender + 1);
		if (s_PathCache.TryGetValue(cacheKey, out var cachedPath))
		{
			return new List<int>(cachedPath);
		}

		Queue<int> queue = new Queue<int>();
		Dictionary<int, int> parent = new Dictionary<int, int>();
		HashSet<int> visited = new HashSet<int>();

		queue.Enqueue(startMapId);
		visited.Add(startMapId);

		bool found = false;
		while (queue.Count > 0)
		{
			int curr = queue.Dequeue();
			if (curr == targetMapId)
			{
				found = true;
				break;
			}

			if (ModNextMapData.mapWaypoints.ContainsKey(curr))
			{
				List<int> neighbors = ModNextMapData.mapWaypoints[curr];
				Char meChar = Char.myCharz();
				int myGender = (meChar != null) ? meChar.cgender : -1;
				for (int i = 0; i < neighbors.Count; i++)
				{
					int next = neighbors[i];
					// Không định tuyến qua nhà đẻ của tộc khác (21: Trái Đất, 22: Namếc, 23: Xayda)
					if (next == 21 && myGender != 0 && targetMapId != 21) continue;
					if (next == 22 && myGender != 1 && targetMapId != 22) continue;
					if (next == 23 && myGender != 2 && targetMapId != 23) continue;

					if (!visited.Contains(next))
					{
						visited.Add(next);
						parent[next] = curr;
						queue.Enqueue(next);
					}
				}
			}
		}

		if (!found)
		{
			return null;
		}

		List<int> path = new List<int>();
		int currNode = targetMapId;
		int maxSteps = 100;
		while (currNode != startMapId && maxSteps-- > 0 && parent.ContainsKey(currNode))
		{
			path.Insert(0, currNode);
			currNode = parent[currNode];
		}

		if (path != null && s_PathCache.Count < 500)
		{
			s_PathCache[cacheKey] = new List<int>(path);
		}

		return path;
	}
}

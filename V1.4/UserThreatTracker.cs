using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000E12 RID: 3602
public static class UserThreatTracker
{
	// Token: 0x17001959 RID: 6489
	// (get) Token: 0x06006A88 RID: 27272 RVA: 0x000877AB File Offset: 0x000859AB
	public static IReadOnlyDictionary<string, float> CurrentThreat
	{
		get
		{
			return UserThreatTracker._threatValues;
		}
	}

	// Token: 0x06006A89 RID: 27273 RVA: 0x0021BF48 File Offset: 0x0021A148
	public static void RecordThreat(string sourceName, float threat)
	{
		if (string.IsNullOrEmpty(sourceName) || threat <= 0f)
		{
			return;
		}
		object @lock = UserThreatTracker._lock;
		lock (@lock)
		{
			float num;
			if (UserThreatTracker._threatValues.TryGetValue(sourceName, out num))
			{
				UserThreatTracker._threatValues[sourceName] = num + threat;
			}
			else
			{
				UserThreatTracker._threatValues[sourceName] = threat;
			}
		}
		Debug.Log(string.Format("[ThreatTracker] Recorded {0} threat for: {1}. Total: {2}", threat, sourceName, UserThreatTracker._threatValues[sourceName]));
	}

	// Token: 0x06006A8A RID: 27274 RVA: 0x0021BFE4 File Offset: 0x0021A1E4
	public static void Reset()
	{
		object @lock = UserThreatTracker._lock;
		lock (@lock)
		{
			UserThreatTracker._threatValues.Clear();
		}
		Debug.Log("[ThreatTracker] All threat values reset.");
	}

	// Token: 0x04005CE6 RID: 23782
	private static readonly Dictionary<string, float> _threatValues = new Dictionary<string, float>();

	// Token: 0x04005CE7 RID: 23783
	private static readonly object _lock = new object();
}

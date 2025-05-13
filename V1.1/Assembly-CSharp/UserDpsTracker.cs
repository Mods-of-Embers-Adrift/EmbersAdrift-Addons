using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000E08 RID: 3592
public class UserDpsTracker : MonoBehaviour
{
	// Token: 0x17001951 RID: 6481
	// (get) Token: 0x06006A60 RID: 27232
	public static Dictionary<string, float> CurrentDps
	{
		get
		{
			UserDpsTracker instance = UserDpsTracker._instance;
			if (instance == null)
			{
				return null;
			}
			return instance._GetDpsSnapshot();
		}
	}

	// Token: 0x06006A61 RID: 27233
	public static void Launch()
	{
		if (UserDpsTracker._instance != null)
		{
			return;
		}
		GameObject gameObject = new GameObject("UserDpsTracker");
		UserDpsTracker._instance = gameObject.AddComponent<UserDpsTracker>();
		UnityEngine.Object.DontDestroyOnLoad(gameObject);
	}

	// Token: 0x06006A62 RID: 27234
	public static void RecordDamage(string source, float amount)
	{
		if (UserDpsTracker._instance == null)
		{
			return;
		}
		UserDpsTracker.DpsEntry entry;
		if (!UserDpsTracker._instance._dpsDict.TryGetValue(source, out entry))
		{
			entry = new UserDpsTracker.DpsEntry
			{
				StartTime = Time.time
			};
			UserDpsTracker._instance._dpsDict[source] = entry;
		}
		entry.TotalDamage += amount;
	}

	// Token: 0x06006A63 RID: 27235
	private Dictionary<string, float> _GetDpsSnapshot()
	{
		Dictionary<string, float> snapshot = new Dictionary<string, float>();
		foreach (KeyValuePair<string, UserDpsTracker.DpsEntry> kvp in this._dpsDict)
		{
			snapshot[kvp.Key] = kvp.Value.DPS;
		}
		return snapshot;
	}

	// Token: 0x06006A64 RID: 27236
	public static void Reset()
	{
		UserDpsTracker instance = UserDpsTracker._instance;
		if (instance == null)
		{
			return;
		}
		instance._dpsDict.Clear();
	}

	// Token: 0x17001958 RID: 6488
	// (get) Token: 0x06006ABD RID: 27325
	public static IReadOnlyDictionary<string, UserDpsTracker.DpsEntry> Entries
	{
		get
		{
			UserDpsTracker instance = UserDpsTracker._instance;
			if (instance == null)
			{
				return null;
			}
			return instance._dpsDict;
		}
	}

	// Token: 0x04005CB7 RID: 23735
	private Dictionary<string, UserDpsTracker.DpsEntry> _dpsDict = new Dictionary<string, UserDpsTracker.DpsEntry>();

	// Token: 0x04005CB8 RID: 23736
	private static UserDpsTracker _instance;

	// Token: 0x02000E09 RID: 3593
	public class DpsEntry
	{
		// Token: 0x17001952 RID: 6482
		// (get) Token: 0x06006A66 RID: 27238
		public float DPS
		{
			get
			{
				if (this.Duration <= 0f)
				{
					return 0f;
				}
				return this.TotalDamage / this.Duration;
			}
		}

		// Token: 0x17001956 RID: 6486
		// (get) Token: 0x06006ABA RID: 27322
		public float Duration
		{
			get
			{
				return Mathf.Max(Time.time - this.StartTime, 0.01f);
			}
		}

		// Token: 0x04005CB9 RID: 23737
		public float TotalDamage;

		// Token: 0x04005CBA RID: 23738
		public float StartTime;
	}
}

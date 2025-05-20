using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000E08 RID: 3592
public class UserDpsTracker : MonoBehaviour
{
	// Token: 0x17001951 RID: 6481
	// (get) Token: 0x06006A60 RID: 27232 RVA: 0x00087582 File Offset: 0x00085782
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

	// Token: 0x06006A61 RID: 27233 RVA: 0x00087594 File Offset: 0x00085794
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

	// Token: 0x06006A62 RID: 27234 RVA: 0x0021AD44 File Offset: 0x00218F44
	public static void RecordDamage(string source, float amount)
	{
		if (UserDpsTracker._instance == null)
		{
			return;
		}
		UserDpsTracker.DpsEntry dpsEntry;
		if (!UserDpsTracker._instance._dpsDict.TryGetValue(source, out dpsEntry))
		{
			dpsEntry = new UserDpsTracker.DpsEntry
			{
				StartTime = Time.time
			};
			UserDpsTracker._instance._dpsDict[source] = dpsEntry;
		}
		dpsEntry.TotalDamage += amount;
	}

	// Token: 0x06006A63 RID: 27235 RVA: 0x0021ADA4 File Offset: 0x00218FA4
	private Dictionary<string, float> _GetDpsSnapshot()
	{
		Dictionary<string, float> dictionary = new Dictionary<string, float>();
		foreach (KeyValuePair<string, UserDpsTracker.DpsEntry> keyValuePair in this._dpsDict)
		{
			dictionary[keyValuePair.Key] = keyValuePair.Value.DPS;
		}
		return dictionary;
	}

	// Token: 0x06006A64 RID: 27236 RVA: 0x000875BE File Offset: 0x000857BE
	public static void Reset()
	{
		UserDpsTracker instance = UserDpsTracker._instance;
		if (instance == null)
		{
			return;
		}
		instance._dpsDict.Clear();
	}

	// Token: 0x04005CB7 RID: 23735
	private Dictionary<string, UserDpsTracker.DpsEntry> _dpsDict = new Dictionary<string, UserDpsTracker.DpsEntry>();

	// Token: 0x04005CB8 RID: 23736
	private static UserDpsTracker _instance;

	// Token: 0x02000E09 RID: 3593
	private class DpsEntry
	{
		// Token: 0x17001952 RID: 6482
		// (get) Token: 0x06006A66 RID: 27238 RVA: 0x000875E7 File Offset: 0x000857E7
		public float DPS
		{
			get
			{
				if (Time.time - this.StartTime <= 0f)
				{
					return 0f;
				}
				return this.TotalDamage / (Time.time - this.StartTime);
			}
		}

		// Token: 0x04005CB9 RID: 23737
		public float TotalDamage;

		// Token: 0x04005CBA RID: 23738
		public float StartTime;
	}
}

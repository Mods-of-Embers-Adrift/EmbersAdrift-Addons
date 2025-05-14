using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000E09 RID: 3593
public class UserDpsTracker : MonoBehaviour
{
	// Token: 0x17001951 RID: 6481
	// (get) Token: 0x06006A67 RID: 27239
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

	// Token: 0x06006A68 RID: 27240
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

	// Token: 0x06006A69 RID: 27241
	public static void RecordDamage(string source, float amount)
	{
		if (UserDpsTracker._instance == null)
		{
			return;
		}
		UserDpsTracker._instance._lastDamageTime = Time.time;
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

	// Token: 0x06006A6A RID: 27242
	private Dictionary<string, float> _GetDpsSnapshot()
	{
		Dictionary<string, float> snap = new Dictionary<string, float>();
		foreach (KeyValuePair<string, UserDpsTracker.DpsEntry> kvp in this._dpsDict)
		{
			snap[kvp.Key] = kvp.Value.DPS;
		}
		return snap;
	}

	// Token: 0x06006A6B RID: 27243
	public static void Reset()
	{
		if (UserDpsTracker._instance == null)
		{
			return;
		}
		UserDpsTracker._instance._dpsDict.Clear();
		UserDpsTracker._instance._pastFights.Clear();
	}

	// Token: 0x17001952 RID: 6482
	// (get) Token: 0x06006A6D RID: 27245
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

	// Token: 0x17001957 RID: 6487
	// (get) Token: 0x06006A75 RID: 27253
	public static IReadOnlyList<UserDpsTracker.FightSession> PastFights
	{
		get
		{
			UserDpsTracker instance = UserDpsTracker._instance;
			if (instance == null)
			{
				return null;
			}
			return instance._pastFights;
		}
	}

	// Token: 0x06006A78 RID: 27256
	private void Awake()
	{
		UserDpsTracker._instance = this;
	}

	// Token: 0x06006A7A RID: 27258
	private void Update()
	{
		if (this._dpsDict.Count > 0 && Time.time - this._lastDamageTime > this._combatTimeout)
		{
			this.SaveSession();
			this._dpsDict.Clear();
		}
	}

	// Token: 0x06006A7C RID: 27260
	private void SaveSession()
	{
		UserDpsTracker.FightSession session = new UserDpsTracker.FightSession
		{
			StartTimestamp = DateTime.Now,
			EndTime = Time.time
		};
		foreach (KeyValuePair<string, UserDpsTracker.DpsEntry> pair in this._dpsDict)
		{
			UserDpsTracker.DpsEntry copy = pair.Value.CloneForSession();
			session.Entries[pair.Key] = copy;
		}
		this._pastFights.Insert(0, session);
		if (this._pastFights.Count > 20)
		{
			this._pastFights.RemoveAt(this._pastFights.Count - 1);
		}
	}

	// Token: 0x04005CC0 RID: 23744
	private Dictionary<string, UserDpsTracker.DpsEntry> _dpsDict = new Dictionary<string, UserDpsTracker.DpsEntry>();

	// Token: 0x04005CC1 RID: 23745
	private static UserDpsTracker _instance;

	// Token: 0x04005CC7 RID: 23751
	private List<UserDpsTracker.FightSession> _pastFights = new List<UserDpsTracker.FightSession>();

	// Token: 0x04005CC8 RID: 23752
	private float _lastDamageTime;

	// Token: 0x04005CC9 RID: 23753
	private float _combatTimeout = 8f;

	// Token: 0x02000E0A RID: 3594
	public class DpsEntry
	{
		// Token: 0x17001953 RID: 6483
		// (get) Token: 0x06006A6E RID: 27246
		public float DPS
		{
			get
			{
				return this.TotalDamage / this.Duration;
			}
		}

		// Token: 0x17001954 RID: 6484
		// (get) Token: 0x06006A70 RID: 27248
		public float Duration
		{
			get
			{
				return Mathf.Max(((this.EndTime > 0f) ? this.EndTime : Time.time) - this.StartTime, 0.01f);
			}
		}

		// Token: 0x06006A82 RID: 27266
		public UserDpsTracker.DpsEntry CloneForSession()
		{
			return new UserDpsTracker.DpsEntry
			{
				TotalDamage = this.TotalDamage,
				StartTime = this.StartTime,
				EndTime = Time.time
			};
		}

		// Token: 0x04005CC2 RID: 23746
		public float TotalDamage;

		// Token: 0x04005CC3 RID: 23747
		public float StartTime;

		// Token: 0x04005CCD RID: 23757
		public float EndTime;
	}

	// Token: 0x02000E0B RID: 3595
	public class FightSession
	{
		// Token: 0x17001955 RID: 6485
		// (get) Token: 0x06006A71 RID: 27249
		public float Duration
		{
			get
			{
				float minStart = float.MaxValue;
				foreach (UserDpsTracker.DpsEntry e in this.Entries.Values)
				{
					minStart = Mathf.Min(minStart, e.StartTime);
				}
				return Mathf.Max(this.EndTime - minStart, 0.01f);
			}
		}

		// Token: 0x06006A73 RID: 27251
		public override string ToString()
		{
			return this.StartTimestamp.ToString("HH:mm:ss") + " - Duration: " + this.Duration.ToString("F1") + "s";
		}

		// Token: 0x04005CC4 RID: 23748
		public DateTime StartTimestamp;

		// Token: 0x04005CC5 RID: 23749
		public Dictionary<string, UserDpsTracker.DpsEntry> Entries = new Dictionary<string, UserDpsTracker.DpsEntry>();

		// Token: 0x04005CE2 RID: 23778
		public float EndTime;
	}
}

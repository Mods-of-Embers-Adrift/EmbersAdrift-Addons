using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000E09 RID: 3593
public class UserDpsTracker : MonoBehaviour
{
	// Token: 0x17001953 RID: 6483
	// (get) Token: 0x06006A73 RID: 27251
	public static Dictionary<string, float> CurrentDps
	{
		get
		{
			if (!(UserDpsTracker._instance != null))
			{
				return null;
			}
			return UserDpsTracker._instance._GetDpsSnapshot();
		}
	}

	// Token: 0x06006A74 RID: 27252
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

	// Token: 0x06006A75 RID: 27253
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

	// Token: 0x06006A76 RID: 27254
	private Dictionary<string, float> _GetDpsSnapshot()
	{
		Dictionary<string, float> snapshot = new Dictionary<string, float>();
		foreach (KeyValuePair<string, UserDpsTracker.DpsEntry> kvp in this._dpsDict)
		{
			snapshot[kvp.Key] = kvp.Value.DPS;
		}
		return snapshot;
	}

	// Token: 0x06006A77 RID: 27255
	public static void Reset()
	{
		if (UserDpsTracker._instance == null)
		{
			return;
		}
		UserDpsTracker._instance._dpsDict.Clear();
		UserDpsTracker._instance._hpsDict.Clear();
		UserDpsTracker._instance._pastFights.Clear();
	}

	// Token: 0x06006A78 RID: 27256
	public UserDpsTracker()
	{
		this._pastFights = new List<UserDpsTracker.FightSession>();
		this._combatTimeout = 8f;
		base..ctor();
	}

	// Token: 0x17001954 RID: 6484
	// (get) Token: 0x06006A79 RID: 27257
	public static IReadOnlyDictionary<string, UserDpsTracker.DpsEntry> Entries
	{
		get
		{
			if (!(UserDpsTracker._instance != null))
			{
				return null;
			}
			return UserDpsTracker._instance._dpsDict;
		}
	}

	// Token: 0x06006A7A RID: 27258
	private void Awake()
	{
		UserDpsTracker._instance = this;
	}

	// Token: 0x06006A7B RID: 27259
	private void Update()
	{
		if (this._dpsDict.Count > 0 && Time.time - this._lastDamageTime > this._combatTimeout)
		{
			this.SaveSession();
			this._dpsDict.Clear();
			this._hpsDict.Clear();
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
		foreach (KeyValuePair<string, UserDpsTracker.DpsEntry> kvp in this._dpsDict)
		{
			session.Entries[kvp.Key] = kvp.Value.CloneForSession();
		}
		this._pastFights.Insert(0, session);
		if (this._pastFights.Count > 20)
		{
			this._pastFights.RemoveAt(this._pastFights.Count - 1);
		}
	}

	// Token: 0x17001955 RID: 6485
	// (get) Token: 0x06006A7D RID: 27261
	public static IReadOnlyList<UserDpsTracker.FightSession> PastFights
	{
		get
		{
			if (!(UserDpsTracker._instance != null))
			{
				return null;
			}
			return UserDpsTracker._instance._pastFights;
		}
	}

	// Token: 0x17001960 RID: 6496
	// (get) Token: 0x06006AE0 RID: 27360
	public static Dictionary<string, float> CurrentHps
	{
		get
		{
			if (UserDpsTracker._instance == null)
			{
				return null;
			}
			Dictionary<string, float> snapshot = new Dictionary<string, float>();
			foreach (KeyValuePair<string, UserDpsTracker.DpsEntry> kvp in UserDpsTracker._instance._hpsDict)
			{
				snapshot[kvp.Key] = kvp.Value.HPS;
			}
			return snapshot;
		}
	}

	// Token: 0x17001962 RID: 6498
	// (get) Token: 0x06006AE2 RID: 27362
	public static IReadOnlyDictionary<string, UserDpsTracker.DpsEntry> HealingEntries
	{
		get
		{
			UserDpsTracker instance = UserDpsTracker._instance;
			if (instance == null)
			{
				return null;
			}
			return instance._hpsDict;
		}
	}

	// Token: 0x06006AE6 RID: 27366
	public static void RecordHealing(string source, float amount)
	{
		if (UserDpsTracker._instance == null)
		{
			return;
		}
		UserDpsTracker.DpsEntry entry;
		if (!UserDpsTracker._instance._hpsDict.TryGetValue(source, out entry))
		{
			entry = new UserDpsTracker.DpsEntry
			{
				StartTime = Time.time
			};
			UserDpsTracker._instance._hpsDict[source] = entry;
		}
		entry.TotalHealing += amount;
	}

	// Token: 0x04005CD1 RID: 23761
	private Dictionary<string, UserDpsTracker.DpsEntry> _dpsDict = new Dictionary<string, UserDpsTracker.DpsEntry>();

	// Token: 0x04005CD2 RID: 23762
	private static UserDpsTracker _instance;

	// Token: 0x04005CD3 RID: 23763
	private List<UserDpsTracker.FightSession> _pastFights;

	// Token: 0x04005CD4 RID: 23764
	private float _lastDamageTime;

	// Token: 0x04005CD5 RID: 23765
	private float _combatTimeout;

	// Token: 0x04005D5D RID: 23901
	private Dictionary<string, UserDpsTracker.DpsEntry> _hpsDict = new Dictionary<string, UserDpsTracker.DpsEntry>();

	// Token: 0x02000E0A RID: 3594
	public class DpsEntry
	{
		// Token: 0x17001956 RID: 6486
		// (get) Token: 0x06006A7E RID: 27262
		public float DPS
		{
			get
			{
				return this.TotalDamage / this.Duration;
			}
		}

		// Token: 0x17001957 RID: 6487
		// (get) Token: 0x06006A80 RID: 27264
		public float Duration
		{
			get
			{
				return Mathf.Max(((this.EndTime > 0f) ? this.EndTime : Time.time) - this.StartTime, 0.01f);
			}
		}

		// Token: 0x06006A81 RID: 27265
		public UserDpsTracker.DpsEntry CloneForSession()
		{
			return new UserDpsTracker.DpsEntry
			{
				TotalDamage = this.TotalDamage,
				TotalHealing = this.TotalHealing,
				StartTime = this.StartTime,
				EndTime = Time.time
			};
		}

		// Token: 0x17001965 RID: 6501
		// (get) Token: 0x06006AEE RID: 27374
		public float HPS
		{
			get
			{
				return this.TotalHealing / this.Duration;
			}
		}

		// Token: 0x04005CD6 RID: 23766
		public float TotalDamage;

		// Token: 0x04005CD7 RID: 23767
		public float StartTime;

		// Token: 0x04005CD8 RID: 23768
		public float EndTime;

		// Token: 0x04005D63 RID: 23907
		public float TotalHealing;
	}

	// Token: 0x02000E0B RID: 3595
	public class FightSession
	{
		// Token: 0x17001958 RID: 6488
		// (get) Token: 0x06006A82 RID: 27266
		public float Duration
		{
			get
			{
				float minStart = float.MaxValue;
				foreach (UserDpsTracker.DpsEntry entry in this.Entries.Values)
				{
					minStart = Mathf.Min(minStart, entry.StartTime);
				}
				return Mathf.Max(this.EndTime - minStart, 0.01f);
			}
		}

		// Token: 0x06006A83 RID: 27267
		public override string ToString()
		{
			return this.StartTimestamp.ToString("HH:mm:ss") + " - Duration: " + this.Duration.ToString("F1") + "s";
		}

		// Token: 0x04005CD9 RID: 23769
		public DateTime StartTimestamp;

		// Token: 0x04005CDA RID: 23770
		public Dictionary<string, UserDpsTracker.DpsEntry> Entries = new Dictionary<string, UserDpsTracker.DpsEntry>();

		// Token: 0x04005CDB RID: 23771
		public float EndTime;
	}
}

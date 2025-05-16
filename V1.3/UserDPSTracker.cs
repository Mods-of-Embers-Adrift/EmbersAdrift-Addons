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
			if (!(UserDpsTracker._instance != null))
			{
				return null;
			}
			return UserDpsTracker._instance._GetDpsSnapshot();
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

	// Token: 0x06006A6A RID: 27242
	private Dictionary<string, float> _GetDpsSnapshot()
	{
		Dictionary<string, float> dict = new Dictionary<string, float>();
		foreach (KeyValuePair<string, UserDpsTracker.DpsEntry> pair in this._dpsDict)
		{
			dict[pair.Key] = pair.Value.DPS;
		}
		return dict;
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
		UserDpsTracker._instance._threatDict.Clear();
	}

	// Token: 0x06006A6C RID: 27244
	public UserDpsTracker()
	{
		this._pastFights = new List<UserDpsTracker.FightSession>();
		this._combatTimeout = 8f;
		base..ctor();
	}

	// Token: 0x17001952 RID: 6482
	// (get) Token: 0x06006A6D RID: 27245
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

	// Token: 0x06006A6E RID: 27246
	private void Awake()
	{
		UserDpsTracker._instance = this;
	}

	// Token: 0x06006A6F RID: 27247
	private void Update()
	{
		if (this._dpsDict.Count > 0 && Time.time - this._lastDamageTime > this._combatTimeout)
		{
			this.SaveSession();
			this._dpsDict.Clear();
			this._threatDict.Clear();
		}
	}

	// Token: 0x06006A70 RID: 27248
	private void SaveSession()
	{
		UserDpsTracker.FightSession session = new UserDpsTracker.FightSession
		{
			StartTimestamp = DateTime.Now,
			EndTime = Time.time
		};
		foreach (KeyValuePair<string, UserDpsTracker.DpsEntry> pair in this._dpsDict)
		{
			session.Entries[pair.Key] = pair.Value.CloneForSession();
		}
		this._pastFights.Insert(0, session);
		if (this._pastFights.Count > 20)
		{
			this._pastFights.RemoveAt(this._pastFights.Count - 1);
		}
	}

	// Token: 0x17001953 RID: 6483
	// (get) Token: 0x06006A71 RID: 27249
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

	// Token: 0x1700195A RID: 6490
	// (get) Token: 0x06006AA9 RID: 27305
	public static Dictionary<string, float> CurrentThreat
	{
		get
		{
			if (!(UserDpsTracker._instance != null))
			{
				return null;
			}
			return new Dictionary<string, float>(UserDpsTracker._instance._threatDict);
		}
	}

	// Token: 0x06006AAC RID: 27308
	public static void RecordThreat(string source, float rawDamage, EffectApplicationFlags flags)
	{
		if (UserDpsTracker._instance == null)
		{
			return;
		}
		float dmgMult;
		float absorbMult;
		flags.GetThreatMultipliers(out dmgMult, out absorbMult);
		float threat = rawDamage * dmgMult;
		float current;
		if (!UserDpsTracker._instance._threatDict.TryGetValue(source, out current))
		{
			UserDpsTracker._instance._threatDict[source] = threat;
			return;
		}
		UserDpsTracker._instance._threatDict[source] = current + threat;
	}

	// Token: 0x04005CC1 RID: 23745
	private Dictionary<string, UserDpsTracker.DpsEntry> _dpsDict = new Dictionary<string, UserDpsTracker.DpsEntry>();

	// Token: 0x04005CC2 RID: 23746
	private static UserDpsTracker _instance;

	// Token: 0x04005CC3 RID: 23747
	private List<UserDpsTracker.FightSession> _pastFights;

	// Token: 0x04005CC4 RID: 23748
	private float _lastDamageTime;

	// Token: 0x04005CC5 RID: 23749
	private float _combatTimeout;

	// Token: 0x04005D03 RID: 23811
	private Dictionary<string, float> _threatDict = new Dictionary<string, float>();

	// Token: 0x02000E0A RID: 3594
	public class DpsEntry
	{
		// Token: 0x17001954 RID: 6484
		// (get) Token: 0x06006A72 RID: 27250
		public float DPS
		{
			get
			{
				return this.TotalDamage / this.Duration;
			}
		}

		// Token: 0x17001955 RID: 6485
		// (get) Token: 0x06006A74 RID: 27252
		public float Duration
		{
			get
			{
				return Mathf.Max(((this.EndTime > 0f) ? this.EndTime : Time.time) - this.StartTime, 0.01f);
			}
		}

		// Token: 0x06006A75 RID: 27253
		public UserDpsTracker.DpsEntry CloneForSession()
		{
			return new UserDpsTracker.DpsEntry
			{
				TotalDamage = this.TotalDamage,
				StartTime = this.StartTime,
				EndTime = Time.time
			};
		}

		// Token: 0x04005CC6 RID: 23750
		public float TotalDamage;

		// Token: 0x04005CC7 RID: 23751
		public float StartTime;

		// Token: 0x04005CC8 RID: 23752
		public float EndTime;
	}

	// Token: 0x02000E0B RID: 3595
	public class FightSession
	{
		// Token: 0x17001956 RID: 6486
		// (get) Token: 0x06006A76 RID: 27254
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

		// Token: 0x06006A77 RID: 27255
		public override string ToString()
		{
			return this.StartTimestamp.ToString("HH:mm:ss") + " - Duration: " + this.Duration.ToString("F1") + "s";
		}

		// Token: 0x04005CC9 RID: 23753
		public DateTime StartTimestamp;

		// Token: 0x04005CCA RID: 23754
		public Dictionary<string, UserDpsTracker.DpsEntry> Entries = new Dictionary<string, UserDpsTracker.DpsEntry>();

		// Token: 0x04005CCB RID: 23755
		public float EndTime;
	}
}

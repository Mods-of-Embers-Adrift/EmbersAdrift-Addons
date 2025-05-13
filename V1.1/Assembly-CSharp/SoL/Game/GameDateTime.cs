using System;

namespace SoL.Game
{
	// Token: 0x02000579 RID: 1401
	public struct GameDateTime : IComparable<GameDateTime>, IEquatable<GameDateTime>
	{
		// Token: 0x06002B33 RID: 11059 RVA: 0x001464F0 File Offset: 0x001446F0
		private static DateTime GetGameDateTime(DateTime dateTime)
		{
			double value = (dateTime - GameDateTime.kStormhavenFounding).TotalSeconds / 6780.0;
			return GameDateTime.kStartingDate.AddDays(value);
		}

		// Token: 0x170008F1 RID: 2289
		// (get) Token: 0x06002B34 RID: 11060 RVA: 0x0005DFA8 File Offset: 0x0005C1A8
		public static GameDateTime Now
		{
			get
			{
				return new GameDateTime(GameTimeReplicator.GetServerCorrectedDateTime(DateTime.Now));
			}
		}

		// Token: 0x170008F2 RID: 2290
		// (get) Token: 0x06002B35 RID: 11061 RVA: 0x0005DFB9 File Offset: 0x0005C1B9
		public static GameDateTime UtcNow
		{
			get
			{
				return new GameDateTime(GameTimeReplicator.GetServerCorrectedDateTime(DateTime.UtcNow));
			}
		}

		// Token: 0x170008F3 RID: 2291
		// (get) Token: 0x06002B36 RID: 11062 RVA: 0x0005DFCA File Offset: 0x0005C1CA
		public static GameDateTime Today
		{
			get
			{
				return new GameDateTime(GameTimeReplicator.GetServerCorrectedDateTime(DateTime.Today));
			}
		}

		// Token: 0x170008F4 RID: 2292
		// (get) Token: 0x06002B37 RID: 11063 RVA: 0x0014652C File Offset: 0x0014472C
		public int Day
		{
			get
			{
				return this.m_gameDateTime.Day;
			}
		}

		// Token: 0x170008F5 RID: 2293
		// (get) Token: 0x06002B38 RID: 11064 RVA: 0x00146548 File Offset: 0x00144748
		public DayOfWeek DayOfWeek
		{
			get
			{
				return this.m_gameDateTime.DayOfWeek;
			}
		}

		// Token: 0x170008F6 RID: 2294
		// (get) Token: 0x06002B39 RID: 11065 RVA: 0x00146564 File Offset: 0x00144764
		public int DayOfYear
		{
			get
			{
				return this.m_gameDateTime.DayOfYear;
			}
		}

		// Token: 0x170008F7 RID: 2295
		// (get) Token: 0x06002B3A RID: 11066 RVA: 0x00146580 File Offset: 0x00144780
		public int Year
		{
			get
			{
				return this.m_gameDateTime.Year;
			}
		}

		// Token: 0x170008F8 RID: 2296
		// (get) Token: 0x06002B3B RID: 11067 RVA: 0x0014659C File Offset: 0x0014479C
		public int Month
		{
			get
			{
				return this.m_gameDateTime.Month;
			}
		}

		// Token: 0x170008F9 RID: 2297
		// (get) Token: 0x06002B3C RID: 11068 RVA: 0x001465B8 File Offset: 0x001447B8
		public int Hour
		{
			get
			{
				return this.m_gameDateTime.Hour;
			}
		}

		// Token: 0x170008FA RID: 2298
		// (get) Token: 0x06002B3D RID: 11069 RVA: 0x001465D4 File Offset: 0x001447D4
		public int Minute
		{
			get
			{
				return this.m_gameDateTime.Minute;
			}
		}

		// Token: 0x170008FB RID: 2299
		// (get) Token: 0x06002B3E RID: 11070 RVA: 0x001465F0 File Offset: 0x001447F0
		public int Second
		{
			get
			{
				return this.m_gameDateTime.Second;
			}
		}

		// Token: 0x170008FC RID: 2300
		// (get) Token: 0x06002B3F RID: 11071 RVA: 0x0014660C File Offset: 0x0014480C
		public int Millisecond
		{
			get
			{
				return this.m_gameDateTime.Millisecond;
			}
		}

		// Token: 0x170008FD RID: 2301
		// (get) Token: 0x06002B40 RID: 11072 RVA: 0x00146628 File Offset: 0x00144828
		public long Ticks
		{
			get
			{
				return this.m_gameDateTime.Ticks;
			}
		}

		// Token: 0x170008FE RID: 2302
		// (get) Token: 0x06002B41 RID: 11073 RVA: 0x00146644 File Offset: 0x00144844
		public TimeSpan TimeOfDay
		{
			get
			{
				return this.m_gameDateTime.TimeOfDay;
			}
		}

		// Token: 0x170008FF RID: 2303
		// (get) Token: 0x06002B42 RID: 11074 RVA: 0x0005DFDB File Offset: 0x0005C1DB
		public DateTime DateTime
		{
			get
			{
				return this.m_gameDateTime;
			}
		}

		// Token: 0x06002B43 RID: 11075 RVA: 0x0005DFE3 File Offset: 0x0005C1E3
		public GameDateTime(DateTime dateTime)
		{
			this.m_dateTime = dateTime;
			this.m_gameDateTime = GameDateTime.GetGameDateTime(dateTime);
		}

		// Token: 0x06002B44 RID: 11076 RVA: 0x0005DFF8 File Offset: 0x0005C1F8
		public GameDateTime(long ticks)
		{
			this.m_dateTime = new DateTime(ticks);
			this.m_gameDateTime = GameDateTime.GetGameDateTime(this.m_dateTime);
		}

		// Token: 0x06002B45 RID: 11077 RVA: 0x0005E017 File Offset: 0x0005C217
		public GameDateTime(int year, int month, int day)
		{
			this.m_dateTime = new DateTime(year, month, day);
			this.m_gameDateTime = GameDateTime.GetGameDateTime(this.m_dateTime);
		}

		// Token: 0x06002B46 RID: 11078 RVA: 0x0005E038 File Offset: 0x0005C238
		public GameDateTime(int year, int month, int day, int hour, int minute, int second)
		{
			this.m_dateTime = new DateTime(year, month, day, hour, minute, second);
			this.m_gameDateTime = GameDateTime.GetGameDateTime(this.m_dateTime);
		}

		// Token: 0x06002B47 RID: 11079 RVA: 0x00146660 File Offset: 0x00144860
		public override string ToString()
		{
			return this.m_gameDateTime.ToString();
		}

		// Token: 0x06002B48 RID: 11080 RVA: 0x0014667C File Offset: 0x0014487C
		public string ToString(string format)
		{
			return this.m_gameDateTime.ToString(format);
		}

		// Token: 0x06002B49 RID: 11081 RVA: 0x00146698 File Offset: 0x00144898
		public bool Equals(GameDateTime other)
		{
			return this.m_gameDateTime.Equals(other.m_gameDateTime);
		}

		// Token: 0x06002B4A RID: 11082 RVA: 0x001466BC File Offset: 0x001448BC
		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (obj is GameDateTime)
			{
				GameDateTime other = (GameDateTime)obj;
				return this.Equals(other);
			}
			return false;
		}

		// Token: 0x06002B4B RID: 11083 RVA: 0x001466E8 File Offset: 0x001448E8
		public override int GetHashCode()
		{
			return this.m_gameDateTime.GetHashCode();
		}

		// Token: 0x06002B4C RID: 11084 RVA: 0x00146704 File Offset: 0x00144904
		public int CompareTo(GameDateTime other)
		{
			return this.m_gameDateTime.CompareTo(other.m_gameDateTime);
		}

		// Token: 0x06002B4D RID: 11085 RVA: 0x00146728 File Offset: 0x00144928
		public static bool operator ==(GameDateTime d1, GameDateTime d2)
		{
			return d1.m_gameDateTime.Ticks == d2.m_gameDateTime.Ticks;
		}

		// Token: 0x06002B4E RID: 11086 RVA: 0x00146754 File Offset: 0x00144954
		public static bool operator !=(GameDateTime d1, GameDateTime d2)
		{
			return d1.m_gameDateTime.Ticks != d2.m_gameDateTime.Ticks;
		}

		// Token: 0x06002B4F RID: 11087 RVA: 0x00146784 File Offset: 0x00144984
		public static bool operator <(GameDateTime t1, GameDateTime t2)
		{
			return t1.m_gameDateTime.Ticks < t2.m_gameDateTime.Ticks;
		}

		// Token: 0x06002B50 RID: 11088 RVA: 0x001467B0 File Offset: 0x001449B0
		public static bool operator <=(GameDateTime t1, GameDateTime t2)
		{
			return t1.m_gameDateTime.Ticks <= t2.m_gameDateTime.Ticks;
		}

		// Token: 0x06002B51 RID: 11089 RVA: 0x001467E0 File Offset: 0x001449E0
		public static bool operator >(GameDateTime t1, GameDateTime t2)
		{
			return t1.m_gameDateTime.Ticks > t2.m_gameDateTime.Ticks;
		}

		// Token: 0x06002B52 RID: 11090 RVA: 0x0014680C File Offset: 0x00144A0C
		public static bool operator >=(GameDateTime t1, GameDateTime t2)
		{
			return t1.m_gameDateTime.Ticks >= t2.m_gameDateTime.Ticks;
		}

		// Token: 0x04002B66 RID: 11110
		public const float kEarthMinutesPerGameDay = 113f;

		// Token: 0x04002B67 RID: 11111
		public const float kEarthSecondsPerGameDay = 6780f;

		// Token: 0x04002B68 RID: 11112
		public const float kEarthHoursPerGameDay = 1.8833333f;

		// Token: 0x04002B69 RID: 11113
		public const float kSecondsPerMinute = 60f;

		// Token: 0x04002B6A RID: 11114
		public const float kMinutesPerHour = 60f;

		// Token: 0x04002B6B RID: 11115
		public const float kEarthHoursPerDay = 24f;

		// Token: 0x04002B6C RID: 11116
		private const float kEarthSecondsPerDay = 86400f;

		// Token: 0x04002B6D RID: 11117
		public const float kGameHoursPerDay = 1.8833333f;

		// Token: 0x04002B6E RID: 11118
		private const float kGameDaysInEarthSeconds = 6780f;

		// Token: 0x04002B6F RID: 11119
		private const float kGameSeconds = 0.07847222f;

		// Token: 0x04002B70 RID: 11120
		public const float kGameSecondsPerEarthSecond = 12.743362f;

		// Token: 0x04002B71 RID: 11121
		private static readonly DateTime kStormhavenFounding = new DateTime(2015, 9, 1);

		// Token: 0x04002B72 RID: 11122
		public static readonly DateTime kStartingDate = new DateTime(1574, 1, 1);

		// Token: 0x04002B73 RID: 11123
		private readonly DateTime m_dateTime;

		// Token: 0x04002B74 RID: 11124
		private readonly DateTime m_gameDateTime;
	}
}

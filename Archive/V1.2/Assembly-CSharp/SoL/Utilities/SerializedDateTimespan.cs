using System;
using System.Globalization;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x020002C6 RID: 710
	[Serializable]
	public class SerializedDateTimespan
	{
		// Token: 0x060014C7 RID: 5319 RVA: 0x000FB768 File Offset: 0x000F9968
		public bool WithinTimespan()
		{
			DateTime t = this.m_useUtc ? DateTime.UtcNow : DateTime.Now;
			if (this.m_cachedStartDateTime == null || this.m_cachedEndDateTime == null)
			{
				DateTime value;
				DateTime value2;
				this.GetStartEndDateTimes(out value, out value2);
				this.m_cachedStartDateTime = new DateTime?(value);
				this.m_cachedEndDateTime = new DateTime?(value2);
			}
			return t >= this.m_cachedStartDateTime.Value && t <= this.m_cachedEndDateTime.Value;
		}

		// Token: 0x060014C8 RID: 5320 RVA: 0x000507F8 File Offset: 0x0004E9F8
		private void PrintStartEnd()
		{
			Debug.Log(this.GetStartEndDateTimeString());
		}

		// Token: 0x060014C9 RID: 5321 RVA: 0x000FB7EC File Offset: 0x000F99EC
		internal string GetStartEndDateTimeString()
		{
			DateTime dateTime;
			DateTime dateTime2;
			this.GetStartEndDateTimes(out dateTime, out dateTime2);
			return dateTime.ToString(CultureInfo.InvariantCulture) + " through " + dateTime2.ToString(CultureInfo.InvariantCulture);
		}

		// Token: 0x060014CA RID: 5322 RVA: 0x000FB828 File Offset: 0x000F9A28
		private void GetStartEndDateTimes(out DateTime start, out DateTime end)
		{
			start = new DateTime((this.m_useUtc ? DateTime.UtcNow : DateTime.Now).Year, this.m_month, this.m_day, this.m_hour, this.m_minute, 0);
			TimeSpan t = new TimeSpan(this.m_days, this.m_hours, this.m_minutes, 0);
			end = start + t;
		}

		// Token: 0x04001D01 RID: 7425
		private const string kStart = "Start Date";

		// Token: 0x04001D02 RID: 7426
		private const string kTimespan = "Timespan";

		// Token: 0x04001D03 RID: 7427
		private DateTime? m_cachedStartDateTime;

		// Token: 0x04001D04 RID: 7428
		private DateTime? m_cachedEndDateTime;

		// Token: 0x04001D05 RID: 7429
		[SerializeField]
		private bool m_useUtc;

		// Token: 0x04001D06 RID: 7430
		[Range(1f, 12f)]
		[SerializeField]
		private int m_month = 1;

		// Token: 0x04001D07 RID: 7431
		[Range(1f, 31f)]
		[SerializeField]
		private int m_day = 1;

		// Token: 0x04001D08 RID: 7432
		[Range(0f, 23f)]
		[SerializeField]
		private int m_hour;

		// Token: 0x04001D09 RID: 7433
		[Range(0f, 59f)]
		[SerializeField]
		private int m_minute;

		// Token: 0x04001D0A RID: 7434
		[SerializeField]
		private int m_days;

		// Token: 0x04001D0B RID: 7435
		[SerializeField]
		private int m_hours;

		// Token: 0x04001D0C RID: 7436
		[SerializeField]
		private int m_minutes;
	}
}

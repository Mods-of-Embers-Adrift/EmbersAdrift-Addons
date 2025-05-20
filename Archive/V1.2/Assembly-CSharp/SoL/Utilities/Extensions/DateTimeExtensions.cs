using System;

namespace SoL.Utilities.Extensions
{
	// Token: 0x0200032D RID: 813
	public static class DateTimeExtensions
	{
		// Token: 0x06001657 RID: 5719 RVA: 0x001003EC File Offset: 0x000FE5EC
		public static long ToUnixTime(this DateTime time)
		{
			if (time.Kind != DateTimeKind.Utc)
			{
				return -1L;
			}
			return (time - DateTimeExtensions.UnixEpoch).Ticks / 10000000L;
		}

		// Token: 0x06001658 RID: 5720 RVA: 0x00100420 File Offset: 0x000FE620
		public static DateTime FromUnixTime(this long unixEpochSeconds)
		{
			return DateTimeExtensions.UnixEpoch.AddTicks(unixEpochSeconds * 10000000L);
		}

		// Token: 0x06001659 RID: 5721 RVA: 0x00051986 File Offset: 0x0004FB86
		public static float GetYearFraction(this DateTime time)
		{
			return (float)time.DayOfYear / 366f;
		}

		// Token: 0x0600165A RID: 5722 RVA: 0x00051996 File Offset: 0x0004FB96
		public static DateTime ChangeYear(this DateTime time, int newYear)
		{
			return new DateTime(newYear, time.Month, time.Day, time.Hour, time.Minute, time.Second, time.Millisecond);
		}

		// Token: 0x04001E4C RID: 7756
		public static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
	}
}

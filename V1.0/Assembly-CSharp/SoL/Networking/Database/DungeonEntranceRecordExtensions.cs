using System;

namespace SoL.Networking.Database
{
	// Token: 0x02000451 RID: 1105
	public static class DungeonEntranceRecordExtensions
	{
		// Token: 0x06001F2E RID: 7982 RVA: 0x0011EE0C File Offset: 0x0011D00C
		public static bool IsActive(this DungeonEntranceRecord record)
		{
			bool flag = record != null && record.Status == DungeonEntranceStatus.Active && record.DeactivationTime != null;
			if (flag)
			{
				DateTime utcNow = DateTime.UtcNow;
				DateTime t = record.DeactivationTime.Value.AddSeconds(20.0);
				flag = (utcNow < t);
			}
			return flag;
		}

		// Token: 0x040024A3 RID: 9379
		private const double kExpirationBuffer = 20.0;
	}
}

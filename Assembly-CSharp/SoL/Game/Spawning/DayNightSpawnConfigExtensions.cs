using System;

namespace SoL.Game.Spawning
{
	// Token: 0x02000677 RID: 1655
	internal static class DayNightSpawnConfigExtensions
	{
		// Token: 0x06003346 RID: 13126 RVA: 0x000635BC File Offset: 0x000617BC
		internal static string GetPrimaryProfileLabel(this DayNightSpawnCondition condition)
		{
			if (condition - DayNightSpawnCondition.SeparateDayNight <= 1)
			{
				return "Day Spawn Profiles";
			}
			if (condition != DayNightSpawnCondition.NightOnly)
			{
				return "Spawn Profiles";
			}
			return "Night Spawn Profiles";
		}

		// Token: 0x06003347 RID: 13127 RVA: 0x000635DB File Offset: 0x000617DB
		internal static string GetAlternateProfileLabel(this DayNightSpawnCondition condition)
		{
			if (condition == DayNightSpawnCondition.SeparateDayNight)
			{
				return "Night Spawn Profiles";
			}
			return "Alternate Spawn Profiles";
		}
	}
}

using System;
using SoL.Game;

namespace SoL.Utilities
{
	// Token: 0x0200029D RID: 669
	public static class MonolithFlagsExtensions
	{
		// Token: 0x0600142E RID: 5166 RVA: 0x0004FB40 File Offset: 0x0004DD40
		public static bool HasBitFlag(this MonolithFlags a, MonolithFlags b)
		{
			return (a & b) == b;
		}

		// Token: 0x0600142F RID: 5167 RVA: 0x000502DF File Offset: 0x0004E4DF
		public static bool HasAnyFlags(this MonolithFlags a, MonolithFlags b)
		{
			return a != MonolithFlags.None && b != MonolithFlags.None && (a & b) > MonolithFlags.None;
		}

		// Token: 0x06001430 RID: 5168 RVA: 0x000F9480 File Offset: 0x000F7680
		public static ZoneId GetZoneIdForFlag(this MonolithFlags flag)
		{
			if (flag <= MonolithFlags.Redshore)
			{
				switch (flag)
				{
				case MonolithFlags.NewhavenValley:
					return ZoneId.NewhavenValley;
				case MonolithFlags.Northreach:
					return ZoneId.Northreach;
				case MonolithFlags.NewhavenValley | MonolithFlags.Northreach:
					break;
				case MonolithFlags.Meadowlands:
					return ZoneId.Meadowlands;
				default:
					if (flag == MonolithFlags.Dryfoot)
					{
						return ZoneId.Dryfoot;
					}
					if (flag == MonolithFlags.Redshore)
					{
						return ZoneId.RedshoreForest;
					}
					break;
				}
			}
			else if (flag <= MonolithFlags.Highlands)
			{
				if (flag == MonolithFlags.Grimstone)
				{
					return ZoneId.GrimstoneCanyon;
				}
				if (flag == MonolithFlags.Highlands)
				{
					return ZoneId.HighlandHills;
				}
			}
			else
			{
				if (flag == MonolithFlags.Grizzled)
				{
					return ZoneId.GrizzledPeaks;
				}
				if (flag == MonolithFlags.NewhavenCity)
				{
					return ZoneId.NewhavenCity;
				}
			}
			return ZoneId.None;
		}
	}
}

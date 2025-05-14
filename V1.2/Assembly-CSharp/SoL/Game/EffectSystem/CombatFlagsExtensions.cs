using System;
using Cysharp.Text;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C65 RID: 3173
	public static class CombatFlagsExtensions
	{
		// Token: 0x0600613E RID: 24894 RVA: 0x0004FB40 File Offset: 0x0004DD40
		public static bool HasBitFlag(this CombatFlags a, CombatFlags b)
		{
			return (a & b) == b;
		}

		// Token: 0x0600613F RID: 24895 RVA: 0x0008189F File Offset: 0x0007FA9F
		public static string GetShortTooltipDescription(this CombatFlags flag)
		{
			switch (flag)
			{
			case CombatFlags.Advantage:
				return "ADV";
			case CombatFlags.Disadvantage:
				return "DIS";
			case CombatFlags.IgnoreActiveDefenses:
				return "IAD";
			}
			return string.Empty;
		}

		// Token: 0x06006140 RID: 24896 RVA: 0x001FF5A4 File Offset: 0x001FD7A4
		public static string GetLongTooltipDescription(this CombatFlags flag)
		{
			switch (flag)
			{
			case CombatFlags.Advantage:
				return ZString.Format<string, string, string>("{0} {1}(Advantage): dice are rolled twice and the higher result is taken.{2}", flag.GetShortTooltipDescription(), "<i><size=80%>", "</size></i>");
			case CombatFlags.Disadvantage:
				return ZString.Format<string, string, string>("{0} {1}(Disadvantage): dice are rolled twice and the lower result is taken.{2}", flag.GetShortTooltipDescription(), "<i><size=80%>", "</size></i>");
			case CombatFlags.IgnoreActiveDefenses:
				return ZString.Format<string, string, string>("{0} {1}(Ignore Active Defenses): bypasses avoid, block, and parry.{2}", flag.GetShortTooltipDescription(), "<i><size=80%>", "</size></i>");
			}
			return string.Empty;
		}
	}
}

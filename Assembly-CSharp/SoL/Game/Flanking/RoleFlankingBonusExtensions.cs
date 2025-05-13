using System;

namespace SoL.Game.Flanking
{
	// Token: 0x02000BF9 RID: 3065
	public static class RoleFlankingBonusExtensions
	{
		// Token: 0x06005E83 RID: 24195 RVA: 0x0007F8A0 File Offset: 0x0007DAA0
		public static string GetAbbreviation(this RoleFlankingBonusType type)
		{
			if (type - RoleFlankingBonusType.IncreaseThreat <= 1)
			{
				return "THR";
			}
			if (type != RoleFlankingBonusType.ArmorDamage)
			{
				return string.Empty;
			}
			return "ARMOR DMG";
		}
	}
}

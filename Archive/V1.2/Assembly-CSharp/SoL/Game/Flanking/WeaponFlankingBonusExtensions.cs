using System;

namespace SoL.Game.Flanking
{
	// Token: 0x02000BFE RID: 3070
	public static class WeaponFlankingBonusExtensions
	{
		// Token: 0x06005E9C RID: 24220 RVA: 0x0007F9D2 File Offset: 0x0007DBD2
		public static string GetAbbreviation(this WeaponFlankingBonusType type)
		{
			switch (type)
			{
			case WeaponFlankingBonusType.Damage:
				return "DMG";
			case WeaponFlankingBonusType.Hit:
				return "HIT";
			case WeaponFlankingBonusType.Penetration:
				return "PEN";
			default:
				return string.Empty;
			}
		}
	}
}

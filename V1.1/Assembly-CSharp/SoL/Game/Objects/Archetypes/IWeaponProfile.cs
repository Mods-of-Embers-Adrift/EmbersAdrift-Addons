using System;
using SoL.Game.EffectSystem;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000AE1 RID: 2785
	public interface IWeaponProfile
	{
		// Token: 0x060055D9 RID: 21977
		StatType GetDamageType();

		// Token: 0x170013E0 RID: 5088
		// (get) Token: 0x060055DA RID: 21978
		WeaponTypes WeaponType { get; }

		// Token: 0x170013E1 RID: 5089
		// (get) Token: 0x060055DB RID: 21979
		DamageType DamageType { get; }

		// Token: 0x170013E2 RID: 5090
		// (get) Token: 0x060055DC RID: 21980
		MinMaxFloatRange Distance { get; }

		// Token: 0x170013E3 RID: 5091
		// (get) Token: 0x060055DD RID: 21981
		int Angle { get; }

		// Token: 0x170013E4 RID: 5092
		// (get) Token: 0x060055DE RID: 21982
		float AoeRadius { get; }

		// Token: 0x170013E5 RID: 5093
		// (get) Token: 0x060055DF RID: 21983
		float AoeAngle { get; }

		// Token: 0x170013E6 RID: 5094
		// (get) Token: 0x060055E0 RID: 21984
		float OffHandDamageMultiplier { get; }

		// Token: 0x060055E1 RID: 21985
		void CopyValuesFrom(IWeaponProfile other);
	}
}

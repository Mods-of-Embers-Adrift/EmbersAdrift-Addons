using System;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C37 RID: 3127
	public interface ICombatEffectSource
	{
		// Token: 0x1700172D RID: 5933
		// (get) Token: 0x0600609C RID: 24732
		UniqueId ArchetypeId { get; }

		// Token: 0x1700172E RID: 5934
		// (get) Token: 0x0600609D RID: 24733
		DeliveryParams DeliveryParams { get; }

		// Token: 0x0600609E RID: 24734
		TargetingParams GetTargetingParams(float level, AlchemyPowerLevel alchemyPowerLevel = AlchemyPowerLevel.None);

		// Token: 0x0600609F RID: 24735
		KinematicParameters GetKinematicParams(float level, AlchemyPowerLevel alchemyPowerLevel = AlchemyPowerLevel.None);

		// Token: 0x060060A0 RID: 24736
		CombatEffect GetCombatEffect(float level, AlchemyPowerLevel alchemyPowerLevel = AlchemyPowerLevel.None);
	}
}

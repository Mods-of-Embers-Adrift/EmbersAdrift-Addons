using System;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C3A RID: 3130
	public interface IVfxSource
	{
		// Token: 0x060060A7 RID: 24743
		bool TryGetEffects(int abilityLevel, AlchemyPowerLevel alchemyPowerLevel, bool isSecondary, out AbilityVFX effects);
	}
}

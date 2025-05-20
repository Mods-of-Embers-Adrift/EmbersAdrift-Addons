using System;
using SoL.Game.EffectSystem;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000ACF RID: 2767
	public interface IStatModifier
	{
		// Token: 0x170013BE RID: 5054
		// (get) Token: 0x06005572 RID: 21874
		VitalScalingValue[] Vitals { get; }

		// Token: 0x170013BF RID: 5055
		// (get) Token: 0x06005573 RID: 21875
		StatModifierScaling[] Stats { get; }
	}
}

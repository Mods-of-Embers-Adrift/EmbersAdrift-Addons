using System;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A2F RID: 2607
	public interface IDurability
	{
		// Token: 0x170011F1 RID: 4593
		// (get) Token: 0x060050A8 RID: 20648
		int MaxDamageAbsorption { get; }

		// Token: 0x060050A9 RID: 20649
		float GetCurrentDurability(float dmgAbsorbed);

		// Token: 0x170011F2 RID: 4594
		// (get) Token: 0x060050AA RID: 20650
		bool DegradeOnHit { get; }
	}
}

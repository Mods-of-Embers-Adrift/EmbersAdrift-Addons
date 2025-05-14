using System;
using SoL.Game.EffectSystem;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A2E RID: 2606
	public interface IDamageSource
	{
		// Token: 0x170011EF RID: 4591
		// (get) Token: 0x060050A6 RID: 20646
		UniqueId Id { get; }

		// Token: 0x170011F0 RID: 4592
		// (get) Token: 0x060050A7 RID: 20647
		DamageType Type { get; }
	}
}

using System;
using SoL.Game.EffectSystem;
using UnityEngine.AI;

namespace SoL.Game
{
	// Token: 0x02000592 RID: 1426
	public interface IMotor
	{
		// Token: 0x17000968 RID: 2408
		// (get) Token: 0x06002C8E RID: 11406
		// (set) Token: 0x06002C8F RID: 11407
		float SpeedModifier { get; set; }

		// Token: 0x17000969 RID: 2409
		// (get) Token: 0x06002C90 RID: 11408
		NavMeshAgent UnityNavAgent { get; }

		// Token: 0x06002C91 RID: 11409
		void ApplyRootEffect(bool adding, CombatEffect effect);

		// Token: 0x1700096A RID: 2410
		// (get) Token: 0x06002C92 RID: 11410
		bool IsRooted { get; }
	}
}

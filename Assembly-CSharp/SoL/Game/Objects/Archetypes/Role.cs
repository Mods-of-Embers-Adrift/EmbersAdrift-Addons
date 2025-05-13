using System;
using SoL.Game.EffectSystem;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000AD9 RID: 2777
	public abstract class Role : MasteryArchetype
	{
		// Token: 0x060055A5 RID: 21925
		public abstract bool MeetsHandheldRequirements(GameEntity entity);

		// Token: 0x170013D1 RID: 5073
		// (get) Token: 0x060055A6 RID: 21926 RVA: 0x00079294 File Offset: 0x00077494
		public StatModifierScaling[] StatModifiers
		{
			get
			{
				return this.m_statModifiers;
			}
		}

		// Token: 0x04004C02 RID: 19458
		private const string kActiveModifiersGroupName = "Modifiers/Active (combat stance)";

		// Token: 0x04004C03 RID: 19459
		[SerializeField]
		private MinMaxIntRange m_passiveModifierLevelRange = new MinMaxIntRange(1, 100);

		// Token: 0x04004C04 RID: 19460
		[SerializeField]
		private MinMaxIntRange m_activeModifierLevelRange = new MinMaxIntRange(1, 100);

		// Token: 0x04004C05 RID: 19461
		[SerializeField]
		private StatModifierScaling[] m_statModifiers;
	}
}

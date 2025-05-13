using System;
using SoL.Game.Objects.Archetypes;
using UnityEngine;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C43 RID: 3139
	[CreateAssetMenu(menuName = "SoL/NEW_EFFECTS/Combat Effect")]
	public class ScriptableCombatEffect : BaseArchetype, ICombatEffectSource
	{
		// Token: 0x17001741 RID: 5953
		// (get) Token: 0x060060D6 RID: 24790 RVA: 0x000813AA File Offset: 0x0007F5AA
		public CombatEffect Effect
		{
			get
			{
				return this.m_effect;
			}
		}

		// Token: 0x17001742 RID: 5954
		// (get) Token: 0x060060D7 RID: 24791 RVA: 0x000672BB File Offset: 0x000654BB
		UniqueId ICombatEffectSource.ArchetypeId
		{
			get
			{
				return base.Id;
			}
		}

		// Token: 0x17001743 RID: 5955
		// (get) Token: 0x060060D8 RID: 24792 RVA: 0x00049FFA File Offset: 0x000481FA
		DeliveryParams ICombatEffectSource.DeliveryParams
		{
			get
			{
				return null;
			}
		}

		// Token: 0x060060D9 RID: 24793 RVA: 0x00049FFA File Offset: 0x000481FA
		TargetingParams ICombatEffectSource.GetTargetingParams(float level, AlchemyPowerLevel alchemyPowerLevel)
		{
			return null;
		}

		// Token: 0x060060DA RID: 24794 RVA: 0x00049FFA File Offset: 0x000481FA
		KinematicParameters ICombatEffectSource.GetKinematicParams(float level, AlchemyPowerLevel alchemyPowerLevel)
		{
			return null;
		}

		// Token: 0x060060DB RID: 24795 RVA: 0x000813AA File Offset: 0x0007F5AA
		CombatEffect ICombatEffectSource.GetCombatEffect(float level, AlchemyPowerLevel alchemyPowerLevel)
		{
			return this.m_effect;
		}

		// Token: 0x04005380 RID: 21376
		[SerializeField]
		private CombatEffect m_effect;
	}
}

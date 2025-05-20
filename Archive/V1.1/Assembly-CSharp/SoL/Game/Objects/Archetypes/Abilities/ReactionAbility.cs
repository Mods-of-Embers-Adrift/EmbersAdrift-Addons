using System;
using SoL.Game.Animation;
using SoL.Game.EffectSystem;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes.Abilities
{
	// Token: 0x02000AF3 RID: 2803
	[CreateAssetMenu(menuName = "SoL/Objects/Itemization/Skills/Abilities/Reaction Ability")]
	public class ReactionAbility : DynamicAbility, ICombatEffectSource
	{
		// Token: 0x060056BB RID: 22203 RVA: 0x001E0D38 File Offset: 0x001DEF38
		private float GetLevel(GameEntity entity)
		{
			ArchetypeInstance archetypeInstance;
			MasteryArchetype masteryArchetype;
			if (entity.TryGetActiveWeaponMasteryAsType(out archetypeInstance, out masteryArchetype))
			{
				return archetypeInstance.GetAssociatedLevel(entity);
			}
			return 0f;
		}

		// Token: 0x17001445 RID: 5189
		// (get) Token: 0x060056BC RID: 22204 RVA: 0x00074515 File Offset: 0x00072715
		UniqueId ICombatEffectSource.ArchetypeId
		{
			get
			{
				return this.m_id;
			}
		}

		// Token: 0x17001446 RID: 5190
		// (get) Token: 0x060056BD RID: 22205 RVA: 0x00049FFA File Offset: 0x000481FA
		DeliveryParams ICombatEffectSource.DeliveryParams
		{
			get
			{
				return null;
			}
		}

		// Token: 0x060056BE RID: 22206 RVA: 0x00049FFA File Offset: 0x000481FA
		TargetingParams ICombatEffectSource.GetTargetingParams(float level, AlchemyPowerLevel alchemyPowerLevel)
		{
			return null;
		}

		// Token: 0x060056BF RID: 22207 RVA: 0x00049FFA File Offset: 0x000481FA
		KinematicParameters ICombatEffectSource.GetKinematicParams(float level, AlchemyPowerLevel alchemyPowerLevel)
		{
			return null;
		}

		// Token: 0x060056C0 RID: 22208 RVA: 0x00079C50 File Offset: 0x00077E50
		CombatEffect ICombatEffectSource.GetCombatEffect(float level, AlchemyPowerLevel alchemyPowerLevel)
		{
			return this.m_effect;
		}

		// Token: 0x060056C1 RID: 22209 RVA: 0x001E10B4 File Offset: 0x001DF2B4
		protected override bool TryGetMastery(GameEntity entity, out MasteryArchetype mastery)
		{
			ArchetypeInstance archetypeInstance;
			return entity.TryGetActiveWeaponMasteryAsType(out archetypeInstance, out mastery);
		}

		// Token: 0x060056C2 RID: 22210 RVA: 0x00079A8C File Offset: 0x00077C8C
		public override string GetInstanceId()
		{
			return base.Id;
		}

		// Token: 0x17001447 RID: 5191
		// (get) Token: 0x060056C3 RID: 22211 RVA: 0x00045BCA File Offset: 0x00043DCA
		protected override bool CreateInstanceUI
		{
			get
			{
				return false;
			}
		}

		// Token: 0x060056C4 RID: 22212 RVA: 0x0004475B File Offset: 0x0004295B
		protected override void PostExecution(ExecutionCache executionCache)
		{
		}

		// Token: 0x060056C5 RID: 22213 RVA: 0x0004479C File Offset: 0x0004299C
		protected override bool MeetsRequirementsForUI(GameEntity entity)
		{
			return true;
		}

		// Token: 0x060056C6 RID: 22214 RVA: 0x0005BFF3 File Offset: 0x0005A1F3
		protected override bool TryGetAbilityAnimation(GameEntity entity, out AbilityAnimation animation)
		{
			animation = null;
			return false;
		}

		// Token: 0x17001448 RID: 5192
		// (get) Token: 0x060056C7 RID: 22215 RVA: 0x0006108D File Offset: 0x0005F28D
		protected override float m_masteryCreditFactor
		{
			get
			{
				return 0f;
			}
		}

		// Token: 0x17001449 RID: 5193
		// (get) Token: 0x060056C8 RID: 22216 RVA: 0x00045BCA File Offset: 0x00043DCA
		protected override bool m_addGroupBonus
		{
			get
			{
				return false;
			}
		}

		// Token: 0x04004C84 RID: 19588
		[SerializeField]
		private CombatEffect m_effect;
	}
}

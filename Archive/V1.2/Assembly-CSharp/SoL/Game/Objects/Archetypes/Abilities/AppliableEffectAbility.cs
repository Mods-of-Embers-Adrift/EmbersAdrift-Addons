using System;
using SoL.Game.Animation;
using SoL.Game.EffectSystem;
using SoL.Managers;
using SoL.UI;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes.Abilities
{
	// Token: 0x02000AEC RID: 2796
	[CreateAssetMenu(menuName = "SoL/Objects/Itemization/Skills/Abilities/Appliable Effect Ability")]
	public class AppliableEffectAbility : AbilityArchetype, ICombatEffectSource
	{
		// Token: 0x17001402 RID: 5122
		// (get) Token: 0x06005624 RID: 22052 RVA: 0x0004479C File Offset: 0x0004299C
		protected override bool ShowTargetingParameters
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17001403 RID: 5123
		// (get) Token: 0x06005625 RID: 22053 RVA: 0x0004479C File Offset: 0x0004299C
		protected override bool ShowKinematicParameters
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17001404 RID: 5124
		// (get) Token: 0x06005626 RID: 22054 RVA: 0x0004479C File Offset: 0x0004299C
		protected override bool ShowCombatParameters
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17001405 RID: 5125
		// (get) Token: 0x06005627 RID: 22055 RVA: 0x0007974D File Offset: 0x0007794D
		protected override bool ShowAbilityAnimation
		{
			get
			{
				return !this.m_useAutoAttackAnimation && !this.m_useStanceAnimationIndex && this.m_animationIndexProfile == null;
			}
		}

		// Token: 0x17001406 RID: 5126
		// (get) Token: 0x06005628 RID: 22056 RVA: 0x0007976D File Offset: 0x0007796D
		public override bool IsWarlordSong
		{
			get
			{
				return this.m_isWarlordSong;
			}
		}

		// Token: 0x17001407 RID: 5127
		// (get) Token: 0x06005629 RID: 22057 RVA: 0x00079775 File Offset: 0x00077975
		protected override bool UseAutoAttackAnimation
		{
			get
			{
				return this.m_useAutoAttackAnimation;
			}
		}

		// Token: 0x0600562A RID: 22058 RVA: 0x001E06DC File Offset: 0x001DE8DC
		protected override AbilityAnimation GetAbilityAnimation(GameEntity entity)
		{
			if (this.m_animationIndexProfile)
			{
				if (!entity || entity.AnimancerController.CurrentAnimationSet == null)
				{
					return null;
				}
				return this.m_animationIndexProfile.GetAbilityAnimation(entity.AnimancerController.CurrentAnimationSet.Id);
			}
			else
			{
				if (!this.m_requireWeapons || !this.m_useStanceAnimationIndex)
				{
					return base.GetAbilityAnimation(entity);
				}
				if (!entity || entity.AnimancerController.CurrentAnimationSet == null)
				{
					return null;
				}
				return entity.AnimancerController.CurrentAnimationSet.GetAbilityAnimation(this.m_exeTime, this.m_animationIndex);
			}
		}

		// Token: 0x17001408 RID: 5128
		// (get) Token: 0x0600562B RID: 22059 RVA: 0x0007977D File Offset: 0x0007797D
		private bool m_allowAnimationIndex
		{
			get
			{
				return !this.m_useAutoAttackAnimation;
			}
		}

		// Token: 0x17001409 RID: 5129
		// (get) Token: 0x0600562C RID: 22060 RVA: 0x00079788 File Offset: 0x00077988
		private bool m_showAnimationIndexProfile
		{
			get
			{
				return this.m_allowAnimationIndex;
			}
		}

		// Token: 0x1700140A RID: 5130
		// (get) Token: 0x0600562D RID: 22061 RVA: 0x00079790 File Offset: 0x00077990
		private bool m_showUseAnimationIndex
		{
			get
			{
				return this.m_allowAnimationIndex && this.m_animationIndexProfile == null;
			}
		}

		// Token: 0x1700140B RID: 5131
		// (get) Token: 0x0600562E RID: 22062 RVA: 0x000797A8 File Offset: 0x000779A8
		private bool m_showUseAnimationIndexData
		{
			get
			{
				return this.m_showUseAnimationIndex && this.m_useStanceAnimationIndex;
			}
		}

		// Token: 0x1700140C RID: 5132
		// (get) Token: 0x0600562F RID: 22063 RVA: 0x00079788 File Offset: 0x00077988
		private bool m_showAnimationInfoBox
		{
			get
			{
				return this.m_allowAnimationIndex;
			}
		}

		// Token: 0x06005630 RID: 22064 RVA: 0x000797BA File Offset: 0x000779BA
		private string GetAnimIndexDescription()
		{
			if (!this.m_showAnimationInfoBox)
			{
				return null;
			}
			if (this.m_animationIndexProfile)
			{
				return this.m_animationIndexProfile.GetAnimIndexDescription();
			}
			if (!this.m_showUseAnimationIndexData)
			{
				return null;
			}
			return AnimancerUtilities.GetAnimIndexDescription(this.m_exeTime, this.m_animationIndex);
		}

		// Token: 0x06005631 RID: 22065 RVA: 0x001E0774 File Offset: 0x001DE974
		private bool HasCompatibleWeaponsInternal(IHandHeldItems handheldItems)
		{
			if (!(this.m_specialization != null))
			{
				CombatMasteryArchetype combatMasteryArchetype;
				return this.m_mastery.TryGetAsType(out combatMasteryArchetype) && combatMasteryArchetype.EntityHasCompatibleWeapons(handheldItems);
			}
			return this.m_specialization.EntityHasCompatibleWeapons(handheldItems);
		}

		// Token: 0x06005632 RID: 22066 RVA: 0x000797FA File Offset: 0x000779FA
		protected override bool MeetsRequirementsForUI(GameEntity entity, float level)
		{
			return (base.IsOnlyCombatStance || base.MeetsRequirementsForUI(entity, level)) && this.MeetsWeaponRequirementsForUI(entity);
		}

		// Token: 0x06005633 RID: 22067 RVA: 0x001E07B4 File Offset: 0x001DE9B4
		private bool MeetsWeaponRequirementsForUI(GameEntity entity)
		{
			ArchetypeInstance archetypeInstance;
			CombatMasteryArchetype combatMasteryArchetype;
			return !this.m_requireWeapons || (entity.TryGetActiveWeaponMasteryAsType(out archetypeInstance, out combatMasteryArchetype) && !(combatMasteryArchetype.Id != base.Mastery.Id) && this.HasCompatibleWeaponsInternal(entity.HandHeldItemCache) && entity.HandHeldItemCache.MainHand.WeaponItem && entity.HandHeldItemCache.MainHand.WeaponItem.CanExecuteWith(entity.HandHeldItemCache) && (this.m_bypassAmmoRequirement || !entity.HandHeldItemCache.MainHand.WeaponItem.RequiresAmmo || entity.HandHeldItemCache.MainHand.WeaponItem.HasProperAmmo(entity.HandHeldItemCache)));
		}

		// Token: 0x06005634 RID: 22068 RVA: 0x001E0878 File Offset: 0x001DEA78
		protected override bool ExecutionCheck(ExecutionCache executionCache, float executionProgress)
		{
			bool flag = executionProgress <= 0f;
			bool flag2 = base.ExecutionCheck(executionCache, executionProgress);
			if (flag2)
			{
				flag2 = executionCache.TargetingParams.ExecutionCheck(executionCache, executionProgress);
			}
			else if (executionCache.ForceTargetingCheck && executionCache.ExecutionParams != null && executionCache.ExecutionParams.ValidStances == StanceFlags.Combat)
			{
				executionCache.TargetingParams.ExecutionCheck(executionCache, executionProgress);
			}
			if (executionCache.MasteryInstance == null)
			{
				if (flag2)
				{
					executionCache.Message = "Invalid role!";
				}
				return false;
			}
			if (this.m_requireWeapons)
			{
				if (!executionCache.MainHand.WeaponItem)
				{
					executionCache.Message = "Invalid weapon!";
					return false;
				}
				ArchetypeInstance archetypeInstance;
				CombatMasteryArchetype combatMasteryArchetype;
				if (!executionCache.SourceEntity.TryGetActiveWeaponMasteryAsType(out archetypeInstance, out combatMasteryArchetype) || archetypeInstance.Archetype.Id != base.Mastery.Id)
				{
					executionCache.Message = "Invalid role!";
					return false;
				}
				executionCache.MasteryInstance = archetypeInstance;
				if (!this.HasCompatibleWeaponsInternal(executionCache))
				{
					executionCache.Message = "Incompatible weapons!";
					return false;
				}
				if (!executionCache.MainHand.WeaponItem.CanExecuteWith(executionCache))
				{
					executionCache.Message = "Invalid weapon config!";
					return false;
				}
				IDurability source;
				if (executionCache.MainHand.WeaponItem.TryGetAsType(out source) && source.IsWeaponBroken(executionCache.MainHand.Instance))
				{
					executionCache.Message = "Weapon is broken!";
					return false;
				}
				if (!this.m_bypassAmmoRequirement && executionCache.MainHand.WeaponItem.RequiresAmmo)
				{
					if (!executionCache.OffHand.RangedAmmo)
					{
						executionCache.Message = "Invalid ammo!";
						return false;
					}
					if (!executionCache.MainHand.WeaponItem.HasProperAmmo(executionCache))
					{
						executionCache.Message = "Invalid ammo!";
						return false;
					}
					if (flag)
					{
						executionCache.AddReductionTask(ReductionTaskType.Count, executionCache.OffHand.Instance, 1);
					}
				}
			}
			return flag2;
		}

		// Token: 0x1700140D RID: 5133
		// (get) Token: 0x06005635 RID: 22069 RVA: 0x00074515 File Offset: 0x00072715
		UniqueId ICombatEffectSource.ArchetypeId
		{
			get
			{
				return this.m_id;
			}
		}

		// Token: 0x1700140E RID: 5134
		// (get) Token: 0x06005636 RID: 22070 RVA: 0x00079817 File Offset: 0x00077A17
		DeliveryParams ICombatEffectSource.DeliveryParams
		{
			get
			{
				return this.m_delivery;
			}
		}

		// Token: 0x06005637 RID: 22071 RVA: 0x0007981F File Offset: 0x00077A1F
		TargetingParams ICombatEffectSource.GetTargetingParams(float level, AlchemyPowerLevel alchemyPowerLevel)
		{
			return base.GetTargetingParams(level, alchemyPowerLevel);
		}

		// Token: 0x06005638 RID: 22072 RVA: 0x00079829 File Offset: 0x00077A29
		KinematicParameters ICombatEffectSource.GetKinematicParams(float level, AlchemyPowerLevel alchemyPowerLevel)
		{
			return base.GetTieredAbilityParameter(TieredAbilityParameterType.Kinematic, level, alchemyPowerLevel).KinematicParams;
		}

		// Token: 0x06005639 RID: 22073 RVA: 0x00079839 File Offset: 0x00077A39
		CombatEffect ICombatEffectSource.GetCombatEffect(float level, AlchemyPowerLevel alchemyPowerLevel)
		{
			return base.GetCombatEffect(level, alchemyPowerLevel);
		}

		// Token: 0x0600563A RID: 22074 RVA: 0x00079843 File Offset: 0x00077A43
		protected override void FillTooltipTieredParams(GameEntity entity, float level)
		{
			base.FillTooltipTieredParams(entity, level);
			AbilityArchetype.tt_targetingParams = base.GetTargetingParams(level, AbilityArchetype.tt_alchemyPowerLevel);
			AbilityArchetype.tt_kinematicParams = base.GetTieredAbilityParameter(TieredAbilityParameterType.Kinematic, level).KinematicParams;
			AbilityArchetype.tt_combatEffect = base.GetCombatEffect(level, AbilityArchetype.tt_alchemyPowerLevel);
		}

		// Token: 0x0600563B RID: 22075 RVA: 0x001E0A38 File Offset: 0x001DEC38
		protected override void FillTooltipBlocksInternal(ArchetypeTooltip tooltip, ArchetypeInstance instance, GameEntity entity, int masteryLevel, int abilityLevel)
		{
			base.FillTooltipBlocksInternal(tooltip, instance, entity, masteryLevel, abilityLevel);
			CombatEffectExtensions.FillTooltipEffectsBlock(base.Id, AbilityArchetype.tt_combatEffect, AbilityArchetype.tt_targetingParams, AbilityArchetype.tt_reagentItem, tooltip, entity, true, null);
			TargetingParams tt_targetingParams = AbilityArchetype.tt_targetingParams;
			if (tt_targetingParams != null)
			{
				tt_targetingParams.FillTooltipTargetingBlock(tooltip, entity);
			}
			if (this.m_requireWeapons)
			{
				CombatMasteryArchetype combatMasteryArchetype;
				ArchetypeInstance archetypeInstance;
				CombatMasteryArchetype combatMasteryArchetype2;
				if (!base.Mastery.TryGetAsType(out combatMasteryArchetype) || !entity.TryGetActiveWeaponMasteryAsType(out archetypeInstance, out combatMasteryArchetype2))
				{
					return;
				}
				TooltipTextBlock requirementsBlock = tooltip.RequirementsBlock;
				string txt;
				if (combatMasteryArchetype2.Id == combatMasteryArchetype.Id && (!combatMasteryArchetype.HasCompatibleWeapons(entity, out txt) || UIManager.TooltipShowMore))
				{
					requirementsBlock.AppendLine(txt, 0);
				}
			}
		}

		// Token: 0x0600563C RID: 22076 RVA: 0x00079881 File Offset: 0x00077A81
		protected override bool TryGetTargetDistanceAngleForTooltip(GameEntity entity, out string distance, out string angle)
		{
			distance = string.Empty;
			angle = string.Empty;
			return AbilityArchetype.tt_targetingParams != null && AbilityArchetype.tt_targetingParams.TryGetTargetDistanceAngleForTooltip(entity, out distance, out angle);
		}

		// Token: 0x04004C66 RID: 19558
		[SerializeField]
		private DeliveryParams m_delivery;

		// Token: 0x04004C67 RID: 19559
		[SerializeField]
		private bool m_requireWeapons;

		// Token: 0x04004C68 RID: 19560
		[Tooltip("This classifies the combat text as WarlordSong for filtering purposes")]
		[SerializeField]
		private bool m_isWarlordSong;

		// Token: 0x04004C69 RID: 19561
		[SerializeField]
		protected bool m_bypassAmmoRequirement;

		// Token: 0x04004C6A RID: 19562
		[SerializeField]
		private bool m_useAutoAttackAnimation;

		// Token: 0x04004C6B RID: 19563
		[SerializeField]
		protected AnimationIndexProfile m_animationIndexProfile;

		// Token: 0x04004C6C RID: 19564
		[SerializeField]
		protected bool m_useStanceAnimationIndex;

		// Token: 0x04004C6D RID: 19565
		[SerializeField]
		protected AnimationExecutionTime m_exeTime;

		// Token: 0x04004C6E RID: 19566
		[Range(0f, 6f)]
		[SerializeField]
		protected int m_animationIndex;

		// Token: 0x04004C6F RID: 19567
		[SerializeField]
		private DummyClass m_animDescriptionDummy;
	}
}

using System;
using SoL.Game.EffectSystem;
using SoL.Managers;
using SoL.UI;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;
using UnityEngine.Serialization;

namespace SoL.Game.Objects.Archetypes.Abilities
{
	// Token: 0x02000AED RID: 2797
	[CreateAssetMenu(menuName = "SoL/Objects/Itemization/Skills/Abilities/Aura Ability")]
	public class AuraAbility : AbilityArchetype, ICombatEffectSource
	{
		// Token: 0x1700140F RID: 5135
		// (get) Token: 0x0600563E RID: 22078 RVA: 0x00045BCA File Offset: 0x00043DCA
		public override bool ShowProgress
		{
			get
			{
				return false;
			}
		}

		// Token: 0x0600563F RID: 22079 RVA: 0x001E0A8C File Offset: 0x001DEC8C
		public override int GetStartingLevel()
		{
			return base.LevelRange.Max;
		}

		// Token: 0x17001410 RID: 5136
		// (get) Token: 0x06005640 RID: 22080 RVA: 0x00045BCA File Offset: 0x00043DCA
		public override bool ConsiderHaste
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17001411 RID: 5137
		// (get) Token: 0x06005641 RID: 22081 RVA: 0x00045BCA File Offset: 0x00043DCA
		public override bool AllowAlchemy
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17001412 RID: 5138
		// (get) Token: 0x06005642 RID: 22082 RVA: 0x000798AF File Offset: 0x00077AAF
		public CombatEffect CombatEffect
		{
			get
			{
				return this.m_effect;
			}
		}

		// Token: 0x17001413 RID: 5139
		// (get) Token: 0x06005643 RID: 22083 RVA: 0x00074515 File Offset: 0x00072715
		UniqueId ICombatEffectSource.ArchetypeId
		{
			get
			{
				return this.m_id;
			}
		}

		// Token: 0x17001414 RID: 5140
		// (get) Token: 0x06005644 RID: 22084 RVA: 0x00049FFA File Offset: 0x000481FA
		DeliveryParams ICombatEffectSource.DeliveryParams
		{
			get
			{
				return null;
			}
		}

		// Token: 0x06005645 RID: 22085 RVA: 0x00049FFA File Offset: 0x000481FA
		TargetingParams ICombatEffectSource.GetTargetingParams(float level, AlchemyPowerLevel alchemyPowerLevel)
		{
			return null;
		}

		// Token: 0x06005646 RID: 22086 RVA: 0x00049FFA File Offset: 0x000481FA
		KinematicParameters ICombatEffectSource.GetKinematicParams(float level, AlchemyPowerLevel alchemyPowerLevel)
		{
			return null;
		}

		// Token: 0x06005647 RID: 22087 RVA: 0x000798AF File Offset: 0x00077AAF
		CombatEffect ICombatEffectSource.GetCombatEffect(float level, AlchemyPowerLevel alchemyPowerLevel)
		{
			return this.m_effect;
		}

		// Token: 0x17001415 RID: 5141
		// (get) Token: 0x06005648 RID: 22088 RVA: 0x000798B7 File Offset: 0x00077AB7
		public AudioClip LoopingClip
		{
			get
			{
				return this.m_loopingClip;
			}
		}

		// Token: 0x17001416 RID: 5142
		// (get) Token: 0x06005649 RID: 22089 RVA: 0x000798BF File Offset: 0x00077ABF
		public bool SilenceMusic
		{
			get
			{
				return this.m_silenceMusic;
			}
		}

		// Token: 0x17001417 RID: 5143
		// (get) Token: 0x0600564A RID: 22090 RVA: 0x000798C7 File Offset: 0x00077AC7
		public float ClipVolume
		{
			get
			{
				return this.m_clipVolume;
			}
		}

		// Token: 0x17001418 RID: 5144
		// (get) Token: 0x0600564B RID: 22091 RVA: 0x000798CF File Offset: 0x00077ACF
		private bool m_showAudioFields
		{
			get
			{
				return this.m_loopingClip != null;
			}
		}

		// Token: 0x17001419 RID: 5145
		// (get) Token: 0x0600564C RID: 22092 RVA: 0x000798DD File Offset: 0x00077ADD
		public bool IsCombatAura
		{
			get
			{
				return base.RequireCombatStance;
			}
		}

		// Token: 0x0600564D RID: 22093 RVA: 0x001E0AA8 File Offset: 0x001DECA8
		protected override bool ExecutionCheck(ExecutionCache executionCache, float executionProgress)
		{
			if (executionProgress <= 0f && !GameManager.IsServer && LocalPlayer.ActiveAura != null && LocalPlayer.ActiveAura.Value.ArchetypeId == this.m_id)
			{
				executionCache.SourceEntity.NetworkEntity.PlayerRpcHandler.Client_DismissActiveAura();
				return false;
			}
			if (!base.ExecutionCheck(executionCache, executionProgress))
			{
				return false;
			}
			if (this.IsCombatAura)
			{
				if (this.m_mastery.Id != executionCache.SourceEntity.CharacterData.ActiveMasteryId || executionCache.SourceEntity.Vitals.Stance != Stance.Combat)
				{
					executionCache.Message = "Invalid stance!";
					return false;
				}
			}
			else if (executionCache.SourceEntity.Vitals.Stance == Stance.Combat)
			{
				executionCache.Message = "Invalid stance!";
				return false;
			}
			if (!this.MeetsWeaponRequirements(executionCache.SourceEntity, executionCache))
			{
				executionCache.Message = "Invalid weapons!";
				return false;
			}
			executionCache.SetTargetNetworkEntity(executionCache.SourceEntity.NetworkEntity);
			return true;
		}

		// Token: 0x0600564E RID: 22094 RVA: 0x001E0BB4 File Offset: 0x001DEDB4
		protected override void PostExecution(ExecutionCache executionCache)
		{
			base.PostExecution(executionCache);
			if (GameManager.IsServer)
			{
				AuraController fromPool = StaticPool<AuraController>.GetFromPool();
				fromPool.InitAura(executionCache.SourceEntity, this, executionCache.MasteryInstance.GetAssociatedLevelInteger(executionCache.SourceEntity));
				executionCache.SourceEntity.EffectController.SourceAura = fromPool;
			}
		}

		// Token: 0x0600564F RID: 22095 RVA: 0x000798E5 File Offset: 0x00077AE5
		protected override bool InRequiredStance(GameEntity entity)
		{
			if (!this.IsCombatAura)
			{
				return entity.Vitals.Stance != Stance.Combat;
			}
			return entity.Vitals.Stance == Stance.Combat;
		}

		// Token: 0x06005650 RID: 22096 RVA: 0x0007990F File Offset: 0x00077B0F
		protected override bool MeetsRequirementsForUI(GameEntity entity, float level)
		{
			return base.MeetsRequirementsForUI(entity, level) && this.MeetsWeaponRequirements(entity, null) && (!this.IsCombatAura || this.m_mastery.Id == entity.CharacterData.ActiveMasteryId);
		}

		// Token: 0x06005651 RID: 22097 RVA: 0x001E0C04 File Offset: 0x001DEE04
		private bool MeetsWeaponRequirements(GameEntity entity, IHandHeldItems handHeldItems)
		{
			if (!this.m_requiresAccessory)
			{
				return true;
			}
			WeaponItem weaponItem = null;
			if (handHeldItems != null)
			{
				weaponItem = handHeldItems.OffHand.WeaponItem;
			}
			else
			{
				ArchetypeInstance archetypeInstance;
				entity.TryGetHandheldItem_OffHandAsType(out archetypeInstance, out weaponItem);
			}
			return weaponItem && weaponItem.GetWeaponType() == WeaponTypes.OffhandAccessory;
		}

		// Token: 0x06005652 RID: 22098 RVA: 0x001E0C50 File Offset: 0x001DEE50
		protected override void FillTooltipBlocksInternal(ArchetypeTooltip tooltip, ArchetypeInstance instance, GameEntity entity, int masteryLevel, int abilityLevel)
		{
			base.FillTooltipBlocksInternal(tooltip, instance, entity, masteryLevel, abilityLevel);
			CombatEffectExtensions.FillTooltipEffectsBlock(this, tooltip, entity, (float)masteryLevel);
			TooltipTextBlock requirementsBlock = tooltip.RequirementsBlock;
			if (this.m_requiresAccessory && (!this.MeetsWeaponRequirements(entity, null) || UIManager.TooltipShowMore))
			{
				Color color = this.MeetsWeaponRequirements(entity, null) ? UIManager.RequirementsMetColor : UIManager.RequirementsNotMetColor;
				requirementsBlock.AppendLine("Accessory".Color(color), 0);
			}
		}

		// Token: 0x06005653 RID: 22099 RVA: 0x0007994C File Offset: 0x00077B4C
		public override GameObject GetInstanceUIPrefabReference()
		{
			return ClientGameManager.UIManager.AuraAbilityInstanceUIPrefab;
		}

		// Token: 0x04004C70 RID: 19568
		[SerializeField]
		private CombatEffect m_effect;

		// Token: 0x04004C71 RID: 19569
		[FormerlySerializedAs("m_requiresBanner")]
		[SerializeField]
		private bool m_requiresAccessory;

		// Token: 0x04004C72 RID: 19570
		[SerializeField]
		private AudioClip m_loopingClip;

		// Token: 0x04004C73 RID: 19571
		[SerializeField]
		private float m_clipVolume = 0.5f;

		// Token: 0x04004C74 RID: 19572
		[SerializeField]
		private bool m_silenceMusic;
	}
}

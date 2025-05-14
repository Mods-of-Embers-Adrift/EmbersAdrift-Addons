using System;
using SoL.Game.Animation;
using SoL.Game.EffectSystem;
using SoL.Game.Settings;
using SoL.Managers;
using SoL.UI;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes.Abilities
{
	// Token: 0x02000AEE RID: 2798
	[CreateAssetMenu(menuName = "SoL/Objects/Itemization/Skills/Abilities/Auto Attack")]
	public class AutoAttackAbility : DynamicAbility, ICombatEffectSource
	{
		// Token: 0x1700141A RID: 5146
		// (get) Token: 0x06005655 RID: 22101 RVA: 0x0004479C File Offset: 0x0004299C
		protected override bool CreateInstanceUI
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06005656 RID: 22102 RVA: 0x0007996B File Offset: 0x00077B6B
		public override string GetInstanceId()
		{
			return "AUTOATTACK";
		}

		// Token: 0x1700141B RID: 5147
		// (get) Token: 0x06005657 RID: 22103 RVA: 0x001E0D1C File Offset: 0x001DEF1C
		public override Sprite Icon
		{
			get
			{
				GameEntity gameEntity = LocalPlayer.GameEntity;
				ArchetypeInstance archetypeInstance;
				IHandheldItem handheldItem = (gameEntity != null) ? gameEntity.GetHandheldItem_MainHand(out archetypeInstance) : null;
				if (handheldItem == null)
				{
					return null;
				}
				return handheldItem.Icon;
			}
		}

		// Token: 0x1700141C RID: 5148
		// (get) Token: 0x06005658 RID: 22104 RVA: 0x0004479C File Offset: 0x0004299C
		protected override bool PauseWhileHandSwapActive
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700141D RID: 5149
		// (get) Token: 0x06005659 RID: 22105 RVA: 0x00045BCA File Offset: 0x00043DCA
		protected override bool PauseWhileExecuting
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700141E RID: 5150
		// (get) Token: 0x0600565A RID: 22106 RVA: 0x0004479C File Offset: 0x0004299C
		public override bool ConsiderHaste
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700141F RID: 5151
		// (get) Token: 0x0600565B RID: 22107 RVA: 0x0004479C File Offset: 0x0004299C
		public override bool ClampHasteTo100
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600565C RID: 22108 RVA: 0x001E0D48 File Offset: 0x001DEF48
		protected override float GetCooldown(GameEntity entity, ExecutionCache executionCache = null)
		{
			WeaponItem weaponItem = null;
			if (executionCache == null)
			{
				ArchetypeInstance archetypeInstance;
				entity.TryGetHandheldItem_MainHandAsType(out archetypeInstance, out weaponItem);
			}
			else
			{
				weaponItem = executionCache.MainHand.WeaponItem;
			}
			if (!weaponItem)
			{
				weaponItem = GlobalSettings.Values.Combat.FallbackWeapon;
			}
			return (float)weaponItem.Delay;
		}

		// Token: 0x0600565D RID: 22109 RVA: 0x0006108D File Offset: 0x0005F28D
		private float GetTravelVelocity(float masteryLevel)
		{
			return 0f;
		}

		// Token: 0x0600565E RID: 22110 RVA: 0x001E0D94 File Offset: 0x001DEF94
		private float GetMasteryLevel(GameEntity entity)
		{
			ArchetypeInstance archetypeInstance;
			MasteryArchetype masteryArchetype;
			if (entity.TryGetActiveWeaponMasteryAsType(out archetypeInstance, out masteryArchetype))
			{
				return archetypeInstance.GetAssociatedLevel(entity);
			}
			return 0f;
		}

		// Token: 0x0600565F RID: 22111 RVA: 0x001E0DBC File Offset: 0x001DEFBC
		private bool MeetsWeaponRequirements(GameEntity entity, IHandHeldItems handHeldItems)
		{
			if (!entity || handHeldItems == null)
			{
				return false;
			}
			WeaponItem weaponItem = handHeldItems.MainHand.WeaponItem;
			ArchetypeInstance archetypeInstance;
			CombatMasteryArchetype combatMasteryArchetype;
			return weaponItem && (weaponItem == GlobalSettings.Values.Combat.FallbackWeapon || (weaponItem.CanExecuteWith(handHeldItems) && entity.TryGetActiveWeaponMasteryAsType(out archetypeInstance, out combatMasteryArchetype) && combatMasteryArchetype.EntityHasCompatibleWeapons(handHeldItems)));
		}

		// Token: 0x06005660 RID: 22112 RVA: 0x001E0E28 File Offset: 0x001DF028
		private bool MeetsAmmoRequirements(IHandHeldItems handHeldItems)
		{
			if (handHeldItems == null)
			{
				return false;
			}
			WeaponItem weaponItem = handHeldItems.MainHand.WeaponItem;
			return !weaponItem || weaponItem.HasProperAmmo(handHeldItems);
		}

		// Token: 0x06005661 RID: 22113 RVA: 0x00079972 File Offset: 0x00077B72
		public bool MeetsDistanceRequirements(GameEntity source, GameEntity target, IHandHeldItems handHeldItems)
		{
			return source && target && this.m_target != null && this.m_target.MeetsDistanceRequirements(source, target, handHeldItems);
		}

		// Token: 0x06005662 RID: 22114 RVA: 0x0007999C File Offset: 0x00077B9C
		public bool MeetsDistanceRequirements(GameEntity source, GameEntity target, IHandHeldItems handHeldItems, float distance)
		{
			return source && target && this.m_target != null && this.m_target.MeetsDistanceRequirements(source, target, handHeldItems, distance);
		}

		// Token: 0x06005663 RID: 22115 RVA: 0x000799C8 File Offset: 0x00077BC8
		public bool MeetsAngleRequirements(GameEntity source, GameEntity target, IHandHeldItems handHeldItems)
		{
			return source && target && this.m_target != null && this.m_target.MeetsAngleRequirements(source, target, handHeldItems);
		}

		// Token: 0x06005664 RID: 22116 RVA: 0x000799F2 File Offset: 0x00077BF2
		public bool MeetsAngleRequirements(GameEntity source, GameEntity target, IHandHeldItems handHeldItems, float angle)
		{
			return source && target && this.m_target != null && this.m_target.MeetsAngleRequirements(source, target, handHeldItems, angle);
		}

		// Token: 0x06005665 RID: 22117 RVA: 0x001E0E5C File Offset: 0x001DF05C
		public override void FillTooltipBlocks(ArchetypeTooltip tooltip, ArchetypeInstance instance, GameEntity entity)
		{
			base.FillTooltipBlocks(tooltip, instance, entity);
			ArchetypeInstance archetypeInstance;
			CombatMasteryArchetype combatMasteryArchetype;
			entity.TryGetActiveWeaponMasteryAsType(out archetypeInstance, out combatMasteryArchetype);
			WeaponItem weaponItem = entity.HandHeldItemCache.MainHand.WeaponItem;
			if (archetypeInstance == null || combatMasteryArchetype == null)
			{
				tooltip.DataBlock.AppendLine("Select a weapon stance!", 0);
				return;
			}
			if (weaponItem != null)
			{
				weaponItem.AddWeaponDataToTooltip(tooltip, true);
			}
			this.m_target.FillTooltipTargetingBlock(tooltip, entity);
			string left;
			string right;
			if (this.m_target.TryGetTargetDistanceAngleForTooltip(entity, out left, out right))
			{
				tooltip.RequirementsBlock.AppendLine(left, right);
			}
			string txt;
			if (!combatMasteryArchetype.HasCompatibleWeapons(entity, out txt) || UIManager.TooltipShowMore)
			{
				tooltip.RequirementsBlock.AppendLine(txt, 0);
			}
		}

		// Token: 0x17001420 RID: 5152
		// (get) Token: 0x06005666 RID: 22118 RVA: 0x0004479C File Offset: 0x0004299C
		protected override EffectSourceType m_effectSourceType
		{
			get
			{
				return EffectSourceType.Ability;
			}
		}

		// Token: 0x06005667 RID: 22119 RVA: 0x001E0F0C File Offset: 0x001DF10C
		protected override bool PreExecution(ExecutionCache executionCache, bool initial)
		{
			if (!base.PreExecution(executionCache, initial))
			{
				return false;
			}
			if (!initial)
			{
				return true;
			}
			if (executionCache.SourceEntity.Vitals.Stance != Stance.Combat)
			{
				executionCache.Message = "Not in combat!";
				return false;
			}
			if (!this.MeetsWeaponRequirements(executionCache.SourceEntity, executionCache))
			{
				executionCache.Message = "Incompatible weapon!";
				return false;
			}
			if (executionCache.MainHand.WeaponItem)
			{
				IDurability source;
				if (executionCache.MainHand.WeaponItem.TryGetAsType(out source) && source.IsWeaponBroken(executionCache.MainHand.Instance))
				{
					executionCache.Message = "Weapon is broken!";
					return false;
				}
				if (executionCache.MainHand.WeaponItem.RequiresAmmo)
				{
					if (!this.MeetsAmmoRequirements(executionCache))
					{
						executionCache.Message = "Invalid ammo!";
						return false;
					}
					executionCache.AddReductionTask(ReductionTaskType.Count, executionCache.OffHand.Instance, 1);
				}
			}
			float cooldown = this.GetCooldown(executionCache.SourceEntity, executionCache);
			float num = (executionCache.Instance.AbilityData.Cooldown_Base.Elapsed != null) ? executionCache.Instance.AbilityData.Cooldown_Base.Elapsed.Value : float.MaxValue;
			bool flag = num >= cooldown;
			if (!flag && GameManager.IsServer)
			{
				flag = (Mathf.Abs(cooldown - num) < 0.1f);
			}
			if (!flag)
			{
				executionCache.Message = "Cooldown not met!";
				return false;
			}
			if (executionCache.SourceEntity.VitalsReplicator.BehaviorFlags.Value.HasBitFlag(BehaviorEffectTypeFlags.Stunned))
			{
				executionCache.Message = executionCache.SourceEntity.VitalsReplicator.BehaviorFlags.Value.CancelExecutionForFlagDescription();
				return false;
			}
			return this.m_target.ExecutionCheck(executionCache, 0f);
		}

		// Token: 0x06005668 RID: 22120 RVA: 0x00079A1E File Offset: 0x00077C1E
		protected override void PostExecution(ExecutionCache executionCache)
		{
			ArchetypeInstance instance = executionCache.Instance;
			if (instance != null)
			{
				AbilityInstanceData abilityData = instance.AbilityData;
				if (abilityData != null)
				{
					abilityData.AbilityExecuted(AlchemyPowerLevel.None);
				}
			}
			executionCache.PerformReduction();
		}

		// Token: 0x06005669 RID: 22121 RVA: 0x0005BFF3 File Offset: 0x0005A1F3
		protected override bool TryGetAbilityAnimation(GameEntity entity, out AbilityAnimation animation)
		{
			animation = null;
			return false;
		}

		// Token: 0x0600566A RID: 22122 RVA: 0x001E10C0 File Offset: 0x001DF2C0
		protected override bool MeetsRequirementsForUI(GameEntity entity)
		{
			return entity && entity.Vitals && entity.Vitals.Stance == Stance.Combat && this.MeetsWeaponRequirements(entity, entity.HandHeldItemCache) && this.MeetsAmmoRequirements(entity.HandHeldItemCache);
		}

		// Token: 0x0600566B RID: 22123 RVA: 0x001E1110 File Offset: 0x001DF310
		protected override bool TryGetMastery(GameEntity entity, out MasteryArchetype mastery)
		{
			ArchetypeInstance archetypeInstance;
			return entity.TryGetActiveWeaponMasteryAsType(out archetypeInstance, out mastery);
		}

		// Token: 0x17001421 RID: 5153
		// (get) Token: 0x0600566C RID: 22124 RVA: 0x0006108D File Offset: 0x0005F28D
		protected override float m_masteryCreditFactor
		{
			get
			{
				return 0f;
			}
		}

		// Token: 0x17001422 RID: 5154
		// (get) Token: 0x0600566D RID: 22125 RVA: 0x00045BCA File Offset: 0x00043DCA
		protected override bool m_addGroupBonus
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17001423 RID: 5155
		// (get) Token: 0x0600566E RID: 22126 RVA: 0x00079A43 File Offset: 0x00077C43
		public TargetingParamsSpatial Targeting
		{
			get
			{
				return this.m_target;
			}
		}

		// Token: 0x17001424 RID: 5156
		// (get) Token: 0x0600566F RID: 22127 RVA: 0x00074515 File Offset: 0x00072715
		UniqueId ICombatEffectSource.ArchetypeId
		{
			get
			{
				return this.m_id;
			}
		}

		// Token: 0x17001425 RID: 5157
		// (get) Token: 0x06005670 RID: 22128 RVA: 0x00079A4B File Offset: 0x00077C4B
		DeliveryParams ICombatEffectSource.DeliveryParams
		{
			get
			{
				return this.m_delivery;
			}
		}

		// Token: 0x06005671 RID: 22129 RVA: 0x00079A43 File Offset: 0x00077C43
		TargetingParams ICombatEffectSource.GetTargetingParams(float level, AlchemyPowerLevel alchemyPowerLevel)
		{
			return this.m_target;
		}

		// Token: 0x06005672 RID: 22130 RVA: 0x00049FFA File Offset: 0x000481FA
		KinematicParameters ICombatEffectSource.GetKinematicParams(float level, AlchemyPowerLevel alchemyPowerLevel)
		{
			return null;
		}

		// Token: 0x06005673 RID: 22131 RVA: 0x00079A53 File Offset: 0x00077C53
		CombatEffect ICombatEffectSource.GetCombatEffect(float level, AlchemyPowerLevel alchemyPowerLevel)
		{
			return this.m_effect;
		}

		// Token: 0x06005674 RID: 22132 RVA: 0x00079A5B File Offset: 0x00077C5B
		public override GameObject GetInstanceUIPrefabReference()
		{
			return ClientGameManager.UIManager.AutoAttackInstanceUIPrefab;
		}

		// Token: 0x04004C75 RID: 19573
		public const string kInstanceId = "AUTOATTACK";

		// Token: 0x04004C76 RID: 19574
		private static MinMaxIntRange kLevelRange = new MinMaxIntRange(1, 1);

		// Token: 0x04004C77 RID: 19575
		[SerializeField]
		private TargetingParamsSpatial m_target;

		// Token: 0x04004C78 RID: 19576
		[SerializeField]
		private DeliveryParams m_delivery;

		// Token: 0x04004C79 RID: 19577
		[SerializeField]
		private CombatEffect m_effect;
	}
}

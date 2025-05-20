using System;
using System.Collections;
using Cysharp.Text;
using SoL.Game.Animation;
using SoL.Game.EffectSystem;
using SoL.UI;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A5D RID: 2653
	public abstract class ConsumableItem : ItemArchetype, IExecutable, IVfxSource
	{
		// Token: 0x17001291 RID: 4753
		// (get) Token: 0x06005225 RID: 21029 RVA: 0x00076D27 File Offset: 0x00074F27
		public LevelRequirement LevelRequirement
		{
			get
			{
				return this.m_levelRequirement;
			}
		}

		// Token: 0x17001292 RID: 4754
		// (get) Token: 0x06005226 RID: 21030 RVA: 0x00076D2F File Offset: 0x00074F2F
		private bool m_showVfx
		{
			get
			{
				return this.m_vfxOverride == null;
			}
		}

		// Token: 0x17001293 RID: 4755
		// (get) Token: 0x06005227 RID: 21031 RVA: 0x00076D3D File Offset: 0x00074F3D
		private bool m_showAnimation
		{
			get
			{
				return this.m_animationOverride == null;
			}
		}

		// Token: 0x17001294 RID: 4756
		// (get) Token: 0x06005228 RID: 21032 RVA: 0x00075825 File Offset: 0x00073A25
		private IEnumerable GetVfxOverrides
		{
			get
			{
				return SolOdinUtilities.GetDropdownItems<AbilityVFXScriptable>();
			}
		}

		// Token: 0x17001295 RID: 4757
		// (get) Token: 0x06005229 RID: 21033 RVA: 0x00076D4B File Offset: 0x00074F4B
		private IEnumerable GetAnimationOverrides
		{
			get
			{
				return SolOdinUtilities.GetDropdownItems<AbilityAnimationScriptable>();
			}
		}

		// Token: 0x17001296 RID: 4758
		// (get) Token: 0x0600522A RID: 21034 RVA: 0x00076D52 File Offset: 0x00074F52
		public ConsumableCategory ConsumableCategory
		{
			get
			{
				return this.m_category;
			}
		}

		// Token: 0x17001297 RID: 4759
		// (get) Token: 0x0600522B RID: 21035 RVA: 0x00076D5A File Offset: 0x00074F5A
		public int Cooldown
		{
			get
			{
				return this.m_execution.Cooldown;
			}
		}

		// Token: 0x17001298 RID: 4760
		// (get) Token: 0x0600522C RID: 21036 RVA: 0x0004479C File Offset: 0x0004299C
		public override bool CanPlaceInPouch
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17001299 RID: 4761
		// (get) Token: 0x0600522D RID: 21037
		protected abstract ReductionTaskType m_reductionTaskType { get; }

		// Token: 0x1700129A RID: 4762
		// (get) Token: 0x0600522E RID: 21038 RVA: 0x00045BCA File Offset: 0x00043DCA
		protected virtual bool IsLearning
		{
			get
			{
				return false;
			}
		}

		// Token: 0x0600522F RID: 21039 RVA: 0x00076D67 File Offset: 0x00074F67
		protected virtual void PostExecution(ExecutionCache executionCache)
		{
			executionCache.SourceEntity.SkillsController.MarkConsumableUsed(this.m_category);
			executionCache.PerformReduction();
		}

		// Token: 0x06005230 RID: 21040 RVA: 0x001D3204 File Offset: 0x001D1404
		private bool ExecutionCheck(ExecutionCache executionCache, bool initial)
		{
			if (executionCache.SourceEntity.Vitals.GetCurrentHealthState() != HealthState.Alive)
			{
				executionCache.Message = "Not awake!";
				return false;
			}
			if (this.m_levelRequirement != null && !this.m_levelRequirement.MeetsAllRequirements(executionCache.SourceEntity))
			{
				executionCache.Message = "Do not meet level requirements!";
				return false;
			}
			executionCache.SetTargetNetworkEntity(executionCache.SourceEntity.NetworkEntity);
			if (executionCache.SourceEntity.SkillsController.GetElapsedSinceLastConsumable(this.m_category) < (float)this.m_execution.Cooldown)
			{
				executionCache.Message = "Cooldown not met!";
				return false;
			}
			if (!this.m_execution.ExecutionCheck(executionCache, initial))
			{
				return false;
			}
			if (!this.ExecutionCheckInternal(executionCache))
			{
				return false;
			}
			if (executionCache.SourceEntity.VitalsReplicator.BehaviorFlags.Value.CancelExecutionForFlag())
			{
				executionCache.Message = executionCache.SourceEntity.VitalsReplicator.BehaviorFlags.Value.CancelExecutionForFlagDescription();
				return false;
			}
			if (executionCache.TargetNetworkEntity.GameEntity.VitalsReplicator)
			{
				for (int i = 0; i < executionCache.TargetNetworkEntity.GameEntity.VitalsReplicator.Effects.Count; i++)
				{
					EffectSyncData effectSyncData = executionCache.TargetNetworkEntity.GameEntity.VitalsReplicator.Effects[i];
					ConsumableItemStackable consumableItemStackable;
					if (InternalGameDatabase.Archetypes.TryGetAsType<ConsumableItemStackable>(effectSyncData.ArchetypeId, out consumableItemStackable) && (consumableItemStackable.ConsumableCategory & this.m_category) > ConsumableCategory.None)
					{
						executionCache.Message = "Would not take hold!";
						return false;
					}
				}
			}
			if (initial)
			{
				executionCache.AddReductionTask(this.m_reductionTaskType, executionCache.Instance, 1);
			}
			return true;
		}

		// Token: 0x06005231 RID: 21041 RVA: 0x0004479C File Offset: 0x0004299C
		protected virtual bool ExecutionCheckInternal(ExecutionCache executionCache)
		{
			return true;
		}

		// Token: 0x1700129B RID: 4763
		// (get) Token: 0x06005232 RID: 21042 RVA: 0x000759FE File Offset: 0x00073BFE
		string IExecutable.DisplayName
		{
			get
			{
				return this.DisplayName;
			}
		}

		// Token: 0x1700129C RID: 4764
		// (get) Token: 0x06005233 RID: 21043 RVA: 0x000580DD File Offset: 0x000562DD
		EffectSourceType IExecutable.Type
		{
			get
			{
				return EffectSourceType.Consumable;
			}
		}

		// Token: 0x06005234 RID: 21044 RVA: 0x00076D85 File Offset: 0x00074F85
		bool IExecutable.PreExecution(ExecutionCache executionCache)
		{
			return this.ExecutionCheck(executionCache, true);
		}

		// Token: 0x06005235 RID: 21045 RVA: 0x00076D8F File Offset: 0x00074F8F
		bool IExecutable.ContinuedExecution(ExecutionCache executionCache, float executionProgress)
		{
			return this.ExecutionCheck(executionCache, false);
		}

		// Token: 0x06005236 RID: 21046 RVA: 0x00076D99 File Offset: 0x00074F99
		void IExecutable.PostExecution(ExecutionCache executionCache)
		{
			this.PostExecution(executionCache);
		}

		// Token: 0x06005237 RID: 21047 RVA: 0x00045BCA File Offset: 0x00043DCA
		bool IExecutable.UseAutoAttackAnimation()
		{
			return false;
		}

		// Token: 0x06005238 RID: 21048 RVA: 0x00076DA2 File Offset: 0x00074FA2
		bool IExecutable.TryGetAbilityAnimation(GameEntity entity, out AbilityAnimation animation)
		{
			animation = ((this.m_animationOverride && this.m_animationOverride.Animation != null) ? this.m_animationOverride.Animation : this.m_animation);
			return animation != null;
		}

		// Token: 0x1700129D RID: 4765
		// (get) Token: 0x06005239 RID: 21049 RVA: 0x0004479C File Offset: 0x0004299C
		bool IExecutable.TriggerGlobalCooldown
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700129E RID: 4766
		// (get) Token: 0x0600523A RID: 21050 RVA: 0x0004479C File Offset: 0x0004299C
		bool IExecutable.AllowStaminaRegenDuringExecution
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700129F RID: 4767
		// (get) Token: 0x0600523B RID: 21051 RVA: 0x00045BCA File Offset: 0x00043DCA
		bool IExecutable.AllowAlchemy
		{
			get
			{
				return false;
			}
		}

		// Token: 0x0600523C RID: 21052 RVA: 0x00076DD8 File Offset: 0x00074FD8
		bool IExecutable.MeetsRequirementsForUI(GameEntity entity, float level)
		{
			return entity && entity.Vitals && entity.Vitals.Stance.CanExecute(this.m_execution.ValidStances);
		}

		// Token: 0x0600523D RID: 21053 RVA: 0x00076E0C File Offset: 0x0007500C
		ExecutionParams IExecutable.GetExecutionParams(float level, AlchemyPowerLevel alchemyPowerLevel)
		{
			return this.m_execution;
		}

		// Token: 0x0600523E RID: 21054 RVA: 0x00045BCA File Offset: 0x00043DCA
		bool IExecutable.HasValidAlchemyOverride(float level, AlchemyPowerLevel alchemyPowerLevel)
		{
			return false;
		}

		// Token: 0x170012A0 RID: 4768
		// (get) Token: 0x0600523F RID: 21055 RVA: 0x001D3398 File Offset: 0x001D1598
		float? IExecutable.DeferHandIkDuration
		{
			get
			{
				if (!this.m_deferHandIk)
				{
					return null;
				}
				return new float?(this.m_deferHandIkDuration);
			}
		}

		// Token: 0x170012A1 RID: 4769
		// (get) Token: 0x06005240 RID: 21056 RVA: 0x00076E14 File Offset: 0x00075014
		bool IExecutable.IsLearning
		{
			get
			{
				return this.IsLearning;
			}
		}

		// Token: 0x170012A2 RID: 4770
		// (get) Token: 0x06005241 RID: 21057 RVA: 0x00076E1C File Offset: 0x0007501C
		StanceFlags IExecutable.ValidStances
		{
			get
			{
				if (this.m_execution == null)
				{
					return StanceFlags.None;
				}
				return this.m_execution.ValidStances;
			}
		}

		// Token: 0x170012A3 RID: 4771
		// (get) Token: 0x06005242 RID: 21058 RVA: 0x00076E33 File Offset: 0x00075033
		AutoAttackStateChange IExecutable.AutoAttackState
		{
			get
			{
				if (this.m_execution == null)
				{
					return AutoAttackStateChange.DoNothing;
				}
				return this.m_execution.AutoAttackState;
			}
		}

		// Token: 0x170012A4 RID: 4772
		// (get) Token: 0x06005243 RID: 21059 RVA: 0x00076E4A File Offset: 0x0007504A
		bool IExecutable.PreventRotation
		{
			get
			{
				return this.m_execution != null && this.m_execution.PreventRotation;
			}
		}

		// Token: 0x170012A5 RID: 4773
		// (get) Token: 0x06005244 RID: 21060 RVA: 0x00076E61 File Offset: 0x00075061
		float IExecutable.ExecutionTime
		{
			get
			{
				if (this.m_execution == null)
				{
					return 10f;
				}
				return this.m_execution.ExecutionTime;
			}
		}

		// Token: 0x06005245 RID: 21061 RVA: 0x00076E7C File Offset: 0x0007507C
		bool IVfxSource.TryGetEffects(int abilityLevel, AlchemyPowerLevel alchemyPowerLevel, bool isSecondary, out AbilityVFX effects)
		{
			effects = (this.m_vfxOverride ? this.m_vfxOverride.VFX : this.m_vfx);
			return effects != null;
		}

		// Token: 0x06005246 RID: 21062 RVA: 0x001D33C4 File Offset: 0x001D15C4
		public override void FillTooltipBlocks(ArchetypeTooltip tooltip, ArchetypeInstance instance, GameEntity entity)
		{
			base.FillTooltipBlocks(tooltip, instance, entity);
			this.m_execution.FillTooltip(tooltip);
			tooltip.DataBlock.AppendLine("Category:", this.m_category.ToString());
			float value = (float)this.m_execution.Cooldown;
			float elapsedSinceLastConsumable = entity.SkillsController.GetElapsedSinceLastConsumable(this.m_category);
			if (elapsedSinceLastConsumable < (float)this.m_execution.Cooldown)
			{
				value = (float)this.m_execution.Cooldown - elapsedSinceLastConsumable;
			}
			string left = ZString.Format<string>("<b>{0}</b> Execution", this.m_execution.ExecutionTime.GetFormattedTime(false));
			string right = ZString.Format<string>("<b>{0}</b> Cooldown", value.GetFormattedTime(true));
			tooltip.DataBlock.AppendLine(left, right);
			string txt;
			if (this.m_execution.TryGetValidMovementTooltip(out txt))
			{
				tooltip.DataBlock.AppendLine(txt, 0);
			}
			TooltipExtensions.AddRoleLevelRequirement(tooltip, instance, entity, this.m_levelRequirement, null);
			this.FillTooltipDistanceAngle(tooltip, entity);
			string txt2;
			if (this.m_execution.TryGetValidStanceTooltip(entity, out txt2))
			{
				tooltip.ReagentBlock.Title = "Stances";
				tooltip.ReagentBlock.AppendLine(txt2, 0);
			}
		}

		// Token: 0x06005247 RID: 21063 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void FillTooltipDistanceAngle(ArchetypeTooltip tooltip, GameEntity entity)
		{
		}

		// Token: 0x06005248 RID: 21064 RVA: 0x00076EA7 File Offset: 0x000750A7
		public override bool IsAssignerHandled(ComponentEffectAssignerName assignerName)
		{
			return assignerName - ComponentEffectAssignerName.ExecutionTime <= 2 || base.IsAssignerHandled(assignerName);
		}

		// Token: 0x06005249 RID: 21065 RVA: 0x001D34E4 File Offset: 0x001D16E4
		public override bool PopulateDynamicValue(ComponentEffectAssignerName assignerName, float value, ComponentEffectOutputType type, MinMaxFloatRange? rangeOverride)
		{
			switch (assignerName)
			{
			case ComponentEffectAssignerName.ExecutionTime:
				this.m_executionTime = ComponentEffectAssigners.Apply(assignerName, value, type, rangeOverride, this.m_executionTime);
				return true;
			case ComponentEffectAssignerName.MovementModifier:
				this.m_movementModifier = ComponentEffectAssigners.Apply(assignerName, value, type, rangeOverride, this.m_movementModifier);
				return true;
			case ComponentEffectAssignerName.Cooldown:
				this.m_cooldown = ComponentEffectAssigners.Apply(assignerName, value, type, rangeOverride, this.m_cooldown);
				return true;
			default:
				return base.PopulateDynamicValue(assignerName, value, type, rangeOverride);
			}
		}

		// Token: 0x04004993 RID: 18835
		[SerializeField]
		protected AbilityVFXScriptable m_vfxOverride;

		// Token: 0x04004994 RID: 18836
		[SerializeField]
		protected AbilityVFX m_vfx;

		// Token: 0x04004995 RID: 18837
		[SerializeField]
		protected AbilityAnimationScriptable m_animationOverride;

		// Token: 0x04004996 RID: 18838
		[SerializeField]
		protected AbilityAnimation m_animation;

		// Token: 0x04004997 RID: 18839
		[SerializeField]
		private bool m_deferHandIk;

		// Token: 0x04004998 RID: 18840
		[SerializeField]
		private float m_deferHandIkDuration;

		// Token: 0x04004999 RID: 18841
		[SerializeField]
		private LevelRequirement m_levelRequirement;

		// Token: 0x0400499A RID: 18842
		[SerializeField]
		protected ExecutionParams m_execution;

		// Token: 0x0400499B RID: 18843
		[SerializeField]
		private ConsumableCategory m_category;

		// Token: 0x0400499C RID: 18844
		[SerializeField]
		private float m_executionTime = 10f;

		// Token: 0x0400499D RID: 18845
		[SerializeField]
		private float m_movementModifier = 1f;

		// Token: 0x0400499E RID: 18846
		[SerializeField]
		private float m_cooldown = 10f;
	}
}

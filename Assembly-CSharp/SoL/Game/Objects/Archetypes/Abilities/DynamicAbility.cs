using System;
using SoL.Game.Animation;
using SoL.Game.EffectSystem;
using SoL.Game.Objects.Containers;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes.Abilities
{
	// Token: 0x02000AF1 RID: 2801
	public abstract class DynamicAbility : BaseArchetype, IExecutable, IVfxSource, IMasterySource, IAbilityCooldown
	{
		// Token: 0x17001427 RID: 5159
		// (get) Token: 0x06005681 RID: 22145 RVA: 0x00053500 File Offset: 0x00051700
		public override ArchetypeCategory Category
		{
			get
			{
				return ArchetypeCategory.Ability;
			}
		}

		// Token: 0x17001428 RID: 5160
		// (get) Token: 0x06005682 RID: 22146 RVA: 0x0006ADA7 File Offset: 0x00068FA7
		protected virtual EffectSourceType m_effectSourceType
		{
			get
			{
				return EffectSourceType.Dynamic;
			}
		}

		// Token: 0x06005683 RID: 22147
		public abstract string GetInstanceId();

		// Token: 0x17001429 RID: 5161
		// (get) Token: 0x06005684 RID: 22148
		protected abstract bool CreateInstanceUI { get; }

		// Token: 0x1700142A RID: 5162
		// (get) Token: 0x06005685 RID: 22149 RVA: 0x00079AAC File Offset: 0x00077CAC
		public int Cooldown
		{
			get
			{
				if (this.m_execution == null)
				{
					return int.MaxValue;
				}
				return this.m_execution.Cooldown;
			}
		}

		// Token: 0x06005686 RID: 22150 RVA: 0x001E1248 File Offset: 0x001DF448
		public ArchetypeInstance DynamicallyLoad(ContainerInstance containerInstance)
		{
			ArchetypeInstance result;
			if (!containerInstance.TryGetInstanceForArchetypeId(base.Id, out result))
			{
				ArchetypeInstance archetypeInstance = this.CreateNewInstance();
				archetypeInstance.AbilityData.MemorizationTimestamp = new DateTime?(DateTime.UtcNow);
				archetypeInstance.Index = -1;
				archetypeInstance.InstanceId = new UniqueId(this.GetInstanceId());
				if (this.CreateInstanceUI)
				{
					archetypeInstance.CreateItemInstanceUI();
				}
				containerInstance.Add(archetypeInstance, false);
				return archetypeInstance;
			}
			return result;
		}

		// Token: 0x06005687 RID: 22151 RVA: 0x00079AC7 File Offset: 0x00077CC7
		public override void OnInstanceCreated(ArchetypeInstance instance)
		{
			base.OnInstanceCreated(instance);
			instance.AbilityData = new AbilityInstanceData(true);
		}

		// Token: 0x1700142B RID: 5163
		// (get) Token: 0x06005688 RID: 22152 RVA: 0x00045BCA File Offset: 0x00043DCA
		protected virtual bool PauseWhileHandSwapActive
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700142C RID: 5164
		// (get) Token: 0x06005689 RID: 22153 RVA: 0x00045BCA File Offset: 0x00043DCA
		protected virtual bool PauseWhileExecuting
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700142D RID: 5165
		// (get) Token: 0x0600568A RID: 22154 RVA: 0x00045BCA File Offset: 0x00043DCA
		public virtual bool ConsiderHaste
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700142E RID: 5166
		// (get) Token: 0x0600568B RID: 22155 RVA: 0x00045BCA File Offset: 0x00043DCA
		public virtual bool ClampHasteTo100
		{
			get
			{
				return false;
			}
		}

		// Token: 0x0600568C RID: 22156 RVA: 0x0006109C File Offset: 0x0005F29C
		protected virtual float GetCooldown(GameEntity entity, ExecutionCache executionCache = null)
		{
			return 1f;
		}

		// Token: 0x1700142F RID: 5167
		// (get) Token: 0x0600568D RID: 22157 RVA: 0x00079ADC File Offset: 0x00077CDC
		bool IAbilityCooldown.PauseWhileHandSwapActive
		{
			get
			{
				return this.PauseWhileHandSwapActive;
			}
		}

		// Token: 0x17001430 RID: 5168
		// (get) Token: 0x0600568E RID: 22158 RVA: 0x00079AE4 File Offset: 0x00077CE4
		bool IAbilityCooldown.PauseWhileExecuting
		{
			get
			{
				return this.PauseWhileExecuting;
			}
		}

		// Token: 0x17001431 RID: 5169
		// (get) Token: 0x0600568F RID: 22159 RVA: 0x00079AEC File Offset: 0x00077CEC
		bool IAbilityCooldown.ConsiderHaste
		{
			get
			{
				return this.ConsiderHaste;
			}
		}

		// Token: 0x17001432 RID: 5170
		// (get) Token: 0x06005690 RID: 22160 RVA: 0x00079AF4 File Offset: 0x00077CF4
		bool IAbilityCooldown.ClampHasteTo100
		{
			get
			{
				return this.ClampHasteTo100;
			}
		}

		// Token: 0x06005691 RID: 22161 RVA: 0x00079AFC File Offset: 0x00077CFC
		float IAbilityCooldown.GetCooldown(GameEntity entity, ExecutionCache executionCache, float level)
		{
			return this.GetCooldown(entity, executionCache);
		}

		// Token: 0x17001433 RID: 5171
		// (get) Token: 0x06005692 RID: 22162 RVA: 0x00079B06 File Offset: 0x00077D06
		protected virtual float ExecutionTime
		{
			get
			{
				return this.m_executionTime;
			}
		}

		// Token: 0x06005693 RID: 22163
		protected abstract bool TryGetAbilityAnimation(GameEntity entity, out AbilityAnimation animation);

		// Token: 0x17001434 RID: 5172
		// (get) Token: 0x06005694 RID: 22164 RVA: 0x00049FFA File Offset: 0x000481FA
		protected virtual AbilityVFX m_abilityVFX
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17001435 RID: 5173
		// (get) Token: 0x06005695 RID: 22165 RVA: 0x00045BCA File Offset: 0x00043DCA
		protected virtual bool m_triggerGlobalCooldown
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06005696 RID: 22166 RVA: 0x00079B0E File Offset: 0x00077D0E
		protected virtual bool PreExecution(ExecutionCache executionCache, bool initial)
		{
			if (executionCache.SourceEntity.Vitals.GetCurrentHealthState() != HealthState.Alive)
			{
				executionCache.Message = "Not awake!";
				return false;
			}
			return true;
		}

		// Token: 0x06005697 RID: 22167
		protected abstract void PostExecution(ExecutionCache executionCache);

		// Token: 0x06005698 RID: 22168
		protected abstract bool MeetsRequirementsForUI(GameEntity entity);

		// Token: 0x17001436 RID: 5174
		// (get) Token: 0x06005699 RID: 22169 RVA: 0x00079B31 File Offset: 0x00077D31
		EffectSourceType IExecutable.Type
		{
			get
			{
				return this.m_effectSourceType;
			}
		}

		// Token: 0x0600569A RID: 22170 RVA: 0x00045BCA File Offset: 0x00043DCA
		bool IExecutable.UseAutoAttackAnimation()
		{
			return false;
		}

		// Token: 0x0600569B RID: 22171 RVA: 0x00079B39 File Offset: 0x00077D39
		bool IExecutable.TryGetAbilityAnimation(GameEntity entity, out AbilityAnimation animation)
		{
			return this.TryGetAbilityAnimation(entity, out animation);
		}

		// Token: 0x17001437 RID: 5175
		// (get) Token: 0x0600569C RID: 22172 RVA: 0x00079B43 File Offset: 0x00077D43
		bool IExecutable.TriggerGlobalCooldown
		{
			get
			{
				return this.m_triggerGlobalCooldown;
			}
		}

		// Token: 0x17001438 RID: 5176
		// (get) Token: 0x0600569D RID: 22173 RVA: 0x00045BCA File Offset: 0x00043DCA
		bool IExecutable.AllowStaminaRegenDuringExecution
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17001439 RID: 5177
		// (get) Token: 0x0600569E RID: 22174 RVA: 0x00045BCA File Offset: 0x00043DCA
		bool IExecutable.AllowAlchemy
		{
			get
			{
				return false;
			}
		}

		// Token: 0x0600569F RID: 22175 RVA: 0x00079B4B File Offset: 0x00077D4B
		void IExecutable.PostExecution(ExecutionCache executionCache)
		{
			this.PostExecution(executionCache);
		}

		// Token: 0x060056A0 RID: 22176 RVA: 0x00079B54 File Offset: 0x00077D54
		bool IExecutable.PreExecution(ExecutionCache executionCache)
		{
			return this.PreExecution(executionCache, true);
		}

		// Token: 0x060056A1 RID: 22177 RVA: 0x00079B5E File Offset: 0x00077D5E
		bool IExecutable.ContinuedExecution(ExecutionCache executionCache, float executionProgress)
		{
			return this.PreExecution(executionCache, false);
		}

		// Token: 0x060056A2 RID: 22178 RVA: 0x00079B68 File Offset: 0x00077D68
		bool IExecutable.MeetsRequirementsForUI(GameEntity entity, float level)
		{
			return this.MeetsRequirementsForUI(entity);
		}

		// Token: 0x060056A3 RID: 22179 RVA: 0x00079B71 File Offset: 0x00077D71
		ExecutionParams IExecutable.GetExecutionParams(float level, AlchemyPowerLevel alchemyPowerLevel)
		{
			return this.m_execution;
		}

		// Token: 0x060056A4 RID: 22180 RVA: 0x00045BCA File Offset: 0x00043DCA
		bool IExecutable.HasValidAlchemyOverride(float level, AlchemyPowerLevel alchemyPowerLevel)
		{
			return false;
		}

		// Token: 0x1700143A RID: 5178
		// (get) Token: 0x060056A5 RID: 22181 RVA: 0x00154B30 File Offset: 0x00152D30
		float? IExecutable.DeferHandIkDuration
		{
			get
			{
				return null;
			}
		}

		// Token: 0x1700143B RID: 5179
		// (get) Token: 0x060056A6 RID: 22182 RVA: 0x00045BCA File Offset: 0x00043DCA
		bool IExecutable.IsLearning
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700143C RID: 5180
		// (get) Token: 0x060056A7 RID: 22183 RVA: 0x00079B79 File Offset: 0x00077D79
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

		// Token: 0x1700143D RID: 5181
		// (get) Token: 0x060056A8 RID: 22184 RVA: 0x00079B90 File Offset: 0x00077D90
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

		// Token: 0x1700143E RID: 5182
		// (get) Token: 0x060056A9 RID: 22185 RVA: 0x00079BA7 File Offset: 0x00077DA7
		bool IExecutable.PreventRotation
		{
			get
			{
				return this.m_execution != null && this.m_execution.PreventRotation;
			}
		}

		// Token: 0x1700143F RID: 5183
		// (get) Token: 0x060056AA RID: 22186 RVA: 0x00079BBE File Offset: 0x00077DBE
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

		// Token: 0x060056AB RID: 22187 RVA: 0x00079BD9 File Offset: 0x00077DD9
		bool IVfxSource.TryGetEffects(int abilityLevel, AlchemyPowerLevel alchemyPowerLevel, bool isSecondary, out AbilityVFX effects)
		{
			effects = this.m_abilityVFX;
			return effects != null;
		}

		// Token: 0x17001440 RID: 5184
		// (get) Token: 0x060056AC RID: 22188 RVA: 0x0006109C File Offset: 0x0005F29C
		protected virtual float m_masteryCreditFactor
		{
			get
			{
				return 1f;
			}
		}

		// Token: 0x17001441 RID: 5185
		// (get) Token: 0x060056AD RID: 22189 RVA: 0x00045BCA File Offset: 0x00043DCA
		protected virtual bool m_addGroupBonus
		{
			get
			{
				return false;
			}
		}

		// Token: 0x060056AE RID: 22190
		protected abstract bool TryGetMastery(GameEntity entity, out MasteryArchetype mastery);

		// Token: 0x060056AF RID: 22191 RVA: 0x00079BEA File Offset: 0x00077DEA
		bool IMasterySource.TryGetMastery(GameEntity entity, out MasteryArchetype mastery)
		{
			return this.TryGetMastery(entity, out mastery);
		}

		// Token: 0x17001442 RID: 5186
		// (get) Token: 0x060056B0 RID: 22192 RVA: 0x00079BF4 File Offset: 0x00077DF4
		float IMasterySource.CreditFactor
		{
			get
			{
				return this.m_masteryCreditFactor;
			}
		}

		// Token: 0x17001443 RID: 5187
		// (get) Token: 0x060056B1 RID: 22193 RVA: 0x00079BFC File Offset: 0x00077DFC
		bool IMasterySource.AddGroupBonus
		{
			get
			{
				return this.m_addGroupBonus;
			}
		}

		// Token: 0x04004C7F RID: 19583
		[SerializeField]
		protected float m_executionTime = 1f;

		// Token: 0x04004C80 RID: 19584
		[SerializeField]
		protected float m_movementModifier;

		// Token: 0x04004C81 RID: 19585
		[SerializeField]
		private ExecutionParams m_execution;
	}
}

using System;
using System.Collections.Generic;
using Animancer;
using SoL.Game.Animation;
using SoL.Game.Crafting;
using SoL.Game.EffectSystem;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Archetypes.Abilities;
using SoL.Game.Objects.Containers;
using SoL.Game.Pooling;
using SoL.Game.Settings;
using SoL.Managers;
using SoL.Networking.Database;
using SoL.Networking.Objects;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game
{
	// Token: 0x020005C3 RID: 1475
	public class SkillsController : GameEntityComponent
	{
		// Token: 0x140000A3 RID: 163
		// (add) Token: 0x06002EC5 RID: 11973 RVA: 0x00154598 File Offset: 0x00152798
		// (remove) Token: 0x06002EC6 RID: 11974 RVA: 0x001545D0 File Offset: 0x001527D0
		public event Action<SkillsController.PendingExecution> PendingExecutionChanged;

		// Token: 0x140000A4 RID: 164
		// (add) Token: 0x06002EC7 RID: 11975 RVA: 0x00154608 File Offset: 0x00152808
		// (remove) Token: 0x06002EC8 RID: 11976 RVA: 0x00154640 File Offset: 0x00152840
		public event Action TriggerGlobalCooldown;

		// Token: 0x140000A5 RID: 165
		// (add) Token: 0x06002EC9 RID: 11977 RVA: 0x00154678 File Offset: 0x00152878
		// (remove) Token: 0x06002ECA RID: 11978 RVA: 0x001546B0 File Offset: 0x001528B0
		public event Action LastConsumableConsumedChanged;

		// Token: 0x170009EA RID: 2538
		// (get) Token: 0x06002ECB RID: 11979 RVA: 0x000605FD File Offset: 0x0005E7FD
		// (set) Token: 0x06002ECC RID: 11980 RVA: 0x00060605 File Offset: 0x0005E805
		public bool AutoAttackPending { get; protected set; }

		// Token: 0x170009EB RID: 2539
		// (get) Token: 0x06002ECD RID: 11981 RVA: 0x0006060E File Offset: 0x0005E80E
		// (set) Token: 0x06002ECE RID: 11982 RVA: 0x00060616 File Offset: 0x0005E816
		public float? AutoAttackServerFailedDelayUntil { get; protected set; }

		// Token: 0x170009EC RID: 2540
		// (get) Token: 0x06002ECF RID: 11983 RVA: 0x0006061F File Offset: 0x0005E81F
		// (set) Token: 0x06002ED0 RID: 11984 RVA: 0x00060627 File Offset: 0x0005E827
		public DateTime? NextNpcAttackTimeDelay { get; set; }

		// Token: 0x170009ED RID: 2541
		// (get) Token: 0x06002ED1 RID: 11985 RVA: 0x00060630 File Offset: 0x0005E830
		protected ExecutionCache AutoAttackExecutionCache
		{
			get
			{
				if (this.m_autoAttackExecutionCache == null)
				{
					this.m_autoAttackExecutionCache = StaticPool<ExecutionCache>.GetFromPool();
				}
				return this.m_autoAttackExecutionCache;
			}
		}

		// Token: 0x170009EE RID: 2542
		// (get) Token: 0x06002ED2 RID: 11986 RVA: 0x0006064B File Offset: 0x0005E84B
		protected static AutoAttackAbility m_autoAttack
		{
			get
			{
				return GlobalSettings.Values.Combat.AutoAttack;
			}
		}

		// Token: 0x170009EF RID: 2543
		// (get) Token: 0x06002ED3 RID: 11987 RVA: 0x0006065C File Offset: 0x0005E85C
		protected static IExecutable m_autoAttackExecutable
		{
			get
			{
				return SkillsController.m_autoAttack;
			}
		}

		// Token: 0x170009F0 RID: 2544
		// (get) Token: 0x06002ED4 RID: 11988 RVA: 0x0006065C File Offset: 0x0005E85C
		private static ICombatEffectSource m_autoAttackCombatEffectSource
		{
			get
			{
				return SkillsController.m_autoAttack;
			}
		}

		// Token: 0x170009F1 RID: 2545
		// (get) Token: 0x06002ED5 RID: 11989 RVA: 0x00060663 File Offset: 0x0005E863
		public SkillsController.PendingExecution Pending
		{
			get
			{
				return this.m_pending;
			}
		}

		// Token: 0x170009F2 RID: 2546
		// (get) Token: 0x06002ED6 RID: 11990 RVA: 0x0006066B File Offset: 0x0005E86B
		public bool PendingIsActive
		{
			get
			{
				return this.m_pending != null && this.m_pending.Active;
			}
		}

		// Token: 0x06002ED7 RID: 11991 RVA: 0x001546E8 File Offset: 0x001528E8
		public float GetElapsedSinceLastConsumable(ConsumableCategory category)
		{
			DateTime lastConsumableUsed = this.GetLastConsumableUsed(category);
			return (float)(GameTimeReplicator.GetServerCorrectedDateTimeUtc() - lastConsumableUsed).TotalSeconds;
		}

		// Token: 0x06002ED8 RID: 11992 RVA: 0x00060682 File Offset: 0x0005E882
		public DateTime GetTimestampOfLastConsumable(ConsumableCategory category)
		{
			return this.GetLastConsumableUsed(category);
		}

		// Token: 0x06002ED9 RID: 11993 RVA: 0x00154714 File Offset: 0x00152914
		public void MarkConsumableUsed(ConsumableCategory category)
		{
			if (this.m_record == null)
			{
				return;
			}
			if (this.m_record.ConsumableLastUseTimes == null)
			{
				this.m_record.ConsumableLastUseTimes = new Dictionary<ConsumableCategory, DateTime>(default(ConsumableCategoryComparer));
			}
			DateTime serverCorrectedDateTimeUtc = GameTimeReplicator.GetServerCorrectedDateTimeUtc();
			for (int i = 0; i < ConsumableCategoryExtensions.ConsumableCategories.Length; i++)
			{
				ConsumableCategory consumableCategory = ConsumableCategoryExtensions.ConsumableCategories[i];
				if (category.HasBitFlag(consumableCategory))
				{
					this.m_record.ConsumableLastUseTimes.AddOrReplace(consumableCategory, serverCorrectedDateTimeUtc);
				}
			}
			Action lastConsumableConsumedChanged = this.LastConsumableConsumedChanged;
			if (lastConsumableConsumedChanged == null)
			{
				return;
			}
			lastConsumableConsumedChanged();
		}

		// Token: 0x06002EDA RID: 11994 RVA: 0x0006068B File Offset: 0x0005E88B
		public void TriggerLastConsumableConsumedChanged()
		{
			Action lastConsumableConsumedChanged = this.LastConsumableConsumedChanged;
			if (lastConsumableConsumedChanged == null)
			{
				return;
			}
			lastConsumableConsumedChanged();
		}

		// Token: 0x06002EDB RID: 11995 RVA: 0x001547A0 File Offset: 0x001529A0
		private DateTime GetLastConsumableUsed(ConsumableCategory category)
		{
			if (this.m_record == null || this.m_record.ConsumableLastUseTimes == null || this.m_record.ConsumableLastUseTimes.Count <= 0)
			{
				return DateTime.MinValue;
			}
			DateTime dateTime = DateTime.MinValue;
			foreach (KeyValuePair<ConsumableCategory, DateTime> keyValuePair in this.m_record.ConsumableLastUseTimes)
			{
				if (category.HasBitFlag(keyValuePair.Key) && keyValuePair.Value > dateTime)
				{
					dateTime = keyValuePair.Value;
				}
			}
			return dateTime;
		}

		// Token: 0x06002EDC RID: 11996 RVA: 0x0006069D File Offset: 0x0005E89D
		protected virtual void Awake()
		{
			base.GameEntity.SkillsController = this;
		}

		// Token: 0x06002EDD RID: 11997 RVA: 0x000606AB File Offset: 0x0005E8AB
		protected virtual void OnDestroy()
		{
			if (this.m_autoAttackExecutionCache != null)
			{
				StaticPool<ExecutionCache>.ReturnToPool(this.m_autoAttackExecutionCache);
				this.m_autoAttackExecutionCache = null;
			}
		}

		// Token: 0x06002EDE RID: 11998 RVA: 0x000606C7 File Offset: 0x0005E8C7
		protected void OnPendingExecutionChanged(SkillsController.PendingExecution pending)
		{
			Action<SkillsController.PendingExecution> pendingExecutionChanged = this.PendingExecutionChanged;
			if (pendingExecutionChanged == null)
			{
				return;
			}
			pendingExecutionChanged(pending);
		}

		// Token: 0x06002EDF RID: 11999 RVA: 0x000606DA File Offset: 0x0005E8DA
		protected void OnGlobalCooldown()
		{
			Action triggerGlobalCooldown = this.TriggerGlobalCooldown;
			if (triggerGlobalCooldown == null)
			{
				return;
			}
			triggerGlobalCooldown();
		}

		// Token: 0x06002EE0 RID: 12000 RVA: 0x000606EC File Offset: 0x0005E8EC
		public void Initialize(CharacterRecord record)
		{
			this.m_record = record;
		}

		// Token: 0x06002EE1 RID: 12001 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void CacheAbilities(bool useHealthFractionAutoAttack, bool bypassLevelDeltaCombatAdjustments)
		{
		}

		// Token: 0x06002EE2 RID: 12002 RVA: 0x0015484C File Offset: 0x00152A4C
		public bool TryGetCachedMasteryInstance(ArchetypeInstance abilityInstance, out ArchetypeInstance masteryInstance)
		{
			masteryInstance = null;
			if (abilityInstance == null || !abilityInstance.IsAbility)
			{
				return false;
			}
			if (this.m_abilityToMastery == null)
			{
				this.m_abilityToMastery = new Dictionary<ArchetypeInstance, ArchetypeInstance>();
			}
			AbilityArchetype abilityArchetype;
			if (!this.m_abilityToMastery.TryGetValue(abilityInstance, out masteryInstance) && abilityInstance.Archetype.TryGetAsType(out abilityArchetype) && base.GameEntity.CollectionController.Masteries != null && base.GameEntity.CollectionController.Masteries.TryGetInstanceForArchetypeId(abilityArchetype.Mastery.Id, out masteryInstance))
			{
				this.m_abilityToMastery.Add(abilityInstance, masteryInstance);
			}
			return masteryInstance != null;
		}

		// Token: 0x06002EE3 RID: 12003 RVA: 0x000606F5 File Offset: 0x0005E8F5
		public void ClearAbilityToMasteryCache()
		{
			Dictionary<ArchetypeInstance, ArchetypeInstance> abilityToMastery = this.m_abilityToMastery;
			if (abilityToMastery == null)
			{
				return;
			}
			abilityToMastery.Clear();
		}

		// Token: 0x06002EE4 RID: 12004 RVA: 0x00060707 File Offset: 0x0005E907
		protected virtual bool BypassCooldownUpdate()
		{
			if (base.GameEntity)
			{
				ICollectionController collectionController = base.GameEntity.CollectionController;
				return ((collectionController != null) ? collectionController.Abilities : null) == null;
			}
			return true;
		}

		// Token: 0x06002EE5 RID: 12005 RVA: 0x001548E4 File Offset: 0x00152AE4
		protected void UpdateCooldowns()
		{
			if (this.BypassCooldownUpdate())
			{
				return;
			}
			float num = base.GameEntity.Vitals ? ((float)base.GameEntity.Vitals.GetHaste() * 0.01f) : 0f;
			float num2 = 0f;
			float num3 = 0f;
			if (num > 0f)
			{
				num2 = num;
				num3 = Mathf.Clamp01(num2);
			}
			else if (num < 0f)
			{
				float t = Mathf.Clamp(num, -1f, 0f) + 1f;
				num2 = Mathf.Lerp(-0.8f, 0f, t);
				num3 = num2;
			}
			float deltaTime = Time.deltaTime;
			float num4 = num2 * deltaTime;
			float num5 = deltaTime + num4;
			float num6 = num3 * deltaTime;
			float num7 = deltaTime + num6;
			for (int i = 0; i < base.GameEntity.CollectionController.Abilities.Count; i++)
			{
				ArchetypeInstance index = base.GameEntity.CollectionController.Abilities.GetIndex(i);
				if (index != null && index.IsAbility)
				{
					AbilityInstanceData abilityData = index.AbilityData;
					bool flag = abilityData.Cooldown_AlchemyI.UpdateElapsed(deltaTime, AlchemyPowerLevel.I);
					flag = (abilityData.Cooldown_AlchemyII.UpdateElapsed(deltaTime, AlchemyPowerLevel.II) || flag);
					if (flag)
					{
						index.AbilityData.TriggerAlchemyCooldownStatusChanged();
					}
					if (abilityData.Cooldown_Base.Elapsed != null)
					{
						IAbilityCooldown abilityCooldown2;
						if (!abilityData.IsDynamic)
						{
							IAbilityCooldown abilityCooldown = index.Ability;
							abilityCooldown2 = abilityCooldown;
						}
						else
						{
							IAbilityCooldown abilityCooldown = index.DynamicAbility;
							abilityCooldown2 = abilityCooldown;
						}
						IAbilityCooldown abilityCooldown3 = abilityCooldown2;
						if (abilityCooldown3 != null && (!abilityCooldown3.PauseWhileExecuting || !this.PendingIsActive) && (!abilityCooldown3.PauseWhileHandSwapActive || GameManager.IsServer || !UIManager.AutoAttackButton || !UIManager.AutoAttackButton.HandSwapActive))
						{
							float deltaTime2 = deltaTime;
							if (abilityCooldown3.ConsiderHaste)
							{
								deltaTime2 = (abilityCooldown3.ClampHasteTo100 ? num7 : num5);
							}
							if (abilityData.Cooldown_Base.Cooldown == null)
							{
								float associatedLevel = index.GetAssociatedLevel(base.GameEntity);
								abilityData.Cooldown_Base.Cooldown = new float?(abilityCooldown3.GetCooldown(base.GameEntity, null, associatedLevel));
							}
							if (abilityData.Cooldown_Base.UpdateElapsed(deltaTime2, AlchemyPowerLevel.None))
							{
								abilityData.Cooldown_Base.Reset();
							}
						}
					}
				}
			}
		}

		// Token: 0x06002EE6 RID: 12006 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void Client_Execute_Instant_Failed(string message)
		{
		}

		// Token: 0x06002EE7 RID: 12007 RVA: 0x00045BCA File Offset: 0x00043DCA
		public virtual bool BeginExecution(ArchetypeInstance instance)
		{
			return false;
		}

		// Token: 0x06002EE8 RID: 12008 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void Client_BeginExecution_Failed(UniqueId archetypeId, string message)
		{
		}

		// Token: 0x06002EE9 RID: 12009 RVA: 0x00045BCA File Offset: 0x00043DCA
		public virtual bool EscapePressed()
		{
			return false;
		}

		// Token: 0x06002EEA RID: 12010 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void MasteryLevelChanged(UniqueId masteryArchetypeId, float newLevel)
		{
		}

		// Token: 0x06002EEB RID: 12011 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void MasteryAbilityLevelChanged(InstanceNewLevelData newLevelData)
		{
		}

		// Token: 0x06002EEC RID: 12012 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void LevelProgressionEvent(LevelProgressionEvent levelProgressionEvent)
		{
		}

		// Token: 0x06002EED RID: 12013 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void LevelProgressionUpdate(LevelProgressionUpdate levelProgressionUpdate)
		{
		}

		// Token: 0x06002EEE RID: 12014 RVA: 0x00154B30 File Offset: 0x00152D30
		public virtual float? ExecuteAutoAttack(ArchetypeInstance instance, bool logError)
		{
			return null;
		}

		// Token: 0x06002EEF RID: 12015 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void ExecuteAutoAttackFailed(string message)
		{
		}

		// Token: 0x06002EF0 RID: 12016 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void Client_Execute_Instant(UniqueId archetypeId, NetworkEntity targetEntity, byte abilityLevel)
		{
		}

		// Token: 0x06002EF1 RID: 12017 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void Client_Execution_Begin(UniqueId archetypeId, NetworkEntity targetEntity, byte abilityLevel, AlchemyPowerLevel alchemyPowerLevel)
		{
		}

		// Token: 0x06002EF2 RID: 12018 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void Client_Execution_Cancelled(UniqueId archetypeId)
		{
		}

		// Token: 0x06002EF3 RID: 12019 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void Client_Execution_Complete(UniqueId archetypeId, NetworkEntity updatedNetworkEntity)
		{
		}

		// Token: 0x06002EF4 RID: 12020 RVA: 0x00154B48 File Offset: 0x00152D48
		public virtual void Client_AutoAttack(NetworkEntity targetEntity, AnimationFlags animFlags)
		{
			AbilityVFX abilityVFX;
			if (SkillsController.m_autoAttackExecutable.TryGetEffects(0, AlchemyPowerLevel.None, false, out abilityVFX))
			{
				if (abilityVFX.SourceApplication != null)
				{
					abilityVFX.SourceApplication.GetPooledInstance<PooledVFX>().Initialize(base.GameEntity, 5f, null);
				}
				if (targetEntity != null && targetEntity.GameEntity != null)
				{
					if (abilityVFX.TargetApplication != null)
					{
						abilityVFX.TargetApplication.GetPooledInstance<PooledVFX>().Initialize(targetEntity.GameEntity, 5f, base.GameEntity);
					}
					if (abilityVFX.Projectile != null && SkillsController.m_autoAttackCombatEffectSource.DeliveryParams.DeliveryMethod == DeliveryMethodTypes.Projectile)
					{
						float velocity = SkillsController.m_autoAttackCombatEffectSource.DeliveryParams.Velocity;
						abilityVFX.Projectile.GetPooledInstance<PooledProjectile>().Initialize(base.GameEntity, targetEntity.GameEntity, velocity);
					}
				}
			}
			if (base.GameEntity && base.GameEntity.AnimancerController != null)
			{
				if (base.GameEntity.AnimancerController.CurrentAnimationSet != null)
				{
					AnimationSequence nextAutoAttackSequence = base.GameEntity.AnimancerController.CurrentAnimationSet.GetNextAutoAttackSequence(animFlags);
					if (nextAutoAttackSequence != null)
					{
						base.GameEntity.AnimancerController.StartSequence(nextAutoAttackSequence, null);
						return;
					}
				}
				else
				{
					base.GameEntity.AnimancerController.StartSequence(null, null);
				}
			}
		}

		// Token: 0x06002EF5 RID: 12021 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void Server_Execute_Instant(ClientExecutionCache clientExecutionCache)
		{
		}

		// Token: 0x06002EF6 RID: 12022 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void Server_Execution_Begin(DateTime timestamp, ClientExecutionCache clientExecutionCache)
		{
		}

		// Token: 0x06002EF7 RID: 12023 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void Server_Execution_Cancel(UniqueId archetypeId)
		{
		}

		// Token: 0x06002EF8 RID: 12024 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void Server_Execution_Complete(UniqueId archetypeId, DateTime timestamp)
		{
		}

		// Token: 0x06002EF9 RID: 12025 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void Server_Execute_AutoAttack(NetworkEntity targetEntity)
		{
		}

		// Token: 0x04002E1B RID: 11803
		private ExecutionCache m_autoAttackExecutionCache;

		// Token: 0x04002E1C RID: 11804
		private CharacterRecord m_record;

		// Token: 0x04002E1D RID: 11805
		protected SkillsController.PendingExecution m_pending;

		// Token: 0x04002E1E RID: 11806
		private Dictionary<ArchetypeInstance, ArchetypeInstance> m_abilityToMastery;

		// Token: 0x020005C4 RID: 1476
		public class PendingExecution : IAnimancerStateTracker
		{
			// Token: 0x170009F3 RID: 2547
			// (get) Token: 0x06002EFB RID: 12027 RVA: 0x00060732 File Offset: 0x0005E932
			// (set) Token: 0x06002EFC RID: 12028 RVA: 0x0006073A File Offset: 0x0005E93A
			AnimancerState IAnimancerStateTracker.State
			{
				get
				{
					return this.AnimancerState;
				}
				set
				{
					this.AnimancerState = value;
				}
			}

			// Token: 0x170009F4 RID: 2548
			// (get) Token: 0x06002EFD RID: 12029 RVA: 0x00060743 File Offset: 0x0005E943
			public CraftingToolType ActiveToolType
			{
				get
				{
					return this.m_activeToolType;
				}
			}

			// Token: 0x170009F5 RID: 2549
			// (get) Token: 0x06002EFE RID: 12030 RVA: 0x0006074B File Offset: 0x0005E94B
			public bool IsLearning
			{
				get
				{
					return this.m_isLearning;
				}
			}

			// Token: 0x170009F6 RID: 2550
			// (get) Token: 0x06002EFF RID: 12031 RVA: 0x00060753 File Offset: 0x0005E953
			// (set) Token: 0x06002F00 RID: 12032 RVA: 0x0006075B File Offset: 0x0005E95B
			public UniqueId PendingId { get; private set; }

			// Token: 0x170009F7 RID: 2551
			// (get) Token: 0x06002F01 RID: 12033 RVA: 0x00060764 File Offset: 0x0005E964
			public DateTime ServerTimestamp
			{
				get
				{
					return this.m_serverTimestamp;
				}
			}

			// Token: 0x170009F8 RID: 2552
			// (get) Token: 0x06002F02 RID: 12034 RVA: 0x0006076C File Offset: 0x0005E96C
			public DateTime ClientTimestamp
			{
				get
				{
					return this.m_clientTimestamp;
				}
			}

			// Token: 0x170009F9 RID: 2553
			// (get) Token: 0x06002F03 RID: 12035 RVA: 0x00060774 File Offset: 0x0005E974
			public ExecutionCache ExecutionCache
			{
				get
				{
					return this.m_executionCache;
				}
			}

			// Token: 0x170009FA RID: 2554
			// (get) Token: 0x06002F04 RID: 12036 RVA: 0x0006077C File Offset: 0x0005E97C
			public IExecutable Executable
			{
				get
				{
					return this.m_executable;
				}
			}

			// Token: 0x170009FB RID: 2555
			// (get) Token: 0x06002F05 RID: 12037 RVA: 0x00060784 File Offset: 0x0005E984
			public ExecutionParams ExecutionParams
			{
				get
				{
					return this.m_executionParams;
				}
			}

			// Token: 0x170009FC RID: 2556
			// (get) Token: 0x06002F06 RID: 12038 RVA: 0x0006078C File Offset: 0x0005E98C
			public ICombatEffectSource EffectsNew
			{
				get
				{
					return this.m_effectsNew;
				}
			}

			// Token: 0x170009FD RID: 2557
			// (get) Token: 0x06002F07 RID: 12039 RVA: 0x00060794 File Offset: 0x0005E994
			public TargetingParams TargetingParams
			{
				get
				{
					return this.m_targetingParams;
				}
			}

			// Token: 0x170009FE RID: 2558
			// (get) Token: 0x06002F08 RID: 12040 RVA: 0x0006079C File Offset: 0x0005E99C
			public NetworkEntity TargetNetworkEntity
			{
				get
				{
					return this.m_targetNetworkEntity;
				}
			}

			// Token: 0x170009FF RID: 2559
			// (get) Token: 0x06002F09 RID: 12041 RVA: 0x000607A4 File Offset: 0x0005E9A4
			public UniqueId ArchetypeId
			{
				get
				{
					return this.m_archetypeId;
				}
			}

			// Token: 0x17000A00 RID: 2560
			// (get) Token: 0x06002F0A RID: 12042 RVA: 0x000607AC File Offset: 0x0005E9AC
			public ArchetypeInstance Instance
			{
				get
				{
					return this.m_instance;
				}
			}

			// Token: 0x17000A01 RID: 2561
			// (get) Token: 0x06002F0B RID: 12043 RVA: 0x000607B4 File Offset: 0x0005E9B4
			public float ExecutionTime
			{
				get
				{
					return this.m_executionTime;
				}
			}

			// Token: 0x17000A02 RID: 2562
			// (get) Token: 0x06002F0C RID: 12044 RVA: 0x000607BC File Offset: 0x0005E9BC
			public float ExecutionTimeRemaining
			{
				get
				{
					return this.m_executionTimeRemaining;
				}
			}

			// Token: 0x17000A03 RID: 2563
			// (get) Token: 0x06002F0D RID: 12045 RVA: 0x000607C4 File Offset: 0x0005E9C4
			public byte AbilityLevelAsByte
			{
				get
				{
					return this.m_abilityLevelAsByte;
				}
			}

			// Token: 0x17000A04 RID: 2564
			// (get) Token: 0x06002F0E RID: 12046 RVA: 0x000607CC File Offset: 0x0005E9CC
			public AlchemyPowerLevel AlchemyPowerLevel
			{
				get
				{
					return this.m_alchemyPowerLevel;
				}
			}

			// Token: 0x17000A05 RID: 2565
			// (get) Token: 0x06002F0F RID: 12047 RVA: 0x000607D4 File Offset: 0x0005E9D4
			public bool Active
			{
				get
				{
					return this.m_status == SkillsController.PendingExecution.PendingStatus.Active || this.m_status == SkillsController.PendingExecution.PendingStatus.ActiveAcknowledged;
				}
			}

			// Token: 0x17000A06 RID: 2566
			// (get) Token: 0x06002F10 RID: 12048 RVA: 0x000607EA File Offset: 0x0005E9EA
			public bool Cancelled
			{
				get
				{
					return this.m_status == SkillsController.PendingExecution.PendingStatus.CancelSent || this.m_status == SkillsController.PendingExecution.PendingStatus.CancelReceived;
				}
			}

			// Token: 0x17000A07 RID: 2567
			// (get) Token: 0x06002F11 RID: 12049 RVA: 0x00060800 File Offset: 0x0005EA00
			public bool Complete
			{
				get
				{
					return this.m_status == SkillsController.PendingExecution.PendingStatus.CompleteSent || this.m_status == SkillsController.PendingExecution.PendingStatus.CompleteReceived;
				}
			}

			// Token: 0x17000A08 RID: 2568
			// (get) Token: 0x06002F12 RID: 12050 RVA: 0x00060816 File Offset: 0x0005EA16
			// (set) Token: 0x06002F13 RID: 12051 RVA: 0x00154C98 File Offset: 0x00152E98
			public SkillsController.PendingExecution.PendingStatus Status
			{
				get
				{
					return this.m_status;
				}
				set
				{
					this.m_status = value;
					SkillsController.PendingExecution.PendingStatus status = this.m_status;
					if (status != SkillsController.PendingExecution.PendingStatus.CompleteReceived)
					{
						if (status - SkillsController.PendingExecution.PendingStatus.CancelSent <= 2)
						{
							this.CancelAnimation();
							return;
						}
					}
					else
					{
						this.Client_Execution_Complete();
					}
				}
			}

			// Token: 0x06002F14 RID: 12052 RVA: 0x0006081E File Offset: 0x0005EA1E
			public void UpdateTargetNetworkEntity(NetworkEntity targetEntity)
			{
				this.m_targetNetworkEntity = targetEntity;
			}

			// Token: 0x06002F15 RID: 12053 RVA: 0x00154CCC File Offset: 0x00152ECC
			public PendingExecution(SkillsController controller)
			{
				this.m_controller = controller;
				this.m_gameEntity = controller.GameEntity;
				this.m_pooledObjects = new List<PooledObject>(6);
				this.m_executionVfx = new List<PooledVFX>(6);
			}

			// Token: 0x06002F16 RID: 12054 RVA: 0x00154D44 File Offset: 0x00152F44
			public void InitLocalClient(ExecutionCache cache)
			{
				this.PendingId = UniqueId.GenerateFromGuid();
				this.m_archetypeId = cache.ArchetypeId;
				this.m_clientTimestamp = DateTime.UtcNow;
				this.m_executionCache = cache;
				this.m_instance = cache.Instance;
				this.m_targetNetworkEntity = cache.TargetNetworkEntity;
				this.m_executionTime = cache.ExecutionTime;
				this.m_executionTimeRemaining = this.m_executionTime;
				this.m_executable = cache.Executable;
				this.m_executionParams = cache.ExecutionParams;
				this.m_effectsNew = cache.EffectSource;
				this.m_targetingParams = cache.TargetingParams;
				this.m_abilityLevelAsByte = cache.AbilityLevelAsByte;
				this.m_alchemyPowerLevel = cache.AlchemyPowerLevel;
				this.m_status = SkillsController.PendingExecution.PendingStatus.Active;
				this.m_isLocal = true;
				this.m_isInstant = (this.m_executionTime <= 0f);
				this.AlchemyAnimancerState = null;
				this.AssignActiveTool();
				this.AssignIsRecipe();
			}

			// Token: 0x06002F17 RID: 12055 RVA: 0x00154E2C File Offset: 0x0015302C
			public void InitRemoteClient(UniqueId archetypeId, IExecutable executable, NetworkEntity targetEntity, byte abilityLevel, AlchemyPowerLevel alchemyPowerLevel, bool isInstant = false)
			{
				this.ResetInternal();
				this.PendingId = UniqueId.GenerateFromGuid();
				this.m_archetypeId = archetypeId;
				this.m_executable = executable;
				this.m_targetNetworkEntity = targetEntity;
				this.m_abilityLevelAsByte = abilityLevel;
				this.m_alchemyPowerLevel = alchemyPowerLevel;
				this.m_status = SkillsController.PendingExecution.PendingStatus.Active;
				this.m_isLocal = false;
				this.m_isInstant = isInstant;
				this.AssignActiveTool();
				this.AssignIsRecipe();
			}

			// Token: 0x06002F18 RID: 12056 RVA: 0x00154E94 File Offset: 0x00153094
			public void InitServer(ExecutionCache cache, DateTime clientTimestamp)
			{
				this.PendingId = UniqueId.GenerateFromGuid();
				this.m_archetypeId = cache.ArchetypeId;
				this.m_serverTimestamp = DateTime.UtcNow;
				this.m_clientTimestamp = clientTimestamp;
				this.m_executionCache = cache;
				this.m_instance = cache.Instance;
				this.m_targetNetworkEntity = cache.TargetNetworkEntity;
				this.m_executionTime = cache.ExecutionTime;
				this.m_executionTimeRemaining = this.m_executionTime;
				this.m_executable = cache.Executable;
				this.m_executionParams = cache.ExecutionParams;
				this.m_effectsNew = cache.EffectSource;
				this.m_targetingParams = cache.TargetingParams;
				this.StaminaCost = (float)cache.ExecutionParams.StaminaCost;
				this.StaminaDrained = 0f;
				if (GlobalSettings.Values.Player.StaminaDrainsDuringExecution)
				{
					this.StaminaCostPerSecond = new float?(this.StaminaCost / this.m_executionTime);
				}
				this.m_abilityLevelAsByte = cache.AbilityLevelAsByte;
				this.m_alchemyPowerLevel = cache.AlchemyPowerLevel;
				this.m_status = SkillsController.PendingExecution.PendingStatus.Active;
				this.m_isLocal = false;
				this.m_isInstant = (this.m_executionTime <= 0f);
			}

			// Token: 0x06002F19 RID: 12057 RVA: 0x00154FB4 File Offset: 0x001531B4
			private void AssignActiveTool()
			{
				if (this.m_executable is GatheringAbility && this.m_targetNetworkEntity)
				{
					IGatheringNode component = this.m_targetNetworkEntity.GetComponent<IGatheringNode>();
					if (component != null)
					{
						this.m_activeToolType = component.RequiredTool;
						return;
					}
				}
				this.m_activeToolType = CraftingToolType.None;
			}

			// Token: 0x06002F1A RID: 12058 RVA: 0x00060827 File Offset: 0x0005EA27
			private void AssignIsRecipe()
			{
				this.m_isLearning = this.m_executable.IsLearning;
			}

			// Token: 0x06002F1B RID: 12059 RVA: 0x00155000 File Offset: 0x00153200
			public void Client_Execution_Begin(IExecutable executable, NetworkEntity targetEntity)
			{
				this.m_executable = executable;
				this.m_targetNetworkEntity = targetEntity;
				AbilityVFX abilityVFX;
				if (executable.TryGetEffects((int)this.m_abilityLevelAsByte, this.m_alchemyPowerLevel, false, out abilityVFX) && !this.m_isInstant)
				{
					if (abilityVFX.SourceExecution)
					{
						PooledVFX pooledInstance = abilityVFX.SourceExecution.GetPooledInstance<PooledVFX>();
						pooledInstance.Initialize(this.m_gameEntity, null);
						if (!pooledInstance.HasTimeoutOverride)
						{
							this.m_pooledObjects.Add(pooledInstance);
						}
						else
						{
							this.m_executionVfx.Add(pooledInstance);
						}
					}
					if (targetEntity && targetEntity.GameEntity && abilityVFX.TargetExecution)
					{
						PooledVFX pooledInstance2 = abilityVFX.TargetExecution.GetPooledInstance<PooledVFX>();
						pooledInstance2.Initialize(targetEntity.GameEntity, null);
						if (!pooledInstance2.HasTimeoutOverride)
						{
							this.m_pooledObjects.Add(pooledInstance2);
						}
						else
						{
							this.m_executionVfx.Add(pooledInstance2);
						}
					}
				}
				if (this.m_gameEntity.HandheldMountController)
				{
					float? num = executable.DeferHandIkDuration;
					if (this.m_alchemyPowerLevel != AlchemyPowerLevel.None)
					{
						float addedExecutionTime = this.m_alchemyPowerLevel.GetAddedExecutionTime();
						if (num != null)
						{
							num = new float?(num.Value + addedExecutionTime);
						}
						else
						{
							num = new float?(addedExecutionTime);
						}
						num += this.m_alchemyPowerLevel.GetAddedExecutionTime();
					}
					this.m_gameEntity.HandheldMountController.DeferHandIk(num);
				}
				if (this.m_alchemyPowerLevel != AlchemyPowerLevel.None)
				{
					this.AlchemyAnimancerState = this.m_gameEntity.AnimancerController.StartAlchemySequence(null, this.m_alchemyPowerLevel);
				}
				this.PlayAnimation(AbilityAnimationType.Start, this.m_alchemyPowerLevel.GetAddedExecutionTime());
			}

			// Token: 0x06002F1C RID: 12060 RVA: 0x001551B8 File Offset: 0x001533B8
			private void Client_Execution_Complete()
			{
				ExecutionCache executionCache = this.m_executionCache;
				if (executionCache != null)
				{
					executionCache.LocalPlayerComplete(true, false);
				}
				this.m_executionVfx.Clear();
				if (this.Executable == null)
				{
					return;
				}
				AbilityVFX abilityVFX;
				if (this.Executable.TryGetEffects((int)this.m_abilityLevelAsByte, this.m_alchemyPowerLevel, false, out abilityVFX))
				{
					if (this.m_isInstant && ClientGameManager.DelayedEventManager)
					{
						GameEntity targetEntity = (this.TargetNetworkEntity && this.TargetNetworkEntity.GameEntity) ? this.TargetNetworkEntity.GameEntity : null;
						ClientGameManager.DelayedEventManager.RegisterDelayedVFX(abilityVFX.SourceApplication, this.m_gameEntity, abilityVFX.TargetApplication, targetEntity);
					}
					else
					{
						if (abilityVFX.SourceApplication != null)
						{
							abilityVFX.SourceApplication.GetPooledInstance<PooledVFX>().Initialize(this.m_gameEntity, 5f, null);
						}
						if (abilityVFX.TargetApplication != null && this.TargetNetworkEntity && this.TargetNetworkEntity.GameEntity)
						{
							abilityVFX.TargetApplication.GetPooledInstance<PooledVFX>().Initialize(this.TargetNetworkEntity.GameEntity, 5f, this.m_gameEntity);
						}
						abilityVFX.Projectile != null;
					}
				}
				this.PlayAnimation(AbilityAnimationType.End, 0f);
			}

			// Token: 0x06002F1D RID: 12061 RVA: 0x0006083A File Offset: 0x0005EA3A
			public void Reset()
			{
				this.ResetInternal();
			}

			// Token: 0x06002F1E RID: 12062 RVA: 0x00155308 File Offset: 0x00153508
			private void PlayAnimation(AbilityAnimationType type, float additionalSequenceDelay = 0f)
			{
				if (this.m_delayedAnimation != null)
				{
					this.m_delayedAnimation = null;
				}
				if (this.Executable == null || this.m_gameEntity.AnimancerController == null)
				{
					return;
				}
				AnimationSequence animationSequence = null;
				AbilityAnimation abilityAnimation;
				if (this.Executable.UseAutoAttackAnimation())
				{
					if (this.m_gameEntity.AnimancerController.CurrentAnimationSet != null)
					{
						animationSequence = this.m_gameEntity.AnimancerController.CurrentAnimationSet.GetNextAutoAttackSequence(AnimationFlags.None);
					}
				}
				else if (this.Executable.TryGetAbilityAnimation(this.m_gameEntity, out abilityAnimation))
				{
					animationSequence = abilityAnimation.GetAnimationSequence(type);
				}
				if (animationSequence == null || animationSequence.IsEmpty)
				{
					return;
				}
				float num = animationSequence.Delay + additionalSequenceDelay;
				if (num > 0f && !this.m_isInstant)
				{
					this.m_delayedAnimation = new SkillsController.PendingExecution.DelayedAnimation?(new SkillsController.PendingExecution.DelayedAnimation
					{
						Sequence = animationSequence,
						PlayTime = Time.time + num
					});
					return;
				}
				this.AnimancerState = this.m_gameEntity.AnimancerController.StartSequence(animationSequence, this);
				if (this.m_gameEntity.HandheldMountController != null)
				{
					this.m_gameEntity.HandheldMountController.RefreshIsGathering();
					this.m_gameEntity.HandheldMountController.RefreshIsLearning();
				}
			}

			// Token: 0x06002F1F RID: 12063 RVA: 0x00155438 File Offset: 0x00153638
			private void CancelAnimation()
			{
				if (this.m_controller && this.m_controller.GameEntity)
				{
					if (this.AlchemyAnimancerState != null)
					{
						IAnimationController animancerController = this.m_controller.GameEntity.AnimancerController;
						if (animancerController != null)
						{
							animancerController.FadeOutState(this.AlchemyAnimancerState);
						}
					}
					if (this.AnimancerState != null)
					{
						AnimancerState animancerState = this.AnimancerState;
						this.PlayAnimation(AbilityAnimationType.Cancel, 0f);
						IAnimationController animancerController2 = this.m_controller.GameEntity.AnimancerController;
						if (animancerController2 != null)
						{
							animancerController2.FadeOutState(animancerState);
						}
					}
					if (this.m_controller.GameEntity.HandheldMountController)
					{
						this.m_controller.GameEntity.HandheldMountController.CancelDeferredHandIk();
					}
				}
			}

			// Token: 0x06002F20 RID: 12064 RVA: 0x001554F8 File Offset: 0x001536F8
			private void ResetInternal()
			{
				this.PendingId = UniqueId.Empty;
				this.m_archetypeId = UniqueId.Empty;
				if (this.m_executionCache != null)
				{
					StaticPool<ExecutionCache>.ReturnToPool(this.m_executionCache);
				}
				this.m_executionCache = null;
				this.m_serverTimestamp = DateTime.MinValue;
				this.m_clientTimestamp = DateTime.MinValue;
				this.m_instance = null;
				this.m_targetNetworkEntity = null;
				this.m_executionTime = float.MaxValue;
				this.m_executionTimeRemaining = float.MaxValue;
				this.m_executable = null;
				this.m_executionParams = null;
				this.m_effectsNew = null;
				this.m_targetingParams = null;
				this.StaminaCost = 0f;
				this.StaminaCostPerSecond = null;
				this.StaminaDrained = 0f;
				this.AnimancerState = null;
				this.AlchemyAnimancerState = null;
				this.m_delayedAnimation = null;
				for (int i = 0; i < this.m_pooledObjects.Count; i++)
				{
					if (this.m_pooledObjects[i])
					{
						this.m_pooledObjects[i].ReturnToPool();
					}
				}
				this.m_pooledObjects.Clear();
				for (int j = 0; j < this.m_executionVfx.Count; j++)
				{
					if (this.m_executionVfx[j])
					{
						this.m_executionVfx[j].ReturnToPool();
					}
				}
				this.m_executionVfx.Clear();
				this.m_status = SkillsController.PendingExecution.PendingStatus.Idle;
				this.m_isLocal = false;
				this.m_isInstant = false;
				this.m_activeToolType = CraftingToolType.None;
				this.m_isLearning = false;
				this.m_abilityLevelAsByte = 0;
				this.m_alchemyPowerLevel = AlchemyPowerLevel.None;
			}

			// Token: 0x06002F21 RID: 12065 RVA: 0x00155684 File Offset: 0x00153884
			public void UpdateLocalClientTimeRemaining()
			{
				if (this.m_isLocal && this.Active)
				{
					float num = (float)(DateTime.UtcNow - this.m_clientTimestamp).TotalSeconds;
					this.m_executionTimeRemaining = this.m_executionTime - num;
				}
			}

			// Token: 0x06002F22 RID: 12066 RVA: 0x001556CC File Offset: 0x001538CC
			public void UpdateDelayedAnimation()
			{
				if (this.m_delayedAnimation != null && Time.time >= this.m_delayedAnimation.Value.PlayTime)
				{
					if (this.AlchemyAnimancerState != null)
					{
						this.m_gameEntity.AnimancerController.FadeOutState(this.AlchemyAnimancerState);
					}
					this.AnimancerState = this.m_gameEntity.AnimancerController.StartSequence(this.m_delayedAnimation.Value.Sequence, this);
					this.m_delayedAnimation = null;
				}
			}

			// Token: 0x06002F23 RID: 12067 RVA: 0x00060842 File Offset: 0x0005EA42
			public void NullifyExecutionCache()
			{
				this.m_executionCache = null;
			}

			// Token: 0x06002F24 RID: 12068 RVA: 0x00155750 File Offset: 0x00153950
			public float GetServerTimeElapsedPercent()
			{
				return (float)(DateTime.UtcNow - this.m_serverTimestamp).TotalSeconds / this.m_executionTime;
			}

			// Token: 0x04002E1F RID: 11807
			private CraftingToolType m_activeToolType;

			// Token: 0x04002E20 RID: 11808
			private bool m_isLearning;

			// Token: 0x04002E21 RID: 11809
			private readonly SkillsController m_controller;

			// Token: 0x04002E22 RID: 11810
			private readonly GameEntity m_gameEntity;

			// Token: 0x04002E23 RID: 11811
			private readonly List<PooledObject> m_pooledObjects;

			// Token: 0x04002E24 RID: 11812
			private readonly List<PooledVFX> m_executionVfx;

			// Token: 0x04002E25 RID: 11813
			private SkillsController.PendingExecution.PendingStatus m_status;

			// Token: 0x04002E26 RID: 11814
			private bool m_isLocal;

			// Token: 0x04002E27 RID: 11815
			private bool m_isInstant;

			// Token: 0x04002E28 RID: 11816
			private DateTime m_serverTimestamp = DateTime.MinValue;

			// Token: 0x04002E29 RID: 11817
			private DateTime m_clientTimestamp = DateTime.MinValue;

			// Token: 0x04002E2A RID: 11818
			private UniqueId m_archetypeId = UniqueId.Empty;

			// Token: 0x04002E2B RID: 11819
			private ExecutionCache m_executionCache;

			// Token: 0x04002E2C RID: 11820
			private ArchetypeInstance m_instance;

			// Token: 0x04002E2D RID: 11821
			private NetworkEntity m_targetNetworkEntity;

			// Token: 0x04002E2E RID: 11822
			private IExecutable m_executable;

			// Token: 0x04002E2F RID: 11823
			private ExecutionParams m_executionParams;

			// Token: 0x04002E30 RID: 11824
			private ICombatEffectSource m_effectsNew;

			// Token: 0x04002E31 RID: 11825
			private TargetingParams m_targetingParams;

			// Token: 0x04002E32 RID: 11826
			private float m_executionTime = float.MaxValue;

			// Token: 0x04002E33 RID: 11827
			private float m_executionTimeRemaining = float.MaxValue;

			// Token: 0x04002E34 RID: 11828
			private byte m_abilityLevelAsByte;

			// Token: 0x04002E35 RID: 11829
			private AlchemyPowerLevel m_alchemyPowerLevel;

			// Token: 0x04002E36 RID: 11830
			private SkillsController.PendingExecution.DelayedAnimation? m_delayedAnimation;

			// Token: 0x04002E38 RID: 11832
			public float StaminaCost;

			// Token: 0x04002E39 RID: 11833
			public float? StaminaCostPerSecond;

			// Token: 0x04002E3A RID: 11834
			public float StaminaDrained;

			// Token: 0x04002E3B RID: 11835
			private AnimancerState AnimancerState;

			// Token: 0x04002E3C RID: 11836
			private AnimancerState AlchemyAnimancerState;

			// Token: 0x020005C5 RID: 1477
			public enum PendingStatus
			{
				// Token: 0x04002E3E RID: 11838
				Idle,
				// Token: 0x04002E3F RID: 11839
				Active,
				// Token: 0x04002E40 RID: 11840
				ActiveAcknowledged,
				// Token: 0x04002E41 RID: 11841
				CompleteSent,
				// Token: 0x04002E42 RID: 11842
				CompleteReceived,
				// Token: 0x04002E43 RID: 11843
				CancelSent,
				// Token: 0x04002E44 RID: 11844
				CancelReceived,
				// Token: 0x04002E45 RID: 11845
				Failed
			}

			// Token: 0x020005C6 RID: 1478
			private struct DelayedAnimation
			{
				// Token: 0x04002E46 RID: 11846
				public float PlayTime;

				// Token: 0x04002E47 RID: 11847
				public AnimationSequence Sequence;
			}
		}
	}
}

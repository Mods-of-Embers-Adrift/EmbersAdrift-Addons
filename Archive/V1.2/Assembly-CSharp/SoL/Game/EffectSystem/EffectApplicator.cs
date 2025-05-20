using System;
using System.Collections.Generic;
using ENet;
using NetStack.Serialization;
using SoL.Game.Flanking;
using SoL.Game.HuntingLog;
using SoL.Game.NPCs;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Settings;
using SoL.Game.Targeting;
using SoL.Managers;
using SoL.Networking;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C10 RID: 3088
	public class EffectApplicator : IHandHeldItems, IPoolable
	{
		// Token: 0x06005EFF RID: 24319 RVA: 0x0007FF37 File Offset: 0x0007E137
		public static int GetTickCount(int duration, int tickRate)
		{
			return 1 + Mathf.Clamp(Mathf.FloorToInt((float)duration / (float)tickRate), 1, int.MaxValue);
		}

		// Token: 0x1700168A RID: 5770
		// (get) Token: 0x06005F00 RID: 24320 RVA: 0x0007FF50 File Offset: 0x0007E150
		CachedHandHeldItem IHandHeldItems.MainHand
		{
			get
			{
				return this.m_mainHandCache;
			}
		}

		// Token: 0x1700168B RID: 5771
		// (get) Token: 0x06005F01 RID: 24321 RVA: 0x0007FF58 File Offset: 0x0007E158
		CachedHandHeldItem IHandHeldItems.OffHand
		{
			get
			{
				return this.m_offHandCache;
			}
		}

		// Token: 0x06005F02 RID: 24322 RVA: 0x0007FF60 File Offset: 0x0007E160
		void IPoolable.Reset()
		{
			this.ResetInternal();
		}

		// Token: 0x1700168C RID: 5772
		// (get) Token: 0x06005F03 RID: 24323 RVA: 0x0007FF68 File Offset: 0x0007E168
		// (set) Token: 0x06005F04 RID: 24324 RVA: 0x0007FF70 File Offset: 0x0007E170
		bool IPoolable.InPool
		{
			get
			{
				return this.m_inPool;
			}
			set
			{
				this.m_inPool = value;
			}
		}

		// Token: 0x06005F05 RID: 24325 RVA: 0x001F850C File Offset: 0x001F670C
		private void ResetInternal()
		{
			this.Key = default(EffectKey);
			this.SyncData = default(EffectSyncData);
			this.Flags = EffectApplicationFlags.None;
			this.TriggeredEffect = null;
			this.PreserverOnUnconscious = false;
			if (this.m_record != null)
			{
				StaticPool<EffectRecord>.ReturnToPool(this.m_record);
			}
			this.m_record = null;
			this.m_expirationTime = DateTime.MinValue;
			this.m_effect = null;
			this.m_reagentItem = null;
			this.m_reagentItemTriggerCountMod = 0;
			this.m_offensiveHuntingLogEntry = null;
			this.m_defensiveHuntingLogEntry = null;
			this.m_sourceEntity = null;
			this.m_targetEntity = null;
			this.m_sourceGroupedLevel = 0;
			this.m_targetGroupedLevel = 0;
			this.m_stackTally = 0;
			this.m_positive = false;
			EffectApplicationResult results = this.m_results;
			if (results != null)
			{
				results.ResetAll();
			}
			this.m_resistDurationMultiplier = 1f;
			this.m_diminishingReturnsDurationMultiplier = 1f;
			this.m_triggerOnHit = false;
			this.m_isAbility = false;
			this.m_isAutoAttack = false;
			this.m_isInstant = false;
			this.m_isConsumable = false;
			this.m_isPlayerTooLow = false;
			this.m_applySlowDebuff = false;
			this.m_bypassLevelDeltaCombatAdjustments = false;
			this.m_applyPvpModifiers = false;
			this.m_alchemyPowerLevel = AlchemyPowerLevel.None;
			this.m_isLasting = false;
			this.m_angleTo = null;
			this.m_immediate.Reset();
			this.m_mainHandCache.Reset();
			this.m_offHandCache.Reset();
			this.SourceOverride = null;
			this.ResetApplicatorsToOverwrite();
		}

		// Token: 0x06005F06 RID: 24326 RVA: 0x0007FF79 File Offset: 0x0007E179
		private void ResetApplicatorsToOverwrite()
		{
			StaticListPool<EffectApplicator>.ReturnToPool(this.m_applicatorsToOverwrite);
			this.m_applicatorsToOverwrite = null;
		}

		// Token: 0x1700168D RID: 5773
		// (get) Token: 0x06005F07 RID: 24327 RVA: 0x0007FF8D File Offset: 0x0007E18D
		private bool Positive
		{
			get
			{
				return this.m_positive;
			}
		}

		// Token: 0x1700168E RID: 5774
		// (get) Token: 0x06005F08 RID: 24328 RVA: 0x0007FF95 File Offset: 0x0007E195
		private bool Negative
		{
			get
			{
				return !this.m_positive;
			}
		}

		// Token: 0x1700168F RID: 5775
		// (get) Token: 0x06005F09 RID: 24329 RVA: 0x0007FFA0 File Offset: 0x0007E1A0
		private bool AllowLevelDeltaCombatAdjustments
		{
			get
			{
				return !this.m_bypassLevelDeltaCombatAdjustments;
			}
		}

		// Token: 0x17001690 RID: 5776
		// (get) Token: 0x06005F0A RID: 24330 RVA: 0x0007FFAB File Offset: 0x0007E1AB
		// (set) Token: 0x06005F0B RID: 24331 RVA: 0x0007FFB3 File Offset: 0x0007E1B3
		public EffectKey Key { get; private set; }

		// Token: 0x17001691 RID: 5777
		// (get) Token: 0x06005F0C RID: 24332 RVA: 0x0007FFBC File Offset: 0x0007E1BC
		// (set) Token: 0x06005F0D RID: 24333 RVA: 0x0007FFC4 File Offset: 0x0007E1C4
		public EffectSyncData SyncData { get; private set; }

		// Token: 0x17001692 RID: 5778
		// (get) Token: 0x06005F0E RID: 24334 RVA: 0x0007FFCD File Offset: 0x0007E1CD
		// (set) Token: 0x06005F0F RID: 24335 RVA: 0x0007FFD5 File Offset: 0x0007E1D5
		public EffectApplicationFlags Flags { get; private set; }

		// Token: 0x17001693 RID: 5779
		// (get) Token: 0x06005F10 RID: 24336 RVA: 0x0007FFDE File Offset: 0x0007E1DE
		// (set) Token: 0x06005F11 RID: 24337 RVA: 0x0007FFE6 File Offset: 0x0007E1E6
		public ScriptableCombatEffect TriggeredEffect { get; private set; }

		// Token: 0x17001694 RID: 5780
		// (get) Token: 0x06005F12 RID: 24338 RVA: 0x0007FFEF File Offset: 0x0007E1EF
		// (set) Token: 0x06005F13 RID: 24339 RVA: 0x0007FFF7 File Offset: 0x0007E1F7
		public bool PreserverOnUnconscious { get; private set; }

		// Token: 0x17001695 RID: 5781
		// (get) Token: 0x06005F14 RID: 24340 RVA: 0x00080000 File Offset: 0x0007E200
		public EffectRecord Record
		{
			get
			{
				return this.m_record;
			}
		}

		// Token: 0x17001696 RID: 5782
		// (get) Token: 0x06005F15 RID: 24341 RVA: 0x00080008 File Offset: 0x0007E208
		public CombatEffect CombatEffect
		{
			get
			{
				return this.m_effect;
			}
		}

		// Token: 0x17001697 RID: 5783
		// (get) Token: 0x06005F16 RID: 24342 RVA: 0x00080010 File Offset: 0x0007E210
		public ReagentItem ReagentItem
		{
			get
			{
				return this.m_reagentItem;
			}
		}

		// Token: 0x17001698 RID: 5784
		// (get) Token: 0x06005F17 RID: 24343 RVA: 0x00080018 File Offset: 0x0007E218
		public BehaviorEffectTypeFlags BehaviorFlags
		{
			get
			{
				if (!(this.m_record == null))
				{
					return this.m_record.BehaviorFlags;
				}
				return BehaviorEffectTypeFlags.None;
			}
		}

		// Token: 0x17001699 RID: 5785
		// (get) Token: 0x06005F18 RID: 24344 RVA: 0x00080035 File Offset: 0x0007E235
		public CombatFlags CombatFlags
		{
			get
			{
				if (this.m_effect == null || this.m_effect.Effects == null || !this.m_effect.Effects.HasCombatFlags)
				{
					return CombatFlags.None;
				}
				return this.m_effect.Effects.CombatFlags;
			}
		}

		// Token: 0x1700169A RID: 5786
		// (get) Token: 0x06005F19 RID: 24345 RVA: 0x00080070 File Offset: 0x0007E270
		public bool ContainsTrigger
		{
			get
			{
				return this.m_effect != null && this.m_effect.IsTriggerBased;
			}
		}

		// Token: 0x1700169B RID: 5787
		// (get) Token: 0x06005F1A RID: 24346 RVA: 0x00080087 File Offset: 0x0007E287
		public bool PostRefreshFlags
		{
			get
			{
				return this.m_effect != null && this.m_effect.Effects != null && (this.m_effect.Effects.HasBehaviorEffects || this.m_effect.Effects.HasCombatFlags);
			}
		}

		// Token: 0x1700169C RID: 5788
		// (get) Token: 0x06005F1B RID: 24347 RVA: 0x000800C4 File Offset: 0x0007E2C4
		public GameEntity SourceEntity
		{
			get
			{
				return this.m_sourceEntity;
			}
		}

		// Token: 0x1700169D RID: 5789
		// (get) Token: 0x06005F1C RID: 24348 RVA: 0x000800CC File Offset: 0x0007E2CC
		public GameEntity TargetEntity
		{
			get
			{
				return this.m_targetEntity;
			}
		}

		// Token: 0x1700169E RID: 5790
		// (get) Token: 0x06005F1D RID: 24349 RVA: 0x000800D4 File Offset: 0x0007E2D4
		private EffectApplicationResult Results
		{
			get
			{
				if (this.m_results == null)
				{
					this.m_results = new EffectApplicationResult();
				}
				return this.m_results;
			}
		}

		// Token: 0x1700169F RID: 5791
		// (get) Token: 0x06005F1E RID: 24350 RVA: 0x000800EF File Offset: 0x0007E2EF
		private bool IsLasting
		{
			get
			{
				return this.m_isLasting;
			}
		}

		// Token: 0x170016A0 RID: 5792
		// (get) Token: 0x06005F1F RID: 24351 RVA: 0x001F867C File Offset: 0x001F687C
		private float AngleTo
		{
			get
			{
				if (this.m_angleTo == null)
				{
					this.m_angleTo = new float?(this.m_targetEntity.gameObject.AngleTo(this.m_sourceEntity.gameObject, true));
				}
				return this.m_angleTo.Value;
			}
		}

		// Token: 0x170016A1 RID: 5793
		// (get) Token: 0x06005F20 RID: 24352 RVA: 0x000800F7 File Offset: 0x0007E2F7
		private FlankingPosition FlankingPosition
		{
			get
			{
				return FlankingPositionExtensions.GetFlankingPosition(this.AngleTo);
			}
		}

		// Token: 0x170016A2 RID: 5794
		// (get) Token: 0x06005F21 RID: 24353 RVA: 0x00080104 File Offset: 0x0007E304
		// (set) Token: 0x06005F22 RID: 24354 RVA: 0x0008010C File Offset: 0x0007E30C
		public EffectSourceOverride? SourceOverride { get; private set; }

		// Token: 0x06005F23 RID: 24355 RVA: 0x001F86C8 File Offset: 0x001F68C8
		public bool Init(GameEntity targetEntity, EffectRecord record)
		{
			if (targetEntity == null || record == null)
			{
				return false;
			}
			if (record.ArchetypeId.IsEmpty)
			{
				return false;
			}
			if (record.EffectSource == null)
			{
				return false;
			}
			if (record.CombatEffect == null)
			{
				return false;
			}
			this.Key = new EffectKey(record);
			this.m_record = record;
			this.m_targetEntity = targetEntity;
			this.m_expirationTime = DateTime.UtcNow.AddSeconds((double)((float)record.TimingData.Duration - record.TimingData.Elapsed));
			this.m_isLasting = true;
			this.Results.SourceId = this.m_record.SourceNetworkId;
			this.Results.TargetId = this.m_targetEntity.NetworkEntity.NetworkId.Value;
			this.Results.SourceName = this.m_record.SourceData.Name;
			this.Results.ArchetypeId = this.m_record.ArchetypeId;
			this.Results.IsSecondary = this.m_record.IsSecondary;
			this.m_effect = this.m_record.CombatEffect;
			this.m_reagentItem = this.m_record.ReagentItem;
			this.m_reagentItemTriggerCountMod = (this.m_reagentItem ? this.m_reagentItem.GetTriggerCountMod() : 0);
			this.m_positive = (this.m_effect.Polarity == Polarity.Positive);
			this.PreserverOnUnconscious = this.m_effect.PreserveOnUnconscious;
			this.m_alchemyPowerLevel = this.m_record.AlchemyPowerLevel;
			this.Flags = (this.m_positive ? EffectApplicationFlags.Positive : EffectApplicationFlags.None);
			if (this.m_effect.Effects.HasTriggerEffects && this.m_effect.Effects.TriggeredEffect)
			{
				this.TriggeredEffect = this.m_effect.Effects.TriggeredEffect;
			}
			this.ApplyLasting();
			return this.IsLasting;
		}

		// Token: 0x06005F24 RID: 24356 RVA: 0x001F88B4 File Offset: 0x001F6AB4
		public bool Init(GameEntity targetEntity, ExecutionCache executionCache, CombatEffect effect, bool isSecondary)
		{
			if (!targetEntity)
			{
				throw new ArgumentNullException("targetEntity");
			}
			if (executionCache == null)
			{
				throw new ArgumentNullException("executionCache");
			}
			this.m_mainHandCache.CopyFromExecutionCache(executionCache);
			this.m_offHandCache.CopyFromExecutionCache(executionCache);
			if (executionCache.Instance != null)
			{
				this.m_isAbility = executionCache.Instance.IsAbility;
				this.m_isInstant = (executionCache.ExecutionTime <= 0f);
			}
			return this.InitInternalInitial(executionCache.SourceEntity, targetEntity, executionCache.ArchetypeId, executionCache.ReagentItem, effect, executionCache.MasteryLevel, executionCache.AbilityLevel, isSecondary, executionCache.AlchemyPowerLevel);
		}

		// Token: 0x06005F25 RID: 24357 RVA: 0x001F8958 File Offset: 0x001F6B58
		public bool Init(GameEntity sourceEntity, GameEntity targetEntity, UniqueId archetypeId, ReagentItem reagentItem, CombatEffect effect, float effectiveEntityLevel, float abilityLevel, bool isSecondary, EffectSourceOverride? sourceOverride)
		{
			if (!sourceEntity)
			{
				throw new ArgumentNullException("sourceEntity");
			}
			if (!targetEntity)
			{
				throw new ArgumentNullException("targetEntity");
			}
			this.m_mainHandCache.Init(sourceEntity);
			this.m_offHandCache.Init(sourceEntity);
			this.SourceOverride = sourceOverride;
			return this.InitInternalInitial(sourceEntity, targetEntity, archetypeId, reagentItem, effect, effectiveEntityLevel, abilityLevel, isSecondary, AlchemyPowerLevel.None);
		}

		// Token: 0x06005F26 RID: 24358 RVA: 0x00045BCA File Offset: 0x00043DCA
		private bool InitInternalInitial(GameEntity sourceEntity, GameEntity targetEntity, UniqueId archetypeId, ReagentItem reagentItem, CombatEffect effect, float effectiveEntityLevel, float abilityLevel, bool isSecondary, AlchemyPowerLevel alchemyPowerLevel)
		{
			return false;
		}

		// Token: 0x06005F27 RID: 24359 RVA: 0x0004475B File Offset: 0x0004295B
		private void InitLasting()
		{
		}

		// Token: 0x06005F28 RID: 24360 RVA: 0x00080115 File Offset: 0x0007E315
		private int GetResistAdjustedValuePerTick(int initialValue, float resistMultiplier)
		{
			if (initialValue >= 0)
			{
				return initialValue;
			}
			if (resistMultiplier >= 1f)
			{
				return initialValue;
			}
			return Mathf.Clamp(Mathf.FloorToInt((float)initialValue * resistMultiplier), int.MinValue, -1);
		}

		// Token: 0x06005F29 RID: 24361 RVA: 0x0008013B File Offset: 0x0007E33B
		private bool RandomCheck(int value)
		{
			return UnityEngine.Random.Range(1, 101) <= value;
		}

		// Token: 0x06005F2A RID: 24362 RVA: 0x001F89C0 File Offset: 0x001F6BC0
		private bool CheckDiminishingReturns(DiminishingReturnType type)
		{
			if (this.Positive || type == DiminishingReturnType.None || this.m_targetEntity.Type != GameEntityType.Npc)
			{
				return false;
			}
			this.m_diminishingReturnsDurationMultiplier = this.m_targetEntity.EffectController.GetDiminishingReturnMultiplier(type, this.m_record.InstanceId);
			if (this.m_diminishingReturnsDurationMultiplier < 1f)
			{
				this.Flags |= EffectApplicationFlags.Diminished;
			}
			return this.m_diminishingReturnsDurationMultiplier <= 0f;
		}

		// Token: 0x06005F2B RID: 24363 RVA: 0x0008014B File Offset: 0x0007E34B
		private void UpdateEntityCombatTimestamp(GameEntity entity)
		{
			if (entity && entity.Vitals)
			{
				entity.Vitals.UpdateLastCombatTimestamp();
			}
		}

		// Token: 0x06005F2C RID: 24364 RVA: 0x001F8A3C File Offset: 0x001F6C3C
		private void ApplyLasting()
		{
			if (!this.IsLasting)
			{
				return;
			}
			if (this.m_applicatorsToOverwrite != null)
			{
				for (int i = 0; i < this.m_applicatorsToOverwrite.Count; i++)
				{
					this.m_targetEntity.EffectController.RemoveApplicator(this.m_applicatorsToOverwrite[i]);
				}
				this.ResetApplicatorsToOverwrite();
			}
			this.SyncData = new EffectSyncData
			{
				InstanceId = this.m_record.InstanceId,
				ArchetypeId = this.m_record.ArchetypeId,
				CombatEffectReagentId = this.m_record.CombatEffectReagentId,
				ApplicatorName = this.m_record.SourceData.Name,
				SourceNetworkId = this.m_record.SourceNetworkId,
				Level = (byte)this.m_record.AbilityLevel,
				Duration = this.m_record.TimingData.Duration,
				ExpirationTime = this.m_expirationTime,
				Dismissible = this.m_effect.Expiration.CanDismiss,
				IsSecondary = this.m_record.IsSecondary,
				Diminished = this.Flags.HasBitFlag(EffectApplicationFlags.Diminished),
				SourceIsPlayer = (this.m_record.SourceData.Type == GameEntityType.Player),
				StackCount = ((this.m_record.StackCount > 1) ? new byte?((byte)this.m_record.StackCount) : null),
				TriggerCount = ((this.m_record.TriggerCount > 0) ? new byte?((byte)this.m_record.TriggerCount) : null),
				AlchemyPowerLevel = this.m_alchemyPowerLevel
			};
			this.ToggleLastingEffects(true);
		}

		// Token: 0x06005F2D RID: 24365 RVA: 0x001F8C08 File Offset: 0x001F6E08
		private void AddStackCounts(EffectRecord record)
		{
			if (record == null)
			{
				return;
			}
			int num = (record.StackCount > 0) ? record.StackCount : 1;
			this.m_stackTally += num;
		}

		// Token: 0x06005F2E RID: 24366 RVA: 0x0008016D File Offset: 0x0007E36D
		public void RemoveLasting()
		{
			this.ToggleLastingEffects(false);
		}

		// Token: 0x06005F2F RID: 24367 RVA: 0x001F8C40 File Offset: 0x001F6E40
		public bool UpdateExternal()
		{
			if (this.m_record == null)
			{
				return true;
			}
			this.m_record.TimingData.Elapsed += Time.deltaTime;
			if (this.ExpireFromTriggers())
			{
				return true;
			}
			if (this.m_record.TimingData.TickRate <= 0)
			{
				return this.m_record.TimingData.Elapsed >= (float)this.m_record.TimingData.Duration;
			}
			if (this.m_record.TimingData.TicksRemaining > 0 && this.m_record.TimingData.Elapsed >= this.m_record.TimingData.TimeOfNextTick)
			{
				EffectApplicationFlags effectApplicationFlags = EffectApplicationFlags.None;
				for (int i = 0; i < EffectApplicator.m_overTimeFlagsToReapply.Length; i++)
				{
					if (this.Flags.HasBitFlag(EffectApplicator.m_overTimeFlagsToReapply[i]))
					{
						effectApplicationFlags |= EffectApplicator.m_overTimeFlagsToReapply[i];
					}
				}
				this.Flags = effectApplicationFlags;
				if (this.m_record.HealthMod.Value < 0f)
				{
					this.m_immediate.AddThreat(-1f * this.m_record.HealthMod.Value);
				}
				else if (this.m_record.HealthMod.Value > 0f)
				{
					this.AddAoeThreatForPositive(this.m_record.HealthMod.Value, GlobalSettings.Values.Combat.HealThreatSettings.HotMultiplier);
				}
				this.ApplyVitalDeltas(false);
				bool flag = false;
				if (this.m_targetEntity && this.m_triggerOnHit && this.m_record.DamageChannelStatType != null)
				{
					this.m_targetEntity.EffectController.OnGetHit(this, this.m_record.DamageChannelStatType.Value);
					flag = (this.m_sourceEntity == this.m_targetEntity);
				}
				this.m_record.TimingData.TimeOfNextTick += (float)this.m_record.TimingData.TickRate;
				this.m_record.TimingData.TicksRemaining--;
				if (flag && this.ExpireFromTriggers())
				{
					return true;
				}
			}
			return this.m_record.TimingData.TicksRemaining <= 0;
		}

		// Token: 0x06005F30 RID: 24368 RVA: 0x001F8E74 File Offset: 0x001F7074
		private bool ExpireFromTriggers()
		{
			return this.m_record != null && this.m_effect != null && this.m_effect.Expiration != null && this.m_effect.Expiration.ExpireFromTriggers(this.m_record.TriggerCount, this.m_reagentItemTriggerCountMod);
		}

		// Token: 0x06005F31 RID: 24369 RVA: 0x0004475B File Offset: 0x0004295B
		private void ApplyVitalDeltas(bool isInitial)
		{
		}

		// Token: 0x06005F32 RID: 24370 RVA: 0x001F8EC8 File Offset: 0x001F70C8
		private void ToggleLastingEffects(bool adding)
		{
			if (this.m_effect == null || this.m_effect.Effects == null)
			{
				Debug.LogError("NULL m_effect.Effects! ArchetypeId: " + this.Key.ArchetypeId.Value + ", Applicator: " + this.Key.ApplicantName);
				return;
			}
			Effects effects = this.m_effect.Effects;
			bool flag = false;
			if (effects.HasStatusEffects)
			{
				byte? stackCount = (this.m_record.StackCount > 1) ? new byte?((byte)this.m_record.StackCount) : null;
				this.m_effect.ApplyStatusEffects(adding, this.m_targetEntity, this.m_reagentItem, stackCount);
			}
			if (effects.HasBehaviorEffects)
			{
				if (adding)
				{
					this.m_targetEntity.AddBehaviorFlags(this.m_record.BehaviorFlags);
				}
				else
				{
					flag = true;
				}
				if (this.m_targetEntity.Type == GameEntityType.Npc && this.m_sourceEntity && this.m_targetEntity.TargetController)
				{
					this.m_targetEntity.TargetController.BehaviorFlagsUpdated(this.m_sourceEntity, this.m_expirationTime, this.m_record.BehaviorFlags, adding);
				}
			}
			if (effects.HasCombatFlags)
			{
				if (adding)
				{
					this.m_targetEntity.EffectController.CombatFlags |= effects.CombatFlags;
				}
				else
				{
					flag = true;
				}
			}
			if (flag)
			{
				this.m_targetEntity.EffectController.RefreshFlags(this);
			}
		}

		// Token: 0x06005F33 RID: 24371 RVA: 0x0004475B File Offset: 0x0004295B
		private void ApplyInstant(VitalsEffect_Instant instant)
		{
		}

		// Token: 0x06005F34 RID: 24372 RVA: 0x001F9034 File Offset: 0x001F7234
		private void AddAoeThreatForPositive(float value, float externalMultiplier = 1f)
		{
			GameEntity targetEntity = this.m_targetEntity;
			GameEntity sourceEntity = this.m_sourceEntity;
			if (this.Negative || !targetEntity || !sourceEntity || sourceEntity.Type != GameEntityType.Player || targetEntity.Type != GameEntityType.Player || value == 0f || this.m_isConsumable)
			{
				return;
			}
			value *= GlobalSettings.Values.Combat.HealThreatSettings.OverallMultiplier;
			Collider[] colliders = Hits.Colliders25;
			int num = Physics.OverlapSphereNonAlloc(targetEntity.gameObject.transform.position, 25f, colliders, LayerMap.Detection.LayerMask, QueryTriggerInteraction.Ignore);
			for (int i = 0; i < num; i++)
			{
				GameEntity gameEntity;
				NpcTargetController npcTargetController;
				if (colliders[i] && DetectionCollider.TryGetEntityForCollider(colliders[i], out gameEntity) && gameEntity.Type == GameEntityType.Npc && gameEntity.TargetController != null && gameEntity.TargetController.TryGetAsType(out npcTargetController) && npcTargetController.HostileTargetCount > 0 && gameEntity.TargetController.IsHostileTo(sourceEntity.NetworkEntity, targetEntity.NetworkEntity) && !gameEntity.TargetController.IsLulled && (sourceEntity.gameObject.transform.position - gameEntity.gameObject.transform.position).sqrMagnitude <= 2500f && (!GlobalSettings.Values.Combat.HealThreatSettings.RequireLineOfSight || LineOfSight.NpcHasLineOfSight(gameEntity, targetEntity)))
				{
					float num2 = GlobalSettings.Values.Combat.HealThreatSettings.UnknownMultiplier;
					if (gameEntity.TargetController.InTargetList(targetEntity.NetworkEntity))
					{
						num2 = ((targetEntity != sourceEntity && gameEntity.TargetController.IsCurrentHostileTarget(targetEntity.NetworkEntity)) ? GlobalSettings.Values.Combat.HealThreatSettings.HostileMultiplier : GlobalSettings.Values.Combat.HealThreatSettings.KnownMultiplier);
					}
					float num3 = value * num2 * externalMultiplier;
					gameEntity.TargetController.AddThreat(sourceEntity, num3, num3, false);
				}
			}
		}

		// Token: 0x06005F35 RID: 24373 RVA: 0x00080176 File Offset: 0x0007E376
		private bool HasCombatFlag(CombatFlags flag, VitalsEffect_Instant instant)
		{
			return instant.Mods.CombatFlags.HasBitFlag(flag) || this.m_sourceEntity.EffectController.CombatFlags.HasBitFlag(flag);
		}

		// Token: 0x06005F36 RID: 24374 RVA: 0x001F9260 File Offset: 0x001F7460
		private void SendResults()
		{
			this.Results.TriggerOnAnimEvent = (this.m_isInstant && !this.m_isAutoAttack && this.m_sourceEntity && this.m_sourceEntity.Type == GameEntityType.Player);
			this.Results.Flags = this.Flags;
			Peer[] observersWithinRange = this.m_targetEntity.NetworkEntity.GetObserversWithinRange(25f);
			if (observersWithinRange == null || observersWithinRange.Length == 0)
			{
				if (observersWithinRange != null)
				{
					observersWithinRange.ReturnToPool();
				}
				return;
			}
			BitBuffer fromPool = BitBufferExtensions.GetFromPool();
			fromPool.AddHeader(this.m_targetEntity.NetworkEntity, OpCodes.ChatMessage, true);
			this.Results.PackData(fromPool);
			Packet packetFromBuffer_ReturnBufferToPool = fromPool.GetPacketFromBuffer_ReturnBufferToPool(PacketFlags.None);
			NetworkCommand networkCommand = NetworkCommandPool.GetNetworkCommand();
			networkCommand.Channel = NetworkChannel.CombatResults;
			networkCommand.Packet = packetFromBuffer_ReturnBufferToPool;
			networkCommand.Type = CommandType.BroadcastGroup;
			networkCommand.TargetGroup = observersWithinRange;
			GameManager.NetworkManager.AddCommandToQueue(networkCommand);
		}

		// Token: 0x06005F37 RID: 24375 RVA: 0x0004475B File Offset: 0x0004295B
		private void ModifyDurability(float absorbed)
		{
		}

		// Token: 0x04005221 RID: 21025
		public const float kResultsRangeSquared = 625f;

		// Token: 0x04005222 RID: 21026
		private const float kDefenseAngleThreshold = 40f;

		// Token: 0x04005223 RID: 21027
		public const float kResultsRange = 25f;

		// Token: 0x04005224 RID: 21028
		public const int kArmorClassRoll = 100;

		// Token: 0x04005225 RID: 21029
		private static Dictionary<UniqueId, RoleFlankingBonus> m_roleFlankingBonuses = null;

		// Token: 0x04005226 RID: 21030
		private static readonly Dictionary<UniqueId, ArchetypeInstance> m_augmentedWeaponInstances = new Dictionary<UniqueId, ArchetypeInstance>(default(UniqueIdComparer));

		// Token: 0x04005227 RID: 21031
		private bool m_inPool;

		// Token: 0x04005228 RID: 21032
		private EffectRecord m_record;

		// Token: 0x04005229 RID: 21033
		private DateTime m_expirationTime = DateTime.MinValue;

		// Token: 0x0400522A RID: 21034
		private CombatEffect m_effect;

		// Token: 0x0400522B RID: 21035
		private ReagentItem m_reagentItem;

		// Token: 0x0400522C RID: 21036
		private int m_reagentItemTriggerCountMod;

		// Token: 0x0400522D RID: 21037
		private HuntingLogEntry m_offensiveHuntingLogEntry;

		// Token: 0x0400522E RID: 21038
		private HuntingLogEntry m_defensiveHuntingLogEntry;

		// Token: 0x0400522F RID: 21039
		private GameEntity m_sourceEntity;

		// Token: 0x04005230 RID: 21040
		private GameEntity m_targetEntity;

		// Token: 0x04005231 RID: 21041
		private int m_sourceGroupedLevel;

		// Token: 0x04005232 RID: 21042
		private int m_targetGroupedLevel;

		// Token: 0x04005233 RID: 21043
		private int m_stackTally;

		// Token: 0x04005234 RID: 21044
		private bool m_positive;

		// Token: 0x04005235 RID: 21045
		private float m_resistDurationMultiplier = 1f;

		// Token: 0x04005236 RID: 21046
		private float m_diminishingReturnsDurationMultiplier = 1f;

		// Token: 0x04005237 RID: 21047
		private bool m_triggerOnHit;

		// Token: 0x04005238 RID: 21048
		private bool m_isAbility;

		// Token: 0x04005239 RID: 21049
		private bool m_isAutoAttack;

		// Token: 0x0400523A RID: 21050
		private bool m_isInstant;

		// Token: 0x0400523B RID: 21051
		private bool m_isConsumable;

		// Token: 0x0400523C RID: 21052
		private bool m_isPlayerTooLow;

		// Token: 0x0400523D RID: 21053
		private bool m_applySlowDebuff;

		// Token: 0x0400523E RID: 21054
		private bool m_bypassLevelDeltaCombatAdjustments;

		// Token: 0x0400523F RID: 21055
		private bool m_applyPvpModifiers;

		// Token: 0x04005240 RID: 21056
		private AlchemyPowerLevel m_alchemyPowerLevel;

		// Token: 0x04005241 RID: 21057
		private readonly EffectApplicator.ImmediateData m_immediate = new EffectApplicator.ImmediateData();

		// Token: 0x04005242 RID: 21058
		private readonly CachedHandHeldItem m_mainHandCache = new CachedHandHeldItem(true);

		// Token: 0x04005243 RID: 21059
		private readonly CachedHandHeldItem m_offHandCache = new CachedHandHeldItem(false);

		// Token: 0x04005244 RID: 21060
		private List<EffectApplicator> m_applicatorsToOverwrite;

		// Token: 0x0400524A RID: 21066
		private EffectApplicationResult m_results;

		// Token: 0x0400524B RID: 21067
		private bool m_isLasting;

		// Token: 0x0400524C RID: 21068
		private float? m_angleTo;

		// Token: 0x0400524E RID: 21070
		private static readonly EffectApplicationFlags[] m_overTimeFlagsToReapply = new EffectApplicationFlags[]
		{
			EffectApplicationFlags.Positive,
			EffectApplicationFlags.Applied,
			EffectApplicationFlags.OverTime,
			EffectApplicationFlags.Diminished
		};

		// Token: 0x02000C11 RID: 3089
		private class ImmediateData
		{
			// Token: 0x170016A3 RID: 5795
			// (get) Token: 0x06005F3A RID: 24378 RVA: 0x000801A3 File Offset: 0x0007E3A3
			public float HealthValue
			{
				get
				{
					return this.m_health.Value;
				}
			}

			// Token: 0x170016A4 RID: 5796
			// (get) Token: 0x06005F3B RID: 24379 RVA: 0x000801B0 File Offset: 0x0007E3B0
			public float HealthWound
			{
				get
				{
					return this.m_health.Wound;
				}
			}

			// Token: 0x170016A5 RID: 5797
			// (get) Token: 0x06005F3C RID: 24380 RVA: 0x000801BD File Offset: 0x0007E3BD
			public float StaminaValue
			{
				get
				{
					return this.m_stamina.Value;
				}
			}

			// Token: 0x170016A6 RID: 5798
			// (get) Token: 0x06005F3D RID: 24381 RVA: 0x000801CA File Offset: 0x0007E3CA
			public float StaminaWound
			{
				get
				{
					return this.m_stamina.Wound;
				}
			}

			// Token: 0x170016A7 RID: 5799
			// (get) Token: 0x06005F3E RID: 24382 RVA: 0x000801D7 File Offset: 0x0007E3D7
			public float Threat
			{
				get
				{
					return this.m_threat;
				}
			}

			// Token: 0x170016A8 RID: 5800
			// (get) Token: 0x06005F3F RID: 24383 RVA: 0x000801DF File Offset: 0x0007E3DF
			public float Absorption
			{
				get
				{
					return this.m_absorption;
				}
			}

			// Token: 0x06005F40 RID: 24384 RVA: 0x000801E7 File Offset: 0x0007E3E7
			public ImmediateData()
			{
				this.m_health = new VitalModification();
				this.m_stamina = new VitalModification();
			}

			// Token: 0x06005F41 RID: 24385 RVA: 0x00080205 File Offset: 0x0007E405
			public void Reset()
			{
				this.m_health.Reset();
				this.m_stamina.Reset();
				this.m_threat = 0f;
				this.m_absorption = 0f;
			}

			// Token: 0x06005F42 RID: 24386 RVA: 0x00080233 File Offset: 0x0007E433
			public void AddVitalValue(EffectResourceType resourceType, float value)
			{
				this.AddVitalModification(resourceType, false, value);
			}

			// Token: 0x06005F43 RID: 24387 RVA: 0x0008023E File Offset: 0x0007E43E
			public void AddWoundValue(EffectResourceType resourceType, float value)
			{
				this.AddVitalModification(resourceType, true, value);
			}

			// Token: 0x06005F44 RID: 24388 RVA: 0x001F93D8 File Offset: 0x001F75D8
			private void AddVitalModification(EffectResourceType resourceType, bool wounds, float value)
			{
				VitalModification vitalModification = null;
				if (resourceType != EffectResourceType.Health)
				{
					if (resourceType == EffectResourceType.Stamina)
					{
						vitalModification = this.m_stamina;
					}
				}
				else
				{
					vitalModification = this.m_health;
				}
				if (vitalModification != null)
				{
					if (wounds)
					{
						vitalModification.Wound += value;
						return;
					}
					vitalModification.Value += value;
				}
			}

			// Token: 0x06005F45 RID: 24389 RVA: 0x00080249 File Offset: 0x0007E449
			public void AddThreat(float value)
			{
				this.m_threat += value;
			}

			// Token: 0x06005F46 RID: 24390 RVA: 0x00080259 File Offset: 0x0007E459
			public void AddAbsorption(float value)
			{
				this.m_absorption += value;
			}

			// Token: 0x0400524F RID: 21071
			private readonly VitalModification m_health;

			// Token: 0x04005250 RID: 21072
			private readonly VitalModification m_stamina;

			// Token: 0x04005251 RID: 21073
			private float m_threat;

			// Token: 0x04005252 RID: 21074
			private float m_absorption;
		}
	}
}

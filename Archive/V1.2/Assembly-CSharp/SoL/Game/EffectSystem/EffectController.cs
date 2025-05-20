using System;
using System.Collections.Generic;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using SoL.Managers;
using SoL.Networking;
using SoL.Networking.Database;
using SoL.Networking.RPC;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C33 RID: 3123
	public class EffectController : GameEntityComponent
	{
		// Token: 0x17001728 RID: 5928
		// (get) Token: 0x06006060 RID: 24672 RVA: 0x00080E9D File Offset: 0x0007F09D
		// (set) Token: 0x06006061 RID: 24673 RVA: 0x00080EA5 File Offset: 0x0007F0A5
		public CombatFlags CombatFlags { get; set; }

		// Token: 0x06006062 RID: 24674 RVA: 0x00080EAE File Offset: 0x0007F0AE
		private void Awake()
		{
			if (base.GameEntity != null)
			{
				base.GameEntity.EffectController = this;
			}
		}

		// Token: 0x06006063 RID: 24675 RVA: 0x001FC74C File Offset: 0x001FA94C
		private void Update()
		{
			if (this.m_removeEffectsReason != EffectController.RemoveEffectsReason.None)
			{
				for (int i = 0; i < this.m_lastingApplicatorsByKey.Count; i++)
				{
					if (this.m_removeEffectsReason == EffectController.RemoveEffectsReason.Death || (this.m_removeEffectsReason == EffectController.RemoveEffectsReason.Unconscious && !this.m_lastingApplicatorsByKey[i].PreserverOnUnconscious))
					{
						this.RemoveApplicator(this.m_lastingApplicatorsByKey[i]);
						i--;
					}
				}
				this.m_removeEffectsReason = EffectController.RemoveEffectsReason.None;
				return;
			}
			this.UpdateLastingApplicators();
		}

		// Token: 0x06006064 RID: 24676 RVA: 0x00080ECA File Offset: 0x0007F0CA
		private void OnDestroy()
		{
			this.CleanupAuras();
			this.CleanupCollection();
			if (NullifyMemoryLeakSettings.CleanEffectController)
			{
				Dictionary<DiminishingReturnType, DiminishingReturnCalculator> diminishingReturns = this.m_diminishingReturns;
				if (diminishingReturns != null)
				{
					diminishingReturns.Clear();
				}
				this.m_diminishingReturns = null;
				this.m_characterRecord = null;
			}
		}

		// Token: 0x06006065 RID: 24677 RVA: 0x001FC7C0 File Offset: 0x001FA9C0
		public void Init(CharacterRecord record)
		{
			this.m_characterRecord = record;
			if (base.GameEntity.Type == GameEntityType.Npc)
			{
				this.m_diminishingReturns = new Dictionary<DiminishingReturnType, DiminishingReturnCalculator>(default(DiminishingReturnTypeComparer));
				for (int i = 0; i < DiminishingReturnTypeExtensions.DiminishingReturnTypes.Length; i++)
				{
					DiminishingReturnType diminishingReturnType = DiminishingReturnTypeExtensions.DiminishingReturnTypes[i];
					if (diminishingReturnType != DiminishingReturnType.None)
					{
						this.m_diminishingReturns.Add(diminishingReturnType, new DiminishingReturnCalculator(diminishingReturnType));
					}
				}
			}
			if (record.Effects != null && record.Effects.Count > 0)
			{
				for (int j = 0; j < record.Effects.Count; j++)
				{
					if (!this.ApplyApplicator(record.Effects[j]))
					{
						record.Effects.RemoveAt(j);
						j--;
					}
				}
			}
		}

		// Token: 0x06006066 RID: 24678 RVA: 0x001FC87C File Offset: 0x001FAA7C
		public float GetDiminishingReturnMultiplier(DiminishingReturnType diminishingReturnType, UniqueId instanceId)
		{
			DiminishingReturnCalculator diminishingReturnCalculator;
			if (!this.m_diminishingReturns.TryGetValue(diminishingReturnType, out diminishingReturnCalculator))
			{
				return 1f;
			}
			return diminishingReturnCalculator.GetMultiplier(instanceId);
		}

		// Token: 0x06006067 RID: 24679 RVA: 0x001FC8A8 File Offset: 0x001FAAA8
		private void UpdateLastingApplicators()
		{
			for (int i = 0; i < this.m_lastingApplicatorsByKey.Count; i++)
			{
				EffectApplicator effectApplicator = this.m_lastingApplicatorsByKey[i];
				if (effectApplicator.UpdateExternal())
				{
					this.RemoveApplicator(effectApplicator);
					i--;
				}
			}
		}

		// Token: 0x06006068 RID: 24680 RVA: 0x001FC8EC File Offset: 0x001FAAEC
		public void RefreshFlags(EffectApplicator toExclude)
		{
			BehaviorEffectTypeFlags behaviorEffectTypeFlags = BehaviorEffectTypeFlags.None;
			CombatFlags combatFlags = CombatFlags.None;
			for (int i = 0; i < this.m_lastingApplicatorsByKey.Count; i++)
			{
				EffectApplicator effectApplicator = this.m_lastingApplicatorsByKey[i];
				if (toExclude == null || effectApplicator != toExclude)
				{
					behaviorEffectTypeFlags |= effectApplicator.BehaviorFlags;
					combatFlags |= effectApplicator.CombatFlags;
				}
			}
			base.GameEntity.SetBehaviorFlags(behaviorEffectTypeFlags);
			this.CombatFlags = combatFlags;
		}

		// Token: 0x06006069 RID: 24681 RVA: 0x001FC94C File Offset: 0x001FAB4C
		public void RemoveBehaviorApplicationsForDamage(EffectApplicator toExclude)
		{
			for (int i = 0; i < this.m_lastingApplicatorsByKey.Count; i++)
			{
				EffectApplicator effectApplicator = this.m_lastingApplicatorsByKey[i];
				if (effectApplicator != toExclude && effectApplicator.BehaviorFlags.RemoveOnDamage())
				{
					this.RemoveApplicator(effectApplicator);
					i--;
				}
			}
		}

		// Token: 0x0600606A RID: 24682 RVA: 0x001FC998 File Offset: 0x001FAB98
		public void RemoveCrowdControlForSummon()
		{
			for (int i = 0; i < this.m_lastingApplicatorsByKey.Count; i++)
			{
				EffectApplicator effectApplicator = this.m_lastingApplicatorsByKey[i];
				if (effectApplicator.BehaviorFlags.RemoveOnSummon())
				{
					this.RemoveApplicator(effectApplicator);
					i--;
				}
				else if (effectApplicator.CombatEffect != null && effectApplicator.CombatEffect.Polarity == Polarity.Negative && effectApplicator.CombatEffect.Effects != null && effectApplicator.CombatEffect.Effects.HasStatusEffects && effectApplicator.CombatEffect.Effects.StatusEffect != null && effectApplicator.CombatEffect.Effects.StatusEffect.Values != null)
				{
					foreach (StatusEffect.StatusEffectValue statusEffectValue in effectApplicator.CombatEffect.Effects.StatusEffect.Values)
					{
						if (statusEffectValue.Type.IsValid() && (statusEffectValue.Type == StatType.Movement || statusEffectValue.Type == StatType.CombatMovement) && statusEffectValue.Value * -1 <= -100)
						{
							this.RemoveApplicator(effectApplicator);
							i--;
							break;
						}
					}
				}
			}
		}

		// Token: 0x0600606B RID: 24683 RVA: 0x001FCABC File Offset: 0x001FACBC
		public void DismissEffectRequest(UniqueId instanceId)
		{
			EffectApplicator applicator;
			if (this.m_lastingApplicatorsByInstanceId.TryGetValue(instanceId, out applicator))
			{
				this.RemoveApplicator(applicator);
			}
		}

		// Token: 0x0600606C RID: 24684 RVA: 0x00080EFE File Offset: 0x0007F0FE
		public void RemoveEffectsForDeath()
		{
			if (GameManager.IsServer)
			{
				this.m_removeEffectsReason = EffectController.RemoveEffectsReason.Death;
			}
		}

		// Token: 0x0600606D RID: 24685 RVA: 0x00080F0E File Offset: 0x0007F10E
		public void RemoveEffectsForUnconscious()
		{
			if (GameManager.IsServer)
			{
				this.m_removeEffectsReason = EffectController.RemoveEffectsReason.Unconscious;
			}
		}

		// Token: 0x17001729 RID: 5929
		// (get) Token: 0x0600606E RID: 24686 RVA: 0x00080F1E File Offset: 0x0007F11E
		// (set) Token: 0x0600606F RID: 24687 RVA: 0x00080F26 File Offset: 0x0007F126
		public AuraController SourceAura
		{
			get
			{
				return this.m_sourceAura;
			}
			set
			{
				if (this.m_sourceAura != null && this.m_sourceAura != value)
				{
					this.m_sourceAura.CancelAura();
					StaticPool<AuraController>.ReturnToPool(this.m_sourceAura);
					this.m_sourceAura = null;
				}
				this.m_sourceAura = value;
			}
		}

		// Token: 0x1700172A RID: 5930
		// (get) Token: 0x06006070 RID: 24688 RVA: 0x00080F5D File Offset: 0x0007F15D
		// (set) Token: 0x06006071 RID: 24689 RVA: 0x00080F65 File Offset: 0x0007F165
		private AuraController ActiveAura
		{
			get
			{
				return this.m_activeAura;
			}
			set
			{
				if (this.m_activeAura == value)
				{
					return;
				}
				if (this.m_activeAura != null)
				{
					this.ToggleAura(false);
				}
				this.m_activeAura = value;
				if (this.m_activeAura != null)
				{
					this.ToggleAura(true);
				}
			}
		}

		// Token: 0x06006072 RID: 24690 RVA: 0x00080F96 File Offset: 0x0007F196
		private void CleanupAuras()
		{
			this.SourceAura = null;
			if (NullifyMemoryLeakSettings.CleanEffectController)
			{
				this.m_sourceAura = null;
				this.m_activeAura = null;
				Dictionary<UniqueId, AuraController> appliedAuras = this.m_appliedAuras;
				if (appliedAuras != null)
				{
					appliedAuras.Clear();
				}
				this.m_appliedAuras = null;
			}
		}

		// Token: 0x06006073 RID: 24691 RVA: 0x001FCAE0 File Offset: 0x001FACE0
		private void ToggleAura(bool adding)
		{
			if (this.m_activeAura == null || this.m_activeAura.SyncData == null || !this.m_activeAura.AuraAbility)
			{
				return;
			}
			this.m_activeAura.AuraAbility.CombatEffect.ApplyStatusEffects(adding, base.GameEntity, null, null);
			if (adding)
			{
				base.GameEntity.VitalsReplicator.Effects.Add(this.m_activeAura.SyncData.Value.InstanceId, this.m_activeAura.SyncData.Value);
				return;
			}
			base.GameEntity.VitalsReplicator.Effects.Remove(this.m_activeAura.SyncData.Value.InstanceId);
		}

		// Token: 0x06006074 RID: 24692 RVA: 0x001FCBB8 File Offset: 0x001FADB8
		public void AddAppliedAura(AuraController aura)
		{
			if (this.m_appliedAuras == null)
			{
				this.m_appliedAuras = new Dictionary<UniqueId, AuraController>(default(UniqueIdComparer));
			}
			if (!this.m_appliedAuras.ContainsKey(aura.AuraInstanceId))
			{
				this.m_appliedAuras.Add(aura.AuraInstanceId, aura);
			}
			this.RefreshActiveAura();
		}

		// Token: 0x06006075 RID: 24693 RVA: 0x00080FCC File Offset: 0x0007F1CC
		public void RemoveAppliedAura(AuraController aura)
		{
			if (this.m_appliedAuras != null && this.m_appliedAuras.Remove(aura.AuraInstanceId) && aura == this.ActiveAura)
			{
				this.RefreshActiveAura();
			}
		}

		// Token: 0x06006076 RID: 24694 RVA: 0x001FCC14 File Offset: 0x001FAE14
		private void RefreshActiveAura()
		{
			if (this.m_appliedAuras == null)
			{
				return;
			}
			AuraController activeAura = null;
			int num = 0;
			foreach (KeyValuePair<UniqueId, AuraController> keyValuePair in this.m_appliedAuras)
			{
				if (keyValuePair.Value.SourceEntity && !keyValuePair.Value.Paused && keyValuePair.Value.IsValidForStance(base.GameEntity.Vitals.Stance))
				{
					if (keyValuePair.Value.SourceEntity == base.GameEntity)
					{
						activeAura = keyValuePair.Value;
						break;
					}
					if (!base.GameEntity.CharacterData.GroupId.IsEmpty && base.GameEntity.CharacterData.GroupId == keyValuePair.Value.SourceEntity.CharacterData.GroupId && keyValuePair.Value.SyncData != null && (int)keyValuePair.Value.SyncData.Value.Level >= num)
					{
						activeAura = keyValuePair.Value;
						num = (int)keyValuePair.Value.SyncData.Value.Level;
					}
				}
			}
			this.ActiveAura = activeAura;
		}

		// Token: 0x06006077 RID: 24695 RVA: 0x00080FF8 File Offset: 0x0007F1F8
		public void NearbyGroupMembersUpdated()
		{
			AuraController sourceAura = this.SourceAura;
			if (sourceAura == null)
			{
				return;
			}
			sourceAura.ExternalUpdate();
		}

		// Token: 0x06006078 RID: 24696 RVA: 0x001FCD98 File Offset: 0x001FAF98
		public void RefreshAuras()
		{
			if (this.SourceAura != null)
			{
				Stance stance = base.GameEntity.Vitals.Stance;
				if (this.SourceAura.ShouldOutrightCancel(stance))
				{
					this.SourceAura = null;
				}
				else if (this.SourceAura.Paused)
				{
					if (!this.SourceAura.CanResume(base.GameEntity))
					{
						this.SourceAura = null;
					}
					else if (!this.SourceAura.ShouldChangeStateForStance(stance))
					{
						this.SourceAura.Paused = false;
					}
				}
				else if (this.SourceAura.ShouldChangeStateForStance(stance))
				{
					if (this.SourceAura.CanPause(base.GameEntity))
					{
						this.SourceAura.Paused = true;
					}
					else
					{
						this.SourceAura = null;
					}
				}
				else if (base.GameEntity.Vitals.Stance == Stance.Combat && this.SourceAura.AuraAbility.Mastery.Id != base.GameEntity.CharacterData.ActiveMasteryId)
				{
					this.SourceAura = null;
				}
			}
			this.RefreshActiveAura();
		}

		// Token: 0x1700172B RID: 5931
		// (get) Token: 0x06006079 RID: 24697 RVA: 0x0008100A File Offset: 0x0007F20A
		internal DictionaryList<EffectKey, EffectApplicator> LastingApplicatorsByKey
		{
			get
			{
				return this.m_lastingApplicatorsByKey;
			}
		}

		// Token: 0x0600607A RID: 24698 RVA: 0x00081012 File Offset: 0x0007F212
		public bool TryGetLastingApplicatorByKey(EffectKey key, out EffectApplicator applicator)
		{
			return this.m_lastingApplicatorsByKey.TryGetValue(key, out applicator);
		}

		// Token: 0x0600607B RID: 24699 RVA: 0x001FCEAC File Offset: 0x001FB0AC
		private void CleanupCollection()
		{
			if (NullifyMemoryLeakSettings.CleanEffectController)
			{
				foreach (KeyValuePair<UniqueId, EffectApplicator> keyValuePair in this.m_lastingApplicatorsByInstanceId)
				{
					StaticPool<EffectApplicator>.ReturnToPool(keyValuePair.Value);
				}
				this.m_lastingApplicatorsByInstanceId.Clear();
				this.m_lastingApplicatorsByKey.Clear();
				this.m_activeTriggers.Clear();
			}
		}

		// Token: 0x0600607C RID: 24700 RVA: 0x001FCF2C File Offset: 0x001FB12C
		private bool ApplyApplicator(EffectRecord record)
		{
			if (record == null || record.ArchetypeId.IsEmpty || record.EffectSource == null || record.CombatEffect == null)
			{
				return false;
			}
			if (record.TimingData.Elapsed >= (float)record.TimingData.Duration)
			{
				return false;
			}
			EffectApplicator fromPool = StaticPool<EffectApplicator>.GetFromPool();
			if (fromPool.Init(base.GameEntity, record))
			{
				this.AddApplicator(fromPool, false);
				return true;
			}
			StaticPool<EffectApplicator>.ReturnToPool(fromPool);
			return false;
		}

		// Token: 0x0600607D RID: 24701 RVA: 0x001FCFA4 File Offset: 0x001FB1A4
		public EffectApplicationFlags ApplyEffect(ExecutionCache cache, CombatEffect effect, bool isSecondary)
		{
			return this.ApplyEffectInternal(cache.SourceEntity, cache.ArchetypeId, null, effect, cache.MasteryLevel, cache.AbilityLevel, isSecondary, cache, null);
		}

		// Token: 0x0600607E RID: 24702 RVA: 0x001FCFDC File Offset: 0x001FB1DC
		public EffectApplicationFlags ApplyEffect(GameEntity sourceEntity, UniqueId archetypeId, CombatEffect effect, float masteryLevel, float abilityLevel, bool isSecondary, bool selfOverride = false)
		{
			EffectSourceOverride? effectSourceOverride = null;
			if (selfOverride && base.GameEntity && base.GameEntity.NetworkEntity && base.GameEntity.CharacterData)
			{
				effectSourceOverride = new EffectSourceOverride?(new EffectSourceOverride
				{
					Entity = base.GameEntity,
					Id = base.GameEntity.NetworkEntity.NetworkId.Value,
					Name = base.GameEntity.CharacterData.Name.Value
				});
			}
			return this.ApplyEffectInternal(sourceEntity, archetypeId, null, effect, masteryLevel, abilityLevel, isSecondary, null, effectSourceOverride);
		}

		// Token: 0x0600607F RID: 24703 RVA: 0x001FD094 File Offset: 0x001FB294
		private EffectApplicationFlags ApplyEffectWithReagent(GameEntity sourceEntity, UniqueId archetypeId, ReagentItem reagentItem, CombatEffect effect, float masteryLevel, float abilityLevel, bool isSecondary, EffectSourceOverride? effectSourceOverride)
		{
			return this.ApplyEffectInternal(sourceEntity, archetypeId, reagentItem, effect, masteryLevel, abilityLevel, isSecondary, null, effectSourceOverride);
		}

		// Token: 0x06006080 RID: 24704 RVA: 0x001FD0B8 File Offset: 0x001FB2B8
		private EffectApplicationFlags ApplyEffectInternal(GameEntity sourceEntity, UniqueId archetypeId, ReagentItem reagentItem, CombatEffect effect, float masteryLevel, float abilityLevel, bool isSecondary, ExecutionCache executionCache, EffectSourceOverride? effectSourceOverride)
		{
			if (effect == null)
			{
				Debug.LogWarning("Effect was null for ArcheTypeId " + archetypeId.ToString());
				return EffectApplicationFlags.None;
			}
			GameEntity gameEntity = base.GameEntity;
			EffectApplicator fromPool = StaticPool<EffectApplicator>.GetFromPool();
			if ((executionCache == null) ? fromPool.Init(sourceEntity, gameEntity, archetypeId, reagentItem, effect, masteryLevel, abilityLevel, isSecondary, effectSourceOverride) : fromPool.Init(gameEntity, executionCache, effect, isSecondary))
			{
				gameEntity.EffectController.AddApplicator(fromPool, true);
				if (fromPool.PostRefreshFlags)
				{
					this.RefreshFlags(null);
				}
				return fromPool.Flags;
			}
			EffectApplicationFlags flags = fromPool.Flags;
			StaticPool<EffectApplicator>.ReturnToPool(fromPool);
			return flags;
		}

		// Token: 0x06006081 RID: 24705 RVA: 0x001FD14C File Offset: 0x001FB34C
		private void AddApplicator(EffectApplicator applicator, bool addToRecord)
		{
			if (applicator == null)
			{
				Debug.LogError("NULL applicator (AddApplicator)");
				return;
			}
			EffectKey key = applicator.Key;
			if (key.ArchetypeId.IsEmpty)
			{
				Debug.LogError("NULL applicator.Key.ArchetypeId (AddApplicator)");
				return;
			}
			if (applicator.Record == null)
			{
				string str = "NULL applicator.Record (AddApplicator) ";
				key = applicator.Key;
				Debug.LogError(str + key.ArchetypeId.Value + " " + applicator.Key.ApplicantName);
				return;
			}
			if (applicator.CombatEffect == null)
			{
				string str2 = "NULL applicator.CombatEffect (AddApplicator) ";
				key = applicator.Key;
				Debug.LogError(str2 + key.ArchetypeId.Value + " " + applicator.Key.ApplicantName);
				return;
			}
			this.m_lastingApplicatorsByKey.Add(applicator.Key, applicator);
			this.m_lastingApplicatorsByInstanceId.Add(applicator.Record.InstanceId, applicator);
			if (applicator.ContainsTrigger)
			{
				this.m_activeTriggers.Add(applicator);
			}
			if (addToRecord && base.GameEntity.Type == GameEntityType.Player && this.m_characterRecord != null)
			{
				if (this.m_characterRecord.Effects == null)
				{
					this.m_characterRecord.Effects = new List<EffectRecord>
					{
						applicator.Record
					};
				}
				else
				{
					this.m_characterRecord.Effects.Add(applicator.Record);
				}
			}
			base.GameEntity.VitalsReplicator.Effects.Add(applicator.SyncData.InstanceId, applicator.SyncData);
		}

		// Token: 0x06006082 RID: 24706 RVA: 0x001FD2C0 File Offset: 0x001FB4C0
		internal void RemoveApplicator(EffectApplicator applicator)
		{
			if (applicator == null)
			{
				Debug.LogError("NULL applicator (RemoveApplicator)");
				return;
			}
			EffectKey key = applicator.Key;
			if (key.ArchetypeId.IsEmpty)
			{
				Debug.LogError("NULL applicator.Key.ArchetypeId (RemoveApplicator)");
				return;
			}
			EffectKey key2 = applicator.Key;
			if (applicator.Record == null)
			{
				string str = "NULL applicator.Record (RemoveApplicator) ";
				key = applicator.Key;
				Debug.LogError(str + key.ArchetypeId.Value + " " + applicator.Key.ApplicantName);
				return;
			}
			EffectRecord record = applicator.Record;
			UniqueId instanceId = record.InstanceId;
			if (applicator.CombatEffect == null)
			{
				string str2 = "NULL applicator.CombatEffect (RemoveApplicator) ";
				key = applicator.Key;
				Debug.LogError(str2 + key.ArchetypeId.Value + " " + applicator.Key.ApplicantName);
				return;
			}
			applicator.RemoveLasting();
			DictionaryList<EffectKey, EffectApplicator> lastingApplicatorsByKey = this.m_lastingApplicatorsByKey;
			if (lastingApplicatorsByKey != null)
			{
				lastingApplicatorsByKey.Remove(key2);
			}
			Dictionary<UniqueId, EffectApplicator> lastingApplicatorsByInstanceId = this.m_lastingApplicatorsByInstanceId;
			if (lastingApplicatorsByInstanceId != null)
			{
				lastingApplicatorsByInstanceId.Remove(instanceId);
			}
			if (applicator.ContainsTrigger)
			{
				this.m_activeTriggers.Remove(applicator);
			}
			if (base.GameEntity.Type == GameEntityType.Player)
			{
				CharacterRecord characterRecord = this.m_characterRecord;
				if (characterRecord != null)
				{
					List<EffectRecord> effects = characterRecord.Effects;
					if (effects != null)
					{
						effects.Remove(record);
					}
				}
			}
			base.GameEntity.VitalsReplicator.Effects.Remove(applicator.SyncData.InstanceId);
			StaticPool<EffectApplicator>.ReturnToPool(applicator);
		}

		// Token: 0x06006083 RID: 24707 RVA: 0x00081021 File Offset: 0x0007F221
		public void OnGetHit(EffectApplicator applicator, StatType damageChannel)
		{
			this.CheckTriggers(applicator, damageChannel, true);
		}

		// Token: 0x06006084 RID: 24708 RVA: 0x0008102C File Offset: 0x0007F22C
		public void OnPerformHit(EffectApplicator applicator, StatType damageChannel)
		{
			this.CheckTriggers(applicator, damageChannel, false);
		}

		// Token: 0x06006085 RID: 24709 RVA: 0x001FD420 File Offset: 0x001FB620
		private void CheckTriggers(EffectApplicator applicator, StatType damageChannel, bool isOnGetHit)
		{
			DamageCategoriesForTriggerType damageCategoryForTriggerType = this.GetDamageCategoryForTriggerType(damageChannel);
			if (damageCategoryForTriggerType == DamageCategoriesForTriggerType.None)
			{
				return;
			}
			for (int i = 0; i < this.m_activeTriggers.Count; i++)
			{
				EffectApplicator effectApplicator = this.m_activeTriggers[i];
				if (effectApplicator != null)
				{
					if (effectApplicator.CombatEffect == null || effectApplicator.Record == null)
					{
						EffectSyncData syncData = effectApplicator.SyncData;
						BaseArchetype baseArchetype;
						if (!syncData.ArchetypeId.IsEmpty && InternalGameDatabase.Archetypes.TryGetItem(effectApplicator.SyncData.ArchetypeId, out baseArchetype))
						{
							Debug.LogWarning("Error with " + baseArchetype.DisplayName + " not having an EffectRecord on a triggered effect? [FIRST]");
						}
					}
					else if (effectApplicator.CombatEffect.TriggerParams.ValidTrigger(isOnGetHit, damageCategoryForTriggerType, applicator.Flags) && effectApplicator.CombatEffect.Expiration.HasRemainingTriggers(effectApplicator.Record.TriggerCount, effectApplicator.ReagentItem))
					{
						this.ApplyTriggeredEffect(isOnGetHit, applicator, effectApplicator);
						if (effectApplicator.Record == null)
						{
							EffectSyncData syncData = effectApplicator.SyncData;
							BaseArchetype baseArchetype2;
							if (!syncData.ArchetypeId.IsEmpty && InternalGameDatabase.Archetypes.TryGetItem(effectApplicator.SyncData.ArchetypeId, out baseArchetype2))
							{
								Debug.LogWarning("Error with " + baseArchetype2.DisplayName + " not having an EffectRecord on a triggered effect? [SECOND]");
							}
						}
						else
						{
							effectApplicator.Record.TriggerCount++;
							if (effectApplicator.Record.TriggerCount >= 0)
							{
								NetworkEntityRpcs networkEntityRpcs = base.GameEntity.NetworkEntity.RpcHandler as NetworkEntityRpcs;
								if (networkEntityRpcs)
								{
									networkEntityRpcs.UpdateLastingEffectTriggerCount(effectApplicator.SyncData.InstanceId, (byte)effectApplicator.Record.TriggerCount);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06006086 RID: 24710 RVA: 0x001FD5DC File Offset: 0x001FB7DC
		private void ApplyTriggeredEffect(bool isOnGetHit, EffectApplicator sourceApplicator, EffectApplicator triggeredApplicator)
		{
			if (!triggeredApplicator.TriggeredEffect)
			{
				return;
			}
			GameEntity gameEntity;
			if (triggeredApplicator.CombatEffect.TriggerParams.TargetSelf)
			{
				gameEntity = base.GameEntity;
			}
			else if (isOnGetHit)
			{
				gameEntity = sourceApplicator.SourceEntity;
			}
			else
			{
				gameEntity = sourceApplicator.TargetEntity;
			}
			if (!gameEntity)
			{
				return;
			}
			EffectSourceOverride? effectSourceOverride = null;
			if (triggeredApplicator.SourceOverride != null)
			{
				effectSourceOverride = new EffectSourceOverride?(triggeredApplicator.SourceOverride.Value);
			}
			else if (base.GameEntity && base.GameEntity.NetworkEntity && base.GameEntity.NetworkEntity.NetworkId.Value != triggeredApplicator.Record.SourceNetworkId)
			{
				effectSourceOverride = new EffectSourceOverride?(new EffectSourceOverride
				{
					Id = triggeredApplicator.Record.SourceNetworkId,
					Name = triggeredApplicator.Record.SourceData.Name,
					Entity = triggeredApplicator.SourceEntity
				});
			}
			gameEntity.EffectController.ApplyEffectWithReagent(base.GameEntity, triggeredApplicator.TriggeredEffect.Id, triggeredApplicator.ReagentItem, triggeredApplicator.TriggeredEffect.Effect, (float)sourceApplicator.Record.SourceData.Level, (float)sourceApplicator.Record.AbilityLevel, false, effectSourceOverride);
		}

		// Token: 0x06006087 RID: 24711 RVA: 0x001FD73C File Offset: 0x001FB93C
		private DamageCategoriesForTriggerType GetDamageCategoryForTriggerType(DamageType dmgType)
		{
			switch (dmgType)
			{
			case DamageType.Melee_Slashing:
			case DamageType.Melee_Crushing:
			case DamageType.Melee_Piercing:
				return DamageCategoriesForTriggerType.Melee;
			case DamageType.Ranged_Auditory:
			case DamageType.Ranged_Crushing:
			case DamageType.Ranged_Piercing:
				return DamageCategoriesForTriggerType.Ranged;
			case DamageType.Natural_Life:
			case DamageType.Natural_Death:
			case DamageType.Natural_Chemical:
			case DamageType.Natural_Spirit:
				return DamageCategoriesForTriggerType.Chemical;
			case DamageType.Elemental_Air:
			case DamageType.Elemental_Earth:
			case DamageType.Elemental_Fire:
			case DamageType.Elemental_Water:
				return DamageCategoriesForTriggerType.Ember;
			default:
				throw new ArgumentException("dmgType");
			}
		}

		// Token: 0x06006088 RID: 24712 RVA: 0x001FD79C File Offset: 0x001FB99C
		private DamageCategoriesForTriggerType GetDamageCategoryForTriggerType(StatType damageChannel)
		{
			switch (damageChannel)
			{
			case StatType.Damage1H:
			case StatType.Damage2H:
				return DamageCategoriesForTriggerType.Melee;
			case StatType.DamageRanged:
				return DamageCategoriesForTriggerType.Ranged;
			case StatType.DamageMental:
				return DamageCategoriesForTriggerType.Mental;
			case StatType.DamageChemical:
				return DamageCategoriesForTriggerType.Chemical;
			case StatType.DamageEmber:
				return DamageCategoriesForTriggerType.Ember;
			case StatType.DamagePhysical:
			case StatType.DamageRaw:
				return DamageCategoriesForTriggerType.None;
			default:
				throw new ArgumentException("damageChannel");
			}
		}

		// Token: 0x040052F5 RID: 21237
		private EffectController.RemoveEffectsReason m_removeEffectsReason;

		// Token: 0x040052F6 RID: 21238
		private CharacterRecord m_characterRecord;

		// Token: 0x040052F7 RID: 21239
		private Dictionary<DiminishingReturnType, DiminishingReturnCalculator> m_diminishingReturns;

		// Token: 0x040052F9 RID: 21241
		private AuraController m_sourceAura;

		// Token: 0x040052FA RID: 21242
		private AuraController m_activeAura;

		// Token: 0x040052FB RID: 21243
		private Dictionary<UniqueId, AuraController> m_appliedAuras;

		// Token: 0x040052FC RID: 21244
		private readonly Dictionary<UniqueId, EffectApplicator> m_lastingApplicatorsByInstanceId = new Dictionary<UniqueId, EffectApplicator>(default(UniqueIdComparer));

		// Token: 0x040052FD RID: 21245
		private readonly DictionaryList<EffectKey, EffectApplicator> m_lastingApplicatorsByKey = new DictionaryList<EffectKey, EffectApplicator>(default(EffectKeyComparer), false);

		// Token: 0x040052FE RID: 21246
		private readonly List<EffectApplicator> m_activeTriggers = new List<EffectApplicator>(10);

		// Token: 0x02000C34 RID: 3124
		private enum RemoveEffectsReason
		{
			// Token: 0x04005300 RID: 21248
			None,
			// Token: 0x04005301 RID: 21249
			Death,
			// Token: 0x04005302 RID: 21250
			Unconscious
		}
	}
}

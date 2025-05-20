using System;
using System.Collections.Generic;
using ENet;
using NetStack.Serialization;
using SoL.Game.EffectSystem;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Containers;
using SoL.Game.Player;
using SoL.Game.Settings;
using SoL.Managers;
using SoL.Networking;
using SoL.Networking.Database;
using SoL.Networking.Game;
using SoL.Networking.Replication;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using SoL.Utilities.Logging;
using UnityEngine;

namespace SoL.Game
{
	// Token: 0x020005F1 RID: 1521
	public class PlayerVitals_Server : ServerVitals
	{
		// Token: 0x17000A43 RID: 2627
		// (get) Token: 0x06003037 RID: 12343 RVA: 0x000614C1 File Offset: 0x0005F6C1
		private VitalsReplicatorPlayer PlayerReplicator
		{
			get
			{
				if (!this.m_playerReplicator)
				{
					this.m_playerReplicator = (base.m_replicator as VitalsReplicatorPlayer);
				}
				return this.m_playerReplicator;
			}
		}

		// Token: 0x17000A44 RID: 2628
		// (get) Token: 0x06003038 RID: 12344 RVA: 0x000614E7 File Offset: 0x0005F6E7
		private bool IsUnco
		{
			get
			{
				return this.LocalHealthState != HealthState.Alive;
			}
		}

		// Token: 0x17000A45 RID: 2629
		// (get) Token: 0x06003039 RID: 12345 RVA: 0x000614F5 File Offset: 0x0005F6F5
		// (set) Token: 0x0600303A RID: 12346 RVA: 0x00061507 File Offset: 0x0005F707
		private HealthState LocalHealthState
		{
			get
			{
				return this.PlayerReplicator.CurrentHealthState.Value;
			}
			set
			{
				this.PlayerReplicator.CurrentHealthState.Value = value;
				if (this.LocalHealthState == HealthState.Alive)
				{
					this.m_timeOfLastHealthStateChange = null;
					return;
				}
				this.m_timeOfLastHealthStateChange = new DateTime?(DateTime.UtcNow);
			}
		}

		// Token: 0x0600303B RID: 12347 RVA: 0x00061540 File Offset: 0x0005F740
		public override float GetHealthPercent()
		{
			return this.m_healthWrapper.Value / (float)this.MaxHealth;
		}

		// Token: 0x0600303C RID: 12348 RVA: 0x000612C2 File Offset: 0x0005F4C2
		public override float GetArmorClassPercent()
		{
			if (this.MaxArmorClass <= 0)
			{
				return 0f;
			}
			return (float)this.ArmorClass / (float)this.MaxArmorClass;
		}

		// Token: 0x17000A46 RID: 2630
		// (get) Token: 0x0600303D RID: 12349 RVA: 0x00061555 File Offset: 0x0005F755
		public override float Health
		{
			get
			{
				return this.m_healthWrapper.Value;
			}
		}

		// Token: 0x17000A47 RID: 2631
		// (get) Token: 0x0600303E RID: 12350 RVA: 0x00061562 File Offset: 0x0005F762
		public override float HealthWound { get; }

		// Token: 0x17000A48 RID: 2632
		// (get) Token: 0x0600303F RID: 12351 RVA: 0x0006156A File Offset: 0x0005F76A
		public override int MaxHealth
		{
			get
			{
				return this.m_healthWrapper.MaxValue;
			}
		}

		// Token: 0x17000A49 RID: 2633
		// (get) Token: 0x06003040 RID: 12352 RVA: 0x00061577 File Offset: 0x0005F777
		public override float Stamina
		{
			get
			{
				return this.m_staminaWrapper.Value;
			}
		}

		// Token: 0x17000A4A RID: 2634
		// (get) Token: 0x06003041 RID: 12353 RVA: 0x00061584 File Offset: 0x0005F784
		public override float StaminaWound { get; }

		// Token: 0x17000A4B RID: 2635
		// (get) Token: 0x06003042 RID: 12354 RVA: 0x0006158C File Offset: 0x0005F78C
		public override int ArmorClass
		{
			get
			{
				if (this.Stance != Stance.Combat)
				{
					return this.PlayerReplicator.ArmorClass.Value;
				}
				return this.PlayerReplicator.ArmorClass.Value + this.m_shieldArmorClass;
			}
		}

		// Token: 0x17000A4C RID: 2636
		// (get) Token: 0x06003043 RID: 12355 RVA: 0x000615BF File Offset: 0x0005F7BF
		public override int MaxArmorClass { get; }

		// Token: 0x17000A4D RID: 2637
		// (get) Token: 0x06003044 RID: 12356 RVA: 0x000615C7 File Offset: 0x0005F7C7
		protected override float CombatRecoveryTime
		{
			get
			{
				return GlobalSettings.Values.Combat.CombatRecoveryTime;
			}
		}

		// Token: 0x06003045 RID: 12357 RVA: 0x000615D8 File Offset: 0x0005F7D8
		public override void FinalizeExternal()
		{
			if (this.m_finalized)
			{
				return;
			}
			if (this.LocalHealthState == HealthState.Unconscious)
			{
				this.GiveUp();
			}
			base.FinalizeExternal();
		}

		// Token: 0x06003046 RID: 12358 RVA: 0x00158DDC File Offset: 0x00156FDC
		protected override void InitInternal()
		{
			base.InitInternal();
			if (this.m_record.Vitals == null)
			{
				this.m_record.Vitals = new CharacterVitals
				{
					Health = new WoundableValue
					{
						Value = (float)this.PlayerReplicator.MaxHealth.Value,
						Wound = ((GlobalSettings.Values != null && GlobalSettings.Values.Player != null) ? GlobalSettings.Values.Player.StartingHealthWounds : 0f)
					},
					Stamina = new WoundableValue
					{
						Value = 100f,
						Wound = ((GlobalSettings.Values != null && GlobalSettings.Values.Player != null) ? GlobalSettings.Values.Player.StartingStaminaWounds : 0f)
					}
				};
			}
			this.m_healthWrapper = new PlayerVitals_Server.HealthWoundableWrapper(this.m_record.Vitals.Health, this.PlayerReplicator.Health, this.PlayerReplicator.HealthWound, (float)this.PlayerReplicator.MaxHealth.Value, GlobalSettings.Values.Player.HealthWoundProfile.Max * 100f);
			this.m_staminaWrapper = new PlayerVitals_Server.StaminaWoundableWrapper(this.m_record.Vitals.Stamina, this.PlayerReplicator.Stamina, this.PlayerReplicator.StaminaWound, 100f, GlobalSettings.Values.Player.StaminaWoundProfile.Max * 100f);
			if (base.GameEntity && base.GameEntity.CharacterData)
			{
				base.GameEntity.CharacterData.CharacterFlags.Value |= PlayerFlags.NoTarget;
				this.m_noTargetSpawnData = new PlayerVitals_Server.NoTargetSpawnData?(new PlayerVitals_Server.NoTargetSpawnData
				{
					Timeout = Time.time + 10f,
					Pos = base.GameEntity.gameObject.transform.position,
					Heading = base.GameEntity.gameObject.transform.eulerAngles.y
				});
			}
		}

		// Token: 0x06003047 RID: 12359 RVA: 0x000615F8 File Offset: 0x0005F7F8
		protected override void Subscribe()
		{
			base.Subscribe();
			base.GameEntity.CollectionController.Masteries.ContentsChanged += this.MasteriesOnContentsChanged;
		}

		// Token: 0x06003048 RID: 12360 RVA: 0x00159008 File Offset: 0x00157208
		protected override void Unsubscribe()
		{
			base.Unsubscribe();
			if (base.GameEntity && base.GameEntity.CollectionController != null && base.GameEntity.CollectionController.Masteries != null)
			{
				base.GameEntity.CollectionController.Masteries.ContentsChanged -= this.MasteriesOnContentsChanged;
			}
		}

		// Token: 0x06003049 RID: 12361 RVA: 0x0015906C File Offset: 0x0015726C
		public override void TriggerUndonesWrath(GameEntity sourceEntity)
		{
			if (PlayerVitals_Server.m_undonesWrathTriggers == null)
			{
				PlayerVitals_Server.m_undonesWrathTriggers = new Dictionary<int, int>();
			}
			if (Time.time - this.m_timeOfNextUndonesWrath >= 7f)
			{
				this.m_undonesWrathOffenses = 0;
				this.m_timeOfNextUndonesWrath = Time.time + 5f;
				return;
			}
			if (Time.time >= this.m_timeOfNextUndonesWrath)
			{
				this.m_undonesWrathOffenses++;
				this.m_timeOfNextUndonesWrath = Time.time + 5f;
				float undonesWrathHealthFraction = GlobalSettings.Values.Npcs.UndonesWrathHealthFraction;
				float num = -1f * undonesWrathHealthFraction * (float)this.MaxHealth;
				this.AlterHealth(num);
				EffectApplicationResult fromPool = StaticPool<EffectApplicationResult>.GetFromPool();
				fromPool.SourceId = sourceEntity.NetworkEntity.NetworkId.Value;
				fromPool.TargetId = base.GameEntity.NetworkEntity.NetworkId.Value;
				fromPool.Flags = EffectApplicationFlags.Critical;
				fromPool.ArchetypeId = GlobalSettings.Values.Npcs.UndonesWrathArchetype.Id;
				fromPool.HealthAdjustment = new float?(num);
				Peer[] observersWithinRange = base.GameEntity.NetworkEntity.GetObserversWithinRange(25f);
				if (observersWithinRange == null || observersWithinRange.Length == 0)
				{
					if (observersWithinRange != null)
					{
						observersWithinRange.ReturnToPool();
					}
				}
				else
				{
					BitBuffer fromPool2 = BitBufferExtensions.GetFromPool();
					fromPool2.AddHeader(base.GameEntity.NetworkEntity, OpCodes.ChatMessage, true);
					fromPool.PackData(fromPool2);
					Packet packetFromBuffer_ReturnBufferToPool = fromPool2.GetPacketFromBuffer_ReturnBufferToPool(PacketFlags.None);
					NetworkCommand networkCommand = NetworkCommandPool.GetNetworkCommand();
					networkCommand.Channel = NetworkChannel.CombatResults;
					networkCommand.Packet = packetFromBuffer_ReturnBufferToPool;
					networkCommand.Type = CommandType.BroadcastGroup;
					networkCommand.TargetGroup = observersWithinRange;
					GameManager.NetworkManager.AddCommandToQueue(networkCommand);
				}
				StaticPool<EffectApplicationResult>.ReturnToPool(fromPool);
				Vector3 position = sourceEntity.gameObject.transform.position;
				Vector3 position2 = base.GameEntity.gameObject.transform.position;
				IndexedVector3 indexedVector = new IndexedVector3(position2, 2048, 1f);
				int num2;
				if (PlayerVitals_Server.m_undonesWrathTriggers.TryGetValue(indexedVector.Index, out num2))
				{
					num2++;
					PlayerVitals_Server.m_undonesWrathTriggers[indexedVector.Index] = num2;
					return;
				}
				num2 = 1;
				PlayerVitals_Server.m_undonesWrathTriggers.Add(indexedVector.Index, num2);
				PlayerVitals_Server.m_undonesWrathDataArray[0] = "Wrath";
				PlayerVitals_Server.m_undonesWrathDataArray[1] = LocalZoneManager.ZoneRecord.ZoneId;
				PlayerVitals_Server.m_undonesWrathDataArray[2] = position2.ToString();
				PlayerVitals_Server.m_undonesWrathDataArray[3] = new DebugLocation(LocalZoneManager.ZoneRecord.ZoneId, position2, base.GameEntity.gameObject.transform.rotation).DebugString;
				PlayerVitals_Server.m_undonesWrathDataArray[4] = position.ToString();
				PlayerVitals_Server.m_undonesWrathDataArray[5] = new DebugLocation(LocalZoneManager.ZoneRecord.ZoneId, position, sourceEntity.gameObject.transform.rotation).DebugString;
				SolDebug.LogToIndex(LogLevel.Information, LogIndex.Stuck, "{@EventType} triggered for {@ZoneId}! PlayerPos={@PlayerPosition} ({@PlayerDebugLocation}), NpcPos={@NpcPosition} ({@NpcDebugLocation})", PlayerVitals_Server.m_undonesWrathDataArray);
			}
		}

		// Token: 0x0600304A RID: 12362 RVA: 0x00159350 File Offset: 0x00157550
		public override void TakeFallDamage(float distanceFallen)
		{
			if (this.LocalHealthState != HealthState.Alive)
			{
				return;
			}
			if (base.GameEntity.GM && (base.GameEntity.CharacterData.CharacterFlags.Value.HasBitFlag(PlayerFlags.Invulnerable) || base.GameEntity.CharacterData.CharacterFlags.Value.HasBitFlag(PlayerFlags.Invisible)))
			{
				return;
			}
			float num = GlobalSettings.Values.Player.SafeFallEffectiveness.Evaluate(distanceFallen);
			float num2 = (float)base.GetStatusEffectValue(StatType.SafeFall) * 0.01f;
			num2 *= num;
			float num3 = GlobalSettings.Values.Player.FallDamageCurve.Evaluate(distanceFallen);
			num3 = num3.PercentModification(-num2);
			if (num3 <= 0f)
			{
				return;
			}
			this.AlterHealth(-num3);
			bool flag = this.LocalHealthState != HealthState.Alive;
			base.GameEntity.NetworkEntity.PlayerRpcHandler.Server_TakeFallDamage(Mathf.CeilToInt(num3), flag);
			float percentOfHealth = num3 / (float)this.PlayerReplicator.MaxHealth.Value;
			float num4 = GlobalSettings.Values.Player.HealthWoundProfile.GetWoundForFallDamage(percentOfHealth);
			if (flag)
			{
				num4 = Mathf.Clamp01(num4 - GlobalSettings.Values.Player.HealthWoundProfile.OnUnconscious);
			}
			if (num4 > 0f)
			{
				num4 = Mathf.Clamp(num4, 0f, 0.5f);
				this.AlterHealthWound(num4);
			}
		}

		// Token: 0x0600304B RID: 12363 RVA: 0x00061622 File Offset: 0x0005F822
		protected override void ApplyMaxArmorClassDelta(int delta)
		{
			this.PlayerReplicator.MaxArmorClass.Value = Mathf.Clamp(this.PlayerReplicator.MaxArmorClass.Value + delta, 0, 2000);
		}

		// Token: 0x0600304C RID: 12364 RVA: 0x00061651 File Offset: 0x0005F851
		public override void ApplyArmorClassDelta(int delta)
		{
			this.PlayerReplicator.ArmorClass.Value = Mathf.Clamp(this.PlayerReplicator.ArmorClass.Value + delta, 0, 2000);
		}

		// Token: 0x0600304D RID: 12365 RVA: 0x001594AC File Offset: 0x001576AC
		public override void RecalculateTotalArmorClass()
		{
			int num = 0;
			if (base.GameEntity != null && base.GameEntity.CollectionController != null && base.GameEntity.CollectionController.Equipment != null)
			{
				for (int i = 0; i < base.GameEntity.CollectionController.Equipment.Count; i++)
				{
					ArchetypeInstance index = base.GameEntity.CollectionController.Equipment.GetIndex(i);
					IArmorClass armorClass;
					if (index != null && index.Index != 65536 && index.Archetype.TryGetAsType(out armorClass) && armorClass.Type.ContributesArmorClass())
					{
						ItemInstanceData itemData = index.ItemData;
						if (((itemData != null) ? itemData.Durability : null) != null)
						{
							num += armorClass.GetCurrentArmorClass((float)index.ItemData.Durability.Absorbed);
						}
					}
				}
			}
			this.PlayerReplicator.ArmorClass.Value = Mathf.Clamp(num, 0, 2000);
		}

		// Token: 0x0600304E RID: 12366 RVA: 0x001595A4 File Offset: 0x001577A4
		public override void AlterHealth(float delta)
		{
			if (this.LocalHealthState == HealthState.Dead)
			{
				return;
			}
			if (delta < 0f && base.GameEntity.CharacterData.CharacterFlags.Value.HasBitFlag(PlayerFlags.Invulnerable))
			{
				return;
			}
			this.m_healthWrapper.Value = this.m_healthWrapper.Value + delta;
			if (this.LocalHealthState == HealthState.Unconscious)
			{
				if (this.Health > 0f)
				{
					this.LocalHealthState = HealthState.WakingUp;
					return;
				}
			}
			else if (this.Health <= 0f)
			{
				if (base.CheckResilience())
				{
					this.m_healthWrapper.Value = base.ResilienceRoll();
					base.GameEntity.NetworkEntity.PlayerRpcHandler.SendChatNotification("Resilience save!");
					return;
				}
				if (this.LocalHealthState == HealthState.Alive && this.ApplyWoundsToPlayer())
				{
					this.m_healthWrapper.Wound = this.m_healthWrapper.Wound + GlobalSettings.Values.Player.HealthWoundProfile.OnUnconscious * 100f;
					this.m_staminaWrapper.Wound = this.m_staminaWrapper.Wound + GlobalSettings.Values.Player.StaminaWoundProfile.OnUnconscious * 100f;
				}
				base.GameEntity.EffectController.RemoveEffectsForUnconscious();
				this.m_staminaWrapper.Value = 0f;
				this.LocalHealthState = HealthState.Unconscious;
			}
		}

		// Token: 0x0600304F RID: 12367 RVA: 0x00061680 File Offset: 0x0005F880
		public override void AlterStamina(float delta)
		{
			this.m_staminaWrapper.Value = this.m_staminaWrapper.Value + delta;
		}

		// Token: 0x06003050 RID: 12368 RVA: 0x001596FC File Offset: 0x001578FC
		public override void AlterHealthWound(float delta)
		{
			if (delta > 0f && base.GameEntity.CharacterData.CharacterFlags.Value.HasBitFlag(PlayerFlags.Invulnerable))
			{
				return;
			}
			this.m_healthWrapper.Wound = this.m_healthWrapper.Wound + delta;
		}

		// Token: 0x06003051 RID: 12369 RVA: 0x0006169A File Offset: 0x0005F89A
		public override void AlterStaminaWound(float delta)
		{
			this.m_staminaWrapper.Wound = this.m_staminaWrapper.Wound + delta;
		}

		// Token: 0x06003052 RID: 12370 RVA: 0x000616B4 File Offset: 0x0005F8B4
		protected override void CurrentStanceOnChanged(Stance obj)
		{
			base.CurrentStanceOnChanged(obj);
			base.GameEntity.EffectController.RefreshAuras();
		}

		// Token: 0x06003053 RID: 12371 RVA: 0x000616CD File Offset: 0x0005F8CD
		protected override void CharacterDataOnHandConfigurationChanged()
		{
			base.CharacterDataOnHandConfigurationChanged();
			base.GameEntity.EffectController.RefreshAuras();
		}

		// Token: 0x06003054 RID: 12372 RVA: 0x000616E5 File Offset: 0x0005F8E5
		protected override void UpdateMaxHealth(int value)
		{
			this.PlayerReplicator.MaxHealth.Value = value;
			PlayerVitals_Server.HealthWoundableWrapper healthWrapper = this.m_healthWrapper;
			if (healthWrapper == null)
			{
				return;
			}
			healthWrapper.UpdateMaxValue((float)value);
		}

		// Token: 0x06003055 RID: 12373 RVA: 0x00159748 File Offset: 0x00157948
		protected override void UpdateCombatState()
		{
			base.UpdateCombatState();
			if (this.m_recoveryFraction < 1f)
			{
				float delta = Time.deltaTime * GlobalSettings.Values.Player.StaminaWoundProfile.AccumulationRate * (1f - this.m_recoveryFraction);
				this.AlterStaminaWound(delta);
			}
			if (this.m_recoveryFraction < 1f && !base.GameEntity.CharacterData.CharacterFlags.Value.HasBitFlag(PlayerFlags.InCombat))
			{
				base.GameEntity.CharacterData.CharacterFlags.Value |= PlayerFlags.InCombat;
				return;
			}
			if (this.m_recoveryFraction >= 1f && base.GameEntity.CharacterData.CharacterFlags.Value.HasBitFlag(PlayerFlags.InCombat))
			{
				base.GameEntity.CharacterData.CharacterFlags.Value &= ~PlayerFlags.InCombat;
			}
		}

		// Token: 0x06003056 RID: 12374 RVA: 0x0006170A File Offset: 0x0005F90A
		protected override bool ShouldResetFullyRestedTimer()
		{
			return this.IsUnco || base.ShouldResetFullyRestedTimer();
		}

		// Token: 0x06003057 RID: 12375 RVA: 0x00159828 File Offset: 0x00157A28
		protected override void UpdateWounds()
		{
			if (this.m_lastApplicator == null || this.IsUnco)
			{
				return;
			}
			float num = -1f * this.m_lastApplicator.WoundRegenRatePerSecond * 100f * Time.deltaTime;
			if (this.m_healthWrapper.Wound > 0f)
			{
				this.m_healthWrapper.Wound = this.m_healthWrapper.Wound + num * this.m_recoveryFraction;
			}
			if (this.m_staminaWrapper.Wound > 0f)
			{
				this.m_staminaWrapper.Wound = this.m_staminaWrapper.Wound + num * this.m_recoveryFraction;
			}
		}

		// Token: 0x06003058 RID: 12376 RVA: 0x001598CC File Offset: 0x00157ACC
		protected override void UpdateHealth()
		{
			if (this.IsUnco || this.m_healthWrapper.Value <= 0f || this.m_healthWrapper.Value >= this.m_healthWrapper.EffectiveMax)
			{
				return;
			}
			StanceProfile stanceProfile = this.PlayerReplicator.CurrentStance.Value.GetStanceProfile();
			float num = stanceProfile.GetHealthRegenRate(this.m_recoveryFraction);
			float healthRegenBonusPercent = base.GetHealthRegenBonusPercent();
			float num2 = Mathf.Clamp01(healthRegenBonusPercent * GlobalSettings.Values.Player.HealthRegenStatRateMultiplier);
			if (this.Stance.ApplyFullyRestedBonus())
			{
				float fullyRestedBonus = GlobalSettings.Values.Combat.GetFullyRestedBonus(this.m_timeFullyRested, healthRegenBonusPercent);
				num += Mathf.Max(num2, fullyRestedBonus * stanceProfile.FullyRestedMultiplier);
			}
			else if (this.Stance.ApplyRegenStat())
			{
				num += num2;
			}
			if (this.m_lastApplicator)
			{
				num += this.m_lastApplicator.HealthRegenRatePerSecond * this.m_recoveryFraction;
			}
			this.m_healthWrapper.Value = this.m_healthWrapper.Value + num * Time.deltaTime;
		}

		// Token: 0x06003059 RID: 12377 RVA: 0x001599D8 File Offset: 0x00157BD8
		protected override void UpdateStamina()
		{
			if (this.IsUnco)
			{
				return;
			}
			SkillsController.PendingExecution pending = base.GameEntity.SkillsController.Pending;
			if ((!pending.Active || (pending.Executable != null && pending.Executable.AllowStaminaRegenDuringExecution)) && this.m_staminaWrapper.Value < this.m_staminaWrapper.EffectiveMax)
			{
				float num = this.PlayerReplicator.CurrentStance.Value.GetStanceProfile().GetStaminaRegenMultiplier(this.m_recoveryFraction).PercentModification(base.GetStaminaRegenBonusPercent()) * (100f / GlobalSettings.Values.Player.StaminaRecoveryTime) * Time.deltaTime;
				this.m_staminaWrapper.Value = this.m_staminaWrapper.Value + num;
			}
		}

		// Token: 0x0600305A RID: 12378 RVA: 0x00159A9C File Offset: 0x00157C9C
		protected override void UpdateHealthState()
		{
			if (this.m_timeOfLastHealthStateChange == null)
			{
				return;
			}
			float num = (float)(DateTime.UtcNow - this.m_timeOfLastHealthStateChange.Value).TotalSeconds;
			switch (this.LocalHealthState)
			{
			case HealthState.Unconscious:
				this.m_cachedPlayerSpawn = false;
				if (num > GlobalSettings.Values.Player.GiveUpTime)
				{
					this.GiveUp();
					return;
				}
				break;
			case HealthState.WakingUp:
				if (num >= GlobalSettings.Values.Player.WakeUpTime)
				{
					this.LocalHealthState = HealthState.Alive;
					return;
				}
				break;
			case HealthState.Dead:
				if (!this.m_cachedPlayerSpawn)
				{
					LocalZoneManager.CacheClosestPlayerSpawn(base.GameEntity.gameObject.transform.position);
					this.m_cachedPlayerSpawn = true;
				}
				if (num > GlobalSettings.Values.Player.DeadDelayTime)
				{
					Vector3 position = base.GameEntity.gameObject.transform.position;
					Quaternion rotation = base.GameEntity.gameObject.transform.rotation;
					this.m_healthWrapper.Value = 1f;
					PlayerSpawn respawnPoint = LocalZoneManager.GetRespawnPoint(base.GameEntity);
					Vector3 position2 = respawnPoint.GetPosition();
					Quaternion rotation2 = respawnPoint.GetRotation();
					base.GameEntity.NetworkEntity.PlayerRpcHandler.Server_Respawn(position2, rotation2.eulerAngles.y);
					this.LocalHealthState = HealthState.WakingUp;
					this.LogRespawn(position, rotation, position2, rotation2);
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x0600305B RID: 12379 RVA: 0x00159C00 File Offset: 0x00157E00
		private void LogRespawn(Vector3 sourcePos, Quaternion sourceRot, Vector3 respawnPos, Quaternion respawnRot)
		{
			PlayerVitals_Server.m_respawnArguments[0] = "Respawn";
			PlayerVitals_Server.m_respawnArguments[1] = "UNKNOWN";
			PlayerVitals_Server.m_respawnArguments[2] = "UNKNOWN";
			PlayerVitals_Server.m_respawnArguments[3] = "UNKNOWN";
			PlayerVitals_Server.m_respawnArguments[4] = -1;
			PlayerVitals_Server.m_respawnArguments[5] = "UNKNOWN";
			PlayerVitals_Server.m_respawnArguments[6] = "UNKNOWN";
			if (base.GameEntity)
			{
				PlayerRpcHandler.PlayerUserData playerUserData = PlayerRpcHandler.GetPlayerUserData(base.GameEntity);
				PlayerVitals_Server.m_respawnArguments[1] = playerUserData.UserId;
				PlayerVitals_Server.m_respawnArguments[2] = playerUserData.CharacterId;
				PlayerVitals_Server.m_respawnArguments[3] = playerUserData.CharacterName;
			}
			if (LocalZoneManager.ZoneRecord != null)
			{
				PlayerVitals_Server.m_respawnArguments[4] = LocalZoneManager.ZoneRecord.ZoneId;
				PlayerVitals_Server.m_respawnArguments[5] = new DebugLocation(LocalZoneManager.ZoneRecord.ZoneId, sourcePos, sourceRot).DebugString;
				PlayerVitals_Server.m_respawnArguments[6] = new DebugLocation(LocalZoneManager.ZoneRecord.ZoneId, respawnPos, respawnRot).DebugString;
			}
			SolDebug.LogToIndex(LogLevel.Information, LogIndex.Stuck, "{@EventType} {@UserId}.{@CharacterId}.{@PlayerName} has respawned in {@ZoneId} from {@DeathDebugLocation} to {@RespawnDebugLocation}", PlayerVitals_Server.m_respawnArguments);
		}

		// Token: 0x0600305C RID: 12380 RVA: 0x00159D14 File Offset: 0x00157F14
		protected override void UpdateArmorAugments()
		{
			base.UpdateArmorAugments();
			if (this.m_recoveryFraction >= 1f)
			{
				return;
			}
			this.m_augmentAccumulatedCombatTime += Time.deltaTime;
			int num = Mathf.FloorToInt(this.m_augmentAccumulatedCombatTime / 60f);
			this.m_augmentAccumulatedCombatTime = Mathf.Clamp(this.m_augmentAccumulatedCombatTime - (float)num * 60f, 0f, float.MaxValue);
			if ((float)num <= 0f || this.m_activeAugments == null || this.m_activeAugments.Count <= 0)
			{
				return;
			}
			bool flag = false;
			for (int i = 0; i < this.m_activeAugments.Count; i++)
			{
				if (this.m_activeAugments[i].Instance != null && this.m_activeAugments[i].AugmentItem != null)
				{
					this.m_activeAugments[i].AugmentItem.AugmentUsed(base.GameEntity, this.m_activeAugments[i].Instance, num);
					flag = (!this.m_activeAugments[i].Instance.ItemData.HasAugment || flag);
				}
			}
			if (flag)
			{
				base.RefreshAugmentStats(false);
			}
		}

		// Token: 0x0600305D RID: 12381 RVA: 0x00159E40 File Offset: 0x00158040
		protected override void UpdateSpawnNoTarget()
		{
			if (this.m_noTargetSpawnData != null && base.GameEntity && base.GameEntity.CharacterData)
			{
				bool flag;
				if (Time.time >= this.m_noTargetSpawnData.Value.Timeout)
				{
					flag = true;
				}
				else
				{
					Transform transform = base.GameEntity.gameObject.transform;
					float sqrMagnitude = (this.m_noTargetSpawnData.Value.Pos - transform.position).sqrMagnitude;
					float num = Mathf.Abs(this.m_noTargetSpawnData.Value.Heading - transform.eulerAngles.y);
					flag = (sqrMagnitude >= 0.040000003f || num >= 0.1f);
				}
				if (flag)
				{
					base.GameEntity.CharacterData.CharacterFlags.Value &= ~PlayerFlags.NoTarget;
					this.m_noTargetSpawnData = null;
				}
			}
		}

		// Token: 0x0600305E RID: 12382 RVA: 0x00159F3C File Offset: 0x0015813C
		public override void CancelSpawnNoTarget()
		{
			if (this.m_noTargetSpawnData != null && base.GameEntity && base.GameEntity.CharacterData)
			{
				base.GameEntity.CharacterData.CharacterFlags.Value &= ~PlayerFlags.NoTarget;
				this.m_noTargetSpawnData = null;
			}
		}

		// Token: 0x0600305F RID: 12383 RVA: 0x00159FA4 File Offset: 0x001581A4
		public void GiveUp()
		{
			if (this.LocalHealthState == HealthState.Dead)
			{
				return;
			}
			base.GameEntity.EffectController.RemoveEffectsForDeath();
			if (this.ApplyWoundsToPlayer())
			{
				this.m_healthWrapper.Wound = this.m_healthWrapper.Wound + GlobalSettings.Values.Player.HealthWoundProfile.OnRelease * 100f;
				this.m_staminaWrapper.Wound = this.m_staminaWrapper.Wound + GlobalSettings.Values.Player.StaminaWoundProfile.OnRelease * 100f;
			}
			this.m_lastCombatTimestamp = DateTime.MinValue;
			this.m_record.Location.UpdateFromTransform(base.GameEntity.transform);
			ContainerRecord containerRecord;
			if (this.m_record.Corpse == null && this.m_record.Storage.TryGetValue(ContainerType.Inventory, out containerRecord) && !containerRecord.IsEmpty())
			{
				CharacterLocation characterLocation = this.m_record.Location.Clone();
				RaycastHit raycastHit;
				if (Physics.Raycast(characterLocation.GetPosition(), Vector3.up, out raycastHit, 10f, LayerMap.Water.LayerMask, QueryTriggerInteraction.Ignore))
				{
					characterLocation.y = raycastHit.collider.gameObject.transform.position.y;
				}
				Vector3 vector;
				if (LocalZoneManager.TryRelocationBackpack(characterLocation.GetPosition(), out vector))
				{
					characterLocation.x = vector.x;
					characterLocation.y = vector.y;
					characterLocation.z = vector.z;
				}
				this.m_record.Corpse = new CharacterCorpse
				{
					TimeCreated = DateTime.UtcNow,
					Location = characterLocation
				};
				this.m_record.UpdateCorpseData(ExternalGameDatabase.Database);
				base.GameEntity.CharacterData.CharacterFlags.Value |= PlayerFlags.MissingBag;
				CorpseManager.SpawnWorldCorpse(this.m_record, base.GameEntity.NetworkEntity);
			}
			if (base.ApplyDamageToGearForDeath())
			{
				base.GameEntity.NetworkEntity.PlayerRpcHandler.Server_ApplyDamageToGearForDeath();
			}
			this.LocalHealthState = HealthState.Dead;
		}

		// Token: 0x06003060 RID: 12384 RVA: 0x0006171C File Offset: 0x0005F91C
		private bool ApplyWoundsToPlayer()
		{
			return !LocalZoneManager.IsWithinPvpCollider(base.GameEntity.gameObject.transform.position);
		}

		// Token: 0x04002EED RID: 12013
		private VitalsReplicatorPlayer m_playerReplicator;

		// Token: 0x04002EEE RID: 12014
		private const float kSpawnNoTargetTime = 10f;

		// Token: 0x04002EEF RID: 12015
		private const float kSpawnNoTargetAngleThreshold = 0.1f;

		// Token: 0x04002EF0 RID: 12016
		private const float kSpawnNoTargetPositionThreshold = 0.2f;

		// Token: 0x04002EF1 RID: 12017
		private const float kSpawnNoTargetPositionThresholdSqr = 0.040000003f;

		// Token: 0x04002EF2 RID: 12018
		private PlayerVitals_Server.NoTargetSpawnData? m_noTargetSpawnData;

		// Token: 0x04002EF3 RID: 12019
		private bool m_cachedPlayerSpawn;

		// Token: 0x04002EF4 RID: 12020
		private PlayerVitals_Server.HealthWoundableWrapper m_healthWrapper;

		// Token: 0x04002EF5 RID: 12021
		private PlayerVitals_Server.StaminaWoundableWrapper m_staminaWrapper;

		// Token: 0x04002EF6 RID: 12022
		private DateTime? m_timeOfLastHealthStateChange;

		// Token: 0x04002EFA RID: 12026
		private const string kLogTemplate = "{@EventType} triggered for {@ZoneId}! PlayerPos={@PlayerPosition} ({@PlayerDebugLocation}), NpcPos={@NpcPosition} ({@NpcDebugLocation})";

		// Token: 0x04002EFB RID: 12027
		private static Dictionary<int, int> m_undonesWrathTriggers = null;

		// Token: 0x04002EFC RID: 12028
		private static readonly object[] m_undonesWrathDataArray = new object[6];

		// Token: 0x04002EFD RID: 12029
		private const float kUndonesWrathCadence = 5f;

		// Token: 0x04002EFE RID: 12030
		private const float kUndonesWrathTimeout = 7f;

		// Token: 0x04002EFF RID: 12031
		private int m_undonesWrathOffenses;

		// Token: 0x04002F00 RID: 12032
		private float m_timeOfNextUndonesWrath = -100f;

		// Token: 0x04002F01 RID: 12033
		private static readonly object[] m_respawnArguments = new object[7];

		// Token: 0x04002F02 RID: 12034
		private const float kToAccumulate = 60f;

		// Token: 0x04002F03 RID: 12035
		private float m_augmentAccumulatedCombatTime;

		// Token: 0x020005F2 RID: 1522
		private abstract class WoundableWrapper<TSync, TType> where TSync : SynchronizedVariable<TType>
		{
			// Token: 0x17000A4E RID: 2638
			// (get) Token: 0x06003063 RID: 12387 RVA: 0x0006176C File Offset: 0x0005F96C
			public float EffectiveMax
			{
				get
				{
					return this.m_effectiveMaxValue;
				}
			}

			// Token: 0x17000A4F RID: 2639
			// (get) Token: 0x06003064 RID: 12388 RVA: 0x00061774 File Offset: 0x0005F974
			// (set) Token: 0x06003065 RID: 12389 RVA: 0x0006177C File Offset: 0x0005F97C
			public int MaxValue { get; private set; }

			// Token: 0x06003066 RID: 12390 RVA: 0x0015A1A8 File Offset: 0x001583A8
			public WoundableWrapper(WoundableValue woundable, TSync valueSync, SynchronizedByte woundSync, float maxValue, float maxWound)
			{
				this.m_woundable = woundable;
				this.m_valueSync = valueSync;
				this.m_woundSync = woundSync;
				this.m_maxValue = maxValue;
				this.m_maxWound = maxWound;
				this.MaxValue = Mathf.FloorToInt(this.m_maxValue);
				this.Wound = this.m_woundable.Wound;
			}

			// Token: 0x06003067 RID: 12391 RVA: 0x0015A204 File Offset: 0x00158404
			private void RecalculateEffectiveMax()
			{
				this.m_effectiveMaxValue = Mathf.Clamp(this.m_maxValue - this.m_maxValue * this.Wound * 0.01f, 0f, this.m_maxValue);
				this.m_effectiveMaxValueInt = Mathf.FloorToInt(this.m_effectiveMaxValue);
				this.m_effectiveMaxDisplay = Mathf.FloorToInt(Mathf.Clamp(this.m_maxValue - this.m_maxValue * (float)this.m_woundSync.Value * 0.01f, 0f, this.m_maxValue));
			}

			// Token: 0x17000A50 RID: 2640
			// (get) Token: 0x06003068 RID: 12392 RVA: 0x00061785 File Offset: 0x0005F985
			// (set) Token: 0x06003069 RID: 12393 RVA: 0x0015A290 File Offset: 0x00158490
			public float Value
			{
				get
				{
					return this.m_woundable.Value;
				}
				set
				{
					float value2 = Mathf.Clamp(value, 0f, this.m_effectiveMaxValue);
					this.m_woundable.Value = value2;
					this.m_valueSync.Value = this.GetValueSync();
				}
			}

			// Token: 0x0600306A RID: 12394
			protected abstract TType GetValueSync();

			// Token: 0x17000A51 RID: 2641
			// (get) Token: 0x0600306B RID: 12395 RVA: 0x00061792 File Offset: 0x0005F992
			// (set) Token: 0x0600306C RID: 12396 RVA: 0x0015A2D4 File Offset: 0x001584D4
			public float Wound
			{
				get
				{
					return this.m_woundable.Wound;
				}
				set
				{
					float num = Mathf.Clamp(value, 0f, this.m_maxWound);
					this.m_woundable.Wound = num;
					this.m_woundSync.Value = (byte)Mathf.FloorToInt(num);
					this.RecalculateEffectiveMax();
					this.Value = this.Value;
				}
			}

			// Token: 0x0600306D RID: 12397 RVA: 0x0006179F File Offset: 0x0005F99F
			public void UpdateMaxValue(float maxValue)
			{
				this.m_maxValue = maxValue;
				this.MaxValue = Mathf.FloorToInt(this.m_maxValue);
				this.RecalculateEffectiveMax();
				this.Value = Mathf.Clamp(this.Value, 0f, this.m_effectiveMaxValue);
			}

			// Token: 0x04002F04 RID: 12036
			protected readonly WoundableValue m_woundable;

			// Token: 0x04002F05 RID: 12037
			private readonly TSync m_valueSync;

			// Token: 0x04002F06 RID: 12038
			private readonly SynchronizedByte m_woundSync;

			// Token: 0x04002F07 RID: 12039
			private float m_maxValue;

			// Token: 0x04002F08 RID: 12040
			private readonly float m_maxWound;

			// Token: 0x04002F09 RID: 12041
			protected int m_effectiveMaxDisplay;

			// Token: 0x04002F0A RID: 12042
			protected int m_effectiveMaxValueInt;

			// Token: 0x04002F0B RID: 12043
			private float m_effectiveMaxValue;
		}

		// Token: 0x020005F3 RID: 1523
		private class HealthWoundableWrapper : PlayerVitals_Server.WoundableWrapper<SynchronizedInt, int>
		{
			// Token: 0x0600306E RID: 12398 RVA: 0x000617DB File Offset: 0x0005F9DB
			public HealthWoundableWrapper(WoundableValue woundable, SynchronizedInt valueSync, SynchronizedByte woundSync, float maxValue, float maxWound) : base(woundable, valueSync, woundSync, maxValue, maxWound)
			{
			}

			// Token: 0x0600306F RID: 12399 RVA: 0x000617EA File Offset: 0x0005F9EA
			protected override int GetValueSync()
			{
				if (this.m_woundable.Value < (float)this.m_effectiveMaxValueInt)
				{
					return Mathf.FloorToInt(this.m_woundable.Value);
				}
				return this.m_effectiveMaxDisplay;
			}
		}

		// Token: 0x020005F4 RID: 1524
		private class StaminaWoundableWrapper : PlayerVitals_Server.WoundableWrapper<SynchronizedByte, byte>
		{
			// Token: 0x06003070 RID: 12400 RVA: 0x00061817 File Offset: 0x0005FA17
			public StaminaWoundableWrapper(WoundableValue woundable, SynchronizedByte valueSync, SynchronizedByte woundSync, float maxValue, float maxWound) : base(woundable, valueSync, woundSync, maxValue, maxWound)
			{
			}

			// Token: 0x06003071 RID: 12401 RVA: 0x00061826 File Offset: 0x0005FA26
			protected override byte GetValueSync()
			{
				if (this.m_woundable.Value < (float)this.m_effectiveMaxValueInt)
				{
					return (byte)Mathf.FloorToInt(this.m_woundable.Value);
				}
				return (byte)this.m_effectiveMaxDisplay;
			}
		}

		// Token: 0x020005F5 RID: 1525
		private struct NoTargetSpawnData
		{
			// Token: 0x04002F0D RID: 12045
			public float Timeout;

			// Token: 0x04002F0E RID: 12046
			public Vector3 Pos;

			// Token: 0x04002F0F RID: 12047
			public float Heading;
		}
	}
}

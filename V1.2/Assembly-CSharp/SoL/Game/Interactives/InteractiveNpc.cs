using System;
using System.Collections.Generic;
using Cysharp.Text;
using SoL.Game.Crafting;
using SoL.Game.HuntingLog;
using SoL.Game.Loot;
using SoL.Game.Messages;
using SoL.Game.NPCs;
using SoL.Game.NPCs.Interactions;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Containers;
using SoL.Game.Quests;
using SoL.Game.Quests.Objectives;
using SoL.Game.Settings;
using SoL.Game.Spawning;
using SoL.Game.Spawning.Behavior;
using SoL.Game.Targeting;
using SoL.Game.UI.Dialog;
using SoL.Managers;
using SoL.Networking.Objects;
using SoL.Networking.Replication;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Interactives
{
	// Token: 0x02000B9D RID: 2973
	public class InteractiveNpc : InteractiveWithPermissions, IGatheringNode, ILootRollSource, IDialogueNpc, ISpawnController
	{
		// Token: 0x17001579 RID: 5497
		// (get) Token: 0x06005B91 RID: 23441 RVA: 0x0004479C File Offset: 0x0004299C
		protected override bool AllowInteractionWhileMissingBag
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700157A RID: 5498
		// (get) Token: 0x06005B92 RID: 23442 RVA: 0x0007D727 File Offset: 0x0007B927
		// (set) Token: 0x06005B93 RID: 23443 RVA: 0x0007D72F File Offset: 0x0007B92F
		public bool IsAshen { get; set; }

		// Token: 0x1700157B RID: 5499
		// (get) Token: 0x06005B94 RID: 23444 RVA: 0x0007D738 File Offset: 0x0007B938
		// (set) Token: 0x06005B95 RID: 23445 RVA: 0x0007D740 File Offset: 0x0007B940
		public bool AlwaysGoAshen { get; set; }

		// Token: 0x1700157C RID: 5500
		// (get) Token: 0x06005B96 RID: 23446 RVA: 0x0007D749 File Offset: 0x0007B949
		// (set) Token: 0x06005B97 RID: 23447 RVA: 0x0007D751 File Offset: 0x0007B951
		public ISpawnController SpawnController { get; set; }

		// Token: 0x1700157D RID: 5501
		// (get) Token: 0x06005B98 RID: 23448 RVA: 0x0007D75A File Offset: 0x0007B95A
		// (set) Token: 0x06005B99 RID: 23449 RVA: 0x0007D762 File Offset: 0x0007B962
		public NpcSpawnProfileV2 SpawnProfile { get; set; }

		// Token: 0x1700157E RID: 5502
		// (get) Token: 0x06005B9A RID: 23450 RVA: 0x0007D76B File Offset: 0x0007B96B
		// (set) Token: 0x06005B9B RID: 23451 RVA: 0x0007D773 File Offset: 0x0007B973
		public AshenController AshenController { get; set; }

		// Token: 0x1700157F RID: 5503
		// (get) Token: 0x06005B9C RID: 23452 RVA: 0x0007D77C File Offset: 0x0007B97C
		// (set) Token: 0x06005B9D RID: 23453 RVA: 0x0007D784 File Offset: 0x0007B984
		public NpcSpawnProfileV2 AshenSpawnProfile { get; set; }

		// Token: 0x17001580 RID: 5504
		// (get) Token: 0x06005B9E RID: 23454 RVA: 0x0007D78D File Offset: 0x0007B98D
		// (set) Token: 0x06005B9F RID: 23455 RVA: 0x0007D795 File Offset: 0x0007B995
		public LootTableSampleCount ItemTable { get; set; }

		// Token: 0x17001581 RID: 5505
		// (get) Token: 0x06005BA0 RID: 23456 RVA: 0x0007D79E File Offset: 0x0007B99E
		// (set) Token: 0x06005BA1 RID: 23457 RVA: 0x0007D7A6 File Offset: 0x0007B9A6
		public LootTableSampleCount ResourceTable { get; set; }

		// Token: 0x17001582 RID: 5506
		// (get) Token: 0x06005BA2 RID: 23458 RVA: 0x0007D7AF File Offset: 0x0007B9AF
		// (set) Token: 0x06005BA3 RID: 23459 RVA: 0x0007D7B7 File Offset: 0x0007B9B7
		public ILootTable LootTable { get; set; }

		// Token: 0x17001583 RID: 5507
		// (get) Token: 0x06005BA4 RID: 23460 RVA: 0x0007D7C0 File Offset: 0x0007B9C0
		// (set) Token: 0x06005BA5 RID: 23461 RVA: 0x0007D7C8 File Offset: 0x0007B9C8
		public ILootTable ResourceLootTable { get; set; }

		// Token: 0x17001584 RID: 5508
		// (get) Token: 0x06005BA6 RID: 23462 RVA: 0x0007D7D1 File Offset: 0x0007B9D1
		// (set) Token: 0x06005BA7 RID: 23463 RVA: 0x0007D7D9 File Offset: 0x0007B9D9
		public HuntingLogProfile HuntingLogProfile { get; private set; }

		// Token: 0x06005BA8 RID: 23464 RVA: 0x0007D7E2 File Offset: 0x0007B9E2
		public void SetHuntingLogProfileOverride(HuntingLogProfile profile)
		{
			this.HuntingLogProfile = (profile ? profile : this.m_huntingLogProfile);
		}

		// Token: 0x17001585 RID: 5509
		// (get) Token: 0x06005BA9 RID: 23465 RVA: 0x0007D7FB File Offset: 0x0007B9FB
		// (set) Token: 0x06005BAA RID: 23466 RVA: 0x0007D803 File Offset: 0x0007BA03
		public SpawnTier SpawnTier { get; set; }

		// Token: 0x17001586 RID: 5510
		// (get) Token: 0x06005BAB RID: 23467 RVA: 0x0007D80C File Offset: 0x0007BA0C
		// (set) Token: 0x06005BAC RID: 23468 RVA: 0x0007D814 File Offset: 0x0007BA14
		public MinMaxIntRange Currency { get; set; } = new MinMaxIntRange(0, 0);

		// Token: 0x17001587 RID: 5511
		// (get) Token: 0x06005BAD RID: 23469 RVA: 0x0007D81D File Offset: 0x0007BA1D
		// (set) Token: 0x06005BAE RID: 23470 RVA: 0x0007D825 File Offset: 0x0007BA25
		public ulong EventCurrency { get; set; }

		// Token: 0x17001588 RID: 5512
		// (get) Token: 0x06005BAF RID: 23471 RVA: 0x0007D82E File Offset: 0x0007BA2E
		// (set) Token: 0x06005BB0 RID: 23472 RVA: 0x0007D836 File Offset: 0x0007BA36
		public bool PreventExperienceDistribution { get; set; }

		// Token: 0x17001589 RID: 5513
		// (get) Token: 0x06005BB1 RID: 23473 RVA: 0x0007D83F File Offset: 0x0007BA3F
		// (set) Token: 0x06005BB2 RID: 23474 RVA: 0x0007D847 File Offset: 0x0007BA47
		public int Level { get; set; } = 1;

		// Token: 0x1700158A RID: 5514
		// (get) Token: 0x06005BB3 RID: 23475 RVA: 0x0007D850 File Offset: 0x0007BA50
		// (set) Token: 0x06005BB4 RID: 23476 RVA: 0x0007D858 File Offset: 0x0007BA58
		public bool LogLoot { get; set; }

		// Token: 0x1700158B RID: 5515
		// (get) Token: 0x06005BB5 RID: 23477 RVA: 0x0007D861 File Offset: 0x0007BA61
		// (set) Token: 0x06005BB6 RID: 23478 RVA: 0x0007D869 File Offset: 0x0007BA69
		public float? XpAdjustmentMultiplier { get; set; }

		// Token: 0x1700158C RID: 5516
		// (get) Token: 0x06005BB7 RID: 23479 RVA: 0x0007D872 File Offset: 0x0007BA72
		// (set) Token: 0x06005BB8 RID: 23480 RVA: 0x0007D87A File Offset: 0x0007BA7A
		public ChallengeRating ChallengeRating { get; set; }

		// Token: 0x1700158D RID: 5517
		// (get) Token: 0x06005BB9 RID: 23481 RVA: 0x0007D883 File Offset: 0x0007BA83
		// (set) Token: 0x06005BBA RID: 23482 RVA: 0x0007D88B File Offset: 0x0007BA8B
		public bool ExtendCorpseDecay { get; set; }

		// Token: 0x06005BBB RID: 23483 RVA: 0x0007D894 File Offset: 0x0007BA94
		internal bool TryGetAdditionalCorpseWaitTimes(out int additionalCycles)
		{
			additionalCycles = 0;
			if (this.ChallengeRating == ChallengeRating.CRB)
			{
				additionalCycles = 2;
			}
			else if (this.ExtendCorpseDecay)
			{
				additionalCycles = 1;
			}
			return additionalCycles != 0;
		}

		// Token: 0x1700158E RID: 5518
		// (get) Token: 0x06005BBC RID: 23484 RVA: 0x0007D8B7 File Offset: 0x0007BAB7
		// (set) Token: 0x06005BBD RID: 23485 RVA: 0x0007D8BF File Offset: 0x0007BABF
		protected override float m_permissionTimeoutTime { get; set; }

		// Token: 0x1700158F RID: 5519
		// (get) Token: 0x06005BBE RID: 23486 RVA: 0x0007D8C8 File Offset: 0x0007BAC8
		public SynchronizedEnum<InteractiveFlags> NpcInteractiveFlags
		{
			get
			{
				return this.m_interactiveFlags;
			}
		}

		// Token: 0x17001590 RID: 5520
		// (get) Token: 0x06005BBF RID: 23487 RVA: 0x0007D8D0 File Offset: 0x0007BAD0
		public SynchronizedString Tagger
		{
			get
			{
				return this.m_tagger;
			}
		}

		// Token: 0x17001591 RID: 5521
		// (get) Token: 0x06005BC0 RID: 23488 RVA: 0x0007D8D8 File Offset: 0x0007BAD8
		public SynchronizedUniqueId GroupId
		{
			get
			{
				return this.m_groupId;
			}
		}

		// Token: 0x17001592 RID: 5522
		// (get) Token: 0x06005BC1 RID: 23489 RVA: 0x0007D8E0 File Offset: 0x0007BAE0
		public SynchronizedUniqueId RaidId
		{
			get
			{
				return this.m_raidId;
			}
		}

		// Token: 0x17001593 RID: 5523
		// (get) Token: 0x06005BC2 RID: 23490 RVA: 0x0007D8E8 File Offset: 0x0007BAE8
		// (set) Token: 0x06005BC3 RID: 23491 RVA: 0x001EF104 File Offset: 0x001ED304
		public bool NpcContributed
		{
			get
			{
				if (!GameManager.IsServer)
				{
					return this.m_interactiveFlags.Value.HasBitFlag(InteractiveFlags.NpcContributed);
				}
				return this.m_npcContributed;
			}
			set
			{
				this.m_npcContributed = value;
				if (this.m_npcContributed)
				{
					this.m_interactiveFlags.Value |= InteractiveFlags.NpcContributed;
					if (this.m_interactiveFlags.Value.HasBitFlag(InteractiveFlags.Interactive))
					{
						this.m_interactiveFlags.Value &= ~InteractiveFlags.Interactive;
						return;
					}
				}
				else
				{
					this.m_interactiveFlags.Value &= ~InteractiveFlags.NpcContributed;
				}
			}
		}

		// Token: 0x06005BC4 RID: 23492 RVA: 0x0007D909 File Offset: 0x0007BB09
		protected override void PermissionsCleared()
		{
			this.NpcContributed = false;
		}

		// Token: 0x06005BC5 RID: 23493 RVA: 0x0007D912 File Offset: 0x0007BB12
		protected override bool CanInteractInternal()
		{
			return !this.NpcContributed && base.CanInteractInternal();
		}

		// Token: 0x06005BC6 RID: 23494 RVA: 0x0007D924 File Offset: 0x0007BB24
		public override bool CanInteract(GameEntity entity)
		{
			return base.CanInteract(entity);
		}

		// Token: 0x06005BC7 RID: 23495 RVA: 0x0007D372 File Offset: 0x0007B572
		public bool CanConverseWith(GameEntity entity)
		{
			return base.IsWithinDistance(entity) && base.GameEntity.CharacterData.Faction.GetPlayerTargetType() == TargetType.Defensive;
		}

		// Token: 0x06005BC8 RID: 23496 RVA: 0x001EF178 File Offset: 0x001ED378
		protected override void Start()
		{
			base.Start();
			if (InteractiveNpc.m_nearbyEntities == null)
			{
				InteractiveNpc.m_nearbyEntities = new List<NetworkEntity>(256);
				InteractiveNpc.m_compiledList = new List<NetworkEntity>(256);
			}
			this.m_permissionTimeoutTime = GlobalSettings.Values.Npcs.CorpseDecayTime * GlobalSettings.Values.Npcs.CorpsePermissionTimeout;
			int num;
			if (this.TryGetAdditionalCorpseWaitTimes(out num))
			{
				this.m_permissionTimeoutTime += this.m_permissionTimeoutTime * (float)num;
			}
			if (GameManager.IsServer)
			{
				base.GameEntity.VitalsReplicator.CurrentHealthState.Changed += this.CurrentHealthStateOnChanged;
				return;
			}
			NpcProfile npcProfile;
			InternalGameDatabase.Archetypes.TryGetAsType<NpcProfile>(base.GameEntity.CharacterData.NpcInitData.ProfileId, out npcProfile);
			DialogueSource dialogueSource2;
			DialogueSource dialogueSource = InternalGameDatabase.Archetypes.TryGetAsType<DialogueSource>(base.GameEntity.CharacterData.NpcInitData.OverrideDialogueId, out dialogueSource2) ? dialogueSource2 : npcProfile;
			if (dialogueSource != null && dialogueSource.HasDefaultDialogue)
			{
				this.m_hasDialogue = true;
				return;
			}
			if (dialogueSource != null && GameManager.QuestManager)
			{
				GameManager.QuestManager.QuestsUpdated += this.OnQuestsUpdated;
				this.OnQuestsUpdated();
			}
		}

		// Token: 0x06005BC9 RID: 23497 RVA: 0x0007D92D File Offset: 0x0007BB2D
		protected override void Update()
		{
			if (this.m_distributeExperienceFrame != null)
			{
				if (this.m_distributeExperienceFrame.Value <= Time.frameCount)
				{
					this.m_distributeExperienceFrame = null;
					this.DistributeAdventuringExperience();
					return;
				}
			}
			else
			{
				base.Update();
				this.ExpireNpcContributed();
			}
		}

		// Token: 0x06005BCA RID: 23498 RVA: 0x001EF2B4 File Offset: 0x001ED4B4
		protected override void OnDestroy()
		{
			if (GameManager.IsServer)
			{
				if (base.GameEntity && base.GameEntity.VitalsReplicator)
				{
					base.GameEntity.VitalsReplicator.CurrentHealthState.Changed -= this.CurrentHealthStateOnChanged;
				}
			}
			else if (GameManager.QuestManager != null)
			{
				GameManager.QuestManager.QuestsUpdated -= this.OnQuestsUpdated;
			}
			base.OnDestroy();
		}

		// Token: 0x06005BCB RID: 23499 RVA: 0x001EF334 File Offset: 0x001ED534
		private void OnQuestsUpdated()
		{
			NpcProfile npcProfile;
			InternalGameDatabase.Archetypes.TryGetAsType<NpcProfile>(base.GameEntity.CharacterData.NpcInitData.ProfileId, out npcProfile);
			DialogueSource dialogueSource;
			if ((InternalGameDatabase.Archetypes.TryGetAsType<DialogueSource>(base.GameEntity.CharacterData.NpcInitData.OverrideDialogueId, out dialogueSource) ? dialogueSource : npcProfile).HasQuestDialogue)
			{
				this.m_hasDialogue = true;
				return;
			}
			this.m_hasDialogue = false;
		}

		// Token: 0x06005BCC RID: 23500 RVA: 0x001EF3A0 File Offset: 0x001ED5A0
		private void ExpireNpcContributed()
		{
			if (!GameManager.IsServer)
			{
				return;
			}
			if (!base.IsTagged && this.NpcContributed && base.GameEntity && base.GameEntity.Vitals && base.GameEntity.Vitals.HealthPercent >= 1f && (float)(DateTime.UtcNow - base.GameEntity.Vitals.LastCombatTimestamp).TotalSeconds > GlobalSettings.Values.Combat.CombatRecoveryTime)
			{
				this.NpcContributed = false;
			}
		}

		// Token: 0x06005BCD RID: 23501 RVA: 0x001EF438 File Offset: 0x001ED638
		protected override void UpdateTaggers()
		{
			base.UpdateTaggers();
			if (!GameManager.IsServer || this.m_permissionsInitializedTime != null || base.GameEntity.TargetController == null)
			{
				return;
			}
			if (base.IsTagged)
			{
				NpcTargetController npcTargetController;
				if (base.GameEntity.TargetController.TryGetAsType(out npcTargetController) && npcTargetController.LastUpdateTargetFrame > (float)this.m_frameTagged && npcTargetController.AlertCount <= 0 && npcTargetController.HostileTargetCount <= 0)
				{
					base.ClearTaggers();
					return;
				}
				if (this.m_groupId.Value.IsEmpty && !base.TaggerEntity.CharacterData.GroupId.IsEmpty)
				{
					this.m_groupId.Value = base.TaggerEntity.CharacterData.GroupId;
				}
			}
		}

		// Token: 0x06005BCE RID: 23502 RVA: 0x0004475B File Offset: 0x0004295B
		protected override void GenerateRecord(GameEntity interactionSource)
		{
		}

		// Token: 0x06005BCF RID: 23503 RVA: 0x001EF504 File Offset: 0x001ED704
		private void CurrentHealthStateOnChanged(HealthState obj)
		{
			if (obj == HealthState.Dead)
			{
				this.m_permissionsInitializedTime = new DateTime?(DateTime.UtcNow);
				if (this.NpcContributed)
				{
					this.m_interactiveFlags.Value |= InteractiveFlags.Destroy;
					this.m_permissionsExpire = false;
				}
				else if (this.ShouldAshen())
				{
					this.m_interactiveFlags.Value |= InteractiveFlags.Destroy;
					this.m_permissionsExpire = false;
					GameEntity gameEntity = this.AshenSpawnProfile.DynamicSpawn(this, base.gameObject.transform.position, base.gameObject.transform.eulerAngles.y, new uint?(base.GameEntity.NetworkEntity.NetworkId.Value));
					if (gameEntity)
					{
						InteractiveNpc interactiveNpc;
						if (gameEntity.Interactive != null && gameEntity.Interactive.TryGetAsType(out interactiveNpc))
						{
							interactiveNpc.IsAshen = true;
						}
						if (gameEntity.Vitals)
						{
							gameEntity.Vitals.UpdateLastCombatTimestamp();
							float num;
							if (GlobalSettings.Values.Ashen.ReduceSpawnHealth(out num))
							{
								float delta = -1f * (float)gameEntity.Vitals.MaxHealth * num;
								gameEntity.Vitals.AlterHealth(delta);
							}
						}
						if (gameEntity.SkillsController)
						{
							gameEntity.SkillsController.NextNpcAttackTimeDelay = new DateTime?(DateTime.UtcNow.AddSeconds(2.0));
						}
						if (gameEntity.TargetController)
						{
							gameEntity.TargetController.InitializeAsAshen(base.GameEntity.TargetController);
						}
					}
					this.m_isGoingAshen = true;
					this.DistributeAdventuringExperience();
					UnityEngine.Object.Destroy(base.GameEntity.gameObject);
				}
				else
				{
					this.m_interactiveFlags.Value |= InteractiveFlags.Interactive;
					this.m_distributeExperienceFrame = new int?(Time.frameCount + 1);
				}
				if (this.SpawnController != null)
				{
					this.SpawnController.NotifyOfDeath(base.GameEntity);
					if (this.SpawnController.LogSpawns && this.SpawnProfile)
					{
						this.SpawnProfile.KilledMessage(base.gameObject.transform.position, base.gameObject.transform.rotation);
					}
				}
			}
		}

		// Token: 0x06005BD0 RID: 23504 RVA: 0x001EF734 File Offset: 0x001ED934
		private bool ShouldAshen()
		{
			if (this.AshenSpawnProfile == null || this.SpawnController == null)
			{
				return false;
			}
			if (this.AlwaysGoAshen)
			{
				return true;
			}
			if (this.AshenController != null)
			{
				return this.AshenController.ShouldAshen(base.gameObject.transform.position);
			}
			return GlobalAshenController.Instance != null && GlobalAshenController.Instance.ShouldAshen();
		}

		// Token: 0x06005BD1 RID: 23505 RVA: 0x001EF7A8 File Offset: 0x001ED9A8
		private void DistributeAdventuringExperience()
		{
			if (!GameManager.IsServer || this.NpcContributed || this.PreventExperienceDistribution)
			{
				return;
			}
			if (this.EventCurrency > 0UL)
			{
				ServerGameManager.SpatialManager.PhysicsQueryRadius(base.GameEntity.gameObject.transform.position, 50f, InteractiveNpc.m_nearbyEntities, true, Hits.Colliders200);
				for (int i = 0; i < InteractiveNpc.m_nearbyEntities.Count; i++)
				{
					if (InteractiveNpc.m_nearbyEntities[i] && InteractiveNpc.m_nearbyEntities[i].GameEntity && InteractiveNpc.m_nearbyEntities[i].GameEntity.CollectionController != null)
					{
						InteractiveNpc.m_nearbyEntities[i].GameEntity.CollectionController.ModifyEventCurrency(this.EventCurrency, false);
					}
				}
			}
			if (base.TaggerEntity == null && this.m_groupId.Value.IsEmpty)
			{
				return;
			}
			int adventuringLevel = base.GameEntity.CharacterData.AdventuringLevel;
			float num = GlobalSettings.Values.Progression.AdventuringLevelCurve.GetRewardForLevel(adventuringLevel);
			num *= this.ChallengeRating.GetDefaultExperienceModifier();
			if (this.XpAdjustmentMultiplier != null)
			{
				num *= this.XpAdjustmentMultiplier.Value;
			}
			if (num <= 0f)
			{
				return;
			}
			if (base.TaggerEntity != null && this.m_groupId.Value.IsEmpty)
			{
				if ((base.GameEntity.gameObject.transform.position - base.TaggerEntity.gameObject.transform.position).sqrMagnitude <= 2500f)
				{
					ulong experiencePoints = (ulong)Mathf.Clamp(num, 1f, float.MaxValue);
					this.AddAdventuringExperienceToPlayer(base.TaggerEntity, experiencePoints, adventuringLevel);
					this.AddEssenceToPlayer(base.TaggerEntity, adventuringLevel);
					this.AdvanceKillObjectives(base.TaggerEntity);
					this.IncrementHuntingLog(base.TaggerEntity);
					return;
				}
			}
			else if (!this.m_groupId.Value.IsEmpty || !this.m_raidId.Value.IsEmpty)
			{
				InteractiveNpc.m_nearbyEntities.Clear();
				InteractiveNpc.m_compiledList.Clear();
				ServerGameManager.SpatialManager.PhysicsQueryRadius(base.GameEntity.gameObject.transform.position, 50f, InteractiveNpc.m_nearbyEntities, true, null);
				for (int j = 0; j < InteractiveNpc.m_nearbyEntities.Count; j++)
				{
					if (!(InteractiveNpc.m_nearbyEntities[j] == null) && !(InteractiveNpc.m_nearbyEntities[j].GameEntity == null) && InteractiveNpc.m_nearbyEntities[j].GameEntity.Type == GameEntityType.Player && !(InteractiveNpc.m_nearbyEntities[j].GameEntity.CharacterData == null) && (InteractiveNpc.m_nearbyEntities[j].GameEntity == base.TaggerEntity || (!this.m_groupId.Value.IsEmpty && InteractiveNpc.m_nearbyEntities[j].GameEntity.CharacterData.GroupId == this.m_groupId.Value) || (!this.m_raidId.Value.IsEmpty && InteractiveNpc.m_nearbyEntities[j].GameEntity.CharacterData.RaidId == this.m_raidId.Value)))
					{
						InteractiveNpc.m_compiledList.Add(InteractiveNpc.m_nearbyEntities[j]);
					}
				}
				if (InteractiveNpc.m_compiledList.Count > 0)
				{
					ulong experiencePerGroupMember = ProgressionCalculator.GetExperiencePerGroupMember(num, InteractiveNpc.m_compiledList.Count, this.ChallengeRating);
					for (int k = 0; k < InteractiveNpc.m_compiledList.Count; k++)
					{
						this.AddAdventuringExperienceToPlayer(InteractiveNpc.m_compiledList[k].GameEntity, experiencePerGroupMember, adventuringLevel);
						this.AddEssenceToPlayer(InteractiveNpc.m_compiledList[k].GameEntity, adventuringLevel);
						this.AdvanceKillObjectives(InteractiveNpc.m_compiledList[k].GameEntity);
						this.IncrementHuntingLog(InteractiveNpc.m_compiledList[k].GameEntity);
					}
				}
			}
		}

		// Token: 0x06005BD2 RID: 23506 RVA: 0x001EFC10 File Offset: 0x001EDE10
		private void AddAdventuringExperienceToPlayer(GameEntity entity, ulong experiencePoints, int targetLevel)
		{
			if (!this.m_raidId.Value.IsEmpty)
			{
				return;
			}
			ProgressionCalculator.OnNpcKilled(entity, experiencePoints, targetLevel, this.XpAdjustmentMultiplier != null && this.XpAdjustmentMultiplier.Value > 1f);
		}

		// Token: 0x06005BD3 RID: 23507 RVA: 0x001EFC64 File Offset: 0x001EDE64
		private void AddEssenceToPlayer(GameEntity entity, int targetLevel)
		{
			if (!this.m_raidId.Value.IsEmpty)
			{
				return;
			}
			if (entity && entity.CharacterData && entity.CollectionController != null && entity.CollectionController.CurrentEmberStone != null)
			{
				int levelDelta = entity.CharacterData.AdventuringLevel - targetLevel;
				if (this.IsAshen)
				{
					int num = GlobalSettings.Values.Ashen.GetAshenDistribution(levelDelta);
					if (num > 0)
					{
						num += this.ChallengeRating.GetEmberEssenceBonus(levelDelta);
						entity.CollectionController.AdjustEmberEssenceCount(num);
						return;
					}
				}
				else if (!this.m_isGoingAshen && this.AshenSpawnProfile)
				{
					int nonAshenDistribution = GlobalSettings.Values.Ashen.GetNonAshenDistribution(levelDelta, entity.CollectionController.GetEmberEssenceCount(), entity.CollectionController.CurrentEmberStone.MaxCapacity);
					if (nonAshenDistribution > 0)
					{
						entity.CollectionController.AdjustEmberEssenceCount(nonAshenDistribution);
					}
				}
			}
		}

		// Token: 0x06005BD4 RID: 23508 RVA: 0x001EFD5C File Offset: 0x001EDF5C
		private void AdvanceKillObjectives(GameEntity entity)
		{
			if (entity.CharacterData.ObjectiveOrders.HasOrders<KillNpcObjective>())
			{
				List<ValueTuple<UniqueId, KillNpcObjective>> pooledOrderList = entity.CharacterData.ObjectiveOrders.GetPooledOrderList<KillNpcObjective>();
				foreach (ValueTuple<UniqueId, KillNpcObjective> valueTuple in pooledOrderList)
				{
					if (valueTuple.Item2.Prevalidate(this, entity))
					{
						Quest quest;
						int hash;
						if (InternalGameDatabase.Quests.TryGetItem(valueTuple.Item1, out quest) && quest.TryGetObjectiveHashForActiveObjective(valueTuple.Item2.Id, entity, out hash))
						{
							GameManager.QuestManager.Progress(new ObjectiveIterationCache
							{
								QuestId = valueTuple.Item1,
								ObjectiveHashes = ObjectiveIterationCache.SharedSingleItemArray(hash),
								NpcEntity = this.m_netEntity
							}, entity, false);
						}
						BBTask bbtask;
						if (InternalGameDatabase.BBTasks.TryGetItem(valueTuple.Item1, out bbtask))
						{
							GameManager.QuestManager.ProgressTask(new ObjectiveIterationCache
							{
								QuestId = valueTuple.Item1,
								ObjectiveHashes = ObjectiveIterationCache.SharedSingleItemArray(valueTuple.Item2.CombinedId(valueTuple.Item1)),
								NpcEntity = this.m_netEntity
							}, entity, false);
						}
					}
				}
				entity.CharacterData.ObjectiveOrders.ReturnPooledOrderList<KillNpcObjective>(pooledOrderList);
			}
			foreach (ValueTuple<KillNpcObjective, ObjectiveIterationCache> valueTuple2 in ((ServerQuestManager)GameManager.QuestManager).KillStartQuests)
			{
				if (valueTuple2.Item1.Prevalidate(this, entity))
				{
					ObjectiveIterationCache item = valueTuple2.Item2;
					item.NpcEntity = this.m_netEntity;
					GameManager.QuestManager.Progress(item, entity, false);
				}
			}
		}

		// Token: 0x06005BD5 RID: 23509 RVA: 0x001EFF38 File Offset: 0x001EE138
		private void IncrementHuntingLog(GameEntity entity)
		{
			if (GlobalSettings.Values.HuntingLog.Disabled)
			{
				return;
			}
			if (this.IsAshen || !this.HuntingLogProfile || !this.HuntingLogProfile.ShowInLog || !entity || entity.CollectionController == null)
			{
				return;
			}
			if (entity.CollectionController.IncrementHuntingLog(this.HuntingLogProfile, this.Level) && this.HuntingLogProfile.ShowInLog && entity.NetworkEntity && entity.NetworkEntity.PlayerRpcHandler)
			{
				entity.NetworkEntity.PlayerRpcHandler.IncrementHuntingLog(this.HuntingLogProfile.Id);
			}
		}

		// Token: 0x06005BD6 RID: 23510 RVA: 0x0007D96D File Offset: 0x0007BB6D
		public override void BeginInteraction(GameEntity interactionSource)
		{
			if (!GameManager.IsServer || this.m_permissionsInitializedTime == null || !base.HasValidPermissions(interactionSource))
			{
				return;
			}
			base.BeginInteraction(interactionSource);
		}

		// Token: 0x06005BD7 RID: 23511 RVA: 0x001EFFEC File Offset: 0x001EE1EC
		public override bool ClientInteraction()
		{
			if (GameManager.IsServer)
			{
				return false;
			}
			if (base.GameEntity && base.GameEntity.VitalsReplicator && base.GameEntity.Targetable != null && base.GameEntity.VitalsReplicator.CurrentHealthState.Value != HealthState.Dead && LocalPlayer.GameEntity && LocalPlayer.GameEntity.TargetController)
			{
				TargetType playerTargetType = base.GameEntity.Targetable.Faction.GetPlayerTargetType();
				LocalPlayer.GameEntity.TargetController.SetTarget(playerTargetType, base.GameEntity.Targetable);
				if (playerTargetType == TargetType.Offensive && UIManager.AutoAttackButton)
				{
					UIManager.AutoAttackButton.InitiateCombat();
					return true;
				}
			}
			bool result = false;
			ArchetypeInstance archetypeInstance;
			if (base.GameEntity && base.GameEntity.VitalsReplicator && base.GameEntity.VitalsReplicator.CurrentHealthState.Value != HealthState.Dead && this.m_hasDialogue)
			{
				this.InitiateDialogue();
				result = true;
			}
			else if (!this.m_interactiveFlags.Value.HasBitFlag(InteractiveFlags.ResourceNode) || this.m_interactiveFlags.Value.HasBitFlag(InteractiveFlags.RecordGenerated))
			{
				result = base.ClientInteraction();
			}
			else if (this.m_interactiveFlags.Value.HasBitFlag(InteractiveFlags.ResourceNode) && !LocalPlayer.GameEntity.SkillsController.PendingIsActive && this.CanInteract(LocalPlayer.GameEntity) && base.GatheringParams.EntityCanInteract(LocalPlayer.GameEntity, this, out archetypeInstance))
			{
				ArchetypeInstance abilityInstance = base.GatheringParams.GetAbilityInstance(LocalPlayer.GameEntity);
				if (abilityInstance != null)
				{
					LocalPlayer.GameEntity.CollectionController.GatheringNode = this;
					LocalPlayer.GameEntity.SkillsController.BeginExecution(abilityInstance);
					result = true;
				}
				else
				{
					MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "You must know " + base.GatheringParams.GetGatheringMasteryDisplayName() + " to harvest!");
				}
			}
			return result;
		}

		// Token: 0x06005BD8 RID: 23512 RVA: 0x001F01F8 File Offset: 0x001EE3F8
		public bool InitiateDialogue()
		{
			if (!GameManager.IsServer)
			{
				NpcProfile npcProfile;
				InternalGameDatabase.Archetypes.TryGetAsType<NpcProfile>(base.GameEntity.CharacterData.NpcInitData.ProfileId, out npcProfile);
				DialogueSource dialogueSource2;
				DialogueSource dialogueSource = InternalGameDatabase.Archetypes.TryGetAsType<DialogueSource>(base.GameEntity.CharacterData.NpcInitData.OverrideDialogueId, out dialogueSource2) ? dialogueSource2 : npcProfile;
				if (dialogueSource != null && dialogueSource.HasAnyDialogue)
				{
					DialogueManager.InitiateDialogue(dialogueSource, this, null, DialogSourceType.NPC);
				}
				return true;
			}
			return false;
		}

		// Token: 0x06005BD9 RID: 23513 RVA: 0x001F0274 File Offset: 0x001EE474
		protected override void OnRecordEmpty()
		{
			if (this.ResourceTable == null || !this.ResourceTable.HasLoot)
			{
				base.OnRecordEmpty();
				return;
			}
			InteractiveFlags interactiveFlags = this.m_interactiveFlags.Value;
			if (this.m_isResourceNode)
			{
				if (interactiveFlags.HasBitFlag(InteractiveFlags.RecordGenerated))
				{
					base.OnRecordEmpty();
				}
				return;
			}
			interactiveFlags &= ~InteractiveFlags.RecordGenerated;
			interactiveFlags |= InteractiveFlags.ResourceNode;
			interactiveFlags |= InteractiveFlags.Interactive;
			this.m_interactiveFlags.Value = interactiveFlags;
			ContainerRecord record = this.m_record;
			if (record != null)
			{
				record.ReturnInstancesToPoolAndNullifyList();
			}
			this.m_record = null;
			this.m_isResourceNode = true;
		}

		// Token: 0x06005BDA RID: 23514 RVA: 0x0007D994 File Offset: 0x0007BB94
		protected override void InteractiveFlagsOnChanged(InteractiveFlags obj)
		{
			base.InteractiveFlagsOnChanged(obj);
			if (!GameManager.IsServer && !obj.HasBitFlag(InteractiveFlags.RecordGenerated) && this.m_record != null)
			{
				this.m_record = null;
			}
		}

		// Token: 0x17001594 RID: 5524
		// (get) Token: 0x06005BDB RID: 23515 RVA: 0x0007D9BC File Offset: 0x0007BBBC
		protected override bool m_hideTooltip
		{
			get
			{
				return base.GameEntity.Vitals.GetCurrentHealthState() != HealthState.Dead;
			}
		}

		// Token: 0x17001595 RID: 5525
		// (get) Token: 0x06005BDC RID: 23516 RVA: 0x001F0300 File Offset: 0x001EE500
		protected override string TooltipDescription
		{
			get
			{
				if (!this.m_interactiveFlags.Value.HasBitFlag(InteractiveFlags.ResourceNode))
				{
					return ZString.Format<string>("{0}'s Corpse", base.GameEntity.CharacterData.Name.Value);
				}
				return ZString.Format<string, string>("{0}'s Corpse\n{1}", base.GameEntity.CharacterData.Name.Value, base.GatheringParams.GetTooltipDescription(this));
			}
		}

		// Token: 0x06005BDD RID: 23517 RVA: 0x001F0370 File Offset: 0x001EE570
		private CursorType GetCursorType(bool isActive)
		{
			if (this.m_interactiveFlags.Value.HasBitFlag(InteractiveFlags.Destroy))
			{
				return CursorType.MainCursor;
			}
			if (base.GameEntity.Vitals.GetCurrentHealthState() == HealthState.Dead)
			{
				bool flag = this.m_interactiveFlags.Value.HasBitFlag(InteractiveFlags.RecordGenerated);
				if (this.m_interactiveFlags.Value.HasBitFlag(InteractiveFlags.ResourceNode) && !flag)
				{
					return base.GatheringParams.RequiredTool.GetCursorForTool(isActive);
				}
				if (!isActive)
				{
					return CursorType.MerchantCursorInactive;
				}
				return CursorType.MerchantCursor;
			}
			else
			{
				TargetType playerTargetType = base.GameEntity.CharacterData.Faction.GetPlayerTargetType();
				if (playerTargetType == TargetType.Offensive)
				{
					return CursorType.SwordCursor1;
				}
				if (playerTargetType != TargetType.Defensive)
				{
					return CursorType.MainCursor;
				}
				if (this.m_hasDialogue)
				{
					if (!isActive)
					{
						return CursorType.TextCursorInactive;
					}
					return CursorType.TextCursor;
				}
				else
				{
					if (!isActive)
					{
						return CursorType.GloveCursorInactive;
					}
					return CursorType.GloveCursor;
				}
			}
		}

		// Token: 0x17001596 RID: 5526
		// (get) Token: 0x06005BDE RID: 23518 RVA: 0x0007D9D4 File Offset: 0x0007BBD4
		protected override CursorType ActiveCursorType
		{
			get
			{
				return this.GetCursorType(true);
			}
		}

		// Token: 0x17001597 RID: 5527
		// (get) Token: 0x06005BDF RID: 23519 RVA: 0x0007D9DD File Offset: 0x0007BBDD
		protected override CursorType InactiveCursorType
		{
			get
			{
				return this.GetCursorType(false);
			}
		}

		// Token: 0x17001598 RID: 5528
		// (get) Token: 0x06005BE0 RID: 23520 RVA: 0x0007D9E6 File Offset: 0x0007BBE6
		float IGatheringNode.GatherTime
		{
			get
			{
				return (float)base.GatheringParams.GatherTime;
			}
		}

		// Token: 0x17001599 RID: 5529
		// (get) Token: 0x06005BE1 RID: 23521 RVA: 0x0007D9F4 File Offset: 0x0007BBF4
		CraftingToolType IGatheringNode.RequiredTool
		{
			get
			{
				return base.GatheringParams.RequiredTool;
			}
		}

		// Token: 0x06005BE2 RID: 23522 RVA: 0x001F0424 File Offset: 0x001EE624
		MasteryArchetype IGatheringNode.GetGatheringMastery()
		{
			return base.GatheringParams.GetGatheringMastery();
		}

		// Token: 0x1700159A RID: 5530
		// (get) Token: 0x06005BE3 RID: 23523 RVA: 0x0007DA01 File Offset: 0x0007BC01
		UniqueId? IGatheringNode.RequiredItemId
		{
			get
			{
				return base.GatheringParams.RequiredItemId;
			}
		}

		// Token: 0x1700159B RID: 5531
		// (get) Token: 0x06005BE4 RID: 23524 RVA: 0x0007DA0E File Offset: 0x0007BC0E
		bool IGatheringNode.RemoveRequiredItemOnUse
		{
			get
			{
				return base.GatheringParams.RemoveRequiredItemOnUse;
			}
		}

		// Token: 0x1700159C RID: 5532
		// (get) Token: 0x06005BE5 RID: 23525 RVA: 0x0007DA1B File Offset: 0x0007BC1B
		InteractiveFlags IGatheringNode.InteractiveFlags
		{
			get
			{
				return this.m_interactiveFlags.Value;
			}
		}

		// Token: 0x06005BE6 RID: 23526 RVA: 0x001F0440 File Offset: 0x001EE640
		bool IGatheringNode.CanInteract(GameEntity entity, out ArchetypeInstance requiredItemInstance)
		{
			requiredItemInstance = null;
			return this.CanInteract(entity) && base.GatheringParams.EntityCanInteract(entity, this, out requiredItemInstance);
		}

		// Token: 0x1700159D RID: 5533
		// (get) Token: 0x06005BE7 RID: 23527 RVA: 0x001F046C File Offset: 0x001EE66C
		int IGatheringNode.ResourceLevel
		{
			get
			{
				if (base.GatheringParams.Level > 0)
				{
					return base.GatheringParams.Level;
				}
				if (!(base.GameEntity != null) || !(base.GameEntity.CharacterData != null))
				{
					return 1;
				}
				return base.GameEntity.CharacterData.AdventuringLevel;
			}
		}

		// Token: 0x1700159E RID: 5534
		// (get) Token: 0x06005BE8 RID: 23528 RVA: 0x0007DA28 File Offset: 0x0007BC28
		bool ILootRollSource.LootRollIsPending
		{
			get
			{
				return this.m_interactiveFlags.Value.HasBitFlag(InteractiveFlags.LootRollPending);
			}
		}

		// Token: 0x06005BE9 RID: 23529 RVA: 0x0007DA3C File Offset: 0x0007BC3C
		void ILootRollSource.SetLootRollCount(int count)
		{
			if (count > 0)
			{
				this.m_lootRollCount = new int?(count);
				this.m_interactiveFlags.Value |= InteractiveFlags.LootRollPending;
			}
		}

		// Token: 0x06005BEA RID: 23530 RVA: 0x001F04C8 File Offset: 0x001EE6C8
		void ILootRollSource.LootRollCompleted()
		{
			if (this.m_lootRollCount != null)
			{
				this.m_lootRollCount--;
				if (this.m_lootRollCount.Value <= 0)
				{
					this.m_lootRollCount = null;
					this.m_interactiveFlags.Value &= ~InteractiveFlags.LootRollPending;
				}
			}
		}

		// Token: 0x06005BEB RID: 23531 RVA: 0x0007DA62 File Offset: 0x0007BC62
		void ILootRollSource.RemoveFromRecord(ArchetypeInstance instance)
		{
			base.RemoveFromRecordIfPresent(instance);
		}

		// Token: 0x1700159F RID: 5535
		// (get) Token: 0x06005BEC RID: 23532 RVA: 0x001F0544 File Offset: 0x001EE744
		bool ILootRollSource.IsRaid
		{
			get
			{
				return !this.m_raidId.Value.IsEmpty;
			}
		}

		// Token: 0x170015A0 RID: 5536
		// (get) Token: 0x06005BED RID: 23533 RVA: 0x0007DA6B File Offset: 0x0007BC6B
		ChallengeRating ILootRollSource.ChallengeRating
		{
			get
			{
				return this.ChallengeRating;
			}
		}

		// Token: 0x06005BEE RID: 23534 RVA: 0x0007DA73 File Offset: 0x0007BC73
		bool ISpawnController.TryGetBehaviorProfile(out BehaviorProfile profile)
		{
			profile = null;
			return this.SpawnController != null && this.SpawnController.TryGetBehaviorProfile(out profile);
		}

		// Token: 0x06005BEF RID: 23535 RVA: 0x0007DA8E File Offset: 0x0007BC8E
		bool ISpawnController.TryGetLevel(out int level)
		{
			level = this.Level;
			return this.SpawnController != null && this.SpawnController.TryGetLevel(out level);
		}

		// Token: 0x170015A1 RID: 5537
		// (get) Token: 0x06005BF0 RID: 23536 RVA: 0x0007DAAE File Offset: 0x0007BCAE
		BehaviorSubTreeCollection ISpawnController.BehaviorOverrides
		{
			get
			{
				ISpawnController spawnController = this.SpawnController;
				if (spawnController == null)
				{
					return null;
				}
				return spawnController.BehaviorOverrides;
			}
		}

		// Token: 0x170015A2 RID: 5538
		// (get) Token: 0x06005BF1 RID: 23537 RVA: 0x0007DAC1 File Offset: 0x0007BCC1
		DialogueSource ISpawnController.OverrideDialogue
		{
			get
			{
				ISpawnController spawnController = this.SpawnController;
				if (spawnController == null)
				{
					return null;
				}
				return spawnController.OverrideDialogue;
			}
		}

		// Token: 0x06005BF2 RID: 23538 RVA: 0x0007DAD4 File Offset: 0x0007BCD4
		int ISpawnController.GetLevel()
		{
			return this.Level;
		}

		// Token: 0x170015A3 RID: 5539
		// (get) Token: 0x06005BF3 RID: 23539 RVA: 0x001F0568 File Offset: 0x001EE768
		float? ISpawnController.LeashDistance
		{
			get
			{
				ISpawnController spawnController = this.SpawnController;
				if (spawnController == null)
				{
					return null;
				}
				return spawnController.LeashDistance;
			}
		}

		// Token: 0x170015A4 RID: 5540
		// (get) Token: 0x06005BF4 RID: 23540 RVA: 0x001F0590 File Offset: 0x001EE790
		float? ISpawnController.ResetDistance
		{
			get
			{
				ISpawnController spawnController = this.SpawnController;
				if (spawnController == null)
				{
					return null;
				}
				return spawnController.ResetDistance;
			}
		}

		// Token: 0x170015A5 RID: 5541
		// (get) Token: 0x06005BF5 RID: 23541 RVA: 0x0007DADC File Offset: 0x0007BCDC
		bool ISpawnController.DespawnOnDeath
		{
			get
			{
				return this.SpawnController != null && this.SpawnController.DespawnOnDeath;
			}
		}

		// Token: 0x170015A6 RID: 5542
		// (get) Token: 0x06005BF6 RID: 23542 RVA: 0x0007DAF3 File Offset: 0x0007BCF3
		bool ISpawnController.CallForHelpRequiresLos
		{
			get
			{
				return this.SpawnController != null && this.SpawnController.CallForHelpRequiresLos;
			}
		}

		// Token: 0x170015A7 RID: 5543
		// (get) Token: 0x06005BF7 RID: 23543 RVA: 0x0007DB0A File Offset: 0x0007BD0A
		bool ISpawnController.ForceIndoorProfiles
		{
			get
			{
				return this.SpawnController != null && this.SpawnController.ForceIndoorProfiles;
			}
		}

		// Token: 0x170015A8 RID: 5544
		// (get) Token: 0x06005BF8 RID: 23544 RVA: 0x0007DB21 File Offset: 0x0007BD21
		bool ISpawnController.MatchAttackerLevel
		{
			get
			{
				return this.SpawnController != null && this.SpawnController.MatchAttackerLevel;
			}
		}

		// Token: 0x170015A9 RID: 5545
		// (get) Token: 0x06005BF9 RID: 23545 RVA: 0x0007DB38 File Offset: 0x0007BD38
		bool ISpawnController.LogSpawns
		{
			get
			{
				return this.SpawnController != null && this.SpawnController.LogSpawns;
			}
		}

		// Token: 0x06005BFA RID: 23546 RVA: 0x0007DB4F File Offset: 0x0007BD4F
		bool ISpawnController.OverrideInteractionFlags(out NpcInteractionFlags flags)
		{
			flags = NpcInteractionFlags.None;
			return this.SpawnController != null && this.SpawnController.OverrideInteractionFlags(out flags);
		}

		// Token: 0x170015AA RID: 5546
		// (get) Token: 0x06005BFB RID: 23547 RVA: 0x0007DB6A File Offset: 0x0007BD6A
		MinMaxIntRange ISpawnController.LevelRange
		{
			get
			{
				if (this.SpawnController == null)
				{
					return new MinMaxIntRange(1, 1);
				}
				return this.SpawnController.LevelRange;
			}
		}

		// Token: 0x170015AB RID: 5547
		// (get) Token: 0x06005BFC RID: 23548 RVA: 0x001F05B8 File Offset: 0x001EE7B8
		Vector3? ISpawnController.CurrentPosition
		{
			get
			{
				ISpawnController spawnController = this.SpawnController;
				if (spawnController == null)
				{
					return null;
				}
				return spawnController.CurrentPosition;
			}
		}

		// Token: 0x06005BFD RID: 23549 RVA: 0x0007DB87 File Offset: 0x0007BD87
		void ISpawnController.NotifyOfDeath(GameEntity entity)
		{
			ISpawnController spawnController = this.SpawnController;
			if (spawnController == null)
			{
				return;
			}
			spawnController.NotifyOfDeath(entity);
		}

		// Token: 0x170015AC RID: 5548
		// (get) Token: 0x06005BFE RID: 23550 RVA: 0x0007DB9A File Offset: 0x0007BD9A
		int ISpawnController.XpAdjustment
		{
			get
			{
				if (this.SpawnController == null)
				{
					return 0;
				}
				return this.SpawnController.XpAdjustment;
			}
		}

		// Token: 0x06005BFF RID: 23551 RVA: 0x0005BFF3 File Offset: 0x0005A1F3
		bool ISpawnController.TryGetOverrideData(SpawnProfile profile, out SpawnControllerOverrideData data)
		{
			data = null;
			return false;
		}

		// Token: 0x04004FED RID: 20461
		[SerializeField]
		private HuntingLogProfile m_huntingLogProfile;

		// Token: 0x04004FF7 RID: 20471
		private int? m_distributeExperienceFrame;

		// Token: 0x04004FF8 RID: 20472
		private bool m_isGoingAshen;

		// Token: 0x04004FFA RID: 20474
		private bool m_isResourceNode;

		// Token: 0x04004FFB RID: 20475
		private bool m_hasDialogue;

		// Token: 0x04004FFC RID: 20476
		private bool m_npcContributed;

		// Token: 0x04004FFD RID: 20477
		private static List<NetworkEntity> m_nearbyEntities;

		// Token: 0x04004FFE RID: 20478
		private static List<NetworkEntity> m_compiledList;

		// Token: 0x04004FFF RID: 20479
		private int? m_lootRollCount;
	}
}

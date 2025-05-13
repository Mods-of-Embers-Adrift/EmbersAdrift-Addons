using System;
using System.Collections;
using System.Collections.Generic;
using SoL.Game.NPCs;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Containers;
using SoL.Game.Player;
using SoL.Managers;
using SoL.Networking.Database;
using SoL.Networking.Managers;
using SoL.Networking.Replication;
using SoL.Networking.SolServer;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game
{
	// Token: 0x02000568 RID: 1384
	public class PlayerCharacterData : CharacterData
	{
		// Token: 0x170008C1 RID: 2241
		// (get) Token: 0x06002A58 RID: 10840 RVA: 0x0005D2C8 File Offset: 0x0005B4C8
		// (set) Token: 0x06002A59 RID: 10841 RVA: 0x0005D2D5 File Offset: 0x0005B4D5
		public override UniqueId? EmberStoneId
		{
			get
			{
				return this.m_emberStoneId.Value;
			}
			set
			{
				if (GameManager.IsServer)
				{
					this.m_emberStoneId.Value = value;
				}
			}
		}

		// Token: 0x170008C2 RID: 2242
		// (get) Token: 0x06002A5A RID: 10842 RVA: 0x0005D2EA File Offset: 0x0005B4EA
		// (set) Token: 0x06002A5B RID: 10843 RVA: 0x0005D2F7 File Offset: 0x0005B4F7
		public override EmberStoneFillLevels EmberStoneFillLevel
		{
			get
			{
				return this.m_emberStoneFillLevel.Value;
			}
			set
			{
				if (GameManager.IsServer)
				{
					this.m_emberStoneFillLevel.Value = value;
				}
			}
		}

		// Token: 0x170008C3 RID: 2243
		// (get) Token: 0x06002A5C RID: 10844 RVA: 0x0005D30C File Offset: 0x0005B50C
		// (set) Token: 0x06002A5D RID: 10845 RVA: 0x0004475B File Offset: 0x0004295B
		public override NpcTagSet NpcTagsSet
		{
			get
			{
				if (this.m_npcTagSet == null)
				{
					this.m_npcTagSet = new NpcTagSet(NpcTags.Player, NpcTagsB.None);
				}
				return this.m_npcTagSet;
			}
			set
			{
			}
		}

		// Token: 0x170008C4 RID: 2244
		// (get) Token: 0x06002A5E RID: 10846 RVA: 0x0005D329 File Offset: 0x0005B529
		public SynchronizedStruct<NearbyGroupInfo> SyncedNearbyGroupInfo
		{
			get
			{
				return this.m_nearbyGroupInfo;
			}
		}

		// Token: 0x170008C5 RID: 2245
		// (get) Token: 0x06002A5F RID: 10847 RVA: 0x00053500 File Offset: 0x00051700
		// (set) Token: 0x06002A60 RID: 10848 RVA: 0x0004475B File Offset: 0x0004295B
		public override Faction Faction
		{
			get
			{
				return Faction.Player;
			}
			set
			{
			}
		}

		// Token: 0x170008C6 RID: 2246
		// (get) Token: 0x06002A61 RID: 10849 RVA: 0x0005D331 File Offset: 0x0005B531
		// (set) Token: 0x06002A62 RID: 10850 RVA: 0x0005D33E File Offset: 0x0005B53E
		public override bool HideEmberStone
		{
			get
			{
				return this.m_hideEmberStone.Value;
			}
			set
			{
				if (this.m_netEntity.IsLocal)
				{
					this.m_hideEmberStone.Value = value;
				}
			}
		}

		// Token: 0x170008C7 RID: 2247
		// (get) Token: 0x06002A63 RID: 10851 RVA: 0x0005D359 File Offset: 0x0005B559
		// (set) Token: 0x06002A64 RID: 10852 RVA: 0x0005D366 File Offset: 0x0005B566
		public override bool HideHelm
		{
			get
			{
				return this.m_hideHelm.Value;
			}
			set
			{
				if (this.m_netEntity.IsLocal)
				{
					this.m_hideHelm.Value = value;
				}
			}
		}

		// Token: 0x170008C8 RID: 2248
		// (get) Token: 0x06002A65 RID: 10853 RVA: 0x0005D381 File Offset: 0x0005B581
		// (set) Token: 0x06002A66 RID: 10854 RVA: 0x0005D38E File Offset: 0x0005B58E
		public override bool BlockInspections
		{
			get
			{
				return this.m_blockInspections.Value;
			}
			set
			{
				if (this.m_netEntity.IsLocal)
				{
					this.m_blockInspections.Value = value;
				}
			}
		}

		// Token: 0x170008C9 RID: 2249
		// (get) Token: 0x06002A67 RID: 10855 RVA: 0x0005D3A9 File Offset: 0x0005B5A9
		// (set) Token: 0x06002A68 RID: 10856 RVA: 0x0005D3B6 File Offset: 0x0005B5B6
		public override bool PauseAdventuringExperience
		{
			get
			{
				return this.m_pauseAdventuringExperience.Value;
			}
			set
			{
				if (this.m_netEntity.IsLocal)
				{
					this.m_pauseAdventuringExperience.Value = value;
					base.InvokePauseAdventuringExperienceChanged();
				}
			}
		}

		// Token: 0x170008CA RID: 2250
		// (get) Token: 0x06002A69 RID: 10857 RVA: 0x0005D3D7 File Offset: 0x0005B5D7
		public override bool IsLfg
		{
			get
			{
				return this.m_lfg.Value;
			}
		}

		// Token: 0x170008CB RID: 2251
		// (get) Token: 0x06002A6A RID: 10858 RVA: 0x0005D3E4 File Offset: 0x0005B5E4
		// (set) Token: 0x06002A6B RID: 10859 RVA: 0x0005D3F1 File Offset: 0x0005B5F1
		public override UniqueId CharacterId
		{
			get
			{
				return this.m_characterId.Value;
			}
			set
			{
				this.m_characterId.Value = value;
			}
		}

		// Token: 0x170008CC RID: 2252
		// (get) Token: 0x06002A6C RID: 10860 RVA: 0x0005D3FF File Offset: 0x0005B5FF
		// (set) Token: 0x06002A6D RID: 10861 RVA: 0x0005D40C File Offset: 0x0005B60C
		public override UniqueId GroupId
		{
			get
			{
				return this.m_groupId.Value;
			}
			set
			{
				this.m_groupId.Value = value;
			}
		}

		// Token: 0x170008CD RID: 2253
		// (get) Token: 0x06002A6E RID: 10862 RVA: 0x0005D41A File Offset: 0x0005B61A
		// (set) Token: 0x06002A6F RID: 10863 RVA: 0x0005D427 File Offset: 0x0005B627
		public override UniqueId RaidId
		{
			get
			{
				return this.m_raidId.Value;
			}
			set
			{
				this.m_raidId.Value = value;
			}
		}

		// Token: 0x170008CE RID: 2254
		// (get) Token: 0x06002A70 RID: 10864 RVA: 0x0005D435 File Offset: 0x0005B635
		public override int GroupMembersNearby
		{
			get
			{
				return (int)this.m_nearbyGroupInfo.Value.GroupMembersNearby;
			}
		}

		// Token: 0x170008CF RID: 2255
		// (get) Token: 0x06002A71 RID: 10865 RVA: 0x0005D447 File Offset: 0x0005B647
		public override int GroupedLevel
		{
			get
			{
				return (int)this.m_nearbyGroupInfo.Value.GroupedLevel;
			}
		}

		// Token: 0x170008D0 RID: 2256
		// (get) Token: 0x06002A72 RID: 10866 RVA: 0x0005D459 File Offset: 0x0005B659
		public override List<GameEntity> NearbyGroupMembers
		{
			get
			{
				if (this.m_nearbyGroupMembers == null)
				{
					this.m_nearbyGroupMembers = StaticListPool<GameEntity>.GetFromPool();
				}
				return this.m_nearbyGroupMembers;
			}
		}

		// Token: 0x170008D1 RID: 2257
		// (get) Token: 0x06002A73 RID: 10867 RVA: 0x0005D474 File Offset: 0x0005B674
		public override List<GameEntity> NearbyRaidMembers
		{
			get
			{
				if (this.m_nearbyRaidMembers == null)
				{
					this.m_nearbyRaidMembers = StaticListPool<GameEntity>.GetFromPool();
				}
				return this.m_nearbyRaidMembers;
			}
		}

		// Token: 0x170008D2 RID: 2258
		// (get) Token: 0x06002A74 RID: 10868 RVA: 0x0005D48F File Offset: 0x0005B68F
		// (set) Token: 0x06002A75 RID: 10869 RVA: 0x0005D49C File Offset: 0x0005B69C
		public override UniqueId BaseRoleId
		{
			get
			{
				return this.m_baseRoleId.Value;
			}
			set
			{
				if (GameManager.IsServer)
				{
					this.m_baseRoleId.Value = value;
				}
			}
		}

		// Token: 0x170008D3 RID: 2259
		// (get) Token: 0x06002A76 RID: 10870 RVA: 0x0005D4B1 File Offset: 0x0005B6B1
		// (set) Token: 0x06002A77 RID: 10871 RVA: 0x0005D4BE File Offset: 0x0005B6BE
		public override UniqueId SpecializedRoleId
		{
			get
			{
				return this.m_specializedRoleId.Value;
			}
			set
			{
				if (GameManager.IsServer)
				{
					this.m_specializedRoleId.Value = value;
				}
			}
		}

		// Token: 0x170008D4 RID: 2260
		// (get) Token: 0x06002A78 RID: 10872 RVA: 0x0005D4D3 File Offset: 0x0005B6D3
		// (set) Token: 0x06002A79 RID: 10873 RVA: 0x0005D4E0 File Offset: 0x0005B6E0
		public override bool MainHand_SecondaryActive
		{
			get
			{
				return this.m_secondaryWeaponsActive.Value;
			}
			set
			{
				if (this.MainHand_SecondaryActive == value)
				{
					return;
				}
				if (this.m_netEntity.IsLocal)
				{
					this.m_secondaryWeaponsActive.Value = value;
					this.RefreshRole();
				}
				base.InvokeHandConfigurationChanged();
			}
		}

		// Token: 0x170008D5 RID: 2261
		// (get) Token: 0x06002A7A RID: 10874 RVA: 0x0005D4D3 File Offset: 0x0005B6D3
		public override bool OffHand_SecondaryActive
		{
			get
			{
				return this.m_secondaryWeaponsActive.Value;
			}
		}

		// Token: 0x06002A7B RID: 10875 RVA: 0x0005D511 File Offset: 0x0005B711
		public override void SetSecondaryWeaponsActive(bool isActive)
		{
			this.MainHand_SecondaryActive = isActive;
		}

		// Token: 0x170008D6 RID: 2262
		// (get) Token: 0x06002A7C RID: 10876 RVA: 0x0005D51A File Offset: 0x0005B71A
		// (set) Token: 0x06002A7D RID: 10877 RVA: 0x00142DDC File Offset: 0x00140FDC
		public UniqueId TrackedMastery
		{
			get
			{
				return this.m_record.Settings.TrackedMastery;
			}
			set
			{
				if (value == this.m_record.Settings.TrackedMastery)
				{
					return;
				}
				if (this.m_netEntity.IsLocal)
				{
					base.GameEntity.NetworkEntity.PlayerRpcHandler.SetTrackedMasteryOption(value);
					this.m_record.Settings.TrackedMastery = value;
				}
			}
		}

		// Token: 0x170008D7 RID: 2263
		// (get) Token: 0x06002A7E RID: 10878 RVA: 0x0005D52C File Offset: 0x0005B72C
		// (set) Token: 0x06002A7F RID: 10879 RVA: 0x00142E38 File Offset: 0x00141038
		public override Presence Presence
		{
			get
			{
				return this.m_record.Settings.Presence;
			}
			set
			{
				if (this.m_record.Settings.Presence == value)
				{
					return;
				}
				if (!this.m_netEntity.IsLocal)
				{
					return;
				}
				PresenceFlags presenceFlags = this.m_presenceFlags.Value;
				presenceFlags &= ~this.m_record.Settings.Presence.ToFlags();
				this.m_record.Settings.Presence = value;
				presenceFlags |= this.m_record.Settings.Presence.ToFlags();
				this.PresenceFlags = presenceFlags;
			}
		}

		// Token: 0x170008D8 RID: 2264
		// (get) Token: 0x06002A80 RID: 10880 RVA: 0x0005D53E File Offset: 0x0005B73E
		// (set) Token: 0x06002A81 RID: 10881 RVA: 0x00142EC0 File Offset: 0x001410C0
		public override PresenceFlags PresenceFlags
		{
			get
			{
				return this.m_presenceFlags.Value;
			}
			set
			{
				if (!this.m_netEntity.IsLocal)
				{
					return;
				}
				this.m_presenceFlags.Value = value;
				base.InvokePresenceChanged();
				Dictionary<string, object> dictionary = SolServerCommandDictionaryPool.GetDictionary();
				dictionary.Add("presence", (int)this.m_presenceFlags.Value);
				SolServerConnectionManager.CurrentConnection.Send(CommandClass.client.NewCommand(CommandType.presence, dictionary));
			}
		}

		// Token: 0x170008D9 RID: 2265
		// (get) Token: 0x06002A82 RID: 10882 RVA: 0x0005D54B File Offset: 0x0005B74B
		// (set) Token: 0x06002A83 RID: 10883 RVA: 0x0004475B File Offset: 0x0004295B
		public override bool IsSwimming
		{
			get
			{
				return base.GameEntity.Vitals != null && base.GameEntity.Vitals.Stance == Stance.Swim;
			}
			set
			{
			}
		}

		// Token: 0x170008DA RID: 2266
		// (get) Token: 0x06002A84 RID: 10884 RVA: 0x0005D575 File Offset: 0x0005B775
		public override bool IsSubscriber
		{
			get
			{
				return this.m_isSubscriber.Value;
			}
		}

		// Token: 0x170008DB RID: 2267
		// (get) Token: 0x06002A85 RID: 10885 RVA: 0x0005D582 File Offset: 0x0005B782
		public override bool IsTrial
		{
			get
			{
				return this.m_isTrial.Value;
			}
		}

		// Token: 0x170008DC RID: 2268
		// (get) Token: 0x06002A86 RID: 10886 RVA: 0x0005D58F File Offset: 0x0005B78F
		public override bool IsGM
		{
			get
			{
				return this.m_presenceFlags.Value.HasBitFlag(PresenceFlags.GM);
			}
		}

		// Token: 0x06002A87 RID: 10887 RVA: 0x00142F24 File Offset: 0x00141124
		protected override void Awake()
		{
			base.Awake();
			this.CurrentStanceData.PermitClientToModify(this);
			this.CurrentCombatId.PermitClientToModify(this);
			this.m_secondaryWeaponsActive.PermitClientToModify(this);
			this.ItemsAttached.PermitClientToModify(this);
			this.GuildName.PermitClientToModify(this);
			this.m_presenceFlags.PermitClientToModify(this);
			this.m_lfg.PermitClientToModify(this);
		}

		// Token: 0x06002A88 RID: 10888 RVA: 0x0005D5A3 File Offset: 0x0005B7A3
		protected override void OnDestroy()
		{
			if (this.m_nearbyGroupMembers != null)
			{
				StaticListPool<GameEntity>.ReturnToPool(this.m_nearbyGroupMembers);
			}
			if (this.m_nearbyRaidMembers != null)
			{
				StaticListPool<GameEntity>.ReturnToPool(this.m_nearbyRaidMembers);
			}
			base.OnDestroy();
		}

		// Token: 0x06002A89 RID: 10889 RVA: 0x00142F8C File Offset: 0x0014118C
		public override void InitializeFromRecord(CharacterRecord record)
		{
			base.InitializeFromRecord(record);
			this.m_hideEmberStone.SetInitialValue(this.m_record.Settings.HideEmberStone);
			this.m_hideEmberStone.PermitClientToModify(this);
			this.m_hideHelm.SetInitialValue(this.m_record.Settings.HideHelm);
			this.m_hideHelm.PermitClientToModify(this);
			this.m_blockInspections.SetInitialValue(this.m_record.Settings.BlockInspections);
			this.m_blockInspections.PermitClientToModify(this);
			this.m_pauseAdventuringExperience.SetInitialValue(this.m_record.Settings.PauseAdventuringExperience);
			this.m_pauseAdventuringExperience.PermitClientToModify(this);
			if (GameManager.IsServer)
			{
				if (base.GameEntity)
				{
					if (base.GameEntity.Subscriber)
					{
						this.m_isSubscriber.Value = true;
					}
					if (base.GameEntity.IsTrial)
					{
						this.m_isTrial.Value = true;
					}
				}
				this.m_characterId.SetInitialValue(new UniqueId(this.m_record.Id));
				this.Name.SetInitialValue(this.m_record.Name);
				this.Title.SetInitialValue(this.m_record.Title);
				this.m_characterVisuals.SetInitialValue(this.m_record.Visuals);
				this.PortraitId.Value = this.m_record.Settings.PortraitId;
				if (this.m_record.EmberStoneData != null && !this.m_record.EmberStoneData.StoneId.IsEmpty)
				{
					EmberStone stoneItem;
					if (InternalGameDatabase.Archetypes.TryGetAsType<EmberStone>(this.m_record.EmberStoneData.StoneId, out stoneItem))
					{
						this.m_emberStoneFillLevel.Value = stoneItem.GetFillLevel(this.m_record.EmberStoneData.Count);
					}
					this.m_emberStoneId.Value = new UniqueId?(this.m_record.EmberStoneData.StoneId);
				}
				if (this.m_record.Corpse != null)
				{
					this.CharacterFlags.Value |= PlayerFlags.MissingBag;
				}
				ContainerRecord containerRecord;
				if (this.m_record.Storage.TryGetValue(ContainerType.Masteries, out containerRecord))
				{
					for (int i = 0; i < containerRecord.Instances.Count; i++)
					{
						ArchetypeInstance archetypeInstance = containerRecord.Instances[i];
						BaseRole baseRole;
						if (archetypeInstance != null && archetypeInstance.Archetype && archetypeInstance.Archetype.TryGetAsType(out baseRole) && baseRole.Type == MasteryType.Combat)
						{
							this.BaseRoleId = baseRole.Id;
							this.SpecializedRoleId = ((archetypeInstance.MasteryData != null && archetypeInstance.MasteryData.Specialization != null) ? archetypeInstance.MasteryData.Specialization.Value : UniqueId.Empty);
							break;
						}
					}
					MasteryArchetype.RefreshHighestLevelMastery(base.GameEntity, containerRecord.Instances);
				}
				base.InvokeRepeating("ServerAutoAfk", 60f, 60f);
				return;
			}
			if (this.m_netEntity.IsLocal)
			{
				this.SetSecondaryWeaponsActive(this.m_record.Settings.SecondaryWeaponsActive);
				this.CurrentCombatId.Value = base.ActiveMasteryId;
				this.RefreshRole();
				this.RefreshLocalLfg();
				ContainerRecord containerRecord2;
				if (this.m_record.Storage.TryGetValue(ContainerType.Masteries, out containerRecord2))
				{
					MasteryArchetype.RefreshHighestLevelMastery(base.GameEntity, containerRecord2.Instances);
				}
				base.gameObject.AddComponent<AutoAfkController>().Init(this);
			}
		}

		// Token: 0x06002A8A RID: 10890 RVA: 0x001432FC File Offset: 0x001414FC
		public override void InitPresenceFlags()
		{
			base.InitPresenceFlags();
			if (this.m_netEntity.IsLocal && this.m_record != null && this.m_record.Settings != null)
			{
				if (this.m_record.Settings.Presence == Presence.Away)
				{
					this.Presence = Presence.Online;
					return;
				}
				if (this.m_record.Settings.Presence == Presence.Invisible)
				{
					this.Presence = Presence.Online;
					return;
				}
				PresenceFlags presenceFlags = this.Presence.ToFlags();
				if (PlayerPrefs.GetInt("gm_enabled_" + this.CharacterId, 0) == 1)
				{
					presenceFlags |= PresenceFlags.GM;
				}
				this.PresenceFlags = presenceFlags;
			}
		}

		// Token: 0x06002A8B RID: 10891 RVA: 0x001433A0 File Offset: 0x001415A0
		protected override void Subscribe()
		{
			base.Subscribe();
			if (GameManager.IsServer)
			{
				if (base.GameEntity && base.GameEntity.VitalsReplicator)
				{
					base.GameEntity.VitalsReplicator.CurrentStance.Changed += this.CurrentStanceOnChanged;
					this.CurrentStanceOnChanged(base.GameEntity.VitalsReplicator.CurrentStance.Value);
					base.GameEntity.VitalsReplicator.CurrentHealthState.Changed += this.CurrentHealthStateOnChanged;
					this.CurrentHealthStateOnChanged(base.GameEntity.VitalsReplicator.CurrentHealthState.Value);
				}
				this.m_hideHelm.Changed += this.HideHelmOnChanged;
				this.m_blockInspections.Changed += this.BlockInspectionsOnChanged;
				this.m_pauseAdventuringExperience.Changed += this.PauseAdventuringExperienceOnChanged;
				this.m_hideEmberStone.Changed += this.HideEmberStoneOnChanged;
				this.m_emberStoneId.Changed += this.EmberStoneIdOnChanged;
			}
			if (!this.m_netEntity.IsLocal)
			{
				this.m_secondaryWeaponsActive.Changed += this.HandActiveFlagsOnChanged;
				this.m_presenceFlags.Changed += this.PresenceFlagsOnChanged;
				this.m_lfg.Changed += this.LfgOnChanged;
			}
			else
			{
				ClientGameManager.SocialManager.LookingUpdated += this.RefreshLocalLfg;
				this.AdventuringLevelSync.Changed += this.AdventuringLevelChanged;
				this.PortraitId.Changed += this.PortraitIdOnChanged;
				this.CharacterFlags.Changed += this.CharacterFlagsOnChanged;
				this.m_previousPlayerFlags = this.CharacterFlags.Value;
			}
			if (!GameManager.IsServer)
			{
				this.m_characterVisuals.Changed += this.CharacterVisualsOnChanged;
				this.m_visibleEquipment.Changed += this.VisibleEquipmentOnChanged;
				this.m_visibleEquipment.ReadComplete += this.VisibleEquipmentOnReadComplete;
				this.m_baseRoleId.Changed += this.RoleIdOnChanged;
				this.m_specializedRoleId.Changed += this.RoleIdOnChanged;
				this.m_emberStoneFillLevel.Changed += this.EmberStoneFillLevelOnChanged;
			}
		}

		// Token: 0x06002A8C RID: 10892 RVA: 0x00143620 File Offset: 0x00141820
		protected override void Unsubscribe()
		{
			base.Unsubscribe();
			if (GameManager.IsServer)
			{
				if (base.GameEntity && base.GameEntity.VitalsReplicator)
				{
					base.GameEntity.VitalsReplicator.CurrentStance.Changed -= this.CurrentStanceOnChanged;
					base.GameEntity.VitalsReplicator.CurrentHealthState.Changed -= this.CurrentHealthStateOnChanged;
				}
				this.m_hideHelm.Changed -= this.HideHelmOnChanged;
				this.m_blockInspections.Changed -= this.BlockInspectionsOnChanged;
				this.m_pauseAdventuringExperience.Changed -= this.PauseAdventuringExperienceOnChanged;
				this.m_hideEmberStone.Changed -= this.HideEmberStoneOnChanged;
				this.m_emberStoneId.Changed -= this.EmberStoneIdOnChanged;
			}
			if (!this.m_netEntity.IsLocal)
			{
				this.m_secondaryWeaponsActive.Changed -= this.HandActiveFlagsOnChanged;
				this.m_presenceFlags.Changed -= this.PresenceFlagsOnChanged;
				this.m_lfg.Changed -= this.LfgOnChanged;
			}
			else
			{
				ClientGameManager.SocialManager.LookingUpdated -= this.RefreshLocalLfg;
				this.AdventuringLevelSync.Changed -= this.AdventuringLevelChanged;
				this.PortraitId.Changed -= this.PortraitIdOnChanged;
				this.CharacterFlags.Changed -= this.CharacterFlagsOnChanged;
			}
			if (!GameManager.IsServer)
			{
				this.m_characterVisuals.Changed -= this.CharacterVisualsOnChanged;
				this.m_visibleEquipment.Changed -= this.VisibleEquipmentOnChanged;
				this.m_visibleEquipment.ReadComplete -= this.VisibleEquipmentOnReadComplete;
				this.m_baseRoleId.Changed -= this.RoleIdOnChanged;
				this.m_specializedRoleId.Changed += this.RoleIdOnChanged;
				this.m_emberStoneFillLevel.Changed -= this.EmberStoneFillLevelOnChanged;
			}
			if (this.m_knowledge != null && GameManager.QuestManager)
			{
				GameManager.QuestManager.KnowledgeUpdated -= this.OnKnowledgeUpdated;
			}
		}

		// Token: 0x06002A8D RID: 10893 RVA: 0x00143880 File Offset: 0x00141A80
		private void RefreshLocalLfg()
		{
			if (!GameManager.IsServer && this.m_netEntity.IsLocal)
			{
				bool value = this.m_lfg.Value;
				this.m_lfg.Value = (ClientGameManager.SocialManager != null && ClientGameManager.SocialManager.IsLookingForGroup());
				if (value != this.m_lfg.Value)
				{
					base.InvokeLfgChanged();
				}
			}
		}

		// Token: 0x06002A8E RID: 10894 RVA: 0x001438E4 File Offset: 0x00141AE4
		public override void UpdateHighestMasteryLevel(ArchetypeInstance instance)
		{
			int adventuringLevel = base.AdventuringLevel;
			base.UpdateHighestMasteryLevel(instance);
			this.RefreshLevelAndDifficultyIndicators(adventuringLevel);
		}

		// Token: 0x06002A8F RID: 10895 RVA: 0x00143908 File Offset: 0x00141B08
		public override void SetHighestMasteryLevels(Dictionary<MasteryType, ArchetypeInstance> instances)
		{
			int adventuringLevel = base.AdventuringLevel;
			base.SetHighestMasteryLevels(instances);
			this.RefreshLevelAndDifficultyIndicators(adventuringLevel);
		}

		// Token: 0x06002A90 RID: 10896 RVA: 0x0005D5D1 File Offset: 0x0005B7D1
		private void RefreshLevelAndDifficultyIndicators(int previousLevel)
		{
			if (!GameManager.IsServer && this.m_netEntity.IsLocal && previousLevel != base.AdventuringLevel)
			{
				this.LevelChanged();
			}
		}

		// Token: 0x06002A91 RID: 10897 RVA: 0x0005D5F6 File Offset: 0x0005B7F6
		private void AdventuringLevelChanged(byte currentLevel)
		{
			this.LevelChanged();
		}

		// Token: 0x06002A92 RID: 10898 RVA: 0x0005D5FE File Offset: 0x0005B7FE
		private void LevelChanged()
		{
			if (ClientGameManager.UIManager && ClientGameManager.UIManager.OffensiveNameplate)
			{
				ClientGameManager.UIManager.OffensiveNameplate.RefreshDifficultyIndicator();
			}
			LocalPlayer.InvokeHighestMasteryLevelChanged();
		}

		// Token: 0x06002A93 RID: 10899 RVA: 0x0014392C File Offset: 0x00141B2C
		public override void UpdateNameplateHeightBasedOnDna()
		{
			base.UpdateNameplateHeightBasedOnDna();
			if (base.GameEntity && this.m_characterVisuals != null && this.m_characterVisuals.Value != null)
			{
				base.GameEntity.NameplateHeightOffset = new Vector3?(UMAManager.GetNameplateHeightOffset(this.m_characterVisuals.Value.Dna));
			}
		}

		// Token: 0x06002A94 RID: 10900 RVA: 0x00143988 File Offset: 0x00141B88
		public override void UpdateAnimatorSpeedBasedOnDna()
		{
			base.UpdateAnimatorSpeedBasedOnDna();
			if (base.GameEntity && base.GameEntity.AnimatorReplicator != null && this.m_characterVisuals != null && this.m_characterVisuals.Value != null && this.m_characterVisuals.Value.Dna != null)
			{
				float sizeSmall;
				this.m_characterVisuals.Value.Dna.TryGetValue("sizeSmall", out sizeSmall);
				float sizeLarge;
				this.m_characterVisuals.Value.Dna.TryGetValue("sizeLarge", out sizeLarge);
				base.GameEntity.AnimatorReplicator.SetHumanoidSpeedBasedOnSizeValue(sizeSmall, sizeLarge);
			}
		}

		// Token: 0x06002A95 RID: 10901 RVA: 0x0005D631 File Offset: 0x0005B831
		public override void SetNearbyGroupInfo(NearbyGroupInfo info)
		{
			base.SetNearbyGroupInfo(info);
			this.m_nearbyGroupInfo.Value = info;
		}

		// Token: 0x06002A96 RID: 10902 RVA: 0x0005D646 File Offset: 0x0005B846
		private void EmberStoneFillLevelOnChanged(EmberStoneFillLevels obj)
		{
			base.InvokeEmberStoneFillLevelChanged(obj);
		}

		// Token: 0x06002A97 RID: 10903 RVA: 0x00143A34 File Offset: 0x00141C34
		private void EmberStoneIdOnChanged(UniqueId? obj)
		{
			if (obj == null || this.m_hideEmberStone.Value)
			{
				this.m_visibleEquipment.Remove(512);
				return;
			}
			EquipableItemVisualData value;
			EquipableItemVisualData equipableItemVisualData;
			if (this.m_visibleEquipment.TryGetValue(512, out value))
			{
				SynchronizedDictionary<Dictionary<int, EquipableItemVisualData>, int, EquipableItemVisualData> visibleEquipment = this.m_visibleEquipment;
				int key = 512;
				equipableItemVisualData = new EquipableItemVisualData
				{
					ArchetypeId = obj.Value,
					VisualIndex = null,
					ColorIndex = null
				};
				visibleEquipment[key] = equipableItemVisualData;
				return;
			}
			equipableItemVisualData = new EquipableItemVisualData
			{
				ArchetypeId = obj.Value,
				VisualIndex = null,
				ColorIndex = null
			};
			value = equipableItemVisualData;
			this.m_visibleEquipment.Add(512, value);
		}

		// Token: 0x06002A98 RID: 10904 RVA: 0x0005D64F File Offset: 0x0005B84F
		private void RoleIdOnChanged(UniqueId obj)
		{
			base.InvokeRoleChanged();
		}

		// Token: 0x06002A99 RID: 10905 RVA: 0x0005D657 File Offset: 0x0005B857
		private void PortraitIdOnChanged(UniqueId obj)
		{
			if (this.m_netEntity.IsLocal && this.m_record != null && this.m_record.Settings != null)
			{
				this.m_record.Settings.PortraitId = obj;
			}
		}

		// Token: 0x06002A9A RID: 10906 RVA: 0x00143B08 File Offset: 0x00141D08
		private void CharacterFlagsOnChanged(PlayerFlags obj)
		{
			if (this.m_netEntity && this.m_netEntity.IsLocal && Options.GameOptions.LeaveCombatAfterCombatFlagDrop.Value && this.m_previousPlayerFlags.HasBitFlag(PlayerFlags.InCombat) && !obj.HasBitFlag(PlayerFlags.InCombat) && LocalPlayer.Animancer && LocalPlayer.Animancer.Stance == Stance.Combat && LocalPlayer.GameEntity && LocalPlayer.GameEntity.SkillsController && !LocalPlayer.GameEntity.SkillsController.PendingIsActive)
			{
				LocalPlayer.Animancer.ToggleCombat();
			}
			this.m_previousPlayerFlags = obj;
		}

		// Token: 0x06002A9B RID: 10907 RVA: 0x0005D68C File Offset: 0x0005B88C
		private void HandActiveFlagsOnChanged(bool isActive)
		{
			if (GameManager.IsServer)
			{
				this.m_record.Settings.SecondaryWeaponsActive = isActive;
				base.InvokeHandConfigurationChanged();
				return;
			}
			if (!this.m_netEntity.IsLocal)
			{
				base.InvokeHandConfigurationChanged();
			}
		}

		// Token: 0x06002A9C RID: 10908 RVA: 0x0005D6C0 File Offset: 0x0005B8C0
		private void PresenceFlagsOnChanged(PresenceFlags obj)
		{
			if (GameManager.IsServer)
			{
				this.m_record.Settings.Presence = obj.ExplicitPresenceFromFlags();
				base.InvokePresenceChanged();
				return;
			}
			if (!this.m_netEntity.IsLocal)
			{
				base.InvokePresenceChanged();
			}
		}

		// Token: 0x06002A9D RID: 10909 RVA: 0x0005D6F9 File Offset: 0x0005B8F9
		private void LfgOnChanged(bool obj)
		{
			if (!GameManager.IsServer && !this.m_netEntity.IsLocal)
			{
				base.InvokeLfgChanged();
			}
		}

		// Token: 0x06002A9E RID: 10910 RVA: 0x00143BB0 File Offset: 0x00141DB0
		private void HideEmberStoneOnChanged(bool obj)
		{
			if (!this.m_netEntity.IsServer)
			{
				return;
			}
			this.m_record.Settings.HideEmberStone = obj;
			if (this.m_record.Settings.HideEmberStone || this.EmberStoneId == null)
			{
				this.m_visibleEquipment.Remove(512);
				return;
			}
			EquipableItemVisualData value = new EquipableItemVisualData
			{
				ArchetypeId = this.EmberStoneId.Value,
				VisualIndex = null,
				ColorIndex = null
			};
			if (this.m_visibleEquipment.ContainsKey(512))
			{
				this.m_visibleEquipment[512] = value;
				return;
			}
			this.m_visibleEquipment.Add(512, value);
		}

		// Token: 0x06002A9F RID: 10911 RVA: 0x00143C80 File Offset: 0x00141E80
		private void HideHelmOnChanged(bool obj)
		{
			if (!this.m_netEntity.IsServer)
			{
				return;
			}
			this.m_record.Settings.HideHelm = obj;
			ArchetypeInstance archetypeInstance;
			if (base.GameEntity.CollectionController != null && base.GameEntity.CollectionController.Equipment != null && base.GameEntity.CollectionController.Equipment.TryGetInstanceForIndex(32768, out archetypeInstance))
			{
				if (!this.m_record.Settings.HideHelm)
				{
					if (this.m_visibleEquipment.ContainsKey(archetypeInstance.Index))
					{
						this.m_visibleEquipment[archetypeInstance.Index] = new EquipableItemVisualData(archetypeInstance);
						return;
					}
					this.m_visibleEquipment.Add(archetypeInstance.Index, new EquipableItemVisualData(archetypeInstance));
					return;
				}
				else
				{
					this.m_visibleEquipment.Remove(archetypeInstance.Index);
				}
			}
		}

		// Token: 0x06002AA0 RID: 10912 RVA: 0x0005D715 File Offset: 0x0005B915
		private void BlockInspectionsOnChanged(bool obj)
		{
			if (this.m_netEntity.IsServer)
			{
				this.m_record.Settings.BlockInspections = obj;
			}
		}

		// Token: 0x06002AA1 RID: 10913 RVA: 0x0005D735 File Offset: 0x0005B935
		private void PauseAdventuringExperienceOnChanged(bool obj)
		{
			if (this.m_netEntity.IsServer)
			{
				this.m_record.Settings.PauseAdventuringExperience = obj;
			}
		}

		// Token: 0x06002AA2 RID: 10914 RVA: 0x00143D58 File Offset: 0x00141F58
		private void VisibleEquipmentOnReadComplete()
		{
			if (base.GameEntity && base.GameEntity.DCAController && base.GameEntity.DCAController.DCA)
			{
				base.GameEntity.DCAController.DCA.Refresh(true, true, true);
			}
		}

		// Token: 0x06002AA3 RID: 10915 RVA: 0x00143DB4 File Offset: 0x00141FB4
		private void VisibleEquipmentOnChanged(SynchronizedCollection<int, EquipableItemVisualData>.Operation op, int index, EquipableItemVisualData previous, EquipableItemVisualData current)
		{
			if (GameManager.IsServer || base.GameEntity.DCAController == null || base.GameEntity.DCAController.DCA == null)
			{
				return;
			}
			EquipableItem equipableItem = null;
			switch (op)
			{
			case SynchronizedCollection<int, EquipableItemVisualData>.Operation.Add:
			case SynchronizedCollection<int, EquipableItemVisualData>.Operation.Insert:
			case SynchronizedCollection<int, EquipableItemVisualData>.Operation.Set:
			case SynchronizedCollection<int, EquipableItemVisualData>.Operation.InitialAdd:
			case SynchronizedCollection<int, EquipableItemVisualData>.Operation.InitialAddFinal:
				if (InternalGameDatabase.Archetypes.TryGetAsType<EquipableItem>(current.ArchetypeId, out equipableItem))
				{
					bool flag = op == SynchronizedCollection<int, EquipableItemVisualData>.Operation.InitialAdd;
					bool flag2 = op == SynchronizedCollection<int, EquipableItemVisualData>.Operation.InitialAddFinal;
					bool flag3 = !flag && !flag2;
					if (index == 65536)
					{
						this.m_cosmeticItem = equipableItem;
						if (flag3)
						{
							this.EquipCosmeticItem(equipableItem, ref current);
						}
					}
					else if (!this.m_cosmeticItem || this.m_cosmeticItem.Type != equipableItem.Type)
					{
						equipableItem.OnEquipVisuals(this.m_characterVisuals.Value.Sex, base.GameEntity.DCAController.DCA, index, current.VisualIndex, current.ColorIndex, false);
					}
					EquipableItemVisualData equipableItemVisualData;
					if (op == SynchronizedCollection<int, EquipableItemVisualData>.Operation.InitialAddFinal && this.m_cosmeticItem && this.m_visibleEquipment.TryGetValue(65536, out equipableItemVisualData))
					{
						this.EquipCosmeticItem(this.m_cosmeticItem, ref equipableItemVisualData);
						return;
					}
				}
				break;
			case SynchronizedCollection<int, EquipableItemVisualData>.Operation.Clear:
				break;
			case SynchronizedCollection<int, EquipableItemVisualData>.Operation.RemoveAt:
				if (InternalGameDatabase.Archetypes.TryGetAsType<EquipableItem>(previous.ArchetypeId, out equipableItem))
				{
					if (index == 65536)
					{
						this.UnequipCosmeticItem(this.m_cosmeticItem, ref previous);
						this.m_cosmeticItem = null;
						return;
					}
					if (!this.m_cosmeticItem || this.m_cosmeticItem.Type != equipableItem.Type)
					{
						equipableItem.OnUnequipVisuals(this.m_characterVisuals.Value.Sex, base.GameEntity.DCAController.DCA, index, previous.VisualIndex, previous.ColorIndex, false);
					}
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x06002AA4 RID: 10916 RVA: 0x00143F7C File Offset: 0x0014217C
		private void EquipCosmeticItem(EquipableItem eqItem, ref EquipableItemVisualData visualData)
		{
			if (!eqItem)
			{
				return;
			}
			using (IEnumerator<EquipmentSlot> enumerator = eqItem.Type.GetCachedCompatibleSlots().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					int num = (int)enumerator.Current;
					EquipableItemVisualData equipableItemVisualData;
					EquipableItem equipableItem;
					if (this.m_visibleEquipment.TryGetValue(num, out equipableItemVisualData) && InternalGameDatabase.Archetypes.TryGetAsType<EquipableItem>(equipableItemVisualData.ArchetypeId, out equipableItem))
					{
						equipableItem.OnUnequipVisuals(this.m_characterVisuals.Value.Sex, base.GameEntity.DCAController.DCA, num, equipableItemVisualData.VisualIndex, equipableItemVisualData.ColorIndex, false);
					}
					eqItem.OnEquipVisuals(this.m_characterVisuals.Value.Sex, base.GameEntity.DCAController.DCA, num, visualData.VisualIndex, visualData.ColorIndex, false);
				}
			}
		}

		// Token: 0x06002AA5 RID: 10917 RVA: 0x00144064 File Offset: 0x00142264
		private void UnequipCosmeticItem(EquipableItem eqItem, ref EquipableItemVisualData visualData)
		{
			if (!eqItem)
			{
				return;
			}
			using (IEnumerator<EquipmentSlot> enumerator = eqItem.Type.GetCachedCompatibleSlots().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					int num = (int)enumerator.Current;
					eqItem.OnUnequipVisuals(this.m_characterVisuals.Value.Sex, base.GameEntity.DCAController.DCA, num, visualData.VisualIndex, visualData.ColorIndex, false);
					EquipableItemVisualData equipableItemVisualData;
					EquipableItem equipableItem;
					if (this.m_visibleEquipment.TryGetValue(num, out equipableItemVisualData) && InternalGameDatabase.Archetypes.TryGetAsType<EquipableItem>(equipableItemVisualData.ArchetypeId, out equipableItem))
					{
						equipableItem.OnEquipVisuals(this.m_characterVisuals.Value.Sex, base.GameEntity.DCAController.DCA, num, equipableItemVisualData.VisualIndex, equipableItemVisualData.ColorIndex, false);
					}
				}
			}
		}

		// Token: 0x06002AA6 RID: 10918 RVA: 0x0014414C File Offset: 0x0014234C
		private void CharacterVisualsOnChanged(CharacterVisuals obj)
		{
			if (base.GameEntity.DCAController != null && base.GameEntity.DCAController.DCA != null)
			{
				UMAManager.BuildBaseDca(base.GameEntity, base.GameEntity.DCAController.DCA, this.m_characterVisuals.Value, null, null);
			}
		}

		// Token: 0x06002AA7 RID: 10919 RVA: 0x001441AC File Offset: 0x001423AC
		private void ServerAutoAfk()
		{
			if (GameManager.IsServer && base.GameEntity && base.GameEntity.NetworkEntity && this.PresenceFlags.HasBitFlag(PresenceFlags.AwayAutomatic))
			{
				if (this.m_firstAfkTime == null)
				{
					this.m_firstAfkTime = new float?(Time.time);
					return;
				}
				if (Time.time - this.m_firstAfkTime.Value >= 1200f && ServerNetworkEntityManager.DisconnectNetworkEntity(base.GameEntity.NetworkEntity))
				{
					Debug.Log("AFK Booting " + this.Name.Value + "!");
					return;
				}
			}
			else
			{
				this.m_firstAfkTime = null;
			}
		}

		// Token: 0x06002AA8 RID: 10920 RVA: 0x0005D755 File Offset: 0x0005B955
		public override void EnterRoadCollider(RoadCollider roadCollider)
		{
			base.EnterRoadCollider(roadCollider);
			if (GameManager.IsServer && roadCollider)
			{
				if (this.m_roadColliders == null)
				{
					this.m_roadColliders = new HashSet<RoadCollider>(10);
				}
				this.m_roadColliders.Add(roadCollider);
				this.RefreshOnRoadFlag();
			}
		}

		// Token: 0x06002AA9 RID: 10921 RVA: 0x0005D795 File Offset: 0x0005B995
		public override void ExitRoadCollider(RoadCollider roadCollider)
		{
			base.ExitRoadCollider(roadCollider);
			if (GameManager.IsServer && roadCollider && this.m_roadColliders != null)
			{
				this.m_roadColliders.Remove(roadCollider);
				this.RefreshOnRoadFlag();
			}
		}

		// Token: 0x06002AAA RID: 10922 RVA: 0x0005D7C8 File Offset: 0x0005B9C8
		private void CurrentStanceOnChanged(Stance obj)
		{
			this.RefreshOnRoadFlag();
		}

		// Token: 0x06002AAB RID: 10923 RVA: 0x00144268 File Offset: 0x00142468
		private bool IsOnRoad()
		{
			return this.m_roadColliders != null && this.m_roadColliders.Count > 0 && base.GameEntity && base.GameEntity.VitalsReplicator && base.GameEntity.VitalsReplicator.CurrentStance.Value.AllowOnRoad();
		}

		// Token: 0x06002AAC RID: 10924 RVA: 0x001442C8 File Offset: 0x001424C8
		private void RefreshOnRoadFlag()
		{
			if (!GameManager.IsServer)
			{
				return;
			}
			PlayerFlags playerFlags = this.CharacterFlags.Value;
			if (this.IsOnRoad())
			{
				playerFlags |= PlayerFlags.OnRoad;
			}
			else
			{
				playerFlags &= ~PlayerFlags.OnRoad;
			}
			this.CharacterFlags.Value = playerFlags;
		}

		// Token: 0x06002AAD RID: 10925 RVA: 0x0005D7D0 File Offset: 0x0005B9D0
		public override void EnterPvpCollider(PvpCollider pvpCollider)
		{
			base.EnterPvpCollider(pvpCollider);
			if (GameManager.IsServer && pvpCollider)
			{
				if (this.m_pvpVolumes == null)
				{
					this.m_pvpVolumes = new HashSet<PvpCollider>(10);
				}
				this.m_pvpVolumes.Add(pvpCollider);
				this.RefreshPvpFlag();
			}
		}

		// Token: 0x06002AAE RID: 10926 RVA: 0x0005D810 File Offset: 0x0005BA10
		public override void ExitPvpCollider(PvpCollider pvpCollider)
		{
			base.ExitPvpCollider(pvpCollider);
			if (GameManager.IsServer && pvpCollider && this.m_pvpVolumes != null)
			{
				this.m_pvpVolumes.Remove(pvpCollider);
				this.RefreshPvpFlag();
			}
		}

		// Token: 0x06002AAF RID: 10927 RVA: 0x0005D843 File Offset: 0x0005BA43
		private void CurrentHealthStateOnChanged(HealthState obj)
		{
			this.RefreshPvpFlag();
		}

		// Token: 0x06002AB0 RID: 10928 RVA: 0x00144310 File Offset: 0x00142510
		private bool IsPvp()
		{
			return this.m_pvpVolumes != null && this.m_pvpVolumes.Count > 0 && base.GameEntity && base.GameEntity.VitalsReplicator && base.GameEntity.VitalsReplicator.CurrentHealthState.Value == HealthState.Alive;
		}

		// Token: 0x06002AB1 RID: 10929 RVA: 0x0014436C File Offset: 0x0014256C
		private void RefreshPvpFlag()
		{
			if (!GameManager.IsServer)
			{
				return;
			}
			PlayerFlags playerFlags = this.CharacterFlags.Value;
			if (this.IsPvp())
			{
				playerFlags |= PlayerFlags.Pvp;
			}
			else
			{
				playerFlags &= ~PlayerFlags.Pvp;
			}
			this.CharacterFlags.Value = playerFlags;
		}

		// Token: 0x06002AB2 RID: 10930 RVA: 0x001443B4 File Offset: 0x001425B4
		private void InitKnowledge()
		{
			if (this.m_knowledge != null)
			{
				return;
			}
			this.m_knowledge = new Dictionary<string, bool>();
			if (GameManager.QuestManager)
			{
				GameManager.QuestManager.KnowledgeUpdated += this.OnKnowledgeUpdated;
			}
			if (base.GameEntity && base.GameEntity.CollectionController != null && base.GameEntity.CollectionController.Record != null && base.GameEntity.CollectionController.Record.Progression != null && base.GameEntity.CollectionController.Record.Progression.NpcKnowledge != null)
			{
				foreach (KeyValuePair<UniqueId, BitArray> keyValuePair in base.GameEntity.CollectionController.Record.Progression.NpcKnowledge)
				{
					NpcProfile npcProfile;
					if (InternalGameDatabase.Archetypes.TryGetAsType<NpcProfile>(keyValuePair.Key, out npcProfile))
					{
						for (int i = 0; i < npcProfile.KnowledgeLabels.Length; i++)
						{
							this.m_knowledge.AddOrReplace(npcProfile.KnowledgeLabels[i], keyValuePair.Value.Length > i && keyValuePair.Value[i]);
						}
					}
				}
			}
		}

		// Token: 0x06002AB3 RID: 10931 RVA: 0x00144514 File Offset: 0x00142714
		private void OnKnowledgeUpdated(NpcProfile profile, int knowledgeIndex)
		{
			if (profile && profile.KnowledgeLabels != null && knowledgeIndex < profile.KnowledgeLabels.Length && this.m_knowledge != null)
			{
				string key = profile.KnowledgeLabels[knowledgeIndex];
				this.m_knowledge.AddOrReplace(key, true);
			}
		}

		// Token: 0x06002AB4 RID: 10932 RVA: 0x0014455C File Offset: 0x0014275C
		public override bool Knows(string label)
		{
			this.InitKnowledge();
			bool flag;
			return this.m_knowledge != null && this.m_knowledge.TryGetValue(label, out flag) && flag;
		}

		// Token: 0x06002AB5 RID: 10933 RVA: 0x0005D84B File Offset: 0x0005BA4B
		public override void ResetLabel(string label)
		{
			if (this.m_knowledge != null && this.m_knowledge.ContainsKey(label))
			{
				this.m_knowledge.Remove(label);
			}
		}

		// Token: 0x06002AB6 RID: 10934 RVA: 0x0005D870 File Offset: 0x0005BA70
		public override void ReinitKnowledge()
		{
			this.m_knowledge = null;
			this.InitKnowledge();
		}

		// Token: 0x06002AB7 RID: 10935 RVA: 0x0014458C File Offset: 0x0014278C
		public override IEnumerable<string> ListKnowledge()
		{
			this.InitKnowledge();
			if (this.m_knowledge == null)
			{
				return Array.Empty<string>();
			}
			return this.m_knowledge.Keys;
		}

		// Token: 0x06002AB8 RID: 10936 RVA: 0x001445BC File Offset: 0x001427BC
		protected override int RegisterSyncs()
		{
			int num = base.RegisterSyncs();
			this.m_syncs.Add(this.m_baseRoleId);
			this.m_baseRoleId.BitFlag = 1 << num;
			num++;
			this.m_syncs.Add(this.m_blockInspections);
			this.m_blockInspections.BitFlag = 1 << num;
			num++;
			this.m_syncs.Add(this.m_characterId);
			this.m_characterId.BitFlag = 1 << num;
			num++;
			this.m_syncs.Add(this.m_emberStoneFillLevel);
			this.m_emberStoneFillLevel.BitFlag = 1 << num;
			num++;
			this.m_syncs.Add(this.m_emberStoneId);
			this.m_emberStoneId.BitFlag = 1 << num;
			num++;
			this.m_syncs.Add(this.m_groupId);
			this.m_groupId.BitFlag = 1 << num;
			num++;
			this.m_syncs.Add(this.m_hideEmberStone);
			this.m_hideEmberStone.BitFlag = 1 << num;
			num++;
			this.m_syncs.Add(this.m_hideHelm);
			this.m_hideHelm.BitFlag = 1 << num;
			num++;
			this.m_syncs.Add(this.m_isSubscriber);
			this.m_isSubscriber.BitFlag = 1 << num;
			num++;
			this.m_syncs.Add(this.m_isTrial);
			this.m_isTrial.BitFlag = 1 << num;
			num++;
			this.m_syncs.Add(this.m_lfg);
			this.m_lfg.BitFlag = 1 << num;
			num++;
			this.m_syncs.Add(this.m_nearbyGroupInfo);
			this.m_nearbyGroupInfo.BitFlag = 1 << num;
			num++;
			this.m_syncs.Add(this.m_pauseAdventuringExperience);
			this.m_pauseAdventuringExperience.BitFlag = 1 << num;
			num++;
			this.m_syncs.Add(this.m_presenceFlags);
			this.m_presenceFlags.BitFlag = 1 << num;
			num++;
			this.m_syncs.Add(this.m_raidId);
			this.m_raidId.BitFlag = 1 << num;
			num++;
			this.m_syncs.Add(this.m_secondaryWeaponsActive);
			this.m_secondaryWeaponsActive.BitFlag = 1 << num;
			num++;
			this.m_syncs.Add(this.m_specializedRoleId);
			this.m_specializedRoleId.BitFlag = 1 << num;
			return num + 1;
		}

		// Token: 0x04002AF5 RID: 10997
		private readonly SynchronizedNullableUniqueId m_emberStoneId = new SynchronizedNullableUniqueId();

		// Token: 0x04002AF6 RID: 10998
		private readonly SynchronizedEnum<EmberStoneFillLevels> m_emberStoneFillLevel = new SynchronizedEnum<EmberStoneFillLevels>();

		// Token: 0x04002AF7 RID: 10999
		private readonly SynchronizedBool m_hideEmberStone = new SynchronizedBool();

		// Token: 0x04002AF8 RID: 11000
		private readonly SynchronizedBool m_hideHelm = new SynchronizedBool();

		// Token: 0x04002AF9 RID: 11001
		private readonly SynchronizedBool m_secondaryWeaponsActive = new SynchronizedBool();

		// Token: 0x04002AFA RID: 11002
		private readonly SynchronizedBool m_blockInspections = new SynchronizedBool();

		// Token: 0x04002AFB RID: 11003
		private readonly SynchronizedBool m_pauseAdventuringExperience = new SynchronizedBool();

		// Token: 0x04002AFC RID: 11004
		private readonly SynchronizedUniqueId m_characterId = new SynchronizedUniqueId();

		// Token: 0x04002AFD RID: 11005
		private readonly SynchronizedUniqueId m_groupId = new SynchronizedUniqueId();

		// Token: 0x04002AFE RID: 11006
		private readonly SynchronizedUniqueId m_raidId = new SynchronizedUniqueId();

		// Token: 0x04002AFF RID: 11007
		private readonly SynchronizedStruct<NearbyGroupInfo> m_nearbyGroupInfo = new SynchronizedStruct<NearbyGroupInfo>();

		// Token: 0x04002B00 RID: 11008
		private readonly SynchronizedEnum<PresenceFlags> m_presenceFlags = new SynchronizedEnum<PresenceFlags>(PresenceFlags.Online);

		// Token: 0x04002B01 RID: 11009
		private readonly SynchronizedBool m_lfg = new SynchronizedBool(false);

		// Token: 0x04002B02 RID: 11010
		private readonly SynchronizedUniqueId m_baseRoleId = new SynchronizedUniqueId();

		// Token: 0x04002B03 RID: 11011
		private readonly SynchronizedUniqueId m_specializedRoleId = new SynchronizedUniqueId();

		// Token: 0x04002B04 RID: 11012
		private readonly SynchronizedBool m_isSubscriber = new SynchronizedBool();

		// Token: 0x04002B05 RID: 11013
		private readonly SynchronizedBool m_isTrial = new SynchronizedBool();

		// Token: 0x04002B06 RID: 11014
		private PlayerFlags m_previousPlayerFlags;

		// Token: 0x04002B07 RID: 11015
		private NpcTagSet m_npcTagSet;

		// Token: 0x04002B08 RID: 11016
		private List<GameEntity> m_nearbyGroupMembers;

		// Token: 0x04002B09 RID: 11017
		private List<GameEntity> m_nearbyRaidMembers;

		// Token: 0x04002B0A RID: 11018
		private EquipableItem m_cosmeticItem;

		// Token: 0x04002B0B RID: 11019
		private float? m_firstAfkTime;

		// Token: 0x04002B0C RID: 11020
		private HashSet<RoadCollider> m_roadColliders;

		// Token: 0x04002B0D RID: 11021
		private HashSet<PvpCollider> m_pvpVolumes;

		// Token: 0x04002B0E RID: 11022
		private Dictionary<string, bool> m_knowledge;
	}
}

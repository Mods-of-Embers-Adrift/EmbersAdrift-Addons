using System;
using System.Collections.Generic;
using SoL.Game.Animation;
using SoL.Game.NPCs;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Quests.Objectives;
using SoL.Game.Spawning;
using SoL.Managers;
using SoL.Networking.Database;
using SoL.Networking.Replication;
using SoL.Utilities;
using SoL.Utilities.Extensions;

namespace SoL.Game
{
	// Token: 0x02000562 RID: 1378
	public abstract class CharacterData : SyncVarReplicator
	{
		// Token: 0x14000085 RID: 133
		// (add) Token: 0x060029B3 RID: 10675 RVA: 0x001421E8 File Offset: 0x001403E8
		// (remove) Token: 0x060029B4 RID: 10676 RVA: 0x00142220 File Offset: 0x00140420
		public event Action HandConfigurationChanged;

		// Token: 0x14000086 RID: 134
		// (add) Token: 0x060029B5 RID: 10677 RVA: 0x00142258 File Offset: 0x00140458
		// (remove) Token: 0x060029B6 RID: 10678 RVA: 0x00142290 File Offset: 0x00140490
		public event Action MasteryConfigurationChanged;

		// Token: 0x14000087 RID: 135
		// (add) Token: 0x060029B7 RID: 10679 RVA: 0x001422C8 File Offset: 0x001404C8
		// (remove) Token: 0x060029B8 RID: 10680 RVA: 0x00142300 File Offset: 0x00140500
		public event Action HighestMasteryLevelChanged;

		// Token: 0x14000088 RID: 136
		// (add) Token: 0x060029B9 RID: 10681 RVA: 0x00142338 File Offset: 0x00140538
		// (remove) Token: 0x060029BA RID: 10682 RVA: 0x00142370 File Offset: 0x00140570
		public event Action AnyMasteryLevelChanged;

		// Token: 0x14000089 RID: 137
		// (add) Token: 0x060029BB RID: 10683 RVA: 0x001423A8 File Offset: 0x001405A8
		// (remove) Token: 0x060029BC RID: 10684 RVA: 0x001423E0 File Offset: 0x001405E0
		public event Action RoleChanged;

		// Token: 0x1400008A RID: 138
		// (add) Token: 0x060029BD RID: 10685 RVA: 0x00142418 File Offset: 0x00140618
		// (remove) Token: 0x060029BE RID: 10686 RVA: 0x00142450 File Offset: 0x00140650
		public event Action PresenceChanged;

		// Token: 0x1400008B RID: 139
		// (add) Token: 0x060029BF RID: 10687 RVA: 0x00142488 File Offset: 0x00140688
		// (remove) Token: 0x060029C0 RID: 10688 RVA: 0x001424C0 File Offset: 0x001406C0
		public event Action LfgChanged;

		// Token: 0x1400008C RID: 140
		// (add) Token: 0x060029C1 RID: 10689 RVA: 0x001424F8 File Offset: 0x001406F8
		// (remove) Token: 0x060029C2 RID: 10690 RVA: 0x00142530 File Offset: 0x00140730
		public event Action<EmberStoneFillLevels> EmberStoneFillLevelChanged;

		// Token: 0x1400008D RID: 141
		// (add) Token: 0x060029C3 RID: 10691 RVA: 0x00142568 File Offset: 0x00140768
		// (remove) Token: 0x060029C4 RID: 10692 RVA: 0x001425A0 File Offset: 0x001407A0
		public event Action PauseAdventuringExperienceChanged;

		// Token: 0x1700088E RID: 2190
		// (get) Token: 0x060029C5 RID: 10693 RVA: 0x0005CD25 File Offset: 0x0005AF25
		// (set) Token: 0x060029C6 RID: 10694 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual CharacterSex Sex
		{
			get
			{
				if (this.m_characterVisuals.Value != null)
				{
					return this.m_characterVisuals.Value.Sex;
				}
				return CharacterSex.Male;
			}
			set
			{
			}
		}

		// Token: 0x1700088F RID: 2191
		// (get) Token: 0x060029C7 RID: 10695 RVA: 0x0005CD46 File Offset: 0x0005AF46
		// (set) Token: 0x060029C8 RID: 10696 RVA: 0x0005CD4E File Offset: 0x0005AF4E
		public int PortraitIndex { get; set; }

		// Token: 0x17000890 RID: 2192
		// (get) Token: 0x060029C9 RID: 10697 RVA: 0x0005CD57 File Offset: 0x0005AF57
		public SynchronizedDictIntKey<EquipableItemVisualData> VisibleEquipment
		{
			get
			{
				return this.m_visibleEquipment;
			}
		}

		// Token: 0x17000891 RID: 2193
		// (get) Token: 0x060029CA RID: 10698 RVA: 0x0005CD5F File Offset: 0x0005AF5F
		// (set) Token: 0x060029CB RID: 10699 RVA: 0x0005CD67 File Offset: 0x0005AF67
		public virtual bool HideEmberStone { get; set; }

		// Token: 0x17000892 RID: 2194
		// (get) Token: 0x060029CC RID: 10700 RVA: 0x0005CD70 File Offset: 0x0005AF70
		// (set) Token: 0x060029CD RID: 10701 RVA: 0x0005CD78 File Offset: 0x0005AF78
		public virtual bool HideHelm { get; set; }

		// Token: 0x17000893 RID: 2195
		// (get) Token: 0x060029CE RID: 10702 RVA: 0x0005CD81 File Offset: 0x0005AF81
		// (set) Token: 0x060029CF RID: 10703 RVA: 0x0005CD89 File Offset: 0x0005AF89
		public virtual bool BlockInspections { get; set; }

		// Token: 0x17000894 RID: 2196
		// (get) Token: 0x060029D0 RID: 10704 RVA: 0x0005CD92 File Offset: 0x0005AF92
		// (set) Token: 0x060029D1 RID: 10705 RVA: 0x0005CD9A File Offset: 0x0005AF9A
		public virtual bool PauseAdventuringExperience { get; set; }

		// Token: 0x17000895 RID: 2197
		// (get) Token: 0x060029D2 RID: 10706 RVA: 0x0005CDA3 File Offset: 0x0005AFA3
		// (set) Token: 0x060029D3 RID: 10707 RVA: 0x0005CDAB File Offset: 0x0005AFAB
		public virtual UniqueId CharacterId { get; set; }

		// Token: 0x17000896 RID: 2198
		// (get) Token: 0x060029D4 RID: 10708 RVA: 0x0005CDB4 File Offset: 0x0005AFB4
		// (set) Token: 0x060029D5 RID: 10709 RVA: 0x0005CDBC File Offset: 0x0005AFBC
		public virtual UniqueId GroupId { get; set; }

		// Token: 0x17000897 RID: 2199
		// (get) Token: 0x060029D6 RID: 10710 RVA: 0x0005CDC5 File Offset: 0x0005AFC5
		// (set) Token: 0x060029D7 RID: 10711 RVA: 0x0005CDCD File Offset: 0x0005AFCD
		public virtual UniqueId RaidId { get; set; }

		// Token: 0x17000898 RID: 2200
		// (get) Token: 0x060029D8 RID: 10712 RVA: 0x0005CDD6 File Offset: 0x0005AFD6
		public virtual int GroupMembersNearby { get; }

		// Token: 0x17000899 RID: 2201
		// (get) Token: 0x060029D9 RID: 10713 RVA: 0x0005CDDE File Offset: 0x0005AFDE
		public virtual int GroupedLevel { get; }

		// Token: 0x1700089A RID: 2202
		// (get) Token: 0x060029DA RID: 10714 RVA: 0x0005CDE6 File Offset: 0x0005AFE6
		// (set) Token: 0x060029DB RID: 10715 RVA: 0x0005CDEE File Offset: 0x0005AFEE
		public virtual List<GameEntity> NearbyGroupMembers { get; set; }

		// Token: 0x1700089B RID: 2203
		// (get) Token: 0x060029DC RID: 10716 RVA: 0x0005CDF7 File Offset: 0x0005AFF7
		// (set) Token: 0x060029DD RID: 10717 RVA: 0x0005CDFF File Offset: 0x0005AFFF
		public virtual List<GameEntity> NearbyRaidMembers { get; set; }

		// Token: 0x1700089C RID: 2204
		// (get) Token: 0x060029DE RID: 10718 RVA: 0x0005CE08 File Offset: 0x0005B008
		// (set) Token: 0x060029DF RID: 10719 RVA: 0x0005CE10 File Offset: 0x0005B010
		public virtual UniqueId BaseRoleId { get; set; }

		// Token: 0x1700089D RID: 2205
		// (get) Token: 0x060029E0 RID: 10720 RVA: 0x0005CE19 File Offset: 0x0005B019
		// (set) Token: 0x060029E1 RID: 10721 RVA: 0x0005CE21 File Offset: 0x0005B021
		public virtual UniqueId SpecializedRoleId { get; set; }

		// Token: 0x1700089E RID: 2206
		// (get) Token: 0x060029E2 RID: 10722 RVA: 0x0005CE2A File Offset: 0x0005B02A
		// (set) Token: 0x060029E3 RID: 10723 RVA: 0x0005CE32 File Offset: 0x0005B032
		public virtual bool MainHand_SecondaryActive { get; set; }

		// Token: 0x1700089F RID: 2207
		// (get) Token: 0x060029E4 RID: 10724 RVA: 0x0005CE3B File Offset: 0x0005B03B
		public virtual bool OffHand_SecondaryActive { get; }

		// Token: 0x170008A0 RID: 2208
		// (get) Token: 0x060029E5 RID: 10725 RVA: 0x0005CE43 File Offset: 0x0005B043
		// (set) Token: 0x060029E6 RID: 10726 RVA: 0x0005CE4B File Offset: 0x0005B04B
		public virtual Faction Faction { get; set; }

		// Token: 0x170008A1 RID: 2209
		// (get) Token: 0x060029E7 RID: 10727 RVA: 0x0005CE54 File Offset: 0x0005B054
		// (set) Token: 0x060029E8 RID: 10728 RVA: 0x0005CE5C File Offset: 0x0005B05C
		public virtual float? TransformScale { get; set; }

		// Token: 0x170008A2 RID: 2210
		// (get) Token: 0x060029E9 RID: 10729 RVA: 0x0005CE65 File Offset: 0x0005B065
		// (set) Token: 0x060029EA RID: 10730 RVA: 0x0005CE6D File Offset: 0x0005B06D
		public virtual NpcInitData NpcInitData { get; set; }

		// Token: 0x170008A3 RID: 2211
		// (get) Token: 0x060029EB RID: 10731 RVA: 0x0005CE76 File Offset: 0x0005B076
		// (set) Token: 0x060029EC RID: 10732 RVA: 0x0005CE7E File Offset: 0x0005B07E
		public virtual Presence Presence { get; set; }

		// Token: 0x170008A4 RID: 2212
		// (get) Token: 0x060029ED RID: 10733 RVA: 0x0005CE87 File Offset: 0x0005B087
		// (set) Token: 0x060029EE RID: 10734 RVA: 0x0005CE8F File Offset: 0x0005B08F
		public virtual PresenceFlags PresenceFlags { get; set; }

		// Token: 0x170008A5 RID: 2213
		// (get) Token: 0x060029EF RID: 10735 RVA: 0x0005CE98 File Offset: 0x0005B098
		// (set) Token: 0x060029F0 RID: 10736 RVA: 0x0005CEA0 File Offset: 0x0005B0A0
		public virtual NpcTagSet NpcTagsSet { get; set; }

		// Token: 0x170008A6 RID: 2214
		// (get) Token: 0x060029F1 RID: 10737 RVA: 0x0005CEA9 File Offset: 0x0005B0A9
		public virtual bool IsLfg { get; }

		// Token: 0x170008A7 RID: 2215
		// (get) Token: 0x060029F2 RID: 10738 RVA: 0x0005CEB1 File Offset: 0x0005B0B1
		// (set) Token: 0x060029F3 RID: 10739 RVA: 0x0005CEB9 File Offset: 0x0005B0B9
		public virtual bool IsSwimming { get; set; }

		// Token: 0x170008A8 RID: 2216
		// (get) Token: 0x060029F4 RID: 10740 RVA: 0x0005CEC2 File Offset: 0x0005B0C2
		// (set) Token: 0x060029F5 RID: 10741 RVA: 0x0005CECA File Offset: 0x0005B0CA
		public virtual UniqueId? EmberStoneId { get; set; }

		// Token: 0x170008A9 RID: 2217
		// (get) Token: 0x060029F6 RID: 10742 RVA: 0x0005CED3 File Offset: 0x0005B0D3
		// (set) Token: 0x060029F7 RID: 10743 RVA: 0x0005CEDB File Offset: 0x0005B0DB
		public virtual EmberStoneFillLevels EmberStoneFillLevel { get; set; }

		// Token: 0x170008AA RID: 2218
		// (get) Token: 0x060029F8 RID: 10744 RVA: 0x0005CEE4 File Offset: 0x0005B0E4
		public virtual bool IsSubscriber { get; }

		// Token: 0x170008AB RID: 2219
		// (get) Token: 0x060029F9 RID: 10745 RVA: 0x0005CEEC File Offset: 0x0005B0EC
		public virtual bool IsTrial { get; }

		// Token: 0x170008AC RID: 2220
		// (get) Token: 0x060029FA RID: 10746 RVA: 0x0005CEF4 File Offset: 0x0005B0F4
		public virtual bool IsGM { get; }

		// Token: 0x060029FB RID: 10747 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void SetNearbyGroupInfo(NearbyGroupInfo info)
		{
		}

		// Token: 0x060029FC RID: 10748 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void SetSecondaryWeaponsActive(bool active)
		{
		}

		// Token: 0x060029FD RID: 10749 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void RefreshRole()
		{
		}

		// Token: 0x170008AD RID: 2221
		// (get) Token: 0x060029FE RID: 10750 RVA: 0x0005CEFC File Offset: 0x0005B0FC
		public UniqueId ActiveMasteryId
		{
			get
			{
				return this.BaseRoleId;
			}
		}

		// Token: 0x170008AE RID: 2222
		// (get) Token: 0x060029FF RID: 10751 RVA: 0x0005CF04 File Offset: 0x0005B104
		// (set) Token: 0x06002A00 RID: 10752 RVA: 0x0005CF0C File Offset: 0x0005B10C
		public virtual ChallengeRating ChallengeRating { get; set; }

		// Token: 0x170008AF RID: 2223
		// (get) Token: 0x06002A01 RID: 10753 RVA: 0x0005CF15 File Offset: 0x0005B115
		// (set) Token: 0x06002A02 RID: 10754 RVA: 0x0005CF22 File Offset: 0x0005B122
		public int AdventuringLevel
		{
			get
			{
				return (int)this.AdventuringLevelSync.Value;
			}
			protected set
			{
				if (GameManager.IsServer)
				{
					this.AdventuringLevelSync.Value = (byte)value;
				}
			}
		}

		// Token: 0x06002A03 RID: 10755 RVA: 0x001425D8 File Offset: 0x001407D8
		public int GetGroupedLevel()
		{
			if (this.GroupedLevel <= this.AdventuringLevel || this.GroupId.IsEmpty)
			{
				return this.AdventuringLevel;
			}
			return this.GroupedLevel;
		}

		// Token: 0x170008B0 RID: 2224
		// (get) Token: 0x06002A04 RID: 10756 RVA: 0x0005CF38 File Offset: 0x0005B138
		// (set) Token: 0x06002A05 RID: 10757 RVA: 0x0005CF40 File Offset: 0x0005B140
		public int GatheringLevel { get; private set; } = 1;

		// Token: 0x170008B1 RID: 2225
		// (get) Token: 0x06002A06 RID: 10758 RVA: 0x0005CF49 File Offset: 0x0005B149
		// (set) Token: 0x06002A07 RID: 10759 RVA: 0x0005CF51 File Offset: 0x0005B151
		public int CraftingLevel { get; private set; } = 1;

		// Token: 0x170008B2 RID: 2226
		// (get) Token: 0x06002A08 RID: 10760 RVA: 0x0005CF5A File Offset: 0x0005B15A
		// (set) Token: 0x06002A09 RID: 10761 RVA: 0x0005CF62 File Offset: 0x0005B162
		public virtual int ResourceLevel { get; set; }

		// Token: 0x170008B3 RID: 2227
		// (get) Token: 0x06002A0A RID: 10762 RVA: 0x0005CF6B File Offset: 0x0005B16B
		// (set) Token: 0x06002A0B RID: 10763 RVA: 0x0005CF73 File Offset: 0x0005B173
		public HumanoidReferencePoints? ReferencePoints { get; set; }

		// Token: 0x170008B4 RID: 2228
		// (get) Token: 0x06002A0C RID: 10764 RVA: 0x0005CF7C File Offset: 0x0005B17C
		// (set) Token: 0x06002A0D RID: 10765 RVA: 0x0005CF84 File Offset: 0x0005B184
		public bool MatchAttackerLevel { get; set; }

		// Token: 0x170008B5 RID: 2229
		// (get) Token: 0x06002A0E RID: 10766 RVA: 0x0005CF8D File Offset: 0x0005B18D
		// (set) Token: 0x06002A0F RID: 10767 RVA: 0x0005CF95 File Offset: 0x0005B195
		public ObjectiveOrdersCollection ObjectiveOrders { get; set; } = new ObjectiveOrdersCollection();

		// Token: 0x06002A10 RID: 10768 RVA: 0x0005CF9E File Offset: 0x0005B19E
		protected virtual void Awake()
		{
			base.GameEntity.CharacterData = this;
		}

		// Token: 0x06002A11 RID: 10769 RVA: 0x0005CFAC File Offset: 0x0005B1AC
		protected override void OnDestroy()
		{
			this.Unsubscribe();
			base.OnDestroy();
		}

		// Token: 0x06002A12 RID: 10770 RVA: 0x0005CFBA File Offset: 0x0005B1BA
		protected override void PostInit()
		{
			this.Subscribe();
		}

		// Token: 0x06002A13 RID: 10771 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void InitNpcLevel(int level)
		{
		}

		// Token: 0x06002A14 RID: 10772 RVA: 0x0005CFC2 File Offset: 0x0005B1C2
		public virtual void InitializeFromRecord(CharacterRecord record)
		{
			this.m_record = record;
		}

		// Token: 0x06002A15 RID: 10773 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void InitPresenceFlags()
		{
		}

		// Token: 0x06002A16 RID: 10774 RVA: 0x0005CFCB File Offset: 0x0005B1CB
		protected virtual void Subscribe()
		{
			if (!GameManager.IsServer)
			{
				this.CurrentStanceData.Changed += this.CurrentStanceDataOnChanged;
			}
		}

		// Token: 0x06002A17 RID: 10775 RVA: 0x0005CFEB File Offset: 0x0005B1EB
		protected virtual void Unsubscribe()
		{
			if (!GameManager.IsServer)
			{
				this.CurrentStanceData.Changed -= this.CurrentStanceDataOnChanged;
			}
		}

		// Token: 0x06002A18 RID: 10776 RVA: 0x0005D00B File Offset: 0x0005B20B
		protected void InvokeHandConfigurationChanged()
		{
			Action handConfigurationChanged = this.HandConfigurationChanged;
			if (handConfigurationChanged == null)
			{
				return;
			}
			handConfigurationChanged();
		}

		// Token: 0x06002A19 RID: 10777 RVA: 0x0005D01D File Offset: 0x0005B21D
		protected void InvokeMasteryConfigurationChanged()
		{
			Action masteryConfigurationChanged = this.MasteryConfigurationChanged;
			if (masteryConfigurationChanged == null)
			{
				return;
			}
			masteryConfigurationChanged();
		}

		// Token: 0x06002A1A RID: 10778 RVA: 0x0005D02F File Offset: 0x0005B22F
		protected void InvokeRoleChanged()
		{
			Action roleChanged = this.RoleChanged;
			if (roleChanged == null)
			{
				return;
			}
			roleChanged();
		}

		// Token: 0x06002A1B RID: 10779 RVA: 0x0005D041 File Offset: 0x0005B241
		protected void InvokePresenceChanged()
		{
			Action presenceChanged = this.PresenceChanged;
			if (presenceChanged == null)
			{
				return;
			}
			presenceChanged();
		}

		// Token: 0x06002A1C RID: 10780 RVA: 0x0005D053 File Offset: 0x0005B253
		protected void InvokeLfgChanged()
		{
			Action lfgChanged = this.LfgChanged;
			if (lfgChanged == null)
			{
				return;
			}
			lfgChanged();
		}

		// Token: 0x06002A1D RID: 10781 RVA: 0x0005D065 File Offset: 0x0005B265
		protected void InvokeEmberStoneFillLevelChanged(EmberStoneFillLevels level)
		{
			Action<EmberStoneFillLevels> emberStoneFillLevelChanged = this.EmberStoneFillLevelChanged;
			if (emberStoneFillLevelChanged == null)
			{
				return;
			}
			emberStoneFillLevelChanged(level);
		}

		// Token: 0x06002A1E RID: 10782 RVA: 0x0005D078 File Offset: 0x0005B278
		protected void InvokePauseAdventuringExperienceChanged()
		{
			Action pauseAdventuringExperienceChanged = this.PauseAdventuringExperienceChanged;
			if (pauseAdventuringExperienceChanged == null)
			{
				return;
			}
			pauseAdventuringExperienceChanged();
		}

		// Token: 0x06002A1F RID: 10783 RVA: 0x0005D08A File Offset: 0x0005B28A
		private void CurrentStanceIdOnChanged(UniqueId obj)
		{
			IAnimationController animancerController = base.GameEntity.AnimancerController;
			if (animancerController == null)
			{
				return;
			}
			animancerController.SetCurrentStanceId(obj, false);
		}

		// Token: 0x06002A20 RID: 10784 RVA: 0x0005D0A3 File Offset: 0x0005B2A3
		private void CurrentStanceDataOnChanged(StanceData obj)
		{
			IAnimationController animancerController = base.GameEntity.AnimancerController;
			if (animancerController == null)
			{
				return;
			}
			animancerController.SetCurrentStanceId(obj.StanceId, obj.BypassTransition);
		}

		// Token: 0x06002A21 RID: 10785 RVA: 0x00142610 File Offset: 0x00140810
		public void UpdateHighestMasteryLevel(InstanceNewLevelData newInstanceLevelData)
		{
			ArchetypeInstance instance;
			if (newInstanceLevelData.MasteryId != null && base.GameEntity && base.GameEntity.CollectionController != null && base.GameEntity.CollectionController.Masteries != null && base.GameEntity.CollectionController.Masteries.TryGetInstanceForArchetypeId(newInstanceLevelData.MasteryId.Value, out instance))
			{
				this.UpdateHighestMasteryLevel(instance);
			}
		}

		// Token: 0x06002A22 RID: 10786 RVA: 0x00142684 File Offset: 0x00140884
		public virtual void UpdateHighestMasteryLevel(ArchetypeInstance instance)
		{
			bool flag = false;
			MasteryArchetype masteryArchetype;
			if (instance != null && instance.Archetype && instance.Archetype.TryGetAsType(out masteryArchetype))
			{
				int associatedLevelInteger = instance.GetAssociatedLevelInteger(base.GameEntity);
				switch (masteryArchetype.Type)
				{
				case MasteryType.Combat:
					flag = (this.AdventuringLevel != associatedLevelInteger);
					this.AdventuringLevel = associatedLevelInteger;
					break;
				case MasteryType.Trade:
					if (associatedLevelInteger > this.CraftingLevel)
					{
						this.CraftingLevel = associatedLevelInteger;
						flag = true;
					}
					break;
				case MasteryType.Harvesting:
					if (associatedLevelInteger > this.GatheringLevel)
					{
						this.GatheringLevel = associatedLevelInteger;
						flag = true;
					}
					break;
				}
			}
			if (flag)
			{
				Action highestMasteryLevelChanged = this.HighestMasteryLevelChanged;
				if (highestMasteryLevelChanged != null)
				{
					highestMasteryLevelChanged();
				}
			}
			Action anyMasteryLevelChanged = this.AnyMasteryLevelChanged;
			if (anyMasteryLevelChanged == null)
			{
				return;
			}
			anyMasteryLevelChanged();
		}

		// Token: 0x06002A23 RID: 10787 RVA: 0x00142748 File Offset: 0x00140948
		public virtual void SetHighestMasteryLevels(Dictionary<MasteryType, ArchetypeInstance> instances)
		{
			if (instances == null || instances.Count <= 0)
			{
				return;
			}
			bool flag = false;
			foreach (KeyValuePair<MasteryType, ArchetypeInstance> keyValuePair in instances)
			{
				if (keyValuePair.Value != null)
				{
					int associatedLevelInteger = keyValuePair.Value.GetAssociatedLevelInteger(base.GameEntity);
					switch (keyValuePair.Key)
					{
					case MasteryType.Combat:
						flag = (associatedLevelInteger != this.AdventuringLevel);
						this.AdventuringLevel = associatedLevelInteger;
						break;
					case MasteryType.Trade:
						if (associatedLevelInteger > this.CraftingLevel)
						{
							this.CraftingLevel = associatedLevelInteger;
							flag = true;
						}
						break;
					case MasteryType.Harvesting:
						if (associatedLevelInteger > this.GatheringLevel)
						{
							this.GatheringLevel = associatedLevelInteger;
							flag = true;
						}
						break;
					}
				}
			}
			if (flag)
			{
				Action highestMasteryLevelChanged = this.HighestMasteryLevelChanged;
				if (highestMasteryLevelChanged != null)
				{
					highestMasteryLevelChanged();
				}
			}
			Action anyMasteryLevelChanged = this.AnyMasteryLevelChanged;
			if (anyMasteryLevelChanged == null)
			{
				return;
			}
			anyMasteryLevelChanged();
		}

		// Token: 0x06002A24 RID: 10788 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void UpdateNameplateHeightBasedOnDna()
		{
		}

		// Token: 0x06002A25 RID: 10789 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void UpdateAnimatorSpeedBasedOnDna()
		{
		}

		// Token: 0x06002A26 RID: 10790 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void EnterRoadCollider(RoadCollider roadCollider)
		{
		}

		// Token: 0x06002A27 RID: 10791 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void ExitRoadCollider(RoadCollider roadCollider)
		{
		}

		// Token: 0x06002A28 RID: 10792 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void EnterPvpCollider(PvpCollider pvpCollider)
		{
		}

		// Token: 0x06002A29 RID: 10793 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void ExitPvpCollider(PvpCollider pvpCollider)
		{
		}

		// Token: 0x06002A2A RID: 10794 RVA: 0x00045BCA File Offset: 0x00043DCA
		public virtual bool Knows(string label)
		{
			return false;
		}

		// Token: 0x06002A2B RID: 10795 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void ResetLabel(string label)
		{
		}

		// Token: 0x06002A2C RID: 10796 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void ReinitKnowledge()
		{
		}

		// Token: 0x06002A2D RID: 10797 RVA: 0x0005D0C6 File Offset: 0x0005B2C6
		public virtual IEnumerable<string> ListKnowledge()
		{
			return Array.Empty<string>();
		}

		// Token: 0x06002A2E RID: 10798 RVA: 0x00142848 File Offset: 0x00140A48
		public bool TryGetCharacterVisualData(out CharacterData.CharacterSizeData data)
		{
			data = default(CharacterData.CharacterSizeData);
			if (this.m_characterVisuals == null || this.m_characterVisuals.Value == null)
			{
				return false;
			}
			data.Sex = this.m_characterVisuals.Value.Sex;
			data.BuildType = this.m_characterVisuals.Value.BuildType;
			data.Size = 0.5f;
			if (this.m_characterVisuals.Value.Dna != null)
			{
				float left = 0f;
				float num;
				if (this.m_characterVisuals.Value.Dna.TryGetValue("sizeSmall", out num))
				{
					left = num;
				}
				float right = 0f;
				float num2;
				if (this.m_characterVisuals.Value.Dna.TryGetValue("sizeLarge", out num2))
				{
					right = num2;
				}
				data.Size = UMAManager.GetValueForLeftRight(left, right);
			}
			return true;
		}

		// Token: 0x06002A2F RID: 10799 RVA: 0x00142918 File Offset: 0x00140B18
		protected override int RegisterSyncs()
		{
			int num = base.RegisterSyncs();
			this.m_syncs.Add(this.AdventuringLevelSync);
			this.AdventuringLevelSync.BitFlag = 1 << num;
			num++;
			this.m_syncs.Add(this.CharacterFlags);
			this.CharacterFlags.BitFlag = 1 << num;
			num++;
			this.m_syncs.Add(this.CurrentCombatId);
			this.CurrentCombatId.BitFlag = 1 << num;
			num++;
			this.m_syncs.Add(this.CurrentStanceData);
			this.CurrentStanceData.BitFlag = 1 << num;
			num++;
			this.m_syncs.Add(this.GuildName);
			this.GuildName.BitFlag = 1 << num;
			num++;
			this.m_syncs.Add(this.ItemsAttached);
			this.ItemsAttached.BitFlag = 1 << num;
			num++;
			this.m_syncs.Add(this.m_characterVisuals);
			this.m_characterVisuals.BitFlag = 1 << num;
			num++;
			this.m_syncs.Add(this.m_visibleEquipment);
			this.m_visibleEquipment.BitFlag = 1 << num;
			num++;
			this.m_syncs.Add(this.Name);
			this.Name.BitFlag = 1 << num;
			num++;
			this.m_syncs.Add(this.PortraitId);
			this.PortraitId.BitFlag = 1 << num;
			num++;
			this.m_syncs.Add(this.Title);
			this.Title.BitFlag = 1 << num;
			return num + 1;
		}

		// Token: 0x04002AAB RID: 10923
		public const string kShowAsGMPlayerPref = "gm_enabled_";

		// Token: 0x04002AB5 RID: 10933
		protected CharacterRecord m_record;

		// Token: 0x04002AB6 RID: 10934
		protected readonly SynchronizedCharacterVisuals m_characterVisuals = new SynchronizedCharacterVisuals();

		// Token: 0x04002AB7 RID: 10935
		protected readonly SynchronizedDictIntKey<EquipableItemVisualData> m_visibleEquipment = new SynchronizedDictIntKey<EquipableItemVisualData>();

		// Token: 0x04002AB8 RID: 10936
		public readonly SynchronizedStruct<StanceData> CurrentStanceData = new SynchronizedStruct<StanceData>();

		// Token: 0x04002AB9 RID: 10937
		public readonly SynchronizedUniqueId CurrentCombatId = new SynchronizedUniqueId();

		// Token: 0x04002ABA RID: 10938
		public readonly SynchronizedEnum<PlayerFlags> CharacterFlags = new SynchronizedEnum<PlayerFlags>();

		// Token: 0x04002ABB RID: 10939
		public readonly SynchronizedEnum<ItemsAttached> ItemsAttached = new SynchronizedEnum<ItemsAttached>();

		// Token: 0x04002ABC RID: 10940
		public readonly SynchronizedString Name = new SynchronizedString();

		// Token: 0x04002ABD RID: 10941
		public readonly SynchronizedString Title = new SynchronizedString();

		// Token: 0x04002ABE RID: 10942
		public readonly SynchronizedString GuildName = new SynchronizedString();

		// Token: 0x04002ABF RID: 10943
		public readonly SynchronizedUniqueId PortraitId = new SynchronizedUniqueId();

		// Token: 0x04002AC0 RID: 10944
		public readonly SynchronizedByte AdventuringLevelSync = new SynchronizedByte(1);

		// Token: 0x02000563 RID: 1379
		public struct CharacterSizeData
		{
			// Token: 0x04002AE5 RID: 10981
			public CharacterSex Sex;

			// Token: 0x04002AE6 RID: 10982
			public CharacterBuildType BuildType;

			// Token: 0x04002AE7 RID: 10983
			public float Size;
		}
	}
}

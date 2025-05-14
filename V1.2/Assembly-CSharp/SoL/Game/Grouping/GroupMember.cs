using System;
using Cysharp.Text;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Settings;
using SoL.Managers;
using SoL.Networking.Database;
using SoL.Networking.Objects;
using SoL.Networking.SolServer;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Grouping
{
	// Token: 0x02000BDE RID: 3038
	public class GroupMember : IPoolable
	{
		// Token: 0x14000121 RID: 289
		// (add) Token: 0x06005DEF RID: 24047 RVA: 0x001F58DC File Offset: 0x001F3ADC
		// (remove) Token: 0x06005DF0 RID: 24048 RVA: 0x001F5914 File Offset: 0x001F3B14
		public event Action MemberUpdated;

		// Token: 0x1700162C RID: 5676
		// (get) Token: 0x06005DF1 RID: 24049 RVA: 0x0007F2F9 File Offset: 0x0007D4F9
		// (set) Token: 0x06005DF2 RID: 24050 RVA: 0x0007F301 File Offset: 0x0007D501
		public GameEntity Entity
		{
			get
			{
				return this.m_entity;
			}
			set
			{
				if (value == this.m_entity)
				{
					return;
				}
				this.m_entity = value;
				Action memberUpdated = this.MemberUpdated;
				if (memberUpdated == null)
				{
					return;
				}
				memberUpdated();
			}
		}

		// Token: 0x1700162D RID: 5677
		// (get) Token: 0x06005DF3 RID: 24051 RVA: 0x0007F329 File Offset: 0x0007D529
		// (set) Token: 0x06005DF4 RID: 24052 RVA: 0x001F594C File Offset: 0x001F3B4C
		public int ZoneId
		{
			get
			{
				return this.m_zoneId;
			}
			private set
			{
				if (this.m_zoneId == value)
				{
					return;
				}
				this.m_zoneId = value;
				this.m_status = ((this.m_zoneId <= 0) ? GroupMemberStatus.Offline : GroupMemberStatus.Online);
				if (LocalZoneManager.ZoneRecord != null && LocalZoneManager.ZoneRecord.ZoneId == this.m_zoneId)
				{
					NetworkEntity networkEntity;
					if (this.m_status == GroupMemberStatus.Online && this.Entity == null && !string.IsNullOrEmpty(this.m_name) && ClientGameManager.GroupManager.TryGetEntityForName(this.m_name, out networkEntity))
					{
						this.Entity = networkEntity.GameEntity;
						return;
					}
				}
				else
				{
					this.Entity = null;
				}
			}
		}

		// Token: 0x1700162E RID: 5678
		// (get) Token: 0x06005DF5 RID: 24053 RVA: 0x0007F331 File Offset: 0x0007D531
		// (set) Token: 0x06005DF6 RID: 24054 RVA: 0x0007F339 File Offset: 0x0007D539
		public byte SubZoneId
		{
			get
			{
				return this.m_subZoneId;
			}
			private set
			{
				this.m_subZoneId = value;
			}
		}

		// Token: 0x1700162F RID: 5679
		// (get) Token: 0x06005DF7 RID: 24055 RVA: 0x0007F342 File Offset: 0x0007D542
		// (set) Token: 0x06005DF8 RID: 24056 RVA: 0x0007F34A File Offset: 0x0007D54A
		public byte EmberRingIndex
		{
			get
			{
				return this.m_emberRingIndex;
			}
			private set
			{
				this.m_emberRingIndex = value;
			}
		}

		// Token: 0x17001630 RID: 5680
		// (get) Token: 0x06005DF9 RID: 24057 RVA: 0x0007F353 File Offset: 0x0007D553
		public bool InDifferentZone
		{
			get
			{
				return !this.InSameZone;
			}
		}

		// Token: 0x17001631 RID: 5681
		// (get) Token: 0x06005DFA RID: 24058 RVA: 0x0007F35E File Offset: 0x0007D55E
		public bool InSameZone
		{
			get
			{
				return this.m_status == GroupMemberStatus.Online && LocalZoneManager.ZoneRecord != null && LocalZoneManager.ZoneRecord.ZoneId == this.m_zoneId;
			}
		}

		// Token: 0x17001632 RID: 5682
		// (get) Token: 0x06005DFB RID: 24059 RVA: 0x0007F384 File Offset: 0x0007D584
		public GroupMemberStatus Status
		{
			get
			{
				return this.m_status;
			}
		}

		// Token: 0x17001633 RID: 5683
		// (get) Token: 0x06005DFC RID: 24060 RVA: 0x0007F38C File Offset: 0x0007D58C
		public UniqueId CharacterId
		{
			get
			{
				return this.m_characterId;
			}
		}

		// Token: 0x17001634 RID: 5684
		// (get) Token: 0x06005DFD RID: 24061 RVA: 0x0007F394 File Offset: 0x0007D594
		// (set) Token: 0x06005DFE RID: 24062 RVA: 0x001F59E4 File Offset: 0x001F3BE4
		public string Name
		{
			get
			{
				return this.m_name;
			}
			set
			{
				this.m_name = value;
				if (LocalZoneManager.ZoneRecord != null && LocalZoneManager.ZoneRecord.ZoneId == this.m_zoneId)
				{
					NetworkEntity networkEntity;
					if (this.m_status == GroupMemberStatus.Online && this.Entity == null && !string.IsNullOrEmpty(this.m_name) && ClientGameManager.GroupManager.TryGetEntityForName(this.m_name, out networkEntity))
					{
						this.Entity = networkEntity.GameEntity;
					}
				}
				else
				{
					this.Entity = null;
				}
				Action memberUpdated = this.MemberUpdated;
				if (memberUpdated == null)
				{
					return;
				}
				memberUpdated();
			}
		}

		// Token: 0x17001635 RID: 5685
		// (get) Token: 0x06005DFF RID: 24063 RVA: 0x0007F39C File Offset: 0x0007D59C
		public bool IsSelf
		{
			get
			{
				return SessionData.SelectedCharacter != null && this.m_name == SessionData.SelectedCharacter.Name;
			}
		}

		// Token: 0x17001636 RID: 5686
		// (get) Token: 0x06005E00 RID: 24064 RVA: 0x0007F3BC File Offset: 0x0007D5BC
		// (set) Token: 0x06005E01 RID: 24065 RVA: 0x0007F3C4 File Offset: 0x0007D5C4
		public bool MarkedForRemoval { get; set; }

		// Token: 0x06005E02 RID: 24066 RVA: 0x001F5A70 File Offset: 0x001F3C70
		public string GetStatusString()
		{
			switch (this.m_status)
			{
			case GroupMemberStatus.Offline:
			case GroupMemberStatus.Zoning:
				return this.m_status.ToString();
			case GroupMemberStatus.Online:
			{
				ZoneRecord zoneRecord = SessionData.GetZoneRecord((ZoneId)this.m_zoneId);
				if (zoneRecord == null)
				{
					return "Unknown";
				}
				return LocalZoneManager.GetFormattedZoneName(zoneRecord.DisplayName, (SubZoneId)this.m_subZoneId);
			}
			default:
				return "Unknown";
			}
		}

		// Token: 0x06005E03 RID: 24067 RVA: 0x0007F3CD File Offset: 0x0007D5CD
		public string GetLevelString()
		{
			if (this.m_status != GroupMemberStatus.Online || this.m_latestGroupMemberZoneStatus.Level <= 0)
			{
				return string.Empty;
			}
			return this.m_latestGroupMemberZoneStatus.Level.ToString();
		}

		// Token: 0x06005E04 RID: 24068 RVA: 0x001F5AD8 File Offset: 0x001F3CD8
		public Sprite GetRoleIcon(out Color color)
		{
			Sprite result = null;
			color = Color.white;
			if (this.m_status == GroupMemberStatus.Online && this.m_latestGroupMemberZoneStatus.Role > 0 && GlobalSettings.Values != null && GlobalSettings.Values.Roles != null)
			{
				RolePacked role = (RolePacked)this.m_latestGroupMemberZoneStatus.Role;
				BaseArchetype roleFromPacked = GlobalSettings.Values.Roles.GetRoleFromPacked(role);
				if (roleFromPacked)
				{
					result = roleFromPacked.Icon;
					color = roleFromPacked.IconTint;
				}
			}
			return result;
		}

		// Token: 0x06005E05 RID: 24069 RVA: 0x001F5B5C File Offset: 0x001F3D5C
		public string GetRoleLevelTooltipText()
		{
			if (this.m_status == GroupMemberStatus.Online && this.m_latestGroupMemberZoneStatus.Level > 0 && this.m_latestGroupMemberZoneStatus.Role > 0)
			{
				RolePacked role = (RolePacked)this.m_latestGroupMemberZoneStatus.Role;
				return ZString.Format<byte, RolePacked>("{0} {1}", this.m_latestGroupMemberZoneStatus.Level, role);
			}
			return string.Empty;
		}

		// Token: 0x06005E06 RID: 24070 RVA: 0x0007F3FC File Offset: 0x0007D5FC
		public GroupMember()
		{
			this.m_characterId = UniqueId.Empty;
		}

		// Token: 0x06005E07 RID: 24071 RVA: 0x001F5BB8 File Offset: 0x001F3DB8
		public GroupMember(GroupMemberZoneStatus status)
		{
			this.m_latestGroupMemberZoneStatus = status;
			this.m_characterId = new UniqueId(status.CharacterId);
			this.m_name = status.CharacterName;
			this.MarkedForRemoval = false;
			bool flag = this.ZoneId != status.ZoneId || this.SubZoneId != status.SubZoneId;
			this.ZoneId = status.ZoneId;
			this.SubZoneId = status.SubZoneId;
			if (flag)
			{
				Action memberUpdated = this.MemberUpdated;
				if (memberUpdated == null)
				{
					return;
				}
				memberUpdated();
			}
		}

		// Token: 0x06005E08 RID: 24072 RVA: 0x001F5C4C File Offset: 0x001F3E4C
		public void UpdateStatus(GroupMemberZoneStatus status)
		{
			this.m_latestGroupMemberZoneStatus = status;
			this.MarkedForRemoval = false;
			bool flag = this.ZoneId != status.ZoneId || this.SubZoneId != status.SubZoneId;
			this.ZoneId = status.ZoneId;
			this.SubZoneId = status.SubZoneId;
			if (flag)
			{
				Action memberUpdated = this.MemberUpdated;
				if (memberUpdated == null)
				{
					return;
				}
				memberUpdated();
			}
		}

		// Token: 0x06005E09 RID: 24073 RVA: 0x001F5CB4 File Offset: 0x001F3EB4
		public void UpdateStatus(PlayerStatus status)
		{
			if (status != null)
			{
				bool flag = this.m_latestGroupMemberZoneStatus.Role != status.Role || this.m_latestGroupMemberZoneStatus.Level != status.Level;
				this.m_latestGroupMemberZoneStatus.UpdateValues(status);
				bool flag2 = this.m_zoneId != status.ZoneId;
				bool flag3 = this.m_subZoneId != status.SubZoneId;
				bool flag4 = this.m_emberRingIndex != status.EmberRingIndex;
				this.ZoneId = status.ZoneId;
				this.SubZoneId = status.SubZoneId;
				this.EmberRingIndex = status.EmberRingIndex;
				if (flag || flag2 || flag3 || flag4)
				{
					Action memberUpdated = this.MemberUpdated;
					if (memberUpdated == null)
					{
						return;
					}
					memberUpdated();
				}
			}
		}

		// Token: 0x17001637 RID: 5687
		// (get) Token: 0x06005E0A RID: 24074 RVA: 0x0007F417 File Offset: 0x0007D617
		// (set) Token: 0x06005E0B RID: 24075 RVA: 0x0007F41F File Offset: 0x0007D61F
		bool IPoolable.InPool { get; set; }

		// Token: 0x06005E0C RID: 24076 RVA: 0x0007F428 File Offset: 0x0007D628
		void IPoolable.Reset()
		{
			this.m_name = null;
			this.m_entity = null;
			this.m_status = GroupMemberStatus.Offline;
			this.m_zoneId = -100;
			this.m_subZoneId = 0;
			this.m_latestGroupMemberZoneStatus = default(GroupMemberZoneStatus);
		}

		// Token: 0x04005135 RID: 20789
		private readonly UniqueId m_characterId;

		// Token: 0x04005136 RID: 20790
		private string m_name;

		// Token: 0x04005137 RID: 20791
		private GameEntity m_entity;

		// Token: 0x04005138 RID: 20792
		private GroupMemberStatus m_status;

		// Token: 0x04005139 RID: 20793
		private int m_zoneId = -100;

		// Token: 0x0400513A RID: 20794
		private byte m_subZoneId;

		// Token: 0x0400513B RID: 20795
		private byte m_emberRingIndex;

		// Token: 0x0400513C RID: 20796
		private GroupMemberZoneStatus m_latestGroupMemberZoneStatus;
	}
}

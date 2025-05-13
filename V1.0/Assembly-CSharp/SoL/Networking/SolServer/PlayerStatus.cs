using System;
using SoL.Game;
using SoL.Game.Objects.Archetypes;
using SoL.Networking.Database;

namespace SoL.Networking.SolServer
{
	// Token: 0x020003E8 RID: 1000
	public class PlayerStatus
	{
		// Token: 0x17000641 RID: 1601
		// (get) Token: 0x06001A98 RID: 6808 RVA: 0x00054A2E File Offset: 0x00052C2E
		// (set) Token: 0x06001A99 RID: 6809 RVA: 0x00054A36 File Offset: 0x00052C36
		public string Character { get; set; }

		// Token: 0x17000642 RID: 1602
		// (get) Token: 0x06001A9A RID: 6810 RVA: 0x00054A3F File Offset: 0x00052C3F
		// (set) Token: 0x06001A9B RID: 6811 RVA: 0x00054A47 File Offset: 0x00052C47
		public int Presence { get; set; }

		// Token: 0x17000643 RID: 1603
		// (get) Token: 0x06001A9C RID: 6812 RVA: 0x00054A50 File Offset: 0x00052C50
		// (set) Token: 0x06001A9D RID: 6813 RVA: 0x00054A58 File Offset: 0x00052C58
		public int Access { get; set; }

		// Token: 0x17000644 RID: 1604
		// (get) Token: 0x06001A9E RID: 6814 RVA: 0x00054A61 File Offset: 0x00052C61
		// (set) Token: 0x06001A9F RID: 6815 RVA: 0x00054A69 File Offset: 0x00052C69
		public int ZoneId { get; set; }

		// Token: 0x17000645 RID: 1605
		// (get) Token: 0x06001AA0 RID: 6816 RVA: 0x00054A72 File Offset: 0x00052C72
		// (set) Token: 0x06001AA1 RID: 6817 RVA: 0x00054A7A File Offset: 0x00052C7A
		public byte SubZoneId { get; set; }

		// Token: 0x17000646 RID: 1606
		// (get) Token: 0x06001AA2 RID: 6818 RVA: 0x00054A83 File Offset: 0x00052C83
		// (set) Token: 0x06001AA3 RID: 6819 RVA: 0x00054A8B File Offset: 0x00052C8B
		public byte Role { get; set; }

		// Token: 0x17000647 RID: 1607
		// (get) Token: 0x06001AA4 RID: 6820 RVA: 0x00054A94 File Offset: 0x00052C94
		// (set) Token: 0x06001AA5 RID: 6821 RVA: 0x00054A9C File Offset: 0x00052C9C
		public byte Level { get; set; }

		// Token: 0x17000648 RID: 1608
		// (get) Token: 0x06001AA6 RID: 6822 RVA: 0x00054AA5 File Offset: 0x00052CA5
		// (set) Token: 0x06001AA7 RID: 6823 RVA: 0x00054AAD File Offset: 0x00052CAD
		public byte EmberRingIndex { get; set; }

		// Token: 0x17000649 RID: 1609
		// (get) Token: 0x06001AA8 RID: 6824 RVA: 0x00054AB6 File Offset: 0x00052CB6
		// (set) Token: 0x06001AA9 RID: 6825 RVA: 0x00054ABE File Offset: 0x00052CBE
		public DateTime? LastOnline { get; set; }

		// Token: 0x1700064A RID: 1610
		// (get) Token: 0x06001AAA RID: 6826 RVA: 0x00054AC7 File Offset: 0x00052CC7
		public PresenceFlags PresenceFlags
		{
			get
			{
				return (PresenceFlags)this.Presence;
			}
		}

		// Token: 0x1700064B RID: 1611
		// (get) Token: 0x06001AAB RID: 6827 RVA: 0x00054AD0 File Offset: 0x00052CD0
		public AccessFlags AccessFlags
		{
			get
			{
				return (AccessFlags)this.Access;
			}
		}

		// Token: 0x1700064C RID: 1612
		// (get) Token: 0x06001AAC RID: 6828 RVA: 0x00054AD8 File Offset: 0x00052CD8
		public ZoneId ZoneIdEnum
		{
			get
			{
				return (ZoneId)this.ZoneId;
			}
		}

		// Token: 0x1700064D RID: 1613
		// (get) Token: 0x06001AAD RID: 6829 RVA: 0x00054AE0 File Offset: 0x00052CE0
		public SubZoneId SubZoneIdEnum
		{
			get
			{
				return (SubZoneId)this.SubZoneId;
			}
		}

		// Token: 0x1700064E RID: 1614
		// (get) Token: 0x06001AAE RID: 6830 RVA: 0x00054AE8 File Offset: 0x00052CE8
		public RolePacked RolePacked
		{
			get
			{
				return (RolePacked)this.Role;
			}
		}
	}
}

using System;
using SoL.Networking.SolServer;

namespace SoL.Game.Grouping
{
	// Token: 0x02000BDD RID: 3037
	[Serializable]
	public struct GroupMemberZoneStatus
	{
		// Token: 0x06005DEE RID: 24046 RVA: 0x0007F2BB File Offset: 0x0007D4BB
		public void UpdateValues(PlayerStatus status)
		{
			this.ZoneId = status.ZoneId;
			this.SubZoneId = status.SubZoneId;
			this.Role = status.Role;
			this.Level = status.Level;
			this.EmberRingIndex = status.EmberRingIndex;
		}

		// Token: 0x0400512D RID: 20781
		public string CharacterId;

		// Token: 0x0400512E RID: 20782
		public string CharacterName;

		// Token: 0x0400512F RID: 20783
		public int ZoneId;

		// Token: 0x04005130 RID: 20784
		public byte SubZoneId;

		// Token: 0x04005131 RID: 20785
		public byte Role;

		// Token: 0x04005132 RID: 20786
		public byte Level;

		// Token: 0x04005133 RID: 20787
		public byte EmberRingIndex;
	}
}

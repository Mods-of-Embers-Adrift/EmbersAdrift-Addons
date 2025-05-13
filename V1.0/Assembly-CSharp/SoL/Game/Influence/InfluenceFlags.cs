using System;

namespace SoL.Game.Influence
{
	// Token: 0x02000BC0 RID: 3008
	[Flags]
	public enum InfluenceFlags
	{
		// Token: 0x04005082 RID: 20610
		None = 0,
		// Token: 0x04005083 RID: 20611
		PlayerLocation = 1,
		// Token: 0x04005084 RID: 20612
		NpcLocation = 2,
		// Token: 0x04005085 RID: 20613
		PlayerThreat = 4,
		// Token: 0x04005086 RID: 20614
		NpcThreat = 8,
		// Token: 0x04005087 RID: 20615
		Resources = 16,
		// Token: 0x04005088 RID: 20616
		Fire = 32,
		// Token: 0x04005089 RID: 20617
		DungeonEntrance = 64,
		// Token: 0x0400508A RID: 20618
		All = -1
	}
}

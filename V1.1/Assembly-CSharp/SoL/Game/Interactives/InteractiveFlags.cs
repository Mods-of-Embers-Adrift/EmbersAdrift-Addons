using System;

namespace SoL.Game.Interactives
{
	// Token: 0x02000B8B RID: 2955
	[Flags]
	public enum InteractiveFlags : byte
	{
		// Token: 0x04004FAF RID: 20399
		None = 0,
		// Token: 0x04004FB0 RID: 20400
		Interactive = 1,
		// Token: 0x04004FB1 RID: 20401
		RecordGenerated = 2,
		// Token: 0x04004FB2 RID: 20402
		Destroy = 4,
		// Token: 0x04004FB3 RID: 20403
		NpcContributed = 8,
		// Token: 0x04004FB4 RID: 20404
		ResourceNode = 16,
		// Token: 0x04004FB5 RID: 20405
		Dialogue = 32,
		// Token: 0x04004FB6 RID: 20406
		LootRollPending = 64
	}
}

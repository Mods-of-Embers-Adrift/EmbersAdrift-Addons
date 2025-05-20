using System;

namespace SoL.Networking
{
	// Token: 0x020003D1 RID: 977
	public enum OpCodes : ushort
	{
		// Token: 0x04002130 RID: 8496
		None,
		// Token: 0x04002131 RID: 8497
		ConnectionEvent,
		// Token: 0x04002132 RID: 8498
		Ok,
		// Token: 0x04002133 RID: 8499
		Spawn,
		// Token: 0x04002134 RID: 8500
		BulkSpawn,
		// Token: 0x04002135 RID: 8501
		Destroy,
		// Token: 0x04002136 RID: 8502
		StateUpdate,
		// Token: 0x04002137 RID: 8503
		SyncUpdate,
		// Token: 0x04002138 RID: 8504
		ClientSyncUpdate,
		// Token: 0x04002139 RID: 8505
		RPC,
		// Token: 0x0400213A RID: 8506
		Error,
		// Token: 0x0400213B RID: 8507
		ChatMessage,
		// Token: 0x0400213C RID: 8508
		LootRoll,
		// Token: 0x0400213D RID: 8509
		ServerTime,
		// Token: 0x0400213E RID: 8510
		DuelRoll,
		// Token: 0x0400213F RID: 8511
		LocationEvent,
		// Token: 0x04002140 RID: 8512
		AuctionHouse
	}
}

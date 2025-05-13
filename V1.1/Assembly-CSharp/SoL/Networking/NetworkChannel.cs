using System;

namespace SoL.Networking
{
	// Token: 0x020003BF RID: 959
	public enum NetworkChannel : byte
	{
		// Token: 0x040020F1 RID: 8433
		Invalid,
		// Token: 0x040020F2 RID: 8434
		State_Client,
		// Token: 0x040020F3 RID: 8435
		State_Server,
		// Token: 0x040020F4 RID: 8436
		SyncVar_Client,
		// Token: 0x040020F5 RID: 8437
		SyncVar_Server,
		// Token: 0x040020F6 RID: 8438
		AnimatorState_Client,
		// Token: 0x040020F7 RID: 8439
		AnimatorState_Server,
		// Token: 0x040020F8 RID: 8440
		AnimatorSync_Client,
		// Token: 0x040020F9 RID: 8441
		AnimatorSync_Server,
		// Token: 0x040020FA RID: 8442
		Spawn_Self,
		// Token: 0x040020FB RID: 8443
		Spawn_Other,
		// Token: 0x040020FC RID: 8444
		Destroy_Client,
		// Token: 0x040020FD RID: 8445
		Destroy_Server,
		// Token: 0x040020FE RID: 8446
		Rpc_Client,
		// Token: 0x040020FF RID: 8447
		Rpc_Server,
		// Token: 0x04002100 RID: 8448
		CombatResults,
		// Token: 0x04002101 RID: 8449
		LootMessage,
		// Token: 0x04002102 RID: 8450
		UtilityResult,
		// Token: 0x04002103 RID: 8451
		LootRoll,
		// Token: 0x04002104 RID: 8452
		GenericChatMessage,
		// Token: 0x04002105 RID: 8453
		AuctionHouse
	}
}

using System;

namespace SoL.Networking
{
	// Token: 0x020003C2 RID: 962
	public enum CommandType
	{
		// Token: 0x04002107 RID: 8455
		None,
		// Token: 0x04002108 RID: 8456
		StartHost,
		// Token: 0x04002109 RID: 8457
		StopHost,
		// Token: 0x0400210A RID: 8458
		DisconnectClient,
		// Token: 0x0400210B RID: 8459
		Send,
		// Token: 0x0400210C RID: 8460
		BroadcastAll,
		// Token: 0x0400210D RID: 8461
		BroadcastOthers,
		// Token: 0x0400210E RID: 8462
		BroadcastGroup
	}
}

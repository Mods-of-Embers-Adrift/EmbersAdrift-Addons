using System;

namespace SoL.Networking
{
	// Token: 0x020003D3 RID: 979
	[Flags]
	public enum ReplicationFlags : byte
	{
		// Token: 0x04002143 RID: 8515
		DryRun = 0,
		// Token: 0x04002144 RID: 8516
		Client = 1,
		// Token: 0x04002145 RID: 8517
		Server = 2
	}
}

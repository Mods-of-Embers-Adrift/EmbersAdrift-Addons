using System;

namespace SoL.Networking.Objects
{
	// Token: 0x020004B9 RID: 1209
	[Flags]
	public enum NetworkInclusionFlags
	{
		// Token: 0x04002627 RID: 9767
		None = 0,
		// Token: 0x04002628 RID: 9768
		Server = 1,
		// Token: 0x04002629 RID: 9769
		RemoteClient = 2,
		// Token: 0x0400262A RID: 9770
		LocalClient = 4,
		// Token: 0x0400262B RID: 9771
		All = 7
	}
}

using System;

namespace SoL.Utilities
{
	// Token: 0x02000273 RID: 627
	[Flags]
	public enum DeploymentBranchFlags
	{
		// Token: 0x04001BF9 RID: 7161
		None = 0,
		// Token: 0x04001BFA RID: 7162
		DEV = 1,
		// Token: 0x04001BFB RID: 7163
		QA = 2,
		// Token: 0x04001BFC RID: 7164
		LIVE = 4
	}
}

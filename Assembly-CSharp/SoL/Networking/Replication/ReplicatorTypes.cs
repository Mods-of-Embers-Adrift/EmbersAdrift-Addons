using System;

namespace SoL.Networking.Replication
{
	// Token: 0x02000480 RID: 1152
	[Flags]
	public enum ReplicatorTypes
	{
		// Token: 0x04002587 RID: 9607
		None = 0,
		// Token: 0x04002588 RID: 9608
		Transform = 1,
		// Token: 0x04002589 RID: 9609
		Animator = 2,
		// Token: 0x0400258A RID: 9610
		SyncVar = 4,
		// Token: 0x0400258B RID: 9611
		Animancer = 8
	}
}

using System;

namespace SoL.Game.Objects.Containers
{
	// Token: 0x02000A0A RID: 2570
	[Flags]
	public enum ContainerLockFlags
	{
		// Token: 0x04004793 RID: 18323
		None = 0,
		// Token: 0x04004794 RID: 18324
		Combat = 1,
		// Token: 0x04004795 RID: 18325
		MissingBag = 2,
		// Token: 0x04004796 RID: 18326
		Trade = 4,
		// Token: 0x04004797 RID: 18327
		UI = 8,
		// Token: 0x04004798 RID: 18328
		Harvesting = 16,
		// Token: 0x04004799 RID: 18329
		NotAlive = 32,
		// Token: 0x0400479A RID: 18330
		PostIncoming = 64,
		// Token: 0x0400479B RID: 18331
		Inspection = 128
	}
}

using System;

namespace SoL.Game.Dueling
{
	// Token: 0x02000C9C RID: 3228
	public enum DuelStatus : byte
	{
		// Token: 0x04005576 RID: 21878
		None,
		// Token: 0x04005577 RID: 21879
		Requested,
		// Token: 0x04005578 RID: 21880
		Accepted,
		// Token: 0x04005579 RID: 21881
		Declined,
		// Token: 0x0400557A RID: 21882
		Cancelled,
		// Token: 0x0400557B RID: 21883
		Expired,
		// Token: 0x0400557C RID: 21884
		Forfeited,
		// Token: 0x0400557D RID: 21885
		Executing,
		// Token: 0x0400557E RID: 21886
		Complete
	}
}

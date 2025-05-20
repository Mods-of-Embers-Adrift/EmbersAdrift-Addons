using System;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C62 RID: 3170
	[Flags]
	internal enum ExpirationConditionFlags
	{
		// Token: 0x0400546A RID: 21610
		None = 0,
		// Token: 0x0400546B RID: 21611
		TriggerCount = 1,
		// Token: 0x0400546C RID: 21612
		CanDismiss = 4,
		// Token: 0x0400546D RID: 21613
		Curable = 8
	}
}

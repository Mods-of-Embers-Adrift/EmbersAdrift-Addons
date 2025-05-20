using System;

namespace SoL.Game.Transactions
{
	// Token: 0x02000628 RID: 1576
	public enum ItemDestructionContext : byte
	{
		// Token: 0x04003028 RID: 12328
		None,
		// Token: 0x04003029 RID: 12329
		Charges,
		// Token: 0x0400302A RID: 12330
		Count,
		// Token: 0x0400302B RID: 12331
		Request,
		// Token: 0x0400302C RID: 12332
		Server,
		// Token: 0x0400302D RID: 12333
		Quest,
		// Token: 0x0400302E RID: 12334
		Durability,
		// Token: 0x0400302F RID: 12335
		Consumption,
		// Token: 0x04003030 RID: 12336
		Post
	}
}

using System;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A87 RID: 2695
	[Flags]
	public enum ItemFlags
	{
		// Token: 0x04004A87 RID: 19079
		None = 0,
		// Token: 0x04004A88 RID: 19080
		NoTrade = 1,
		// Token: 0x04004A89 RID: 19081
		NoSharedBank = 2,
		// Token: 0x04004A8A RID: 19082
		Quest = 4,
		// Token: 0x04004A8B RID: 19083
		Single = 8,
		// Token: 0x04004A8C RID: 19084
		Crafted = 16,
		// Token: 0x04004A8D RID: 19085
		NoSale = 32,
		// Token: 0x04004A8E RID: 19086
		Soulbound = 3
	}
}

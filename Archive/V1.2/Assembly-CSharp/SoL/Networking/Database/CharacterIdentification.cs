using System;

namespace SoL.Networking.Database
{
	// Token: 0x0200042B RID: 1067
	public class CharacterIdentification
	{
		// Token: 0x17000664 RID: 1636
		// (get) Token: 0x06001E8A RID: 7818 RVA: 0x00056BCF File Offset: 0x00054DCF
		// (set) Token: 0x06001E8B RID: 7819 RVA: 0x00056BD7 File Offset: 0x00054DD7
		public UniqueId _id { get; set; }

		// Token: 0x17000665 RID: 1637
		// (get) Token: 0x06001E8C RID: 7820 RVA: 0x00056BE0 File Offset: 0x00054DE0
		// (set) Token: 0x06001E8D RID: 7821 RVA: 0x00056BE8 File Offset: 0x00054DE8
		public string Name { get; set; }

		// Token: 0x17000666 RID: 1638
		// (get) Token: 0x06001E8E RID: 7822 RVA: 0x00056BF1 File Offset: 0x00054DF1
		// (set) Token: 0x06001E8F RID: 7823 RVA: 0x00056BF9 File Offset: 0x00054DF9
		public string UserId { get; set; }

		// Token: 0x17000667 RID: 1639
		// (get) Token: 0x06001E90 RID: 7824 RVA: 0x00056C02 File Offset: 0x00054E02
		// (set) Token: 0x06001E91 RID: 7825 RVA: 0x00056C0A File Offset: 0x00054E0A
		public AccessFlags AccessFlags { get; set; }

		// Token: 0x17000668 RID: 1640
		// (get) Token: 0x06001E92 RID: 7826 RVA: 0x00056C13 File Offset: 0x00054E13
		// (set) Token: 0x06001E93 RID: 7827 RVA: 0x00056C1B File Offset: 0x00054E1B
		public bool IsDeleted { get; set; }
	}
}

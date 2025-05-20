using System;

namespace SoL.Networking.SolServer
{
	// Token: 0x020003E6 RID: 998
	public class Penalty
	{
		// Token: 0x1700063B RID: 1595
		// (get) Token: 0x06001A8B RID: 6795 RVA: 0x000549C8 File Offset: 0x00052BC8
		// (set) Token: 0x06001A8C RID: 6796 RVA: 0x000549D0 File Offset: 0x00052BD0
		public string _id { get; set; }

		// Token: 0x1700063C RID: 1596
		// (get) Token: 0x06001A8D RID: 6797 RVA: 0x000549D9 File Offset: 0x00052BD9
		// (set) Token: 0x06001A8E RID: 6798 RVA: 0x000549E1 File Offset: 0x00052BE1
		public string UserId { get; set; }

		// Token: 0x1700063D RID: 1597
		// (get) Token: 0x06001A8F RID: 6799 RVA: 0x000549EA File Offset: 0x00052BEA
		// (set) Token: 0x06001A90 RID: 6800 RVA: 0x000549F2 File Offset: 0x00052BF2
		public string InfractionDescription { get; set; }

		// Token: 0x1700063E RID: 1598
		// (get) Token: 0x06001A91 RID: 6801 RVA: 0x000549FB File Offset: 0x00052BFB
		// (set) Token: 0x06001A92 RID: 6802 RVA: 0x00054A03 File Offset: 0x00052C03
		public PenaltyType Type { get; set; }

		// Token: 0x1700063F RID: 1599
		// (get) Token: 0x06001A93 RID: 6803 RVA: 0x00054A0C File Offset: 0x00052C0C
		// (set) Token: 0x06001A94 RID: 6804 RVA: 0x00054A14 File Offset: 0x00052C14
		public DateTime Created { get; set; }

		// Token: 0x17000640 RID: 1600
		// (get) Token: 0x06001A95 RID: 6805 RVA: 0x00054A1D File Offset: 0x00052C1D
		// (set) Token: 0x06001A96 RID: 6806 RVA: 0x00054A25 File Offset: 0x00052C25
		public DateTime? Expiration { get; set; }
	}
}

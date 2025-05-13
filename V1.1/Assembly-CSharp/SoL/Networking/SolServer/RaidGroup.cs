using System;
using System.Collections.Generic;

namespace SoL.Networking.SolServer
{
	// Token: 0x020003E9 RID: 1001
	public class RaidGroup
	{
		// Token: 0x1700064F RID: 1615
		// (get) Token: 0x06001AB0 RID: 6832 RVA: 0x00054AF0 File Offset: 0x00052CF0
		// (set) Token: 0x06001AB1 RID: 6833 RVA: 0x00054AF8 File Offset: 0x00052CF8
		public string GroupId { get; set; }

		// Token: 0x17000650 RID: 1616
		// (get) Token: 0x06001AB2 RID: 6834 RVA: 0x00054B01 File Offset: 0x00052D01
		// (set) Token: 0x06001AB3 RID: 6835 RVA: 0x00054B09 File Offset: 0x00052D09
		public string Leader { get; set; }

		// Token: 0x17000651 RID: 1617
		// (get) Token: 0x06001AB4 RID: 6836 RVA: 0x00054B12 File Offset: 0x00052D12
		// (set) Token: 0x06001AB5 RID: 6837 RVA: 0x00054B1A File Offset: 0x00052D1A
		public List<string> Members { get; set; }
	}
}

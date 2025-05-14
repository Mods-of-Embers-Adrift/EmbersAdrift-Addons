using System;
using System.Collections.Generic;

namespace SoL.Networking.SolServer
{
	// Token: 0x020003F8 RID: 1016
	public class UserIdentification
	{
		// Token: 0x17000659 RID: 1625
		// (get) Token: 0x06001B09 RID: 6921 RVA: 0x00054FDA File Offset: 0x000531DA
		// (set) Token: 0x06001B0A RID: 6922 RVA: 0x00054FE2 File Offset: 0x000531E2
		public string _id { get; set; }

		// Token: 0x1700065A RID: 1626
		// (get) Token: 0x06001B0B RID: 6923 RVA: 0x00054FEB File Offset: 0x000531EB
		// (set) Token: 0x06001B0C RID: 6924 RVA: 0x00054FF3 File Offset: 0x000531F3
		public string Username { get; set; }

		// Token: 0x1700065B RID: 1627
		// (get) Token: 0x06001B0D RID: 6925 RVA: 0x00054FFC File Offset: 0x000531FC
		// (set) Token: 0x06001B0E RID: 6926 RVA: 0x00055004 File Offset: 0x00053204
		public List<string> CharacterNames { get; set; }
	}
}

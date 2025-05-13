using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SoL.Networking.Database
{
	// Token: 0x0200045C RID: 1116
	public class GuildRank
	{
		// Token: 0x17000685 RID: 1669
		// (get) Token: 0x06001F5F RID: 8031 RVA: 0x00057169 File Offset: 0x00055369
		// (set) Token: 0x06001F60 RID: 8032 RVA: 0x00057171 File Offset: 0x00055371
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string _id { get; set; }

		// Token: 0x17000686 RID: 1670
		// (get) Token: 0x06001F61 RID: 8033 RVA: 0x0005717A File Offset: 0x0005537A
		// (set) Token: 0x06001F62 RID: 8034 RVA: 0x00057182 File Offset: 0x00055382
		public string Name { get; set; }

		// Token: 0x17000687 RID: 1671
		// (get) Token: 0x06001F63 RID: 8035 RVA: 0x0005718B File Offset: 0x0005538B
		// (set) Token: 0x06001F64 RID: 8036 RVA: 0x00057193 File Offset: 0x00055393
		public int Sort { get; set; }

		// Token: 0x17000688 RID: 1672
		// (get) Token: 0x06001F65 RID: 8037 RVA: 0x0005719C File Offset: 0x0005539C
		// (set) Token: 0x06001F66 RID: 8038 RVA: 0x000571A4 File Offset: 0x000553A4
		public GuildPermissions Permissions { get; set; }

		// Token: 0x17000689 RID: 1673
		// (get) Token: 0x06001F67 RID: 8039 RVA: 0x000571AD File Offset: 0x000553AD
		public bool IsGuildMaster
		{
			get
			{
				return this.Sort == int.MaxValue;
			}
		}
	}
}

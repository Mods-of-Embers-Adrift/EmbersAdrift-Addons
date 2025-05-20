using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace SoL.Networking.Database
{
	// Token: 0x02000458 RID: 1112
	public class Guild
	{
		// Token: 0x17000679 RID: 1657
		// (get) Token: 0x06001F43 RID: 8003 RVA: 0x0005709D File Offset: 0x0005529D
		// (set) Token: 0x06001F44 RID: 8004 RVA: 0x000570A5 File Offset: 0x000552A5
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string _id { get; set; }

		// Token: 0x1700067A RID: 1658
		// (get) Token: 0x06001F45 RID: 8005 RVA: 0x000570AE File Offset: 0x000552AE
		// (set) Token: 0x06001F46 RID: 8006 RVA: 0x000570B6 File Offset: 0x000552B6
		public string Name { get; set; }

		// Token: 0x1700067B RID: 1659
		// (get) Token: 0x06001F47 RID: 8007 RVA: 0x000570BF File Offset: 0x000552BF
		// (set) Token: 0x06001F48 RID: 8008 RVA: 0x000570C7 File Offset: 0x000552C7
		[BsonIgnoreIfNull]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string Description { get; set; }

		// Token: 0x1700067C RID: 1660
		// (get) Token: 0x06001F49 RID: 8009 RVA: 0x000570D0 File Offset: 0x000552D0
		// (set) Token: 0x06001F4A RID: 8010 RVA: 0x000570D8 File Offset: 0x000552D8
		[BsonIgnoreIfNull]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string Motd { get; set; }

		// Token: 0x1700067D RID: 1661
		// (get) Token: 0x06001F4B RID: 8011 RVA: 0x000570E1 File Offset: 0x000552E1
		// (set) Token: 0x06001F4C RID: 8012 RVA: 0x000570E9 File Offset: 0x000552E9
		[BsonIgnoreIfNull]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public List<GuildRank> Ranks { get; set; }

		// Token: 0x06001F4D RID: 8013 RVA: 0x0011F2E0 File Offset: 0x0011D4E0
		public GuildRank GetRankById(string rankId)
		{
			foreach (GuildRank guildRank in this.Ranks)
			{
				if (guildRank._id == rankId)
				{
					return guildRank;
				}
			}
			return null;
		}
	}
}

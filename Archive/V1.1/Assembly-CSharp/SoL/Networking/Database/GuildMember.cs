using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace SoL.Networking.Database
{
	// Token: 0x02000459 RID: 1113
	public class GuildMember
	{
		// Token: 0x1700067E RID: 1662
		// (get) Token: 0x06001F4F RID: 8015 RVA: 0x000570F2 File Offset: 0x000552F2
		// (set) Token: 0x06001F50 RID: 8016 RVA: 0x000570FA File Offset: 0x000552FA
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string _id { get; set; }

		// Token: 0x1700067F RID: 1663
		// (get) Token: 0x06001F51 RID: 8017 RVA: 0x00057103 File Offset: 0x00055303
		// (set) Token: 0x06001F52 RID: 8018 RVA: 0x0005710B File Offset: 0x0005530B
		public string CharacterId { get; set; }

		// Token: 0x17000680 RID: 1664
		// (get) Token: 0x06001F53 RID: 8019 RVA: 0x00057114 File Offset: 0x00055314
		// (set) Token: 0x06001F54 RID: 8020 RVA: 0x0005711C File Offset: 0x0005531C
		public string GuildId { get; set; }

		// Token: 0x17000681 RID: 1665
		// (get) Token: 0x06001F55 RID: 8021 RVA: 0x00057125 File Offset: 0x00055325
		// (set) Token: 0x06001F56 RID: 8022 RVA: 0x0005712D File Offset: 0x0005532D
		public string RankId { get; set; }

		// Token: 0x17000682 RID: 1666
		// (get) Token: 0x06001F57 RID: 8023 RVA: 0x00057136 File Offset: 0x00055336
		// (set) Token: 0x06001F58 RID: 8024 RVA: 0x0005713E File Offset: 0x0005533E
		[BsonIgnoreIfNull]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string PublicNote { get; set; }

		// Token: 0x17000683 RID: 1667
		// (get) Token: 0x06001F59 RID: 8025 RVA: 0x00057147 File Offset: 0x00055347
		// (set) Token: 0x06001F5A RID: 8026 RVA: 0x0005714F File Offset: 0x0005534F
		[BsonIgnoreIfNull]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string OfficerNote { get; set; }

		// Token: 0x17000684 RID: 1668
		// (get) Token: 0x06001F5B RID: 8027 RVA: 0x00057158 File Offset: 0x00055358
		// (set) Token: 0x06001F5C RID: 8028 RVA: 0x00057160 File Offset: 0x00055360
		public DateTime Created { get; set; }
	}
}

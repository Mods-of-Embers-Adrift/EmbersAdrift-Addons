using System;
using MongoDB.Bson.Serialization.Attributes;
using SoL.Networking.Database;

namespace SoL.Networking
{
	// Token: 0x020003B9 RID: 953
	[BsonIgnoreExtraElements]
	[Serializable]
	public class MailConfig : ConfigRecordBase
	{
		// Token: 0x17000626 RID: 1574
		// (get) Token: 0x060019DB RID: 6619 RVA: 0x000543B9 File Offset: 0x000525B9
		// (set) Token: 0x060019DC RID: 6620 RVA: 0x000543C1 File Offset: 0x000525C1
		public int PostExpirationInDays { get; set; }

		// Token: 0x060019DD RID: 6621 RVA: 0x00107B0C File Offset: 0x00105D0C
		public static MailConfig GetConfigFromDB()
		{
			ConfigRecord configRecord;
			return ConfigRecord.GetDeserializedConfigRecord<MailConfig>(ExternalGameDatabase.Database, "mail", out configRecord);
		}

		// Token: 0x040020DA RID: 8410
		[BsonIgnore]
		private const string kKey = "mail";
	}
}

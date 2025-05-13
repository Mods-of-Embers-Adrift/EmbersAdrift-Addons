using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using SoL.Game.Settings;
using SoL.Networking.Database;
using SoL.UI;

namespace SoL.Networking
{
	// Token: 0x020003B8 RID: 952
	[Serializable]
	public class LoginConfig : ConfigRecordBase
	{
		// Token: 0x060019D8 RID: 6616 RVA: 0x00107A04 File Offset: 0x00105C04
		public static LoginConfig GetConfigFromDB()
		{
			ConfigRecord configRecord;
			LoginConfig deserializedConfigRecord = ConfigRecord.GetDeserializedConfigRecord<LoginConfig>(ExternalGameDatabase.Database, "login", out configRecord);
			if (deserializedConfigRecord == null)
			{
				return null;
			}
			deserializedConfigRecord.ServerApiVersion = GlobalSettings.Values.Configs.Data.ServerApiVersion;
			deserializedConfigRecord.ServerApiTimestamp = GlobalSettings.Values.Configs.Data.LastServerApiUpdate;
			configRecord.data = deserializedConfigRecord.ToBsonDocument(null, null, default(BsonSerializationArgs));
			configRecord.UpdateServerApiVersion(ExternalGameDatabase.Database);
			return deserializedConfigRecord;
		}

		// Token: 0x060019D9 RID: 6617 RVA: 0x00107A80 File Offset: 0x00105C80
		public override string ToString()
		{
			return string.Concat(new string[]
			{
				"ApiVersion: ",
				this.ServerApiVersion,
				", ApiTimestamp: ",
				new DateTime(this.ServerApiTimestamp).ToString(),
				", LoginMessage: ",
				this.LoginMessage.ToJson(null, null, null, default(BsonSerializationArgs))
			});
		}

		// Token: 0x040020D6 RID: 8406
		[BsonIgnore]
		private const string kKey = "login";

		// Token: 0x040020D7 RID: 8407
		public string ServerApiVersion;

		// Token: 0x040020D8 RID: 8408
		public long ServerApiTimestamp;

		// Token: 0x040020D9 RID: 8409
		public TutorialPopupOptions LoginMessage;
	}
}

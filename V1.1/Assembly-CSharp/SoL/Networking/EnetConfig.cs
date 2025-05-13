using System;
using MongoDB.Bson.Serialization.Attributes;
using SoL.Networking.Database;

namespace SoL.Networking
{
	// Token: 0x020003B5 RID: 949
	[Serializable]
	public class EnetConfig : ConfigRecordBase
	{
		// Token: 0x060019C4 RID: 6596 RVA: 0x00107754 File Offset: 0x00105954
		public static EnetConfig GetConfigFromDB()
		{
			ConfigRecord configRecord;
			return ConfigRecord.GetDeserializedConfigRecord<EnetConfig>(ExternalGameDatabase.Database, "enet", out configRecord);
		}

		// Token: 0x060019C5 RID: 6597 RVA: 0x00107774 File Offset: 0x00105974
		public override string ToString()
		{
			return string.Concat(new string[]
			{
				"UpdateTime=",
				this.UpdateTime.ToString(),
				", ChannelCount=",
				this.ChannelCount.ToString(),
				", PeerLimit=",
				this.PeerLimit.ToString(),
				", UseSendInsteadOfBroadcast=",
				this.UseSendInsteadOfBroadcast.ToString()
			});
		}

		// Token: 0x040020B9 RID: 8377
		[BsonIgnore]
		private const string kKey = "enet";

		// Token: 0x040020BA RID: 8378
		public int UpdateTime;

		// Token: 0x040020BB RID: 8379
		public int ChannelCount;

		// Token: 0x040020BC RID: 8380
		public int PeerLimit;

		// Token: 0x040020BD RID: 8381
		public bool UseSendInsteadOfBroadcast;
	}
}

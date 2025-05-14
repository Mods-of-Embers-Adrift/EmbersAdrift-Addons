using System;
using MongoDB.Bson.Serialization.Attributes;
using SoL.Networking.Database;

namespace SoL.Networking
{
	// Token: 0x020003BB RID: 955
	[Serializable]
	public class ProximityConfig : ConfigRecordBase
	{
		// Token: 0x060019E0 RID: 6624 RVA: 0x00107B2C File Offset: 0x00105D2C
		public static ProximityConfig GetFromDB()
		{
			ConfigRecord configRecord;
			return ConfigRecord.GetDeserializedConfigRecord<ProximityConfig>(ExternalGameDatabase.Database, "playerProximity", out configRecord);
		}

		// Token: 0x040020DE RID: 8414
		[BsonIgnore]
		private const string kKey = "playerProximity";

		// Token: 0x040020DF RID: 8415
		public bool NpcsUseProximity;

		// Token: 0x040020E0 RID: 8416
		public bool PlayersUseProximity;

		// Token: 0x040020E1 RID: 8417
		public DistanceBandConfig[] DistanceBandConfigs;
	}
}

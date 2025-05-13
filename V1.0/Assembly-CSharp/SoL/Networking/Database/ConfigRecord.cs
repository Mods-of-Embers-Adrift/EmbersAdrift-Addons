using System;
using System.Threading;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using UnityEngine;

namespace SoL.Networking.Database
{
	// Token: 0x02000441 RID: 1089
	public class ConfigRecord
	{
		// Token: 0x06001EF3 RID: 7923 RVA: 0x0011D8A8 File Offset: 0x0011BAA8
		public static T GetDeserializedConfigRecord<T>(IMongoDatabase db, string requestedKey, out ConfigRecord configRecord) where T : ConfigRecordBase
		{
			configRecord = ConfigRecord.GetConfigRecord(db, requestedKey);
			if (configRecord == null)
			{
				return default(T);
			}
			T result = default(T);
			try
			{
				result = BsonSerializer.Deserialize<T>(configRecord.data, null);
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception while deserializing ConfigRecord " + requestedKey + "!  " + ex.ToString());
			}
			return result;
		}

		// Token: 0x06001EF4 RID: 7924 RVA: 0x0011D914 File Offset: 0x0011BB14
		private static ConfigRecord GetConfigRecord(IMongoDatabase db, string requestedKey)
		{
			IMongoCollection<ConfigRecord> collection = db.GetCollection<ConfigRecord>(ConfigRecord.kTableName, null);
			FilterDefinition<ConfigRecord> filter = Builders<ConfigRecord>.Filter.Eq<string>((ConfigRecord cr) => cr.key, requestedKey);
			ConfigRecord configRecord = collection.Find(filter, null).FirstOrDefault(default(CancellationToken));
			if (configRecord == null)
			{
				Debug.LogError("Unable to find a config for: " + requestedKey);
				return null;
			}
			return configRecord;
		}

		// Token: 0x06001EF5 RID: 7925 RVA: 0x0011D99C File Offset: 0x0011BB9C
		public bool UpdateServerApiVersion(IMongoDatabase db)
		{
			IMongoCollection<ConfigRecord> collection = db.GetCollection<ConfigRecord>(ConfigRecord.kTableName, null);
			FilterDefinition<ConfigRecord> filter = Builders<ConfigRecord>.Filter.Eq<string>((ConfigRecord cr) => cr.key, this.key);
			UpdateDefinition<ConfigRecord> update = Builders<ConfigRecord>.Update.Set<BsonDocument>((ConfigRecord cr) => cr.data, this.data);
			return collection.UpdateOne(filter, update, null, default(CancellationToken)).ModifiedCount > 0L;
		}

		// Token: 0x04002457 RID: 9303
		private static string kTableName = "configs";

		// Token: 0x04002458 RID: 9304
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string Id;

		// Token: 0x04002459 RID: 9305
		public int world_id;

		// Token: 0x0400245A RID: 9306
		public string key;

		// Token: 0x0400245B RID: 9307
		public BsonDocument data;
	}
}

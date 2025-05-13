using System;
using System.Collections.Generic;
using System.Threading;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace SoL.Networking.Database
{
	// Token: 0x02000422 RID: 1058
	public class AuthRecord
	{
		// Token: 0x06001E75 RID: 7797 RVA: 0x0011AD5C File Offset: 0x00118F5C
		public static AuthRecord Load(IMongoDatabase db, string id)
		{
			IMongoCollection<AuthRecord> collection = db.GetCollection<AuthRecord>(AuthRecord.kTableName, null);
			FilterDefinition<AuthRecord> filter = Builders<AuthRecord>.Filter.Eq<string>((AuthRecord ar) => ar.Id, id);
			return collection.Find(filter, null).FirstOrDefault(default(CancellationToken));
		}

		// Token: 0x06001E76 RID: 7798 RVA: 0x0011ADCC File Offset: 0x00118FCC
		public static IEnumerable<AuthRecord> LoadAll(IMongoDatabase db)
		{
			return db.GetCollection<AuthRecord>(AuthRecord.kTableName, null).Find(FilterDefinition<AuthRecord>.Empty, null).ToList(default(CancellationToken));
		}

		// Token: 0x040023C7 RID: 9159
		[BsonId]
		[BsonRepresentation(BsonType.String)]
		public string Id;

		// Token: 0x040023C8 RID: 9160
		[BsonDateTimeOptions(Representation = BsonType.String)]
		public DateTime last_active;

		// Token: 0x040023C9 RID: 9161
		public string session;

		// Token: 0x040023CA RID: 9162
		public string steamid;

		// Token: 0x040023CB RID: 9163
		private static string kTableName = "active_sessions";
	}
}

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace SoL.Networking.Database
{
	// Token: 0x02000455 RID: 1109
	public class GroupRecord
	{
		// Token: 0x06001F3A RID: 7994 RVA: 0x0011F0D0 File Offset: 0x0011D2D0
		public static IEnumerable<GroupRecord> LoadAll(IMongoDatabase db)
		{
			return db.GetCollection<GroupRecord>(GroupRecord.kTableName, null).Find(FilterDefinition<GroupRecord>.Empty, null).ToList(default(CancellationToken));
		}

		// Token: 0x06001F3B RID: 7995 RVA: 0x0011F104 File Offset: 0x0011D304
		public static GroupRecord Load(IMongoDatabase db, string id)
		{
			IMongoCollection<GroupRecord> collection = db.GetCollection<GroupRecord>(GroupRecord.kTableName, null);
			FilterDefinition<GroupRecord> filter = Builders<GroupRecord>.Filter.Eq<string>((GroupRecord gr) => gr.Id, id);
			return collection.Find(filter, null).FirstOrDefault(default(CancellationToken));
		}

		// Token: 0x06001F3C RID: 7996 RVA: 0x0011F174 File Offset: 0x0011D374
		public static Task<GroupRecord> LoadAsync(IMongoDatabase db, string id)
		{
			GroupRecord.<LoadAsync>d__8 <LoadAsync>d__;
			<LoadAsync>d__.<>t__builder = AsyncTaskMethodBuilder<GroupRecord>.Create();
			<LoadAsync>d__.db = db;
			<LoadAsync>d__.id = id;
			<LoadAsync>d__.<>1__state = -1;
			<LoadAsync>d__.<>t__builder.Start<GroupRecord.<LoadAsync>d__8>(ref <LoadAsync>d__);
			return <LoadAsync>d__.<>t__builder.Task;
		}

		// Token: 0x040024AD RID: 9389
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string Id;

		// Token: 0x040024AE RID: 9390
		public string Leader;

		// Token: 0x040024AF RID: 9391
		public List<string> Members;

		// Token: 0x040024B0 RID: 9392
		public DateTime LastActive;

		// Token: 0x040024B1 RID: 9393
		public string RaidId;

		// Token: 0x040024B2 RID: 9394
		private static string kTableName = "groups";
	}
}

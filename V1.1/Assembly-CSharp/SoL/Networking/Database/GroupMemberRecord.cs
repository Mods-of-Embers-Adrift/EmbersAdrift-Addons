using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace SoL.Networking.Database
{
	// Token: 0x02000453 RID: 1107
	public class GroupMemberRecord
	{
		// Token: 0x06001F33 RID: 7987 RVA: 0x0011EFBC File Offset: 0x0011D1BC
		public static IEnumerable<GroupMemberRecord> LoadAll(IMongoDatabase db)
		{
			return db.GetCollection<GroupMemberRecord>(GroupMemberRecord.kTableName, null).Find(FilterDefinition<GroupMemberRecord>.Empty, null).ToList(default(CancellationToken));
		}

		// Token: 0x06001F34 RID: 7988 RVA: 0x0011EFF0 File Offset: 0x0011D1F0
		public static GroupMemberRecord Load(IMongoDatabase db, string id)
		{
			IMongoCollection<GroupMemberRecord> collection = db.GetCollection<GroupMemberRecord>(GroupMemberRecord.kTableName, null);
			FilterDefinition<GroupMemberRecord> filter = Builders<GroupMemberRecord>.Filter.Eq<string>((GroupMemberRecord gmr) => gmr.Id, id);
			return collection.Find(filter, null).FirstOrDefault(default(CancellationToken));
		}

		// Token: 0x06001F35 RID: 7989 RVA: 0x0011F060 File Offset: 0x0011D260
		public static Task<List<GroupMemberRecord>> LoadGroupMembers(IMongoDatabase db, string groupId)
		{
			IMongoCollection<GroupMemberRecord> collection = db.GetCollection<GroupMemberRecord>(GroupMemberRecord.kTableName, null);
			FilterDefinition<GroupMemberRecord> filter = Builders<GroupMemberRecord>.Filter.Eq<string>((GroupMemberRecord gmr) => gmr.GroupId, groupId);
			return collection.Find(filter, null).ToListAsync(default(CancellationToken));
		}

		// Token: 0x040024A8 RID: 9384
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string Id;

		// Token: 0x040024A9 RID: 9385
		public string GroupId;

		// Token: 0x040024AA RID: 9386
		public DateTime LastActive;

		// Token: 0x040024AB RID: 9387
		private static string kTableName = "group_members";
	}
}

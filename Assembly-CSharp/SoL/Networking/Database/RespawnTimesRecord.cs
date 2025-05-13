using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using SoL.Game.Spawning;
using UnityEngine;

namespace SoL.Networking.Database
{
	// Token: 0x02000469 RID: 1129
	[BsonIgnoreExtraElements]
	public class RespawnTimesRecord
	{
		// Token: 0x06001FB8 RID: 8120 RVA: 0x00120474 File Offset: 0x0011E674
		public static RespawnTimesRecord Load(IMongoDatabase db, string id)
		{
			IMongoCollection<RespawnTimesRecord> collection = db.GetCollection<RespawnTimesRecord>("respawn_times", null);
			FilterDefinition<RespawnTimesRecord> filter = Builders<RespawnTimesRecord>.Filter.Eq<string>((RespawnTimesRecord record) => record.Id, id);
			return collection.Find(filter, null).FirstOrDefault(default(CancellationToken));
		}

		// Token: 0x06001FB9 RID: 8121 RVA: 0x001204E4 File Offset: 0x0011E6E4
		public void Save(IMongoDatabase db)
		{
			db.GetCollection<RespawnTimesRecord>("respawn_times", null).InsertOne(this, null, default(CancellationToken));
			Debug.LogFormat("[MONGO] Inserted RespawnTimesRecord for {0} ({1})", new object[]
			{
				this.Id,
				this.Description
			});
		}

		// Token: 0x06001FBA RID: 8122 RVA: 0x00120530 File Offset: 0x0011E730
		public static RespawnTimesRecord StoreNew(IMongoDatabase db, SpawnControllerDbProfile profile)
		{
			if (!profile)
			{
				throw new ArgumentNullException("profile");
			}
			RespawnTimesRecord respawnTimesRecord = new RespawnTimesRecord
			{
				Id = profile.Id.Value,
				Updated = DateTime.UtcNow,
				Description = profile.Description,
				RespawnTimes = new List<DateTime>(10)
			};
			db.GetCollection<RespawnTimesRecord>("respawn_times", null).InsertOne(respawnTimesRecord, null, default(CancellationToken));
			Debug.LogFormat("[MONGO] Inserted RespawnTimesRecord for {0} ({1})", new object[]
			{
				respawnTimesRecord.Id,
				respawnTimesRecord.Description
			});
			return respawnTimesRecord;
		}

		// Token: 0x06001FBB RID: 8123 RVA: 0x001205D0 File Offset: 0x0011E7D0
		public bool Update(IMongoDatabase db)
		{
			IMongoCollection<RespawnTimesRecord> collection = db.GetCollection<RespawnTimesRecord>("respawn_times", null);
			FilterDefinition<RespawnTimesRecord> filter = Builders<RespawnTimesRecord>.Filter.Eq<string>((RespawnTimesRecord record) => record.Id, this.Id);
			UpdateDefinition<RespawnTimesRecord> update = Builders<RespawnTimesRecord>.Update.Set<DateTime>((RespawnTimesRecord record) => record.Updated, DateTime.UtcNow).Set((RespawnTimesRecord record) => record.RespawnTimes, this.RespawnTimes);
			return collection.UpdateOne(filter, update, null, default(CancellationToken)).ModifiedCount > 0L;
		}

		// Token: 0x06001FBC RID: 8124 RVA: 0x001206D0 File Offset: 0x0011E8D0
		public Task<bool> UpdateAsync(IMongoDatabase db)
		{
			RespawnTimesRecord.<UpdateAsync>d__9 <UpdateAsync>d__;
			<UpdateAsync>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<UpdateAsync>d__.<>4__this = this;
			<UpdateAsync>d__.db = db;
			<UpdateAsync>d__.<>1__state = -1;
			<UpdateAsync>d__.<>t__builder.Start<RespawnTimesRecord.<UpdateAsync>d__9>(ref <UpdateAsync>d__);
			return <UpdateAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06001FBD RID: 8125 RVA: 0x0012071C File Offset: 0x0011E91C
		public bool Delete(IMongoDatabase db)
		{
			IMongoCollection<RespawnTimesRecord> collection = db.GetCollection<RespawnTimesRecord>("respawn_times", null);
			FilterDefinition<RespawnTimesRecord> filter = Builders<RespawnTimesRecord>.Filter.Eq<string>((RespawnTimesRecord record) => record.Id, this.Id);
			DeleteResult deleteResult = collection.DeleteOne(filter, default(CancellationToken));
			Debug.LogFormat("[MONGO] Deleted RespawnTimesRecord for {0}: {1}", new object[]
			{
				this.Id,
				deleteResult.DeletedCount > 0L
			});
			return deleteResult.DeletedCount > 0L;
		}

		// Token: 0x0400251F RID: 9503
		[BsonId]
		[BsonRepresentation(BsonType.String)]
		public string Id;

		// Token: 0x04002520 RID: 9504
		public DateTime Updated;

		// Token: 0x04002521 RID: 9505
		public string Description;

		// Token: 0x04002522 RID: 9506
		public List<DateTime> RespawnTimes;

		// Token: 0x04002523 RID: 9507
		private const string kTableName = "respawn_times";
	}
}

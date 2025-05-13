using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using SoL.Networking.Managers;
using UnityEngine;

namespace SoL.Networking.Database
{
	// Token: 0x02000419 RID: 1049
	[BsonIgnoreExtraElements]
	public class ActiveZoneRecord
	{
		// Token: 0x06001E4F RID: 7759 RVA: 0x00119CE0 File Offset: 0x00117EE0
		private static FilterDefinition<ActiveZoneRecord> GetIdFilter(ActiveZoneRecord zoneRecord)
		{
			FilterDefinitionBuilder<ActiveZoneRecord> filter = Builders<ActiveZoneRecord>.Filter;
			return filter.Eq<string>((ActiveZoneRecord zr) => zr.ZoneRecordId, zoneRecord.ZoneRecordId) & filter.Eq<int>((ActiveZoneRecord zr) => zr.WorldId, zoneRecord.WorldId) & filter.Eq<int>((ActiveZoneRecord zr) => zr.InstanceId, zoneRecord.InstanceId);
		}

		// Token: 0x06001E50 RID: 7760 RVA: 0x00119DC0 File Offset: 0x00117FC0
		public static ActiveZoneRecord StoreNew(IMongoDatabase db, ZoneRecord zoneRecord, int instanceId)
		{
			zoneRecord.Port = (int)ServerNetworkManager.Port;
			ActiveZoneRecord activeZoneRecord = new ActiveZoneRecord
			{
				ZoneRecordId = zoneRecord.Id,
				WorldId = zoneRecord.WorldId,
				Port = (int)ServerNetworkManager.Port,
				InstanceId = instanceId,
				PlayerCount = 0,
				PopulationThreshold = zoneRecord.DefaultPopulationThreshold,
				Created = DateTime.UtcNow,
				LastActive = DateTime.UtcNow
			};
			IMongoCollection<ActiveZoneRecord> collection = db.GetCollection<ActiveZoneRecord>(ActiveZoneRecord.kTableName, null);
			FilterDefinition<ActiveZoneRecord> filter = ActiveZoneRecord.GetIdFilter(activeZoneRecord);
			ActiveZoneRecord activeZoneRecord2 = collection.Find(filter, null).FirstOrDefault(default(CancellationToken));
			if (activeZoneRecord2 == null)
			{
				collection.InsertOne(activeZoneRecord, null, default(CancellationToken));
				activeZoneRecord2 = collection.Find(filter, null).FirstOrDefault(default(CancellationToken));
				if (activeZoneRecord2 == null)
				{
					Debug.LogError("[MONGO] Could not find current ActiveZoneRecord...even though we just created/replaced it!");
					return null;
				}
				Debug.LogFormat("[MONGO] Inserted new ActiveZoneRecord for {0}", new object[]
				{
					zoneRecord.DisplayName
				});
			}
			else
			{
				filter = Builders<ActiveZoneRecord>.Filter.Eq<string>((ActiveZoneRecord azr) => azr.Id, activeZoneRecord2.Id);
				UpdateDefinition<ActiveZoneRecord> update = Builders<ActiveZoneRecord>.Update.Set<int>((ActiveZoneRecord azr) => azr.Port, activeZoneRecord.Port).Set((ActiveZoneRecord azr) => azr.InstanceId, activeZoneRecord.InstanceId).Set((ActiveZoneRecord azr) => azr.PlayerCount, BaseNetworkEntityManager.PlayerConnectedCount).Set((ActiveZoneRecord azr) => azr.LastActive, activeZoneRecord.LastActive);
				UpdateResult updateResult = collection.UpdateOne(filter, update, null, default(CancellationToken));
				Debug.LogFormat("[MONGO] Updated current ActiveZoneRecord for {0}: {1}", new object[]
				{
					zoneRecord.DisplayName,
					updateResult.IsAcknowledged
				});
			}
			activeZoneRecord2.ZoneRecord = zoneRecord;
			return activeZoneRecord2;
		}

		// Token: 0x06001E51 RID: 7761 RVA: 0x0011A058 File Offset: 0x00118258
		public static List<ActiveZoneRecord> GetActiveZones(IMongoDatabase db)
		{
			return db.GetCollection<ActiveZoneRecord>(ActiveZoneRecord.kTableName, null).Find(FilterDefinition<ActiveZoneRecord>.Empty, null).ToList(default(CancellationToken));
		}

		// Token: 0x06001E52 RID: 7762 RVA: 0x0011A08C File Offset: 0x0011828C
		public static ActiveZoneRecord Load(IMongoDatabase db, string zoneRecordId)
		{
			IMongoCollection<ActiveZoneRecord> collection = db.GetCollection<ActiveZoneRecord>(ActiveZoneRecord.kTableName, null);
			FilterDefinition<ActiveZoneRecord> filter = Builders<ActiveZoneRecord>.Filter.Eq<string>((ActiveZoneRecord zr) => zr.ZoneRecordId, zoneRecordId);
			return collection.Find(filter, null).FirstOrDefault(default(CancellationToken));
		}

		// Token: 0x06001E53 RID: 7763 RVA: 0x0011A0FC File Offset: 0x001182FC
		public static List<ActiveZoneRecord> LoadAll(IMongoDatabase db, string zoneRecordId, int worldId)
		{
			IMongoCollection<ActiveZoneRecord> collection = db.GetCollection<ActiveZoneRecord>(ActiveZoneRecord.kTableName, null);
			FilterDefinitionBuilder<ActiveZoneRecord> filter = Builders<ActiveZoneRecord>.Filter;
			FilterDefinition<ActiveZoneRecord> filter2 = filter.Eq<string>((ActiveZoneRecord zr) => zr.ZoneRecordId, zoneRecordId) & filter.Eq<int>((ActiveZoneRecord zr) => zr.WorldId, worldId);
			return collection.Find(filter2, null).ToList(default(CancellationToken));
		}

		// Token: 0x06001E54 RID: 7764 RVA: 0x0011A1B0 File Offset: 0x001183B0
		public static Task<ActiveZoneRecord> LoadAsync(IMongoDatabase db, string zoneRecordId, int worldId)
		{
			ActiveZoneRecord.<LoadAsync>d__17 <LoadAsync>d__;
			<LoadAsync>d__.<>t__builder = AsyncTaskMethodBuilder<ActiveZoneRecord>.Create();
			<LoadAsync>d__.db = db;
			<LoadAsync>d__.zoneRecordId = zoneRecordId;
			<LoadAsync>d__.worldId = worldId;
			<LoadAsync>d__.<>1__state = -1;
			<LoadAsync>d__.<>t__builder.Start<ActiveZoneRecord.<LoadAsync>d__17>(ref <LoadAsync>d__);
			return <LoadAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06001E55 RID: 7765 RVA: 0x0011A204 File Offset: 0x00118404
		public static Task<List<ActiveZoneRecord>> LoadAllAsync(IMongoDatabase db, string zoneRecordId, int worldId)
		{
			ActiveZoneRecord.<LoadAllAsync>d__18 <LoadAllAsync>d__;
			<LoadAllAsync>d__.<>t__builder = AsyncTaskMethodBuilder<List<ActiveZoneRecord>>.Create();
			<LoadAllAsync>d__.db = db;
			<LoadAllAsync>d__.zoneRecordId = zoneRecordId;
			<LoadAllAsync>d__.worldId = worldId;
			<LoadAllAsync>d__.<>1__state = -1;
			<LoadAllAsync>d__.<>t__builder.Start<ActiveZoneRecord.<LoadAllAsync>d__18>(ref <LoadAllAsync>d__);
			return <LoadAllAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06001E56 RID: 7766 RVA: 0x0011A258 File Offset: 0x00118458
		public Task<bool> UpdateZoneState(IMongoDatabase db)
		{
			ActiveZoneRecord.<UpdateZoneState>d__19 <UpdateZoneState>d__;
			<UpdateZoneState>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<UpdateZoneState>d__.<>4__this = this;
			<UpdateZoneState>d__.db = db;
			<UpdateZoneState>d__.<>1__state = -1;
			<UpdateZoneState>d__.<>t__builder.Start<ActiveZoneRecord.<UpdateZoneState>d__19>(ref <UpdateZoneState>d__);
			return <UpdateZoneState>d__.<>t__builder.Task;
		}

		// Token: 0x06001E57 RID: 7767 RVA: 0x0011A2A4 File Offset: 0x001184A4
		public void Delete(IMongoDatabase db)
		{
			IMongoCollection<ActiveZoneRecord> collection = db.GetCollection<ActiveZoneRecord>(ActiveZoneRecord.kTableName, null);
			FilterDefinition<ActiveZoneRecord> idFilter = ActiveZoneRecord.GetIdFilter(this);
			DeleteResult deleteResult = collection.DeleteOne(idFilter, default(CancellationToken));
			Debug.LogFormat("[MONGO] Deleted ActiveZoneRecord for {0}: {1}", new object[]
			{
				(this.ZoneRecord == null) ? "Unknown" : this.ZoneRecord.DisplayName,
				deleteResult.DeletedCount > 0L
			});
		}

		// Token: 0x0400238C RID: 9100
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string Id;

		// Token: 0x0400238D RID: 9101
		[BsonRepresentation(BsonType.ObjectId)]
		public string ZoneRecordId;

		// Token: 0x0400238E RID: 9102
		public int WorldId;

		// Token: 0x0400238F RID: 9103
		public DateTime Created;

		// Token: 0x04002390 RID: 9104
		public DateTime LastActive;

		// Token: 0x04002391 RID: 9105
		public int Port;

		// Token: 0x04002392 RID: 9106
		public int InstanceId;

		// Token: 0x04002393 RID: 9107
		public int PlayerCount;

		// Token: 0x04002394 RID: 9108
		public int PopulationThreshold;

		// Token: 0x04002395 RID: 9109
		[BsonIgnore]
		public ZoneRecord ZoneRecord;

		// Token: 0x04002396 RID: 9110
		private static string kTableName = "active_zones";

		// Token: 0x0200041A RID: 1050
		public enum ZoneStatus
		{
			// Token: 0x04002398 RID: 9112
			Offline,
			// Token: 0x04002399 RID: 9113
			Maintenance,
			// Token: 0x0400239A RID: 9114
			Online
		}
	}
}

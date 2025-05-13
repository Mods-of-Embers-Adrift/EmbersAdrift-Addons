using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using SoL.Game;
using SoL.Game.NPCs;
using SoL.Managers;

namespace SoL.Networking.Database
{
	// Token: 0x02000446 RID: 1094
	[BsonIgnoreExtraElements]
	public class DungeonEntranceRecord
	{
		// Token: 0x17000673 RID: 1651
		// (get) Token: 0x06001F07 RID: 7943 RVA: 0x00056F55 File Offset: 0x00055155
		[BsonIgnore]
		public ZoneRecord DungeonZone
		{
			get
			{
				if (this.m_dungeonZone == null)
				{
					this.m_dungeonZone = SessionData.GetZoneRecord((ZoneId)this.DungeonZoneId);
				}
				return this.m_dungeonZone;
			}
		}

		// Token: 0x17000674 RID: 1652
		// (get) Token: 0x06001F08 RID: 7944 RVA: 0x00056F76 File Offset: 0x00055176
		[BsonIgnore]
		public ZoneRecord OverworldZone
		{
			get
			{
				if (this.m_overworldZone == null)
				{
					this.m_overworldZone = SessionData.GetZoneRecord((ZoneId)this.OverworldZoneId);
				}
				return this.m_overworldZone;
			}
		}

		// Token: 0x06001F09 RID: 7945 RVA: 0x00056F97 File Offset: 0x00055197
		public static void ValidateInternal()
		{
			DungeonEntranceRecord.ValidateIndexes(ExternalGameDatabase.Database);
		}

		// Token: 0x06001F0A RID: 7946 RVA: 0x0011DD00 File Offset: 0x0011BF00
		public static void ValidateCollection(IMongoDatabase db)
		{
			if (db.GetCollection<DungeonEntranceRecord>(DungeonEntranceRecord.kTableName, null) == null)
			{
				db.CreateCollection(DungeonEntranceRecord.kTableName, null, default(CancellationToken));
				DungeonEntranceRecord.ValidateIndexes(db);
			}
		}

		// Token: 0x06001F0B RID: 7947 RVA: 0x0011DD38 File Offset: 0x0011BF38
		private static void ValidateIndexes(IMongoDatabase db)
		{
			IMongoCollection<DungeonEntranceRecord> collection = db.GetCollection<DungeonEntranceRecord>(DungeonEntranceRecord.kTableName, null);
			if (collection != null)
			{
				CreateIndexModel<DungeonEntranceRecord> model = new CreateIndexModel<DungeonEntranceRecord>(new IndexKeysDefinitionBuilder<DungeonEntranceRecord>().Ascending((DungeonEntranceRecord i) => (object)i.DungeonZoneId), null);
				CreateIndexModel<DungeonEntranceRecord> model2 = new CreateIndexModel<DungeonEntranceRecord>(new IndexKeysDefinitionBuilder<DungeonEntranceRecord>().Ascending((DungeonEntranceRecord i) => (object)i.OverworldZoneId), null);
				collection.Indexes.CreateOne(model, null, default(CancellationToken));
				collection.Indexes.CreateOne(model2, null, default(CancellationToken));
			}
		}

		// Token: 0x06001F0C RID: 7948 RVA: 0x0011DE30 File Offset: 0x0011C030
		public static Task<DungeonEntranceRecord> LoadForIdAsync(IMongoDatabase db, string id)
		{
			DungeonEntranceRecord.<LoadForIdAsync>d__19 <LoadForIdAsync>d__;
			<LoadForIdAsync>d__.<>t__builder = AsyncTaskMethodBuilder<DungeonEntranceRecord>.Create();
			<LoadForIdAsync>d__.db = db;
			<LoadForIdAsync>d__.id = id;
			<LoadForIdAsync>d__.<>1__state = -1;
			<LoadForIdAsync>d__.<>t__builder.Start<DungeonEntranceRecord.<LoadForIdAsync>d__19>(ref <LoadForIdAsync>d__);
			return <LoadForIdAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06001F0D RID: 7949 RVA: 0x0011DE7C File Offset: 0x0011C07C
		public static IEnumerable<DungeonEntranceRecord> LoadForOverworldZone(IMongoDatabase db, int zoneId)
		{
			IMongoCollection<DungeonEntranceRecord> collection = db.GetCollection<DungeonEntranceRecord>(DungeonEntranceRecord.kTableName, null);
			FilterDefinition<DungeonEntranceRecord> filter = Builders<DungeonEntranceRecord>.Filter.Eq<int>((DungeonEntranceRecord der) => der.OverworldZoneId, zoneId);
			return collection.Find(filter, null).ToList(default(CancellationToken));
		}

		// Token: 0x06001F0E RID: 7950 RVA: 0x0011DEEC File Offset: 0x0011C0EC
		public static Task<List<DungeonEntranceRecord>> LoadForOverworldZoneAsync(IMongoDatabase db, int zoneId)
		{
			DungeonEntranceRecord.<LoadForOverworldZoneAsync>d__21 <LoadForOverworldZoneAsync>d__;
			<LoadForOverworldZoneAsync>d__.<>t__builder = AsyncTaskMethodBuilder<List<DungeonEntranceRecord>>.Create();
			<LoadForOverworldZoneAsync>d__.db = db;
			<LoadForOverworldZoneAsync>d__.zoneId = zoneId;
			<LoadForOverworldZoneAsync>d__.<>1__state = -1;
			<LoadForOverworldZoneAsync>d__.<>t__builder.Start<DungeonEntranceRecord.<LoadForOverworldZoneAsync>d__21>(ref <LoadForOverworldZoneAsync>d__);
			return <LoadForOverworldZoneAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06001F0F RID: 7951 RVA: 0x0011DF38 File Offset: 0x0011C138
		public static IEnumerable<DungeonEntranceRecord> LoadForDungeonZone(IMongoDatabase db, int zoneId)
		{
			IMongoCollection<DungeonEntranceRecord> collection = db.GetCollection<DungeonEntranceRecord>(DungeonEntranceRecord.kTableName, null);
			FilterDefinition<DungeonEntranceRecord> filter = Builders<DungeonEntranceRecord>.Filter.Eq<int>((DungeonEntranceRecord der) => der.DungeonZoneId, zoneId);
			return collection.Find(filter, null).ToList(default(CancellationToken));
		}

		// Token: 0x06001F10 RID: 7952 RVA: 0x0011DFA8 File Offset: 0x0011C1A8
		public static Task<List<DungeonEntranceRecord>> LoadForDungeonZoneAsync(IMongoDatabase db, int zoneId)
		{
			DungeonEntranceRecord.<LoadForDungeonZoneAsync>d__23 <LoadForDungeonZoneAsync>d__;
			<LoadForDungeonZoneAsync>d__.<>t__builder = AsyncTaskMethodBuilder<List<DungeonEntranceRecord>>.Create();
			<LoadForDungeonZoneAsync>d__.db = db;
			<LoadForDungeonZoneAsync>d__.zoneId = zoneId;
			<LoadForDungeonZoneAsync>d__.<>1__state = -1;
			<LoadForDungeonZoneAsync>d__.<>t__builder.Start<DungeonEntranceRecord.<LoadForDungeonZoneAsync>d__23>(ref <LoadForDungeonZoneAsync>d__);
			return <LoadForDungeonZoneAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06001F11 RID: 7953 RVA: 0x0011DFF4 File Offset: 0x0011C1F4
		public static Task<List<DungeonEntranceRecord>> LoadForDungeonZoneAsync(IMongoDatabase db, int localZoneId, int sourceZoneId)
		{
			DungeonEntranceRecord.<LoadForDungeonZoneAsync>d__24 <LoadForDungeonZoneAsync>d__;
			<LoadForDungeonZoneAsync>d__.<>t__builder = AsyncTaskMethodBuilder<List<DungeonEntranceRecord>>.Create();
			<LoadForDungeonZoneAsync>d__.db = db;
			<LoadForDungeonZoneAsync>d__.localZoneId = localZoneId;
			<LoadForDungeonZoneAsync>d__.sourceZoneId = sourceZoneId;
			<LoadForDungeonZoneAsync>d__.<>1__state = -1;
			<LoadForDungeonZoneAsync>d__.<>t__builder.Start<DungeonEntranceRecord.<LoadForDungeonZoneAsync>d__24>(ref <LoadForDungeonZoneAsync>d__);
			return <LoadForDungeonZoneAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06001F12 RID: 7954 RVA: 0x0011E048 File Offset: 0x0011C248
		public static Task CreateEntranceAsync(IMongoDatabase db, DungeonEntranceRecord record)
		{
			DungeonEntranceRecord.<CreateEntranceAsync>d__25 <CreateEntranceAsync>d__;
			<CreateEntranceAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<CreateEntranceAsync>d__.db = db;
			<CreateEntranceAsync>d__.record = record;
			<CreateEntranceAsync>d__.<>1__state = -1;
			<CreateEntranceAsync>d__.<>t__builder.Start<DungeonEntranceRecord.<CreateEntranceAsync>d__25>(ref <CreateEntranceAsync>d__);
			return <CreateEntranceAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06001F13 RID: 7955 RVA: 0x0011E094 File Offset: 0x0011C294
		private static Task DeleteOne(IMongoDatabase db, string id)
		{
			DungeonEntranceRecord.<DeleteOne>d__26 <DeleteOne>d__;
			<DeleteOne>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<DeleteOne>d__.db = db;
			<DeleteOne>d__.id = id;
			<DeleteOne>d__.<>1__state = -1;
			<DeleteOne>d__.<>t__builder.Start<DungeonEntranceRecord.<DeleteOne>d__26>(ref <DeleteOne>d__);
			return <DeleteOne>d__.<>t__builder.Task;
		}

		// Token: 0x06001F14 RID: 7956 RVA: 0x0011E0E0 File Offset: 0x0011C2E0
		public Task<bool> UpdateRecordAsync(IMongoDatabase db)
		{
			DungeonEntranceRecord.<UpdateRecordAsync>d__27 <UpdateRecordAsync>d__;
			<UpdateRecordAsync>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<UpdateRecordAsync>d__.<>4__this = this;
			<UpdateRecordAsync>d__.db = db;
			<UpdateRecordAsync>d__.<>1__state = -1;
			<UpdateRecordAsync>d__.<>t__builder.Start<DungeonEntranceRecord.<UpdateRecordAsync>d__27>(ref <UpdateRecordAsync>d__);
			return <UpdateRecordAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06001F15 RID: 7957 RVA: 0x0011E12C File Offset: 0x0011C32C
		public bool UpdateRecord(IMongoDatabase db)
		{
			IMongoCollection<DungeonEntranceRecord> collection = db.GetCollection<DungeonEntranceRecord>(DungeonEntranceRecord.kTableName, null);
			FilterDefinition<DungeonEntranceRecord> filter = Builders<DungeonEntranceRecord>.Filter.Eq<string>((DungeonEntranceRecord cr) => cr.Id, this.Id);
			UpdateDefinition<DungeonEntranceRecord> update = Builders<DungeonEntranceRecord>.Update.Set<DateTime>((DungeonEntranceRecord cr) => cr.ActivationTime, this.ActivationTime).Set((DungeonEntranceRecord cr) => cr.DeactivationTime, this.DeactivationTime).Set((DungeonEntranceRecord cr) => cr.Status, this.Status);
			return collection.UpdateOne(filter, update, null, default(CancellationToken)).ModifiedCount > 0L;
		}

		// Token: 0x06001F16 RID: 7958 RVA: 0x0011E26C File Offset: 0x0011C46C
		public Task<bool> UpdateLocationAsync(IMongoDatabase db)
		{
			DungeonEntranceRecord.<UpdateLocationAsync>d__29 <UpdateLocationAsync>d__;
			<UpdateLocationAsync>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<UpdateLocationAsync>d__.<>4__this = this;
			<UpdateLocationAsync>d__.db = db;
			<UpdateLocationAsync>d__.<>1__state = -1;
			<UpdateLocationAsync>d__.<>t__builder.Start<DungeonEntranceRecord.<UpdateLocationAsync>d__29>(ref <UpdateLocationAsync>d__);
			return <UpdateLocationAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06001F17 RID: 7959 RVA: 0x0011E2B8 File Offset: 0x0011C4B8
		public Task DeleteRecord(IMongoDatabase db)
		{
			DungeonEntranceRecord.<DeleteRecord>d__30 <DeleteRecord>d__;
			<DeleteRecord>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<DeleteRecord>d__.<>4__this = this;
			<DeleteRecord>d__.db = db;
			<DeleteRecord>d__.<>1__state = -1;
			<DeleteRecord>d__.<>t__builder.Start<DungeonEntranceRecord.<DeleteRecord>d__30>(ref <DeleteRecord>d__);
			return <DeleteRecord>d__.<>t__builder.Task;
		}

		// Token: 0x04002468 RID: 9320
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string Id;

		// Token: 0x04002469 RID: 9321
		public int DungeonZoneId;

		// Token: 0x0400246A RID: 9322
		public int OverworldZoneId;

		// Token: 0x0400246B RID: 9323
		public int ZonePointIndex;

		// Token: 0x0400246C RID: 9324
		public DateTime ActivationTime;

		// Token: 0x0400246D RID: 9325
		[BsonIgnoreIfNull]
		public DateTime? DeactivationTime;

		// Token: 0x0400246E RID: 9326
		[BsonRepresentation(BsonType.Int32)]
		public DungeonEntranceStatus Status;

		// Token: 0x0400246F RID: 9327
		[BsonIgnoreIfNull]
		public CharacterLocation Location;

		// Token: 0x04002470 RID: 9328
		[BsonRepresentation(BsonType.Int32)]
		public SpawnTier Tier;

		// Token: 0x04002471 RID: 9329
		[BsonIgnore]
		private ZoneRecord m_dungeonZone;

		// Token: 0x04002472 RID: 9330
		[BsonIgnore]
		private ZoneRecord m_overworldZone;

		// Token: 0x04002473 RID: 9331
		private static string kTableName = "dungeon_entrances";
	}
}

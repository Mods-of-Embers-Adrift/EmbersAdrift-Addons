using System;
using System.Collections.Generic;
using System.Threading;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using NetStack.Serialization;
using Newtonsoft.Json;
using SoL.Game;
using SoL.Game.Objects.Containers;

namespace SoL.Networking.Database
{
	// Token: 0x02000443 RID: 1091
	public class CorpseRecord : INetworkSerializable
	{
		// Token: 0x06001EFA RID: 7930 RVA: 0x0011DA5C File Offset: 0x0011BC5C
		public bool IsExpired()
		{
			return (DateTime.UtcNow - this.TimeCreated).TotalHours > 4.0;
		}

		// Token: 0x06001EFB RID: 7931 RVA: 0x0011DA8C File Offset: 0x0011BC8C
		public static CorpseRecord Load(IMongoDatabase db, CharacterRecord record)
		{
			IMongoCollection<CorpseRecord> collection = db.GetCollection<CorpseRecord>(CorpseRecord.kTableName, null);
			FilterDefinition<CorpseRecord> filter = Builders<CorpseRecord>.Filter.Eq<string>((CorpseRecord cr) => cr.Id, record.Id);
			return collection.Find(filter, null).FirstOrDefault(default(CancellationToken));
		}

		// Token: 0x06001EFC RID: 7932 RVA: 0x0011DB04 File Offset: 0x0011BD04
		public static IEnumerable<CorpseRecord> LoadForZone(IMongoDatabase db, int zoneId)
		{
			IMongoCollection<CorpseRecord> collection = db.GetCollection<CorpseRecord>(CorpseRecord.kTableName, null);
			FilterDefinition<CorpseRecord> filter = Builders<CorpseRecord>.Filter.Eq<int>((CorpseRecord cr) => cr.Location.ZoneId, zoneId);
			return collection.Find(filter, null).ToList(default(CancellationToken));
		}

		// Token: 0x06001EFD RID: 7933 RVA: 0x00056EBE File Offset: 0x000550BE
		public static IEnumerable<CorpseRecord> LoadForZone(IMongoDatabase db, ZoneId zoneId)
		{
			return CorpseRecord.LoadForZone(db, (int)zoneId);
		}

		// Token: 0x06001EFE RID: 7934 RVA: 0x0011DB84 File Offset: 0x0011BD84
		public static IEnumerable<CorpseRecord> LoadAll(IMongoDatabase db)
		{
			return db.GetCollection<CorpseRecord>(CorpseRecord.kTableName, null).Find(FilterDefinition<CorpseRecord>.Empty, null).ToList(default(CancellationToken));
		}

		// Token: 0x06001EFF RID: 7935 RVA: 0x0011DBB8 File Offset: 0x0011BDB8
		public static CorpseRecord CreateCorpse(IMongoDatabase db, CharacterRecord record)
		{
			if (record.CorpseCreationTime != null)
			{
				return null;
			}
			ContainerRecord containerRecord;
			if (!record.Storage.TryGetValue(ContainerType.Inventory, out containerRecord) || containerRecord.IsEmpty())
			{
				return null;
			}
			CorpseRecord corpseRecord = new CorpseRecord
			{
				Id = record.Id,
				TimeCreated = DateTime.UtcNow,
				CharacterName = record.Name,
				Location = record.Location.Clone()
			};
			db.GetCollection<CorpseRecord>(CorpseRecord.kTableName, null).InsertOne(corpseRecord, null, default(CancellationToken));
			record.CorpseCreationTime = new DateTime?(corpseRecord.TimeCreated);
			record.UpdateCorpseCreationTime(ExternalGameDatabase.Database);
			return corpseRecord;
		}

		// Token: 0x06001F00 RID: 7936 RVA: 0x0011DC68 File Offset: 0x0011BE68
		public bool DeleteCorpse(IMongoDatabase db, CharacterRecord record)
		{
			IMongoCollection<CorpseRecord> collection = db.GetCollection<CorpseRecord>(CorpseRecord.kTableName, null);
			FilterDefinition<CorpseRecord> filter = Builders<CorpseRecord>.Filter.Eq<string>((CorpseRecord cr) => cr.Id, this.Id);
			bool flag = collection.DeleteOne(filter, default(CancellationToken)).DeletedCount > 0L;
			if (flag)
			{
				record.CorpseCreationTime = null;
				record.UpdateCorpseCreationTime(ExternalGameDatabase.Database);
			}
			return flag;
		}

		// Token: 0x06001F01 RID: 7937 RVA: 0x00056EC7 File Offset: 0x000550C7
		public BitBuffer PackData(BitBuffer buffer)
		{
			buffer.AddString(this.Id);
			buffer.AddDateTime(this.TimeCreated);
			buffer.AddString(this.CharacterName);
			this.Location.PackData(buffer);
			return buffer;
		}

		// Token: 0x06001F02 RID: 7938 RVA: 0x00056EFE File Offset: 0x000550FE
		public BitBuffer ReadData(BitBuffer buffer)
		{
			this.Id = buffer.ReadString();
			this.TimeCreated = buffer.ReadDateTime();
			this.CharacterName = buffer.ReadString();
			this.Location = new CharacterLocation();
			this.Location.ReadData(buffer);
			return buffer;
		}

		// Token: 0x0400245D RID: 9309
		public const float kCorpseExpirationTime = 4f;

		// Token: 0x0400245E RID: 9310
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		[JsonProperty(PropertyName = "_id")]
		public string Id;

		// Token: 0x0400245F RID: 9311
		[BsonDateTimeOptions(Representation = BsonType.String)]
		public DateTime TimeCreated;

		// Token: 0x04002460 RID: 9312
		public string CharacterName;

		// Token: 0x04002461 RID: 9313
		public CharacterLocation Location;

		// Token: 0x04002462 RID: 9314
		private static string kTableName = "corpses";
	}
}

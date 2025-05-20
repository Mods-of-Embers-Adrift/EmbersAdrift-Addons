using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Newtonsoft.Json;
using SoL.Game;

namespace SoL.Networking.Database
{
	// Token: 0x02000470 RID: 1136
	[BsonIgnoreExtraElements]
	public class ZoneRecord
	{
		// Token: 0x06001FD6 RID: 8150 RVA: 0x00120DB4 File Offset: 0x0011EFB4
		public static IEnumerable<ZoneRecord> LoadAll(IMongoDatabase db)
		{
			return db.GetCollection<ZoneRecord>(ZoneRecord.kTableName, null).Find(FilterDefinition<ZoneRecord>.Empty, null).ToList(default(CancellationToken));
		}

		// Token: 0x06001FD7 RID: 8151 RVA: 0x00120DE8 File Offset: 0x0011EFE8
		public static Task<List<ZoneRecord>> LoadAllAsync(IMongoDatabase db)
		{
			ZoneRecord.<LoadAllAsync>d__13 <LoadAllAsync>d__;
			<LoadAllAsync>d__.<>t__builder = AsyncTaskMethodBuilder<List<ZoneRecord>>.Create();
			<LoadAllAsync>d__.db = db;
			<LoadAllAsync>d__.<>1__state = -1;
			<LoadAllAsync>d__.<>t__builder.Start<ZoneRecord.<LoadAllAsync>d__13>(ref <LoadAllAsync>d__);
			return <LoadAllAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06001FD8 RID: 8152 RVA: 0x00120E2C File Offset: 0x0011F02C
		public static ZoneRecord LoadZoneId(IMongoDatabase db, ZoneId zoneId)
		{
			IMongoCollection<ZoneRecord> collection = db.GetCollection<ZoneRecord>(ZoneRecord.kTableName, null);
			FilterDefinition<ZoneRecord> filter = Builders<ZoneRecord>.Filter.Eq<int>((ZoneRecord zr) => zr.ZoneId, (int)zoneId);
			return collection.Find(filter, null).FirstOrDefault(default(CancellationToken));
		}

		// Token: 0x04002545 RID: 9541
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string Id;

		// Token: 0x04002546 RID: 9542
		public string SceneName;

		// Token: 0x04002547 RID: 9543
		public string DisplayName;

		// Token: 0x04002548 RID: 9544
		public int ZoneId;

		// Token: 0x04002549 RID: 9545
		public int WorldId;

		// Token: 0x0400254A RID: 9546
		public int DefaultPopulationThreshold = 64;

		// Token: 0x0400254B RID: 9547
		public bool KeepAlive;

		// Token: 0x0400254C RID: 9548
		[BsonElement("Flags")]
		[JsonProperty(PropertyName = "Flags")]
		public int Flags;

		// Token: 0x0400254D RID: 9549
		public string Address;

		// Token: 0x0400254E RID: 9550
		[BsonIgnore]
		[JsonIgnore]
		public int Port;

		// Token: 0x0400254F RID: 9551
		[BsonIgnore]
		[JsonIgnore]
		public int InstanceId;

		// Token: 0x04002550 RID: 9552
		private static string kTableName = "zones";
	}
}

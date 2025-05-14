using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using NetStack.Serialization;
using Newtonsoft.Json;
using SoL.Networking;
using SoL.Networking.Database;

namespace SoL.Game.Objects.Containers
{
	// Token: 0x02000A17 RID: 2583
	public class LearnableContainerRecord : INetworkSerializable
	{
		// Token: 0x06004F12 RID: 20242 RVA: 0x00075448 File Offset: 0x00073648
		public string GetId()
		{
			if (!this.Type.RequiresUniqueId())
			{
				return this.Type.ToString();
			}
			return this.ContainerId;
		}

		// Token: 0x06004F13 RID: 20243 RVA: 0x0007546F File Offset: 0x0007366F
		private bool HasContents()
		{
			return this.LearnableIds != null && this.LearnableIds.Count > 0;
		}

		// Token: 0x06004F14 RID: 20244 RVA: 0x00075489 File Offset: 0x00073689
		public bool IsEmpty()
		{
			return !this.HasContents();
		}

		// Token: 0x06004F15 RID: 20245 RVA: 0x00075494 File Offset: 0x00073694
		public void ClearContents()
		{
			this.LearnableIds.Clear();
		}

		// Token: 0x06004F16 RID: 20246 RVA: 0x001C49A8 File Offset: 0x001C2BA8
		public static LearnableContainerRecord Load(IMongoDatabase db, string userId, string containerId)
		{
			IMongoCollection<LearnableContainerRecord> collection = db.GetCollection<LearnableContainerRecord>("learnables", null);
			FilterDefinitionBuilder<LearnableContainerRecord> filter = Builders<LearnableContainerRecord>.Filter;
			FilterDefinition<LearnableContainerRecord> filter2 = filter.Eq<string>((LearnableContainerRecord cr) => cr.UserId, userId) & filter.Eq<string>((LearnableContainerRecord cr) => cr.ContainerId, containerId);
			return collection.Find(filter2, null).FirstOrDefault(default(CancellationToken));
		}

		// Token: 0x06004F17 RID: 20247 RVA: 0x001C4A5C File Offset: 0x001C2C5C
		public static LearnableContainerRecord Load(IMongoDatabase db, string id)
		{
			IMongoCollection<LearnableContainerRecord> collection = db.GetCollection<LearnableContainerRecord>("learnables", null);
			FilterDefinition<LearnableContainerRecord> filter = Builders<LearnableContainerRecord>.Filter.Eq<string>((LearnableContainerRecord cr) => cr.Id, id);
			return collection.Find(filter, null).FirstOrDefault(default(CancellationToken));
		}

		// Token: 0x06004F18 RID: 20248 RVA: 0x001C4ACC File Offset: 0x001C2CCC
		public static IEnumerable<LearnableContainerRecord> LoadAll(IMongoDatabase db)
		{
			return db.GetCollection<LearnableContainerRecord>("learnables", null).Find(FilterDefinition<LearnableContainerRecord>.Empty, null).ToList(default(CancellationToken));
		}

		// Token: 0x06004F19 RID: 20249 RVA: 0x001C4B00 File Offset: 0x001C2D00
		public void StoreNew(IMongoDatabase db)
		{
			db.GetCollection<LearnableContainerRecord>("learnables", null).InsertOne(this, null, default(CancellationToken));
		}

		// Token: 0x06004F1A RID: 20250 RVA: 0x001C4B2C File Offset: 0x001C2D2C
		public void StoreNewAsync(IMongoDatabase db)
		{
			LearnableContainerRecord.<StoreNewAsync>d__14 <StoreNewAsync>d__;
			<StoreNewAsync>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<StoreNewAsync>d__.<>4__this = this;
			<StoreNewAsync>d__.db = db;
			<StoreNewAsync>d__.<>1__state = -1;
			<StoreNewAsync>d__.<>t__builder.Start<LearnableContainerRecord.<StoreNewAsync>d__14>(ref <StoreNewAsync>d__);
		}

		// Token: 0x06004F1B RID: 20251 RVA: 0x001C4B6C File Offset: 0x001C2D6C
		public void UpdateRecord(IMongoDatabase db)
		{
			IMongoCollection<LearnableContainerRecord> collection = db.GetCollection<LearnableContainerRecord>("learnables", null);
			FilterDefinition<LearnableContainerRecord> filter = Builders<LearnableContainerRecord>.Filter.Eq<string>((LearnableContainerRecord sr) => sr.Id, this.Id);
			if (this.IsEmpty())
			{
				collection.DeleteOne(filter, default(CancellationToken));
				return;
			}
			collection.ReplaceOne(filter, this, ExternalGameDatabase.ReplaceOptions_Upstart, default(CancellationToken));
		}

		// Token: 0x06004F1C RID: 20252 RVA: 0x001C4BFC File Offset: 0x001C2DFC
		public BitBuffer PackData(BitBuffer buffer)
		{
			buffer.AddEnum(this.Type);
			if (this.Type.RequiresUniqueId())
			{
				buffer.AddString(this.ContainerId);
			}
			buffer.AddInt(this.LearnableIds.Count);
			for (int i = 0; i < this.LearnableIds.Count; i++)
			{
				buffer.AddUniqueId(this.LearnableIds[i]);
			}
			return buffer;
		}

		// Token: 0x06004F1D RID: 20253 RVA: 0x001C4C6C File Offset: 0x001C2E6C
		public BitBuffer ReadData(BitBuffer buffer)
		{
			this.Type = buffer.ReadEnum<ContainerType>();
			if (this.Type.RequiresUniqueId())
			{
				this.ContainerId = buffer.ReadString();
			}
			int num = buffer.ReadInt();
			this.LearnableIds = new List<UniqueId>(num);
			for (int i = 0; i < num; i++)
			{
				this.LearnableIds.Add(buffer.ReadUniqueId());
			}
			return buffer;
		}

		// Token: 0x040047DB RID: 18395
		[BsonId]
		[BsonIgnoreIfNull]
		[JsonProperty(PropertyName = "_id", NullValueHandling = NullValueHandling.Ignore)]
		public string Id;

		// Token: 0x040047DC RID: 18396
		[BsonIgnoreIfNull]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string ContainerId;

		// Token: 0x040047DD RID: 18397
		public ContainerType Type;

		// Token: 0x040047DE RID: 18398
		[BsonIgnoreIfNull]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string UserId;

		// Token: 0x040047DF RID: 18399
		[BsonIgnoreIfNull]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public List<UniqueId> LearnableIds;

		// Token: 0x040047E0 RID: 18400
		private const string kTableName = "learnables";
	}
}

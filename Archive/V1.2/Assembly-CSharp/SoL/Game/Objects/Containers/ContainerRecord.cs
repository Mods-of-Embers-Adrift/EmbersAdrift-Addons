using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using NetStack.Serialization;
using Newtonsoft.Json;
using SoL.Game.Objects.Archetypes;
using SoL.Networking;
using SoL.Networking.Database;
using SoL.Utilities;

namespace SoL.Game.Objects.Containers
{
	// Token: 0x02000A0C RID: 2572
	[Serializable]
	public class ContainerRecord : INetworkSerializable
	{
		// Token: 0x06004E88 RID: 20104 RVA: 0x00075103 File Offset: 0x00073303
		public string GetId()
		{
			if (!this.Type.RequiresUniqueId())
			{
				return this.Type.ToString();
			}
			return this.ContainerId;
		}

		// Token: 0x06004E89 RID: 20105 RVA: 0x0007512A File Offset: 0x0007332A
		private bool HasContents()
		{
			return this.Instances != null && this.Instances.Count > 0;
		}

		// Token: 0x06004E8A RID: 20106 RVA: 0x001C3B0C File Offset: 0x001C1D0C
		private bool HasCurrency()
		{
			ulong? currency = this.Currency;
			ulong num = 0UL;
			return currency.GetValueOrDefault() > num & currency != null;
		}

		// Token: 0x06004E8B RID: 20107 RVA: 0x00075144 File Offset: 0x00073344
		public bool IsEmpty()
		{
			return !this.HasContents() && !this.HasCurrency();
		}

		// Token: 0x06004E8C RID: 20108 RVA: 0x00075159 File Offset: 0x00073359
		public void ClearContents()
		{
			this.Instances.Clear();
		}

		// Token: 0x06004E8D RID: 20109 RVA: 0x00075166 File Offset: 0x00073366
		public void ClearCurrency()
		{
			this.Currency = new ulong?(0UL);
		}

		// Token: 0x06004E8E RID: 20110 RVA: 0x00075175 File Offset: 0x00073375
		public void ClearContentsAndCurrency()
		{
			this.ClearContents();
			this.ClearCurrency();
		}

		// Token: 0x06004E8F RID: 20111 RVA: 0x001C3B38 File Offset: 0x001C1D38
		public int? GetFirstAvailableIndex(int maxCapacity)
		{
			if (this.Instances == null || this.Instances.Count >= maxCapacity)
			{
				return null;
			}
			PlayerCollectionController.CachedIndexes.Clear();
			for (int i = 0; i < this.Instances.Count; i++)
			{
				if (this.Instances[i] != null && this.Instances[i].Index > -1)
				{
					PlayerCollectionController.CachedIndexes.Add(this.Instances[i].Index);
				}
			}
			int? num = null;
			int num2 = 0;
			for (int j = 0; j < maxCapacity; j++)
			{
				if (j < this.Instances.Count && this.Instances[j] != null && this.Instances[j].Index > num2)
				{
					num2 = this.Instances[j].Index;
				}
				if (!PlayerCollectionController.CachedIndexes.Contains(j))
				{
					num = new int?(j);
					break;
				}
			}
			PlayerCollectionController.CachedIndexes.Clear();
			return new int?((num != null) ? num.Value : (num2 + 1));
		}

		// Token: 0x06004E90 RID: 20112 RVA: 0x0004475B File Offset: 0x0004295B
		public void ReturnInstancesToPoolAndNullifyList()
		{
		}

		// Token: 0x06004E91 RID: 20113 RVA: 0x0004475B File Offset: 0x0004295B
		public void RemoveContainerInstanceReferences()
		{
		}

		// Token: 0x06004E92 RID: 20114 RVA: 0x001C3C60 File Offset: 0x001C1E60
		public void CleanupReferences()
		{
			if (this.Instances != null)
			{
				for (int i = 0; i < this.Instances.Count; i++)
				{
					if (this.Instances[i] != null)
					{
						StaticPool<ArchetypeInstance>.ReturnToPool(this.Instances[i]);
					}
				}
				this.Instances.Clear();
			}
		}

		// Token: 0x06004E93 RID: 20115 RVA: 0x001C3CB8 File Offset: 0x001C1EB8
		public static ContainerRecord Load(IMongoDatabase db, string userId, string containerId)
		{
			GlobalCounters.ContainerLoads += 1U;
			IMongoCollection<ContainerRecord> collection = db.GetCollection<ContainerRecord>("storage", null);
			FilterDefinitionBuilder<ContainerRecord> filter = Builders<ContainerRecord>.Filter;
			FilterDefinition<ContainerRecord> filter2 = filter.Eq<string>((ContainerRecord cr) => cr.UserId, userId) & filter.Eq<string>((ContainerRecord cr) => cr.ContainerId, containerId);
			return collection.Find(filter2, null).FirstOrDefault(default(CancellationToken));
		}

		// Token: 0x06004E94 RID: 20116 RVA: 0x001C3D78 File Offset: 0x001C1F78
		public static ContainerRecord Load(IMongoDatabase db, string id)
		{
			GlobalCounters.ContainerLoads += 1U;
			IMongoCollection<ContainerRecord> collection = db.GetCollection<ContainerRecord>("storage", null);
			FilterDefinition<ContainerRecord> filter = Builders<ContainerRecord>.Filter.Eq<string>((ContainerRecord cr) => cr.Id, id);
			return collection.Find(filter, null).FirstOrDefault(default(CancellationToken));
		}

		// Token: 0x06004E95 RID: 20117 RVA: 0x001C3DF4 File Offset: 0x001C1FF4
		public static IEnumerable<ContainerRecord> LoadAll(IMongoDatabase db)
		{
			return db.GetCollection<ContainerRecord>("storage", null).Find(FilterDefinition<ContainerRecord>.Empty, null).ToList(default(CancellationToken));
		}

		// Token: 0x06004E96 RID: 20118 RVA: 0x001C3E28 File Offset: 0x001C2028
		public void StoreNew(IMongoDatabase db)
		{
			GlobalCounters.ContainerSaves += 1U;
			db.GetCollection<ContainerRecord>("storage", null).InsertOne(this, null, default(CancellationToken));
		}

		// Token: 0x06004E97 RID: 20119 RVA: 0x001C3E60 File Offset: 0x001C2060
		public void StoreNewAsync(IMongoDatabase db)
		{
			ContainerRecord.<StoreNewAsync>d__24 <StoreNewAsync>d__;
			<StoreNewAsync>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<StoreNewAsync>d__.<>4__this = this;
			<StoreNewAsync>d__.db = db;
			<StoreNewAsync>d__.<>1__state = -1;
			<StoreNewAsync>d__.<>t__builder.Start<ContainerRecord.<StoreNewAsync>d__24>(ref <StoreNewAsync>d__);
		}

		// Token: 0x06004E98 RID: 20120 RVA: 0x001C3EA0 File Offset: 0x001C20A0
		public void UpdateRecord(IMongoDatabase db)
		{
			GlobalCounters.ContainerSaves += 1U;
			IMongoCollection<ContainerRecord> collection = db.GetCollection<ContainerRecord>("storage", null);
			FilterDefinition<ContainerRecord> filter = Builders<ContainerRecord>.Filter.Eq<string>((ContainerRecord sr) => sr.Id, this.Id);
			if (this.IsEmpty() && this.ExpansionsPurchased == null)
			{
				collection.DeleteOne(filter, default(CancellationToken));
				return;
			}
			collection.ReplaceOne(filter, this, ExternalGameDatabase.ReplaceOptions_Upstart, default(CancellationToken));
		}

		// Token: 0x06004E99 RID: 20121 RVA: 0x001C3F4C File Offset: 0x001C214C
		public BitBuffer PackData(BitBuffer buffer)
		{
			buffer.AddEnum(this.Type);
			if (this.Type.RequiresUniqueId())
			{
				buffer.AddString(this.ContainerId);
			}
			buffer.AddNullableUlong(this.Currency);
			buffer.AddNullableInt(this.ExpansionsPurchased);
			buffer.AddInt(this.Instances.Count);
			for (int i = 0; i < this.Instances.Count; i++)
			{
				this.Instances[i].PackData(buffer);
			}
			bool flag = this.Toggles != null;
			buffer.AddBool(flag);
			if (flag)
			{
				buffer.AddInt(this.Toggles.Length);
				for (int j = 0; j < this.Toggles.Length; j++)
				{
					buffer.AddBool(this.Toggles[j]);
				}
			}
			return buffer;
		}

		// Token: 0x06004E9A RID: 20122 RVA: 0x001C401C File Offset: 0x001C221C
		public BitBuffer ReadData(BitBuffer buffer)
		{
			this.Type = buffer.ReadEnum<ContainerType>();
			if (this.Type.RequiresUniqueId())
			{
				this.ContainerId = buffer.ReadString();
			}
			this.Currency = buffer.ReadNullableUlong();
			this.ExpansionsPurchased = buffer.ReadNullableInt();
			int num = buffer.ReadInt();
			this.Instances = new List<ArchetypeInstance>(num);
			for (int i = 0; i < num; i++)
			{
				ArchetypeInstance fromPool = StaticPool<ArchetypeInstance>.GetFromPool();
				fromPool.ReadData(buffer);
				this.Instances.Add(fromPool);
			}
			if (buffer.ReadBool())
			{
				num = buffer.ReadInt();
				this.Toggles = new bool[num];
				for (int j = 0; j < num; j++)
				{
					this.Toggles[j] = buffer.ReadBool();
				}
			}
			return buffer;
		}

		// Token: 0x0400479C RID: 18332
		[BsonId]
		[BsonIgnoreIfNull]
		[JsonProperty(PropertyName = "_id", NullValueHandling = NullValueHandling.Ignore)]
		public string Id;

		// Token: 0x0400479D RID: 18333
		[BsonIgnoreIfNull]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string ContainerId;

		// Token: 0x0400479E RID: 18334
		public ContainerType Type;

		// Token: 0x0400479F RID: 18335
		[BsonIgnoreIfNull]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string UserId;

		// Token: 0x040047A0 RID: 18336
		[BsonIgnoreIfNull]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public ulong? Currency;

		// Token: 0x040047A1 RID: 18337
		[BsonIgnoreIfNull]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public int? ExpansionsPurchased;

		// Token: 0x040047A2 RID: 18338
		[BsonIgnoreIfNull]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public List<ArchetypeInstance> Instances;

		// Token: 0x040047A3 RID: 18339
		[BsonIgnoreIfNull]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public bool[] Toggles;

		// Token: 0x040047A4 RID: 18340
		private const string kTableName = "storage";
	}
}

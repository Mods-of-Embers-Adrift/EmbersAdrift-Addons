using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using NetStack.Serialization;
using Newtonsoft.Json;
using SoL.Game.Objects.Archetypes;
using SoL.Utilities;

namespace SoL.Networking.Database
{
	// Token: 0x0200041F RID: 1055
	[BsonIgnoreExtraElements]
	public class AuctionRecord : INetworkSerializable, IEquatable<AuctionRecord>
	{
		// Token: 0x17000662 RID: 1634
		// (get) Token: 0x06001E62 RID: 7778 RVA: 0x0011A7A0 File Offset: 0x001189A0
		public string CachedItemName
		{
			get
			{
				if (string.IsNullOrEmpty(this.m_cachedItemName) && this.Instance != null && this.Instance.Archetype)
				{
					this.m_cachedItemName = this.Instance.Archetype.GetModifiedDisplayName(this.Instance);
				}
				return this.m_cachedItemName;
			}
		}

		// Token: 0x06001E63 RID: 7779 RVA: 0x00056A9E File Offset: 0x00054C9E
		public void ClearBuyer()
		{
			this.BuyerUserId = null;
			this.BuyerCharacterId = null;
			this.BuyerName = null;
		}

		// Token: 0x17000663 RID: 1635
		// (get) Token: 0x06001E64 RID: 7780 RVA: 0x0011A7F8 File Offset: 0x001189F8
		[BsonIgnore]
		[JsonIgnore]
		private FilterDefinition<AuctionRecord> IdFilter
		{
			get
			{
				if (this.m_idFilter == null)
				{
					this.m_idFilter = Builders<AuctionRecord>.Filter.Eq<string>((AuctionRecord cr) => cr.Id, this.Id);
				}
				return this.m_idFilter;
			}
		}

		// Token: 0x06001E65 RID: 7781 RVA: 0x0011A860 File Offset: 0x00118A60
		public static IEnumerable<AuctionRecord> LoadAllForZone(IMongoDatabase db, int zid)
		{
			IMongoCollection<AuctionRecord> collection = db.GetCollection<AuctionRecord>(AuctionRecord.kTableName, null);
			FilterDefinition<AuctionRecord> filter = Builders<AuctionRecord>.Filter.Eq<int>((AuctionRecord ar) => ar.ZoneId, zid);
			return collection.Find(filter, null).ToList(default(CancellationToken));
		}

		// Token: 0x06001E66 RID: 7782 RVA: 0x0011A8D0 File Offset: 0x00118AD0
		public void StoreNew(IMongoDatabase db)
		{
			db.GetCollection<AuctionRecord>(AuctionRecord.kTableName, null).InsertOne(this, null, default(CancellationToken));
		}

		// Token: 0x06001E67 RID: 7783 RVA: 0x0011A8FC File Offset: 0x00118AFC
		public bool UpdateRecordForBid(IMongoDatabase db)
		{
			IMongoCollection<AuctionRecord> collection = db.GetCollection<AuctionRecord>(AuctionRecord.kTableName, null);
			UpdateDefinition<AuctionRecord> update = Builders<AuctionRecord>.Update.Set<DateTime>((AuctionRecord tp) => tp.Updated, DateTime.UtcNow).Set((AuctionRecord tp) => tp.BuyerUserId, this.BuyerUserId).Set((AuctionRecord tp) => tp.BuyerCharacterId, this.BuyerCharacterId).Set((AuctionRecord tp) => tp.BuyerName, this.BuyerName).Set((AuctionRecord tp) => tp.CurrentBid, this.CurrentBid).Set((AuctionRecord tp) => tp.BidCount, this.BidCount);
			return collection.UpdateOne(this.IdFilter, update, null, default(CancellationToken)).ModifiedCount > 0L;
		}

		// Token: 0x06001E68 RID: 7784 RVA: 0x0011AAB8 File Offset: 0x00118CB8
		public bool DeleteRecord(IMongoDatabase db)
		{
			return db.GetCollection<AuctionRecord>(AuctionRecord.kTableName, null).DeleteOne(this.IdFilter, default(CancellationToken)).DeletedCount == 1L;
		}

		// Token: 0x06001E69 RID: 7785 RVA: 0x0011AAF0 File Offset: 0x00118CF0
		public Task<bool> DeleteRecordAsync(IMongoDatabase db)
		{
			AuctionRecord.<DeleteRecordAsync>d__29 <DeleteRecordAsync>d__;
			<DeleteRecordAsync>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<DeleteRecordAsync>d__.<>4__this = this;
			<DeleteRecordAsync>d__.db = db;
			<DeleteRecordAsync>d__.<>1__state = -1;
			<DeleteRecordAsync>d__.<>t__builder.Start<AuctionRecord.<DeleteRecordAsync>d__29>(ref <DeleteRecordAsync>d__);
			return <DeleteRecordAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06001E6A RID: 7786 RVA: 0x0011AB3C File Offset: 0x00118D3C
		public BitBuffer PackData(BitBuffer buffer)
		{
			buffer.AddString(this.Id);
			buffer.AddDateTime(this.Created);
			buffer.AddDateTime(this.Expiration);
			buffer.AddDateTime(this.Updated);
			buffer.AddString(this.SellerCharacterId);
			buffer.AddString(this.SellerName);
			buffer.AddNullableString(this.BuyerCharacterId);
			buffer.AddNullableUlong(this.BuyNowPrice);
			buffer.AddNullableUlong(this.CurrentBid);
			buffer.AddInt(this.BidCount);
			this.Instance.PackData(buffer);
			return buffer;
		}

		// Token: 0x06001E6B RID: 7787 RVA: 0x0011ABDC File Offset: 0x00118DDC
		public BitBuffer ReadData(BitBuffer buffer)
		{
			this.Id = buffer.ReadString();
			this.Created = buffer.ReadDateTime();
			this.Expiration = buffer.ReadDateTime();
			this.Updated = buffer.ReadDateTime();
			this.SellerCharacterId = buffer.ReadString();
			this.SellerName = buffer.ReadString();
			this.BuyerCharacterId = buffer.ReadNullableString();
			this.BuyNowPrice = buffer.ReadNullableUlong();
			this.CurrentBid = buffer.ReadNullableUlong();
			this.BidCount = buffer.ReadInt();
			this.Instance = StaticPool<ArchetypeInstance>.GetFromPool();
			this.Instance.ReadData(buffer);
			return buffer;
		}

		// Token: 0x06001E6C RID: 7788 RVA: 0x00056AB5 File Offset: 0x00054CB5
		public bool Equals(AuctionRecord other)
		{
			return other != null && (this == other || this.Id == other.Id);
		}

		// Token: 0x06001E6D RID: 7789 RVA: 0x00056AD3 File Offset: 0x00054CD3
		public override bool Equals(object obj)
		{
			return obj != null && (this == obj || (!(obj.GetType() != base.GetType()) && this.Equals((AuctionRecord)obj)));
		}

		// Token: 0x06001E6E RID: 7790 RVA: 0x00056B01 File Offset: 0x00054D01
		public override int GetHashCode()
		{
			if (this.Id == null)
			{
				return 0;
			}
			return this.Id.GetHashCode();
		}

		// Token: 0x040023AD RID: 9133
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		[JsonProperty(PropertyName = "_id")]
		public string Id;

		// Token: 0x040023AE RID: 9134
		public int ZoneId;

		// Token: 0x040023AF RID: 9135
		[BsonDateTimeOptions(Representation = BsonType.String)]
		public DateTime Created;

		// Token: 0x040023B0 RID: 9136
		[BsonDateTimeOptions(Representation = BsonType.String)]
		public DateTime Expiration;

		// Token: 0x040023B1 RID: 9137
		[BsonDateTimeOptions(Representation = BsonType.String)]
		public DateTime Updated;

		// Token: 0x040023B2 RID: 9138
		public ulong Deposit;

		// Token: 0x040023B3 RID: 9139
		public string SellerUserId;

		// Token: 0x040023B4 RID: 9140
		public string SellerCharacterId;

		// Token: 0x040023B5 RID: 9141
		public string SellerName;

		// Token: 0x040023B6 RID: 9142
		public bool SellerIsSubscriber;

		// Token: 0x040023B7 RID: 9143
		public string BuyerUserId;

		// Token: 0x040023B8 RID: 9144
		public string BuyerCharacterId;

		// Token: 0x040023B9 RID: 9145
		public string BuyerName;

		// Token: 0x040023BA RID: 9146
		public ulong? BuyNowPrice;

		// Token: 0x040023BB RID: 9147
		public ulong? CurrentBid;

		// Token: 0x040023BC RID: 9148
		public int BidCount;

		// Token: 0x040023BD RID: 9149
		public ArchetypeInstance Instance;

		// Token: 0x040023BE RID: 9150
		[NonSerialized]
		private string m_cachedItemName;

		// Token: 0x040023BF RID: 9151
		private static string kTableName = "auctions";

		// Token: 0x040023C0 RID: 9152
		[BsonIgnore]
		[JsonIgnore]
		private FilterDefinition<AuctionRecord> m_idFilter;
	}
}

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
using SoL.Game.Objects.Archetypes;
using SoL.Game.Settings;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Networking.Database
{
	// Token: 0x0200045D RID: 1117
	[BsonIgnoreExtraElements]
	public class Mail
	{
		// Token: 0x1700068A RID: 1674
		// (get) Token: 0x06001F69 RID: 8041 RVA: 0x000571BC File Offset: 0x000553BC
		// (set) Token: 0x06001F6A RID: 8042 RVA: 0x000571C4 File Offset: 0x000553C4
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string _id { get; set; }

		// Token: 0x1700068B RID: 1675
		// (get) Token: 0x06001F6B RID: 8043 RVA: 0x000571CD File Offset: 0x000553CD
		// (set) Token: 0x06001F6C RID: 8044 RVA: 0x000571D5 File Offset: 0x000553D5
		public MailType Type { get; set; }

		// Token: 0x1700068C RID: 1676
		// (get) Token: 0x06001F6D RID: 8045 RVA: 0x000571DE File Offset: 0x000553DE
		// (set) Token: 0x06001F6E RID: 8046 RVA: 0x000571E6 File Offset: 0x000553E6
		[BsonRepresentation(BsonType.ObjectId)]
		public string Sender { get; set; }

		// Token: 0x1700068D RID: 1677
		// (get) Token: 0x06001F6F RID: 8047 RVA: 0x000571EF File Offset: 0x000553EF
		// (set) Token: 0x06001F70 RID: 8048 RVA: 0x000571F7 File Offset: 0x000553F7
		[BsonRepresentation(BsonType.ObjectId)]
		public string Recipient { get; set; }

		// Token: 0x1700068E RID: 1678
		// (get) Token: 0x06001F71 RID: 8049 RVA: 0x00057200 File Offset: 0x00055400
		// (set) Token: 0x06001F72 RID: 8050 RVA: 0x00057208 File Offset: 0x00055408
		[BsonIgnoreIfNull]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string GuildId { get; set; }

		// Token: 0x1700068F RID: 1679
		// (get) Token: 0x06001F73 RID: 8051 RVA: 0x00057211 File Offset: 0x00055411
		// (set) Token: 0x06001F74 RID: 8052 RVA: 0x00057219 File Offset: 0x00055419
		[BsonIgnoreIfNull]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string GuildName { get; set; }

		// Token: 0x17000690 RID: 1680
		// (get) Token: 0x06001F75 RID: 8053 RVA: 0x00057222 File Offset: 0x00055422
		// (set) Token: 0x06001F76 RID: 8054 RVA: 0x0005722A File Offset: 0x0005542A
		[BsonIgnoreIfNull]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string GuildDescription { get; set; }

		// Token: 0x17000691 RID: 1681
		// (get) Token: 0x06001F77 RID: 8055 RVA: 0x00057233 File Offset: 0x00055433
		// (set) Token: 0x06001F78 RID: 8056 RVA: 0x0005723B File Offset: 0x0005543B
		[BsonIgnoreIfNull]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string Message { get; set; }

		// Token: 0x17000692 RID: 1682
		// (get) Token: 0x06001F79 RID: 8057 RVA: 0x00057244 File Offset: 0x00055444
		// (set) Token: 0x06001F7A RID: 8058 RVA: 0x0005724C File Offset: 0x0005544C
		[BsonIgnoreIfNull]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string Subject { get; set; }

		// Token: 0x17000693 RID: 1683
		// (get) Token: 0x06001F7B RID: 8059 RVA: 0x00057255 File Offset: 0x00055455
		// (set) Token: 0x06001F7C RID: 8060 RVA: 0x0005725D File Offset: 0x0005545D
		[BsonIgnoreIfNull]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public List<ArchetypeInstance> ItemAttachments { get; set; }

		// Token: 0x17000694 RID: 1684
		// (get) Token: 0x06001F7D RID: 8061 RVA: 0x00057266 File Offset: 0x00055466
		// (set) Token: 0x06001F7E RID: 8062 RVA: 0x0005726E File Offset: 0x0005546E
		[BsonIgnoreIfNull]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public ulong? CashOnDelivery { get; set; }

		// Token: 0x17000695 RID: 1685
		// (get) Token: 0x06001F7F RID: 8063 RVA: 0x00057277 File Offset: 0x00055477
		// (set) Token: 0x06001F80 RID: 8064 RVA: 0x0005727F File Offset: 0x0005547F
		[BsonDateTimeOptions(Representation = BsonType.DateTime)]
		public DateTime Created { get; set; }

		// Token: 0x17000696 RID: 1686
		// (get) Token: 0x06001F81 RID: 8065 RVA: 0x00057288 File Offset: 0x00055488
		// (set) Token: 0x06001F82 RID: 8066 RVA: 0x00057290 File Offset: 0x00055490
		[BsonIgnoreIfNull]
		[BsonDateTimeOptions(Representation = BsonType.DateTime)]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public DateTime? Expires { get; set; }

		// Token: 0x17000697 RID: 1687
		// (get) Token: 0x06001F83 RID: 8067 RVA: 0x00057299 File Offset: 0x00055499
		// (set) Token: 0x06001F84 RID: 8068 RVA: 0x000572A1 File Offset: 0x000554A1
		[BsonIgnoreIfNull]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public bool? Auction { get; set; }

		// Token: 0x17000698 RID: 1688
		// (get) Token: 0x06001F85 RID: 8069 RVA: 0x000572AA File Offset: 0x000554AA
		// (set) Token: 0x06001F86 RID: 8070 RVA: 0x000572B2 File Offset: 0x000554B2
		[BsonIgnoreIfNull]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public bool? Returned { get; set; }

		// Token: 0x17000699 RID: 1689
		// (get) Token: 0x06001F87 RID: 8071 RVA: 0x000572BB File Offset: 0x000554BB
		// (set) Token: 0x06001F88 RID: 8072 RVA: 0x000572C3 File Offset: 0x000554C3
		public bool Read { get; set; }

		// Token: 0x1700069A RID: 1690
		// (get) Token: 0x06001F89 RID: 8073 RVA: 0x000572CC File Offset: 0x000554CC
		[BsonIgnore]
		[JsonIgnore]
		public bool IsSystemMail
		{
			get
			{
				return this.Sender == "000000000000000000000000" || this.Sender == "Postmaster General";
			}
		}

		// Token: 0x1700069B RID: 1691
		// (get) Token: 0x06001F8A RID: 8074 RVA: 0x0011F364 File Offset: 0x0011D564
		[BsonIgnore]
		[JsonIgnore]
		public bool CanBeReturned
		{
			get
			{
				if (this.IsSystemMail)
				{
					return false;
				}
				if (this.ItemAttachments == null || this.ItemAttachments.Count <= 0)
				{
					ulong? currencyAttachment = this.CurrencyAttachment;
					ulong num = 0UL;
					return currencyAttachment.GetValueOrDefault() > num & currencyAttachment != null;
				}
				return true;
			}
		}

		// Token: 0x06001F8B RID: 8075 RVA: 0x000572F2 File Offset: 0x000554F2
		public ulong GetPostage(GameEntity entity)
		{
			if (this.ItemAttachments == null || this.ItemAttachments.Count <= 0)
			{
				return GlobalSettings.Values.Social.BasePostage;
			}
			return Mail.GetPostage(entity, this.ItemAttachments);
		}

		// Token: 0x06001F8C RID: 8076 RVA: 0x0011F3B0 File Offset: 0x0011D5B0
		public static ulong GetPostage(GameEntity entity, IEnumerable<ArchetypeInstance> instances)
		{
			ulong num = GlobalSettings.Values.Social.BasePostage;
			if (!entity)
			{
				return num;
			}
			float stackablePostageFraction = GlobalSettings.Values.Social.StackablePostageFraction;
			foreach (ArchetypeInstance instance in instances)
			{
				num += Mail.GetPostageForInstance(instance);
			}
			return num;
		}

		// Token: 0x06001F8D RID: 8077 RVA: 0x0011F428 File Offset: 0x0011D628
		public static ulong GetPostageForInstance(ArchetypeInstance instance)
		{
			ulong num = 0UL;
			if (instance != null && instance.Archetype && instance.Archetype.ItemCategory)
			{
				num += ((instance.Archetype.ArchetypeHasCount() && instance.ItemData != null && instance.ItemData.Count != null && instance.ItemData.Count.Value > 0) ? (instance.Archetype.ItemCategory.StackablePostage * (ulong)((long)instance.ItemData.Count.Value)) : instance.Archetype.ItemCategory.Postage);
			}
			else
			{
				Debug.LogError("Non Instance or no ItemCategory in GetPostageForInstance");
			}
			return num;
		}

		// Token: 0x1700069C RID: 1692
		// (get) Token: 0x06001F8E RID: 8078 RVA: 0x0011F4E8 File Offset: 0x0011D6E8
		[BsonIgnore]
		[JsonIgnore]
		private FilterDefinition<Mail> IdFilter
		{
			get
			{
				if (this.m_idFilter == null)
				{
					this.m_idFilter = Builders<Mail>.Filter.Eq<string>((Mail cr) => cr._id, this._id);
				}
				return this.m_idFilter;
			}
		}

		// Token: 0x06001F8F RID: 8079 RVA: 0x0011F554 File Offset: 0x0011D754
		public static Mail Load(IMongoDatabase db, string id)
		{
			IMongoCollection<Mail> collection = db.GetCollection<Mail>("mail_post", null);
			FilterDefinition<Mail> filter = Builders<Mail>.Filter.Eq<string>((Mail cr) => cr._id, id);
			return collection.Find(filter, null).FirstOrDefault(default(CancellationToken));
		}

		// Token: 0x06001F90 RID: 8080 RVA: 0x0011F5CC File Offset: 0x0011D7CC
		public static IEnumerable<Mail> LoadAll(IMongoDatabase db)
		{
			return db.GetCollection<Mail>("mail_post", null).Find(FilterDefinition<Mail>.Empty, null).ToList(default(CancellationToken));
		}

		// Token: 0x06001F91 RID: 8081 RVA: 0x0011F600 File Offset: 0x0011D800
		public void StoreNew(IMongoDatabase db)
		{
			db.GetCollection<Mail>("mail_post", null).InsertOne(this, null, default(CancellationToken));
		}

		// Token: 0x06001F92 RID: 8082 RVA: 0x0011F62C File Offset: 0x0011D82C
		public Task StoreNewAsync(IMongoDatabase db)
		{
			Mail.<StoreNewAsync>d__82 <StoreNewAsync>d__;
			<StoreNewAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<StoreNewAsync>d__.<>4__this = this;
			<StoreNewAsync>d__.db = db;
			<StoreNewAsync>d__.<>1__state = -1;
			<StoreNewAsync>d__.<>t__builder.Start<Mail.<StoreNewAsync>d__82>(ref <StoreNewAsync>d__);
			return <StoreNewAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06001F93 RID: 8083 RVA: 0x0011F678 File Offset: 0x0011D878
		public Task<bool> UpdateRecordAsync(IMongoDatabase db)
		{
			Mail.<UpdateRecordAsync>d__83 <UpdateRecordAsync>d__;
			<UpdateRecordAsync>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<UpdateRecordAsync>d__.<>4__this = this;
			<UpdateRecordAsync>d__.db = db;
			<UpdateRecordAsync>d__.<>1__state = -1;
			<UpdateRecordAsync>d__.<>t__builder.Start<Mail.<UpdateRecordAsync>d__83>(ref <UpdateRecordAsync>d__);
			return <UpdateRecordAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06001F94 RID: 8084 RVA: 0x0011F6C4 File Offset: 0x0011D8C4
		public bool UpdateRecord(IMongoDatabase db)
		{
			IMongoCollection<Mail> collection = db.GetCollection<Mail>("mail_post", null);
			UpdateDefinition<Mail> update = Builders<Mail>.Update.Set<string>((Mail cr) => cr._id, this._id).Set((Mail cr) => cr.Type, this.Type).Set((Mail cr) => cr.Sender, this.Sender).Set((Mail cr) => cr.Recipient, this.Recipient).Set((Mail cr) => cr.GuildId, this.GuildId).Set((Mail cr) => cr.GuildName, this.GuildName).Set((Mail cr) => cr.GuildDescription, this.GuildDescription).Set((Mail cr) => cr.Message, this.Message).Set((Mail cr) => cr.Subject, this.Subject).Set((Mail cr) => cr.ItemAttachments, this.ItemAttachments).Set((Mail cr) => cr.CurrencyAttachment, this.CurrencyAttachment).Set((Mail cr) => cr.CashOnDelivery, this.CashOnDelivery).Set((Mail cr) => cr.Created, this.Created).Set((Mail cr) => cr.Expires, this.Expires).Set((Mail cr) => cr.Returned, this.Returned).Set((Mail cr) => cr.Read, this.Read);
			return collection.UpdateOne(this.IdFilter, update, null, default(CancellationToken)).ModifiedCount > 0L;
		}

		// Token: 0x06001F95 RID: 8085 RVA: 0x0011FB40 File Offset: 0x0011DD40
		public bool UpdateAttachments(IMongoDatabase db)
		{
			IMongoCollection<Mail> collection = db.GetCollection<Mail>("mail_post", null);
			UpdateDefinition<Mail> update = Builders<Mail>.Update.Set<List<ArchetypeInstance>>((Mail cr) => cr.ItemAttachments, this.ItemAttachments).Set((Mail cr) => cr.CurrencyAttachment, this.CurrencyAttachment);
			return collection.UpdateOne(this.IdFilter, update, null, default(CancellationToken)).ModifiedCount > 0L;
		}

		// Token: 0x06001F96 RID: 8086 RVA: 0x0011FC04 File Offset: 0x0011DE04
		public bool UpdateAttachmentsAndCod(IMongoDatabase db)
		{
			IMongoCollection<Mail> collection = db.GetCollection<Mail>("mail_post", null);
			UpdateDefinition<Mail> update = Builders<Mail>.Update.Set<List<ArchetypeInstance>>((Mail cr) => cr.ItemAttachments, this.ItemAttachments).Set((Mail cr) => cr.CurrencyAttachment, this.CurrencyAttachment).Set((Mail cr) => cr.CashOnDelivery, this.CashOnDelivery);
			return collection.UpdateOne(this.IdFilter, update, null, default(CancellationToken)).ModifiedCount > 0L;
		}

		// Token: 0x06001F97 RID: 8087 RVA: 0x0011FD0C File Offset: 0x0011DF0C
		public bool DeleteRecord(IMongoDatabase db)
		{
			return db.GetCollection<Mail>("mail_post", null).DeleteOne(this.IdFilter, default(CancellationToken)).DeletedCount == 1L;
		}

		// Token: 0x040024DA RID: 9434
		public const float kLocalExpirationBuffer = -30f;

		// Token: 0x040024DB RID: 9435
		[BsonIgnore]
		[JsonIgnore]
		public const string kSystemAuthorId = "000000000000000000000000";

		// Token: 0x040024DC RID: 9436
		[BsonIgnore]
		[JsonIgnore]
		public const string kSystemAuthorName = "Postmaster General";

		// Token: 0x040024E7 RID: 9447
		[BsonIgnoreIfNull]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public ulong? CurrencyAttachment;

		// Token: 0x040024EE RID: 9454
		private const string kTableName = "mail_post";

		// Token: 0x040024EF RID: 9455
		[BsonIgnore]
		[JsonIgnore]
		private FilterDefinition<Mail> m_idFilter;
	}
}

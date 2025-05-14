using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace SoL.Networking.Database
{
	// Token: 0x0200046C RID: 1132
	[BsonIgnoreExtraElements]
	public class UserRecord
	{
		// Token: 0x170006A4 RID: 1700
		// (get) Token: 0x06001FC3 RID: 8131 RVA: 0x00057464 File Offset: 0x00055664
		[BsonIgnore]
		[JsonIgnore]
		public AccessFlags Flags
		{
			get
			{
				return (AccessFlags)this.m_flags;
			}
		}

		// Token: 0x06001FC4 RID: 8132 RVA: 0x0005746C File Offset: 0x0005566C
		public bool IsGM()
		{
			return this.Flags.HasBitFlag(AccessFlags.GM);
		}

		// Token: 0x06001FC5 RID: 8133 RVA: 0x0005747A File Offset: 0x0005567A
		public bool IsSubscriber()
		{
			return this.IsGM() || this.Flags.HasBitFlag(AccessFlags.Subscriber);
		}

		// Token: 0x06001FC6 RID: 8134 RVA: 0x00057493 File Offset: 0x00055693
		public bool IsTrial()
		{
			return !this.Flags.HasBitFlag(AccessFlags.FullClient);
		}

		// Token: 0x06001FC7 RID: 8135 RVA: 0x001209EC File Offset: 0x0011EBEC
		public bool CharacterIsSelectable(CharacterRecord record)
		{
			if (this.IsSubscriber())
			{
				return true;
			}
			if (record != null && this.ActiveCharacters != null)
			{
				for (int i = 0; i < this.ActiveCharacters.Length; i++)
				{
					if (this.ActiveCharacters[i] == record.Id)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06001FC8 RID: 8136 RVA: 0x000574A5 File Offset: 0x000556A5
		public static int GetMaxCharacterCount(UserRecord record)
		{
			if (record == null || !record.IsSubscriber())
			{
				return 2;
			}
			return 9;
		}

		// Token: 0x06001FC9 RID: 8137 RVA: 0x00120A3C File Offset: 0x0011EC3C
		public static UserRecord Load(IMongoDatabase db, string id)
		{
			IMongoCollection<UserRecord> collection = db.GetCollection<UserRecord>(UserRecord.kTableName, null);
			FilterDefinition<UserRecord> filter = Builders<UserRecord>.Filter.Eq<string>((UserRecord u) => u.Id, id);
			return collection.Find(filter, null).FirstOrDefault(default(CancellationToken));
		}

		// Token: 0x06001FCA RID: 8138 RVA: 0x00120AAC File Offset: 0x0011ECAC
		public static IEnumerable<UserRecord> LoadAll(IMongoDatabase db)
		{
			return db.GetCollection<UserRecord>(UserRecord.kTableName, null).Find(FilterDefinition<UserRecord>.Empty, null).ToList(default(CancellationToken));
		}

		// Token: 0x170006A5 RID: 1701
		// (get) Token: 0x06001FCB RID: 8139 RVA: 0x00120AE0 File Offset: 0x0011ECE0
		[BsonIgnore]
		[JsonIgnore]
		private FilterDefinition<UserRecord> IdFilter
		{
			get
			{
				if (this.m_idFilter == null)
				{
					this.m_idFilter = Builders<UserRecord>.Filter.Eq<string>((UserRecord ur) => ur.Id, this.Id);
				}
				return this.m_idFilter;
			}
		}

		// Token: 0x06001FCC RID: 8140 RVA: 0x00120B48 File Offset: 0x0011ED48
		public Task<bool> UpdateEventCurrencyAsync(IMongoDatabase db)
		{
			UserRecord.<UpdateEventCurrencyAsync>d__24 <UpdateEventCurrencyAsync>d__;
			<UpdateEventCurrencyAsync>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<UpdateEventCurrencyAsync>d__.<>4__this = this;
			<UpdateEventCurrencyAsync>d__.db = db;
			<UpdateEventCurrencyAsync>d__.<>1__state = -1;
			<UpdateEventCurrencyAsync>d__.<>t__builder.Start<UserRecord.<UpdateEventCurrencyAsync>d__24>(ref <UpdateEventCurrencyAsync>d__);
			return <UpdateEventCurrencyAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06001FCD RID: 8141 RVA: 0x00120B94 File Offset: 0x0011ED94
		public bool UpdateEventCurrency(IMongoDatabase db)
		{
			IMongoCollection<UserRecord> collection = db.GetCollection<UserRecord>(UserRecord.kTableName, null);
			UpdateDefinition<UserRecord> update = Builders<UserRecord>.Update.Set<DateTime>((UserRecord ur) => ur.Updated, DateTime.UtcNow).Set((UserRecord ur) => ur.EventCurrency, this.EventCurrency);
			return collection.UpdateOne(this.IdFilter, update, null, default(CancellationToken)).ModifiedCount > 0L;
		}

		// Token: 0x0400252A RID: 9514
		[BsonId]
		[BsonRepresentation(BsonType.String)]
		[JsonProperty(PropertyName = "_id")]
		public string Id;

		// Token: 0x0400252B RID: 9515
		[BsonElement("flags")]
		[JsonProperty(PropertyName = "flags")]
		private int m_flags;

		// Token: 0x0400252C RID: 9516
		[BsonElement("userName")]
		[JsonProperty(PropertyName = "userName")]
		public string UserName;

		// Token: 0x0400252D RID: 9517
		[BsonElement("email")]
		[JsonProperty(PropertyName = "email")]
		public string Email;

		// Token: 0x0400252E RID: 9518
		[BsonElement("created")]
		[JsonProperty(PropertyName = "created")]
		public DateTime Created;

		// Token: 0x0400252F RID: 9519
		[BsonElement("updated")]
		[JsonProperty(PropertyName = "updated")]
		public DateTime Updated;

		// Token: 0x04002530 RID: 9520
		[BsonElement("rewards")]
		[JsonProperty(PropertyName = "rewards")]
		public ClientReward[] Rewards;

		// Token: 0x04002531 RID: 9521
		[BsonElement("activeCharacters")]
		[JsonProperty(PropertyName = "activeCharacters")]
		public string[] ActiveCharacters;

		// Token: 0x04002532 RID: 9522
		[BsonIgnoreIfNull]
		[BsonElement("eventCurrency")]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "eventCurrency")]
		public ulong? EventCurrency;

		// Token: 0x04002533 RID: 9523
		public const int kMaxSubscriberCharacters = 9;

		// Token: 0x04002534 RID: 9524
		public const int kMaxNonSubscriberCharacters = 2;

		// Token: 0x04002535 RID: 9525
		private static string kTableName = "users";

		// Token: 0x04002536 RID: 9526
		[BsonIgnore]
		[JsonIgnore]
		private FilterDefinition<UserRecord> m_idFilter;
	}
}

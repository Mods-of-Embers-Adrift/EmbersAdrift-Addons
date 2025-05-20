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
using SoL.Game;
using SoL.Game.EffectSystem;
using SoL.Game.HuntingLog;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Containers;
using SoL.Game.Quests;
using SoL.Utilities;

namespace SoL.Networking.Database
{
	// Token: 0x0200042C RID: 1068
	[BsonIgnoreExtraElements]
	public class CharacterRecord : INetworkSerializable
	{
		// Token: 0x17000669 RID: 1641
		// (get) Token: 0x06001E95 RID: 7829 RVA: 0x00056C24 File Offset: 0x00054E24
		// (set) Token: 0x06001E96 RID: 7830 RVA: 0x00056C2C File Offset: 0x00054E2C
		[BsonIgnore]
		[JsonIgnore]
		public int CorpseZoneId { get; private set; }

		// Token: 0x1700066A RID: 1642
		// (get) Token: 0x06001E97 RID: 7831 RVA: 0x00056C35 File Offset: 0x00054E35
		// (set) Token: 0x06001E98 RID: 7832 RVA: 0x00056C3D File Offset: 0x00054E3D
		[BsonIgnoreIfNull]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public DateTime? CorpseCreationTime
		{
			get
			{
				return this.m_corpseCreationTime;
			}
			set
			{
				this.m_corpseCreationTime = value;
			}
		}

		// Token: 0x1700066B RID: 1643
		// (get) Token: 0x06001E99 RID: 7833 RVA: 0x00056C46 File Offset: 0x00054E46
		// (set) Token: 0x06001E9A RID: 7834 RVA: 0x00056C4E File Offset: 0x00054E4E
		[BsonIgnoreIfNull]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public bool? RequiresRenaming { get; set; }

		// Token: 0x1700066C RID: 1644
		// (get) Token: 0x06001E9B RID: 7835 RVA: 0x0011B178 File Offset: 0x00119378
		[BsonIgnore]
		[JsonIgnore]
		private FilterDefinition<CharacterRecord> IdFilter
		{
			get
			{
				if (this.m_idFilter == null)
				{
					this.m_idFilter = Builders<CharacterRecord>.Filter.Eq<string>((CharacterRecord cr) => cr.Id, this.Id);
				}
				return this.m_idFilter;
			}
		}

		// Token: 0x06001E9C RID: 7836 RVA: 0x0011B1E0 File Offset: 0x001193E0
		public static CharacterRecord Load(IMongoDatabase db, string id)
		{
			IMongoCollection<CharacterRecord> collection = db.GetCollection<CharacterRecord>(CharacterRecord.kTableName, null);
			FilterDefinition<CharacterRecord> filter = Builders<CharacterRecord>.Filter.Eq<string>((CharacterRecord cr) => cr.Id, id);
			return collection.Find(filter, null).FirstOrDefault(default(CancellationToken));
		}

		// Token: 0x06001E9D RID: 7837 RVA: 0x0011B250 File Offset: 0x00119450
		public static Task<CharacterRecord> LoadAsync(IMongoDatabase db, string id)
		{
			IMongoCollection<CharacterRecord> collection = db.GetCollection<CharacterRecord>(CharacterRecord.kTableName, null);
			FilterDefinition<CharacterRecord> filter = Builders<CharacterRecord>.Filter.Eq<string>((CharacterRecord cr) => cr.Id, id);
			return collection.Find(filter, null).FirstOrDefaultAsync(default(CancellationToken));
		}

		// Token: 0x06001E9E RID: 7838 RVA: 0x0011B2C0 File Offset: 0x001194C0
		public static CharacterRecord LoadForCharacterName(IMongoDatabase db, string characterName)
		{
			IMongoCollection<CharacterRecord> collection = db.GetCollection<CharacterRecord>(CharacterRecord.kTableName, null);
			FilterDefinition<CharacterRecord> filter = Builders<CharacterRecord>.Filter.Eq<string>((CharacterRecord cr) => cr.Name, characterName);
			return collection.Find(filter, null).FirstOrDefault(default(CancellationToken));
		}

		// Token: 0x06001E9F RID: 7839 RVA: 0x0011B330 File Offset: 0x00119530
		public static IEnumerable<CharacterRecord> LoadAll(IMongoDatabase db)
		{
			return db.GetCollection<CharacterRecord>(CharacterRecord.kTableName, null).Find(FilterDefinition<CharacterRecord>.Empty, null).ToList(default(CancellationToken));
		}

		// Token: 0x06001EA0 RID: 7840 RVA: 0x0011B364 File Offset: 0x00119564
		public static IEnumerable<CharacterRecord> LoadAllForUser(IMongoDatabase db, string userId)
		{
			IMongoCollection<CharacterRecord> collection = db.GetCollection<CharacterRecord>(CharacterRecord.kTableName, null);
			FilterDefinition<CharacterRecord> filter = Builders<CharacterRecord>.Filter.Eq<string>((CharacterRecord cr) => cr.UserId, userId);
			return collection.Find(filter, null).ToList(default(CancellationToken));
		}

		// Token: 0x06001EA1 RID: 7841 RVA: 0x0011B3D4 File Offset: 0x001195D4
		public Task<bool> UpdateRecordAsync(IMongoDatabase db)
		{
			CharacterRecord.<UpdateRecordAsync>d__50 <UpdateRecordAsync>d__;
			<UpdateRecordAsync>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<UpdateRecordAsync>d__.<>4__this = this;
			<UpdateRecordAsync>d__.db = db;
			<UpdateRecordAsync>d__.<>1__state = -1;
			<UpdateRecordAsync>d__.<>t__builder.Start<CharacterRecord.<UpdateRecordAsync>d__50>(ref <UpdateRecordAsync>d__);
			return <UpdateRecordAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06001EA2 RID: 7842 RVA: 0x0011B420 File Offset: 0x00119620
		public Task<bool> UpdateTitleData(IMongoDatabase db)
		{
			CharacterRecord.<UpdateTitleData>d__51 <UpdateTitleData>d__;
			<UpdateTitleData>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<UpdateTitleData>d__.<>4__this = this;
			<UpdateTitleData>d__.db = db;
			<UpdateTitleData>d__.<>1__state = -1;
			<UpdateTitleData>d__.<>t__builder.Start<CharacterRecord.<UpdateTitleData>d__51>(ref <UpdateTitleData>d__);
			return <UpdateTitleData>d__.<>t__builder.Task;
		}

		// Token: 0x06001EA3 RID: 7843 RVA: 0x0011B46C File Offset: 0x0011966C
		public bool NonAsyncUpdateTitleData(IMongoDatabase db)
		{
			GlobalCounters.CharTitleWrites += 1U;
			IMongoCollection<CharacterRecord> collection = db.GetCollection<CharacterRecord>(CharacterRecord.kTableName, null);
			UpdateDefinition<CharacterRecord> update = Builders<CharacterRecord>.Update.Set<DateTime>((CharacterRecord cr) => cr.Updated, DateTime.UtcNow).Set((CharacterRecord cr) => cr.Title, this.Title).Set((CharacterRecord cr) => cr.AvailableTitles, this.AvailableTitles);
			return collection.UpdateOne(this.IdFilter, update, null, default(CancellationToken)).ModifiedCount > 0L;
		}

		// Token: 0x06001EA4 RID: 7844 RVA: 0x0011B574 File Offset: 0x00119774
		public bool UpdateRecord(IMongoDatabase db)
		{
			GlobalCounters.CharRecordWrites += 1U;
			IMongoCollection<CharacterRecord> collection = db.GetCollection<CharacterRecord>(CharacterRecord.kTableName, null);
			UpdateDefinition<CharacterRecord> update = Builders<CharacterRecord>.Update.Set<DateTime>((CharacterRecord cr) => cr.Updated, DateTime.UtcNow).Set((CharacterRecord cr) => cr.TimePlayed, this.TimePlayed).Set((CharacterRecord cr) => cr.Location, this.Location).Set((CharacterRecord cr) => cr.Vitals, this.Vitals).Set((CharacterRecord cr) => cr.ZoningState, this.ZoningState).Set((CharacterRecord cr) => cr.Storage, this.Storage).Set((CharacterRecord cr) => cr.Learnables, this.Learnables).Set((CharacterRecord cr) => cr.Discoveries, this.Discoveries).Set((CharacterRecord cr) => cr.Effects, this.Effects).Set((CharacterRecord cr) => cr.Settings, this.Settings).Set((CharacterRecord cr) => cr.ConsumableLastUseTimes, this.ConsumableLastUseTimes).Set((CharacterRecord cr) => cr.EmberStoneData, this.EmberStoneData).Set((CharacterRecord cr) => cr.MerchantBuybackItems, this.MerchantBuybackItems).Set((CharacterRecord cr) => cr.BagBuybackItems, this.BagBuybackItems).Set((CharacterRecord cr) => cr.HuntingLogVersion, this.HuntingLogVersion).Set((CharacterRecord cr) => cr.HuntingLog, this.HuntingLog);
			return collection.UpdateOne(this.IdFilter, update, null, default(CancellationToken)).ModifiedCount > 0L;
		}

		// Token: 0x06001EA5 RID: 7845 RVA: 0x0011B9B0 File Offset: 0x00119BB0
		public Task<bool> UpdatePositionAsync(IMongoDatabase db)
		{
			CharacterRecord.<UpdatePositionAsync>d__54 <UpdatePositionAsync>d__;
			<UpdatePositionAsync>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<UpdatePositionAsync>d__.<>4__this = this;
			<UpdatePositionAsync>d__.db = db;
			<UpdatePositionAsync>d__.<>1__state = -1;
			<UpdatePositionAsync>d__.<>t__builder.Start<CharacterRecord.<UpdatePositionAsync>d__54>(ref <UpdatePositionAsync>d__);
			return <UpdatePositionAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06001EA6 RID: 7846 RVA: 0x0011B9FC File Offset: 0x00119BFC
		public bool UpdatePosition(IMongoDatabase db)
		{
			IMongoCollection<CharacterRecord> collection = db.GetCollection<CharacterRecord>(CharacterRecord.kTableName, null);
			UpdateDefinition<CharacterRecord> update = Builders<CharacterRecord>.Update.Set<DateTime>((CharacterRecord cr) => cr.Updated, DateTime.UtcNow).Set((CharacterRecord cr) => cr.Location, this.Location);
			return collection.UpdateOne(this.IdFilter, update, null, default(CancellationToken)).ModifiedCount > 0L;
		}

		// Token: 0x06001EA7 RID: 7847 RVA: 0x0011BABC File Offset: 0x00119CBC
		public bool UpdateCorpseCreationTime(IMongoDatabase db)
		{
			IMongoCollection<CharacterRecord> collection = db.GetCollection<CharacterRecord>(CharacterRecord.kTableName, null);
			UpdateDefinition<CharacterRecord> update = Builders<CharacterRecord>.Update.Set<DateTime?>((CharacterRecord cr) => cr.CorpseCreationTime, this.CorpseCreationTime);
			return collection.UpdateOne(this.IdFilter, update, null, default(CancellationToken)).ModifiedCount > 0L;
		}

		// Token: 0x06001EA8 RID: 7848 RVA: 0x0011BB40 File Offset: 0x00119D40
		public bool UpdateCorpseMigration(IMongoDatabase db)
		{
			IMongoCollection<CharacterRecord> collection = db.GetCollection<CharacterRecord>(CharacterRecord.kTableName, null);
			UpdateDefinition<CharacterRecord> update = Builders<CharacterRecord>.Update.Set<CharacterCorpse>((CharacterRecord cr) => cr.Corpse, this.Corpse).Set((CharacterRecord cr) => cr.CorpseCreationTime, this.CorpseCreationTime);
			return collection.UpdateOne(this.IdFilter, update, null, default(CancellationToken)).ModifiedCount > 0L;
		}

		// Token: 0x06001EA9 RID: 7849 RVA: 0x0011BC04 File Offset: 0x00119E04
		public bool UpdateCorpseData(IMongoDatabase db)
		{
			IMongoCollection<CharacterRecord> collection = db.GetCollection<CharacterRecord>(CharacterRecord.kTableName, null);
			UpdateDefinition<CharacterRecord> update = Builders<CharacterRecord>.Update.Set<CharacterCorpse>((CharacterRecord cr) => cr.Corpse, this.Corpse);
			return collection.UpdateOne(this.IdFilter, update, null, default(CancellationToken)).ModifiedCount > 0L;
		}

		// Token: 0x06001EAA RID: 7850 RVA: 0x0011BC84 File Offset: 0x00119E84
		public Task<bool> UpdateCorpseDataAsync(IMongoDatabase db)
		{
			CharacterRecord.<UpdateCorpseDataAsync>d__59 <UpdateCorpseDataAsync>d__;
			<UpdateCorpseDataAsync>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<UpdateCorpseDataAsync>d__.<>4__this = this;
			<UpdateCorpseDataAsync>d__.db = db;
			<UpdateCorpseDataAsync>d__.<>1__state = -1;
			<UpdateCorpseDataAsync>d__.<>t__builder.Start<CharacterRecord.<UpdateCorpseDataAsync>d__59>(ref <UpdateCorpseDataAsync>d__);
			return <UpdateCorpseDataAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06001EAB RID: 7851 RVA: 0x0011BCD0 File Offset: 0x00119ED0
		public bool UpdateQuests(IMongoDatabase db)
		{
			GlobalCounters.CharQuestWrites += 1U;
			IMongoCollection<CharacterRecord> collection = db.GetCollection<CharacterRecord>(CharacterRecord.kTableName, null);
			UpdateDefinition<CharacterRecord> update = Builders<CharacterRecord>.Update.Set<DateTime>((CharacterRecord cr) => cr.Updated, DateTime.UtcNow).Set((CharacterRecord cr) => cr.Progression, this.Progression);
			return collection.UpdateOne(this.IdFilter, update, null, default(CancellationToken)).ModifiedCount > 0L;
		}

		// Token: 0x06001EAC RID: 7852 RVA: 0x0011BD9C File Offset: 0x00119F9C
		public bool UpdateLearnables(IMongoDatabase db)
		{
			GlobalCounters.CharLearnableWrites += 1U;
			IMongoCollection<CharacterRecord> collection = db.GetCollection<CharacterRecord>(CharacterRecord.kTableName, null);
			UpdateDefinition<CharacterRecord> update = Builders<CharacterRecord>.Update.Set<DateTime>((CharacterRecord cr) => cr.Updated, DateTime.UtcNow).Set((CharacterRecord cr) => cr.Learnables, this.Learnables);
			return collection.UpdateOne(this.IdFilter, update, null, default(CancellationToken)).ModifiedCount > 0L;
		}

		// Token: 0x06001EAD RID: 7853 RVA: 0x0011BE68 File Offset: 0x0011A068
		public bool UpdateDiscoveries(IMongoDatabase db)
		{
			GlobalCounters.CharDiscoveryWrites += 1U;
			IMongoCollection<CharacterRecord> collection = db.GetCollection<CharacterRecord>(CharacterRecord.kTableName, null);
			UpdateDefinition<CharacterRecord> update = Builders<CharacterRecord>.Update.Set<DateTime>((CharacterRecord cr) => cr.Updated, DateTime.UtcNow).Set((CharacterRecord cr) => cr.Discoveries, this.Discoveries);
			return collection.UpdateOne(this.IdFilter, update, null, default(CancellationToken)).ModifiedCount > 0L;
		}

		// Token: 0x06001EAE RID: 7854 RVA: 0x0011BF34 File Offset: 0x0011A134
		public bool UpdateSelectionPositionIndex(IMongoDatabase db)
		{
			IMongoCollection<CharacterRecord> collection = db.GetCollection<CharacterRecord>(CharacterRecord.kTableName, null);
			UpdateDefinition<CharacterRecord> update = Builders<CharacterRecord>.Update.Set<int>((CharacterRecord cr) => cr.SelectionPositionIndex, this.SelectionPositionIndex);
			return collection.UpdateOne(this.IdFilter, update, null, default(CancellationToken)).ModifiedCount > 0L;
		}

		// Token: 0x06001EAF RID: 7855 RVA: 0x0011BFB4 File Offset: 0x0011A1B4
		public bool UpdateStorage(IMongoDatabase db)
		{
			GlobalCounters.CharStorageWrites += 1U;
			IMongoCollection<CharacterRecord> collection = db.GetCollection<CharacterRecord>(CharacterRecord.kTableName, null);
			UpdateDefinition<CharacterRecord> update = Builders<CharacterRecord>.Update.Set<Dictionary<ContainerType, ContainerRecord>>((CharacterRecord cr) => cr.Storage, this.Storage);
			return collection.UpdateOne(this.IdFilter, update, null, default(CancellationToken)).ModifiedCount > 0L;
		}

		// Token: 0x06001EB0 RID: 7856 RVA: 0x0011C040 File Offset: 0x0011A240
		public bool UpdateInvalidItemsAndStorage(IMongoDatabase db)
		{
			GlobalCounters.CharStorageWrites += 1U;
			IMongoCollection<CharacterRecord> collection = db.GetCollection<CharacterRecord>(CharacterRecord.kTableName, null);
			UpdateDefinition<CharacterRecord> update = Builders<CharacterRecord>.Update.Set<Dictionary<ContainerType, ContainerRecord>>((CharacterRecord cr) => cr.Storage, this.Storage).Set((CharacterRecord cr) => cr.InvalidItems, this.InvalidItems);
			return collection.UpdateOne(this.IdFilter, update, null, default(CancellationToken)).ModifiedCount > 0L;
		}

		// Token: 0x06001EB1 RID: 7857 RVA: 0x0011C10C File Offset: 0x0011A30C
		public bool UpdateInvalidItems(IMongoDatabase db)
		{
			GlobalCounters.CharStorageWrites += 1U;
			IMongoCollection<CharacterRecord> collection = db.GetCollection<CharacterRecord>(CharacterRecord.kTableName, null);
			UpdateDefinition<CharacterRecord> update = Builders<CharacterRecord>.Update.Set<List<ArchetypeInstance>>((CharacterRecord cr) => cr.InvalidItems, this.InvalidItems);
			return collection.UpdateOne(this.IdFilter, update, null, default(CancellationToken)).ModifiedCount > 0L;
		}

		// Token: 0x06001EB2 RID: 7858 RVA: 0x0011C198 File Offset: 0x0011A398
		public bool UpdateEmberStoneData(IMongoDatabase db)
		{
			IMongoCollection<CharacterRecord> collection = db.GetCollection<CharacterRecord>(CharacterRecord.kTableName, null);
			UpdateDefinition<CharacterRecord> update = Builders<CharacterRecord>.Update.Set<EmberStoneInstanceData>((CharacterRecord cr) => cr.EmberStoneData, this.EmberStoneData);
			return collection.UpdateOne(this.IdFilter, update, null, default(CancellationToken)).ModifiedCount > 0L;
		}

		// Token: 0x06001EB3 RID: 7859 RVA: 0x0011C218 File Offset: 0x0011A418
		public void CleanupReferences()
		{
			if (NullifyMemoryLeakSettings.CleanCharacterRecord)
			{
				if (this.Storage != null)
				{
					foreach (KeyValuePair<ContainerType, ContainerRecord> keyValuePair in this.Storage)
					{
						ContainerRecord value = keyValuePair.Value;
						if (value != null)
						{
							value.CleanupReferences();
						}
					}
					this.Storage.Clear();
					this.Storage = null;
				}
				if (this.InvalidItems != null)
				{
					for (int i = 0; i < this.InvalidItems.Count; i++)
					{
						StaticPool<ArchetypeInstance>.ReturnToPool(this.InvalidItems[i]);
					}
					this.InvalidItems.Clear();
					this.InvalidItems = null;
				}
				if (this.MerchantBuybackItems != null)
				{
					for (int j = 0; j < this.MerchantBuybackItems.Count; j++)
					{
						if (this.MerchantBuybackItems[j].Instance != null)
						{
							StaticPool<ArchetypeInstance>.ReturnToPool(this.MerchantBuybackItems[j].Instance);
						}
						StaticPool<MerchantBuybackItem>.ReturnToPool(this.MerchantBuybackItems[j]);
					}
					this.MerchantBuybackItems.Clear();
					this.MerchantBuybackItems = null;
				}
				if (this.BagBuybackItems != null)
				{
					for (int k = 0; k < this.BagBuybackItems.Count; k++)
					{
						if (this.BagBuybackItems[k].Instance != null)
						{
							StaticPool<ArchetypeInstance>.ReturnToPool(this.BagBuybackItems[k].Instance);
						}
						StaticPool<MerchantBuybackItem>.ReturnToPool(this.BagBuybackItems[k]);
					}
					this.BagBuybackItems.Clear();
					this.BagBuybackItems = null;
				}
				if (this.Effects != null)
				{
					for (int l = 0; l < this.Effects.Count; l++)
					{
						StaticPool<EffectRecord>.ReturnToPool(this.Effects[l]);
					}
					this.Effects.Clear();
					this.Effects = null;
				}
				if (this.HuntingLog != null)
				{
					foreach (KeyValuePair<UniqueId, HuntingLogEntry> keyValuePair2 in this.HuntingLog)
					{
						HuntingLogEntry value2 = keyValuePair2.Value;
						if (value2 != null)
						{
							value2.ResetReferences();
						}
					}
					this.HuntingLog.Clear();
					this.HuntingLog = null;
				}
			}
		}

		// Token: 0x06001EB4 RID: 7860 RVA: 0x0011C468 File Offset: 0x0011A668
		public BitBuffer PackData(BitBuffer buffer)
		{
			buffer.AddString(this.Id);
			buffer.AddNullableString(this.Title);
			bool flag = this.AvailableTitles != null && this.AvailableTitles.Count > 0;
			buffer.AddBool(flag);
			if (flag)
			{
				buffer.AddInt(this.AvailableTitles.Count);
				for (int i = 0; i < this.AvailableTitles.Count; i++)
				{
					buffer.AddString(this.AvailableTitles[i]);
				}
			}
			buffer.AddDateTime(this.Created);
			buffer.AddNullableTimeSpan(this.TimePlayed);
			buffer.AddInt(this.Storage.Count);
			foreach (KeyValuePair<ContainerType, ContainerRecord> keyValuePair in this.Storage)
			{
				buffer.AddEnum(keyValuePair.Key);
				keyValuePair.Value.PackData(buffer);
			}
			buffer.AddInt(this.Learnables.Count);
			foreach (KeyValuePair<ContainerType, LearnableContainerRecord> keyValuePair2 in this.Learnables)
			{
				buffer.AddEnum(keyValuePair2.Key);
				keyValuePair2.Value.PackData(buffer);
			}
			bool flag2 = this.ConsumableLastUseTimes != null;
			buffer.AddBool(flag2);
			if (flag2)
			{
				buffer.AddInt(this.ConsumableLastUseTimes.Count);
				foreach (KeyValuePair<ConsumableCategory, DateTime> keyValuePair3 in this.ConsumableLastUseTimes)
				{
					buffer.AddEnum(keyValuePair3.Key);
					buffer.AddDateTime(keyValuePair3.Value);
				}
			}
			bool flag3 = this.Progression != null;
			buffer.AddBool(flag3);
			if (flag3)
			{
				this.Progression.PackData(buffer);
			}
			bool flag4 = this.Discoveries != null;
			buffer.AddBool(flag4);
			if (flag4)
			{
				buffer.AddInt(this.Discoveries.Count);
				foreach (KeyValuePair<ZoneId, List<UniqueId>> keyValuePair4 in this.Discoveries)
				{
					buffer.AddEnum(keyValuePair4.Key);
					if (keyValuePair4.Value == null)
					{
						buffer.AddInt(0);
					}
					else
					{
						buffer.AddInt(keyValuePair4.Value.Count);
						for (int j = 0; j < keyValuePair4.Value.Count; j++)
						{
							buffer.AddUniqueId(keyValuePair4.Value[j]);
						}
					}
				}
			}
			bool flag5 = this.EmberStoneData != null;
			buffer.AddBool(flag5);
			if (flag5)
			{
				this.EmberStoneData.PackData(buffer);
			}
			bool flag6 = this.HuntingLog != null;
			buffer.AddBool(flag6);
			if (flag6)
			{
				buffer.AddInt(this.HuntingLog.Count);
				foreach (KeyValuePair<UniqueId, HuntingLogEntry> keyValuePair5 in this.HuntingLog)
				{
					buffer.AddUniqueId(keyValuePair5.Key);
					keyValuePair5.Value.PackData(buffer);
				}
			}
			this.Settings.PackData(buffer);
			int value = (this.Corpse != null && this.Corpse.Location != null) ? this.Corpse.Location.ZoneId : 0;
			buffer.AddInt(value);
			return buffer;
		}

		// Token: 0x06001EB5 RID: 7861 RVA: 0x0011C840 File Offset: 0x0011AA40
		public BitBuffer ReadData(BitBuffer buffer)
		{
			this.Id = buffer.ReadString();
			this.Title = buffer.ReadNullableString();
			if (buffer.ReadBool())
			{
				int num = buffer.ReadInt();
				this.AvailableTitles = new List<string>(num);
				for (int i = 0; i < num; i++)
				{
					this.AvailableTitles.Add(buffer.ReadString());
				}
			}
			this.Created = buffer.ReadDateTime();
			this.TimePlayed = buffer.ReadNullableTimeSpan();
			int num2 = buffer.ReadInt();
			this.Storage = new Dictionary<ContainerType, ContainerRecord>(num2, default(ContainerTypeComparer));
			for (int j = 0; j < num2; j++)
			{
				ContainerType key = buffer.ReadEnum<ContainerType>();
				ContainerRecord containerRecord = new ContainerRecord();
				containerRecord.ReadData(buffer);
				this.Storage.Add(key, containerRecord);
			}
			num2 = buffer.ReadInt();
			this.Learnables = new Dictionary<ContainerType, LearnableContainerRecord>(num2, default(ContainerTypeComparer));
			for (int k = 0; k < num2; k++)
			{
				ContainerType key2 = buffer.ReadEnum<ContainerType>();
				LearnableContainerRecord learnableContainerRecord = new LearnableContainerRecord();
				learnableContainerRecord.ReadData(buffer);
				this.Learnables.Add(key2, learnableContainerRecord);
			}
			if (buffer.ReadBool())
			{
				num2 = buffer.ReadInt();
				this.ConsumableLastUseTimes = new Dictionary<ConsumableCategory, DateTime>(num2, default(ConsumableCategoryComparer));
				for (int l = 0; l < num2; l++)
				{
					ConsumableCategory key3 = buffer.ReadEnum<ConsumableCategory>();
					DateTime value = buffer.ReadDateTime();
					this.ConsumableLastUseTimes.Add(key3, value);
				}
			}
			if (buffer.ReadBool())
			{
				this.Progression = new PlayerProgressionData();
				this.Progression.ReadData(buffer);
			}
			if (buffer.ReadBool())
			{
				num2 = buffer.ReadInt();
				this.Discoveries = new Dictionary<ZoneId, List<UniqueId>>(num2, default(ZoneIdComparer));
				for (int m = 0; m < num2; m++)
				{
					ZoneId key4 = buffer.ReadEnum<ZoneId>();
					int num3 = buffer.ReadInt();
					List<UniqueId> list = new List<UniqueId>(num3);
					for (int n = 0; n < num3; n++)
					{
						list.Add(buffer.ReadUniqueId());
					}
					this.Discoveries.Add(key4, list);
				}
			}
			if (buffer.ReadBool())
			{
				this.EmberStoneData = new EmberStoneInstanceData();
				this.EmberStoneData.ReadData(buffer);
			}
			if (buffer.ReadBool())
			{
				num2 = buffer.ReadInt();
				this.HuntingLog = new Dictionary<UniqueId, HuntingLogEntry>(num2, default(UniqueIdComparer));
				for (int num4 = 0; num4 < num2; num4++)
				{
					UniqueId key5 = buffer.ReadUniqueId();
					HuntingLogEntry huntingLogEntry = new HuntingLogEntry();
					huntingLogEntry.ReadData(buffer);
					this.HuntingLog.Add(key5, huntingLogEntry);
				}
			}
			if (this.Settings == null)
			{
				this.Settings = new CharacterSettings();
			}
			this.Settings.ReadData(buffer);
			this.CorpseZoneId = buffer.ReadInt();
			return buffer;
		}

		// Token: 0x040023EB RID: 9195
		public static float kStartingPosition = -222f;

		// Token: 0x040023EC RID: 9196
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		[JsonProperty(PropertyName = "_id")]
		public string Id;

		// Token: 0x040023ED RID: 9197
		[BsonRepresentation(BsonType.String)]
		public string UserId;

		// Token: 0x040023EE RID: 9198
		public int WorldId;

		// Token: 0x040023EF RID: 9199
		public int SelectionPositionIndex = -1;

		// Token: 0x040023F0 RID: 9200
		[BsonDateTimeOptions(Representation = BsonType.String)]
		public DateTime Created;

		// Token: 0x040023F1 RID: 9201
		[BsonDateTimeOptions(Representation = BsonType.String)]
		public DateTime Updated;

		// Token: 0x040023F2 RID: 9202
		public string Name;

		// Token: 0x040023F3 RID: 9203
		public string Title;

		// Token: 0x040023F4 RID: 9204
		[BsonIgnoreIfNull]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public List<string> AvailableTitles;

		// Token: 0x040023F5 RID: 9205
		public CharacterVisuals Visuals;

		// Token: 0x040023F6 RID: 9206
		public CharacterVitals Vitals;

		// Token: 0x040023F7 RID: 9207
		public CharacterLocation Location;

		// Token: 0x040023F8 RID: 9208
		public CharacterZoningState ZoningState;

		// Token: 0x040023F9 RID: 9209
		public CharacterSettings Settings;

		// Token: 0x040023FA RID: 9210
		public CharacterCorpse Corpse;

		// Token: 0x040023FC RID: 9212
		public Dictionary<ContainerType, ContainerRecord> Storage;

		// Token: 0x040023FD RID: 9213
		public Dictionary<ContainerType, LearnableContainerRecord> Learnables;

		// Token: 0x040023FE RID: 9214
		[BsonIgnoreIfNull]
		[JsonIgnore]
		public List<ArchetypeInstance> InvalidItems;

		// Token: 0x040023FF RID: 9215
		[BsonIgnoreIfNull]
		[JsonIgnore]
		public List<MerchantBuybackItem> MerchantBuybackItems;

		// Token: 0x04002400 RID: 9216
		[BsonIgnoreIfNull]
		[JsonIgnore]
		public List<MerchantBuybackItem> BagBuybackItems;

		// Token: 0x04002401 RID: 9217
		[BsonIgnoreIfNull]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public Dictionary<ZoneId, List<UniqueId>> Discoveries;

		// Token: 0x04002402 RID: 9218
		[BsonIgnoreIfNull]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public Dictionary<ConsumableCategory, DateTime> ConsumableLastUseTimes;

		// Token: 0x04002403 RID: 9219
		[BsonIgnoreIfNull]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public List<EffectRecord> Effects;

		// Token: 0x04002404 RID: 9220
		[BsonIgnoreIfNull]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public PlayerProgressionData Progression;

		// Token: 0x04002405 RID: 9221
		[BsonIgnoreIfNull]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public EmberStoneInstanceData EmberStoneData;

		// Token: 0x04002406 RID: 9222
		public int HuntingLogVersion = 1;

		// Token: 0x04002407 RID: 9223
		[BsonIgnoreIfNull]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public Dictionary<UniqueId, HuntingLogEntry> HuntingLog;

		// Token: 0x04002408 RID: 9224
		private DateTime? m_corpseCreationTime;

		// Token: 0x04002409 RID: 9225
		[BsonIgnoreIfNull]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public TimeSpan? TimePlayed;

		// Token: 0x0400240B RID: 9227
		private static string kTableName = "characters";

		// Token: 0x0400240C RID: 9228
		[BsonIgnore]
		[JsonIgnore]
		private FilterDefinition<CharacterRecord> m_idFilter;
	}
}

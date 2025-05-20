using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Bson.Serialization.Serializers;
using Newtonsoft.Json;
using SoL.Game;
using SoL.Game.EffectSystem;
using SoL.Game.HuntingLog;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Containers;
using SoL.Networking.Database;
using SoL.UI;

namespace SoL.Utilities
{
	// Token: 0x0200026C RID: 620
	public static class CustomSerialization
	{
		// Token: 0x060013A1 RID: 5025 RVA: 0x000F7354 File Offset: 0x000F5554
		static CustomSerialization()
		{
			BsonSerializer.RegisterSerializer(typeof(float), new FloatBsonSerializer());
			BsonSerializer.RegisterSerializer(typeof(UniqueId), new UniqueIdBsonSerializer());
			BsonSerializer.RegisterSerializer(typeof(TutorialPopupOptions), new TutorialPopupOptionsSerializer());
			BsonSerializer.RegisterSerializer(typeof(ContainerType), new EnumSerializer<ContainerType>(BsonType.String));
			BsonSerializer.RegisterSerializer(typeof(ConsumableCategory), new EnumSerializer<ConsumableCategory>(BsonType.String));
			BsonSerializer.RegisterSerializer(typeof(CharacterColorType), new EnumSerializer<CharacterColorType>(BsonType.String));
			BsonSerializer.RegisterSerializer(typeof(ZoneId), new EnumSerializer<ZoneId>(BsonType.String));
			BsonSerializer.RegisterSerializer(typeof(DamageType), new EnumSerializer<DamageType>(BsonType.Int32));
			BsonSerializer.RegisterSerializer(typeof(StatType), new EnumSerializer<StatType>(BsonType.Int32));
			BsonSerializer.RegisterSerializer(typeof(HuntingLogPerkType), new EnumSerializer<HuntingLogPerkType>(BsonType.Int32));
			BsonSerializer.RegisterSerializer(typeof(AlchemyPowerLevel), new EnumSerializer<AlchemyPowerLevel>(BsonType.Int32));
			BsonClassMap.RegisterClassMap<HuntingLogEntry>(delegate(BsonClassMap<HuntingLogEntry> cm)
			{
				cm.AutoMap();
				DictionaryInterfaceImplementerSerializer<Dictionary<int, HuntingLogPerkType>> serializer = new DictionaryInterfaceImplementerSerializer<Dictionary<int, HuntingLogPerkType>>(DictionaryRepresentation.Document, new Int32Serializer(BsonType.String), BsonSerializer.SerializerRegistry.GetSerializer<HuntingLogPerkType>());
				cm.GetMemberMap<Dictionary<int, HuntingLogPerkType>>((HuntingLogEntry c) => c.ActivePerks).SetSerializer(serializer);
			});
			JsonConvert.DefaultSettings = (() => new JsonSerializerSettings
			{
				Converters = new List<JsonConverter>
				{
					new UniqueIdJsonSerializer()
				}
			});
		}

		// Token: 0x060013A2 RID: 5026 RVA: 0x0004475B File Offset: 0x0004295B
		public static void Initialize()
		{
		}
	}
}

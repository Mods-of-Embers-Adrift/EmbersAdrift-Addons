using System;
using Newtonsoft.Json;

namespace SoL
{
	// Token: 0x02000231 RID: 561
	public class UniqueIdJsonSerializer : JsonConverter
	{
		// Token: 0x060012C9 RID: 4809 RVA: 0x000E7EF8 File Offset: 0x000E60F8
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			writer.WriteValue(((UniqueId)value).Value);
		}

		// Token: 0x060012CA RID: 4810 RVA: 0x0004F60E File Offset: 0x0004D80E
		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			return new UniqueId((string)reader.Value);
		}

		// Token: 0x060012CB RID: 4811 RVA: 0x0004F625 File Offset: 0x0004D825
		public override bool CanConvert(Type objectType)
		{
			return typeof(UniqueId).IsAssignableFrom(objectType) || typeof(UniqueId?).IsAssignableFrom(objectType);
		}
	}
}

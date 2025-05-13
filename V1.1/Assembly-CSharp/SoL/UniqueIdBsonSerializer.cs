using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace SoL
{
	// Token: 0x02000230 RID: 560
	public class UniqueIdBsonSerializer : SerializerBase<UniqueId>
	{
		// Token: 0x060012C6 RID: 4806 RVA: 0x0004F5AB File Offset: 0x0004D7AB
		public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, UniqueId value)
		{
			if (value.IsEmpty)
			{
				context.Writer.WriteNull();
				return;
			}
			context.Writer.WriteString(value.Value);
		}

		// Token: 0x060012C7 RID: 4807 RVA: 0x0004F5D4 File Offset: 0x0004D7D4
		public override UniqueId Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
		{
			if (context.Reader.CurrentBsonType == BsonType.Null)
			{
				context.Reader.ReadNull();
				return UniqueId.Empty;
			}
			return new UniqueId(context.Reader.ReadString());
		}
	}
}

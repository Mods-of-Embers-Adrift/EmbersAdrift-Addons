using System;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace SoL.Utilities
{
	// Token: 0x0200026E RID: 622
	public class FloatBsonSerializer : SerializerBase<float>
	{
		// Token: 0x060013A7 RID: 5031 RVA: 0x0004FD4B File Offset: 0x0004DF4B
		public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, float value)
		{
			context.Writer.WriteDouble((double)value);
		}

		// Token: 0x060013A8 RID: 5032 RVA: 0x0004FD5A File Offset: 0x0004DF5A
		public override float Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
		{
			return (float)context.Reader.ReadDouble();
		}
	}
}

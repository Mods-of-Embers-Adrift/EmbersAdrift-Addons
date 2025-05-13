using System;
using System.Collections.Generic;
using System.Reflection;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace SoL.Utilities
{
	// Token: 0x0200026F RID: 623
	public class BasicStructSerializer<T> : StructSerializerBase<T> where T : struct
	{
		// Token: 0x060013AA RID: 5034 RVA: 0x000F74C0 File Offset: 0x000F56C0
		public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, T value)
		{
			Type nominalType = args.NominalType;
			FieldInfo[] fields = nominalType.GetFields(BindingFlags.Instance | BindingFlags.Public);
			PropertyInfo[] properties = nominalType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
			if (BasicStructSerializer<T>.m_propertyInfo == null)
			{
				BasicStructSerializer<T>.m_propertyInfo = new List<PropertyInfo>();
			}
			else
			{
				BasicStructSerializer<T>.m_propertyInfo.Clear();
			}
			List<PropertyInfo> propertyInfo = BasicStructSerializer<T>.m_propertyInfo;
			foreach (PropertyInfo propertyInfo2 in properties)
			{
				if (propertyInfo2.CanWrite)
				{
					propertyInfo.Add(propertyInfo2);
				}
			}
			IBsonWriter writer = context.Writer;
			writer.WriteStartDocument();
			foreach (FieldInfo fieldInfo in fields)
			{
				writer.WriteName(fieldInfo.Name);
				BsonSerializer.Serialize(writer, fieldInfo.FieldType, fieldInfo.GetValue(value), null, default(BsonSerializationArgs));
			}
			foreach (PropertyInfo propertyInfo3 in propertyInfo)
			{
				writer.WriteName(propertyInfo3.Name);
				BsonSerializer.Serialize(writer, propertyInfo3.PropertyType, propertyInfo3.GetValue(value, null), null, default(BsonSerializationArgs));
			}
			writer.WriteEndDocument();
		}

		// Token: 0x060013AB RID: 5035 RVA: 0x000F7604 File Offset: 0x000F5804
		public override T Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
		{
			object obj = Activator.CreateInstance<T>();
			Type nominalType = args.NominalType;
			IBsonReader reader = context.Reader;
			reader.ReadStartDocument();
			while (reader.ReadBsonType() != BsonType.EndOfDocument)
			{
				if (BasicStructSerializer<T>.m_nameDecoder == null)
				{
					BasicStructSerializer<T>.m_nameDecoder = new Utf8NameDecoder();
				}
				string name = reader.ReadName(BasicStructSerializer<T>.m_nameDecoder);
				FieldInfo field = nominalType.GetField(name);
				if (field != null)
				{
					object value = BsonSerializer.Deserialize(reader, field.FieldType, null);
					field.SetValue(obj, value);
				}
				PropertyInfo property = nominalType.GetProperty(name);
				if (property != null)
				{
					object value2 = BsonSerializer.Deserialize(reader, property.PropertyType, null);
					property.SetValue(obj, value2, null);
				}
			}
			reader.ReadEndDocument();
			return (T)((object)obj);
		}

		// Token: 0x04001BED RID: 7149
		private static List<PropertyInfo> m_propertyInfo;

		// Token: 0x04001BEE RID: 7150
		private static Utf8NameDecoder m_nameDecoder;
	}
}

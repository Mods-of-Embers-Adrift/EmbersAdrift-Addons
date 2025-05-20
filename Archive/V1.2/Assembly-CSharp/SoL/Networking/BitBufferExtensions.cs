using System;
using System.Collections.Generic;
using System.Text;
using ENet;
using NetStack.Quantization;
using NetStack.Serialization;
using SoL.Networking.Objects;
using SoL.Networking.Replication;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Networking
{
	// Token: 0x020003CD RID: 973
	public static class BitBufferExtensions
	{
		// Token: 0x06001A08 RID: 6664 RVA: 0x000545FB File Offset: 0x000527FB
		public static BitBuffer GetFromPool()
		{
			if (BitBufferExtensions.m_pool.Count > 0)
			{
				return BitBufferExtensions.m_pool.Pop();
			}
			return new BitBuffer(375);
		}

		// Token: 0x06001A09 RID: 6665 RVA: 0x0005461F File Offset: 0x0005281F
		public static void ReturnToPool(this BitBuffer buffer)
		{
			buffer.Clear();
			if (BitBufferExtensions.m_pool.Count < 128)
			{
				BitBufferExtensions.m_pool.Push(buffer);
			}
		}

		// Token: 0x06001A0B RID: 6667 RVA: 0x00107E68 File Offset: 0x00106068
		public static Packet GetPacketFromBuffer(this BitBuffer buffer, PacketFlags flags = PacketFlags.None)
		{
			byte[] byteArray = ByteArrayPool.GetByteArray(buffer.Length);
			buffer.ToArray(byteArray);
			Packet result = default(Packet);
			result.Create(byteArray, flags);
			byteArray.ReturnToPool();
			return result;
		}

		// Token: 0x06001A0C RID: 6668 RVA: 0x00054654 File Offset: 0x00052854
		public static Packet GetPacketFromBuffer_ReturnBufferToPool(this BitBuffer buffer, PacketFlags flags = PacketFlags.None)
		{
			Packet packetFromBuffer = buffer.GetPacketFromBuffer(flags);
			buffer.ReturnToPool();
			return packetFromBuffer;
		}

		// Token: 0x06001A0D RID: 6669 RVA: 0x00054663 File Offset: 0x00052863
		public static BitBuffer AddHeader(this BitBuffer buffer, Peer peer, OpCodes opCode, bool clearBuffer = true)
		{
			if (clearBuffer)
			{
				buffer.Clear();
			}
			buffer.AddUShort((ushort)opCode).AddUInt(peer.ID);
			return buffer;
		}

		// Token: 0x06001A0E RID: 6670 RVA: 0x00107EA4 File Offset: 0x001060A4
		public static BitBuffer AddHeader(this BitBuffer buffer, NetworkEntity entity, OpCodes opCode, bool clearBuffer = true)
		{
			if (clearBuffer)
			{
				buffer.Clear();
			}
			buffer.AddUShort((ushort)opCode).AddUInt(entity.NetworkId.Value);
			return buffer;
		}

		// Token: 0x06001A0F RID: 6671 RVA: 0x00107ED8 File Offset: 0x001060D8
		public static PacketHeader GetHeader(this BitBuffer buffer)
		{
			return new PacketHeader
			{
				OpCode = (OpCodes)buffer.ReadUShort(),
				Id = buffer.ReadUInt()
			};
		}

		// Token: 0x06001A10 RID: 6672 RVA: 0x00054683 File Offset: 0x00052883
		public static BitBuffer AddInitialSyncData(this BitBuffer buffer, ISynchronizedVariable value)
		{
			return value.PackInitialData(buffer);
		}

		// Token: 0x06001A11 RID: 6673 RVA: 0x0005468C File Offset: 0x0005288C
		public static BitBuffer AddSyncData(this BitBuffer buffer, ISynchronizedVariable value)
		{
			return value.PackData(buffer);
		}

		// Token: 0x06001A12 RID: 6674 RVA: 0x00054695 File Offset: 0x00052895
		public static void ReadSyncVar(this BitBuffer buffer, ISynchronizedVariable var)
		{
			var.ReadData(buffer);
		}

		// Token: 0x06001A13 RID: 6675 RVA: 0x0005469F File Offset: 0x0005289F
		public static BitBuffer AddInitialAnimatorParameter(this BitBuffer buffer, AnimatorParameter value)
		{
			return value.PackInitialData(buffer);
		}

		// Token: 0x06001A14 RID: 6676 RVA: 0x000546A8 File Offset: 0x000528A8
		public static BitBuffer AddAnimatorParameter(this BitBuffer buffer, AnimatorParameter value)
		{
			return value.PackData(buffer);
		}

		// Token: 0x06001A15 RID: 6677 RVA: 0x000546B1 File Offset: 0x000528B1
		public static BitBuffer AddUShort(this BitBuffer buffer, ushort value)
		{
			buffer.AddUInt((uint)value);
			return buffer;
		}

		// Token: 0x06001A16 RID: 6678 RVA: 0x000546BC File Offset: 0x000528BC
		public static ushort ReadUShort(this BitBuffer buffer)
		{
			return (ushort)buffer.ReadUInt();
		}

		// Token: 0x06001A17 RID: 6679 RVA: 0x00107F08 File Offset: 0x00106108
		public static BitBuffer AddFloat(this BitBuffer buffer, float value)
		{
			buffer.AddUInt(new UIntFloat
			{
				floatValue = value
			}.uintValue);
			return buffer;
		}

		// Token: 0x06001A18 RID: 6680 RVA: 0x00107F34 File Offset: 0x00106134
		public static float ReadFloat(this BitBuffer buffer)
		{
			return new UIntFloat
			{
				uintValue = buffer.ReadUInt()
			}.floatValue;
		}

		// Token: 0x06001A19 RID: 6681 RVA: 0x00107F5C File Offset: 0x0010615C
		public static BitBuffer AddASCII(this BitBuffer buffer, string value)
		{
			if (value == null)
			{
				value = "";
			}
			int length = value.Length;
			buffer.AddInt(length);
			for (int i = 0; i < length; i++)
			{
				buffer.AddByte((byte)value[i]);
			}
			return buffer;
		}

		// Token: 0x06001A1A RID: 6682 RVA: 0x00107FA0 File Offset: 0x001061A0
		public static string ReadASCII(this BitBuffer buffer)
		{
			StringBuilder fromPool = StringBuilderExtensions.GetFromPool();
			int num = buffer.ReadInt();
			for (int i = 0; i < num; i++)
			{
				fromPool.Insert(i, (char)buffer.ReadByte());
			}
			return fromPool.ToString_ReturnToPool();
		}

		// Token: 0x06001A1B RID: 6683 RVA: 0x00107FDC File Offset: 0x001061DC
		public static BitBuffer AddVector2(this BitBuffer buffer, Vector2 value, BoundedRange[] range)
		{
			QuantizedVector2 quantizedVector = BoundedRange.Quantize(value, range);
			buffer.AddUInt(quantizedVector.x).AddUInt(quantizedVector.y);
			return buffer;
		}

		// Token: 0x06001A1C RID: 6684 RVA: 0x000546C5 File Offset: 0x000528C5
		public static Vector3 ReadVector2(this BitBuffer buffer, BoundedRange[] range)
		{
			return BoundedRange.Dequantize(new QuantizedVector2(buffer.ReadUInt(), buffer.ReadUInt()), range);
		}

		// Token: 0x06001A1D RID: 6685 RVA: 0x0010800C File Offset: 0x0010620C
		public static BitBuffer AddVector3(this BitBuffer buffer, Vector3 value, BoundedRange[] range)
		{
			QuantizedVector3 quantizedVector = BoundedRange.Quantize(value, range);
			buffer.AddUInt(quantizedVector.x).AddUInt(quantizedVector.y).AddUInt(quantizedVector.z);
			return buffer;
		}

		// Token: 0x06001A1E RID: 6686 RVA: 0x000546E3 File Offset: 0x000528E3
		public static Vector3 ReadVector3(this BitBuffer buffer, BoundedRange[] range)
		{
			return BoundedRange.Dequantize(new QuantizedVector3(buffer.ReadUInt(), buffer.ReadUInt(), buffer.ReadUInt()), range);
		}

		// Token: 0x06001A1F RID: 6687 RVA: 0x00108048 File Offset: 0x00106248
		public static BitBuffer AddVector4(this BitBuffer buffer, Vector4 value, BoundedRange[] range)
		{
			QuantizedVector4 quantizedVector = BoundedRange.Quantize(value, range);
			buffer.AddUInt(quantizedVector.x).AddUInt(quantizedVector.y).AddUInt(quantizedVector.z).AddUInt(quantizedVector.w);
			return buffer;
		}

		// Token: 0x06001A20 RID: 6688 RVA: 0x00054702 File Offset: 0x00052902
		public static Vector4 ReadVector4(this BitBuffer buffer, BoundedRange[] range)
		{
			return BoundedRange.Dequantize(new QuantizedVector4(buffer.ReadUInt(), buffer.ReadUInt(), buffer.ReadUInt(), buffer.ReadUInt()), range);
		}

		// Token: 0x06001A21 RID: 6689 RVA: 0x0010808C File Offset: 0x0010628C
		public static BitBuffer AddQuaternion(this BitBuffer buffer, Quaternion value)
		{
			QuantizedQuaternion quantizedQuaternion = SmallestThree.Quantize(value, 12);
			buffer.AddUInt(quantizedQuaternion.m).AddUInt(quantizedQuaternion.a).AddUInt(quantizedQuaternion.b).AddUInt(quantizedQuaternion.c);
			return buffer;
		}

		// Token: 0x06001A22 RID: 6690 RVA: 0x00054727 File Offset: 0x00052927
		public static Quaternion ReadQuaternion(this BitBuffer buffer)
		{
			return SmallestThree.Dequantize(new QuantizedQuaternion(buffer.ReadUInt(), buffer.ReadUInt(), buffer.ReadUInt(), buffer.ReadUInt()), 12);
		}

		// Token: 0x06001A23 RID: 6691 RVA: 0x0005474D File Offset: 0x0005294D
		public static BitBuffer AddInitialState(this BitBuffer buffer, NetworkEntity netEntity)
		{
			return netEntity.AddInitialState(buffer);
		}

		// Token: 0x06001A24 RID: 6692 RVA: 0x00054756 File Offset: 0x00052956
		public static BitBuffer AddUniqueId(this BitBuffer buffer, UniqueId id)
		{
			if (id.Value == null)
			{
				id = new UniqueId("");
			}
			buffer.AddString(id.Value);
			return buffer;
		}

		// Token: 0x06001A25 RID: 6693 RVA: 0x0005477C File Offset: 0x0005297C
		public static UniqueId ReadUniqueId(this BitBuffer buffer)
		{
			return new UniqueId(buffer.ReadString());
		}

		// Token: 0x06001A26 RID: 6694 RVA: 0x001080D4 File Offset: 0x001062D4
		public static BitBuffer AddNullableUniqueId(this BitBuffer buffer, UniqueId? id)
		{
			bool flag = id != null;
			buffer.AddBool(flag);
			if (flag)
			{
				buffer.AddUniqueId(id.Value);
			}
			return buffer;
		}

		// Token: 0x06001A27 RID: 6695 RVA: 0x00108104 File Offset: 0x00106304
		public static UniqueId? ReadNullableUniqueId(this BitBuffer buffer)
		{
			if (buffer.ReadBool())
			{
				return new UniqueId?(buffer.ReadUniqueId());
			}
			return null;
		}

		// Token: 0x06001A28 RID: 6696 RVA: 0x00108130 File Offset: 0x00106330
		public static BitBuffer AddNullableString(this BitBuffer buffer, string value)
		{
			bool flag = !string.IsNullOrEmpty(value);
			buffer.AddBool(flag);
			if (flag)
			{
				buffer.AddString(value);
			}
			return buffer;
		}

		// Token: 0x06001A29 RID: 6697 RVA: 0x00054789 File Offset: 0x00052989
		public static string ReadNullableString(this BitBuffer buffer)
		{
			if (!buffer.ReadBool())
			{
				return string.Empty;
			}
			return buffer.ReadString();
		}

		// Token: 0x06001A2A RID: 6698 RVA: 0x0010815C File Offset: 0x0010635C
		public static BitBuffer AddNullableFloat(this BitBuffer buffer, float? value)
		{
			bool flag = value != null;
			buffer.AddBool(flag);
			if (flag)
			{
				buffer.AddFloat(value.Value);
			}
			return buffer;
		}

		// Token: 0x06001A2B RID: 6699 RVA: 0x0010818C File Offset: 0x0010638C
		public static float? ReadNullableFloat(this BitBuffer buffer)
		{
			float? result = null;
			if (buffer.ReadBool())
			{
				result = new float?(buffer.ReadFloat());
			}
			return result;
		}

		// Token: 0x06001A2C RID: 6700 RVA: 0x001081B8 File Offset: 0x001063B8
		public static BitBuffer AddNullableInt(this BitBuffer buffer, int? value)
		{
			bool flag = value != null;
			buffer.AddBool(flag);
			if (flag)
			{
				buffer.AddInt(value.Value);
			}
			return buffer;
		}

		// Token: 0x06001A2D RID: 6701 RVA: 0x001081E8 File Offset: 0x001063E8
		public static int? ReadNullableInt(this BitBuffer buffer)
		{
			int? result = null;
			if (buffer.ReadBool())
			{
				result = new int?(buffer.ReadInt());
			}
			return result;
		}

		// Token: 0x06001A2E RID: 6702 RVA: 0x00108214 File Offset: 0x00106414
		public static BitBuffer AddNullableUInt(this BitBuffer buffer, uint? value)
		{
			bool flag = value != null;
			buffer.AddBool(flag);
			if (flag)
			{
				buffer.AddUInt(value.Value);
			}
			return buffer;
		}

		// Token: 0x06001A2F RID: 6703 RVA: 0x00108244 File Offset: 0x00106444
		public static uint? ReadNullableUInt(this BitBuffer buffer)
		{
			uint? result = null;
			if (buffer.ReadBool())
			{
				result = new uint?(buffer.ReadUInt());
			}
			return result;
		}

		// Token: 0x06001A30 RID: 6704 RVA: 0x00108270 File Offset: 0x00106470
		public static BitBuffer AddNullableUlong(this BitBuffer buffer, ulong? value)
		{
			bool flag = value != null;
			buffer.AddBool(flag);
			if (flag)
			{
				buffer.AddULong(value.Value);
			}
			return buffer;
		}

		// Token: 0x06001A31 RID: 6705 RVA: 0x001082A0 File Offset: 0x001064A0
		public static ulong? ReadNullableUlong(this BitBuffer buffer)
		{
			ulong? result = null;
			if (buffer.ReadBool())
			{
				result = new ulong?(buffer.ReadULong());
			}
			return result;
		}

		// Token: 0x06001A32 RID: 6706 RVA: 0x001082CC File Offset: 0x001064CC
		public static BitBuffer AddNullableByte(this BitBuffer buffer, byte? value)
		{
			bool flag = value != null;
			buffer.AddBool(flag);
			if (flag)
			{
				buffer.AddByte(value.Value);
			}
			return buffer;
		}

		// Token: 0x06001A33 RID: 6707 RVA: 0x001082FC File Offset: 0x001064FC
		public static byte? ReadNullableByte(this BitBuffer buffer)
		{
			byte? result = null;
			if (buffer.ReadBool())
			{
				result = new byte?(buffer.ReadByte());
			}
			return result;
		}

		// Token: 0x06001A34 RID: 6708 RVA: 0x00108328 File Offset: 0x00106528
		public static BitBuffer AddNullableDateTime(this BitBuffer buffer, DateTime? time)
		{
			bool flag = time != null;
			buffer.AddBool(flag);
			if (flag)
			{
				buffer.AddDateTime(time.Value);
			}
			return buffer;
		}

		// Token: 0x06001A35 RID: 6709 RVA: 0x00108358 File Offset: 0x00106558
		public static DateTime? ReadNullableDateTime(this BitBuffer buffer)
		{
			DateTime? result = null;
			if (buffer.ReadBool())
			{
				result = new DateTime?(buffer.ReadDateTime());
			}
			return result;
		}

		// Token: 0x06001A36 RID: 6710 RVA: 0x0005479F File Offset: 0x0005299F
		public static BitBuffer AddDateTime(this BitBuffer buffer, DateTime time)
		{
			buffer.AddLong(time.Ticks);
			return buffer;
		}

		// Token: 0x06001A37 RID: 6711 RVA: 0x000547B0 File Offset: 0x000529B0
		public static DateTime ReadDateTime(this BitBuffer buffer)
		{
			return new DateTime(buffer.ReadLong());
		}

		// Token: 0x06001A38 RID: 6712 RVA: 0x00108384 File Offset: 0x00106584
		public static BitBuffer AddNullableTimeSpan(this BitBuffer buffer, TimeSpan? timeSpan)
		{
			bool flag = timeSpan != null;
			buffer.AddBool(flag);
			if (flag)
			{
				buffer.AddTimeSpan(timeSpan.Value);
			}
			return buffer;
		}

		// Token: 0x06001A39 RID: 6713 RVA: 0x001083B4 File Offset: 0x001065B4
		public static TimeSpan? ReadNullableTimeSpan(this BitBuffer buffer)
		{
			TimeSpan? result = null;
			if (buffer.ReadBool())
			{
				result = new TimeSpan?(buffer.ReadTimeSpan());
			}
			return result;
		}

		// Token: 0x06001A3A RID: 6714 RVA: 0x000547BD File Offset: 0x000529BD
		public static BitBuffer AddTimeSpan(this BitBuffer buffer, TimeSpan timeSpan)
		{
			buffer.AddLong(timeSpan.Ticks);
			return buffer;
		}

		// Token: 0x06001A3B RID: 6715 RVA: 0x000547CE File Offset: 0x000529CE
		public static TimeSpan ReadTimeSpan(this BitBuffer buffer)
		{
			return new TimeSpan(buffer.ReadLong());
		}

		// Token: 0x06001A3C RID: 6716 RVA: 0x001083E0 File Offset: 0x001065E0
		public static BitBuffer AddEnum<T>(this BitBuffer buffer, T value) where T : Enum, IConvertible
		{
			UnderlyingEnumType underlyingEnumType = EnumExtensions.GetUnderlyingEnumType<T>();
			buffer.AddEnum(value, underlyingEnumType);
			return buffer;
		}

		// Token: 0x06001A3D RID: 6717 RVA: 0x00108400 File Offset: 0x00106600
		public static BitBuffer AddEnum<T>(this BitBuffer buffer, T value, UnderlyingEnumType enumType) where T : Enum, IConvertible
		{
			switch (enumType)
			{
			case UnderlyingEnumType.Byte:
				buffer.AddByte(value.ToByte(null));
				break;
			case UnderlyingEnumType.Short:
				buffer.AddShort(value.ToInt16(null));
				break;
			case UnderlyingEnumType.UShort:
				buffer.AddUShort(value.ToUInt16(null));
				break;
			case UnderlyingEnumType.Int:
				buffer.AddInt(value.ToInt32(null));
				break;
			case UnderlyingEnumType.UInt:
				buffer.AddUInt(value.ToUInt32(null));
				break;
			case UnderlyingEnumType.Long:
				buffer.AddLong(value.ToInt64(null));
				break;
			case UnderlyingEnumType.ULong:
				buffer.AddULong(value.ToUInt64(null));
				break;
			}
			return buffer;
		}

		// Token: 0x06001A3E RID: 6718 RVA: 0x001084E0 File Offset: 0x001066E0
		public static T ReadEnum<T>(this BitBuffer buffer) where T : Enum, IConvertible
		{
			UnderlyingEnumType underlyingEnumType = EnumExtensions.GetUnderlyingEnumType<T>();
			return buffer.ReadEnum(underlyingEnumType);
		}

		// Token: 0x06001A3F RID: 6719 RVA: 0x001084FC File Offset: 0x001066FC
		public static T ReadEnum<T>(this BitBuffer buffer, UnderlyingEnumType enumType) where T : Enum, IConvertible
		{
			switch (enumType)
			{
			case UnderlyingEnumType.Byte:
				return (T)((object)buffer.ReadByte());
			case UnderlyingEnumType.Short:
				return (T)((object)buffer.ReadShort());
			case UnderlyingEnumType.UShort:
				return (T)((object)buffer.ReadUShort());
			case UnderlyingEnumType.Int:
				return (T)((object)buffer.ReadInt());
			case UnderlyingEnumType.UInt:
				return (T)((object)buffer.ReadUInt());
			case UnderlyingEnumType.Long:
				return (T)((object)buffer.ReadLong());
			case UnderlyingEnumType.ULong:
				return (T)((object)buffer.ReadULong());
			}
			throw new Exception();
		}

		// Token: 0x0400212C RID: 8492
		public const int kOffset = 0;

		// Token: 0x0400212D RID: 8493
		private const int kMaxPoolSize = 128;

		// Token: 0x0400212E RID: 8494
		private static readonly Stack<BitBuffer> m_pool = new Stack<BitBuffer>(128);
	}
}

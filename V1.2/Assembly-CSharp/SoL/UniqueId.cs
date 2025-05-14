using System;
using System.Text;
using MongoDB.Bson;
using NetStack.Serialization;
using Newtonsoft.Json;
using SoL.Networking;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL
{
	// Token: 0x0200022E RID: 558
	[Serializable]
	public struct UniqueId : IEquatable<UniqueId>, INetworkSerializable
	{
		// Token: 0x170004DE RID: 1246
		// (get) Token: 0x060012AD RID: 4781 RVA: 0x0004F4BB File Offset: 0x0004D6BB
		public string Value
		{
			get
			{
				return this.m_value;
			}
		}

		// Token: 0x170004DF RID: 1247
		// (get) Token: 0x060012AE RID: 4782 RVA: 0x0004F4C3 File Offset: 0x0004D6C3
		[JsonIgnore]
		public bool IsEmpty
		{
			get
			{
				return string.IsNullOrEmpty(this.Value);
			}
		}

		// Token: 0x060012AF RID: 4783 RVA: 0x0004F4D0 File Offset: 0x0004D6D0
		public UniqueId(string value)
		{
			this.m_value = value;
			this.m_binaryValue = new byte[16];
			this.ParseIntoBinary();
		}

		// Token: 0x060012B0 RID: 4784 RVA: 0x000E7B90 File Offset: 0x000E5D90
		public UniqueId(byte[] value)
		{
			if (value == null)
			{
				this.m_value = null;
				this.m_binaryValue = new byte[16];
				return;
			}
			this.m_value = null;
			if (value.Length != 16)
			{
				this.m_binaryValue = new byte[16];
				for (int i = 0; i < Math.Min(value.Length, this.m_binaryValue.Length); i++)
				{
					this.m_binaryValue[i] = value[i];
				}
			}
			else
			{
				this.m_binaryValue = value;
			}
			this.ParseIntoString(false);
		}

		// Token: 0x170004E0 RID: 1248
		// (get) Token: 0x060012B1 RID: 4785 RVA: 0x0004F4EC File Offset: 0x0004D6EC
		public static UniqueId Empty
		{
			get
			{
				return new UniqueId(null);
			}
		}

		// Token: 0x060012B2 RID: 4786 RVA: 0x000E7C08 File Offset: 0x000E5E08
		public static UniqueId GenerateFromGuid()
		{
			return new UniqueId(Guid.NewGuid().ToString());
		}

		// Token: 0x060012B3 RID: 4787 RVA: 0x000E7C30 File Offset: 0x000E5E30
		public static UniqueId GenerateFromObjectId()
		{
			return new UniqueId(ObjectId.GenerateNewId().ToString());
		}

		// Token: 0x060012B4 RID: 4788 RVA: 0x000E7C58 File Offset: 0x000E5E58
		private void ParseIntoBinary()
		{
			if (this.m_binaryValue == null || this.m_binaryValue.Length == 0)
			{
				this.m_binaryValue = new byte[16];
			}
			if (string.IsNullOrEmpty(this.m_value))
			{
				return;
			}
			int num = -1;
			int num2 = 0;
			foreach (char c in this.m_value)
			{
				if (c != '-')
				{
					if ((c < '0' || c > '9') && (c < 'A' || c > 'F') && (c < 'a' || c > 'f'))
					{
						break;
					}
					int num3 = (int)char.ToLower(c);
					if (num == -1)
					{
						num = (int)((byte)((c <= '9') ? (num3 & -49) : ((num3 & -97) + 9)));
					}
					else
					{
						int num4 = (int)((byte)((c <= '9') ? (num3 & -49) : ((num3 & -97) + 9)));
						this.m_binaryValue[num2] = (byte)(num4 | num << 4);
						num = -1;
						num2++;
					}
				}
			}
		}

		// Token: 0x060012B5 RID: 4789 RVA: 0x000E7D40 File Offset: 0x000E5F40
		private void ParseIntoString(bool isUuidv4 = false)
		{
			bool flag = false;
			bool flag2 = this.m_binaryValue[12] == 0 && this.m_binaryValue[13] == 0 && this.m_binaryValue[14] == 0 && this.m_binaryValue[15] == 0;
			StringBuilder fromPool = StringBuilderExtensions.GetFromPool();
			for (int i = 0; i < (flag2 ? 12 : 16); i++)
			{
				if (this.m_binaryValue[i] > 0)
				{
					flag = true;
				}
				int num = (int)(this.m_binaryValue[i] / 16);
				int num2 = (int)(this.m_binaryValue[i] % 16);
				fromPool.Append((num <= 9) ? ((char)(num + 48)) : ((char)(num + 87)));
				fromPool.Append((num2 <= 9) ? ((char)(num2 + 48)) : ((char)(num2 + 87)));
				if (isUuidv4 && !flag2 && (i == 3 || i == 5 || i == 7 || i == 9))
				{
					fromPool.Append('-');
				}
			}
			if (flag)
			{
				this.m_value = fromPool.ToString_ReturnToPool();
				return;
			}
			fromPool.ReturnToPool();
		}

		// Token: 0x060012B6 RID: 4790 RVA: 0x0004F4BB File Offset: 0x0004D6BB
		public override string ToString()
		{
			return this.m_value;
		}

		// Token: 0x060012B7 RID: 4791 RVA: 0x0004F4F4 File Offset: 0x0004D6F4
		public byte[] ToBytes()
		{
			return this.m_binaryValue;
		}

		// Token: 0x060012B8 RID: 4792 RVA: 0x000E7E34 File Offset: 0x000E6034
		public BitBuffer PackData(BitBuffer buffer)
		{
			bool flag = !this.IsEmpty;
			buffer.AddBool(flag);
			if (flag)
			{
				buffer.AddString(this.m_value);
			}
			return buffer;
		}

		// Token: 0x060012B9 RID: 4793 RVA: 0x0004F4FC File Offset: 0x0004D6FC
		public BitBuffer ReadData(BitBuffer buffer)
		{
			if (buffer.ReadBool())
			{
				this.m_value = buffer.ReadString();
				this.ParseIntoBinary();
			}
			return buffer;
		}

		// Token: 0x060012BA RID: 4794 RVA: 0x000E7E64 File Offset: 0x000E6064
		public BitBuffer PackData_Binary(BitBuffer buffer)
		{
			if (this.m_binaryValue == null || this.m_binaryValue.Length == 0)
			{
				this.ParseIntoBinary();
			}
			for (int i = 0; i < 16; i++)
			{
				buffer.AddByte(this.m_binaryValue[i]);
			}
			return buffer;
		}

		// Token: 0x060012BB RID: 4795 RVA: 0x000E7EA8 File Offset: 0x000E60A8
		public BitBuffer ReadData_Binary(BitBuffer buffer, bool isUuidv4 = false)
		{
			if (this.m_binaryValue == null || this.m_binaryValue.Length == 0)
			{
				this.m_binaryValue = new byte[16];
			}
			for (int i = 0; i < 16; i++)
			{
				this.m_binaryValue[i] = buffer.ReadByte();
			}
			this.ParseIntoString(isUuidv4);
			return buffer;
		}

		// Token: 0x060012BC RID: 4796 RVA: 0x0004F519 File Offset: 0x0004D719
		BitBuffer INetworkSerializable.PackData(BitBuffer buffer)
		{
			return this.PackData(buffer);
		}

		// Token: 0x060012BD RID: 4797 RVA: 0x0004F522 File Offset: 0x0004D722
		BitBuffer INetworkSerializable.ReadData(BitBuffer buffer)
		{
			return this.ReadData(buffer);
		}

		// Token: 0x060012BE RID: 4798 RVA: 0x0004F52B File Offset: 0x0004D72B
		public bool Equals(UniqueId other)
		{
			return string.Equals(this.m_value, other.m_value);
		}

		// Token: 0x060012BF RID: 4799 RVA: 0x0004F53E File Offset: 0x0004D73E
		public override bool Equals(object obj)
		{
			return obj != null && obj is UniqueId && this.Equals((UniqueId)obj);
		}

		// Token: 0x060012C0 RID: 4800 RVA: 0x0004F55B File Offset: 0x0004D75B
		public override int GetHashCode()
		{
			if (this.m_value == null)
			{
				return 0;
			}
			return this.m_value.GetHashCode();
		}

		// Token: 0x060012C1 RID: 4801 RVA: 0x0004F572 File Offset: 0x0004D772
		public static bool operator ==(UniqueId a, UniqueId b)
		{
			return a.Equals(b);
		}

		// Token: 0x060012C2 RID: 4802 RVA: 0x0004F57C File Offset: 0x0004D77C
		public static bool operator !=(UniqueId a, UniqueId b)
		{
			return !a.Equals(b);
		}

		// Token: 0x060012C3 RID: 4803 RVA: 0x0004F589 File Offset: 0x0004D789
		public static implicit operator string(UniqueId value)
		{
			return value.Value;
		}

		// Token: 0x0400107C RID: 4220
		private const int kBinarySize = 16;

		// Token: 0x0400107D RID: 4221
		[SerializeField]
		private string m_value;

		// Token: 0x0400107E RID: 4222
		[NonSerialized]
		private byte[] m_binaryValue;
	}
}

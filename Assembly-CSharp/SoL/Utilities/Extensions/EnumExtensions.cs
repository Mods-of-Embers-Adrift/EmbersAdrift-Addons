using System;
using System.Collections.Generic;
using System.Text;

namespace SoL.Utilities.Extensions
{
	// Token: 0x02000330 RID: 816
	public static class EnumExtensions
	{
		// Token: 0x0600165E RID: 5726 RVA: 0x001004DC File Offset: 0x000FE6DC
		public static UnderlyingEnumType GetUnderlyingEnumType<T>() where T : Enum, IConvertible
		{
			if (EnumExtensions.m_enums == null)
			{
				EnumExtensions.m_enums = new Dictionary<Type, UnderlyingEnumType>();
			}
			Type typeFromHandle = typeof(T);
			UnderlyingEnumType underlyingEnumType = UnderlyingEnumType.None;
			if (EnumExtensions.m_enums.TryGetValue(typeFromHandle, out underlyingEnumType))
			{
				return underlyingEnumType;
			}
			Type underlyingType = Enum.GetUnderlyingType(typeFromHandle);
			if (underlyingType == typeof(byte))
			{
				underlyingEnumType = UnderlyingEnumType.Byte;
			}
			else if (underlyingType == typeof(short))
			{
				underlyingEnumType = UnderlyingEnumType.Short;
			}
			else if (underlyingType == typeof(ushort))
			{
				underlyingEnumType = UnderlyingEnumType.UShort;
			}
			else if (underlyingType == typeof(int))
			{
				underlyingEnumType = UnderlyingEnumType.Int;
			}
			else if (underlyingType == typeof(uint))
			{
				underlyingEnumType = UnderlyingEnumType.UInt;
			}
			else if (underlyingType == typeof(long))
			{
				underlyingEnumType = UnderlyingEnumType.Long;
			}
			else if (underlyingType == typeof(ulong))
			{
				underlyingEnumType = UnderlyingEnumType.ULong;
			}
			if (underlyingEnumType == UnderlyingEnumType.None)
			{
				throw new Exception(string.Format("Invalid underlying type! {0}", typeFromHandle));
			}
			EnumExtensions.m_enums.Add(typeFromHandle, underlyingEnumType);
			return underlyingEnumType;
		}

		// Token: 0x0600165F RID: 5727 RVA: 0x001005DC File Offset: 0x000FE7DC
		public static string ToStringWithSpaces(this Enum enumValue)
		{
			string text = enumValue.ToString();
			StringBuilder fromPool = StringBuilderExtensions.GetFromPool();
			for (int i = 0; i < text.Length; i++)
			{
				char c = text[i];
				if (char.IsUpper(c) && i != 0 && text[i - 1] != ' ')
				{
					fromPool.Append(' ');
				}
				fromPool.Append(c);
			}
			return fromPool.ToString_ReturnToPool();
		}

		// Token: 0x04001E57 RID: 7767
		private static Dictionary<Type, UnderlyingEnumType> m_enums;
	}
}

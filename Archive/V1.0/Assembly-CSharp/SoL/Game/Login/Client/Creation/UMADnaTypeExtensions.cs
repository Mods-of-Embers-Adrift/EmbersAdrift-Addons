using System;
using System.Collections.Generic;

namespace SoL.Game.Login.Client.Creation
{
	// Token: 0x02000B54 RID: 2900
	public static class UMADnaTypeExtensions
	{
		// Token: 0x170014D8 RID: 5336
		// (get) Token: 0x06005946 RID: 22854 RVA: 0x001E97DC File Offset: 0x001E79DC
		public static Dictionary<string, UMADnaType> DnaTypeDict
		{
			get
			{
				if (UMADnaTypeExtensions.m_strToType == null)
				{
					UMADnaTypeExtensions.m_strToType = new Dictionary<string, UMADnaType>();
					UMADnaType[] array = (UMADnaType[])Enum.GetValues(typeof(UMADnaType));
					for (int i = 0; i < array.Length; i++)
					{
						UMADnaTypeExtensions.m_strToType.Add(array[i].ToString(), array[i]);
					}
				}
				return UMADnaTypeExtensions.m_strToType;
			}
		}

		// Token: 0x04004E9B RID: 20123
		private static Dictionary<string, UMADnaType> m_strToType;
	}
}

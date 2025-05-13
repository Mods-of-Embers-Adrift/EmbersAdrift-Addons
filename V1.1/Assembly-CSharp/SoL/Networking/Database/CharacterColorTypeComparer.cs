using System;
using System.Collections.Generic;

namespace SoL.Networking.Database
{
	// Token: 0x02000429 RID: 1065
	public struct CharacterColorTypeComparer : IEqualityComparer<CharacterColorType>
	{
		// Token: 0x06001E82 RID: 7810 RVA: 0x0004FB72 File Offset: 0x0004DD72
		public bool Equals(CharacterColorType x, CharacterColorType y)
		{
			return x == y;
		}

		// Token: 0x06001E83 RID: 7811 RVA: 0x00049A92 File Offset: 0x00047C92
		public int GetHashCode(CharacterColorType obj)
		{
			return (int)obj;
		}
	}
}

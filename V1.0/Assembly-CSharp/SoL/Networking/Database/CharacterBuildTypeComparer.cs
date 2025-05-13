using System;
using System.Collections.Generic;

namespace SoL.Networking.Database
{
	// Token: 0x02000426 RID: 1062
	public struct CharacterBuildTypeComparer : IEqualityComparer<CharacterBuildType>
	{
		// Token: 0x06001E7B RID: 7803 RVA: 0x0004FB72 File Offset: 0x0004DD72
		public bool Equals(CharacterBuildType x, CharacterBuildType y)
		{
			return x == y;
		}

		// Token: 0x06001E7C RID: 7804 RVA: 0x00049A92 File Offset: 0x00047C92
		public int GetHashCode(CharacterBuildType obj)
		{
			return (int)obj;
		}
	}
}

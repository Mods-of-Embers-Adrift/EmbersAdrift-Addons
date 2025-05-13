using System;
using System.Collections.Generic;

namespace SoL.Networking.Database
{
	// Token: 0x0200043C RID: 1084
	public struct CharacterSexComparer : IEqualityComparer<CharacterSex>
	{
		// Token: 0x06001EE3 RID: 7907 RVA: 0x0004FB72 File Offset: 0x0004DD72
		public bool Equals(CharacterSex x, CharacterSex y)
		{
			return x == y;
		}

		// Token: 0x06001EE4 RID: 7908 RVA: 0x00049A92 File Offset: 0x00047C92
		public int GetHashCode(CharacterSex obj)
		{
			return (int)obj;
		}
	}
}

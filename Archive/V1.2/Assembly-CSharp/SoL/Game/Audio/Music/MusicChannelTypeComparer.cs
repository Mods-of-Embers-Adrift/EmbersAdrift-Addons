using System;
using System.Collections.Generic;

namespace SoL.Game.Audio.Music
{
	// Token: 0x02000D1E RID: 3358
	public struct MusicChannelTypeComparer : IEqualityComparer<MusicChannelType>
	{
		// Token: 0x06006516 RID: 25878 RVA: 0x0004FB72 File Offset: 0x0004DD72
		public bool Equals(MusicChannelType x, MusicChannelType y)
		{
			return x == y;
		}

		// Token: 0x06006517 RID: 25879 RVA: 0x00049A92 File Offset: 0x00047C92
		public int GetHashCode(MusicChannelType obj)
		{
			return (int)obj;
		}
	}
}

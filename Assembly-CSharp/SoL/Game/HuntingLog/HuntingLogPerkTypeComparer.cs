using System;
using System.Collections.Generic;

namespace SoL.Game.HuntingLog
{
	// Token: 0x02000BC9 RID: 3017
	public struct HuntingLogPerkTypeComparer : IEqualityComparer<HuntingLogPerkType>
	{
		// Token: 0x06005D37 RID: 23863 RVA: 0x0004FB72 File Offset: 0x0004DD72
		public bool Equals(HuntingLogPerkType x, HuntingLogPerkType y)
		{
			return x == y;
		}

		// Token: 0x06005D38 RID: 23864 RVA: 0x00049A92 File Offset: 0x00047C92
		public int GetHashCode(HuntingLogPerkType obj)
		{
			return (int)obj;
		}
	}
}

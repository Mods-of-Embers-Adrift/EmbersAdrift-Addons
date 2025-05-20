using System;
using System.Collections.Generic;

namespace SoL.Game.Audio
{
	// Token: 0x02000D11 RID: 3345
	public struct GroundSurfaceTypeComparer : IEqualityComparer<GroundSurfaceType>
	{
		// Token: 0x060064EB RID: 25835 RVA: 0x0004FB72 File Offset: 0x0004DD72
		public bool Equals(GroundSurfaceType x, GroundSurfaceType y)
		{
			return x == y;
		}

		// Token: 0x060064EC RID: 25836 RVA: 0x00049A92 File Offset: 0x00047C92
		public int GetHashCode(GroundSurfaceType obj)
		{
			return (int)obj;
		}
	}
}

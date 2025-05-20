using System;

namespace SoL.Game
{
	// Token: 0x02000561 RID: 1377
	public static class ActiveHandFlagsExtensions
	{
		// Token: 0x060029B2 RID: 10674 RVA: 0x0004FB40 File Offset: 0x0004DD40
		public static bool HasBitFlag(this ActiveHandFlags a, ActiveHandFlags b)
		{
			return (a & b) == b;
		}
	}
}

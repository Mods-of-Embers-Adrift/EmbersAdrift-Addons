using System;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C51 RID: 3153
	public static class ConalTypesExtensions
	{
		// Token: 0x06006119 RID: 24857 RVA: 0x0004FB40 File Offset: 0x0004DD40
		public static bool HasBitFlag(this ConalTypes a, ConalTypes b)
		{
			return (a & b) == b;
		}
	}
}

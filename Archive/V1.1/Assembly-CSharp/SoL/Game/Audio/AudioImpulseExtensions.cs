using System;

namespace SoL.Game.Audio
{
	// Token: 0x02000D04 RID: 3332
	public static class AudioImpulseExtensions
	{
		// Token: 0x060064AE RID: 25774 RVA: 0x0004FB40 File Offset: 0x0004DD40
		public static bool HasBitFlag(this AudioImpulseFlags a, AudioImpulseFlags b)
		{
			return (a & b) == b;
		}

		// Token: 0x060064AF RID: 25775 RVA: 0x000502DF File Offset: 0x0004E4DF
		public static bool HasAnyFlags(this AudioImpulseFlags a, AudioImpulseFlags b)
		{
			return a != AudioImpulseFlags.None && b != AudioImpulseFlags.None && (a & b) > AudioImpulseFlags.None;
		}
	}
}

using System;

namespace SoL.Game.Scenes
{
	// Token: 0x02000756 RID: 1878
	public static class SceneInclusionFlagsExtensions
	{
		// Token: 0x060037F3 RID: 14323 RVA: 0x0004FB40 File Offset: 0x0004DD40
		public static bool HasBitFlag(this SceneInclusionFlags a, SceneInclusionFlags b)
		{
			return (a & b) == b;
		}
	}
}

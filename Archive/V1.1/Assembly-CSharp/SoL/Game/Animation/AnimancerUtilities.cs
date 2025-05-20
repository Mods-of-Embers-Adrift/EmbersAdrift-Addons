using System;

namespace SoL.Game.Animation
{
	// Token: 0x02000D6B RID: 3435
	public static class AnimancerUtilities
	{
		// Token: 0x0600677D RID: 26493 RVA: 0x00049FFA File Offset: 0x000481FA
		public static string GetAnimIndexDescription(AnimationExecutionTime exeTime, int animIndex)
		{
			return null;
		}

		// Token: 0x0600677E RID: 26494 RVA: 0x00049FFA File Offset: 0x000481FA
		public static string GetAnimIndexDescription(AnimancerAnimationSet set, AnimationExecutionTime exeTime, int animIndex, bool includeIndex = false, bool shortened = false)
		{
			return null;
		}

		// Token: 0x040059F7 RID: 23031
		private const int kLocoNameLength = 16;

		// Token: 0x040059F8 RID: 23032
		private const string kLocoSeparator = "\t";

		// Token: 0x040059F9 RID: 23033
		public const string kDescriptionHeader = "LOCOMOTION\tDESCRIPTION";
	}
}

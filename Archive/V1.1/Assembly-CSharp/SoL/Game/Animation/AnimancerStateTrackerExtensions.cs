using System;
using SoL.Utilities;

namespace SoL.Game.Animation
{
	// Token: 0x02000D4F RID: 3407
	public static class AnimancerStateTrackerExtensions
	{
		// Token: 0x06006671 RID: 26225 RVA: 0x00084F3F File Offset: 0x0008313F
		internal static void ReturnToPool(this AnimancerStateTracker tracker)
		{
			StaticPool<AnimancerStateTracker>.ReturnToPool(tracker);
		}
	}
}

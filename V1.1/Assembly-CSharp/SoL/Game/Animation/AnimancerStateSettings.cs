using System;
using Animancer;

namespace SoL.Game.Animation
{
	// Token: 0x02000D4D RID: 3405
	internal struct AnimancerStateSettings
	{
		// Token: 0x06006669 RID: 26217 RVA: 0x00210840 File Offset: 0x0020EA40
		public AnimancerStateSettings(AnimancerState state, AnimationSequence sequence, IAnimancerStateTracker stateTracker, int layerIndex, int clipIndex, bool isLooping)
		{
			this.State = state;
			this.Sequence = sequence;
			this.LayerIndex = layerIndex;
			this.ClipIndex = clipIndex;
			this.StateTracker = stateTracker;
			this.IsLooping = isLooping;
			if (this.StateTracker != null)
			{
				this.StateTracker.State = this.State;
			}
		}

		// Token: 0x04005908 RID: 22792
		public AnimancerState State;

		// Token: 0x04005909 RID: 22793
		public AnimationSequence Sequence;

		// Token: 0x0400590A RID: 22794
		public IAnimancerStateTracker StateTracker;

		// Token: 0x0400590B RID: 22795
		public int LayerIndex;

		// Token: 0x0400590C RID: 22796
		public int ClipIndex;

		// Token: 0x0400590D RID: 22797
		public bool IsLooping;
	}
}

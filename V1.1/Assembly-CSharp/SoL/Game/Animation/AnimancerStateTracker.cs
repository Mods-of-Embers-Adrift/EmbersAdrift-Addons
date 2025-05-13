using System;
using Animancer;
using SoL.Utilities;

namespace SoL.Game.Animation
{
	// Token: 0x02000D4E RID: 3406
	public class AnimancerStateTracker : IAnimancerStateTracker, IPoolable
	{
		// Token: 0x17001880 RID: 6272
		// (get) Token: 0x0600666A RID: 26218 RVA: 0x00084F0D File Offset: 0x0008310D
		// (set) Token: 0x0600666B RID: 26219 RVA: 0x00084F15 File Offset: 0x00083115
		bool IPoolable.InPool
		{
			get
			{
				return this.m_inPool;
			}
			set
			{
				this.m_inPool = value;
			}
		}

		// Token: 0x17001881 RID: 6273
		// (get) Token: 0x0600666C RID: 26220 RVA: 0x00084F1E File Offset: 0x0008311E
		// (set) Token: 0x0600666D RID: 26221 RVA: 0x00084F26 File Offset: 0x00083126
		AnimancerState IAnimancerStateTracker.State
		{
			get
			{
				return this.AnimancerState;
			}
			set
			{
				this.AnimancerState = value;
			}
		}

		// Token: 0x0600666E RID: 26222 RVA: 0x00084F2F File Offset: 0x0008312F
		void IPoolable.Reset()
		{
			this.AnimancerState = null;
		}

		// Token: 0x0600666F RID: 26223 RVA: 0x00084F38 File Offset: 0x00083138
		public static AnimancerStateTracker GetFromPool()
		{
			return StaticPool<AnimancerStateTracker>.GetFromPool();
		}

		// Token: 0x0400590E RID: 22798
		private bool m_inPool;

		// Token: 0x0400590F RID: 22799
		public AnimancerState AnimancerState;

		// Token: 0x04005910 RID: 22800
		public bool DeferredHandIk;
	}
}

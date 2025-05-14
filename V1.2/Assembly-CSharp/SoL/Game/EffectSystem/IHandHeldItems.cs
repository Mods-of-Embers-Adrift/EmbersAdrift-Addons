using System;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C38 RID: 3128
	public interface IHandHeldItems
	{
		// Token: 0x1700172F RID: 5935
		// (get) Token: 0x060060A1 RID: 24737
		CachedHandHeldItem MainHand { get; }

		// Token: 0x17001730 RID: 5936
		// (get) Token: 0x060060A2 RID: 24738
		CachedHandHeldItem OffHand { get; }
	}
}

using System;

namespace SoL.Game.Audio
{
	// Token: 0x02000D13 RID: 3347
	public interface IAudioEvent
	{
		// Token: 0x17001838 RID: 6200
		// (get) Token: 0x060064EE RID: 25838
		string EventName { get; }

		// Token: 0x060064EF RID: 25839
		void Play(float volumeFraction = 1f);

		// Token: 0x17001839 RID: 6201
		// (get) Token: 0x060064F0 RID: 25840
		AudioImpulseFlags ImpulseFlags { get; }

		// Token: 0x1700183A RID: 6202
		// (get) Token: 0x060064F1 RID: 25841
		float ImpulseForce { get; }
	}
}

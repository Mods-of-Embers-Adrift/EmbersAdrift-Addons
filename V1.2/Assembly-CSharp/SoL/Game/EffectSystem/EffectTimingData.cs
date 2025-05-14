using System;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C40 RID: 3136
	[Serializable]
	public class EffectTimingData
	{
		// Token: 0x060060C6 RID: 24774 RVA: 0x0008133F File Offset: 0x0007F53F
		public void Reset()
		{
			this.Duration = 0;
			this.Elapsed = 0f;
			this.TickRate = 0;
			this.TimeOfNextTick = 0f;
			this.TicksRemaining = 0;
		}

		// Token: 0x04005363 RID: 21347
		public int Duration;

		// Token: 0x04005364 RID: 21348
		public float Elapsed;

		// Token: 0x04005365 RID: 21349
		public int TickRate;

		// Token: 0x04005366 RID: 21350
		public float TimeOfNextTick;

		// Token: 0x04005367 RID: 21351
		public int TicksRemaining;
	}
}

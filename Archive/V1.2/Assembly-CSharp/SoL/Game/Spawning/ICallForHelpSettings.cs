using System;

namespace SoL.Game.Spawning
{
	// Token: 0x0200066C RID: 1644
	public interface ICallForHelpSettings
	{
		// Token: 0x17000AE4 RID: 2788
		// (get) Token: 0x06003310 RID: 13072
		CallForHelpData InitialThreat { get; }

		// Token: 0x17000AE5 RID: 2789
		// (get) Token: 0x06003311 RID: 13073
		CallForHelpData Death { get; }

		// Token: 0x17000AE6 RID: 2790
		// (get) Token: 0x06003312 RID: 13074
		CallForHelpPeriodicData Periodic { get; }

		// Token: 0x17000AE7 RID: 2791
		// (get) Token: 0x06003313 RID: 13075
		CallForHelpPeriodicData Fleeing { get; }
	}
}

using System;
using UnityEngine;

namespace SoL.Game.Targeting
{
	// Token: 0x02000651 RID: 1617
	public interface INpcTargetController
	{
		// Token: 0x17000AB7 RID: 2743
		// (get) Token: 0x06003264 RID: 12900
		int HostileTargetCount { get; }

		// Token: 0x17000AB8 RID: 2744
		// (get) Token: 0x06003265 RID: 12901
		int AlertCount { get; }

		// Token: 0x17000AB9 RID: 2745
		// (get) Token: 0x06003266 RID: 12902
		int NeutralTargetCount { get; }

		// Token: 0x06003267 RID: 12903
		float GetHostileSpeedPercentage();

		// Token: 0x17000ABA RID: 2746
		// (get) Token: 0x06003268 RID: 12904
		Vector3? ResetPosition { get; }
	}
}

using System;
using UnityEngine;

namespace SoL.Game.Culling
{
	// Token: 0x02000CCC RID: 3276
	public interface ICullee
	{
		// Token: 0x170017B1 RID: 6065
		// (get) Token: 0x06006342 RID: 25410
		// (set) Token: 0x06006343 RID: 25411
		int? Index { get; set; }

		// Token: 0x170017B2 RID: 6066
		// (get) Token: 0x06006344 RID: 25412
		float Radius { get; }

		// Token: 0x170017B3 RID: 6067
		// (get) Token: 0x06006345 RID: 25413
		// (set) Token: 0x06006346 RID: 25414
		float SqrMagnitudeDistance { get; set; }

		// Token: 0x170017B4 RID: 6068
		// (get) Token: 0x06006347 RID: 25415
		GameObject gameObject { get; }

		// Token: 0x170017B5 RID: 6069
		// (get) Token: 0x06006348 RID: 25416
		CullingFlags CullingFlags { get; }

		// Token: 0x170017B6 RID: 6070
		// (get) Token: 0x06006349 RID: 25417
		CullingFlags LimitFlags { get; }

		// Token: 0x170017B7 RID: 6071
		// (get) Token: 0x0600634A RID: 25418
		CullingDistance CurrentDistance { get; }

		// Token: 0x0600634B RID: 25419
		void OnCulleeBecameVisible();

		// Token: 0x0600634C RID: 25420
		void OnCulleeBecameInvisible();

		// Token: 0x0600634D RID: 25421
		void OnDistanceBandChanged(int previous, int current, bool force);

		// Token: 0x0600634E RID: 25422
		void SetFlag(CullingFlags flag);

		// Token: 0x0600634F RID: 25423
		void UnsetFlag(CullingFlags flag);

		// Token: 0x06006350 RID: 25424
		int GetHashCode();
	}
}

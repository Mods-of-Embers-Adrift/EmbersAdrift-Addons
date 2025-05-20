using System;
using UnityEngine;

namespace SoL.Game.Settings
{
	// Token: 0x0200072C RID: 1836
	[Serializable]
	public class IkSettings
	{
		// Token: 0x04003599 RID: 13721
		public float HeadLookWeightLerpRate = 0.5f;

		// Token: 0x0400359A RID: 13722
		public float HeadLookTargetMoveRate = 2f;

		// Token: 0x0400359B RID: 13723
		public float HeadLookAngle = 90f;

		// Token: 0x0400359C RID: 13724
		public float HeadLookDistance = 50f;

		// Token: 0x0400359D RID: 13725
		public LayerMask GroundLayers;
	}
}

using System;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x02000291 RID: 657
	public static class LayerMap
	{
		// Token: 0x04001C5D RID: 7261
		public static readonly LayerMap.LayerMaskData Detection = new LayerMap.LayerMaskData("Detection");

		// Token: 0x04001C5E RID: 7262
		public static readonly LayerMap.LayerMaskData Interaction = new LayerMap.LayerMaskData("Interaction");

		// Token: 0x04001C5F RID: 7263
		public static readonly LayerMap.LayerMaskData Water = new LayerMap.LayerMaskData("Water");

		// Token: 0x04001C60 RID: 7264
		public static readonly LayerMap.LayerMaskData PlayerCollideCameraIgnore = new LayerMap.LayerMaskData("P_PlayerCollideCameraIgnore");

		// Token: 0x04001C61 RID: 7265
		public static readonly LayerMap.LayerMaskData CameraCollidePlayerIgnore = new LayerMap.LayerMaskData("P_CameraCollidePlayerIgnore");

		// Token: 0x02000292 RID: 658
		public class LayerMaskData
		{
			// Token: 0x0600140E RID: 5134 RVA: 0x000500B3 File Offset: 0x0004E2B3
			public LayerMaskData(string layerName)
			{
				this.Layer = LayerMask.NameToLayer(layerName);
				this.LayerMask = 1 << LayerMask.NameToLayer(layerName);
			}

			// Token: 0x04001C62 RID: 7266
			public readonly LayerMask Layer;

			// Token: 0x04001C63 RID: 7267
			public readonly LayerMask LayerMask;
		}
	}
}

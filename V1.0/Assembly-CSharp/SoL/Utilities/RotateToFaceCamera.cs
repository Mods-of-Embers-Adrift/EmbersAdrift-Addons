using System;
using SoL.Managers;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x020002BC RID: 700
	public class RotateToFaceCamera : MonoBehaviour
	{
		// Token: 0x1700050D RID: 1293
		// (get) Token: 0x060014AE RID: 5294 RVA: 0x0005069E File Offset: 0x0004E89E
		// (set) Token: 0x060014AF RID: 5295 RVA: 0x000506A6 File Offset: 0x0004E8A6
		public bool PreventRotation { get; set; }

		// Token: 0x060014B0 RID: 5296 RVA: 0x000506AF File Offset: 0x0004E8AF
		private void LateUpdate()
		{
			if (this.PreventRotation || GameManager.IsServer || !ClientGameManager.MainCamera)
			{
				return;
			}
			base.gameObject.transform.rotation = ClientGameManager.MainCamera.transform.rotation;
		}
	}
}

using System;
using SoL.Managers;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x020002BE RID: 702
	public class RotateToLookAtCamera : MonoBehaviour
	{
		// Token: 0x1700050F RID: 1295
		// (get) Token: 0x060014B6 RID: 5302 RVA: 0x000506FD File Offset: 0x0004E8FD
		// (set) Token: 0x060014B7 RID: 5303 RVA: 0x00050705 File Offset: 0x0004E905
		public bool PreventRotation { get; set; }

		// Token: 0x060014B8 RID: 5304 RVA: 0x000FB610 File Offset: 0x000F9810
		private void LateUpdate()
		{
			if (this.PreventRotation || GameManager.IsServer || !ClientGameManager.MainCamera)
			{
				return;
			}
			if (!this.m_transform)
			{
				this.m_transform = base.gameObject.transform;
			}
			Vector3 position = ClientGameManager.MainCamera.gameObject.transform.position;
			if (this.m_yAxisOnly)
			{
				position.y = this.m_transform.position.y;
			}
			this.m_transform.LookAt(position);
		}

		// Token: 0x04001CF2 RID: 7410
		[SerializeField]
		private bool m_yAxisOnly;

		// Token: 0x04001CF3 RID: 7411
		private Transform m_transform;
	}
}

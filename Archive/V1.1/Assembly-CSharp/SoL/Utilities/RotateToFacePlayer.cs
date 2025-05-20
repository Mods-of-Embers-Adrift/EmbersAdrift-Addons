using System;
using SoL.Game;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x020002BD RID: 701
	public class RotateToFacePlayer : MonoBehaviour
	{
		// Token: 0x1700050E RID: 1294
		// (get) Token: 0x060014B2 RID: 5298 RVA: 0x000506EC File Offset: 0x0004E8EC
		// (set) Token: 0x060014B3 RID: 5299 RVA: 0x000506F4 File Offset: 0x0004E8F4
		public bool PreventRotation { get; set; }

		// Token: 0x060014B4 RID: 5300 RVA: 0x000FB590 File Offset: 0x000F9790
		private void LateUpdate()
		{
			if (this.PreventRotation || !LocalPlayer.GameEntity)
			{
				return;
			}
			if (!this.m_transform)
			{
				this.m_transform = base.gameObject.transform;
			}
			Vector3 position = LocalPlayer.GameEntity.gameObject.transform.position;
			if (this.m_yAxisOnly)
			{
				position.y = this.m_transform.position.y;
			}
			this.m_transform.LookAt(position);
		}

		// Token: 0x04001CEF RID: 7407
		[SerializeField]
		private bool m_yAxisOnly;

		// Token: 0x04001CF0 RID: 7408
		private Transform m_transform;
	}
}

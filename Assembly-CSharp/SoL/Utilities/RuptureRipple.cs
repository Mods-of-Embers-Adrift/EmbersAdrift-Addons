using System;
using SoL.Game;
using SoL.Managers;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace SoL.Utilities
{
	// Token: 0x020002DB RID: 731
	public class RuptureRipple : MonoBehaviour
	{
		// Token: 0x1700051F RID: 1311
		// (get) Token: 0x0600150E RID: 5390 RVA: 0x00050BBB File Offset: 0x0004EDBB
		// (set) Token: 0x0600150F RID: 5391 RVA: 0x00050BC3 File Offset: 0x0004EDC3
		private float DecalSize { get; set; }

		// Token: 0x06001510 RID: 5392 RVA: 0x000FBF5C File Offset: 0x000FA15C
		private void Start()
		{
			if (GameManager.IsServer)
			{
				base.enabled = false;
				base.gameObject.SetActive(false);
				return;
			}
			if (this.m_decal == null || this.m_toScale == null)
			{
				base.enabled = false;
				return;
			}
			this.DecalSize = Mathf.Max(this.m_decal.size.x, this.m_decal.size.y);
		}

		// Token: 0x06001511 RID: 5393 RVA: 0x000FBFD4 File Offset: 0x000FA1D4
		private void Update()
		{
			if (!this.m_toScale || !LocalPlayer.GameEntity)
			{
				return;
			}
			float num = this.DecalSize * RuptureDecalAnimator.Radius;
			bool flag = num > 0f && num < this.DecalSize;
			this.m_toScale.localScale = Vector3.one * num;
			this.m_toScale.gameObject.SetActive(flag);
			if (flag)
			{
				Vector3 position = LocalPlayer.GameEntity.gameObject.transform.position;
				position.y = base.gameObject.transform.position.y;
				this.m_toScale.LookAt(position);
			}
		}

		// Token: 0x04001D4F RID: 7503
		[SerializeField]
		private DecalProjector m_decal;

		// Token: 0x04001D50 RID: 7504
		[SerializeField]
		private Transform m_toScale;
	}
}

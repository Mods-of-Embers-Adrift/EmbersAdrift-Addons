using System;
using SoL.Managers;
using UnityEngine;

namespace SoL.Game.Culling
{
	// Token: 0x02000CB6 RID: 3254
	public class CulledIK : CulledObject
	{
		// Token: 0x060062AC RID: 25260 RVA: 0x000826C5 File Offset: 0x000808C5
		private void Awake()
		{
			if (!this.m_controller || GameManager.IsServer)
			{
				base.gameObject.SetActive(false);
				return;
			}
			this.m_limitFlags |= CullingFlags.IKLimit;
		}

		// Token: 0x060062AD RID: 25261 RVA: 0x000826F7 File Offset: 0x000808F7
		protected override bool IsCulled()
		{
			return base.IsCulled() || this.m_cullingFlags.HasBitFlag(CullingFlags.IKLimit);
		}

		// Token: 0x060062AE RID: 25262 RVA: 0x00082710 File Offset: 0x00080910
		protected override void RefreshCullee()
		{
			base.RefreshCullee();
			this.m_controller.RefreshCullee(this.IsCulled(), this.m_sqrMagnitudeDistance);
		}

		// Token: 0x0400560B RID: 22027
		[SerializeField]
		private IKController m_controller;
	}
}

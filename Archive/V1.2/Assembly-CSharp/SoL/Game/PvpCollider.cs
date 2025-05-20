using System;
using SoL.Managers;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game
{
	// Token: 0x0200061D RID: 1565
	public class PvpCollider : MonoBehaviour
	{
		// Token: 0x06003195 RID: 12693 RVA: 0x0015D2EC File Offset: 0x0015B4EC
		private void Awake()
		{
			if (!this.m_collider)
			{
				base.gameObject.SetActive(false);
				return;
			}
			if (GameManager.IsServer)
			{
				this.m_bounds = this.m_collider.bounds;
				this.m_collider.isTrigger = true;
				base.gameObject.layer = LayerMap.Detection.Layer;
				return;
			}
			this.m_collider.enabled = false;
		}

		// Token: 0x06003196 RID: 12694 RVA: 0x000622EE File Offset: 0x000604EE
		public bool IsWithinBounds(Vector3 pos)
		{
			return this.m_collider && this.m_bounds.WithinBounds(pos);
		}

		// Token: 0x04002FFE RID: 12286
		[SerializeField]
		private Collider m_collider;

		// Token: 0x04002FFF RID: 12287
		private Bounds m_bounds;
	}
}

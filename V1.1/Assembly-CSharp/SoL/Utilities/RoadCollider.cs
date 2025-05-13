using System;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x020002B9 RID: 697
	public class RoadCollider : MonoBehaviour
	{
		// Token: 0x060014A9 RID: 5289 RVA: 0x00050662 File Offset: 0x0004E862
		public void Awake()
		{
			if (!this.m_collider)
			{
				base.gameObject.SetActive(false);
				return;
			}
			this.SetupCollider();
		}

		// Token: 0x060014AA RID: 5290 RVA: 0x000FB4B4 File Offset: 0x000F96B4
		private void SetupCollider()
		{
			if (this.m_collider)
			{
				this.m_collider.isTrigger = true;
				this.m_collider.gameObject.isStatic = true;
				this.m_collider.gameObject.layer = LayerMap.Detection.Layer;
				this.m_collider.enabled = true;
			}
		}

		// Token: 0x04001CE6 RID: 7398
		[SerializeField]
		private Collider m_collider;
	}
}

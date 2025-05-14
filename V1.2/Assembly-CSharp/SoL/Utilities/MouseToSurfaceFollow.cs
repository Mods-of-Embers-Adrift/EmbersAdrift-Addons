using System;
using SoL.Managers;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x0200029E RID: 670
	public class MouseToSurfaceFollow : MonoBehaviour
	{
		// Token: 0x06001431 RID: 5169 RVA: 0x000F9530 File Offset: 0x000F7730
		private void Update()
		{
			Ray ray = ClientGameManager.MainCamera.ScreenPointToRay(Input.mousePosition);
			RaycastHit[] hits = Hits.Hits5;
			int num = Physics.RaycastNonAlloc(ray, hits, this.m_maxDistance, this.m_mask);
			if (num <= 0)
			{
				num = Physics.RaycastNonAlloc(ray.GetPoint(this.m_maxDistance), Vector3.down, hits, (float)this.m_mask);
			}
			if (num > 0)
			{
				base.gameObject.transform.position = hits[0].point;
			}
		}

		// Token: 0x04001C8B RID: 7307
		[SerializeField]
		private LayerMask m_mask;

		// Token: 0x04001C8C RID: 7308
		[SerializeField]
		private float m_maxDistance = 25f;
	}
}

using System;
using UnityEngine;

namespace UnityStandardAssets.SceneUtils
{
	// Token: 0x02000085 RID: 133
	public class PlaceTargetWithMouse : MonoBehaviour
	{
		// Token: 0x0600055E RID: 1374 RVA: 0x0009F878 File Offset: 0x0009DA78
		private void Update()
		{
			RaycastHit raycastHit;
			if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out raycastHit))
			{
				this.setTargetOn.transform.position = raycastHit.point + raycastHit.normal * this.surfaceOffset;
				this.setTargetOn.transform.forward = -raycastHit.normal;
			}
		}

		// Token: 0x040005F8 RID: 1528
		public float surfaceOffset = 1.5f;

		// Token: 0x040005F9 RID: 1529
		public GameObject setTargetOn;
	}
}

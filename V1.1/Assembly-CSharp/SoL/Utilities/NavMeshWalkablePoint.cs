using System;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x020002A3 RID: 675
	public class NavMeshWalkablePoint : MonoBehaviour
	{
		// Token: 0x0600143C RID: 5180 RVA: 0x000F9778 File Offset: 0x000F7978
		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.yellow;
			Vector3 position = base.gameObject.transform.position;
			Gizmos.DrawWireSphere(position, 0.2f);
			Gizmos.DrawLine(position, position + Vector3.up * 0.5f);
		}
	}
}

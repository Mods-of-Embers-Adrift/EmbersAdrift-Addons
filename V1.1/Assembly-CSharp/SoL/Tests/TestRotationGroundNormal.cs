using System;
using SoL.Utilities;
using UnityEngine;
using UnityEngine.AI;

namespace SoL.Tests
{
	// Token: 0x02000DC1 RID: 3521
	public class TestRotationGroundNormal : MonoBehaviour
	{
		// Token: 0x0600692D RID: 26925 RVA: 0x00216D1C File Offset: 0x00214F1C
		public void Align()
		{
			Vector3 vector = base.gameObject.transform.position + Vector3.up;
			NavMeshHit value;
			if (NavMesh.SamplePosition(vector, out value, this.maxDistance, -1))
			{
				this.m_previousHit = new NavMeshHit?(value);
				RaycastHit[] hits = Hits.Hits5;
				if (Physics.RaycastNonAlloc(vector, Vector3.down, hits, this.maxDistance) > 0)
				{
					this.m_previousRaycastHit = new RaycastHit?(hits[0]);
					base.gameObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, hits[0].normal);
				}
			}
		}

		// Token: 0x0600692E RID: 26926 RVA: 0x00086852 File Offset: 0x00084A52
		public void Rotate()
		{
			base.gameObject.transform.Rotate(0f, UnityEngine.Random.Range(0f, 360f), 0f, Space.Self);
		}

		// Token: 0x0600692F RID: 26927 RVA: 0x00216DB4 File Offset: 0x00214FB4
		private void OnDrawGizmos()
		{
			if (this.m_previousHit == null)
			{
				return;
			}
			Gizmos.color = Color.blue;
			Gizmos.DrawLine(base.gameObject.transform.position, this.m_previousHit.Value.position);
			Gizmos.DrawLine(this.m_previousHit.Value.position, this.m_previousHit.Value.position + this.m_previousHit.Value.normal);
			Gizmos.color = Color.red;
			Gizmos.DrawLine(this.m_previousRaycastHit.Value.point, this.m_previousRaycastHit.Value.point + this.m_previousRaycastHit.Value.normal);
		}

		// Token: 0x04005B86 RID: 23430
		public float maxDistance = 100f;

		// Token: 0x04005B87 RID: 23431
		private NavMeshHit? m_previousHit;

		// Token: 0x04005B88 RID: 23432
		private RaycastHit? m_previousRaycastHit;
	}
}

using System;
using UnityEngine;
using UnityEngine.AI;

namespace SoL.Tests
{
	// Token: 0x02000DB1 RID: 3505
	public class NavMeshTestAgent : MonoBehaviour
	{
		// Token: 0x060068F4 RID: 26868 RVA: 0x000866CA File Offset: 0x000848CA
		private void Awake()
		{
			this.m_path = new NavMeshPath();
		}

		// Token: 0x060068F5 RID: 26869 RVA: 0x00216244 File Offset: 0x00214444
		private void Update()
		{
			NavMeshHit navMeshHit;
			if (this.m_snapToNavMesh && NavMesh.SamplePosition(base.gameObject.transform.position, out navMeshHit, 5f, -1))
			{
				base.gameObject.transform.position = navMeshHit.position;
			}
			if (this.m_canReachTest != null)
			{
				this.HasPathViaCalculate = NavMesh.CalculatePath(base.gameObject.transform.position, this.m_canReachTest.gameObject.transform.position, -1, this.m_path);
				if (this.HasPathViaCalculate)
				{
					this.PathStatus = this.m_path.status;
				}
				NavMeshHit navMeshHit2;
				this.HasPathViaRaycast = NavMesh.Raycast(base.gameObject.transform.position, this.m_canReachTest.gameObject.transform.position, out navMeshHit2, -1);
			}
		}

		// Token: 0x04005B4E RID: 23374
		[SerializeField]
		private bool m_snapToNavMesh;

		// Token: 0x04005B4F RID: 23375
		[SerializeField]
		private GameObject m_canReachTest;

		// Token: 0x04005B50 RID: 23376
		private NavMeshPath m_path;

		// Token: 0x04005B51 RID: 23377
		public bool HasPathViaCalculate;

		// Token: 0x04005B52 RID: 23378
		public NavMeshPathStatus PathStatus = NavMeshPathStatus.PathInvalid;

		// Token: 0x04005B53 RID: 23379
		public bool HasPathViaRaycast;
	}
}

using System;
using SoL.Game.Settings;
using SoL.Utilities;
using UnityEngine;
using UnityEngine.AI;

namespace SoL.Tests
{
	// Token: 0x02000DB2 RID: 3506
	public class NavMeshValidPathDisplay : MonoBehaviour
	{
		// Token: 0x060068F7 RID: 26871 RVA: 0x000866E6 File Offset: 0x000848E6
		private void Start()
		{
			this.m_path = new NavMeshPath();
		}

		// Token: 0x060068F8 RID: 26872 RVA: 0x00216324 File Offset: 0x00214524
		private void Update()
		{
			if (!this.m_target)
			{
				this.m_pathValid = false;
				this.m_hasNavUnder = false;
				return;
			}
			Vector3 vector = this.m_target.transform.position;
			if (this.m_useIndexedPos)
			{
				vector = new IndexedVector3(vector, 2048, 1f).Pos;
			}
			this.m_targetPos = vector;
			this.m_pathValid = NavMesh.CalculatePath(base.gameObject.transform.position, vector, -1, this.m_path);
			NavMeshHit navMeshHit;
			this.m_hasNavUnder = NavMesh.SamplePosition(vector, out navMeshHit, GlobalSettings.Values.Npcs.CanHitSampleDistance, -1);
		}

		// Token: 0x04005B54 RID: 23380
		[SerializeField]
		private bool m_useIndexedPos;

		// Token: 0x04005B55 RID: 23381
		[SerializeField]
		private GameObject m_target;

		// Token: 0x04005B56 RID: 23382
		private bool m_pathValid;

		// Token: 0x04005B57 RID: 23383
		private bool m_hasNavUnder;

		// Token: 0x04005B58 RID: 23384
		private NavMeshPath m_path;

		// Token: 0x04005B59 RID: 23385
		private Vector3 m_targetPos;
	}
}

using System;
using Drawing;
using UnityEngine;
using UnityEngine.AI;

namespace SoL.Game.Spawning
{
	// Token: 0x0200067B RID: 1659
	public class FixedSpawnMarker : MonoBehaviourGizmos, ISpawnLocation
	{
		// Token: 0x17000B07 RID: 2823
		// (get) Token: 0x06003350 RID: 13136 RVA: 0x0006361A File Offset: 0x0006181A
		public bool RandomizeRotation
		{
			get
			{
				return this.m_randomizeRotation;
			}
		}

		// Token: 0x17000B08 RID: 2824
		// (get) Token: 0x06003351 RID: 13137 RVA: 0x00063622 File Offset: 0x00061822
		// (set) Token: 0x06003352 RID: 13138 RVA: 0x0006362A File Offset: 0x0006182A
		public bool Occupied
		{
			get
			{
				return this.m_occupied;
			}
			set
			{
				this.m_occupied = value;
				if (!this.m_occupied)
				{
					this.m_hit = null;
				}
			}
		}

		// Token: 0x06003353 RID: 13139 RVA: 0x00063647 File Offset: 0x00061847
		public void SetPosition(NavMeshHit hit)
		{
			this.m_hit = new NavMeshHit?(hit);
		}

		// Token: 0x06003354 RID: 13140 RVA: 0x001620F4 File Offset: 0x001602F4
		public Vector3 GetPosition()
		{
			if (this.m_hit == null)
			{
				return base.gameObject.transform.position;
			}
			return this.m_hit.Value.position;
		}

		// Token: 0x06003355 RID: 13141 RVA: 0x00162134 File Offset: 0x00160334
		public Quaternion GetRotation()
		{
			float y = this.m_randomizeRotation ? UnityEngine.Random.Range(0f, 360f) : base.gameObject.transform.eulerAngles.y;
			return Quaternion.Euler(new Vector3(0f, y, 0f));
		}

		// Token: 0x04003182 RID: 12674
		[SerializeField]
		private SpawnController m_controller;

		// Token: 0x04003183 RID: 12675
		[SerializeField]
		private bool m_randomizeRotation;

		// Token: 0x04003184 RID: 12676
		private NavMeshHit? m_hit;

		// Token: 0x04003185 RID: 12677
		private bool m_occupied;
	}
}

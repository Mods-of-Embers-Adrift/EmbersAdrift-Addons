using System;
using UnityEngine;
using UnityEngine.AI;

namespace SoL.Game.Spawning
{
	// Token: 0x0200067A RID: 1658
	public struct DynamicSpawnPoint : ISpawnLocation
	{
		// Token: 0x17000B06 RID: 2822
		// (get) Token: 0x0600334A RID: 13130 RVA: 0x0004479C File Offset: 0x0004299C
		// (set) Token: 0x0600334B RID: 13131 RVA: 0x0004475B File Offset: 0x0004295B
		public bool Occupied
		{
			get
			{
				return true;
			}
			set
			{
			}
		}

		// Token: 0x0600334C RID: 13132 RVA: 0x0004475B File Offset: 0x0004295B
		public void SetPosition(NavMeshHit hit)
		{
		}

		// Token: 0x0600334D RID: 13133 RVA: 0x001620F8 File Offset: 0x001602F8
		public Vector3 GetPosition()
		{
			return this.m_hit.position;
		}

		// Token: 0x0600334E RID: 13134 RVA: 0x000635EC File Offset: 0x000617EC
		public Quaternion GetRotation()
		{
			return Quaternion.Euler(new Vector3(0f, UnityEngine.Random.Range(0f, 360f), 0f));
		}

		// Token: 0x0600334F RID: 13135 RVA: 0x00063611 File Offset: 0x00061811
		public DynamicSpawnPoint(NavMeshHit hit)
		{
			this.m_hit = hit;
		}

		// Token: 0x04003181 RID: 12673
		private readonly NavMeshHit m_hit;
	}
}

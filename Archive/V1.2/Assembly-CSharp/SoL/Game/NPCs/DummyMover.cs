using System;
using SoL.Managers;
using UnityEngine;

namespace SoL.Game.NPCs
{
	// Token: 0x020007F7 RID: 2039
	public class DummyMover : MonoBehaviour
	{
		// Token: 0x06003B3C RID: 15164 RVA: 0x000681C7 File Offset: 0x000663C7
		private void Start()
		{
			if (this.m_toMove == null)
			{
				base.enabled = false;
			}
		}

		// Token: 0x06003B3D RID: 15165 RVA: 0x0017AEB0 File Offset: 0x001790B0
		private void Update()
		{
			if (!GameManager.IsServer)
			{
				return;
			}
			Vector3 vector = this.m_towardsB ? this.m_positionB : this.m_positionA;
			this.m_toMove.transform.position = Vector3.MoveTowards(this.m_toMove.transform.position, vector, Time.deltaTime * this.m_speed);
			if ((this.m_toMove.transform.position - vector).sqrMagnitude <= 1f)
			{
				this.m_towardsB = !this.m_towardsB;
			}
		}

		// Token: 0x040039AF RID: 14767
		[SerializeField]
		private GameObject m_toMove;

		// Token: 0x040039B0 RID: 14768
		[SerializeField]
		private Vector3 m_positionA = Vector3.zero;

		// Token: 0x040039B1 RID: 14769
		[SerializeField]
		private Vector3 m_positionB = Vector3.zero;

		// Token: 0x040039B2 RID: 14770
		[SerializeField]
		private float m_speed = 20f;

		// Token: 0x040039B3 RID: 14771
		private bool m_towardsB;
	}
}

using System;
using SoL.Managers;
using UnityEngine;

namespace SoL.Game.Culling
{
	// Token: 0x02000CC2 RID: 3266
	public class CulledRigidBody : CulledObject
	{
		// Token: 0x0600630B RID: 25355 RVA: 0x00205F2C File Offset: 0x0020412C
		private void Awake()
		{
			if (!this.m_rigidbody || GameManager.IsServer)
			{
				base.gameObject.SetActive(false);
				return;
			}
			this.m_rigidbody.isKinematic = true;
			this.m_parent = base.gameObject.transform.parent;
			this.m_limitFlags |= CullingFlags.Physics;
		}

		// Token: 0x0600630C RID: 25356 RVA: 0x00082C0B File Offset: 0x00080E0B
		protected override bool IsCulled()
		{
			return base.IsCulled() || this.m_cullingFlags.HasBitFlag(CullingFlags.Physics);
		}

		// Token: 0x0600630D RID: 25357 RVA: 0x00082C27 File Offset: 0x00080E27
		protected override void RefreshCullee()
		{
			base.RefreshCullee();
			this.RefreshRigidbody();
		}

		// Token: 0x0600630E RID: 25358 RVA: 0x00205F90 File Offset: 0x00204190
		public void RefreshRigidbody()
		{
			Transform transform = base.gameObject.transform;
			if (this.IsCulled())
			{
				this.m_rigidbody.isKinematic = true;
				transform.SetParent(this.m_parent);
				transform.localPosition = Vector3.zero;
				transform.localRotation = Quaternion.identity;
				return;
			}
			transform.SetParent(null, true);
			transform.SetPositionAndRotation(this.m_parent.gameObject.transform.position, Quaternion.identity);
			this.m_rigidbody.isKinematic = false;
		}

		// Token: 0x04005636 RID: 22070
		[SerializeField]
		private Rigidbody m_rigidbody;

		// Token: 0x04005637 RID: 22071
		private Transform m_parent;
	}
}

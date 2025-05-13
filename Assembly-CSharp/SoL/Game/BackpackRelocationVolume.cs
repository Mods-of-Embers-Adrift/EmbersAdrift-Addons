using System;
using SoL.Managers;
using UnityEngine;

namespace SoL.Game
{
	// Token: 0x02000600 RID: 1536
	public class BackpackRelocationVolume : BaseVolumeOverride
	{
		// Token: 0x17000A73 RID: 2675
		// (get) Token: 0x0600310B RID: 12555 RVA: 0x00061C73 File Offset: 0x0005FE73
		public TargetPosition RelocationTarget
		{
			get
			{
				return this.m_relocationTarget;
			}
		}

		// Token: 0x17000A74 RID: 2676
		// (get) Token: 0x0600310C RID: 12556 RVA: 0x00045BCA File Offset: 0x00043DCA
		protected override bool DisableColliderOnStart
		{
			get
			{
				return false;
			}
		}

		// Token: 0x0600310D RID: 12557 RVA: 0x00061C7B File Offset: 0x0005FE7B
		protected override void Register()
		{
			base.Register();
			LocalZoneManager.RegisterBackpackRelocationVolume(this);
		}

		// Token: 0x0600310E RID: 12558 RVA: 0x00061C89 File Offset: 0x0005FE89
		protected override void Deregister()
		{
			base.Deregister();
			LocalZoneManager.DeregisterBackpackRelocationVolume(this);
		}

		// Token: 0x0600310F RID: 12559 RVA: 0x0015C134 File Offset: 0x0015A334
		private void OnDrawGizmos()
		{
			if (this.m_relocationTarget)
			{
				Gizmos.color = Color.cyan;
				Gizmos.DrawLine(base.gameObject.transform.position, this.m_relocationTarget.gameObject.transform.position);
			}
		}

		// Token: 0x04002F55 RID: 12117
		[SerializeField]
		private TargetPosition m_relocationTarget;
	}
}

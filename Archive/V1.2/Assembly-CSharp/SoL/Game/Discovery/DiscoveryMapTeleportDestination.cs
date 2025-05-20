using System;
using SoL.Managers;
using UnityEngine;

namespace SoL.Game.Discovery
{
	// Token: 0x02000CA3 RID: 3235
	public class DiscoveryMapTeleportDestination : MonoBehaviour
	{
		// Token: 0x1700176C RID: 5996
		// (get) Token: 0x06006216 RID: 25110 RVA: 0x0008219D File Offset: 0x0008039D
		// (set) Token: 0x06006217 RID: 25111 RVA: 0x000821A5 File Offset: 0x000803A5
		public DiscoveryProfile DiscoveryProfile { get; private set; }

		// Token: 0x1700176D RID: 5997
		// (get) Token: 0x06006218 RID: 25112 RVA: 0x000821AE File Offset: 0x000803AE
		public TargetPosition TargetPosition
		{
			get
			{
				return this.m_targetPosition;
			}
		}

		// Token: 0x06006219 RID: 25113 RVA: 0x002037F4 File Offset: 0x002019F4
		private void Start()
		{
			DiscoveryTrigger discoveryTrigger = (this.m_discoveryForwarder && this.m_discoveryForwarder.Trigger) ? this.m_discoveryForwarder.Trigger : this.m_discoveryTrigger;
			if (!discoveryTrigger)
			{
				base.enabled = false;
				return;
			}
			this.DiscoveryProfile = discoveryTrigger.Profile;
			if (!this.DiscoveryProfile)
			{
				base.enabled = false;
				return;
			}
			LocalZoneManager.RegisterMapTeleportDestination(this);
		}

		// Token: 0x0600621A RID: 25114 RVA: 0x000821B6 File Offset: 0x000803B6
		private void OnDestroy()
		{
			LocalZoneManager.DeregisterMapTeleportDestination(this);
		}

		// Token: 0x040055B8 RID: 21944
		[SerializeField]
		private TargetPosition m_targetPosition;

		// Token: 0x040055B9 RID: 21945
		[SerializeField]
		private DiscoveryForwarder m_discoveryForwarder;

		// Token: 0x040055BA RID: 21946
		[SerializeField]
		private DiscoveryTrigger m_discoveryTrigger;
	}
}

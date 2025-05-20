using System;
using SoL.Game.Discovery;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x02000293 RID: 659
	public class LeyLinkVisuals : MonoBehaviour
	{
		// Token: 0x0600140F RID: 5135 RVA: 0x000F91A0 File Offset: 0x000F73A0
		private void Start()
		{
			if (this.m_toToggle && this.m_discoveryForwarder && this.m_discoveryForwarder.Trigger && this.m_discoveryForwarder.Trigger.Profile && this.m_discoveryForwarder.Trigger.Profile is LeyLinkProfile)
			{
				this.m_toToggle.SetActive(true);
			}
		}

		// Token: 0x04001C64 RID: 7268
		[SerializeField]
		private GameObject m_toToggle;

		// Token: 0x04001C65 RID: 7269
		[SerializeField]
		private DiscoveryForwarder m_discoveryForwarder;
	}
}

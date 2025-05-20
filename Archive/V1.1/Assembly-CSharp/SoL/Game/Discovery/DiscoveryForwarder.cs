using System;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Discovery
{
	// Token: 0x02000CA2 RID: 3234
	public class DiscoveryForwarder : MonoBehaviour
	{
		// Token: 0x1700176B RID: 5995
		// (get) Token: 0x06006211 RID: 25105 RVA: 0x0008211F File Offset: 0x0008031F
		public DiscoveryTrigger Trigger
		{
			get
			{
				return this.m_trigger;
			}
		}

		// Token: 0x06006212 RID: 25106 RVA: 0x00082127 File Offset: 0x00080327
		private void Awake()
		{
			if (this.m_collider == null)
			{
				base.enabled = false;
				return;
			}
			this.m_collider.isTrigger = true;
			base.gameObject.layer = LayerMap.Detection.Layer;
		}

		// Token: 0x06006213 RID: 25107 RVA: 0x00082165 File Offset: 0x00080365
		private void OnTriggerEnter(Collider other)
		{
			if (this.m_trigger != null)
			{
				this.m_trigger.OnTriggerEnter(other);
			}
		}

		// Token: 0x06006214 RID: 25108 RVA: 0x00082181 File Offset: 0x00080381
		private void OnTriggerExit(Collider other)
		{
			if (this.m_trigger != null)
			{
				this.m_trigger.OnTriggerExit(other);
			}
		}

		// Token: 0x040055B6 RID: 21942
		[SerializeField]
		private Collider m_collider;

		// Token: 0x040055B7 RID: 21943
		[SerializeField]
		private DiscoveryTrigger m_trigger;
	}
}

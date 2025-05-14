using System;
using SoL.Networking.Objects;
using UnityEngine;

namespace SoL.Networking.Proximity
{
	// Token: 0x020004B2 RID: 1202
	public class ProximitySensor : MonoBehaviour
	{
		// Token: 0x170006DD RID: 1757
		// (get) Token: 0x06002180 RID: 8576 RVA: 0x000583F3 File Offset: 0x000565F3
		public SensorBand SensorBand
		{
			get
			{
				return this.m_band;
			}
		}

		// Token: 0x170006DE RID: 1758
		// (get) Token: 0x06002181 RID: 8577 RVA: 0x000583FB File Offset: 0x000565FB
		// (set) Token: 0x06002182 RID: 8578 RVA: 0x00058403 File Offset: 0x00056603
		public NetworkEntity NetworkEntity { private get; set; }

		// Token: 0x170006DF RID: 1759
		// (get) Token: 0x06002183 RID: 8579 RVA: 0x0005840C File Offset: 0x0005660C
		// (set) Token: 0x06002184 RID: 8580 RVA: 0x00058414 File Offset: 0x00056614
		public bool CanUpdate { get; private set; }

		// Token: 0x06002185 RID: 8581 RVA: 0x0005841D File Offset: 0x0005661D
		public void SetUpdateFlag()
		{
			if (this.m_band == SensorBand.None)
			{
				return;
			}
			this.CanUpdate = (Time.time > this.m_timeOfNextUpdate);
			if (this.CanUpdate)
			{
				this.m_timeOfNextUpdate = Time.time + this.m_band.GetUpdateTime();
			}
		}

		// Token: 0x06002186 RID: 8582 RVA: 0x00123FE4 File Offset: 0x001221E4
		private void Awake()
		{
			this.m_collider = base.gameObject.GetComponent<Collider>();
			if (this.m_collider == null)
			{
				Debug.LogWarning("No collider on " + base.gameObject.name + "!");
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			this.m_collider.enabled = false;
			this.m_collider.isTrigger = true;
			base.gameObject.layer = LayerMask.NameToLayer("Proximity");
		}

		// Token: 0x06002187 RID: 8583 RVA: 0x0005845A File Offset: 0x0005665A
		private void Start()
		{
			this.m_collider.enabled = true;
		}

		// Token: 0x06002188 RID: 8584 RVA: 0x00058468 File Offset: 0x00056668
		private void OnTriggerEnter(Collider other)
		{
			if (this.NetworkEntity == null)
			{
				return;
			}
			other.gameObject.GetComponent<NetworkEntity>() != null;
		}

		// Token: 0x06002189 RID: 8585 RVA: 0x00058468 File Offset: 0x00056668
		private void OnTriggerExit(Collider other)
		{
			if (this.NetworkEntity == null)
			{
				return;
			}
			other.gameObject.GetComponent<NetworkEntity>() != null;
		}

		// Token: 0x040025E5 RID: 9701
		private const string kLayerName = "Proximity";

		// Token: 0x040025E6 RID: 9702
		[SerializeField]
		private SensorBand m_band;

		// Token: 0x040025E7 RID: 9703
		private Collider m_collider;

		// Token: 0x040025EA RID: 9706
		private float m_timeOfNextUpdate;
	}
}

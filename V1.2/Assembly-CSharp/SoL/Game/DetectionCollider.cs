using System;
using System.Collections.Generic;
using SoL.Networking.Objects;
using UnityEngine;

namespace SoL.Game
{
	// Token: 0x0200056C RID: 1388
	public class DetectionCollider : GameEntityComponent
	{
		// Token: 0x06002AD6 RID: 10966 RVA: 0x0005DA92 File Offset: 0x0005BC92
		public static bool TryGetEntityForCollider(Collider collider, out GameEntity entity)
		{
			return DetectionCollider.m_detectionColliders.TryGetValue(collider, out entity);
		}

		// Token: 0x170008E3 RID: 2275
		// (get) Token: 0x06002AD7 RID: 10967 RVA: 0x0005DAA0 File Offset: 0x0005BCA0
		// (set) Token: 0x06002AD8 RID: 10968 RVA: 0x0005DAA8 File Offset: 0x0005BCA8
		public NetworkEntity NetworkEntity { get; private set; }

		// Token: 0x170008E4 RID: 2276
		// (get) Token: 0x06002AD9 RID: 10969 RVA: 0x0005DAB1 File Offset: 0x0005BCB1
		public Collider Collider
		{
			get
			{
				if (!this.m_colliderCached && this.m_collider == null)
				{
					this.m_collider = base.gameObject.GetComponent<Collider>();
					this.m_colliderCached = true;
				}
				return this.m_collider;
			}
		}

		// Token: 0x06002ADA RID: 10970 RVA: 0x0005DAE7 File Offset: 0x0005BCE7
		private void OnDestroy()
		{
			if (this.Collider != null)
			{
				DetectionCollider.m_detectionColliders.Remove(this.Collider);
			}
		}

		// Token: 0x06002ADB RID: 10971 RVA: 0x00144D4C File Offset: 0x00142F4C
		public void Init(NetworkEntity netEntity)
		{
			this.NetworkEntity = netEntity;
			if (this.Collider != null)
			{
				DetectionCollider.m_detectionColliders.Add(this.Collider, base.GameEntity);
			}
			if (this.NetworkEntity.IsLocal)
			{
				LocalPlayer.DetectionCollider = this;
			}
			base.gameObject.SetActive(true);
		}

		// Token: 0x04002B1A RID: 11034
		private static readonly Dictionary<Collider, GameEntity> m_detectionColliders = new Dictionary<Collider, GameEntity>();

		// Token: 0x04002B1B RID: 11035
		private bool m_colliderCached;

		// Token: 0x04002B1D RID: 11037
		private Collider m_collider;
	}
}

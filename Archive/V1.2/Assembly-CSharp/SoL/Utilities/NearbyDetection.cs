using System;
using System.Collections.Generic;
using SoL.Game;
using SoL.Managers;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x020002A4 RID: 676
	public class NearbyDetection : MonoBehaviour
	{
		// Token: 0x17000505 RID: 1285
		// (get) Token: 0x0600143E RID: 5182 RVA: 0x00050314 File Offset: 0x0004E514
		public int CurrentlyNearby
		{
			get
			{
				if (this.m_nearbyEntities == null)
				{
					return 0;
				}
				return this.m_nearbyEntities.Count;
			}
		}

		// Token: 0x0600143F RID: 5183 RVA: 0x000F97E4 File Offset: 0x000F79E4
		private void Start()
		{
			if (this.m_collider == null)
			{
				Debug.LogError("No collider on NearbyDetection!");
				base.gameObject.SetActive(false);
				return;
			}
			if (!GameManager.IsServer)
			{
				this.m_collider.enabled = false;
				return;
			}
			this.m_collider.isTrigger = true;
			this.m_collider.gameObject.layer = LayerMap.Detection.Layer;
			this.m_nearbyEntities = new List<GameEntity>(100);
		}

		// Token: 0x06001440 RID: 5184 RVA: 0x000F9864 File Offset: 0x000F7A64
		private void OnTriggerEnter(Collider other)
		{
			if (!GameManager.IsServer)
			{
				return;
			}
			GameEntity gameEntity;
			if (DetectionCollider.TryGetEntityForCollider(other, out gameEntity) && gameEntity.Type == GameEntityType.Player)
			{
				this.m_nearbyEntities.Add(gameEntity);
			}
		}

		// Token: 0x06001441 RID: 5185 RVA: 0x000F9898 File Offset: 0x000F7A98
		private void OnTriggerExit(Collider other)
		{
			if (!GameManager.IsServer)
			{
				return;
			}
			GameEntity gameEntity;
			if (DetectionCollider.TryGetEntityForCollider(other, out gameEntity) && gameEntity.Type == GameEntityType.Player)
			{
				this.m_nearbyEntities.Remove(gameEntity);
			}
		}

		// Token: 0x06001442 RID: 5186 RVA: 0x000F98D0 File Offset: 0x000F7AD0
		internal void FlushNulls()
		{
			for (int i = 0; i < this.m_nearbyEntities.Count; i++)
			{
				if (this.m_nearbyEntities[i] == null)
				{
					this.m_nearbyEntities.RemoveAt(i);
					i--;
				}
			}
		}

		// Token: 0x04001C93 RID: 7315
		[SerializeField]
		private Collider m_collider;

		// Token: 0x04001C94 RID: 7316
		private List<GameEntity> m_nearbyEntities;
	}
}

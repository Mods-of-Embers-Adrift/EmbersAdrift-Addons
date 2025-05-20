using System;
using SoL.Game;
using SoL.Managers;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x02000304 RID: 772
	public class ZoneBoundary : MonoBehaviour
	{
		// Token: 0x060015A8 RID: 5544 RVA: 0x000FD704 File Offset: 0x000FB904
		private void Awake()
		{
			this.m_collider = base.gameObject.GetComponent<Collider>();
			if (this.m_collider == null)
			{
				throw new Exception("No collider on ZoneBoundary!");
			}
			this.m_collider.isTrigger = true;
			LocalZoneManager.RegisterZoneBoundary(this.m_collider.bounds);
		}

		// Token: 0x060015A9 RID: 5545 RVA: 0x000FD758 File Offset: 0x000FB958
		private void OnTriggerExit(Collider other)
		{
			GameEntity x;
			if (GameManager.IsServer || !DetectionCollider.TryGetEntityForCollider(other, out x) || x != LocalPlayer.GameEntity)
			{
				return;
			}
			LocalPlayer.Motor.OutOfBounds();
		}

		// Token: 0x04001DB1 RID: 7601
		private Collider m_collider;
	}
}

using System;
using SoL.Managers;
using SoL.Networking.Database;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game
{
	// Token: 0x02000618 RID: 1560
	public class ZonePointTrigger : ZonePoint
	{
		// Token: 0x06003183 RID: 12675 RVA: 0x00062229 File Offset: 0x00060429
		public override bool IsWithinRange(GameEntity entity)
		{
			return this.m_bounds.WithinBounds(entity.gameObject.transform.position);
		}

		// Token: 0x06003184 RID: 12676 RVA: 0x0015CDE0 File Offset: 0x0015AFE0
		private void Awake()
		{
			this.m_collider = base.gameObject.GetComponent<Collider>();
			if (this.m_collider == null)
			{
				Debug.LogWarning("No collider on zone point trigger on " + base.gameObject.transform.GetPath() + "!");
				base.gameObject.SetActive(false);
				return;
			}
			this.m_accessFlagRequirement &= ~AccessFlags.Active;
			if (!GameManager.IsServer && this.m_accessFlagRequirement != AccessFlags.None && SessionData.User != null && (this.m_accessFlagRequirement & SessionData.User.Flags) == AccessFlags.None)
			{
				this.m_collider.enabled = false;
				return;
			}
			this.RefreshBounds(false);
			this.m_collider.isTrigger = true;
			base.gameObject.layer = LayerMap.Detection.Layer;
		}

		// Token: 0x06003185 RID: 12677 RVA: 0x0015CEB0 File Offset: 0x0015B0B0
		private void OnTriggerEnter(Collider other)
		{
			if (GameManager.IsServer)
			{
				return;
			}
			if (LocalPlayer.DetectionCollider == null || LocalPlayer.DetectionCollider.Collider == null || LocalPlayer.DetectionCollider.Collider != other || LocalPlayer.NetworkEntity == null)
			{
				return;
			}
			if (!base.LocalPlayerIsAlive(false))
			{
				return;
			}
			LoginApiManager.PerformZoneCheck(this.m_targetZone, new Action<bool>(this.ZoneCheckResponse));
		}

		// Token: 0x06003186 RID: 12678 RVA: 0x0015CF28 File Offset: 0x0015B128
		public void RefreshBounds(bool updatePosition)
		{
			if (this.m_collider)
			{
				this.m_bounds = this.m_collider.bounds;
				this.m_bounds.size = this.m_bounds.size + this.m_boundsSizeBuffer;
				if (updatePosition)
				{
					this.m_bounds.center = base.gameObject.transform.position;
				}
			}
		}

		// Token: 0x06003187 RID: 12679 RVA: 0x00062246 File Offset: 0x00060446
		private void ZoneCheckResponse(bool isActive)
		{
			if (isActive && base.LocalPlayerIsAlive(false))
			{
				LocalPlayer.NetworkEntity.PlayerRpcHandler.RequestZone((int)this.m_targetZone, this.m_targetZonePointIndex, ZonePoint.RegisterZoneRequest(this));
			}
		}

		// Token: 0x06003188 RID: 12680 RVA: 0x0015CF90 File Offset: 0x0015B190
		private void OnDrawGizmosSelected()
		{
			if (this.m_boundsSizeBuffer == Vector3.zero)
			{
				return;
			}
			Collider component = base.gameObject.GetComponent<Collider>();
			if (component == null)
			{
				return;
			}
			Bounds bounds = component.bounds;
			bounds.size += this.m_boundsSizeBuffer;
			float radius = Mathf.Max(new float[]
			{
				bounds.extents.x,
				bounds.extents.y,
				bounds.extents.z
			});
			Gizmos.DrawWireSphere(base.gameObject.transform.position, radius);
		}

		// Token: 0x04002FE1 RID: 12257
		[SerializeField]
		private Vector3 m_boundsSizeBuffer = Vector3.zero;

		// Token: 0x04002FE2 RID: 12258
		[SerializeField]
		private AccessFlags m_accessFlagRequirement;

		// Token: 0x04002FE3 RID: 12259
		private Collider m_collider;

		// Token: 0x04002FE4 RID: 12260
		private Bounds m_bounds;
	}
}

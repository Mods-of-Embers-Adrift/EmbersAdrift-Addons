using System;
using SoL.Managers;
using UnityEngine;
using UnityEngine.Serialization;

namespace SoL.Utilities
{
	// Token: 0x02000302 RID: 770
	public class WaterCollider : MonoBehaviour
	{
		// Token: 0x060015A6 RID: 5542 RVA: 0x000FD4D8 File Offset: 0x000FB6D8
		private void Awake()
		{
			if (this.m_waterCollider == null)
			{
				return;
			}
			WaterCollider.ColliderType type = this.m_type;
			if (type != WaterCollider.ColliderType.Client)
			{
				if (type != WaterCollider.ColliderType.Server)
				{
					return;
				}
				if (GameManager.IsServer)
				{
					if (this.m_waterCollider)
					{
						this.m_waterCollider.gameObject.tag = "Water";
						this.m_waterCollider.gameObject.layer = LayerMap.Water.Layer;
					}
					if (this.m_cameraCollider)
					{
						this.m_cameraCollider.enabled = false;
					}
					if (this.m_invertedCameraCollider)
					{
						this.m_invertedCameraCollider.enabled = false;
						return;
					}
				}
				else
				{
					if (this.m_waterCollider)
					{
						this.m_waterCollider.enabled = false;
					}
					if (this.m_cameraCollider)
					{
						this.m_cameraCollider.enabled = false;
					}
					if (this.m_invertedCameraCollider)
					{
						this.m_invertedCameraCollider.enabled = false;
					}
				}
			}
			else if (GameManager.IsServer)
			{
				if (this.m_waterCollider)
				{
					this.m_waterCollider.enabled = false;
				}
				if (this.m_cameraCollider)
				{
					this.m_cameraCollider.enabled = false;
				}
				if (this.m_invertedCameraCollider)
				{
					this.m_invertedCameraCollider.enabled = false;
					return;
				}
			}
			else
			{
				if (this.m_waterCollider)
				{
					GameObject gameObject = this.m_waterCollider.gameObject;
					gameObject.tag = "Water";
					gameObject.layer = LayerMap.Water.Layer;
					Vector3 localPosition = gameObject.transform.localPosition;
					localPosition.y = -1.25f;
					gameObject.transform.localPosition = localPosition;
				}
				if (this.m_cameraCollider)
				{
					this.m_cameraCollider.gameObject.tag = "Water";
					this.m_cameraCollider.gameObject.layer = LayerMap.CameraCollidePlayerIgnore.Layer;
				}
				if (this.m_invertedCameraCollider)
				{
					this.m_invertedCameraCollider.gameObject.tag = "Water";
					this.m_invertedCameraCollider.gameObject.layer = LayerMap.CameraCollidePlayerIgnore.Layer;
					return;
				}
			}
		}

		// Token: 0x04001DA9 RID: 7593
		private const float kPlayerCollisionOffset = -1.25f;

		// Token: 0x04001DAA RID: 7594
		[SerializeField]
		private WaterCollider.ColliderType m_type;

		// Token: 0x04001DAB RID: 7595
		[FormerlySerializedAs("m_collider")]
		[SerializeField]
		private Collider m_waterCollider;

		// Token: 0x04001DAC RID: 7596
		[SerializeField]
		private Collider m_cameraCollider;

		// Token: 0x04001DAD RID: 7597
		[SerializeField]
		private Collider m_invertedCameraCollider;

		// Token: 0x02000303 RID: 771
		private enum ColliderType
		{
			// Token: 0x04001DAF RID: 7599
			Client,
			// Token: 0x04001DB0 RID: 7600
			Server
		}
	}
}

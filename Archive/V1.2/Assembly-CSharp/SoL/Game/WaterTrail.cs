using System;
using SoL.Game.Audio;
using SoL.Managers;
using UnityEngine;

namespace SoL.Game
{
	// Token: 0x02000604 RID: 1540
	public class WaterTrail : GameEntityComponent
	{
		// Token: 0x06003125 RID: 12581 RVA: 0x0015C2A4 File Offset: 0x0015A4A4
		private void Start()
		{
			if (!base.GameEntity)
			{
				base.enabled = false;
				return;
			}
			if (base.GameEntity.GroundSampler)
			{
				base.GameEntity.GroundSampler.WaterColliderChanged += this.GroundSamplerOnWaterColliderChanged;
			}
		}

		// Token: 0x06003126 RID: 12582 RVA: 0x00061DB7 File Offset: 0x0005FFB7
		private void OnDestroy()
		{
			if (base.GameEntity.GroundSampler)
			{
				base.GameEntity.GroundSampler.WaterColliderChanged -= this.GroundSamplerOnWaterColliderChanged;
			}
			this.DisconnectWaterTrailController();
		}

		// Token: 0x06003127 RID: 12583 RVA: 0x0015C2F4 File Offset: 0x0015A4F4
		private void GroundSamplerOnWaterColliderChanged(Collider obj)
		{
			bool flag = base.GameEntity == LocalPlayer.GameEntity;
			if (this.m_waterTrailController == null)
			{
				if (ClientGameManager.WaterTrailManager)
				{
					if (flag)
					{
						this.ToggleForLocal(obj);
					}
					this.m_waterTrailController = ClientGameManager.WaterTrailManager.RequestWaterTrail(this, obj);
				}
				return;
			}
			if (obj)
			{
				if (flag)
				{
					this.ToggleForLocal(obj);
				}
				this.m_waterTrailController.Height = obj.gameObject.transform.position.y;
				return;
			}
			if (flag)
			{
				this.ToggleForLocal(obj);
			}
			this.DisconnectWaterTrailController();
		}

		// Token: 0x06003128 RID: 12584 RVA: 0x0015C388 File Offset: 0x0015A588
		private void DisconnectWaterTrailController()
		{
			if (this.m_waterTrailController != null && base.GameEntity && this.m_waterTrailController.Entity == base.GameEntity)
			{
				this.m_waterTrailController.Entity = null;
			}
			this.m_waterTrailController = null;
		}

		// Token: 0x06003129 RID: 12585 RVA: 0x0015C3D8 File Offset: 0x0015A5D8
		private void ToggleForLocal(Collider obj)
		{
			if (this.m_previousCollider != obj && this.m_previousWaterAudioData)
			{
				this.m_previousWaterAudioData.ResetAudio();
			}
			this.m_previousCollider = obj;
			this.m_previousWaterAudioData = null;
			if (!obj)
			{
				return;
			}
			WaterAudioData waterAudioData;
			if (obj.gameObject.TryGetComponent<WaterAudioData>(out waterAudioData))
			{
				waterAudioData.InitAudio(base.GameEntity);
				this.m_previousWaterAudioData = waterAudioData;
			}
		}

		// Token: 0x04002F5C RID: 12124
		private WaterTrailManager.WaterTrailController m_waterTrailController;

		// Token: 0x04002F5D RID: 12125
		private Collider m_previousCollider;

		// Token: 0x04002F5E RID: 12126
		private WaterAudioData m_previousWaterAudioData;
	}
}

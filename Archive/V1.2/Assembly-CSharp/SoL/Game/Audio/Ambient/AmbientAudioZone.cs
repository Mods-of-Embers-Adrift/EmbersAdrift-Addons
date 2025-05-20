using System;
using SoL.Managers;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Audio.Ambient
{
	// Token: 0x02000D2B RID: 3371
	public class AmbientAudioZone : MonoBehaviour, IAmbientAudioZone
	{
		// Token: 0x17001855 RID: 6229
		// (get) Token: 0x0600656E RID: 25966 RVA: 0x000844D3 File Offset: 0x000826D3
		public AmbientAudioZoneProfile Profile
		{
			get
			{
				return this.m_profile;
			}
		}

		// Token: 0x17001856 RID: 6230
		// (get) Token: 0x0600656F RID: 25967 RVA: 0x00082BC2 File Offset: 0x00080DC2
		public int Key
		{
			get
			{
				return this.GetHashCode();
			}
		}

		// Token: 0x06006570 RID: 25968 RVA: 0x0020CE04 File Offset: 0x0020B004
		private void Awake()
		{
			if (this.m_collider == null || this.m_profile == null || this.m_profile.GetAudioClip() == null)
			{
				base.enabled = false;
				if (this.m_collider != null)
				{
					this.m_collider.enabled = false;
				}
				return;
			}
			if (this.m_collider != null)
			{
				this.m_collider.isTrigger = true;
				this.m_collider.gameObject.layer = LayerMap.Detection.Layer;
			}
		}

		// Token: 0x06006571 RID: 25969 RVA: 0x0020CE9C File Offset: 0x0020B09C
		private void OnTriggerEnter(Collider other)
		{
			if (GameManager.IsServer)
			{
				return;
			}
			GameEntity gameEntity;
			if (DetectionCollider.TryGetEntityForCollider(other, out gameEntity) && gameEntity.NetworkEntity && gameEntity.NetworkEntity.IsLocal)
			{
				this.AddZone(gameEntity);
			}
		}

		// Token: 0x06006572 RID: 25970 RVA: 0x0020CEDC File Offset: 0x0020B0DC
		private void OnTriggerExit(Collider other)
		{
			if (GameManager.IsServer)
			{
				return;
			}
			GameEntity gameEntity;
			if (DetectionCollider.TryGetEntityForCollider(other, out gameEntity) && gameEntity.NetworkEntity && gameEntity.NetworkEntity.IsLocal)
			{
				this.RemoveZone(gameEntity);
			}
		}

		// Token: 0x06006573 RID: 25971 RVA: 0x000844DB File Offset: 0x000826DB
		private void AddZone(GameEntity entity)
		{
			if (entity.AmbientAudioController != null)
			{
				entity.AmbientAudioController.EnterZone(this);
			}
		}

		// Token: 0x06006574 RID: 25972 RVA: 0x000844F7 File Offset: 0x000826F7
		private void RemoveZone(GameEntity entity)
		{
			if (entity.AmbientAudioController != null)
			{
				entity.AmbientAudioController.ExitZone(this);
			}
		}

		// Token: 0x04005833 RID: 22579
		[SerializeField]
		private AmbientAudioZoneProfile m_profile;

		// Token: 0x04005834 RID: 22580
		[SerializeField]
		private Collider m_collider;
	}
}

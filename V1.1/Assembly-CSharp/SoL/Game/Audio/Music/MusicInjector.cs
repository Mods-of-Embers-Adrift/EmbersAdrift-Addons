using System;
using SoL.Managers;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Audio.Music
{
	// Token: 0x02000D20 RID: 3360
	public class MusicInjector : MonoBehaviour
	{
		// Token: 0x17001841 RID: 6209
		// (get) Token: 0x0600651A RID: 25882 RVA: 0x0020BB34 File Offset: 0x00209D34
		private MusicSetList SetList
		{
			get
			{
				MusicInjector.InjectionType type = this.m_type;
				if (type == MusicInjector.InjectionType.Set)
				{
					return this.m_set;
				}
				if (type != MusicInjector.InjectionType.Collection)
				{
					throw new ArgumentException("m_type");
				}
				if (!(this.m_collection == null))
				{
					return this.m_collection.SetList;
				}
				return null;
			}
		}

		// Token: 0x0600651B RID: 25883 RVA: 0x0020BB80 File Offset: 0x00209D80
		private void Awake()
		{
			if (this.m_collider != null)
			{
				if (this.SetList == null || this.SetList.Type != MusicChannelType.Area)
				{
					this.m_collider.enabled = false;
					return;
				}
				this.m_collider.isTrigger = true;
				this.m_collider.gameObject.layer = LayerMap.Detection.Layer;
			}
		}

		// Token: 0x0600651C RID: 25884 RVA: 0x000840D0 File Offset: 0x000822D0
		private void Start()
		{
			if (this.m_addOnStart)
			{
				this.AddMusic();
			}
		}

		// Token: 0x0600651D RID: 25885 RVA: 0x0020BBEC File Offset: 0x00209DEC
		private void OnTriggerEnter(Collider other)
		{
			if (GameManager.IsServer)
			{
				return;
			}
			GameEntity gameEntity;
			if (DetectionCollider.TryGetEntityForCollider(other, out gameEntity) && gameEntity.Type == GameEntityType.Player && gameEntity.NetworkEntity.IsLocal)
			{
				this.AddMusic();
			}
		}

		// Token: 0x0600651E RID: 25886 RVA: 0x0020BC28 File Offset: 0x00209E28
		private void OnTriggerExit(Collider other)
		{
			if (GameManager.IsServer)
			{
				return;
			}
			GameEntity gameEntity;
			if (DetectionCollider.TryGetEntityForCollider(other, out gameEntity) && gameEntity.Type == GameEntityType.Player && gameEntity.NetworkEntity.IsLocal)
			{
				this.RemoveMusic();
			}
		}

		// Token: 0x0600651F RID: 25887 RVA: 0x000840E0 File Offset: 0x000822E0
		private void AddMusic()
		{
			if (ClientGameManager.MusicManager)
			{
				ClientGameManager.MusicManager.AddMusic(this.SetList, true);
			}
		}

		// Token: 0x06006520 RID: 25888 RVA: 0x000840FF File Offset: 0x000822FF
		private void RemoveMusic()
		{
			if (ClientGameManager.MusicManager)
			{
				ClientGameManager.MusicManager.RemoveMusic(this.SetList, true);
			}
		}

		// Token: 0x040057E4 RID: 22500
		[SerializeField]
		private MusicInjector.InjectionType m_type;

		// Token: 0x040057E5 RID: 22501
		[SerializeField]
		private bool m_addOnStart;

		// Token: 0x040057E6 RID: 22502
		[SerializeField]
		private Collider m_collider;

		// Token: 0x040057E7 RID: 22503
		[SerializeField]
		private MusicSetList m_set;

		// Token: 0x040057E8 RID: 22504
		[SerializeField]
		private MusicCollection m_collection;

		// Token: 0x02000D21 RID: 3361
		private enum InjectionType
		{
			// Token: 0x040057EA RID: 22506
			Set,
			// Token: 0x040057EB RID: 22507
			Collection
		}
	}
}

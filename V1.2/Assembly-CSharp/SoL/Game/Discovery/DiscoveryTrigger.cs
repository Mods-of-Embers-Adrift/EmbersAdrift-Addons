using System;
using System.Collections;
using SoL.Managers;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Discovery
{
	// Token: 0x02000CA8 RID: 3240
	public class DiscoveryTrigger : MonoBehaviour
	{
		// Token: 0x17001770 RID: 6000
		// (get) Token: 0x06006231 RID: 25137 RVA: 0x00082277 File Offset: 0x00080477
		public DiscoveryProfile Profile
		{
			get
			{
				return this.m_profile;
			}
		}

		// Token: 0x17001771 RID: 6001
		// (get) Token: 0x06006232 RID: 25138 RVA: 0x0008227F File Offset: 0x0008047F
		public UniqueId DiscoveryId
		{
			get
			{
				if (!(this.m_profile == null))
				{
					return this.m_profile.Id;
				}
				return UniqueId.Empty;
			}
		}

		// Token: 0x17001772 RID: 6002
		// (get) Token: 0x06006233 RID: 25139 RVA: 0x000822A0 File Offset: 0x000804A0
		private IEnumerable GetDiscoveryProfiles
		{
			get
			{
				return SolOdinUtilities.GetDropdownItems<DiscoveryProfile>();
			}
		}

		// Token: 0x06006234 RID: 25140 RVA: 0x00203BE8 File Offset: 0x00201DE8
		private void Awake()
		{
			if (this.m_collider == null)
			{
				base.enabled = false;
				return;
			}
			if (this.m_profile == null)
			{
				Debug.LogWarning("DiscoveryTrigger missing DiscoveryProfile on " + base.gameObject.name + "!");
				base.enabled = false;
				return;
			}
			this.m_collider.isTrigger = true;
			base.gameObject.layer = LayerMap.Detection.Layer;
		}

		// Token: 0x06006235 RID: 25141 RVA: 0x00203C68 File Offset: 0x00201E68
		public void OnTriggerEnter(Collider other)
		{
			GameEntity gameEntity;
			if (this.m_profile != null && DetectionCollider.TryGetEntityForCollider(other, out gameEntity) && gameEntity.Type == GameEntityType.Player)
			{
				this.DiscoveryForEntity(gameEntity);
				if (!GameManager.IsServer && gameEntity == LocalPlayer.GameEntity && ClientGameManager.UIManager != null && ClientGameManager.UIManager.MapUI != null)
				{
					ClientGameManager.UIManager.MapUI.EnableDiscoveryHighlight(this.m_profile);
				}
			}
		}

		// Token: 0x06006236 RID: 25142 RVA: 0x00203CE8 File Offset: 0x00201EE8
		public void OnTriggerExit(Collider other)
		{
			GameEntity gameEntity;
			if (!GameManager.IsServer && this.m_profile != null && DetectionCollider.TryGetEntityForCollider(other, out gameEntity) && gameEntity.Type == GameEntityType.Player && gameEntity == LocalPlayer.GameEntity && ClientGameManager.UIManager != null && ClientGameManager.UIManager.MapUI != null)
			{
				ClientGameManager.UIManager.MapUI.DisableDiscoveryHighlight(this.m_profile);
			}
		}

		// Token: 0x06006237 RID: 25143 RVA: 0x0004475B File Offset: 0x0004295B
		public void DiscoveryForEntity(GameEntity entity)
		{
		}

		// Token: 0x040055C4 RID: 21956
		[SerializeField]
		private DiscoveryProfile m_profile;

		// Token: 0x040055C5 RID: 21957
		[SerializeField]
		private Collider m_collider;
	}
}

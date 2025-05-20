using System;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game
{
	// Token: 0x02000611 RID: 1553
	public class LocalDefaultPlayerSpawn : MonoBehaviour
	{
		// Token: 0x06003160 RID: 12640 RVA: 0x000620DF File Offset: 0x000602DF
		private void Awake()
		{
			if (this.m_collider == null)
			{
				base.enabled = false;
				return;
			}
			base.gameObject.layer = LayerMap.Detection.Layer;
			this.m_collider.isTrigger = true;
		}

		// Token: 0x06003161 RID: 12641 RVA: 0x0015C820 File Offset: 0x0015AA20
		private void OnTriggerEnter(Collider other)
		{
			GameEntity gameEntity;
			if (DetectionCollider.TryGetEntityForCollider(other, out gameEntity) && gameEntity && gameEntity.Type == GameEntityType.Player)
			{
				gameEntity.LocalDefaultPlayerSpawn = this.m_spawn;
			}
		}

		// Token: 0x06003162 RID: 12642 RVA: 0x0015C854 File Offset: 0x0015AA54
		private void OnTriggerExit(Collider other)
		{
			GameEntity gameEntity;
			if (DetectionCollider.TryGetEntityForCollider(other, out gameEntity) && gameEntity && gameEntity.Type == GameEntityType.Player && gameEntity.LocalDefaultPlayerSpawn == this.m_spawn)
			{
				gameEntity.LocalDefaultPlayerSpawn = null;
			}
		}

		// Token: 0x04002FC2 RID: 12226
		[SerializeField]
		private Collider m_collider;

		// Token: 0x04002FC3 RID: 12227
		[SerializeField]
		private PlayerSpawn m_spawn;
	}
}

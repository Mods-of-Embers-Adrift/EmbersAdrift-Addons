using System;
using SoL.Game.Culling;
using SoL.Managers;
using UnityEngine;

namespace SoL.Game.Nameplates
{
	// Token: 0x020009D2 RID: 2514
	public class CulledOverheadNameplate : CulledObject, IOverheadNameplateSpawner
	{
		// Token: 0x06004C89 RID: 19593 RVA: 0x00073C53 File Offset: 0x00071E53
		private void Awake()
		{
			if (!this.m_disable && !GameManager.IsServer && this.m_gameEntity)
			{
				this.m_gameEntity.OverheadNameplate = this;
			}
		}

		// Token: 0x06004C8A RID: 19594 RVA: 0x001BCF64 File Offset: 0x001BB164
		protected override void RefreshCullee()
		{
			base.RefreshCullee();
			if (this.IsCulled())
			{
				if (this.m_controller)
				{
					this.m_controller.ResetData();
					this.m_controller = null;
					return;
				}
			}
			else if (!this.m_controller)
			{
				this.m_controller = ClientGameManager.OverheadNameplateManager.RequestNameplate(this.m_gameEntity);
			}
		}

		// Token: 0x06004C8B RID: 19595 RVA: 0x001BCFC4 File Offset: 0x001BB1C4
		public void Initialize()
		{
			if (!GameManager.IsServer && this.m_gameEntity != null && ClientGameManager.OverheadNameplateManager != null)
			{
				if (this.m_gameEntity.NameplateHeightOffset == null)
				{
					Vector3 value = this.m_heightOffset;
					if (this.m_gameEntity.NpcReferencePoints != null && this.m_gameEntity.NpcReferencePoints.Overhead != null)
					{
						value = this.m_gameEntity.NpcReferencePoints.Overhead.transform.localPosition;
					}
					this.m_gameEntity.NameplateHeightOffset = new Vector3?(value);
				}
				if (base.InternalInitialized)
				{
					this.RefreshCullee();
				}
			}
		}

		// Token: 0x06004C8C RID: 19596 RVA: 0x00073C7D File Offset: 0x00071E7D
		public void SetController(WorldSpaceOverheadController controller)
		{
			this.m_controller = controller;
		}

		// Token: 0x0400466B RID: 18027
		[SerializeField]
		private GameEntity m_gameEntity;

		// Token: 0x0400466C RID: 18028
		[SerializeField]
		private bool m_disable;

		// Token: 0x0400466D RID: 18029
		[SerializeField]
		private Vector3 m_heightOffset = WorldSpaceOverheadController.kDefaultHeightOffset;

		// Token: 0x0400466E RID: 18030
		private WorldSpaceOverheadController m_controller;
	}
}

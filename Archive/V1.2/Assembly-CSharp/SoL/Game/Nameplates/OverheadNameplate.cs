using System;
using SoL.Managers;
using UnityEngine;

namespace SoL.Game.Nameplates
{
	// Token: 0x020009D9 RID: 2521
	public class OverheadNameplate : GameEntityComponent, IOverheadNameplateSpawner
	{
		// Token: 0x06004CB3 RID: 19635 RVA: 0x00073E0E File Offset: 0x0007200E
		private void Awake()
		{
			if (!this.m_disable && !GameManager.IsServer && base.GameEntity)
			{
				base.GameEntity.OverheadNameplate = this;
			}
		}

		// Token: 0x06004CB4 RID: 19636 RVA: 0x001BD918 File Offset: 0x001BBB18
		public void Initialize()
		{
			if (!GameManager.IsServer && base.GameEntity != null && ClientGameManager.OverheadNameplateManager != null)
			{
				if (base.GameEntity.NameplateHeightOffset == null)
				{
					Vector3 value = this.m_heightOffset;
					if (base.GameEntity.NpcReferencePoints != null && base.GameEntity.NpcReferencePoints.Overhead != null)
					{
						value = base.GameEntity.NpcReferencePoints.Overhead.transform.localPosition;
					}
					base.GameEntity.NameplateHeightOffset = new Vector3?(value);
				}
				this.m_controller = ClientGameManager.OverheadNameplateManager.RequestNameplate(base.GameEntity);
			}
		}

		// Token: 0x06004CB5 RID: 19637 RVA: 0x00073E38 File Offset: 0x00072038
		private void OnDestroy()
		{
			if (this.m_controller != null)
			{
				this.m_controller.ResetData();
			}
		}

		// Token: 0x06004CB6 RID: 19638 RVA: 0x00073E53 File Offset: 0x00072053
		public void SetController(WorldSpaceOverheadController controller)
		{
			this.m_controller = controller;
		}

		// Token: 0x0400468C RID: 18060
		[SerializeField]
		private bool m_disable;

		// Token: 0x0400468D RID: 18061
		[SerializeField]
		private Vector3 m_heightOffset = WorldSpaceOverheadController.kDefaultHeightOffset;

		// Token: 0x0400468E RID: 18062
		private WorldSpaceOverheadController m_controller;
	}
}

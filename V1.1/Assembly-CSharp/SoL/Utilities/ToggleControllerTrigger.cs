using System;
using SoL.Game;
using SoL.Managers;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x020002F6 RID: 758
	public class ToggleControllerTrigger : MonoBehaviour
	{
		// Token: 0x06001576 RID: 5494 RVA: 0x000FC8D4 File Offset: 0x000FAAD4
		protected virtual void Awake()
		{
			if (GameManager.IsServer || !this.m_controller)
			{
				if (this.m_collider)
				{
					this.m_collider.enabled = false;
				}
				base.enabled = false;
				return;
			}
			this.m_collider.gameObject.layer = LayerMap.Detection.Layer;
			this.m_collider.isTrigger = true;
		}

		// Token: 0x06001577 RID: 5495 RVA: 0x000FC944 File Offset: 0x000FAB44
		private void OnTriggerEnter(Collider other)
		{
			if (GameManager.IsServer || !this.m_controller)
			{
				return;
			}
			GameEntity gameEntity;
			if (DetectionCollider.TryGetEntityForCollider(other, out gameEntity) && gameEntity.Type == GameEntityType.Player && gameEntity == LocalPlayer.GameEntity)
			{
				this.m_controller.State = this.m_stateOnEnter;
			}
		}

		// Token: 0x04001D93 RID: 7571
		[SerializeField]
		private Collider m_collider;

		// Token: 0x04001D94 RID: 7572
		[SerializeField]
		protected ToggleController m_controller;

		// Token: 0x04001D95 RID: 7573
		[SerializeField]
		protected ToggleController.ToggleState m_stateOnEnter = ToggleController.ToggleState.ON;
	}
}

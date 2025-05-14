using System;
using SoL.Game;
using SoL.Managers;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x020002F7 RID: 759
	public class ToggleControllerTriggerSingle : ToggleControllerTrigger
	{
		// Token: 0x17000527 RID: 1319
		// (get) Token: 0x06001579 RID: 5497 RVA: 0x000511AD File Offset: 0x0004F3AD
		private ToggleController.ToggleState OppositeState
		{
			get
			{
				if (this.m_stateOnEnter != ToggleController.ToggleState.ON)
				{
					return ToggleController.ToggleState.ON;
				}
				return ToggleController.ToggleState.OFF;
			}
		}

		// Token: 0x0600157A RID: 5498 RVA: 0x000511BB File Offset: 0x0004F3BB
		protected override void Awake()
		{
			base.Awake();
			if (base.enabled && this.m_controller)
			{
				this.m_controller.State = this.OppositeState;
			}
		}

		// Token: 0x0600157B RID: 5499 RVA: 0x000FC9B8 File Offset: 0x000FABB8
		private void OnTriggerExit(Collider other)
		{
			if (GameManager.IsServer || !this.m_controller)
			{
				return;
			}
			GameEntity gameEntity;
			if (DetectionCollider.TryGetEntityForCollider(other, out gameEntity) && gameEntity.Type == GameEntityType.Player && gameEntity == LocalPlayer.GameEntity)
			{
				this.m_controller.State = this.OppositeState;
			}
		}
	}
}

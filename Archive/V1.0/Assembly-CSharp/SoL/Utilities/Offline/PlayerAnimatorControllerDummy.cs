using System;
using SoL.Game.Player;
using SoL.Managers;
using UnityEngine;

namespace SoL.Utilities.Offline
{
	// Token: 0x0200030F RID: 783
	public class PlayerAnimatorControllerDummy : MonoBehaviour
	{
		// Token: 0x060015EB RID: 5611 RVA: 0x000515AE File Offset: 0x0004F7AE
		private void Awake()
		{
			this.m_locoForwardKey = Animator.StringToHash("LocomotionForward");
			this.m_locoRightKey = Animator.StringToHash("LocomotionRight");
		}

		// Token: 0x060015EC RID: 5612 RVA: 0x000FDEC4 File Offset: 0x000FC0C4
		private void Update()
		{
			if (ClientGameManager.InputManager == null)
			{
				return;
			}
			Vector2 movementInput = ClientGameManager.InputManager.MovementInput;
			this.m_animator.SetFloat(this.m_locoForwardKey, movementInput.y);
			this.m_animator.SetFloat(this.m_locoRightKey, movementInput.x);
		}

		// Token: 0x060015ED RID: 5613 RVA: 0x000515D0 File Offset: 0x0004F7D0
		private void OnAnimatorMove()
		{
			if (this.m_dummyMotorController != null)
			{
				this.m_dummyMotorController.SetRootMotionDelta(this.m_animator.deltaPosition);
			}
		}

		// Token: 0x060015EE RID: 5614 RVA: 0x0004475B File Offset: 0x0004295B
		private void Run()
		{
		}

		// Token: 0x060015EF RID: 5615 RVA: 0x0004475B File Offset: 0x0004295B
		private void Walk()
		{
		}

		// Token: 0x04001DE0 RID: 7648
		[SerializeField]
		private Animator m_animator;

		// Token: 0x04001DE1 RID: 7649
		[SerializeField]
		private PlayerMotorControllerDummy m_dummyMotorController;

		// Token: 0x04001DE2 RID: 7650
		private int m_locoForwardKey;

		// Token: 0x04001DE3 RID: 7651
		private int m_locoRightKey;
	}
}

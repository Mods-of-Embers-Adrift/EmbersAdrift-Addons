using System;
using UnityEngine;
using UnityEngine.Events;

namespace SoL.Game.Login.Client
{
	// Token: 0x02000B39 RID: 2873
	public class LoginStageTrigger : MonoBehaviour
	{
		// Token: 0x06005854 RID: 22612 RVA: 0x0007AF90 File Offset: 0x00079190
		private void Awake()
		{
			LoginController.StageChanged += this.LoginControllerOnStageChanged;
		}

		// Token: 0x06005855 RID: 22613 RVA: 0x0007AFA3 File Offset: 0x000791A3
		private void OnDestroy()
		{
			LoginController.StageChanged -= this.LoginControllerOnStageChanged;
		}

		// Token: 0x06005856 RID: 22614 RVA: 0x0007AFB6 File Offset: 0x000791B6
		private void LoginControllerOnStageChanged(LoginStageType obj)
		{
			if (this.m_stageType == obj)
			{
				UnityEvent @event = this.m_event;
				if (@event == null)
				{
					return;
				}
				@event.Invoke();
			}
		}

		// Token: 0x04004DB7 RID: 19895
		[SerializeField]
		private LoginStageType m_stageType;

		// Token: 0x04004DB8 RID: 19896
		[SerializeField]
		private UnityEvent m_event;
	}
}

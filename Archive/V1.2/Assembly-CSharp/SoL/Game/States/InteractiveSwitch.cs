using System;
using SoL.Game.Audio;
using SoL.Managers;
using UnityEngine;

namespace SoL.Game.States
{
	// Token: 0x02000660 RID: 1632
	public class InteractiveSwitch : InteractiveState
	{
		// Token: 0x17000AD8 RID: 2776
		// (get) Token: 0x060032D3 RID: 13011 RVA: 0x00161720 File Offset: 0x0015F920
		protected override bool PreventClientFromProgressing
		{
			get
			{
				return base.PreventClientFromProgressing || (this.m_disableInteractWhileActive != null && this.m_disableInteractWhileActive.State && this.m_disableInteractWhileActive.State.CurrentState == this.m_disableInteractWhileActive.Value);
			}
		}

		// Token: 0x060032D4 RID: 13012 RVA: 0x00161770 File Offset: 0x0015F970
		public override bool CanInteract(GameEntity entity)
		{
			return (this.m_disableInteractWhileActive == null || !this.m_disableInteractWhileActive.State || this.m_disableInteractWhileActive.State.CurrentState != this.m_disableInteractWhileActive.Value) && base.CanInteract(entity);
		}

		// Token: 0x060032D5 RID: 13013 RVA: 0x001617C0 File Offset: 0x0015F9C0
		protected override void StateChangedInternal()
		{
			base.StateChangedInternal();
			if (!GameManager.IsServer)
			{
				if (this.m_animator)
				{
					this.m_animator.SetInteger(BaseState.kStateKey, (int)base.CurrentState);
				}
				AudioEvent audioEvent = this.m_audioEvent;
				if (audioEvent == null)
				{
					return;
				}
				audioEvent.Play(1f);
			}
		}

		// Token: 0x04003125 RID: 12581
		[SerializeField]
		private StateSetting m_disableInteractWhileActive;

		// Token: 0x04003126 RID: 12582
		[SerializeField]
		private Animator m_animator;

		// Token: 0x04003127 RID: 12583
		[SerializeField]
		private AudioEvent m_audioEvent;
	}
}

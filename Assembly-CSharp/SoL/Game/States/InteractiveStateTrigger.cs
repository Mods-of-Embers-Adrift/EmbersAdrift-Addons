using System;
using SoL.Game.Audio;
using SoL.Managers;
using UnityEngine;

namespace SoL.Game.States
{
	// Token: 0x0200065F RID: 1631
	public class InteractiveStateTrigger : InteractiveState
	{
		// Token: 0x060032D1 RID: 13009 RVA: 0x001616B0 File Offset: 0x0015F8B0
		protected override void StateChangedInternal()
		{
			base.StateChangedInternal();
			if (!GameManager.IsServer)
			{
				AudioEvent audioEvent = this.m_audioEvent;
				if (audioEvent != null)
				{
					audioEvent.Play(1f);
				}
				if (this.m_animator)
				{
					this.m_animator.SetTrigger(this.m_animatorTrigger);
				}
			}
		}

		// Token: 0x04003122 RID: 12578
		[SerializeField]
		private AudioEvent m_audioEvent;

		// Token: 0x04003123 RID: 12579
		[SerializeField]
		private Animator m_animator;

		// Token: 0x04003124 RID: 12580
		[SerializeField]
		private string m_animatorTrigger;
	}
}

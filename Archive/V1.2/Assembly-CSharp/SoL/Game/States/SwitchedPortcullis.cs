using System;
using SoL.Game.Audio;
using SoL.Managers;
using UnityEngine;

namespace SoL.Game.States
{
	// Token: 0x02000668 RID: 1640
	public class SwitchedPortcullis : BaseState
	{
		// Token: 0x060032FC RID: 13052 RVA: 0x00063146 File Offset: 0x00061346
		protected override void Awake()
		{
			if (GameManager.IsOnline)
			{
				this.RefreshServerCollider();
				if (GameManager.IsServer && this.m_visuals)
				{
					this.m_visuals.SetActive(false);
				}
			}
			base.Awake();
		}

		// Token: 0x060032FD RID: 13053 RVA: 0x0006317B File Offset: 0x0006137B
		private void RefreshServerCollider()
		{
			if (this.m_serverCollider)
			{
				this.m_serverCollider.SetActive(GameManager.IsServer && base.CurrentState == 0);
			}
		}

		// Token: 0x060032FE RID: 13054 RVA: 0x00161CD4 File Offset: 0x0015FED4
		protected override void StateChangedInternal()
		{
			this.RefreshServerCollider();
			if (!GameManager.IsServer)
			{
				this.m_animator.SetInteger(BaseState.kStateKey, (int)base.CurrentState);
				if (base.CurrentState == 1)
				{
					AudioEvent openAudio = this.m_openAudio;
					if (openAudio != null)
					{
						openAudio.Play(1f);
					}
				}
				else
				{
					AudioEvent closeAudio = this.m_closeAudio;
					if (closeAudio != null)
					{
						closeAudio.Play(1f);
					}
				}
			}
			base.StateChangedInternal();
		}

		// Token: 0x060032FF RID: 13055 RVA: 0x000631A8 File Offset: 0x000613A8
		private void Open()
		{
			this.m_animator.SetInteger(BaseState.kStateKey, 1);
			AudioEvent openAudio = this.m_openAudio;
			if (openAudio == null)
			{
				return;
			}
			openAudio.Play(1f);
		}

		// Token: 0x06003300 RID: 13056 RVA: 0x000631D0 File Offset: 0x000613D0
		private void Close()
		{
			this.m_animator.SetInteger(BaseState.kStateKey, 0);
			AudioEvent closeAudio = this.m_closeAudio;
			if (closeAudio == null)
			{
				return;
			}
			closeAudio.Play(1f);
		}

		// Token: 0x0400313F RID: 12607
		[SerializeField]
		private Animator m_animator;

		// Token: 0x04003140 RID: 12608
		[SerializeField]
		private GameObject m_visuals;

		// Token: 0x04003141 RID: 12609
		[SerializeField]
		private GameObject m_serverCollider;

		// Token: 0x04003142 RID: 12610
		[SerializeField]
		private AudioEvent m_openAudio;

		// Token: 0x04003143 RID: 12611
		[SerializeField]
		private AudioEvent m_closeAudio;
	}
}

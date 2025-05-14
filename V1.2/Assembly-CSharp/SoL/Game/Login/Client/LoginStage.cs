using System;
using SoL.UI;
using SoL.Utilities;
using TMPro;
using UnityEngine;

namespace SoL.Game.Login.Client
{
	// Token: 0x02000B31 RID: 2865
	public abstract class LoginStage : ToggleController
	{
		// Token: 0x14000116 RID: 278
		// (add) Token: 0x060057FA RID: 22522 RVA: 0x001E4E2C File Offset: 0x001E302C
		// (remove) Token: 0x060057FB RID: 22523 RVA: 0x001E4E64 File Offset: 0x001E3064
		public event Action OnStageEnter;

		// Token: 0x14000117 RID: 279
		// (add) Token: 0x060057FC RID: 22524 RVA: 0x001E4E9C File Offset: 0x001E309C
		// (remove) Token: 0x060057FD RID: 22525 RVA: 0x001E4ED4 File Offset: 0x001E30D4
		public event Action OnStageExit;

		// Token: 0x14000118 RID: 280
		// (add) Token: 0x060057FE RID: 22526 RVA: 0x001E4F0C File Offset: 0x001E310C
		// (remove) Token: 0x060057FF RID: 22527 RVA: 0x001E4F44 File Offset: 0x001E3144
		public event Action OnStageError;

		// Token: 0x14000119 RID: 281
		// (add) Token: 0x06005800 RID: 22528 RVA: 0x001E4F7C File Offset: 0x001E317C
		// (remove) Token: 0x06005801 RID: 22529 RVA: 0x001E4FB4 File Offset: 0x001E31B4
		public event Action OnStageRefresh;

		// Token: 0x06005802 RID: 22530 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void StatusTextUpdated(string txt)
		{
		}

		// Token: 0x06005803 RID: 22531 RVA: 0x0007AAEB File Offset: 0x00078CEB
		protected virtual void Awake()
		{
			this.m_window.gameObject.SetActive(true);
		}

		// Token: 0x06005804 RID: 22532 RVA: 0x001E4FEC File Offset: 0x001E31EC
		protected virtual void Update()
		{
			if (!this.m_window || !this.m_controller || !this.m_controller.Brain || this.m_controller.Stage != this.m_type || this.m_controller.LastFrameStageChanged >= Time.frameCount)
			{
				return;
			}
			if (!this.m_window.Visible && !this.m_controller.Brain.IsBlending)
			{
				this.TransitionComplete();
			}
		}

		// Token: 0x06005805 RID: 22533 RVA: 0x0007AAFE File Offset: 0x00078CFE
		public void AssignController(LoginController controller, LoginStageType type)
		{
			this.m_controller = controller;
			this.m_type = type;
		}

		// Token: 0x06005806 RID: 22534 RVA: 0x0007AB0E File Offset: 0x00078D0E
		public virtual void StageEnter()
		{
			Action onStageEnter = this.OnStageEnter;
			if (onStageEnter == null)
			{
				return;
			}
			onStageEnter();
		}

		// Token: 0x06005807 RID: 22535 RVA: 0x0007AB20 File Offset: 0x00078D20
		public virtual void StageExit()
		{
			if (this.m_window.Visible)
			{
				this.m_window.Hide(false);
			}
			Action onStageExit = this.OnStageExit;
			if (onStageExit == null)
			{
				return;
			}
			onStageExit();
		}

		// Token: 0x06005808 RID: 22536 RVA: 0x0007AB4B File Offset: 0x00078D4B
		protected virtual void TransitionComplete()
		{
			this.m_window.Show(false);
		}

		// Token: 0x06005809 RID: 22537 RVA: 0x0007AB59 File Offset: 0x00078D59
		public virtual void StageError(string err)
		{
			Action onStageError = this.OnStageError;
			if (onStageError != null)
			{
				onStageError();
			}
			this.StatusUpdate(err);
		}

		// Token: 0x0600580A RID: 22538 RVA: 0x0007AB73 File Offset: 0x00078D73
		public virtual void StageErrorCritical(string err)
		{
			this.StageError(err);
		}

		// Token: 0x0600580B RID: 22539 RVA: 0x0007AB7C File Offset: 0x00078D7C
		public void StatusUpdate(string txt)
		{
			if (this.m_status)
			{
				this.m_status.text = txt;
				this.StatusTextUpdated(txt);
			}
		}

		// Token: 0x0600580C RID: 22540 RVA: 0x0007AB9E File Offset: 0x00078D9E
		public virtual void StageRefresh()
		{
			Action onStageRefresh = this.OnStageRefresh;
			if (onStageRefresh == null)
			{
				return;
			}
			onStageRefresh();
		}

		// Token: 0x0600580D RID: 22541 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void EnterPressed()
		{
		}

		// Token: 0x04004D95 RID: 19861
		protected LoginController m_controller;

		// Token: 0x04004D96 RID: 19862
		[SerializeField]
		private UIWindow m_window;

		// Token: 0x04004D97 RID: 19863
		[SerializeField]
		protected TextMeshProUGUI m_status;

		// Token: 0x04004D98 RID: 19864
		private LoginStageType m_type;
	}
}

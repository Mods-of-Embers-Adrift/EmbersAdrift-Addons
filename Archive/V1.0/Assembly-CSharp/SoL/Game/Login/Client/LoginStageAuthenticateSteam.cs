using System;
using SoL.Managers;
using SoL.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SoL.Game.Login.Client
{
	// Token: 0x02000B35 RID: 2869
	public class LoginStageAuthenticateSteam : LoginStageAuthenticateBase
	{
		// Token: 0x06005829 RID: 22569 RVA: 0x001E54AC File Offset: 0x001E36AC
		protected override void Awake()
		{
			base.Awake();
			this.m_defaultFrameColor = this.m_frame.color;
			this.m_retryButton.onClick.AddListener(new UnityAction(this.RetryClicked));
			this.m_retryButton.gameObject.SetActive(false);
		}

		// Token: 0x0600582A RID: 22570 RVA: 0x0007ACFD File Offset: 0x00078EFD
		private void OnDestroy()
		{
			this.m_retryButton.onClick.RemoveListener(new UnityAction(this.RetryClicked));
		}

		// Token: 0x0600582B RID: 22571 RVA: 0x0007AD1B File Offset: 0x00078F1B
		private void RetryClicked()
		{
			this.StageEnterInternalEnd();
		}

		// Token: 0x0600582C RID: 22572 RVA: 0x0007AD23 File Offset: 0x00078F23
		protected override void StageEnterInternalEnd()
		{
			this.m_controller.SetStatusText("Logging In via Steam...");
			LoginApiManager.LoginWithSteam();
			this.SetupLoadingVisuals(true, this.m_defaultFrameColor, false);
		}

		// Token: 0x0600582D RID: 22573 RVA: 0x001E5500 File Offset: 0x001E3700
		public override void StageExit()
		{
			Color frameColor = (SessionData.User != null) ? Color.green : this.m_defaultFrameColor;
			this.SetupLoadingVisuals(false, frameColor, false);
			base.StageExit();
		}

		// Token: 0x0600582E RID: 22574 RVA: 0x0007AC92 File Offset: 0x00078E92
		protected override void TransitionComplete()
		{
			base.TransitionComplete();
			this.m_controller.ToggleBackButton(true);
		}

		// Token: 0x0600582F RID: 22575 RVA: 0x0007AD48 File Offset: 0x00078F48
		public override void StageError(string err)
		{
			base.StageError(err);
			this.SetupLoadingVisuals(false, UIManager.RedColor, true);
		}

		// Token: 0x06005830 RID: 22576 RVA: 0x0007AD5E File Offset: 0x00078F5E
		public override void StageErrorCritical(string err)
		{
			base.StageErrorCritical(err);
			this.SetupLoadingVisuals(false, UIManager.RedColor, false);
		}

		// Token: 0x06005831 RID: 22577 RVA: 0x0007AD74 File Offset: 0x00078F74
		private void SetupLoadingVisuals(bool isAnimating, Color frameColor, bool isRetry)
		{
			this.m_frame.color = frameColor;
			this.m_animatedLoading.SetActive(isAnimating);
			this.m_retryButton.gameObject.SetActive(isRetry);
		}

		// Token: 0x04004DA5 RID: 19877
		[SerializeField]
		private GameObject m_animatedLoading;

		// Token: 0x04004DA6 RID: 19878
		[SerializeField]
		private SolButton m_retryButton;

		// Token: 0x04004DA7 RID: 19879
		[SerializeField]
		private Image m_frame;

		// Token: 0x04004DA8 RID: 19880
		private Color m_defaultFrameColor;
	}
}

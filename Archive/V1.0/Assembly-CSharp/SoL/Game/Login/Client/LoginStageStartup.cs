using System;
using SoL.Managers;
using SoL.UI;
using UnityEngine;
using UnityEngine.Events;

namespace SoL.Game.Login.Client
{
	// Token: 0x02000B38 RID: 2872
	public class LoginStageStartup : LoginStage
	{
		// Token: 0x06005847 RID: 22599 RVA: 0x001E59DC File Offset: 0x001E3BDC
		protected override void Awake()
		{
			base.Awake();
			this.m_toLogin.onClick.AddListener(new UnityAction(this.OnLoginClicked));
			this.m_toExit.onClick.AddListener(new UnityAction(this.OnExitClicked));
			this.m_toSteam.onClick.AddListener(new UnityAction(this.OnSteamLoginClicked));
		}

		// Token: 0x06005848 RID: 22600 RVA: 0x0007AE68 File Offset: 0x00079068
		private void OnDestroy()
		{
			this.m_toLogin.onClick.RemoveAllListeners();
			this.m_toExit.onClick.RemoveAllListeners();
			this.m_toSteam.onClick.RemoveAllListeners();
		}

		// Token: 0x06005849 RID: 22601 RVA: 0x001E5A44 File Offset: 0x001E3C44
		public override void StageEnter()
		{
			base.StageEnter();
			this.ToggleButtons(true);
			bool steamIsAvailable = SteamManager.SteamIsAvailable;
			this.m_toLogin.text = "Legacy Login";
			this.SetSelectedGameObject();
			this.m_controller.ToggleBackButton(false);
			this.m_controller.SetStatusText("");
		}

		// Token: 0x0600584A RID: 22602 RVA: 0x0007AE9A File Offset: 0x0007909A
		public override void EnterPressed()
		{
			base.EnterPressed();
			this.GetTargetButton().onClick.Invoke();
		}

		// Token: 0x0600584B RID: 22603 RVA: 0x0007AEB2 File Offset: 0x000790B2
		private SolButton GetTargetButton()
		{
			if (!SteamManager.SteamIsAvailable || !this.m_toSteam.interactable)
			{
				return this.m_toLogin;
			}
			return this.m_toSteam;
		}

		// Token: 0x0600584C RID: 22604 RVA: 0x0007AED5 File Offset: 0x000790D5
		private void SetSelectedGameObject()
		{
			if (UIManager.EventSystem)
			{
				UIManager.EventSystem.SetSelectedGameObject(this.GetTargetButton().gameObject);
			}
		}

		// Token: 0x0600584D RID: 22605 RVA: 0x0007AEF8 File Offset: 0x000790F8
		private void OnSteamLoginClicked()
		{
			this.ToggleButtons(false);
			this.m_controller.IsSteam = true;
			this.m_controller.SetStage(LoginStageType.AuthenticateSteam);
		}

		// Token: 0x0600584E RID: 22606 RVA: 0x0007AF19 File Offset: 0x00079119
		private void OnLoginClicked()
		{
			this.ToggleButtons(false);
			this.m_controller.IsSteam = false;
			this.m_controller.SetStage(LoginStageType.Authenticate);
		}

		// Token: 0x0600584F RID: 22607 RVA: 0x0007AF3A File Offset: 0x0007913A
		private void ToggleButtons(bool isEnabled)
		{
			this.m_toSteam.interactable = (isEnabled && SteamManager.SteamIsAvailable);
			this.m_toLogin.interactable = isEnabled;
			this.m_toExit.interactable = isEnabled;
		}

		// Token: 0x06005850 RID: 22608 RVA: 0x0007AF6A File Offset: 0x0007916A
		public override void StageError(string err)
		{
			base.StageError(err);
			this.ToggleButtons(true);
			this.SetSelectedGameObject();
		}

		// Token: 0x06005851 RID: 22609 RVA: 0x0007AF80 File Offset: 0x00079180
		public override void StageErrorCritical(string err)
		{
			base.StageErrorCritical(err);
			this.ToggleButtons(false);
		}

		// Token: 0x06005852 RID: 22610 RVA: 0x00067F11 File Offset: 0x00066111
		private void OnExitClicked()
		{
			Application.Quit();
		}

		// Token: 0x04004DB4 RID: 19892
		[SerializeField]
		private SolButton m_toLogin;

		// Token: 0x04004DB5 RID: 19893
		[SerializeField]
		private SolButton m_toExit;

		// Token: 0x04004DB6 RID: 19894
		[SerializeField]
		private SolButton m_toSteam;
	}
}

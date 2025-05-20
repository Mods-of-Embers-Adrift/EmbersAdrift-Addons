using System;
using SoL.Managers;
using SoL.Networking.SolServer;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SoL.Game.Login.Client
{
	// Token: 0x02000B34 RID: 2868
	public class LoginStageAuthenticateLegacy : LoginStageAuthenticateBase
	{
		// Token: 0x0600581C RID: 22556 RVA: 0x001E51C8 File Offset: 0x001E33C8
		protected override void Awake()
		{
			base.Awake();
			this.m_login.onClick.AddListener(new UnityAction(this.OnLoginClicked));
			this.m_saveUsername.isOn = Options.GameOptions.SaveUsername;
			this.m_saveUsername.onValueChanged.AddListener(new UnityAction<bool>(this.OnSaveUsernameChanged));
			this.m_runningStatus.text = null;
		}

		// Token: 0x0600581D RID: 22557 RVA: 0x0007ABFE File Offset: 0x00078DFE
		private void OnDestroy()
		{
			this.m_login.onClick.RemoveListener(new UnityAction(this.OnLoginClicked));
			this.m_saveUsername.onValueChanged.RemoveListener(new UnityAction<bool>(this.OnSaveUsernameChanged));
		}

		// Token: 0x0600581E RID: 22558 RVA: 0x001E5234 File Offset: 0x001E3434
		protected override void Update()
		{
			base.Update();
			if (this.m_loggingIn)
			{
				this.m_login.interactable = false;
				this.m_userName.interactable = false;
				this.m_password.interactable = false;
				return;
			}
			this.m_userName.interactable = true;
			this.m_password.interactable = true;
			this.m_atLogin = (SolServerConnectionManager.CurrentConnection == null);
			if (this.m_atLogin)
			{
				this.m_login.interactable = (!string.IsNullOrEmpty(this.m_userName.text) && !string.IsNullOrEmpty(this.m_password.text));
				this.m_runningStatus.text = "Connected to Login Server";
			}
		}

		// Token: 0x0600581F RID: 22559 RVA: 0x0007AC38 File Offset: 0x00078E38
		protected override void StatusTextUpdated(string txt)
		{
			base.StatusTextUpdated(txt);
			this.m_statusBackground.enabled = !string.IsNullOrEmpty(txt);
		}

		// Token: 0x06005820 RID: 22560 RVA: 0x001E52E8 File Offset: 0x001E34E8
		protected override void StageEnterInternal()
		{
			this.m_userName.text = (Options.GameOptions.SaveUsername ? Options.GameOptions.Username : "");
			this.m_password.text = "";
			this.m_userName.interactable = true;
			this.m_password.interactable = true;
			if (UIManager.EventSystem != null)
			{
				TMP_InputField tmp_InputField = this.m_userName;
				if (Options.GameOptions.SaveUsername && !string.IsNullOrEmpty(Options.GameOptions.Username))
				{
					tmp_InputField = this.m_password;
				}
				UIManager.EventSystem.SetSelectedGameObject(tmp_InputField.gameObject);
			}
		}

		// Token: 0x06005821 RID: 22561 RVA: 0x001E5390 File Offset: 0x001E3590
		public override void StageError(string err)
		{
			base.StageError(err);
			this.m_userName.interactable = true;
			this.m_password.interactable = true;
			if (UIManager.EventSystem != null)
			{
				UIManager.EventSystem.SetSelectedGameObject(this.m_password.gameObject);
			}
		}

		// Token: 0x06005822 RID: 22562 RVA: 0x001E53E0 File Offset: 0x001E35E0
		public override void StageErrorCritical(string err)
		{
			base.StageErrorCritical(err);
			this.m_userName.interactable = false;
			this.m_password.interactable = false;
			this.m_login.interactable = false;
			this.m_saveUsername.interactable = false;
			this.m_userName.gameObject.SetActive(false);
			this.m_password.gameObject.SetActive(false);
			this.m_login.gameObject.SetActive(false);
			this.m_saveUsername.gameObject.SetActive(false);
		}

		// Token: 0x06005823 RID: 22563 RVA: 0x0007AC55 File Offset: 0x00078E55
		public override void EnterPressed()
		{
			base.EnterPressed();
			if (string.IsNullOrEmpty(this.m_userName.text) || string.IsNullOrEmpty(this.m_password.text))
			{
				return;
			}
			this.m_login.onClick.Invoke();
		}

		// Token: 0x06005824 RID: 22564 RVA: 0x0007AC92 File Offset: 0x00078E92
		protected override void TransitionComplete()
		{
			base.TransitionComplete();
			this.m_controller.ToggleBackButton(true);
		}

		// Token: 0x06005825 RID: 22565 RVA: 0x0007ACA6 File Offset: 0x00078EA6
		private void OnSaveUsernameChanged(bool value)
		{
			Options.GameOptions.SaveUsername.Value = value;
			if (!value)
			{
				Options.GameOptions.Username.Value = "";
			}
		}

		// Token: 0x06005826 RID: 22566 RVA: 0x001E5468 File Offset: 0x001E3668
		private void OnLoginClicked()
		{
			this.m_loggingIn = true;
			this.m_login.interactable = false;
			this.m_userName.interactable = false;
			this.m_password.interactable = false;
			this.m_controller.SetStatusText("Logging In...");
			if (!SolServerEncryption.CanBeEncoded(this.m_password.text))
			{
				this.StageError("Password is too long :(");
				this.m_password.text = string.Empty;
				return;
			}
			this.m_controller.IsLoggedInViaSteam = false;
			LoginApiManager.LoginWithCredentials(this.m_userName.text, this.m_password.text);
		}

		// Token: 0x06005827 RID: 22567 RVA: 0x0007ACC5 File Offset: 0x00078EC5
		public void SaveUsername()
		{
			if (Options.GameOptions.SaveUsername && !this.m_controller.IsLoggedInViaSteam)
			{
				Options.GameOptions.Username.Value = this.m_userName.text;
			}
		}

		// Token: 0x04004D9F RID: 19871
		[SerializeField]
		private TMP_InputField m_userName;

		// Token: 0x04004DA0 RID: 19872
		[SerializeField]
		private TMP_InputField m_password;

		// Token: 0x04004DA1 RID: 19873
		[SerializeField]
		private Button m_login;

		// Token: 0x04004DA2 RID: 19874
		[SerializeField]
		private Toggle m_saveUsername;

		// Token: 0x04004DA3 RID: 19875
		[SerializeField]
		private TextMeshProUGUI m_runningStatus;

		// Token: 0x04004DA4 RID: 19876
		[SerializeField]
		private Image m_statusBackground;
	}
}

using System;
using Cysharp.Text;
using SoL.Game.Login.Client;
using SoL.Game.Player;
using SoL.Managers;
using SoL.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace SoL.Game.UI
{
	// Token: 0x02000892 RID: 2194
	public class InGameUIMenu : UIWindow
	{
		// Token: 0x06003FE3 RID: 16355 RVA: 0x00189F20 File Offset: 0x00188120
		protected override void Awake()
		{
			base.Awake();
			this.m_returnButton.onClick.AddListener(new UnityAction(this.ReturnClicked));
			this.m_feedbackButton.onClick.AddListener(new UnityAction(this.FeedbackClicked));
			this.m_optionsButton.onClick.AddListener(new UnityAction(this.OptionsClicked));
			this.m_selectionButton.onClick.AddListener(new UnityAction(this.SelectionClicked));
			this.m_signOutButton.onClick.AddListener(new UnityAction(this.SignOutClicked));
			this.m_exitGameButton.onClick.AddListener(new UnityAction(this.ExitGameClicked));
			if (this.m_campBlockerParent)
			{
				this.m_campBlockerParent.gameObject.SetActive(false);
			}
			if (this.m_campCancelButton)
			{
				this.m_campCancelButton.onClick.AddListener(new UnityAction(this.CampCancelClicked));
			}
			SessionData.UserRecordUpdated += this.RefreshFeedbackButton;
		}

		// Token: 0x06003FE4 RID: 16356 RVA: 0x0018A034 File Offset: 0x00188234
		protected override void OnDestroy()
		{
			base.OnDestroy();
			this.m_returnButton.onClick.RemoveListener(new UnityAction(this.ReturnClicked));
			this.m_feedbackButton.onClick.RemoveListener(new UnityAction(this.FeedbackClicked));
			this.m_optionsButton.onClick.RemoveListener(new UnityAction(this.OptionsClicked));
			this.m_selectionButton.onClick.RemoveListener(new UnityAction(this.SelectionClicked));
			this.m_signOutButton.onClick.RemoveListener(new UnityAction(this.SignOutClicked));
			this.m_exitGameButton.onClick.RemoveListener(new UnityAction(this.ExitGameClicked));
			if (this.m_campCancelButton)
			{
				this.m_campCancelButton.onClick.RemoveListener(new UnityAction(this.CampCancelClicked));
			}
			SessionData.UserRecordUpdated -= this.RefreshFeedbackButton;
		}

		// Token: 0x06003FE5 RID: 16357 RVA: 0x0018A12C File Offset: 0x0018832C
		private void Update()
		{
			if (base.Visible)
			{
				this.m_selectionButton.interactable = (!string.IsNullOrEmpty(SessionData.SessionKey) && LoginController.Instance == null);
				if (ClientGameManager.CampManager)
				{
					bool camping = CampManager.Camping;
					if (this.m_campBlockerParent.activeSelf != camping)
					{
						this.m_campBlockerParent.SetActive(camping);
					}
					if (camping && this.m_campLabel)
					{
						this.m_campLabel.SetTextFormat("Preparing Camp\n{0}s", CampManager.TimeRemaining);
					}
				}
			}
		}

		// Token: 0x06003FE6 RID: 16358 RVA: 0x0006B2BE File Offset: 0x000694BE
		public override void Show(bool skipTransition = false)
		{
			IInputManager inputManager = ClientGameManager.InputManager;
			if (inputManager != null)
			{
				inputManager.SetInputPreventionFlag(InputPreventionFlags.Options);
			}
			this.m_menuShown = true;
			this.m_backgroundBlocker.Show(skipTransition);
			this.RefreshFeedbackButton();
			base.Show(skipTransition);
		}

		// Token: 0x06003FE7 RID: 16359 RVA: 0x0006B2F2 File Offset: 0x000694F2
		public override void Hide(bool skipTransition = false)
		{
			IInputManager inputManager = ClientGameManager.InputManager;
			if (inputManager != null)
			{
				inputManager.UnsetInputPreventionFlag(InputPreventionFlags.Options);
			}
			this.m_menuShown = false;
			this.m_backgroundBlocker.Hide(skipTransition);
			base.Hide(skipTransition);
		}

		// Token: 0x06003FE8 RID: 16360 RVA: 0x0006B320 File Offset: 0x00069520
		private void ExitGameClicked()
		{
			if (ClientGameManager.CampManager)
			{
				ClientGameManager.CampManager.Quit();
			}
			this.Hide(false);
		}

		// Token: 0x06003FE9 RID: 16361 RVA: 0x0006B33F File Offset: 0x0006953F
		private void SignOutClicked()
		{
			if (ClientGameManager.CampManager)
			{
				ClientGameManager.CampManager.InitiateCamp(false);
			}
			this.Hide(false);
		}

		// Token: 0x06003FEA RID: 16362 RVA: 0x0006B35F File Offset: 0x0006955F
		private void SelectionClicked()
		{
			if (ClientGameManager.CampManager)
			{
				ClientGameManager.CampManager.InitiateCamp(true);
			}
			this.Hide(false);
		}

		// Token: 0x06003FEB RID: 16363 RVA: 0x0006B37F File Offset: 0x0006957F
		private void OptionsClicked()
		{
			ClientGameManager.UIManager.InGameUiOptions.Show(false);
		}

		// Token: 0x06003FEC RID: 16364 RVA: 0x0018A1B8 File Offset: 0x001883B8
		private void FeedbackClicked()
		{
			switch (this.m_feedbackButtonMode)
			{
			case InGameUIMenu.FeedbackButtonMode.Feedback:
				Application.OpenURL(this.m_feedbackURL);
				return;
			case InGameUIMenu.FeedbackButtonMode.ActivateSubscription:
			case InGameUIMenu.FeedbackButtonMode.PurchaseGame:
				Application.OpenURL("https://www.embersadrift.com/Account");
				return;
			case InGameUIMenu.FeedbackButtonMode.Donate:
				Application.OpenURL("https://www.embersadrift.com/Donate");
				return;
			default:
				return;
			}
		}

		// Token: 0x06003FED RID: 16365 RVA: 0x0006B391 File Offset: 0x00069591
		private void ReturnClicked()
		{
			this.CloseButtonPressed();
		}

		// Token: 0x06003FEE RID: 16366 RVA: 0x0006B399 File Offset: 0x00069599
		public bool EscapePressedFirstPass()
		{
			if (!this.m_menuShown)
			{
				return false;
			}
			if (this.m_menuShown)
			{
				this.ShowHideClicked();
				return true;
			}
			return false;
		}

		// Token: 0x06003FEF RID: 16367 RVA: 0x0006B3B6 File Offset: 0x000695B6
		public bool EscapePressedLastPass()
		{
			if (Options.GameOptions.EscapeTogglesMenu.Value)
			{
				this.ShowHideClicked();
				return true;
			}
			return false;
		}

		// Token: 0x06003FF0 RID: 16368 RVA: 0x0006B3CD File Offset: 0x000695CD
		public void ShowHideClicked()
		{
			this.m_menuShown = !this.m_menuShown;
			if (this.m_menuShown)
			{
				this.Show(false);
				return;
			}
			this.Hide(false);
		}

		// Token: 0x06003FF1 RID: 16369 RVA: 0x0006B3F5 File Offset: 0x000695F5
		private void CampCancelClicked()
		{
			if (ClientGameManager.CampManager)
			{
				ClientGameManager.CampManager.CancelCamp();
			}
		}

		// Token: 0x06003FF2 RID: 16370 RVA: 0x0018A204 File Offset: 0x00188404
		private void RefreshFeedbackButton()
		{
			if (this.m_feedbackButton)
			{
				if (SessionData.User == null || SessionData.User.IsSubscriber())
				{
					this.m_feedbackButton.text = "Feedback";
					this.m_feedbackButtonMode = InGameUIMenu.FeedbackButtonMode.Feedback;
					return;
				}
				if (SessionData.User.IsTrial())
				{
					this.m_feedbackButton.text = "Purchase Game";
					this.m_feedbackButtonMode = InGameUIMenu.FeedbackButtonMode.PurchaseGame;
					return;
				}
				this.m_feedbackButton.text = "Activate Subscription";
				this.m_feedbackButtonMode = InGameUIMenu.FeedbackButtonMode.ActivateSubscription;
			}
		}

		// Token: 0x04003D74 RID: 15732
		[SerializeField]
		private SolButton m_returnButton;

		// Token: 0x04003D75 RID: 15733
		[SerializeField]
		private SolButton m_feedbackButton;

		// Token: 0x04003D76 RID: 15734
		[SerializeField]
		private SolButton m_optionsButton;

		// Token: 0x04003D77 RID: 15735
		[SerializeField]
		private SolButton m_selectionButton;

		// Token: 0x04003D78 RID: 15736
		[SerializeField]
		private SolButton m_signOutButton;

		// Token: 0x04003D79 RID: 15737
		[SerializeField]
		private SolButton m_exitGameButton;

		// Token: 0x04003D7A RID: 15738
		[SerializeField]
		private string m_feedbackURL;

		// Token: 0x04003D7B RID: 15739
		[SerializeField]
		private GameObject m_campBlockerParent;

		// Token: 0x04003D7C RID: 15740
		[SerializeField]
		private TextMeshProUGUI m_campLabel;

		// Token: 0x04003D7D RID: 15741
		[SerializeField]
		private SolButton m_campCancelButton;

		// Token: 0x04003D7E RID: 15742
		[SerializeField]
		private UIWindow m_backgroundBlocker;

		// Token: 0x04003D7F RID: 15743
		private bool m_menuShown;

		// Token: 0x04003D80 RID: 15744
		private InGameUIMenu.FeedbackButtonMode m_feedbackButtonMode;

		// Token: 0x02000893 RID: 2195
		private enum FeedbackButtonMode
		{
			// Token: 0x04003D82 RID: 15746
			Feedback,
			// Token: 0x04003D83 RID: 15747
			ActivateSubscription,
			// Token: 0x04003D84 RID: 15748
			PurchaseGame,
			// Token: 0x04003D85 RID: 15749
			Donate
		}
	}
}

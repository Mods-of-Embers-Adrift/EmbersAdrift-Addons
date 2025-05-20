using System;
using SoL.Managers;
using SoL.UI;
using UnityEngine;
using UnityEngine.Events;

namespace SoL.Subscription
{
	// Token: 0x020003A8 RID: 936
	public class ActivateSubscriptionButton : MonoBehaviour
	{
		// Token: 0x17000613 RID: 1555
		// (get) Token: 0x06001991 RID: 6545 RVA: 0x000540AC File Offset: 0x000522AC
		// (set) Token: 0x06001992 RID: 6546 RVA: 0x00106CA8 File Offset: 0x00104EA8
		private ElementMode Mode
		{
			get
			{
				return this.m_mode;
			}
			set
			{
				this.m_mode = value;
				string text = string.Empty;
				bool flag = false;
				switch (this.m_mode)
				{
				case ElementMode.Donate:
					text = "Donate";
					flag = true;
					break;
				case ElementMode.Purchase:
					text = "Purchase Game";
					flag = true;
					break;
				case ElementMode.ActivateSub:
					text = "Activate Subscription";
					flag = true;
					break;
				}
				this.m_button.SetText(text);
				this.m_button.interactable = flag;
				this.m_button.gameObject.SetActive(flag);
			}
		}

		// Token: 0x06001993 RID: 6547 RVA: 0x000540B4 File Offset: 0x000522B4
		private void OnEnable()
		{
			if (this.m_toggleOnEnable)
			{
				this.RefreshMode();
			}
		}

		// Token: 0x06001994 RID: 6548 RVA: 0x00106D28 File Offset: 0x00104F28
		private void Start()
		{
			if (this.m_button)
			{
				this.m_button.onClick.AddListener(new UnityAction(this.OnButtonClicked));
			}
			SessionData.UserRecordUpdated += this.RefreshMode;
			this.RefreshMode();
		}

		// Token: 0x06001995 RID: 6549 RVA: 0x000540C4 File Offset: 0x000522C4
		private void OnDestroy()
		{
			if (this.m_button)
			{
				this.m_button.onClick.RemoveListener(new UnityAction(this.OnButtonClicked));
			}
			SessionData.UserRecordUpdated -= this.RefreshMode;
		}

		// Token: 0x06001996 RID: 6550 RVA: 0x00054100 File Offset: 0x00052300
		private void OnButtonClicked()
		{
			SubscriptionExtensions.ExecuteActivateSubscription(this.Mode);
		}

		// Token: 0x06001997 RID: 6551 RVA: 0x00106D78 File Offset: 0x00104F78
		public void RefreshMode()
		{
			ElementMode mode = ElementMode.Hidden;
			if (SessionData.User != null)
			{
				if (SessionData.User.IsTrial())
				{
					mode = ElementMode.Purchase;
				}
				else if (!SessionData.User.IsSubscriber())
				{
					mode = ElementMode.ActivateSub;
				}
			}
			this.Mode = mode;
		}

		// Token: 0x04002080 RID: 8320
		public const bool kAlwaysDonateMode = false;

		// Token: 0x04002081 RID: 8321
		public const string kDonateText = "Donate";

		// Token: 0x04002082 RID: 8322
		[SerializeField]
		private bool m_toggleOnEnable;

		// Token: 0x04002083 RID: 8323
		[SerializeField]
		private SolButton m_button;

		// Token: 0x04002084 RID: 8324
		private ElementMode m_mode;
	}
}

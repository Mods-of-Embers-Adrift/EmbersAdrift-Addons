using System;
using Cysharp.Text;
using SoL.Managers;
using SoL.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SoL.Game.UI.Notifications
{
	// Token: 0x02000963 RID: 2403
	public class NotificationsWindow : DraggableUIWindow
	{
		// Token: 0x06004738 RID: 18232 RVA: 0x001A6470 File Offset: 0x001A4670
		protected override void Start()
		{
			base.Start();
			ClientGameManager.NotificationsManager.ArchiveChanged += this.OnArchiveChanged;
			ClientGameManager.SocialManager.UnreadMailUpdated += this.OnUnreadMailUpdated;
			this.m_dismissAllButton.onClick.AddListener(new UnityAction(this.DismissAllClicked));
			this.OnUnreadMailUpdated();
		}

		// Token: 0x06004739 RID: 18233 RVA: 0x001A64D4 File Offset: 0x001A46D4
		protected override void OnDestroy()
		{
			base.OnDestroy();
			ClientGameManager.NotificationsManager.ArchiveChanged -= this.OnArchiveChanged;
			ClientGameManager.SocialManager.UnreadMailUpdated -= this.OnUnreadMailUpdated;
			this.m_dismissAllButton.onClick.RemoveListener(new UnityAction(this.DismissAllClicked));
		}

		// Token: 0x0600473A RID: 18234 RVA: 0x00070010 File Offset: 0x0006E210
		public override void Show(bool skipTransition = false)
		{
			base.Show(skipTransition);
			this.m_list.UpdateItems(ClientGameManager.NotificationsManager.Archive);
			this.OnUnreadMailUpdated();
		}

		// Token: 0x0600473B RID: 18235 RVA: 0x00070034 File Offset: 0x0006E234
		private void OnArchiveChanged()
		{
			this.m_list.UpdateItems(ClientGameManager.NotificationsManager.Archive);
		}

		// Token: 0x0600473C RID: 18236 RVA: 0x001A6530 File Offset: 0x001A4730
		private void OnUnreadMailUpdated()
		{
			if (this.m_unreadMailPanel)
			{
				int num = ClientGameManager.SocialManager ? ClientGameManager.SocialManager.GetUnreadMailCount() : 0;
				bool flag = num > 0;
				this.m_unreadMailHighlight.enabled = (flag && !ClientGameManager.SocialManager.UnreadMailNotificationDismissed);
				if (flag && this.m_unreadMailLabel)
				{
					this.m_unreadMailLabel.SetTextFormat("You have {0} unread mail!", num);
				}
				this.m_unreadMailPanel.SetActive(flag);
			}
		}

		// Token: 0x0600473D RID: 18237 RVA: 0x0007004B File Offset: 0x0006E24B
		private void DismissAllClicked()
		{
			if (ClientGameManager.SocialManager)
			{
				ClientGameManager.SocialManager.UnreadMailNotificationDismissed = true;
				this.OnUnreadMailUpdated();
			}
			if (ClientGameManager.NotificationsManager)
			{
				ClientGameManager.NotificationsManager.ClearArchived();
			}
		}

		// Token: 0x04004322 RID: 17186
		[SerializeField]
		private NotificationsList m_list;

		// Token: 0x04004323 RID: 17187
		[SerializeField]
		private SolButton m_dismissAllButton;

		// Token: 0x04004324 RID: 17188
		[SerializeField]
		private Image m_unreadMailHighlight;

		// Token: 0x04004325 RID: 17189
		[SerializeField]
		private GameObject m_unreadMailPanel;

		// Token: 0x04004326 RID: 17190
		[SerializeField]
		private TextMeshProUGUI m_unreadMailLabel;
	}
}

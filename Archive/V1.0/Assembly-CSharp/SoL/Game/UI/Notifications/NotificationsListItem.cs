using System;
using SoL.Game.Interactives;
using SoL.Game.Notifications;
using SoL.Managers;
using SoL.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SoL.Game.UI.Notifications
{
	// Token: 0x02000960 RID: 2400
	public class NotificationsListItem : MonoBehaviour, ITooltip, IInteractiveBase
	{
		// Token: 0x06004723 RID: 18211 RVA: 0x0006FED8 File Offset: 0x0006E0D8
		private void Start()
		{
			this.m_openButton.onClick.AddListener(new UnityAction(this.OnOpen));
			this.m_dismissButton.onClick.AddListener(new UnityAction(this.OnDismiss));
		}

		// Token: 0x06004724 RID: 18212 RVA: 0x0006FF12 File Offset: 0x0006E112
		private void OnDestroy()
		{
			this.m_openButton.onClick.RemoveListener(new UnityAction(this.OnOpen));
			this.m_dismissButton.onClick.RemoveListener(new UnityAction(this.OnDismiss));
		}

		// Token: 0x06004725 RID: 18213 RVA: 0x001A5EA4 File Offset: 0x001A40A4
		public void Init(NotificationsList parent, int index, Notification notification)
		{
			this.m_parent = parent;
			this.m_index = index;
			this.m_notification = notification;
			this.m_icon.sprite = notification.Type.Icon;
			this.m_icon.color = notification.Type.IconTint;
			this.m_label.text = notification.TitleWithData;
			this.m_openButton.gameObject.SetActive(notification.Type.CanOpen);
		}

		// Token: 0x06004726 RID: 18214 RVA: 0x0006FF4C File Offset: 0x0006E14C
		private void OnOpen()
		{
			this.m_notification.Type.Open(this.m_notification.OpenData);
		}

		// Token: 0x06004727 RID: 18215 RVA: 0x0006FF69 File Offset: 0x0006E169
		private void OnDismiss()
		{
			NotificationsManager notificationsManager = ClientGameManager.NotificationsManager;
			if (notificationsManager == null)
			{
				return;
			}
			notificationsManager.DismissNotification(this.m_notification);
		}

		// Token: 0x06004728 RID: 18216 RVA: 0x001A5F20 File Offset: 0x001A4120
		private ITooltipParameter GetTooltipParameter()
		{
			if (!string.IsNullOrEmpty(this.m_notification.Type.Description))
			{
				return new ObjectTextTooltipParameter
				{
					Text = this.m_notification.DescriptionWithData
				};
			}
			return null;
		}

		// Token: 0x17000FD1 RID: 4049
		// (get) Token: 0x06004729 RID: 18217 RVA: 0x0006FF80 File Offset: 0x0006E180
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x17000FD2 RID: 4050
		// (get) Token: 0x0600472A RID: 18218 RVA: 0x0006FF8E File Offset: 0x0006E18E
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x17000FD3 RID: 4051
		// (get) Token: 0x0600472B RID: 18219 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x0600472D RID: 18221 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x0400430C RID: 17164
		[SerializeField]
		private Image m_icon;

		// Token: 0x0400430D RID: 17165
		[SerializeField]
		private TextMeshProUGUI m_label;

		// Token: 0x0400430E RID: 17166
		[SerializeField]
		private SolButton m_openButton;

		// Token: 0x0400430F RID: 17167
		[SerializeField]
		private SolButton m_dismissButton;

		// Token: 0x04004310 RID: 17168
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x04004311 RID: 17169
		private Notification m_notification;

		// Token: 0x04004312 RID: 17170
		private NotificationsList m_parent;

		// Token: 0x04004313 RID: 17171
		private int m_index = -1;
	}
}

using System;
using SoL.Managers;
using UnityEngine;

namespace SoL.Subscription
{
	// Token: 0x020003B4 RID: 948
	public class SubscriptionUI : MonoBehaviour
	{
		// Token: 0x060019BF RID: 6591 RVA: 0x0010762C File Offset: 0x0010582C
		private void Start()
		{
			this.m_thisRect = (RectTransform)base.gameObject.transform;
			this.m_infoPanelRect = (RectTransform)this.m_infoPanel.transform;
			this.m_activatePanelRect = (RectTransform)this.m_activatePanel.transform;
			SessionData.UserRecordUpdated += this.RefreshState;
			this.RefreshState();
		}

		// Token: 0x060019C0 RID: 6592 RVA: 0x0005426E File Offset: 0x0005246E
		private void OnDestroy()
		{
			SessionData.UserRecordUpdated -= this.RefreshState;
		}

		// Token: 0x060019C1 RID: 6593 RVA: 0x00107694 File Offset: 0x00105894
		private void RefreshState()
		{
			if (SessionData.User != null)
			{
				bool showActivate = SessionData.User.IsTrial() || !SessionData.User.IsSubscriber();
				this.TogglePanels(true, showActivate);
				return;
			}
			this.TogglePanels(false, false);
		}

		// Token: 0x060019C2 RID: 6594 RVA: 0x001076D8 File Offset: 0x001058D8
		private void TogglePanels(bool showInfo, bool showActivate)
		{
			this.m_infoPanel.SetActive(showInfo);
			this.m_activatePanel.SetActive(showActivate);
			Vector2 sizeDelta = this.m_thisRect.sizeDelta;
			if (showInfo)
			{
				sizeDelta.y += this.m_infoPanelRect.sizeDelta.y;
			}
			if (showActivate)
			{
				sizeDelta.y += this.m_activatePanelRect.sizeDelta.y;
			}
			this.m_thisRect.sizeDelta = sizeDelta;
		}

		// Token: 0x040020B4 RID: 8372
		[SerializeField]
		private GameObject m_infoPanel;

		// Token: 0x040020B5 RID: 8373
		[SerializeField]
		private GameObject m_activatePanel;

		// Token: 0x040020B6 RID: 8374
		private RectTransform m_thisRect;

		// Token: 0x040020B7 RID: 8375
		private RectTransform m_infoPanelRect;

		// Token: 0x040020B8 RID: 8376
		private RectTransform m_activatePanelRect;
	}
}

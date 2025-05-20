using System;
using Cysharp.Text;
using SoL.Game;
using SoL.Game.Interactives;
using SoL.Managers;
using SoL.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace SoL.Subscription
{
	// Token: 0x020003AC RID: 940
	public class SubscriptionControlPanelIcon : MonoBehaviour, ITooltip, IInteractiveBase
	{
		// Token: 0x0600199D RID: 6557 RVA: 0x00106DD4 File Offset: 0x00104FD4
		private void Start()
		{
			this.m_button.onClick.AddListener(new UnityAction(this.ButtonClicked));
			SessionData.UserRecordUpdated += this.RefreshIcon;
			LocalPlayer.LocalPlayerInitialized += this.RefreshIcon;
			base.InvokeRepeating("RefreshIcon", 0f, 60f);
		}

		// Token: 0x0600199E RID: 6558 RVA: 0x00106E34 File Offset: 0x00105034
		private void OnDestroy()
		{
			this.m_button.onClick.RemoveListener(new UnityAction(this.ButtonClicked));
			SessionData.UserRecordUpdated -= this.RefreshIcon;
			LocalPlayer.LocalPlayerInitialized -= this.RefreshIcon;
			base.CancelInvoke("RefreshIcon");
		}

		// Token: 0x0600199F RID: 6559 RVA: 0x00054156 File Offset: 0x00052356
		private void ButtonClicked()
		{
			if (SessionData.User != null && !SessionData.User.IsSubscriber())
			{
				SubscriptionExtensions.ExecuteActivateSubscription();
			}
		}

		// Token: 0x060019A0 RID: 6560 RVA: 0x00054170 File Offset: 0x00052370
		private void RefreshIcon()
		{
			if (this.m_iconObj)
			{
				this.m_iconObj.ZStringSetText(SubscriptionExtensions.GetIconUnicode());
			}
		}

		// Token: 0x060019A1 RID: 6561 RVA: 0x0005418F File Offset: 0x0005238F
		private ITooltipParameter GetTooltipParameter()
		{
			if (!this.m_showTooltip)
			{
				return null;
			}
			return SubscriptionExtensions.GetTooltipParameter(this, false);
		}

		// Token: 0x17000614 RID: 1556
		// (get) Token: 0x060019A2 RID: 6562 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000615 RID: 1557
		// (get) Token: 0x060019A3 RID: 6563 RVA: 0x000541A2 File Offset: 0x000523A2
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x17000616 RID: 1558
		// (get) Token: 0x060019A4 RID: 6564 RVA: 0x000541B0 File Offset: 0x000523B0
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x060019A6 RID: 6566 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x0400208B RID: 8331
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x0400208C RID: 8332
		[SerializeField]
		private SolButton m_button;

		// Token: 0x0400208D RID: 8333
		[SerializeField]
		private TextMeshProUGUI m_iconObj;

		// Token: 0x0400208E RID: 8334
		[SerializeField]
		private bool m_showTooltip;

		// Token: 0x0400208F RID: 8335
		[SerializeField]
		private bool m_isOptionsMenu;
	}
}

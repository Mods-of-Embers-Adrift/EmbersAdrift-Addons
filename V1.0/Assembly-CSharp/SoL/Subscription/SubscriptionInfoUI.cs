using System;
using Cysharp.Text;
using SoL.Game;
using SoL.Game.Interactives;
using SoL.Managers;
using SoL.UI;
using TMPro;
using UnityEngine;

namespace SoL.Subscription
{
	// Token: 0x020003B3 RID: 947
	public class SubscriptionInfoUI : MonoBehaviour, ITooltip, IInteractiveBase
	{
		// Token: 0x17000617 RID: 1559
		// (get) Token: 0x060019B4 RID: 6580 RVA: 0x00054214 File Offset: 0x00052414
		// (set) Token: 0x060019B5 RID: 6581 RVA: 0x00107440 File Offset: 0x00105640
		private SubscriptionState State
		{
			get
			{
				return this.m_state;
			}
			set
			{
				if (this.m_state == value)
				{
					return;
				}
				this.m_state = value;
				SubscriptionState state = this.m_state;
				if (state - SubscriptionState.NoSubscription > 1)
				{
					return;
				}
				this.m_label.ZStringSetText(SubscriptionExtensions.GetLabel(null, false));
				this.m_subIcon.ZStringSetText(SubscriptionExtensions.GetIconUnicode());
			}
		}

		// Token: 0x060019B6 RID: 6582 RVA: 0x0005421C File Offset: 0x0005241C
		private void OnEnable()
		{
			this.ClearData();
		}

		// Token: 0x060019B7 RID: 6583 RVA: 0x0010749C File Offset: 0x0010569C
		private void Update()
		{
			if (!SessionData.IsSubscriber)
			{
				this.State = SubscriptionState.NoSubscription;
				this.ClearData();
				return;
			}
			if (SessionData.SubscriptionExpires != this.m_cachedExpirationTime)
			{
				this.m_cachedExpirationTime = SessionData.SubscriptionExpires;
				if (this.m_cachedExpirationTime != null)
				{
					this.m_truncatedExpirationTime = new DateTime?(new DateTime(this.m_cachedExpirationTime.Value.Year, this.m_cachedExpirationTime.Value.Month, this.m_cachedExpirationTime.Value.Day, this.m_cachedExpirationTime.Value.Hour, 0, 0));
				}
			}
			if (this.m_truncatedExpirationTime != null)
			{
				DateTime serverCorrectedDateTimeUtc = GameTimeReplicator.GetServerCorrectedDateTimeUtc();
				TimeSpan timeSpan = (serverCorrectedDateTimeUtc > this.m_truncatedExpirationTime.Value) ? new TimeSpan(0L) : (this.m_truncatedExpirationTime.Value - serverCorrectedDateTimeUtc);
				if (this.m_lastTimeDelta == null || this.m_lastTimeDelta.Value != timeSpan)
				{
					this.m_lastTimeDelta = new TimeSpan?(timeSpan);
					this.m_label.ZStringSetText(SubscriptionExtensions.GetLabel(new TimeSpan?(this.m_lastTimeDelta.Value), false));
					this.m_subIcon.ZStringSetText(SubscriptionExtensions.GetIconUnicode());
				}
				this.State = SubscriptionState.ExpiringSubscription;
				return;
			}
			this.State = SubscriptionState.ActiveSubscription;
		}

		// Token: 0x060019B8 RID: 6584 RVA: 0x00054224 File Offset: 0x00052424
		private void ClearData()
		{
			this.m_cachedExpirationTime = null;
			this.m_truncatedExpirationTime = null;
			this.m_lastTimeDelta = null;
		}

		// Token: 0x060019B9 RID: 6585 RVA: 0x0005424A File Offset: 0x0005244A
		private ITooltipParameter GetTooltipParameter()
		{
			return SubscriptionExtensions.GetTooltipParameter(this, this.m_isOptionsMenu);
		}

		// Token: 0x17000618 RID: 1560
		// (get) Token: 0x060019BA RID: 6586 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000619 RID: 1561
		// (get) Token: 0x060019BB RID: 6587 RVA: 0x00054258 File Offset: 0x00052458
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x1700061A RID: 1562
		// (get) Token: 0x060019BC RID: 6588 RVA: 0x00054266 File Offset: 0x00052466
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x060019BE RID: 6590 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x040020AC RID: 8364
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x040020AD RID: 8365
		[SerializeField]
		private bool m_isOptionsMenu;

		// Token: 0x040020AE RID: 8366
		[SerializeField]
		private TextMeshProUGUI m_subIcon;

		// Token: 0x040020AF RID: 8367
		[SerializeField]
		private TextMeshProUGUI m_label;

		// Token: 0x040020B0 RID: 8368
		private DateTime? m_cachedExpirationTime;

		// Token: 0x040020B1 RID: 8369
		private DateTime? m_truncatedExpirationTime;

		// Token: 0x040020B2 RID: 8370
		private TimeSpan? m_lastTimeDelta;

		// Token: 0x040020B3 RID: 8371
		private SubscriptionState m_state;
	}
}

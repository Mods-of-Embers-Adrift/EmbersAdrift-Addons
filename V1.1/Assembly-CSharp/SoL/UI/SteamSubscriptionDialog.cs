using System;
using Cysharp.Text;
using SoL.Networking;
using SoL.Subscription;
using TMPro;
using UnityEngine;

namespace SoL.UI
{
	// Token: 0x02000359 RID: 857
	public class SteamSubscriptionDialog : BaseDialog<SteamSubscriptionDialogOptions>
	{
		// Token: 0x06001768 RID: 5992 RVA: 0x00052573 File Offset: 0x00050773
		private void Update()
		{
			if (base.Visible && this.m_currentOptions.AutoCancel != null && this.m_currentOptions.AutoCancel())
			{
				base.Cancel();
				this.m_currentOptions.AutoCancel = null;
			}
		}

		// Token: 0x06001769 RID: 5993 RVA: 0x0010242C File Offset: 0x0010062C
		protected override void InitInternal()
		{
			base.InitInternal();
			if (this.m_topLabel)
			{
				this.m_topLabel.ZStringSetText(SteamConfig.GetPurchaseSubscriptionDescription());
			}
			if (this.m_costLabel)
			{
				this.m_costLabel.ZStringSetText(SteamConfig.GetSubscriptionCostDisplay());
			}
			if (this.m_perksHeaderLabel)
			{
				this.m_perksHeaderLabel.ZStringSetText(SteamConfig.GetSubscriptionPerksHeader());
			}
			if (this.m_perksLabel)
			{
				Utf16ValueStringBuilder stringBuilder = ZString.CreateStringBuilder();
				try
				{
					foreach (SubscriptionPerk subscriptionPerk in SteamConfig.GetSubscriberPerks())
					{
						subscriptionPerk.AddLine(ref stringBuilder, null);
						if (subscriptionPerk.Children != null && subscriptionPerk.Children.Length != 0)
						{
							SubscriptionPerkInfo[] children = subscriptionPerk.Children;
							for (int j = 0; j < children.Length; j++)
							{
								children[j].AddLine(ref stringBuilder, "18pt");
							}
						}
					}
					this.m_perksLabel.SetText(stringBuilder);
				}
				finally
				{
					stringBuilder.Dispose();
				}
			}
			this.m_confirm.text = SteamConfig.GetAcceptText();
			this.m_cancel.text = SteamConfig.GetCancelText();
		}

		// Token: 0x04001F25 RID: 7973
		[SerializeField]
		private TextMeshProUGUI m_topLabel;

		// Token: 0x04001F26 RID: 7974
		[SerializeField]
		private TextMeshProUGUI m_perksHeaderLabel;

		// Token: 0x04001F27 RID: 7975
		[SerializeField]
		private TextMeshProUGUI m_perksLabel;

		// Token: 0x04001F28 RID: 7976
		[SerializeField]
		private TextMeshProUGUI m_costLabel;
	}
}

using System;
using Cysharp.Text;
using SoL.Game.Interactives;
using SoL.UI;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.AuctionHouse
{
	// Token: 0x02000D40 RID: 3392
	public class DepositTooltip : MonoBehaviour, ITooltip, IInteractiveBase
	{
		// Token: 0x06006630 RID: 26160 RVA: 0x00210350 File Offset: 0x0020E550
		private ITooltipParameter GetTooltipParameter()
		{
			if (!this.m_newAuctionUI || this.m_newAuctionUI.CurrentDeposit <= 0UL || !LocalPlayer.GameEntity)
			{
				return null;
			}
			string txt = string.Empty;
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder())
			{
				int arg = LocalPlayer.GameEntity.Subscriber ? 5 : 10;
				utf16ValueStringBuilder.AppendFormat<string>("A deposit of {0} is required to post this Auction.\n\n", new CurrencyConverter(this.m_newAuctionUI.CurrentDeposit).ToString());
				utf16ValueStringBuilder.AppendFormat<int>("If this item sells: the Auction House will take the greater of this deposit or {0}% of the sale price.\n\n", arg);
				utf16ValueStringBuilder.Append("If this auction is cancelled or expires: the Auction House will retain this deposit.");
				txt = utf16ValueStringBuilder.ToString();
			}
			return new ObjectTextTooltipParameter(this, txt, false);
		}

		// Token: 0x17001872 RID: 6258
		// (get) Token: 0x06006631 RID: 26161 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17001873 RID: 6259
		// (get) Token: 0x06006632 RID: 26162 RVA: 0x00084DA2 File Offset: 0x00082FA2
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x17001874 RID: 6260
		// (get) Token: 0x06006633 RID: 26163 RVA: 0x00084DB0 File Offset: 0x00082FB0
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x06006635 RID: 26165 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x040058D3 RID: 22739
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x040058D4 RID: 22740
		[SerializeField]
		private AuctionHouseNewAuctionUI m_newAuctionUI;
	}
}

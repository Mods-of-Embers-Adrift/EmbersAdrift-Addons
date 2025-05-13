using System;
using SoL.Game.Interactives;
using SoL.UI;
using UnityEngine;

namespace SoL.Game.AuctionHouse
{
	// Token: 0x02000D3E RID: 3390
	public class AuctionHouseSelectedItemTooltip : MonoBehaviour, ITooltip, IInteractiveBase
	{
		// Token: 0x0600661A RID: 26138 RVA: 0x00210120 File Offset: 0x0020E320
		private ITooltipParameter GetTooltipParameter()
		{
			if (this.m_auctionList && this.m_auctionList.SelectedAuction != null && this.m_auctionList.SelectedAuction.Instance != null)
			{
				return new ArchetypeTooltipParameter
				{
					Instance = this.m_auctionList.SelectedAuction.Instance
				};
			}
			return null;
		}

		// Token: 0x1700186D RID: 6253
		// (get) Token: 0x0600661B RID: 26139 RVA: 0x00084CA6 File Offset: 0x00082EA6
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x1700186E RID: 6254
		// (get) Token: 0x0600661C RID: 26140 RVA: 0x00084CB4 File Offset: 0x00082EB4
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x1700186F RID: 6255
		// (get) Token: 0x0600661D RID: 26141 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x0600661F RID: 26143 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x040058CE RID: 22734
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x040058CF RID: 22735
		[SerializeField]
		private AuctionHouseForSaleList m_auctionList;
	}
}

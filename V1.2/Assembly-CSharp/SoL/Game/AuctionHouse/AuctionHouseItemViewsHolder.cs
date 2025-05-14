using System;
using Com.TheFallenGames.OSA.Core;
using SoL.Networking.Database;

namespace SoL.Game.AuctionHouse
{
	// Token: 0x02000D39 RID: 3385
	[Serializable]
	public class AuctionHouseItemViewsHolder : BaseItemViewsHolder
	{
		// Token: 0x060065B7 RID: 26039 RVA: 0x000848AC File Offset: 0x00082AAC
		public void RefreshAvailability()
		{
			this.m_saleListItem.RefreshAvailability();
		}

		// Token: 0x060065B8 RID: 26040 RVA: 0x000848B9 File Offset: 0x00082AB9
		public void RefreshHoldingShift()
		{
			this.m_saleListItem.RefreshCurrencyPanels();
		}

		// Token: 0x060065B9 RID: 26041 RVA: 0x000848C6 File Offset: 0x00082AC6
		public void RefreshTimeLeftLabel()
		{
			this.m_saleListItem.RefreshTimeLeftLabel();
		}

		// Token: 0x060065BA RID: 26042 RVA: 0x000848D3 File Offset: 0x00082AD3
		public void RefreshSelected()
		{
			this.m_saleListItem.RefreshSelected();
		}

		// Token: 0x060065BB RID: 26043 RVA: 0x000848E0 File Offset: 0x00082AE0
		public void UpdateSaleItem(AuctionHouseForSaleList controller, AuctionRecord auction)
		{
			this.m_auction = auction;
			this.m_saleListItem.InitItem(controller, this.m_auction);
		}

		// Token: 0x060065BC RID: 26044 RVA: 0x000848FB File Offset: 0x00082AFB
		public override void CollectViews()
		{
			base.CollectViews();
			this.m_saleListItem = this.root.GetComponent<AuctionHouseForSaleListItem>();
		}

		// Token: 0x0400587F RID: 22655
		private AuctionHouseForSaleListItem m_saleListItem;

		// Token: 0x04005880 RID: 22656
		private AuctionRecord m_auction;
	}
}

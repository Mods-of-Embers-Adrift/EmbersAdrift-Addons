using System;
using Com.TheFallenGames.OSA.Core;
using SoL.Networking.Database;

namespace SoL.Game.UI.Merchants
{
	// Token: 0x0200096A RID: 2410
	[Serializable]
	public class MerchantBuybackItemViewsHolder : BaseItemViewsHolder
	{
		// Token: 0x0600478B RID: 18315 RVA: 0x0007028E File Offset: 0x0006E48E
		public void RefreshAvailability()
		{
			this.m_buybackListItem.RefreshAvailability();
		}

		// Token: 0x0600478C RID: 18316 RVA: 0x0007029B File Offset: 0x0006E49B
		public void UpdateSaleItem(MerchantBuybackList controller, MerchantBuybackItem buybackItem)
		{
			if (this.m_buybackItem == buybackItem)
			{
				return;
			}
			this.m_buybackItem = buybackItem;
			this.m_buybackListItem.InitItem(controller, this.m_buybackItem);
		}

		// Token: 0x0600478D RID: 18317 RVA: 0x000702C0 File Offset: 0x0006E4C0
		public override void CollectViews()
		{
			base.CollectViews();
			this.m_buybackListItem = this.root.GetComponent<MerchantBuybackListItem>();
		}

		// Token: 0x04004342 RID: 17218
		private MerchantBuybackListItem m_buybackListItem;

		// Token: 0x04004343 RID: 17219
		private MerchantBuybackItem m_buybackItem;
	}
}

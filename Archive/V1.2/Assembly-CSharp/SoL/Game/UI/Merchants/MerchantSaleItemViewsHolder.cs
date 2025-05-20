using System;
using Com.TheFallenGames.OSA.Core;
using SoL.Game.Objects.Archetypes;

namespace SoL.Game.UI.Merchants
{
	// Token: 0x0200096F RID: 2415
	[Serializable]
	public class MerchantSaleItemViewsHolder : BaseItemViewsHolder
	{
		// Token: 0x060047BF RID: 18367 RVA: 0x000704C1 File Offset: 0x0006E6C1
		public void RefreshAvailability()
		{
			this.m_saleListItem.RefreshAvailability();
		}

		// Token: 0x060047C0 RID: 18368 RVA: 0x000704CE File Offset: 0x0006E6CE
		public void RefreshHoldingShift()
		{
			this.m_saleListItem.RefreshHoldingShiftForStackable();
		}

		// Token: 0x060047C1 RID: 18369 RVA: 0x000704DB File Offset: 0x0006E6DB
		public void UpdateSaleItem(MerchantForSaleList controller, IMerchantInventory sellable)
		{
			if (this.m_sellable == sellable)
			{
				return;
			}
			this.m_sellable = sellable;
			this.m_saleListItem.InitItem(controller, this.m_sellable);
		}

		// Token: 0x060047C2 RID: 18370 RVA: 0x00070500 File Offset: 0x0006E700
		public override void CollectViews()
		{
			base.CollectViews();
			this.m_saleListItem = this.root.GetComponent<MerchantForSaleListItem>();
		}

		// Token: 0x04004356 RID: 17238
		private MerchantForSaleListItem m_saleListItem;

		// Token: 0x04004357 RID: 17239
		private IMerchantInventory m_sellable;
	}
}

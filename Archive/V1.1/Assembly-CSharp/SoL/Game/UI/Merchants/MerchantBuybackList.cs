using System;
using System.Collections.Generic;
using Com.TheFallenGames.OSA.Core;
using SoL.Game.Interactives;
using SoL.Game.Objects.Containers;
using SoL.Networking.Database;
using SoL.UI;
using UnityEngine;

namespace SoL.Game.UI.Merchants
{
	// Token: 0x02000968 RID: 2408
	public class MerchantBuybackList : OSA<MerchantBuybackItemParam, MerchantBuybackItemViewsHolder>
	{
		// Token: 0x17000FDC RID: 4060
		// (get) Token: 0x0600477F RID: 18303 RVA: 0x00070212 File Offset: 0x0006E412
		internal bool IsVisible
		{
			get
			{
				return this.m_window && this.m_window.Visible;
			}
		}

		// Token: 0x17000FDD RID: 4061
		// (get) Token: 0x06004780 RID: 18304 RVA: 0x0007022E File Offset: 0x0006E42E
		internal MerchantType MerchantType
		{
			get
			{
				return this.m_merchantType;
			}
		}

		// Token: 0x06004781 RID: 18305 RVA: 0x001A72A8 File Offset: 0x001A54A8
		protected override void Start()
		{
			base.Start();
			LocalPlayer.GameEntity.CollectionController.Inventory.CurrencyChanged += this.InventoryOnCurrencyChanged;
			LocalPlayer.GameEntity.CollectionController.Inventory.ContentsChanged += this.InventoryOnContentsChanged;
		}

		// Token: 0x06004782 RID: 18306 RVA: 0x001A72FC File Offset: 0x001A54FC
		protected override void OnDestroy()
		{
			base.OnDestroy();
			if (LocalPlayer.GameEntity != null)
			{
				ICollectionController collectionController = LocalPlayer.GameEntity.CollectionController;
				if (collectionController != null && collectionController.Inventory != null)
				{
					LocalPlayer.GameEntity.CollectionController.Inventory.CurrencyChanged -= this.InventoryOnCurrencyChanged;
					LocalPlayer.GameEntity.CollectionController.Inventory.ContentsChanged -= this.InventoryOnContentsChanged;
				}
			}
		}

		// Token: 0x06004783 RID: 18307 RVA: 0x001A7374 File Offset: 0x001A5574
		public void UpdateItems(BuybackItemData buybackItemData)
		{
			if (!base.gameObject.activeInHierarchy)
			{
				return;
			}
			if (this.m_buybackItems == null)
			{
				this.m_buybackItems = new List<MerchantBuybackItem>(buybackItemData.Items.Count);
			}
			this.m_buybackItems.Clear();
			for (int i = 0; i < buybackItemData.Items.Count; i++)
			{
				this.m_buybackItems.Add(buybackItemData.Items[i]);
			}
			if (!base.IsInitialized)
			{
				base.Init();
			}
			this.ResetItems(this.m_buybackItems.Count, false, false);
		}

		// Token: 0x06004784 RID: 18308 RVA: 0x00070236 File Offset: 0x0006E436
		protected override MerchantBuybackItemViewsHolder CreateViewsHolder(int itemIndex)
		{
			MerchantBuybackItemViewsHolder merchantBuybackItemViewsHolder = new MerchantBuybackItemViewsHolder();
			merchantBuybackItemViewsHolder.Init(this._Params.ItemPrefab, this._Params.Content, itemIndex, true, true);
			return merchantBuybackItemViewsHolder;
		}

		// Token: 0x06004785 RID: 18309 RVA: 0x0007025C File Offset: 0x0006E45C
		protected override void UpdateViewsHolder(MerchantBuybackItemViewsHolder newOrRecycled)
		{
			newOrRecycled.UpdateSaleItem(this, this.m_buybackItems[newOrRecycled.ItemIndex]);
		}

		// Token: 0x06004786 RID: 18310 RVA: 0x00070276 File Offset: 0x0006E476
		private void InventoryOnCurrencyChanged(ulong obj)
		{
			this.RefreshAvailability();
		}

		// Token: 0x06004787 RID: 18311 RVA: 0x00070276 File Offset: 0x0006E476
		private void InventoryOnContentsChanged()
		{
			this.RefreshAvailability();
		}

		// Token: 0x06004788 RID: 18312 RVA: 0x001A7408 File Offset: 0x001A5608
		public void RefreshAvailability()
		{
			if (this.m_buybackItems == null || this.m_buybackItems.Count <= 0 || !this.m_window.Visible)
			{
				return;
			}
			for (int i = 0; i < this.m_buybackItems.Count; i++)
			{
				MerchantBuybackItemViewsHolder itemViewsHolder = base.GetItemViewsHolder(i);
				if (itemViewsHolder != null)
				{
					itemViewsHolder.RefreshAvailability();
				}
			}
		}

		// Token: 0x0400433F RID: 17215
		[SerializeField]
		private MerchantType m_merchantType;

		// Token: 0x04004340 RID: 17216
		[SerializeField]
		private UIWindow m_window;

		// Token: 0x04004341 RID: 17217
		private List<MerchantBuybackItem> m_buybackItems;
	}
}

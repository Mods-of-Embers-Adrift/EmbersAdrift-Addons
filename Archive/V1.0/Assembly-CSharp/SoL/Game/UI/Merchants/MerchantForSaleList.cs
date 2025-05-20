using System;
using System.Collections.Generic;
using Com.TheFallenGames.OSA.Core;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Containers;
using SoL.Managers;
using SoL.UI;
using UnityEngine;

namespace SoL.Game.UI.Merchants
{
	// Token: 0x0200096D RID: 2413
	public class MerchantForSaleList : OSA<MerchantSaleItemParam, MerchantSaleItemViewsHolder>
	{
		// Token: 0x17000FE7 RID: 4071
		// (get) Token: 0x060047AD RID: 18349 RVA: 0x00070409 File Offset: 0x0006E609
		public bool HoldingShift
		{
			get
			{
				return this.m_holdingShift;
			}
		}

		// Token: 0x060047AE RID: 18350 RVA: 0x00070411 File Offset: 0x0006E611
		protected override void Start()
		{
			base.Start();
			if (LocalPlayer.IsInitialized)
			{
				this.InitData();
				return;
			}
			LocalPlayer.LocalPlayerInitialized += this.OnLocalPlayerInitialized;
		}

		// Token: 0x060047AF RID: 18351 RVA: 0x001A784C File Offset: 0x001A5A4C
		protected override void OnDestroy()
		{
			base.OnDestroy();
			if (LocalPlayer.GameEntity)
			{
				ICollectionController collectionController = LocalPlayer.GameEntity.CollectionController;
				if (collectionController != null)
				{
					if (collectionController.Inventory != null)
					{
						LocalPlayer.GameEntity.CollectionController.Inventory.CurrencyChanged -= this.InventoryOnCurrencyChanged;
						LocalPlayer.GameEntity.CollectionController.Inventory.ContentsChanged -= this.InventoryOnContentsChanged;
					}
					if (collectionController.Masteries != null)
					{
						LocalPlayer.GameEntity.CollectionController.Masteries.ContentsChanged -= this.InventoryOnContentsChanged;
					}
					if (collectionController.Abilities != null)
					{
						LocalPlayer.GameEntity.CollectionController.Abilities.ContentsChanged -= this.InventoryOnContentsChanged;
					}
				}
				if (LocalPlayer.GameEntity.CharacterData)
				{
					LocalPlayer.GameEntity.CharacterData.HighestMasteryLevelChanged -= this.CharacterDataOnHighestMasteryLevelChanged;
				}
			}
			UIManager.EventCurrencyChanged -= this.OnEventCurrencyChanged;
		}

		// Token: 0x060047B0 RID: 18352 RVA: 0x001A7954 File Offset: 0x001A5B54
		protected override void Update()
		{
			base.Update();
			bool holdingShift = this.m_holdingShift;
			this.m_holdingShift = (ClientGameManager.InputManager != null && ClientGameManager.InputManager.HoldingShift);
			if (this.m_window && this.m_window.Visible && holdingShift != this.m_holdingShift)
			{
				this.RefreshHoldingShiftForStackables();
			}
		}

		// Token: 0x060047B1 RID: 18353 RVA: 0x00070438 File Offset: 0x0006E638
		private void OnLocalPlayerInitialized()
		{
			LocalPlayer.LocalPlayerInitialized -= this.OnLocalPlayerInitialized;
			this.InitData();
		}

		// Token: 0x060047B2 RID: 18354 RVA: 0x001A79B4 File Offset: 0x001A5BB4
		private void InitData()
		{
			if (LocalPlayer.GameEntity)
			{
				ICollectionController collectionController = LocalPlayer.GameEntity.CollectionController;
				if (collectionController != null)
				{
					if (collectionController.Inventory != null)
					{
						LocalPlayer.GameEntity.CollectionController.Inventory.CurrencyChanged += this.InventoryOnCurrencyChanged;
						LocalPlayer.GameEntity.CollectionController.Inventory.ContentsChanged += this.InventoryOnContentsChanged;
					}
					if (collectionController.Masteries != null)
					{
						LocalPlayer.GameEntity.CollectionController.Masteries.ContentsChanged += this.InventoryOnContentsChanged;
					}
					if (collectionController.Abilities != null)
					{
						LocalPlayer.GameEntity.CollectionController.Abilities.ContentsChanged += this.InventoryOnContentsChanged;
					}
				}
				if (LocalPlayer.GameEntity.CharacterData)
				{
					LocalPlayer.GameEntity.CharacterData.HighestMasteryLevelChanged += this.CharacterDataOnHighestMasteryLevelChanged;
				}
			}
			UIManager.EventCurrencyChanged += this.OnEventCurrencyChanged;
		}

		// Token: 0x060047B3 RID: 18355 RVA: 0x001A7AB8 File Offset: 0x001A5CB8
		public void UpdateItems(UniqueId[] forSaleItemIds, string nameFilter)
		{
			if (!base.gameObject.activeInHierarchy || forSaleItemIds == null)
			{
				return;
			}
			if (this.m_saleItems == null)
			{
				this.m_saleItems = new List<IMerchantInventory>(forSaleItemIds.Length);
			}
			this.m_saleItems.Clear();
			bool flag = string.IsNullOrEmpty(nameFilter);
			for (int i = 0; i < forSaleItemIds.Length; i++)
			{
				IMerchantInventory merchantInventory;
				if (InternalGameDatabase.Archetypes.TryGetAsType<IMerchantInventory>(forSaleItemIds[i], out merchantInventory) && (flag || MerchantForSaleList.MatchesTextFilter(merchantInventory, nameFilter)))
				{
					this.m_saleItems.Add(merchantInventory);
				}
			}
			this.ResetItems(this.m_saleItems.Count, false, false);
		}

		// Token: 0x060047B4 RID: 18356 RVA: 0x00070451 File Offset: 0x0006E651
		private static bool MatchesTextFilter(IMerchantInventory sellable, string textFilter)
		{
			return sellable != null && sellable.Archetype && sellable.Archetype.MatchesTextFilter(textFilter);
		}

		// Token: 0x060047B5 RID: 18357 RVA: 0x00070471 File Offset: 0x0006E671
		protected override MerchantSaleItemViewsHolder CreateViewsHolder(int itemIndex)
		{
			MerchantSaleItemViewsHolder merchantSaleItemViewsHolder = new MerchantSaleItemViewsHolder();
			merchantSaleItemViewsHolder.Init(this._Params.ItemPrefab, this._Params.Content, itemIndex, true, true);
			return merchantSaleItemViewsHolder;
		}

		// Token: 0x060047B6 RID: 18358 RVA: 0x00070497 File Offset: 0x0006E697
		protected override void UpdateViewsHolder(MerchantSaleItemViewsHolder newOrRecycled)
		{
			newOrRecycled.UpdateSaleItem(this, this.m_saleItems[newOrRecycled.ItemIndex]);
		}

		// Token: 0x060047B7 RID: 18359 RVA: 0x000704B1 File Offset: 0x0006E6B1
		private void InventoryOnCurrencyChanged(ulong obj)
		{
			this.RefreshAvailability();
		}

		// Token: 0x060047B8 RID: 18360 RVA: 0x000704B1 File Offset: 0x0006E6B1
		private void OnEventCurrencyChanged()
		{
			this.RefreshAvailability();
		}

		// Token: 0x060047B9 RID: 18361 RVA: 0x000704B1 File Offset: 0x0006E6B1
		private void InventoryOnContentsChanged()
		{
			this.RefreshAvailability();
		}

		// Token: 0x060047BA RID: 18362 RVA: 0x000704B1 File Offset: 0x0006E6B1
		private void CharacterDataOnHighestMasteryLevelChanged()
		{
			this.RefreshAvailability();
		}

		// Token: 0x060047BB RID: 18363 RVA: 0x001A7B4C File Offset: 0x001A5D4C
		public void RefreshAvailability()
		{
			if (this.m_saleItems == null || this.m_saleItems.Count <= 0 || !this.m_window.Visible)
			{
				return;
			}
			for (int i = 0; i < this.m_saleItems.Count; i++)
			{
				MerchantSaleItemViewsHolder itemViewsHolderIfVisible = base.GetItemViewsHolderIfVisible(i);
				if (itemViewsHolderIfVisible != null)
				{
					itemViewsHolderIfVisible.RefreshAvailability();
				}
			}
		}

		// Token: 0x060047BC RID: 18364 RVA: 0x001A7BA8 File Offset: 0x001A5DA8
		public void RefreshHoldingShiftForStackables()
		{
			if (this.m_saleItems == null || this.m_saleItems.Count <= 0 || !this.m_window.Visible)
			{
				return;
			}
			for (int i = 0; i < this.m_saleItems.Count; i++)
			{
				MerchantSaleItemViewsHolder itemViewsHolderIfVisible = base.GetItemViewsHolderIfVisible(i);
				if (itemViewsHolderIfVisible != null)
				{
					itemViewsHolderIfVisible.RefreshHoldingShift();
				}
			}
		}

		// Token: 0x04004353 RID: 17235
		[SerializeField]
		private UIWindow m_window;

		// Token: 0x04004354 RID: 17236
		private List<IMerchantInventory> m_saleItems;

		// Token: 0x04004355 RID: 17237
		private bool m_holdingShift;
	}
}

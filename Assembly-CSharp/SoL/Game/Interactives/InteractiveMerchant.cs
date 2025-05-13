using System;
using System.Collections;
using System.Collections.Generic;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Containers;
using SoL.Game.Transactions;
using SoL.Managers;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using SoL.Utilities.Logging;
using UnityEngine;

namespace SoL.Game.Interactives
{
	// Token: 0x02000B98 RID: 2968
	public class InteractiveMerchant : BaseNetworkInteractiveStation
	{
		// Token: 0x1700156B RID: 5483
		// (get) Token: 0x06005B6A RID: 23402 RVA: 0x0007D64B File Offset: 0x0007B84B
		// (set) Token: 0x06005B6B RID: 23403 RVA: 0x0007D653 File Offset: 0x0007B853
		public MerchantType MerchantType
		{
			get
			{
				return this.m_merchantType;
			}
			set
			{
				this.m_merchantType = value;
			}
		}

		// Token: 0x1700156C RID: 5484
		// (get) Token: 0x06005B6C RID: 23404 RVA: 0x0007D65C File Offset: 0x0007B85C
		public override string CurrencyRemovalMessage
		{
			get
			{
				return "for purchase.";
			}
		}

		// Token: 0x06005B6D RID: 23405 RVA: 0x001EE9E8 File Offset: 0x001ECBE8
		protected override bool UseEventCurrency()
		{
			if (GameManager.IsServer)
			{
				return this.MerchantType == MerchantType.Event;
			}
			return ClientGameManager.UIManager && ClientGameManager.UIManager.MerchantUI && ClientGameManager.UIManager.MerchantUI.UseEventCurrency();
		}

		// Token: 0x1700156D RID: 5485
		// (get) Token: 0x06005B6E RID: 23406 RVA: 0x0007D663 File Offset: 0x0007B863
		protected override string m_tooltipText
		{
			get
			{
				return "Merchant";
			}
		}

		// Token: 0x1700156E RID: 5486
		// (get) Token: 0x06005B6F RID: 23407 RVA: 0x000706BE File Offset: 0x0006E8BE
		protected override ContainerType m_containerType
		{
			get
			{
				return ContainerType.MerchantOutgoing;
			}
		}

		// Token: 0x1700156F RID: 5487
		// (get) Token: 0x06005B70 RID: 23408 RVA: 0x0004479C File Offset: 0x0004299C
		protected override bool AllowInteractionWhileMissingBag
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17001570 RID: 5488
		// (get) Token: 0x06005B71 RID: 23409 RVA: 0x0007D66A File Offset: 0x0007B86A
		public bool IsTrainer
		{
			get
			{
				return this.m_isTrainer;
			}
		}

		// Token: 0x17001571 RID: 5489
		// (get) Token: 0x06005B72 RID: 23410 RVA: 0x0007D672 File Offset: 0x0007B872
		// (set) Token: 0x06005B73 RID: 23411 RVA: 0x0007D67A File Offset: 0x0007B87A
		public ItemFlags ItemFlagsToSet
		{
			get
			{
				return this.m_flagsToSet;
			}
			set
			{
				this.m_flagsToSet = value;
			}
		}

		// Token: 0x17001572 RID: 5490
		// (get) Token: 0x06005B74 RID: 23412 RVA: 0x0007D683 File Offset: 0x0007B883
		// (set) Token: 0x06005B75 RID: 23413 RVA: 0x0007D68B File Offset: 0x0007B88B
		public bool MarkAsSoulbound
		{
			get
			{
				return this.m_markAsSoulbound;
			}
			set
			{
				this.m_markAsSoulbound = value;
			}
		}

		// Token: 0x17001573 RID: 5491
		// (get) Token: 0x06005B76 RID: 23414 RVA: 0x0007D694 File Offset: 0x0007B894
		public float CostMultiplier
		{
			get
			{
				return this.m_costMultiplier;
			}
		}

		// Token: 0x17001574 RID: 5492
		// (get) Token: 0x06005B77 RID: 23415 RVA: 0x0007D69C File Offset: 0x0007B89C
		private bool m_showMerchantBundles
		{
			get
			{
				return this.m_merchantBundleCollection == null;
			}
		}

		// Token: 0x06005B78 RID: 23416 RVA: 0x001EEA34 File Offset: 0x001ECC34
		public override void BeginInteraction(GameEntity interactionSource)
		{
			base.BeginInteraction(interactionSource);
			if (GameManager.IsServer && interactionSource.CollectionController.InteractiveStation == this)
			{
				this.InitSellables();
				interactionSource.CollectionController.RefreshBuybackItems();
				switch (this.MerchantType)
				{
				case MerchantType.Standard:
				case MerchantType.Event:
					interactionSource.NetworkEntity.PlayerRpcHandler.MerchantInventoryUpdate(this.MerchantType, this.m_itemIds);
					return;
				case MerchantType.BagRecovery:
					interactionSource.NetworkEntity.PlayerRpcHandler.MerchantBuybackUpdateRequest();
					break;
				default:
					return;
				}
			}
		}

		// Token: 0x06005B79 RID: 23417 RVA: 0x001EEABC File Offset: 0x001ECCBC
		private void InitSellables()
		{
			if (this.m_initializedItems)
			{
				return;
			}
			this.m_validSellableItems = new HashSet<UniqueId>(default(UniqueIdComparer));
			MerchantBundle[] array = this.m_merchantBundleCollection ? this.m_merchantBundleCollection.MerchantBundles : this.m_merchantBundles;
			if (array == null || array.Length == 0)
			{
				this.m_itemIds = default(ForSaleItemIds);
			}
			else
			{
				List<UniqueId> list = new List<UniqueId>();
				foreach (MerchantBundle merchantBundle in array)
				{
					if (!(merchantBundle == null))
					{
						foreach (BaseArchetype baseArchetype in merchantBundle.Items)
						{
							if (!(baseArchetype == null) && !this.m_validSellableItems.Contains(baseArchetype.Id) && baseArchetype is IMerchantInventory)
							{
								list.Add(baseArchetype.Id);
								this.m_validSellableItems.Add(baseArchetype.Id);
							}
						}
					}
				}
				this.m_itemIds = new ForSaleItemIds
				{
					ItemIds = list.ToArray()
				};
			}
			this.m_initializedItems = true;
		}

		// Token: 0x06005B7A RID: 23418 RVA: 0x00063B81 File Offset: 0x00061D81
		private IEnumerable GetMerchantBundles()
		{
			return SolOdinUtilities.GetDropdownItems<MerchantBundle>();
		}

		// Token: 0x06005B7B RID: 23419 RVA: 0x00063B7A File Offset: 0x00061D7A
		private IEnumerable GetMerchantBundleCollections()
		{
			return SolOdinUtilities.GetDropdownItems<MerchantBundleCollection>();
		}

		// Token: 0x06005B7C RID: 23420 RVA: 0x0007D356 File Offset: 0x0007B556
		private void SendErrorMessage(GameEntity interactionSource, string msg)
		{
			if (interactionSource != null)
			{
				interactionSource.NetworkEntity.PlayerRpcHandler.SendChatNotification(msg);
			}
		}

		// Token: 0x06005B7D RID: 23421 RVA: 0x001EEC04 File Offset: 0x001ECE04
		public void PurchaseRequest(GameEntity interactionSource, UniqueId itemId, uint quantity)
		{
			if (!base.EnabledForBranch())
			{
				this.SendErrorMessage(interactionSource, "Invalid merchant!");
				return;
			}
			if (!this.m_validSellableItems.Contains(itemId))
			{
				this.SendErrorMessage(interactionSource, "Invalid item!");
				return;
			}
			BaseArchetype baseArchetype;
			if (!InternalGameDatabase.Archetypes.TryGetItem(itemId, out baseArchetype))
			{
				this.SendErrorMessage(interactionSource, "Invalid itemId:" + itemId.ToString() + "!");
				return;
			}
			IMerchantInventory merchantInventory = baseArchetype as IMerchantInventory;
			if (merchantInventory == null)
			{
				this.SendErrorMessage(interactionSource, baseArchetype.DisplayName + " is not able to be sold!");
				return;
			}
			ulong num = this.UseEventCurrency() ? merchantInventory.GetEventCost(interactionSource) : merchantInventory.GetSellPrice(interactionSource);
			if (baseArchetype.ArchetypeHasCount())
			{
				if (quantity <= 0U)
				{
					this.SendErrorMessage(interactionSource, "Invalid count!");
					return;
				}
				num *= (ulong)quantity;
			}
			if (this.m_costMultiplier < 1f || this.m_costMultiplier > 1f)
			{
				num = (ulong)((long)Mathf.FloorToInt(num * this.m_costMultiplier));
			}
			CurrencySources currencySources;
			ulong availableCurrency = base.GetAvailableCurrency(interactionSource, out currencySources);
			if (num > availableCurrency)
			{
				this.SendErrorMessage(interactionSource, "Insufficient funds!");
				return;
			}
			string msg;
			if (!merchantInventory.EntityCanAcquire(interactionSource, out msg))
			{
				this.SendErrorMessage(interactionSource, msg);
				return;
			}
			ArchetypeInstance archetypeInstance;
			if (merchantInventory.AddToPlayer(interactionSource, ItemAddContext.Merchant, quantity, this.m_flagsToSet, this.m_markAsSoulbound, out archetypeInstance) && (num <= 0UL || base.TryRemoveCurrency(interactionSource, num)))
			{
				GlobalCounters.ItemsPurchased += 1U;
				if (this.MerchantType != MerchantType.Event)
				{
					this.LogPurchase(interactionSource, merchantInventory.Archetype, quantity, num);
				}
				return;
			}
			this.SendErrorMessage(interactionSource, "Unknown error attempting to purchase " + baseArchetype.DisplayName + "!");
		}

		// Token: 0x06005B7E RID: 23422 RVA: 0x001EED94 File Offset: 0x001ECF94
		private void LogPurchase(GameEntity entity, BaseArchetype archetype, uint count, ulong currency)
		{
			try
			{
				if (InteractiveMerchant.m_purchaseLogArguments == null)
				{
					InteractiveMerchant.m_purchaseLogArguments = new object[7];
				}
				InteractiveMerchant.m_purchaseLogArguments[0] = "PurchaseItem";
				InteractiveMerchant.m_purchaseLogArguments[1] = entity.User.Id;
				InteractiveMerchant.m_purchaseLogArguments[2] = entity.CollectionController.Record.Id;
				InteractiveMerchant.m_purchaseLogArguments[3] = entity.CollectionController.Record.Name;
				InteractiveMerchant.m_purchaseLogArguments[4] = count;
				InteractiveMerchant.m_purchaseLogArguments[5] = archetype.DisplayName;
				InteractiveMerchant.m_purchaseLogArguments[6] = currency;
				SolDebug.LogToIndex(LogLevel.Information, LogIndex.Economy, "{@EventType}.{@UserId}.{@CharacterId}.{@PlayerName} purchased {@Count} {@ItemName} from Merchant for {@Currency}", InteractiveMerchant.m_purchaseLogArguments);
			}
			catch
			{
			}
		}

		// Token: 0x04004FCF RID: 20431
		[SerializeField]
		private MerchantType m_merchantType;

		// Token: 0x04004FD0 RID: 20432
		[SerializeField]
		private bool m_isTrainer;

		// Token: 0x04004FD1 RID: 20433
		[SerializeField]
		private ItemFlags m_flagsToSet;

		// Token: 0x04004FD2 RID: 20434
		[SerializeField]
		private bool m_markAsSoulbound;

		// Token: 0x04004FD3 RID: 20435
		[Range(0f, 2f)]
		[SerializeField]
		private float m_costMultiplier = 1f;

		// Token: 0x04004FD4 RID: 20436
		private const string kBundleGroupName = "Items For Sale";

		// Token: 0x04004FD5 RID: 20437
		[SerializeField]
		protected MerchantBundleCollection m_merchantBundleCollection;

		// Token: 0x04004FD6 RID: 20438
		[SerializeField]
		protected MerchantBundle[] m_merchantBundles;

		// Token: 0x04004FD7 RID: 20439
		private HashSet<UniqueId> m_validSellableItems;

		// Token: 0x04004FD8 RID: 20440
		private bool m_initializedItems;

		// Token: 0x04004FD9 RID: 20441
		private ForSaleItemIds m_itemIds;

		// Token: 0x04004FDA RID: 20442
		private static object[] m_purchaseLogArguments;
	}
}

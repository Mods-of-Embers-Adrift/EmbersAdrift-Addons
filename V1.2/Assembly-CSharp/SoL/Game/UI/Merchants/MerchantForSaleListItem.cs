using System;
using Cysharp.Text;
using SoL.Game.Interactives;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Containers;
using SoL.Managers;
using SoL.UI;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SoL.Game.UI.Merchants
{
	// Token: 0x02000970 RID: 2416
	public class MerchantForSaleListItem : MonoBehaviour, ICursor, IInteractiveBase, IPointerClickHandler, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
	{
		// Token: 0x17000FE8 RID: 4072
		// (get) Token: 0x060047C4 RID: 18372 RVA: 0x00070519 File Offset: 0x0006E719
		// (set) Token: 0x060047C5 RID: 18373 RVA: 0x00070521 File Offset: 0x0006E721
		private bool CanPurchase
		{
			get
			{
				return this.m_canPurchase;
			}
			set
			{
				this.m_canPurchase = value;
				this.m_disabledPanel.enabled = !this.m_canPurchase;
				if (!this.m_canPurchase && this.m_hover.enabled)
				{
					this.m_hover.enabled = false;
				}
			}
		}

		// Token: 0x17000FE9 RID: 4073
		// (get) Token: 0x060047C6 RID: 18374 RVA: 0x0007055F File Offset: 0x0006E75F
		// (set) Token: 0x060047C7 RID: 18375 RVA: 0x00070567 File Offset: 0x0006E767
		public bool CanPurchaseButNoRoom { get; private set; }

		// Token: 0x060047C8 RID: 18376 RVA: 0x00070570 File Offset: 0x0006E770
		private void Awake()
		{
			this.m_currency.InitCoinDisplay(false, true, true, false);
		}

		// Token: 0x060047C9 RID: 18377 RVA: 0x001A7C60 File Offset: 0x001A5E60
		public void InitItem(MerchantForSaleList controller, IMerchantInventory sellable)
		{
			this.m_controller = controller;
			this.m_sellable = sellable;
			ArchetypeIconUI archetypeIcon = this.m_archetypeIcon;
			IMerchantInventory sellable2 = this.m_sellable;
			BaseArchetype archetype = (sellable2 != null) ? sellable2.Archetype : null;
			IMerchantInventory sellable3 = this.m_sellable;
			archetypeIcon.SetIcon(archetype, (sellable3 != null) ? new Color?(sellable3.Archetype.IconTint) : null);
			this.RefreshAvailability();
		}

		// Token: 0x060047CA RID: 18378 RVA: 0x001A7CC4 File Offset: 0x001A5EC4
		public void RefreshAvailability()
		{
			ulong sellPrice = this.GetSellPrice();
			if (this.UseEventCurrency())
			{
				this.m_currency.UpdateEventCurrency(sellPrice);
			}
			else
			{
				this.m_currency.UpdateCoin(sellPrice);
			}
			this.RefreshLabel();
			if (this.m_deathIcon)
			{
				this.m_deathIcon.gameObject.SetActive(false);
			}
			this.CanPurchaseButNoRoom = false;
			if (this.m_sellable != null)
			{
				bool canPurchase = false;
				ItemArchetype item;
				if (this.m_sellable.Archetype.TryGetAsType(out item))
				{
					ContainerInstance containerInstance;
					CannotAcquireReason cannotAcquireReason;
					if (LocalPlayer.GameEntity.EntityCanAcquire(item, out containerInstance, out cannotAcquireReason, null))
					{
						CurrencySources currencySources;
						canPurchase = (containerInstance != null && LocalPlayer.GameEntity.GetAvailableCurrencyForInteractiveStation(out currencySources) >= sellPrice);
					}
					else
					{
						this.CanPurchaseButNoRoom = (cannotAcquireReason == CannotAcquireReason.NoRoom);
						if (this.m_deathIcon)
						{
							this.m_deathIcon.gameObject.SetActive(cannotAcquireReason == CannotAcquireReason.Dead);
						}
					}
				}
				else
				{
					string text;
					canPurchase = this.m_sellable.EntityCanAcquire(LocalPlayer.GameEntity, out text);
				}
				this.CanPurchase = canPurchase;
				return;
			}
			this.CanPurchase = false;
		}

		// Token: 0x060047CB RID: 18379 RVA: 0x00070581 File Offset: 0x0006E781
		public void RefreshHoldingShiftForStackable()
		{
			if (this.m_sellable != null && this.m_sellable.Archetype is IStackable)
			{
				this.RefreshAvailability();
			}
		}

		// Token: 0x060047CC RID: 18380 RVA: 0x000705A3 File Offset: 0x0006E7A3
		private bool IsHoldingShiftValid()
		{
			return this.m_controller && this.m_controller.HoldingShift && this.m_sellable.Archetype is IStackable;
		}

		// Token: 0x060047CD RID: 18381 RVA: 0x001A7DC4 File Offset: 0x001A5FC4
		private void RefreshLabel()
		{
			if (this.m_sellable == null)
			{
				this.m_label.ZStringSetText("");
				return;
			}
			string text = this.m_sellable.Archetype.DisplayName;
			Color color;
			if (this.m_sellable.Archetype.TryGetItemCategoryColor(ItemCategory.ColorFlags.DisplayName, out color))
			{
				text = text.Color(color);
			}
			if (this.IsHoldingShiftValid())
			{
				this.m_label.SetTextFormat("{0} <size=75%>(x{1})</size>", text, 20);
				return;
			}
			if (this.m_sellable.Archetype.ArchetypeHasCharges())
			{
				this.m_label.SetTextFormat("{0} <size=75%>({1} charges)</size>", text, 100);
				return;
			}
			this.m_label.ZStringSetText(text);
		}

		// Token: 0x060047CE RID: 18382 RVA: 0x001A7E68 File Offset: 0x001A6068
		private ulong GetSellPrice()
		{
			if (this.m_sellable == null)
			{
				return 0UL;
			}
			InteractiveMerchant interactiveMerchant = (ClientGameManager.UIManager && ClientGameManager.UIManager.MerchantUI) ? ClientGameManager.UIManager.MerchantUI.Interactive : null;
			ulong num = this.UseEventCurrency() ? this.m_sellable.GetEventCost(LocalPlayer.GameEntity) : this.m_sellable.GetSellPrice(LocalPlayer.GameEntity);
			if (interactiveMerchant && (interactiveMerchant.CostMultiplier < 1f || interactiveMerchant.CostMultiplier > 1f))
			{
				num = (ulong)((long)Mathf.FloorToInt(num * interactiveMerchant.CostMultiplier));
			}
			if (this.IsHoldingShiftValid())
			{
				num *= 20UL;
			}
			return num;
		}

		// Token: 0x060047CF RID: 18383 RVA: 0x000705D4 File Offset: 0x0006E7D4
		private bool UseEventCurrency()
		{
			return ClientGameManager.UIManager && ClientGameManager.UIManager.MerchantUI && ClientGameManager.UIManager.MerchantUI.UseEventCurrency();
		}

		// Token: 0x060047D0 RID: 18384 RVA: 0x00070604 File Offset: 0x0006E804
		void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
		{
			this.m_chatWasFocusedOnPointerDown = UIManager.IsChatActive;
		}

		// Token: 0x060047D1 RID: 18385 RVA: 0x001A7F20 File Offset: 0x001A6120
		void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
		{
			if (this.m_mouseHover && this.m_chatWasFocusedOnPointerDown && this.m_sellable != null && this.m_sellable.Archetype && eventData.button == PointerEventData.InputButton.Left && ClientGameManager.InputManager != null && ClientGameManager.InputManager.HoldingShift && ClientGameManager.UIManager && !ClientGameManager.UIManager.IsDragging)
			{
				UIManager.ActiveChatInput.AddArchetypeLink(this.m_sellable.Archetype);
			}
			this.m_chatWasFocusedOnPointerDown = false;
		}

		// Token: 0x060047D2 RID: 18386 RVA: 0x001A7FA8 File Offset: 0x001A61A8
		void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
		{
			if (eventData.button == PointerEventData.InputButton.Right && this.m_sellable != null && this.CanPurchase)
			{
				int quantity = (ClientGameManager.InputManager.HoldingShift && this.m_sellable.Archetype is IStackable) ? 20 : 1;
				LocalPlayer.NetworkEntity.PlayerRpcHandler.MerchantPurchaseRequest(this.m_sellable.Archetype.Id, (uint)quantity);
			}
		}

		// Token: 0x060047D3 RID: 18387 RVA: 0x00070611 File Offset: 0x0006E811
		void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
		{
			this.m_mouseHover = true;
			if (this.CanPurchase)
			{
				this.m_hover.enabled = true;
			}
		}

		// Token: 0x060047D4 RID: 18388 RVA: 0x0007062E File Offset: 0x0006E82E
		void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
		{
			this.m_mouseHover = false;
			this.m_hover.enabled = false;
		}

		// Token: 0x060047D5 RID: 18389 RVA: 0x001A8014 File Offset: 0x001A6214
		internal ITooltipParameter GetTooltipParameter()
		{
			if (this.m_sellable == null || this.m_sellable.Archetype == null)
			{
				return null;
			}
			string additionalText = string.Empty;
			if (this.IsHoldingShiftValid())
			{
				additionalText = "x" + 20.ToString();
			}
			else if (this.m_sellable.Archetype.ArchetypeHasCharges())
			{
				additionalText = "Charges: " + 100.ToString();
			}
			return new ArchetypeTooltipParameter
			{
				Archetype = this.m_sellable.Archetype,
				AtMerchant = true,
				AdditionalText = additionalText
			};
		}

		// Token: 0x17000FEA RID: 4074
		// (get) Token: 0x060047D6 RID: 18390 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000FEB RID: 4075
		// (get) Token: 0x060047D7 RID: 18391 RVA: 0x00070643 File Offset: 0x0006E843
		internal CursorType CursorType
		{
			get
			{
				if (!this.CanPurchase)
				{
					return CursorType.MerchantCursorInactive;
				}
				return CursorType.MerchantCursor;
			}
		}

		// Token: 0x17000FEC RID: 4076
		// (get) Token: 0x060047D8 RID: 18392 RVA: 0x00070652 File Offset: 0x0006E852
		CursorType ICursor.Type
		{
			get
			{
				return this.CursorType;
			}
		}

		// Token: 0x060047DA RID: 18394 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04004358 RID: 17240
		private const int kShiftSellMultiplier = 20;

		// Token: 0x04004359 RID: 17241
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x0400435A RID: 17242
		[SerializeField]
		private ArchetypeIconUI m_archetypeIcon;

		// Token: 0x0400435B RID: 17243
		[SerializeField]
		private CurrencyDisplayPanelUI m_currency;

		// Token: 0x0400435C RID: 17244
		[SerializeField]
		private TextMeshProUGUI m_label;

		// Token: 0x0400435D RID: 17245
		[SerializeField]
		private Image m_hover;

		// Token: 0x0400435E RID: 17246
		[SerializeField]
		private Image m_disabledPanel;

		// Token: 0x0400435F RID: 17247
		[SerializeField]
		private Image m_deathIcon;

		// Token: 0x04004360 RID: 17248
		private MerchantForSaleList m_controller;

		// Token: 0x04004361 RID: 17249
		private IMerchantInventory m_sellable;

		// Token: 0x04004362 RID: 17250
		private bool m_canPurchase;

		// Token: 0x04004364 RID: 17252
		private bool m_mouseHover;

		// Token: 0x04004365 RID: 17253
		private bool m_chatWasFocusedOnPointerDown;
	}
}

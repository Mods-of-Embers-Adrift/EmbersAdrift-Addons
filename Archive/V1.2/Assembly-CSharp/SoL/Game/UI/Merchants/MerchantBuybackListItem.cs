using System;
using Cysharp.Text;
using SoL.Game.Interactives;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Containers;
using SoL.Networking.Database;
using SoL.UI;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SoL.Game.UI.Merchants
{
	// Token: 0x0200096B RID: 2411
	public class MerchantBuybackListItem : MonoBehaviour, ICursor, IInteractiveBase, IPointerClickHandler, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
	{
		// Token: 0x17000FDE RID: 4062
		// (get) Token: 0x0600478F RID: 18319 RVA: 0x000702D9 File Offset: 0x0006E4D9
		// (set) Token: 0x06004790 RID: 18320 RVA: 0x000702E1 File Offset: 0x0006E4E1
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

		// Token: 0x17000FDF RID: 4063
		// (get) Token: 0x06004791 RID: 18321 RVA: 0x0007031F File Offset: 0x0006E51F
		// (set) Token: 0x06004792 RID: 18322 RVA: 0x00070327 File Offset: 0x0006E527
		public bool CanPurchaseButNoRoom { get; private set; }

		// Token: 0x06004793 RID: 18323 RVA: 0x00070330 File Offset: 0x0006E530
		private void Awake()
		{
			this.m_currency.InitCoinDisplay(false, true, true, false);
		}

		// Token: 0x06004794 RID: 18324 RVA: 0x00070341 File Offset: 0x0006E541
		private void Update()
		{
			if (this.m_controller && this.m_controller.IsVisible)
			{
				this.RefreshFill();
			}
		}

		// Token: 0x06004795 RID: 18325 RVA: 0x001A74C0 File Offset: 0x001A56C0
		public void InitItem(MerchantBuybackList controller, MerchantBuybackItem buybackItem)
		{
			this.m_controller = controller;
			this.m_buybackItem = buybackItem;
			this.m_sellable = buybackItem;
			ArchetypeIconUI archetypeIcon = this.m_archetypeIcon;
			IMerchantInventory sellable = this.m_sellable;
			BaseArchetype archetype = (sellable != null) ? sellable.Archetype : null;
			IMerchantInventory sellable2 = this.m_sellable;
			archetypeIcon.SetIcon(archetype, (sellable2 != null) ? new Color?(sellable2.Archetype.IconTint) : null);
			this.RefreshAvailability();
			this.RefreshFill();
		}

		// Token: 0x06004796 RID: 18326 RVA: 0x001A7530 File Offset: 0x001A5730
		private void RefreshFill()
		{
			float num;
			if (this.m_timeRemainingFill && this.m_controller && this.TryGetTimeRemaining(out num))
			{
				int num2 = (this.m_controller.MerchantType == MerchantType.Standard) ? 3600 : 7200;
				float fillAmount = num / (float)num2;
				this.m_timeRemainingFill.fillAmount = fillAmount;
			}
		}

		// Token: 0x06004797 RID: 18327 RVA: 0x001A758C File Offset: 0x001A578C
		public void RefreshAvailability()
		{
			ulong sellPrice = this.GetSellPrice();
			this.m_currency.UpdateCoin(sellPrice);
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

		// Token: 0x06004798 RID: 18328 RVA: 0x001A7678 File Offset: 0x001A5878
		private void RefreshLabel()
		{
			if (this.m_buybackItem == null || this.m_buybackItem.Instance == null || this.m_buybackItem.Instance.Archetype == null)
			{
				this.m_label.ZStringSetText("");
				return;
			}
			string text = this.m_buybackItem.Instance.Archetype.GetModifiedDisplayName(this.m_buybackItem.Instance);
			Color color;
			if (this.m_buybackItem.Instance.Archetype.TryGetItemCategoryColor(ItemCategory.ColorFlags.DisplayName, out color))
			{
				text = text.Color(color);
			}
			if (this.m_buybackItem.Instance.IsItem && this.m_buybackItem.Instance.ItemData.Quantity != null)
			{
				this.m_label.SetTextFormat("{0} <size=75%>x{1}</size>", text, this.m_buybackItem.Instance.ItemData.Quantity.Value);
				return;
			}
			this.m_label.ZStringSetText(text);
		}

		// Token: 0x06004799 RID: 18329 RVA: 0x00070363 File Offset: 0x0006E563
		private ulong GetSellPrice()
		{
			if (this.m_sellable == null)
			{
				return 0UL;
			}
			return this.m_sellable.GetSellPrice(LocalPlayer.GameEntity);
		}

		// Token: 0x0600479A RID: 18330 RVA: 0x001A7774 File Offset: 0x001A5974
		private bool TryGetTimeRemaining(out float timeRemaining)
		{
			timeRemaining = 0f;
			if (this.m_buybackItem == null || this.m_sellable == null || !this.m_sellable.Archetype)
			{
				return false;
			}
			DateTime serverCorrectedDateTimeUtc = GameTimeReplicator.GetServerCorrectedDateTimeUtc();
			timeRemaining = Mathf.Clamp((float)(this.m_buybackItem.ExpirationTime - serverCorrectedDateTimeUtc).TotalSeconds, 0f, float.MaxValue);
			return true;
		}

		// Token: 0x0600479B RID: 18331 RVA: 0x0004475B File Offset: 0x0004295B
		void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
		{
		}

		// Token: 0x0600479C RID: 18332 RVA: 0x0004475B File Offset: 0x0004295B
		void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
		{
		}

		// Token: 0x0600479D RID: 18333 RVA: 0x001A77E0 File Offset: 0x001A59E0
		void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
		{
			if (eventData.button == PointerEventData.InputButton.Right && this.m_buybackItem != null && this.m_buybackItem.Instance != null && !this.m_buybackItem.Instance.InstanceId.IsEmpty && this.CanPurchase)
			{
				LocalPlayer.NetworkEntity.PlayerRpcHandler.MerchantBuybackRequest(this.m_buybackItem.Instance.InstanceId);
			}
		}

		// Token: 0x0600479E RID: 18334 RVA: 0x00070380 File Offset: 0x0006E580
		void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
		{
			if (this.CanPurchase)
			{
				this.m_hover.enabled = true;
			}
		}

		// Token: 0x0600479F RID: 18335 RVA: 0x00070396 File Offset: 0x0006E596
		void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
		{
			this.m_hover.enabled = false;
		}

		// Token: 0x060047A0 RID: 18336 RVA: 0x001A784C File Offset: 0x001A5A4C
		internal ITooltipParameter GetTooltipParameter()
		{
			float value;
			if (this.TryGetTimeRemaining(out value))
			{
				string additionalText = ZString.Format<string>("<color=\"red\">Buyback expires in: {0}</color>", value.FormattedTime(0));
				return new ArchetypeTooltipParameter
				{
					Instance = this.m_buybackItem.Instance,
					AtMerchant = false,
					AdditionalText = additionalText
				};
			}
			return null;
		}

		// Token: 0x17000FE0 RID: 4064
		// (get) Token: 0x060047A1 RID: 18337 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000FE1 RID: 4065
		// (get) Token: 0x060047A2 RID: 18338 RVA: 0x000703A4 File Offset: 0x0006E5A4
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

		// Token: 0x17000FE2 RID: 4066
		// (get) Token: 0x060047A3 RID: 18339 RVA: 0x000703B3 File Offset: 0x0006E5B3
		CursorType ICursor.Type
		{
			get
			{
				return this.CursorType;
			}
		}

		// Token: 0x060047A5 RID: 18341 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04004344 RID: 17220
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x04004345 RID: 17221
		[SerializeField]
		private ArchetypeIconUI m_archetypeIcon;

		// Token: 0x04004346 RID: 17222
		[SerializeField]
		private CurrencyDisplayPanelUI m_currency;

		// Token: 0x04004347 RID: 17223
		[SerializeField]
		private TextMeshProUGUI m_label;

		// Token: 0x04004348 RID: 17224
		[SerializeField]
		private Image m_hover;

		// Token: 0x04004349 RID: 17225
		[SerializeField]
		private Image m_disabledPanel;

		// Token: 0x0400434A RID: 17226
		[SerializeField]
		private Image m_deathIcon;

		// Token: 0x0400434B RID: 17227
		[SerializeField]
		private Image m_timeRemainingFill;

		// Token: 0x0400434C RID: 17228
		private MerchantBuybackList m_controller;

		// Token: 0x0400434D RID: 17229
		private MerchantBuybackItem m_buybackItem;

		// Token: 0x0400434E RID: 17230
		private IMerchantInventory m_sellable;

		// Token: 0x0400434F RID: 17231
		private bool m_canPurchase;
	}
}

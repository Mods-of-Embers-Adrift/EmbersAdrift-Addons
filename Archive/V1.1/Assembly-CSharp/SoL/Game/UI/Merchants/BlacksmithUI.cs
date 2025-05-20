using System;
using SoL.Game.Interactives;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Containers;
using SoL.Managers;
using SoL.UI;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;
using UnityEngine.Events;

namespace SoL.Game.UI.Merchants
{
	// Token: 0x02000965 RID: 2405
	public class BlacksmithUI : BaseMerchantUI<InteractiveBlacksmith>
	{
		// Token: 0x17000FD9 RID: 4057
		// (get) Token: 0x06004756 RID: 18262 RVA: 0x0007014A File Offset: 0x0006E34A
		protected override ContainerType m_containerType
		{
			get
			{
				return ContainerType.BlacksmithOutgoing;
			}
		}

		// Token: 0x06004757 RID: 18263 RVA: 0x001A68B4 File Offset: 0x001A4AB4
		protected override void Awake()
		{
			base.Awake();
			this.m_repairButton.onClick.AddListener(new UnityAction(this.RepairButtonClicked));
			this.m_repairEquipped.onClick.AddListener(new UnityAction(this.RepairEquippedClicked));
			this.m_repairBag.onClick.AddListener(new UnityAction(this.RepairBagClicked));
			base.UIWindow.ShowCalled += this.WindowShown;
			base.UIWindow.HideCalled += this.WindowHidden;
		}

		// Token: 0x06004758 RID: 18264 RVA: 0x001A694C File Offset: 0x001A4B4C
		protected override void OnDestroy()
		{
			base.OnDestroy();
			this.m_repairButton.onClick.RemoveListener(new UnityAction(this.RepairButtonClicked));
			this.m_repairEquipped.onClick.RemoveListener(new UnityAction(this.RepairEquippedClicked));
			this.m_repairBag.onClick.RemoveListener(new UnityAction(this.RepairBagClicked));
			base.UIWindow.ShowCalled -= this.WindowShown;
			base.UIWindow.HideCalled -= this.WindowHidden;
			this.ExitRepairMode();
		}

		// Token: 0x06004759 RID: 18265 RVA: 0x001A69E8 File Offset: 0x001A4BE8
		private ulong GetLocalPlayerCurrency()
		{
			if (!LocalPlayer.GameEntity)
			{
				return 0UL;
			}
			CurrencySources currencySources;
			return LocalPlayer.GameEntity.GetAvailableCurrencyForInteractiveStation(out currencySources);
		}

		// Token: 0x0600475A RID: 18266 RVA: 0x001A6A10 File Offset: 0x001A4C10
		private void WindowShown()
		{
			base.RefreshAvailableCurrency();
			this.RefreshEquipmentCost();
			this.RefreshBagCost();
			if (!this.m_subscribed)
			{
				LocalPlayer.GameEntity.CollectionController.Inventory.ItemRepaired += this.InventoryOnItemRepaired;
				LocalPlayer.GameEntity.CollectionController.Inventory.CurrencyChanged += this.CurrencyChanged;
				LocalPlayer.GameEntity.CollectionController.Inventory.ContentsChanged += this.InventoryOnContentsChanged;
				LocalPlayer.GameEntity.CollectionController.Equipment.ItemRepaired += this.EquipmentOnItemRepaired;
				LocalPlayer.GameEntity.CollectionController.Equipment.ContentsChanged += this.EquipmentOnContentsChanged;
				LocalPlayer.GameEntity.CollectionController.PersonalBank.CurrencyChanged += this.CurrencyChanged;
				LocalPlayer.GameEntity.CharacterData.CharacterFlags.Changed += this.CharacterFlagsOnChanged;
				this.m_subscribed = true;
			}
		}

		// Token: 0x0600475B RID: 18267 RVA: 0x001A6B24 File Offset: 0x001A4D24
		private void WindowHidden()
		{
			if (this.m_subscribed)
			{
				LocalPlayer.GameEntity.CollectionController.Inventory.ItemRepaired -= this.InventoryOnItemRepaired;
				LocalPlayer.GameEntity.CollectionController.Inventory.CurrencyChanged -= this.CurrencyChanged;
				LocalPlayer.GameEntity.CollectionController.Inventory.ContentsChanged -= this.InventoryOnContentsChanged;
				LocalPlayer.GameEntity.CollectionController.Equipment.ItemRepaired -= this.EquipmentOnItemRepaired;
				LocalPlayer.GameEntity.CollectionController.Equipment.ContentsChanged -= this.EquipmentOnContentsChanged;
				LocalPlayer.GameEntity.CollectionController.PersonalBank.CurrencyChanged -= this.CurrencyChanged;
				LocalPlayer.GameEntity.CharacterData.CharacterFlags.Changed -= this.CharacterFlagsOnChanged;
				this.m_subscribed = false;
			}
		}

		// Token: 0x0600475C RID: 18268 RVA: 0x0007014E File Offset: 0x0006E34E
		private void CurrencyChanged(ulong obj)
		{
			this.RefreshAll();
		}

		// Token: 0x0600475D RID: 18269 RVA: 0x00070156 File Offset: 0x0006E356
		private void InventoryOnContentsChanged()
		{
			this.RefreshBagCost();
		}

		// Token: 0x0600475E RID: 18270 RVA: 0x0007015E File Offset: 0x0006E35E
		private void EquipmentOnContentsChanged()
		{
			this.RefreshEquipmentCost();
		}

		// Token: 0x0600475F RID: 18271 RVA: 0x0007014E File Offset: 0x0006E34E
		private void CharacterFlagsOnChanged(PlayerFlags obj)
		{
			this.RefreshAll();
		}

		// Token: 0x06004760 RID: 18272 RVA: 0x00070166 File Offset: 0x0006E366
		private void RefreshAll()
		{
			base.RefreshAvailableCurrency();
			this.RefreshBagCost();
			this.RefreshEquipmentCost();
		}

		// Token: 0x06004761 RID: 18273 RVA: 0x00045BCA File Offset: 0x00043DCA
		protected override bool ButtonClickedInternal()
		{
			return false;
		}

		// Token: 0x06004762 RID: 18274 RVA: 0x0007017A File Offset: 0x0006E37A
		protected override void LeavingWindow()
		{
			base.LeavingWindow();
			this.ExitRepairMode();
		}

		// Token: 0x06004763 RID: 18275 RVA: 0x00070188 File Offset: 0x0006E388
		private void RepairButtonClicked()
		{
			CursorManager.ToggleGameMode(CursorGameMode.Repair);
		}

		// Token: 0x06004764 RID: 18276 RVA: 0x00070190 File Offset: 0x0006E390
		private void ExitRepairMode()
		{
			CursorManager.ExitGameMode(CursorGameMode.Repair);
		}

		// Token: 0x06004765 RID: 18277 RVA: 0x00070198 File Offset: 0x0006E398
		private void RepairBagClicked()
		{
			LocalPlayer.GameEntity.NetworkEntity.PlayerRpcHandler.BlacksmithContainerRepairRequest(ContainerType.Inventory);
			this.m_repairBag.interactable = false;
		}

		// Token: 0x06004766 RID: 18278 RVA: 0x000701BB File Offset: 0x0006E3BB
		private void RepairEquippedClicked()
		{
			LocalPlayer.GameEntity.NetworkEntity.PlayerRpcHandler.BlacksmithContainerRepairRequest(ContainerType.Equipment);
			this.m_repairEquipped.interactable = false;
		}

		// Token: 0x06004767 RID: 18279 RVA: 0x0007015E File Offset: 0x0006E35E
		private void EquipmentOnItemRepaired()
		{
			this.RefreshEquipmentCost();
		}

		// Token: 0x06004768 RID: 18280 RVA: 0x00070156 File Offset: 0x0006E356
		private void InventoryOnItemRepaired()
		{
			this.RefreshBagCost();
		}

		// Token: 0x06004769 RID: 18281 RVA: 0x001A6C24 File Offset: 0x001A4E24
		private void RefreshEquipmentCost()
		{
			ulong localPlayerCurrency = this.GetLocalPlayerCurrency();
			ulong costForContainer = this.GetCostForContainer(LocalPlayer.GameEntity.CollectionController.Equipment);
			this.m_equippedCurrency.UpdateCoin(costForContainer);
			this.m_repairEquipped.interactable = (costForContainer > 0UL && localPlayerCurrency >= costForContainer);
		}

		// Token: 0x0600476A RID: 18282 RVA: 0x001A6C74 File Offset: 0x001A4E74
		private void RefreshBagCost()
		{
			ulong localPlayerCurrency = this.GetLocalPlayerCurrency();
			ulong costForContainer = this.GetCostForContainer(LocalPlayer.GameEntity.CollectionController.Inventory);
			this.m_bagCurrency.UpdateCoin(costForContainer);
			this.m_repairBag.interactable = (costForContainer > 0UL && localPlayerCurrency >= costForContainer && !LocalPlayer.GameEntity.IsMissingBag);
		}

		// Token: 0x0600476B RID: 18283 RVA: 0x001A6CD0 File Offset: 0x001A4ED0
		private ulong GetCostForContainer(ContainerInstance containerInstance)
		{
			ulong num = 0UL;
			if (containerInstance == null)
			{
				return num;
			}
			for (int i = 0; i < containerInstance.Count; i++)
			{
				ArchetypeInstance instanceForListIndex = containerInstance.GetInstanceForListIndex(i);
				num += (ulong)instanceForListIndex.GetRepairCost();
			}
			return num;
		}

		// Token: 0x04004332 RID: 17202
		[SerializeField]
		private SolButton m_repairButton;

		// Token: 0x04004333 RID: 17203
		[SerializeField]
		private SolButton m_repairEquipped;

		// Token: 0x04004334 RID: 17204
		[SerializeField]
		private CurrencyDisplayPanelUI m_equippedCurrency;

		// Token: 0x04004335 RID: 17205
		[SerializeField]
		private SolButton m_repairBag;

		// Token: 0x04004336 RID: 17206
		[SerializeField]
		private CurrencyDisplayPanelUI m_bagCurrency;

		// Token: 0x04004337 RID: 17207
		private bool m_subscribed;
	}
}

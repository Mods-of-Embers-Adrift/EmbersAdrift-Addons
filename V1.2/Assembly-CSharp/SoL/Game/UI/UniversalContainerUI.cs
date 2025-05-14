using System;
using SoL.Game.Audio;
using SoL.Game.Interactives;
using SoL.Game.Messages;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Containers;
using SoL.Game.Settings;
using SoL.Game.Transactions;
using SoL.Game.UI.Archetypes;
using SoL.Managers;
using SoL.UI;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace SoL.Game.UI
{
	// Token: 0x020008EC RID: 2284
	public class UniversalContainerUI : ContainerUI<int, InventorySlotUI>
	{
		// Token: 0x17000F36 RID: 3894
		// (get) Token: 0x060042F4 RID: 17140 RVA: 0x0006D2B8 File Offset: 0x0006B4B8
		internal ContainerInstance Instance
		{
			get
			{
				return this.m_container;
			}
		}

		// Token: 0x060042F5 RID: 17141 RVA: 0x00193AC8 File Offset: 0x00191CC8
		protected override void Awake()
		{
			base.Awake();
			if (!this.m_slotsInitialized && this.m_slotElements != null)
			{
				for (int i = 0; i < this.m_slotElements.Length; i++)
				{
					this.m_slotElements[i].gameObject.SetActive(false);
				}
			}
		}

		// Token: 0x060042F6 RID: 17142 RVA: 0x00193B14 File Offset: 0x00191D14
		protected override void OnDestroy()
		{
			base.OnDestroy();
			if (this.m_coinPanelToggle != null && this.m_container != null && this.m_container.ContainerType.HasCurrency() && this.m_currencyPanel != null)
			{
				this.m_container.CurrencyChanged -= this.ContainerOnCurrencyChanged;
			}
			if (this.m_purchaseExpansionUi && this.m_uiWindow)
			{
				this.m_uiWindow.ShowCalled -= this.RefreshPurchaseExpansionUi;
			}
		}

		// Token: 0x060042F7 RID: 17143 RVA: 0x00193BA8 File Offset: 0x00191DA8
		protected override void InitializeSlots()
		{
			if (this.m_container == null)
			{
				return;
			}
			if (this.m_slots == null)
			{
				this.m_slots = new DictionaryList<int, InventorySlotUI>(false);
			}
			else
			{
				this.m_slots.Clear();
			}
			if (this.m_coinPanelToggle != null)
			{
				bool flag = this.m_container.ContainerType.HasCurrency();
				this.m_coinPanelToggle.Toggle(flag);
				if (this.m_currencyPanel != null)
				{
					this.m_currencyPanel.InitCoinDisplay(this.m_container.ContainerType);
					this.m_currencyPanel.Container = this.m_container;
					if (flag)
					{
						this.m_currencyPanel.UpdateCoin(this.m_container.Currency);
						this.m_container.CurrencyChanged += this.ContainerOnCurrencyChanged;
					}
				}
			}
			if (this.m_deathIconUI != null)
			{
				this.m_deathIconUI.gameObject.SetActive(this.m_container.ContainerType.LockedInDeath());
			}
			BankProfile bankProfile = null;
			int num = this.m_container.GetMaxCapacity();
			ContainerType containerType = this.m_container.ContainerType;
			switch (containerType)
			{
			case ContainerType.Inventory:
			case ContainerType.Gathering:
				this.m_gridLayoutGroup.constraintCount = 6;
				this.m_gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
				goto IL_386;
			case (ContainerType)3:
			case ContainerType.ReagentPouch:
				break;
			case ContainerType.Pouch:
				goto IL_386;
			case ContainerType.PersonalBank:
				if (GlobalSettings.Values == null || GlobalSettings.Values.Player == null || GlobalSettings.Values.Player.PersonalBankProfile == null)
				{
					throw new ArgumentException("No container profile for personal bank!");
				}
				bankProfile = GlobalSettings.Values.Player.PersonalBankProfile;
				this.m_gridLayoutGroup.constraint = bankProfile.Constraint;
				this.m_gridLayoutGroup.constraintCount = bankProfile.ConstraintCount;
				goto IL_386;
			default:
				switch (containerType)
				{
				case ContainerType.Loot:
				{
					this.FlushLeftoverLoot();
					int recordCount = this.m_container.RecordCount;
					this.m_gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
					if (recordCount <= 6)
					{
						num = 6;
						this.m_gridLayoutGroup.constraintCount = 3;
						goto IL_386;
					}
					if (recordCount <= 9)
					{
						num = 9;
						this.m_gridLayoutGroup.constraintCount = 3;
						goto IL_386;
					}
					if (recordCount <= 12)
					{
						num = 12;
						this.m_gridLayoutGroup.constraintCount = 4;
						goto IL_386;
					}
					if (recordCount <= 16)
					{
						num = 16;
						this.m_gridLayoutGroup.constraintCount = 4;
						goto IL_386;
					}
					if (recordCount <= 25)
					{
						num = 25;
						this.m_gridLayoutGroup.constraintCount = 5;
						goto IL_386;
					}
					if (recordCount <= 30)
					{
						num = 30;
						this.m_gridLayoutGroup.constraintCount = 6;
						goto IL_386;
					}
					if (recordCount <= 36)
					{
						num = 36;
						this.m_gridLayoutGroup.constraintCount = 6;
						goto IL_386;
					}
					goto IL_386;
				}
				case ContainerType.TradeOutgoing:
				case ContainerType.TradeIncoming:
					this.m_gridLayoutGroup.constraintCount = 3;
					this.m_gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
					goto IL_386;
				case ContainerType.MerchantOutgoing:
					this.m_gridLayoutGroup.constraintCount = 6;
					this.m_gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
					goto IL_386;
				case ContainerType.BlacksmithOutgoing:
					this.m_gridLayoutGroup.constraintCount = 5;
					this.m_gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
					goto IL_386;
				default:
					if (containerType == ContainerType.Bank)
					{
						if (this.m_container.ContainerProfile == null || !this.m_container.ContainerProfile.TryGetAsType(out bankProfile))
						{
							throw new ArgumentException("No container profile for bank with ID: " + this.m_container.Id + "!");
						}
						this.m_gridLayoutGroup.constraint = bankProfile.Constraint;
						this.m_gridLayoutGroup.constraintCount = bankProfile.ConstraintCount;
						goto IL_386;
					}
					break;
				}
				break;
			}
			if (this.m_gridLayoutGroup != null)
			{
				this.m_gridLayoutGroup.constraint = GridLayoutGroup.Constraint.Flexible;
			}
			IL_386:
			this.SetupAudioClipCollections();
			this.SetHeaderText();
			this.SetContainerIcon();
			for (int i = 0; i < this.m_slotElements.Length; i++)
			{
				bool flag2 = i < num;
				this.m_slotElements[i].gameObject.SetActive(flag2);
				if (flag2)
				{
					this.m_slotElements[i].Initialize(this, i);
					this.m_slots.Add(i, this.m_slotElements[i]);
				}
			}
			if (this.m_purchaseExpansionUi)
			{
				this.m_purchaseExpansionUi.Init(this, bankProfile);
				if (this.m_uiWindow)
				{
					this.m_uiWindow.ShowCalled += this.RefreshPurchaseExpansionUi;
				}
			}
			if (this.m_uiWindow)
			{
				DraggableUIWindow draggableUIWindow = this.m_uiWindow as DraggableUIWindow;
				if (draggableUIWindow != null)
				{
					draggableUIWindow.ClampToScreen(true);
				}
			}
			this.m_slotsInitialized = true;
		}

		// Token: 0x060042F8 RID: 17144 RVA: 0x00194014 File Offset: 0x00192214
		private void SetupAudioClipCollections()
		{
			if (this.m_uiWindow == null)
			{
				return;
			}
			for (int i = 0; i < this.m_audioClipCollections.Length; i++)
			{
				if (this.m_audioClipCollections[i] != null && this.m_audioClipCollections[i].Collection != null && this.m_audioClipCollections[i].Type != ContainerType.None && this.m_audioClipCollections[i].Type == this.m_container.ContainerType)
				{
					this.m_uiWindow.SetAudioClipCollection(true, this.m_audioClipCollections[i].Collection);
					this.m_uiWindow.SetAudioClipCollection(false, this.m_audioClipCollections[i].Collection);
					return;
				}
			}
		}

		// Token: 0x060042F9 RID: 17145 RVA: 0x0006D2C0 File Offset: 0x0006B4C0
		protected virtual void ContainerOnCurrencyChanged(ulong obj)
		{
			this.m_currencyPanel.UpdateCoin(obj);
			this.RefreshPurchaseExpansionUi();
		}

		// Token: 0x060042FA RID: 17146 RVA: 0x0006D2D4 File Offset: 0x0006B4D4
		private void RefreshPurchaseExpansionUi()
		{
			if (this.m_purchaseExpansionUi)
			{
				this.m_purchaseExpansionUi.RefreshUI();
			}
		}

		// Token: 0x060042FB RID: 17147 RVA: 0x0006D2EE File Offset: 0x0006B4EE
		protected void SetHeaderText()
		{
			if (this.m_headerText != null)
			{
				this.m_headerText.text = this.m_container.GetHeaderString();
			}
		}

		// Token: 0x060042FC RID: 17148 RVA: 0x001940C4 File Offset: 0x001922C4
		private void SetContainerIcon()
		{
			if (this.m_containerIcon)
			{
				Sprite overrideSprite;
				if (this.m_container != null && GlobalSettings.Values && GlobalSettings.Values.UI != null && GlobalSettings.Values.UI.TryGetContainerIcon(this.m_container.ContainerType, out overrideSprite))
				{
					this.m_containerIcon.enabled = true;
					this.m_containerIcon.overrideSprite = overrideSprite;
					return;
				}
				this.m_containerIcon.enabled = false;
			}
		}

		// Token: 0x060042FD RID: 17149 RVA: 0x0006D314 File Offset: 0x0006B514
		protected override void RightInstanceClicked(PointerEventData eventData, ArchetypeInstance instance)
		{
			this.InteractWithInstance(instance);
		}

		// Token: 0x060042FE RID: 17150 RVA: 0x0006D314 File Offset: 0x0006B514
		protected override void LeftInstanceDoubleClicked(PointerEventData eventData, ArchetypeInstance instance)
		{
			this.InteractWithInstance(instance);
		}

		// Token: 0x060042FF RID: 17151 RVA: 0x00194144 File Offset: 0x00192344
		private bool TryMergeWithinContainer(ArchetypeInstance newInstance, ContainerInstance containerInstance, bool preventSubscriberSlots = false)
		{
			if (containerInstance.ContainerType == ContainerType.Pouch && !newInstance.Archetype.CanPlaceInPouch)
			{
				return false;
			}
			if (containerInstance.ContainerType == ContainerType.Gathering && !newInstance.Archetype.CanPlaceInGathering)
			{
				return false;
			}
			if (containerInstance.ContainerType.LockedInDeath() && LocalPlayer.GameEntity.IsMissingBag)
			{
				return false;
			}
			foreach (ArchetypeInstance archetypeInstance in containerInstance.Instances)
			{
				if (archetypeInstance.CanMergeWith(newInstance) && (!preventSubscriberSlots || archetypeInstance.ContainerInstance == null || !archetypeInstance.ContainerInstance.ContainerType.IsBank() || !archetypeInstance.ContainerInstance.IsSubscriberOnlySlot(archetypeInstance.Index)))
				{
					MergeRequest request = new MergeRequest
					{
						TransactionId = UniqueId.GenerateFromGuid(),
						SourceInstanceId = newInstance.InstanceId,
						TargetInstanceId = archetypeInstance.InstanceId,
						SourceContainer = this.m_container.Id,
						TargetContainer = containerInstance.Id,
						LocalSourceDisplayName = (newInstance.Archetype ? newInstance.Archetype.GetModifiedDisplayName(newInstance) : string.Empty),
						LocalSourceQuantity = ((newInstance.ItemData != null) ? newInstance.ItemData.Quantity : null)
					};
					LocalPlayer.NetworkEntity.PlayerRpcHandler.MergeRequest(request);
					this.PlayDragDropAudioForInstance(newInstance);
					return true;
				}
			}
			return false;
		}

		// Token: 0x06004300 RID: 17152 RVA: 0x001942E4 File Offset: 0x001924E4
		public void TriggerIndex(int index)
		{
			if (this.m_container == null || this.m_slotElements == null)
			{
				return;
			}
			if (this.m_container.ContainerType == ContainerType.ReagentPouch)
			{
				for (int i = 0; i < this.m_slotElements.Length; i++)
				{
					if (this.m_slotElements[i].Index == index)
					{
						this.m_slotElements[i].ToggleReagentToggle();
						return;
					}
				}
				return;
			}
			InventorySlotUI inventorySlotUI;
			if (this.m_slots != null && this.m_slots.TryGetValue(index, out inventorySlotUI) && inventorySlotUI.Instance != null)
			{
				this.InteractWithInstance(inventorySlotUI.Instance);
			}
		}

		// Token: 0x06004301 RID: 17153 RVA: 0x0006D31D File Offset: 0x0006B51D
		public void RefreshPurchaseContainerExpansionUI(bool success)
		{
			if (success)
			{
				this.InitializeSlots();
				return;
			}
			if (this.m_purchaseExpansionUi)
			{
				this.m_purchaseExpansionUi.RefreshUI();
			}
		}

		// Token: 0x06004302 RID: 17154 RVA: 0x00194370 File Offset: 0x00192570
		private bool TryGetBankContainer(out ContainerInstance primaryBankContainerInstance, out ContainerInstance secondaryBankContainerInstance)
		{
			primaryBankContainerInstance = null;
			secondaryBankContainerInstance = null;
			if (LocalPlayer.GameEntity.CollectionController.InteractiveStation != null && LocalPlayer.GameEntity.CollectionController.InteractiveStation.ContainerType.IsBank())
			{
				primaryBankContainerInstance = LocalPlayer.GameEntity.CollectionController.PersonalBank;
			}
			if (LocalPlayer.GameEntity.CollectionController.RemoteContainer != null && LocalPlayer.GameEntity.CollectionController.RemoteContainer.ContainerType.IsBank())
			{
				if (primaryBankContainerInstance == null)
				{
					primaryBankContainerInstance = LocalPlayer.GameEntity.CollectionController.RemoteContainer;
				}
				else
				{
					secondaryBankContainerInstance = LocalPlayer.GameEntity.CollectionController.RemoteContainer;
				}
			}
			return primaryBankContainerInstance != null;
		}

		// Token: 0x06004303 RID: 17155 RVA: 0x0006D341 File Offset: 0x0006B541
		private bool CanTransferToContainerInstance(ArchetypeInstance instance, ContainerInstance containerInstance, out int targetIndex)
		{
			targetIndex = -1;
			if (instance == null || containerInstance == null)
			{
				return false;
			}
			if (!containerInstance.HasRoom())
			{
				return false;
			}
			targetIndex = containerInstance.GetFirstAvailableIndex();
			return containerInstance.CanPlace(instance, targetIndex);
		}

		// Token: 0x06004304 RID: 17156 RVA: 0x00194420 File Offset: 0x00192620
		private void AttemptItemEquip(ArchetypeInstance instance, ContainerInstance equipContainerInstance, IEquipable eqItem)
		{
			if (ClientGameManager.UIManager.EquipmentStats.Locked)
			{
				UIManager.TriggerCannotPerform("In Combat Stance!");
				MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "Equipment locked! (in combat stance)");
				return;
			}
			if (instance.ItemData != null && instance.ItemData.Augment != null && instance.ItemData.Augment.AugmentItemRef && !instance.ItemData.Augment.AugmentItemRef.MeetsLevelRequirement(LocalPlayer.GameEntity))
			{
				UIManager.TriggerCannotPerform("Cannot Equip Item!");
				MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "Cannot equip this item!");
				return;
			}
			if (!eqItem.CanEquip(LocalPlayer.GameEntity))
			{
				UIManager.TriggerCannotPerform("Cannot Equip Item!");
				MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "Cannot equip this item!");
				return;
			}
			int num = -1;
			EquipmentSlot targetEquipmentSlot = eqItem.GetTargetEquipmentSlot(LocalPlayer.GameEntity);
			if (targetEquipmentSlot != EquipmentSlot.None)
			{
				num = (int)targetEquipmentSlot;
				if (!equipContainerInstance.CanPlace(instance, num))
				{
					return;
				}
			}
			ArchetypeInstance archetypeInstance;
			if (equipContainerInstance.TryGetInstanceForIndex(num, out archetypeInstance))
			{
				SwapRequest request = new SwapRequest
				{
					TransactionId = UniqueId.GenerateFromGuid(),
					InstanceIdA = instance.InstanceId,
					InstanceIdB = archetypeInstance.InstanceId,
					SourceContainerA = instance.ContainerInstance.Id,
					SourceContainerB = archetypeInstance.ContainerInstance.Id
				};
				LocalPlayer.NetworkEntity.PlayerRpcHandler.SwapRequest(request);
			}
			else
			{
				TransferRequest request2 = new TransferRequest
				{
					TransactionId = UniqueId.GenerateFromGuid(),
					InstanceId = instance.InstanceId,
					SourceContainer = this.m_container.Id,
					TargetContainer = ContainerType.Equipment.ToString(),
					TargetIndex = num,
					Instance = instance
				};
				LocalPlayer.NetworkEntity.PlayerRpcHandler.TransferRequest(request2);
			}
			this.PlayDragDropAudioForInstance(instance);
		}

		// Token: 0x06004305 RID: 17157 RVA: 0x0006D369 File Offset: 0x0006B569
		private void PlayDragDropAudioForInstance(ArchetypeInstance instance)
		{
			if (instance != null && instance.InstanceUI)
			{
				instance.InstanceUI.PlayDragDropAudio();
			}
		}

		// Token: 0x06004306 RID: 17158 RVA: 0x001945F0 File Offset: 0x001927F0
		private bool HandleRangedAmmo(ArchetypeInstance instance)
		{
			RangedAmmoItem rangedAmmoItem;
			if (instance == null || !instance.Archetype.TryGetAsType(out rangedAmmoItem) || LocalPlayer.GameEntity == null || LocalPlayer.GameEntity.CollectionController == null || LocalPlayer.GameEntity.CollectionController.Equipment == null)
			{
				return false;
			}
			ArchetypeInstance archetypeInstance;
			if (rangedAmmoItem.EntityHasMatchingAmmoEquipped(LocalPlayer.GameEntity, out archetypeInstance) && instance.CanMergeWith(archetypeInstance))
			{
				MergeRequest request = new MergeRequest
				{
					TransactionId = UniqueId.GenerateFromGuid(),
					SourceInstanceId = instance.InstanceId,
					TargetInstanceId = archetypeInstance.InstanceId,
					SourceContainer = this.m_container.Id,
					TargetContainer = LocalPlayer.GameEntity.CollectionController.Equipment.Id
				};
				LocalPlayer.NetworkEntity.PlayerRpcHandler.MergeRequest(request);
				this.PlayDragDropAudioForInstance(instance);
				return true;
			}
			EquipmentSlot targetIndex;
			if (rangedAmmoItem.EntityHasEmptyAmmoSlotForWeapon(LocalPlayer.GameEntity, out targetIndex))
			{
				TransferRequest request2 = new TransferRequest
				{
					TransactionId = UniqueId.GenerateFromGuid(),
					InstanceId = instance.InstanceId,
					SourceContainer = this.m_container.Id,
					TargetContainer = LocalPlayer.GameEntity.CollectionController.Equipment.Id,
					TargetIndex = (int)targetIndex,
					Instance = instance
				};
				LocalPlayer.NetworkEntity.PlayerRpcHandler.TransferRequest(request2);
				this.PlayDragDropAudioForInstance(instance);
				return true;
			}
			return false;
		}

		// Token: 0x06004307 RID: 17159 RVA: 0x0019475C File Offset: 0x0019295C
		private bool TryAddToBank(ArchetypeInstance instance)
		{
			ContainerInstance containerInstance;
			ContainerInstance containerInstance2;
			if (!this.TryGetBankContainer(out containerInstance, out containerInstance2))
			{
				return false;
			}
			bool flag = LocalPlayer.GameEntity && LocalPlayer.GameEntity.Subscriber;
			if (instance.Archetype is IStackable)
			{
				if (this.TryMergeWithinContainer(instance, containerInstance, !flag))
				{
					return true;
				}
				if (containerInstance2 != null && this.TryMergeWithinContainer(instance, containerInstance2, !flag))
				{
					return true;
				}
			}
			ContainerInstance containerInstance3 = containerInstance;
			int num;
			if (!containerInstance3.TryGetFirstAvailableIndex(LocalPlayer.GameEntity, out num))
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "No room!");
				return true;
			}
			int targetIndex = -1;
			if (containerInstance3.CanPlace(instance, targetIndex))
			{
				TransferRequest request = new TransferRequest
				{
					TransactionId = UniqueId.GenerateFromGuid(),
					InstanceId = instance.InstanceId,
					SourceContainer = this.m_container.ContainerType.ToString(),
					TargetContainer = containerInstance3.Id,
					TargetIndex = targetIndex,
					Instance = instance
				};
				LocalPlayer.NetworkEntity.PlayerRpcHandler.TransferRequest(request);
				this.PlayDragDropAudioForInstance(instance);
			}
			else
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "Cannot place!");
			}
			return true;
		}

		// Token: 0x06004308 RID: 17160 RVA: 0x00194884 File Offset: 0x00192A84
		protected override void InteractWithInstance(ArchetypeInstance instance)
		{
			base.InteractWithInstance(instance);
			if (instance == null || instance.Archetype == null)
			{
				return;
			}
			if (this.m_container.LockFlags.IsLocked())
			{
				return;
			}
			if (instance.IsItem && instance.ItemData.Locked)
			{
				return;
			}
			if (this.m_container.ContainerType == ContainerType.Bank && instance.ItemData.IsSoulbound && instance.ItemData.SoulboundPlayerId != LocalPlayer.GameEntity.CollectionController.Record.Id)
			{
				return;
			}
			ContainerType containerType = this.m_container.ContainerType;
			switch (containerType)
			{
			case ContainerType.Inventory:
			{
				if (this.TryAddToBank(instance))
				{
					return;
				}
				IEquipable eqItem;
				ContainerInstance equipContainerInstance;
				if (instance.Archetype.TryGetAsType(out eqItem) && LocalPlayer.GameEntity.CollectionController.TryGetInstance(ContainerType.Equipment, out equipContainerInstance))
				{
					this.AttemptItemEquip(instance, equipContainerInstance, eqItem);
					return;
				}
				IUtilityItem utilityItem;
				if (instance.Archetype.TryGetAsType(out utilityItem))
				{
					UtilityItemExtensions.InitializeUtilityItemMode(instance, utilityItem);
					return;
				}
				if (!instance.Archetype.CanPlaceInGathering)
				{
					this.AttemptToExecute(instance);
					return;
				}
				if (!this.TryMergeWithinContainer(instance, LocalPlayer.GameEntity.CollectionController.Gathering, false) && LocalPlayer.GameEntity.CollectionController.Gathering.HasRoom())
				{
					int targetIndex = -1;
					TransferRequest request = new TransferRequest
					{
						TransactionId = UniqueId.GenerateFromGuid(),
						InstanceId = instance.InstanceId,
						SourceContainer = this.m_container.Id,
						TargetContainer = LocalPlayer.GameEntity.CollectionController.Gathering.Id,
						TargetIndex = targetIndex,
						Instance = instance
					};
					LocalPlayer.NetworkEntity.PlayerRpcHandler.TransferRequest(request);
					return;
				}
				return;
			}
			case (ContainerType)3:
			case ContainerType.ReagentPouch:
				return;
			case ContainerType.Pouch:
			{
				IUtilityItem utilityItem2;
				if (!this.AttemptToExecute(instance) && this.m_container.ContainerType.AllowConsumableUse() && instance.Archetype.TryGetAsType(out utilityItem2))
				{
					UtilityItemExtensions.InitializeUtilityItemMode(instance, utilityItem2);
					return;
				}
				return;
			}
			case ContainerType.PersonalBank:
				break;
			case ContainerType.Gathering:
				this.TryAddToBank(instance);
				return;
			default:
				switch (containerType)
				{
				case ContainerType.Loot:
				case ContainerType.MerchantOutgoing:
				case ContainerType.BlacksmithOutgoing:
				case ContainerType.RuneCollector:
				case ContainerType.PostOutgoing:
				case ContainerType.AuctionOutgoing:
				case ContainerType.RefinementInput:
				case ContainerType.RefinementOutput:
					break;
				case ContainerType.TradeOutgoing:
				case ContainerType.TradeIncoming:
				case ContainerType.PostIncoming:
				case ContainerType.Inspection:
					return;
				default:
					if (containerType != ContainerType.Bank)
					{
						return;
					}
					break;
				}
				break;
			}
			if ((!(instance.Archetype is IStackable) || (!this.TryMergeWithinContainer(instance, LocalPlayer.GameEntity.CollectionController.ReagentPouch, false) && !this.TryMergeWithinContainer(instance, LocalPlayer.GameEntity.CollectionController.Pouch, false) && !this.TryMergeWithinContainer(instance, LocalPlayer.GameEntity.CollectionController.Gathering, false) && !this.TryMergeWithinContainer(instance, LocalPlayer.GameEntity.CollectionController.Inventory, false))) && !this.HandleRangedAmmo(instance))
			{
				IEquipable eqItem2;
				ContainerInstance equipContainerInstance2;
				if (this.m_container.ContainerType.IsBank() && LocalPlayer.GameEntity.IsMissingBag && instance.Archetype.TryGetAsType(out eqItem2) && LocalPlayer.GameEntity.CollectionController.TryGetInstance(ContainerType.Equipment, out equipContainerInstance2))
				{
					this.AttemptItemEquip(instance, equipContainerInstance2, eqItem2);
					return;
				}
				ContainerInstance containerInstance = null;
				ReagentItem reagentItem;
				if (instance.Archetype.TryGetAsType(out reagentItem) && LocalPlayer.GameEntity.CollectionController.ReagentPouch.HasRoom() && reagentItem.Type.IsCompatibleWithEntity(LocalPlayer.GameEntity))
				{
					containerInstance = LocalPlayer.GameEntity.CollectionController.ReagentPouch;
				}
				else if (instance.Archetype.CanPlaceInPouch && LocalPlayer.GameEntity.CollectionController.Pouch.HasRoom())
				{
					containerInstance = LocalPlayer.GameEntity.CollectionController.Pouch;
				}
				else if (instance.Archetype.CanPlaceInGathering && LocalPlayer.GameEntity.CollectionController.Gathering.HasRoom())
				{
					containerInstance = LocalPlayer.GameEntity.CollectionController.Gathering;
				}
				else if (LocalPlayer.GameEntity.IsMissingBag)
				{
					MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "Nowhere to put " + instance.Archetype.GetModifiedDisplayName(instance) + " because your bag is missing!");
				}
				else if (LocalPlayer.GameEntity.CollectionController.Inventory.HasRoom())
				{
					containerInstance = LocalPlayer.GameEntity.CollectionController.Inventory;
				}
				else
				{
					UIManager.TriggerCannotPerform("No Room!");
					MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "No room in your bag for " + instance.Archetype.GetModifiedDisplayName(instance) + "!");
				}
				if (containerInstance != null)
				{
					int targetIndex2 = -1;
					TransferRequest request2 = new TransferRequest
					{
						TransactionId = UniqueId.GenerateFromGuid(),
						InstanceId = instance.InstanceId,
						SourceContainer = this.m_container.Id,
						TargetContainer = containerInstance.Id,
						TargetIndex = targetIndex2,
						Instance = instance
					};
					LocalPlayer.NetworkEntity.PlayerRpcHandler.TransferRequest(request2);
					this.PlayDragDropAudioForInstance(instance);
				}
			}
		}

		// Token: 0x06004309 RID: 17161 RVA: 0x00194D7C File Offset: 0x00192F7C
		private bool AttemptToExecute(ArchetypeInstance instance)
		{
			ConsumableItem consumableItem;
			if (instance != null && instance.Archetype.TryGetAsType(out consumableItem) && this.m_container.ContainerType.AllowConsumableUse())
			{
				if (instance.InstanceUI != null && !instance.InstanceUI.Locked)
				{
					LocalPlayer.GameEntity.SkillsController.BeginExecution(instance);
				}
				return true;
			}
			return false;
		}

		// Token: 0x0600430A RID: 17162 RVA: 0x00194DDC File Offset: 0x00192FDC
		private void FlushLeftoverLoot()
		{
			if (this.m_slotElements == null || this.m_slotElements.Length == 0)
			{
				return;
			}
			int num = 0;
			for (int i = 0; i < this.m_slotElements.Length; i++)
			{
				if (!(this.m_slotElements[i] == null) && this.m_slotElements[i].gameObject)
				{
					ArchetypeInstanceUI componentInChildren = this.m_slotElements[i].gameObject.GetComponentInChildren<ArchetypeInstanceUI>();
					if (componentInChildren != null)
					{
						componentInChildren.ExternalOnDestroy();
						if (componentInChildren.Instance != null)
						{
							StaticPool<ArchetypeInstance>.ReturnToPool(componentInChildren.Instance);
						}
						UnityEngine.Object.Destroy(componentInChildren.gameObject);
						num++;
					}
				}
			}
			if (num > 0)
			{
				Debug.LogWarning("Flushed " + num.ToString() + " leftover loot InstanceUIs");
			}
		}

		// Token: 0x0600430B RID: 17163 RVA: 0x00194E98 File Offset: 0x00193098
		protected override bool TryGetContainerSlotUI(int index, out ContainerSlotUI slotUI)
		{
			slotUI = null;
			InventorySlotUI inventorySlotUI;
			if (this.m_slots.TryGetValue(index, out inventorySlotUI))
			{
				slotUI = inventorySlotUI;
			}
			return slotUI != null;
		}

		// Token: 0x04003FB3 RID: 16307
		[SerializeField]
		private TextMeshProUGUI m_headerText;

		// Token: 0x04003FB4 RID: 16308
		[SerializeField]
		private GridLayoutGroup m_gridLayoutGroup;

		// Token: 0x04003FB5 RID: 16309
		[SerializeField]
		private InventorySlotUI[] m_slotElements;

		// Token: 0x04003FB6 RID: 16310
		[SerializeField]
		private ToggleController m_coinPanelToggle;

		// Token: 0x04003FB7 RID: 16311
		[FormerlySerializedAs("m_coinPanel")]
		[SerializeField]
		private CurrencyDisplayPanelUI m_currencyPanel;

		// Token: 0x04003FB8 RID: 16312
		[SerializeField]
		private PurchaseContainerExpansionUI m_purchaseExpansionUi;

		// Token: 0x04003FB9 RID: 16313
		[SerializeField]
		private DeathIconUI m_deathIconUI;

		// Token: 0x04003FBA RID: 16314
		[SerializeField]
		private Image m_containerIcon;

		// Token: 0x04003FBB RID: 16315
		[SerializeField]
		private UniversalContainerUI.AudioClipForContainerType[] m_audioClipCollections;

		// Token: 0x04003FBC RID: 16316
		private bool m_slotsInitialized;

		// Token: 0x020008ED RID: 2285
		[Serializable]
		private class AudioClipForContainerType
		{
			// Token: 0x04003FBD RID: 16317
			public ContainerType Type;

			// Token: 0x04003FBE RID: 16318
			public AudioClipCollection Collection;
		}
	}
}

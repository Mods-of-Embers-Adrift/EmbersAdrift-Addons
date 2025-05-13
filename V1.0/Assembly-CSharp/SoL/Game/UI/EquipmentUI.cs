using System;
using System.Collections;
using System.Collections.Generic;
using SoL.Game.Crafting;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Containers;
using SoL.Game.Settings;
using SoL.Game.Transactions;
using SoL.Game.UI.Archetypes;
using SoL.Managers;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SoL.Game.UI
{
	// Token: 0x0200087F RID: 2175
	public class EquipmentUI : ContainerUI<EquipmentSlot, EquipmentSlotUI>, IBindingLabel
	{
		// Token: 0x17000EA3 RID: 3747
		// (get) Token: 0x06003F47 RID: 16199 RVA: 0x0004F9FB File Offset: 0x0004DBFB
		private IEnumerable GetColors
		{
			get
			{
				return SolOdinUtilities.GetColorValues();
			}
		}

		// Token: 0x06003F48 RID: 16200 RVA: 0x0006ACC8 File Offset: 0x00068EC8
		protected override void Awake()
		{
			base.Awake();
			ArchetypeInstanceUI.BeginDragEvent += this.ArchetypeInstanceUiOnBeginDragEvent;
			ArchetypeInstanceUI.EndDragEvent += this.ArchetypeInstanceUiOnEndDragEvent;
			BindingLabels.RegisterBinding(this, false);
		}

		// Token: 0x06003F49 RID: 16201 RVA: 0x001882D0 File Offset: 0x001864D0
		protected override void OnDestroy()
		{
			base.OnDestroy();
			if (LocalPlayer.GameEntity)
			{
				if (LocalPlayer.GameEntity.CharacterData)
				{
					LocalPlayer.GameEntity.CharacterData.HandConfigurationChanged -= this.EntityControllerOnSettingsChanged;
				}
				if (LocalPlayer.GameEntity.SkillsController)
				{
					LocalPlayer.GameEntity.SkillsController.PendingExecutionChanged -= this.SkillsControllerOnPendingExecutionChanged;
				}
			}
			if (this.m_container != null)
			{
				this.m_container.ContentsChanged -= this.EquipmentOnContentsChanged;
			}
			ArchetypeInstanceUI.BeginDragEvent -= this.ArchetypeInstanceUiOnBeginDragEvent;
			ArchetypeInstanceUI.EndDragEvent -= this.ArchetypeInstanceUiOnEndDragEvent;
			BindingLabels.DeregisterBinding(this);
		}

		// Token: 0x06003F4A RID: 16202 RVA: 0x00188390 File Offset: 0x00186590
		protected override void InitializeSlots()
		{
			this.m_slots = new DictionaryList<EquipmentSlot, EquipmentSlotUI>(default(EquipmentSlotComparer), false);
			for (int i = 0; i < this.m_slotSettings.Length; i++)
			{
				EquipmentSlotSetting equipmentSlotSetting = this.m_slotSettings[i];
				equipmentSlotSetting.UI.AssignSettings(equipmentSlotSetting, this.m_defaultBorderColor, this.m_armorBorderColor, true);
				equipmentSlotSetting.UI.Initialize(this, (int)equipmentSlotSetting.Type);
				this.m_slots.Add(equipmentSlotSetting.Type, equipmentSlotSetting.UI);
			}
			if (this.m_emberStoneSlotUi)
			{
				this.m_emberStoneSlotUi.Initialize(this);
			}
			this.EntityControllerOnSettingsChanged();
			LocalPlayer.GameEntity.CharacterData.HandConfigurationChanged += this.EntityControllerOnSettingsChanged;
			LocalPlayer.GameEntity.SkillsController.PendingExecutionChanged += this.SkillsControllerOnPendingExecutionChanged;
			this.m_container.ContentsChanged += this.EquipmentOnContentsChanged;
		}

		// Token: 0x06003F4B RID: 16203 RVA: 0x0006ACF9 File Offset: 0x00068EF9
		public override void PostInit()
		{
			base.PostInit();
			this.EquipmentOnContentsChanged();
		}

		// Token: 0x06003F4C RID: 16204 RVA: 0x00188488 File Offset: 0x00186688
		private void EntityControllerOnSettingsChanged()
		{
			bool flag = !LocalPlayer.GameEntity.CharacterData.MainHand_SecondaryActive;
			if (this.m_primaryBorder)
			{
				this.m_primaryBorder.enabled = flag;
			}
			if (this.m_secondaryBorder)
			{
				this.m_secondaryBorder.enabled = !flag;
			}
			this.RefreshAvailableCharges();
		}

		// Token: 0x06003F4D RID: 16205 RVA: 0x001884E4 File Offset: 0x001866E4
		protected override void LeftInstanceDoubleClicked(PointerEventData eventData, ArchetypeInstance instance)
		{
			if (instance.Archetype == null)
			{
				return;
			}
			ContainerType containerType = ContainerType.Inventory;
			if (ClientGameManager.UIManager.Inventory.Locked || base.Locked)
			{
				return;
			}
			if (!LocalPlayer.GameEntity.CollectionController.Inventory.HasRoom())
			{
				return;
			}
			TransferRequest request = new TransferRequest
			{
				TransactionId = UniqueId.GenerateFromGuid(),
				InstanceId = instance.InstanceId,
				SourceContainer = ContainerType.Equipment.ToString(),
				TargetContainer = containerType.ToString(),
				TargetIndex = -1,
				Instance = instance
			};
			LocalPlayer.NetworkEntity.PlayerRpcHandler.TransferRequest(request);
		}

		// Token: 0x06003F4E RID: 16206 RVA: 0x0006AD07 File Offset: 0x00068F07
		private void ArchetypeInstanceUiOnEndDragEvent(ArchetypeInstance obj)
		{
			this.ToggleHighlight(obj, false);
		}

		// Token: 0x06003F4F RID: 16207 RVA: 0x0006AD11 File Offset: 0x00068F11
		private void ArchetypeInstanceUiOnBeginDragEvent(ArchetypeInstance obj)
		{
			this.ToggleHighlight(obj, true);
		}

		// Token: 0x06003F50 RID: 16208 RVA: 0x001885A0 File Offset: 0x001867A0
		private void SkillsControllerOnPendingExecutionChanged(SkillsController.PendingExecution obj)
		{
			GatheringAbility gatheringAbility;
			if (obj.Active && obj.Instance != null && obj.Instance.Archetype.TryGetAsType(out gatheringAbility))
			{
				this.m_container.LockFlags |= ContainerLockFlags.Harvesting;
				return;
			}
			this.m_container.LockFlags &= ~ContainerLockFlags.Harvesting;
		}

		// Token: 0x06003F51 RID: 16209 RVA: 0x001885FC File Offset: 0x001867FC
		private void EquipmentOnContentsChanged()
		{
			if (this.m_primaryBackground)
			{
				if (this.m_defaultPrimaryBackgroundColor == null)
				{
					this.m_defaultPrimaryBackgroundColor = new Color?(this.m_primaryBackground.color);
				}
				Color color = this.HasCompatibleWeapons(true) ? this.m_defaultPrimaryBackgroundColor.Value : UIManager.RedColor;
				color.a = this.m_defaultPrimaryBackgroundColor.Value.a;
				this.m_primaryBackground.color = color;
			}
			if (this.m_secondaryBackground)
			{
				if (this.m_defaultSecondaryBackgroundColor == null)
				{
					this.m_defaultSecondaryBackgroundColor = new Color?(this.m_secondaryBackground.color);
				}
				Color color2 = this.HasCompatibleWeapons(false) ? this.m_defaultSecondaryBackgroundColor.Value : UIManager.RedColor;
				color2.a = this.m_defaultSecondaryBackgroundColor.Value.a;
				this.m_secondaryBackground.color = color2;
			}
		}

		// Token: 0x06003F52 RID: 16210 RVA: 0x001886E8 File Offset: 0x001868E8
		private bool HasCompatibleWeapons(bool primaryWeapon)
		{
			if (LocalPlayer.GameEntity && LocalPlayer.GameEntity.CharacterData && this.m_container != null)
			{
				IHandheldItem fallbackWeapon = GlobalSettings.Values.Combat.FallbackWeapon;
				EquipmentSlot index = primaryWeapon ? EquipmentSlot.PrimaryWeapon_MainHand : EquipmentSlot.SecondaryWeapon_MainHand;
				ArchetypeInstance archetypeInstance;
				IHandheldItem handheldItem2;
				IHandheldItem handheldItem = (this.m_container.TryGetInstanceForIndex((int)index, out archetypeInstance) && archetypeInstance.Archetype && archetypeInstance.Archetype.TryGetAsType(out handheldItem2)) ? handheldItem2 : fallbackWeapon;
				EquipmentSlot index2 = primaryWeapon ? EquipmentSlot.PrimaryWeapon_OffHand : EquipmentSlot.SecondaryWeapon_OffHand;
				IHandheldItem handheldItem3 = (this.m_container.TryGetInstanceForIndex((int)index2, out archetypeInstance) && archetypeInstance.Archetype && archetypeInstance.Archetype.TryGetAsType(out handheldItem2)) ? handheldItem2 : fallbackWeapon;
				HandheldFlagConfig config = new HandheldFlagConfig
				{
					MainHand = handheldItem.HandheldItemFlag,
					OffHand = handheldItem3.HandheldItemFlag
				};
				BaseRole baseRole;
				return InternalGameDatabase.Archetypes.TryGetAsType<BaseRole>(LocalPlayer.GameEntity.CharacterData.BaseRoleId, out baseRole) && baseRole.MeetsHandheldRequirements(config);
			}
			return false;
		}

		// Token: 0x06003F53 RID: 16211 RVA: 0x001887FC File Offset: 0x001869FC
		private void ToggleHighlight(ArchetypeInstance instance, bool isEnabled)
		{
			EquipableItem equipableItem;
			if (instance.Archetype.TryGetAsType(out equipableItem))
			{
				EquipmentSlot allCompatibleSlots = equipableItem.Type.GetAllCompatibleSlots(false);
				for (int i = 0; i < this.m_slots.Count; i++)
				{
					if (allCompatibleSlots.HasBitFlag(this.m_slots[i].Slot) || (this.m_slots[i].Slot == EquipmentSlot.Cosmetic && EquipmentExtensions.CosmeticEquipmentTypes.Contains(equipableItem.Type)))
					{
						this.m_slots[i].ToggleHighlight(isEnabled, false);
					}
				}
			}
		}

		// Token: 0x140000BD RID: 189
		// (add) Token: 0x06003F54 RID: 16212 RVA: 0x00188894 File Offset: 0x00186A94
		// (remove) Token: 0x06003F55 RID: 16213 RVA: 0x001888C8 File Offset: 0x00186AC8
		public static event Action AvailableChargesChanged;

		// Token: 0x06003F56 RID: 16214 RVA: 0x0006AD1B File Offset: 0x00068F1B
		public static bool TryGetAvailableCharges(RuneSourceType runeSourceType, out int charges)
		{
			return EquipmentUI.m_availableCharges.TryGetValue(runeSourceType, out charges);
		}

		// Token: 0x06003F57 RID: 16215 RVA: 0x0006AD29 File Offset: 0x00068F29
		public override void AddInstance(ArchetypeInstance instance)
		{
			if (instance.Archetype is RunicBattery)
			{
				instance.ItemData.ChargesChanged += this.ItemDataOnChargesChanged;
				this.RefreshAvailableCharges();
			}
			this.InstancedAddedRemoved(instance, true);
			base.AddInstance(instance);
		}

		// Token: 0x06003F58 RID: 16216 RVA: 0x0006AD64 File Offset: 0x00068F64
		public override void RemoveInstance(ArchetypeInstance instance)
		{
			if (instance.Archetype is RunicBattery)
			{
				instance.ItemData.ChargesChanged -= this.ItemDataOnChargesChanged;
				this.RefreshAvailableCharges();
			}
			this.InstancedAddedRemoved(instance, false);
			base.RemoveInstance(instance);
		}

		// Token: 0x06003F59 RID: 16217 RVA: 0x001888FC File Offset: 0x00186AFC
		private void InstancedAddedRemoved(ArchetypeInstance instance, bool added)
		{
			IEquipable equipable;
			if (instance.Archetype.TryGetAsType(out equipable) && equipable.Type.BlockOffhandSlot())
			{
				EquipmentSlot index = (EquipmentSlot)instance.Index;
				if (index == EquipmentSlot.PrimaryWeapon_MainHand)
				{
					this.m_primaryOffHand.Refresh(instance, added);
					return;
				}
				if (index != EquipmentSlot.SecondaryWeapon_MainHand)
				{
					return;
				}
				this.m_secondaryOffHand.Refresh(instance, added);
			}
		}

		// Token: 0x06003F5A RID: 16218 RVA: 0x0006AD9F File Offset: 0x00068F9F
		private void ItemDataOnChargesChanged()
		{
			this.RefreshAvailableCharges();
		}

		// Token: 0x06003F5B RID: 16219 RVA: 0x00188950 File Offset: 0x00186B50
		public void RefreshAvailableCharges()
		{
			if (EquipmentUI.m_availableCharges == null)
			{
				EquipmentUI.m_availableCharges = new Dictionary<RuneSourceType, int>(default(RuneSourceTypeComparer));
			}
			EquipmentUI.m_availableCharges.Clear();
			for (int i = 0; i < RuneSourceTypeExtensions.RuneSourceTypes.Length; i++)
			{
				EquipmentUI.m_availableCharges.Add(RuneSourceTypeExtensions.RuneSourceTypes[i], 0);
			}
			ArchetypeInstance archetypeInstance;
			LocalPlayer.GameEntity.GetHandheldItem_MainHand(out archetypeInstance);
			RunicBattery runicBattery;
			if (archetypeInstance != null && archetypeInstance.Archetype.TryGetAsType(out runicBattery) && archetypeInstance.ItemData.Charges != null)
			{
				Dictionary<RuneSourceType, int> availableCharges = EquipmentUI.m_availableCharges;
				RuneSourceType runeSource = runicBattery.Mastery.RuneSource;
				availableCharges[runeSource] += archetypeInstance.ItemData.Charges.Value;
			}
			ArchetypeInstance archetypeInstance2;
			LocalPlayer.GameEntity.GetHandheldItem_OffHand(out archetypeInstance2);
			RunicBattery runicBattery2;
			if (archetypeInstance2 != null && archetypeInstance2.Archetype.TryGetAsType(out runicBattery2) && archetypeInstance2.ItemData.Charges != null)
			{
				Dictionary<RuneSourceType, int> availableCharges = EquipmentUI.m_availableCharges;
				RuneSourceType runeSource = runicBattery2.Mastery.RuneSource;
				availableCharges[runeSource] += archetypeInstance2.ItemData.Charges.Value;
			}
			Action availableChargesChanged = EquipmentUI.AvailableChargesChanged;
			if (availableChargesChanged == null)
			{
				return;
			}
			availableChargesChanged();
		}

		// Token: 0x17000EA4 RID: 3748
		// (get) Token: 0x06003F5C RID: 16220 RVA: 0x0006ADA7 File Offset: 0x00068FA7
		BindingType IBindingLabel.Type
		{
			get
			{
				return BindingType.Action;
			}
		}

		// Token: 0x17000EA5 RID: 3749
		// (get) Token: 0x06003F5D RID: 16221 RVA: 0x00045BCA File Offset: 0x00043DCA
		int IBindingLabel.Index
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x17000EA6 RID: 3750
		// (get) Token: 0x06003F5E RID: 16222 RVA: 0x0006ADAA File Offset: 0x00068FAA
		TextMeshProUGUI IBindingLabel.Label
		{
			get
			{
				return this.m_swapKeybindLabel;
			}
		}

		// Token: 0x17000EA7 RID: 3751
		// (get) Token: 0x06003F5F RID: 16223 RVA: 0x0006ADB2 File Offset: 0x00068FB2
		string IBindingLabel.FormattedString
		{
			get
			{
				return "<b>{0}</b> To Swap";
			}
		}

		// Token: 0x06003F60 RID: 16224 RVA: 0x00188AA0 File Offset: 0x00186CA0
		private void AssignSettings()
		{
			for (int i = 0; i < this.m_slotSettings.Length; i++)
			{
				EquipmentSlotSetting equipmentSlotSetting = this.m_slotSettings[i];
				equipmentSlotSetting.UI.AssignSettings(equipmentSlotSetting, this.m_defaultBorderColor, this.m_armorBorderColor, true);
			}
		}

		// Token: 0x06003F61 RID: 16225 RVA: 0x0006ADB9 File Offset: 0x00068FB9
		private void ToggleHighlightOn()
		{
			this.ToggleHighlight(true);
		}

		// Token: 0x06003F62 RID: 16226 RVA: 0x0006ADC2 File Offset: 0x00068FC2
		private void ToggleHighlightOff()
		{
			this.ToggleHighlight(false);
		}

		// Token: 0x06003F63 RID: 16227 RVA: 0x00188AE8 File Offset: 0x00186CE8
		private void ToggleHighlight(bool enabled)
		{
			for (int i = 0; i < this.m_slotSettings.Length; i++)
			{
				this.m_slotSettings[i].UI.ToggleHighlight(enabled, false);
			}
		}

		// Token: 0x04003D2C RID: 15660
		[SerializeField]
		private EquipmentSlotSetting[] m_slotSettings;

		// Token: 0x04003D2D RID: 15661
		[SerializeField]
		private ToggleController m_weaponToggle;

		// Token: 0x04003D2E RID: 15662
		[SerializeField]
		private ToggleController m_runeToggle;

		// Token: 0x04003D2F RID: 15663
		[SerializeField]
		private Image m_primaryBorder;

		// Token: 0x04003D30 RID: 15664
		[SerializeField]
		private Image m_secondaryBorder;

		// Token: 0x04003D31 RID: 15665
		[SerializeField]
		private TextMeshProUGUI m_swapKeybindLabel;

		// Token: 0x04003D32 RID: 15666
		[SerializeField]
		private Image m_primaryBackground;

		// Token: 0x04003D33 RID: 15667
		[SerializeField]
		private Image m_secondaryBackground;

		// Token: 0x04003D34 RID: 15668
		[SerializeField]
		private OffHandSlotBlockerUI m_primaryOffHand;

		// Token: 0x04003D35 RID: 15669
		[SerializeField]
		private OffHandSlotBlockerUI m_secondaryOffHand;

		// Token: 0x04003D36 RID: 15670
		[SerializeField]
		private Color m_defaultBorderColor = new Color(0.1490196f, 0.1098039f, 0.07450981f);

		// Token: 0x04003D37 RID: 15671
		[SerializeField]
		private Color m_armorBorderColor = Colors.Gold;

		// Token: 0x04003D38 RID: 15672
		[SerializeField]
		private EmberStoneSlotUI m_emberStoneSlotUi;

		// Token: 0x04003D39 RID: 15673
		private Color? m_defaultPrimaryBackgroundColor;

		// Token: 0x04003D3A RID: 15674
		private Color? m_defaultSecondaryBackgroundColor;

		// Token: 0x04003D3C RID: 15676
		private static Dictionary<RuneSourceType, int> m_availableCharges;
	}
}

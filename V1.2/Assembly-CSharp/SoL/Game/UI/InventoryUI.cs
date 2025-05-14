using System;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Containers;
using SoL.Game.Transactions;
using SoL.Managers;
using SoL.Utilities.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SoL.Game.UI
{
	// Token: 0x0200089F RID: 2207
	[Obsolete]
	public class InventoryUI : ContainerUI<int, InventorySlotUI>
	{
		// Token: 0x0600404F RID: 16463 RVA: 0x0018BF3C File Offset: 0x0018A13C
		protected override void InitializeSlots()
		{
			this.m_slots = new DictionaryList<int, InventorySlotUI>(false);
			for (int i = 0; i < this.m_inventory.Length; i++)
			{
				this.m_inventory[i].Initialize(this, i);
				this.m_slots.Add(i, this.m_inventory[i]);
			}
		}

		// Token: 0x06004050 RID: 16464 RVA: 0x0018BF8C File Offset: 0x0018A18C
		protected override void LeftInstanceDoubleClicked(PointerEventData eventData, ArchetypeInstance instance)
		{
			if (instance.Archetype == null)
			{
				return;
			}
			IEquipable equipable;
			ContainerInstance containerInstance;
			if (!instance.Archetype.TryGetAsType(out equipable) || !LocalPlayer.GameEntity.CollectionController.TryGetInstance(ContainerType.Equipment, out containerInstance))
			{
				ConsumableItem consumableItem;
				if (instance.Archetype.TryGetAsType(out consumableItem))
				{
					LocalPlayer.GameEntity.SkillsController.BeginExecution(instance);
				}
				return;
			}
			if (ClientGameManager.UIManager.EquipmentStats.Locked)
			{
				return;
			}
			int num = -1;
			EquipmentSlot targetEquipmentSlot = equipable.GetTargetEquipmentSlot(LocalPlayer.GameEntity);
			if (targetEquipmentSlot != EquipmentSlot.None)
			{
				num = (int)targetEquipmentSlot;
			}
			ArchetypeInstance archetypeInstance;
			if (containerInstance.TryGetInstanceForIndex(num, out archetypeInstance))
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
				return;
			}
			TransferRequest request2 = new TransferRequest
			{
				TransactionId = UniqueId.GenerateFromGuid(),
				InstanceId = instance.InstanceId,
				SourceContainer = ContainerType.Inventory.ToString(),
				TargetContainer = ContainerType.Equipment.ToString(),
				TargetIndex = num,
				Instance = instance
			};
			LocalPlayer.NetworkEntity.PlayerRpcHandler.TransferRequest(request2);
		}

		// Token: 0x04003E25 RID: 15909
		[SerializeField]
		private InventorySlotUI[] m_inventory;
	}
}

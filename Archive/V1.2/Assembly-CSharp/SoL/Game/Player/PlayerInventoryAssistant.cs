using System;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Containers;
using SoL.Game.Transactions;
using SoL.Networking;
using SoL.Utilities;
using SoL.Utilities.Extensions;

namespace SoL.Game.Player
{
	// Token: 0x020007ED RID: 2029
	public static class PlayerInventoryAssistant
	{
		// Token: 0x06003B05 RID: 15109 RVA: 0x00067F96 File Offset: 0x00066196
		public static bool TryAddItemToPlayer(this ICollectionController controller, ItemArchetype archetype, ItemAddContext context, out ArchetypeInstance resultInstance, int quantity = 1, int index = -1, ItemFlags flags = ItemFlags.None, bool markAsSoulbound = false)
		{
			resultInstance = controller.AddItemToPlayer(archetype, context, quantity, index, flags, markAsSoulbound);
			return resultInstance != null;
		}

		// Token: 0x06003B06 RID: 15110 RVA: 0x00179EA8 File Offset: 0x001780A8
		public static ArchetypeInstance AddItemToPlayer(this ICollectionController controller, ItemArchetype archetype, ItemAddContext context, int quantity = 1, int index = -1, ItemFlags flags = ItemFlags.None, bool markAsSoulbound = false)
		{
			if (controller == null)
			{
				throw new ArgumentNullException("controller");
			}
			if (archetype == null)
			{
				throw new ArgumentNullException("archetype");
			}
			ArchetypeInstance archetypeInstance = archetype.CreateNewInstance();
			archetypeInstance.ItemData.ItemFlags = flags;
			if (markAsSoulbound)
			{
				archetypeInstance.ItemData.MarkAsSoulbound(controller.Record);
			}
			if (archetypeInstance.Archetype.ArchetypeHasCount())
			{
				archetypeInstance.ItemData.Count = new int?(quantity);
			}
			return controller.AddItemInstanceToPlayer(archetypeInstance, context, index, true);
		}

		// Token: 0x06003B07 RID: 15111 RVA: 0x00179F2C File Offset: 0x0017812C
		public static ArchetypeInstance AddItemInstanceToPlayer(this ICollectionController controller, ArchetypeInstance newInstance, ItemAddContext context, int index = -1, bool returnToPoolOnFailure = false)
		{
			if (controller == null)
			{
				throw new ArgumentNullException("controller");
			}
			if (controller.Pouch == null)
			{
				throw new ArgumentNullException("Pouch");
			}
			if (controller.ReagentPouch == null)
			{
				throw new ArgumentNullException("ReagentPouch");
			}
			if (controller.Inventory == null)
			{
				throw new ArgumentNullException("Inventory");
			}
			if (controller.Gathering == null)
			{
				throw new ArgumentNullException("Gathering");
			}
			if (newInstance == null)
			{
				throw new ArgumentNullException("newInstance");
			}
			if (!newInstance.IsItem)
			{
				throw new InvalidOperationException("Not an item! " + newInstance.Archetype.DisplayName);
			}
			ArchetypeInstance archetypeInstance = null;
			if (newInstance.Archetype is IStackable && (PlayerInventoryAssistant.TryMergeWithinContainer(controller, newInstance, controller.ReagentPouch, out archetypeInstance) || PlayerInventoryAssistant.TryMergeWithinContainer(controller, newInstance, controller.Pouch, out archetypeInstance) || PlayerInventoryAssistant.TryMergeWithinContainer(controller, newInstance, controller.Gathering, out archetypeInstance) || PlayerInventoryAssistant.TryMergeWithinContainer(controller, newInstance, controller.Inventory, out archetypeInstance)))
			{
				ContainerInstance containerInstance = newInstance.ContainerInstance;
				if (containerInstance != null)
				{
					containerInstance.Remove(newInstance.InstanceId);
				}
				StaticPool<ArchetypeInstance>.ReturnToPool(newInstance);
				return archetypeInstance;
			}
			RangedAmmoItem rangedAmmoItem;
			if (controller.Equipment != null && newInstance.Archetype.TryGetAsType(out rangedAmmoItem))
			{
				if (rangedAmmoItem.EntityHasMatchingAmmoEquipped(controller.GameEntity, out archetypeInstance) && archetypeInstance.CanMergeWith(newInstance))
				{
					int num = 0;
					if (archetypeInstance.ItemData.Count != null)
					{
						num += archetypeInstance.ItemData.Count.Value;
					}
					if (newInstance.ItemData.Count != null)
					{
						num += newInstance.ItemData.Count.Value;
					}
					archetypeInstance.ItemData.Count = new int?(num);
					ItemCountUpdatedTransaction transaction = new ItemCountUpdatedTransaction
					{
						InstanceId = archetypeInstance.InstanceId,
						Container = archetypeInstance.ContainerInstance.Id,
						NewCount = archetypeInstance.ItemData.Count.Value
					};
					controller.GameEntity.NetworkEntity.PlayerRpcHandler.UpdateItemCount(transaction);
					ContainerInstance containerInstance2 = newInstance.ContainerInstance;
					if (containerInstance2 != null)
					{
						containerInstance2.Remove(newInstance.InstanceId);
					}
					StaticPool<ArchetypeInstance>.ReturnToPool(newInstance);
					return archetypeInstance;
				}
				EquipmentSlot index2;
				if (rangedAmmoItem.EntityHasEmptyAmmoSlotForWeapon(controller.GameEntity, out index2))
				{
					ContainerInstance containerInstance3 = newInstance.ContainerInstance;
					if (containerInstance3 != null)
					{
						containerInstance3.Remove(newInstance.InstanceId);
					}
					newInstance.Index = (int)index2;
					controller.Equipment.Add(newInstance, true);
					ArchetypeAddedTransaction response = new ArchetypeAddedTransaction
					{
						Op = OpCodes.Ok,
						Instance = newInstance,
						TargetContainer = controller.Equipment.Id,
						Context = context
					};
					controller.GameEntity.NetworkEntity.PlayerRpcHandler.AddItemResponse(response);
					return newInstance;
				}
			}
			ContainerInstance containerInstance4 = null;
			ReagentItem reagentItem;
			if (newInstance.Archetype.TryGetAsType(out reagentItem) && controller.ReagentPouch.HasRoom() && reagentItem.Type.IsCompatibleWithEntity(controller.GameEntity))
			{
				containerInstance4 = controller.ReagentPouch;
			}
			else if (newInstance.Archetype.CanPlaceInPouch && controller.Pouch.HasRoom())
			{
				containerInstance4 = controller.Pouch;
			}
			else if (newInstance.Archetype.CanPlaceInGathering && controller.Gathering.HasRoom())
			{
				containerInstance4 = controller.Gathering;
			}
			else if (controller.Inventory.HasRoom() && !controller.GameEntity.IsMissingBag)
			{
				containerInstance4 = controller.Inventory;
			}
			if (containerInstance4 != null)
			{
				ContainerInstance containerInstance5 = newInstance.ContainerInstance;
				if (containerInstance5 != null)
				{
					containerInstance5.Remove(newInstance.InstanceId);
				}
				newInstance.Index = ((index == -1) ? containerInstance4.GetFirstAvailableIndex() : index);
				ArchetypeAddedTransaction response2 = new ArchetypeAddedTransaction
				{
					Op = OpCodes.Error,
					Instance = null,
					TargetContainer = containerInstance4.Id,
					Context = context
				};
				if (containerInstance4.Add(newInstance, true))
				{
					response2.Op = OpCodes.Ok;
					response2.Instance = newInstance;
				}
				else
				{
					PlayerCollectionController.AddInstanceToInvalid(controller.Record, newInstance, containerInstance4.ContainerType, "InvalidAddItemInstanceToPlayer", true);
				}
				controller.GameEntity.NetworkEntity.PlayerRpcHandler.AddItemResponse(response2);
				return newInstance;
			}
			if (returnToPoolOnFailure)
			{
				ContainerInstance containerInstance6 = newInstance.ContainerInstance;
				if (containerInstance6 != null)
				{
					containerInstance6.Remove(newInstance.InstanceId);
				}
				StaticPool<ArchetypeInstance>.ReturnToPool(newInstance);
			}
			return null;
		}

		// Token: 0x06003B08 RID: 15112 RVA: 0x0017A364 File Offset: 0x00178564
		private static bool TryMergeWithinContainer(ICollectionController controller, ArchetypeInstance newInstance, ContainerInstance containerInstance, out ArchetypeInstance mergedInstance)
		{
			mergedInstance = null;
			if (containerInstance.ContainerType == ContainerType.Pouch && !newInstance.Archetype.CanPlaceInPouch)
			{
				return false;
			}
			if (containerInstance.ContainerType == ContainerType.Gathering && !newInstance.Archetype.CanPlaceInGathering)
			{
				return false;
			}
			if (containerInstance.ContainerType == ContainerType.Inventory && controller.GameEntity.IsMissingBag)
			{
				return false;
			}
			int num = (newInstance.ItemData.Count != null) ? newInstance.ItemData.Count.Value : 1;
			foreach (ArchetypeInstance archetypeInstance in containerInstance.Instances)
			{
				if (archetypeInstance.CanMergeWith(newInstance))
				{
					mergedInstance = archetypeInstance;
					archetypeInstance.ItemData.Count = new int?((archetypeInstance.ItemData.Count != null) ? (archetypeInstance.ItemData.Count.Value + num) : (1 + num));
					ItemCountUpdatedTransaction transaction = new ItemCountUpdatedTransaction
					{
						InstanceId = archetypeInstance.InstanceId,
						Container = archetypeInstance.ContainerInstance.Id,
						NewCount = archetypeInstance.ItemData.Count.Value
					};
					controller.GameEntity.NetworkEntity.PlayerRpcHandler.UpdateItemCount(transaction);
					return true;
				}
			}
			return false;
		}
	}
}

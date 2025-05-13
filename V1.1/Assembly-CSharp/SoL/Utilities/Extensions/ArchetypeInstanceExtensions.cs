using System;
using SoL.Game;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Containers;
using SoL.Game.Settings;
using SoL.Game.Transactions;
using SoL.Networking.Database;
using UnityEngine;

namespace SoL.Utilities.Extensions
{
	// Token: 0x02000324 RID: 804
	public static class ArchetypeInstanceExtensions
	{
		// Token: 0x06001632 RID: 5682 RVA: 0x000FF2D4 File Offset: 0x000FD4D4
		public static ArchetypeInstance CreateNewInstance(IArchetype archetype)
		{
			ArchetypeInstance fromPool = StaticPool<ArchetypeInstance>.GetFromPool();
			fromPool.InitializeNew(archetype.Id);
			archetype.OnInstanceCreated(fromPool);
			return fromPool;
		}

		// Token: 0x06001633 RID: 5683 RVA: 0x0005186A File Offset: 0x0004FA6A
		public static void ReturnToPool(this ArchetypeInstance instance)
		{
			StaticPool<ArchetypeInstance>.ReturnToPool(instance);
		}

		// Token: 0x06001634 RID: 5684 RVA: 0x000FF2FC File Offset: 0x000FD4FC
		public static bool ReduceChargeBy(this ArchetypeInstance instance, int toRemove, GameEntity entity)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			if (!instance.Archetype.ArchetypeHasCharges() || instance.ItemData == null || instance.ItemData.Charges == null)
			{
				throw new ArgumentException("Cannot reduce charges if item has no ItemData or charges!");
			}
			instance.ItemData.Charges = new int?(Mathf.Clamp(instance.ItemData.Charges.Value - toRemove, 0, int.MaxValue));
			int? charges = instance.ItemData.Charges;
			int num = 0;
			return (charges.GetValueOrDefault() > num & charges != null) || instance.RemoveFromRemoteClient(entity, ItemDestructionContext.Charges);
		}

		// Token: 0x06001635 RID: 5685 RVA: 0x000FF3A8 File Offset: 0x000FD5A8
		public static bool ReduceCountBy(this ArchetypeInstance instance, int toRemove, GameEntity entity)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			if (!instance.Archetype.ArchetypeHasCount() || instance.ItemData == null || instance.ItemData.Count == null)
			{
				throw new ArgumentException("Cannot reduce count if item has no ItemData or count!");
			}
			instance.ItemData.Count = new int?(Mathf.Clamp(instance.ItemData.Count.Value - toRemove, 0, int.MaxValue));
			int? count = instance.ItemData.Count;
			int num = 0;
			if (count.GetValueOrDefault() <= num & count != null)
			{
				return instance.RemoveFromRemoteClient(entity, ItemDestructionContext.Count);
			}
			ItemCountUpdatedTransaction transaction = new ItemCountUpdatedTransaction
			{
				InstanceId = instance.InstanceId,
				Container = instance.ContainerInstance.Id,
				NewCount = instance.ItemData.Count.Value
			};
			entity.NetworkEntity.PlayerRpcHandler.UpdateItemCount(transaction);
			return true;
		}

		// Token: 0x06001636 RID: 5686 RVA: 0x00051872 File Offset: 0x0004FA72
		public static bool Consume(this ArchetypeInstance instance, GameEntity entity)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			if (!instance.IsItem)
			{
				throw new ArgumentException("Cannot consume something that is not an item!");
			}
			return instance.RemoveFromRemoteClient(entity, ItemDestructionContext.Consumption);
		}

		// Token: 0x06001637 RID: 5687 RVA: 0x000FF4AC File Offset: 0x000FD6AC
		private static bool RemoveFromRemoteClient(this ArchetypeInstance instance, GameEntity entity, ItemDestructionContext context)
		{
			if (!entity)
			{
				throw new ArgumentNullException("entity");
			}
			PlayerCollectionController playerCollectionController;
			if (entity.CollectionController != null && entity.CollectionController.TryGetAsType(out playerCollectionController))
			{
				playerCollectionController.RemoveInstanceFromRemoteClient(instance, context);
				return true;
			}
			return false;
		}

		// Token: 0x06001638 RID: 5688 RVA: 0x000FF4F0 File Offset: 0x000FD6F0
		public static void AddItemFlag(this ArchetypeInstance instance, ItemFlags flags, CharacterRecord characterRecord)
		{
			if (!instance.IsItem)
			{
				return;
			}
			instance.ItemData.ItemFlags |= flags;
			if (characterRecord == null)
			{
				return;
			}
			if (flags.HasBitFlag(ItemFlags.Crafted) && !(instance.Archetype is IStackable))
			{
				instance.ItemData.PlayerName = characterRecord.Name;
			}
		}

		// Token: 0x06001639 RID: 5689 RVA: 0x0005189D File Offset: 0x0004FA9D
		public static void MarkAsSoulbound(this ArchetypeInstance instance, CharacterRecord record)
		{
			if (instance != null && instance.IsItem && record != null)
			{
				instance.ItemData.MarkAsSoulbound(record);
			}
		}

		// Token: 0x0600163A RID: 5690 RVA: 0x000FF548 File Offset: 0x000FD748
		public static uint GetRepairCost(this ArchetypeInstance instance)
		{
			if (instance != null && instance.IsItem && instance.ItemData.Durability != null)
			{
				return (uint)Mathf.FloorToInt((float)instance.ItemData.Durability.Absorbed * GlobalSettings.Values.Player.BlacksmithRepairMultiplier);
			}
			return 0U;
		}

		// Token: 0x0600163B RID: 5691 RVA: 0x000FF598 File Offset: 0x000FD798
		public static ulong GetSellPrice(this ArchetypeInstance instance)
		{
			ItemArchetype itemArchetype;
			ulong result;
			if (instance != null && instance.IsItem && instance.Archetype.TryGetAsType(out itemArchetype) && itemArchetype.TryGetSalePrice(instance, out result))
			{
				return result;
			}
			return 0UL;
		}

		// Token: 0x0600163C RID: 5692 RVA: 0x000518B9 File Offset: 0x0004FAB9
		public static bool ArchetypeHasCount(this BaseArchetype baseArchetype)
		{
			return baseArchetype is IStackable;
		}

		// Token: 0x0600163D RID: 5693 RVA: 0x00045BCA File Offset: 0x00043DCA
		public static bool ArchetypeHasCharges(this BaseArchetype baseArchetype)
		{
			return false;
		}

		// Token: 0x0600163E RID: 5694 RVA: 0x000FF5D0 File Offset: 0x000FD7D0
		public static bool CanDeconstruct(this ArchetypeInstance instance, GameEntity entity)
		{
			IStackable stackable;
			return entity != null && instance != null && instance.Archetype != null && !instance.Archetype.TryGetAsType(out stackable) && instance.IsItem && instance.ItemData != null && instance.ItemData.ItemFlags.HasBitFlag(ItemFlags.Crafted) && instance.ItemData.PlayerName == entity.CharacterData.Name.Value;
		}
	}
}

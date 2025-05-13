using System;
using System.Collections.Generic;
using SoL.Game.EffectSystem;
using SoL.Game.HuntingLog;
using SoL.Game.Interactives;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Containers;
using SoL.Game.Settings;
using SoL.Game.Transactions;
using SoL.Managers;
using SoL.Networking;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game
{
	// Token: 0x0200057C RID: 1404
	public static class GameEntityExtensions
	{
		// Token: 0x06002BE0 RID: 11232 RVA: 0x0005E6EA File Offset: 0x0005C8EA
		public static bool TryGetHandheldItem_MainHandAsType<T>(this GameEntity entity, out ArchetypeInstance instance, out T item) where T : class
		{
			item = (entity.GetHandheldItem_MainHand(out instance) as T);
			return item != null && instance != null;
		}

		// Token: 0x06002BE1 RID: 11233 RVA: 0x0005E717 File Offset: 0x0005C917
		public static bool TryGetHandheldItem_OffHandAsType<T>(this GameEntity entity, out ArchetypeInstance instance, out T item) where T : class
		{
			item = (entity.GetHandheldItem_OffHand(out instance) as T);
			return item != null && instance != null;
		}

		// Token: 0x06002BE2 RID: 11234 RVA: 0x00146AF4 File Offset: 0x00144CF4
		public static bool TryGetRangedAmmo(this GameEntity entity, IHandHeldItems handHeldItems, out ArchetypeInstance instance, out RangedAmmoItem item)
		{
			instance = null;
			item = null;
			if (handHeldItems != null)
			{
				if (handHeldItems.OffHand.RangedAmmo != null)
				{
					instance = handHeldItems.OffHand.Instance;
					item = handHeldItems.OffHand.RangedAmmo;
				}
			}
			else
			{
				EquipmentSlot index = entity.CharacterData.OffHand_SecondaryActive ? EquipmentSlot.SecondaryWeapon_OffHand : EquipmentSlot.PrimaryWeapon_OffHand;
				if (entity.CollectionController != null && entity.CollectionController.Equipment != null && entity.CollectionController.Equipment.TryGetInstanceForIndex((int)index, out instance))
				{
					instance.Archetype.TryGetAsType(out item);
				}
			}
			return instance != null && item != null;
		}

		// Token: 0x06002BE3 RID: 11235 RVA: 0x0005E744 File Offset: 0x0005C944
		public static IHandheldItem GetHandheldItem_MainHand(this GameEntity entity, out ArchetypeInstance instance)
		{
			return GameEntityExtensions.GetHandheldItem(entity, true, out instance);
		}

		// Token: 0x06002BE4 RID: 11236 RVA: 0x0005E74E File Offset: 0x0005C94E
		public static IHandheldItem GetHandheldItem_OffHand(this GameEntity entity, out ArchetypeInstance instance)
		{
			return GameEntityExtensions.GetHandheldItem(entity, false, out instance);
		}

		// Token: 0x06002BE5 RID: 11237 RVA: 0x00146B90 File Offset: 0x00144D90
		private static IHandheldItem GetHandheldItem(GameEntity entity, bool main, out ArchetypeInstance instance)
		{
			IHandheldItem fallbackWeapon = GlobalSettings.Values.Combat.FallbackWeapon;
			instance = null;
			EquipmentSlot index;
			if (main)
			{
				index = (entity.CharacterData.MainHand_SecondaryActive ? EquipmentSlot.SecondaryWeapon_MainHand : EquipmentSlot.PrimaryWeapon_MainHand);
			}
			else
			{
				index = (entity.CharacterData.OffHand_SecondaryActive ? EquipmentSlot.SecondaryWeapon_OffHand : EquipmentSlot.PrimaryWeapon_OffHand);
			}
			if (entity.CollectionController != null && entity.CollectionController.Equipment != null && entity.CollectionController.Equipment.TryGetInstanceForIndex((int)index, out instance))
			{
				instance.Archetype.TryGetAsType(out fallbackWeapon);
			}
			return fallbackWeapon;
		}

		// Token: 0x06002BE6 RID: 11238 RVA: 0x0005E758 File Offset: 0x0005C958
		public static UniqueId GetActiveWeaponMasteryId(this GameEntity entity)
		{
			return entity.CharacterData.BaseRoleId;
		}

		// Token: 0x06002BE7 RID: 11239 RVA: 0x00146C14 File Offset: 0x00144E14
		public static bool TryGetActiveWeaponMasteryAsType<T>(this GameEntity entity, out ArchetypeInstance instance, out T item) where T : MasteryArchetype
		{
			instance = null;
			item = default(T);
			UniqueId activeWeaponMasteryId = entity.GetActiveWeaponMasteryId();
			if (!activeWeaponMasteryId.IsEmpty && entity.CollectionController.Masteries.TryGetInstanceForArchetypeId(activeWeaponMasteryId, out instance) && instance.Archetype)
			{
				instance.Archetype.TryGetAsType(out item);
			}
			return instance != null && item;
		}

		// Token: 0x06002BE8 RID: 11240 RVA: 0x00146C84 File Offset: 0x00144E84
		public static bool IsAtTrainer(this GameEntity entity)
		{
			InteractiveMerchant interactiveMerchant;
			return entity && entity.Type == GameEntityType.Player && entity.CollectionController != null && !(entity.CollectionController.InteractiveStation == null) && entity.CollectionController.InteractiveStation.TryGetAsType(out interactiveMerchant) && interactiveMerchant.IsTrainer;
		}

		// Token: 0x06002BE9 RID: 11241 RVA: 0x00146CDC File Offset: 0x00144EDC
		public static ulong GetAvailableCurrency(this GameEntity entity, out CurrencySources currencySource, CurrencySources allowedSources = CurrencySources.Inventory | CurrencySources.PersonalBank)
		{
			currencySource = CurrencySources.None;
			if (entity == null || entity.CollectionController == null)
			{
				return 0UL;
			}
			ulong num = 0UL;
			if (allowedSources.HasBitFlag(CurrencySources.Inventory) && !entity.IsMissingBag && entity.CollectionController.Inventory != null)
			{
				num += entity.CollectionController.Inventory.Currency;
				currencySource |= CurrencySources.Inventory;
			}
			if (allowedSources.HasBitFlag(CurrencySources.PersonalBank) && entity.CollectionController.PersonalBank != null)
			{
				num += entity.CollectionController.PersonalBank.Currency;
				currencySource |= CurrencySources.PersonalBank;
			}
			return num;
		}

		// Token: 0x06002BEA RID: 11242 RVA: 0x0005E765 File Offset: 0x0005C965
		public static ulong GetAvailableCurrencyForInteractiveStation(this GameEntity entity, out CurrencySources currencySource)
		{
			currencySource = CurrencySources.None;
			if (entity == null || entity.CollectionController == null || entity.CollectionController.InteractiveStation == null)
			{
				return 0UL;
			}
			return entity.CollectionController.InteractiveStation.GetAvailableCurrency(entity, out currencySource);
		}

		// Token: 0x06002BEB RID: 11243 RVA: 0x00146D6C File Offset: 0x00144F6C
		public static bool RemoveCurrency(this GameEntity entity, ulong currencyToRemove, out MultiContainerCurrencyTransaction transaction, ReplicationFlags replicationFlags = ReplicationFlags.Client | ReplicationFlags.Server)
		{
			transaction = default(MultiContainerCurrencyTransaction);
			if (!GameManager.IsServer)
			{
				return false;
			}
			ContainerInstance containerInstance;
			ContainerInstance containerInstance2;
			if (entity.CollectionController.TryGetInstance(ContainerType.Inventory, out containerInstance) && entity.CollectionController.TryGetInstance(ContainerType.PersonalBank, out containerInstance2))
			{
				if (!entity.IsMissingBag)
				{
					if (containerInstance.Currency >= currencyToRemove)
					{
						transaction.Adjustments = new CurrencyAdjustment[1];
						transaction.Adjustments[0] = new CurrencyAdjustment
						{
							Add = false,
							Amount = currencyToRemove,
							TargetContainer = containerInstance.Id
						};
						if (replicationFlags.HasBitFlag(ReplicationFlags.Server))
						{
							containerInstance.RemoveCurrency(currencyToRemove);
						}
						if (replicationFlags.HasBitFlag(ReplicationFlags.Client))
						{
							entity.NetworkEntity.PlayerRpcHandler.ProcessInteractiveStationCurrencyTransaction(new ulong?(containerInstance.Currency), null);
						}
						return true;
					}
					ulong num = currencyToRemove - containerInstance.Currency;
					if (containerInstance2.Currency >= num)
					{
						if (containerInstance.Currency > 0UL)
						{
							transaction.Adjustments = new CurrencyAdjustment[2];
							transaction.Adjustments[1] = new CurrencyAdjustment
							{
								Add = false,
								Amount = containerInstance.Currency,
								TargetContainer = containerInstance.Id
							};
							if (replicationFlags.HasBitFlag(ReplicationFlags.Server))
							{
								containerInstance.RemoveCurrency(containerInstance.Currency);
							}
						}
						if (transaction.Adjustments == null)
						{
							transaction.Adjustments = new CurrencyAdjustment[1];
						}
						transaction.Adjustments[0] = new CurrencyAdjustment
						{
							Add = false,
							Amount = num,
							TargetContainer = containerInstance2.Id
						};
						if (replicationFlags.HasBitFlag(ReplicationFlags.Server))
						{
							containerInstance2.RemoveCurrency(num);
						}
						if (replicationFlags.HasBitFlag(ReplicationFlags.Client))
						{
							entity.NetworkEntity.PlayerRpcHandler.ProcessInteractiveStationCurrencyTransaction(new ulong?(containerInstance.Currency), new ulong?(containerInstance2.Currency));
						}
						return true;
					}
				}
				else if (containerInstance2.Currency >= currencyToRemove)
				{
					transaction.Adjustments = new CurrencyAdjustment[1];
					transaction.Adjustments[0] = new CurrencyAdjustment
					{
						Add = false,
						Amount = currencyToRemove,
						TargetContainer = containerInstance2.Id
					};
					if (replicationFlags.HasBitFlag(ReplicationFlags.Server))
					{
						containerInstance2.RemoveCurrency(currencyToRemove);
					}
					if (replicationFlags.HasBitFlag(ReplicationFlags.Client))
					{
						entity.NetworkEntity.PlayerRpcHandler.ProcessInteractiveStationCurrencyTransaction(null, new ulong?(containerInstance2.Currency));
					}
					return true;
				}
			}
			return false;
		}

		// Token: 0x06002BEC RID: 11244 RVA: 0x00146FC8 File Offset: 0x001451C8
		public static bool EntityCanAcquire(this GameEntity entity, ItemArchetype item, out ContainerInstance targetContainerInstance, out CannotAcquireReason reason, ContainerInstance bypassContainer = null)
		{
			targetContainerInstance = null;
			reason = CannotAcquireReason.None;
			if (entity == null || entity.CollectionController == null || item == null)
			{
				reason = CannotAcquireReason.Unknown;
				return false;
			}
			bool flag = item is IStackable;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			ConsumableItem consumableItem;
			if (entity.CollectionController.Pouch != null && entity.CollectionController.Pouch != bypassContainer && item.CanPlaceInPouch && item.TryGetAsType(out consumableItem))
			{
				ContainerInstance pouch = entity.CollectionController.Pouch;
				bool flag5 = pouch.HasRoom();
				ArchetypeInstance archetypeInstance;
				if (flag5 || (flag && pouch.TryGetInstanceForArchetypeId(item.Id, out archetypeInstance)))
				{
					targetContainerInstance = pouch;
				}
				flag2 = !flag5;
			}
			ReagentItem reagentItem;
			if (targetContainerInstance == null && entity.CollectionController.ReagentPouch != null && entity.CollectionController.ReagentPouch != bypassContainer && item.TryGetAsType(out reagentItem) && reagentItem.Type.IsCompatibleWithEntity(entity))
			{
				ContainerInstance reagentPouch = entity.CollectionController.ReagentPouch;
				bool flag6 = reagentPouch.HasRoom();
				ArchetypeInstance archetypeInstance2;
				if (flag6 || (flag && reagentPouch.TryGetInstanceForArchetypeId(item.Id, out archetypeInstance2)))
				{
					targetContainerInstance = reagentPouch;
				}
				flag3 = !flag6;
			}
			RangedAmmoItem rangedAmmoItem;
			ArchetypeInstance archetypeInstance3;
			EquipmentSlot equipmentSlot;
			if (targetContainerInstance == null && item.TryGetAsType(out rangedAmmoItem) && (rangedAmmoItem.EntityHasMatchingAmmoEquipped(entity, out archetypeInstance3) || rangedAmmoItem.EntityHasEmptyAmmoSlotForWeapon(entity, out equipmentSlot)))
			{
				targetContainerInstance = entity.CollectionController.Equipment;
			}
			if (targetContainerInstance == null && entity.CollectionController.Gathering != null && entity.CollectionController.Gathering != bypassContainer && item.CanPlaceInGathering)
			{
				ContainerInstance gathering = entity.CollectionController.Gathering;
				bool flag7 = gathering.HasRoom();
				ArchetypeInstance archetypeInstance4;
				if (flag7 || (flag && gathering.TryGetInstanceForArchetypeId(item.Id, out archetypeInstance4)))
				{
					targetContainerInstance = gathering;
				}
				flag4 = !flag7;
			}
			if (targetContainerInstance == null)
			{
				if (entity.IsMissingBag)
				{
					reason = ((flag2 || flag3 || flag4) ? CannotAcquireReason.NoRoom : CannotAcquireReason.Dead);
				}
				else if (entity.CollectionController.Inventory != null)
				{
					ContainerInstance inventory = entity.CollectionController.Inventory;
					ArchetypeInstance archetypeInstance5;
					if (flag && inventory.TryGetInstanceForArchetypeId(item.Id, out archetypeInstance5))
					{
						targetContainerInstance = inventory;
					}
					else if (inventory.HasRoom())
					{
						targetContainerInstance = inventory;
					}
					else
					{
						reason = CannotAcquireReason.NoRoom;
					}
				}
			}
			if (reason == CannotAcquireReason.None && targetContainerInstance == null)
			{
				reason = CannotAcquireReason.Unknown;
			}
			return targetContainerInstance != null;
		}

		// Token: 0x06002BED RID: 11245 RVA: 0x001471D8 File Offset: 0x001453D8
		public static bool EntityCanAcquire(this GameEntity entity, IEnumerable<ItemArchetype> items, out CannotAcquireReason reason)
		{
			int num = entity.CollectionController.Pouch.GetMaxCapacity() - entity.CollectionController.Pouch.Count;
			int num2 = entity.CollectionController.ReagentPouch.GetMaxCapacity() - entity.CollectionController.ReagentPouch.Count;
			int num3 = entity.CollectionController.Gathering.GetMaxCapacity() - entity.CollectionController.Gathering.Count;
			int num4 = entity.CollectionController.Inventory.GetMaxCapacity() - entity.CollectionController.Inventory.Count;
			foreach (ItemArchetype itemArchetype in items)
			{
				bool flag = itemArchetype is IStackable;
				bool flag2 = false;
				ContainerInstance containerInstance;
				CannotAcquireReason cannotAcquireReason;
				if (!entity.EntityCanAcquire(itemArchetype, out containerInstance, out cannotAcquireReason, null))
				{
					reason = cannotAcquireReason;
					return false;
				}
				ArchetypeInstance archetypeInstance;
				if (!flag || !containerInstance.TryGetInstanceForArchetypeId(itemArchetype.Id, out archetypeInstance))
				{
					while (!flag2 && containerInstance.ContainerType != ContainerType.Inventory)
					{
						if (containerInstance == entity.CollectionController.Pouch && num > 0)
						{
							num--;
							flag2 = true;
						}
						else if (containerInstance == entity.CollectionController.ReagentPouch && num2 > 0)
						{
							num2--;
							flag2 = true;
						}
						else if (containerInstance == entity.CollectionController.Gathering && num3 > 0)
						{
							num3--;
							flag2 = true;
						}
						else if (!entity.EntityCanAcquire(itemArchetype, out containerInstance, out cannotAcquireReason, containerInstance))
						{
							reason = cannotAcquireReason;
							return false;
						}
					}
					if (!flag2)
					{
						if (containerInstance != entity.CollectionController.Inventory)
						{
							Debug.LogError("Problem in EntityCanAcquire for lists, should not reach this point without trying all other container types!");
							reason = CannotAcquireReason.Unknown;
							return false;
						}
						if (num4 <= 0)
						{
							reason = CannotAcquireReason.NoRoom;
							return false;
						}
						num4--;
					}
				}
			}
			reason = CannotAcquireReason.None;
			return true;
		}

		// Token: 0x06002BEE RID: 11246 RVA: 0x0005E7A3 File Offset: 0x0005C9A3
		public static float GetDiminishedStatAsFloat(this GameEntity entity, StatType statType, StatSettings.DiminishingCurve diminishingCurve, HuntingLogEntry huntingLogEntry, bool applyPvpModifiers, int weaponAugmentValue, float weaponFlankingValue)
		{
			return (float)entity.GetDiminishedStatAsInt(statType, diminishingCurve, huntingLogEntry, applyPvpModifiers, weaponAugmentValue, weaponFlankingValue) * 0.01f;
		}

		// Token: 0x06002BEF RID: 11247 RVA: 0x001473C0 File Offset: 0x001455C0
		public static int GetDiminishedStatAsInt(this GameEntity entity, StatType statType, StatSettings.DiminishingCurve diminishingCurve, HuntingLogEntry huntingLogEntry, bool applyPvpModifiers, int weaponAugmentValue, float weaponFlankingValue)
		{
			if (statType == StatType.None || !entity || !entity.Vitals)
			{
				return 0;
			}
			int num;
			int num2;
			entity.Vitals.GetStatusEffectValues(statType, out num, out num2);
			int num3;
			if (huntingLogEntry != null && huntingLogEntry.TryGetPerksForStat(statType, out num3))
			{
				num2 += num3;
			}
			if (applyPvpModifiers)
			{
				num2 += statType.GetPvpResist();
			}
			num += weaponAugmentValue;
			num += NumberExtensions.ToIntTowardsZero(weaponFlankingValue * 100f);
			if (entity.Type == GameEntityType.Player && diminishingCurve != null)
			{
				num = diminishingCurve.GetDiminishedValue(num);
			}
			return num + num2;
		}
	}
}

using System;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Settings;
using SoL.Utilities.Extensions;
using SoL.Utilities.Logging;

namespace SoL.Game.Objects.Containers
{
	// Token: 0x02000A10 RID: 2576
	public static class ContainerTypeExtensions
	{
		// Token: 0x06004EA0 RID: 20128 RVA: 0x001C41B4 File Offset: 0x001C23B4
		public static string GetDescription(this ContainerType type)
		{
			switch (type)
			{
			case ContainerType.Equipment:
				return "Inventory";
			case ContainerType.Inventory:
				return "Bag";
			case (ContainerType)3:
				goto IL_4F;
			case ContainerType.Pouch:
				return "Pouch";
			case ContainerType.ReagentPouch:
				return "Reagent Pouch";
			case ContainerType.PersonalBank:
				break;
			case ContainerType.Gathering:
				return "Gathering Bag";
			default:
				if (type != ContainerType.Bank)
				{
					goto IL_4F;
				}
				break;
			}
			return "Bank";
			IL_4F:
			return type.ToStringWithSpaces();
		}

		// Token: 0x06004EA1 RID: 20129 RVA: 0x001C421C File Offset: 0x001C241C
		public static int GetMaxCapacity(this ContainerType type)
		{
			switch (type)
			{
			case ContainerType.Inventory:
				return 36;
			case (ContainerType)3:
				return int.MaxValue;
			case ContainerType.Pouch:
			case ContainerType.ReagentPouch:
				return 4;
			case ContainerType.PersonalBank:
				if (!(GlobalSettings.Values != null) || GlobalSettings.Values.Player == null || !(GlobalSettings.Values.Player.PersonalBankProfile != null))
				{
					return 64;
				}
				return GlobalSettings.Values.Player.PersonalBankProfile.RawMaxCapacity;
			case ContainerType.Gathering:
				return 30;
			case ContainerType.LostAndFound:
				break;
			default:
				switch (type)
				{
				case ContainerType.Loot:
					break;
				case ContainerType.TradeOutgoing:
				case ContainerType.TradeIncoming:
				case ContainerType.Inspection:
					return int.MaxValue;
				case ContainerType.MerchantOutgoing:
					return 6;
				case ContainerType.BlacksmithOutgoing:
					return 5;
				case ContainerType.RuneCollector:
				case ContainerType.AuctionOutgoing:
					return 1;
				case ContainerType.PostOutgoing:
				case ContainerType.PostIncoming:
					return 7;
				case ContainerType.RefinementInput:
				case ContainerType.RefinementOutput:
					return 4;
				default:
					if (type != ContainerType.Bank)
					{
						return int.MaxValue;
					}
					return 64;
				}
				break;
			}
			return 64;
		}

		// Token: 0x06004EA2 RID: 20130 RVA: 0x0007519D File Offset: 0x0007339D
		public static bool IsLocal(this ContainerType type)
		{
			return type < ContainerType.Loot;
		}

		// Token: 0x06004EA3 RID: 20131 RVA: 0x001C42F4 File Offset: 0x001C24F4
		public static bool IsRemote(this ContainerType type)
		{
			return type >= ContainerType.Loot && type < ContainerType.Bank;
		}

		// Token: 0x06004EA4 RID: 20132 RVA: 0x000751A4 File Offset: 0x000733A4
		public static bool IsBank(this ContainerType type)
		{
			return type == ContainerType.PersonalBank || type == ContainerType.Bank;
		}

		// Token: 0x06004EA5 RID: 20133 RVA: 0x000751B2 File Offset: 0x000733B2
		public static bool RequiresUniqueId(this ContainerType type)
		{
			return type >= ContainerType.Bank;
		}

		// Token: 0x06004EA6 RID: 20134 RVA: 0x000751BC File Offset: 0x000733BC
		public static bool HasCurrency(this ContainerType type)
		{
			if (type <= ContainerType.PersonalBank)
			{
				if (type != ContainerType.Inventory && type != ContainerType.PersonalBank)
				{
					return false;
				}
			}
			else if (type - ContainerType.TradeOutgoing > 6 && type != ContainerType.Bank)
			{
				return false;
			}
			return true;
		}

		// Token: 0x06004EA7 RID: 20135 RVA: 0x000751DB File Offset: 0x000733DB
		public static bool AllowCurrencyDepositWithrdaw(this ContainerType type)
		{
			return type == ContainerType.PersonalBank || type == ContainerType.TradeOutgoing || type == ContainerType.Bank;
		}

		// Token: 0x06004EA8 RID: 20136 RVA: 0x001C4310 File Offset: 0x001C2510
		public static bool AllowPlacement(this ContainerType type)
		{
			switch (type)
			{
			case ContainerType.Equipment:
			case ContainerType.Inventory:
			case ContainerType.Pouch:
			case ContainerType.ReagentPouch:
			case ContainerType.PersonalBank:
			case ContainerType.Gathering:
			case ContainerType.Abilities:
				break;
			case (ContainerType)3:
			case ContainerType.LostAndFound:
			case (ContainerType)9:
			case ContainerType.Masteries:
				return false;
			default:
				switch (type)
				{
				case ContainerType.TradeOutgoing:
				case ContainerType.MerchantOutgoing:
				case ContainerType.BlacksmithOutgoing:
				case ContainerType.RuneCollector:
				case ContainerType.PostOutgoing:
				case ContainerType.AuctionOutgoing:
				case ContainerType.RefinementInput:
					break;
				case ContainerType.TradeIncoming:
				case ContainerType.PostIncoming:
				case ContainerType.Inspection:
					return false;
				default:
					if (type != ContainerType.Bank)
					{
						return false;
					}
					break;
				}
				break;
			}
			return true;
		}

		// Token: 0x06004EA9 RID: 20137 RVA: 0x000751EE File Offset: 0x000733EE
		public static bool ShowContextMenuForRightClick(this ContainerType type)
		{
			return type - ContainerType.Masteries <= 1;
		}

		// Token: 0x06004EAA RID: 20138 RVA: 0x001C438C File Offset: 0x001C258C
		public static bool AllowSplitting(this ContainerType type)
		{
			switch (type)
			{
			case ContainerType.Pouch:
			case ContainerType.ReagentPouch:
			case ContainerType.LostAndFound:
			case ContainerType.Masteries:
			case ContainerType.Abilities:
				break;
			case ContainerType.PersonalBank:
			case ContainerType.Gathering:
			case (ContainerType)9:
				return true;
			default:
				switch (type)
				{
				case ContainerType.Loot:
				case ContainerType.TradeIncoming:
				case ContainerType.RuneCollector:
				case ContainerType.PostOutgoing:
				case ContainerType.PostIncoming:
				case ContainerType.AuctionOutgoing:
					break;
				case ContainerType.TradeOutgoing:
				case ContainerType.MerchantOutgoing:
				case ContainerType.BlacksmithOutgoing:
					return true;
				default:
					if (type != ContainerType.Recipes)
					{
						return true;
					}
					break;
				}
				break;
			}
			return false;
		}

		// Token: 0x06004EAB RID: 20139 RVA: 0x000751FA File Offset: 0x000733FA
		public static bool AllowSplitToSelf(this ContainerType type)
		{
			return type == ContainerType.Inventory || type - ContainerType.PersonalBank <= 1 || type == ContainerType.Bank;
		}

		// Token: 0x06004EAC RID: 20140 RVA: 0x0004479C File Offset: 0x0004299C
		public static bool AllowRemoval(this ContainerType type)
		{
			return true;
		}

		// Token: 0x06004EAD RID: 20141 RVA: 0x00045BCA File Offset: 0x00043DCA
		public static bool AllowReplacement(this ContainerType type)
		{
			return false;
		}

		// Token: 0x06004EAE RID: 20142 RVA: 0x0007520E File Offset: 0x0007340E
		public static bool AllowDestruction(this ContainerType type)
		{
			return type - ContainerType.Equipment <= 1 || type - ContainerType.Pouch <= 4 || type == ContainerType.Bank;
		}

		// Token: 0x06004EAF RID: 20143 RVA: 0x00075224 File Offset: 0x00073424
		public static bool AllowConsumableUse(this ContainerType type)
		{
			return type == ContainerType.Inventory || type == ContainerType.Pouch || type == ContainerType.LostAndFound;
		}

		// Token: 0x06004EB0 RID: 20144 RVA: 0x00075235 File Offset: 0x00073435
		public static bool SaveToStorage(this ContainerType type)
		{
			return type == ContainerType.Bank;
		}

		// Token: 0x06004EB1 RID: 20145 RVA: 0x0007523F File Offset: 0x0007343F
		public static bool DestroyContentsOnClose(this ContainerType type)
		{
			return type != ContainerType.Loot && (type == ContainerType.Bank || type.IsLocal());
		}

		// Token: 0x06004EB2 RID: 20146 RVA: 0x00075257 File Offset: 0x00073457
		public static bool LockedInCombatStance(this ContainerType type)
		{
			return type == ContainerType.Equipment || type == ContainerType.Abilities;
		}

		// Token: 0x06004EB3 RID: 20147 RVA: 0x001C43F8 File Offset: 0x001C25F8
		public static bool LockedInDeath(this ContainerType type)
		{
			switch (type)
			{
			case ContainerType.Equipment:
			case ContainerType.Pouch:
			case ContainerType.ReagentPouch:
			case ContainerType.PersonalBank:
			case ContainerType.Gathering:
			case ContainerType.LostAndFound:
			case ContainerType.Masteries:
			case ContainerType.Abilities:
				break;
			case ContainerType.Inventory:
			case (ContainerType)3:
			case (ContainerType)9:
				return true;
			default:
				switch (type)
				{
				case ContainerType.Loot:
				case ContainerType.MerchantOutgoing:
				case ContainerType.PostOutgoing:
				case ContainerType.PostIncoming:
				case ContainerType.AuctionOutgoing:
					break;
				case ContainerType.TradeOutgoing:
				case ContainerType.TradeIncoming:
				case ContainerType.BlacksmithOutgoing:
				case ContainerType.RuneCollector:
					return true;
				default:
					if (type != ContainerType.Bank)
					{
						return true;
					}
					break;
				}
				break;
			}
			return false;
		}

		// Token: 0x06004EB4 RID: 20148 RVA: 0x00075265 File Offset: 0x00073465
		public static bool LockedWhenNotAlive(this ContainerType type)
		{
			return type != ContainerType.Inventory && type != ContainerType.Gathering;
		}

		// Token: 0x06004EB5 RID: 20149 RVA: 0x00075272 File Offset: 0x00073472
		public static bool UpdateOutgoingCurrency(this ContainerType type)
		{
			return type - ContainerType.MerchantOutgoing <= 2;
		}

		// Token: 0x06004EB6 RID: 20150 RVA: 0x0007527E File Offset: 0x0007347E
		public static bool RequireShiftToMove(this ContainerType type)
		{
			return type - ContainerType.Pouch <= 1;
		}

		// Token: 0x06004EB7 RID: 20151 RVA: 0x00075289 File Offset: 0x00073489
		public static bool CanRepairFrom(this ContainerType type)
		{
			return type.IsLocal();
		}

		// Token: 0x06004EB8 RID: 20152 RVA: 0x00075289 File Offset: 0x00073489
		public static bool CanSellFrom(this ContainerType type)
		{
			return type.IsLocal();
		}

		// Token: 0x06004EB9 RID: 20153 RVA: 0x00075291 File Offset: 0x00073491
		public static bool CanDeconstructFrom(this ContainerType type)
		{
			return type == ContainerType.Inventory;
		}

		// Token: 0x06004EBA RID: 20154 RVA: 0x00075297 File Offset: 0x00073497
		public static bool TransferContentsToInventoryOnEndInteraction(this ContainerType type)
		{
			return type != ContainerType.PersonalBank && type != ContainerType.LostAndFound;
		}

		// Token: 0x06004EBB RID: 20155 RVA: 0x001C4470 File Offset: 0x001C2670
		public static bool IsValidInstanceForContainer(this ContainerType type, ArchetypeInstance instance)
		{
			if (type == ContainerType.None || instance == null || !instance.Archetype || instance.Archetype.NpcOnly)
			{
				return false;
			}
			switch (type)
			{
			case ContainerType.Equipment:
			{
				EquipableItem equipableItem;
				return InternalGameDatabase.Archetypes.TryGetAsType<EquipableItem>(instance.ArchetypeId, out equipableItem);
			}
			case ContainerType.Inventory:
			case (ContainerType)3:
			case ContainerType.Pouch:
			case ContainerType.ReagentPouch:
			case ContainerType.PersonalBank:
			case ContainerType.Gathering:
			case ContainerType.LostAndFound:
			case (ContainerType)9:
				break;
			case ContainerType.Masteries:
			{
				MasteryArchetype masteryArchetype;
				return InternalGameDatabase.Archetypes.TryGetAsType<MasteryArchetype>(instance.ArchetypeId, out masteryArchetype);
			}
			case ContainerType.Abilities:
			{
				AbilityArchetype abilityArchetype;
				return InternalGameDatabase.Archetypes.TryGetAsType<AbilityArchetype>(instance.ArchetypeId, out abilityArchetype);
			}
			default:
				if (type != ContainerType.Bank)
				{
				}
				break;
			}
			ItemArchetype itemArchetype;
			return InternalGameDatabase.Archetypes.TryGetAsType<ItemArchetype>(instance.ArchetypeId, out itemArchetype);
		}

		// Token: 0x06004EBC RID: 20156 RVA: 0x000752A4 File Offset: 0x000734A4
		public static bool IsValidIndexForContainer(this ContainerType type, ArchetypeInstance instance)
		{
			if (instance == null)
			{
				return false;
			}
			if (!type.ValidateIndexesForContainer())
			{
				return true;
			}
			if (type != ContainerType.Equipment)
			{
				return type - ContainerType.Masteries <= 1 || instance.Index < type.GetMaxCapacity();
			}
			return EquipmentExtensions.ValidEquipmentSlotIndexes.Contains(instance.Index);
		}

		// Token: 0x06004EBD RID: 20157 RVA: 0x000752E3 File Offset: 0x000734E3
		public static bool ValidateIndexesForContainer(this ContainerType type)
		{
			return type - ContainerType.Equipment <= 1 || type - ContainerType.Pouch <= 3;
		}

		// Token: 0x06004EBE RID: 20158 RVA: 0x000752F4 File Offset: 0x000734F4
		public static bool RequiresSymbolicLinkForPlacement(this ContainerType type)
		{
			return type == ContainerType.TradeOutgoing || type == ContainerType.PostOutgoing || type == ContainerType.AuctionOutgoing;
		}

		// Token: 0x06004EBF RID: 20159 RVA: 0x00075308 File Offset: 0x00073508
		public static bool CanContainSymbolicLinks(this ContainerType type)
		{
			return type == ContainerType.Inventory || type == ContainerType.Gathering;
		}

		// Token: 0x06004EC0 RID: 20160 RVA: 0x00075315 File Offset: 0x00073515
		public static bool AllowInternalSwap(this ContainerType type)
		{
			return type != ContainerType.Equipment && type != ContainerType.Abilities;
		}

		// Token: 0x17001158 RID: 4440
		// (get) Token: 0x06004EC1 RID: 20161 RVA: 0x00075323 File Offset: 0x00073523
		private static object[] InvalidLootInstanceArguments
		{
			get
			{
				if (ContainerTypeExtensions.m_invalidLootInstanceArguments == null)
				{
					ContainerTypeExtensions.m_invalidLootInstanceArguments = new object[2];
				}
				return ContainerTypeExtensions.m_invalidLootInstanceArguments;
			}
		}

		// Token: 0x06004EC2 RID: 20162 RVA: 0x001C4528 File Offset: 0x001C2728
		public static bool IsValidInstanceForLootContainer(ArchetypeInstance instance)
		{
			if (instance == null)
			{
				ContainerTypeExtensions.InvalidLootInstanceArguments[0] = "NULL_INSTANCE";
				ContainerTypeExtensions.InvalidLootInstanceArguments[1] = "NullInstance";
				SolDebug.LogToIndex(LogLevel.Error, LogIndex.Error, "Invalid item being added to loot! {@ItemId} ({@Reason})", ContainerTypeExtensions.InvalidLootInstanceArguments);
				return false;
			}
			ItemArchetype itemArchetype;
			if (!InternalGameDatabase.Archetypes.TryGetAsType<ItemArchetype>(instance.ArchetypeId, out itemArchetype))
			{
				ContainerTypeExtensions.InvalidLootInstanceArguments[0] = instance.ArchetypeId.Value;
				ContainerTypeExtensions.InvalidLootInstanceArguments[1] = "NonValidItemArchetype";
				SolDebug.LogToIndex(LogLevel.Error, LogIndex.Error, "Invalid item being added to loot! {@ItemId} ({@Reason})", ContainerTypeExtensions.InvalidLootInstanceArguments);
				return false;
			}
			if (itemArchetype != null && itemArchetype.NpcOnly)
			{
				ContainerTypeExtensions.InvalidLootInstanceArguments[0] = instance.ArchetypeId.Value;
				ContainerTypeExtensions.InvalidLootInstanceArguments[1] = "NpcOnly";
				SolDebug.LogToIndex(LogLevel.Error, LogIndex.Error, "Invalid item being added to loot! {@ItemId} ({@Reason})", ContainerTypeExtensions.InvalidLootInstanceArguments);
				return false;
			}
			return true;
		}

		// Token: 0x040047C6 RID: 18374
		public static readonly ContainerType[] ContainerTypesToValidate = new ContainerType[]
		{
			ContainerType.Equipment,
			ContainerType.Inventory,
			ContainerType.Pouch,
			ContainerType.ReagentPouch,
			ContainerType.PersonalBank,
			ContainerType.Gathering,
			ContainerType.LostAndFound,
			ContainerType.Masteries,
			ContainerType.Abilities
		};

		// Token: 0x040047C7 RID: 18375
		private const string kInvalidTemplate = "Invalid item being added to loot! {@ItemId} ({@Reason})";

		// Token: 0x040047C8 RID: 18376
		private static object[] m_invalidLootInstanceArguments = null;
	}
}

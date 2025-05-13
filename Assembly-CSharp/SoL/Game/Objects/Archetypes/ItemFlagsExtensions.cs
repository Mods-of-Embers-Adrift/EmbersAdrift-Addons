using System;
using System.Collections.Generic;
using Cysharp.Text;
using SoL.Game.Messages;
using SoL.Managers;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A88 RID: 2696
	public static class ItemFlagsExtensions
	{
		// Token: 0x17001307 RID: 4871
		// (get) Token: 0x06005381 RID: 21377 RVA: 0x00077BE1 File Offset: 0x00075DE1
		public static ItemFlags[] AllItemFlags
		{
			get
			{
				if (ItemFlagsExtensions.m_allItemFlags == null)
				{
					ItemFlagsExtensions.m_allItemFlags = (ItemFlags[])Enum.GetValues(typeof(ItemFlags));
				}
				return ItemFlagsExtensions.m_allItemFlags;
			}
		}

		// Token: 0x06005382 RID: 21378 RVA: 0x00077C08 File Offset: 0x00075E08
		public static bool IsSoulbound(this ItemFlags flags)
		{
			return flags.HasBitFlag(ItemFlags.NoTrade) && flags.HasBitFlag(ItemFlags.NoSharedBank);
		}

		// Token: 0x06005383 RID: 21379 RVA: 0x0004FB40 File Offset: 0x0004DD40
		public static bool HasBitFlag(this ItemFlags a, ItemFlags b)
		{
			return (a & b) == b;
		}

		// Token: 0x06005384 RID: 21380 RVA: 0x000578B5 File Offset: 0x00055AB5
		public static ItemFlags SetBitFlag(this ItemFlags a, ItemFlags b)
		{
			return a | b;
		}

		// Token: 0x06005385 RID: 21381 RVA: 0x000578BA File Offset: 0x00055ABA
		public static ItemFlags UnsetBitFlag(this ItemFlags a, ItemFlags b)
		{
			return a & ~b;
		}

		// Token: 0x06005386 RID: 21382 RVA: 0x0004479C File Offset: 0x0004299C
		public static bool AllowDestruction(this ItemFlags flags)
		{
			return true;
		}

		// Token: 0x06005387 RID: 21383 RVA: 0x00077C1C File Offset: 0x00075E1C
		public static bool CanBeSold(this ItemFlags flags)
		{
			return !flags.HasBitFlag(ItemFlags.NoSale) && !flags.HasBitFlag(ItemFlags.Crafted);
		}

		// Token: 0x06005388 RID: 21384 RVA: 0x001D8214 File Offset: 0x001D6414
		public static bool TryGetItemFlagColor(this ItemFlags flag, out Color color)
		{
			color = Color.white;
			if (flag <= ItemFlags.Quest)
			{
				if (flag - ItemFlags.NoTrade > 2)
				{
					if (flag != ItemFlags.Quest)
					{
						return false;
					}
					return MessageType.Quest.GetColor(out color, false);
				}
			}
			else
			{
				if (flag == ItemFlags.Crafted)
				{
					color = Colors.GreenCrayola;
					return true;
				}
				if (flag != ItemFlags.NoSale)
				{
					return false;
				}
			}
			color = UIManager.RedColor;
			return true;
		}

		// Token: 0x06005389 RID: 21385 RVA: 0x001D8270 File Offset: 0x001D6470
		private static string GetTooltipDescription(this ItemFlags flags)
		{
			switch (flags)
			{
			case ItemFlags.NoTrade:
				return "No Trade";
			case ItemFlags.NoSharedBank:
				return "No Shared Bank";
			case ItemFlags.Soulbound:
				return "Soulbound";
			case ItemFlags.Quest:
				return "Quest";
			default:
				if (flags == ItemFlags.Crafted)
				{
					return "Crafted";
				}
				if (flags != ItemFlags.NoSale)
				{
					return string.Empty;
				}
				return "No Sale";
			}
		}

		// Token: 0x0600538A RID: 21386 RVA: 0x001D82CC File Offset: 0x001D64CC
		private static void AddItemFlagDescription(ItemFlags flag)
		{
			Color color;
			if (flag.TryGetItemFlagColor(out color))
			{
				string arg = color.ToHex();
				ItemFlagsExtensions.m_itemFlagDescriptions.Add(ZString.Format<string, string>("<color={0}>{1}</color>", arg, flag.GetTooltipDescription()));
				return;
			}
			ItemFlagsExtensions.m_itemFlagDescriptions.Add(flag.GetTooltipDescription());
		}

		// Token: 0x0600538B RID: 21387 RVA: 0x001D8318 File Offset: 0x001D6518
		public static string GetColoredItemFlags(this ItemFlags flags)
		{
			if (ItemFlagsExtensions.m_itemFlagDescriptions == null)
			{
				ItemFlagsExtensions.m_itemFlagDescriptions = new List<string>(100);
			}
			else
			{
				ItemFlagsExtensions.m_itemFlagDescriptions.Clear();
			}
			bool flag = flags.IsSoulbound();
			for (int i = 0; i < ItemFlagsExtensions.AllItemFlags.Length; i++)
			{
				if (ItemFlagsExtensions.AllItemFlags[i] != ItemFlags.None && ItemFlagsExtensions.AllItemFlags[i] != ItemFlags.Soulbound && flags.HasBitFlag(ItemFlagsExtensions.AllItemFlags[i]) && (!flag || (ItemFlagsExtensions.AllItemFlags[i] != ItemFlags.NoTrade && ItemFlagsExtensions.AllItemFlags[i] != ItemFlags.NoSharedBank)))
				{
					ItemFlagsExtensions.AddItemFlagDescription(ItemFlagsExtensions.AllItemFlags[i]);
				}
			}
			if (flag)
			{
				ItemFlagsExtensions.AddItemFlagDescription(ItemFlags.Soulbound);
			}
			if (ItemFlagsExtensions.m_itemFlagDescriptions.Count <= 0)
			{
				return string.Empty;
			}
			if (ItemFlagsExtensions.m_itemFlagDescriptions.Count <= 1)
			{
				return ItemFlagsExtensions.m_itemFlagDescriptions[0];
			}
			return ZString.Join<string>(", ", ItemFlagsExtensions.m_itemFlagDescriptions);
		}

		// Token: 0x04004A8F RID: 19087
		private static ItemFlags[] m_allItemFlags;

		// Token: 0x04004A90 RID: 19088
		private static List<string> m_itemFlagDescriptions;
	}
}

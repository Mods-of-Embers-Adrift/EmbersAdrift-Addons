using System;
using SoL.Game.Crafting;
using SoL.Game.Settings;
using SoL.Utilities;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A6E RID: 2670
	public static class CraftingToolTypeExtensions
	{
		// Token: 0x060052A1 RID: 21153 RVA: 0x00077264 File Offset: 0x00075464
		public static bool TryGetAbilityForToolType(this CraftingToolType toolType, out GatheringAbility ability)
		{
			ability = null;
			return GlobalSettings.Values != null && GlobalSettings.Values.Gathering != null && GlobalSettings.Values.Gathering.TryGetAbilityForToolType(toolType, out ability);
		}

		// Token: 0x060052A2 RID: 21154 RVA: 0x00077295 File Offset: 0x00075495
		public static UniqueId GetAbilityIdForToolType(this CraftingToolType toolType)
		{
			if (!(GlobalSettings.Values != null) || GlobalSettings.Values.Gathering == null)
			{
				return UniqueId.Empty;
			}
			return GlobalSettings.Values.Gathering.GetAbilityIdForToolType(toolType);
		}

		// Token: 0x060052A3 RID: 21155 RVA: 0x000772C6 File Offset: 0x000754C6
		public static string GetTooltipDescription(this CraftingToolType toolType)
		{
			switch (toolType)
			{
			case CraftingToolType.Axe:
				return "Axe";
			case CraftingToolType.PickAxe:
				return "Pick Axe";
			case CraftingToolType.SkinningKnife:
				return "Skinning Knife";
			case CraftingToolType.Sickle:
				return "Sickle";
			default:
				return "Unknown";
			}
		}

		// Token: 0x060052A4 RID: 21156 RVA: 0x001D4678 File Offset: 0x001D2878
		public static CursorType GetCursorForTool(this CraftingToolType toolType, bool isActive)
		{
			switch (toolType)
			{
			case CraftingToolType.Axe:
				if (!isActive)
				{
					return CursorType.AxeInactive;
				}
				return CursorType.Axe;
			case CraftingToolType.PickAxe:
				if (!isActive)
				{
					return CursorType.PickAxeInactive;
				}
				return CursorType.PickAxe;
			case CraftingToolType.SkinningKnife:
				if (!isActive)
				{
					return CursorType.KnifeInactive;
				}
				return CursorType.Knife;
			case CraftingToolType.Sickle:
				if (!isActive)
				{
					return CursorType.SickleInactive;
				}
				return CursorType.Sickle;
			default:
				return CursorType.MainCursor;
			}
		}
	}
}

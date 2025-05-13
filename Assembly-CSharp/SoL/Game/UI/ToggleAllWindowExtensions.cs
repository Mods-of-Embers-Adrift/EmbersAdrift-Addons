using System;
using System.Collections.Generic;
using SoL.Managers;
using SoL.UI;
using SoL.Utilities;

namespace SoL.Game.UI
{
	// Token: 0x020008C6 RID: 2246
	public static class ToggleAllWindowExtensions
	{
		// Token: 0x17000EFD RID: 3837
		// (get) Token: 0x060041B4 RID: 16820 RVA: 0x0006C663 File Offset: 0x0006A863
		private static ToggleAllWindowType[] WindowTypes
		{
			get
			{
				if (ToggleAllWindowExtensions.m_windowTypes == null)
				{
					ToggleAllWindowExtensions.m_windowTypes = (ToggleAllWindowType[])Enum.GetValues(typeof(ToggleAllWindowType));
				}
				return ToggleAllWindowExtensions.m_windowTypes;
			}
		}

		// Token: 0x060041B5 RID: 16821 RVA: 0x0004FB40 File Offset: 0x0004DD40
		public static bool HasBitFlag(this ToggleAllWindowFlags a, ToggleAllWindowFlags b)
		{
			return (a & b) == b;
		}

		// Token: 0x060041B6 RID: 16822 RVA: 0x001901B4 File Offset: 0x0018E3B4
		public static ToggleAllWindowFlags GetFlag(this ToggleAllWindowType type)
		{
			ToggleAllWindowFlags result = ToggleAllWindowFlags.None;
			switch (type)
			{
			case ToggleAllWindowType.Time:
				result = ToggleAllWindowFlags.Time;
				break;
			case ToggleAllWindowType.Bag:
				result = ToggleAllWindowFlags.Bag;
				break;
			case ToggleAllWindowType.Gathering:
				result = ToggleAllWindowFlags.Gathering;
				break;
			case ToggleAllWindowType.Inventory:
				result = ToggleAllWindowFlags.Inventory;
				break;
			case ToggleAllWindowType.Skills:
				result = ToggleAllWindowFlags.Skills;
				break;
			case ToggleAllWindowType.Recipes:
				result = ToggleAllWindowFlags.Recipes;
				break;
			case ToggleAllWindowType.Log:
				result = ToggleAllWindowFlags.Log;
				break;
			case ToggleAllWindowType.Map:
				result = ToggleAllWindowFlags.Map;
				break;
			case ToggleAllWindowType.Social:
				result = ToggleAllWindowFlags.Social;
				break;
			case ToggleAllWindowType.Notifications:
				result = ToggleAllWindowFlags.Notifications;
				break;
			}
			return result;
		}

		// Token: 0x060041B7 RID: 16823 RVA: 0x0019022C File Offset: 0x0018E42C
		private static bool TryGetWindow(this ToggleAllWindowType type, out UIWindow window)
		{
			window = null;
			if (!ClientGameManager.UIManager)
			{
				return false;
			}
			switch (type)
			{
			case ToggleAllWindowType.Time:
				if (ClientGameManager.UIManager.TimeUI)
				{
					window = ClientGameManager.UIManager.TimeUI.Window;
				}
				break;
			case ToggleAllWindowType.Bag:
				if (ClientGameManager.UIManager.Inventory)
				{
					window = ClientGameManager.UIManager.Inventory.Window;
				}
				break;
			case ToggleAllWindowType.Gathering:
				if (ClientGameManager.UIManager.Gathering)
				{
					window = ClientGameManager.UIManager.Gathering.Window;
				}
				break;
			case ToggleAllWindowType.Inventory:
				if (ClientGameManager.UIManager.EquipmentStats)
				{
					window = ClientGameManager.UIManager.EquipmentStats;
				}
				break;
			case ToggleAllWindowType.Skills:
				if (ClientGameManager.UIManager.SkillsUI)
				{
					window = ClientGameManager.UIManager.SkillsUI;
				}
				break;
			case ToggleAllWindowType.Recipes:
				if (ClientGameManager.UIManager.CraftingUI)
				{
					window = ClientGameManager.UIManager.CraftingUI;
				}
				break;
			case ToggleAllWindowType.Log:
				if (ClientGameManager.UIManager.LogUI)
				{
					window = ClientGameManager.UIManager.LogUI;
				}
				break;
			case ToggleAllWindowType.Map:
				if (ClientGameManager.UIManager.MapUI)
				{
					window = ClientGameManager.UIManager.MapUI.Window;
				}
				break;
			case ToggleAllWindowType.Social:
				if (ClientGameManager.UIManager.SocialUI)
				{
					window = ClientGameManager.UIManager.SocialUI;
				}
				break;
			}
			return window != null;
		}

		// Token: 0x060041B8 RID: 16824 RVA: 0x001903C8 File Offset: 0x0018E5C8
		public static void ToggleAllPressed()
		{
			ToggleAllWindowFlags value = (ToggleAllWindowFlags)Options.GameOptions.ToggleAllWindows.Value;
			List<UIWindow> fromPool = StaticListPool<UIWindow>.GetFromPool();
			bool flag = false;
			for (int i = 0; i < ToggleAllWindowExtensions.WindowTypes.Length; i++)
			{
				ToggleAllWindowFlags flag2 = ToggleAllWindowExtensions.WindowTypes[i].GetFlag();
				UIWindow uiwindow;
				if (value.HasBitFlag(flag2) && ToggleAllWindowExtensions.WindowTypes[i].TryGetWindow(out uiwindow))
				{
					fromPool.Add(uiwindow);
					flag = (flag || uiwindow.Visible);
				}
			}
			for (int j = 0; j < fromPool.Count; j++)
			{
				if (fromPool[j])
				{
					if (flag && fromPool[j].Visible)
					{
						fromPool[j].Hide(false);
					}
					else if (!flag && !fromPool[j].Visible)
					{
						fromPool[j].Show(false);
					}
				}
			}
			StaticListPool<UIWindow>.ReturnToPool(fromPool);
		}

		// Token: 0x04003EFB RID: 16123
		private static ToggleAllWindowType[] m_windowTypes;
	}
}

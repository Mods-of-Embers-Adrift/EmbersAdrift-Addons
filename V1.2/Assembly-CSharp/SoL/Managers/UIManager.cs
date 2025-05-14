using System;
using System.Collections;
using System.Collections.Generic;
using SoL.Game;
using SoL.Game.AuctionHouse;
using SoL.Game.Audio;
using SoL.Game.Crafting;
using SoL.Game.GM;
using SoL.Game.Objects.Containers;
using SoL.Game.Settings;
using SoL.Game.SkyDome;
using SoL.Game.Targeting;
using SoL.Game.UI;
using SoL.Game.UI.Archetypes;
using SoL.Game.UI.Chat;
using SoL.Game.UI.Crafting;
using SoL.Game.UI.Dialog;
using SoL.Game.UI.Loot;
using SoL.Game.UI.Macros;
using SoL.Game.UI.Merchants;
using SoL.Game.UI.Notifications;
using SoL.Game.UI.Penalties;
using SoL.Game.UI.Quests;
using SoL.Game.UI.Recipes;
using SoL.Game.UI.Skills;
using SoL.Game.UI.Social;
using SoL.UI;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SoL.Managers
{
	// Token: 0x02000517 RID: 1303
	public class UIManager : MonoBehaviour
	{
		// Token: 0x14000071 RID: 113
		// (add) Token: 0x0600263A RID: 9786 RVA: 0x0013527C File Offset: 0x0013347C
		// (remove) Token: 0x0600263B RID: 9787 RVA: 0x001352B4 File Offset: 0x001334B4
		public event Action ResetUIEvent;

		// Token: 0x14000072 RID: 114
		// (add) Token: 0x0600263C RID: 9788 RVA: 0x001352EC File Offset: 0x001334EC
		// (remove) Token: 0x0600263D RID: 9789 RVA: 0x00135320 File Offset: 0x00133520
		public static event Action UiHiddenChanged;

		// Token: 0x170007BD RID: 1981
		// (get) Token: 0x0600263E RID: 9790 RVA: 0x0005AF16 File Offset: 0x00059116
		// (set) Token: 0x0600263F RID: 9791 RVA: 0x0005AF1D File Offset: 0x0005911D
		public static bool UiHidden
		{
			get
			{
				return UIManager.m_UiHidden;
			}
			private set
			{
				if (UIManager.m_UiHidden == value)
				{
					return;
				}
				UIManager.m_UiHidden = value;
				Action uiHiddenChanged = UIManager.UiHiddenChanged;
				if (uiHiddenChanged == null)
				{
					return;
				}
				uiHiddenChanged();
			}
		}

		// Token: 0x14000073 RID: 115
		// (add) Token: 0x06002640 RID: 9792 RVA: 0x00135354 File Offset: 0x00133554
		// (remove) Token: 0x06002641 RID: 9793 RVA: 0x00135388 File Offset: 0x00133588
		public static event Action AvailableCurrencyChanged;

		// Token: 0x06002642 RID: 9794 RVA: 0x0005AF3D File Offset: 0x0005913D
		public static void InvokeAvailableCurrencyChanged()
		{
			Action availableCurrencyChanged = UIManager.AvailableCurrencyChanged;
			if (availableCurrencyChanged == null)
			{
				return;
			}
			availableCurrencyChanged();
		}

		// Token: 0x14000074 RID: 116
		// (add) Token: 0x06002643 RID: 9795 RVA: 0x001353BC File Offset: 0x001335BC
		// (remove) Token: 0x06002644 RID: 9796 RVA: 0x001353F0 File Offset: 0x001335F0
		public static event Action<ContainerType> ItemAddedToContainer;

		// Token: 0x06002645 RID: 9797 RVA: 0x0005AF4E File Offset: 0x0005914E
		public static void InvokeItemAddedToContainer(ContainerType containerType)
		{
			Action<ContainerType> itemAddedToContainer = UIManager.ItemAddedToContainer;
			if (itemAddedToContainer == null)
			{
				return;
			}
			itemAddedToContainer(containerType);
		}

		// Token: 0x14000075 RID: 117
		// (add) Token: 0x06002646 RID: 9798 RVA: 0x00135424 File Offset: 0x00133624
		// (remove) Token: 0x06002647 RID: 9799 RVA: 0x00135458 File Offset: 0x00133658
		internal static event Action<WindowToggler.WindowType> TriggerControlPanelUsageHighlight;

		// Token: 0x06002648 RID: 9800 RVA: 0x0005AF60 File Offset: 0x00059160
		public static void InvokeTriggerControlPanelUsageHighlight(WindowToggler.WindowType windowType)
		{
			Action<WindowToggler.WindowType> triggerControlPanelUsageHighlight = UIManager.TriggerControlPanelUsageHighlight;
			if (triggerControlPanelUsageHighlight == null)
			{
				return;
			}
			triggerControlPanelUsageHighlight(windowType);
		}

		// Token: 0x14000076 RID: 118
		// (add) Token: 0x06002649 RID: 9801 RVA: 0x0013548C File Offset: 0x0013368C
		// (remove) Token: 0x0600264A RID: 9802 RVA: 0x001354C0 File Offset: 0x001336C0
		public static event Action AbilityContainerChanged;

		// Token: 0x0600264B RID: 9803 RVA: 0x0005AF72 File Offset: 0x00059172
		public static void InvokeAbilityContainerChanged()
		{
			Action abilityContainerChanged = UIManager.AbilityContainerChanged;
			if (abilityContainerChanged == null)
			{
				return;
			}
			abilityContainerChanged();
		}

		// Token: 0x14000077 RID: 119
		// (add) Token: 0x0600264C RID: 9804 RVA: 0x001354F4 File Offset: 0x001336F4
		// (remove) Token: 0x0600264D RID: 9805 RVA: 0x00135528 File Offset: 0x00133728
		public static event Action EventCurrencyChanged;

		// Token: 0x0600264E RID: 9806 RVA: 0x0005AF83 File Offset: 0x00059183
		public static void InvokeEventCurrencyChanged()
		{
			Action eventCurrencyChanged = UIManager.EventCurrencyChanged;
			if (eventCurrencyChanged == null)
			{
				return;
			}
			eventCurrencyChanged();
		}

		// Token: 0x170007BE RID: 1982
		// (get) Token: 0x0600264F RID: 9807 RVA: 0x0005AF94 File Offset: 0x00059194
		public static bool TooltipShowMore
		{
			get
			{
				return ClientGameManager.InputManager != null && ClientGameManager.InputManager.HoldingAlt;
			}
		}

		// Token: 0x170007BF RID: 1983
		// (get) Token: 0x06002650 RID: 9808 RVA: 0x0005AFA9 File Offset: 0x000591A9
		public static bool TooltipShowNext
		{
			get
			{
				return ClientGameManager.InputManager != null && ClientGameManager.InputManager.HoldingShift;
			}
		}

		// Token: 0x170007C0 RID: 1984
		// (get) Token: 0x06002651 RID: 9809 RVA: 0x0005AFBE File Offset: 0x000591BE
		// (set) Token: 0x06002652 RID: 9810 RVA: 0x0005AFC5 File Offset: 0x000591C5
		public static bool UIResetting { get; private set; } = false;

		// Token: 0x170007C1 RID: 1985
		// (get) Token: 0x06002653 RID: 9811 RVA: 0x0005AFCD File Offset: 0x000591CD
		public static Color RequirementsMetColor
		{
			get
			{
				return UIManager.BlueColor;
			}
		}

		// Token: 0x170007C2 RID: 1986
		// (get) Token: 0x06002654 RID: 9812 RVA: 0x0005AFD4 File Offset: 0x000591D4
		public static Color RequirementsNotMetColor
		{
			get
			{
				return UIManager.RedColor;
			}
		}

		// Token: 0x170007C3 RID: 1987
		// (get) Token: 0x06002655 RID: 9813 RVA: 0x0005AFDB File Offset: 0x000591DB
		public static Color ReagentBonusColor
		{
			get
			{
				return Colors.GreenCyan;
			}
		}

		// Token: 0x170007C4 RID: 1988
		// (get) Token: 0x06002656 RID: 9814 RVA: 0x0005AFE2 File Offset: 0x000591E2
		// (set) Token: 0x06002657 RID: 9815 RVA: 0x0005AFE9 File Offset: 0x000591E9
		public static float AlchemyPulseAlpha_I { get; private set; } = 1f;

		// Token: 0x170007C5 RID: 1989
		// (get) Token: 0x06002658 RID: 9816 RVA: 0x0005AFF1 File Offset: 0x000591F1
		// (set) Token: 0x06002659 RID: 9817 RVA: 0x0005AFF8 File Offset: 0x000591F8
		public static float AlchemyPulseAlpha_II { get; private set; } = 1f;

		// Token: 0x170007C6 RID: 1990
		// (get) Token: 0x0600265A RID: 9818 RVA: 0x0005B000 File Offset: 0x00059200
		// (set) Token: 0x0600265B RID: 9819 RVA: 0x0005B007 File Offset: 0x00059207
		public static float RepairPulseAlpha { get; private set; } = 1f;

		// Token: 0x170007C7 RID: 1991
		// (get) Token: 0x0600265C RID: 9820 RVA: 0x0005B00F File Offset: 0x0005920F
		public static Color SubscriberColor
		{
			get
			{
				return GlobalSettings.Values.Subscribers.SubscriberColor;
			}
		}

		// Token: 0x170007C8 RID: 1992
		// (get) Token: 0x0600265D RID: 9821 RVA: 0x0005B020 File Offset: 0x00059220
		public static Color TrialColor
		{
			get
			{
				return GlobalSettings.Values.UI.TrialColor;
			}
		}

		// Token: 0x170007C9 RID: 1993
		// (get) Token: 0x0600265E RID: 9822 RVA: 0x0005B031 File Offset: 0x00059231
		// (set) Token: 0x0600265F RID: 9823 RVA: 0x0005B038 File Offset: 0x00059238
		public static GroupWindowUI GroupWindowUI { get; set; }

		// Token: 0x170007CA RID: 1994
		// (get) Token: 0x06002660 RID: 9824 RVA: 0x0005B040 File Offset: 0x00059240
		// (set) Token: 0x06002661 RID: 9825 RVA: 0x0005B047 File Offset: 0x00059247
		public static RaidWindowUI RaidWindowUI { get; set; }

		// Token: 0x170007CB RID: 1995
		// (get) Token: 0x06002662 RID: 9826 RVA: 0x0005B04F File Offset: 0x0005924F
		// (set) Token: 0x06002663 RID: 9827 RVA: 0x0005B056 File Offset: 0x00059256
		public static LoadingScreenUI LoadingScreenUI { get; set; }

		// Token: 0x170007CC RID: 1996
		// (get) Token: 0x06002664 RID: 9828 RVA: 0x0005B05E File Offset: 0x0005925E
		// (set) Token: 0x06002665 RID: 9829 RVA: 0x0005B065 File Offset: 0x00059265
		public static EventSystem EventSystem { get; private set; }

		// Token: 0x170007CD RID: 1997
		// (get) Token: 0x06002666 RID: 9830 RVA: 0x0005B06D File Offset: 0x0005926D
		// (set) Token: 0x06002667 RID: 9831 RVA: 0x0005B074 File Offset: 0x00059274
		public static SolStandaloneInputModule InputModule { get; private set; }

		// Token: 0x170007CE RID: 1998
		// (get) Token: 0x06002668 RID: 9832 RVA: 0x0005B07C File Offset: 0x0005927C
		// (set) Token: 0x06002669 RID: 9833 RVA: 0x0005B083 File Offset: 0x00059283
		public static AutoAttackEventsUI AutoAttackButton { get; set; }

		// Token: 0x170007CF RID: 1999
		// (get) Token: 0x0600266A RID: 9834 RVA: 0x0005B08B File Offset: 0x0005928B
		// (set) Token: 0x0600266B RID: 9835 RVA: 0x0005B092 File Offset: 0x00059292
		public static ChatTabUI ActiveChatTabInput { get; set; }

		// Token: 0x170007D0 RID: 2000
		// (get) Token: 0x0600266C RID: 9836 RVA: 0x0005B09A File Offset: 0x0005929A
		public static List<ChatWindowUI> ChatWindows
		{
			get
			{
				return UIManager.m_chatWindows;
			}
		}

		// Token: 0x170007D1 RID: 2001
		// (get) Token: 0x0600266D RID: 9837 RVA: 0x0005B0A1 File Offset: 0x000592A1
		// (set) Token: 0x0600266E RID: 9838 RVA: 0x0005B0A8 File Offset: 0x000592A8
		public static EquippedRepairIcon EquippedRepairIcon { get; set; }

		// Token: 0x170007D2 RID: 2002
		// (get) Token: 0x0600266F RID: 9839 RVA: 0x0013555C File Offset: 0x0013375C
		public static bool IsChatActive
		{
			get
			{
				return UIManager.m_lastActiveChatWindow != null && UIManager.m_lastActiveChatTab != null && UIManager.m_lastActiveChatWindow.ActiveTab.Mode == ChatTabMode.Chat && UIManager.m_lastActiveChatWindow.InputActiveInHierarchy && (UIManager.m_lastActiveChatWindow.WasFocusedLastFrame || UIManager.m_lastActiveChatWindow.InputFocused);
			}
		}

		// Token: 0x170007D3 RID: 2003
		// (get) Token: 0x06002670 RID: 9840 RVA: 0x001355BC File Offset: 0x001337BC
		public static ChatWindowUI ActiveChatInput
		{
			get
			{
				if (UIManager.m_lastActiveChatWindow != null && UIManager.m_lastActiveChatTab != null && UIManager.m_lastActiveChatWindow.gameObject.activeInHierarchy)
				{
					if (!UIManager.m_lastActiveChatWindow.IsTabFocused(UIManager.m_lastActiveChatTab))
					{
						UIManager.m_lastActiveChatWindow.FocusTab(UIManager.m_lastActiveChatTab);
					}
					return UIManager.m_lastActiveChatWindow;
				}
				foreach (ChatWindowUI chatWindowUI in UIManager.m_chatWindows)
				{
					if (chatWindowUI.ActiveTab.Mode == ChatTabMode.Chat)
					{
						UIManager.m_lastActiveChatWindow = chatWindowUI;
						UIManager.m_lastActiveChatTab = chatWindowUI.ActiveTab;
						return UIManager.m_lastActiveChatWindow;
					}
				}
				foreach (ChatWindowUI chatWindowUI2 in UIManager.m_chatWindows)
				{
					if (chatWindowUI2.FocusChatTab())
					{
						UIManager.m_lastActiveChatWindow = chatWindowUI2;
						UIManager.m_lastActiveChatTab = chatWindowUI2.ActiveTab;
						return UIManager.m_lastActiveChatWindow;
					}
				}
				if (UIManager.m_lastActiveChatWindow != null && UIManager.m_lastActiveChatTab == null && UIManager.m_lastActiveChatWindow.gameObject.activeInHierarchy)
				{
					UIManager.m_lastActiveChatWindow.AddDefaultTab();
					UIManager.m_lastActiveChatTab = UIManager.m_lastActiveChatWindow.ActiveTab;
					return UIManager.m_lastActiveChatWindow;
				}
				UIManager.m_chatWindows[0].AddDefaultTab();
				UIManager.m_lastActiveChatWindow = UIManager.m_chatWindows[0];
				UIManager.m_lastActiveChatTab = UIManager.m_lastActiveChatWindow.ActiveTab;
				return UIManager.m_lastActiveChatWindow;
			}
		}

		// Token: 0x06002671 RID: 9841 RVA: 0x00135760 File Offset: 0x00133960
		public static ChatWindowUI GetChatWindowForMacro()
		{
			if (UIManager.m_lastActiveChatTab && UIManager.m_lastActiveChatTab.ParentWindow)
			{
				return UIManager.m_lastActiveChatTab.ParentWindow;
			}
			ChatWindowUI result = null;
			foreach (ChatWindowUI chatWindowUI in UIManager.m_chatWindows)
			{
				result = chatWindowUI;
				if (chatWindowUI && chatWindowUI.ActiveTab && chatWindowUI.ActiveTab.Mode == ChatTabMode.Chat)
				{
					return chatWindowUI;
				}
			}
			return result;
		}

		// Token: 0x06002672 RID: 9842 RVA: 0x0005B0B0 File Offset: 0x000592B0
		public static void SetLastActiveChat(ChatWindowUI window, ChatTab tab)
		{
			UIManager.m_lastActiveChatWindow = window;
			UIManager.m_lastActiveChatTab = tab;
		}

		// Token: 0x06002673 RID: 9843 RVA: 0x0005B0BE File Offset: 0x000592BE
		public static void UnregisterActiveChatInput(ChatWindowUI chatWindow)
		{
			if (UIManager.m_lastActiveChatWindow == chatWindow)
			{
				UIManager.m_lastActiveChatWindow = null;
			}
		}

		// Token: 0x06002674 RID: 9844 RVA: 0x0005B0D3 File Offset: 0x000592D3
		public static void RegisterContainerUI(IContainerUI containerUI)
		{
			UIManager.m_containers.AddOrReplace(containerUI.Id, containerUI);
		}

		// Token: 0x06002675 RID: 9845 RVA: 0x0005B0E6 File Offset: 0x000592E6
		public static void UnregisterContainerUI(IContainerUI containerUI)
		{
			if (!string.IsNullOrEmpty(containerUI.Id))
			{
				UIManager.m_containers.Remove(containerUI.Id);
			}
		}

		// Token: 0x06002676 RID: 9846 RVA: 0x0005B106 File Offset: 0x00059306
		public static void RegisterChatTab(ChatTabUI chatTab)
		{
			UIManager.m_chatTabs.Add(chatTab);
		}

		// Token: 0x06002677 RID: 9847 RVA: 0x0005B113 File Offset: 0x00059313
		public static void UnregisterChatTab(ChatTabUI chatTab)
		{
			UIManager.m_chatTabs.Remove(chatTab);
		}

		// Token: 0x06002678 RID: 9848 RVA: 0x0005B121 File Offset: 0x00059321
		public static void RegisterChatWindow(ChatWindowUI chatWindow)
		{
			UIManager.m_chatWindows.Add(chatWindow);
			chatWindow.PlayerPrefsKey = string.Format("{0}{1}", "ChatWindow_", UIManager.m_chatWindows.Count - 1);
		}

		// Token: 0x06002679 RID: 9849 RVA: 0x00135800 File Offset: 0x00133A00
		public static void UnregisterChatWindow(ChatWindowUI chatWindow)
		{
			UIManager.m_chatWindows.Remove(chatWindow);
			for (int i = 0; i < UIManager.m_chatWindows.Count; i++)
			{
				UIManager.m_chatWindows[i].PlayerPrefsKey = string.Format("{0}{1}", "ChatWindow_", i);
				UIManager.m_chatWindows[i].SaveSettings(false);
			}
		}

		// Token: 0x0600267A RID: 9850 RVA: 0x00135864 File Offset: 0x00133A64
		public static void LoadAdditionalSavedWindows()
		{
			int desiredWindowCount = ChatWindowUI.DesiredWindowCount;
			for (int i = 2; i < desiredWindowCount; i++)
			{
				ChatWindowUI chatWindowUI = UnityEngine.Object.Instantiate<ChatWindowUI>(UIManager.m_chatWindows[0], UIManager.m_chatWindows[0].transform.parent);
				chatWindowUI.TabController.RecoverOrphanedTabs();
				for (int j = 0; j < chatWindowUI.ChatList.Content.childCount; j++)
				{
					chatWindowUI.ChatList.Content.GetChild(j).gameObject.SetActive(false);
				}
				for (int k = 0; k < chatWindowUI.ChatTabFilterList.Content.childCount; k++)
				{
					chatWindowUI.ChatTabFilterList.Content.GetChild(k).gameObject.SetActive(false);
				}
				chatWindowUI.TabController.ClearTabs();
			}
		}

		// Token: 0x0600267B RID: 9851 RVA: 0x0005B154 File Offset: 0x00059354
		public static bool TryGetContainerUI(string id, out IContainerUI container)
		{
			return UIManager.m_containers.TryGetValue(id, out container);
		}

		// Token: 0x0600267C RID: 9852 RVA: 0x0005B162 File Offset: 0x00059362
		public static bool TryGetTooltipUI(TooltipType type, out BaseTooltip tooltip)
		{
			return UIManager.m_tooltips.TryGetValue(type, out tooltip);
		}

		// Token: 0x0600267D RID: 9853 RVA: 0x0005B170 File Offset: 0x00059370
		public static void RegisterUIWindow(UIWindow window)
		{
			UIManager.m_windows.Add(window);
		}

		// Token: 0x0600267E RID: 9854 RVA: 0x0005B17D File Offset: 0x0005937D
		public static void UnregisterUIWindow(UIWindow window)
		{
			UIManager.m_windows.Remove(window);
		}

		// Token: 0x0600267F RID: 9855 RVA: 0x0005B18B File Offset: 0x0005938B
		public static void UIWindowToFront(UIWindow window)
		{
			if (UIManager.m_windows.Remove(window))
			{
				UIManager.m_windows.Add(window);
			}
		}

		// Token: 0x06002680 RID: 9856 RVA: 0x0005B1A5 File Offset: 0x000593A5
		public static bool HelpEscapePressed()
		{
			if (ClientGameManager.UIManager.GameHelpPanel.Visible)
			{
				ClientGameManager.UIManager.GameHelpPanel.ToggleWindow();
				return true;
			}
			return false;
		}

		// Token: 0x06002681 RID: 9857 RVA: 0x0005B1CA File Offset: 0x000593CA
		public static bool MenuEscapePressed()
		{
			return ClientGameManager.UIManager.InGameUiOptions.EscapePressedFirstPass() || ClientGameManager.UIManager.InGameUiMenu.EscapePressedFirstPass();
		}

		// Token: 0x06002682 RID: 9858 RVA: 0x00135938 File Offset: 0x00133B38
		public static bool ChatTabEscapePressed()
		{
			for (int i = 0; i < UIManager.m_chatTabs.Count; i++)
			{
				if (UIManager.m_chatTabs[i] == null)
				{
					UIManager.m_chatTabs.RemoveAt(i);
					i--;
				}
				else if (UIManager.m_chatTabs[i].WasFocusedLastFrame)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06002683 RID: 9859 RVA: 0x00135994 File Offset: 0x00133B94
		public static bool ChatWindowEscapePressed()
		{
			for (int i = 0; i < UIManager.m_chatWindows.Count; i++)
			{
				if (UIManager.m_chatWindows[i] == null)
				{
					UIManager.m_chatWindows.RemoveAt(i);
					i--;
				}
				else if (UIManager.m_chatWindows[i].WasFocusedLastFrame)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06002684 RID: 9860 RVA: 0x001359F0 File Offset: 0x00133BF0
		public static bool EscapePressed()
		{
			if (UIManager.MenuEscapePressed())
			{
				return true;
			}
			if (ClientGameManager.UIManager.m_dragged != null)
			{
				ClientGameManager.UIManager.DeregisterDrag(true);
				return true;
			}
			if (ClientGameManager.UIManager.ContextMenu.Visible)
			{
				ClientGameManager.UIManager.ContextMenu.Hide(false);
				return true;
			}
			if (CursorManager.GameMode != CursorGameMode.None)
			{
				CursorManager.ResetGameMode();
				return true;
			}
			for (int i = UIManager.m_windows.Count - 1; i >= 0; i--)
			{
				if (UIManager.m_windows[i] == null)
				{
					UIManager.m_windows.RemoveAt(i);
					i++;
				}
				else if (UIManager.m_windows[i].CloseWithEscape && UIManager.m_windows[i].Visible)
				{
					UIManager.m_windows[i].CloseButtonPressed();
					return true;
				}
			}
			return ClientGameManager.UIManager.InGameUiMenu.EscapePressedLastPass();
		}

		// Token: 0x170007D4 RID: 2004
		// (get) Token: 0x06002685 RID: 9861 RVA: 0x0005B1F3 File Offset: 0x000593F3
		// (set) Token: 0x06002686 RID: 9862 RVA: 0x0005B1FB File Offset: 0x000593FB
		public EquipmentStatPanelUI EquipmentStats { get; private set; }

		// Token: 0x170007D5 RID: 2005
		// (get) Token: 0x06002687 RID: 9863 RVA: 0x0005B204 File Offset: 0x00059404
		// (set) Token: 0x06002688 RID: 9864 RVA: 0x0005B20C File Offset: 0x0005940C
		public UniversalContainerUI RemoteInventory { get; private set; }

		// Token: 0x170007D6 RID: 2006
		// (get) Token: 0x06002689 RID: 9865 RVA: 0x0005B215 File Offset: 0x00059415
		// (set) Token: 0x0600268A RID: 9866 RVA: 0x0005B21D File Offset: 0x0005941D
		public UniversalContainerUI LootInventory { get; private set; }

		// Token: 0x170007D7 RID: 2007
		// (get) Token: 0x0600268B RID: 9867 RVA: 0x0005B226 File Offset: 0x00059426
		// (set) Token: 0x0600268C RID: 9868 RVA: 0x0005B22E File Offset: 0x0005942E
		public UniversalContainerUI Inventory { get; private set; }

		// Token: 0x170007D8 RID: 2008
		// (get) Token: 0x0600268D RID: 9869 RVA: 0x0005B237 File Offset: 0x00059437
		// (set) Token: 0x0600268E RID: 9870 RVA: 0x0005B23F File Offset: 0x0005943F
		public UniversalContainerUI Gathering { get; private set; }

		// Token: 0x170007D9 RID: 2009
		// (get) Token: 0x0600268F RID: 9871 RVA: 0x0005B248 File Offset: 0x00059448
		// (set) Token: 0x06002690 RID: 9872 RVA: 0x0005B250 File Offset: 0x00059450
		public UniversalContainerUI Pouch { get; private set; }

		// Token: 0x170007DA RID: 2010
		// (get) Token: 0x06002691 RID: 9873 RVA: 0x0005B259 File Offset: 0x00059459
		// (set) Token: 0x06002692 RID: 9874 RVA: 0x0005B261 File Offset: 0x00059461
		public UniversalContainerUI ReagentPouch { get; private set; }

		// Token: 0x170007DB RID: 2011
		// (get) Token: 0x06002693 RID: 9875 RVA: 0x0005B26A File Offset: 0x0005946A
		// (set) Token: 0x06002694 RID: 9876 RVA: 0x0005B272 File Offset: 0x00059472
		public ActionBarUI ActionBar { get; private set; }

		// Token: 0x170007DC RID: 2012
		// (get) Token: 0x06002695 RID: 9877 RVA: 0x0005B27B File Offset: 0x0005947B
		// (set) Token: 0x06002696 RID: 9878 RVA: 0x0005B283 File Offset: 0x00059483
		public AlchemySelectionUI AlchemySelectionUI { get; set; }

		// Token: 0x170007DD RID: 2013
		// (get) Token: 0x06002697 RID: 9879 RVA: 0x0005B28C File Offset: 0x0005948C
		// (set) Token: 0x06002698 RID: 9880 RVA: 0x0005B294 File Offset: 0x00059494
		public TradeContainerUI Trade { get; private set; }

		// Token: 0x170007DE RID: 2014
		// (get) Token: 0x06002699 RID: 9881 RVA: 0x0005B29D File Offset: 0x0005949D
		// (set) Token: 0x0600269A RID: 9882 RVA: 0x0005B2A5 File Offset: 0x000594A5
		public RefinementStationUI Refinement { get; private set; }

		// Token: 0x170007DF RID: 2015
		// (get) Token: 0x0600269B RID: 9883 RVA: 0x0005B2AE File Offset: 0x000594AE
		// (set) Token: 0x0600269C RID: 9884 RVA: 0x0005B2B6 File Offset: 0x000594B6
		public RecipesUI RecipesUI { get; private set; }

		// Token: 0x170007E0 RID: 2016
		// (get) Token: 0x0600269D RID: 9885 RVA: 0x0005B2BF File Offset: 0x000594BF
		// (set) Token: 0x0600269E RID: 9886 RVA: 0x0005B2C7 File Offset: 0x000594C7
		public CraftingUI CraftingUI { get; private set; }

		// Token: 0x170007E1 RID: 2017
		// (get) Token: 0x0600269F RID: 9887 RVA: 0x0005B2D0 File Offset: 0x000594D0
		// (set) Token: 0x060026A0 RID: 9888 RVA: 0x0005B2D8 File Offset: 0x000594D8
		public ContextMenuUI ContextMenu { get; private set; }

		// Token: 0x170007E2 RID: 2018
		// (get) Token: 0x060026A1 RID: 9889 RVA: 0x0005B2E1 File Offset: 0x000594E1
		// (set) Token: 0x060026A2 RID: 9890 RVA: 0x0005B2E9 File Offset: 0x000594E9
		public CodexMasteryUI MasteryPanel { get; private set; }

		// Token: 0x170007E3 RID: 2019
		// (get) Token: 0x060026A3 RID: 9891 RVA: 0x0005B2F2 File Offset: 0x000594F2
		// (set) Token: 0x060026A4 RID: 9892 RVA: 0x0005B2FA File Offset: 0x000594FA
		public CodexAbilityUI AbilityPanel { get; private set; }

		// Token: 0x170007E4 RID: 2020
		// (get) Token: 0x060026A5 RID: 9893 RVA: 0x0005B303 File Offset: 0x00059503
		// (set) Token: 0x060026A6 RID: 9894 RVA: 0x0005B30B File Offset: 0x0005950B
		public SkillsUI SkillsUI { get; private set; }

		// Token: 0x170007E5 RID: 2021
		// (get) Token: 0x060026A7 RID: 9895 RVA: 0x0005B314 File Offset: 0x00059514
		// (set) Token: 0x060026A8 RID: 9896 RVA: 0x0005B31C File Offset: 0x0005951C
		public SkillsMasteryUI MasteryUI { get; private set; }

		// Token: 0x170007E6 RID: 2022
		// (get) Token: 0x060026A9 RID: 9897 RVA: 0x0005B325 File Offset: 0x00059525
		// (set) Token: 0x060026AA RID: 9898 RVA: 0x0005B32D File Offset: 0x0005952D
		public SkillsAbilityUI AbilityUI { get; private set; }

		// Token: 0x170007E7 RID: 2023
		// (get) Token: 0x060026AB RID: 9899 RVA: 0x0005B336 File Offset: 0x00059536
		// (set) Token: 0x060026AC RID: 9900 RVA: 0x0005B33E File Offset: 0x0005953E
		public SocialUI SocialUI { get; private set; }

		// Token: 0x170007E8 RID: 2024
		// (get) Token: 0x060026AD RID: 9901 RVA: 0x0005B347 File Offset: 0x00059547
		// (set) Token: 0x060026AE RID: 9902 RVA: 0x0005B34F File Offset: 0x0005954F
		public TabbedStatPanelUI StatPanel { get; private set; }

		// Token: 0x170007E9 RID: 2025
		// (get) Token: 0x060026AF RID: 9903 RVA: 0x0005B358 File Offset: 0x00059558
		// (set) Token: 0x060026B0 RID: 9904 RVA: 0x0005B360 File Offset: 0x00059560
		public CodexUI Codex { get; private set; }

		// Token: 0x170007EA RID: 2026
		// (get) Token: 0x060026B1 RID: 9905 RVA: 0x0005B369 File Offset: 0x00059569
		// (set) Token: 0x060026B2 RID: 9906 RVA: 0x0005B371 File Offset: 0x00059571
		public InkDriver QuestDialog { get; private set; }

		// Token: 0x170007EB RID: 2027
		// (get) Token: 0x060026B3 RID: 9907 RVA: 0x0005B37A File Offset: 0x0005957A
		// (set) Token: 0x060026B4 RID: 9908 RVA: 0x0005B382 File Offset: 0x00059582
		public MerchantUI MerchantUI { get; private set; }

		// Token: 0x170007EC RID: 2028
		// (get) Token: 0x060026B5 RID: 9909 RVA: 0x0005B38B File Offset: 0x0005958B
		// (set) Token: 0x060026B6 RID: 9910 RVA: 0x0005B393 File Offset: 0x00059593
		public BlacksmithUI BlacksmithUI { get; private set; }

		// Token: 0x170007ED RID: 2029
		// (get) Token: 0x060026B7 RID: 9911 RVA: 0x0005B39C File Offset: 0x0005959C
		// (set) Token: 0x060026B8 RID: 9912 RVA: 0x0005B3A4 File Offset: 0x000595A4
		public PersonalBankUI PersonalBankUI { get; private set; }

		// Token: 0x170007EE RID: 2030
		// (get) Token: 0x060026B9 RID: 9913 RVA: 0x0005B3AD File Offset: 0x000595AD
		// (set) Token: 0x060026BA RID: 9914 RVA: 0x0005B3B5 File Offset: 0x000595B5
		public LostAndFoundUI LostAndFoundUI { get; private set; }

		// Token: 0x170007EF RID: 2031
		// (get) Token: 0x060026BB RID: 9915 RVA: 0x0005B3BE File Offset: 0x000595BE
		// (set) Token: 0x060026BC RID: 9916 RVA: 0x0005B3C6 File Offset: 0x000595C6
		public EssenceConverterUI EssenceConverterUI { get; private set; }

		// Token: 0x170007F0 RID: 2032
		// (get) Token: 0x060026BD RID: 9917 RVA: 0x0005B3CF File Offset: 0x000595CF
		// (set) Token: 0x060026BE RID: 9918 RVA: 0x0005B3D7 File Offset: 0x000595D7
		public RuneCollectorUI RuneCollectorUI { get; private set; }

		// Token: 0x170007F1 RID: 2033
		// (get) Token: 0x060026BF RID: 9919 RVA: 0x0005B3E0 File Offset: 0x000595E0
		// (set) Token: 0x060026C0 RID: 9920 RVA: 0x0005B3E8 File Offset: 0x000595E8
		public LootRollWindow LootRollUI { get; private set; }

		// Token: 0x170007F2 RID: 2034
		// (get) Token: 0x060026C1 RID: 9921 RVA: 0x0005B3F1 File Offset: 0x000595F1
		// (set) Token: 0x060026C2 RID: 9922 RVA: 0x0005B3F9 File Offset: 0x000595F9
		public MapUI MapUI { get; private set; }

		// Token: 0x170007F3 RID: 2035
		// (get) Token: 0x060026C3 RID: 9923 RVA: 0x0005B402 File Offset: 0x00059602
		// (set) Token: 0x060026C4 RID: 9924 RVA: 0x0005B40A File Offset: 0x0005960A
		public NotificationsWindow NotificationsWindow { get; private set; }

		// Token: 0x170007F4 RID: 2036
		// (get) Token: 0x060026C5 RID: 9925 RVA: 0x0005B413 File Offset: 0x00059613
		// (set) Token: 0x060026C6 RID: 9926 RVA: 0x0005B41B File Offset: 0x0005961B
		public LogUI LogUI { get; private set; }

		// Token: 0x170007F5 RID: 2037
		// (get) Token: 0x060026C7 RID: 9927 RVA: 0x0005B424 File Offset: 0x00059624
		// (set) Token: 0x060026C8 RID: 9928 RVA: 0x0005B42C File Offset: 0x0005962C
		public DocumentUI DocumentUI { get; private set; }

		// Token: 0x170007F6 RID: 2038
		// (get) Token: 0x060026C9 RID: 9929 RVA: 0x0005B435 File Offset: 0x00059635
		// (set) Token: 0x060026CA RID: 9930 RVA: 0x0005B43D File Offset: 0x0005963D
		public PenaltiesUI PenaltiesUI { get; private set; }

		// Token: 0x170007F7 RID: 2039
		// (get) Token: 0x060026CB RID: 9931 RVA: 0x0005B446 File Offset: 0x00059646
		// (set) Token: 0x060026CC RID: 9932 RVA: 0x0005B44E File Offset: 0x0005964E
		public BulletinBoardUI BulletinBoardUI { get; private set; }

		// Token: 0x170007F8 RID: 2040
		// (get) Token: 0x060026CD RID: 9933 RVA: 0x0005B457 File Offset: 0x00059657
		// (set) Token: 0x060026CE RID: 9934 RVA: 0x0005B45F File Offset: 0x0005965F
		public MailboxUI MailboxUI { get; private set; }

		// Token: 0x170007F9 RID: 2041
		// (get) Token: 0x060026CF RID: 9935 RVA: 0x0005B468 File Offset: 0x00059668
		// (set) Token: 0x060026D0 RID: 9936 RVA: 0x0005B470 File Offset: 0x00059670
		public TimeWindowUI TimeUI { get; private set; }

		// Token: 0x170007FA RID: 2042
		// (get) Token: 0x060026D1 RID: 9937 RVA: 0x0005B479 File Offset: 0x00059679
		// (set) Token: 0x060026D2 RID: 9938 RVA: 0x0005B481 File Offset: 0x00059681
		public InspectionUI Inspection { get; private set; }

		// Token: 0x170007FB RID: 2043
		// (get) Token: 0x060026D3 RID: 9939 RVA: 0x0005B48A File Offset: 0x0005968A
		// (set) Token: 0x060026D4 RID: 9940 RVA: 0x0005B492 File Offset: 0x00059692
		public AuctionHouseUI AuctionHouseUI { get; private set; }

		// Token: 0x170007FC RID: 2044
		// (get) Token: 0x060026D5 RID: 9941 RVA: 0x0005B49B File Offset: 0x0005969B
		public LocalGameTimeUI LocalGameTimeUI
		{
			get
			{
				return this._localGameTimeUI;
			}
		}

		// Token: 0x170007FD RID: 2045
		// (get) Token: 0x060026D6 RID: 9942 RVA: 0x0005B4A3 File Offset: 0x000596A3
		public DpsMeterController DpsMeter
		{
			get
			{
				return this._dpsMeter;
			}
		}

		// Token: 0x170007FE RID: 2046
		// (get) Token: 0x060026D7 RID: 9943 RVA: 0x0005B4AB File Offset: 0x000596AB
		public CountdownUI CountdownUI
		{
			get
			{
				return this._countdownUI;
			}
		}

		// Token: 0x170007FF RID: 2047
		// (get) Token: 0x060026D8 RID: 9944 RVA: 0x0005B4B3 File Offset: 0x000596B3
		public RectTransform DragPanel
		{
			get
			{
				return this.m_dragPanel;
			}
		}

		// Token: 0x17000800 RID: 2048
		// (get) Token: 0x060026D9 RID: 9945 RVA: 0x0005B4BB File Offset: 0x000596BB
		public GameObject ItemInstanceUIPrefab
		{
			get
			{
				return this.m_itemInstanceUIPrefab;
			}
		}

		// Token: 0x17000801 RID: 2049
		// (get) Token: 0x060026DA RID: 9946 RVA: 0x0005B4C3 File Offset: 0x000596C3
		public GameObject AbilityInstanceUIPrefab
		{
			get
			{
				return this.m_abilityInstanceUIPrefab;
			}
		}

		// Token: 0x17000802 RID: 2050
		// (get) Token: 0x060026DB RID: 9947 RVA: 0x0005B4CB File Offset: 0x000596CB
		public GameObject AuraAbilityInstanceUIPrefab
		{
			get
			{
				return this.m_auraAbilityInstanceUIPrefab;
			}
		}

		// Token: 0x17000803 RID: 2051
		// (get) Token: 0x060026DC RID: 9948 RVA: 0x0005B4D3 File Offset: 0x000596D3
		public GameObject AutoAttackInstanceUIPrefab
		{
			get
			{
				return this.m_autoAttackInstanceUIPrefab;
			}
		}

		// Token: 0x17000804 RID: 2052
		// (get) Token: 0x060026DD RID: 9949 RVA: 0x0005B4DB File Offset: 0x000596DB
		public GameObject ArchetypeInstanceSymbolicLinkUIPrefab
		{
			get
			{
				return this.m_archetypeInstanceSymbolicLinkPrefab;
			}
		}

		// Token: 0x17000805 RID: 2053
		// (get) Token: 0x060026DE RID: 9950 RVA: 0x0005B4E3 File Offset: 0x000596E3
		public ArchetypeInstanceUIDragShadow DragShadow
		{
			get
			{
				return this.m_dragShadow;
			}
		}

		// Token: 0x17000806 RID: 2054
		// (get) Token: 0x060026DF RID: 9951 RVA: 0x0005B4EB File Offset: 0x000596EB
		public RectTransform UiSpaceNameplatePanel
		{
			get
			{
				return this.m_uiSpaceNameplatePanel;
			}
		}

		// Token: 0x17000807 RID: 2055
		// (get) Token: 0x060026E0 RID: 9952 RVA: 0x0005B4F3 File Offset: 0x000596F3
		// (set) Token: 0x060026E1 RID: 9953 RVA: 0x0005B4FB File Offset: 0x000596FB
		public bool IsDragging { get; private set; }

		// Token: 0x17000808 RID: 2056
		// (get) Token: 0x060026E2 RID: 9954 RVA: 0x0005B504 File Offset: 0x00059704
		// (set) Token: 0x060026E3 RID: 9955 RVA: 0x0005B50C File Offset: 0x0005970C
		public bool IsUsingSlider { get; set; }

		// Token: 0x17000809 RID: 2057
		// (get) Token: 0x060026E4 RID: 9956 RVA: 0x0005B515 File Offset: 0x00059715
		// (set) Token: 0x060026E5 RID: 9957 RVA: 0x0005B51D File Offset: 0x0005971D
		public ConfirmationDialog InformationDialog { get; private set; }

		// Token: 0x1700080A RID: 2058
		// (get) Token: 0x060026E6 RID: 9958 RVA: 0x0005B526 File Offset: 0x00059726
		// (set) Token: 0x060026E7 RID: 9959 RVA: 0x0005B52E File Offset: 0x0005972E
		public ConfirmationDialog ConfirmationDialog { get; private set; }

		// Token: 0x1700080B RID: 2059
		// (get) Token: 0x060026E8 RID: 9960 RVA: 0x0005B537 File Offset: 0x00059737
		// (set) Token: 0x060026E9 RID: 9961 RVA: 0x0005B53F File Offset: 0x0005973F
		public ConfirmationDialog ItemConfirmationDialog { get; private set; }

		// Token: 0x1700080C RID: 2060
		// (get) Token: 0x060026EA RID: 9962 RVA: 0x0005B548 File Offset: 0x00059748
		// (set) Token: 0x060026EB RID: 9963 RVA: 0x0005B550 File Offset: 0x00059750
		public TeleportConfirmationDialog TeleportConfirmationDialog { get; private set; }

		// Token: 0x1700080D RID: 2061
		// (get) Token: 0x060026EC RID: 9964 RVA: 0x0005B559 File Offset: 0x00059759
		// (set) Token: 0x060026ED RID: 9965 RVA: 0x0005B561 File Offset: 0x00059761
		public MacroEditDialog MacroEditDialog { get; private set; }

		// Token: 0x1700080E RID: 2062
		// (get) Token: 0x060026EE RID: 9966 RVA: 0x0005B56A File Offset: 0x0005976A
		// (set) Token: 0x060026EF RID: 9967 RVA: 0x0005B572 File Offset: 0x00059772
		public SteamSubscriptionDialog SteamSubscriptionDialog { get; private set; }

		// Token: 0x1700080F RID: 2063
		// (get) Token: 0x060026F0 RID: 9968 RVA: 0x0005B57B File Offset: 0x0005977B
		// (set) Token: 0x060026F1 RID: 9969 RVA: 0x0005B583 File Offset: 0x00059783
		public SelectOneDialog SelectOneDialog { get; private set; }

		// Token: 0x17000810 RID: 2064
		// (get) Token: 0x060026F2 RID: 9970 RVA: 0x0005B58C File Offset: 0x0005978C
		// (set) Token: 0x060026F3 RID: 9971 RVA: 0x0005B594 File Offset: 0x00059794
		public SelectValueDialog SelectValueDialog { get; private set; }

		// Token: 0x17000811 RID: 2065
		// (get) Token: 0x060026F4 RID: 9972 RVA: 0x0005B59D File Offset: 0x0005979D
		// (set) Token: 0x060026F5 RID: 9973 RVA: 0x0005B5A5 File Offset: 0x000597A5
		public TextEntryDialog TextEntryDialog { get; private set; }

		// Token: 0x17000812 RID: 2066
		// (get) Token: 0x060026F6 RID: 9974 RVA: 0x0005B5AE File Offset: 0x000597AE
		// (set) Token: 0x060026F7 RID: 9975 RVA: 0x0005B5B6 File Offset: 0x000597B6
		public CurrencyPickerDialog CurrencyPickerDialog { get; private set; }

		// Token: 0x17000813 RID: 2067
		// (get) Token: 0x060026F8 RID: 9976 RVA: 0x0005B5BF File Offset: 0x000597BF
		// (set) Token: 0x060026F9 RID: 9977 RVA: 0x0005B5C7 File Offset: 0x000597C7
		private CenterScreenAnnouncement CenterScreenAnnouncement { get; set; }

		// Token: 0x17000814 RID: 2068
		// (get) Token: 0x060026FA RID: 9978 RVA: 0x0005B5D0 File Offset: 0x000597D0
		// (set) Token: 0x060026FB RID: 9979 RVA: 0x0005B5D8 File Offset: 0x000597D8
		private TutorialPopup TutorialPopup { get; set; }

		// Token: 0x17000815 RID: 2069
		// (get) Token: 0x060026FC RID: 9980 RVA: 0x0005B5E1 File Offset: 0x000597E1
		// (set) Token: 0x060026FD RID: 9981 RVA: 0x0005B5E9 File Offset: 0x000597E9
		public TutorialPopup TutorialPopupCenter { get; private set; }

		// Token: 0x17000816 RID: 2070
		// (get) Token: 0x060026FE RID: 9982 RVA: 0x0005B5F2 File Offset: 0x000597F2
		// (set) Token: 0x060026FF RID: 9983 RVA: 0x0005B5FA File Offset: 0x000597FA
		public EffectIconPanelUI EffectPanel { get; private set; }

		// Token: 0x17000817 RID: 2071
		// (get) Token: 0x06002700 RID: 9984 RVA: 0x0005B603 File Offset: 0x00059803
		// (set) Token: 0x06002701 RID: 9985 RVA: 0x0005B60B File Offset: 0x0005980B
		public NameplateControllerUI SelfNameplate { get; private set; }

		// Token: 0x17000818 RID: 2072
		// (get) Token: 0x06002702 RID: 9986 RVA: 0x0005B614 File Offset: 0x00059814
		// (set) Token: 0x06002703 RID: 9987 RVA: 0x0005B61C File Offset: 0x0005981C
		public NameplateControllerUI OffensiveNameplate { get; private set; }

		// Token: 0x17000819 RID: 2073
		// (get) Token: 0x06002704 RID: 9988 RVA: 0x0005B625 File Offset: 0x00059825
		// (set) Token: 0x06002705 RID: 9989 RVA: 0x0005B62D File Offset: 0x0005982D
		public NameplateControllerUI DefensiveNameplate { get; private set; }

		// Token: 0x1700081A RID: 2074
		// (get) Token: 0x06002706 RID: 9990 RVA: 0x0005B636 File Offset: 0x00059836
		public UIWindow GameHelpPanel
		{
			get
			{
				return this.m_gameHelpPanel;
			}
		}

		// Token: 0x1700081B RID: 2075
		// (get) Token: 0x06002707 RID: 9991 RVA: 0x0005B63E File Offset: 0x0005983E
		public InGameUIOptions InGameUiOptions
		{
			get
			{
				return this.m_inGameUiOptions;
			}
		}

		// Token: 0x1700081C RID: 2076
		// (get) Token: 0x06002708 RID: 9992 RVA: 0x0005B646 File Offset: 0x00059846
		public InGameUIMenu InGameUiMenu
		{
			get
			{
				return this.m_inGameUiMenu;
			}
		}

		// Token: 0x1700081D RID: 2077
		// (get) Token: 0x06002709 RID: 9993 RVA: 0x0005B64E File Offset: 0x0005984E
		public NotificationsUI NotificationsUI
		{
			get
			{
				return this.m_notificationsUI;
			}
		}

		// Token: 0x1700081E RID: 2078
		// (get) Token: 0x0600270A RID: 9994 RVA: 0x0005B656 File Offset: 0x00059856
		public Image CenterReticle
		{
			get
			{
				return this.m_centerReticle;
			}
		}

		// Token: 0x1700081F RID: 2079
		// (get) Token: 0x0600270B RID: 9995 RVA: 0x0005B65E File Offset: 0x0005985E
		// (set) Token: 0x0600270C RID: 9996 RVA: 0x0005B666 File Offset: 0x00059866
		public SelectPortraitWindow SelectPortraitWindow { get; private set; }

		// Token: 0x0600270D RID: 9997 RVA: 0x00135AD8 File Offset: 0x00133CD8
		private void Awake()
		{
			ClientGameManager.UIManager = this;
			UIManager.EventSystem = this.m_eventSystem;
			UIManager.InputModule = this.m_inputModule;
			this.m_blackoutPanel.gameObject.SetActive(true);
			this.m_uncoPanel.gameObject.SetActive(true);
			this.CheckGlobalPrefabs();
			this.m_audioPool = new UIManager.UIPooledAudio(this);
			this.m_gameHelpPanel.gameObject.SetActive(true);
			this.m_inGameUiOptions.gameObject.transform.parent.gameObject.SetActive(true);
			this.m_centerReticle.enabled = false;
			if (this.m_nonLiveGameDataLabel)
			{
				DeploymentBranchFlags branchFlags = DeploymentBranchFlagsExtensions.GetBranchFlags();
				if (branchFlags == DeploymentBranchFlags.DEV || branchFlags == DeploymentBranchFlags.QA)
				{
					this.m_nonLiveGameDataLabel.gameObject.SetActive(true);
				}
			}
		}

		// Token: 0x0600270E RID: 9998 RVA: 0x0005B66F File Offset: 0x0005986F
		private IEnumerator Start()
		{
			this.SelectPortraitWindow = this.InstantiatePrefabPanel<SelectPortraitWindow>(this.m_selectPortraitUi);
			yield return new WaitForSeconds(1f);
			this.ToggleBlackoutPanel(false);
			UIManager.LoadAdditionalSavedWindows();
			yield break;
		}

		// Token: 0x0600270F RID: 9999 RVA: 0x00135BA0 File Offset: 0x00133DA0
		private void Update()
		{
			this.CheckForResolutionChange();
			this.UpdateAlchemyAlphaValue();
			if (ClientGameManager.InputManager.PreventInputForUI)
			{
				return;
			}
			if (Options.GameOptions.OpenBagAndGatheringTogether.Value && this.Inventory && this.Gathering)
			{
				if (SolInput.GetButtonDown(15) || SolInput.GetButtonDown(96))
				{
					this.Inventory.ToggleWindow();
					if (this.Gathering.IsShown != this.Inventory.IsShown)
					{
						this.Gathering.ForceToggleWindow(this.Inventory.IsShown);
					}
				}
			}
			else
			{
				if (this.Inventory && SolInput.GetButtonDown(15))
				{
					this.Inventory.ToggleWindow();
				}
				if (this.Gathering && SolInput.GetButtonDown(96))
				{
					this.Gathering.ToggleWindow();
				}
			}
			if (this.EquipmentStats && SolInput.GetButtonDown(17))
			{
				this.EquipmentStats.ToggleWindow();
			}
			if (this.SkillsUI && SolInput.GetButtonDown(16))
			{
				this.SkillsUI.ToggleWindow();
			}
			if (this.MapUI && SolInput.GetButtonDown(67))
			{
				this.MapUI.ToggleWindow();
			}
			if (this.CraftingUI && SolInput.GetButtonDown(64))
			{
				this.CraftingUI.ToggleWindow();
			}
			if (this.SocialUI && SolInput.GetButtonDown(71))
			{
				this.SocialUI.ToggleWindow();
			}
			if (this.LogUI && SolInput.GetButtonDown(97))
			{
				this.LogUI.ToggleWindow();
			}
			if (this.TimeUI && SolInput.GetButtonDown(112))
			{
				this.TimeUI.ToggleWindow();
			}
			if (SolInput.GetButtonDown(114))
			{
				ToggleAllWindowExtensions.ToggleAllPressed();
			}
			if (this.m_centerScreenQueue.Count > 0 && !this.CenterScreenAnnouncement.Active)
			{
				CenterScreenAnnouncementOptions opts = this.m_centerScreenQueue.Dequeue();
				this.CenterScreenAnnouncement.DelayedInit(opts);
			}
			if (this.m_tutorialQueue.Count > 0 && !this.TutorialPopup.Active)
			{
				TutorialPopupOptions opts2 = this.m_tutorialQueue.Dequeue();
				this.TutorialPopup.Init(opts2);
			}
			if (this.m_dragged != null && this.m_dragged.ExternallyHandlePositionUpdate && Time.frameCount > this.m_dragFrame)
			{
				if (this.m_dragged.RectTransform)
				{
					this.m_dragged.RectTransform.position = Input.mousePosition;
				}
				if (Input.GetMouseButtonUp(0))
				{
					this.DeregisterDrag(false);
					return;
				}
				if (Input.GetMouseButtonUp(1))
				{
					this.DeregisterDrag(true);
				}
			}
		}

		// Token: 0x06002710 RID: 10000 RVA: 0x00135E40 File Offset: 0x00134040
		public void InitCenterScreenAnnouncement(CenterScreenAnnouncementOptions opts)
		{
			if (!this.CenterScreenAnnouncement.Active)
			{
				this.CenterScreenAnnouncement.DelayedInit(opts);
				return;
			}
			if (opts.SourceId != null && this.CenterScreenAnnouncement.SourceId == opts.SourceId)
			{
				this.CenterScreenAnnouncement.ExtendInit(opts);
				return;
			}
			this.m_centerScreenQueue.Enqueue(opts);
		}

		// Token: 0x06002711 RID: 10001 RVA: 0x0005B67E File Offset: 0x0005987E
		public void InitTutorialPopup(TutorialPopupOptions opts)
		{
			if (this.TutorialPopup.Active)
			{
				this.m_tutorialQueue.Enqueue(opts);
				return;
			}
			this.TutorialPopup.Init(opts);
		}

		// Token: 0x06002712 RID: 10002 RVA: 0x00135ED8 File Offset: 0x001340D8
		private void UpdateAlchemyAlphaValue()
		{
			float t = Mathf.PingPong(Time.time * 2f, 1f);
			UIManager.AlchemyPulseAlpha_I = Mathf.Lerp(0.5f, 1f, t);
			t = Mathf.PingPong(Time.time * 4f, 1f);
			UIManager.AlchemyPulseAlpha_II = Mathf.Lerp(0.5f, 1f, t);
		}

		// Token: 0x06002713 RID: 10003 RVA: 0x00135F3C File Offset: 0x0013413C
		private void UpdateRepairAlphaValue()
		{
			float t = Mathf.PingPong(Time.time * 2f, 2f);
			UIManager.RepairPulseAlpha = Mathf.Lerp(0.5f, 1f, t);
		}

		// Token: 0x06002714 RID: 10004 RVA: 0x00135F74 File Offset: 0x00134174
		private void CheckForResolutionChange()
		{
			if (Screen.width != this.m_previousWidth || Screen.height != this.m_previousHeight)
			{
				for (int i = 0; i < UIManager.m_windows.Count; i++)
				{
					if (UIManager.m_windows[i])
					{
						UIManager.m_windows[i].ResolutionChanged();
					}
				}
			}
			this.m_previousWidth = Screen.width;
			this.m_previousHeight = Screen.height;
		}

		// Token: 0x06002715 RID: 10005 RVA: 0x00135FE8 File Offset: 0x001341E8
		public IContainerUI GetContainerUI(ContainerType type)
		{
			switch (type)
			{
			case ContainerType.Equipment:
				return this.EquipmentStats.Equipment;
			case ContainerType.Inventory:
				return this.Inventory;
			case ContainerType.Pouch:
				return this.Pouch;
			case ContainerType.ReagentPouch:
				return this.ReagentPouch;
			case ContainerType.PersonalBank:
				return this.PersonalBankUI.OutgoingUI;
			case ContainerType.Gathering:
				return this.Gathering;
			case ContainerType.LostAndFound:
				return this.LostAndFoundUI.OutgoingUI;
			case ContainerType.Masteries:
				return this.MasteryUI;
			case ContainerType.Abilities:
				return this.AbilityUI;
			}
			return null;
		}

		// Token: 0x06002716 RID: 10006 RVA: 0x0005B6A6 File Offset: 0x000598A6
		public UniversalContainerUI GetRemoteContainerUI(ContainerType type)
		{
			if (type != ContainerType.Loot)
			{
				return this.RemoteInventory;
			}
			return this.LootInventory;
		}

		// Token: 0x06002717 RID: 10007 RVA: 0x0005B6BA File Offset: 0x000598BA
		public void ToggleBlackoutPanel(bool enabled)
		{
			if (enabled)
			{
				if (!this.m_blackoutPanel.Visible)
				{
					this.m_blackoutPanel.Show(false);
					return;
				}
			}
			else if (this.m_blackoutPanel.Visible)
			{
				this.m_blackoutPanel.Hide(false);
			}
		}

		// Token: 0x06002718 RID: 10008 RVA: 0x0005B6F2 File Offset: 0x000598F2
		public void ToggleUI()
		{
			this.ToggleUI(!UIManager.UiHidden);
		}

		// Token: 0x06002719 RID: 10009 RVA: 0x0013607C File Offset: 0x0013427C
		private void ToggleUI(bool hidden)
		{
			float alpha = hidden ? 0f : 1f;
			for (int i = 0; i < this.m_uiCanvasGroups.Length; i++)
			{
				if (this.m_uiCanvasGroups[i] != null)
				{
					this.m_uiCanvasGroups[i].alpha = alpha;
					this.m_uiCanvasGroups[i].interactable = !hidden;
					this.m_uiCanvasGroups[i].blocksRaycasts = !hidden;
				}
			}
			UIManager.UiHidden = hidden;
		}

		// Token: 0x0600271A RID: 10010 RVA: 0x0005B702 File Offset: 0x00059902
		private void DestroyUIElement(MonoBehaviour mb)
		{
			if (mb != null)
			{
				UnityEngine.Object.Destroy(mb.gameObject);
			}
		}

		// Token: 0x0600271B RID: 10011 RVA: 0x001360F4 File Offset: 0x001342F4
		public void ResetUI()
		{
			UIManager.UIResetting = true;
			this.DestroyUIElement(this.EquipmentStats);
			this.DestroyUIElement(this.RemoteInventory);
			this.DestroyUIElement(this.LootInventory);
			this.DestroyUIElement(this.Inventory);
			this.DestroyUIElement(this.Gathering);
			this.DestroyUIElement(this.ActionBar);
			this.DestroyUIElement(this.Trade);
			this.DestroyUIElement(this.Refinement);
			this.DestroyUIElement(this.RecipesUI);
			this.DestroyUIElement(this.CraftingUI);
			this.DestroyUIElement(this.MasteryPanel);
			this.DestroyUIElement(this.Codex);
			this.DestroyUIElement(this.SkillsUI);
			this.DestroyUIElement(this.SocialUI);
			this.DestroyUIElement(this.QuestDialog);
			this.DestroyUIElement(this.MerchantUI);
			this.DestroyUIElement(this.BlacksmithUI);
			this.DestroyUIElement(this.PersonalBankUI);
			this.DestroyUIElement(this.LostAndFoundUI);
			this.DestroyUIElement(this.EssenceConverterUI);
			this.DestroyUIElement(this.RuneCollectorUI);
			this.DestroyUIElement(this.LootRollUI);
			this.DestroyUIElement(this.MapUI);
			this.DestroyUIElement(this.NotificationsWindow);
			this.DestroyUIElement(this.LogUI);
			this.DestroyUIElement(this.DocumentUI);
			this.DestroyUIElement(this.PenaltiesUI);
			this.DestroyUIElement(this.BulletinBoardUI);
			this.DestroyUIElement(this.MailboxUI);
			this.DestroyUIElement(this.AuctionHouseUI);
			this.DestroyUIElement(this.TimeUI);
			this.DestroyUIElement(this.Inspection);
			this.DestroyUIElement(this._localGameTimeUI);
			this.DeregisterDrag(true);
			this.ToggleUI(false);
			Action resetUIEvent = this.ResetUIEvent;
			if (resetUIEvent != null)
			{
				resetUIEvent();
			}
			this.m_centerScreenQueue.Clear();
			this.m_tutorialQueue.Clear();
			if (this.CenterScreenAnnouncement.Visible)
			{
				this.CenterScreenAnnouncement.ResetDialog();
			}
			if (this.TutorialPopup.Visible)
			{
				this.TutorialPopup.ResetDialog();
			}
			if (UIManager.GroupWindowUI != null)
			{
				UIManager.GroupWindowUI.UnsetLocalPlayer();
			}
			if (UIManager.RaidWindowUI != null)
			{
				UIManager.RaidWindowUI.UnsetLocalPlayer();
			}
			this.CheckGlobalPrefabs();
			UIManager.UIResetting = false;
		}

		// Token: 0x0600271C RID: 10012 RVA: 0x0005B718 File Offset: 0x00059918
		public IEnumerator InitializePlayerUI()
		{
			this.EquipmentStats = this.InstantiatePrefabPanel<EquipmentStatPanelUI>(this.m_equipmentStats);
			this.StatPanel = this.EquipmentStats.Stats;
			this.RemoteInventory = this.InstantiatePrefabPanel<UniversalContainerUI>(this.m_remoteInventory);
			this.LootInventory = this.InstantiatePrefabPanel<UniversalContainerUI>(this.m_lootInventory);
			this.Inventory = this.InstantiatePrefabPanel<UniversalContainerUI>(this.m_inventory);
			this.Gathering = this.InstantiatePrefabPanel<UniversalContainerUI>(this.m_gathering);
			this.ActionBar = this.InstantiatePrefabPanel<ActionBarUI>(this.m_actionBar);
			this.Pouch = this.ActionBar.Pouch;
			this.ReagentPouch = this.ActionBar.ReagentPouch;
			this.Trade = this.InstantiatePrefabPanel<TradeContainerUI>(this.m_trade);
			this.QuestDialog = this.InstantiatePrefabPanel<InkDriver>(this.m_questDialog);
			this.CraftingUI = this.InstantiatePrefabPanel<CraftingUI>(this.m_craftingUi);
			this.SkillsUI = this.InstantiatePrefabPanel<SkillsUI>(this.m_skillUi);
			this.MasteryUI = this.SkillsUI.MasteryUI;
			this.AbilityUI = this.SkillsUI.AbilityUI;
			this.SocialUI = this.InstantiatePrefabPanel<SocialUI>(this.m_socialUi);
			this.EffectPanel = this.m_selfEffectPanelUI;
			this.EffectPanel.Init(LocalPlayer.NetworkEntity, true);
			if (UIManager.LoadingScreenUI)
			{
				UIManager.LoadingScreenUI.SetLoadingPercent(0.95f);
			}
			yield return null;
			this.MerchantUI = this.InstantiatePrefabPanel<MerchantUI>(this.m_merchantUi);
			this.BlacksmithUI = this.InstantiatePrefabPanel<BlacksmithUI>(this.m_blacksmithUi);
			this.PersonalBankUI = this.InstantiatePrefabPanel<PersonalBankUI>(this.m_personalBankUi);
			this.LostAndFoundUI = this.InstantiatePrefabPanel<LostAndFoundUI>(this.m_lostAndFoundUi);
			this.EssenceConverterUI = this.InstantiatePrefabPanel<EssenceConverterUI>(this.m_essenceConverterUi);
			this.LootRollUI = this.InstantiatePrefabPanel<LootRollWindow>(this.m_lootRollUi);
			this.MapUI = this.InstantiatePrefabPanel<MapUI>(this.m_mapUi);
			this.NotificationsWindow = this.InstantiatePrefabPanel<NotificationsWindow>(this.m_notificationsWindow);
			this.LogUI = this.InstantiatePrefabPanel<LogUI>(this.m_logUi);
			this.DocumentUI = this.InstantiatePrefabPanel<DocumentUI>(this.m_documentUI);
			this.PenaltiesUI = this.InstantiatePrefabPanel<PenaltiesUI>(this.m_penaltiesUI);
			this.BulletinBoardUI = this.InstantiatePrefabPanel<BulletinBoardUI>(this.m_bulletinBoardUI);
			this.MailboxUI = this.InstantiatePrefabPanel<MailboxUI>(this.m_mailboxUI);
			this.AuctionHouseUI = this.InstantiatePrefabPanel<AuctionHouseUI>(this.m_auctionHouseUI);
			this.TimeUI = this.InstantiatePrefabPanel<TimeWindowUI>(this.m_timeUI);
			this.Inspection = this.InstantiatePrefabPanel<InspectionUI>(this.m_inspectionUI);
			if (UIManager.LoadingScreenUI)
			{
				UIManager.LoadingScreenUI.SetLoadingPercent(0.96f);
			}
			yield return null;
			this.SelfNameplate = this.m_selfNamePlateUi;
			this.SelfNameplate.Init(LocalPlayer.GameEntity.Targetable);
			this.OffensiveNameplate = this.m_offensiveNameplateUI;
			this.OffensiveNameplate.Init(null);
			LocalPlayer.GameEntity.TargetController.AssignNameplate(TargetType.Offensive, this.OffensiveNameplate);
			this.DefensiveNameplate = this.m_defensiveNamePlateUI;
			this.DefensiveNameplate.Init(null);
			LocalPlayer.GameEntity.TargetController.AssignNameplate(TargetType.Defensive, this.DefensiveNameplate);
			this.m_uncoPanel.Init();
			if (UIManager.GroupWindowUI != null)
			{
				UIManager.GroupWindowUI.InitLocalPlayer();
			}
			if (UIManager.RaidWindowUI != null)
			{
				UIManager.RaidWindowUI.InitLocalPlayer();
			}
			yield break;
		}

		// Token: 0x0600271D RID: 10013 RVA: 0x00136334 File Offset: 0x00134534
		private T InstantiatePrefabPanel<T>(UIManager.PrefabPanel prefabPanel)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prefabPanel.Prefab, prefabPanel.Panel, false);
			if (!string.IsNullOrEmpty(prefabPanel.PlayerPrefsKey))
			{
				DraggableUIWindow component = gameObject.GetComponent<DraggableUIWindow>();
				if (component != null)
				{
					component.PlayerPrefsKey = prefabPanel.PlayerPrefsKey;
				}
			}
			return gameObject.GetComponent<T>();
		}

		// Token: 0x0600271E RID: 10014 RVA: 0x0005B727 File Offset: 0x00059927
		public void OnDestroyLocalPlayerUI()
		{
			this.m_uncoPanel.Unset();
			this.ResetUI();
		}

		// Token: 0x0600271F RID: 10015 RVA: 0x00136384 File Offset: 0x00134584
		private static T1 InstantiateDialogIfNeeded<T1, T2>(UIManager.DialogPanel dialogPanel, T1 assignment) where T1 : BaseDialog<T2> where T2 : IDialogOptions
		{
			if (assignment && assignment != null)
			{
				return assignment;
			}
			if (dialogPanel == null || !dialogPanel.Prefab)
			{
				return default(T1);
			}
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(dialogPanel.Prefab, dialogPanel.Panel);
			T1 component = gameObject.GetComponent<T1>();
			if (!component || component == null)
			{
				UnityEngine.Object.Destroy(gameObject);
				return default(T1);
			}
			component.Hide(true);
			return component;
		}

		// Token: 0x06002720 RID: 10016 RVA: 0x0013641C File Offset: 0x0013461C
		private void ValidateDialog(UIManager.DialogPanel dialogPanel)
		{
			if (dialogPanel == null || dialogPanel.Type == DialogType.None || !dialogPanel.Prefab || dialogPanel.Prefab == null)
			{
				return;
			}
			switch (dialogPanel.Type)
			{
			case DialogType.Information:
				this.InformationDialog = UIManager.InstantiateDialogIfNeeded<ConfirmationDialog, DialogOptions>(dialogPanel, this.InformationDialog);
				return;
			case DialogType.Confirmation:
				this.ConfirmationDialog = UIManager.InstantiateDialogIfNeeded<ConfirmationDialog, DialogOptions>(dialogPanel, this.ConfirmationDialog);
				return;
			case DialogType.SelectOne:
				this.SelectOneDialog = UIManager.InstantiateDialogIfNeeded<SelectOneDialog, SelectOneOptions>(dialogPanel, this.SelectOneDialog);
				return;
			case DialogType.SelectValue:
				this.SelectValueDialog = UIManager.InstantiateDialogIfNeeded<SelectValueDialog, SelectValueOptions>(dialogPanel, this.SelectValueDialog);
				return;
			case DialogType.TextEntry:
				this.TextEntryDialog = UIManager.InstantiateDialogIfNeeded<TextEntryDialog, DialogOptions>(dialogPanel, this.TextEntryDialog);
				return;
			case DialogType.SelectCurrency:
				this.CurrencyPickerDialog = UIManager.InstantiateDialogIfNeeded<CurrencyPickerDialog, SelectCurrencyOptions>(dialogPanel, this.CurrencyPickerDialog);
				return;
			case DialogType.CenterScreenAnnouncement:
				this.CenterScreenAnnouncement = UIManager.InstantiateDialogIfNeeded<CenterScreenAnnouncement, CenterScreenAnnouncementOptions>(dialogPanel, this.CenterScreenAnnouncement);
				return;
			case DialogType.TutorialPopup:
				this.TutorialPopup = UIManager.InstantiateDialogIfNeeded<TutorialPopup, TutorialPopupOptions>(dialogPanel, this.TutorialPopup);
				return;
			case DialogType.CenterScreenParchment:
				this.TutorialPopupCenter = UIManager.InstantiateDialogIfNeeded<TutorialPopup, TutorialPopupOptions>(dialogPanel, this.TutorialPopupCenter);
				return;
			case DialogType.ItemConfirmation:
				this.ItemConfirmationDialog = UIManager.InstantiateDialogIfNeeded<ConfirmationDialog, DialogOptions>(dialogPanel, this.ItemConfirmationDialog);
				return;
			case DialogType.TeleportConfirmation:
				this.TeleportConfirmationDialog = UIManager.InstantiateDialogIfNeeded<TeleportConfirmationDialog, TeleportConfirmationOptions>(dialogPanel, this.TeleportConfirmationDialog);
				return;
			case DialogType.MacroEdit:
				this.MacroEditDialog = UIManager.InstantiateDialogIfNeeded<MacroEditDialog, MacroEditDialogOptions>(dialogPanel, this.MacroEditDialog);
				return;
			case DialogType.SteamSubscription:
				this.SteamSubscriptionDialog = UIManager.InstantiateDialogIfNeeded<SteamSubscriptionDialog, SteamSubscriptionDialogOptions>(dialogPanel, this.SteamSubscriptionDialog);
				return;
			default:
				return;
			}
		}

		// Token: 0x06002721 RID: 10017 RVA: 0x0013658C File Offset: 0x0013478C
		private void CheckGlobalPrefabs()
		{
			for (int i = 0; i < this.m_tooltipPrefabs.Length; i++)
			{
				TooltipType type = this.m_tooltipPrefabs[i].Type;
				BaseTooltip component;
				bool flag = UIManager.m_tooltips.TryGetValue(type, out component);
				if (flag && (!component || component == null))
				{
					UIManager.m_tooltips.Remove(type);
					flag = false;
				}
				if (!flag)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.m_tooltipPrefabs[i].Prefab, this.m_tooltipPrefabs[i].Panel);
					gameObject.name = type.ToString();
					component = gameObject.GetComponent<BaseTooltip>();
					if (!component || component == null)
					{
						UnityEngine.Object.Destroy(gameObject);
					}
					else
					{
						component.Hide(true);
						UIManager.m_tooltips.Add(type, component);
					}
				}
			}
			for (int j = 0; j < this.m_dialogPrefabs.Length; j++)
			{
				this.ValidateDialog(this.m_dialogPrefabs[j]);
			}
			if (this.m_contextMenu != null && this.m_contextMenu.Prefab && (!this.ContextMenu || this.ContextMenu == null))
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(this.m_contextMenu.Prefab, this.m_contextMenu.Panel);
				this.ContextMenu = gameObject2.GetComponent<ContextMenuUI>();
			}
		}

		// Token: 0x17000820 RID: 2080
		// (get) Token: 0x06002722 RID: 10018 RVA: 0x0005B73A File Offset: 0x0005993A
		public IDraggable Dragged
		{
			get
			{
				return this.m_dragged;
			}
		}

		// Token: 0x06002723 RID: 10019 RVA: 0x001366E4 File Offset: 0x001348E4
		public void RegisterDrag(IDraggable draggable)
		{
			if (this.IsDragging && draggable != this.m_dragged)
			{
				this.DeregisterDrag(true);
			}
			this.IsDragging = true;
			if (draggable != null)
			{
				this.m_previousParent = draggable.RectTransform.parent;
				this.m_dragged = draggable;
				draggable.RectTransform.SetParent(this.m_dragPanel);
				this.m_dragFrame = Time.frameCount;
			}
			LocalPlayer.UpdateTimeOfLastInput();
		}

		// Token: 0x06002724 RID: 10020 RVA: 0x0013674C File Offset: 0x0013494C
		public void DeregisterDrag(bool canceled = false)
		{
			this.IsDragging = false;
			if (this.m_dragged != null)
			{
				if (this.m_dragged.RectTransform)
				{
					this.m_dragged.RectTransform.SetParent(this.m_previousParent);
				}
				this.m_dragged.CompleteDrag(canceled);
			}
			this.m_dragged = null;
			this.m_previousParent = null;
		}

		// Token: 0x06002725 RID: 10021 RVA: 0x0005B742 File Offset: 0x00059942
		public void IDraggableDestroyed(IDraggable draggable)
		{
			if (this.IsDragging && this.m_dragged != null && draggable != null && this.m_dragged == draggable)
			{
				this.IsDragging = false;
				this.m_dragged = null;
				this.m_previousParent = null;
			}
		}

		// Token: 0x06002726 RID: 10022 RVA: 0x001367AC File Offset: 0x001349AC
		[ContextMenu("Test Confirmation Dialog")]
		private void TestDialogUI()
		{
			DialogOptions dialogOptions = default(DialogOptions);
			dialogOptions.ShowCloseButton = false;
			dialogOptions.AllowDragging = true;
			dialogOptions.BlockInteractions = true;
			dialogOptions.BackgroundBlockerColor = Color.yellow;
			dialogOptions.Title = "Testing!";
			dialogOptions.Text = "lest go for a walk outside, would you like to go for a walk outside?";
			dialogOptions.ConfirmationText = "gogogo";
			dialogOptions.CancelText = "do not go";
			dialogOptions.Callback = delegate(bool b, object o)
			{
				Debug.Log(string.Format("I answered {0}", b));
			};
			DialogOptions opts = dialogOptions;
			this.ConfirmationDialog.Init(opts);
		}

		// Token: 0x06002727 RID: 10023 RVA: 0x0013684C File Offset: 0x00134A4C
		public void PlayMoneyAudio()
		{
			this.m_audioPool.PlayRandomClip(GlobalSettings.Values.Audio.MoneyClipCollection, null);
		}

		// Token: 0x06002728 RID: 10024 RVA: 0x0013687C File Offset: 0x00134A7C
		public void PlayEventCurrencyAudio()
		{
			this.m_audioPool.PlayRandomClip(GlobalSettings.Values.Audio.EventCurrencyClipCollection, null);
		}

		// Token: 0x06002729 RID: 10025 RVA: 0x0005B775 File Offset: 0x00059975
		public void PlayRandomClip(AudioClipCollection collection, float? volume = null)
		{
			this.m_audioPool.PlayRandomClip(collection, volume);
		}

		// Token: 0x0600272A RID: 10026 RVA: 0x0005B784 File Offset: 0x00059984
		public void PlayClip(AudioClip clip, float? pitch, float? volume)
		{
			this.m_audioPool.PlayClip(clip, pitch, volume);
		}

		// Token: 0x0600272B RID: 10027 RVA: 0x001368AC File Offset: 0x00134AAC
		public void PlayScreenshotAudio()
		{
			this.m_audioPool.PlayClip(GlobalSettings.Values.Audio.ScreenshotAudio, null, null);
		}

		// Token: 0x0600272C RID: 10028 RVA: 0x001368E8 File Offset: 0x00134AE8
		public void PlayCannotEquipAudio()
		{
			this.m_audioPool.PlayClip(GlobalSettings.Values.Audio.CannotEquipClip, null, null);
		}

		// Token: 0x0600272D RID: 10029 RVA: 0x00136924 File Offset: 0x00134B24
		public void PlayTaskCompleteAudio()
		{
			this.m_audioPool.PlayClip(GlobalSettings.Values.Audio.TaskCompleteClip, null, null);
		}

		// Token: 0x0600272E RID: 10030 RVA: 0x00136960 File Offset: 0x00134B60
		public void PlayDefaultClick()
		{
			if (GlobalSettings.Values && GlobalSettings.Values.Audio != null && GlobalSettings.Values.Audio.DefaultClickClip)
			{
				AudioClip defaultClickClip = GlobalSettings.Values.Audio.DefaultClickClip;
				float defaultClickVolume = GlobalSettings.Values.Audio.DefaultClickVolume;
				this.PlayClip(defaultClickClip, new float?(1f), new float?(defaultClickVolume));
			}
		}

		// Token: 0x0600272F RID: 10031 RVA: 0x001369D4 File Offset: 0x00134BD4
		public static void TriggerCannotPerform(string reason)
		{
			if (ClientGameManager.UIManager)
			{
				ClientGameManager.UIManager.PlayCannotEquipAudio();
			}
			if (!string.IsNullOrEmpty(reason) && ClientGameManager.CombatTextManager && LocalPlayer.GameEntity)
			{
				ClientGameManager.CombatTextManager.InitializeOverheadCombatText(reason, LocalPlayer.GameEntity, Color.yellow, null);
			}
		}

		// Token: 0x04002850 RID: 10320
		private static bool m_UiHidden = false;

		// Token: 0x04002856 RID: 10326
		public const string kTooltipDetailsPrefix = "<i><size=80%>";

		// Token: 0x04002857 RID: 10327
		public const string kTooltipDetailsSuffix = "</size></i>";

		// Token: 0x04002859 RID: 10329
		public static readonly Color BlueColor = Colors.CornflowerBlue;

		// Token: 0x0400285A RID: 10330
		public static readonly Color RedColor = Colors.Crimson;

		// Token: 0x0400285B RID: 10331
		public static readonly Color GrayColor = Colors.DimGray;

		// Token: 0x0400285C RID: 10332
		public static readonly Color RaidColor = Colors.OrangePeel;

		// Token: 0x04002860 RID: 10336
		public static readonly Color EmberColor = new Color(0.88235295f, 0.4509804f, 0.078431375f);

		// Token: 0x04002861 RID: 10337
		public static readonly Color AugmentColor = Colors.VividLimeGreen;

		// Token: 0x04002862 RID: 10338
		public static readonly Color TemporaryStatColor = Colors.GreenCyan;

		// Token: 0x04002863 RID: 10339
		public static readonly string SubscriberChatIcon = "<font=\"Font Awesome 5 Free-Solid-900 SDF\"></font>";

		// Token: 0x04002864 RID: 10340
		public const int kRequirementsIndent = 5;

		// Token: 0x04002865 RID: 10341
		public const int kMaxChatWindows = 10;

		// Token: 0x0400286C RID: 10348
		private static readonly List<UIWindow> m_windows = new List<UIWindow>(10);

		// Token: 0x0400286D RID: 10349
		private static readonly Dictionary<string, IContainerUI> m_containers = new Dictionary<string, IContainerUI>();

		// Token: 0x0400286E RID: 10350
		private static readonly Dictionary<TooltipType, BaseTooltip> m_tooltips = new Dictionary<TooltipType, BaseTooltip>(default(TooltipTypeComparer));

		// Token: 0x0400286F RID: 10351
		private static readonly List<ChatTabUI> m_chatTabs = new List<ChatTabUI>();

		// Token: 0x04002870 RID: 10352
		private static readonly List<ChatWindowUI> m_chatWindows = new List<ChatWindowUI>();

		// Token: 0x04002871 RID: 10353
		private const string kChatWindowPrefPrefix = "ChatWindow_";

		// Token: 0x04002874 RID: 10356
		private static ChatTab m_lastActiveChatTab = null;

		// Token: 0x04002875 RID: 10357
		private static ChatWindowUI m_lastActiveChatWindow = null;

		// Token: 0x04002876 RID: 10358
		private static int m_savedWindowCount = 0;

		// Token: 0x04002877 RID: 10359
		[SerializeField]
		private EventSystem m_eventSystem;

		// Token: 0x04002878 RID: 10360
		[SerializeField]
		private SolStandaloneInputModule m_inputModule;

		// Token: 0x04002879 RID: 10361
		[SerializeField]
		private UIWindow m_blackoutPanel;

		// Token: 0x0400287A RID: 10362
		[SerializeField]
		private RectTransform m_dragPanel;

		// Token: 0x0400287B RID: 10363
		[SerializeField]
		private GameObject m_itemInstanceUIPrefab;

		// Token: 0x0400287C RID: 10364
		[SerializeField]
		private GameObject m_abilityInstanceUIPrefab;

		// Token: 0x0400287D RID: 10365
		[SerializeField]
		private GameObject m_auraAbilityInstanceUIPrefab;

		// Token: 0x0400287E RID: 10366
		[SerializeField]
		private GameObject m_autoAttackInstanceUIPrefab;

		// Token: 0x0400287F RID: 10367
		[SerializeField]
		private GameObject m_archetypeInstanceSymbolicLinkPrefab;

		// Token: 0x04002880 RID: 10368
		[SerializeField]
		private UIManager.PrefabPanel m_equipmentStats;

		// Token: 0x04002881 RID: 10369
		[SerializeField]
		private UIManager.PrefabPanel m_remoteInventory;

		// Token: 0x04002882 RID: 10370
		[SerializeField]
		private UIManager.PrefabPanel m_lootInventory;

		// Token: 0x04002883 RID: 10371
		[SerializeField]
		private UIManager.PrefabPanel m_inventory;

		// Token: 0x04002884 RID: 10372
		[SerializeField]
		private UIManager.PrefabPanel m_gathering;

		// Token: 0x04002885 RID: 10373
		[SerializeField]
		private UIManager.PrefabPanel m_actionBar;

		// Token: 0x04002886 RID: 10374
		[SerializeField]
		private UIManager.PrefabPanel m_trade;

		// Token: 0x04002887 RID: 10375
		[SerializeField]
		private UIManager.PrefabPanel m_contextMenu;

		// Token: 0x04002888 RID: 10376
		[SerializeField]
		private UIManager.PrefabPanel m_codex;

		// Token: 0x04002889 RID: 10377
		[SerializeField]
		private UIManager.PrefabPanel m_skillUi;

		// Token: 0x0400288A RID: 10378
		[SerializeField]
		private UIManager.PrefabPanel m_socialUi;

		// Token: 0x0400288B RID: 10379
		[SerializeField]
		private UIManager.PrefabPanel m_questDialog;

		// Token: 0x0400288C RID: 10380
		[SerializeField]
		private UIManager.PrefabPanel m_craftingUi;

		// Token: 0x0400288D RID: 10381
		[SerializeField]
		private UIManager.PrefabPanel m_merchantUi;

		// Token: 0x0400288E RID: 10382
		[SerializeField]
		private UIManager.PrefabPanel m_blacksmithUi;

		// Token: 0x0400288F RID: 10383
		[SerializeField]
		private UIManager.PrefabPanel m_personalBankUi;

		// Token: 0x04002890 RID: 10384
		[SerializeField]
		private UIManager.PrefabPanel m_lostAndFoundUi;

		// Token: 0x04002891 RID: 10385
		[SerializeField]
		private UIManager.PrefabPanel m_essenceConverterUi;

		// Token: 0x04002892 RID: 10386
		[SerializeField]
		private UIManager.PrefabPanel m_lootRollUi;

		// Token: 0x04002893 RID: 10387
		[SerializeField]
		private UIManager.PrefabPanel m_selectPortraitUi;

		// Token: 0x04002894 RID: 10388
		[SerializeField]
		private UIManager.PrefabPanel m_mapUi;

		// Token: 0x04002895 RID: 10389
		[SerializeField]
		private UIManager.PrefabPanel m_localGameTimeUi;

		// Token: 0x04002896 RID: 10390
		[SerializeField]
		private UIManager.PrefabPanel m_dpsMeterUi;

		// Token: 0x04002897 RID: 10391
		[SerializeField]
		private UIManager.PrefabPanel m_countdownUi;

		// Token: 0x04002898 RID: 10392
		[SerializeField]
		private UIManager.PrefabPanel m_notificationsWindow;

		// Token: 0x04002899 RID: 10393
		[SerializeField]
		private UIManager.PrefabPanel m_logUi;

		// Token: 0x0400289A RID: 10394
		[SerializeField]
		private UIManager.PrefabPanel m_documentUI;

		// Token: 0x0400289B RID: 10395
		[SerializeField]
		private UIManager.PrefabPanel m_penaltiesUI;

		// Token: 0x0400289C RID: 10396
		[SerializeField]
		private UIManager.PrefabPanel m_bulletinBoardUI;

		// Token: 0x0400289D RID: 10397
		[SerializeField]
		private UIManager.PrefabPanel m_mailboxUI;

		// Token: 0x0400289E RID: 10398
		[SerializeField]
		private UIManager.PrefabPanel m_auctionHouseUI;

		// Token: 0x0400289F RID: 10399
		[SerializeField]
		private UIManager.PrefabPanel m_timeUI;

		// Token: 0x040028A0 RID: 10400
		[SerializeField]
		private UIManager.PrefabPanel m_inspectionUI;

		// Token: 0x040028A1 RID: 10401
		[SerializeField]
		private UIManager.TooltipPanel[] m_tooltipPrefabs;

		// Token: 0x040028A2 RID: 10402
		[SerializeField]
		private UIManager.DialogPanel[] m_dialogPrefabs;

		// Token: 0x040028A3 RID: 10403
		[SerializeField]
		private EffectIconPanelUI m_selfEffectPanelUI;

		// Token: 0x040028A4 RID: 10404
		[SerializeField]
		private NameplateControllerUI m_selfNamePlateUi;

		// Token: 0x040028A5 RID: 10405
		[SerializeField]
		private NameplateControllerUI m_offensiveNameplateUI;

		// Token: 0x040028A6 RID: 10406
		[SerializeField]
		private NameplateControllerUI m_defensiveNamePlateUI;

		// Token: 0x040028A7 RID: 10407
		[SerializeField]
		private UnconsciousPanelUI m_uncoPanel;

		// Token: 0x040028A8 RID: 10408
		[SerializeField]
		private UIWindow m_gameHelpPanel;

		// Token: 0x040028A9 RID: 10409
		[SerializeField]
		private InGameUIOptions m_inGameUiOptions;

		// Token: 0x040028AA RID: 10410
		[SerializeField]
		private InGameUIMenu m_inGameUiMenu;

		// Token: 0x040028AB RID: 10411
		[SerializeField]
		private NotificationsUI m_notificationsUI;

		// Token: 0x040028AC RID: 10412
		[SerializeField]
		private CanvasGroup[] m_uiCanvasGroups;

		// Token: 0x040028AD RID: 10413
		[SerializeField]
		private Image m_centerReticle;

		// Token: 0x040028AE RID: 10414
		[SerializeField]
		private ArchetypeInstanceUIDragShadow m_dragShadow;

		// Token: 0x040028AF RID: 10415
		[SerializeField]
		private GameDataLabel m_nonLiveGameDataLabel;

		// Token: 0x040028B0 RID: 10416
		[SerializeField]
		private RectTransform m_uiSpaceNameplatePanel;

		// Token: 0x040028D9 RID: 10457
		private LocalGameTimeUI _localGameTimeUI;

		// Token: 0x040028DA RID: 10458
		private DpsMeterController _dpsMeter;

		// Token: 0x040028DB RID: 10459
		private CountdownUI _countdownUI;

		// Token: 0x040028F0 RID: 10480
		private readonly Queue<CenterScreenAnnouncementOptions> m_centerScreenQueue = new Queue<CenterScreenAnnouncementOptions>();

		// Token: 0x040028F1 RID: 10481
		private readonly Queue<TutorialPopupOptions> m_tutorialQueue = new Queue<TutorialPopupOptions>();

		// Token: 0x040028F2 RID: 10482
		private int m_previousWidth;

		// Token: 0x040028F3 RID: 10483
		private int m_previousHeight;

		// Token: 0x040028F4 RID: 10484
		private Transform m_previousParent;

		// Token: 0x040028F5 RID: 10485
		private IDraggable m_dragged;

		// Token: 0x040028F6 RID: 10486
		private int m_dragFrame;

		// Token: 0x040028F7 RID: 10487
		private UIManager.UIPooledAudio m_audioPool;

		// Token: 0x040028F8 RID: 10488
		public const string kCannotPlaceHere = "Cannot place here!";

		// Token: 0x040028F9 RID: 10489
		public const string kInCombatStance = "In Combat Stance!";

		// Token: 0x040028FA RID: 10490
		public const string kCannotEquipItem = "Cannot Equip Item!";

		// Token: 0x040028FB RID: 10491
		public const string kNoRoom = "No Room!";

		// Token: 0x02000518 RID: 1304
		[Serializable]
		private class PrefabPanel
		{
			// Token: 0x040028FC RID: 10492
			public string PlayerPrefsKey;

			// Token: 0x040028FD RID: 10493
			public RectTransform Panel;

			// Token: 0x040028FE RID: 10494
			public GameObject Prefab;
		}

		// Token: 0x02000519 RID: 1305
		[Serializable]
		private class TooltipPanel : UIManager.PrefabPanel
		{
			// Token: 0x040028FF RID: 10495
			public TooltipType Type;
		}

		// Token: 0x0200051A RID: 1306
		[Serializable]
		private class DialogPanel : UIManager.PrefabPanel
		{
			// Token: 0x04002900 RID: 10496
			public DialogType Type;
		}

		// Token: 0x0200051B RID: 1307
		private class UIPooledAudio
		{
			// Token: 0x06002735 RID: 10037 RVA: 0x00136B1C File Offset: 0x00134D1C
			public UIPooledAudio(UIManager manager)
			{
				this.m_manager = manager;
				this.m_defaultSource = this.m_manager.gameObject.AddComponent<AudioSource>();
				this.m_defaultSource.ConfigureAudioSourceForUI();
				this.m_sources = new List<UIManager.UIPooledAudio.AudioSourceStatus>(3);
				for (int i = 0; i < 3; i++)
				{
					this.AddSource();
				}
			}

			// Token: 0x06002736 RID: 10038 RVA: 0x00136B78 File Offset: 0x00134D78
			private void AddSource()
			{
				AudioSource source = this.m_manager.gameObject.AddComponent<AudioSource>();
				source.ConfigureAudioSourceForUI();
				UIManager.UIPooledAudio.AudioSourceStatus item = new UIManager.UIPooledAudio.AudioSourceStatus(source);
				this.m_sources.Add(item);
			}

			// Token: 0x06002737 RID: 10039 RVA: 0x00136BB0 File Offset: 0x00134DB0
			public void PlayRandomClip(AudioClipCollection collection, float? volume = null)
			{
				if (collection == null)
				{
					return;
				}
				AudioClip randomClip = collection.GetRandomClip();
				float value = collection.PitchRange.RandomWithinRange();
				this.PlayClip(randomClip, new float?(value), volume);
			}

			// Token: 0x06002738 RID: 10040 RVA: 0x00136BEC File Offset: 0x00134DEC
			public void PlayClip(AudioClip clip, float? pitch, float? volume)
			{
				if (clip == null)
				{
					return;
				}
				bool flag = pitch == null || pitch.Value == 1f;
				bool flag2 = volume == null || volume.Value == GlobalSettings.Values.Audio.DefaultUIVolume;
				if (flag && flag2)
				{
					this.m_defaultSource.PlayOneShot(clip);
					return;
				}
				UIManager.UIPooledAudio.AudioSourceStatus audioSourceStatus = null;
				for (int i = 0; i < this.m_sources.Count; i++)
				{
					if (this.m_sources[i].IsFree())
					{
						audioSourceStatus = this.m_sources[i];
						break;
					}
				}
				if (audioSourceStatus == null)
				{
					this.AddSource();
					audioSourceStatus = this.m_sources[this.m_sources.Count - 1];
				}
				audioSourceStatus.Source.volume = (flag2 ? GlobalSettings.Values.Audio.DefaultUIVolume : volume.Value);
				audioSourceStatus.Source.pitch = (flag ? 1f : pitch.Value);
				audioSourceStatus.LastPlayTime = DateTime.UtcNow;
				audioSourceStatus.LastDuration = clip.length;
				audioSourceStatus.Source.PlayOneShot(clip);
			}

			// Token: 0x04002901 RID: 10497
			private const int kStartingPool = 3;

			// Token: 0x04002902 RID: 10498
			private readonly UIManager m_manager;

			// Token: 0x04002903 RID: 10499
			private AudioSource m_defaultSource;

			// Token: 0x04002904 RID: 10500
			private List<UIManager.UIPooledAudio.AudioSourceStatus> m_sources;

			// Token: 0x0200051C RID: 1308
			private class AudioSourceStatus
			{
				// Token: 0x06002739 RID: 10041 RVA: 0x0005B7BA File Offset: 0x000599BA
				public AudioSourceStatus(AudioSource source)
				{
					this.Source = source;
				}

				// Token: 0x0600273A RID: 10042 RVA: 0x00136D18 File Offset: 0x00134F18
				public bool IsFree()
				{
					return (DateTime.UtcNow - this.LastPlayTime).TotalSeconds > (double)this.LastDuration;
				}

				// Token: 0x04002905 RID: 10501
				public readonly AudioSource Source;

				// Token: 0x04002906 RID: 10502
				public DateTime LastPlayTime = DateTime.MinValue;

				// Token: 0x04002907 RID: 10503
				public float LastDuration;
			}
		}
	}
}

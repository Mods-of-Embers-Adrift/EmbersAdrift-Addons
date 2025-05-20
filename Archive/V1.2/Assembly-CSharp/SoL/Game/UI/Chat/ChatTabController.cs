using System;
using System.Collections.Generic;
using SoL.Game.Messages;
using UnityEngine;
using UnityEngine.UI;

namespace SoL.Game.UI.Chat
{
	// Token: 0x020009AB RID: 2475
	public class ChatTabController : MonoBehaviour
	{
		// Token: 0x17001069 RID: 4201
		// (get) Token: 0x06004A3B RID: 19003 RVA: 0x00071FE9 File Offset: 0x000701E9
		public ScrollRect ScrollRect
		{
			get
			{
				return this.m_scrollRect;
			}
		}

		// Token: 0x140000E8 RID: 232
		// (add) Token: 0x06004A3C RID: 19004 RVA: 0x001B288C File Offset: 0x001B0A8C
		// (remove) Token: 0x06004A3D RID: 19005 RVA: 0x001B28C4 File Offset: 0x001B0AC4
		public event Action<ChatTab> TabAdded;

		// Token: 0x140000E9 RID: 233
		// (add) Token: 0x06004A3E RID: 19006 RVA: 0x001B28FC File Offset: 0x001B0AFC
		// (remove) Token: 0x06004A3F RID: 19007 RVA: 0x001B2934 File Offset: 0x001B0B34
		public event Action<ChatTab> TabChanged;

		// Token: 0x140000EA RID: 234
		// (add) Token: 0x06004A40 RID: 19008 RVA: 0x001B296C File Offset: 0x001B0B6C
		// (remove) Token: 0x06004A41 RID: 19009 RVA: 0x001B29A4 File Offset: 0x001B0BA4
		public event Action<int> TabRemoved;

		// Token: 0x1700106A RID: 4202
		// (get) Token: 0x06004A42 RID: 19010 RVA: 0x00071FF1 File Offset: 0x000701F1
		public int ActiveTabIndex
		{
			get
			{
				return this.m_activeTabIndex;
			}
		}

		// Token: 0x1700106B RID: 4203
		// (get) Token: 0x06004A43 RID: 19011 RVA: 0x00071FF9 File Offset: 0x000701F9
		public ChatTab ActiveTab
		{
			get
			{
				if (this.m_activeTabIndex < 0)
				{
					return null;
				}
				return this.m_tabs[this.m_activeTabIndex];
			}
		}

		// Token: 0x1700106C RID: 4204
		// (get) Token: 0x06004A44 RID: 19012 RVA: 0x00072017 File Offset: 0x00070217
		public List<ChatTab> Tabs
		{
			get
			{
				return this.m_tabs;
			}
		}

		// Token: 0x06004A45 RID: 19013 RVA: 0x001B29DC File Offset: 0x001B0BDC
		private void OnDestroy()
		{
			foreach (ChatTab chatTab in this.m_tabs)
			{
				chatTab.TabChanged -= this.OnInternalTabChanged;
				chatTab.CloseClicked -= this.OnCloseClicked;
				chatTab.Renamed -= this.OnTabRenamed;
			}
		}

		// Token: 0x06004A46 RID: 19014 RVA: 0x001B2A5C File Offset: 0x001B0C5C
		public void AddTab(ChatTabMode mode = ChatTabMode.Chat)
		{
			Rect rect = this.m_tabPrefab.RectTransform.rect;
			ChatTab chatTab = (this.m_tabPool.Count == 0) ? UnityEngine.Object.Instantiate<ChatTab>(this.m_tabPrefab, this.m_scrollRect.content) : this.m_tabPool.Pop();
			chatTab.gameObject.SetActive(true);
			chatTab.Init(this.m_parent, (mode == ChatTabMode.Chat) ? MessageManager.ChatQueue : MessageManager.CombatQueue, this.m_toggleGroup);
			chatTab.TabChanged += this.OnInternalTabChanged;
			chatTab.CloseClicked += this.OnCloseClicked;
			chatTab.Renamed += this.OnTabRenamed;
			this.m_tabs.Add(chatTab);
			this.FocusTab(this.m_tabs.Count - 1);
			Action<ChatTab> tabAdded = this.TabAdded;
			if (tabAdded == null)
			{
				return;
			}
			tabAdded(chatTab);
		}

		// Token: 0x06004A47 RID: 19015 RVA: 0x001B2B40 File Offset: 0x001B0D40
		public void AddTab(ChatTab fromTab, int insertionIndex = -1)
		{
			Rect rect = this.m_tabPrefab.RectTransform.rect;
			ChatTab chatTab = (this.m_tabPool.Count == 0) ? UnityEngine.Object.Instantiate<ChatTab>(this.m_tabPrefab, this.m_scrollRect.content) : this.m_tabPool.Pop();
			chatTab.gameObject.SetActive(true);
			chatTab.Name = fromTab.Name;
			chatTab.ChatFilter = fromTab.ChatFilter;
			chatTab.CombatFilter = fromTab.CombatFilter;
			chatTab.SetInputChannel(fromTab.InputChannel);
			chatTab.Init(this.m_parent, fromTab.Queue, this.m_toggleGroup);
			chatTab.TabChanged += this.OnInternalTabChanged;
			chatTab.CloseClicked += this.OnCloseClicked;
			chatTab.Renamed += this.OnTabRenamed;
			if (insertionIndex < 0 || insertionIndex >= this.m_tabs.Count)
			{
				this.m_tabs.Add(chatTab);
				this.FocusTab(this.m_tabs.Count - 1);
			}
			else
			{
				this.m_tabs.Insert(insertionIndex, chatTab);
				chatTab.transform.SetSiblingIndex(insertionIndex);
				this.FocusTab(insertionIndex);
			}
			Action<ChatTab> tabAdded = this.TabAdded;
			if (tabAdded == null)
			{
				return;
			}
			tabAdded(chatTab);
		}

		// Token: 0x06004A48 RID: 19016 RVA: 0x0007201F File Offset: 0x0007021F
		public void RemoveTab(int index)
		{
			if (this.m_tabs.Count == 0 || index < 0 || index >= this.m_tabs.Count)
			{
				return;
			}
			this.RemoveTab(this.m_tabs[index], true);
		}

		// Token: 0x06004A49 RID: 19017 RVA: 0x001B2C80 File Offset: 0x001B0E80
		public void RemoveTab(ChatTab tab, bool resetTab = true)
		{
			tab.TabChanged -= this.OnInternalTabChanged;
			tab.CloseClicked -= this.OnCloseClicked;
			tab.Renamed -= this.OnTabRenamed;
			int num = this.m_tabs.IndexOf(tab);
			if (resetTab)
			{
				tab.Reset();
			}
			tab.gameObject.SetActive(false);
			this.m_tabs.Remove(tab);
			this.m_tabPool.Push(tab);
			if (this.m_tabs.Count == 0)
			{
				this.m_activeTabIndex = -1;
			}
			else if (num > 0)
			{
				this.FocusTab(num - 1);
			}
			else
			{
				this.FocusTab(0);
			}
			Action<int> tabRemoved = this.TabRemoved;
			if (tabRemoved == null)
			{
				return;
			}
			tabRemoved(num);
		}

		// Token: 0x06004A4A RID: 19018 RVA: 0x00072054 File Offset: 0x00070254
		public void TransferTab(ChatTab tab, ChatWindowUI destinationWindow, int insertionIndex = -1)
		{
			if (destinationWindow == null)
			{
				throw new InvalidOperationException();
			}
			this.RemoveTab(tab, false);
			destinationWindow.TabController.AddTab(tab, insertionIndex);
		}

		// Token: 0x06004A4B RID: 19019 RVA: 0x001B2D3C File Offset: 0x001B0F3C
		public void TransferTab(ChatTab tab, Vector3 position)
		{
			this.RemoveTab(tab, false);
			ChatWindowUI chatWindowUI = UnityEngine.Object.Instantiate<ChatWindowUI>(this.m_parent, this.m_parent.transform.parent);
			chatWindowUI.ShouldLoadFromSettings = false;
			chatWindowUI.CreateAtPosition = position;
			chatWindowUI.TabController.RecoverOrphanedTabs();
			for (int i = 0; i < chatWindowUI.ChatList.Content.childCount; i++)
			{
				chatWindowUI.ChatList.Content.GetChild(i).gameObject.SetActive(false);
			}
			for (int j = 0; j < chatWindowUI.ChatTabFilterList.Content.childCount; j++)
			{
				chatWindowUI.ChatTabFilterList.Content.GetChild(j).gameObject.SetActive(false);
			}
			chatWindowUI.TabController.ClearTabs();
			chatWindowUI.TabController.AddTab(tab, -1);
			chatWindowUI.CurrentSettings = new ChatWindowSettings
			{
				ShowTimestamps = this.m_parent.CurrentSettings.ShowTimestamps,
				Opacity = this.m_parent.CurrentSettings.Opacity,
				TabSettings = new List<ChatTabSettings>
				{
					new ChatTabSettings
					{
						Mode = tab.Mode,
						ChatFilter = tab.ChatFilter,
						CombatFilter = tab.CombatFilter,
						InputChannel = tab.InputChannel
					}
				}
			};
			chatWindowUI.CurrentSettings.SetAsCurrentVersion();
		}

		// Token: 0x06004A4C RID: 19020 RVA: 0x001B2E94 File Offset: 0x001B1094
		public void RecoverOrphanedTabs()
		{
			for (int i = 0; i < this.m_scrollRect.content.childCount; i++)
			{
				ChatTab item;
				if (this.m_scrollRect.content.GetChild(i).gameObject.TryGetComponent<ChatTab>(out item))
				{
					this.m_tabs.Add(item);
				}
			}
		}

		// Token: 0x06004A4D RID: 19021 RVA: 0x001B2EE8 File Offset: 0x001B10E8
		public void ClearTabs()
		{
			int count = this.m_tabs.Count;
			for (int i = 0; i < count; i++)
			{
				this.RemoveTab(0);
			}
		}

		// Token: 0x06004A4E RID: 19022 RVA: 0x0007207A File Offset: 0x0007027A
		public void FocusTab(int index)
		{
			if (this.m_tabs.Count == 0 || index < 0 || index >= this.m_tabs.Count)
			{
				return;
			}
			this.m_activeTabIndex = index;
			this.m_tabs[index].Toggle.isOn = true;
		}

		// Token: 0x06004A4F RID: 19023 RVA: 0x000720BA File Offset: 0x000702BA
		private void OnInternalTabChanged(ChatTab tab)
		{
			this.m_activeTabIndex = this.m_tabs.IndexOf(tab);
			Action<ChatTab> tabChanged = this.TabChanged;
			if (tabChanged == null)
			{
				return;
			}
			tabChanged(tab);
		}

		// Token: 0x06004A50 RID: 19024 RVA: 0x000720DF File Offset: 0x000702DF
		private void OnCloseClicked(ChatTab tab)
		{
			this.RemoveTab(tab, true);
		}

		// Token: 0x06004A51 RID: 19025 RVA: 0x001B2F14 File Offset: 0x001B1114
		private void OnTabRenamed(ChatTab tab)
		{
			int num = this.m_tabs.IndexOf(tab);
			if (this.m_parent.CurrentSettings != null && this.m_parent.CurrentSettings.TabSettings != null && this.m_parent.CurrentSettings.TabSettings.Count > num)
			{
				this.m_parent.CurrentSettings.TabSettings[num].Name = tab.Name;
				this.m_parent.CurrentSettings.MarkAsDirty();
			}
		}

		// Token: 0x06004A52 RID: 19026 RVA: 0x001B2F98 File Offset: 0x001B1198
		public void ChatColorsChanged()
		{
			for (int i = 0; i < this.m_tabs.Count; i++)
			{
				if (this.m_tabs[i].Mode == ChatTabMode.Chat)
				{
					this.m_tabs[i].RefreshTabVisuals(this.m_tabs[i].Toggle.isOn);
				}
			}
		}

		// Token: 0x04004521 RID: 17697
		[SerializeField]
		private ScrollRect m_scrollRect;

		// Token: 0x04004522 RID: 17698
		[SerializeField]
		private ChatWindowUI m_parent;

		// Token: 0x04004523 RID: 17699
		[SerializeField]
		private ChatTab m_tabPrefab;

		// Token: 0x04004524 RID: 17700
		[SerializeField]
		private ChatList m_contentPrefab;

		// Token: 0x04004525 RID: 17701
		[SerializeField]
		private ToggleGroup m_toggleGroup;

		// Token: 0x04004526 RID: 17702
		private int m_activeTabIndex = -1;

		// Token: 0x04004527 RID: 17703
		private Stack<ChatTab> m_tabPool = new Stack<ChatTab>();

		// Token: 0x04004528 RID: 17704
		private List<ChatTab> m_tabs = new List<ChatTab>();
	}
}

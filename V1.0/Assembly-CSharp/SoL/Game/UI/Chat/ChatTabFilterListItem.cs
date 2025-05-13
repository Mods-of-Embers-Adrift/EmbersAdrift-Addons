using System;
using SoL.Game.Messages;
using SoL.Game.Settings;
using SoL.UI;
using UnityEngine;
using UnityEngine.Events;

namespace SoL.Game.UI.Chat
{
	// Token: 0x020009AE RID: 2478
	public class ChatTabFilterListItem : MonoBehaviour
	{
		// Token: 0x17001070 RID: 4208
		// (get) Token: 0x06004A64 RID: 19044 RVA: 0x000721A5 File Offset: 0x000703A5
		public ChatFilter ChatFilter
		{
			get
			{
				return (ChatFilter)this.m_itemFilter;
			}
		}

		// Token: 0x17001071 RID: 4209
		// (get) Token: 0x06004A65 RID: 19045 RVA: 0x000721A5 File Offset: 0x000703A5
		public CombatFilter CombatFilter
		{
			get
			{
				return (CombatFilter)this.m_itemFilter;
			}
		}

		// Token: 0x06004A66 RID: 19046 RVA: 0x000721AD File Offset: 0x000703AD
		private void Start()
		{
			this.m_toggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnToggleChanged));
		}

		// Token: 0x06004A67 RID: 19047 RVA: 0x000721CB File Offset: 0x000703CB
		private void OnDestroy()
		{
			this.m_toggle.onValueChanged.RemoveListener(new UnityAction<bool>(this.OnToggleChanged));
		}

		// Token: 0x06004A68 RID: 19048 RVA: 0x000721E9 File Offset: 0x000703E9
		public void Init(ChatTabFilterList parent, int data, int currentFilter, int index)
		{
			this.m_parent = parent;
			this.m_itemFilter = data;
			this.m_index = index;
			this.Refresh();
		}

		// Token: 0x06004A69 RID: 19049 RVA: 0x001B3240 File Offset: 0x001B1440
		public void Refresh()
		{
			Color white = Color.white;
			bool color;
			if (this.m_parent.Mode == ChatTabMode.Chat)
			{
				color = this.ChatFilter.GetMessageType().GetColor(out white, false);
			}
			else
			{
				color = this.CombatFilter.GetMessageType().GetColor(out white, false);
			}
			this.m_ignoreToggle = true;
			SolToggle toggle = this.m_toggle;
			string text;
			if (this.m_parent.Mode != ChatTabMode.Chat)
			{
				CombatFilter itemFilter = (CombatFilter)this.m_itemFilter;
				text = itemFilter.ToString();
			}
			else
			{
				ChatFilter itemFilter2 = (ChatFilter)this.m_itemFilter;
				text = itemFilter2.ToString();
			}
			toggle.text = text;
			this.m_toggle.isOn = ((this.m_parent.Filter & this.m_itemFilter) == this.m_itemFilter);
			if (color)
			{
				this.m_toggle.SetTextColor(white);
			}
			else
			{
				this.m_toggle.SetTextColor(UISettings.StandardTextColor);
			}
			this.m_ignoreToggle = false;
		}

		// Token: 0x06004A6A RID: 19050 RVA: 0x00072207 File Offset: 0x00070407
		public void UpdateToggle(int newFilter)
		{
			this.m_ignoreToggle = true;
			this.m_toggle.isOn = ((newFilter & this.m_itemFilter) == this.m_itemFilter);
			this.m_ignoreToggle = false;
		}

		// Token: 0x06004A6B RID: 19051 RVA: 0x00072232 File Offset: 0x00070432
		private void OnToggleChanged(bool isOn)
		{
			if (!this.m_ignoreToggle)
			{
				this.m_parent.InternalFilterUpdate(this.m_itemFilter, isOn);
			}
		}

		// Token: 0x04004532 RID: 17714
		[SerializeField]
		private SolToggle m_toggle;

		// Token: 0x04004533 RID: 17715
		private ChatTabFilterList m_parent;

		// Token: 0x04004534 RID: 17716
		private int m_itemFilter;

		// Token: 0x04004535 RID: 17717
		private int m_index = -1;

		// Token: 0x04004536 RID: 17718
		private bool m_ignoreToggle;
	}
}

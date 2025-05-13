using System;
using System.Collections.Generic;
using SoL.Game.Notifications;
using SoL.UI;
using SoL.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SoL.Game.UI.Quests
{
	// Token: 0x02000953 RID: 2387
	public class TutorialCategoriesListItem : MonoBehaviour
	{
		// Token: 0x17000FBC RID: 4028
		// (get) Token: 0x060046AF RID: 18095 RVA: 0x0006F91E File Offset: 0x0006DB1E
		// (set) Token: 0x060046B0 RID: 18096 RVA: 0x0006F936 File Offset: 0x0006DB36
		public bool Expanded
		{
			get
			{
				return this.m_parent.Expanded[this.m_itemIndex];
			}
			set
			{
				this.m_parent.Expanded[this.m_itemIndex] = value;
			}
		}

		// Token: 0x17000FBD RID: 4029
		// (get) Token: 0x060046B1 RID: 18097 RVA: 0x0006F94F File Offset: 0x0006DB4F
		public bool IsListInitialized
		{
			get
			{
				return this.m_nestedList.IsInitialized;
			}
		}

		// Token: 0x060046B2 RID: 18098 RVA: 0x001A4EFC File Offset: 0x001A30FC
		private void Start()
		{
			this.m_nestedList.SelectionChanged += this.OnSelectionChanged;
			this.m_categoryToggle.onClick.AddListener(new UnityAction(this.ToggleExpand));
			this.m_nestedList.Initialized += this.m_parent.OnListInitialized;
		}

		// Token: 0x060046B3 RID: 18099 RVA: 0x001A4F58 File Offset: 0x001A3158
		private void OnDestroy()
		{
			this.m_nestedList.SelectionChanged -= this.OnSelectionChanged;
			this.m_categoryToggle.onClick.RemoveAllListeners();
			this.m_nestedList.Initialized -= this.m_parent.OnListInitialized;
			this.m_nestedList.Initialized -= this.UpdateList;
		}

		// Token: 0x060046B4 RID: 18100 RVA: 0x001A4FC0 File Offset: 0x001A31C0
		public void Init(TutorialCategoriesList parent, Category<BaseNotification> category, int index)
		{
			this.m_parent = parent;
			this.m_category = category;
			this.m_itemIndex = index;
			this.m_playerPrefsKey = this.m_parent.PlayerPrefsKey + "_" + category.Name;
			this.RefreshVisuals();
			this.UpdateListWhenReady();
		}

		// Token: 0x060046B5 RID: 18101 RVA: 0x0006F95C File Offset: 0x0006DB5C
		public void Refresh()
		{
			this.RefreshVisuals();
			this.UpdateListWhenReady();
		}

		// Token: 0x060046B6 RID: 18102 RVA: 0x001A5010 File Offset: 0x001A3210
		public void RefreshVisuals()
		{
			this.m_iconSymbol.sprite = (this.Expanded ? this.m_expandedIcon : this.m_unexpandedIcon);
			this.m_label.text = this.m_category.Name;
			this.m_bodyRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, this.Expanded ? ((float)this.m_category.Data.Count * 20f) : 0f);
			this.m_selfRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, this.Expanded ? ((float)this.m_category.Data.Count * 20f) : 0f);
			this.m_nestedList.gameObject.SetActive(this.Expanded);
		}

		// Token: 0x060046B7 RID: 18103 RVA: 0x0006F96A File Offset: 0x0006DB6A
		public void Reindex(int index, BaseNotification selectedItem)
		{
			this.m_itemIndex = index;
			if (this.m_nestedList.IsInitialized)
			{
				this.m_nestedList.ReindexItems(selectedItem);
			}
			this.RefreshVisuals();
		}

		// Token: 0x060046B8 RID: 18104 RVA: 0x0006F992 File Offset: 0x0006DB92
		public void Select(BaseNotification item)
		{
			if (this.m_nestedList.IsInitialized)
			{
				this.m_nestedList.Select(item);
				return;
			}
			this.m_selectOnInit = item;
		}

		// Token: 0x060046B9 RID: 18105 RVA: 0x0006F9B5 File Offset: 0x0006DBB5
		public void Deselect(bool suppressEvents = false)
		{
			if (this.m_nestedList.IsInitialized)
			{
				this.m_nestedList.DeselectAll(suppressEvents);
			}
		}

		// Token: 0x060046BA RID: 18106 RVA: 0x0006F9D0 File Offset: 0x0006DBD0
		public void Expand()
		{
			if (!this.Expanded)
			{
				this.ToggleExpand();
			}
		}

		// Token: 0x060046BB RID: 18107 RVA: 0x0006F9E0 File Offset: 0x0006DBE0
		public void Collapse()
		{
			if (this.Expanded)
			{
				this.ToggleExpand();
			}
		}

		// Token: 0x060046BC RID: 18108 RVA: 0x001A50D0 File Offset: 0x001A32D0
		private void ToggleExpand()
		{
			this.Expanded = !this.Expanded;
			PlayerPrefs.SetInt(this.m_playerPrefsKey + "_Expanded", this.Expanded ? 1 : 0);
			this.RefreshVisuals();
			this.m_parent.Refresh(false, false);
			this.m_parent.ReindexInPlace();
		}

		// Token: 0x060046BD RID: 18109 RVA: 0x0006F9F0 File Offset: 0x0006DBF0
		private void UpdateListWhenReady()
		{
			if (this.m_nestedList.IsInitialized)
			{
				this.UpdateList();
				return;
			}
			this.m_nestedList.Initialized += this.UpdateList;
		}

		// Token: 0x060046BE RID: 18110 RVA: 0x001A512C File Offset: 0x001A332C
		private void UpdateList()
		{
			List<BaseNotification> fromPool = StaticListPool<BaseNotification>.GetFromPool();
			fromPool.AddRange(this.m_category.Data);
			this.m_nestedList.Sort(fromPool);
			this.m_nestedList.UpdateItems(fromPool);
			StaticListPool<BaseNotification>.ReturnToPool(fromPool);
			if (this.m_selectOnInit != null)
			{
				this.m_nestedList.Select(this.m_selectOnInit);
				this.m_selectOnInit = null;
			}
		}

		// Token: 0x060046BF RID: 18111 RVA: 0x0006FA1D File Offset: 0x0006DC1D
		private void OnSelectionChanged(BaseNotification selectedItem)
		{
			this.m_parent.OnSelectionChanged(this.m_itemIndex, selectedItem);
		}

		// Token: 0x040042AB RID: 17067
		public const float kItemHeight = 20f;

		// Token: 0x040042AC RID: 17068
		[SerializeField]
		private SolButton m_categoryToggle;

		// Token: 0x040042AD RID: 17069
		[SerializeField]
		private Image m_iconSymbol;

		// Token: 0x040042AE RID: 17070
		[SerializeField]
		private TextMeshProUGUI m_label;

		// Token: 0x040042AF RID: 17071
		[SerializeField]
		private RectTransform m_selfRectTransform;

		// Token: 0x040042B0 RID: 17072
		[SerializeField]
		private RectTransform m_bodyRectTransform;

		// Token: 0x040042B1 RID: 17073
		[SerializeField]
		private TutorialsList m_nestedList;

		// Token: 0x040042B2 RID: 17074
		[SerializeField]
		private Sprite m_unexpandedIcon;

		// Token: 0x040042B3 RID: 17075
		[SerializeField]
		private Sprite m_expandedIcon;

		// Token: 0x040042B4 RID: 17076
		private TutorialCategoriesList m_parent;

		// Token: 0x040042B5 RID: 17077
		private Category<BaseNotification> m_category;

		// Token: 0x040042B6 RID: 17078
		private int m_itemIndex = -1;

		// Token: 0x040042B7 RID: 17079
		private string m_playerPrefsKey = string.Empty;

		// Token: 0x040042B8 RID: 17080
		private BaseNotification m_selectOnInit;
	}
}

using System;
using System.Collections.Generic;
using SoL.Game.Quests;
using SoL.UI;
using SoL.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SoL.Game.UI.Quests
{
	// Token: 0x02000947 RID: 2375
	public class QuestCategoriesListItem : MonoBehaviour
	{
		// Token: 0x17000FAA RID: 4010
		// (get) Token: 0x06004625 RID: 17957 RVA: 0x0006F2AF File Offset: 0x0006D4AF
		// (set) Token: 0x06004626 RID: 17958 RVA: 0x0006F2C7 File Offset: 0x0006D4C7
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

		// Token: 0x17000FAB RID: 4011
		// (get) Token: 0x06004627 RID: 17959 RVA: 0x0006F2E0 File Offset: 0x0006D4E0
		public bool IsListInitialized
		{
			get
			{
				return this.m_nestedList.IsInitialized;
			}
		}

		// Token: 0x06004628 RID: 17960 RVA: 0x001A2688 File Offset: 0x001A0888
		private void Start()
		{
			this.m_nestedList.SelectionChanged += this.OnSelectionChanged;
			this.m_categoryToggle.onClick.AddListener(new UnityAction(this.ToggleExpand));
			this.m_nestedList.Initialized += this.m_parent.OnListInitialized;
		}

		// Token: 0x06004629 RID: 17961 RVA: 0x001A26E4 File Offset: 0x001A08E4
		private void OnDestroy()
		{
			this.m_nestedList.SelectionChanged -= this.OnSelectionChanged;
			this.m_categoryToggle.onClick.RemoveAllListeners();
			this.m_nestedList.Initialized -= this.m_parent.OnListInitialized;
			this.m_nestedList.Initialized -= this.UpdateList;
		}

		// Token: 0x0600462A RID: 17962 RVA: 0x001A274C File Offset: 0x001A094C
		public void Init(QuestCategoriesList parent, Category<Quest> category, int index)
		{
			this.m_parent = parent;
			this.m_category = category;
			this.m_itemIndex = index;
			this.m_playerPrefsKey = this.m_parent.PlayerPrefsKey + "_" + category.Name;
			this.RefreshVisuals();
			this.UpdateListWhenReady();
		}

		// Token: 0x0600462B RID: 17963 RVA: 0x0006F2ED File Offset: 0x0006D4ED
		public void Refresh(List<Quest> quests)
		{
			this.m_category.Data = quests;
			this.RefreshVisuals();
			this.UpdateListWhenReady();
		}

		// Token: 0x0600462C RID: 17964 RVA: 0x001A279C File Offset: 0x001A099C
		public void RefreshVisuals()
		{
			this.m_iconSymbol.sprite = (this.Expanded ? this.m_expandedIcon : this.m_unexpandedIcon);
			this.m_label.text = this.m_category.Name;
			this.m_bodyRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, this.Expanded ? ((float)this.m_category.Data.Count * 20f) : 0f);
			this.m_selfRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, this.Expanded ? ((float)this.m_category.Data.Count * 20f) : 0f);
			this.m_nestedList.gameObject.SetActive(this.Expanded);
		}

		// Token: 0x0600462D RID: 17965 RVA: 0x0006F307 File Offset: 0x0006D507
		public void Reindex(int index, Quest selectedItem)
		{
			this.m_itemIndex = index;
			if (this.m_nestedList.IsInitialized)
			{
				this.m_nestedList.ReindexItems(selectedItem);
			}
			this.RefreshVisuals();
		}

		// Token: 0x0600462E RID: 17966 RVA: 0x0006F32F File Offset: 0x0006D52F
		public void Deselect(bool suppressEvents = false)
		{
			if (this.m_nestedList.IsInitialized)
			{
				this.m_nestedList.DeselectAll(suppressEvents);
			}
		}

		// Token: 0x0600462F RID: 17967 RVA: 0x001A285C File Offset: 0x001A0A5C
		private void ToggleExpand()
		{
			this.Expanded = !this.Expanded;
			PlayerPrefs.SetInt(this.m_playerPrefsKey + "_Expanded", this.Expanded ? 1 : 0);
			this.RefreshVisuals();
			this.m_parent.Refresh(false, false);
			this.m_parent.ReindexInPlace();
		}

		// Token: 0x06004630 RID: 17968 RVA: 0x0006F34A File Offset: 0x0006D54A
		private void UpdateListWhenReady()
		{
			if (this.m_nestedList.IsInitialized)
			{
				this.UpdateList();
				return;
			}
			this.m_nestedList.Initialized += this.UpdateList;
		}

		// Token: 0x06004631 RID: 17969 RVA: 0x001A28B8 File Offset: 0x001A0AB8
		private void UpdateList()
		{
			List<Quest> fromPool = StaticListPool<Quest>.GetFromPool();
			fromPool.AddRange(this.m_category.Data);
			this.m_nestedList.Sort(fromPool);
			this.m_nestedList.UpdateItems(fromPool);
			StaticListPool<Quest>.ReturnToPool(fromPool);
		}

		// Token: 0x06004632 RID: 17970 RVA: 0x0006F377 File Offset: 0x0006D577
		private void OnSelectionChanged(Quest selectedItem)
		{
			this.m_parent.Select(this.m_itemIndex, selectedItem);
		}

		// Token: 0x04004246 RID: 16966
		public const float kItemHeight = 20f;

		// Token: 0x04004247 RID: 16967
		[SerializeField]
		private SolButton m_categoryToggle;

		// Token: 0x04004248 RID: 16968
		[SerializeField]
		private Image m_iconSymbol;

		// Token: 0x04004249 RID: 16969
		[SerializeField]
		private TextMeshProUGUI m_label;

		// Token: 0x0400424A RID: 16970
		[SerializeField]
		private RectTransform m_selfRectTransform;

		// Token: 0x0400424B RID: 16971
		[SerializeField]
		private RectTransform m_bodyRectTransform;

		// Token: 0x0400424C RID: 16972
		[SerializeField]
		private QuestsList m_nestedList;

		// Token: 0x0400424D RID: 16973
		[SerializeField]
		private Sprite m_unexpandedIcon;

		// Token: 0x0400424E RID: 16974
		[SerializeField]
		private Sprite m_expandedIcon;

		// Token: 0x0400424F RID: 16975
		private QuestCategoriesList m_parent;

		// Token: 0x04004250 RID: 16976
		private Category<Quest> m_category;

		// Token: 0x04004251 RID: 16977
		private int m_itemIndex = -1;

		// Token: 0x04004252 RID: 16978
		private string m_playerPrefsKey = string.Empty;
	}
}

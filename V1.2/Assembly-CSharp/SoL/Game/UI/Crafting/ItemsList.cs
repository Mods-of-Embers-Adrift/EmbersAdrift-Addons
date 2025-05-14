using System;
using System.Collections.Generic;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomParams;
using SoL.Game.Objects.Archetypes;
using SoL.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SoL.Game.UI.Crafting
{
	// Token: 0x02000997 RID: 2455
	public class ItemsList : OSA<BaseParamsWithPrefab, ItemItemViewsHolder>, IPointerDownHandler, IEventSystemHandler
	{
		// Token: 0x17001041 RID: 4161
		// (get) Token: 0x06004974 RID: 18804 RVA: 0x00071593 File Offset: 0x0006F793
		public RecipeComponent Component
		{
			get
			{
				return this.m_component;
			}
		}

		// Token: 0x140000E2 RID: 226
		// (add) Token: 0x06004975 RID: 18805 RVA: 0x001AFB6C File Offset: 0x001ADD6C
		// (remove) Token: 0x06004976 RID: 18806 RVA: 0x001AFBA4 File Offset: 0x001ADDA4
		public event Action<int> ItemPicked;

		// Token: 0x140000E3 RID: 227
		// (add) Token: 0x06004977 RID: 18807 RVA: 0x001AFBDC File Offset: 0x001ADDDC
		// (remove) Token: 0x06004978 RID: 18808 RVA: 0x001AFC14 File Offset: 0x001ADE14
		public event Action DropdownBackingClicked;

		// Token: 0x17001042 RID: 4162
		// (get) Token: 0x06004979 RID: 18809 RVA: 0x0007159B File Offset: 0x0006F79B
		private bool m_shouldShowDeselectOption
		{
			get
			{
				return this.m_component.RequirementType != ComponentRequirementType.Required && this.m_parent.SelectedTypeCode != 0;
			}
		}

		// Token: 0x0600497A RID: 18810 RVA: 0x001AFC4C File Offset: 0x001ADE4C
		public void UpdateItems(ComponentsListItem parent, RecipeComponent component, ICollection<ArchetypeInstance> instances)
		{
			if (!base.gameObject.activeInHierarchy)
			{
				return;
			}
			this.m_parent = parent;
			this.m_component = component;
			if (this.m_items == null)
			{
				this.m_items = new Dictionary<int, List<ArchetypeInstance>>();
			}
			foreach (List<ArchetypeInstance> item in this.m_items.Values)
			{
				StaticListPool<ArchetypeInstance>.ReturnToPool(item);
			}
			this.m_items.Clear();
			foreach (ArchetypeInstance archetypeInstance in instances)
			{
				int combinedTypeCode = archetypeInstance.CombinedTypeCode;
				if (!this.m_items.ContainsKey(combinedTypeCode))
				{
					this.m_items.Add(combinedTypeCode, StaticListPool<ArchetypeInstance>.GetFromPool());
				}
				this.m_items[combinedTypeCode].Add(archetypeInstance);
			}
			int num = this.m_shouldShowDeselectOption ? (this.m_items.Count + 1) : this.m_items.Count;
			this.m_noItemsLabel.gameObject.SetActive(num == 0);
			float num2 = Mathf.Max(this.m_itemRect.rect.height, this.m_itemRect.rect.height * (float)num);
			num2 = Mathf.Min(this.m_itemRect.rect.height * 3f, num2);
			num2 += (float)(2 * Math.Max(num - 1, 0));
			num2 += 5f;
			this.m_container.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, num2);
			this.ResetItems(num, false, false);
		}

		// Token: 0x0600497B RID: 18811 RVA: 0x000715BA File Offset: 0x0006F7BA
		public void PickItem(int typeCode)
		{
			Action<int> itemPicked = this.ItemPicked;
			if (itemPicked == null)
			{
				return;
			}
			itemPicked(typeCode);
		}

		// Token: 0x0600497C RID: 18812 RVA: 0x000715CD File Offset: 0x0006F7CD
		protected override ItemItemViewsHolder CreateViewsHolder(int itemIndex)
		{
			ItemItemViewsHolder itemItemViewsHolder = new ItemItemViewsHolder();
			itemItemViewsHolder.Init(this._Params.ItemPrefab, this._Params.Content, itemIndex, true, true);
			return itemItemViewsHolder;
		}

		// Token: 0x0600497D RID: 18813 RVA: 0x001AFE08 File Offset: 0x001AE008
		protected override void UpdateViewsHolder(ItemItemViewsHolder newOrRecycled)
		{
			List<ArchetypeInstance> instances = null;
			SpecialItemListItemType? special = null;
			int num = 0;
			if (this.m_component.RequirementType != ComponentRequirementType.Required && this.m_parent.SelectedTypeCode != 0)
			{
				if (newOrRecycled.ItemIndex == 0)
				{
					special = new SpecialItemListItemType?(SpecialItemListItemType.Deselect);
				}
				num--;
			}
			if (special == null)
			{
				int num2 = 0;
				foreach (List<ArchetypeInstance> list in this.m_items.Values)
				{
					if (num2 == newOrRecycled.ItemIndex + num)
					{
						instances = list;
						break;
					}
					num2++;
				}
			}
			newOrRecycled.UpdateItem(this, instances, special);
		}

		// Token: 0x0600497E RID: 18814 RVA: 0x000715F3 File Offset: 0x0006F7F3
		public void OnPointerDown(PointerEventData eventData)
		{
			Action dropdownBackingClicked = this.DropdownBackingClicked;
			if (dropdownBackingClicked == null)
			{
				return;
			}
			dropdownBackingClicked();
		}

		// Token: 0x04004485 RID: 17541
		[SerializeField]
		private Image m_container;

		// Token: 0x04004486 RID: 17542
		[SerializeField]
		private RectTransform m_itemRect;

		// Token: 0x04004487 RID: 17543
		[SerializeField]
		private TextMeshProUGUI m_noItemsLabel;

		// Token: 0x04004488 RID: 17544
		private ComponentsListItem m_parent;

		// Token: 0x04004489 RID: 17545
		private RecipeComponent m_component;

		// Token: 0x0400448A RID: 17546
		private Dictionary<int, List<ArchetypeInstance>> m_items;
	}
}

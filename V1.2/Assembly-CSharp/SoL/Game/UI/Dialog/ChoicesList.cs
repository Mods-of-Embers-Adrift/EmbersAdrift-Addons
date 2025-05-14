using System;
using System.Collections.Generic;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomParams;
using Cysharp.Text;
using Ink.Runtime;
using TMPro;
using UnityEngine;

namespace SoL.Game.UI.Dialog
{
	// Token: 0x02000984 RID: 2436
	public class ChoicesList : OSA<BaseParamsWithPrefab, ChoiceItemViewsHolder>
	{
		// Token: 0x17001025 RID: 4133
		// (get) Token: 0x060048A3 RID: 18595 RVA: 0x00070D2D File Offset: 0x0006EF2D
		public float CombinedItemHeights
		{
			get
			{
				return this.m_combinedHeight;
			}
		}

		// Token: 0x140000DD RID: 221
		// (add) Token: 0x060048A4 RID: 18596 RVA: 0x001AAC04 File Offset: 0x001A8E04
		// (remove) Token: 0x060048A5 RID: 18597 RVA: 0x001AAC3C File Offset: 0x001A8E3C
		public event Action<int> Chosen;

		// Token: 0x060048A6 RID: 18598 RVA: 0x001AAC74 File Offset: 0x001A8E74
		public void UpdateItems(ICollection<Choice> items)
		{
			if (!base.gameObject.activeInHierarchy)
			{
				return;
			}
			if (this.m_items == null)
			{
				this.m_items = new List<Choice>(items.Count);
			}
			if (this.m_heights == null)
			{
				this.m_heights = new List<float>(items.Count);
			}
			this.m_items.Clear();
			this.m_heights.Clear();
			this.m_items.AddRange(items);
			for (int i = 0; i < this.m_items.Count; i++)
			{
				this.m_heights.Add(0f);
			}
			this.RecaulculateHeights();
			this.ResetItems(this.m_items.Count, false, false);
		}

		// Token: 0x060048A7 RID: 18599 RVA: 0x00070D35 File Offset: 0x0006EF35
		public void Choose(Choice choice)
		{
			Action<int> chosen = this.Chosen;
			if (chosen == null)
			{
				return;
			}
			chosen(choice.index);
		}

		// Token: 0x060048A8 RID: 18600 RVA: 0x00070D4D File Offset: 0x0006EF4D
		protected override ChoiceItemViewsHolder CreateViewsHolder(int itemIndex)
		{
			ChoiceItemViewsHolder choiceItemViewsHolder = new ChoiceItemViewsHolder();
			choiceItemViewsHolder.Init(this._Params.ItemPrefab, this._Params.Content, itemIndex, true, true);
			return choiceItemViewsHolder;
		}

		// Token: 0x060048A9 RID: 18601 RVA: 0x00070D73 File Offset: 0x0006EF73
		protected override void UpdateViewsHolder(ChoiceItemViewsHolder newOrRecycled)
		{
			newOrRecycled.UpdateItem(this, this.m_items[newOrRecycled.ItemIndex]);
		}

		// Token: 0x060048AA RID: 18602 RVA: 0x001AAD24 File Offset: 0x001A8F24
		protected override void CollectItemsSizes(ItemCountChangeMode changeMode, int count, int indexIfInsertingOrRemoving, ItemsDescriptor itemsDesc)
		{
			base.CollectItemsSizes(changeMode, count, indexIfInsertingOrRemoving, itemsDesc);
			if (changeMode != ItemCountChangeMode.RESET)
			{
				return;
			}
			if (count == 0)
			{
				return;
			}
			int num = 0;
			int num2 = num + count;
			itemsDesc.BeginChangingItemsSizes(num);
			for (int i = num; i < num2; i++)
			{
				itemsDesc[i] = (double)this.m_heights[i];
			}
			itemsDesc.EndChangingItemsSizes();
		}

		// Token: 0x060048AB RID: 18603 RVA: 0x001AAD7C File Offset: 0x001A8F7C
		private void RecaulculateHeights()
		{
			this.m_combinedHeight = 0f;
			if (this.m_items != null)
			{
				for (int i = 0; i < this.m_items.Count; i++)
				{
					this.m_heights[i] = this.GetPreferredHeightForContent(this.m_items[i].text);
					this.m_combinedHeight += this.m_heights[i];
				}
			}
		}

		// Token: 0x060048AC RID: 18604 RVA: 0x001AADF0 File Offset: 0x001A8FF0
		private float GetPreferredHeightForContent(string content)
		{
			TextMeshProUGUI samplerText = this.m_samplerText;
			if ((int)samplerText.fontSize != Options.GameOptions.DialogueFontSize.Value + 2)
			{
				samplerText.fontSize = (float)(Options.GameOptions.DialogueFontSize.Value + 2);
			}
			samplerText.ZStringSetText(content);
			samplerText.ForceMeshUpdate(true, false);
			Vector4 margin = samplerText.margin;
			RectOffset contentPadding = base.Parameters.ContentPadding;
			float width = samplerText.rectTransform.rect.width - margin.x - margin.z - (float)contentPadding.left - (float)contentPadding.right;
			return samplerText.GetPreferredValues(width, 24f).y;
		}

		// Token: 0x040043D7 RID: 17367
		[SerializeField]
		private TextMeshProUGUI m_samplerText;

		// Token: 0x040043D8 RID: 17368
		private List<Choice> m_items;

		// Token: 0x040043D9 RID: 17369
		private List<float> m_heights;

		// Token: 0x040043DA RID: 17370
		private float m_combinedHeight;
	}
}

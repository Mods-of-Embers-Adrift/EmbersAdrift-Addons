using System;
using SoL.Game.Objects.Archetypes;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SoL.Game.UI.Recipes
{
	// Token: 0x02000936 RID: 2358
	public class RecipeListItemUI : Selectable
	{
		// Token: 0x140000CC RID: 204
		// (add) Token: 0x06004587 RID: 17799 RVA: 0x001A0284 File Offset: 0x0019E484
		// (remove) Token: 0x06004588 RID: 17800 RVA: 0x001A02BC File Offset: 0x0019E4BC
		public event Action<int, Recipe, bool> ValueChanged;

		// Token: 0x17000F92 RID: 3986
		// (get) Token: 0x06004589 RID: 17801 RVA: 0x0006ED13 File Offset: 0x0006CF13
		public Image Icon
		{
			get
			{
				return this.m_icon;
			}
		}

		// Token: 0x17000F93 RID: 3987
		// (get) Token: 0x0600458A RID: 17802 RVA: 0x0006ED1B File Offset: 0x0006CF1B
		public TextMeshProUGUI Text
		{
			get
			{
				return this.m_text;
			}
		}

		// Token: 0x17000F94 RID: 3988
		// (get) Token: 0x0600458B RID: 17803 RVA: 0x0006ED23 File Offset: 0x0006CF23
		// (set) Token: 0x0600458C RID: 17804 RVA: 0x001A02F4 File Offset: 0x0019E4F4
		public bool Selected
		{
			get
			{
				return this.m_isSelected;
			}
			set
			{
				this.m_isSelected = value;
				Action<int, Recipe, bool> valueChanged = this.ValueChanged;
				if (valueChanged != null)
				{
					valueChanged(this.Index, this.Recipe, value);
				}
				if (value)
				{
					this.m_highlight.color = this.m_selectedColor;
					return;
				}
				if (this.m_isHovered)
				{
					this.m_highlight.color = this.m_highlightColor;
					return;
				}
				this.m_highlight.color = this.m_noColor;
			}
		}

		// Token: 0x17000F95 RID: 3989
		// (get) Token: 0x0600458D RID: 17805 RVA: 0x0006ED2B File Offset: 0x0006CF2B
		// (set) Token: 0x0600458E RID: 17806 RVA: 0x0006ED33 File Offset: 0x0006CF33
		public bool Hovered
		{
			get
			{
				return this.m_isHovered;
			}
			private set
			{
				this.m_isHovered = value;
				if (!this.m_isSelected)
				{
					this.m_highlight.color = (value ? this.m_highlightColor : this.m_noColor);
				}
			}
		}

		// Token: 0x0600458F RID: 17807 RVA: 0x0006ED60 File Offset: 0x0006CF60
		public override void OnPointerDown(PointerEventData eventData)
		{
			this.Selected = !this.Selected;
		}

		// Token: 0x06004590 RID: 17808 RVA: 0x0006ED71 File Offset: 0x0006CF71
		public override void OnPointerEnter(PointerEventData eventData)
		{
			this.Hovered = true;
		}

		// Token: 0x06004591 RID: 17809 RVA: 0x0006ED7A File Offset: 0x0006CF7A
		public override void OnPointerExit(PointerEventData eventData)
		{
			this.Hovered = false;
		}

		// Token: 0x040041E3 RID: 16867
		[SerializeField]
		private Image m_icon;

		// Token: 0x040041E4 RID: 16868
		[SerializeField]
		private TextMeshProUGUI m_text;

		// Token: 0x040041E5 RID: 16869
		[SerializeField]
		private Image m_highlight;

		// Token: 0x040041E6 RID: 16870
		[NonSerialized]
		public Recipe Recipe;

		// Token: 0x040041E7 RID: 16871
		[NonSerialized]
		public int Index;

		// Token: 0x040041E8 RID: 16872
		private bool m_isSelected;

		// Token: 0x040041E9 RID: 16873
		private bool m_isHovered;

		// Token: 0x040041EA RID: 16874
		private Color m_selectedColor = new Color(1f, 0.8566537f, 0.6650944f, 0.4f);

		// Token: 0x040041EB RID: 16875
		private Color m_highlightColor = new Color(1f, 0.8566537f, 0.6650944f, 0.2f);

		// Token: 0x040041EC RID: 16876
		private Color m_noColor = new Color(1f, 0.8566537f, 0.6650944f, 0f);
	}
}

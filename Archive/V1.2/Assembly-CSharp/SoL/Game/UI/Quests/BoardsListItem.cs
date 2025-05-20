using System;
using Cysharp.Text;
using SoL.Game.Quests;
using SoL.Managers;
using SoL.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SoL.Game.UI.Quests
{
	// Token: 0x0200093C RID: 2364
	public class BoardsListItem : Selectable
	{
		// Token: 0x17000F9A RID: 3994
		// (get) Token: 0x060045C4 RID: 17860 RVA: 0x0006EF1D File Offset: 0x0006D11D
		public bool IsSelected
		{
			get
			{
				return this.m_itemIndex == this.m_parent.SelectedIndex;
			}
		}

		// Token: 0x17000F9B RID: 3995
		// (get) Token: 0x060045C5 RID: 17861 RVA: 0x0006EF32 File Offset: 0x0006D132
		// (set) Token: 0x060045C6 RID: 17862 RVA: 0x0006EF3A File Offset: 0x0006D13A
		public bool Hovered
		{
			get
			{
				return this.m_isHovered;
			}
			private set
			{
				this.m_isHovered = value;
				this.RefreshHighlightVisuals();
			}
		}

		// Token: 0x060045C7 RID: 17863 RVA: 0x0006EF49 File Offset: 0x0006D149
		public void Init(BoardsList parent, BulletinBoard item, int index)
		{
			this.m_parent = parent;
			this.m_item = item;
			this.m_itemIndex = index;
			this.RefreshVisuals();
		}

		// Token: 0x060045C8 RID: 17864 RVA: 0x0006EF66 File Offset: 0x0006D166
		public void RefreshVisuals()
		{
			this.m_label.SetTextFormat("{0} [{1}]", this.m_item.Title, LocalZoneManager.GetFormattedZoneName(this.m_item.ZoneId, this.m_item.SubZoneId));
			this.RefreshHighlightVisuals();
		}

		// Token: 0x060045C9 RID: 17865 RVA: 0x001A11B4 File Offset: 0x0019F3B4
		private void RefreshHighlightVisuals()
		{
			if (this.IsSelected)
			{
				this.m_highlight.gameObject.SetActive(true);
				this.m_highlight.color = this.m_selectedColor;
				return;
			}
			if (this.m_isHovered)
			{
				this.m_highlight.gameObject.SetActive(true);
				this.m_highlight.color = this.m_highlightColor;
				return;
			}
			this.m_highlight.gameObject.SetActive(false);
		}

		// Token: 0x060045CA RID: 17866 RVA: 0x0006EFA4 File Offset: 0x0006D1A4
		public void Reindex(int index)
		{
			this.m_itemIndex = index;
			this.RefreshVisuals();
		}

		// Token: 0x060045CB RID: 17867 RVA: 0x0006EFB3 File Offset: 0x0006D1B3
		public override void OnPointerDown(PointerEventData eventData)
		{
			if (!this.IsSelected)
			{
				this.m_parent.Select(this.m_itemIndex);
			}
		}

		// Token: 0x060045CC RID: 17868 RVA: 0x0006EFCE File Offset: 0x0006D1CE
		public override void OnPointerEnter(PointerEventData eventData)
		{
			this.Hovered = true;
		}

		// Token: 0x060045CD RID: 17869 RVA: 0x0006EFD7 File Offset: 0x0006D1D7
		public override void OnPointerExit(PointerEventData eventData)
		{
			this.Hovered = false;
		}

		// Token: 0x04004207 RID: 16903
		private readonly Color kDefaultTextColor = new Color(0.7921569f, 0.772549f, 0.7686275f, 1f);

		// Token: 0x04004208 RID: 16904
		private readonly Color kInactiveTextColor = new Color(0.7921569f, 0.772549f, 0.7686275f, 0.3f);

		// Token: 0x04004209 RID: 16905
		[SerializeField]
		private Image m_highlight;

		// Token: 0x0400420A RID: 16906
		[SerializeField]
		private TextMeshProUGUI m_label;

		// Token: 0x0400420B RID: 16907
		private BoardsList m_parent;

		// Token: 0x0400420C RID: 16908
		private BulletinBoard m_item;

		// Token: 0x0400420D RID: 16909
		private int m_itemIndex;

		// Token: 0x0400420E RID: 16910
		private Color m_selectedColor = Colors.CadetGrey;

		// Token: 0x0400420F RID: 16911
		private Color m_highlightColor = Colors.GoldMetallic;

		// Token: 0x04004210 RID: 16912
		private bool m_isHovered;
	}
}

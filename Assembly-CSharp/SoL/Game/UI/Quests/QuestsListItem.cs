using System;
using SoL.Game.Quests;
using SoL.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SoL.Game.UI.Quests
{
	// Token: 0x0200094C RID: 2380
	public class QuestsListItem : Selectable
	{
		// Token: 0x17000FB0 RID: 4016
		// (get) Token: 0x0600465B RID: 18011 RVA: 0x0006F56A File Offset: 0x0006D76A
		public bool IsSelected
		{
			get
			{
				return this.m_itemIndex == this.m_parent.SelectedIndex;
			}
		}

		// Token: 0x17000FB1 RID: 4017
		// (get) Token: 0x0600465C RID: 18012 RVA: 0x0006F57F File Offset: 0x0006D77F
		// (set) Token: 0x0600465D RID: 18013 RVA: 0x0006F587 File Offset: 0x0006D787
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

		// Token: 0x0600465E RID: 18014 RVA: 0x0006F596 File Offset: 0x0006D796
		public void Init(QuestsList parent, Quest item, int index)
		{
			this.m_parent = parent;
			this.m_quest = item;
			this.m_itemIndex = index;
			this.RefreshVisuals();
		}

		// Token: 0x0600465F RID: 18015 RVA: 0x0006F5B3 File Offset: 0x0006D7B3
		public void RefreshVisuals()
		{
			this.m_label.text = this.m_quest.Title;
			this.RefreshHighlightVisuals();
		}

		// Token: 0x06004660 RID: 18016 RVA: 0x001A3758 File Offset: 0x001A1958
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

		// Token: 0x06004661 RID: 18017 RVA: 0x0006F5D1 File Offset: 0x0006D7D1
		public void Reindex(int index)
		{
			this.m_itemIndex = index;
			this.RefreshVisuals();
		}

		// Token: 0x06004662 RID: 18018 RVA: 0x0006F5E0 File Offset: 0x0006D7E0
		public override void OnPointerDown(PointerEventData eventData)
		{
			if (!this.IsSelected)
			{
				this.m_parent.Select(this.m_itemIndex);
			}
		}

		// Token: 0x06004663 RID: 18019 RVA: 0x0006F5FB File Offset: 0x0006D7FB
		public override void OnPointerEnter(PointerEventData eventData)
		{
			this.Hovered = true;
		}

		// Token: 0x06004664 RID: 18020 RVA: 0x0006F604 File Offset: 0x0006D804
		public override void OnPointerExit(PointerEventData eventData)
		{
			this.Hovered = false;
		}

		// Token: 0x04004269 RID: 17001
		private readonly Color kDefaultTextColor = new Color(0.7921569f, 0.772549f, 0.7686275f, 1f);

		// Token: 0x0400426A RID: 17002
		private readonly Color kInactiveTextColor = new Color(0.7921569f, 0.772549f, 0.7686275f, 0.3f);

		// Token: 0x0400426B RID: 17003
		[SerializeField]
		private Image m_highlight;

		// Token: 0x0400426C RID: 17004
		[SerializeField]
		private TextMeshProUGUI m_label;

		// Token: 0x0400426D RID: 17005
		private QuestsList m_parent;

		// Token: 0x0400426E RID: 17006
		private Quest m_quest;

		// Token: 0x0400426F RID: 17007
		private int m_itemIndex;

		// Token: 0x04004270 RID: 17008
		private Color m_selectedColor = Colors.CadetGrey;

		// Token: 0x04004271 RID: 17009
		private Color m_highlightColor = Colors.GoldMetallic;

		// Token: 0x04004272 RID: 17010
		private bool m_isHovered;
	}
}

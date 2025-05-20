using System;
using SoL.Game.Notifications;
using SoL.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SoL.Game.UI.Quests
{
	// Token: 0x02000957 RID: 2391
	public class TutorialsListItem : Selectable
	{
		// Token: 0x17000FC1 RID: 4033
		// (get) Token: 0x060046D9 RID: 18137 RVA: 0x0006FB56 File Offset: 0x0006DD56
		public bool IsSelected
		{
			get
			{
				return this.m_itemIndex == this.m_parent.SelectedIndex;
			}
		}

		// Token: 0x17000FC2 RID: 4034
		// (get) Token: 0x060046DA RID: 18138 RVA: 0x0006FB6B File Offset: 0x0006DD6B
		// (set) Token: 0x060046DB RID: 18139 RVA: 0x0006FB73 File Offset: 0x0006DD73
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

		// Token: 0x060046DC RID: 18140 RVA: 0x0006FB82 File Offset: 0x0006DD82
		public void Init(TutorialsList parent, BaseNotification item, int index)
		{
			this.m_parent = parent;
			this.m_tutorial = item;
			this.m_itemIndex = index;
			this.RefreshVisuals();
		}

		// Token: 0x060046DD RID: 18141 RVA: 0x0006FB9F File Offset: 0x0006DD9F
		public void RefreshVisuals()
		{
			this.m_label.text = this.m_tutorial.Title;
			this.RefreshHighlightVisuals();
		}

		// Token: 0x060046DE RID: 18142 RVA: 0x001A5414 File Offset: 0x001A3614
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

		// Token: 0x060046DF RID: 18143 RVA: 0x0006FBBD File Offset: 0x0006DDBD
		public void Reindex(int index)
		{
			this.m_itemIndex = index;
			this.RefreshVisuals();
		}

		// Token: 0x060046E0 RID: 18144 RVA: 0x0006FBCC File Offset: 0x0006DDCC
		public override void OnPointerDown(PointerEventData eventData)
		{
			if (!this.IsSelected)
			{
				this.m_parent.OnSelect(this.m_itemIndex);
			}
		}

		// Token: 0x060046E1 RID: 18145 RVA: 0x0006FBE7 File Offset: 0x0006DDE7
		public override void OnPointerEnter(PointerEventData eventData)
		{
			this.Hovered = true;
		}

		// Token: 0x060046E2 RID: 18146 RVA: 0x0006FBF0 File Offset: 0x0006DDF0
		public override void OnPointerExit(PointerEventData eventData)
		{
			this.Hovered = false;
		}

		// Token: 0x040042C0 RID: 17088
		private readonly Color kDefaultTextColor = new Color(0.7921569f, 0.772549f, 0.7686275f, 1f);

		// Token: 0x040042C1 RID: 17089
		private readonly Color kInactiveTextColor = new Color(0.7921569f, 0.772549f, 0.7686275f, 0.3f);

		// Token: 0x040042C2 RID: 17090
		[SerializeField]
		private Image m_highlight;

		// Token: 0x040042C3 RID: 17091
		[SerializeField]
		private TextMeshProUGUI m_label;

		// Token: 0x040042C4 RID: 17092
		private TutorialsList m_parent;

		// Token: 0x040042C5 RID: 17093
		private BaseNotification m_tutorial;

		// Token: 0x040042C6 RID: 17094
		private int m_itemIndex;

		// Token: 0x040042C7 RID: 17095
		private Color m_selectedColor = Colors.CadetGrey;

		// Token: 0x040042C8 RID: 17096
		private Color m_highlightColor = Colors.GoldMetallic;

		// Token: 0x040042C9 RID: 17097
		private bool m_isHovered;
	}
}

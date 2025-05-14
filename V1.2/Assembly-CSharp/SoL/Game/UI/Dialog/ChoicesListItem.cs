using System;
using Ink.Runtime;
using SoL.Game.Interactives;
using SoL.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SoL.Game.UI.Dialog
{
	// Token: 0x02000986 RID: 2438
	public class ChoicesListItem : Selectable, ICursor, IInteractiveBase
	{
		// Token: 0x17001028 RID: 4136
		// (get) Token: 0x060048B6 RID: 18614 RVA: 0x00070E0D File Offset: 0x0006F00D
		// (set) Token: 0x060048B7 RID: 18615 RVA: 0x00070E15 File Offset: 0x0006F015
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

		// Token: 0x060048B8 RID: 18616 RVA: 0x00070E24 File Offset: 0x0006F024
		public void Init(ChoicesList parent, Choice choice, int index)
		{
			this.m_parent = parent;
			this.m_choice = choice;
			this.m_itemIndex = index;
			this.RefreshVisuals();
		}

		// Token: 0x060048B9 RID: 18617 RVA: 0x001AAE94 File Offset: 0x001A9094
		public void RefreshVisuals()
		{
			this.RefreshHighlightVisuals();
			if (this.m_choice.text.Contains("#MERCHANT"))
			{
				this.m_icon.gameObject.SetActive(true);
				this.m_icon.sprite = this.m_merchantIcon;
				this.m_icon.color = ChoicesListItem.m_iconColor;
			}
			else
			{
				this.m_icon.gameObject.SetActive(false);
			}
			this.m_label.text = this.m_choice.text;
		}

		// Token: 0x060048BA RID: 18618 RVA: 0x001AAF1C File Offset: 0x001A911C
		public void RefreshHighlightVisuals()
		{
			if (this.m_isHovered)
			{
				this.m_label.color = InkDriver.m_selectedChoiceColor;
				this.m_label.fontStyle |= FontStyles.Underline;
				return;
			}
			this.m_label.color = InkDriver.m_choiceColor;
			this.m_label.fontStyle &= ~FontStyles.Underline;
		}

		// Token: 0x060048BB RID: 18619 RVA: 0x00070E41 File Offset: 0x0006F041
		public override void OnPointerDown(PointerEventData eventData)
		{
			this.m_parent.Choose(this.m_choice);
		}

		// Token: 0x060048BC RID: 18620 RVA: 0x00070E54 File Offset: 0x0006F054
		public override void OnPointerEnter(PointerEventData eventData)
		{
			this.Hovered = true;
		}

		// Token: 0x060048BD RID: 18621 RVA: 0x00070E5D File Offset: 0x0006F05D
		public override void OnPointerExit(PointerEventData eventData)
		{
			this.Hovered = false;
		}

		// Token: 0x17001029 RID: 4137
		// (get) Token: 0x060048BE RID: 18622 RVA: 0x00070E66 File Offset: 0x0006F066
		public CursorType Type
		{
			get
			{
				return CursorType.GloveCursor;
			}
		}

		// Token: 0x1700102A RID: 4138
		// (get) Token: 0x060048BF RID: 18623 RVA: 0x00049FFA File Offset: 0x000481FA
		public InteractionSettings Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x060048C2 RID: 18626 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x040043DE RID: 17374
		[SerializeField]
		private Image m_icon;

		// Token: 0x040043DF RID: 17375
		[SerializeField]
		private TextMeshProUGUI m_label;

		// Token: 0x040043E0 RID: 17376
		[SerializeField]
		private Sprite m_merchantIcon;

		// Token: 0x040043E1 RID: 17377
		private static Color m_iconColor = new Color(149f, 107f, 91f, 255f);

		// Token: 0x040043E2 RID: 17378
		private ChoicesList m_parent;

		// Token: 0x040043E3 RID: 17379
		private Choice m_choice;

		// Token: 0x040043E4 RID: 17380
		private int m_itemIndex = -1;

		// Token: 0x040043E5 RID: 17381
		private bool m_isHovered;
	}
}

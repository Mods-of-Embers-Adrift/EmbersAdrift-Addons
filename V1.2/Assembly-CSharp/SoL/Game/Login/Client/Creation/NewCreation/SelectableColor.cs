using System;
using SoL.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SoL.Game.Login.Client.Creation.NewCreation
{
	// Token: 0x02000B73 RID: 2931
	public class SelectableColor : MonoBehaviour
	{
		// Token: 0x170014FE RID: 5374
		// (get) Token: 0x06005A39 RID: 23097 RVA: 0x0007C910 File Offset: 0x0007AB10
		public Color SwatchColor
		{
			get
			{
				return this.m_swatchColor;
			}
		}

		// Token: 0x06005A3A RID: 23098 RVA: 0x0007C918 File Offset: 0x0007AB18
		private void Awake()
		{
			this.m_button.onClick.AddListener(new UnityAction(this.ButtonClicked));
		}

		// Token: 0x06005A3B RID: 23099 RVA: 0x0007C936 File Offset: 0x0007AB36
		private void OnDestroy()
		{
			this.m_button.onClick.RemoveListener(new UnityAction(this.ButtonClicked));
		}

		// Token: 0x06005A3C RID: 23100 RVA: 0x0007C954 File Offset: 0x0007AB54
		private void ButtonClicked()
		{
			this.m_controller.ColorSelected(this);
		}

		// Token: 0x06005A3D RID: 23101 RVA: 0x001EC6E0 File Offset: 0x001EA8E0
		public void Init(ColorPalette palette, Color color)
		{
			this.m_controller = palette;
			this.m_swatchColor = color;
			this.m_swatch.color = color;
			this.m_button.interactable = true;
			this.m_frame.color = this.m_notSelectedColor;
			this.m_highlight.enabled = false;
			this.m_highlight.color = this.m_selectedColor;
		}

		// Token: 0x06005A3E RID: 23102 RVA: 0x0007C962 File Offset: 0x0007AB62
		public void SetIsSelected(bool isSelected)
		{
			this.m_button.interactable = !isSelected;
			this.m_frame.color = (isSelected ? this.m_selectedColor : this.m_notSelectedColor);
			this.m_highlight.enabled = isSelected;
		}

		// Token: 0x04004F55 RID: 20309
		[SerializeField]
		private SolButton m_button;

		// Token: 0x04004F56 RID: 20310
		[SerializeField]
		private Image m_highlight;

		// Token: 0x04004F57 RID: 20311
		[SerializeField]
		private Image m_swatch;

		// Token: 0x04004F58 RID: 20312
		[SerializeField]
		private Image m_frame;

		// Token: 0x04004F59 RID: 20313
		[SerializeField]
		private Color m_notSelectedColor;

		// Token: 0x04004F5A RID: 20314
		[SerializeField]
		private Color m_selectedColor;

		// Token: 0x04004F5B RID: 20315
		private ColorPalette m_controller;

		// Token: 0x04004F5C RID: 20316
		private Color m_swatchColor;
	}
}

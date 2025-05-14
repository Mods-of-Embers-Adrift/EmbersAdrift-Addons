using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SoL.Game.Login.Client.Creation
{
	// Token: 0x02000B56 RID: 2902
	public class ColorButton : MonoBehaviour
	{
		// Token: 0x170014D9 RID: 5337
		// (get) Token: 0x06005952 RID: 22866 RVA: 0x0007BCEF File Offset: 0x00079EEF
		public Color Color
		{
			get
			{
				return this.m_color;
			}
		}

		// Token: 0x06005953 RID: 22867 RVA: 0x001E9A4C File Offset: 0x001E7C4C
		public void Initialize(int index, Color c, ColorSelector selector)
		{
			this.m_index = index;
			this.m_color = c;
			this.m_selector = selector;
			this.m_colorImage.color = this.m_color;
			this.m_button.onClick.AddListener(new UnityAction(this.OnButtonClicked));
			this.m_selector.ColorSelectedEvent += this.ColorSelectedEvent;
			this.m_selector.LockStateChangedEvent += this.LockStateChangedEvent;
		}

		// Token: 0x06005954 RID: 22868 RVA: 0x0007BCF7 File Offset: 0x00079EF7
		private void OnButtonClicked()
		{
			this.m_selector.ColorSelected(this.m_index);
		}

		// Token: 0x06005955 RID: 22869 RVA: 0x0007BD0A File Offset: 0x00079F0A
		private void LockStateChangedEvent(bool b)
		{
			this.m_button.interactable = (!b && this.m_index != this.m_selector.SelectedIndex);
		}

		// Token: 0x06005956 RID: 22870 RVA: 0x001E9ACC File Offset: 0x001E7CCC
		private void OnDestroy()
		{
			if (this.m_selector != null)
			{
				this.m_selector.ColorSelectedEvent -= this.ColorSelectedEvent;
				this.m_selector.LockStateChangedEvent -= this.LockStateChangedEvent;
			}
			this.m_button.onClick.RemoveListener(new UnityAction(this.OnButtonClicked));
		}

		// Token: 0x06005957 RID: 22871 RVA: 0x001E9B34 File Offset: 0x001E7D34
		private void ColorSelectedEvent(int i)
		{
			this.m_button.interactable = (i != this.m_index);
			if (this.m_outline != null)
			{
				this.m_outline.enabled = (i == this.m_index);
			}
			if (this.m_outlineImage != null)
			{
				this.m_outlineImage.enabled = (i == this.m_index);
			}
		}

		// Token: 0x04004E9F RID: 20127
		[SerializeField]
		private Button m_button;

		// Token: 0x04004EA0 RID: 20128
		[SerializeField]
		private Image m_colorImage;

		// Token: 0x04004EA1 RID: 20129
		[SerializeField]
		private Outline m_outline;

		// Token: 0x04004EA2 RID: 20130
		[SerializeField]
		private Image m_outlineImage;

		// Token: 0x04004EA3 RID: 20131
		private int m_index;

		// Token: 0x04004EA4 RID: 20132
		private Color m_color;

		// Token: 0x04004EA5 RID: 20133
		private ColorSelector m_selector;
	}
}

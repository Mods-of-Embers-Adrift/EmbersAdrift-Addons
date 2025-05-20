using System;
using SoL.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SoL.Game.UI
{
	// Token: 0x020008BF RID: 2239
	public class OptionsSlider : MonoBehaviour
	{
		// Token: 0x17000EF6 RID: 3830
		// (get) Token: 0x0600419B RID: 16795 RVA: 0x0006C4E4 File Offset: 0x0006A6E4
		public Slider Slider
		{
			get
			{
				return this.m_slider;
			}
		}

		// Token: 0x17000EF7 RID: 3831
		// (get) Token: 0x0600419C RID: 16796 RVA: 0x0006C4EC File Offset: 0x0006A6EC
		public SolButton ResetButton
		{
			get
			{
				return this.m_resetButton;
			}
		}

		// Token: 0x17000EF8 RID: 3832
		// (get) Token: 0x0600419D RID: 16797 RVA: 0x0006C4F4 File Offset: 0x0006A6F4
		public TextMeshProUGUI Value
		{
			get
			{
				return this.m_value;
			}
		}

		// Token: 0x17000EF9 RID: 3833
		// (get) Token: 0x0600419E RID: 16798 RVA: 0x0006C4FC File Offset: 0x0006A6FC
		public TextMeshProUGUI Label
		{
			get
			{
				return this.m_label;
			}
		}

		// Token: 0x0600419F RID: 16799 RVA: 0x0006C504 File Offset: 0x0006A704
		public void SetInteractable(bool isInteractive)
		{
			if (this.m_slider)
			{
				this.m_slider.interactable = isInteractive;
			}
			if (this.m_resetButton)
			{
				this.m_resetButton.interactable = isInteractive;
			}
		}

		// Token: 0x04003ED4 RID: 16084
		[SerializeField]
		private Slider m_slider;

		// Token: 0x04003ED5 RID: 16085
		[SerializeField]
		private SolButton m_resetButton;

		// Token: 0x04003ED6 RID: 16086
		[SerializeField]
		private TextMeshProUGUI m_value;

		// Token: 0x04003ED7 RID: 16087
		[SerializeField]
		private TextMeshProUGUI m_label;
	}
}

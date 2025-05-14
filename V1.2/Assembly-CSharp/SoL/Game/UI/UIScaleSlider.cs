using System;
using UnityEngine;

namespace SoL.Game.UI
{
	// Token: 0x020008C8 RID: 2248
	[Serializable]
	public class UIScaleSlider : IntSlider
	{
		// Token: 0x17000EFE RID: 3838
		// (get) Token: 0x060041BD RID: 16829 RVA: 0x0006C6BD File Offset: 0x0006A8BD
		// (set) Token: 0x060041BE RID: 16830 RVA: 0x0006C6C5 File Offset: 0x0006A8C5
		public Canvas GameUICanvas { get; set; }

		// Token: 0x060041BF RID: 16831 RVA: 0x0006C577 File Offset: 0x0006A777
		protected override bool InitInternal()
		{
			if (base.InitInternal())
			{
				this.OnSliderChanged((float)this.m_option.Value);
				return true;
			}
			return false;
		}

		// Token: 0x060041C0 RID: 16832 RVA: 0x0006C6CE File Offset: 0x0006A8CE
		protected override void OnSliderChanged(float value)
		{
			base.OnSliderChanged(value);
			if (this.GameUICanvas)
			{
				this.GameUICanvas.scaleFactor = Options.GameOptions.GameUIScalePercentage;
			}
		}
	}
}

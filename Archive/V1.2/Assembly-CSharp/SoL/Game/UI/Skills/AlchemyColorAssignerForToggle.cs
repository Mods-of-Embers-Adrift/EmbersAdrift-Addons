using System;
using SoL.Game.Settings;
using SoL.UI;
using UnityEngine;
using UnityEngine.UI;

namespace SoL.Game.UI.Skills
{
	// Token: 0x02000927 RID: 2343
	public class AlchemyColorAssignerForToggle : AlchemyUI
	{
		// Token: 0x060044F8 RID: 17656 RVA: 0x0019E3A4 File Offset: 0x0019C5A4
		private void Start()
		{
			if (this.m_toggle)
			{
				ColorBlock colors = this.m_toggle.colors;
				colors.normalColor = GlobalSettings.Values.Ashen.GetUIHighlightColor(this.m_alchemyPowerLevel);
				this.m_toggle.colors = colors;
			}
		}

		// Token: 0x04004188 RID: 16776
		[SerializeField]
		private SolToggle m_toggle;
	}
}

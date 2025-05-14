using System;
using SoL.Game.Settings;
using UnityEngine;
using UnityEngine.UI;

namespace SoL.Game.UI.Skills
{
	// Token: 0x02000926 RID: 2342
	public class AlchemyColorAssigner : AlchemyUI
	{
		// Token: 0x060044F6 RID: 17654 RVA: 0x0006E901 File Offset: 0x0006CB01
		private void Start()
		{
			if (this.m_toColor)
			{
				this.m_toColor.color = GlobalSettings.Values.Ashen.GetUIHighlightColor(this.m_alchemyPowerLevel);
			}
		}

		// Token: 0x04004187 RID: 16775
		[SerializeField]
		private Image m_toColor;
	}
}

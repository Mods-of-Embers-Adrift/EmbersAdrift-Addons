using System;
using SoL.UI;
using UnityEngine;
using UnityEngine.Events;

namespace SoL.Game.SkyDome
{
	// Token: 0x020006F6 RID: 1782
	public class ExpanseResetButton : MonoBehaviour
	{
		// Token: 0x060035B5 RID: 13749 RVA: 0x00064C81 File Offset: 0x00062E81
		private void Awake()
		{
			this.m_button.onClick.AddListener(new UnityAction(this.ButtonClicked));
		}

		// Token: 0x060035B6 RID: 13750 RVA: 0x00064C9F File Offset: 0x00062E9F
		private void OnDestroy()
		{
			this.m_button.onClick.RemoveListener(new UnityAction(this.ButtonClicked));
		}

		// Token: 0x060035B7 RID: 13751 RVA: 0x0004475B File Offset: 0x0004295B
		private void ButtonClicked()
		{
		}

		// Token: 0x040033B3 RID: 13235
		[SerializeField]
		private ExpanseResetButton.ResetButtonType m_type = ExpanseResetButton.ResetButtonType.Atmosphere;

		// Token: 0x040033B4 RID: 13236
		[SerializeField]
		private SolButton m_button;

		// Token: 0x020006F7 RID: 1783
		private enum ResetButtonType
		{
			// Token: 0x040033B6 RID: 13238
			CloudRenderer_Fullscreen,
			// Token: 0x040033B7 RID: 13239
			CloudRenderer_Shadows,
			// Token: 0x040033B8 RID: 13240
			CloudCompsitor_Fullscreen,
			// Token: 0x040033B9 RID: 13241
			CloudCompsitor_Shadows,
			// Token: 0x040033BA RID: 13242
			Fog,
			// Token: 0x040033BB RID: 13243
			Atmosphere,
			// Token: 0x040033BC RID: 13244
			SkyCompositor
		}
	}
}

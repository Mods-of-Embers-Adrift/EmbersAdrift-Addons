using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SoL.Game.SkyDome
{
	// Token: 0x020006FF RID: 1791
	[Serializable]
	public class LightControls
	{
		// Token: 0x040033E2 RID: 13282
		[SerializeField]
		private Slider m_intensity;

		// Token: 0x040033E3 RID: 13283
		[SerializeField]
		private TextMeshProUGUI m_intensityLabel;

		// Token: 0x040033E4 RID: 13284
		[SerializeField]
		private Slider m_red;

		// Token: 0x040033E5 RID: 13285
		[SerializeField]
		private TextMeshProUGUI m_redLabel;

		// Token: 0x040033E6 RID: 13286
		[SerializeField]
		private Slider m_green;

		// Token: 0x040033E7 RID: 13287
		[SerializeField]
		private TextMeshProUGUI m_greenLabel;

		// Token: 0x040033E8 RID: 13288
		[SerializeField]
		private Slider m_blue;

		// Token: 0x040033E9 RID: 13289
		[SerializeField]
		private TextMeshProUGUI m_blueLabel;
	}
}

using System;
using SoL.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SoL.Game.SkyDome
{
	// Token: 0x02000717 RID: 1815
	public class WeatherPanelUI : MonoBehaviour
	{
		// Token: 0x04003467 RID: 13415
		[SerializeField]
		private SolButton m_reset;

		// Token: 0x04003468 RID: 13416
		[SerializeField]
		private Slider m_coverage;

		// Token: 0x04003469 RID: 13417
		[SerializeField]
		private TextMeshProUGUI m_coverageLabel;

		// Token: 0x0400346A RID: 13418
		[SerializeField]
		private Slider m_wetness;

		// Token: 0x0400346B RID: 13419
		[SerializeField]
		private TextMeshProUGUI m_wetnessLabel;

		// Token: 0x0400346C RID: 13420
		[SerializeField]
		private Slider m_snow;

		// Token: 0x0400346D RID: 13421
		[SerializeField]
		private TextMeshProUGUI m_snowLabel;
	}
}

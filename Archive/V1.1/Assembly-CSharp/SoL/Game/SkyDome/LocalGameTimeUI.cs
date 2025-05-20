using System;
using SoL.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SoL.Game.SkyDome
{
	// Token: 0x02000702 RID: 1794
	public class LocalGameTimeUI : MonoBehaviour
	{
		// Token: 0x040033F0 RID: 13296
		[SerializeField]
		private UIWindow m_window;

		// Token: 0x040033F1 RID: 13297
		[SerializeField]
		private Toggle m_staticTimeToggle;

		// Token: 0x040033F2 RID: 13298
		[SerializeField]
		private SolButton m_resetButton;

		// Token: 0x040033F3 RID: 13299
		[SerializeField]
		private LocalGameTimeUI.TimeSlider[] m_timeSliders;

		// Token: 0x040033F4 RID: 13300
		[SerializeField]
		private TextMeshProUGUI m_timeLabel;

		// Token: 0x040033F5 RID: 13301
		private bool m_preventEvents;

		// Token: 0x02000703 RID: 1795
		private enum TimeSliderType
		{
			// Token: 0x040033F7 RID: 13303
			Minute,
			// Token: 0x040033F8 RID: 13304
			Hour,
			// Token: 0x040033F9 RID: 13305
			Day,
			// Token: 0x040033FA RID: 13306
			Month
		}

		// Token: 0x02000704 RID: 1796
		[Serializable]
		private class TimeSlider
		{
			// Token: 0x040033FB RID: 13307
			[SerializeField]
			private LocalGameTimeUI.TimeSliderType m_type;

			// Token: 0x040033FC RID: 13308
			[SerializeField]
			private Slider m_slider;

			// Token: 0x040033FD RID: 13309
			[SerializeField]
			private TextMeshProUGUI m_label;
		}
	}
}

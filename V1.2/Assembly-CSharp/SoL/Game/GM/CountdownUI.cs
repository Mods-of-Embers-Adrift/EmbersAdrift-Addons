using System;
using SoL.UI;
using TMPro;
using UnityEngine;

namespace SoL.Game.GM
{
	// Token: 0x02000BE6 RID: 3046
	public class CountdownUI : MonoBehaviour
	{
		// Token: 0x0400517F RID: 20863
		[SerializeField]
		private UIWindow m_window;

		// Token: 0x04005180 RID: 20864
		[SerializeField]
		private SolToggle m_enabled;

		// Token: 0x04005181 RID: 20865
		[SerializeField]
		private SolToggle m_zoneOnly;

		// Token: 0x04005182 RID: 20866
		[SerializeField]
		private TMP_InputField m_message;

		// Token: 0x04005183 RID: 20867
		[SerializeField]
		private TMP_Dropdown m_hour;

		// Token: 0x04005184 RID: 20868
		[SerializeField]
		private TMP_Dropdown m_minute;

		// Token: 0x04005185 RID: 20869
		[SerializeField]
		private TextMeshProUGUI m_label;

		// Token: 0x04005186 RID: 20870
		[SerializeField]
		private SolButton m_sendNowButton;
	}
}

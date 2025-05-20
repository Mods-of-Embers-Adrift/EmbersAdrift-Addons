using System;
using UnityEngine;
using UnityEngine.UI;

namespace SoL.Game.SkyDome
{
	// Token: 0x02000700 RID: 1792
	public class LightControlsUI : MonoBehaviour
	{
		// Token: 0x040033EA RID: 13290
		[SerializeField]
		private Toggle m_toggle;

		// Token: 0x040033EB RID: 13291
		[SerializeField]
		private LightControlsUI.LightControlType m_lightType;

		// Token: 0x040033EC RID: 13292
		[SerializeField]
		private LightControls m_controls;

		// Token: 0x02000701 RID: 1793
		private enum LightControlType
		{
			// Token: 0x040033EE RID: 13294
			Night,
			// Token: 0x040033EF RID: 13295
			Camera
		}
	}
}

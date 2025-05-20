using System;
using SoL.GameCamera;
using UnityEngine;

namespace SoL.Tests
{
	// Token: 0x02000D91 RID: 3473
	public class CameraEffectTester : MonoBehaviour
	{
		// Token: 0x06006867 RID: 26727 RVA: 0x00086180 File Offset: 0x00084380
		private void ToggleEffect()
		{
			MainCameraSettings.ToggleEffectOverride(this.m_type, this.m_isOn);
		}

		// Token: 0x06006868 RID: 26728 RVA: 0x00086193 File Offset: 0x00084393
		private void DisableEffects()
		{
			MainCameraSettings.DisableEffectOverrides();
		}

		// Token: 0x04005A99 RID: 23193
		[SerializeField]
		private bool m_isOn;

		// Token: 0x04005A9A RID: 23194
		[SerializeField]
		private CameraEffectTypes m_type;
	}
}

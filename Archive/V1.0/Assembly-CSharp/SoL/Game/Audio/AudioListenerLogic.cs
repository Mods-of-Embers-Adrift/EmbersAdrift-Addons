using System;
using SoL.Game.Settings;
using SoL.GameCamera;
using UnityEngine;

namespace SoL.Game.Audio
{
	// Token: 0x02000D05 RID: 3333
	public class AudioListenerLogic : MonoBehaviour
	{
		// Token: 0x060064B0 RID: 25776 RVA: 0x00083B90 File Offset: 0x00081D90
		private void Start()
		{
			Options.AudioOptions.AudioListenerAtPlayer.Changed += this.AudioListenerAtPlayerOnChanged;
			this.AudioListenerAtPlayerOnChanged();
		}

		// Token: 0x060064B1 RID: 25777 RVA: 0x00083BAE File Offset: 0x00081DAE
		private void OnDestroy()
		{
			Options.AudioOptions.AudioListenerAtPlayer.Changed -= this.AudioListenerAtPlayerOnChanged;
		}

		// Token: 0x060064B2 RID: 25778 RVA: 0x0020A688 File Offset: 0x00208888
		private void AudioListenerAtPlayerOnChanged()
		{
			if (this.m_audioListenerObject == null)
			{
				return;
			}
			if (GlobalSettings.kAudioListenerAlwaysAtPlayer || Options.AudioOptions.AudioListenerAtPlayer.Value)
			{
				MainCameraSettings.ToggleAudioListener(false);
				this.m_audioListenerObject.SetActive(true);
				return;
			}
			this.m_audioListenerObject.SetActive(false);
			MainCameraSettings.ToggleAudioListener(true);
		}

		// Token: 0x0400576C RID: 22380
		[SerializeField]
		private GameObject m_audioListenerObject;
	}
}

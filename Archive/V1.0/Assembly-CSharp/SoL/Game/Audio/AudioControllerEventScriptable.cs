using System;
using UnityEngine;

namespace SoL.Game.Audio
{
	// Token: 0x02000CFC RID: 3324
	public class AudioControllerEventScriptable : MonoBehaviour, IAudioController
	{
		// Token: 0x17001822 RID: 6178
		// (get) Token: 0x06006483 RID: 25731 RVA: 0x000839A4 File Offset: 0x00081BA4
		// (set) Token: 0x06006484 RID: 25732 RVA: 0x000839AC File Offset: 0x00081BAC
		public AudioEvent[] AudioEvents { get; private set; }

		// Token: 0x06006485 RID: 25733 RVA: 0x00209FD4 File Offset: 0x002081D4
		private void Awake()
		{
			if (this.m_profile == null)
			{
				base.enabled = false;
				return;
			}
			this.AudioEvents = new AudioEvent[this.m_profile.AudioEvents.Length];
			for (int i = 0; i < this.m_profile.AudioEvents.Length; i++)
			{
				this.AudioEvents[i] = new AudioEvent(this.m_profile.AudioEvents[i], this.m_audioSource);
			}
		}

		// Token: 0x04005746 RID: 22342
		[SerializeField]
		private AudioSource m_audioSource;

		// Token: 0x04005747 RID: 22343
		[SerializeField]
		private AudioEventsProfile m_profile;
	}
}

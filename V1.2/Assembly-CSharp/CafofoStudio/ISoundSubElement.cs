using System;
using UnityEngine;
using UnityEngine.Audio;

namespace CafofoStudio
{
	// Token: 0x02000052 RID: 82
	public interface ISoundSubElement
	{
		// Token: 0x060001A3 RID: 419
		void InitializeAudioSources(GameObject parent, AudioMixerGroup outputMixer);

		// Token: 0x060001A4 RID: 420
		void CalculateIntensity(float intensity, float volumeMultiplier);

		// Token: 0x060001A5 RID: 421
		void SetOutputMixerGroup(AudioMixerGroup overrideOutputMixer);

		// Token: 0x060001A6 RID: 422
		void Play();

		// Token: 0x060001A7 RID: 423
		void Stop();
	}
}

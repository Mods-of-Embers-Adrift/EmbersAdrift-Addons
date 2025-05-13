using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace CafofoStudio
{
	// Token: 0x02000055 RID: 85
	[Serializable]
	public class SoundSubElementLoop : ISoundSubElement
	{
		// Token: 0x060001B5 RID: 437 RVA: 0x0009A9E8 File Offset: 0x00098BE8
		public void InitializeAudioSources(GameObject parent, AudioMixerGroup outputMixer)
		{
			this.loopAudioSources = new List<AudioSource>();
			foreach (AudioClip clip in this.audioClips)
			{
				AudioSource audioSource = parent.AddComponent<AudioSource>();
				audioSource.clip = clip;
				audioSource.loop = true;
				audioSource.playOnAwake = false;
				audioSource.outputAudioMixerGroup = outputMixer;
				this.loopAudioSources.Add(audioSource);
			}
		}

		// Token: 0x060001B6 RID: 438 RVA: 0x0009AA70 File Offset: 0x00098C70
		public void CalculateIntensity(float intensity, float volumeMultiplier)
		{
			if (intensity == 0f)
			{
				for (int i = 0; i < this.loopAudioSources.Count; i++)
				{
					this.loopAudioSources[i].volume = 0f;
				}
				return;
			}
			if (this.isAditive)
			{
				this.CalculateAditiveIntensity(intensity, volumeMultiplier);
				return;
			}
			this.CalculateCrossfadeIntensity(intensity, volumeMultiplier);
		}

		// Token: 0x060001B7 RID: 439 RVA: 0x0009AACC File Offset: 0x00098CCC
		private void CalculateAditiveIntensity(float intensity, float volumeMultiplier)
		{
			float num = intensity * (float)this.loopAudioSources.Count;
			for (int i = 0; i < this.loopAudioSources.Count; i++)
			{
				this.loopAudioSources[i].volume = Mathf.Clamp01(num - (float)i) * volumeMultiplier;
			}
		}

		// Token: 0x060001B8 RID: 440 RVA: 0x0009AB1C File Offset: 0x00098D1C
		private void CalculateCrossfadeIntensity(float intensity, float volumeMultiplier)
		{
			float num = 1f / (float)(2 * this.loopAudioSources.Count - 1);
			for (int i = 0; i < this.loopAudioSources.Count; i++)
			{
				float num2 = (float)(i * 2) * num - num;
				float volume = Mathf.Clamp01(this.CalculateCrossfade(num, intensity - num2)) * volumeMultiplier;
				this.loopAudioSources[i].volume = volume;
			}
		}

		// Token: 0x060001B9 RID: 441 RVA: 0x0004545F File Offset: 0x0004365F
		private float CalculateCrossfade(float periodLength, float intensity)
		{
			if (intensity < periodLength)
			{
				return intensity / periodLength;
			}
			if (intensity > 2f * periodLength)
			{
				return 1f - (intensity - periodLength * 2f) / periodLength;
			}
			return 1f;
		}

		// Token: 0x060001BA RID: 442 RVA: 0x0009AB84 File Offset: 0x00098D84
		public void SetOutputMixerGroup(AudioMixerGroup overrideOutputMixer)
		{
			if (this.loopAudioSources != null)
			{
				for (int i = 0; i < this.loopAudioSources.Count; i++)
				{
					this.loopAudioSources[i].outputAudioMixerGroup = overrideOutputMixer;
				}
			}
		}

		// Token: 0x060001BB RID: 443 RVA: 0x0009ABC4 File Offset: 0x00098DC4
		public void Play()
		{
			if (this.loopAudioSources != null)
			{
				foreach (AudioSource audioSource in this.loopAudioSources)
				{
					audioSource.Play();
				}
			}
		}

		// Token: 0x060001BC RID: 444 RVA: 0x0009AC1C File Offset: 0x00098E1C
		public void Stop()
		{
			if (this.loopAudioSources != null)
			{
				foreach (AudioSource audioSource in this.loopAudioSources)
				{
					audioSource.Stop();
				}
			}
		}

		// Token: 0x040003A5 RID: 933
		[SerializeField]
		private bool isAditive = true;

		// Token: 0x040003A6 RID: 934
		[SerializeField]
		private List<AudioClip> audioClips;

		// Token: 0x040003A7 RID: 935
		private List<AudioSource> loopAudioSources;
	}
}

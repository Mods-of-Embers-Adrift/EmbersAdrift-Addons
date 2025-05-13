using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace CafofoStudio
{
	// Token: 0x02000056 RID: 86
	[Serializable]
	public class SoundSubElementSample : ISoundSubElement
	{
		// Token: 0x060001BE RID: 446 RVA: 0x00045499 File Offset: 0x00043699
		public void InitializeAudioSources(GameObject parent, AudioMixerGroup outputMixer)
		{
			this.mParentGO = parent;
			this.mOutputMixer = outputMixer;
			this.audioSourcePool = new List<AudioSource>();
		}

		// Token: 0x060001BF RID: 447 RVA: 0x0009AC74 File Offset: 0x00098E74
		public void CalculateIntensity(float intensity, float volumeMultiplier)
		{
			float num = Mathf.Lerp(this.lowIntensityMinSeconds, this.highIntensityMinSeconds, intensity);
			float num2 = Mathf.Lerp(this.lowIntensityMaxSeconds, this.highIntensityMaxSeconds, intensity);
			if (num > num2)
			{
				float num3 = num;
				num = num2;
				num2 = num3;
			}
			float num4 = UnityEngine.Random.Range(num, num2);
			this.nextSampleCountdown = num4;
		}

		// Token: 0x060001C0 RID: 448 RVA: 0x000454B4 File Offset: 0x000436B4
		public void UpdateSampleTimer(float intensity, float volumeMultiplier)
		{
			if (this.isPlaying && intensity > 0f)
			{
				this.nextSampleCountdown -= Time.deltaTime;
				if (this.nextSampleCountdown <= 0f)
				{
					this.PlayAnySample(volumeMultiplier);
					this.CalculateIntensity(intensity, volumeMultiplier);
				}
			}
		}

		// Token: 0x060001C1 RID: 449 RVA: 0x0009ACC0 File Offset: 0x00098EC0
		private void PlayAnySample(float volumeMultiplier)
		{
			AudioSource audioSource = this.GetAudioSource();
			audioSource.panStereo = UnityEngine.Random.Range(-1f, 1f);
			audioSource.clip = this.audioClips[this.GetRandomSoundIndex()];
			audioSource.volume = UnityEngine.Random.Range(0.5f, 1f) * volumeMultiplier;
			audioSource.pitch = UnityEngine.Random.Range(0.8f, 1.2f);
			audioSource.Play();
		}

		// Token: 0x060001C2 RID: 450 RVA: 0x0009AD30 File Offset: 0x00098F30
		private int GetRandomSoundIndex()
		{
			if (this.availableSoundIndexes.Count == 0)
			{
				for (int i = 0; i < this.audioClips.Count; i++)
				{
					this.availableSoundIndexes.Add(i);
				}
			}
			int index = UnityEngine.Random.Range(0, this.availableSoundIndexes.Count);
			int result = this.availableSoundIndexes[index];
			this.availableSoundIndexes.RemoveAt(index);
			return result;
		}

		// Token: 0x060001C3 RID: 451 RVA: 0x0009AD98 File Offset: 0x00098F98
		private AudioSource GetAudioSource()
		{
			int num = 0;
			foreach (AudioSource audioSource in this.audioSourcePool)
			{
				if (!audioSource.isPlaying)
				{
					this.audioSourcePool.RemoveAt(num);
					this.audioSourcePool.Add(audioSource);
					return audioSource;
				}
				num++;
			}
			AudioSource audioSource2 = this.mParentGO.AddComponent<AudioSource>();
			audioSource2.outputAudioMixerGroup = this.mOutputMixer;
			audioSource2.playOnAwake = false;
			this.audioSourcePool.Add(audioSource2);
			return audioSource2;
		}

		// Token: 0x060001C4 RID: 452 RVA: 0x0009AE40 File Offset: 0x00099040
		public void SetOutputMixerGroup(AudioMixerGroup overrideOutputMixer)
		{
			this.mOutputMixer = overrideOutputMixer;
			foreach (AudioSource audioSource in this.audioSourcePool)
			{
				audioSource.outputAudioMixerGroup = overrideOutputMixer;
			}
		}

		// Token: 0x060001C5 RID: 453 RVA: 0x000454F4 File Offset: 0x000436F4
		public void Play()
		{
			this.isPlaying = true;
		}

		// Token: 0x060001C6 RID: 454 RVA: 0x000454FD File Offset: 0x000436FD
		public void Stop()
		{
			this.isPlaying = false;
		}

		// Token: 0x040003A8 RID: 936
		[SerializeField]
		private List<AudioClip> audioClips;

		// Token: 0x040003A9 RID: 937
		[Tooltip("The least seconds between sounds at the lowest intensity.")]
		public float lowIntensityMinSeconds;

		// Token: 0x040003AA RID: 938
		[Tooltip("The most seconds between sounds at the lowest intensity.")]
		public float lowIntensityMaxSeconds;

		// Token: 0x040003AB RID: 939
		[Tooltip("The least seconds between sounds at the highest intensity.")]
		public float highIntensityMinSeconds;

		// Token: 0x040003AC RID: 940
		[Tooltip("The most seconds between sounds at the highest intensity.")]
		public float highIntensityMaxSeconds;

		// Token: 0x040003AD RID: 941
		private float nextSampleCountdown;

		// Token: 0x040003AE RID: 942
		private List<int> availableSoundIndexes = new List<int>();

		// Token: 0x040003AF RID: 943
		private List<AudioSource> audioSourcePool;

		// Token: 0x040003B0 RID: 944
		private bool isPlaying;

		// Token: 0x040003B1 RID: 945
		private GameObject mParentGO;

		// Token: 0x040003B2 RID: 946
		private AudioMixerGroup mOutputMixer;
	}
}

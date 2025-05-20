using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace CafofoStudio
{
	// Token: 0x02000054 RID: 84
	[Serializable]
	public class SoundElement
	{
		// Token: 0x060001A9 RID: 425 RVA: 0x0009A640 File Offset: 0x00098840
		public void InitializeAudioSources(GameObject parent)
		{
			foreach (SoundSubElementLoop soundSubElementLoop in this.loopSubElements)
			{
				soundSubElementLoop.InitializeAudioSources(parent, this.overrideOutputMixer);
			}
			foreach (SoundSubElementSample soundSubElementSample in this.sampleSubElements)
			{
				soundSubElementSample.InitializeAudioSources(parent, this.overrideOutputMixer);
			}
			this.CalculateIntensity();
		}

		// Token: 0x060001AA RID: 426 RVA: 0x0009A6E4 File Offset: 0x000988E4
		private void CalculateIntensity()
		{
			foreach (SoundSubElementLoop soundSubElementLoop in this.loopSubElements)
			{
				soundSubElementLoop.CalculateIntensity(this.intensity, this.volumeMultiplier);
			}
			foreach (SoundSubElementSample soundSubElementSample in this.sampleSubElements)
			{
				soundSubElementSample.CalculateIntensity(this.intensity, this.volumeMultiplier);
			}
		}

		// Token: 0x060001AB RID: 427 RVA: 0x0009A78C File Offset: 0x0009898C
		public void UpdateSampleTimer()
		{
			foreach (SoundSubElementSample soundSubElementSample in this.sampleSubElements)
			{
				soundSubElementSample.UpdateSampleTimer(this.intensity, this.volumeMultiplier);
			}
		}

		// Token: 0x060001AC RID: 428 RVA: 0x00045420 File Offset: 0x00043620
		public void SetIntensity(float intensity)
		{
			this.intensity = Mathf.Clamp01(intensity);
			this.CalculateIntensity();
		}

		// Token: 0x060001AD RID: 429 RVA: 0x00045434 File Offset: 0x00043634
		public float GetIntensity()
		{
			return this.intensity;
		}

		// Token: 0x060001AE RID: 430 RVA: 0x0009A7E8 File Offset: 0x000989E8
		public void SetVolumeMultiplier(float volumeMultiplier)
		{
			this.volumeMultiplier = Mathf.Clamp01(volumeMultiplier);
			foreach (SoundSubElementLoop soundSubElementLoop in this.loopSubElements)
			{
				soundSubElementLoop.CalculateIntensity(this.intensity, this.volumeMultiplier);
			}
		}

		// Token: 0x060001AF RID: 431 RVA: 0x0004543C File Offset: 0x0004363C
		public float GetVolumeMultiplier()
		{
			return this.volumeMultiplier;
		}

		// Token: 0x060001B0 RID: 432 RVA: 0x0009A850 File Offset: 0x00098A50
		public void SetOutputMixerGroup(AudioMixerGroup overrideOutputMixer)
		{
			this.overrideOutputMixer = overrideOutputMixer;
			foreach (SoundSubElementLoop soundSubElementLoop in this.loopSubElements)
			{
				soundSubElementLoop.SetOutputMixerGroup(overrideOutputMixer);
			}
			foreach (SoundSubElementSample soundSubElementSample in this.sampleSubElements)
			{
				soundSubElementSample.SetOutputMixerGroup(overrideOutputMixer);
			}
		}

		// Token: 0x060001B1 RID: 433 RVA: 0x00045444 File Offset: 0x00043644
		public AudioMixerGroup GetOutputMixerGroup()
		{
			return this.overrideOutputMixer;
		}

		// Token: 0x060001B2 RID: 434 RVA: 0x0009A8E8 File Offset: 0x00098AE8
		public void Play()
		{
			foreach (SoundSubElementLoop soundSubElementLoop in this.loopSubElements)
			{
				soundSubElementLoop.Play();
			}
			foreach (SoundSubElementSample soundSubElementSample in this.sampleSubElements)
			{
				soundSubElementSample.Play();
			}
		}

		// Token: 0x060001B3 RID: 435 RVA: 0x0009A978 File Offset: 0x00098B78
		public void Stop()
		{
			foreach (SoundSubElementLoop soundSubElementLoop in this.loopSubElements)
			{
				soundSubElementLoop.Stop();
			}
			foreach (SoundSubElementSample soundSubElementSample in this.sampleSubElements)
			{
				soundSubElementSample.Stop();
			}
		}

		// Token: 0x0400039D RID: 925
		[SerializeField]
		private string soundName;

		// Token: 0x0400039E RID: 926
		[SerializeField]
		private AudioMixerGroup overrideOutputMixer;

		// Token: 0x0400039F RID: 927
		[SerializeField]
		private float intensity;

		// Token: 0x040003A0 RID: 928
		[SerializeField]
		private string maxIntensityLabel;

		// Token: 0x040003A1 RID: 929
		[SerializeField]
		private string minIntensityLabel;

		// Token: 0x040003A2 RID: 930
		[SerializeField]
		private float volumeMultiplier = 1f;

		// Token: 0x040003A3 RID: 931
		[SerializeField]
		private List<SoundSubElementSample> sampleSubElements;

		// Token: 0x040003A4 RID: 932
		[SerializeField]
		private List<SoundSubElementLoop> loopSubElements;
	}
}

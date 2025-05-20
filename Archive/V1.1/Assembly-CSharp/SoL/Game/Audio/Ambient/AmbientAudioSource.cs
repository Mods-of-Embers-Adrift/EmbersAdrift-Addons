using System;
using System.Collections;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Audio.Ambient
{
	// Token: 0x02000D29 RID: 3369
	public class AmbientAudioSource : MonoBehaviour
	{
		// Token: 0x17001852 RID: 6226
		// (get) Token: 0x06006560 RID: 25952 RVA: 0x00084432 File Offset: 0x00082632
		// (set) Token: 0x06006561 RID: 25953 RVA: 0x0020C8E8 File Offset: 0x0020AAE8
		public IAmbientAudioZone Zone
		{
			get
			{
				return this.m_zone;
			}
			set
			{
				this.InitInternal();
				if (this.m_fadeZoneCo != null)
				{
					base.StopCoroutine(this.m_fadeZoneCo);
				}
				if (this.m_audioSource != null)
				{
					this.m_fadeZoneCo = this.FadeZone(value);
					base.StartCoroutine(this.m_fadeZoneCo);
				}
			}
		}

		// Token: 0x06006562 RID: 25954 RVA: 0x0008443A File Offset: 0x0008263A
		private void Awake()
		{
			this.m_audioSource = base.gameObject.AddComponent<AudioSource>();
			this.m_audioSource.ConfigureAudioSourceForAmbient();
		}

		// Token: 0x06006563 RID: 25955 RVA: 0x0020C938 File Offset: 0x0020AB38
		private void Update()
		{
			if (this.Zone != null && this.Zone.Profile)
			{
				if (LocalPlayer.GameEntity)
				{
					float x = this.Zone.Profile.MovementAmplitude.x * (Mathf.PerlinNoise(this.m_seed1, Time.time * this.Zone.Profile.MovementFrequency.x) - 0.5f);
					float y = this.Zone.Profile.MovementAmplitude.y * Mathf.PerlinNoise(this.m_seed2, Time.time * this.Zone.Profile.MovementFrequency.y);
					float z = this.Zone.Profile.MovementAmplitude.z * (Mathf.PerlinNoise(Time.time * this.Zone.Profile.MovementFrequency.z, this.m_seed1) - 0.5f);
					this.m_audioSource.gameObject.transform.localPosition = new Vector3(x, y, z) + LocalPlayer.GameEntity.gameObject.transform.position;
				}
				this.CrossFadeNewClip();
			}
		}

		// Token: 0x06006564 RID: 25956 RVA: 0x0020CA74 File Offset: 0x0020AC74
		private void CrossFadeNewClip()
		{
			if (this.m_fadeZoneCo != null)
			{
				this.m_previousVolume = null;
				return;
			}
			AudioClip audioClip = this.Zone.Profile.GetAudioClip();
			if (audioClip != this.m_audioSource.clip)
			{
				if (this.m_previousVolume == null)
				{
					this.m_previousVolume = new float?(this.m_audioSource.volume);
				}
				this.m_audioSource.volume = Mathf.MoveTowards(this.m_audioSource.volume, 0f, 0.01f);
				if (this.m_audioSource.volume <= 0f)
				{
					this.m_audioSource.clip = audioClip;
					this.m_audioSource.Play();
					return;
				}
			}
			else if (this.m_previousVolume != null)
			{
				this.m_audioSource.volume = Mathf.MoveTowards(this.m_audioSource.volume, this.m_previousVolume.Value, 0.01f);
				if (this.m_audioSource.volume >= this.m_previousVolume.Value)
				{
					this.m_previousVolume = null;
				}
			}
		}

		// Token: 0x06006565 RID: 25957 RVA: 0x00084458 File Offset: 0x00082658
		private void InitInternal()
		{
			if (!this.m_initialized)
			{
				this.m_seed1 = UnityEngine.Random.Range(0f, 1f);
				this.m_seed2 = UnityEngine.Random.Range(0f, 1f);
				this.m_initialized = true;
			}
		}

		// Token: 0x06006566 RID: 25958 RVA: 0x00084493 File Offset: 0x00082693
		private IEnumerator FadeZone(IAmbientAudioZone zone)
		{
			bool fadeOut = zone == null;
			if (!fadeOut)
			{
				this.m_fadeTime = zone.Profile.FadeTime;
				this.m_initialVolume = zone.Profile.Volume;
				this.m_audioSource.spatialBlend = zone.Profile.SpatialBlend;
			}
			float startVolume = this.m_audioSource.volume;
			float endVolume = fadeOut ? 0f : this.m_initialVolume;
			this.m_audioSource.volume = startVolume;
			if (!fadeOut)
			{
				this.m_zone = zone;
				this.m_audioSource.clip = zone.Profile.GetAudioClip();
				this.m_audioSource.loop = true;
				this.m_audioSource.Play();
			}
			float t = 0f;
			float fadeTime = fadeOut ? Mathf.Lerp(0f, this.m_fadeTime, this.m_audioSource.volume / this.m_initialVolume) : Mathf.Lerp(this.m_fadeTime, 0f, this.m_audioSource.volume / this.m_initialVolume);
			while (t < fadeTime)
			{
				this.m_audioSource.volume = Mathf.Lerp(startVolume, endVolume, t / fadeTime);
				t += Time.deltaTime;
				yield return null;
			}
			this.m_audioSource.volume = endVolume;
			if (fadeOut)
			{
				this.m_audioSource.Stop();
				this.m_audioSource.loop = false;
				this.m_audioSource.clip = null;
				this.m_zone = null;
			}
			this.m_fadeZoneCo = null;
			yield break;
		}

		// Token: 0x04005820 RID: 22560
		private const float kCrossfadeRate = 0.01f;

		// Token: 0x04005821 RID: 22561
		private AudioSource m_audioSource;

		// Token: 0x04005822 RID: 22562
		private float m_fadeTime = 5f;

		// Token: 0x04005823 RID: 22563
		private float m_initialVolume;

		// Token: 0x04005824 RID: 22564
		private IEnumerator m_fadeZoneCo;

		// Token: 0x04005825 RID: 22565
		private bool m_initialized;

		// Token: 0x04005826 RID: 22566
		private float m_seed1;

		// Token: 0x04005827 RID: 22567
		private float m_seed2;

		// Token: 0x04005828 RID: 22568
		private float? m_previousVolume;

		// Token: 0x04005829 RID: 22569
		private IAmbientAudioZone m_zone;
	}
}

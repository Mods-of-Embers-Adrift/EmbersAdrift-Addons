using System;
using System.Collections;
using SoL.Game.Objects;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Audio
{
	// Token: 0x02000CFD RID: 3325
	[Serializable]
	public class AudioEvent : IAudioEvent
	{
		// Token: 0x17001823 RID: 6179
		// (get) Token: 0x06006487 RID: 25735 RVA: 0x000839B5 File Offset: 0x00081BB5
		private bool m_showPitchShift
		{
			get
			{
				return this.m_clipSource == AudioEvent.ClipSource.Array || this.m_clipSource == AudioEvent.ClipSource.Single;
			}
		}

		// Token: 0x17001824 RID: 6180
		// (get) Token: 0x06006488 RID: 25736 RVA: 0x00053971 File Offset: 0x00051B71
		private IEnumerable GetClipCollection
		{
			get
			{
				return SolOdinUtilities.GetDropdownItems<AudioClipCollection>();
			}
		}

		// Token: 0x17001825 RID: 6181
		// (get) Token: 0x06006489 RID: 25737 RVA: 0x000839CB File Offset: 0x00081BCB
		public string EventName
		{
			get
			{
				return this.m_eventName;
			}
		}

		// Token: 0x17001826 RID: 6182
		// (get) Token: 0x0600648A RID: 25738 RVA: 0x000839D3 File Offset: 0x00081BD3
		public AudioImpulseFlags ImpulseFlags
		{
			get
			{
				return this.m_impulseType;
			}
		}

		// Token: 0x17001827 RID: 6183
		// (get) Token: 0x0600648B RID: 25739 RVA: 0x0006109C File Offset: 0x0005F29C
		public float ImpulseForce
		{
			get
			{
				return 1f;
			}
		}

		// Token: 0x0600648C RID: 25740 RVA: 0x000839DB File Offset: 0x00081BDB
		public AudioEvent()
		{
		}

		// Token: 0x0600648D RID: 25741 RVA: 0x0020A048 File Offset: 0x00208248
		public AudioEvent(AudioEvent copyFrom, AudioSource source)
		{
			this.m_clipSource = copyFrom.m_clipSource;
			this.m_playChance = copyFrom.m_playChance;
			this.m_eventName = copyFrom.m_eventName;
			this.m_pitchShift = copyFrom.m_pitchShift;
			this.m_audioSource = source;
			switch (this.m_clipSource)
			{
			case AudioEvent.ClipSource.Single:
				this.m_clip = copyFrom.m_clip;
				return;
			case AudioEvent.ClipSource.Array:
				this.m_clips = new AudioClip[copyFrom.m_clips.Length];
				for (int i = 0; i < copyFrom.m_clips.Length; i++)
				{
					this.m_clips[i] = copyFrom.m_clips[i];
				}
				return;
			case AudioEvent.ClipSource.Collection:
				this.m_clipCollection = copyFrom.m_clipCollection;
				return;
			default:
				return;
			}
		}

		// Token: 0x17001828 RID: 6184
		// (get) Token: 0x0600648E RID: 25742 RVA: 0x00083A03 File Offset: 0x00081C03
		// (set) Token: 0x0600648F RID: 25743 RVA: 0x00083A0B File Offset: 0x00081C0B
		public AudioSource Source
		{
			get
			{
				return this.m_audioSource;
			}
			set
			{
				this.m_audioSource = value;
			}
		}

		// Token: 0x06006490 RID: 25744 RVA: 0x0020A11C File Offset: 0x0020831C
		public int GetClipCount()
		{
			switch (this.m_clipSource)
			{
			case AudioEvent.ClipSource.Single:
				if (!(this.m_clip == null))
				{
					return 1;
				}
				return 0;
			case AudioEvent.ClipSource.Array:
				if (this.m_clips != null)
				{
					return this.m_clips.Length;
				}
				return 0;
			case AudioEvent.ClipSource.Collection:
				return this.m_clipCollection.ClipCount;
			default:
				return 0;
			}
		}

		// Token: 0x06006491 RID: 25745 RVA: 0x0020A178 File Offset: 0x00208378
		public void Play(float volumeFraction = 1f)
		{
			if (this.m_audioSource == null || !this.m_audioSource.enabled)
			{
				return;
			}
			if (this.m_playChance < 1f && UnityEngine.Random.Range(0f, 1f) > this.m_playChance)
			{
				return;
			}
			AudioClip clip = this.GetClip();
			if (clip != null)
			{
				this.RefreshMixerGroup();
				if (this.m_defaultVolume == null)
				{
					this.m_defaultVolume = new float?(this.m_audioSource.volume);
				}
				this.m_audioSource.volume = this.m_defaultVolume.Value * volumeFraction;
				this.m_audioSource.pitch = this.GetPitch();
				this.m_audioSource.PlayOneShot(clip);
			}
		}

		// Token: 0x06006492 RID: 25746 RVA: 0x0020A234 File Offset: 0x00208434
		private AudioClip GetClip()
		{
			AudioClip result = null;
			switch (this.m_clipSource)
			{
			case AudioEvent.ClipSource.Single:
				result = this.m_clip;
				break;
			case AudioEvent.ClipSource.Array:
				if (this.m_clips != null && this.m_clips.Length != 0)
				{
					int num = UnityEngine.Random.Range(0, this.m_clips.Length);
					result = this.m_clips[num];
				}
				break;
			case AudioEvent.ClipSource.Collection:
				if (this.m_clipCollection != null)
				{
					result = this.m_clipCollection.GetRandomClip();
				}
				break;
			}
			return result;
		}

		// Token: 0x06006493 RID: 25747 RVA: 0x0020A2B0 File Offset: 0x002084B0
		private float GetPitch()
		{
			float result = 1f;
			AudioEvent.ClipSource clipSource = this.m_clipSource;
			if (clipSource > AudioEvent.ClipSource.Array)
			{
				if (clipSource == AudioEvent.ClipSource.Collection)
				{
					if (this.m_clipCollection != null)
					{
						result = this.m_clipCollection.PitchRange.RandomWithinRange();
					}
				}
			}
			else
			{
				result = this.m_pitchShift.RandomWithinRange();
			}
			return result;
		}

		// Token: 0x06006494 RID: 25748 RVA: 0x00083A14 File Offset: 0x00081C14
		private void RefreshMixerGroup()
		{
			if (!this.m_refreshedMixer)
			{
				this.m_audioSource.RefreshMixerGroup();
				this.m_refreshedMixer = true;
			}
		}

		// Token: 0x06006495 RID: 25749 RVA: 0x00083A30 File Offset: 0x00081C30
		private void TriggerEvent()
		{
			this.Play(1f);
		}

		// Token: 0x04005749 RID: 22345
		[SerializeField]
		private AudioEvent.ClipSource m_clipSource;

		// Token: 0x0400574A RID: 22346
		[Range(0f, 1f)]
		[SerializeField]
		private float m_playChance = 1f;

		// Token: 0x0400574B RID: 22347
		[SerializeField]
		private string m_eventName;

		// Token: 0x0400574C RID: 22348
		[SerializeField]
		private AudioSource m_audioSource;

		// Token: 0x0400574D RID: 22349
		[SerializeField]
		private AudioClip m_clip;

		// Token: 0x0400574E RID: 22350
		[SerializeField]
		private AudioClip[] m_clips;

		// Token: 0x0400574F RID: 22351
		[SerializeField]
		private AudioClipCollection m_clipCollection;

		// Token: 0x04005750 RID: 22352
		[SerializeField]
		private MinMaxFloatRange m_pitchShift = new MinMaxFloatRange(1f, 1f);

		// Token: 0x04005751 RID: 22353
		[SerializeField]
		private AudioImpulseFlags m_impulseType;

		// Token: 0x04005752 RID: 22354
		private float? m_defaultVolume;

		// Token: 0x04005753 RID: 22355
		private bool m_refreshedMixer;

		// Token: 0x02000CFE RID: 3326
		private enum ClipSource
		{
			// Token: 0x04005755 RID: 22357
			Single,
			// Token: 0x04005756 RID: 22358
			Array,
			// Token: 0x04005757 RID: 22359
			Collection
		}
	}
}

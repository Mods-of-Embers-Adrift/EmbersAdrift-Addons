using System;
using SoL.Game.Objects;
using UnityEngine;

namespace SoL.Game.Audio
{
	// Token: 0x02000CF8 RID: 3320
	[CreateAssetMenu(menuName = "SoL/Collections/AudioClips", order = 7)]
	public class AudioClipCollection : ScriptableObject
	{
		// Token: 0x1700181D RID: 6173
		// (get) Token: 0x06006478 RID: 25720 RVA: 0x000838E1 File Offset: 0x00081AE1
		protected virtual AudioClip[] Clips
		{
			get
			{
				return this.m_clips;
			}
		}

		// Token: 0x1700181E RID: 6174
		// (get) Token: 0x06006479 RID: 25721 RVA: 0x000838E9 File Offset: 0x00081AE9
		public MinMaxFloatRange PitchRange
		{
			get
			{
				return this.m_pitchRange;
			}
		}

		// Token: 0x1700181F RID: 6175
		// (get) Token: 0x0600647A RID: 25722 RVA: 0x000838F1 File Offset: 0x00081AF1
		public int ClipCount
		{
			get
			{
				if (this.Clips != null)
				{
					return this.Clips.Length;
				}
				return 0;
			}
		}

		// Token: 0x0600647B RID: 25723 RVA: 0x00083905 File Offset: 0x00081B05
		public AudioClip GetRandomClip()
		{
			if (this.Clips != null && this.Clips.Length != 0)
			{
				return this.Clips[UnityEngine.Random.Range(0, this.Clips.Length)];
			}
			return null;
		}

		// Token: 0x0600647C RID: 25724 RVA: 0x00209F80 File Offset: 0x00208180
		private void OnValidate()
		{
			if (this.m_pitchRange.Min < -3f || this.m_pitchRange.Max > 3f)
			{
				float min = Mathf.Clamp(this.m_pitchRange.Min, -3f, 3f);
				float max = Mathf.Clamp(this.m_pitchRange.Max, -3f, 3f);
				this.m_pitchRange = new MinMaxFloatRange(min, max);
			}
		}

		// Token: 0x0600647D RID: 25725 RVA: 0x00209FF4 File Offset: 0x002081F4
		public void PlayRandomClip(AudioSource audioSource)
		{
			if (audioSource != null)
			{
				AudioClip randomClip = this.GetRandomClip();
				if (randomClip != null)
				{
					audioSource.pitch = this.m_pitchRange.RandomWithinRange();
					audioSource.PlayOneShot(randomClip);
				}
			}
		}

		// Token: 0x0400573B RID: 22331
		[SerializeField]
		private MinMaxFloatRange m_pitchRange = new MinMaxFloatRange(1f, 1f);

		// Token: 0x0400573C RID: 22332
		[SerializeField]
		protected AudioClip[] m_clips;
	}
}

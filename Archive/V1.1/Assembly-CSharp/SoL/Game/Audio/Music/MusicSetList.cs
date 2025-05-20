using System;
using System.Collections.Generic;
using SoL.Game.Objects;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Audio.Music
{
	// Token: 0x02000D23 RID: 3363
	[Serializable]
	public class MusicSetList
	{
		// Token: 0x17001845 RID: 6213
		// (get) Token: 0x06006533 RID: 25907 RVA: 0x000841D9 File Offset: 0x000823D9
		public MusicChannelType Type
		{
			get
			{
				return this.m_type;
			}
		}

		// Token: 0x17001846 RID: 6214
		// (get) Token: 0x06006534 RID: 25908 RVA: 0x000841E1 File Offset: 0x000823E1
		public bool IsPlaying
		{
			get
			{
				return this.m_isPlaying;
			}
		}

		// Token: 0x17001847 RID: 6215
		// (get) Token: 0x06006535 RID: 25909 RVA: 0x000841E9 File Offset: 0x000823E9
		public float CurrentVolume
		{
			get
			{
				if (this.m_currentTrack != null)
				{
					return this.m_currentTrack.volume;
				}
				return 0f;
			}
		}

		// Token: 0x17001848 RID: 6216
		// (get) Token: 0x06006536 RID: 25910 RVA: 0x00084204 File Offset: 0x00082404
		public MusicTrack CurrentTrack
		{
			get
			{
				return this.m_currentTrack;
			}
		}

		// Token: 0x06006537 RID: 25911 RVA: 0x0020BFC4 File Offset: 0x0020A1C4
		private void Initialize()
		{
			if (this.m_initialized)
			{
				return;
			}
			if (this.m_injectSilence && this.m_startInjectionChance > 0f && UnityEngine.Random.Range(0f, 1f) < this.m_startInjectionChance)
			{
				this.m_silenceUntil = new float?(Time.time + (float)this.m_silenceTime.RandomWithinRange());
			}
			else
			{
				this.m_silenceUntil = null;
			}
			if (this.m_liveTracks == null)
			{
				this.m_liveTracks = new List<MusicTrack>(this.m_tracks);
			}
			else
			{
				this.m_liveTracks.Clear();
				for (int i = 0; i < this.m_tracks.Length; i++)
				{
					this.m_liveTracks.Add(this.m_tracks[i]);
				}
			}
			if (this.m_shuffle)
			{
				this.m_liveTracks.Shuffle<MusicTrack>();
			}
			this.m_initialized = true;
		}

		// Token: 0x06006538 RID: 25912 RVA: 0x0008420C File Offset: 0x0008240C
		private AudioClip CreateSilentClip(int index, int length)
		{
			return AudioClip.Create("silence_" + index.ToString(), 1024 * length, 1, 1024, false);
		}

		// Token: 0x06006539 RID: 25913 RVA: 0x0020C098 File Offset: 0x0020A298
		public void Play(AudioSource source)
		{
			if (this.m_isPlaying)
			{
				return;
			}
			this.Initialize();
			if (this.m_currentClipIndex >= this.m_liveTracks.Count)
			{
				return;
			}
			this.m_currentTrack = this.m_liveTracks[this.m_currentClipIndex];
			this.m_currentTrack.QueueUpTrack(source, this.m_timeOffset);
			this.m_isPlaying = true;
			if (this.m_silenceUntil != null)
			{
				if (Time.time < this.m_silenceUntil.Value)
				{
					return;
				}
				this.m_silenceUntil = null;
			}
			this.PlayTrack();
		}

		// Token: 0x0600653A RID: 25914 RVA: 0x0020C12C File Offset: 0x0020A32C
		private void PlayTrack()
		{
			if (this.m_currentTrack == null || this.m_currentTrack.clip == null)
			{
				Debug.LogWarning("No clip while attempting to play an audio track?!");
				return;
			}
			this.m_trackPlayingUntil = new float?(Time.time + (this.m_currentTrack.clip.length - this.m_timeOffset) + 1f);
			this.m_currentTrack.Play();
		}

		// Token: 0x0600653B RID: 25915 RVA: 0x0020C198 File Offset: 0x0020A398
		public void Stop()
		{
			if (!this.m_isPlaying)
			{
				return;
			}
			if (this.m_currentTrack != null)
			{
				this.m_timeOffset = this.m_currentTrack.time;
				this.m_currentTrack.Stop();
			}
			this.m_trackPlayingUntil = null;
			this.m_isPlaying = false;
		}

		// Token: 0x0600653C RID: 25916 RVA: 0x0020C1E8 File Offset: 0x0020A3E8
		public void Update()
		{
			if (!this.m_isPlaying || this.m_expired || this.m_liveTracks == null || this.m_liveTracks.Count <= this.m_currentClipIndex)
			{
				return;
			}
			if (this.m_silenceUntil != null)
			{
				if (Time.time >= this.m_silenceUntil.Value)
				{
					this.m_silenceUntil = null;
					this.PlayTrack();
				}
				return;
			}
			if (this.m_currentTrack != null && this.m_trackPlayingUntil != null && this.m_trackPlayingUntil.Value <= Time.time)
			{
				this.m_trackPlayingUntil = null;
				if (this.m_currentClipIndex == this.m_liveTracks.Count - 1 && !this.m_loop)
				{
					this.m_currentTrack.Stop();
					this.m_timeOffset = 0f;
					this.m_expired = true;
					return;
				}
				this.m_currentClipIndex = (this.m_currentClipIndex + 1) % this.m_liveTracks.Count;
				this.m_currentTrack.Stop();
				AudioSource source = this.m_currentTrack.Source;
				this.m_currentTrack = this.m_liveTracks[this.m_currentClipIndex];
				this.m_currentTrack.QueueUpTrack(source, 0f);
				this.m_timeOffset = 0f;
				if (this.m_injectSilence && UnityEngine.Random.Range(0f, 1f) < this.m_betweenTrackInjectionChance)
				{
					this.m_silenceUntil = new float?(Time.time + (float)this.m_silenceTime.RandomWithinRange());
					return;
				}
				this.PlayTrack();
			}
		}

		// Token: 0x0600653D RID: 25917 RVA: 0x00084232 File Offset: 0x00082432
		public void UpdateVolumeState()
		{
			MusicTrack currentTrack = this.m_currentTrack;
			if (currentTrack == null)
			{
				return;
			}
			currentTrack.UpdateVolumeState();
		}

		// Token: 0x040057F4 RID: 22516
		private const int kSilenceBitrate = 1024;

		// Token: 0x040057F5 RID: 22517
		[SerializeField]
		private MusicChannelType m_type;

		// Token: 0x040057F6 RID: 22518
		[SerializeField]
		private bool m_shuffle;

		// Token: 0x040057F7 RID: 22519
		[SerializeField]
		private bool m_loop;

		// Token: 0x040057F8 RID: 22520
		[SerializeField]
		private MusicTrack[] m_tracks;

		// Token: 0x040057F9 RID: 22521
		[SerializeField]
		private bool m_injectSilence;

		// Token: 0x040057FA RID: 22522
		[SerializeField]
		[Range(0f, 1f)]
		private float m_startInjectionChance = 0.25f;

		// Token: 0x040057FB RID: 22523
		[SerializeField]
		[Range(0f, 1f)]
		private float m_betweenTrackInjectionChance = 1f;

		// Token: 0x040057FC RID: 22524
		[SerializeField]
		private MinMaxIntRange m_silenceTime = new MinMaxIntRange(30, 60);

		// Token: 0x040057FD RID: 22525
		[NonSerialized]
		private List<MusicTrack> m_liveTracks;

		// Token: 0x040057FE RID: 22526
		[NonSerialized]
		private MusicTrack m_currentTrack;

		// Token: 0x040057FF RID: 22527
		[NonSerialized]
		private int m_currentClipIndex;

		// Token: 0x04005800 RID: 22528
		[NonSerialized]
		private float m_timeOffset;

		// Token: 0x04005801 RID: 22529
		[NonSerialized]
		private bool m_initialized;

		// Token: 0x04005802 RID: 22530
		[NonSerialized]
		private bool m_expired;

		// Token: 0x04005803 RID: 22531
		[NonSerialized]
		private bool m_isPlaying;

		// Token: 0x04005804 RID: 22532
		private float? m_silenceUntil;

		// Token: 0x04005805 RID: 22533
		private float? m_trackPlayingUntil;
	}
}

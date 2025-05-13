using System;
using SoL.Game.SkyDome;
using SoL.Managers;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Audio.Music
{
	// Token: 0x02000D24 RID: 3364
	[Serializable]
	public class MusicTrack
	{
		// Token: 0x17001849 RID: 6217
		// (get) Token: 0x0600653F RID: 25919 RVA: 0x00084271 File Offset: 0x00082471
		public AudioSource Source
		{
			get
			{
				return this.m_source;
			}
		}

		// Token: 0x1700184A RID: 6218
		// (get) Token: 0x06006540 RID: 25920 RVA: 0x00084279 File Offset: 0x00082479
		public float time
		{
			get
			{
				if (!(this.m_source != null))
				{
					return 0f;
				}
				return this.m_source.time;
			}
		}

		// Token: 0x1700184B RID: 6219
		// (get) Token: 0x06006541 RID: 25921 RVA: 0x0008429A File Offset: 0x0008249A
		public float volume
		{
			get
			{
				if (!(this.m_source != null))
				{
					return 0f;
				}
				return this.m_source.volume;
			}
		}

		// Token: 0x1700184C RID: 6220
		// (get) Token: 0x06006542 RID: 25922 RVA: 0x000842BB File Offset: 0x000824BB
		public AudioClip clip
		{
			get
			{
				if (!(this.m_source != null))
				{
					return null;
				}
				return this.m_source.clip;
			}
		}

		// Token: 0x1700184D RID: 6221
		// (get) Token: 0x06006543 RID: 25923 RVA: 0x000842D8 File Offset: 0x000824D8
		public bool IsMultiTrack
		{
			get
			{
				return this.m_type == MusicTrack.ClipType.Multi;
			}
		}

		// Token: 0x06006544 RID: 25924 RVA: 0x000842E3 File Offset: 0x000824E3
		private string GetLabelText()
		{
			if (this.m_type != MusicTrack.ClipType.Single)
			{
				return "Primary";
			}
			return "Clip";
		}

		// Token: 0x06006545 RID: 25925 RVA: 0x0020C374 File Offset: 0x0020A574
		public void QueueUpTrack(AudioSource source, float timeOffset)
		{
			this.m_source = source;
			if (this.m_source == null)
			{
				return;
			}
			float num = Mathf.Min(timeOffset, this.m_primary.length);
			this.m_source.clip = this.m_primary;
			this.m_source.time = num;
			if (this.m_type == MusicTrack.ClipType.Multi)
			{
				ClientGameManager.MusicManager.LayeredTacksInUseBy = this;
				for (int i = 0; i < ClientGameManager.MusicManager.LayeredTrackSources.Length; i++)
				{
					AudioSource audioSource = ClientGameManager.MusicManager.LayeredTrackSources[i];
					AudioClip layerClipForIndex = this.GetLayerClipForIndex(i);
					audioSource.volume = 0f;
					audioSource.clip = layerClipForIndex;
					audioSource.time = ((layerClipForIndex == null) ? 0f : num);
				}
			}
		}

		// Token: 0x06006546 RID: 25926 RVA: 0x0020C430 File Offset: 0x0020A630
		public void Play()
		{
			if (this.m_source == null)
			{
				return;
			}
			double num = AudioSettings.dspTime + 1.0;
			float length = this.m_primary.length;
			this.m_source.SetScheduledEndTime(num + (double)length);
			this.m_source.PlayScheduled(num);
			if (this.m_type == MusicTrack.ClipType.Multi)
			{
				foreach (AudioSource audioSource in ClientGameManager.MusicManager.LayeredTrackSources)
				{
					if (audioSource != null && audioSource.clip != null)
					{
						audioSource.SetScheduledEndTime(num + (double)length);
						audioSource.PlayScheduled(num);
					}
				}
			}
		}

		// Token: 0x06006547 RID: 25927 RVA: 0x0020C4D8 File Offset: 0x0020A6D8
		public void Stop()
		{
			if (this.m_source == null)
			{
				return;
			}
			this.m_source.Stop();
			if (this.m_type == MusicTrack.ClipType.Multi)
			{
				if (ClientGameManager.MusicManager.LayeredTacksInUseBy == this)
				{
					ClientGameManager.MusicManager.LayeredTacksInUseBy = null;
				}
				foreach (AudioSource audioSource in ClientGameManager.MusicManager.LayeredTrackSources)
				{
					if (audioSource != null)
					{
						audioSource.Stop();
					}
				}
			}
		}

		// Token: 0x06006548 RID: 25928 RVA: 0x0020C54C File Offset: 0x0020A74C
		private AudioClip GetLayerClipForIndex(int index)
		{
			switch (index)
			{
			case 0:
				return this.m_poi;
			case 1:
				return this.m_day;
			case 2:
				return this.m_night;
			case 3:
				return this.m_peace;
			case 4:
				return this.m_combat;
			default:
				return null;
			}
		}

		// Token: 0x06006549 RID: 25929 RVA: 0x000842F8 File Offset: 0x000824F8
		public void UpdateVolumeState()
		{
			if (this.m_type == MusicTrack.ClipType.Multi)
			{
				this.UpdateDayNightSources();
				this.UpdatePeaceCombatSources();
				this.UpdatePoiSource();
			}
		}

		// Token: 0x0600654A RID: 25930 RVA: 0x0020C59C File Offset: 0x0020A79C
		private void UpdateDayNightSources()
		{
			AudioSource source = ClientGameManager.MusicManager.LayeredTrackSources[1];
			AudioSource source2 = ClientGameManager.MusicManager.LayeredTrackSources[2];
			bool flag = SkyDomeManager.IsDay();
			float targetVolume = flag ? 0.8f : 0f;
			float targetVolume2 = flag ? 0f : 0.8f;
			source.MoveMusicVolumeTowards(targetVolume, true);
			source2.MoveMusicVolumeTowards(targetVolume2, true);
		}

		// Token: 0x0600654B RID: 25931 RVA: 0x0020C5F8 File Offset: 0x0020A7F8
		private void UpdatePeaceCombatSources()
		{
			AudioSource source = ClientGameManager.MusicManager.LayeredTrackSources[3];
			AudioSource source2 = ClientGameManager.MusicManager.LayeredTrackSources[4];
			bool flag = LocalPlayer.GameEntity != null && LocalPlayer.GameEntity.CharacterData != null && LocalPlayer.GameEntity.CharacterData.CharacterFlags.Value.HasBitFlag(PlayerFlags.InCombat);
			float targetVolume = flag ? 0f : 0.8f;
			float targetVolume2 = flag ? 0.8f : 0f;
			source.MoveMusicVolumeTowards(targetVolume, true);
			source2.MoveMusicVolumeTowards(targetVolume2, true);
		}

		// Token: 0x0600654C RID: 25932 RVA: 0x0020C688 File Offset: 0x0020A888
		private void UpdatePoiSource()
		{
			AudioSource source = ClientGameManager.MusicManager.LayeredTrackSources[0];
			float targetVolume = (LocalPlayer.GameEntity != null && LocalPlayer.GameEntity.CharacterData != null && LocalPlayer.GameEntity.CharacterData.CharacterFlags.Value.HasBitFlag(PlayerFlags.InCampfire)) ? 0.8f : 0f;
			source.MoveMusicVolumeTowards(targetVolume, true);
		}

		// Token: 0x04005806 RID: 22534
		[SerializeField]
		private MusicTrack.ClipType m_type;

		// Token: 0x04005807 RID: 22535
		[SerializeField]
		private AudioClip m_primary;

		// Token: 0x04005808 RID: 22536
		[SerializeField]
		private AudioClip m_day;

		// Token: 0x04005809 RID: 22537
		[SerializeField]
		private AudioClip m_night;

		// Token: 0x0400580A RID: 22538
		[SerializeField]
		private AudioClip m_peace;

		// Token: 0x0400580B RID: 22539
		[SerializeField]
		private AudioClip m_combat;

		// Token: 0x0400580C RID: 22540
		[SerializeField]
		private AudioClip m_poi;

		// Token: 0x0400580D RID: 22541
		private AudioSource m_source;

		// Token: 0x0400580E RID: 22542
		private const float kMaxVolume = 0.8f;

		// Token: 0x0400580F RID: 22543
		private const int kPoiIndex = 0;

		// Token: 0x04005810 RID: 22544
		private const int kDayIndex = 1;

		// Token: 0x04005811 RID: 22545
		private const int kNightIndex = 2;

		// Token: 0x04005812 RID: 22546
		private const int kPeaceIndex = 3;

		// Token: 0x04005813 RID: 22547
		private const int kCombatIndex = 4;

		// Token: 0x02000D25 RID: 3365
		private enum ClipType
		{
			// Token: 0x04005815 RID: 22549
			Single,
			// Token: 0x04005816 RID: 22550
			Multi
		}
	}
}

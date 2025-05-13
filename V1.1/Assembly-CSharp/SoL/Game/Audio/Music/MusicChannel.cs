using System;
using System.Collections.Generic;
using SoL.Managers;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Audio.Music
{
	// Token: 0x02000D1C RID: 3356
	public class MusicChannel
	{
		// Token: 0x1700183E RID: 6206
		// (get) Token: 0x0600650B RID: 25867 RVA: 0x00084057 File Offset: 0x00082257
		public bool HasMusic
		{
			get
			{
				return this.m_setLists != null && this.m_setLists.Count > 0;
			}
		}

		// Token: 0x1700183F RID: 6207
		// (get) Token: 0x0600650C RID: 25868 RVA: 0x00084071 File Offset: 0x00082271
		// (set) Token: 0x0600650D RID: 25869 RVA: 0x00084079 File Offset: 0x00082279
		public bool IsActive { get; set; }

		// Token: 0x0600650E RID: 25870 RVA: 0x0020B7BC File Offset: 0x002099BC
		public MusicChannel(MusicManager manager, MusicChannelType channelType)
		{
			this.m_sources = new AudioSource[2];
			this.m_setLists = new List<MusicSetList>();
			this.m_removedSetLists = new List<MusicSetList>();
			GameObject gameObject = new GameObject(channelType.ToString());
			gameObject.transform.SetParent(manager.gameObject.transform);
			for (int i = 0; i < 2; i++)
			{
				AudioSource audioSource = gameObject.AddComponent<AudioSource>();
				audioSource.ConfigureAudioSourceForMusic();
				this.m_sources[i] = audioSource;
			}
		}

		// Token: 0x0600650F RID: 25871 RVA: 0x00084082 File Offset: 0x00082282
		public void UpdateChannel(bool updatePlayStop, bool updateVolume)
		{
			if (updatePlayStop)
			{
				this.UpdatePlayStopState();
			}
			if (updateVolume)
			{
				this.UpdateVolumeState();
			}
		}

		// Token: 0x06006510 RID: 25872 RVA: 0x0020B83C File Offset: 0x00209A3C
		public void AddMusic(MusicSetList setList)
		{
			this.m_previous = this.m_current;
			int num = this.m_setLists.IndexOf(setList);
			if (num >= 0)
			{
				this.m_current = this.m_setLists[num];
				return;
			}
			this.m_setLists.Add(setList);
			this.m_current = this.GetLast();
		}

		// Token: 0x06006511 RID: 25873 RVA: 0x0020B894 File Offset: 0x00209A94
		public void RemoveMusic(MusicSetList setList)
		{
			int num = this.m_setLists.IndexOf(setList);
			if (num >= 0)
			{
				bool flag = num == this.m_setLists.Count - 1;
				this.m_setLists.Remove(setList);
				this.m_removedSetLists.Add(setList);
				if (flag)
				{
					this.m_previous = this.m_current;
					this.m_current = this.GetLast();
				}
			}
		}

		// Token: 0x06006512 RID: 25874 RVA: 0x0020B8F8 File Offset: 0x00209AF8
		public void ClearChannel(List<MusicSetList> newList)
		{
			for (int i = this.m_setLists.Count - 1; i >= 0; i--)
			{
				MusicSetList musicSetList = this.m_setLists[i];
				if (!newList.Contains(musicSetList))
				{
					if (musicSetList.CurrentTrack != null && musicSetList.CurrentTrack.IsMultiTrack && musicSetList.CurrentTrack == ClientGameManager.MusicManager.LayeredTacksInUseBy)
					{
						ClientGameManager.MusicManager.LayeredTacksInUseBy = null;
					}
					this.m_setLists.RemoveAt(i);
				}
			}
			this.m_removedSetLists.Clear();
			this.m_previous = this.m_current;
			this.m_current = null;
		}

		// Token: 0x06006513 RID: 25875 RVA: 0x00084096 File Offset: 0x00082296
		private MusicSetList GetLast()
		{
			if (this.m_setLists == null || this.m_setLists.Count <= 0)
			{
				return null;
			}
			return this.m_setLists[this.m_setLists.Count - 1];
		}

		// Token: 0x06006514 RID: 25876 RVA: 0x0020B990 File Offset: 0x00209B90
		private void UpdatePlayStopState()
		{
			for (int i = 0; i < this.m_setLists.Count; i++)
			{
				if (this.m_current != null && this.IsActive && this.m_setLists[i] == this.m_current)
				{
					if (!this.m_setLists[i].IsPlaying)
					{
						this.m_sourceIndex = (this.m_sourceIndex + 1) % this.m_sources.Length;
					}
					this.m_setLists[i].Play(this.m_sources[this.m_sourceIndex]);
					this.m_setLists[i].Update();
				}
				else if (this.m_setLists[i].CurrentVolume <= 0f)
				{
					this.m_setLists[i].Stop();
				}
			}
			for (int j = 0; j < this.m_removedSetLists.Count; j++)
			{
				if (this.m_removedSetLists[j].CurrentVolume <= 0f)
				{
					this.m_removedSetLists[j].Stop();
					this.m_removedSetLists.RemoveAt(j);
					j--;
				}
			}
		}

		// Token: 0x06006515 RID: 25877 RVA: 0x0020BAB0 File Offset: 0x00209CB0
		private void UpdateVolumeState()
		{
			for (int i = 0; i < this.m_sources.Length; i++)
			{
				bool flag = this.IsActive && i == this.m_sourceIndex;
				AudioSource audioSource = this.m_sources[i];
				if (flag)
				{
					if (audioSource.volume < 0.8f)
					{
						audioSource.MoveMusicVolumeTowards(0.8f, false);
					}
				}
				else if (audioSource.volume > 0f)
				{
					audioSource.MoveMusicVolumeTowards(0f, false);
				}
			}
			MusicSetList current = this.m_current;
			if (current == null)
			{
				return;
			}
			current.UpdateVolumeState();
		}

		// Token: 0x040057D5 RID: 22485
		private const int kNSources = 2;

		// Token: 0x040057D6 RID: 22486
		public const float kMaxVolume = 0.8f;

		// Token: 0x040057D7 RID: 22487
		private const float kThreshold = 0.02f;

		// Token: 0x040057D8 RID: 22488
		private readonly AudioSource[] m_sources;

		// Token: 0x040057D9 RID: 22489
		private readonly List<MusicSetList> m_setLists;

		// Token: 0x040057DA RID: 22490
		private readonly List<MusicSetList> m_removedSetLists;

		// Token: 0x040057DB RID: 22491
		private int m_sourceIndex;

		// Token: 0x040057DC RID: 22492
		private MusicSetList m_previous;

		// Token: 0x040057DD RID: 22493
		private MusicSetList m_current;
	}
}

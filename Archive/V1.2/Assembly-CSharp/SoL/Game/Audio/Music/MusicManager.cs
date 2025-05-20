using System;
using System.Collections.Generic;
using SoL.Managers;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Audio.Music
{
	// Token: 0x02000D22 RID: 3362
	public class MusicManager : MonoBehaviour
	{
		// Token: 0x17001842 RID: 6210
		// (get) Token: 0x06006522 RID: 25890 RVA: 0x0008411E File Offset: 0x0008231E
		// (set) Token: 0x06006523 RID: 25891 RVA: 0x0020BCC4 File Offset: 0x00209EC4
		private MusicChannelType CurrentChannelType
		{
			get
			{
				return this.m_currentChannelType;
			}
			set
			{
				this.m_currentChannelType = value;
				foreach (KeyValuePair<MusicChannelType, MusicChannel> keyValuePair in this.m_channels)
				{
					keyValuePair.Value.IsActive = (keyValuePair.Key == this.CurrentChannelType);
				}
			}
		}

		// Token: 0x17001843 RID: 6211
		// (get) Token: 0x06006524 RID: 25892 RVA: 0x00084126 File Offset: 0x00082326
		// (set) Token: 0x06006525 RID: 25893 RVA: 0x0008412E File Offset: 0x0008232E
		public AudioSource[] LayeredTrackSources { get; private set; }

		// Token: 0x17001844 RID: 6212
		// (get) Token: 0x06006526 RID: 25894 RVA: 0x00084137 File Offset: 0x00082337
		// (set) Token: 0x06006527 RID: 25895 RVA: 0x0008413F File Offset: 0x0008233F
		public MusicTrack LayeredTacksInUseBy { get; set; }

		// Token: 0x06006528 RID: 25896 RVA: 0x0020BD34 File Offset: 0x00209F34
		private void Awake()
		{
			ClientGameManager.MusicManager = this;
			this.LayeredTrackSources = new AudioSource[5];
			GameObject gameObject = new GameObject("LayeredTrackSources");
			gameObject.transform.SetParent(base.gameObject.transform);
			for (int i = 0; i < 5; i++)
			{
				AudioSource audioSource = gameObject.AddComponent<AudioSource>();
				audioSource.ConfigureAudioSourceForMusic();
				this.LayeredTrackSources[i] = audioSource;
			}
			MusicChannelType[] array = (MusicChannelType[])Enum.GetValues(typeof(MusicChannelType));
			for (int j = 0; j < array.Length; j++)
			{
				this.m_channels.Add(array[j], new MusicChannel(this, array[j]));
				this.m_queue.Add(array[j], new List<MusicSetList>());
			}
		}

		// Token: 0x06006529 RID: 25897 RVA: 0x00084148 File Offset: 0x00082348
		private void Start()
		{
			SceneCompositionManager.ZoneLoadStarted += this.SceneCompositionManagerOnZoneLoadStarted;
			SceneCompositionManager.ZoneLoaded += this.SceneCompositionManagerOnZoneLoaded;
		}

		// Token: 0x0600652A RID: 25898 RVA: 0x0008416C File Offset: 0x0008236C
		private void OnDestroy()
		{
			SceneCompositionManager.ZoneLoadStarted -= this.SceneCompositionManagerOnZoneLoadStarted;
			SceneCompositionManager.ZoneLoaded -= this.SceneCompositionManagerOnZoneLoaded;
		}

		// Token: 0x0600652B RID: 25899 RVA: 0x0020BDEC File Offset: 0x00209FEC
		private void Update()
		{
			foreach (KeyValuePair<MusicChannelType, MusicChannel> keyValuePair in this.m_channels)
			{
				keyValuePair.Value.UpdateChannel(!this.m_isLoadingScene, true);
			}
			if (this.LayeredTacksInUseBy == null)
			{
				foreach (AudioSource audioSource in this.LayeredTrackSources)
				{
					if (audioSource.volume > 0f)
					{
						audioSource.MoveMusicVolumeTowards(0f, true);
					}
					else if (audioSource.isPlaying)
					{
						audioSource.Stop();
					}
				}
			}
		}

		// Token: 0x0600652C RID: 25900 RVA: 0x0020BEA0 File Offset: 0x0020A0A0
		private void SceneCompositionManagerOnZoneLoaded(ZoneId obj)
		{
			this.m_isLoadingScene = false;
			for (int i = 0; i < MusicManager.kMusicChannelTypeQueueOrder.Length; i++)
			{
				MusicChannelType key = MusicManager.kMusicChannelTypeQueueOrder[i];
				List<MusicSetList> list = this.m_queue[key];
				this.m_channels[key].ClearChannel(list);
				for (int j = 0; j < list.Count; j++)
				{
					this.AddMusic(list[j], false);
				}
				this.m_queue[MusicManager.kMusicChannelTypeQueueOrder[i]].Clear();
			}
			this.SetCurrentChannelType();
		}

		// Token: 0x0600652D RID: 25901 RVA: 0x00084190 File Offset: 0x00082390
		private void SceneCompositionManagerOnZoneLoadStarted(ZoneId obj)
		{
			this.m_isLoadingScene = true;
		}

		// Token: 0x0600652E RID: 25902 RVA: 0x0020BF2C File Offset: 0x0020A12C
		public void AddMusic(MusicSetList setList, bool setImmediately = true)
		{
			if (this.m_isLoadingScene)
			{
				if (!this.m_queue[setList.Type].Contains(setList))
				{
					this.m_queue[setList.Type].Add(setList);
				}
				return;
			}
			this.m_channels[setList.Type].AddMusic(setList);
			if (setImmediately)
			{
				this.SetCurrentChannelType();
			}
		}

		// Token: 0x0600652F RID: 25903 RVA: 0x00084199 File Offset: 0x00082399
		public void RemoveMusic(MusicSetList setList, bool setImmediately = true)
		{
			if (this.m_isLoadingScene)
			{
				return;
			}
			this.m_channels[setList.Type].RemoveMusic(setList);
			if (setImmediately)
			{
				this.SetCurrentChannelType();
			}
		}

		// Token: 0x06006530 RID: 25904 RVA: 0x0020BF94 File Offset: 0x0020A194
		private void SetCurrentChannelType()
		{
			for (int i = MusicManager.kMusicChannelTypeQueueOrder.Length - 1; i >= 0; i--)
			{
				if (this.m_channels[MusicManager.kMusicChannelTypeQueueOrder[i]].HasMusic)
				{
					this.CurrentChannelType = MusicManager.kMusicChannelTypeQueueOrder[i];
					return;
				}
			}
		}

		// Token: 0x040057EC RID: 22508
		private static readonly MusicChannelType[] kMusicChannelTypeQueueOrder = new MusicChannelType[]
		{
			MusicChannelType.Scene,
			MusicChannelType.Area,
			MusicChannelType.Contextual
		};

		// Token: 0x040057ED RID: 22509
		private bool m_isLoadingScene;

		// Token: 0x040057EE RID: 22510
		private readonly Dictionary<MusicChannelType, MusicChannel> m_channels = new Dictionary<MusicChannelType, MusicChannel>(default(MusicChannelTypeComparer));

		// Token: 0x040057EF RID: 22511
		private readonly Dictionary<MusicChannelType, List<MusicSetList>> m_queue = new Dictionary<MusicChannelType, List<MusicSetList>>(default(MusicChannelTypeComparer));

		// Token: 0x040057F0 RID: 22512
		private MusicChannelType m_currentChannelType;

		// Token: 0x040057F1 RID: 22513
		private const int kLayeredSourceCount = 5;
	}
}

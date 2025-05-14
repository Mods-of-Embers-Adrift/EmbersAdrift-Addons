using System;
using System.Collections;
using SoL.Game.Audio;
using SoL.Game.Audio.Music;
using SoL.Utilities;
using UnityEngine;
using UnityEngine.Audio;

namespace SoL.Game.Settings
{
	// Token: 0x0200071D RID: 1821
	[Serializable]
	public class AudioSettings
	{
		// Token: 0x060036C0 RID: 14016 RVA: 0x0016A860 File Offset: 0x00168A60
		public bool TryGetTellAudioClip(out AudioClip clip)
		{
			clip = null;
			switch (Options.AudioOptions.AudioOnTellOption.Value)
			{
			case 1:
				clip = this.m_tellAudioScribble;
				break;
			case 2:
				clip = this.m_tellAudioBells;
				break;
			case 3:
				clip = this.m_tellAudioBubble;
				break;
			case 4:
				clip = this.m_tellAudioChimes;
				break;
			}
			return clip != null;
		}

		// Token: 0x17000C25 RID: 3109
		// (get) Token: 0x060036C1 RID: 14017 RVA: 0x00053971 File Offset: 0x00051B71
		private IEnumerable GetClipCollections
		{
			get
			{
				return SolOdinUtilities.GetDropdownItems<AudioClipCollection>();
			}
		}

		// Token: 0x17000C26 RID: 3110
		// (get) Token: 0x060036C2 RID: 14018 RVA: 0x000657C2 File Offset: 0x000639C2
		public GameObject VfxAudioSource
		{
			get
			{
				return this.m_vfxAudioSource;
			}
		}

		// Token: 0x040034C2 RID: 13506
		private const float kMinVolume = 0f;

		// Token: 0x040034C3 RID: 13507
		private const float kMaxVolume = 1f;

		// Token: 0x040034C4 RID: 13508
		[Range(0f, 1f)]
		public float DefaultUIVolume = 0.5f;

		// Token: 0x040034C5 RID: 13509
		public AudioClip LevelUpClip;

		// Token: 0x040034C6 RID: 13510
		[Range(0f, 1f)]
		public float LevelUpVolume = 0.1f;

		// Token: 0x040034C7 RID: 13511
		public AudioClip DefaultClickClip;

		// Token: 0x040034C8 RID: 13512
		[Range(0f, 1f)]
		public float DefaultClickVolume = 0.1f;

		// Token: 0x040034C9 RID: 13513
		public AudioClipCollection MoneyClipCollection;

		// Token: 0x040034CA RID: 13514
		public AudioClipCollection EventCurrencyClipCollection;

		// Token: 0x040034CB RID: 13515
		public AudioClip ScreenshotAudio;

		// Token: 0x040034CC RID: 13516
		public AudioMixer Mixer;

		// Token: 0x040034CD RID: 13517
		public AudioMixerGroup MasterMixerGroup;

		// Token: 0x040034CE RID: 13518
		public AudioMixerGroup SfxParentMixerGroup;

		// Token: 0x040034CF RID: 13519
		public AudioMixerGroup AmbientMixerGroup;

		// Token: 0x040034D0 RID: 13520
		public AudioMixerGroup FootstepMixerGroup;

		// Token: 0x040034D1 RID: 13521
		public AudioMixerGroup UIMixerGroup;

		// Token: 0x040034D2 RID: 13522
		public AudioMixerGroup MusicMixerGroup;

		// Token: 0x040034D3 RID: 13523
		public AudioMixerGroup AuraMixerGroup;

		// Token: 0x040034D4 RID: 13524
		public AudioClipCollection DefaultDragDropClipCollection;

		// Token: 0x040034D5 RID: 13525
		[Range(0f, 1f)]
		public float FootstepVolume = 0.5f;

		// Token: 0x040034D6 RID: 13526
		public float SelfFootstepVolumeReduction = 0.5f;

		// Token: 0x040034D7 RID: 13527
		public float MusicVolumeTransitionSpeed = 1f;

		// Token: 0x040034D8 RID: 13528
		public float MusicLayerTransitionSpeed = 0.2f;

		// Token: 0x040034D9 RID: 13529
		public float TriggerHitAudioEventOnHitChance = 0.25f;

		// Token: 0x040034DA RID: 13530
		public MusicSetList SilenceSetList;

		// Token: 0x040034DB RID: 13531
		[SerializeField]
		private AudioClip m_tellAudioScribble;

		// Token: 0x040034DC RID: 13532
		[SerializeField]
		private AudioClip m_tellAudioBells;

		// Token: 0x040034DD RID: 13533
		[SerializeField]
		private AudioClip m_tellAudioBubble;

		// Token: 0x040034DE RID: 13534
		[SerializeField]
		private AudioClip m_tellAudioChimes;

		// Token: 0x040034DF RID: 13535
		[Range(0f, 1f)]
		public float TellVolume = 0.7f;

		// Token: 0x040034E0 RID: 13536
		public AudioClipCollection GenericDiscoveryClipCollection;

		// Token: 0x040034E1 RID: 13537
		public AudioClip FriendOnlineClip;

		// Token: 0x040034E2 RID: 13538
		public AudioClip FriendOfflineClip;

		// Token: 0x040034E3 RID: 13539
		[Range(0f, 1f)]
		public float FriendVolume = 0.5f;

		// Token: 0x040034E4 RID: 13540
		public AudioClipCollection RepairModeClipCollection;

		// Token: 0x040034E5 RID: 13541
		public AudioClipCollection EmptyLootBagAudioCollection;

		// Token: 0x040034E6 RID: 13542
		[SerializeField]
		private GameObject m_vfxAudioSource;

		// Token: 0x040034E7 RID: 13543
		public AudioClip CannotEquipClip;

		// Token: 0x040034E8 RID: 13544
		public AudioClip TaskCompleteClip;
	}
}

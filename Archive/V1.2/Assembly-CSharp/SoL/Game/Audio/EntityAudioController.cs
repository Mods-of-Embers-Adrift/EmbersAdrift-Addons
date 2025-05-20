using System;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes.Abilities;
using SoL.Game.Settings;
using SoL.Managers;
using SoL.Networking.Database;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Audio
{
	// Token: 0x02000D0A RID: 3338
	public class EntityAudioController : GameEntityComponent
	{
		// Token: 0x17001830 RID: 6192
		// (get) Token: 0x060064C4 RID: 25796 RVA: 0x00083CD7 File Offset: 0x00081ED7
		// (set) Token: 0x060064C5 RID: 25797 RVA: 0x00083CDF File Offset: 0x00081EDF
		private AudioDistortionFilter DistortionFilter { get; set; }

		// Token: 0x17001831 RID: 6193
		// (get) Token: 0x060064C6 RID: 25798 RVA: 0x00083CE8 File Offset: 0x00081EE8
		// (set) Token: 0x060064C7 RID: 25799 RVA: 0x00083CF0 File Offset: 0x00081EF0
		private AudioChorusFilter ChorusFilter { get; set; }

		// Token: 0x17001832 RID: 6194
		// (get) Token: 0x060064C8 RID: 25800 RVA: 0x00083CF9 File Offset: 0x00081EF9
		// (set) Token: 0x060064C9 RID: 25801 RVA: 0x00083D01 File Offset: 0x00081F01
		public bool LoadFemaleOverrides
		{
			get
			{
				return this.m_loadFemaleOverrides;
			}
			set
			{
				this.m_loadFemaleOverrides = value;
				if (this.m_initialized)
				{
					this.LoadFemaleOverrideEvents();
				}
			}
		}

		// Token: 0x060064CA RID: 25802 RVA: 0x00083D18 File Offset: 0x00081F18
		private void Awake()
		{
			if (base.GameEntity != null)
			{
				base.GameEntity.AudioController = this;
			}
			this.m_audioSource.RefreshMixerGroup();
		}

		// Token: 0x060064CB RID: 25803 RVA: 0x0020AA68 File Offset: 0x00208C68
		private void Start()
		{
			if (base.GameEntity != null && base.GameEntity.AudioEventController != null)
			{
				for (int i = 0; i < this.m_audioEvents.Length; i++)
				{
					base.GameEntity.AudioEventController.RegisterEvent(this.m_audioEvents[i]);
				}
				base.GameEntity.AudioEventController.RegisterAudioSource(this.m_audioSource);
				if (this.LoadFemaleOverrides)
				{
					this.LoadFemaleOverrideEvents();
				}
				else if (base.GameEntity.Type == GameEntityType.Player && base.GameEntity.CharacterData.Sex == CharacterSex.Female)
				{
					this.LoadFemaleOverrideEvents();
				}
				if (base.GameEntity.AudioEventController.AddAshenFilters)
				{
					if (GlobalSettings.Values.Ashen.AddDistortionFilter)
					{
						this.DistortionFilter = base.gameObject.AddComponent<AudioDistortionFilter>();
						this.DistortionFilter.distortionLevel = GlobalSettings.Values.Ashen.DistortionLevel;
					}
					if (GlobalSettings.Values.Ashen.AddChorusFilter)
					{
						this.ChorusFilter = base.gameObject.AddComponent<AudioChorusFilter>();
						this.ChorusFilter.rate = GlobalSettings.Values.Ashen.ChorusRate;
						this.ChorusFilter.depth = GlobalSettings.Values.Ashen.ChorusDepth;
					}
				}
			}
			this.m_initialized = true;
		}

		// Token: 0x060064CC RID: 25804 RVA: 0x0020ABC4 File Offset: 0x00208DC4
		private void LoadFemaleOverrideEvents()
		{
			for (int i = 0; i < this.m_femaleOverrides.Length; i++)
			{
				base.GameEntity.AudioEventController.RegisterEvent(this.m_femaleOverrides[i]);
			}
		}

		// Token: 0x060064CD RID: 25805 RVA: 0x0020ABFC File Offset: 0x00208DFC
		public void StartAuraAudio(AuraAbility auraAbility)
		{
			if (!this.m_auraAudioSource || !auraAbility || !auraAbility.LoopingClip)
			{
				return;
			}
			this.m_musicSilenced = auraAbility.SilenceMusic;
			if (this.m_musicSilenced)
			{
				ClientGameManager.MusicManager.AddMusic(GlobalSettings.Values.Audio.SilenceSetList, true);
			}
			this.m_auraAudioSource.clip = auraAbility.LoopingClip;
			this.m_auraAudioSource.volume = auraAbility.ClipVolume;
			this.m_auraAudioSource.loop = true;
			this.m_auraAudioSource.maxDistance = 55f;
			this.m_auraAudioSource.enabled = true;
			this.m_auraAudioSource.dopplerLevel = 0f;
			this.m_auraAudioSource.Play();
		}

		// Token: 0x060064CE RID: 25806 RVA: 0x0020ACC0 File Offset: 0x00208EC0
		public void StopAuraAudio()
		{
			if (!this.m_auraAudioSource || !this.m_auraAudioSource.enabled)
			{
				return;
			}
			if (this.m_musicSilenced)
			{
				ClientGameManager.MusicManager.RemoveMusic(GlobalSettings.Values.Audio.SilenceSetList, true);
				this.m_musicSilenced = false;
			}
			this.m_auraAudioSource.Stop();
			this.m_auraAudioSource.enabled = false;
		}

		// Token: 0x04005779 RID: 22393
		[SerializeField]
		private AudioSource m_audioSource;

		// Token: 0x0400577A RID: 22394
		[SerializeField]
		private MinMaxFloatRange m_pitchShift = new MinMaxFloatRange(1f, 1f);

		// Token: 0x0400577B RID: 22395
		[SerializeField]
		private AudioEvent[] m_audioEvents;

		// Token: 0x0400577C RID: 22396
		[SerializeField]
		private AudioEvent[] m_femaleOverrides;

		// Token: 0x0400577D RID: 22397
		[SerializeField]
		private AudioSource m_auraAudioSource;

		// Token: 0x04005780 RID: 22400
		private bool m_initialized;

		// Token: 0x04005781 RID: 22401
		private bool m_loadFemaleOverrides;

		// Token: 0x04005782 RID: 22402
		private bool m_musicSilenced;
	}
}

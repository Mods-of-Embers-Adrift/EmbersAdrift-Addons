using System;
using System.Collections.Generic;
using SoL.Game.Settings;
using UnityEngine;
using UnityEngine.Audio;

namespace SoL.Utilities.Extensions
{
	// Token: 0x02000326 RID: 806
	public static class AudioExtensions
	{
		// Token: 0x06001641 RID: 5697 RVA: 0x000FF704 File Offset: 0x000FD904
		public static void ConfigureAudioSourceForUI(this AudioSource source)
		{
			source.spatialBlend = 0f;
			source.dopplerLevel = 0f;
			source.playOnAwake = false;
			source.loop = false;
			source.bypassEffects = true;
			source.bypassReverbZones = true;
			source.bypassListenerEffects = true;
			source.volume = GlobalSettings.Values.Audio.DefaultUIVolume;
			source.outputAudioMixerGroup = GlobalSettings.Values.Audio.UIMixerGroup;
		}

		// Token: 0x06001642 RID: 5698 RVA: 0x000FF774 File Offset: 0x000FD974
		public static void ConfigureAudioSourceForAmbient(this AudioSource source)
		{
			source.volume = 0f;
			source.spatialBlend = 0.4f;
			source.dopplerLevel = 0f;
			source.playOnAwake = false;
			source.loop = false;
			source.minDistance = 1f;
			source.maxDistance = 100f;
			AnimationCurve customCurve = source.GetCustomCurve(AudioSourceCurveType.CustomRolloff);
			Keyframe keyframe = customCurve.keys[customCurve.length - 1];
			keyframe.value = 0f;
			customCurve.keys[customCurve.length - 1] = keyframe;
			source.outputAudioMixerGroup = GlobalSettings.Values.Audio.AmbientMixerGroup;
		}

		// Token: 0x06001643 RID: 5699 RVA: 0x000FF818 File Offset: 0x000FDA18
		public static void ConfigureAudioSourceForMusic(this AudioSource source)
		{
			source.mute = false;
			source.bypassEffects = true;
			source.bypassListenerEffects = true;
			source.bypassReverbZones = true;
			source.playOnAwake = false;
			source.loop = false;
			source.priority = 50;
			source.volume = 0.8f;
			source.pitch = 1f;
			source.panStereo = 0f;
			source.spatialBlend = 0f;
			source.outputAudioMixerGroup = GlobalSettings.Values.Audio.MusicMixerGroup;
		}

		// Token: 0x17000551 RID: 1361
		// (get) Token: 0x06001644 RID: 5700 RVA: 0x000FF898 File Offset: 0x000FDA98
		private static Dictionary<int, AudioMixerGroup> MixerGroups
		{
			get
			{
				if (AudioExtensions.m_mixerGroups == null)
				{
					AudioMixerGroup[] array = GlobalSettings.Values.Audio.Mixer.FindMatchingGroups(string.Empty);
					AudioExtensions.m_mixerGroups = new Dictionary<int, AudioMixerGroup>(array.Length);
					for (int i = 0; i < array.Length; i++)
					{
						AudioExtensions.m_mixerGroups.Add(array[i].name.GetHashCode(), array[i]);
					}
				}
				return AudioExtensions.m_mixerGroups;
			}
		}

		// Token: 0x06001645 RID: 5701 RVA: 0x000FF900 File Offset: 0x000FDB00
		public static void RefreshMixerGroup(this AudioSource source)
		{
			if (!source || !source.outputAudioMixerGroup)
			{
				return;
			}
			AudioMixerGroup outputAudioMixerGroup;
			if (AudioExtensions.MixerGroups.TryGetValue(source.outputAudioMixerGroup.name.GetHashCode(), out outputAudioMixerGroup))
			{
				source.outputAudioMixerGroup = outputAudioMixerGroup;
			}
		}

		// Token: 0x06001646 RID: 5702 RVA: 0x000FF948 File Offset: 0x000FDB48
		public static void MoveMusicVolumeTowards(this AudioSource source, float targetVolume, bool isLayer)
		{
			if (!source)
			{
				throw new ArgumentNullException("source");
			}
			float transitionSpeed = isLayer ? GlobalSettings.Values.Audio.MusicLayerTransitionSpeed : GlobalSettings.Values.Audio.MusicVolumeTransitionSpeed;
			source.MoveVolumeTowards(targetVolume, transitionSpeed);
		}

		// Token: 0x06001647 RID: 5703 RVA: 0x000518C4 File Offset: 0x0004FAC4
		public static void MoveVolumeTowards(this AudioSource source, float targetVolume, float transitionSpeed)
		{
			if (!source)
			{
				throw new ArgumentNullException("source");
			}
			source.volume = Mathf.MoveTowards(source.volume, targetVolume, Time.deltaTime * transitionSpeed);
		}

		// Token: 0x04001E45 RID: 7749
		private static Dictionary<int, AudioMixerGroup> m_mixerGroups;
	}
}

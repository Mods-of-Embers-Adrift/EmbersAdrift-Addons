using System;
using System.Collections.Generic;
using SoL.Game.Pooling;
using SoL.Game.Settings;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Audio
{
	// Token: 0x02000D0D RID: 3341
	public class FootstepAudioController : GameEntityComponent
	{
		// Token: 0x060064D6 RID: 25814 RVA: 0x00083DCA File Offset: 0x00081FCA
		private void Awake()
		{
			this.m_audioSource.RefreshMixerGroup();
		}

		// Token: 0x060064D7 RID: 25815 RVA: 0x0020AEE0 File Offset: 0x002090E0
		private void Start()
		{
			this.m_currentSurface = this.m_defaultSurface;
			this.m_audioSource.volume = this.m_audioSourceVolumeScale * GlobalSettings.Values.Audio.FootstepVolume;
			this.m_audioSource.spatialBlend = 1f;
			this.m_audioSource.playOnAwake = false;
			this.m_audioSource.loop = false;
			this.UpdateDefaultAudioVolume();
			if (FootstepAudioController.m_groundSurfaceTypes == null)
			{
				FootstepAudioController.m_groundSurfaceTypes = new Dictionary<string, GroundSurfaceType>();
				GroundSurfaceType[] array = (GroundSurfaceType[])Enum.GetValues(typeof(GroundSurfaceType));
				for (int i = 0; i < array.Length; i++)
				{
					FootstepAudioController.m_groundSurfaceTypes.Add(array[i].ToString(), array[i]);
				}
			}
			this.AssignGroundSampler();
			if (base.GameEntity != null)
			{
				this.m_vitals = base.GameEntity.Vitals;
				if (base.GameEntity.AudioEventController != null)
				{
					if (!string.IsNullOrEmpty(this.m_walk.EventName))
					{
						this.m_walk.Init(this);
						base.GameEntity.AudioEventController.RegisterEvent(this.m_walk);
					}
					if (!string.IsNullOrEmpty(this.m_run.EventName))
					{
						this.m_run.Init(this);
						base.GameEntity.AudioEventController.RegisterEvent(this.m_run);
					}
					base.GameEntity.AudioEventController.RegisterAudioSource(this.m_audioSource);
				}
				if (base.GameEntity.NetworkEntity != null)
				{
					if (base.GameEntity.NetworkEntity.IsInitialized)
					{
						this.UpdateLocalAudio();
						return;
					}
					base.GameEntity.NetworkEntity.OnStartClient += this.NetworkEntityOnOnStartClient;
				}
			}
		}

		// Token: 0x060064D8 RID: 25816 RVA: 0x00083DD7 File Offset: 0x00081FD7
		private void OnDestroy()
		{
			FootstepAudioController.LocomotionAudioEvent walk = this.m_walk;
			if (walk != null)
			{
				walk.OnDestroy();
			}
			FootstepAudioController.LocomotionAudioEvent run = this.m_run;
			if (run == null)
			{
				return;
			}
			run.OnDestroy();
		}

		// Token: 0x060064D9 RID: 25817 RVA: 0x00083DFA File Offset: 0x00081FFA
		private void AssignGroundSampler()
		{
			if (base.GameEntity)
			{
				this.m_groundSampler = base.GameEntity.GroundSampler;
			}
		}

		// Token: 0x060064DA RID: 25818 RVA: 0x00083E1A File Offset: 0x0008201A
		private void SampleSurface()
		{
			if (Time.time < this.m_timeOfNextSample)
			{
				return;
			}
			this.m_currentSurface = this.GetSurfaceType();
			this.m_timeOfNextSample = Time.time + this.m_sampleRate;
		}

		// Token: 0x060064DB RID: 25819 RVA: 0x00083E48 File Offset: 0x00082048
		private void NetworkEntityOnOnStartClient()
		{
			base.GameEntity.NetworkEntity.OnStartClient -= this.NetworkEntityOnOnStartClient;
			this.UpdateLocalAudio();
		}

		// Token: 0x060064DC RID: 25820 RVA: 0x0020B09C File Offset: 0x0020929C
		private void UpdateLocalAudio()
		{
			if (base.GameEntity.NetworkEntity.IsLocal)
			{
				this.m_sampleRate = 0.5f;
				this.m_audioSource.volume = this.m_audioSourceVolumeScale * GlobalSettings.Values.Audio.FootstepVolume * GlobalSettings.Values.Audio.SelfFootstepVolumeReduction;
				this.UpdateDefaultAudioVolume();
				if (this.m_groundSampler)
				{
					this.m_groundSampler.MarkAsSelf();
				}
			}
		}

		// Token: 0x060064DD RID: 25821 RVA: 0x00083E6C File Offset: 0x0008206C
		private void UpdateDefaultAudioVolume()
		{
			this.m_defaultAudioVolume = this.m_audioSource.volume;
		}

		// Token: 0x060064DE RID: 25822 RVA: 0x0020B118 File Offset: 0x00209318
		private GroundSurfaceType GetSurfaceType()
		{
			if (!this.m_groundSampler)
			{
				return this.m_defaultSurface;
			}
			this.m_groundSampler.TimeLimitedSampleGround();
			GroundSurfaceType result = this.m_defaultSurface;
			if (this.m_groundSampler.IsUnderWater || (this.m_vitals && this.m_vitals.Stance == Stance.Swim))
			{
				result = GroundSurfaceType.Water;
			}
			else if (this.m_groundSampler.LastHit.collider != null)
			{
				if (this.m_groundSampler.LastHit.collider is TerrainCollider)
				{
					TerrainSoundController terrainSoundController;
					if (TerrainSoundController.TryGetTerrainSoundController(this.m_groundSampler.LastHit.collider.gameObject, out terrainSoundController))
					{
						this.m_currentActiveTerrain = terrainSoundController.Terrain;
						int maxSplatIndexForPosition = this.GetMaxSplatIndexForPosition(this.m_groundSampler.SourcePos);
						terrainSoundController.SurfaceTypeDict.TryGetValue(maxSplatIndexForPosition, out result);
					}
				}
				else
				{
					FootstepAudioController.m_groundSurfaceTypes.TryGetValue(this.m_groundSampler.LastHit.collider.tag, out result);
				}
			}
			this.m_currentActiveTerrain = null;
			return result;
		}

		// Token: 0x060064DF RID: 25823 RVA: 0x0020B230 File Offset: 0x00209430
		private int GetMaxSplatIndexForPosition(Vector3 position)
		{
			float num = (position.x - this.m_currentActiveTerrain.transform.position.x) / this.m_currentActiveTerrain.terrainData.size.x;
			float num2 = (position.z - this.m_currentActiveTerrain.transform.position.z) / this.m_currentActiveTerrain.terrainData.size.z;
			int x = (int)(num * (float)this.m_currentActiveTerrain.terrainData.alphamapWidth);
			int y = (int)(num2 * (float)this.m_currentActiveTerrain.terrainData.alphamapHeight);
			float[,,] alphamaps = this.m_currentActiveTerrain.terrainData.GetAlphamaps(x, y, 1, 1);
			float num3 = 0f;
			int result = 0;
			for (int i = 0; i < alphamaps.Length; i++)
			{
				if (alphamaps[0, 0, i] > num3)
				{
					num3 = alphamaps[0, 0, i];
					result = i;
				}
			}
			return result;
		}

		// Token: 0x0400578D RID: 22413
		private const float kSurfaceSampleRate = 1f;

		// Token: 0x0400578E RID: 22414
		private const float kSelfSurfaceSampleRate = 0.5f;

		// Token: 0x0400578F RID: 22415
		private static Dictionary<string, GroundSurfaceType> m_groundSurfaceTypes;

		// Token: 0x04005790 RID: 22416
		private GroundSurfaceType m_currentSurface = GroundSurfaceType.Dirt;

		// Token: 0x04005791 RID: 22417
		private float m_timeOfNextSample;

		// Token: 0x04005792 RID: 22418
		private float m_sampleRate = 1f;

		// Token: 0x04005793 RID: 22419
		[SerializeField]
		private AudioSource m_audioSource;

		// Token: 0x04005794 RID: 22420
		[SerializeField]
		private float m_audioSourceVolumeScale = 1f;

		// Token: 0x04005795 RID: 22421
		[SerializeField]
		private GroundSurfaceType m_defaultSurface = GroundSurfaceType.Dirt;

		// Token: 0x04005796 RID: 22422
		[SerializeField]
		private bool m_allowVfx;

		// Token: 0x04005797 RID: 22423
		[SerializeField]
		private FootstepAudioController.LocomotionAudioEvent m_walk;

		// Token: 0x04005798 RID: 22424
		[SerializeField]
		private FootstepAudioController.LocomotionAudioEvent m_run;

		// Token: 0x04005799 RID: 22425
		private GroundSampler m_groundSampler;

		// Token: 0x0400579A RID: 22426
		private Terrain m_currentActiveTerrain;

		// Token: 0x0400579B RID: 22427
		private float m_defaultAudioVolume;

		// Token: 0x0400579C RID: 22428
		private Vitals m_vitals;

		// Token: 0x02000D0E RID: 3342
		[Serializable]
		private class SurfaceClipCollection
		{
			// Token: 0x0400579D RID: 22429
			public GroundSurfaceType SurfaceType;

			// Token: 0x0400579E RID: 22430
			public AudioClipCollection ClipCollection;

			// Token: 0x0400579F RID: 22431
			public PooledVFX VFX;
		}

		// Token: 0x02000D0F RID: 3343
		[Serializable]
		private class LocomotionAudioEvent : IAudioEvent
		{
			// Token: 0x17001834 RID: 6196
			// (get) Token: 0x060064E2 RID: 25826 RVA: 0x0004479C File Offset: 0x0004299C
			public AudioImpulseFlags ImpulseFlags
			{
				get
				{
					return AudioImpulseFlags.Footstep;
				}
			}

			// Token: 0x17001835 RID: 6197
			// (get) Token: 0x060064E3 RID: 25827 RVA: 0x0006109C File Offset: 0x0005F29C
			public float ImpulseForce
			{
				get
				{
					return 1f;
				}
			}

			// Token: 0x060064E4 RID: 25828 RVA: 0x00083EAB File Offset: 0x000820AB
			public void Init(FootstepAudioController controller)
			{
				this.m_controller = controller;
				this.InitInternal();
			}

			// Token: 0x060064E5 RID: 25829 RVA: 0x00083EBA File Offset: 0x000820BA
			public void OnDestroy()
			{
				this.m_controller = null;
				StaticDictionaryPool<GroundSurfaceType, FootstepAudioController.SurfaceClipCollection, GroundSurfaceTypeComparer>.ReturnToPool(this.m_surfaceDict);
			}

			// Token: 0x060064E6 RID: 25830 RVA: 0x0020B320 File Offset: 0x00209520
			private void InitInternal()
			{
				if (this.m_surfaceDict == null)
				{
					this.m_surfaceDict = StaticDictionaryPool<GroundSurfaceType, FootstepAudioController.SurfaceClipCollection, GroundSurfaceTypeComparer>.GetFromPool();
					for (int i = 0; i < this.m_surfaces.Length; i++)
					{
						this.m_surfaceDict.Add(this.m_surfaces[i].SurfaceType, this.m_surfaces[i]);
					}
				}
			}

			// Token: 0x17001836 RID: 6198
			// (get) Token: 0x060064E7 RID: 25831 RVA: 0x00083ECE File Offset: 0x000820CE
			public string EventName
			{
				get
				{
					return this.m_eventName;
				}
			}

			// Token: 0x060064E8 RID: 25832 RVA: 0x0020B374 File Offset: 0x00209574
			public void Play(float volumeFraction = 1f)
			{
				if (this.m_surfaceDict == null || !this.m_controller || !this.m_controller.m_audioSource || !this.m_controller.m_audioSource.enabled)
				{
					return;
				}
				this.m_controller.SampleSurface();
				FootstepAudioController.SurfaceClipCollection surfaceClipCollection;
				if (this.m_surfaceDict.TryGetValue(this.m_controller.m_currentSurface, out surfaceClipCollection))
				{
					if (surfaceClipCollection.ClipCollection)
					{
						this.m_controller.m_audioSource.volume = this.m_controller.m_defaultAudioVolume * volumeFraction;
						surfaceClipCollection.ClipCollection.PlayRandomClip(this.m_controller.m_audioSource);
					}
					if (this.m_controller.m_allowVfx && surfaceClipCollection.VFX && this.m_stepCount >= this.m_stepCountThreshold)
					{
						surfaceClipCollection.VFX.GetPooledInstance<PooledVFX>().Initialize(this.m_controller.GameEntity, 5f, null);
						this.m_stepCount = -1;
						this.m_stepCountThreshold = FootstepAudioController.LocomotionAudioEvent.kStepCountThresholds[UnityEngine.Random.Range(0, FootstepAudioController.LocomotionAudioEvent.kStepCountThresholds.Length)];
					}
					this.m_stepCount++;
				}
			}

			// Token: 0x040057A0 RID: 22432
			[SerializeField]
			private string m_eventName;

			// Token: 0x040057A1 RID: 22433
			[SerializeField]
			private FootstepAudioController.SurfaceClipCollection[] m_surfaces;

			// Token: 0x040057A2 RID: 22434
			private Dictionary<GroundSurfaceType, FootstepAudioController.SurfaceClipCollection> m_surfaceDict;

			// Token: 0x040057A3 RID: 22435
			private FootstepAudioController m_controller;

			// Token: 0x040057A4 RID: 22436
			private static int[] kStepCountThresholds = new int[]
			{
				3,
				5
			};

			// Token: 0x040057A5 RID: 22437
			private int m_stepCountThreshold = FootstepAudioController.LocomotionAudioEvent.kStepCountThresholds[0];

			// Token: 0x040057A6 RID: 22438
			private int m_stepCount;
		}
	}
}

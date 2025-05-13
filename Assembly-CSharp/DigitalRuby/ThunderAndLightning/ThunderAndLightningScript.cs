using System;
using System.Collections;
using UnityEngine;

namespace DigitalRuby.ThunderAndLightning
{
	// Token: 0x020000DE RID: 222
	public class ThunderAndLightningScript : MonoBehaviour
	{
		// Token: 0x060007F1 RID: 2033 RVA: 0x000AFE8C File Offset: 0x000AE08C
		private void Start()
		{
			this.EnableLightning = true;
			if (this.Camera == null)
			{
				this.Camera = Camera.main;
			}
			if (RenderSettings.skybox != null)
			{
				this.skyboxMaterial = (RenderSettings.skybox = new Material(RenderSettings.skybox));
			}
			this.skyboxExposureOriginal = (this.skyboxExposureStorm = ((this.skyboxMaterial == null || !this.skyboxMaterial.HasProperty("_Exposure")) ? 1f : this.skyboxMaterial.GetFloat("_Exposure")));
			this.audioSourceThunder = base.gameObject.AddComponent<AudioSource>();
			this.lightningBoltHandler = new ThunderAndLightningScript.LightningBoltHandler(this);
			this.lightningBoltHandler.VolumeMultiplier = this.VolumeMultiplier;
		}

		// Token: 0x060007F2 RID: 2034 RVA: 0x0004851C File Offset: 0x0004671C
		private void Update()
		{
			if (this.lightningBoltHandler != null && this.EnableLightning)
			{
				this.lightningBoltHandler.VolumeMultiplier = this.VolumeMultiplier;
				this.lightningBoltHandler.Update();
			}
		}

		// Token: 0x060007F3 RID: 2035 RVA: 0x000AFF50 File Offset: 0x000AE150
		public void CallNormalLightning()
		{
			this.CallNormalLightning(null, null);
		}

		// Token: 0x060007F4 RID: 2036 RVA: 0x0004854A File Offset: 0x0004674A
		public void CallNormalLightning(Vector3? start, Vector3? end)
		{
			base.StartCoroutine(this.lightningBoltHandler.ProcessLightning(start, end, false, true));
		}

		// Token: 0x060007F5 RID: 2037 RVA: 0x000AFF78 File Offset: 0x000AE178
		public void CallIntenseLightning()
		{
			this.CallIntenseLightning(null, null);
		}

		// Token: 0x060007F6 RID: 2038 RVA: 0x00048562 File Offset: 0x00046762
		public void CallIntenseLightning(Vector3? start, Vector3? end)
		{
			base.StartCoroutine(this.lightningBoltHandler.ProcessLightning(start, end, true, true));
		}

		// Token: 0x170002AE RID: 686
		// (get) Token: 0x060007F7 RID: 2039 RVA: 0x0004857A File Offset: 0x0004677A
		public float SkyboxExposureOriginal
		{
			get
			{
				return this.skyboxExposureOriginal;
			}
		}

		// Token: 0x170002AF RID: 687
		// (get) Token: 0x060007F8 RID: 2040 RVA: 0x00048582 File Offset: 0x00046782
		// (set) Token: 0x060007F9 RID: 2041 RVA: 0x0004858A File Offset: 0x0004678A
		public bool EnableLightning { get; set; }

		// Token: 0x04000937 RID: 2359
		[Tooltip("Lightning bolt script - optional, leave null if you don't want lightning bolts")]
		public LightningBoltPrefabScript LightningBoltScript;

		// Token: 0x04000938 RID: 2360
		[Tooltip("Camera where the lightning should be centered over. Defaults to main camera.")]
		public Camera Camera;

		// Token: 0x04000939 RID: 2361
		[SingleLine("Random interval between strikes.")]
		public RangeOfFloats LightningIntervalTimeRange = new RangeOfFloats
		{
			Minimum = 10f,
			Maximum = 25f
		};

		// Token: 0x0400093A RID: 2362
		[Tooltip("Probability (0-1) of an intense lightning bolt that hits really close. Intense lightning has increased brightness and louder thunder compared to normal lightning, and the thunder sounds plays a lot sooner.")]
		[Range(0f, 1f)]
		public float LightningIntenseProbability = 0.2f;

		// Token: 0x0400093B RID: 2363
		[Tooltip("Sounds to play for normal thunder. One will be chosen at random for each lightning strike. Depending on intensity, some normal lightning may not play a thunder sound.")]
		public AudioClip[] ThunderSoundsNormal;

		// Token: 0x0400093C RID: 2364
		[Tooltip("Sounds to play for intense thunder. One will be chosen at random for each lightning strike.")]
		public AudioClip[] ThunderSoundsIntense;

		// Token: 0x0400093D RID: 2365
		[Tooltip("Whether lightning strikes should always try to be in the camera view")]
		public bool LightningAlwaysVisible = true;

		// Token: 0x0400093E RID: 2366
		[Tooltip("The chance lightning will simply be in the clouds with no visible bolt")]
		[Range(0f, 1f)]
		public float CloudLightningChance = 0.5f;

		// Token: 0x0400093F RID: 2367
		[Tooltip("Whether to modify the skybox exposure when lightning is created")]
		public bool ModifySkyboxExposure;

		// Token: 0x04000940 RID: 2368
		[Tooltip("Base point light range for lightning bolts. Increases as intensity increases.")]
		[Range(1f, 10000f)]
		public float BaseLightRange = 2000f;

		// Token: 0x04000941 RID: 2369
		[Tooltip("Starting y value for the lightning strikes")]
		[Range(0f, 100000f)]
		public float LightningYStart = 500f;

		// Token: 0x04000942 RID: 2370
		[Tooltip("Volume multiplier")]
		[Range(0f, 1f)]
		public float VolumeMultiplier = 1f;

		// Token: 0x04000943 RID: 2371
		private float skyboxExposureOriginal;

		// Token: 0x04000944 RID: 2372
		private float skyboxExposureStorm;

		// Token: 0x04000945 RID: 2373
		private float nextLightningTime;

		// Token: 0x04000946 RID: 2374
		private bool lightningInProgress;

		// Token: 0x04000947 RID: 2375
		private AudioSource audioSourceThunder;

		// Token: 0x04000948 RID: 2376
		private ThunderAndLightningScript.LightningBoltHandler lightningBoltHandler;

		// Token: 0x04000949 RID: 2377
		private Material skyboxMaterial;

		// Token: 0x0400094A RID: 2378
		private AudioClip lastThunderSound;

		// Token: 0x020000DF RID: 223
		private class LightningBoltHandler
		{
			// Token: 0x170002B0 RID: 688
			// (get) Token: 0x060007FB RID: 2043 RVA: 0x00048593 File Offset: 0x00046793
			// (set) Token: 0x060007FC RID: 2044 RVA: 0x0004859B File Offset: 0x0004679B
			public float VolumeMultiplier { get; set; }

			// Token: 0x060007FD RID: 2045 RVA: 0x000485A4 File Offset: 0x000467A4
			public LightningBoltHandler(ThunderAndLightningScript script)
			{
				this.script = script;
				this.CalculateNextLightningTime();
			}

			// Token: 0x060007FE RID: 2046 RVA: 0x000B0018 File Offset: 0x000AE218
			private void UpdateLighting()
			{
				if (this.script.lightningInProgress)
				{
					return;
				}
				if (this.script.ModifySkyboxExposure)
				{
					this.script.skyboxExposureStorm = 0.35f;
					if (this.script.skyboxMaterial != null && this.script.skyboxMaterial.HasProperty("_Exposure"))
					{
						this.script.skyboxMaterial.SetFloat("_Exposure", this.script.skyboxExposureStorm);
					}
				}
				this.CheckForLightning();
			}

			// Token: 0x060007FF RID: 2047 RVA: 0x000B00A0 File Offset: 0x000AE2A0
			private void CalculateNextLightningTime()
			{
				this.script.nextLightningTime = DigitalRuby.ThunderAndLightning.LightningBoltScript.TimeSinceStart + this.script.LightningIntervalTimeRange.Random(this.random);
				this.script.lightningInProgress = false;
				if (this.script.ModifySkyboxExposure && this.script.skyboxMaterial.HasProperty("_Exposure"))
				{
					this.script.skyboxMaterial.SetFloat("_Exposure", this.script.skyboxExposureStorm);
				}
			}

			// Token: 0x06000800 RID: 2048 RVA: 0x000485C4 File Offset: 0x000467C4
			public IEnumerator ProcessLightning(Vector3? _start, Vector3? _end, bool intense, bool visible)
			{
				this.script.lightningInProgress = true;
				float intensity;
				float time;
				AudioClip[] sounds;
				if (intense)
				{
					float t = UnityEngine.Random.Range(0f, 1f);
					intensity = Mathf.Lerp(2f, 8f, t);
					time = 5f / intensity;
					sounds = this.script.ThunderSoundsIntense;
				}
				else
				{
					float t2 = UnityEngine.Random.Range(0f, 1f);
					intensity = Mathf.Lerp(0f, 2f, t2);
					time = 30f / intensity;
					sounds = this.script.ThunderSoundsNormal;
				}
				if (this.script.skyboxMaterial != null && this.script.ModifySkyboxExposure)
				{
					this.script.skyboxMaterial.SetFloat("_Exposure", Mathf.Max(intensity * 0.5f, this.script.skyboxExposureStorm));
				}
				this.Strike(_start, _end, intense, intensity, this.script.Camera, visible ? this.script.Camera : null);
				this.CalculateNextLightningTime();
				if (intensity >= 1f && sounds != null && sounds.Length != 0)
				{
					yield return new WaitForSecondsLightning(time);
					AudioClip audioClip;
					do
					{
						audioClip = sounds[UnityEngine.Random.Range(0, sounds.Length - 1)];
					}
					while (sounds.Length > 1 && audioClip == this.script.lastThunderSound);
					this.script.lastThunderSound = audioClip;
					this.script.audioSourceThunder.PlayOneShot(audioClip, intensity * 0.5f * this.VolumeMultiplier);
				}
				yield break;
			}

			// Token: 0x06000801 RID: 2049 RVA: 0x000B0124 File Offset: 0x000AE324
			private void Strike(Vector3? _start, Vector3? _end, bool intense, float intensity, Camera camera, Camera visibleInCamera)
			{
				float minInclusive = intense ? -1000f : -5000f;
				float maxInclusive = intense ? 1000f : 5000f;
				float num = intense ? 500f : 2500f;
				float num2 = (UnityEngine.Random.Range(0, 2) == 0) ? UnityEngine.Random.Range(minInclusive, -num) : UnityEngine.Random.Range(num, maxInclusive);
				float y = this.script.LightningYStart;
				float num3 = (UnityEngine.Random.Range(0, 2) == 0) ? UnityEngine.Random.Range(minInclusive, -num) : UnityEngine.Random.Range(num, maxInclusive);
				Vector3 vector = this.script.Camera.transform.position;
				vector.x += num2;
				vector.y = y;
				vector.z += num3;
				if (visibleInCamera != null)
				{
					Quaternion rotation = visibleInCamera.transform.rotation;
					visibleInCamera.transform.rotation = Quaternion.Euler(0f, rotation.eulerAngles.y, 0f);
					float x = UnityEngine.Random.Range((float)visibleInCamera.pixelWidth * 0.1f, (float)visibleInCamera.pixelWidth * 0.9f);
					float z = UnityEngine.Random.Range(visibleInCamera.nearClipPlane + num + num, maxInclusive);
					vector = visibleInCamera.ScreenToWorldPoint(new Vector3(x, 0f, z));
					vector.y = y;
					visibleInCamera.transform.rotation = rotation;
				}
				Vector3 vector2 = vector;
				num2 = UnityEngine.Random.Range(-100f, 100f);
				y = ((UnityEngine.Random.Range(0, 4) == 0) ? UnityEngine.Random.Range(-1f, 600f) : -1f);
				num3 += UnityEngine.Random.Range(-100f, 100f);
				vector2.x += num2;
				vector2.y = y;
				vector2.z += num3;
				vector2.x += num * camera.transform.forward.x;
				vector2.z += num * camera.transform.forward.z;
				while ((vector - vector2).magnitude < 500f)
				{
					vector2.x += num * camera.transform.forward.x;
					vector2.z += num * camera.transform.forward.z;
				}
				vector = (_start ?? vector);
				vector2 = (_end ?? vector2);
				RaycastHit raycastHit;
				if (Physics.Raycast(vector, (vector - vector2).normalized, out raycastHit, 3.4028235E+38f))
				{
					vector2 = raycastHit.point;
				}
				int generations = this.script.LightningBoltScript.Generations;
				RangeOfFloats trunkWidthRange = this.script.LightningBoltScript.TrunkWidthRange;
				if (UnityEngine.Random.value < this.script.CloudLightningChance)
				{
					this.script.LightningBoltScript.TrunkWidthRange = default(RangeOfFloats);
					this.script.LightningBoltScript.Generations = 1;
				}
				this.script.LightningBoltScript.LightParameters.LightIntensity = intensity * 0.5f;
				this.script.LightningBoltScript.Trigger(new Vector3?(vector), new Vector3?(vector2));
				this.script.LightningBoltScript.TrunkWidthRange = trunkWidthRange;
				this.script.LightningBoltScript.Generations = generations;
			}

			// Token: 0x06000802 RID: 2050 RVA: 0x000B049C File Offset: 0x000AE69C
			private void CheckForLightning()
			{
				if (Time.time >= this.script.nextLightningTime)
				{
					bool intense = UnityEngine.Random.value < this.script.LightningIntenseProbability;
					this.script.StartCoroutine(this.ProcessLightning(null, null, intense, this.script.LightningAlwaysVisible));
				}
			}

			// Token: 0x06000803 RID: 2051 RVA: 0x000485F0 File Offset: 0x000467F0
			public void Update()
			{
				this.UpdateLighting();
			}

			// Token: 0x0400094D RID: 2381
			private ThunderAndLightningScript script;

			// Token: 0x0400094E RID: 2382
			private readonly System.Random random = new System.Random();
		}
	}
}

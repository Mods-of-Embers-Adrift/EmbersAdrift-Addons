using System;
using Expanse;
using SoL.Game.Objects;
using SoL.Managers;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace SoL.Game.SkyDome
{
	// Token: 0x020006F8 RID: 1784
	public class ExpanseSkyController : ClientSkyController
	{
		// Token: 0x17000BE2 RID: 3042
		// (get) Token: 0x060035B9 RID: 13753 RVA: 0x00064CCC File Offset: 0x00062ECC
		private bool m_showNightSkyRotation
		{
			get
			{
				return this.m_nightSky != null;
			}
		}

		// Token: 0x060035BA RID: 13754 RVA: 0x001674DC File Offset: 0x001656DC
		private void Awake()
		{
			if (Application.isPlaying && base.enabled)
			{
				if (SceneCompositionManager.IsLoading)
				{
					SceneCompositionManager.SceneCompositionLoaded += this.InitializeAfterLoad;
				}
				else
				{
					this.Initialize();
				}
				if (this.m_initializeTime)
				{
					if (this.m_sun != null && this.m_sun.Light)
					{
						this.m_sun.Light.enabled = false;
					}
					if (this.m_parentPlanet != null && this.m_parentPlanet.Light)
					{
						this.m_parentPlanet.Light.enabled = false;
					}
				}
			}
		}

		// Token: 0x060035BB RID: 13755 RVA: 0x00064CDA File Offset: 0x00062EDA
		protected override void Start()
		{
			base.Start();
			if (Application.isPlaying && !GameManager.IsServer)
			{
				Options.VideoOptions.CloudQuality.Changed += this.CloudQualityOnChanged;
				this.CloudQualityOnChanged();
			}
		}

		// Token: 0x060035BC RID: 13756 RVA: 0x0016757C File Offset: 0x0016577C
		protected override void Update()
		{
			base.Update();
			if (Application.isPlaying && this.m_globalSettings && this.m_reamortizeCloudReflections != null && Time.frameCount >= this.m_reamortizeCloudReflections.Value)
			{
				this.m_globalSettings.m_amortizeCloudReflections = true;
				this.m_reamortizeCloudReflections = null;
			}
			if (this.m_resetSkybox != null && this.m_resetSkybox.Value < Time.time)
			{
				this.m_resetSkybox = null;
				this.ToggleSkybox(true);
			}
		}

		// Token: 0x060035BD RID: 13757 RVA: 0x00064D0C File Offset: 0x00062F0C
		private void OnDestroy()
		{
			if (Application.isPlaying && !GameManager.IsServer)
			{
				Options.VideoOptions.CloudQuality.Changed -= this.CloudQualityOnChanged;
			}
		}

		// Token: 0x060035BE RID: 13758 RVA: 0x00064D32 File Offset: 0x00062F32
		private void InitializeAfterLoad()
		{
			SceneCompositionManager.SceneCompositionLoaded -= this.InitializeAfterLoad;
			this.Initialize();
		}

		// Token: 0x060035BF RID: 13759 RVA: 0x00064D4B File Offset: 0x00062F4B
		private void AmortizeCloudReflectionsOnChanged()
		{
			if (this.m_globalSettings && this.m_reamortizeCloudReflections == null)
			{
				this.m_globalSettings.m_amortizeCloudReflections = Options.VideoOptions.AmortizeCloudReflections.Value;
			}
		}

		// Token: 0x060035C0 RID: 13760 RVA: 0x0016760C File Offset: 0x0016580C
		private void CloudQualityOnChanged()
		{
			if (this.m_cloudVolumes != null)
			{
				for (int i = 0; i < this.m_cloudVolumes.Length; i++)
				{
					ExpanseSkyController.InternalCloudVolume internalCloudVolume = this.m_cloudVolumes[i];
					if (internalCloudVolume != null)
					{
						internalCloudVolume.CloudQualityOnChanged();
					}
				}
			}
		}

		// Token: 0x060035C1 RID: 13761 RVA: 0x00064D7C File Offset: 0x00062F7C
		private void TriggerAmortizeCloudReflectionsDelay()
		{
			if (this.m_globalSettings && Application.isPlaying)
			{
				this.m_globalSettings.m_amortizeCloudReflections = false;
				this.m_reamortizeCloudReflections = new int?(Time.frameCount + this.m_disableAmortizationFrameCount);
			}
		}

		// Token: 0x060035C2 RID: 13762 RVA: 0x00064DB5 File Offset: 0x00062FB5
		protected override WindZone GetWindZone()
		{
			return this.m_windZone;
		}

		// Token: 0x060035C3 RID: 13763 RVA: 0x00167648 File Offset: 0x00165848
		protected override Color GetLightColor()
		{
			Light activeLight = this.GetActiveLight();
			if (!(activeLight != null))
			{
				return Color.white;
			}
			return activeLight.color;
		}

		// Token: 0x060035C4 RID: 13764 RVA: 0x00167674 File Offset: 0x00165874
		protected override float GetLightTemperature()
		{
			Light activeLight = this.GetActiveLight();
			if (!(activeLight != null))
			{
				return 6500f;
			}
			return activeLight.colorTemperature;
		}

		// Token: 0x060035C5 RID: 13765 RVA: 0x00064DBD File Offset: 0x00062FBD
		protected override Light GetActiveLight()
		{
			if (!base.IsDay)
			{
				ExpanseSkyController.InternalCelestialBody parentPlanet = this.m_parentPlanet;
				if (parentPlanet == null)
				{
					return null;
				}
				return parentPlanet.Light;
			}
			else
			{
				ExpanseSkyController.InternalCelestialBody sun = this.m_sun;
				if (sun == null)
				{
					return null;
				}
				return sun.Light;
			}
		}

		// Token: 0x060035C6 RID: 13766 RVA: 0x001676A0 File Offset: 0x001658A0
		protected override void ValidateLighting()
		{
			base.ValidateLighting();
			if (base.IsDay)
			{
				ExpanseSkyController.InternalCelestialBody parentPlanet = this.m_parentPlanet;
				if (parentPlanet != null)
				{
					parentPlanet.ToggleShadowsAndVolumetrics(false);
				}
				ExpanseSkyController.InternalCelestialBody sun = this.m_sun;
				if (sun == null)
				{
					return;
				}
				sun.ToggleShadowsAndVolumetrics(true);
				return;
			}
			else
			{
				ExpanseSkyController.InternalCelestialBody sun2 = this.m_sun;
				if (sun2 != null)
				{
					sun2.ToggleShadowsAndVolumetrics(false);
				}
				ExpanseSkyController.InternalCelestialBody parentPlanet2 = this.m_parentPlanet;
				if (parentPlanet2 == null)
				{
					return;
				}
				parentPlanet2.ToggleShadowsAndVolumetrics(true);
				return;
			}
		}

		// Token: 0x060035C7 RID: 13767 RVA: 0x00064DEA File Offset: 0x00062FEA
		protected override void SetTime(DateTime time, bool updateReflections)
		{
			base.SetTime(time, updateReflections);
			if (updateReflections)
			{
				this.TriggerAmortizeCloudReflectionsDelay();
			}
		}

		// Token: 0x060035C8 RID: 13768 RVA: 0x00167704 File Offset: 0x00165904
		protected override void UpdateCelestialsInternal(double sunZenithAngle, double sunAzimuthAngle)
		{
			base.UpdateCelestialsInternal(sunZenithAngle, sunAzimuthAngle);
			bool isPlaying = Application.isPlaying;
			if (this.m_sun != null && this.m_sun.GameObj)
			{
				this.m_sun.GameObj.transform.eulerAngles = new Vector3((float)sunZenithAngle, (float)sunAzimuthAngle + 180f, 0f);
			}
			if (this.m_parentPlanet != null && this.m_parentPlanet.GameObj)
			{
				int num = SkyDomeUtilities.Latitude - 33;
				int num2 = 23 + num;
				this.m_parentPlanet.GameObj.transform.eulerAngles = new Vector3((float)num2, 180f, 0f);
			}
			ExpanseSkyController.InternalCelestialBody sun = this.m_sun;
			if (sun != null)
			{
				sun.UpdateInternal(base.SunAltitude, isPlaying, true);
			}
			ExpanseSkyController.InternalCelestialBody parentPlanet = this.m_parentPlanet;
			if (parentPlanet != null)
			{
				parentPlanet.UpdateInternal(base.SunAltitude, isPlaying, true);
			}
			float? num3 = null;
			float? num4 = null;
			for (int i = 0; i < this.m_celestialBodies.Length; i++)
			{
				this.m_celestialBodies[i].UpdateInternal(base.SunAltitude, isPlaying, true);
				if (this.m_celestialBodies[i].EarthMoonOrbit && this.m_celestialBodies[i].GameObjIsActive)
				{
					if (num3 == null)
					{
						double num5;
						double num6;
						SkyDomeUtilities.CalculateMoonPositionEnv(base.D, base.Ecl, (float)SkyDomeUtilities.Latitude, out num5, out num6);
						num3 = new float?((float)num5);
						num4 = new float?((float)num6);
					}
					this.m_celestialBodies[i].GameObj.transform.eulerAngles = new Vector3(num4.Value, num3.Value + 180f + this.m_celestialBodies[i].EarthMoonOrbitOffset, 0f);
				}
			}
			if (this.m_nightSky)
			{
				double totalDaysUtc = this.GetTotalDaysUtc();
				double num7 = totalDaysUtc * (double)this.m_nightSkyRotationSpeed.x * 360.0 % 360.0;
				double num8 = totalDaysUtc * (double)this.m_nightSkyRotationSpeed.y * 360.0 % 360.0;
				double num9 = totalDaysUtc * (double)this.m_nightSkyRotationSpeed.z * 360.0 % 360.0;
				Vector3 euler = new Vector3((float)num7, (float)num8 - 180f, (float)num9);
				this.m_nightSky.rotation = Quaternion.Euler(euler);
			}
		}

		// Token: 0x060035C9 RID: 13769 RVA: 0x00064DFD File Offset: 0x00062FFD
		private double GetTotalDaysUtc()
		{
			return SkyDomeUtilities.DaysSinceEpoch(this.m_internalDateTime.DateTime);
		}

		// Token: 0x060035CA RID: 13770 RVA: 0x00167970 File Offset: 0x00165B70
		protected override void UpdateIsDayInternal()
		{
			base.UpdateIsDayInternal();
			bool isDay = base.IsDay;
			base.IsDay = (base.SunAltitude > 0f || (this.m_sunLightControl != null && this.m_sunLightControl.AboveHorizon()));
			if (isDay != base.IsDay)
			{
				this.TriggerAmortizeCloudReflectionsDelay();
			}
		}

		// Token: 0x060035CB RID: 13771 RVA: 0x001679CC File Offset: 0x00165BCC
		protected override void SetCloudHeight()
		{
			if (this.m_cloudVolumes != null)
			{
				for (int i = 0; i < this.m_cloudVolumes.Length; i++)
				{
					ExpanseSkyController.InternalCloudVolume internalCloudVolume = this.m_cloudVolumes[i];
					if (internalCloudVolume != null)
					{
						internalCloudVolume.SetCloudHeight();
					}
				}
			}
		}

		// Token: 0x060035CC RID: 13772 RVA: 0x00167A08 File Offset: 0x00165C08
		protected override void UpdateClouds()
		{
			if (this.m_cloudVolumes != null)
			{
				Vector2? vector = null;
				if (this.m_windZone)
				{
					float f = this.m_windZone.gameObject.transform.eulerAngles.y % 360f * 0.017453292f;
					vector = new Vector2?(new Vector2(Mathf.Sin(f), Mathf.Cos(f)));
					vector *= -1f;
				}
				for (int i = 0; i < this.m_cloudVolumes.Length; i++)
				{
					ExpanseSkyController.InternalCloudVolume internalCloudVolume = this.m_cloudVolumes[i];
					if (internalCloudVolume != null)
					{
						internalCloudVolume.UpdateClouds(this.m_internalDateTime, vector);
					}
				}
			}
		}

		// Token: 0x060035CD RID: 13773 RVA: 0x00064E0F File Offset: 0x0006300F
		protected override void ResetSkybox()
		{
			base.ResetSkybox();
			if (this.m_resetSkybox == null)
			{
				this.m_resetSkybox = new float?(Time.time + 1f);
				this.ToggleSkybox(false);
			}
		}

		// Token: 0x060035CE RID: 13774 RVA: 0x00064E41 File Offset: 0x00063041
		public void SetMoonShadowMode(ShadowUpdateMode mode)
		{
			if (this.m_parentPlanet != null && this.m_parentPlanet.LightData != null)
			{
				this.m_parentPlanet.LightData.SetShadowUpdateMode(mode);
			}
		}

		// Token: 0x060035CF RID: 13775 RVA: 0x00064E6F File Offset: 0x0006306F
		private void ToggleSkybox(bool isEnabled)
		{
			this.m_sun.CelestialBody.gameObject.transform.parent.gameObject.SetActive(isEnabled);
		}

		// Token: 0x040033BD RID: 13245
		public const float kMinimumCloudCoverage = 0.1f;

		// Token: 0x040033BE RID: 13246
		public const string kCloudCoverageTooltip = "0.3f is 'regular' coverage.\n0.26 is a very light coverage.\n0.34 is almost heavy coverage.";

		// Token: 0x040033BF RID: 13247
		public const float kMinStartingCoverage = 0.26f;

		// Token: 0x040033C0 RID: 13248
		public const float kMaxStartingCoverage = 0.34f;

		// Token: 0x040033C1 RID: 13249
		private const float kSkyOffset = 180f;

		// Token: 0x040033C2 RID: 13250
		internal static float? CloudCoverageOffset;

		// Token: 0x040033C3 RID: 13251
		private int? m_reamortizeCloudReflections;

		// Token: 0x040033C4 RID: 13252
		private float? m_resetSkybox;

		// Token: 0x040033C5 RID: 13253
		[SerializeField]
		private GlobalSettings m_globalSettings;

		// Token: 0x040033C6 RID: 13254
		[SerializeField]
		private LightControl m_sunLightControl;

		// Token: 0x040033C7 RID: 13255
		[SerializeField]
		private WindZone m_windZone;

		// Token: 0x040033C8 RID: 13256
		[SerializeField]
		private ExpanseSkyController.InternalCloudVolume[] m_cloudVolumes;

		// Token: 0x040033C9 RID: 13257
		[SerializeField]
		private int m_disableAmortizationFrameCount = 1;

		// Token: 0x040033CA RID: 13258
		[SerializeField]
		private ExpanseSkyController.InternalCelestialBody m_sun;

		// Token: 0x040033CB RID: 13259
		[SerializeField]
		private ExpanseSkyController.InternalCelestialBody m_parentPlanet;

		// Token: 0x040033CC RID: 13260
		[SerializeField]
		private ExpanseSkyController.InternalCelestialBody[] m_celestialBodies;

		// Token: 0x040033CD RID: 13261
		[SerializeField]
		private Transform m_nightSky;

		// Token: 0x040033CE RID: 13262
		[Tooltip("Rotation speed of night sky along each axis.")]
		[SerializeField]
		private Vector3 m_nightSkyRotationSpeed = new Vector3(0.5f, 0.25f, 0.7f);

		// Token: 0x020006F9 RID: 1785
		[Serializable]
		private class InternalCelestialBody
		{
			// Token: 0x17000BE3 RID: 3043
			// (get) Token: 0x060035D1 RID: 13777 RVA: 0x00064EBF File Offset: 0x000630BF
			public Light Light
			{
				get
				{
					return this.m_light;
				}
			}

			// Token: 0x17000BE4 RID: 3044
			// (get) Token: 0x060035D2 RID: 13778 RVA: 0x00064EC7 File Offset: 0x000630C7
			public HDAdditionalLightData LightData
			{
				get
				{
					return this.m_lightData;
				}
			}

			// Token: 0x17000BE5 RID: 3045
			// (get) Token: 0x060035D3 RID: 13779 RVA: 0x00064ECF File Offset: 0x000630CF
			public GameObject GameObj
			{
				get
				{
					if (!(this.m_celestialBody != null))
					{
						return null;
					}
					return this.m_celestialBody.gameObject;
				}
			}

			// Token: 0x17000BE6 RID: 3046
			// (get) Token: 0x060035D4 RID: 13780 RVA: 0x00064EEC File Offset: 0x000630EC
			public bool EarthMoonOrbit
			{
				get
				{
					return this.GameObjIsActive && this.m_earthMoonOrbit;
				}
			}

			// Token: 0x17000BE7 RID: 3047
			// (get) Token: 0x060035D5 RID: 13781 RVA: 0x00064EFE File Offset: 0x000630FE
			public float EarthMoonOrbitOffset
			{
				get
				{
					return this.m_earthMoonOrbitOffset;
				}
			}

			// Token: 0x17000BE8 RID: 3048
			// (get) Token: 0x060035D6 RID: 13782 RVA: 0x00064F06 File Offset: 0x00063106
			public bool GameObjIsActive
			{
				get
				{
					return this.m_celestialBody && this.m_celestialBody.gameObject && this.m_celestialBody.gameObject.activeInHierarchy;
				}
			}

			// Token: 0x17000BE9 RID: 3049
			// (get) Token: 0x060035D7 RID: 13783 RVA: 0x00064F39 File Offset: 0x00063139
			public CelestialBody CelestialBody
			{
				get
				{
					return this.m_celestialBody;
				}
			}

			// Token: 0x060035D8 RID: 13784 RVA: 0x00167AD8 File Offset: 0x00165CD8
			public void ToggleShadowsAndVolumetrics(bool isOn)
			{
				if (!this.m_light)
				{
					return;
				}
				if (this.m_lightData == null)
				{
					this.m_lightData = this.m_light.gameObject.GetComponent<HDAdditionalLightData>();
				}
				if (this.m_lightData && this.m_lightData.affectsVolumetric != isOn)
				{
					this.m_lightData.affectsVolumetric = isOn;
				}
				if (this.m_light.shadows > LightShadows.None != isOn)
				{
					this.m_light.shadows = (isOn ? LightShadows.Soft : LightShadows.None);
				}
				bool flag = this.m_lightAlwaysOn || isOn;
				if (this.m_light.enabled != flag)
				{
					this.m_light.enabled = flag;
				}
			}

			// Token: 0x060035D9 RID: 13785 RVA: 0x00064F41 File Offset: 0x00063141
			public void UpdateInternal(float sunAltitude, bool updateRotation, bool updateColor)
			{
				if (updateRotation)
				{
					this.UpdateRotation();
				}
				if (updateColor)
				{
					this.UpdateColor(sunAltitude);
				}
			}

			// Token: 0x060035DA RID: 13786 RVA: 0x00167B84 File Offset: 0x00165D84
			private void UpdateRotation()
			{
				if (this.m_rotate && this.GameObjIsActive)
				{
					Vector3 albedoTextureRotation = this.m_celestialBody.m_albedoTextureRotation;
					albedoTextureRotation.y += Time.deltaTime * this.m_rotationSpeed;
					this.m_celestialBody.m_albedoTextureRotation = albedoTextureRotation;
					if (this.m_celestialBody.m_emissive)
					{
						this.m_celestialBody.m_emissionTextureRotation = albedoTextureRotation;
					}
				}
			}

			// Token: 0x060035DB RID: 13787 RVA: 0x00167BEC File Offset: 0x00165DEC
			private void UpdateColor(float sunAltitude)
			{
				if (this.m_lerpColor && this.GameObjIsActive)
				{
					float t = this.m_colorLerp.Evaluate(sunAltitude);
					this.m_celestialBody.m_albedoTint = Color.Lerp(this.m_initialColor, Color.white, t);
				}
			}

			// Token: 0x040033CF RID: 13263
			private HDAdditionalLightData m_lightData;

			// Token: 0x040033D0 RID: 13264
			[SerializeField]
			private Light m_light;

			// Token: 0x040033D1 RID: 13265
			[SerializeField]
			private CelestialBody m_celestialBody;

			// Token: 0x040033D2 RID: 13266
			[SerializeField]
			private bool m_lightAlwaysOn;

			// Token: 0x040033D3 RID: 13267
			[SerializeField]
			private bool m_earthMoonOrbit;

			// Token: 0x040033D4 RID: 13268
			[SerializeField]
			private float m_earthMoonOrbitOffset;

			// Token: 0x040033D5 RID: 13269
			[SerializeField]
			private bool m_rotate;

			// Token: 0x040033D6 RID: 13270
			[SerializeField]
			private float m_rotationSpeed = 1f;

			// Token: 0x040033D7 RID: 13271
			[SerializeField]
			private bool m_lerpColor;

			// Token: 0x040033D8 RID: 13272
			[Tooltip("X-Axis represents the altitude from 0-->360 where anything above 180 is under the horizon.")]
			[SerializeField]
			private AnimationCurve m_colorLerp;

			// Token: 0x040033D9 RID: 13273
			[Tooltip("Night albedo tint color")]
			[SerializeField]
			private Color m_initialColor;
		}

		// Token: 0x020006FA RID: 1786
		[Serializable]
		private class InternalCloudVolume
		{
			// Token: 0x060035DD RID: 13789 RVA: 0x00064F69 File Offset: 0x00063169
			public void CloudQualityOnChanged()
			{
				if (this.m_cloudVolume)
				{
					this.m_cloudVolume.m_quality = (Datatypes.Quality)Options.VideoOptions.CloudQuality.Value;
				}
			}

			// Token: 0x060035DE RID: 13790 RVA: 0x00167C34 File Offset: 0x00165E34
			public void UpdateClouds(BaseSkyController.InternalDateTime internalDateTime, Vector2? windMagnitude)
			{
				if (!this.m_cloudVolume)
				{
					return;
				}
				if (this.m_updateCoverage)
				{
					float num = this.m_cloudCoverageRange.Min;
					float num2 = this.m_cloudCoverageRange.Max;
					float num3;
					float num4;
					if (this.m_useZoneSettingsCoverage && ZoneSettings.SettingsProfile && ZoneSettings.SettingsProfile.TryGetCloudCoverageRange(out num3, out num4))
					{
						num = num3;
						num2 = num4;
					}
					float x = (float)(internalDateTime.DateTime - GameDateTime.kStartingDate).TotalDays;
					float y = (float)((SkyDomeUtilities.Latitude << 8) + SkyDomeUtilities.Longitude);
					float t = Mathf.PerlinNoise(x, y);
					float num5 = Mathf.Lerp(num, num2, t);
					if (ExpanseSkyController.CloudCoverageOffset != null)
					{
						float num6 = Mathf.Lerp(num, num2, Mathf.Abs(ExpanseSkyController.CloudCoverageOffset.Value));
						if (ExpanseSkyController.CloudCoverageOffset.Value < 0f)
						{
							num6 *= -1f;
						}
						num5 = Mathf.Clamp(num5 + num6, num, num2);
					}
					this.m_cloudVolume.m_coverage = num5;
				}
				if (windMagnitude != null)
				{
					if (this.m_originalWind == null)
					{
						this.m_originalWind = new Vector3?(this.m_cloudVolume.m_wind);
					}
					this.m_cloudVolume.m_wind.x = this.m_originalWind.Value.x * windMagnitude.Value.x;
					this.m_cloudVolume.m_wind.z = this.m_originalWind.Value.z * windMagnitude.Value.y;
				}
			}

			// Token: 0x060035DF RID: 13791 RVA: 0x00167DB8 File Offset: 0x00165FB8
			public void SetCloudHeight()
			{
				float y;
				if (this.m_adjustHeight && this.m_cloudVolume && ZoneSettings.SettingsProfile && ZoneSettings.SettingsProfile.TryGetCloudHeight(out y))
				{
					Transform transform = this.m_cloudVolume.gameObject.transform;
					Vector3 position = transform.position;
					position.y = y;
					transform.position = position;
				}
			}

			// Token: 0x040033DA RID: 13274
			[SerializeField]
			private CreativeCloudVolume m_cloudVolume;

			// Token: 0x040033DB RID: 13275
			[Tooltip("0.3f is 'regular' coverage.\n0.26 is a very light coverage.\n0.34 is almost heavy coverage.")]
			[SerializeField]
			private MinMaxFloatRange m_cloudCoverageRange = new MinMaxFloatRange(0.26f, 0.34f);

			// Token: 0x040033DC RID: 13276
			[SerializeField]
			private bool m_updateCoverage;

			// Token: 0x040033DD RID: 13277
			[SerializeField]
			private bool m_useZoneSettingsCoverage;

			// Token: 0x040033DE RID: 13278
			[SerializeField]
			private bool m_adjustHeight;

			// Token: 0x040033DF RID: 13279
			private Vector3? m_originalWind;
		}
	}
}

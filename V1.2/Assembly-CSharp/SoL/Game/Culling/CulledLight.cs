using System;
using SoL.Game.Settings;
using SoL.Managers;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace SoL.Game.Culling
{
	// Token: 0x02000CB7 RID: 3255
	public class CulledLight : CulledObject
	{
		// Token: 0x17001792 RID: 6034
		// (get) Token: 0x060062B0 RID: 25264 RVA: 0x0008272F File Offset: 0x0008092F
		private bool m_showVolumetricCullingDistance
		{
			get
			{
				return this.m_additionalLightData != null && this.m_additionalLightData.affectsVolumetric;
			}
		}

		// Token: 0x17001793 RID: 6035
		// (get) Token: 0x060062B1 RID: 25265 RVA: 0x0008274C File Offset: 0x0008094C
		private bool m_showLightOptions
		{
			get
			{
				return this.m_light != null;
			}
		}

		// Token: 0x17001794 RID: 6036
		// (get) Token: 0x060062B2 RID: 25266 RVA: 0x00045BCA File Offset: 0x00043DCA
		private bool m_showLightFlickerAxis
		{
			get
			{
				return false;
			}
		}

		// Token: 0x060062B3 RID: 25267 RVA: 0x00205644 File Offset: 0x00203844
		private void Awake()
		{
			if (!this.m_light || GameManager.IsServer)
			{
				base.gameObject.SetActive(false);
				return;
			}
			if (this.m_additionalLightData == null)
			{
				this.AssignAdditionalLightData();
			}
			this.m_defaultLightSettings = new CulledLight.DefaultLightSettings(this.m_light, this.m_additionalLightData);
			this.m_targetCulledIntensity = this.m_defaultLightSettings.Intensity;
			this.m_targetShadowStrength = this.m_defaultLightSettings.ShadowStrength;
			this.m_intensityLerpRate = this.m_defaultLightSettings.Intensity * 0.2f;
			this.m_targetIntensity = this.m_defaultLightSettings.Intensity;
			this.m_seed = UnityEngine.Random.Range(0f, 655355f);
			if (this.m_defaultLightSettings.Shadows != LightShadows.None)
			{
				this.m_limitFlags |= CullingFlags.LightShadowLimit;
			}
			if (this.m_additionalLightData)
			{
				this.m_additionalLightData.shadowFadeDistance = this.m_shadowCullingDistance.GetDistance();
				this.m_isVolumetric = this.m_additionalLightData.affectsVolumetric;
			}
		}

		// Token: 0x060062B4 RID: 25268 RVA: 0x0020574C File Offset: 0x0020394C
		protected override void RefreshCullee()
		{
			base.RefreshCullee();
			this.m_targetCulledIntensity = (this.IsCulled() ? 0f : this.m_defaultLightSettings.Intensity);
			this.m_targetShadowStrength = ((this.m_cullingFlags.HasBitFlag(CullingFlags.LightShadowDistance) || this.m_cullingFlags.HasBitFlag(CullingFlags.LightShadowLimit)) ? 0f : this.m_defaultLightSettings.ShadowStrength);
		}

		// Token: 0x060062B5 RID: 25269 RVA: 0x0008275A File Offset: 0x0008095A
		protected override CullingFlags GetUpdatedFlags(CullingFlags currentFlags)
		{
			currentFlags = base.GetUpdatedFlags(currentFlags);
			if (this.m_shadowCullingDistance.ShouldBeCulled(this.m_currentBand))
			{
				currentFlags |= CullingFlags.LightShadowDistance;
			}
			else
			{
				currentFlags &= ~CullingFlags.LightShadowDistance;
			}
			return currentFlags;
		}

		// Token: 0x060062B6 RID: 25270 RVA: 0x002057B4 File Offset: 0x002039B4
		private void Update()
		{
			if (!this.m_light)
			{
				return;
			}
			float num = this.m_targetCulledIntensity;
			if (num > 0f && this.m_flicker != FlickerType.None)
			{
				float num2 = (Mathf.PerlinNoise(this.m_seed, Time.fixedTime * this.m_flicker.GetFlickerSpeed()) - 0.5f) * 2f;
				num += num2 * this.m_flicker.GetFlickerIntensityDelta();
			}
			if (num <= 0f && this.m_light.intensity <= 0f)
			{
				if (this.m_light.enabled)
				{
					this.m_light.enabled = false;
				}
				return;
			}
			if (num > 0f && !this.m_light.enabled)
			{
				this.m_light.intensity = 0f;
				this.m_light.enabled = true;
			}
			if (!Mathf.Approximately(this.m_light.intensity, num))
			{
				this.m_light.intensity = Mathf.MoveTowards(this.m_light.intensity, num, Time.deltaTime * GlobalSettings.Values.Culling.LightIntensityFadeRate * this.m_intensityLerpRate);
			}
			if (this.m_defaultLightSettings.Shadows != LightShadows.None)
			{
				if (this.m_targetShadowStrength <= 0f && this.GetShadowStrength() <= 0f && this.m_light.shadows != LightShadows.None)
				{
					this.m_light.shadows = LightShadows.None;
				}
				else
				{
					if (this.m_targetShadowStrength > 0f && this.m_light.shadows == LightShadows.None)
					{
						this.SetShadowStrength(0f);
						this.m_light.shadows = this.m_defaultLightSettings.Shadows;
					}
					float shadowStrength = this.GetShadowStrength();
					if (!Mathf.Approximately(shadowStrength, this.m_targetShadowStrength))
					{
						float shadowStrength2 = Mathf.MoveTowards(shadowStrength, this.m_targetShadowStrength, Time.deltaTime * GlobalSettings.Values.Culling.LightShadowStrengthFadeRate);
						this.SetShadowStrength(shadowStrength2);
					}
				}
			}
			if (this.m_isVolumetric && this.m_additionalLightData)
			{
				bool flag = this.m_currentBand.GetDistance() <= this.m_volumetricCullingDistance.GetDistance();
				if (this.m_additionalLightData.affectsVolumetric != flag)
				{
					this.m_additionalLightData.affectsVolumetric = flag;
				}
			}
		}

		// Token: 0x060062B7 RID: 25271 RVA: 0x00082786 File Offset: 0x00080986
		private void SetShadowStrength(float value)
		{
			if (this.m_additionalLightData)
			{
				this.m_additionalLightData.shadowDimmer = value;
				return;
			}
			this.m_light.shadowStrength = value;
		}

		// Token: 0x060062B8 RID: 25272 RVA: 0x000827AE File Offset: 0x000809AE
		private float GetShadowStrength()
		{
			if (!this.m_additionalLightData)
			{
				return this.m_light.shadowStrength;
			}
			return this.m_additionalLightData.shadowDimmer;
		}

		// Token: 0x17001795 RID: 6037
		// (get) Token: 0x060062B9 RID: 25273 RVA: 0x000827D4 File Offset: 0x000809D4
		private bool m_showGetLightButton
		{
			get
			{
				return this.m_light == null;
			}
		}

		// Token: 0x17001796 RID: 6038
		// (get) Token: 0x060062BA RID: 25274 RVA: 0x000827E2 File Offset: 0x000809E2
		private bool m_showAssignButton
		{
			get
			{
				return this.m_additionalLightData == null;
			}
		}

		// Token: 0x060062BB RID: 25275 RVA: 0x000827F0 File Offset: 0x000809F0
		private void GetLight()
		{
			if (this.m_light == null)
			{
				this.m_light = base.gameObject.GetComponent<Light>();
			}
		}

		// Token: 0x060062BC RID: 25276 RVA: 0x00082811 File Offset: 0x00080A11
		private void AssignAdditionalLightData()
		{
			HDAdditionalLightData additionalLightData = this.m_additionalLightData;
			this.m_additionalLightData = ((this.m_light == null) ? null : this.m_light.GetComponent<HDAdditionalLightData>());
			this.SetLightFadeDistance();
		}

		// Token: 0x060062BD RID: 25277 RVA: 0x00082842 File Offset: 0x00080A42
		private void SetLightFadeDistance()
		{
			if (!this.m_additionalLightData)
			{
				return;
			}
			float shadowFadeDistance = this.m_additionalLightData.shadowFadeDistance;
			this.m_additionalLightData.shadowFadeDistance = this.m_shadowCullingDistance.GetDistance();
		}

		// Token: 0x060062BE RID: 25278 RVA: 0x00082874 File Offset: 0x00080A74
		private void OnValidate()
		{
			this.SetLightFadeDistance();
		}

		// Token: 0x0400560C RID: 22028
		private const string kGroupName = "Light";

		// Token: 0x0400560D RID: 22029
		[SerializeField]
		private Light m_light;

		// Token: 0x0400560E RID: 22030
		[SerializeField]
		private HDAdditionalLightData m_additionalLightData;

		// Token: 0x0400560F RID: 22031
		[SerializeField]
		private CullingDistance m_volumetricCullingDistance;

		// Token: 0x04005610 RID: 22032
		[SerializeField]
		private CullingDistance m_shadowCullingDistance;

		// Token: 0x04005611 RID: 22033
		[SerializeField]
		private FlickerType m_flicker;

		// Token: 0x04005612 RID: 22034
		[SerializeField]
		private Vector3 m_flickerAxisMultiplier = new Vector3(1f, 0f, 1f);

		// Token: 0x04005613 RID: 22035
		private CulledLight.DefaultLightSettings m_defaultLightSettings;

		// Token: 0x04005614 RID: 22036
		private float m_targetCulledIntensity;

		// Token: 0x04005615 RID: 22037
		private float m_targetShadowStrength;

		// Token: 0x04005616 RID: 22038
		private float m_intensityLerpRate = 100f;

		// Token: 0x04005617 RID: 22039
		private float m_targetIntensity;

		// Token: 0x04005618 RID: 22040
		private float m_seed;

		// Token: 0x04005619 RID: 22041
		private bool m_isVolumetric;

		// Token: 0x02000CB8 RID: 3256
		private struct DefaultLightSettings
		{
			// Token: 0x060062C0 RID: 25280 RVA: 0x002059DC File Offset: 0x00203BDC
			public DefaultLightSettings(Light light, HDAdditionalLightData additionalLightData)
			{
				this.Intensity = light.intensity;
				this.Range = light.range;
				this.Shadows = light.shadows;
				this.ShadowStrength = (additionalLightData ? additionalLightData.shadowDimmer : light.shadowStrength);
				this.LocalPosition = light.gameObject.transform.localPosition;
			}

			// Token: 0x0400561A RID: 22042
			public readonly float Intensity;

			// Token: 0x0400561B RID: 22043
			public readonly float Range;

			// Token: 0x0400561C RID: 22044
			public readonly LightShadows Shadows;

			// Token: 0x0400561D RID: 22045
			public readonly float ShadowStrength;

			// Token: 0x0400561E RID: 22046
			public readonly Vector3 LocalPosition;
		}
	}
}

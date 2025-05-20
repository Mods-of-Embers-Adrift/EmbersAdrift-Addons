using System;
using System.Collections.Generic;
using AwesomeTechnologies.VegetationStudio;
using AwesomeTechnologies.VegetationSystem;
using SoL.Game.Settings;
using SoL.Game.SkyDome;
using SoL.Managers;
using SoL.Utilities;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace SoL.GameCamera
{
	// Token: 0x02000DEB RID: 3563
	public class MainCameraSettings : MonoBehaviour
	{
		// Token: 0x06006A35 RID: 27189 RVA: 0x0021A3E4 File Offset: 0x002185E4
		private void Awake()
		{
			if (GameManager.IsServer || this.m_camera == null)
			{
				return;
			}
			if (this.m_hdCameraData)
			{
				this.m_hdCameraData.customRenderingSettings = true;
				this.m_frameSettingsHelpers = new List<MainCameraSettings.BoolFrameSettingsHelper>
				{
					new MainCameraSettings.BoolFrameSettingsHelper(this.m_hdCameraData, Options.VideoOptions.Reflections, FrameSettingsField.SSR),
					new MainCameraSettings.BoolFrameSettingsHelper(this.m_hdCameraData, Options.VideoOptions.ContactShadows, FrameSettingsField.ContactShadows)
				};
				AntiAliasingTypeInternalExtensions.ValidateAntiAliasingSelection();
				this.AntiAliasingTypeOnChanged();
				Options.VideoOptions.AntiAliasingType.Changed += this.AntiAliasingTypeOnChanged;
				this.m_hdCameraData.renderingPathCustomFrameSettingsOverrideMask.mask[26U] = true;
				this.m_hdCameraData.renderingPathCustomFrameSettingsOverrideMask.mask[20U] = true;
				this.ShadowDistanceOnChanged();
				Options.VideoOptions.ShadowDistance.Changed += this.ShadowDistanceOnChanged;
				if (NvidiaDLSS.SupportsNvidiaDLSS)
				{
					this.NvidiaDLSSQualityOnChanged();
					this.NvidiaDLSSEnableOnChanged();
					Options.VideoOptions.NvidiaDLSSEnable.Changed += this.NvidiaDLSSEnableOnChanged;
					Options.VideoOptions.NvidiaDLSSQuality.Changed += this.NvidiaDLSSQualityOnChanged;
				}
				this.ResolutionScaleOnChanged();
				Options.VideoOptions.ResolutionScale.Changed += this.ResolutionScaleOnChanged;
				this.m_allowRefreshDynamicResolution = true;
				this.RefreshAllowDynamicResolution();
			}
			SkyDomeManager.VSPManagerAssigned += this.ValidateVegetationStudio;
			ClientGameManager.MainCamera = this.m_camera;
			this.InitCullingDistances();
			this.m_audioListener = base.gameObject.GetComponent<AudioListener>();
			MainCameraSettings.Instance = this;
		}

		// Token: 0x06006A36 RID: 27190 RVA: 0x0008727C File Offset: 0x0008547C
		private void Start()
		{
			this.ValidateVegetationStudio();
		}

		// Token: 0x06006A37 RID: 27191 RVA: 0x0021A56C File Offset: 0x0021876C
		private void OnDestroy()
		{
			if (GameManager.IsServer || this.m_camera == null)
			{
				return;
			}
			if (this.m_hdCameraData)
			{
				Options.VideoOptions.AntiAliasingType.Changed -= this.AntiAliasingTypeOnChanged;
				Options.VideoOptions.ShadowDistance.Changed -= this.ShadowDistanceOnChanged;
				for (int i = 0; i < this.m_frameSettingsHelpers.Count; i++)
				{
					this.m_frameSettingsHelpers[i].OnDestroy();
				}
				if (NvidiaDLSS.SupportsNvidiaDLSS)
				{
					Options.VideoOptions.NvidiaDLSSEnable.Changed -= this.NvidiaDLSSEnableOnChanged;
					Options.VideoOptions.NvidiaDLSSQuality.Changed -= this.NvidiaDLSSQualityOnChanged;
				}
				Options.VideoOptions.ResolutionScale.Changed -= this.ResolutionScaleOnChanged;
			}
			SkyDomeManager.VSPManagerAssigned -= this.ValidateVegetationStudio;
		}

		// Token: 0x06006A38 RID: 27192 RVA: 0x00087284 File Offset: 0x00085484
		private void AntiAliasingTypeOnChanged()
		{
			if (!this.m_hdCameraData)
			{
				return;
			}
			this.m_hdCameraData.antialiasing = AntiAliasingTypeInternalExtensions.GetEngineAntiAliasingType();
		}

		// Token: 0x06006A39 RID: 27193 RVA: 0x0021A64C File Offset: 0x0021884C
		private void ShadowDistanceOnChanged()
		{
			if (!this.m_hdCameraData)
			{
				return;
			}
			bool value = Options.VideoOptions.ShadowDistance.Value > 0f;
			this.m_hdCameraData.renderingPathCustomFrameSettings.SetEnabled(FrameSettingsField.ShadowMaps, value);
			this.m_hdCameraData.renderingPathCustomFrameSettings.SetEnabled(FrameSettingsField.Transmission, value);
		}

		// Token: 0x06006A3A RID: 27194 RVA: 0x000872A4 File Offset: 0x000854A4
		private void ResolutionScaleOnChanged()
		{
			this.RefreshAllowDynamicResolution();
		}

		// Token: 0x06006A3B RID: 27195 RVA: 0x000872AC File Offset: 0x000854AC
		private void NvidiaDLSSQualityOnChanged()
		{
			this.m_hdCameraData.deepLearningSuperSamplingUseCustomQualitySettings = true;
			this.m_hdCameraData.deepLearningSuperSamplingQuality = (uint)Options.VideoOptions.NvidiaDLSSQuality.Value;
		}

		// Token: 0x06006A3C RID: 27196 RVA: 0x000872CF File Offset: 0x000854CF
		private void NvidiaDLSSEnableOnChanged()
		{
			this.m_hdCameraData.deepLearningSuperSamplingUseCustomAttributes = true;
			this.m_hdCameraData.deepLearningSuperSamplingUseOptimalSettings = true;
			this.m_hdCameraData.allowDeepLearningSuperSampling = Options.VideoOptions.NvidiaDLSSEnable.Value;
			this.RefreshAllowDynamicResolution();
		}

		// Token: 0x06006A3D RID: 27197 RVA: 0x00087304 File Offset: 0x00085504
		private void RefreshAllowDynamicResolution()
		{
			if (this.m_allowRefreshDynamicResolution)
			{
				this.m_hdCameraData.allowDynamicResolution = (Options.VideoOptions.ResolutionScale.Value < 1f || (NvidiaDLSS.SupportsNvidiaDLSS && Options.VideoOptions.NvidiaDLSSEnable.Value));
			}
		}

		// Token: 0x06006A3E RID: 27198 RVA: 0x00087341 File Offset: 0x00085541
		private void InitCullingDistances()
		{
			if (GlobalSettings.Values != null && GlobalSettings.Values.Rendering != null)
			{
				GlobalSettings.Values.Rendering.SetLayerCullingDistanceArray(this.m_camera);
			}
		}

		// Token: 0x06006A3F RID: 27199 RVA: 0x0021A6A0 File Offset: 0x002188A0
		private void ValidateVegetationStudio()
		{
			if (VegetationStudioManager.Instance != null)
			{
				List<VegetationSystemPro> vegetationSystemList = VegetationStudioManager.Instance.VegetationSystemList;
				for (int i = 0; i < vegetationSystemList.Count; i++)
				{
					if (vegetationSystemList[i].GetVegetationStudioCamera(ClientGameManager.MainCamera) == null)
					{
						vegetationSystemList[i].AddCamera(ClientGameManager.MainCamera, false, false, false);
					}
				}
			}
		}

		// Token: 0x06006A40 RID: 27200 RVA: 0x0021A700 File Offset: 0x00218900
		public static void ToggleEffectOverride(CameraEffectTypes type, bool isEnabled)
		{
			if (MainCameraSettings.Instance == null)
			{
				return;
			}
			foreach (CameraEffectOverride cameraEffectOverride in MainCameraSettings.Instance.m_effectOverrides)
			{
				if (cameraEffectOverride.Obj)
				{
					cameraEffectOverride.Obj.SetActive(cameraEffectOverride.Type == type && isEnabled);
				}
			}
		}

		// Token: 0x06006A41 RID: 27201 RVA: 0x0021A75C File Offset: 0x0021895C
		public static void DisableEffectOverrides()
		{
			if (MainCameraSettings.Instance == null)
			{
				return;
			}
			foreach (CameraEffectOverride cameraEffectOverride in MainCameraSettings.Instance.m_effectOverrides)
			{
				if (cameraEffectOverride.Obj)
				{
					cameraEffectOverride.Obj.SetActive(false);
				}
			}
		}

		// Token: 0x06006A42 RID: 27202 RVA: 0x00087371 File Offset: 0x00085571
		public static void ToggleAudioListener(bool isEnabled)
		{
			if (MainCameraSettings.Instance == null || MainCameraSettings.Instance.m_audioListener == null)
			{
				return;
			}
			MainCameraSettings.Instance.m_audioListener.enabled = isEnabled;
		}

		// Token: 0x04005C70 RID: 23664
		private List<MainCameraSettings.BoolFrameSettingsHelper> m_frameSettingsHelpers;

		// Token: 0x04005C71 RID: 23665
		private AudioListener m_audioListener;

		// Token: 0x04005C72 RID: 23666
		private bool m_allowRefreshDynamicResolution;

		// Token: 0x04005C73 RID: 23667
		[SerializeField]
		private Camera m_camera;

		// Token: 0x04005C74 RID: 23668
		[SerializeField]
		private HDAdditionalCameraData m_hdCameraData;

		// Token: 0x04005C75 RID: 23669
		[SerializeField]
		private CameraEffectOverride[] m_effectOverrides;

		// Token: 0x04005C76 RID: 23670
		private static MainCameraSettings Instance;

		// Token: 0x02000DEC RID: 3564
		private class BoolFrameSettingsHelper
		{
			// Token: 0x06006A44 RID: 27204 RVA: 0x0021A7B0 File Offset: 0x002189B0
			public BoolFrameSettingsHelper(HDAdditionalCameraData hdCameraData, Options.Option_Boolean option, FrameSettingsField field)
			{
				if (hdCameraData == null)
				{
					throw new ArgumentNullException("hdCameraData");
				}
				if (option == null)
				{
					throw new ArgumentNullException("option");
				}
				if (field == FrameSettingsField.None)
				{
					throw new ArgumentException("field");
				}
				this.m_hdCameraData = hdCameraData;
				this.m_option = option;
				this.m_field = field;
				this.m_hdCameraData.renderingPathCustomFrameSettingsOverrideMask.mask[(uint)this.m_field] = true;
				this.m_option.Changed += this.OptionOnChanged;
				this.OptionOnChanged();
			}

			// Token: 0x06006A45 RID: 27205 RVA: 0x000873A3 File Offset: 0x000855A3
			public void OnDestroy()
			{
				this.m_option.Changed -= this.OptionOnChanged;
			}

			// Token: 0x06006A46 RID: 27206 RVA: 0x000873BC File Offset: 0x000855BC
			private void OptionOnChanged()
			{
				if (!this.m_hdCameraData)
				{
					return;
				}
				this.m_hdCameraData.renderingPathCustomFrameSettings.SetEnabled(this.m_field, this.m_option.Value);
			}

			// Token: 0x04005C77 RID: 23671
			private readonly HDAdditionalCameraData m_hdCameraData;

			// Token: 0x04005C78 RID: 23672
			private readonly Options.Option_Boolean m_option;

			// Token: 0x04005C79 RID: 23673
			private readonly FrameSettingsField m_field;
		}
	}
}

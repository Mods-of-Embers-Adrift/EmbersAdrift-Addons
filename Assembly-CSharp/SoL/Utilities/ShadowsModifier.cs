using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace SoL.Utilities
{
	// Token: 0x020002FF RID: 767
	public class ShadowsModifier : VolumeComponentModifier<HDShadowSettings>
	{
		// Token: 0x0600159A RID: 5530 RVA: 0x000FCD4C File Offset: 0x000FAF4C
		private void Start()
		{
			if (base.Component == null)
			{
				return;
			}
			Options.VideoOptions.QualitySetting.Changed += this.QualitySettingOnChanged;
			Options.VideoOptions.ShadowDistance.Changed += this.ShadowDistanceOnChanged;
			this.RefreshShadowConfig();
		}

		// Token: 0x0600159B RID: 5531 RVA: 0x0005138D File Offset: 0x0004F58D
		private void OnDestroy()
		{
			Options.VideoOptions.QualitySetting.Changed -= this.QualitySettingOnChanged;
			Options.VideoOptions.ShadowDistance.Changed -= this.ShadowDistanceOnChanged;
		}

		// Token: 0x0600159C RID: 5532 RVA: 0x000513BB File Offset: 0x0004F5BB
		private void QualitySettingOnChanged()
		{
			this.RefreshShadowConfig();
		}

		// Token: 0x0600159D RID: 5533 RVA: 0x000513BB File Offset: 0x0004F5BB
		private void ShadowDistanceOnChanged()
		{
			this.RefreshShadowConfig();
		}

		// Token: 0x0600159E RID: 5534 RVA: 0x000FCD9C File Offset: 0x000FAF9C
		private void RefreshShadowConfig()
		{
			if (this.m_shadowProfiles == null || Options.VideoOptions.QualitySetting.Value >= this.m_shadowProfiles.Length)
			{
				return;
			}
			VolumeProfile volumeProfile = this.m_shadowProfiles[Options.VideoOptions.QualitySetting.Value];
			HDShadowSettings hdshadowSettings;
			if (volumeProfile == null || !volumeProfile.TryGet<HDShadowSettings>(out hdshadowSettings))
			{
				return;
			}
			float value = hdshadowSettings.maxShadowDistance.value;
			float num = hdshadowSettings.cascadeShadowSplit0.value * value;
			float num2 = hdshadowSettings.cascadeShadowSplit1.value * value;
			float num3 = hdshadowSettings.cascadeShadowSplit2.value * value;
			float num4 = hdshadowSettings.cascadeShadowBorder0.value * num;
			float num5 = hdshadowSettings.cascadeShadowBorder1.value * num2;
			float num6 = hdshadowSettings.cascadeShadowBorder2.value * num3;
			base.Component.cascadeShadowSplitCount = hdshadowSettings.cascadeShadowSplitCount;
			base.Component.cascadeShadowSplit0.overrideState = hdshadowSettings.cascadeShadowSplit0.overrideState;
			base.Component.cascadeShadowSplit1.overrideState = hdshadowSettings.cascadeShadowSplit1.overrideState;
			base.Component.cascadeShadowSplit2.overrideState = hdshadowSettings.cascadeShadowSplit2.overrideState;
			base.Component.cascadeShadowBorder0.overrideState = hdshadowSettings.cascadeShadowBorder0.overrideState;
			base.Component.cascadeShadowBorder1.overrideState = hdshadowSettings.cascadeShadowBorder1.overrideState;
			base.Component.cascadeShadowBorder2.overrideState = hdshadowSettings.cascadeShadowBorder2.overrideState;
			base.Component.cascadeShadowBorder3.overrideState = hdshadowSettings.cascadeShadowBorder3.overrideState;
			float num7 = Options.VideoOptions.ShadowDistance.Value * 2f;
			float num8 = num + (value - num) * num7;
			base.Component.maxShadowDistance.value = num8;
			base.Component.cascadeShadowSplit0.value = Mathf.Clamp01(Mathf.Min(num, num8) / num8);
			base.Component.cascadeShadowSplit1.value = Mathf.Clamp01(Mathf.Min(num2, num8) / num8);
			base.Component.cascadeShadowSplit2.value = Mathf.Clamp01(Mathf.Min(num3, num8) / num8);
			base.Component.cascadeShadowBorder0.value = Mathf.Clamp01(num4 / num);
			base.Component.cascadeShadowBorder1.value = Mathf.Clamp01(num5 / num2);
			base.Component.cascadeShadowBorder2.value = Mathf.Clamp01(num6 / num3);
		}

		// Token: 0x0600159F RID: 5535 RVA: 0x000FD000 File Offset: 0x000FB200
		public int SetCascadeCount(int value)
		{
			int num = Mathf.Clamp(value, 1, 4);
			base.Component.cascadeShadowSplitCount.value = num;
			return num;
		}

		// Token: 0x04001DA3 RID: 7587
		[SerializeField]
		private VolumeProfile[] m_shadowProfiles;
	}
}

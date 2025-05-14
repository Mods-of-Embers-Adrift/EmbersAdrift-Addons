using System;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace SoL.Game.SkyDome
{
	// Token: 0x02000714 RID: 1812
	public class SunSettings : MonoBehaviour
	{
		// Token: 0x0600366D RID: 13933 RVA: 0x000654AB File Offset: 0x000636AB
		private void Awake()
		{
			if (!this.m_additionalLightData)
			{
				base.enabled = false;
			}
		}

		// Token: 0x0600366E RID: 13934 RVA: 0x00169920 File Offset: 0x00167B20
		private void Start()
		{
			if (this.m_additionalLightData)
			{
				this.m_defaultValue = this.m_additionalLightData.shadowResolution.level;
			}
			Options.VideoOptions.HighResolutionSunShadows.Changed += this.HighResolutionSunShadowsOnChanged;
			this.HighResolutionSunShadowsOnChanged();
		}

		// Token: 0x0600366F RID: 13935 RVA: 0x000654C1 File Offset: 0x000636C1
		private void OnDestroy()
		{
			Options.VideoOptions.HighResolutionSunShadows.Changed -= this.HighResolutionSunShadowsOnChanged;
		}

		// Token: 0x06003670 RID: 13936 RVA: 0x0016996C File Offset: 0x00167B6C
		private void HighResolutionSunShadowsOnChanged()
		{
			if (this.m_additionalLightData)
			{
				int num = this.m_defaultValue;
				if (Options.VideoOptions.HighResolutionSunShadows.Value)
				{
					num++;
				}
				this.m_additionalLightData.SetShadowResolutionLevel(num);
			}
		}

		// Token: 0x0400345A RID: 13402
		[SerializeField]
		private HDAdditionalLightData m_additionalLightData;

		// Token: 0x0400345B RID: 13403
		private int m_defaultValue = 2;
	}
}

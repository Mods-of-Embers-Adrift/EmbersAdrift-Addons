using System;
using SoL.Game.SkyDome;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace SoL.Utilities
{
	// Token: 0x0200029B RID: 667
	public class ModifyLightIntensityForSceneExposure : MonoBehaviour
	{
		// Token: 0x0600142C RID: 5164 RVA: 0x0005028F File Offset: 0x0004E48F
		private void Update()
		{
			if (SkyDomeManager.ExposureController != null)
			{
				this.m_lightData.intensity = this.m_lightIntensity.Evaluate(SkyDomeManager.ExposureController.Exposure);
			}
		}

		// Token: 0x04001C7E RID: 7294
		[Tooltip("a function of scene exposure")]
		[SerializeField]
		private AnimationCurve m_lightIntensity = AnimationCurve.Linear(0f, 250f, 1f, 5000f);

		// Token: 0x04001C7F RID: 7295
		[SerializeField]
		private HDAdditionalLightData m_lightData;
	}
}

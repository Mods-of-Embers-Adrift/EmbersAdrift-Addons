using System;
using SoL.Game.SkyDome;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace SoL.Utilities
{
	// Token: 0x020002FE RID: 766
	public class HeightFogModifier : VolumeComponentModifier<Fog>
	{
		// Token: 0x06001598 RID: 5528 RVA: 0x000FCC88 File Offset: 0x000FAE88
		private void Update()
		{
			if (base.Component && SkyDomeManager.SkyDomeController != null)
			{
				float dayNightCycleFraction = SkyDomeUtilities.GetDayNightCycleFraction();
				if (this.m_modifyAttenuation)
				{
					base.Component.meanFreePath.value = this.m_attenuationCurve.Evaluate(dayNightCycleFraction);
				}
				if (this.m_modifyHeight)
				{
					base.Component.maximumHeight.value = this.m_heightCurve.Evaluate(dayNightCycleFraction);
				}
			}
		}

		// Token: 0x04001D9F RID: 7583
		[SerializeField]
		private bool m_modifyAttenuation;

		// Token: 0x04001DA0 RID: 7584
		[Tooltip("0.00 midnight\n0.25 sunrise\n0.50 noon\n0.75 sunset\n1.00 midnight")]
		[SerializeField]
		private AnimationCurve m_attenuationCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);

		// Token: 0x04001DA1 RID: 7585
		[SerializeField]
		private bool m_modifyHeight;

		// Token: 0x04001DA2 RID: 7586
		[Tooltip("0.00 midnight\n0.25 sunrise\n0.50 noon\n0.75 sunset\n1.00 midnight")]
		[SerializeField]
		private AnimationCurve m_heightCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);
	}
}

using System;
using UnityEngine;

namespace SoL.Game.SkyDome
{
	// Token: 0x0200070B RID: 1803
	public class BackpackVFXLerper : SunAltitudeLerper
	{
		// Token: 0x0600364B RID: 13899 RVA: 0x0016941C File Offset: 0x0016761C
		protected override void UpdateInternal(float dayNightCycleFraction)
		{
			if (SkyDomeManager.SkyDomeController.IsIndoors || !this.m_particleSystem)
			{
				return;
			}
			ParticleSystem.MainModule main = this.m_particleSystem.main;
			Color color = main.startColor.color;
			color.a = this.m_alphaCurve.Evaluate(dayNightCycleFraction);
			main.startColor = color;
		}

		// Token: 0x04003436 RID: 13366
		[SerializeField]
		private ParticleSystem m_particleSystem;

		// Token: 0x04003437 RID: 13367
		[Tooltip("0.00 midnight\n0.25 sunrise\n0.50 noon\n0.75 sunset\n1.00 midnight")]
		[SerializeField]
		private AnimationCurve m_alphaCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);
	}
}

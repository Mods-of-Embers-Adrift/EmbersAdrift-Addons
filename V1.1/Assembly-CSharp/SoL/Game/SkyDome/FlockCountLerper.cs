using System;
using SoL.Managers;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.SkyDome
{
	// Token: 0x02000710 RID: 1808
	public class FlockCountLerper : SunAltitudeLerper
	{
		// Token: 0x17000C00 RID: 3072
		// (get) Token: 0x06003662 RID: 13922 RVA: 0x000653DB File Offset: 0x000635DB
		private bool m_showPopulationCurve
		{
			get
			{
				return this.m_curveProfile == null;
			}
		}

		// Token: 0x06003663 RID: 13923 RVA: 0x000653E9 File Offset: 0x000635E9
		private void Awake()
		{
			if (this.m_controller == null || GameManager.IsServer)
			{
				base.gameObject.SetActive(false);
				return;
			}
			this.m_maxPopulation = this.m_controller._childAmount;
		}

		// Token: 0x06003664 RID: 13924 RVA: 0x001697B8 File Offset: 0x001679B8
		protected override void UpdateInternal(float dayNightCycleFraction)
		{
			if (this.m_controller)
			{
				float num = ((this.m_curveProfile && this.m_curveProfile.Curve != null) ? this.m_curveProfile.Curve : this.m_populationCurve).Evaluate(dayNightCycleFraction);
				int num2 = (this.m_mode == FlockCountLerper.CurveMode.Fraction) ? Mathf.FloorToInt((float)this.m_maxPopulation * num) : Mathf.FloorToInt(num);
				num2 = Mathf.Clamp(num2, 0, 200);
				this.m_controller._childAmount = num2;
			}
		}

		// Token: 0x0400344C RID: 13388
		private const string kTooltip = "0.00 midnight\n0.25 sunrise\n0.50 noon\n0.75 sunset\n1.00 midnight\n\nY-Axis is population Fraction or Value depending on MODE";

		// Token: 0x0400344D RID: 13389
		[SerializeField]
		private FlockController m_controller;

		// Token: 0x0400344E RID: 13390
		[SerializeField]
		private FlockCountLerper.CurveMode m_mode;

		// Token: 0x0400344F RID: 13391
		[SerializeField]
		private AnimationCurveProfile m_curveProfile;

		// Token: 0x04003450 RID: 13392
		[Tooltip("0.00 midnight\n0.25 sunrise\n0.50 noon\n0.75 sunset\n1.00 midnight\n\nY-Axis is population Fraction or Value depending on MODE")]
		[SerializeField]
		private AnimationCurve m_populationCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);

		// Token: 0x04003451 RID: 13393
		[NonSerialized]
		private int m_maxPopulation;

		// Token: 0x02000711 RID: 1809
		private enum CurveMode
		{
			// Token: 0x04003453 RID: 13395
			Fraction,
			// Token: 0x04003454 RID: 13396
			Value
		}
	}
}

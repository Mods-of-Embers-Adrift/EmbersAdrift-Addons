using System;
using UnityEngine;

namespace SoL.Game.Spawning
{
	// Token: 0x020006CA RID: 1738
	[Serializable]
	public class TargetPopulationThreshold
	{
		// Token: 0x17000B96 RID: 2966
		// (get) Token: 0x060034DC RID: 13532 RVA: 0x0006432F File Offset: 0x0006252F
		public float HealthPercent
		{
			get
			{
				return this.m_healthPercent;
			}
		}

		// Token: 0x17000B97 RID: 2967
		// (get) Token: 0x060034DD RID: 13533 RVA: 0x00064337 File Offset: 0x00062537
		public int TargetPopulation
		{
			get
			{
				return this.m_targetPopulation;
			}
		}

		// Token: 0x04003310 RID: 13072
		[Range(0f, 1f)]
		[SerializeField]
		private float m_healthPercent = 1f;

		// Token: 0x04003311 RID: 13073
		[SerializeField]
		private int m_targetPopulation = 1;
	}
}

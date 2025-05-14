using System;
using System.Collections.Generic;
using SoL.Game;
using SoL.Game.Culling;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x02000270 RID: 624
	public class DayNightManual : MonoBehaviour
	{
		// Token: 0x060013AD RID: 5037 RVA: 0x0004FD78 File Offset: 0x0004DF78
		private void Awake()
		{
			this.m_toggles = new List<IDayNightToggle>();
			this.AddToList(this.m_materialSwapper);
			this.AddToList(this.m_culledLight);
			this.AddToList(this.m_culledParticleSystem);
			this.AddToList(this.m_culledAudioSource);
		}

		// Token: 0x060013AE RID: 5038 RVA: 0x000F76E0 File Offset: 0x000F58E0
		private void AddToList(MonoBehaviour behavior)
		{
			if (behavior == null)
			{
				return;
			}
			IDayNightToggle dayNightToggle = behavior as IDayNightToggle;
			if (dayNightToggle != null)
			{
				this.m_toggles.Add(dayNightToggle);
			}
		}

		// Token: 0x060013AF RID: 5039 RVA: 0x000F7710 File Offset: 0x000F5910
		public void Toggle(bool isEnabled)
		{
			if (this.m_toggles == null)
			{
				return;
			}
			for (int i = 0; i < this.m_toggles.Count; i++)
			{
				if (this.m_toggles[i] != null)
				{
					this.m_toggles[i].Toggle(isEnabled);
				}
			}
		}

		// Token: 0x04001BEF RID: 7151
		[SerializeField]
		private MaterialSwapperDayNight m_materialSwapper;

		// Token: 0x04001BF0 RID: 7152
		[SerializeField]
		private CulledLight m_culledLight;

		// Token: 0x04001BF1 RID: 7153
		[SerializeField]
		private CulledParticleSystem m_culledParticleSystem;

		// Token: 0x04001BF2 RID: 7154
		[SerializeField]
		private CulledAudioSource m_culledAudioSource;

		// Token: 0x04001BF3 RID: 7155
		private List<IDayNightToggle> m_toggles;
	}
}

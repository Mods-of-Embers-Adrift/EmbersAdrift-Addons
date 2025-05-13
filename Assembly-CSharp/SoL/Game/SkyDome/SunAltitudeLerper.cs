using System;
using UnityEngine;

namespace SoL.Game.SkyDome
{
	// Token: 0x02000713 RID: 1811
	public abstract class SunAltitudeLerper : MonoBehaviour
	{
		// Token: 0x0600366A RID: 13930
		protected abstract void UpdateInternal(float dayNightCycleFraction);

		// Token: 0x0600366B RID: 13931 RVA: 0x00065497 File Offset: 0x00063697
		protected void Update()
		{
			if (SkyDomeManager.SkyDomeController != null)
			{
				this.UpdateInternal(SkyDomeUtilities.GetDayNightCycleFraction());
			}
		}
	}
}

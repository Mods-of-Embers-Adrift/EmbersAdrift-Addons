using System;
using UnityEngine;

namespace DigitalRuby.ThunderAndLightning
{
	// Token: 0x020000C3 RID: 195
	public class WaitForSecondsLightning : CustomYieldInstruction
	{
		// Token: 0x0600074B RID: 1867 RVA: 0x00047F49 File Offset: 0x00046149
		public WaitForSecondsLightning(float time)
		{
			this.remaining = time;
		}

		// Token: 0x1700029C RID: 668
		// (get) Token: 0x0600074C RID: 1868 RVA: 0x00047F58 File Offset: 0x00046158
		public override bool keepWaiting
		{
			get
			{
				if (this.remaining <= 0f)
				{
					return false;
				}
				this.remaining -= LightningBoltScript.DeltaTime;
				return true;
			}
		}

		// Token: 0x040008AE RID: 2222
		private float remaining;
	}
}

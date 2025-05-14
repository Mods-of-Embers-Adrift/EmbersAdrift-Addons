using System;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C3F RID: 3135
	[Serializable]
	public class VitalModification
	{
		// Token: 0x060060C4 RID: 24772 RVA: 0x00081327 File Offset: 0x0007F527
		public void Reset()
		{
			this.Value = 0f;
			this.Wound = 0f;
		}

		// Token: 0x04005361 RID: 21345
		public float Value;

		// Token: 0x04005362 RID: 21346
		public float Wound;
	}
}

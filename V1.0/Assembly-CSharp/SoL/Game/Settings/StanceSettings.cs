using System;

namespace SoL.Game.Settings
{
	// Token: 0x02000742 RID: 1858
	[Serializable]
	public class StanceSettings
	{
		// Token: 0x0400365C RID: 13916
		public StanceProfile DefaultStance;

		// Token: 0x0400365D RID: 13917
		public StanceProfile CombatStance;

		// Token: 0x0400365E RID: 13918
		public StanceProfile RestingStance;

		// Token: 0x0400365F RID: 13919
		public StanceProfile CrouchStance;

		// Token: 0x04003660 RID: 13920
		public StanceProfile SwimmingStance;

		// Token: 0x04003661 RID: 13921
		public float StanceBubbleAnimationSpeed = 100f;
	}
}

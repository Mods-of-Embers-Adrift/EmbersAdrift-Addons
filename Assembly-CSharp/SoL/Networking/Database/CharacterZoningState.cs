using System;
using SoL.Game;

namespace SoL.Networking.Database
{
	// Token: 0x02000435 RID: 1077
	[Serializable]
	public class CharacterZoningState
	{
		// Token: 0x0400242B RID: 9259
		public ZoningState State;

		// Token: 0x0400242C RID: 9260
		public int SourceZoneId;

		// Token: 0x0400242D RID: 9261
		public int TargetZoneId;

		// Token: 0x0400242E RID: 9262
		public int TargetIndex;

		// Token: 0x0400242F RID: 9263
		public UniqueId TargetDiscoveryId;

		// Token: 0x04002430 RID: 9264
		public int EssenceCost;

		// Token: 0x04002431 RID: 9265
		public bool UseTravelEssence;
	}
}

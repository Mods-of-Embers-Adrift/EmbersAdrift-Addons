using System;
using SoL.Game.Spawning;

namespace SoL.Utilities
{
	// Token: 0x0200027A RID: 634
	public class SpawnProfileDrawBag : DrawBag<SpawnProfile, SpawnProfileProbabilityEntry, SpawnProfileProbabilityCollection>
	{
		// Token: 0x060013D3 RID: 5075 RVA: 0x0004FE9A File Offset: 0x0004E09A
		public SpawnProfileDrawBag(SpawnProfileProbabilityCollection collection, int sampleSize, float reshuffleThreshold) : base(collection, sampleSize, reshuffleThreshold)
		{
		}
	}
}

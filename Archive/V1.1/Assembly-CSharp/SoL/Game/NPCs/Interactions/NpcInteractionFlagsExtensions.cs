using System;

namespace SoL.Game.NPCs.Interactions
{
	// Token: 0x02000838 RID: 2104
	public static class NpcInteractionFlagsExtensions
	{
		// Token: 0x06003CDB RID: 15579 RVA: 0x0004FB40 File Offset: 0x0004DD40
		public static bool HasBitFlag(this NpcInteractionFlags a, NpcInteractionFlags b)
		{
			return (a & b) == b;
		}

		// Token: 0x06003CDC RID: 15580 RVA: 0x000502DF File Offset: 0x0004E4DF
		public static bool HasAnyFlags(this NpcInteractionFlags a, NpcInteractionFlags b)
		{
			return a != NpcInteractionFlags.None && b != NpcInteractionFlags.None && (a & b) > NpcInteractionFlags.None;
		}
	}
}

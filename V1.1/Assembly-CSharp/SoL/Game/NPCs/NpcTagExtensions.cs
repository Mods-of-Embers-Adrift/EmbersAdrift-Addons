using System;

namespace SoL.Game.NPCs
{
	// Token: 0x0200081D RID: 2077
	public static class NpcTagExtensions
	{
		// Token: 0x06003C1D RID: 15389 RVA: 0x0004FB40 File Offset: 0x0004DD40
		public static bool HasBitFlag(this NpcTags a, NpcTags b)
		{
			return (a & b) == b;
		}

		// Token: 0x06003C1E RID: 15390 RVA: 0x000502DF File Offset: 0x0004E4DF
		public static bool HasAnyFlags(this NpcTags a, NpcTags b)
		{
			return a != NpcTags.None && b != NpcTags.None && (a & b) > NpcTags.None;
		}

		// Token: 0x06003C1F RID: 15391 RVA: 0x0004FB40 File Offset: 0x0004DD40
		public static bool HasBitFlag(long a, long b)
		{
			return (a & b) == b;
		}

		// Token: 0x06003C20 RID: 15392 RVA: 0x00068BB7 File Offset: 0x00066DB7
		public static bool HasAnyFlags(long a, long b)
		{
			return a != 0L && b != 0L && (a & b) != 0L;
		}
	}
}

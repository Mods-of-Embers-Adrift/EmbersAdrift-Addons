using System;

namespace SoL.Game
{
	// Token: 0x0200057F RID: 1407
	public static class GameEntityTypeExtensions
	{
		// Token: 0x06002BF0 RID: 11248 RVA: 0x0005E7BB File Offset: 0x0005C9BB
		public static bool TypeInFlags(this GameEntityType gameEntityType, GameEntityTypeFlags flags)
		{
			switch (gameEntityType)
			{
			case GameEntityType.Player:
				return flags.HasBitFlag(GameEntityTypeFlags.Player);
			case GameEntityType.Npc:
				return flags.HasBitFlag(GameEntityTypeFlags.Npc);
			case GameEntityType.Interactive:
				return flags.HasBitFlag(GameEntityTypeFlags.Interactive);
			default:
				return false;
			}
		}

		// Token: 0x06002BF1 RID: 11249 RVA: 0x0005E7EC File Offset: 0x0005C9EC
		public static GameEntityTypeFlags GetFlagForType(this GameEntityType type)
		{
			switch (type)
			{
			case GameEntityType.Player:
				return GameEntityTypeFlags.Player;
			case GameEntityType.Npc:
				return GameEntityTypeFlags.Npc;
			case GameEntityType.Interactive:
				return GameEntityTypeFlags.Interactive;
			default:
				return GameEntityTypeFlags.None;
			}
		}

		// Token: 0x06002BF2 RID: 11250 RVA: 0x0005E80B File Offset: 0x0005CA0B
		public static GameEntityTypeFlags GetOppositeFlagForType(this GameEntityType type)
		{
			switch (type)
			{
			case GameEntityType.Player:
				return GameEntityTypeFlags.Npc;
			case GameEntityType.Npc:
				return GameEntityTypeFlags.Player;
			case GameEntityType.Interactive:
				return GameEntityTypeFlags.Interactive;
			default:
				return GameEntityTypeFlags.None;
			}
		}

		// Token: 0x06002BF3 RID: 11251 RVA: 0x0004FB40 File Offset: 0x0004DD40
		private static bool HasBitFlag(this GameEntityTypeFlags a, GameEntityTypeFlags b)
		{
			return (a & b) == b;
		}
	}
}

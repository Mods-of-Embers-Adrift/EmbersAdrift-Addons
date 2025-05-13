using System;
using SoL.Game;
using SoL.Managers;

namespace SoL.Networking.Database
{
	// Token: 0x02000416 RID: 1046
	public static class AccessFlagsExtensions
	{
		// Token: 0x06001E3F RID: 7743 RVA: 0x0004FB40 File Offset: 0x0004DD40
		public static bool HasBitFlag(this AccessFlags a, AccessFlags b)
		{
			return (a & b) == b;
		}

		// Token: 0x06001E40 RID: 7744 RVA: 0x000569AD File Offset: 0x00054BAD
		public static bool HasAnyFlags(this AccessFlags a, AccessFlags b)
		{
			return (a & b) > AccessFlags.None;
		}

		// Token: 0x06001E41 RID: 7745 RVA: 0x000569B5 File Offset: 0x00054BB5
		public static bool HasAccess(AccessFlags serverFlags, AccessFlags userFlags)
		{
			return AccessFlagsExtensions.HasAccess((int)serverFlags, (int)userFlags);
		}

		// Token: 0x06001E42 RID: 7746 RVA: 0x000569BE File Offset: 0x00054BBE
		public static bool HasAccess(int serverFlags, int userFlags)
		{
			return serverFlags == 1 || ((serverFlags & 1) != 0 && (serverFlags & userFlags) > 1);
		}

		// Token: 0x06001E43 RID: 7747 RVA: 0x00119A70 File Offset: 0x00117C70
		public static bool HasAccessToInteractive(GameEntity entity, AccessFlags interactiveAccessFlags)
		{
			if (!entity)
			{
				return false;
			}
			AccessFlags userFlags;
			if (GameManager.IsServer)
			{
				userFlags = entity.UserFlags;
			}
			else
			{
				if (SessionData.User == null)
				{
					return false;
				}
				userFlags = SessionData.User.Flags;
			}
			return AccessFlagsExtensions.HasAccessForFlags(userFlags, interactiveAccessFlags);
		}

		// Token: 0x06001E44 RID: 7748 RVA: 0x000569D3 File Offset: 0x00054BD3
		public static bool HasAccessForFlags(AccessFlags userFlags, AccessFlags queryFlags)
		{
			if (queryFlags == AccessFlags.None)
			{
				return true;
			}
			userFlags &= ~AccessFlags.Active;
			queryFlags &= ~AccessFlags.Active;
			return userFlags.HasAnyFlags(queryFlags);
		}
	}
}

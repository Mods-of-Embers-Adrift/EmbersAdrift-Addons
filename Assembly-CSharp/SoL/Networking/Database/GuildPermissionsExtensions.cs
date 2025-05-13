using System;

namespace SoL.Networking.Database
{
	// Token: 0x0200045B RID: 1115
	public static class GuildPermissionsExtensions
	{
		// Token: 0x06001F5E RID: 8030 RVA: 0x0004FB40 File Offset: 0x0004DD40
		public static bool HasBitFlag(this GuildPermissions a, GuildPermissions b)
		{
			return (a & b) == b;
		}
	}
}

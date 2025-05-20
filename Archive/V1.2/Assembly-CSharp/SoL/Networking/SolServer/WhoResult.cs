using System;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Settings;

namespace SoL.Networking.SolServer
{
	// Token: 0x020003F9 RID: 1017
	public class WhoResult
	{
		// Token: 0x1700065C RID: 1628
		// (get) Token: 0x06001B10 RID: 6928 RVA: 0x0005500D File Offset: 0x0005320D
		public BaseArchetype RoleArchetype
		{
			get
			{
				if (this.Role != 0)
				{
					return GlobalSettings.Values.Roles.GetRoleFromPacked((RolePacked)this.Role);
				}
				return null;
			}
		}

		// Token: 0x0400223C RID: 8764
		public string CharacterName;

		// Token: 0x0400223D RID: 8765
		public string GuildName;

		// Token: 0x0400223E RID: 8766
		public int ZoneId;

		// Token: 0x0400223F RID: 8767
		public byte SubZoneId;

		// Token: 0x04002240 RID: 8768
		public byte Role;

		// Token: 0x04002241 RID: 8769
		public byte Level;

		// Token: 0x04002242 RID: 8770
		public bool Lfg;
	}
}

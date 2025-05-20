using System;
using System.Collections.Generic;

namespace SoL.Networking.SolServer
{
	// Token: 0x020003EE RID: 1006
	public static class CommandClassExtensions
	{
		// Token: 0x06001AC5 RID: 6853 RVA: 0x00054C5C File Offset: 0x00052E5C
		public static SolServerCommand NewCommand(this CommandClass cmdClass, CommandType cmdType)
		{
			return new SolServerCommand(cmdClass, cmdType);
		}

		// Token: 0x06001AC6 RID: 6854 RVA: 0x00054C65 File Offset: 0x00052E65
		public static SolServerCommand NewCommand(this CommandClass cmdClass, CommandType cmdType, Dictionary<string, object> args)
		{
			return new SolServerCommand(cmdClass, cmdType, args);
		}

		// Token: 0x06001AC7 RID: 6855 RVA: 0x00054C6F File Offset: 0x00052E6F
		public static bool AllowOfflineCommand(this CommandClass cmdClass)
		{
			return cmdClass == CommandClass.gm || cmdClass == CommandClass.local;
		}
	}
}

using System;

namespace SoL.Utilities
{
	// Token: 0x02000258 RID: 600
	public struct Command
	{
		// Token: 0x04001B88 RID: 7048
		public string CommandText;

		// Token: 0x04001B89 RID: 7049
		public Action<string[]> Action;

		// Token: 0x04001B8A RID: 7050
		public string ShortDescription;

		// Token: 0x04001B8B RID: 7051
		public string Description;

		// Token: 0x04001B8C RID: 7052
		public string[] Aliases;
	}
}

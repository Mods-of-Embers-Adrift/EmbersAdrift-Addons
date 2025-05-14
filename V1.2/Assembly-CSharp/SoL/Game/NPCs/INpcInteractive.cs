using System;

namespace SoL.Game.NPCs
{
	// Token: 0x020007FD RID: 2045
	public interface INpcInteractive
	{
		// Token: 0x17000D92 RID: 3474
		// (get) Token: 0x06003B50 RID: 15184
		NpcInteractiveType ObjectType { get; }

		// Token: 0x17000D93 RID: 3475
		// (get) Token: 0x06003B51 RID: 15185
		NpcTypeFlags NpcTypes { get; }
	}
}

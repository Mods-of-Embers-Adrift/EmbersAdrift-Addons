using System;
using System.Collections.Generic;
using SoL.Game.Objects.Archetypes;

namespace SoL.Game.Objects.Containers
{
	// Token: 0x02000A07 RID: 2567
	public interface ICollectionContents
	{
		// Token: 0x17001149 RID: 4425
		// (get) Token: 0x06004E22 RID: 20002
		List<ArchetypeInstance> Items { get; }

		// Token: 0x1700114A RID: 4426
		// (get) Token: 0x06004E23 RID: 20003
		string Id { get; }

		// Token: 0x1700114B RID: 4427
		// (get) Token: 0x06004E24 RID: 20004
		ContainerType Type { get; }
	}
}

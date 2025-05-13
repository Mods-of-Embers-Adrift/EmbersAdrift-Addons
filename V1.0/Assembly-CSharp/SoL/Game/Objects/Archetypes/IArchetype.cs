using System;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A29 RID: 2601
	public interface IArchetype
	{
		// Token: 0x170011E5 RID: 4581
		// (get) Token: 0x0600508F RID: 20623
		UniqueId Id { get; }

		// Token: 0x06005090 RID: 20624
		void OnInstanceCreated(ArchetypeInstance instance);

		// Token: 0x170011E6 RID: 4582
		// (get) Token: 0x06005091 RID: 20625
		ArchetypeCategory Category { get; }
	}
}

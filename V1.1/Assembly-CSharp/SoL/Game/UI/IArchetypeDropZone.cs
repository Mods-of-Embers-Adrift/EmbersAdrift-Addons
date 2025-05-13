using System;
using SoL.Game.Objects.Archetypes;
using UnityEngine;

namespace SoL.Game.UI
{
	// Token: 0x020008E9 RID: 2281
	public interface IArchetypeDropZone
	{
		// Token: 0x17000F30 RID: 3888
		// (get) Token: 0x060042DC RID: 17116
		GameObject GO { get; }

		// Token: 0x17000F31 RID: 3889
		// (get) Token: 0x060042DD RID: 17117
		int Index { get; }

		// Token: 0x17000F32 RID: 3890
		// (get) Token: 0x060042DE RID: 17118
		ArchetypeInstance CurrentOccupant { get; }

		// Token: 0x17000F33 RID: 3891
		// (get) Token: 0x060042DF RID: 17119
		IContainerUI ContainerUI { get; }

		// Token: 0x060042E0 RID: 17120
		bool CanPlace(ArchetypeInstance instance, int targetIndex);
	}
}

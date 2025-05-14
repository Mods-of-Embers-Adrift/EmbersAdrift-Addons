using System;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A81 RID: 2689
	public interface IHandheldItem
	{
		// Token: 0x170012D9 RID: 4825
		// (get) Token: 0x0600531D RID: 21277
		Sprite Icon { get; }

		// Token: 0x170012DA RID: 4826
		// (get) Token: 0x0600531E RID: 21278
		bool RequiresFreeOffHand { get; }

		// Token: 0x170012DB RID: 4827
		// (get) Token: 0x0600531F RID: 21279
		bool AlternateAnimationSet { get; }

		// Token: 0x170012DC RID: 4828
		// (get) Token: 0x06005320 RID: 21280
		HandheldItemFlags HandheldItemFlag { get; }
	}
}

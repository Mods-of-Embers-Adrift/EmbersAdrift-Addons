using System;
using UnityEngine;

namespace SoL.Game.Interactives
{
	// Token: 0x02000B8E RID: 2958
	public interface IInteractiveBase
	{
		// Token: 0x17001558 RID: 5464
		// (get) Token: 0x06005B35 RID: 23349
		GameObject gameObject { get; }

		// Token: 0x17001559 RID: 5465
		// (get) Token: 0x06005B36 RID: 23350
		InteractionSettings Settings { get; }
	}
}

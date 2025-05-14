using System;
using UnityEngine;

namespace SoL.Game
{
	// Token: 0x02000594 RID: 1428
	public interface IWorldObject
	{
		// Token: 0x1700096B RID: 2411
		// (get) Token: 0x06002C94 RID: 11412
		GameObject gameObject { get; }

		// Token: 0x1700096C RID: 2412
		// (get) Token: 0x06002C95 RID: 11413
		UniqueId WorldId { get; }

		// Token: 0x06002C96 RID: 11414
		bool Validate(GameEntity entity);
	}
}

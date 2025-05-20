using System;
using UnityEngine;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C39 RID: 3129
	public interface ITargetData
	{
		// Token: 0x17001731 RID: 5937
		// (get) Token: 0x060060A3 RID: 24739
		GameEntity Self { get; }

		// Token: 0x17001732 RID: 5938
		// (get) Token: 0x060060A4 RID: 24740
		GameEntity Offensive { get; }

		// Token: 0x17001733 RID: 5939
		// (get) Token: 0x060060A5 RID: 24741
		GameEntity Defensive { get; }

		// Token: 0x17001734 RID: 5940
		// (get) Token: 0x060060A6 RID: 24742
		Vector3? GroundPosition { get; }
	}
}

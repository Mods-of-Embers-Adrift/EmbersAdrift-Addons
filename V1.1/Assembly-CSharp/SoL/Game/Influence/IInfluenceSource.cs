using System;
using UnityEngine;

namespace SoL.Game.Influence
{
	// Token: 0x02000BBE RID: 3006
	public interface IInfluenceSource
	{
		// Token: 0x170015F9 RID: 5625
		// (get) Token: 0x06005D15 RID: 23829
		// (set) Token: 0x06005D16 RID: 23830
		InfluenceProfile InfluenceProfile { get; set; }

		// Token: 0x06005D17 RID: 23831
		float GetInfluence(Vector3 samplePosition, InfluenceFlags flags);
	}
}

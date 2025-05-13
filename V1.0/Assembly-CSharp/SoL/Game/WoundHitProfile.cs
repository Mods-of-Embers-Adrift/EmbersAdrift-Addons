using System;
using SoL.Game.EffectSystem;
using SoL.Game.Objects;
using UnityEngine;

namespace SoL.Game
{
	// Token: 0x020005FE RID: 1534
	[Serializable]
	public struct WoundHitProfile
	{
		// Token: 0x04002F4B RID: 12107
		public HitType HitType;

		// Token: 0x04002F4C RID: 12108
		public MinMaxFloatRange Wound;

		// Token: 0x04002F4D RID: 12109
		public AnimationCurve Chance;
	}
}

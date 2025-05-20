using System;
using UnityEngine;

namespace SoL.Game.Animation
{
	// Token: 0x02000D66 RID: 3430
	[Serializable]
	internal class IdleSet
	{
		// Token: 0x040059D6 RID: 22998
		public Vector2 TurnThreshold = new Vector2(-1f, 1f);

		// Token: 0x040059D7 RID: 22999
		public AnimationClip LeftTurn;

		// Token: 0x040059D8 RID: 23000
		public AnimationClip Idle;

		// Token: 0x040059D9 RID: 23001
		public AnimationClip RightTurn;
	}
}

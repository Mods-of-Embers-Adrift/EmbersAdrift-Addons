using System;
using System.Collections.Generic;
using UnityEngine;

namespace SoL.Game.Animation
{
	// Token: 0x02000D85 RID: 3461
	public class RootMotionMeasure : MonoBehaviour
	{
		// Token: 0x04005A79 RID: 23161
		private const int kCacheCount = 100;

		// Token: 0x04005A7A RID: 23162
		private List<Vector3> m_deltas;

		// Token: 0x04005A7B RID: 23163
		private Vector3 m_lastPos = Vector3.zero;

		// Token: 0x04005A7C RID: 23164
		[SerializeField]
		private float m_statUpdateRate = 1f;

		// Token: 0x04005A7D RID: 23165
		private float m_timeOfNextUpdate;
	}
}

using System;
using Animancer;
using UnityEngine;

namespace SoL.Tests
{
	// Token: 0x02000D8B RID: 3467
	public class AnimancerMixerTester : MonoBehaviour
	{
		// Token: 0x06006854 RID: 26708 RVA: 0x000860E2 File Offset: 0x000842E2
		private void Update()
		{
			this.m_state.Parameter = this.m_blend;
		}

		// Token: 0x04005A8D RID: 23181
		[SerializeField]
		private Vector2 m_blend = Vector2.zero;

		// Token: 0x04005A8E RID: 23182
		[SerializeField]
		private AnimancerComponent m_animancer;

		// Token: 0x04005A8F RID: 23183
		private MixerState<Vector2> m_state;
	}
}

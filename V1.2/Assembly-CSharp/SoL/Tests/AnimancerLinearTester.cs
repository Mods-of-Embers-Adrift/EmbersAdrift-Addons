using System;
using Animancer;
using UnityEngine;

namespace SoL.Tests
{
	// Token: 0x02000D8A RID: 3466
	public class AnimancerLinearTester : MonoBehaviour
	{
		// Token: 0x06006852 RID: 26706 RVA: 0x000860CF File Offset: 0x000842CF
		private void Update()
		{
			this.m_state.Parameter = this.m_blend;
		}

		// Token: 0x04005A8A RID: 23178
		[SerializeField]
		private float m_blend;

		// Token: 0x04005A8B RID: 23179
		[SerializeField]
		private AnimancerComponent m_animancer;

		// Token: 0x04005A8C RID: 23180
		private LinearMixerState m_state;
	}
}

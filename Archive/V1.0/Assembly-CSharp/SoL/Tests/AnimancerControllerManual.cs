using System;
using SoL.Game.Animation;
using UnityEngine;

namespace SoL.Tests
{
	// Token: 0x02000D89 RID: 3465
	public class AnimancerControllerManual : MonoBehaviour
	{
		// Token: 0x04005A86 RID: 23174
		[SerializeField]
		private BaseAnimancerController m_controller;

		// Token: 0x04005A87 RID: 23175
		[SerializeField]
		private float m_speed = 1f;

		// Token: 0x04005A88 RID: 23176
		[SerializeField]
		private float m_rotation;

		// Token: 0x04005A89 RID: 23177
		[SerializeField]
		private Vector2 m_locomotion = Vector2.zero;
	}
}

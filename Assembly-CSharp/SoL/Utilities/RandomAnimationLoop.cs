using System;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x020002B4 RID: 692
	public class RandomAnimationLoop : MonoBehaviour
	{
		// Token: 0x06001496 RID: 5270 RVA: 0x000505A6 File Offset: 0x0004E7A6
		private void Start()
		{
			if (this.m_animator)
			{
				this.m_animator.Play(this.m_animationName, -1, UnityEngine.Random.Range(0f, this.m_timeOffset));
			}
		}

		// Token: 0x04001CD3 RID: 7379
		[SerializeField]
		private Animator m_animator;

		// Token: 0x04001CD4 RID: 7380
		[SerializeField]
		private string m_animationName;

		// Token: 0x04001CD5 RID: 7381
		[SerializeField]
		private float m_timeOffset = 1f;
	}
}

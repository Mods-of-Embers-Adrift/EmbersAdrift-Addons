using System;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x020002AB RID: 683
	public class OnAnimatorMoveForwarder : MonoBehaviour
	{
		// Token: 0x17000509 RID: 1289
		// (get) Token: 0x0600144B RID: 5195 RVA: 0x00050386 File Offset: 0x0004E586
		// (set) Token: 0x0600144C RID: 5196 RVA: 0x0005038E File Offset: 0x0004E58E
		public IOnAnimatorMoveReceiver Receiver { get; set; }

		// Token: 0x0600144D RID: 5197 RVA: 0x00050397 File Offset: 0x0004E597
		private void Awake()
		{
			this.m_animator = base.gameObject.GetComponent<Animator>();
			this.m_animator.applyRootMotion = true;
			this.m_animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
		}

		// Token: 0x0600144E RID: 5198 RVA: 0x000503C2 File Offset: 0x0004E5C2
		private void OnAnimatorMove()
		{
			IOnAnimatorMoveReceiver receiver = this.Receiver;
			if (receiver == null)
			{
				return;
			}
			receiver.OnAnimatorMoveExternal(this.m_animator.deltaPosition);
		}

		// Token: 0x04001CA2 RID: 7330
		private Animator m_animator;
	}
}

using System;
using UnityEngine;

namespace SoL.Game.Culling
{
	// Token: 0x02000CB3 RID: 3251
	public class CulledAnimator : CulledObject
	{
		// Token: 0x060062A4 RID: 25252 RVA: 0x0020545C File Offset: 0x0020365C
		protected override void RefreshCullee()
		{
			base.RefreshCullee();
			if (!this.m_animator)
			{
				return;
			}
			if (this.IsCulled())
			{
				if (this.m_animator.enabled)
				{
					this.m_animator.enabled = false;
					return;
				}
			}
			else if (!this.m_animator.enabled)
			{
				this.m_animator.enabled = true;
			}
		}

		// Token: 0x04005608 RID: 22024
		[SerializeField]
		private Animator m_animator;
	}
}

using System;
using UnityEngine;

namespace SoL.Game.Nameplates
{
	// Token: 0x020009DA RID: 2522
	public class OverheadTargetIndicator : MonoBehaviour
	{
		// Token: 0x06004CB8 RID: 19640 RVA: 0x00073E6F File Offset: 0x0007206F
		private void OnEnable()
		{
			if (OverheadTargetIndicator.Lead == null)
			{
				OverheadTargetIndicator.Lead = this;
				return;
			}
			this.m_animator.Play(0, 0, OverheadTargetIndicator.Lead.GetNormalizedTime());
		}

		// Token: 0x06004CB9 RID: 19641 RVA: 0x00073E9C File Offset: 0x0007209C
		private void OnDisable()
		{
			if (OverheadTargetIndicator.Lead != null && OverheadTargetIndicator.Lead == this)
			{
				OverheadTargetIndicator.Lead = null;
			}
		}

		// Token: 0x06004CBA RID: 19642 RVA: 0x00073EBE File Offset: 0x000720BE
		private void Update()
		{
			if (!OverheadTargetIndicator.Lead)
			{
				OverheadTargetIndicator.Lead = this;
			}
		}

		// Token: 0x06004CBB RID: 19643 RVA: 0x001BD9DC File Offset: 0x001BBBDC
		private float GetNormalizedTime()
		{
			return this.m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
		}

		// Token: 0x0400468F RID: 18063
		private static OverheadTargetIndicator Lead;

		// Token: 0x04004690 RID: 18064
		[SerializeField]
		private Animator m_animator;
	}
}

using System;
using UnityEngine;

namespace SoL.Game.Animation
{
	// Token: 0x02000D76 RID: 3446
	[Serializable]
	public class AnimationSequence
	{
		// Token: 0x170018D9 RID: 6361
		// (get) Token: 0x060067BF RID: 26559 RVA: 0x00085BFB File Offset: 0x00083DFB
		private bool m_showCustomFadeDuration
		{
			get
			{
				return !this.m_hasOverride && this.m_cancelOnMovement;
			}
		}

		// Token: 0x060067C0 RID: 26560 RVA: 0x00213530 File Offset: 0x00211730
		internal virtual AnimationSequence DuplicateForAutoAttack()
		{
			return new AnimationSequence
			{
				m_mask = ((this.Mask == AnimationClipMask.UpperBody) ? AnimationClipMask.UpperBody : AnimationClipMask.AutoAttack),
				m_delay = this.Delay,
				m_loopFinal = this.LoopFinal,
				m_footIk = this.FootIk,
				m_cancelOnMovement = this.CancelOnMovement,
				m_allowAsAutoAttack = this.AllowAsAutoAttack,
				m_cancelOnMovementFadeDuration = this.CancelOnMovementFadeDuration,
				m_animationFlags = this.AnimationFlags,
				m_sequence = this.ClipData
			};
		}

		// Token: 0x170018DA RID: 6362
		// (get) Token: 0x060067C1 RID: 26561 RVA: 0x00045BCA File Offset: 0x00043DCA
		protected virtual bool m_hasOverride
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170018DB RID: 6363
		// (get) Token: 0x060067C2 RID: 26562 RVA: 0x00085C0D File Offset: 0x00083E0D
		public virtual AnimationClipData[] ClipData
		{
			get
			{
				return this.m_sequence;
			}
		}

		// Token: 0x170018DC RID: 6364
		// (get) Token: 0x060067C3 RID: 26563 RVA: 0x00085C15 File Offset: 0x00083E15
		public virtual AnimationClipMask Mask
		{
			get
			{
				return this.m_mask;
			}
		}

		// Token: 0x170018DD RID: 6365
		// (get) Token: 0x060067C4 RID: 26564 RVA: 0x00085C1D File Offset: 0x00083E1D
		public virtual float Delay
		{
			get
			{
				return this.m_delay;
			}
		}

		// Token: 0x170018DE RID: 6366
		// (get) Token: 0x060067C5 RID: 26565 RVA: 0x00085C25 File Offset: 0x00083E25
		public virtual bool LoopFinal
		{
			get
			{
				return this.m_loopFinal;
			}
		}

		// Token: 0x170018DF RID: 6367
		// (get) Token: 0x060067C6 RID: 26566 RVA: 0x00085C2D File Offset: 0x00083E2D
		public virtual bool FootIk
		{
			get
			{
				return this.m_footIk;
			}
		}

		// Token: 0x170018E0 RID: 6368
		// (get) Token: 0x060067C7 RID: 26567 RVA: 0x00085C35 File Offset: 0x00083E35
		public virtual bool CancelOnMovement
		{
			get
			{
				return this.m_cancelOnMovement;
			}
		}

		// Token: 0x170018E1 RID: 6369
		// (get) Token: 0x060067C8 RID: 26568 RVA: 0x00085C3D File Offset: 0x00083E3D
		public virtual bool AllowAsAutoAttack
		{
			get
			{
				return this.m_allowAsAutoAttack;
			}
		}

		// Token: 0x170018E2 RID: 6370
		// (get) Token: 0x060067C9 RID: 26569 RVA: 0x00085C45 File Offset: 0x00083E45
		public virtual float CancelOnMovementFadeDuration
		{
			get
			{
				return this.m_cancelOnMovementFadeDuration;
			}
		}

		// Token: 0x170018E3 RID: 6371
		// (get) Token: 0x060067CA RID: 26570 RVA: 0x00085C4D File Offset: 0x00083E4D
		public virtual AnimationFlags AnimationFlags
		{
			get
			{
				return this.m_animationFlags;
			}
		}

		// Token: 0x170018E4 RID: 6372
		// (get) Token: 0x060067CB RID: 26571 RVA: 0x00085C55 File Offset: 0x00083E55
		public virtual bool IsEmpty
		{
			get
			{
				return this.m_sequence == null || this.m_sequence.Length == 0 || this.m_sequence[0].Clip == null;
			}
		}

		// Token: 0x060067CC RID: 26572 RVA: 0x002135B8 File Offset: 0x002117B8
		public float GetTotalDuration()
		{
			if (this.m_cachedDuration != null)
			{
				return this.m_cachedDuration.Value;
			}
			float num = this.Delay;
			if (!this.IsEmpty)
			{
				for (int i = 0; i < this.ClipData.Length; i++)
				{
					num += this.ClipData[i].Duration;
				}
			}
			if (Application.isPlaying)
			{
				this.m_cachedDuration = new float?(num);
			}
			return num;
		}

		// Token: 0x060067CD RID: 26573 RVA: 0x00085C7D File Offset: 0x00083E7D
		public void SetMaskAndLoop(AnimationClipMask mask, bool loopFinal)
		{
			this.m_mask = mask;
			this.m_loopFinal = loopFinal;
		}

		// Token: 0x04005A2A RID: 23082
		private const string kListDrawerElementLabelName = "GetElementLabel";

		// Token: 0x04005A2B RID: 23083
		[SerializeField]
		private AnimationClipMask m_mask;

		// Token: 0x04005A2C RID: 23084
		[SerializeField]
		private float m_delay;

		// Token: 0x04005A2D RID: 23085
		[SerializeField]
		private bool m_loopFinal;

		// Token: 0x04005A2E RID: 23086
		[SerializeField]
		private bool m_footIk;

		// Token: 0x04005A2F RID: 23087
		[SerializeField]
		private bool m_cancelOnMovement;

		// Token: 0x04005A30 RID: 23088
		[SerializeField]
		private bool m_allowAsAutoAttack;

		// Token: 0x04005A31 RID: 23089
		[SerializeField]
		private float m_cancelOnMovementFadeDuration = 0.1f;

		// Token: 0x04005A32 RID: 23090
		[SerializeField]
		private AnimationFlags m_animationFlags;

		// Token: 0x04005A33 RID: 23091
		[SerializeField]
		private AnimationClipData[] m_sequence;

		// Token: 0x04005A34 RID: 23092
		private float? m_cachedDuration;
	}
}

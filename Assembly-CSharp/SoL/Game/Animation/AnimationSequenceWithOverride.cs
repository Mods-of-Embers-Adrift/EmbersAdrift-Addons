using System;
using System.Collections;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Animation
{
	// Token: 0x02000D77 RID: 3447
	[Serializable]
	public class AnimationSequenceWithOverride : AnimationSequence
	{
		// Token: 0x170018E5 RID: 6373
		// (get) Token: 0x060067CF RID: 26575 RVA: 0x00085CA0 File Offset: 0x00083EA0
		public override AnimationClipData[] ClipData
		{
			get
			{
				if (!this.m_hasOverride)
				{
					return base.ClipData;
				}
				return this.m_override.ClipSequence.ClipData;
			}
		}

		// Token: 0x170018E6 RID: 6374
		// (get) Token: 0x060067D0 RID: 26576 RVA: 0x00085CC1 File Offset: 0x00083EC1
		public override AnimationClipMask Mask
		{
			get
			{
				if (!this.m_hasOverride)
				{
					return base.Mask;
				}
				return this.m_override.ClipSequence.Mask;
			}
		}

		// Token: 0x170018E7 RID: 6375
		// (get) Token: 0x060067D1 RID: 26577 RVA: 0x00085CE2 File Offset: 0x00083EE2
		public override float Delay
		{
			get
			{
				if (!this.m_hasOverride)
				{
					return base.Delay;
				}
				return this.m_override.ClipSequence.Delay;
			}
		}

		// Token: 0x170018E8 RID: 6376
		// (get) Token: 0x060067D2 RID: 26578 RVA: 0x00085D03 File Offset: 0x00083F03
		public override bool LoopFinal
		{
			get
			{
				if (!this.m_hasOverride)
				{
					return base.LoopFinal;
				}
				return this.m_override.ClipSequence.LoopFinal;
			}
		}

		// Token: 0x170018E9 RID: 6377
		// (get) Token: 0x060067D3 RID: 26579 RVA: 0x00085D24 File Offset: 0x00083F24
		public override bool FootIk
		{
			get
			{
				if (!this.m_hasOverride)
				{
					return base.FootIk;
				}
				return this.m_override.ClipSequence.FootIk;
			}
		}

		// Token: 0x170018EA RID: 6378
		// (get) Token: 0x060067D4 RID: 26580 RVA: 0x00085D45 File Offset: 0x00083F45
		public override bool CancelOnMovement
		{
			get
			{
				if (!this.m_override)
				{
					return base.CancelOnMovement;
				}
				return this.m_override.ClipSequence.CancelOnMovement;
			}
		}

		// Token: 0x170018EB RID: 6379
		// (get) Token: 0x060067D5 RID: 26581 RVA: 0x00085D6B File Offset: 0x00083F6B
		public override bool AllowAsAutoAttack
		{
			get
			{
				if (!this.m_override)
				{
					return base.AllowAsAutoAttack;
				}
				return this.m_override.ClipSequence.AllowAsAutoAttack;
			}
		}

		// Token: 0x170018EC RID: 6380
		// (get) Token: 0x060067D6 RID: 26582 RVA: 0x00085D91 File Offset: 0x00083F91
		public override float CancelOnMovementFadeDuration
		{
			get
			{
				if (!this.m_override)
				{
					return base.CancelOnMovementFadeDuration;
				}
				return this.m_override.ClipSequence.CancelOnMovementFadeDuration;
			}
		}

		// Token: 0x170018ED RID: 6381
		// (get) Token: 0x060067D7 RID: 26583 RVA: 0x00085DB7 File Offset: 0x00083FB7
		public override AnimationFlags AnimationFlags
		{
			get
			{
				if (!this.m_hasOverride)
				{
					return base.AnimationFlags;
				}
				return this.m_override.ClipSequence.AnimationFlags;
			}
		}

		// Token: 0x170018EE RID: 6382
		// (get) Token: 0x060067D8 RID: 26584 RVA: 0x00085DD8 File Offset: 0x00083FD8
		public override bool IsEmpty
		{
			get
			{
				if (!this.m_hasOverride)
				{
					return base.IsEmpty;
				}
				return this.m_override.ClipSequence.IsEmpty;
			}
		}

		// Token: 0x170018EF RID: 6383
		// (get) Token: 0x060067D9 RID: 26585 RVA: 0x00085DF9 File Offset: 0x00083FF9
		protected override bool m_hasOverride
		{
			get
			{
				return this.m_override != null;
			}
		}

		// Token: 0x060067DA RID: 26586 RVA: 0x00085E07 File Offset: 0x00084007
		private IEnumerable GetScriptables()
		{
			return SolOdinUtilities.GetDropdownItems<ScriptableAnimationSequence>();
		}

		// Token: 0x04005A35 RID: 23093
		[SerializeField]
		private ScriptableAnimationSequence m_override;
	}
}

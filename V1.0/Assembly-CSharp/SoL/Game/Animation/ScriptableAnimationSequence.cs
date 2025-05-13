using System;
using UnityEngine;

namespace SoL.Game.Animation
{
	// Token: 0x02000D86 RID: 3462
	[CreateAssetMenu(menuName = "SoL/Animation/Animation Sequence")]
	public class ScriptableAnimationSequence : ScriptableObject
	{
		// Token: 0x1700190A RID: 6410
		// (get) Token: 0x0600684A RID: 26698 RVA: 0x0008603A File Offset: 0x0008423A
		public AnimationSequence ClipSequence
		{
			get
			{
				return this.m_animationClipSequence;
			}
		}

		// Token: 0x04005A7E RID: 23166
		[SerializeField]
		private AnimationSequence m_animationClipSequence;
	}
}

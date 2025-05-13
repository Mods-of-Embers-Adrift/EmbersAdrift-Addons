using System;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x02000240 RID: 576
	[CreateAssetMenu(menuName = "SoL/Profiles/Animation Curve")]
	public class AnimationCurveProfile : ScriptableObject
	{
		// Token: 0x170004E3 RID: 1251
		// (get) Token: 0x06001307 RID: 4871 RVA: 0x0004F882 File Offset: 0x0004DA82
		public AnimationCurve Curve
		{
			get
			{
				return this.m_curve;
			}
		}

		// Token: 0x040010D9 RID: 4313
		[SerializeField]
		private AnimationCurve m_curve;
	}
}

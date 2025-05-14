using System;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x020002C5 RID: 709
	[CreateAssetMenu(menuName = "SoL/Scriptable Curve")]
	public class ScriptableCurve : ScriptableObject, IAnimationCurve
	{
		// Token: 0x060014C5 RID: 5317 RVA: 0x000507BA File Offset: 0x0004E9BA
		public float Evaluate(float time)
		{
			if (this.m_curve == null)
			{
				return 0f;
			}
			return this.m_curve.Evaluate(time);
		}

		// Token: 0x04001D00 RID: 7424
		[SerializeField]
		private AnimationCurve m_curve = AnimationCurve.Constant(0f, 1f, 1f);
	}
}

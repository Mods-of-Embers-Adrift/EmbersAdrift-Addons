using System;
using UnityEngine;

namespace SoL.Game.Objects
{
	// Token: 0x020009F2 RID: 2546
	[Serializable]
	public class DynamicDiceSet
	{
		// Token: 0x06004D74 RID: 19828 RVA: 0x001C0A28 File Offset: 0x001BEC28
		public DiceSet GenerateDiceSet(float level)
		{
			int nDice = Mathf.FloorToInt(this.m_nDiceCurve.Evaluate(level));
			int nSides = Mathf.FloorToInt(this.m_nSidesCurve.Evaluate(level));
			int modifier = Mathf.FloorToInt(this.m_modifierCurve.Evaluate(level));
			return new DiceSet(nDice, nSides, modifier);
		}

		// Token: 0x0400471D RID: 18205
		[SerializeField]
		private AnimationCurve m_nDiceCurve = AnimationCurve.Linear(0f, 1f, 50f, 1f);

		// Token: 0x0400471E RID: 18206
		[SerializeField]
		private AnimationCurve m_nSidesCurve = AnimationCurve.Linear(0f, 1f, 50f, 1f);

		// Token: 0x0400471F RID: 18207
		[SerializeField]
		private AnimationCurve m_modifierCurve = AnimationCurve.Linear(0f, 0f, 50f, 0f);
	}
}

using System;
using SoL.Game.Objects;
using UnityEngine;

namespace SoL.Game.Spawning
{
	// Token: 0x0200066F RID: 1647
	[Serializable]
	public class CallForHelpPeriodicData : CallForHelpData
	{
		// Token: 0x17000AED RID: 2797
		// (get) Token: 0x06003320 RID: 13088 RVA: 0x00045BCA File Offset: 0x00043DCA
		protected override bool m_showChance
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000AEE RID: 2798
		// (get) Token: 0x06003321 RID: 13089 RVA: 0x000632FA File Offset: 0x000614FA
		public MinMaxIntRange Frequency
		{
			get
			{
				return this.m_frequency;
			}
		}

		// Token: 0x06003322 RID: 13090 RVA: 0x00063302 File Offset: 0x00061502
		protected override float GetChance(float healthPercent)
		{
			return this.m_chanceCurve.Evaluate(1f - healthPercent);
		}

		// Token: 0x0400315F RID: 12639
		[Tooltip("Chance based on health percent remaining. Full Health: x=0, Dead: x=1")]
		[SerializeField]
		private AnimationCurve m_chanceCurve = AnimationCurve.Linear(0f, 0.2f, 1f, 0.2f);

		// Token: 0x04003160 RID: 12640
		[SerializeField]
		private MinMaxIntRange m_frequency = new MinMaxIntRange(15, 30);
	}
}

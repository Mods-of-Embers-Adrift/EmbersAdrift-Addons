using System;
using UnityEngine;

namespace SoL.Game.Crafting
{
	// Token: 0x02000CE9 RID: 3305
	[Serializable]
	public class RawComponentFailureChance
	{
		// Token: 0x06006410 RID: 25616 RVA: 0x002084F4 File Offset: 0x002066F4
		public RawComponentFailureChance()
		{
		}

		// Token: 0x06006411 RID: 25617 RVA: 0x00208548 File Offset: 0x00206748
		public RawComponentFailureChance(RawComponentFailureChance copyFrom)
		{
			this.m_withinMaterialLevel = new AnimationCurve(copyFrom.m_withinMaterialLevel.keys);
			this.m_exceedsMaterialLevel = new AnimationCurve(copyFrom.m_exceedsMaterialLevel.keys);
		}

		// Token: 0x06006412 RID: 25618 RVA: 0x002085C8 File Offset: 0x002067C8
		public bool CheckForFailure(bool exceedsMaterialLevel, float levelRangeDelta)
		{
			float num = (exceedsMaterialLevel ? this.m_exceedsMaterialLevel : this.m_withinMaterialLevel).Evaluate(levelRangeDelta);
			return UnityEngine.Random.Range(0f, 1f) < num;
		}

		// Token: 0x06006413 RID: 25619 RVA: 0x000835F5 File Offset: 0x000817F5
		public float GetChanceToFail(bool exceedsMaterialLevel, float levelRangeDelta)
		{
			return (exceedsMaterialLevel ? this.m_exceedsMaterialLevel : this.m_withinMaterialLevel).Evaluate(levelRangeDelta);
		}

		// Token: 0x040056F8 RID: 22264
		[SerializeField]
		private AnimationCurve m_withinMaterialLevel = AnimationCurve.Linear(0f, 0f, 1f, 0f);

		// Token: 0x040056F9 RID: 22265
		[SerializeField]
		private AnimationCurve m_exceedsMaterialLevel = AnimationCurve.Linear(0f, 0f, 1f, 0f);
	}
}

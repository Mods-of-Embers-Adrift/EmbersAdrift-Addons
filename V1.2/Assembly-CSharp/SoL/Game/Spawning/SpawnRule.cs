using System;
using SoL.Game.Influence;
using UnityEngine;
using UnityEngine.AI;

namespace SoL.Game.Spawning
{
	// Token: 0x020006D8 RID: 1752
	[Serializable]
	public class SpawnRule
	{
		// Token: 0x06003516 RID: 13590 RVA: 0x00166B44 File Offset: 0x00164D44
		public bool IsValid(NavMeshHit hit)
		{
			switch (this.m_ruleType)
			{
			case SpawnRuleType.None:
				return true;
			case SpawnRuleType.Influence:
				return this.m_influenceEvaluator.Evaluate(hit.position, 50f) <= this.m_influenceThreshold;
			case SpawnRuleType.Angle:
				return Vector3.Angle(Vector3.up, hit.normal) <= this.m_angleLimit;
			case SpawnRuleType.NavArea:
				return (this.m_navArea.AreaMask & hit.mask) == hit.mask;
			}
			return false;
		}

		// Token: 0x0400334B RID: 13131
		[SerializeField]
		private SpawnRuleType m_ruleType;

		// Token: 0x0400334C RID: 13132
		[SerializeField]
		private InfluenceEvaluator m_influenceEvaluator;

		// Token: 0x0400334D RID: 13133
		[SerializeField]
		private float m_influenceThreshold = 0.1f;

		// Token: 0x0400334E RID: 13134
		[SerializeField]
		private float m_angleLimit = 35f;

		// Token: 0x0400334F RID: 13135
		[SerializeField]
		private NavAreaSelector m_navArea = new NavAreaSelector();
	}
}

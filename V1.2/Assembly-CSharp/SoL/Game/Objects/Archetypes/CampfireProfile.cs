using System;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000AE0 RID: 2784
	[CreateAssetMenu(menuName = "SoL/Profiles/Campfire")]
	public class CampfireProfile : ScriptableObject
	{
		// Token: 0x060055D3 RID: 21971 RVA: 0x00079432 File Offset: 0x00077632
		public float GetWoundRegenRage(int masteryLevel)
		{
			return this.m_woundRegenRateRange.GetScaledValueForMasteryLevel(CampfireProfile.kLevelRange, (float)masteryLevel);
		}

		// Token: 0x060055D4 RID: 21972 RVA: 0x00079446 File Offset: 0x00077646
		public float GetHealthRegenRate(int masteryLevel)
		{
			return this.m_healthRegenRateRange.GetScaledValueForMasteryLevel(CampfireProfile.kLevelRange, (float)masteryLevel);
		}

		// Token: 0x060055D5 RID: 21973 RVA: 0x0007945A File Offset: 0x0007765A
		public float GetDuration(int masteryLevel)
		{
			return this.m_duration.GetScaledValueForMasteryLevel(CampfireProfile.kLevelRange, (float)masteryLevel);
		}

		// Token: 0x060055D6 RID: 21974 RVA: 0x0007946E File Offset: 0x0007766E
		public float GetRange(int masteryLevel)
		{
			return this.m_range.GetScaledValueForMasteryLevel(CampfireProfile.kLevelRange, (float)masteryLevel);
		}

		// Token: 0x04004C2A RID: 19498
		[SerializeField]
		private MinMaxFloatRange m_woundRegenRateRange = new MinMaxFloatRange(0.0008f, 0.0008f);

		// Token: 0x04004C2B RID: 19499
		[SerializeField]
		private MinMaxFloatRange m_healthRegenRateRange = new MinMaxFloatRange(0.1f, 0.1f);

		// Token: 0x04004C2C RID: 19500
		[SerializeField]
		private MinMaxFloatRange m_duration = new MinMaxFloatRange(30f, 60f);

		// Token: 0x04004C2D RID: 19501
		[SerializeField]
		private MinMaxFloatRange m_range = new MinMaxFloatRange(5f, 10f);

		// Token: 0x04004C2E RID: 19502
		[TextArea(4, 20)]
		[SerializeField]
		private string m_notes;

		// Token: 0x04004C2F RID: 19503
		private static readonly MinMaxIntRange kLevelRange = new MinMaxIntRange(1, 100);
	}
}

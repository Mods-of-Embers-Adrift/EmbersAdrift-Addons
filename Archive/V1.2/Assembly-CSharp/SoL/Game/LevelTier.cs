using System;
using System.Collections.Generic;
using SoL.Game.Objects;
using UnityEngine;

namespace SoL.Game
{
	// Token: 0x02000595 RID: 1429
	[Serializable]
	public class LevelTier
	{
		// Token: 0x1700096D RID: 2413
		// (get) Token: 0x06002C97 RID: 11415 RVA: 0x0014A784 File Offset: 0x00148984
		private static Dictionary<LevelTiers, MinMaxIntRange> Ranges
		{
			get
			{
				if (LevelTier.m_ranges == null)
				{
					LevelTier.m_ranges = new Dictionary<LevelTiers, MinMaxIntRange>(default(TierComparer))
					{
						{
							LevelTiers.T0,
							new MinMaxIntRange(1, 10)
						},
						{
							LevelTiers.T1,
							new MinMaxIntRange(10, 20)
						},
						{
							LevelTiers.T2,
							new MinMaxIntRange(20, 30)
						},
						{
							LevelTiers.T3,
							new MinMaxIntRange(30, 40)
						},
						{
							LevelTiers.T4,
							new MinMaxIntRange(40, 50)
						},
						{
							LevelTiers.T5,
							new MinMaxIntRange(50, 60)
						},
						{
							LevelTiers.T6,
							new MinMaxIntRange(60, 70)
						},
						{
							LevelTiers.T7,
							new MinMaxIntRange(70, 80)
						},
						{
							LevelTiers.T8,
							new MinMaxIntRange(80, 90)
						},
						{
							LevelTiers.T9,
							new MinMaxIntRange(90, 100)
						},
						{
							LevelTiers.T10,
							new MinMaxIntRange(100, 110)
						},
						{
							LevelTiers.T11,
							new MinMaxIntRange(110, 120)
						},
						{
							LevelTiers.T12,
							new MinMaxIntRange(120, 130)
						},
						{
							LevelTiers.T13,
							new MinMaxIntRange(130, 140)
						},
						{
							LevelTiers.T14,
							new MinMaxIntRange(140, 150)
						}
					};
				}
				return LevelTier.m_ranges;
			}
		}

		// Token: 0x1700096E RID: 2414
		// (get) Token: 0x06002C98 RID: 11416 RVA: 0x0005EEAA File Offset: 0x0005D0AA
		public MinMaxIntRange LevelRange
		{
			get
			{
				if (this.m_tier != LevelTiers.Custom)
				{
					return LevelTier.Ranges[this.m_tier];
				}
				return this.m_customLevelRange;
			}
		}

		// Token: 0x06002C99 RID: 11417 RVA: 0x0014A8BC File Offset: 0x00148ABC
		public int GetRandomLevel()
		{
			if (this.m_tier != LevelTiers.Custom)
			{
				return LevelTier.Ranges[this.m_tier].RandomWithinRange();
			}
			return this.m_customLevelRange.RandomWithinRange();
		}

		// Token: 0x06002C9A RID: 11418 RVA: 0x0005EECD File Offset: 0x0005D0CD
		private void UpdateCustomRange()
		{
			if (this.m_tier != LevelTiers.Custom)
			{
				this.m_customLevelRange = LevelTier.Ranges[this.m_tier];
			}
		}

		// Token: 0x04002C48 RID: 11336
		private static Dictionary<LevelTiers, MinMaxIntRange> m_ranges;

		// Token: 0x04002C49 RID: 11337
		[SerializeField]
		private LevelTiers m_tier;

		// Token: 0x04002C4A RID: 11338
		[SerializeField]
		private MinMaxIntRange m_customLevelRange = new MinMaxIntRange(1, 10);
	}
}

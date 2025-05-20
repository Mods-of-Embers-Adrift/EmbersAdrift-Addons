using System;
using SoL.Game.Objects.Archetypes;
using UnityEngine;

namespace SoL.Game.Settings
{
	// Token: 0x02000723 RID: 1827
	[Serializable]
	public class CraftingSettings
	{
		// Token: 0x17000C42 RID: 3138
		// (get) Token: 0x060036ED RID: 14061 RVA: 0x00065963 File Offset: 0x00063B63
		public ItemArchetype DefaultDeconstructItem
		{
			get
			{
				return this.m_defaultDeconstructItem;
			}
		}

		// Token: 0x17000C43 RID: 3139
		// (get) Token: 0x060036EE RID: 14062 RVA: 0x0006596B File Offset: 0x00063B6B
		public float DeconstructExperienceMultiplier
		{
			get
			{
				return this.m_deconstructExperienceMultiplier;
			}
		}

		// Token: 0x17000C44 RID: 3140
		// (get) Token: 0x060036EF RID: 14063 RVA: 0x00065973 File Offset: 0x00063B73
		public int MinimumQualityModifier
		{
			get
			{
				return this.m_minimumQualityModifier;
			}
		}

		// Token: 0x17000C45 RID: 3141
		// (get) Token: 0x060036F0 RID: 14064 RVA: 0x0006597B File Offset: 0x00063B7B
		public int MaximumQualityModifier
		{
			get
			{
				return this.m_maximumQualityModifier;
			}
		}

		// Token: 0x060036F1 RID: 14065 RVA: 0x00065983 File Offset: 0x00063B83
		public int GetMaxTradeLevel(int adventuringLevel)
		{
			return Mathf.Min(adventuringLevel + this.m_maxAdventuringLevelDelta, 50);
		}

		// Token: 0x0400351F RID: 13599
		[SerializeField]
		private ItemArchetype m_defaultDeconstructItem;

		// Token: 0x04003520 RID: 13600
		[SerializeField]
		private float m_deconstructExperienceMultiplier = 0.1f;

		// Token: 0x04003521 RID: 13601
		[SerializeField]
		private int m_minimumQualityModifier = 1;

		// Token: 0x04003522 RID: 13602
		[SerializeField]
		private int m_maximumQualityModifier = 100;

		// Token: 0x04003523 RID: 13603
		[SerializeField]
		private int m_maxAdventuringLevelDelta = 10;
	}
}

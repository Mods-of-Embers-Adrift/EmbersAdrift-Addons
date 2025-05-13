using System;
using System.Collections;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Loot
{
	// Token: 0x02000B27 RID: 2855
	[Serializable]
	public class LootTableSampleCount
	{
		// Token: 0x170014A2 RID: 5282
		// (get) Token: 0x060057A9 RID: 22441 RVA: 0x0007A787 File Offset: 0x00078987
		private bool m_showSampleCounts
		{
			get
			{
				return !this.HideSampleCounts && this.m_lootTable != null;
			}
		}

		// Token: 0x170014A3 RID: 5283
		// (get) Token: 0x060057AA RID: 22442 RVA: 0x0007A79F File Offset: 0x0007899F
		// (set) Token: 0x060057AB RID: 22443 RVA: 0x0007A7A7 File Offset: 0x000789A7
		public bool HideSampleCounts { get; set; }

		// Token: 0x170014A4 RID: 5284
		// (get) Token: 0x060057AC RID: 22444 RVA: 0x0007A7B0 File Offset: 0x000789B0
		public LootTable Table
		{
			get
			{
				return this.m_lootTable;
			}
		}

		// Token: 0x170014A5 RID: 5285
		// (get) Token: 0x060057AD RID: 22445 RVA: 0x0007A7B8 File Offset: 0x000789B8
		public bool HasLoot
		{
			get
			{
				return this.m_lootTable != null;
			}
		}

		// Token: 0x060057AE RID: 22446 RVA: 0x001E4138 File Offset: 0x001E2338
		public int GetSampleCount()
		{
			int num = (this.m_minSampleCountOverride != null) ? this.m_minSampleCountOverride.Value : this.m_minSampleCount;
			int num2 = (this.m_maxSampleCountOverride != null) ? this.m_maxSampleCountOverride.Value : this.m_maxSampleCount;
			int num3 = Mathf.Max(0, num2);
			return UnityEngine.Random.Range((num > num2) ? num3 : num, num3 + 1);
		}

		// Token: 0x060057AF RID: 22447 RVA: 0x0007A7C6 File Offset: 0x000789C6
		public void OverrideMinMaxCount(LootTableSampleCount lootTableSampleCount)
		{
			if (lootTableSampleCount != null)
			{
				this.m_minSampleCountOverride = new int?(lootTableSampleCount.m_minSampleCount);
				this.m_maxSampleCountOverride = new int?(lootTableSampleCount.m_maxSampleCount);
			}
		}

		// Token: 0x060057B0 RID: 22448 RVA: 0x00063AA4 File Offset: 0x00061CA4
		private IEnumerable GetLootTables()
		{
			return SolOdinUtilities.GetDropdownItems<LootTable>();
		}

		// Token: 0x04004D57 RID: 19799
		private const int kMinSampleCount = 1;

		// Token: 0x04004D58 RID: 19800
		private const int kMaxSampleCount = 10;

		// Token: 0x04004D59 RID: 19801
		[SerializeField]
		private LootTable m_lootTable;

		// Token: 0x04004D5A RID: 19802
		[Range(1f, 10f)]
		[SerializeField]
		private int m_minSampleCount = 1;

		// Token: 0x04004D5B RID: 19803
		[Range(1f, 10f)]
		[SerializeField]
		private int m_maxSampleCount = 1;

		// Token: 0x04004D5C RID: 19804
		private int? m_minSampleCountOverride;

		// Token: 0x04004D5D RID: 19805
		private int? m_maxSampleCountOverride;
	}
}

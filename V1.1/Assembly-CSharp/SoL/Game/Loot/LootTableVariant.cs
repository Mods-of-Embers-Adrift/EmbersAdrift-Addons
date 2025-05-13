using System;
using System.Collections.Generic;
using UnityEngine;

namespace SoL.Game.Loot
{
	// Token: 0x02000B28 RID: 2856
	[CreateAssetMenu(menuName = "SoL/Profiles/Loot/Table V2 Variant")]
	public class LootTableVariant : LootTable
	{
		// Token: 0x060057B2 RID: 22450 RVA: 0x0007A803 File Offset: 0x00078A03
		private bool ValidateParent(LootTable table)
		{
			return !(table is LootTableVariant);
		}

		// Token: 0x170014A6 RID: 5286
		// (get) Token: 0x060057B3 RID: 22451 RVA: 0x0007A811 File Offset: 0x00078A11
		internal override bool HasGuaranteed
		{
			get
			{
				return base.HasGuaranteed || this.m_parentLootTable.HasGuaranteed;
			}
		}

		// Token: 0x170014A7 RID: 5287
		// (get) Token: 0x060057B4 RID: 22452 RVA: 0x0007A828 File Offset: 0x00078A28
		internal override LootTableItem[] GuaranteedAll
		{
			get
			{
				if (this.m_guaranteedAllCombined == null)
				{
					return base.GuaranteedAll;
				}
				return this.m_guaranteedAllCombined;
			}
		}

		// Token: 0x170014A8 RID: 5288
		// (get) Token: 0x060057B5 RID: 22453 RVA: 0x0007A83F File Offset: 0x00078A3F
		internal override LootTableItemWithOverrideProbabilityCollection Guaranteed
		{
			get
			{
				if (this.m_guaranteedCombined == null)
				{
					return base.Guaranteed;
				}
				return this.m_guaranteedCombined;
			}
		}

		// Token: 0x170014A9 RID: 5289
		// (get) Token: 0x060057B6 RID: 22454 RVA: 0x0007A856 File Offset: 0x00078A56
		internal override LootTable.LootTableCategoryProbabilityCollection Categories
		{
			get
			{
				if (this.m_categoriesCombined == null)
				{
					return base.Categories;
				}
				return this.m_categoriesCombined;
			}
		}

		// Token: 0x060057B7 RID: 22455 RVA: 0x001E41A0 File Offset: 0x001E23A0
		protected override void InitializeTable()
		{
			if (this.m_tableInitialized || this.m_parentLootTable == null)
			{
				return;
			}
			if (this.m_parentLootTable.GuaranteedAll.Length != 0)
			{
				List<LootTableItem> list = new List<LootTableItem>(base.GuaranteedAll.Length + this.m_parentLootTable.GuaranteedAll.Length);
				for (int i = 0; i < this.m_parentLootTable.GuaranteedAll.Length; i++)
				{
					list.Add(this.m_parentLootTable.GuaranteedAll[i]);
				}
				for (int j = 0; j < base.GuaranteedAll.Length; j++)
				{
					list.Add(base.GuaranteedAll[j]);
				}
				this.m_guaranteedAllCombined = list.ToArray();
			}
			this.m_guaranteedCombined = this.GetGuaranteedCombined();
			this.m_categoriesCombined = this.GetCombinedCategories();
			this.m_tableInitialized = true;
		}

		// Token: 0x060057B8 RID: 22456 RVA: 0x001E4268 File Offset: 0x001E2468
		private LootTableItemWithOverrideProbabilityCollection GetGuaranteedCombined()
		{
			if (this.m_parentLootTable == null || this.m_parentLootTable.Guaranteed.Count <= 0)
			{
				return null;
			}
			List<LootTableItemWithOverrideProbabilityEntry> list = new List<LootTableItemWithOverrideProbabilityEntry>(base.Guaranteed.Count + this.m_parentLootTable.Guaranteed.Count);
			for (int i = 0; i < this.m_parentLootTable.Guaranteed.Count; i++)
			{
				LootTableItemWithOverrideProbabilityEntry lootTableItemWithOverrideProbabilityEntry = this.m_parentLootTable.Guaranteed.Entries[i];
				if (lootTableItemWithOverrideProbabilityEntry != null)
				{
					LootTableItemWithOverrideProbabilityEntry lootTableItemWithOverrideProbabilityEntry2 = new LootTableItemWithOverrideProbabilityEntry();
					lootTableItemWithOverrideProbabilityEntry2.CloneFrom(lootTableItemWithOverrideProbabilityEntry);
					list.Add(lootTableItemWithOverrideProbabilityEntry2);
				}
			}
			for (int j = 0; j < base.Guaranteed.Count; j++)
			{
				LootTableItemWithOverrideProbabilityEntry lootTableItemWithOverrideProbabilityEntry3 = base.Guaranteed.Entries[j];
				if (lootTableItemWithOverrideProbabilityEntry3 != null)
				{
					LootTableItemWithOverrideProbabilityEntry lootTableItemWithOverrideProbabilityEntry4 = new LootTableItemWithOverrideProbabilityEntry();
					lootTableItemWithOverrideProbabilityEntry4.CloneFrom(lootTableItemWithOverrideProbabilityEntry3);
					list.Add(lootTableItemWithOverrideProbabilityEntry4);
				}
			}
			return new LootTableItemWithOverrideProbabilityCollection
			{
				Entries = list.ToArray()
			};
		}

		// Token: 0x060057B9 RID: 22457 RVA: 0x001E4358 File Offset: 0x001E2558
		private LootTable.LootTableCategoryProbabilityCollection GetCombinedCategories()
		{
			if (this.m_parentLootTable == null || this.m_parentLootTable.Categories.Count <= 0)
			{
				return null;
			}
			List<LootTable.LootTableCategoryProbabilityEntry> list = new List<LootTable.LootTableCategoryProbabilityEntry>(base.Categories.Count + this.m_parentLootTable.Categories.Count);
			for (int i = 0; i < this.m_parentLootTable.Categories.Count; i++)
			{
				LootTable.LootTableCategoryProbabilityEntry lootTableCategoryProbabilityEntry = this.m_parentLootTable.Categories.Entries[i];
				if (lootTableCategoryProbabilityEntry != null)
				{
					LootTable.LootTableCategoryProbabilityEntry lootTableCategoryProbabilityEntry2 = new LootTable.LootTableCategoryProbabilityEntry();
					lootTableCategoryProbabilityEntry2.CloneFrom(lootTableCategoryProbabilityEntry);
					list.Add(lootTableCategoryProbabilityEntry2);
				}
			}
			for (int j = 0; j < base.Categories.Count; j++)
			{
				LootTable.LootTableCategoryProbabilityEntry lootTableCategoryProbabilityEntry3 = base.Categories.Entries[j];
				if (lootTableCategoryProbabilityEntry3 != null)
				{
					LootTable.LootTableCategoryProbabilityEntry lootTableCategoryProbabilityEntry4 = new LootTable.LootTableCategoryProbabilityEntry();
					lootTableCategoryProbabilityEntry4.CloneFrom(lootTableCategoryProbabilityEntry3);
					list.Add(lootTableCategoryProbabilityEntry4);
				}
			}
			return new LootTable.LootTableCategoryProbabilityCollection
			{
				Entries = list.ToArray()
			};
		}

		// Token: 0x04004D5F RID: 19807
		[SerializeField]
		private LootTable m_parentLootTable;

		// Token: 0x04004D60 RID: 19808
		[NonSerialized]
		private bool m_tableInitialized;

		// Token: 0x04004D61 RID: 19809
		[NonSerialized]
		private LootTableItem[] m_guaranteedAllCombined;

		// Token: 0x04004D62 RID: 19810
		[NonSerialized]
		private LootTableItemWithOverrideProbabilityCollection m_guaranteedCombined;

		// Token: 0x04004D63 RID: 19811
		[NonSerialized]
		private LootTable.LootTableCategoryProbabilityCollection m_categoriesCombined;
	}
}

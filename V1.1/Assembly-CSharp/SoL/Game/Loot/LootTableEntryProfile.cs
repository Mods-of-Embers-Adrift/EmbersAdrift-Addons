using System;
using System.Collections;
using SoL.Game.NPCs;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Spawning;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Loot
{
	// Token: 0x02000B10 RID: 2832
	[CreateAssetMenu(menuName = "SoL/Profiles/Loot/Table Entry")]
	public class LootTableEntryProfile : ScriptableObject
	{
		// Token: 0x06005758 RID: 22360 RVA: 0x001E3544 File Offset: 0x001E1744
		public ArchetypeInstance GetForSpawnTier(SpawnTier tier)
		{
			if (tier.GetSpawnTierFlags() == SpawnTierFlags.None)
			{
				return null;
			}
			this.Normalize();
			for (int i = 0; i < 10; i++)
			{
				LootTableEntryProfile.LootTableEntryItemProbabilityEntry entry = this.m_entries.GetEntry(null, false);
				if (entry != null && entry.Obj != null)
				{
					if (entry.Obj.IsEmpty)
					{
						return null;
					}
					if (entry.Obj.Item != null)
					{
						ArchetypeInstance archetypeInstance = entry.Obj.Item.CreateNewInstance();
						if (entry.Obj.Item.ArchetypeHasCount())
						{
							archetypeInstance.ItemData.Count = new int?(entry.Obj.Count.RandomWithinRange());
						}
						else if (entry.Obj.Item.ArchetypeHasCharges())
						{
							archetypeInstance.ItemData.Charges = new int?(entry.Obj.Count.RandomWithinRange());
						}
						return archetypeInstance;
					}
				}
			}
			return null;
		}

		// Token: 0x06005759 RID: 22361 RVA: 0x0007A391 File Offset: 0x00078591
		private void Normalize()
		{
			if (this.m_normalized)
			{
				return;
			}
			this.NormalizeInternal();
			this.m_normalized = true;
		}

		// Token: 0x0600575A RID: 22362 RVA: 0x0007A3A9 File Offset: 0x000785A9
		private void NormalizeInternal()
		{
			this.m_entries.Normalize();
		}

		// Token: 0x04004D19 RID: 19737
		[SerializeField]
		private LootTableEntryProfile.LootTableEntryProbabilityCollection m_entries;

		// Token: 0x04004D1A RID: 19738
		private bool m_normalized;

		// Token: 0x02000B11 RID: 2833
		[Serializable]
		public class LootTableEntryItemProbabilityEntry : ProbabilityEntry<LootTableEntryProfile.LootTableEntryItem>
		{
		}

		// Token: 0x02000B12 RID: 2834
		[Serializable]
		public class LootTableEntryProbabilityCollection : ProbabilityCollection<LootTableEntryProfile.LootTableEntryItemProbabilityEntry>
		{
		}

		// Token: 0x02000B13 RID: 2835
		[Serializable]
		public class LootTableEntryItem
		{
			// Token: 0x17001486 RID: 5254
			// (get) Token: 0x0600575E RID: 22366 RVA: 0x0007A3C6 File Offset: 0x000785C6
			public bool IsEmpty
			{
				get
				{
					return this.m_isEmpty;
				}
			}

			// Token: 0x17001487 RID: 5255
			// (get) Token: 0x0600575F RID: 22367 RVA: 0x0007A3CE File Offset: 0x000785CE
			public ItemArchetype Item
			{
				get
				{
					return this.m_item;
				}
			}

			// Token: 0x17001488 RID: 5256
			// (get) Token: 0x06005760 RID: 22368 RVA: 0x0007A3D6 File Offset: 0x000785D6
			public MinMaxIntRange Count
			{
				get
				{
					return this.m_count;
				}
			}

			// Token: 0x17001489 RID: 5257
			// (get) Token: 0x06005761 RID: 22369 RVA: 0x0007A3DE File Offset: 0x000785DE
			private bool m_showCount
			{
				get
				{
					return !this.m_isEmpty && this.m_item != null && (this.m_item is IStackable || this.m_item is RunicBattery);
				}
			}

			// Token: 0x06005762 RID: 22370 RVA: 0x001E3638 File Offset: 0x001E1838
			private string GetLabelText()
			{
				string result = "Count";
				if (this.m_showCount && this.m_item is RunicBattery)
				{
					result = "Charges";
				}
				return result;
			}

			// Token: 0x06005763 RID: 22371 RVA: 0x00077B72 File Offset: 0x00075D72
			private IEnumerable GetItems()
			{
				return SolOdinUtilities.GetDropdownItems<ItemArchetype>();
			}

			// Token: 0x04004D1B RID: 19739
			[SerializeField]
			private bool m_isEmpty;

			// Token: 0x04004D1C RID: 19740
			[SerializeField]
			private ItemArchetype m_item;

			// Token: 0x04004D1D RID: 19741
			[SerializeField]
			private MinMaxIntRange m_count = new MinMaxIntRange(1, 1);
		}
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using SoL.Game.Crafting;
using SoL.Game.NPCs;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Settings;
using SoL.Game.Spawning;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Loot
{
	// Token: 0x02000B14 RID: 2836
	[CreateAssetMenu(menuName = "SoL/Profiles/Loot/Table")]
	public class LootTableProfile : ScriptableObject, ILootTable
	{
		// Token: 0x06005765 RID: 22373 RVA: 0x001E3668 File Offset: 0x001E1868
		public List<ArchetypeInstance> GenerateLoot(int sampleCount, GameEntity interactionSource, SpawnTier spawnTier, IGatheringNode node)
		{
			if (LootTableProfile.m_lootList == null)
			{
				LootTableProfile.m_lootList = new List<ArchetypeInstance>(10);
				LootTableProfile.m_drawCounts = new Dictionary<LootTableProfile.LootCategory, int>(10);
			}
			this.Normalize();
			LootTableProfile.m_lootList.Clear();
			LootTableProfile.m_drawCounts.Clear();
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < sampleCount; i++)
			{
				num++;
				if (num > sampleCount * 4)
				{
					return LootTableProfile.m_lootList;
				}
				LootTableProfile.LootCategoryProbabilityEntry entry = this.m_categories.GetEntry(null, false);
				if (entry != null && entry.Obj != null)
				{
					LootTableProfile.LootCategory obj = entry.Obj;
					int num3;
					if (obj.LimitDrawCount && LootTableProfile.m_drawCounts.TryGetValue(obj, out num3) && num3 >= obj.DrawCountLimitValue)
					{
						i--;
					}
					else
					{
						if (LootTableProfile.m_drawCounts.ContainsKey(obj))
						{
							Dictionary<LootTableProfile.LootCategory, int> drawCounts = LootTableProfile.m_drawCounts;
							LootTableProfile.LootCategory key = obj;
							drawCounts[key]++;
						}
						else
						{
							LootTableProfile.m_drawCounts.Add(obj, 1);
						}
						LootTableEntryProfile profileForTier = obj.GetProfileForTier(spawnTier);
						if (profileForTier == null)
						{
							Debug.LogWarning(string.Concat(new string[]
							{
								"No suitable LootTableProfile for SpawnTier ",
								spawnTier.ToString(),
								" on ",
								base.name,
								"!"
							}));
						}
						else
						{
							ArchetypeInstance archetypeInstance = profileForTier.GetForSpawnTier(spawnTier);
							if (archetypeInstance != null)
							{
								RawComponent rawComponent;
								ArchetypeInstance archetypeInstance2;
								if (node != null && archetypeInstance.Archetype.TryGetAsType(out rawComponent) && rawComponent.CheckForGatherFailure(interactionSource, node, out archetypeInstance2))
								{
									StaticPool<ArchetypeInstance>.ReturnToPool(archetypeInstance);
									archetypeInstance = archetypeInstance2;
								}
								IDurability durability;
								if (archetypeInstance.ItemData != null && archetypeInstance.ItemData.Durability != null && archetypeInstance.Archetype.TryGetAsType(out durability))
								{
									archetypeInstance.ItemData.Durability.Absorbed = Mathf.FloorToInt((float)durability.MaxDamageAbsorption * GlobalSettings.Values.Npcs.DamageToAbsorbOnGenerate.RandomWithinRange());
								}
								archetypeInstance.Index = num2;
								LootTableProfile.m_lootList.Add(archetypeInstance);
								num2++;
							}
						}
					}
				}
			}
			return LootTableProfile.m_lootList;
		}

		// Token: 0x06005766 RID: 22374 RVA: 0x0007A42A File Offset: 0x0007862A
		private void Normalize()
		{
			if (this.m_normalized)
			{
				return;
			}
			this.NormalizeInternal();
			this.m_normalized = true;
		}

		// Token: 0x06005767 RID: 22375 RVA: 0x0007A442 File Offset: 0x00078642
		private void NormalizeInternal()
		{
			this.m_categories.Normalize();
		}

		// Token: 0x04004D1E RID: 19742
		[SerializeField]
		private LootTableProfile.LootCategoryProbabilityCollection m_categories;

		// Token: 0x04004D1F RID: 19743
		private static List<ArchetypeInstance> m_lootList;

		// Token: 0x04004D20 RID: 19744
		private static Dictionary<LootTableProfile.LootCategory, int> m_drawCounts;

		// Token: 0x04004D21 RID: 19745
		private bool m_normalized;

		// Token: 0x02000B15 RID: 2837
		[Serializable]
		public class LootCategoryProbabilityEntry : ProbabilityEntry<LootTableProfile.LootCategory>
		{
		}

		// Token: 0x02000B16 RID: 2838
		[Serializable]
		public class LootCategoryProbabilityCollection : ProbabilityCollection<LootTableProfile.LootCategoryProbabilityEntry>
		{
		}

		// Token: 0x02000B17 RID: 2839
		[Serializable]
		public class LootCategory
		{
			// Token: 0x1700148A RID: 5258
			// (get) Token: 0x0600576B RID: 22379 RVA: 0x0007A45F File Offset: 0x0007865F
			public bool LimitDrawCount
			{
				get
				{
					return this.m_limitDrawCount;
				}
			}

			// Token: 0x1700148B RID: 5259
			// (get) Token: 0x0600576C RID: 22380 RVA: 0x0007A467 File Offset: 0x00078667
			public int DrawCountLimitValue
			{
				get
				{
					return this.m_drawCountLimitValue;
				}
			}

			// Token: 0x0600576D RID: 22381 RVA: 0x001E3874 File Offset: 0x001E1A74
			public LootTableEntryProfile GetProfileForTier(SpawnTier tier)
			{
				if (this.m_tiered)
				{
					this.SortTiered();
					for (int i = this.m_tieredProfiles.Length - 1; i >= 0; i--)
					{
						if (this.m_tieredProfiles[i].MinimumTier >= tier)
						{
							return this.m_tieredProfiles[i].Profile;
						}
					}
					return null;
				}
				return this.m_profile;
			}

			// Token: 0x0600576E RID: 22382 RVA: 0x0007A46F File Offset: 0x0007866F
			private IEnumerable GetProfiles()
			{
				return SolOdinUtilities.GetDropdownItems<LootTableEntryProfile>();
			}

			// Token: 0x0600576F RID: 22383 RVA: 0x0007A476 File Offset: 0x00078676
			private void SortTiered()
			{
				if (this.m_sorted)
				{
					return;
				}
				this.SortInternal();
				this.m_sorted = true;
			}

			// Token: 0x06005770 RID: 22384 RVA: 0x0007A48E File Offset: 0x0007868E
			private void SortInternal()
			{
				Array.Sort<LootTableProfile.LootCategory.TieredLootTableEntry>(this.m_tieredProfiles, (LootTableProfile.LootCategory.TieredLootTableEntry a, LootTableProfile.LootCategory.TieredLootTableEntry b) => a.MinimumTier.CompareTo(b.MinimumTier));
			}

			// Token: 0x04004D22 RID: 19746
			[SerializeField]
			private string m_label;

			// Token: 0x04004D23 RID: 19747
			[SerializeField]
			private bool m_tiered;

			// Token: 0x04004D24 RID: 19748
			[SerializeField]
			private LootTableEntryProfile m_profile;

			// Token: 0x04004D25 RID: 19749
			[SerializeField]
			private LootTableProfile.LootCategory.TieredLootTableEntry[] m_tieredProfiles;

			// Token: 0x04004D26 RID: 19750
			[SerializeField]
			private bool m_limitDrawCount;

			// Token: 0x04004D27 RID: 19751
			[Range(1f, 20f)]
			[SerializeField]
			private int m_drawCountLimitValue = 1;

			// Token: 0x04004D28 RID: 19752
			private bool m_sorted;

			// Token: 0x02000B18 RID: 2840
			[Serializable]
			private class TieredLootTableEntry
			{
				// Token: 0x1700148C RID: 5260
				// (get) Token: 0x06005772 RID: 22386 RVA: 0x0007A4C9 File Offset: 0x000786C9
				public SpawnTier MinimumTier
				{
					get
					{
						return this.m_minimumTier;
					}
				}

				// Token: 0x1700148D RID: 5261
				// (get) Token: 0x06005773 RID: 22387 RVA: 0x0007A4D1 File Offset: 0x000786D1
				public LootTableEntryProfile Profile
				{
					get
					{
						return this.m_profile;
					}
				}

				// Token: 0x06005774 RID: 22388 RVA: 0x0007A46F File Offset: 0x0007866F
				private IEnumerable GetProfiles()
				{
					return SolOdinUtilities.GetDropdownItems<LootTableEntryProfile>();
				}

				// Token: 0x04004D29 RID: 19753
				[SerializeField]
				private SpawnTier m_minimumTier = SpawnTier.Weak;

				// Token: 0x04004D2A RID: 19754
				[SerializeField]
				private LootTableEntryProfile m_profile;
			}
		}
	}
}

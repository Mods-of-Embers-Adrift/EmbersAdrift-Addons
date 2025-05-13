using System;
using System.Collections.Generic;
using SoL.Game.Crafting;
using SoL.Game.NPCs;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Settings;
using SoL.Game.Spawning;
using SoL.Managers;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using SoL.Utilities.Logging;
using UnityEngine;

namespace SoL.Game.Loot
{
	// Token: 0x02000B1A RID: 2842
	[CreateAssetMenu(menuName = "SoL/Profiles/Loot/Table V2")]
	public class LootTable : ScriptableObject, ILootTable
	{
		// Token: 0x1700148E RID: 5262
		// (get) Token: 0x06005779 RID: 22393 RVA: 0x0007A4F5 File Offset: 0x000786F5
		internal virtual bool HasGuaranteed
		{
			get
			{
				return this.m_hasGuaranteed;
			}
		}

		// Token: 0x1700148F RID: 5263
		// (get) Token: 0x0600577A RID: 22394 RVA: 0x0007A4FD File Offset: 0x000786FD
		internal virtual LootTableItem[] GuaranteedAll
		{
			get
			{
				return this.m_guaranteedAll;
			}
		}

		// Token: 0x17001490 RID: 5264
		// (get) Token: 0x0600577B RID: 22395 RVA: 0x0007A505 File Offset: 0x00078705
		internal virtual LootTableItemWithOverrideProbabilityCollection Guaranteed
		{
			get
			{
				return this.m_guaranteed;
			}
		}

		// Token: 0x17001491 RID: 5265
		// (get) Token: 0x0600577C RID: 22396 RVA: 0x0007A50D File Offset: 0x0007870D
		internal virtual LootTable.LootTableCategoryProbabilityCollection Categories
		{
			get
			{
				return this.m_categories;
			}
		}

		// Token: 0x0600577D RID: 22397 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void InitializeTable()
		{
		}

		// Token: 0x0600577E RID: 22398 RVA: 0x0007A515 File Offset: 0x00078715
		public List<ArchetypeInstance> GenerateLoot(int sampleCount, GameEntity interactionSource, SpawnTier spawnTier, IGatheringNode node)
		{
			return this.GenerateLoot(sampleCount, interactionSource, spawnTier, node, this.m_allowDuplicates, true);
		}

		// Token: 0x0600577F RID: 22399 RVA: 0x0007A529 File Offset: 0x00078729
		private static void ClearData()
		{
			HashSet<UniqueId> lootListArchetypeIds = LootTable.m_lootListArchetypeIds;
			if (lootListArchetypeIds != null)
			{
				lootListArchetypeIds.Clear();
			}
			HashSet<UniqueId> lootListInstanceIds = LootTable.m_lootListInstanceIds;
			if (lootListInstanceIds != null)
			{
				lootListInstanceIds.Clear();
			}
			Dictionary<LootTable.LootTableCategory, int> drawCounts = LootTable.m_drawCounts;
			if (drawCounts != null)
			{
				drawCounts.Clear();
			}
			LootTable.m_index = 0;
		}

		// Token: 0x06005780 RID: 22400 RVA: 0x001E38F8 File Offset: 0x001E1AF8
		private List<ArchetypeInstance> GenerateLoot(int sampleCount, GameEntity interactionSource, SpawnTier spawnTier, IGatheringNode node, bool allowDuplicates, bool considerDropCountRestrictions)
		{
			this.InitializeTable();
			if (LootTable.m_lootList == null)
			{
				LootTable.m_lootList = new List<ArchetypeInstance>(10);
				LootTable.m_lootListArchetypeIds = new HashSet<UniqueId>(default(UniqueIdComparer));
				LootTable.m_lootListInstanceIds = new HashSet<UniqueId>(default(UniqueIdComparer));
				LootTable.m_drawCounts = new Dictionary<LootTable.LootTableCategory, int>(10);
			}
			LootTable.m_lootList.Clear();
			LootTable.ClearData();
			if (this.HasGuaranteed)
			{
				if (this.GuaranteedAll != null && this.GuaranteedAll.Length != 0)
				{
					for (int i = 0; i < this.GuaranteedAll.Length; i++)
					{
						this.AddLootTableItemToLoot(this.GuaranteedAll[i], interactionSource, node, allowDuplicates, considerDropCountRestrictions);
					}
				}
				if (this.Guaranteed != null && this.Guaranteed.Count > 0)
				{
					LootTableItemWithOverrideProbabilityCollection guaranteed = this.Guaranteed;
					LootTableItem lootTableItem;
					if (guaranteed == null)
					{
						lootTableItem = null;
					}
					else
					{
						LootTableItemWithOverrideProbabilityEntry entry = guaranteed.GetEntry(null, true);
						if (entry == null)
						{
							lootTableItem = null;
						}
						else
						{
							LootTableItemCollectionWithOverride obj = entry.Obj;
							lootTableItem = ((obj != null) ? obj.GetLootTableItem() : null);
						}
					}
					LootTableItem lootTableItem2 = lootTableItem;
					this.AddLootTableItemToLoot(lootTableItem2, interactionSource, node, allowDuplicates, considerDropCountRestrictions);
				}
			}
			int num = 0;
			for (int j = 0; j < sampleCount; j++)
			{
				num++;
				if (num > sampleCount * 8)
				{
					return LootTable.m_lootList;
				}
				LootTable.LootTableCategoryProbabilityEntry entry2 = this.Categories.GetEntry(null, true);
				if (entry2 != null && entry2.Obj != null)
				{
					LootTable.LootTableCategory obj2 = entry2.Obj;
					int num2;
					if (obj2.LimitDrawCount && LootTable.m_drawCounts.TryGetValue(obj2, out num2) && num2 >= obj2.DrawCountLimitValue)
					{
						j--;
					}
					else
					{
						if (LootTable.m_drawCounts.ContainsKey(obj2))
						{
							Dictionary<LootTable.LootTableCategory, int> drawCounts = LootTable.m_drawCounts;
							LootTable.LootTableCategory key = obj2;
							drawCounts[key]++;
						}
						else
						{
							LootTable.m_drawCounts.Add(obj2, 1);
						}
						LootTableItem lootTableItem3 = obj2.GetLootTableItem();
						if (!this.AddLootTableItemToLoot(lootTableItem3, interactionSource, node, allowDuplicates, considerDropCountRestrictions) && !obj2.IsEmpty)
						{
							j--;
						}
					}
				}
			}
			LootTable.ClearData();
			return LootTable.m_lootList;
		}

		// Token: 0x06005781 RID: 22401 RVA: 0x001E3AF0 File Offset: 0x001E1CF0
		private bool AddLootTableItemToLoot(LootTableItem lootTableItem, GameEntity interactionSource, IGatheringNode node, bool allowDuplicates, bool considerDropCountRestrictions)
		{
			if (lootTableItem == null || lootTableItem.Item == null)
			{
				return false;
			}
			if (lootTableItem.Item.RestrictDropCount && considerDropCountRestrictions)
			{
				if (LootTable.m_previousDrops == null)
				{
					LootTable.m_previousDrops = new Dictionary<UniqueId, LootTable.PreviousDropData>(default(UniqueIdComparer));
				}
				DateTime utcNow = DateTime.UtcNow;
				LootTable.PreviousDropData previousDropData;
				if (LootTable.m_previousDrops.TryGetValue(lootTableItem.Item.Id, out previousDropData))
				{
					float num = (float)lootTableItem.Item.SlidingTimeFrame * 60f;
					int maxDropCount = lootTableItem.Item.MaxDropCount;
					List<DateTime> previousTimes = previousDropData.PreviousTimes;
					for (int i = previousTimes.Count - 1; i >= 0; i--)
					{
						if ((utcNow - previousTimes[i]).TotalSeconds >= (double)num)
						{
							previousTimes.RemoveAt(i);
						}
					}
					if (previousTimes.Count >= maxDropCount)
					{
						if (Time.frameCount > previousDropData.LastFrameHit)
						{
							this.LogRestrictedItemDrop(lootTableItem, maxDropCount, true);
						}
						return false;
					}
					previousDropData.LastFrameHit = Time.frameCount;
					previousTimes.Add(utcNow);
					this.LogRestrictedItemDrop(lootTableItem, previousTimes.Count, false);
				}
				else
				{
					if (Time.realtimeSinceStartup < (float)lootTableItem.Item.SlidingTimeFrame * 60f)
					{
						return false;
					}
					LootTable.m_previousDrops.Add(lootTableItem.Item.Id, new LootTable.PreviousDropData
					{
						LastFrameHit = Time.frameCount,
						PreviousTimes = new List<DateTime>(lootTableItem.Item.MaxDropCount)
						{
							utcNow
						}
					});
					this.LogRestrictedItemDrop(lootTableItem, 1, false);
				}
			}
			ArchetypeInstance archetypeInstance = lootTableItem.Item.CreateNewInstance();
			if (archetypeInstance == null)
			{
				return false;
			}
			if (lootTableItem.Item.ArchetypeHasCount())
			{
				archetypeInstance.ItemData.Count = new int?(lootTableItem.Count.RandomWithinRange());
			}
			if (LootTable.m_lootListInstanceIds.Contains(archetypeInstance.InstanceId))
			{
				Debug.LogWarning("InstanceId GUARD! Duplicate InstanceId attempted to add to the LootList!  " + archetypeInstance.Archetype.name + " of ArchetypeId " + archetypeInstance.ArchetypeId.ToString());
				StaticPool<ArchetypeInstance>.ReturnToPool(archetypeInstance);
				return false;
			}
			if (!allowDuplicates && LootTable.m_lootListArchetypeIds.Contains(archetypeInstance.ArchetypeId))
			{
				StaticPool<ArchetypeInstance>.ReturnToPool(archetypeInstance);
				return false;
			}
			RawComponent rawComponent;
			ArchetypeInstance archetypeInstance2;
			if (node != null && archetypeInstance.Archetype.TryGetAsType(out rawComponent) && rawComponent.CheckForGatherFailure(interactionSource, node, out archetypeInstance2))
			{
				StaticPool<ArchetypeInstance>.ReturnToPool(archetypeInstance);
				archetypeInstance = archetypeInstance2;
			}
			if (archetypeInstance.ItemData != null && archetypeInstance.ItemData.Count != null && LootTable.m_lootListArchetypeIds.Contains(archetypeInstance.ArchetypeId))
			{
				for (int j = 0; j < LootTable.m_lootList.Count; j++)
				{
					if (LootTable.m_lootList[j].ArchetypeId == archetypeInstance.ArchetypeId && LootTable.m_lootList[j].ItemData != null && LootTable.m_lootList[j].ItemData.Count != null)
					{
						LootTable.m_lootList[j].ItemData.Count += archetypeInstance.ItemData.Count;
						StaticPool<ArchetypeInstance>.ReturnToPool(archetypeInstance);
						return true;
					}
				}
			}
			IDurability durability;
			if (archetypeInstance.ItemData != null && archetypeInstance.ItemData.Durability != null && archetypeInstance.Archetype.TryGetAsType(out durability))
			{
				archetypeInstance.ItemData.Durability.Absorbed = Mathf.FloorToInt((float)durability.MaxDamageAbsorption * GlobalSettings.Values.Npcs.DamageToAbsorbOnGenerate.RandomWithinRange());
			}
			archetypeInstance.Index = LootTable.m_index;
			LootTable.m_lootList.Add(archetypeInstance);
			LootTable.m_lootListArchetypeIds.Add(archetypeInstance.ArchetypeId);
			LootTable.m_lootListInstanceIds.Add(archetypeInstance.InstanceId);
			LootTable.m_index++;
			return true;
		}

		// Token: 0x06005782 RID: 22402 RVA: 0x001E3EFC File Offset: 0x001E20FC
		private void LogRestrictedItemDrop(LootTableItem lootTableItem, int currentDropCount, bool wasRestricted)
		{
			if (wasRestricted)
			{
				Debug.Log(string.Concat(new string[]
				{
					"[",
					DateTime.Now.ToString("MM.dd.HH.mm.ss"),
					"] Restricted loot count on ",
					lootTableItem.Item.DisplayName,
					" (ID=",
					lootTableItem.Item.Id.ToString(),
					")!"
				}));
			}
			if (!Application.isPlaying || LocalZoneManager.ZoneRecord == null)
			{
				return;
			}
			if (LootTable.m_lootCountRestrictionObjectArray == null)
			{
				LootTable.m_lootCountRestrictionObjectArray = new object[7];
			}
			LootTable.m_lootCountRestrictionObjectArray[0] = LocalZoneManager.ZoneRecord.ZoneId;
			LootTable.m_lootCountRestrictionObjectArray[1] = (wasRestricted ? "Restricted" : "Dropped");
			LootTable.m_lootCountRestrictionObjectArray[2] = lootTableItem.Item.DisplayName;
			LootTable.m_lootCountRestrictionObjectArray[3] = lootTableItem.Item.Id.ToString();
			LootTable.m_lootCountRestrictionObjectArray[4] = currentDropCount;
			LootTable.m_lootCountRestrictionObjectArray[5] = lootTableItem.Item.MaxDropCount;
			LootTable.m_lootCountRestrictionObjectArray[6] = lootTableItem.Item.SlidingTimeFrame;
			string messageTemplate = wasRestricted ? "[{@ZoneId}] {@Event} loot count on {@ItemName} (ID={@ArchetypeId}, {@DroppedCount}/{@MaxCount} in {@Timeframe} minutes)!" : "[{@ZoneId}] {@Event} {@ItemName} (ID={@ArchetypeId}, {@DroppedCount}/{@MaxCount} in {@Timeframe} minutes)!";
			SolDebug.LogToIndex(LogLevel.Information, LogIndex.LootRestriction, messageTemplate, LootTable.m_lootCountRestrictionObjectArray);
		}

		// Token: 0x04004D2D RID: 19757
		private const string kLootCountItemDroppedTemplate = "[{@ZoneId}] {@Event} {@ItemName} (ID={@ArchetypeId}, {@DroppedCount}/{@MaxCount} in {@Timeframe} minutes)!";

		// Token: 0x04004D2E RID: 19758
		private const string kLootCountRestrictionTemplate = "[{@ZoneId}] {@Event} loot count on {@ItemName} (ID={@ArchetypeId}, {@DroppedCount}/{@MaxCount} in {@Timeframe} minutes)!";

		// Token: 0x04004D2F RID: 19759
		private const int kLootCountRestrictionObjectArraySize = 7;

		// Token: 0x04004D30 RID: 19760
		private static object[] m_lootCountRestrictionObjectArray;

		// Token: 0x04004D31 RID: 19761
		private const string kGuaranteedGroup = "Guaranteed";

		// Token: 0x04004D32 RID: 19762
		private const string kStandardGroup = "Standard";

		// Token: 0x04004D33 RID: 19763
		private const string kDebugGroup = "Debug";

		// Token: 0x04004D34 RID: 19764
		private static List<ArchetypeInstance> m_lootList;

		// Token: 0x04004D35 RID: 19765
		private static HashSet<UniqueId> m_lootListArchetypeIds;

		// Token: 0x04004D36 RID: 19766
		private static HashSet<UniqueId> m_lootListInstanceIds;

		// Token: 0x04004D37 RID: 19767
		private static Dictionary<LootTable.LootTableCategory, int> m_drawCounts;

		// Token: 0x04004D38 RID: 19768
		private static int m_index;

		// Token: 0x04004D39 RID: 19769
		private static Dictionary<UniqueId, LootTable.PreviousDropData> m_previousDrops;

		// Token: 0x04004D3A RID: 19770
		[SerializeField]
		private bool m_allowDuplicates;

		// Token: 0x04004D3B RID: 19771
		[SerializeField]
		private bool m_hasGuaranteed;

		// Token: 0x04004D3C RID: 19772
		[SerializeField]
		private LootTableItem[] m_guaranteedAll;

		// Token: 0x04004D3D RID: 19773
		[SerializeField]
		private LootTableItemWithOverrideProbabilityCollection m_guaranteed;

		// Token: 0x04004D3E RID: 19774
		[SerializeField]
		private LootTable.LootTableCategoryProbabilityCollection m_categories;

		// Token: 0x04004D3F RID: 19775
		[SerializeField]
		private bool m_debugAllowDuplicates = true;

		// Token: 0x04004D40 RID: 19776
		[SerializeField]
		private bool m_debugConsiderDropCountRestrictions;

		// Token: 0x04004D41 RID: 19777
		[Range(1f, 1000f)]
		[Tooltip("How many item draws per sample.")]
		[SerializeField]
		private int m_debugDrawCount = 1;

		// Token: 0x04004D42 RID: 19778
		[Range(1f, 1000f)]
		[Tooltip("How many times to sample the loot table. 1x sample is equivalent to looting 1 NPC")]
		[SerializeField]
		private int m_debugSampleCount = 1;

		// Token: 0x02000B1B RID: 2843
		[Serializable]
		internal class LootTableCategoryProbabilityEntry : ProbabilityEntry<LootTable.LootTableCategory>
		{
		}

		// Token: 0x02000B1C RID: 2844
		[Serializable]
		internal class LootTableCategoryProbabilityCollection : ProbabilityCollection<LootTable.LootTableCategoryProbabilityEntry>
		{
		}

		// Token: 0x02000B1D RID: 2845
		[Serializable]
		internal class LootTableCategory
		{
			// Token: 0x17001492 RID: 5266
			// (get) Token: 0x06005786 RID: 22406 RVA: 0x0007A58E File Offset: 0x0007878E
			public bool LimitDrawCount
			{
				get
				{
					return this.m_limitDrawCount && !this.m_isEmpty;
				}
			}

			// Token: 0x17001493 RID: 5267
			// (get) Token: 0x06005787 RID: 22407 RVA: 0x0007A5A3 File Offset: 0x000787A3
			public bool IsEmpty
			{
				get
				{
					return this.m_isEmpty;
				}
			}

			// Token: 0x17001494 RID: 5268
			// (get) Token: 0x06005788 RID: 22408 RVA: 0x0007A5AB File Offset: 0x000787AB
			public int DrawCountLimitValue
			{
				get
				{
					return this.m_drawCountLimitValue;
				}
			}

			// Token: 0x06005789 RID: 22409 RVA: 0x0007A5B3 File Offset: 0x000787B3
			public LootTableItem GetLootTableItem()
			{
				if (this.m_isEmpty)
				{
					return null;
				}
				LootTableItemWithOverrideProbabilityCollection collection = this.m_collection;
				if (collection == null)
				{
					return null;
				}
				LootTableItemWithOverrideProbabilityEntry entry = collection.GetEntry(null, false);
				if (entry == null)
				{
					return null;
				}
				LootTableItemCollectionWithOverride obj = entry.Obj;
				if (obj == null)
				{
					return null;
				}
				return obj.GetLootTableItem();
			}

			// Token: 0x0600578A RID: 22410 RVA: 0x001E4054 File Offset: 0x001E2254
			public void OnValidate()
			{
				LootTableItemWithOverrideProbabilityCollection collection = this.m_collection;
				if (collection != null)
				{
					collection.Normalize();
				}
				LootTableItemWithOverrideProbabilityCollection collection2 = this.m_collection;
				if (((collection2 != null) ? collection2.Entries : null) == null)
				{
					return;
				}
				for (int i = 0; i < this.m_collection.Entries.Length; i++)
				{
					LootTableItemWithOverrideProbabilityEntry lootTableItemWithOverrideProbabilityEntry = this.m_collection.Entries[i];
					if (lootTableItemWithOverrideProbabilityEntry != null)
					{
						LootTableItemCollectionWithOverride obj = lootTableItemWithOverrideProbabilityEntry.Obj;
						if (obj != null)
						{
							obj.OnValidate();
						}
					}
				}
			}

			// Token: 0x17001495 RID: 5269
			// (get) Token: 0x0600578B RID: 22411 RVA: 0x0007A5E8 File Offset: 0x000787E8
			private bool m_showLoad
			{
				get
				{
					return this.m_toLoad != null && this.m_toLoad.Length != 0;
				}
			}

			// Token: 0x04004D43 RID: 19779
			[SerializeField]
			private string m_label;

			// Token: 0x04004D44 RID: 19780
			[SerializeField]
			private bool m_isEmpty;

			// Token: 0x04004D45 RID: 19781
			[SerializeField]
			private bool m_limitDrawCount;

			// Token: 0x04004D46 RID: 19782
			[Range(1f, 20f)]
			[SerializeField]
			private int m_drawCountLimitValue = 1;

			// Token: 0x04004D47 RID: 19783
			[SerializeField]
			private LootTableItemWithOverrideProbabilityCollection m_collection;

			// Token: 0x04004D48 RID: 19784
			private const int kLoadOrder = 99999;

			// Token: 0x04004D49 RID: 19785
			[SerializeField]
			private LootTableItemCollectionScriptable[] m_toLoad;
		}

		// Token: 0x02000B1E RID: 2846
		private class PreviousDropData
		{
			// Token: 0x04004D4A RID: 19786
			public int LastFrameHit;

			// Token: 0x04004D4B RID: 19787
			public List<DateTime> PreviousTimes;
		}
	}
}

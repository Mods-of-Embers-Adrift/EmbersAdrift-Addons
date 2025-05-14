using System;
using System.Collections.Generic;
using SoL.Game.EffectSystem;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Spawning;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Loot
{
	// Token: 0x02000AFF RID: 2815
	[CreateAssetMenu(menuName = "SoL/Dynamic Loot Table")]
	public class DynamicLootTable : ScriptableObject
	{
		// Token: 0x06005707 RID: 22279 RVA: 0x001E25C0 File Offset: 0x001E07C0
		public void Sample()
		{
			DynamicProbabilityCollection<DynamicLootTable.DataEntry> fromPool = StaticPool<DynamicProbabilityCollection<DynamicLootTable.DataEntry>>.GetFromPool();
			List<string> fromPool2 = StaticListPool<string>.GetFromPool();
			for (int i = 0; i < this.m_entries.Length; i++)
			{
				if (this.m_entries[i].Qualifies(this.m_hitType, this.m_lureTypes, this.m_yearFraction, this.m_dayFraction))
				{
					fromPool.Add(new DynamicLootTable.DataEntry(this.m_entries[i]));
				}
			}
			for (int j = 0; j < this.m_sampleCount; j++)
			{
				DynamicLootTable.DataEntry dataEntry;
				if (fromPool.TryGetEntry(out dataEntry, null))
				{
					fromPool2.Add(dataEntry.Item.DisplayName);
				}
			}
			Debug.Log(string.Join(", ", fromPool2));
			StaticListPool<string>.ReturnToPool(fromPool2);
			StaticPool<DynamicProbabilityCollection<DynamicLootTable.DataEntry>>.ReturnToPool(fromPool);
		}

		// Token: 0x06005708 RID: 22280 RVA: 0x001E2670 File Offset: 0x001E0870
		public void MultiSample()
		{
			for (int i = 0; i < 1000; i++)
			{
				this.Sample();
			}
		}

		// Token: 0x04004CBD RID: 19645
		[SerializeField]
		private DynamicLootTable.Entry[] m_entries;

		// Token: 0x04004CBE RID: 19646
		private const string kDebugGroup = "Debug";

		// Token: 0x04004CBF RID: 19647
		[Range(1f, 100f)]
		[SerializeField]
		private int m_sampleCount = 1;

		// Token: 0x04004CC0 RID: 19648
		[SerializeField]
		private HitType m_hitType = HitType.Normal;

		// Token: 0x04004CC1 RID: 19649
		[SerializeField]
		private DynamicLootTable.LureType m_lureTypes;

		// Token: 0x04004CC2 RID: 19650
		[Range(0f, 1f)]
		[SerializeField]
		private float m_yearFraction = 0.5f;

		// Token: 0x04004CC3 RID: 19651
		[Range(0f, 1f)]
		[SerializeField]
		private float m_dayFraction = 0.5f;

		// Token: 0x02000B00 RID: 2816
		[Flags]
		public enum HitTypeFlags
		{
			// Token: 0x04004CC5 RID: 19653
			None = 0,
			// Token: 0x04004CC6 RID: 19654
			Miss = 1,
			// Token: 0x04004CC7 RID: 19655
			Glancing = 2,
			// Token: 0x04004CC8 RID: 19656
			Normal = 4,
			// Token: 0x04004CC9 RID: 19657
			Heavy = 8,
			// Token: 0x04004CCA RID: 19658
			Critical = 16,
			// Token: 0x04004CCB RID: 19659
			All = 31
		}

		// Token: 0x02000B01 RID: 2817
		[Flags]
		public enum LureType
		{
			// Token: 0x04004CCD RID: 19661
			None = 0,
			// Token: 0x04004CCE RID: 19662
			Zero = 1,
			// Token: 0x04004CCF RID: 19663
			One = 2,
			// Token: 0x04004CD0 RID: 19664
			Two = 4,
			// Token: 0x04004CD1 RID: 19665
			Three = 8,
			// Token: 0x04004CD2 RID: 19666
			All = 15
		}

		// Token: 0x02000B02 RID: 2818
		[Serializable]
		private class Entry
		{
			// Token: 0x1700146D RID: 5229
			// (get) Token: 0x0600570A RID: 22282 RVA: 0x00079F5F File Offset: 0x0007815F
			public ItemArchetype Item
			{
				get
				{
					return this.m_item;
				}
			}

			// Token: 0x1700146E RID: 5230
			// (get) Token: 0x0600570B RID: 22283 RVA: 0x00079F67 File Offset: 0x00078167
			public float Probability
			{
				get
				{
					return this.m_probability;
				}
			}

			// Token: 0x0600570C RID: 22284 RVA: 0x001E2694 File Offset: 0x001E0894
			public bool Qualifies(HitType hitType, DynamicLootTable.LureType lureTypes, float yearFraction, float dayFraction)
			{
				DynamicLootTable.HitTypeFlags hitTypeFlags = DynamicLootTable.HitTypeFlags.None;
				switch (hitType)
				{
				case HitType.Miss:
					hitTypeFlags = DynamicLootTable.HitTypeFlags.Miss;
					break;
				case HitType.Glancing:
					hitTypeFlags = DynamicLootTable.HitTypeFlags.Glancing;
					break;
				case HitType.Normal:
					hitTypeFlags = DynamicLootTable.HitTypeFlags.Normal;
					break;
				case HitType.Heavy:
					hitTypeFlags = DynamicLootTable.HitTypeFlags.Heavy;
					break;
				case HitType.Critical:
					hitTypeFlags = DynamicLootTable.HitTypeFlags.Critical;
					break;
				}
				bool flag = (this.m_hitTypes & hitTypeFlags) == hitTypeFlags;
				bool flag2 = (this.m_lureTypes & lureTypes) == lureTypes;
				bool flag3 = yearFraction >= this.m_yearRange.x && yearFraction <= this.m_yearRange.y;
				bool flag4 = dayFraction >= this.m_dayRange.x && dayFraction <= this.m_dayRange.y;
				return flag && flag2 && flag3 && flag4;
			}

			// Token: 0x04004CD3 RID: 19667
			[SerializeField]
			private ItemArchetype m_item;

			// Token: 0x04004CD4 RID: 19668
			[Range(0f, 1f)]
			[SerializeField]
			private float m_probability = 1f;

			// Token: 0x04004CD5 RID: 19669
			[SerializeField]
			private DynamicLootTable.HitTypeFlags m_hitTypes = DynamicLootTable.HitTypeFlags.All;

			// Token: 0x04004CD6 RID: 19670
			[SerializeField]
			private DynamicLootTable.LureType m_lureTypes = DynamicLootTable.LureType.All;

			// Token: 0x04004CD7 RID: 19671
			[SerializeField]
			private Vector2 m_yearRange = new Vector2(0f, 1f);

			// Token: 0x04004CD8 RID: 19672
			[SerializeField]
			private Vector2 m_dayRange = new Vector2(0f, 1f);
		}

		// Token: 0x02000B03 RID: 2819
		private struct DataEntry : IProbabilityEntry
		{
			// Token: 0x1700146F RID: 5231
			// (get) Token: 0x0600570E RID: 22286 RVA: 0x00079F6F File Offset: 0x0007816F
			// (set) Token: 0x0600570F RID: 22287 RVA: 0x00079F77 File Offset: 0x00078177
			public float Probability { readonly get; private set; }

			// Token: 0x17001470 RID: 5232
			// (get) Token: 0x06005710 RID: 22288 RVA: 0x00079F80 File Offset: 0x00078180
			// (set) Token: 0x06005711 RID: 22289 RVA: 0x00079F88 File Offset: 0x00078188
			public float Threshold { readonly get; set; }

			// Token: 0x17001471 RID: 5233
			// (get) Token: 0x06005712 RID: 22290 RVA: 0x00079F91 File Offset: 0x00078191
			// (set) Token: 0x06005713 RID: 22291 RVA: 0x00079F99 File Offset: 0x00078199
			public float NormalizedProbability { readonly get; private set; }

			// Token: 0x06005714 RID: 22292 RVA: 0x00079FA2 File Offset: 0x000781A2
			public DataEntry(DynamicLootTable.Entry entry)
			{
				this.Item = entry.Item;
				this.Probability = entry.Probability;
				this.Threshold = 1f;
				this.NormalizedProbability = 0f;
			}

			// Token: 0x06005715 RID: 22293 RVA: 0x00079FD2 File Offset: 0x000781D2
			public void SetTotalProbability(float total)
			{
				this.NormalizedProbability = this.Probability / total;
			}

			// Token: 0x04004CD9 RID: 19673
			public ItemArchetype Item;
		}
	}
}

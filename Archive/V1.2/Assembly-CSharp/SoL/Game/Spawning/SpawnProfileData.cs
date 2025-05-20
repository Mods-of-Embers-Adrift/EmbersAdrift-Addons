using System;
using System.Collections;
using SoL.Utilities;
using UnityEngine;
using UnityEngine.Serialization;

namespace SoL.Game.Spawning
{
	// Token: 0x020006D3 RID: 1747
	[Serializable]
	public class SpawnProfileData
	{
		// Token: 0x17000BA3 RID: 2979
		// (get) Token: 0x06003503 RID: 13571 RVA: 0x000644B8 File Offset: 0x000626B8
		private bool m_showPrimary
		{
			get
			{
				return this.m_sampleType == SpawnProfileData.SampleType.RandomDraw || this.m_sampleType == SpawnProfileData.SampleType.RandomBagDraw;
			}
		}

		// Token: 0x17000BA4 RID: 2980
		// (get) Token: 0x06003504 RID: 13572 RVA: 0x000644CD File Offset: 0x000626CD
		private bool m_showAlternateRandom
		{
			get
			{
				return this.m_condition == DayNightSpawnCondition.SeparateDayNight && (this.m_sampleType == SpawnProfileData.SampleType.RandomDraw || this.m_sampleType == SpawnProfileData.SampleType.RandomBagDraw);
			}
		}

		// Token: 0x17000BA5 RID: 2981
		// (get) Token: 0x06003505 RID: 13573 RVA: 0x000644ED File Offset: 0x000626ED
		private bool m_showAlternateCount
		{
			get
			{
				return this.m_condition == DayNightSpawnCondition.SeparateDayNight && this.m_sampleType == SpawnProfileData.SampleType.FixedDistribution;
			}
		}

		// Token: 0x17000BA6 RID: 2982
		// (get) Token: 0x06003506 RID: 13574 RVA: 0x00064503 File Offset: 0x00062703
		internal DayNightSpawnCondition DayNightCondition
		{
			get
			{
				return this.m_condition;
			}
		}

		// Token: 0x17000BA7 RID: 2983
		// (get) Token: 0x06003507 RID: 13575 RVA: 0x0006450B File Offset: 0x0006270B
		internal bool IsFixedDistribution
		{
			get
			{
				return this.m_sampleType == SpawnProfileData.SampleType.FixedDistribution;
			}
		}

		// Token: 0x06003508 RID: 13576 RVA: 0x0016695C File Offset: 0x00164B5C
		internal int GetFixedDistributionTargetPopulation(bool isDay)
		{
			if (this.m_sampleType != SpawnProfileData.SampleType.FixedDistribution)
			{
				return 0;
			}
			SpawnProfileData.SpawnProfileCount[] array = (this.m_condition == DayNightSpawnCondition.SeparateDayNight && !isDay) ? this.m_alternateCounts : this.m_primaryCounts;
			int num = 0;
			for (int i = 0; i < array.Length; i++)
			{
				num += array[i].Count;
			}
			return num;
		}

		// Token: 0x06003509 RID: 13577 RVA: 0x00064516 File Offset: 0x00062716
		internal void NormalizeProbabilities()
		{
			this.m_primary.Normalize();
			this.m_alternate.Normalize();
		}

		// Token: 0x0600350A RID: 13578 RVA: 0x0006452E File Offset: 0x0006272E
		internal void ReplaceData(SpawnProfileData other)
		{
			this.m_condition = other.m_condition;
			this.m_primary = other.m_primary;
			this.m_alternate = other.m_alternate;
			this.m_primaryCounts = other.m_primaryCounts;
			this.m_alternateCounts = other.m_alternateCounts;
		}

		// Token: 0x0600350B RID: 13579 RVA: 0x001669AC File Offset: 0x00164BAC
		internal void RemoveActiveSpawn(SpawnController.SpawnTracker tracker)
		{
			if (this.m_sampleType != SpawnProfileData.SampleType.FixedDistribution)
			{
				return;
			}
			SpawnProfileData.SpawnProfileCount[] array = (this.m_condition == DayNightSpawnCondition.SeparateDayNight && !tracker.IsDay) ? this.m_alternateCounts : this.m_primaryCounts;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Profile == tracker.Profile)
				{
					array[i].ActiveCount = Mathf.Clamp(array[i].ActiveCount - 1, 0, int.MaxValue);
					return;
				}
			}
		}

		// Token: 0x0600350C RID: 13580 RVA: 0x00166A24 File Offset: 0x00164C24
		internal SpawnProfile GetSpawnProfile(bool isDay)
		{
			switch (this.m_sampleType)
			{
			case SpawnProfileData.SampleType.RandomDraw:
			{
				SpawnProfileProbabilityEntry entry = ((this.m_condition == DayNightSpawnCondition.SeparateDayNight && !isDay) ? this.m_alternate : this.m_primary).GetEntry(null, false);
				if (entry == null)
				{
					return null;
				}
				return entry.Obj;
			}
			case SpawnProfileData.SampleType.FixedDistribution:
			{
				SpawnProfileData.SpawnProfileCount[] array = (this.m_condition == DayNightSpawnCondition.SeparateDayNight && !isDay) ? this.m_alternateCounts : this.m_primaryCounts;
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].ActiveCount < array[i].Count)
					{
						array[i].ActiveCount++;
						return array[i].Profile;
					}
				}
				return null;
			}
			case SpawnProfileData.SampleType.RandomBagDraw:
			{
				if (this.m_primaryDrawBag == null)
				{
					this.m_primaryDrawBag = new SpawnProfileDrawBag(this.m_primary, this.m_sampleSize, this.m_reshuffleThreshold);
					if (this.m_condition == DayNightSpawnCondition.SeparateDayNight)
					{
						this.m_alternateDrawBag = new SpawnProfileDrawBag(this.m_alternate, this.m_sampleSize, this.m_reshuffleThreshold);
					}
				}
				SpawnProfileDrawBag spawnProfileDrawBag = (this.m_condition == DayNightSpawnCondition.SeparateDayNight && !isDay) ? this.m_alternateDrawBag : this.m_primaryDrawBag;
				if (spawnProfileDrawBag == null)
				{
					return null;
				}
				return spawnProfileDrawBag.GetItem();
			}
			default:
				return null;
			}
		}

		// Token: 0x0400332F RID: 13103
		public const string kGroupName = "Spawn Profiles";

		// Token: 0x04003330 RID: 13104
		private const string kGetPrimaryLabel = "$GetPrimarySpawnProfileLabelName";

		// Token: 0x04003331 RID: 13105
		private const string kGetAlternateLabel = "$GetAlternateSpawnProfileLabelName";

		// Token: 0x04003332 RID: 13106
		private const string kCopyLabel = "Copy Day To Night";

		// Token: 0x04003333 RID: 13107
		[FormerlySerializedAs("m_dayNightConfig")]
		[SerializeField]
		private DayNightSpawnCondition m_condition;

		// Token: 0x04003334 RID: 13108
		[SerializeField]
		private SpawnProfileData.SampleType m_sampleType;

		// Token: 0x04003335 RID: 13109
		[Tooltip("How many items to stick in the list. Item count is NormalizedProbability*SampleSize")]
		[Range(10f, 1000f)]
		[SerializeField]
		private int m_sampleSize = 100;

		// Token: 0x04003336 RID: 13110
		[Tooltip("At what point do we reshuffle everything back together? A value of 1 means cycle through the entire list")]
		[Range(0.1f, 1f)]
		[SerializeField]
		private float m_reshuffleThreshold = 0.5f;

		// Token: 0x04003337 RID: 13111
		[SerializeField]
		private SpawnProfileProbabilityCollection m_primary;

		// Token: 0x04003338 RID: 13112
		[SerializeField]
		private SpawnProfileProbabilityCollection m_alternate;

		// Token: 0x04003339 RID: 13113
		[SerializeField]
		private SpawnProfileData.SpawnProfileCount[] m_primaryCounts;

		// Token: 0x0400333A RID: 13114
		[SerializeField]
		private SpawnProfileData.SpawnProfileCount[] m_alternateCounts;

		// Token: 0x0400333B RID: 13115
		private SpawnProfileDrawBag m_primaryDrawBag;

		// Token: 0x0400333C RID: 13116
		private SpawnProfileDrawBag m_alternateDrawBag;

		// Token: 0x020006D4 RID: 1748
		private enum SampleType
		{
			// Token: 0x0400333E RID: 13118
			RandomDraw,
			// Token: 0x0400333F RID: 13119
			FixedDistribution,
			// Token: 0x04003340 RID: 13120
			RandomBagDraw
		}

		// Token: 0x020006D5 RID: 1749
		[Serializable]
		private class SpawnProfileCount
		{
			// Token: 0x17000BA8 RID: 2984
			// (get) Token: 0x0600350E RID: 13582 RVA: 0x00064587 File Offset: 0x00062787
			// (set) Token: 0x0600350F RID: 13583 RVA: 0x0006458F File Offset: 0x0006278F
			internal int ActiveCount { get; set; }

			// Token: 0x17000BA9 RID: 2985
			// (get) Token: 0x06003510 RID: 13584 RVA: 0x00064598 File Offset: 0x00062798
			internal int Count
			{
				get
				{
					return this.m_count;
				}
			}

			// Token: 0x17000BAA RID: 2986
			// (get) Token: 0x06003511 RID: 13585 RVA: 0x000645A0 File Offset: 0x000627A0
			internal SpawnProfile Profile
			{
				get
				{
					return this.m_spawnProfile;
				}
			}

			// Token: 0x17000BAB RID: 2987
			// (get) Token: 0x06003512 RID: 13586 RVA: 0x00063EE9 File Offset: 0x000620E9
			private IEnumerable GetSpawnProfiles
			{
				get
				{
					return SolOdinUtilities.GetDropdownItems<SpawnProfile>();
				}
			}

			// Token: 0x04003342 RID: 13122
			[SerializeField]
			private SpawnProfile m_spawnProfile;

			// Token: 0x04003343 RID: 13123
			[Range(0f, 50f)]
			[SerializeField]
			private int m_count;
		}
	}
}

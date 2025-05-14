using System;
using System.Collections;
using System.Collections.Generic;
using SoL.Game.NPCs;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Interactives
{
	// Token: 0x02000BB6 RID: 2998
	[CreateAssetMenu(menuName = "SoL/Profiles/Loot")]
	public class LootProfile : ScriptableObject
	{
		// Token: 0x06005CF3 RID: 23795 RVA: 0x001F25E8 File Offset: 0x001F07E8
		private void Initialize()
		{
			if (this.m_initialized)
			{
				return;
			}
			if (this.m_parentLootProfiles != null)
			{
				for (int i = 0; i < this.m_parentLootProfiles.Length; i++)
				{
					this.AddParentLootProfile(this.m_parentLootProfiles[i]);
				}
			}
			if (this.m_parentLootProfile != null)
			{
				this.AddParentLootProfile(this.m_parentLootProfile);
			}
			this.m_initialized = true;
		}

		// Token: 0x06005CF4 RID: 23796 RVA: 0x001F2648 File Offset: 0x001F0848
		private void AddParentLootProfile(LootProfile parentProfile)
		{
			if (parentProfile == null)
			{
				return;
			}
			parentProfile.Initialize();
			for (int i = 0; i < parentProfile.m_items.Count; i++)
			{
				if (!this.m_items.Contains(parentProfile.m_items[i]))
				{
					this.m_items.Add(parentProfile.m_items[i]);
				}
			}
		}

		// Token: 0x06005CF5 RID: 23797 RVA: 0x001F26AC File Offset: 0x001F08AC
		public ArchetypeInstance GetForTier(SpawnTier tier)
		{
			this.Initialize();
			SpawnTierFlags spawnTierFlags = tier.GetSpawnTierFlags();
			if (spawnTierFlags == SpawnTierFlags.None)
			{
				return null;
			}
			if (this.m_chanceToDrawItem < 1f && UnityEngine.Random.Range(0f, 1f) > this.m_chanceToDrawItem)
			{
				return null;
			}
			this.m_items.Shuffle<LootProfile.LootTierItem>();
			for (int i = 0; i < this.m_items.Count; i++)
			{
				if (this.m_items[i].TierFlags.HasBitFlag(spawnTierFlags))
				{
					ArchetypeInstance archetypeInstance = this.m_items[i].Item.CreateNewInstance();
					if (this.m_items[i].Item.ArchetypeHasCount())
					{
						archetypeInstance.ItemData.Count = new int?(this.m_items[i].Count.RandomWithinRange());
					}
					else if (this.m_items[i].Item.ArchetypeHasCharges())
					{
						archetypeInstance.ItemData.Charges = new int?(this.m_items[i].Charges.RandomWithinRange());
					}
					return archetypeInstance;
				}
			}
			return null;
		}

		// Token: 0x0400505B RID: 20571
		[SerializeField]
		private LootProfile[] m_parentLootProfiles;

		// Token: 0x0400505C RID: 20572
		[SerializeField]
		private LootProfile m_parentLootProfile;

		// Token: 0x0400505D RID: 20573
		[SerializeField]
		private List<LootProfile.LootTierItem> m_items;

		// Token: 0x0400505E RID: 20574
		[Range(0f, 1f)]
		[SerializeField]
		private float m_chanceToDrawItem = 1f;

		// Token: 0x0400505F RID: 20575
		private bool m_initialized;

		// Token: 0x02000BB7 RID: 2999
		[Serializable]
		private class LootTierItem : IEquatable<LootProfile.LootTierItem>
		{
			// Token: 0x06005CF7 RID: 23799 RVA: 0x00077B72 File Offset: 0x00075D72
			private IEnumerable GetItems()
			{
				return SolOdinUtilities.GetDropdownItems<ItemArchetype>();
			}

			// Token: 0x170015F4 RID: 5620
			// (get) Token: 0x06005CF8 RID: 23800 RVA: 0x0007E6DE File Offset: 0x0007C8DE
			private bool m_showCount
			{
				get
				{
					return this.Item != null && this.Item is IStackable;
				}
			}

			// Token: 0x170015F5 RID: 5621
			// (get) Token: 0x06005CF9 RID: 23801 RVA: 0x0007E6FE File Offset: 0x0007C8FE
			private bool m_showCharges
			{
				get
				{
					return this.Item != null && this.Item is RunicBattery;
				}
			}

			// Token: 0x170015F6 RID: 5622
			// (get) Token: 0x06005CFA RID: 23802 RVA: 0x0007E71E File Offset: 0x0007C91E
			public string IndexName
			{
				get
				{
					if (!(this.Item == null))
					{
						return this.Item.DisplayName;
					}
					return "None";
				}
			}

			// Token: 0x06005CFB RID: 23803 RVA: 0x0007E73F File Offset: 0x0007C93F
			public bool Equals(LootProfile.LootTierItem other)
			{
				return other != null && (this == other || object.Equals(this.Item, other.Item));
			}

			// Token: 0x06005CFC RID: 23804 RVA: 0x0007E75D File Offset: 0x0007C95D
			public override bool Equals(object obj)
			{
				return obj != null && (this == obj || (!(obj.GetType() != base.GetType()) && this.Equals((LootProfile.LootTierItem)obj)));
			}

			// Token: 0x06005CFD RID: 23805 RVA: 0x0007E78B File Offset: 0x0007C98B
			public override int GetHashCode()
			{
				if (!(this.Item != null))
				{
					return 0;
				}
				return this.Item.GetHashCode();
			}

			// Token: 0x04005060 RID: 20576
			public SpawnTierFlags TierFlags;

			// Token: 0x04005061 RID: 20577
			public ItemArchetype Item;

			// Token: 0x04005062 RID: 20578
			public MinMaxIntRange Count = new MinMaxIntRange(1, 1);

			// Token: 0x04005063 RID: 20579
			public MinMaxIntRange Charges = new MinMaxIntRange(1, 1);
		}
	}
}

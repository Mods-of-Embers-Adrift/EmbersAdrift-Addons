using System;
using System.Collections.Generic;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Containers;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Interactives
{
	// Token: 0x02000BA1 RID: 2977
	public class InteractiveRegenerativeChest : InteractiveRemoteContainerSingleLooter
	{
		// Token: 0x170015B7 RID: 5559
		// (get) Token: 0x06005C25 RID: 23589 RVA: 0x0007DD4E File Offset: 0x0007BF4E
		public ItemFlags ItemFlagsToSet
		{
			get
			{
				return this.m_flagsToSet;
			}
		}

		// Token: 0x170015B8 RID: 5560
		// (get) Token: 0x06005C26 RID: 23590 RVA: 0x0007DD56 File Offset: 0x0007BF56
		private ItemArchetype[] m_items
		{
			get
			{
				if (!this.m_useItemSet)
				{
					return this.m_localItems;
				}
				return this.m_itemSet.Items;
			}
		}

		// Token: 0x170015B9 RID: 5561
		// (get) Token: 0x06005C27 RID: 23591 RVA: 0x00045BCA File Offset: 0x00043DCA
		protected override bool NullifyListOnDestroy
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06005C28 RID: 23592 RVA: 0x0004479C File Offset: 0x0004299C
		protected override bool CanInteractInternal()
		{
			return true;
		}

		// Token: 0x06005C29 RID: 23593 RVA: 0x0007DD72 File Offset: 0x0007BF72
		protected override void Start()
		{
			base.Start();
			base.Looter = null;
		}

		// Token: 0x06005C2A RID: 23594 RVA: 0x001F0B5C File Offset: 0x001EED5C
		protected override void GenerateRecord(GameEntity interactionSource)
		{
			if (InteractiveRegenerativeChest.m_currentItems == null)
			{
				InteractiveRegenerativeChest.m_currentItems = new HashSet<UniqueId>();
			}
			if (this.m_record == null)
			{
				this.m_record = new ContainerRecord
				{
					Type = ContainerType.Loot,
					Instances = new List<ArchetypeInstance>()
				};
				this.m_interactiveFlags.Value |= InteractiveFlags.RecordGenerated;
			}
			InteractiveRegenerativeChest.m_currentItems.Clear();
			for (int i = 0; i < this.m_record.Instances.Count; i++)
			{
				InteractiveRegenerativeChest.m_currentItems.Add(this.m_record.Instances[i].ArchetypeId);
			}
			for (int j = 0; j < this.m_items.Length; j++)
			{
				if (this.m_items[j] != null && !InteractiveRegenerativeChest.m_currentItems.Contains(this.m_items[j].Id))
				{
					ArchetypeInstance archetypeInstance = this.m_items[j].CreateNewInstance();
					if (ContainerTypeExtensions.IsValidInstanceForLootContainer(archetypeInstance))
					{
						if (this.m_items[j].ArchetypeHasCount())
						{
							archetypeInstance.ItemData.Count = new int?(this.m_stackCount);
						}
						else if (this.m_items[j].ArchetypeHasCharges())
						{
							archetypeInstance.ItemData.Charges = new int?(this.m_chargeCount);
						}
						archetypeInstance.ItemData.ItemFlags = this.m_flagsToSet;
						this.m_record.Instances.Add(archetypeInstance);
					}
				}
			}
			for (int k = 0; k < this.m_record.Instances.Count; k++)
			{
				this.m_record.Instances[k].Index = k;
			}
		}

		// Token: 0x06005C2B RID: 23595 RVA: 0x0004475B File Offset: 0x0004295B
		protected override void OnRecordEmpty()
		{
		}

		// Token: 0x170015BA RID: 5562
		// (get) Token: 0x06005C2C RID: 23596 RVA: 0x0007DD81 File Offset: 0x0007BF81
		protected override string TooltipDescription
		{
			get
			{
				return this.m_description;
			}
		}

		// Token: 0x04005004 RID: 20484
		[SerializeField]
		private string m_description;

		// Token: 0x04005005 RID: 20485
		[SerializeField]
		private int m_stackCount = 20;

		// Token: 0x04005006 RID: 20486
		[SerializeField]
		private int m_chargeCount = 100;

		// Token: 0x04005007 RID: 20487
		private static HashSet<UniqueId> m_currentItems;

		// Token: 0x04005008 RID: 20488
		[SerializeField]
		private bool m_useItemSet;

		// Token: 0x04005009 RID: 20489
		[SerializeField]
		private ItemArchetype[] m_localItems;

		// Token: 0x0400500A RID: 20490
		[SerializeField]
		private ItemSet m_itemSet;

		// Token: 0x0400500B RID: 20491
		[SerializeField]
		private ItemFlags m_flagsToSet;
	}
}

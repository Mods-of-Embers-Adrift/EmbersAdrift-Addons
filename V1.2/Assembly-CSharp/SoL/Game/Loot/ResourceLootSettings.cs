using System;
using System.Collections;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Archetypes.Abilities;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Loot
{
	// Token: 0x02000B0E RID: 2830
	[Serializable]
	public class ResourceLootSettings
	{
		// Token: 0x1700147D RID: 5245
		// (get) Token: 0x0600574B RID: 22347 RVA: 0x0007A2C1 File Offset: 0x000784C1
		public bool LogLoot
		{
			get
			{
				return this.m_logLoot;
			}
		}

		// Token: 0x0600574C RID: 22348 RVA: 0x0007A2C9 File Offset: 0x000784C9
		public UniqueId GetAbilityIdForGathering()
		{
			if (this.RequiredTool != CraftingToolType.None)
			{
				return this.RequiredTool.GetAbilityIdForToolType();
			}
			if (!this.m_dynamicAbility)
			{
				return UniqueId.Empty;
			}
			return this.m_dynamicAbility.Id;
		}

		// Token: 0x0600574D RID: 22349 RVA: 0x001E356C File Offset: 0x001E176C
		public UniqueId? GetRequiredItemIdForGathering()
		{
			if (!this.m_requiredItem)
			{
				return null;
			}
			return new UniqueId?(this.m_requiredItem.Id);
		}

		// Token: 0x1700147E RID: 5246
		// (get) Token: 0x0600574E RID: 22350 RVA: 0x0007A2FD File Offset: 0x000784FD
		public ItemArchetype RequiredItem
		{
			get
			{
				return this.m_requiredItem;
			}
		}

		// Token: 0x1700147F RID: 5247
		// (get) Token: 0x0600574F RID: 22351 RVA: 0x0007A305 File Offset: 0x00078505
		public DynamicAbility DynamicAbility
		{
			get
			{
				if (!this.m_showDynamicAbility)
				{
					return null;
				}
				return this.m_dynamicAbility;
			}
		}

		// Token: 0x17001480 RID: 5248
		// (get) Token: 0x06005750 RID: 22352 RVA: 0x0007A317 File Offset: 0x00078517
		public bool RemoveRequiredItemOnUse
		{
			get
			{
				return !this.m_keepItemOnUse;
			}
		}

		// Token: 0x17001481 RID: 5249
		// (get) Token: 0x06005751 RID: 22353 RVA: 0x0007A322 File Offset: 0x00078522
		private bool m_showResourceLevel
		{
			get
			{
				return this.ResourceLootTable != null && this.ResourceLootTable.Table != null;
			}
		}

		// Token: 0x17001482 RID: 5250
		// (get) Token: 0x06005752 RID: 22354 RVA: 0x0007A33F File Offset: 0x0007853F
		private bool m_showKeepItemOnUse
		{
			get
			{
				return this.m_showResourceLevel && this.m_requiredItem;
			}
		}

		// Token: 0x17001483 RID: 5251
		// (get) Token: 0x06005753 RID: 22355 RVA: 0x0007A356 File Offset: 0x00078556
		private bool m_showDynamicAbility
		{
			get
			{
				return this.m_showResourceLevel && this.RequiredTool == CraftingToolType.None;
			}
		}

		// Token: 0x17001484 RID: 5252
		// (get) Token: 0x06005754 RID: 22356 RVA: 0x00077B72 File Offset: 0x00075D72
		private IEnumerable GetItems
		{
			get
			{
				return SolOdinUtilities.GetDropdownItems<ItemArchetype>();
			}
		}

		// Token: 0x17001485 RID: 5253
		// (get) Token: 0x06005755 RID: 22357 RVA: 0x0007A36B File Offset: 0x0007856B
		private IEnumerable GetAbilities
		{
			get
			{
				return SolOdinUtilities.GetDropdownItems<DynamicAbility>();
			}
		}

		// Token: 0x04004D0C RID: 19724
		protected const string kGroupName = "Loot";

		// Token: 0x04004D0D RID: 19725
		private const string kResourceTable = "Loot/Resources";

		// Token: 0x04004D0E RID: 19726
		private const string kRequirements = "Loot/Resources/Requirements";

		// Token: 0x04004D0F RID: 19727
		[SerializeField]
		private bool m_logLoot;

		// Token: 0x04004D10 RID: 19728
		public LootTableSampleCount ResourceLootTable;

		// Token: 0x04004D11 RID: 19729
		[Range(1f, 50f)]
		public int ResourceLevel = 1;

		// Token: 0x04004D12 RID: 19730
		public int GatherTime = 10;

		// Token: 0x04004D13 RID: 19731
		public CraftingToolType RequiredTool;

		// Token: 0x04004D14 RID: 19732
		[SerializeField]
		private ItemArchetype m_requiredItem;

		// Token: 0x04004D15 RID: 19733
		[SerializeField]
		private bool m_keepItemOnUse;

		// Token: 0x04004D16 RID: 19734
		[SerializeField]
		private DynamicAbility m_dynamicAbility;
	}
}

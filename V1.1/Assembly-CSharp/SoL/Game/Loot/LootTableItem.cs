using System;
using System.Collections;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Loot
{
	// Token: 0x02000B21 RID: 2849
	[Serializable]
	public class LootTableItem
	{
		// Token: 0x17001496 RID: 5270
		// (get) Token: 0x06005790 RID: 22416 RVA: 0x0007A61D File Offset: 0x0007881D
		public ItemArchetype Item
		{
			get
			{
				return this.m_item;
			}
		}

		// Token: 0x17001497 RID: 5271
		// (get) Token: 0x06005791 RID: 22417 RVA: 0x0007A625 File Offset: 0x00078825
		public MinMaxIntRange Count
		{
			get
			{
				return this.m_count;
			}
		}

		// Token: 0x17001498 RID: 5272
		// (get) Token: 0x06005792 RID: 22418 RVA: 0x0007A62D File Offset: 0x0007882D
		private bool m_itemHasCount
		{
			get
			{
				return this.m_item != null && this.m_item.ArchetypeHasCount();
			}
		}

		// Token: 0x17001499 RID: 5273
		// (get) Token: 0x06005793 RID: 22419 RVA: 0x0007A64A File Offset: 0x0007884A
		private bool m_itemHasCharge
		{
			get
			{
				return this.m_item != null && this.m_item.ArchetypeHasCharges();
			}
		}

		// Token: 0x1700149A RID: 5274
		// (get) Token: 0x06005794 RID: 22420 RVA: 0x0007A667 File Offset: 0x00078867
		private bool m_showCount
		{
			get
			{
				return this.m_itemHasCount || this.m_itemHasCharge;
			}
		}

		// Token: 0x06005795 RID: 22421 RVA: 0x0007A679 File Offset: 0x00078879
		private string GetLabelText()
		{
			if (!this.m_itemHasCharge)
			{
				return "Count";
			}
			return "Charges";
		}

		// Token: 0x06005796 RID: 22422 RVA: 0x00077B72 File Offset: 0x00075D72
		private IEnumerable GetItems()
		{
			return SolOdinUtilities.GetDropdownItems<ItemArchetype>();
		}

		// Token: 0x04004D4C RID: 19788
		[SerializeField]
		private ItemArchetype m_item;

		// Token: 0x04004D4D RID: 19789
		[SerializeField]
		private MinMaxIntRange m_count = new MinMaxIntRange(1, 1);
	}
}

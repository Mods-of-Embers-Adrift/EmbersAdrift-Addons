using System;
using System.Collections;
using System.Collections.Generic;
using SoL.Game.Objects.Archetypes;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Objects
{
	// Token: 0x020009F9 RID: 2553
	[CreateAssetMenu(menuName = "SoL/Objects/Itemization/Misc/Merchant Bundle", order = 4)]
	public class MerchantBundle : ScriptableObject
	{
		// Token: 0x17001120 RID: 4384
		// (get) Token: 0x06004D96 RID: 19862 RVA: 0x00074751 File Offset: 0x00072951
		public List<BaseArchetype> Items
		{
			get
			{
				return this.m_itemsForSale;
			}
		}

		// Token: 0x06004D97 RID: 19863 RVA: 0x00063B81 File Offset: 0x00061D81
		public static IEnumerable GetMerchantBundles()
		{
			return SolOdinUtilities.GetDropdownItems<MerchantBundle>();
		}

		// Token: 0x06004D98 RID: 19864 RVA: 0x001C0C54 File Offset: 0x001BEE54
		private bool ValidateItem(List<BaseArchetype> items, ref string err)
		{
			if (items == null)
			{
				return false;
			}
			for (int i = 0; i < items.Count; i++)
			{
				if (!(items[i] is IMerchantInventory))
				{
					err = "Invalid item at index " + i.ToString() + "!";
					return false;
				}
			}
			return true;
		}

		// Token: 0x04004733 RID: 18227
		[SerializeField]
		private List<BaseArchetype> m_itemsForSale;
	}
}

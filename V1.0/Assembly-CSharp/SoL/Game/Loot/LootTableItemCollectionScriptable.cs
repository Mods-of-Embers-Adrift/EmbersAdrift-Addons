using System;
using UnityEngine;

namespace SoL.Game.Loot
{
	// Token: 0x02000B26 RID: 2854
	[CreateAssetMenu(menuName = "SoL/Profiles/Loot/Items V3")]
	public class LootTableItemCollectionScriptable : ScriptableObject
	{
		// Token: 0x060057A7 RID: 22439 RVA: 0x0007A774 File Offset: 0x00078974
		public LootTableItem GetLootTableItem()
		{
			LootTableItemCollection collection = this.m_collection;
			if (collection == null)
			{
				return null;
			}
			return collection.GetLootTableItem();
		}

		// Token: 0x04004D56 RID: 19798
		[SerializeField]
		private LootTableItemCollection m_collection;
	}
}

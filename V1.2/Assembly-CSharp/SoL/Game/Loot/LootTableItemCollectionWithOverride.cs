using System;
using System.Collections;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Loot
{
	// Token: 0x02000B25 RID: 2853
	[Serializable]
	public class LootTableItemCollectionWithOverride : LootTableItemCollection
	{
		// Token: 0x170014A0 RID: 5280
		// (get) Token: 0x060057A2 RID: 22434 RVA: 0x0007A72A File Offset: 0x0007892A
		protected override bool HideLocalCollection
		{
			get
			{
				return this.m_override != null;
			}
		}

		// Token: 0x170014A1 RID: 5281
		// (get) Token: 0x060057A3 RID: 22435 RVA: 0x0007A738 File Offset: 0x00078938
		protected override bool m_showLoadInternal
		{
			get
			{
				return !this.HideLocalCollection;
			}
		}

		// Token: 0x060057A4 RID: 22436 RVA: 0x0007A743 File Offset: 0x00078943
		public override LootTableItem GetLootTableItem()
		{
			if (!(this.m_override != null))
			{
				return base.GetLootTableItem();
			}
			return this.m_override.GetLootTableItem();
		}

		// Token: 0x060057A5 RID: 22437 RVA: 0x0007A765 File Offset: 0x00078965
		private IEnumerable GetOverride()
		{
			return SolOdinUtilities.GetDropdownItems<LootTableItemCollectionScriptable>();
		}

		// Token: 0x04004D55 RID: 19797
		[SerializeField]
		private LootTableItemCollectionScriptable m_override;
	}
}

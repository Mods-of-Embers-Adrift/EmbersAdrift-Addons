using System;
using System.Collections.Generic;
using Com.TheFallenGames.OSA.Core;
using SoL.Game.Objects.Archetypes;

namespace SoL.Game.UI.Crafting
{
	// Token: 0x02000998 RID: 2456
	[Serializable]
	public class ItemItemViewsHolder : BaseItemViewsHolder
	{
		// Token: 0x17001043 RID: 4163
		// (get) Token: 0x06004980 RID: 18816 RVA: 0x0007160D File Offset: 0x0006F80D
		// (set) Token: 0x06004981 RID: 18817 RVA: 0x00071615 File Offset: 0x0006F815
		public ItemsListItem ListItem { get; private set; }

		// Token: 0x17001044 RID: 4164
		// (get) Token: 0x06004982 RID: 18818 RVA: 0x0007161E File Offset: 0x0006F81E
		// (set) Token: 0x06004983 RID: 18819 RVA: 0x00071626 File Offset: 0x0006F826
		public List<ArchetypeInstance> Data { get; private set; }

		// Token: 0x06004984 RID: 18820 RVA: 0x0007162F File Offset: 0x0006F82F
		public override void CollectViews()
		{
			base.CollectViews();
			this.ListItem = this.root.GetComponent<ItemsListItem>();
		}

		// Token: 0x06004985 RID: 18821 RVA: 0x00071648 File Offset: 0x0006F848
		public void UpdateItem(ItemsList parent, List<ArchetypeInstance> instances, SpecialItemListItemType? special = null)
		{
			this.Data = instances;
			this.ListItem.Init(parent, this.Data, special);
		}
	}
}

using System;
using Com.TheFallenGames.OSA.CustomAdapters.GridView;
using SoL.Game.Objects.Archetypes;

namespace SoL.Game.UI.Crafting
{
	// Token: 0x02000991 RID: 2449
	public class ComponentsListItemViewsHolder : CellViewsHolder
	{
		// Token: 0x17001036 RID: 4150
		// (get) Token: 0x0600490E RID: 18702 RVA: 0x00071120 File Offset: 0x0006F320
		// (set) Token: 0x0600490F RID: 18703 RVA: 0x00071128 File Offset: 0x0006F328
		public ComponentsListItem ListItem { get; private set; }

		// Token: 0x17001037 RID: 4151
		// (get) Token: 0x06004910 RID: 18704 RVA: 0x00071131 File Offset: 0x0006F331
		// (set) Token: 0x06004911 RID: 18705 RVA: 0x00071139 File Offset: 0x0006F339
		public RecipeComponent Data { get; private set; }

		// Token: 0x06004912 RID: 18706 RVA: 0x00071142 File Offset: 0x0006F342
		public override void CollectViews()
		{
			base.CollectViews();
			this.ListItem = this.root.GetComponent<ComponentsListItem>();
		}

		// Token: 0x06004913 RID: 18707 RVA: 0x0007115B File Offset: 0x0006F35B
		public void UpdateItem(ComponentsList parent, RecipeComponent item)
		{
			if (this.Data == item)
			{
				return;
			}
			this.Data = item;
			this.ListItem.Init(parent, this.Data, this.ItemIndex);
		}
	}
}

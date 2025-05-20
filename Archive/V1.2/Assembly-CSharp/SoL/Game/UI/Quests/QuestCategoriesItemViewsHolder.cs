using System;
using Com.TheFallenGames.OSA.Core;
using SoL.Game.Quests;

namespace SoL.Game.UI.Quests
{
	// Token: 0x02000946 RID: 2374
	[Serializable]
	public class QuestCategoriesItemViewsHolder : BaseItemViewsHolder
	{
		// Token: 0x17000FA8 RID: 4008
		// (get) Token: 0x0600461D RID: 17949 RVA: 0x0006F267 File Offset: 0x0006D467
		// (set) Token: 0x0600461E RID: 17950 RVA: 0x0006F26F File Offset: 0x0006D46F
		public QuestCategoriesListItem ListItem { get; private set; }

		// Token: 0x17000FA9 RID: 4009
		// (get) Token: 0x0600461F RID: 17951 RVA: 0x0006F278 File Offset: 0x0006D478
		// (set) Token: 0x06004620 RID: 17952 RVA: 0x0006F280 File Offset: 0x0006D480
		public Category<Quest> Data { get; private set; }

		// Token: 0x06004621 RID: 17953 RVA: 0x0006F289 File Offset: 0x0006D489
		public override void CollectViews()
		{
			base.CollectViews();
			this.ListItem = this.root.GetComponent<QuestCategoriesListItem>();
		}

		// Token: 0x06004622 RID: 17954 RVA: 0x001A268C File Offset: 0x001A088C
		public void UpdateItem(QuestCategoriesList parent, Category<Quest> item)
		{
			if (this.Data.Name == item.Name && this.Data.Data == item.Data)
			{
				return;
			}
			this.Data = item;
			this.ListItem.Init(parent, this.Data, this.ItemIndex);
		}

		// Token: 0x06004623 RID: 17955 RVA: 0x0006F2A2 File Offset: 0x0006D4A2
		public void RefreshVisuals()
		{
			this.ListItem.RefreshVisuals();
		}
	}
}

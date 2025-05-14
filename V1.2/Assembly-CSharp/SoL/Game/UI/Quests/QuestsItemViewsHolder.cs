using System;
using Com.TheFallenGames.OSA.Core;
using SoL.Game.Quests;

namespace SoL.Game.UI.Quests
{
	// Token: 0x0200094B RID: 2379
	[Serializable]
	public class QuestsItemViewsHolder : BaseItemViewsHolder
	{
		// Token: 0x17000FAE RID: 4014
		// (get) Token: 0x06004653 RID: 18003 RVA: 0x0006F4F2 File Offset: 0x0006D6F2
		// (set) Token: 0x06004654 RID: 18004 RVA: 0x0006F4FA File Offset: 0x0006D6FA
		public QuestsListItem ListItem { get; private set; }

		// Token: 0x17000FAF RID: 4015
		// (get) Token: 0x06004655 RID: 18005 RVA: 0x0006F503 File Offset: 0x0006D703
		// (set) Token: 0x06004656 RID: 18006 RVA: 0x0006F50B File Offset: 0x0006D70B
		public Quest Data { get; private set; }

		// Token: 0x06004657 RID: 18007 RVA: 0x0006F514 File Offset: 0x0006D714
		public override void CollectViews()
		{
			base.CollectViews();
			this.ListItem = this.root.GetComponent<QuestsListItem>();
		}

		// Token: 0x06004658 RID: 18008 RVA: 0x0006F52D File Offset: 0x0006D72D
		public void UpdateItem(QuestsList parent, Quest item)
		{
			if (this.Data == item)
			{
				return;
			}
			this.Data = item;
			this.ListItem.Init(parent, this.Data, this.ItemIndex);
		}

		// Token: 0x06004659 RID: 18009 RVA: 0x0006F55D File Offset: 0x0006D75D
		public void RefreshVisuals()
		{
			this.ListItem.RefreshVisuals();
		}
	}
}

using System;
using Com.TheFallenGames.OSA.Core;
using SoL.Game.Quests;

namespace SoL.Game.UI.Quests
{
	// Token: 0x02000950 RID: 2384
	[Serializable]
	public class TaskCardViewsHolder : BaseItemViewsHolder
	{
		// Token: 0x17000FB4 RID: 4020
		// (get) Token: 0x06004688 RID: 18056 RVA: 0x0006F7A4 File Offset: 0x0006D9A4
		// (set) Token: 0x06004689 RID: 18057 RVA: 0x0006F7AC File Offset: 0x0006D9AC
		public TaskCard ListItem { get; private set; }

		// Token: 0x17000FB5 RID: 4021
		// (get) Token: 0x0600468A RID: 18058 RVA: 0x0006F7B5 File Offset: 0x0006D9B5
		// (set) Token: 0x0600468B RID: 18059 RVA: 0x0006F7BD File Offset: 0x0006D9BD
		public BBTask Data { get; private set; }

		// Token: 0x0600468C RID: 18060 RVA: 0x0006F7C6 File Offset: 0x0006D9C6
		public override void CollectViews()
		{
			base.CollectViews();
			this.ListItem = this.root.GetComponent<TaskCard>();
		}

		// Token: 0x0600468D RID: 18061 RVA: 0x0006F7DF File Offset: 0x0006D9DF
		public void UpdateItem(BBTask item)
		{
			if (this.Data == item)
			{
				return;
			}
			this.Data = item;
			this.ListItem.Init(false, true, item.Type, item);
		}

		// Token: 0x0600468E RID: 18062 RVA: 0x0006F80B File Offset: 0x0006DA0B
		public void RefreshVisuals()
		{
			this.ListItem.RefreshVisuals();
		}
	}
}

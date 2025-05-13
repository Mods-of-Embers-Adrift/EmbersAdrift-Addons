using System;
using Com.TheFallenGames.OSA.Core;
using SoL.Game.Quests;

namespace SoL.Game.UI.Quests
{
	// Token: 0x0200093B RID: 2363
	[Serializable]
	public class TasksItemViewsHolder : BaseItemViewsHolder
	{
		// Token: 0x17000F98 RID: 3992
		// (get) Token: 0x060045BC RID: 17852 RVA: 0x0006EEA5 File Offset: 0x0006D0A5
		// (set) Token: 0x060045BD RID: 17853 RVA: 0x0006EEAD File Offset: 0x0006D0AD
		public BoardsListItem ListItem { get; private set; }

		// Token: 0x17000F99 RID: 3993
		// (get) Token: 0x060045BE RID: 17854 RVA: 0x0006EEB6 File Offset: 0x0006D0B6
		// (set) Token: 0x060045BF RID: 17855 RVA: 0x0006EEBE File Offset: 0x0006D0BE
		public BulletinBoard Data { get; private set; }

		// Token: 0x060045C0 RID: 17856 RVA: 0x0006EEC7 File Offset: 0x0006D0C7
		public override void CollectViews()
		{
			base.CollectViews();
			this.ListItem = this.root.GetComponent<BoardsListItem>();
		}

		// Token: 0x060045C1 RID: 17857 RVA: 0x0006EEE0 File Offset: 0x0006D0E0
		public void UpdateItem(BoardsList parent, BulletinBoard item)
		{
			if (this.Data == item)
			{
				return;
			}
			this.Data = item;
			this.ListItem.Init(parent, this.Data, this.ItemIndex);
		}

		// Token: 0x060045C2 RID: 17858 RVA: 0x0006EF10 File Offset: 0x0006D110
		public void RefreshVisuals()
		{
			this.ListItem.RefreshVisuals();
		}
	}
}

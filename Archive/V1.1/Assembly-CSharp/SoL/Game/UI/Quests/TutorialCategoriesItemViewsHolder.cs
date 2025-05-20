using System;
using Com.TheFallenGames.OSA.Core;
using SoL.Game.Notifications;

namespace SoL.Game.UI.Quests
{
	// Token: 0x02000952 RID: 2386
	[Serializable]
	public class TutorialCategoriesItemViewsHolder : BaseItemViewsHolder
	{
		// Token: 0x17000FBA RID: 4026
		// (get) Token: 0x060046A7 RID: 18087 RVA: 0x0006F8D6 File Offset: 0x0006DAD6
		// (set) Token: 0x060046A8 RID: 18088 RVA: 0x0006F8DE File Offset: 0x0006DADE
		public TutorialCategoriesListItem ListItem { get; private set; }

		// Token: 0x17000FBB RID: 4027
		// (get) Token: 0x060046A9 RID: 18089 RVA: 0x0006F8E7 File Offset: 0x0006DAE7
		// (set) Token: 0x060046AA RID: 18090 RVA: 0x0006F8EF File Offset: 0x0006DAEF
		public Category<BaseNotification> Data { get; private set; }

		// Token: 0x060046AB RID: 18091 RVA: 0x0006F8F8 File Offset: 0x0006DAF8
		public override void CollectViews()
		{
			base.CollectViews();
			this.ListItem = this.root.GetComponent<TutorialCategoriesListItem>();
		}

		// Token: 0x060046AC RID: 18092 RVA: 0x001A4EA4 File Offset: 0x001A30A4
		public void UpdateItem(TutorialCategoriesList parent, Category<BaseNotification> item)
		{
			if (this.Data.Name == item.Name && this.Data.Data == item.Data)
			{
				return;
			}
			this.Data = item;
			this.ListItem.Init(parent, this.Data, this.ItemIndex);
		}

		// Token: 0x060046AD RID: 18093 RVA: 0x0006F911 File Offset: 0x0006DB11
		public void RefreshVisuals()
		{
			this.ListItem.RefreshVisuals();
		}
	}
}

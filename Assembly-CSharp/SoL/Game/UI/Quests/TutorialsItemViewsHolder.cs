using System;
using Com.TheFallenGames.OSA.Core;
using SoL.Game.Notifications;

namespace SoL.Game.UI.Quests
{
	// Token: 0x02000956 RID: 2390
	[Serializable]
	public class TutorialsItemViewsHolder : BaseItemViewsHolder
	{
		// Token: 0x17000FBF RID: 4031
		// (get) Token: 0x060046D1 RID: 18129 RVA: 0x0006FADE File Offset: 0x0006DCDE
		// (set) Token: 0x060046D2 RID: 18130 RVA: 0x0006FAE6 File Offset: 0x0006DCE6
		public TutorialsListItem ListItem { get; private set; }

		// Token: 0x17000FC0 RID: 4032
		// (get) Token: 0x060046D3 RID: 18131 RVA: 0x0006FAEF File Offset: 0x0006DCEF
		// (set) Token: 0x060046D4 RID: 18132 RVA: 0x0006FAF7 File Offset: 0x0006DCF7
		public BaseNotification Data { get; private set; }

		// Token: 0x060046D5 RID: 18133 RVA: 0x0006FB00 File Offset: 0x0006DD00
		public override void CollectViews()
		{
			base.CollectViews();
			this.ListItem = this.root.GetComponent<TutorialsListItem>();
		}

		// Token: 0x060046D6 RID: 18134 RVA: 0x0006FB19 File Offset: 0x0006DD19
		public void UpdateItem(TutorialsList parent, BaseNotification item)
		{
			if (this.Data == item)
			{
				return;
			}
			this.Data = item;
			this.ListItem.Init(parent, this.Data, this.ItemIndex);
		}

		// Token: 0x060046D7 RID: 18135 RVA: 0x0006FB49 File Offset: 0x0006DD49
		public void RefreshVisuals()
		{
			this.ListItem.RefreshVisuals();
		}
	}
}

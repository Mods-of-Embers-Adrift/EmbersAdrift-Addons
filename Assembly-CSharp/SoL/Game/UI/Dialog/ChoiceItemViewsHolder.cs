using System;
using Com.TheFallenGames.OSA.Core;
using Ink.Runtime;

namespace SoL.Game.UI.Dialog
{
	// Token: 0x02000985 RID: 2437
	[Serializable]
	public class ChoiceItemViewsHolder : BaseItemViewsHolder
	{
		// Token: 0x17001026 RID: 4134
		// (get) Token: 0x060048AE RID: 18606 RVA: 0x00070D95 File Offset: 0x0006EF95
		// (set) Token: 0x060048AF RID: 18607 RVA: 0x00070D9D File Offset: 0x0006EF9D
		public ChoicesListItem ListItem { get; private set; }

		// Token: 0x17001027 RID: 4135
		// (get) Token: 0x060048B0 RID: 18608 RVA: 0x00070DA6 File Offset: 0x0006EFA6
		// (set) Token: 0x060048B1 RID: 18609 RVA: 0x00070DAE File Offset: 0x0006EFAE
		public Choice Data { get; private set; }

		// Token: 0x060048B2 RID: 18610 RVA: 0x00070DB7 File Offset: 0x0006EFB7
		public override void CollectViews()
		{
			base.CollectViews();
			this.ListItem = this.root.GetComponent<ChoicesListItem>();
		}

		// Token: 0x060048B3 RID: 18611 RVA: 0x00070DD0 File Offset: 0x0006EFD0
		public void UpdateItem(ChoicesList parent, Choice item)
		{
			if (this.Data == item)
			{
				return;
			}
			this.Data = item;
			this.ListItem.Init(parent, this.Data, this.ItemIndex);
		}

		// Token: 0x060048B4 RID: 18612 RVA: 0x00070E00 File Offset: 0x0006F000
		public void RefreshVisuals()
		{
			this.ListItem.RefreshVisuals();
		}
	}
}

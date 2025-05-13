using System;
using Com.TheFallenGames.OSA.Core;

namespace SoL.Game.HuntingLog
{
	// Token: 0x02000BD7 RID: 3031
	[Serializable]
	public class HuntingLogItemViewsHolder : BaseItemViewsHolder
	{
		// Token: 0x17001620 RID: 5664
		// (get) Token: 0x06005DAC RID: 23980 RVA: 0x0007EFDD File Offset: 0x0007D1DD
		// (set) Token: 0x06005DAD RID: 23981 RVA: 0x0007EFE5 File Offset: 0x0007D1E5
		public HuntingLogListItem ListItem { get; private set; }

		// Token: 0x17001621 RID: 5665
		// (get) Token: 0x06005DAE RID: 23982 RVA: 0x0007EFEE File Offset: 0x0007D1EE
		// (set) Token: 0x06005DAF RID: 23983 RVA: 0x0007EFF6 File Offset: 0x0007D1F6
		public HuntingLogEntry Entry { get; private set; }

		// Token: 0x06005DB0 RID: 23984 RVA: 0x0007EFFF File Offset: 0x0007D1FF
		public override void CollectViews()
		{
			base.CollectViews();
			this.ListItem = this.root.GetComponent<HuntingLogListItem>();
		}

		// Token: 0x06005DB1 RID: 23985 RVA: 0x0007F018 File Offset: 0x0007D218
		public void UpdateItem(HuntingLogListUI parent, HuntingLogEntry entry)
		{
			this.Entry = entry;
			this.ListItem.Init(parent, this.Entry, this.ItemIndex);
		}

		// Token: 0x06005DB2 RID: 23986 RVA: 0x0007F039 File Offset: 0x0007D239
		public void RefreshVisuals()
		{
			this.ListItem.RefreshVisuals();
		}
	}
}

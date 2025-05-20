using System;
using Com.TheFallenGames.OSA.Core;
using SoL.Networking.SolServer;

namespace SoL.Game.UI.Penalties
{
	// Token: 0x0200095A RID: 2394
	[Serializable]
	public class PenaltiesListItemViewsHolder : BaseItemViewsHolder
	{
		// Token: 0x17000FC6 RID: 4038
		// (get) Token: 0x060046F8 RID: 18168 RVA: 0x0006FCF2 File Offset: 0x0006DEF2
		// (set) Token: 0x060046F9 RID: 18169 RVA: 0x0006FCFA File Offset: 0x0006DEFA
		public PenaltiesListItem ListItem { get; private set; }

		// Token: 0x17000FC7 RID: 4039
		// (get) Token: 0x060046FA RID: 18170 RVA: 0x0006FD03 File Offset: 0x0006DF03
		// (set) Token: 0x060046FB RID: 18171 RVA: 0x0006FD0B File Offset: 0x0006DF0B
		public Penalty Data { get; private set; }

		// Token: 0x060046FC RID: 18172 RVA: 0x0006FD14 File Offset: 0x0006DF14
		public override void CollectViews()
		{
			base.CollectViews();
			this.ListItem = this.root.GetComponent<PenaltiesListItem>();
		}

		// Token: 0x060046FD RID: 18173 RVA: 0x0006FD2D File Offset: 0x0006DF2D
		public void UpdateItem(PenaltiesList parent, Penalty item)
		{
			if (this.Data == item)
			{
				return;
			}
			this.Data = item;
			this.ListItem.Init(parent, this.ItemIndex, this.Data);
		}
	}
}

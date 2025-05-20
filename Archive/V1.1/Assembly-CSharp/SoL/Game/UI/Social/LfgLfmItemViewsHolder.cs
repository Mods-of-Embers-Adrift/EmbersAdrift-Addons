using System;
using Com.TheFallenGames.OSA.Core;
using SoL.Game.Grouping;

namespace SoL.Game.UI.Social
{
	// Token: 0x0200090A RID: 2314
	[Serializable]
	public class LfgLfmItemViewsHolder : BaseItemViewsHolder
	{
		// Token: 0x060043FE RID: 17406 RVA: 0x0006DEEB File Offset: 0x0006C0EB
		public override void CollectViews()
		{
			base.CollectViews();
			this.ListItem = this.root.GetComponent<LfgLfmListItem>();
		}

		// Token: 0x060043FF RID: 17407 RVA: 0x0006DF04 File Offset: 0x0006C104
		public void UpdateItem(LookingFor lookingFor)
		{
			if (this.Data == lookingFor)
			{
				return;
			}
			this.Data = lookingFor;
			this.ListItem.Init(this.Data);
		}

		// Token: 0x0400407A RID: 16506
		public LfgLfmListItem ListItem;

		// Token: 0x0400407B RID: 16507
		public LookingFor Data;
	}
}

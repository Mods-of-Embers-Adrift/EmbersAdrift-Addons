using System;
using Com.TheFallenGames.OSA.Core;

namespace SoL.Game.GM
{
	// Token: 0x02000BEC RID: 3052
	[Serializable]
	public class DpsMeterEntryViewsHolder : BaseItemViewsHolder
	{
		// Token: 0x06005E51 RID: 24145 RVA: 0x0007F69F File Offset: 0x0007D89F
		internal void UpdateEntry(DpsMeterList controller, DpsMeterInfo listEntry)
		{
			this.m_listItem.SetData(controller, listEntry);
		}

		// Token: 0x06005E52 RID: 24146 RVA: 0x0007F6AE File Offset: 0x0007D8AE
		public override void CollectViews()
		{
			base.CollectViews();
			this.m_listItem = this.root.GetComponent<DpsMeterListEntry>();
		}

		// Token: 0x04005199 RID: 20889
		private DpsMeterListEntry m_listItem;
	}
}

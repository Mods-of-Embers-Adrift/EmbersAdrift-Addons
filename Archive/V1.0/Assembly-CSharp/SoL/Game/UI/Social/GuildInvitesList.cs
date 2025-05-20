using System;
using System.Collections.Generic;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomParams;
using SoL.Networking.Database;

namespace SoL.Game.UI.Social
{
	// Token: 0x020008FC RID: 2300
	public class GuildInvitesList : OSA<BaseParamsWithPrefab, GuildInvitesItemViewsHolder>
	{
		// Token: 0x06004374 RID: 17268 RVA: 0x0006D800 File Offset: 0x0006BA00
		protected override void Start()
		{
			base.Start();
		}

		// Token: 0x06004375 RID: 17269 RVA: 0x001966B0 File Offset: 0x001948B0
		public void UpdateItems(List<Mail> items)
		{
			if (!base.gameObject.activeInHierarchy)
			{
				return;
			}
			if (this.m_items == null)
			{
				this.m_items = new List<Mail>(items.Count);
			}
			this.m_items.Clear();
			this.m_items.AddRange(items);
			this.ResetItems(this.m_items.Count, false, false);
		}

		// Token: 0x06004376 RID: 17270 RVA: 0x0006D808 File Offset: 0x0006BA08
		protected override GuildInvitesItemViewsHolder CreateViewsHolder(int itemIndex)
		{
			GuildInvitesItemViewsHolder guildInvitesItemViewsHolder = new GuildInvitesItemViewsHolder();
			guildInvitesItemViewsHolder.Init(this._Params.ItemPrefab, this._Params.Content, itemIndex, true, true);
			return guildInvitesItemViewsHolder;
		}

		// Token: 0x06004377 RID: 17271 RVA: 0x0006D82E File Offset: 0x0006BA2E
		protected override void UpdateViewsHolder(GuildInvitesItemViewsHolder newOrRecycled)
		{
			newOrRecycled.UpdateItem(this.m_items[newOrRecycled.ItemIndex]);
		}

		// Token: 0x06004378 RID: 17272 RVA: 0x00196710 File Offset: 0x00194910
		public void Reindex()
		{
			for (int i = 0; i < this.GetItemsCount(); i++)
			{
				GuildInvitesItemViewsHolder itemViewsHolderIfVisible = base.GetItemViewsHolderIfVisible(i);
				if (itemViewsHolderIfVisible != null)
				{
					itemViewsHolderIfVisible.ListItem.Init(this.m_items[i]);
				}
			}
		}

		// Token: 0x04004017 RID: 16407
		private List<Mail> m_items;
	}
}

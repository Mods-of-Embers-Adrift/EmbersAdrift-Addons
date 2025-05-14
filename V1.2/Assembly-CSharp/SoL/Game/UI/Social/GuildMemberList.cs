using System;
using System.Collections.Generic;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomParams;
using SoL.Networking.Database;

namespace SoL.Game.UI.Social
{
	// Token: 0x020008FF RID: 2303
	public class GuildMemberList : OSA<BaseParamsWithPrefab, GuildMemberListItemViewsHolder>
	{
		// Token: 0x0600438C RID: 17292 RVA: 0x001969F8 File Offset: 0x00194BF8
		public void UpdateItems(ICollection<GuildMember> items)
		{
			if (!base.gameObject.activeInHierarchy)
			{
				return;
			}
			if (this.m_items == null)
			{
				this.m_items = new List<GuildMember>(items.Count);
			}
			this.m_items.Clear();
			this.m_items.AddRange(items);
			this.ResetItems(this.m_items.Count, false, false);
		}

		// Token: 0x0600438D RID: 17293 RVA: 0x0006D995 File Offset: 0x0006BB95
		protected override GuildMemberListItemViewsHolder CreateViewsHolder(int itemIndex)
		{
			GuildMemberListItemViewsHolder guildMemberListItemViewsHolder = new GuildMemberListItemViewsHolder();
			guildMemberListItemViewsHolder.Init(this._Params.ItemPrefab, this._Params.Content, itemIndex, true, true);
			return guildMemberListItemViewsHolder;
		}

		// Token: 0x0600438E RID: 17294 RVA: 0x0006D9BB File Offset: 0x0006BBBB
		protected override void UpdateViewsHolder(GuildMemberListItemViewsHolder newOrRecycled)
		{
			newOrRecycled.UpdateItem(this, this.m_items[newOrRecycled.ItemIndex]);
		}

		// Token: 0x0600438F RID: 17295 RVA: 0x00196A58 File Offset: 0x00194C58
		public void Reindex()
		{
			for (int i = 0; i < this.GetItemsCount(); i++)
			{
				GuildMemberListItemViewsHolder itemViewsHolderIfVisible = base.GetItemViewsHolderIfVisible(i);
				if (itemViewsHolderIfVisible != null)
				{
					itemViewsHolderIfVisible.ListItem.Init(this, i, this.m_items[i]);
				}
			}
		}

		// Token: 0x04004023 RID: 16419
		private List<GuildMember> m_items;
	}
}

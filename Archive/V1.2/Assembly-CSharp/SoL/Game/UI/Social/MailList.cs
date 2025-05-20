using System;
using System.Collections.Generic;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomParams;
using SoL.Networking.Database;

namespace SoL.Game.UI.Social
{
	// Token: 0x02000914 RID: 2324
	public class MailList : OSA<BaseParamsWithPrefab, MailListItemViewsHolder>
	{
		// Token: 0x140000CA RID: 202
		// (add) Token: 0x06004479 RID: 17529 RVA: 0x0019C604 File Offset: 0x0019A804
		// (remove) Token: 0x0600447A RID: 17530 RVA: 0x0019C63C File Offset: 0x0019A83C
		public event Action<Mail> ViewRequested;

		// Token: 0x0600447B RID: 17531 RVA: 0x0019C674 File Offset: 0x0019A874
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

		// Token: 0x0600447C RID: 17532 RVA: 0x0006E3AC File Offset: 0x0006C5AC
		protected override MailListItemViewsHolder CreateViewsHolder(int itemIndex)
		{
			MailListItemViewsHolder mailListItemViewsHolder = new MailListItemViewsHolder();
			mailListItemViewsHolder.Init(this._Params.ItemPrefab, this._Params.Content, itemIndex, true, true);
			return mailListItemViewsHolder;
		}

		// Token: 0x0600447D RID: 17533 RVA: 0x0006E3D2 File Offset: 0x0006C5D2
		protected override void UpdateViewsHolder(MailListItemViewsHolder newOrRecycled)
		{
			newOrRecycled.UpdateItem(this.m_items[newOrRecycled.ItemIndex], this);
		}

		// Token: 0x0600447E RID: 17534 RVA: 0x0019C6D4 File Offset: 0x0019A8D4
		public void Reindex()
		{
			for (int i = 0; i < this.GetItemsCount(); i++)
			{
				MailListItemViewsHolder itemViewsHolderIfVisible = base.GetItemViewsHolderIfVisible(i);
				if (itemViewsHolderIfVisible != null)
				{
					itemViewsHolderIfVisible.ListItem.Init(this.m_items[i], this);
				}
			}
		}

		// Token: 0x0600447F RID: 17535 RVA: 0x0006E3EC File Offset: 0x0006C5EC
		public void View(Mail mail)
		{
			Action<Mail> viewRequested = this.ViewRequested;
			if (viewRequested == null)
			{
				return;
			}
			viewRequested(mail);
		}

		// Token: 0x04004106 RID: 16646
		private List<Mail> m_items;
	}
}

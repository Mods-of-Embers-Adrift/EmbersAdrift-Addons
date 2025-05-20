using System;
using System.Collections.Generic;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomParams;
using SoL.Networking.SolServer;

namespace SoL.Game.UI.Penalties
{
	// Token: 0x02000959 RID: 2393
	public class PenaltiesList : OSA<BaseParamsWithPrefab, PenaltiesListItemViewsHolder>
	{
		// Token: 0x17000FC4 RID: 4036
		// (get) Token: 0x060046EF RID: 18159 RVA: 0x0006FC7F File Offset: 0x0006DE7F
		// (set) Token: 0x060046F0 RID: 18160 RVA: 0x0006FC87 File Offset: 0x0006DE87
		public bool IsDirty { get; private set; }

		// Token: 0x17000FC5 RID: 4037
		// (get) Token: 0x060046F1 RID: 18161 RVA: 0x0006FC90 File Offset: 0x0006DE90
		// (set) Token: 0x060046F2 RID: 18162 RVA: 0x0006FC98 File Offset: 0x0006DE98
		public UserIdentification CurrentUserData { get; private set; }

		// Token: 0x060046F3 RID: 18163 RVA: 0x001A5874 File Offset: 0x001A3A74
		public void UpdateItems(ICollection<Penalty> items, UserIdentification userData)
		{
			if (!base.gameObject.activeInHierarchy)
			{
				return;
			}
			this.CurrentUserData = userData;
			if (this.m_items == null)
			{
				this.m_items = new List<Penalty>(items.Count);
			}
			this.m_items.Clear();
			this.m_items.AddRange(items);
			this.ResetItems(this.m_items.Count, false, false);
			this.IsDirty = false;
		}

		// Token: 0x060046F4 RID: 18164 RVA: 0x0006FCA1 File Offset: 0x0006DEA1
		public void MarkDirty()
		{
			this.IsDirty = true;
		}

		// Token: 0x060046F5 RID: 18165 RVA: 0x0006FCAA File Offset: 0x0006DEAA
		protected override PenaltiesListItemViewsHolder CreateViewsHolder(int itemIndex)
		{
			PenaltiesListItemViewsHolder penaltiesListItemViewsHolder = new PenaltiesListItemViewsHolder();
			penaltiesListItemViewsHolder.Init(this._Params.ItemPrefab, this._Params.Content, itemIndex, true, true);
			return penaltiesListItemViewsHolder;
		}

		// Token: 0x060046F6 RID: 18166 RVA: 0x0006FCD0 File Offset: 0x0006DED0
		protected override void UpdateViewsHolder(PenaltiesListItemViewsHolder newOrRecycled)
		{
			newOrRecycled.UpdateItem(this, this.m_items[newOrRecycled.ItemIndex]);
		}

		// Token: 0x040042D3 RID: 17107
		private List<Penalty> m_items;
	}
}

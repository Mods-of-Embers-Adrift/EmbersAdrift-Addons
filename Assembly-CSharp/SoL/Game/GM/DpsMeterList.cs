using System;
using Com.TheFallenGames.OSA.Core;
using SoL.UI;
using TMPro;
using UnityEngine;

namespace SoL.Game.GM
{
	// Token: 0x02000BEA RID: 3050
	public class DpsMeterList : OSA<DpsMeterEntryParam, DpsMeterEntryViewsHolder>
	{
		// Token: 0x17001647 RID: 5703
		// (get) Token: 0x06005E47 RID: 24135 RVA: 0x0007F634 File Offset: 0x0007D834
		internal bool IsEnabled
		{
			get
			{
				return this.m_toggle && this.m_toggle.isOn;
			}
		}

		// Token: 0x17001648 RID: 5704
		// (get) Token: 0x06005E48 RID: 24136 RVA: 0x0007F650 File Offset: 0x0007D850
		internal SolButton ResetButton
		{
			get
			{
				return this.m_reset;
			}
		}

		// Token: 0x17001649 RID: 5705
		// (get) Token: 0x06005E49 RID: 24137 RVA: 0x0007F658 File Offset: 0x0007D858
		internal TextMeshProUGUI TimeElapsedLabel
		{
			get
			{
				return this.m_timeElapsed;
			}
		}

		// Token: 0x1700164A RID: 5706
		// (get) Token: 0x06005E4A RID: 24138 RVA: 0x0007F660 File Offset: 0x0007D860
		// (set) Token: 0x06005E4B RID: 24139 RVA: 0x0007F668 File Offset: 0x0007D868
		internal DpsMeterController Controller { get; set; }

		// Token: 0x1700164B RID: 5707
		// (get) Token: 0x06005E4C RID: 24140 RVA: 0x00045BCA File Offset: 0x00043DCA
		internal int SortByIndex
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x06005E4D RID: 24141 RVA: 0x0007F671 File Offset: 0x0007D871
		protected override DpsMeterEntryViewsHolder CreateViewsHolder(int itemIndex)
		{
			DpsMeterEntryViewsHolder dpsMeterEntryViewsHolder = new DpsMeterEntryViewsHolder();
			dpsMeterEntryViewsHolder.Init(this._Params.ItemPrefab, this._Params.Content, itemIndex, true, true);
			return dpsMeterEntryViewsHolder;
		}

		// Token: 0x06005E4E RID: 24142 RVA: 0x0004475B File Offset: 0x0004295B
		protected override void UpdateViewsHolder(DpsMeterEntryViewsHolder newOrRecycled)
		{
		}

		// Token: 0x04005194 RID: 20884
		[SerializeField]
		private SolToggle m_toggle;

		// Token: 0x04005195 RID: 20885
		[SerializeField]
		private SolButton m_reset;

		// Token: 0x04005196 RID: 20886
		[SerializeField]
		private TextMeshProUGUI m_timeElapsed;

		// Token: 0x04005197 RID: 20887
		[SerializeField]
		private SolToggle m_includeNpcs;

		// Token: 0x04005198 RID: 20888
		internal const int kSortBy = 0;
	}
}

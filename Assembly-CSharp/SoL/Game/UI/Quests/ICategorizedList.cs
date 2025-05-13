using System;
using System.Collections.Generic;

namespace SoL.Game.UI.Quests
{
	// Token: 0x02000941 RID: 2369
	public interface ICategorizedList<TData>
	{
		// Token: 0x17000F9F RID: 3999
		// (get) Token: 0x060045F4 RID: 17908
		bool IsInitialized { get; }

		// Token: 0x140000D1 RID: 209
		// (add) Token: 0x060045F5 RID: 17909
		// (remove) Token: 0x060045F6 RID: 17910
		event Action<TData> SelectionChanged;

		// Token: 0x140000D2 RID: 210
		// (add) Token: 0x060045F7 RID: 17911
		// (remove) Token: 0x060045F8 RID: 17912
		event Action Initialized;

		// Token: 0x060045F9 RID: 17913
		void UpdateItems(ICollection<TData> items);

		// Token: 0x060045FA RID: 17914
		void DeselectAll(bool suppressEvents = false);

		// Token: 0x060045FB RID: 17915
		void ReindexItems(TData selectedItem);

		// Token: 0x060045FC RID: 17916
		void Sort(List<TData> unsorted);
	}
}

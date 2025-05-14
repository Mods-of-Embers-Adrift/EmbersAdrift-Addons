using System;
using System.Collections.Generic;
using SoL.Utilities.Extensions;

namespace SoL.Utilities
{
	// Token: 0x02000245 RID: 581
	public class ArrayShufflerOriginalIndex<T> where T : class
	{
		// Token: 0x06001312 RID: 4882 RVA: 0x000E8C44 File Offset: 0x000E6E44
		public ArrayShufflerOriginalIndex(IReadOnlyList<T> originalItems)
		{
			if (originalItems == null || originalItems.Count <= 0)
			{
				return;
			}
			this.m_itemIndexes = new ArrayShufflerOriginalIndex<T>.ItemIndex[originalItems.Count];
			for (int i = 0; i < originalItems.Count; i++)
			{
				this.m_itemIndexes[i] = new ArrayShufflerOriginalIndex<T>.ItemIndex
				{
					Item = originalItems[i],
					OriginalIndex = i
				};
			}
		}

		// Token: 0x06001313 RID: 4883 RVA: 0x000E8CB8 File Offset: 0x000E6EB8
		private ArrayShufflerOriginalIndex<T>.ItemIndex GetNextItemIndex()
		{
			if (this.m_index <= 0 || this.m_index >= this.m_itemIndexes.Length)
			{
				this.m_index = 0;
				this.m_itemIndexes.Shuffle<ArrayShufflerOriginalIndex<T>.ItemIndex>();
			}
			ArrayShufflerOriginalIndex<T>.ItemIndex result = this.m_itemIndexes[this.m_index];
			this.m_index++;
			return result;
		}

		// Token: 0x06001314 RID: 4884 RVA: 0x000E8D10 File Offset: 0x000E6F10
		public T GetNext()
		{
			if (this.m_itemIndexes == null || this.m_itemIndexes.Length == 0)
			{
				return default(T);
			}
			return this.GetNextItemIndex().Item;
		}

		// Token: 0x06001315 RID: 4885 RVA: 0x0004F8E7 File Offset: 0x0004DAE7
		public int GetNextOriginalIndex()
		{
			if (this.m_itemIndexes == null || this.m_itemIndexes.Length == 0)
			{
				return 0;
			}
			return this.GetNextItemIndex().OriginalIndex;
		}

		// Token: 0x040010E8 RID: 4328
		private int m_index = -1;

		// Token: 0x040010E9 RID: 4329
		private readonly ArrayShufflerOriginalIndex<T>.ItemIndex[] m_itemIndexes;

		// Token: 0x02000246 RID: 582
		private struct ItemIndex
		{
			// Token: 0x040010EA RID: 4330
			public T Item;

			// Token: 0x040010EB RID: 4331
			public int OriginalIndex;
		}
	}
}

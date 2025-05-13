using System;
using System.Collections.Generic;
using SoL.Utilities.Extensions;

namespace SoL.Utilities
{
	// Token: 0x02000244 RID: 580
	public class ArrayShuffler<T> where T : class
	{
		// Token: 0x06001310 RID: 4880 RVA: 0x000E8B54 File Offset: 0x000E6D54
		public ArrayShuffler(IReadOnlyList<T> originalItems)
		{
			if (originalItems == null || originalItems.Count <= 0)
			{
				return;
			}
			this.m_items = new T[originalItems.Count];
			for (int i = 0; i < originalItems.Count; i++)
			{
				this.m_items[i] = originalItems[i];
			}
		}

		// Token: 0x06001311 RID: 4881 RVA: 0x000E8BB0 File Offset: 0x000E6DB0
		public T GetNext()
		{
			if (this.m_items == null || this.m_items.Length == 0)
			{
				return default(T);
			}
			if (this.m_index <= 0 || this.m_index >= this.m_items.Length)
			{
				this.m_index = 0;
				this.m_items.Shuffle<T>();
			}
			T result = this.m_items[this.m_index];
			this.m_index++;
			return result;
		}

		// Token: 0x040010E6 RID: 4326
		private int m_index = -1;

		// Token: 0x040010E7 RID: 4327
		private readonly T[] m_items;
	}
}

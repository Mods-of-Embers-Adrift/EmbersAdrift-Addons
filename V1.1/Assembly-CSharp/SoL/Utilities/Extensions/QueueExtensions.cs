using System;
using System.Collections.Generic;

namespace SoL.Utilities.Extensions
{
	// Token: 0x02000335 RID: 821
	public static class QueueExtensions
	{
		// Token: 0x0600168E RID: 5774 RVA: 0x00051BE0 File Offset: 0x0004FDE0
		public static bool TryDequeue<T>(this Queue<T> queue, out T item)
		{
			if (queue.Count > 0)
			{
				item = queue.Dequeue();
				return true;
			}
			item = default(T);
			return false;
		}
	}
}

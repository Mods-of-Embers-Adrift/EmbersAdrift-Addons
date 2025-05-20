using System;
using System.Collections.Generic;
using SoL.Game.Spawning;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x02000279 RID: 633
	public abstract class DrawBag<TType, TEntry, TCollection> where TType : class where TEntry : ProbabilityEntry<TType> where TCollection : ProbabilityCollection<TEntry>
	{
		// Token: 0x060013D0 RID: 5072 RVA: 0x000F804C File Offset: 0x000F624C
		protected DrawBag(TCollection collection, int sampleSize, float reshuffleThreshold)
		{
			this.m_bag = new List<TType>(sampleSize);
			this.m_drawn = new List<TType>(sampleSize);
			this.m_resampleCount = Mathf.FloorToInt((float)sampleSize * reshuffleThreshold);
			this.m_resampleCount = Mathf.Max(this.m_resampleCount, 1);
			collection.Normalize();
			foreach (TEntry tentry in collection.Entries)
			{
				if (tentry.Obj != null)
				{
					int num = Mathf.FloorToInt(tentry.NormalizedProbability * (float)sampleSize);
					for (int j = 0; j < num; j++)
					{
						this.m_bag.Add(tentry.Obj);
					}
				}
			}
			this.m_bag.Shuffle<TType>();
		}

		// Token: 0x060013D1 RID: 5073 RVA: 0x000F811C File Offset: 0x000F631C
		internal TType GetItem()
		{
			if (this.m_bag.Count <= 0 && this.m_drawn.Count <= 0)
			{
				return default(TType);
			}
			if (this.m_bag.Count <= 0 || this.m_drawn.Count >= this.m_resampleCount)
			{
				this.ResetBag();
			}
			int index = this.m_bag.Count - 1;
			TType ttype = this.m_bag[index];
			this.m_bag.RemoveAt(index);
			this.m_drawn.Add(ttype);
			return ttype;
		}

		// Token: 0x060013D2 RID: 5074 RVA: 0x0004FE71 File Offset: 0x0004E071
		private void ResetBag()
		{
			this.m_bag.AddRange(this.m_drawn);
			this.m_bag.Shuffle<TType>();
			this.m_drawn.Clear();
		}

		// Token: 0x04001C04 RID: 7172
		private readonly List<TType> m_bag;

		// Token: 0x04001C05 RID: 7173
		private readonly List<TType> m_drawn;

		// Token: 0x04001C06 RID: 7174
		private readonly int m_resampleCount;
	}
}

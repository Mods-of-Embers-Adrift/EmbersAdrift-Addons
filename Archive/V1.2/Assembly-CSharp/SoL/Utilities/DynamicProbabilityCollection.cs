using System;
using System.Collections.Generic;
using SoL.Game.Spawning;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x0200027C RID: 636
	public class DynamicProbabilityCollection<T> : IPoolable where T : IProbabilityEntry
	{
		// Token: 0x060013D5 RID: 5077 RVA: 0x0004FEB0 File Offset: 0x0004E0B0
		public DynamicProbabilityCollection()
		{
			this.m_internalList = new List<T>(100);
		}

		// Token: 0x060013D6 RID: 5078 RVA: 0x000F81AC File Offset: 0x000F63AC
		public void Add(T item)
		{
			if (this.m_initialized)
			{
				throw new ArgumentException("Collection already initialized!  You must clear it first!");
			}
			if (item != null)
			{
				this.m_internalList.Add(item);
				this.m_totalProbability += item.Probability;
			}
		}

		// Token: 0x060013D7 RID: 5079 RVA: 0x0004FEC5 File Offset: 0x0004E0C5
		public void Clear()
		{
			this.m_internalList.Clear();
			this.m_totalProbability = 0f;
			this.m_initialized = false;
		}

		// Token: 0x060013D8 RID: 5080 RVA: 0x000F81FC File Offset: 0x000F63FC
		public bool TryGetEntry(out T value, System.Random seed = null)
		{
			value = default(T);
			this.InitializeValues();
			float num = (seed != null) ? ((float)seed.NextDouble()) : UnityEngine.Random.Range(0f, 1f);
			for (int i = 0; i < this.m_internalList.Count; i++)
			{
				float num2 = num;
				T t = this.m_internalList[i];
				if (num2 < t.Threshold)
				{
					value = this.m_internalList[i];
					return true;
				}
			}
			return false;
		}

		// Token: 0x060013D9 RID: 5081 RVA: 0x000F827C File Offset: 0x000F647C
		private void InitializeValues()
		{
			if (this.m_initialized)
			{
				return;
			}
			for (int i = 0; i < this.m_internalList.Count; i++)
			{
				T value = this.m_internalList[i];
				value.SetTotalProbability(this.m_totalProbability);
				this.m_internalList[i] = value;
			}
			this.m_internalList.Sort(DynamicProbabilityCollection<T>.m_comparison);
			float num = 0f;
			for (int j = 0; j < this.m_internalList.Count; j++)
			{
				T value2 = this.m_internalList[j];
				num += value2.NormalizedProbability;
				value2.Threshold = num;
				this.m_internalList[j] = value2;
			}
			this.m_initialized = true;
		}

		// Token: 0x170004FD RID: 1277
		// (get) Token: 0x060013DA RID: 5082 RVA: 0x0004FEE4 File Offset: 0x0004E0E4
		// (set) Token: 0x060013DB RID: 5083 RVA: 0x0004FEEC File Offset: 0x0004E0EC
		bool IPoolable.InPool { get; set; }

		// Token: 0x060013DC RID: 5084 RVA: 0x0004FEF5 File Offset: 0x0004E0F5
		void IPoolable.Reset()
		{
			this.Clear();
		}

		// Token: 0x04001C07 RID: 7175
		private static readonly Comparison<T> m_comparison = (T a, T b) => b.NormalizedProbability.CompareTo(a.NormalizedProbability);

		// Token: 0x04001C08 RID: 7176
		private readonly List<T> m_internalList;

		// Token: 0x04001C09 RID: 7177
		private float m_totalProbability;

		// Token: 0x04001C0A RID: 7178
		private bool m_initialized;

		// Token: 0x04001C0B RID: 7179
		private bool _inPool;
	}
}

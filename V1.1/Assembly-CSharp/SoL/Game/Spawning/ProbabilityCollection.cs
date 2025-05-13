using System;
using UnityEngine;

namespace SoL.Game.Spawning
{
	// Token: 0x02000692 RID: 1682
	[Serializable]
	public abstract class ProbabilityCollection<T> where T : class, IProbabilityEntry
	{
		// Token: 0x17000B2C RID: 2860
		// (get) Token: 0x060033B1 RID: 13233 RVA: 0x00063855 File Offset: 0x00061A55
		public int Count
		{
			get
			{
				if (this.m_entries != null)
				{
					return this.m_entries.Length;
				}
				return 0;
			}
		}

		// Token: 0x17000B2D RID: 2861
		// (get) Token: 0x060033B2 RID: 13234 RVA: 0x00063869 File Offset: 0x00061A69
		// (set) Token: 0x060033B3 RID: 13235 RVA: 0x00063871 File Offset: 0x00061A71
		public T[] Entries
		{
			get
			{
				return this.m_entries;
			}
			set
			{
				this.m_entries = value;
			}
		}

		// Token: 0x060033B4 RID: 13236 RVA: 0x0006387A File Offset: 0x00061A7A
		public void Normalize()
		{
			this.NormalizeInternal();
		}

		// Token: 0x060033B5 RID: 13237 RVA: 0x00162744 File Offset: 0x00160944
		private void NormalizeInternal()
		{
			if (this.m_entries == null || this.m_entries.Length == 0)
			{
				return;
			}
			float num = 0f;
			for (int i = 0; i < this.m_entries.Length; i++)
			{
				num += this.m_entries[i].Probability;
			}
			for (int j = 0; j < this.m_entries.Length; j++)
			{
				this.m_entries[j].SetTotalProbability(num);
			}
		}

		// Token: 0x060033B6 RID: 13238 RVA: 0x001627C0 File Offset: 0x001609C0
		public T GetEntry(System.Random seed = null, bool debugLog = false)
		{
			if (this.m_entries == null || this.m_entries.Length == 0)
			{
				return default(T);
			}
			T[] entries = this.m_entries;
			if (!this.m_initialized)
			{
				this.NormalizeInternal();
				this.SortArray(entries);
				this.SetThresholds(entries);
				this.m_initialized = Application.isPlaying;
			}
			float num = (seed != null) ? ((float)seed.NextDouble()) : UnityEngine.Random.Range(0f, 1f);
			for (int i = 0; i < entries.Length; i++)
			{
				if (num < entries[i].Threshold)
				{
					return entries[i];
				}
			}
			return default(T);
		}

		// Token: 0x060033B7 RID: 13239 RVA: 0x00063882 File Offset: 0x00061A82
		private void SortArray(T[] values)
		{
			Array.Sort<T>(values, ProbabilityCollection<T>.m_comparison);
		}

		// Token: 0x060033B8 RID: 13240 RVA: 0x00162868 File Offset: 0x00160A68
		private void SetThresholds(T[] values)
		{
			float num = 0f;
			for (int i = 0; i < values.Length; i++)
			{
				num += values[i].NormalizedProbability;
				values[i].Threshold = num;
			}
		}

		// Token: 0x040031A4 RID: 12708
		[SerializeField]
		private T[] m_entries;

		// Token: 0x040031A5 RID: 12709
		[NonSerialized]
		private T[] m_editorEntries;

		// Token: 0x040031A6 RID: 12710
		[NonSerialized]
		private bool m_initialized;

		// Token: 0x040031A7 RID: 12711
		private static readonly Comparison<T> m_comparison = (T a, T b) => b.NormalizedProbability.CompareTo(a.NormalizedProbability);
	}
}

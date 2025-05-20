using System;
using System.Collections;
using System.Globalization;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Spawning
{
	// Token: 0x0200069E RID: 1694
	[Serializable]
	public abstract class ProbabilityEntry<T> : IProbabilityEntry
	{
		// Token: 0x17000B31 RID: 2865
		// (get) Token: 0x060033CC RID: 13260 RVA: 0x000638FA File Offset: 0x00061AFA
		public T Obj
		{
			get
			{
				return this.m_obj;
			}
		}

		// Token: 0x17000B32 RID: 2866
		// (get) Token: 0x060033CD RID: 13261 RVA: 0x00063902 File Offset: 0x00061B02
		public float Probability
		{
			get
			{
				return this.m_probability;
			}
		}

		// Token: 0x17000B33 RID: 2867
		// (get) Token: 0x060033CE RID: 13262 RVA: 0x0006390A File Offset: 0x00061B0A
		public float NormalizedProbability
		{
			get
			{
				return this.m_normalizedProbability;
			}
		}

		// Token: 0x17000B34 RID: 2868
		// (get) Token: 0x060033CF RID: 13263 RVA: 0x00063912 File Offset: 0x00061B12
		// (set) Token: 0x060033D0 RID: 13264 RVA: 0x0006391A File Offset: 0x00061B1A
		public float Threshold { get; set; }

		// Token: 0x060033D1 RID: 13265 RVA: 0x00063923 File Offset: 0x00061B23
		public void SetTotalProbability(float total)
		{
			this.m_normalizedProbability = this.m_probability / total;
		}

		// Token: 0x060033D2 RID: 13266 RVA: 0x00049FFA File Offset: 0x000481FA
		protected virtual IEnumerable GetDropdownEntries()
		{
			return null;
		}

		// Token: 0x060033D3 RID: 13267 RVA: 0x001628FC File Offset: 0x00160AFC
		private string GetMessage()
		{
			return "Normalized Prob: " + (this.m_normalizedProbability * 100f).ToString("F2", CultureInfo.InvariantCulture) + "%";
		}

		// Token: 0x060033D4 RID: 13268 RVA: 0x00045BC3 File Offset: 0x00043DC3
		private string GetDetails()
		{
			return string.Empty;
		}

		// Token: 0x060033D5 RID: 13269 RVA: 0x00063933 File Offset: 0x00061B33
		public void CloneFrom(ProbabilityEntry<T> from)
		{
			this.m_obj = from.m_obj;
			this.m_probability = from.m_probability;
			this.m_normalizedProbability = from.m_normalizedProbability;
			this.Threshold = from.Threshold;
		}

		// Token: 0x040031A9 RID: 12713
		[SerializeField]
		private T m_obj;

		// Token: 0x040031AA RID: 12714
		[Range(0f, 1f)]
		[SerializeField]
		private float m_probability = 1f;

		// Token: 0x040031AB RID: 12715
		[HideInInspector]
		[SerializeField]
		private float m_normalizedProbability;

		// Token: 0x040031AD RID: 12717
		[SerializeField]
		private DummyClass m_dummy;
	}
}

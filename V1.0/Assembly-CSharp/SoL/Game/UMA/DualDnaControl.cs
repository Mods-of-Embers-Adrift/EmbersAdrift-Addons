using System;
using System.Collections.Generic;
using UMA;
using UMA.CharacterSystem;
using UnityEngine;

namespace SoL.Game.UMA
{
	// Token: 0x0200061E RID: 1566
	public class DualDnaControl
	{
		// Token: 0x17000A92 RID: 2706
		// (get) Token: 0x06003198 RID: 12696 RVA: 0x0006230B File Offset: 0x0006050B
		private Dictionary<string, DnaSetter> DNA
		{
			get
			{
				if (this.m_dna == null && this.m_dca != null)
				{
					this.m_dna = this.m_dca.GetDNA(null);
				}
				return this.m_dna;
			}
		}

		// Token: 0x06003199 RID: 12697 RVA: 0x0006233B File Offset: 0x0006053B
		public DualDnaControl(DynamicCharacterAvatar dca, string dnaLeft, string dnaRight, float startingValue = 0.5f)
		{
			this.m_dca = dca;
			this.m_dnaLeft = dnaLeft;
			this.m_dnaRight = dnaRight;
			this.m_value = startingValue;
		}

		// Token: 0x17000A93 RID: 2707
		// (get) Token: 0x0600319A RID: 12698 RVA: 0x0006236B File Offset: 0x0006056B
		// (set) Token: 0x0600319B RID: 12699 RVA: 0x0015D340 File Offset: 0x0015B540
		public float Value
		{
			get
			{
				return this.m_value;
			}
			set
			{
				this.m_value = value;
				if (this.DNA == null)
				{
					return;
				}
				DnaSetter dnaSetter;
				if (this.m_dna.TryGetValue(this.m_dnaLeft, out dnaSetter))
				{
					if (value >= 0.5f)
					{
						dnaSetter.Set(0f);
					}
					else
					{
						dnaSetter.Set(Mathf.Lerp(1f, 0f, value * 2f));
					}
				}
				DnaSetter dnaSetter2;
				if (this.m_dna.TryGetValue(this.m_dnaRight, out dnaSetter2))
				{
					if (value <= 0.5f)
					{
						dnaSetter2.Set(0f);
						return;
					}
					dnaSetter2.Set(Mathf.Lerp(0f, 1f, (value - 0.5f) * 2f));
				}
			}
		}

		// Token: 0x04003000 RID: 12288
		private const float kDefaultValue = 0.5f;

		// Token: 0x04003001 RID: 12289
		private readonly DynamicCharacterAvatar m_dca;

		// Token: 0x04003002 RID: 12290
		private Dictionary<string, DnaSetter> m_dna;

		// Token: 0x04003003 RID: 12291
		private readonly string m_dnaLeft;

		// Token: 0x04003004 RID: 12292
		private readonly string m_dnaRight;

		// Token: 0x04003005 RID: 12293
		private float m_value = 0.5f;
	}
}

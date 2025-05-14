using System;
using UnityEngine;

namespace SoL.Game.Randomization
{
	// Token: 0x02000766 RID: 1894
	public class ManualRandomizer : MonoBehaviour, IEditorRandomizer
	{
		// Token: 0x0600383A RID: 14394 RVA: 0x0016D0B0 File Offset: 0x0016B2B0
		private void Start()
		{
			int seed = this.m_randomizeSeed ? new System.Random().Next(int.MaxValue) : this.m_seed;
			this.Randomize(seed);
		}

		// Token: 0x0600383B RID: 14395 RVA: 0x0016D0E4 File Offset: 0x0016B2E4
		private void Randomize(int seed)
		{
			this.m_random = new System.Random(seed);
			if (this.m_randomizers != null && this.m_randomizers.Length != 0)
			{
				for (int i = 0; i < this.m_randomizers.Length; i++)
				{
					this.m_randomizers[i].Randomize(this.m_random, null);
				}
			}
		}

		// Token: 0x04003716 RID: 14102
		private System.Random m_random;

		// Token: 0x04003717 RID: 14103
		[SerializeField]
		private bool m_randomizeSeed;

		// Token: 0x04003718 RID: 14104
		[SerializeField]
		private int m_seed;

		// Token: 0x04003719 RID: 14105
		[SerializeField]
		private BaseRandomizer[] m_randomizers;
	}
}

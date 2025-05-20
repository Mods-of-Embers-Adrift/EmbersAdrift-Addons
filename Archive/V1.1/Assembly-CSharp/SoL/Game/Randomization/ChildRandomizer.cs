using System;
using UnityEngine;

namespace SoL.Game.Randomization
{
	// Token: 0x02000762 RID: 1890
	public class ChildRandomizer : MonoBehaviour
	{
		// Token: 0x06003832 RID: 14386 RVA: 0x000664B3 File Offset: 0x000646B3
		private void Get()
		{
			this.m_randomizers = base.gameObject.GetComponentsInChildren<BaseRandomizer>();
		}

		// Token: 0x06003833 RID: 14387 RVA: 0x0016CF48 File Offset: 0x0016B148
		private void Randomize()
		{
			if (this.m_randomizers == null)
			{
				return;
			}
			System.Random seed = new System.Random();
			for (int i = 0; i < this.m_randomizers.Length; i++)
			{
				if (this.m_randomizers[i] != null)
				{
					this.m_randomizers[i].Randomize(seed, null);
				}
			}
		}

		// Token: 0x0400370F RID: 14095
		private const string kRandomizeGroup = "Randomize";

		// Token: 0x04003710 RID: 14096
		[SerializeField]
		private BaseRandomizer[] m_randomizers;
	}
}

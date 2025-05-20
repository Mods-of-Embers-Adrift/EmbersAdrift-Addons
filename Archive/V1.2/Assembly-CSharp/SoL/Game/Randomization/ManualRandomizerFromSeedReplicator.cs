using System;
using UnityEngine;

namespace SoL.Game.Randomization
{
	// Token: 0x02000767 RID: 1895
	public class ManualRandomizerFromSeedReplicator : GameEntityComponent
	{
		// Token: 0x0600383D RID: 14397 RVA: 0x0016D138 File Offset: 0x0016B338
		private void Start()
		{
			if (this.m_randomizers != null && this.m_randomizers.Length != 0 && base.GameEntity && base.GameEntity.SeedReplicator)
			{
				System.Random seed = new System.Random(base.GameEntity.SeedReplicator.Seed);
				for (int i = 0; i < this.m_randomizers.Length; i++)
				{
					if (this.m_randomizers[i])
					{
						this.m_randomizers[i].Randomize(seed, base.GameEntity);
					}
				}
			}
		}

		// Token: 0x0400371A RID: 14106
		[SerializeField]
		private BaseRandomizer[] m_randomizers;
	}
}

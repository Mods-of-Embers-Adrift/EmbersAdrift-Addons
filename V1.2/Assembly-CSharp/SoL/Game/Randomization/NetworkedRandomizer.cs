using System;
using UnityEngine;

namespace SoL.Game.Randomization
{
	// Token: 0x02000771 RID: 1905
	public class NetworkedRandomizer : GameEntityComponent, IEditorRandomizer
	{
		// Token: 0x06003854 RID: 14420 RVA: 0x0016D538 File Offset: 0x0016B738
		protected void Start()
		{
			if (base.GameEntity == null || base.GameEntity.SeedReplicator == null || base.GameEntity.SeedReplicator.Seed == 0)
			{
				return;
			}
			this.Randomize(base.GameEntity.SeedReplicator.Seed);
		}

		// Token: 0x06003855 RID: 14421 RVA: 0x0016D590 File Offset: 0x0016B790
		protected virtual void Randomize(int seed)
		{
			this.m_random = new System.Random(seed);
			if (this.m_randomizers != null && this.m_randomizers.Length != 0)
			{
				for (int i = 0; i < this.m_randomizers.Length; i++)
				{
					this.m_randomizers[i].Randomize(this.m_random, base.GameEntity);
				}
			}
		}

		// Token: 0x04003734 RID: 14132
		protected System.Random m_random;

		// Token: 0x04003735 RID: 14133
		[SerializeField]
		private BaseRandomizer[] m_randomizers;
	}
}

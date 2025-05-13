using System;
using UnityEngine;

namespace SoL.Game.Randomization
{
	// Token: 0x0200076D RID: 1901
	public abstract class MaterialRandomizerBase : BaseRandomizer
	{
		// Token: 0x0600384E RID: 14414 RVA: 0x0016D418 File Offset: 0x0016B618
		protected int GetMaterialIndex(System.Random seed, int length)
		{
			if (!this.m_ignoreColorIndexOverride && this.m_gameEntity && this.m_gameEntity.SeedReplicator != null && this.m_gameEntity.SeedReplicator.VisualIndexOverride != null)
			{
				return Mathf.Clamp((int)this.m_gameEntity.SeedReplicator.VisualIndexOverride.Value, 0, length - 1);
			}
			return seed.Next(0, length);
		}

		// Token: 0x0400372F RID: 14127
		[SerializeField]
		private bool m_ignoreColorIndexOverride;
	}
}

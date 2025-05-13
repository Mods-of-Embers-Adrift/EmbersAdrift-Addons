using System;
using UnityEngine;

namespace SoL.Game.Randomization
{
	// Token: 0x0200075F RID: 1887
	public abstract class BaseRandomizer : MonoBehaviour
	{
		// Token: 0x06003827 RID: 14375
		protected abstract void RandomizeInternal(System.Random seed);

		// Token: 0x06003828 RID: 14376 RVA: 0x00066429 File Offset: 0x00064629
		public void Randomize(System.Random seed, GameEntity entity = null)
		{
			this.m_gameEntity = entity;
			if (!this.m_disableRandomizer)
			{
				this.RandomizeInternal(seed);
			}
		}

		// Token: 0x04003705 RID: 14085
		[SerializeField]
		protected bool m_disableRandomizer;

		// Token: 0x04003706 RID: 14086
		[SerializeField]
		private bool m_silenceRandomizerWarning;

		// Token: 0x04003707 RID: 14087
		[NonSerialized]
		protected GameEntity m_gameEntity;
	}
}

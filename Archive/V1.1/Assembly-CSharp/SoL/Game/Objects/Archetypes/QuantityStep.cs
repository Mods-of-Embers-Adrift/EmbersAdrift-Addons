using System;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000AC0 RID: 2752
	[Serializable]
	public class QuantityStep
	{
		// Token: 0x04004B7A RID: 19322
		public Vector2Int Level = new Vector2Int(1, 100);

		// Token: 0x04004B7B RID: 19323
		[Min(0f)]
		public int AmountToProduce = 1;
	}
}

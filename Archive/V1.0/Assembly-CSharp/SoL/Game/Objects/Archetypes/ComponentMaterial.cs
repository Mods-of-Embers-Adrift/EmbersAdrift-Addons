using System;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000AC6 RID: 2758
	[Serializable]
	public class ComponentMaterial
	{
		// Token: 0x04004BAE RID: 19374
		[SerializeField]
		public ItemArchetype Archetype;

		// Token: 0x04004BAF RID: 19375
		[SerializeField]
		public int AmountRequired = 1;
	}
}

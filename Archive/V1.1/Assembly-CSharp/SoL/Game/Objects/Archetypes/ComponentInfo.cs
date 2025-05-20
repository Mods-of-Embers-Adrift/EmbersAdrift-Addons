using System;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A4B RID: 2635
	[Serializable]
	public class ComponentInfo
	{
		// Token: 0x040048E8 RID: 18664
		[SerializeField]
		public UniqueId RecipeId;

		// Token: 0x040048E9 RID: 18665
		[SerializeField]
		public UniqueId ComponentId;
	}
}

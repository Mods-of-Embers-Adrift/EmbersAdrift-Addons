using System;
using System.Linq;
using SoL.Game.Objects.Archetypes;
using UnityEngine;

namespace SoL.Game.Objects
{
	// Token: 0x020009F7 RID: 2551
	[CreateAssetMenu(menuName = "SoL/Objects/Itemization/Misc/Item Set", order = 4)]
	public class ItemSet : ScriptableObject
	{
		// Token: 0x06004D91 RID: 19857 RVA: 0x00074706 File Offset: 0x00072906
		private void SortItemsByName()
		{
			this.Items = (from x in this.Items
			orderby x.name
			select x).ToArray<ItemArchetype>();
		}

		// Token: 0x0400472E RID: 18222
		public ItemArchetype[] Items;

		// Token: 0x0400472F RID: 18223
		public MasteryArchetype[] Masteries;

		// Token: 0x04004730 RID: 18224
		public AbilityArchetype[] Abilities;
	}
}

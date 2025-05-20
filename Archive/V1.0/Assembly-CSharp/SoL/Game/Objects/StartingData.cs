using System;
using SoL.Game.Objects.Archetypes;
using UnityEngine;

namespace SoL.Game.Objects
{
	// Token: 0x02000A05 RID: 2565
	[CreateAssetMenu(menuName = "SoL/Objects/Itemization/Misc/Starting Data", order = 4)]
	public class StartingData : ScriptableObject
	{
		// Token: 0x04004750 RID: 18256
		public ItemArchetype[] Equipment;

		// Token: 0x04004751 RID: 18257
		public ItemBundle[] Inventory;

		// Token: 0x04004752 RID: 18258
		public MasteryArchetype[] Masteries;

		// Token: 0x04004753 RID: 18259
		public AbilityArchetype[] Abilities;

		// Token: 0x04004754 RID: 18260
		public Recipe[] Recipes;

		// Token: 0x04004755 RID: 18261
		public Emote[] Emotes;
	}
}

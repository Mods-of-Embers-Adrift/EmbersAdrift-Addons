using System;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000AC1 RID: 2753
	[Serializable]
	public class RecipeSubstitution
	{
		// Token: 0x04004B7C RID: 19324
		[Tooltip("The archetypes to look for when determining whether to perform history substitution. All of these must be present in order to perform substitution. If a substitute is selected but this list is empty, the substitution will always be performed.")]
		public ItemArchetype[] WasMadeFrom;

		// Token: 0x04004B7D RID: 19325
		[Tooltip("The archetype with which to substitute the new item's history. Select \"None\" to disable substitution.")]
		public ItemArchetype Substitute;
	}
}

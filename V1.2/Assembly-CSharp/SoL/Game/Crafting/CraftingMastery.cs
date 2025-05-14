using System;
using SoL.Game.Animation;
using SoL.Game.Objects.Archetypes;
using UnityEngine;

namespace SoL.Game.Crafting
{
	// Token: 0x02000CD5 RID: 3285
	[CreateAssetMenu(menuName = "SoL/Objects/Itemization/Skills/Masteries/Crafting/Crafting")]
	public class CraftingMastery : MasteryArchetype
	{
		// Token: 0x0400568D RID: 22157
		[SerializeField]
		protected AbilityAnimation m_refinementAnimation;

		// Token: 0x0400568E RID: 22158
		[SerializeField]
		protected AbilityAnimation m_combinationAnimation;

		// Token: 0x0400568F RID: 22159
		[SerializeField]
		protected AbilityAnimation m_newItemAnimation;
	}
}

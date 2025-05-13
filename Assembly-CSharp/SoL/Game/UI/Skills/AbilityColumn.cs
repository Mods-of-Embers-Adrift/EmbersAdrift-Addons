using System;
using SoL.Game.Objects.Archetypes;

namespace SoL.Game.UI.Skills
{
	// Token: 0x0200091D RID: 2333
	public class AbilityColumn : AbilityGrouping
	{
		// Token: 0x060044B8 RID: 17592 RVA: 0x0019D804 File Offset: 0x0019BA04
		public void AssignBaseAbility(AbilityArchetype baseAbility)
		{
			AbilityArchetype abilityArchetype = baseAbility;
			for (int i = 0; i < this.m_slots.Length; i++)
			{
				this.m_slots[i].AssignAbility(abilityArchetype);
				if (abilityArchetype != null)
				{
					abilityArchetype = abilityArchetype.NextTier;
				}
			}
		}
	}
}

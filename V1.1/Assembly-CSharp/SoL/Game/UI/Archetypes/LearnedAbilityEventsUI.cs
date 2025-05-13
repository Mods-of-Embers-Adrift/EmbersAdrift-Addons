using System;
using SoL.Game.Objects.Archetypes.Abilities;
using UnityEngine;

namespace SoL.Game.UI.Archetypes
{
	// Token: 0x020009D0 RID: 2512
	public class LearnedAbilityEventsUI : AbilityEventsUI<LearnedAbilityCooldownUI>
	{
		// Token: 0x170010E2 RID: 4322
		// (get) Token: 0x06004C7A RID: 19578 RVA: 0x0004479C File Offset: 0x0004299C
		protected override bool ShowTargetOverlay
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06004C7B RID: 19579 RVA: 0x00073BCC File Offset: 0x00071DCC
		protected override void Init(ArchetypeInstanceUI instanceUI)
		{
			base.Init(instanceUI);
			this.m_alchemy.Init(this.m_ui, AbilityCooldownFlags.Alchemy);
		}

		// Token: 0x04004668 RID: 18024
		[SerializeField]
		protected AlchemyCooldownUI m_alchemy;
	}
}

using System;

namespace SoL.Game.UI.Archetypes
{
	// Token: 0x020009CD RID: 2509
	public class LearnedAbilityCooldownUI : AbilityCooldownUI
	{
		// Token: 0x170010DB RID: 4315
		// (get) Token: 0x06004C5F RID: 19551 RVA: 0x00073ACD File Offset: 0x00071CCD
		protected override bool ConsiderHaste
		{
			get
			{
				return this.m_instance.Ability.ConsiderHaste;
			}
		}

		// Token: 0x170010DC RID: 4316
		// (get) Token: 0x06004C60 RID: 19552 RVA: 0x00073ADF File Offset: 0x00071CDF
		protected override bool ClampHasteTo100
		{
			get
			{
				return this.m_instance.Ability.ClampHasteTo100;
			}
		}

		// Token: 0x06004C61 RID: 19553 RVA: 0x001BC920 File Offset: 0x001BAB20
		protected override bool DoInternalUpdate()
		{
			return this.m_instance != null && this.m_instance.AbilityData != null && this.m_instance.AbilityData.Cooldown_Base.Elapsed != null && this.m_instance.Ability;
		}

		// Token: 0x06004C62 RID: 19554 RVA: 0x00073AF1 File Offset: 0x00071CF1
		protected override float GetCooldown()
		{
			return (float)this.m_instance.Ability.GetCooldown(this.m_instance.GetAssociatedLevel(LocalPlayer.GameEntity));
		}
	}
}

using System;

namespace SoL.Game.UI.Archetypes
{
	// Token: 0x020009C3 RID: 2499
	public class AuraEventsUI : AbilityEventsUI<FixedCooldownUI>
	{
		// Token: 0x170010C8 RID: 4296
		// (get) Token: 0x06004C0B RID: 19467 RVA: 0x000736A4 File Offset: 0x000718A4
		protected override bool CanModify
		{
			get
			{
				return base.CanModify && !this.m_auraIsActive;
			}
		}

		// Token: 0x170010C9 RID: 4297
		// (get) Token: 0x06004C0C RID: 19468 RVA: 0x000736B9 File Offset: 0x000718B9
		protected override bool CanPlace
		{
			get
			{
				return !this.m_auraIsActive;
			}
		}

		// Token: 0x170010CA RID: 4298
		// (get) Token: 0x06004C0D RID: 19469 RVA: 0x000736C4 File Offset: 0x000718C4
		protected override bool AuraIsActive
		{
			get
			{
				return this.m_auraIsActive;
			}
		}

		// Token: 0x06004C0E RID: 19470 RVA: 0x000736CC File Offset: 0x000718CC
		protected override void ResetInternal()
		{
			base.ResetInternal();
			this.m_auraCooldown = float.MaxValue;
		}

		// Token: 0x06004C0F RID: 19471 RVA: 0x001BBC20 File Offset: 0x001B9E20
		protected override void LocalPlayerOnActiveAuraChanged(UniqueId previous, UniqueId current)
		{
			if (!previous.IsEmpty && previous == base.m_instance.ArchetypeId)
			{
				this.m_auraIsActive = false;
				this.m_ui.ToggleHighlight(this.m_auraIsActive);
				this.m_primary.StartCooldown(this.m_auraCooldown);
				return;
			}
			if (!current.IsEmpty && current == base.m_instance.ArchetypeId)
			{
				this.m_auraIsActive = true;
				this.m_ui.ToggleHighlight(this.m_auraIsActive);
			}
		}

		// Token: 0x06004C10 RID: 19472 RVA: 0x000736DF File Offset: 0x000718DF
		protected override void AbilityTimeOfLastUseChanged()
		{
			this.m_auraCooldown = (float)base.m_instance.Ability.GetCooldown(base.m_instance.GetAssociatedLevel(LocalPlayer.GameEntity));
		}

		// Token: 0x04004639 RID: 17977
		private bool m_auraIsActive;

		// Token: 0x0400463A RID: 17978
		private float m_auraCooldown = float.MaxValue;
	}
}

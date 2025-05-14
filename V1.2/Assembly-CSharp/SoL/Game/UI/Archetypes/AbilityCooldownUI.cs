using System;
using SoL.Game.EffectSystem;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Archetypes.Abilities;
using SoL.Utilities.Extensions;

namespace SoL.Game.UI.Archetypes
{
	// Token: 0x020009C5 RID: 2501
	public abstract class AbilityCooldownUI : BaseCooldownUI
	{
		// Token: 0x170010D2 RID: 4306
		// (get) Token: 0x06004C2E RID: 19502 RVA: 0x000738DB File Offset: 0x00071ADB
		// (set) Token: 0x06004C2F RID: 19503 RVA: 0x001BC0C0 File Offset: 0x001BA2C0
		public override bool IsActive
		{
			get
			{
				return this.m_isActive;
			}
			protected set
			{
				if (this.m_isActive == value)
				{
					return;
				}
				this.m_isActive = value;
				ArchetypeInstance instance = this.m_instance;
				if (((instance != null) ? instance.AbilityData : null) != null)
				{
					if (this.m_isActive)
					{
						this.m_instance.AbilityData.CooldownFlags |= this.m_flagBit;
						return;
					}
					this.m_instance.AbilityData.CooldownFlags &= ~this.m_flagBit;
				}
			}
		}

		// Token: 0x06004C30 RID: 19504
		protected abstract bool DoInternalUpdate();

		// Token: 0x06004C31 RID: 19505
		protected abstract float GetCooldown();

		// Token: 0x170010D3 RID: 4307
		// (get) Token: 0x06004C32 RID: 19506
		protected abstract bool ConsiderHaste { get; }

		// Token: 0x170010D4 RID: 4308
		// (get) Token: 0x06004C33 RID: 19507
		protected abstract bool ClampHasteTo100 { get; }

		// Token: 0x06004C34 RID: 19508 RVA: 0x001BC138 File Offset: 0x001BA338
		protected override void UpdateInternal()
		{
			base.UpdateInternal();
			if (!this.DoInternalUpdate())
			{
				this.IsActive = false;
				if (this.m_overlay.fillAmount > 0f)
				{
					this.m_overlay.fillAmount = 0f;
					this.m_ui.CenterLabel.SetText(string.Empty);
				}
				return;
			}
			float value = this.m_instance.AbilityData.Cooldown_Base.Elapsed.Value;
			float cooldown = this.GetCooldown();
			float num = value / cooldown;
			if (num >= 1f)
			{
				this.IsActive = false;
				this.m_overlay.fillAmount = 0f;
				this.m_ui.CenterLabel.SetText(string.Empty);
				return;
			}
			this.IsActive = true;
			this.m_overlay.fillAmount = (this.m_flagBit.IncreaseCooldown() ? num : (1f - num));
			float value2 = cooldown - value;
			if (this.ConsiderHaste && LocalPlayer.GameEntity && LocalPlayer.GameEntity.Vitals)
			{
				float num2 = (float)LocalPlayer.GameEntity.Vitals.GetHaste() * 0.01f;
				if (num2 > 0f || num2 < 0f)
				{
					value2 = StatTypeExtensions.GetHasteAdjustedTimeRemaining(cooldown, num2, num, this.ClampHasteTo100);
				}
			}
			this.m_ui.CenterLabel.SetFormattedTime(value2, false);
		}

		// Token: 0x06004C35 RID: 19509 RVA: 0x000738E3 File Offset: 0x00071AE3
		public override void ClearCooldown()
		{
			this.IsActive = false;
			base.ClearCooldown();
		}

		// Token: 0x06004C36 RID: 19510 RVA: 0x000738F2 File Offset: 0x00071AF2
		public override void ResetCooldown()
		{
			this.IsActive = false;
			base.ResetCooldown();
		}

		// Token: 0x04004644 RID: 17988
		private bool m_isActive;
	}
}

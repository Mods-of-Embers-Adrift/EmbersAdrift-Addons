using System;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Archetypes.Abilities;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.UI.Archetypes
{
	// Token: 0x020009CB RID: 2507
	public class FixedCooldownUI : BaseCooldownUI
	{
		// Token: 0x170010D9 RID: 4313
		// (get) Token: 0x06004C55 RID: 19541 RVA: 0x00073A87 File Offset: 0x00071C87
		// (set) Token: 0x06004C56 RID: 19542 RVA: 0x001BC758 File Offset: 0x001BA958
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

		// Token: 0x170010DA RID: 4314
		// (get) Token: 0x06004C57 RID: 19543 RVA: 0x00073A8F File Offset: 0x00071C8F
		internal FixedCooldownUI.CooldownTimingData? Timing
		{
			get
			{
				return this.m_timing;
			}
		}

		// Token: 0x06004C58 RID: 19544 RVA: 0x001BC7D0 File Offset: 0x001BA9D0
		protected override void UpdateInternal()
		{
			base.UpdateInternal();
			this.IsActive = (this.m_timing != null);
			if (!this.IsActive)
			{
				return;
			}
			float num = Time.time - this.m_timing.Value.Initiated;
			float num2 = num / this.m_timing.Value.Duration;
			AbilityCooldownFlags flagBit = this.m_flagBit;
			if (flagBit - AbilityCooldownFlags.Regular <= 1 || flagBit == AbilityCooldownFlags.Memorization)
			{
				this.m_ui.CenterLabel.SetFormattedTime(this.m_timing.Value.Duration - num, false);
			}
			if (num2 >= 1f)
			{
				this.m_timing = null;
				this.m_overlay.fillAmount = 0f;
				this.m_ui.CenterLabel.SetText(string.Empty);
				return;
			}
			this.m_overlay.fillAmount = (this.m_flagBit.IncreaseCooldown() ? num2 : (1f - num2));
		}

		// Token: 0x06004C59 RID: 19545 RVA: 0x001BC8B8 File Offset: 0x001BAAB8
		public void StartCooldown(float duration)
		{
			this.m_timing = new FixedCooldownUI.CooldownTimingData?(new FixedCooldownUI.CooldownTimingData
			{
				Initiated = Time.time,
				Duration = duration
			});
			this.IsActive = (duration > 0f);
		}

		// Token: 0x06004C5A RID: 19546 RVA: 0x001BC8FC File Offset: 0x001BAAFC
		public void StartCooldown(float initiated, float duration)
		{
			this.m_timing = new FixedCooldownUI.CooldownTimingData?(new FixedCooldownUI.CooldownTimingData
			{
				Initiated = initiated,
				Duration = duration
			});
			this.IsActive = (duration > 0f);
		}

		// Token: 0x06004C5B RID: 19547 RVA: 0x00073A97 File Offset: 0x00071C97
		public override void ClearCooldown()
		{
			this.IsActive = false;
			base.ClearCooldown();
			this.m_timing = null;
		}

		// Token: 0x06004C5C RID: 19548 RVA: 0x00073AB2 File Offset: 0x00071CB2
		public override void ResetCooldown()
		{
			this.IsActive = false;
			base.ResetCooldown();
			this.m_timing = null;
		}

		// Token: 0x06004C5D RID: 19549 RVA: 0x001BC93C File Offset: 0x001BAB3C
		public void AddTime(float seconds)
		{
			if (this.m_timing != null)
			{
				FixedCooldownUI.CooldownTimingData value = this.m_timing.Value;
				value.Initiated += seconds;
				this.m_timing = new FixedCooldownUI.CooldownTimingData?(value);
			}
		}

		// Token: 0x0400465E RID: 18014
		private bool m_isActive;

		// Token: 0x0400465F RID: 18015
		private FixedCooldownUI.CooldownTimingData? m_timing;

		// Token: 0x020009CC RID: 2508
		internal struct CooldownTimingData
		{
			// Token: 0x04004660 RID: 18016
			public float Initiated;

			// Token: 0x04004661 RID: 18017
			public float Duration;
		}
	}
}

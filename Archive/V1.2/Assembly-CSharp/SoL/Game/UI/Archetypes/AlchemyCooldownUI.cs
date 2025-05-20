using System;
using SoL.Game.EffectSystem;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Settings;
using SoL.Utilities.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace SoL.Game.UI.Archetypes
{
	// Token: 0x020009C6 RID: 2502
	public class AlchemyCooldownUI : BaseCooldownUI
	{
		// Token: 0x06004C38 RID: 19512 RVA: 0x001BC294 File Offset: 0x001BA494
		private static void AssignColors()
		{
			if (!AlchemyCooldownUI.m_assignedColors)
			{
				AlchemyCooldownUI.m_alchemyIColor = GlobalSettings.Values.Ashen.GetUIHighlightColor(AlchemyPowerLevel.I);
				float num;
				float s;
				float v;
				Color.RGBToHSV(AlchemyCooldownUI.m_alchemyIColor, out num, out s, out v);
				num += 0.055555556f;
				AlchemyCooldownUI.m_alchemyIEdgeColor = Color.HSVToRGB(num, s, v);
				AlchemyCooldownUI.m_alchemyIIColor = GlobalSettings.Values.Ashen.GetUIHighlightColor(AlchemyPowerLevel.II);
				Color.RGBToHSV(AlchemyCooldownUI.m_alchemyIColor, out num, out s, out v);
				num += 0.055555556f;
				AlchemyCooldownUI.m_alchemyIIEdgeColor = Color.HSVToRGB(num, s, v);
				AlchemyCooldownUI.m_assignedColors = true;
			}
		}

		// Token: 0x06004C39 RID: 19513 RVA: 0x00073909 File Offset: 0x00071B09
		private static Color GetFillColor(AlchemyPowerLevel alchemyPowerLevel)
		{
			AlchemyCooldownUI.AssignColors();
			if (alchemyPowerLevel == AlchemyPowerLevel.I)
			{
				return AlchemyCooldownUI.m_alchemyIColor;
			}
			if (alchemyPowerLevel != AlchemyPowerLevel.II)
			{
				return Color.white;
			}
			return AlchemyCooldownUI.m_alchemyIIColor;
		}

		// Token: 0x06004C3A RID: 19514 RVA: 0x0007392B File Offset: 0x00071B2B
		private static Color GetEdgeColor(AlchemyPowerLevel alchemyPowerLevel)
		{
			AlchemyCooldownUI.AssignColors();
			if (alchemyPowerLevel == AlchemyPowerLevel.I)
			{
				return AlchemyCooldownUI.m_alchemyIEdgeColor;
			}
			if (alchemyPowerLevel != AlchemyPowerLevel.II)
			{
				return Color.white;
			}
			return AlchemyCooldownUI.m_alchemyIIEdgeColor;
		}

		// Token: 0x170010D5 RID: 4309
		// (get) Token: 0x06004C3B RID: 19515 RVA: 0x0007394D File Offset: 0x00071B4D
		// (set) Token: 0x06004C3C RID: 19516 RVA: 0x00073955 File Offset: 0x00071B55
		public override bool IsActive { get; protected set; }

		// Token: 0x06004C3D RID: 19517 RVA: 0x001BC324 File Offset: 0x001BA524
		protected override void UpdateInternal()
		{
			base.UpdateInternal();
			if (this.m_instance == null || this.m_instance.AbilityData == null)
			{
				this.Disable();
				return;
			}
			CooldownData cooldown_AlchemyI = this.m_instance.AbilityData.Cooldown_AlchemyI;
			CooldownData cooldown_AlchemyII = this.m_instance.AbilityData.Cooldown_AlchemyII;
			if (cooldown_AlchemyI.Active && cooldown_AlchemyII.Active)
			{
				this.m_single.UpdateCooldown(0f, AlchemyPowerLevel.None);
				this.m_doubleI.UpdateCooldown(this.GetFillAmount(cooldown_AlchemyI), AlchemyPowerLevel.I);
				this.m_doubleII.UpdateCooldown(this.GetFillAmount(cooldown_AlchemyII), AlchemyPowerLevel.II);
				this.m_doubleBorder.enabled = true;
				return;
			}
			if (cooldown_AlchemyII.Active)
			{
				this.m_single.UpdateCooldown(this.GetFillAmount(cooldown_AlchemyII), AlchemyPowerLevel.II);
				this.m_doubleI.UpdateCooldown(0f, AlchemyPowerLevel.None);
				this.m_doubleII.UpdateCooldown(0f, AlchemyPowerLevel.None);
				this.m_doubleBorder.enabled = false;
				return;
			}
			if (cooldown_AlchemyI.Active)
			{
				this.m_single.UpdateCooldown(this.GetFillAmount(cooldown_AlchemyI), AlchemyPowerLevel.I);
				this.m_doubleI.UpdateCooldown(0f, AlchemyPowerLevel.None);
				this.m_doubleII.UpdateCooldown(0f, AlchemyPowerLevel.None);
				this.m_doubleBorder.enabled = false;
				return;
			}
			this.Disable();
		}

		// Token: 0x06004C3E RID: 19518 RVA: 0x001BC46C File Offset: 0x001BA66C
		private void Disable()
		{
			this.m_single.UpdateCooldown(0f, AlchemyPowerLevel.None);
			this.m_doubleI.UpdateCooldown(0f, AlchemyPowerLevel.None);
			this.m_doubleII.UpdateCooldown(0f, AlchemyPowerLevel.None);
			this.m_doubleBorder.enabled = false;
		}

		// Token: 0x06004C3F RID: 19519 RVA: 0x001BC4B8 File Offset: 0x001BA6B8
		private float GetFillAmount(CooldownData cooldownData)
		{
			if (cooldownData != null && cooldownData.Elapsed != null && cooldownData.Cooldown != null)
			{
				return (cooldownData.Elapsed.Value / cooldownData.Cooldown.Value).Remap(0f, 1f, 0.1f, 1f);
			}
			return 0f;
		}

		// Token: 0x04004645 RID: 17989
		private const float kEdgeFillDelta = 0.01f;

		// Token: 0x04004646 RID: 17990
		private const float kEdgeHueDelta = 0.055555556f;

		// Token: 0x04004647 RID: 17991
		private static bool m_assignedColors;

		// Token: 0x04004648 RID: 17992
		private static Color m_alchemyIColor;

		// Token: 0x04004649 RID: 17993
		private static Color m_alchemyIEdgeColor;

		// Token: 0x0400464A RID: 17994
		private static Color m_alchemyIIColor;

		// Token: 0x0400464B RID: 17995
		private static Color m_alchemyIIEdgeColor;

		// Token: 0x0400464C RID: 17996
		[SerializeField]
		private AlchemyCooldownUI.AlchemyCooldown m_single;

		// Token: 0x0400464D RID: 17997
		[SerializeField]
		private AlchemyCooldownUI.AlchemyCooldown m_doubleI;

		// Token: 0x0400464E RID: 17998
		[SerializeField]
		private AlchemyCooldownUI.AlchemyCooldown m_doubleII;

		// Token: 0x0400464F RID: 17999
		[SerializeField]
		private Image m_doubleBorder;

		// Token: 0x020009C7 RID: 2503
		[Serializable]
		private class AlchemyCooldown
		{
			// Token: 0x06004C41 RID: 19521 RVA: 0x001BC524 File Offset: 0x001BA724
			public void UpdateCooldown(float fillAmount, AlchemyPowerLevel alchemyPowerLevel)
			{
				if (this.m_fill)
				{
					this.m_fill.fillAmount = fillAmount;
					if (fillAmount > 0f)
					{
						this.m_fill.color = AlchemyCooldownUI.GetFillColor(alchemyPowerLevel);
					}
				}
				if (this.m_background)
				{
					this.m_background.fillAmount = ((fillAmount > 0f) ? 1f : 0f);
				}
				if (this.m_leadingEdge)
				{
					this.m_leadingEdge.fillAmount = ((fillAmount > 0f) ? Mathf.Clamp(fillAmount + 0.01f, 0f, 1f) : 0f);
					if (fillAmount > 0f)
					{
						this.m_leadingEdge.color = AlchemyCooldownUI.GetEdgeColor(alchemyPowerLevel);
					}
				}
			}

			// Token: 0x04004651 RID: 18001
			[SerializeField]
			private Image m_fill;

			// Token: 0x04004652 RID: 18002
			[SerializeField]
			private Image m_background;

			// Token: 0x04004653 RID: 18003
			[SerializeField]
			private Image m_leadingEdge;
		}
	}
}

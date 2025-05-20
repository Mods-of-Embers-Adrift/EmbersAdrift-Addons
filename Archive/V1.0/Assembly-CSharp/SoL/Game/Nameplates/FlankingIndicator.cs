using System;
using SoL.Game.Flanking;
using SoL.Game.Settings;
using SoL.UI;
using SoL.Utilities.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace SoL.Game.Nameplates
{
	// Token: 0x020009D4 RID: 2516
	public class FlankingIndicator : MonoBehaviour
	{
		// Token: 0x170010ED RID: 4333
		// (get) Token: 0x06004C99 RID: 19609 RVA: 0x00073D59 File Offset: 0x00071F59
		// (set) Token: 0x06004C9A RID: 19610 RVA: 0x001BD174 File Offset: 0x001BB374
		private FlankingPosition FlankingPosition
		{
			get
			{
				return this.m_flankingPosition;
			}
			set
			{
				if (this.m_flankingPosition == value)
				{
					return;
				}
				this.m_flankingPosition = value;
				switch (this.m_flankingPosition)
				{
				case FlankingPosition.Front:
					this.m_front.enabled = true;
					this.m_side.enabled = false;
					this.m_rear.enabled = false;
					return;
				case FlankingPosition.Sides:
					this.m_front.enabled = false;
					this.m_side.enabled = true;
					this.m_rear.enabled = false;
					return;
				case FlankingPosition.Rear:
					this.m_front.enabled = false;
					this.m_side.enabled = false;
					this.m_rear.enabled = true;
					return;
				default:
					return;
				}
			}
		}

		// Token: 0x06004C9B RID: 19611 RVA: 0x001BD21C File Offset: 0x001BB41C
		private void Awake()
		{
			this.m_defaultFrontColor = this.m_front.color;
			this.m_defaultSideColor = this.m_side.color;
			this.m_defaultRearColor = this.m_rear.color;
			this.m_front.enabled = true;
			this.m_side.enabled = false;
			this.m_rear.enabled = false;
		}

		// Token: 0x06004C9C RID: 19612 RVA: 0x001BD280 File Offset: 0x001BB480
		private void Update()
		{
			if (this.m_nameplateControllerUI && this.m_nameplateControllerUI.Targetable != null && this.m_nameplateControllerUI.Targetable.Entity && this.m_nameplateControllerUI.Targetable.Entity.gameObject && LocalPlayer.GameEntity && LocalPlayer.GameEntity.gameObject)
			{
				float angle = this.m_nameplateControllerUI.Targetable.Entity.gameObject.AngleTo(LocalPlayer.GameEntity.gameObject, true);
				this.FlankingPosition = FlankingPositionExtensions.GetFlankingPosition(angle);
				this.UpdateFlankingPositionColors();
			}
		}

		// Token: 0x06004C9D RID: 19613 RVA: 0x001BD338 File Offset: 0x001BB538
		private void UpdateFlankingPositionColors()
		{
			if (LocalPlayer.GameEntity.HandHeldItemCache == null || !(LocalPlayer.GameEntity.HandHeldItemCache.MainHand.WeaponItem != null))
			{
				this.ResetColors();
				return;
			}
			int num;
			int num2;
			int num3;
			LocalPlayer.GameEntity.HandHeldItemCache.MainHand.WeaponItem.FlankingBonus.GetBonusValues(out num, out num2, out num3);
			int num4 = Mathf.Min(new int[]
			{
				num,
				num2,
				num3
			});
			int num5 = Mathf.Max(new int[]
			{
				num,
				num2,
				num3
			});
			if (num4 == 0 && num5 == 0)
			{
				this.ResetColors();
				return;
			}
			this.SetColor(this.m_front, this.m_defaultFrontColor, num, num4, num5);
			this.SetColor(this.m_side, this.m_defaultSideColor, num2, num4, num5);
			this.SetColor(this.m_rear, this.m_defaultRearColor, num3, num4, num5);
		}

		// Token: 0x06004C9E RID: 19614 RVA: 0x001BD420 File Offset: 0x001BB620
		private void SetColor(Image img, Color defaultColor, int value, int min, int max)
		{
			Color color;
			if (value == max)
			{
				color = GlobalSettings.Values.UI.MaxFlankingColor;
			}
			else if (value == min)
			{
				color = GlobalSettings.Values.UI.MinFlankingColor;
			}
			else
			{
				color = GlobalSettings.Values.UI.MidFlankingColor;
			}
			img.color = color;
		}

		// Token: 0x06004C9F RID: 19615 RVA: 0x00073D61 File Offset: 0x00071F61
		private void ResetColors()
		{
			this.m_front.color = this.m_defaultFrontColor;
			this.m_side.color = this.m_defaultSideColor;
			this.m_rear.color = this.m_defaultRearColor;
		}

		// Token: 0x04004676 RID: 18038
		[SerializeField]
		private NameplateControllerUI m_nameplateControllerUI;

		// Token: 0x04004677 RID: 18039
		[SerializeField]
		private Image m_front;

		// Token: 0x04004678 RID: 18040
		[SerializeField]
		private Image m_side;

		// Token: 0x04004679 RID: 18041
		[SerializeField]
		private Image m_rear;

		// Token: 0x0400467A RID: 18042
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x0400467B RID: 18043
		private Color m_defaultFrontColor;

		// Token: 0x0400467C RID: 18044
		private Color m_defaultSideColor;

		// Token: 0x0400467D RID: 18045
		private Color m_defaultRearColor;

		// Token: 0x0400467E RID: 18046
		private FlankingPosition m_flankingPosition;
	}
}

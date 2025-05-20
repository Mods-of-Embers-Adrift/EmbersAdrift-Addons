using System;
using Cysharp.Text;
using SoL.Game.EffectSystem;
using SoL.Game.Flanking;
using SoL.Game.Interactives;
using SoL.Managers;
using SoL.UI;
using SoL.Utilities.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace SoL.Game.Nameplates
{
	// Token: 0x020009D5 RID: 2517
	public class FlankingIndicatorV2 : MonoBehaviour, ITooltip, IInteractiveBase
	{
		// Token: 0x06004CA1 RID: 19617 RVA: 0x00073D96 File Offset: 0x00071F96
		private void Awake()
		{
			this.m_defaultColor = this.m_image.color;
		}

		// Token: 0x06004CA2 RID: 19618 RVA: 0x001BD4D0 File Offset: 0x001BB6D0
		private void Update()
		{
			if (this.m_nameplateControllerUI && this.m_nameplateControllerUI.Targetable != null && this.m_nameplateControllerUI.Targetable.Entity && this.m_nameplateControllerUI.Targetable.Entity.gameObject && LocalPlayer.GameEntity && LocalPlayer.GameEntity.gameObject)
			{
				float angle = this.m_nameplateControllerUI.Targetable.Entity.gameObject.AngleTo(LocalPlayer.GameEntity.gameObject, true);
				this.m_flankingPosition = FlankingPositionExtensions.GetFlankingPosition(angle);
				this.UpdateFlankingIndicator();
			}
		}

		// Token: 0x06004CA3 RID: 19619 RVA: 0x001BD588 File Offset: 0x001BB788
		private void UpdateFlankingIndicator()
		{
			if (LocalPlayer.GameEntity.HandHeldItemCache == null || !LocalPlayer.GameEntity.HandHeldItemCache.MainHand.WeaponItem)
			{
				this.m_flankingPosition = FlankingPosition.Front;
				this.m_currentFlankingBonus = null;
				return;
			}
			this.m_currentFlankingBonus = LocalPlayer.GameEntity.HandHeldItemCache.MainHand.WeaponItem.FlankingBonus;
			float num;
			float num2;
			float num3;
			this.m_currentFlankingBonus.GetMultiplierValues(out num, out num2, out num3);
			FlankingPositionExtensions.BonusArray[0] = num;
			FlankingPositionExtensions.BonusArray[1] = num2;
			FlankingPositionExtensions.BonusArray[2] = num3;
			float num4 = Mathf.Min(FlankingPositionExtensions.BonusArray);
			float num5 = Mathf.Max(FlankingPositionExtensions.BonusArray);
			if (num4 == 0f && num5 == 0f)
			{
				this.m_image.enabled = false;
				return;
			}
			float num6 = 0f;
			float d = 0f;
			switch (this.m_flankingPosition)
			{
			case FlankingPosition.Front:
				num6 = num;
				d = 0f;
				break;
			case FlankingPosition.Sides:
				num6 = num2;
				d = 90f;
				break;
			case FlankingPosition.Rear:
				num6 = num3;
				d = 180f;
				break;
			}
			Sprite overrideSprite;
			if (num6 >= num5)
			{
				overrideSprite = this.m_fullArrow;
			}
			else if (num6 <= num4)
			{
				overrideSprite = this.m_emptyArrow;
			}
			else
			{
				overrideSprite = this.m_halfArrow;
			}
			this.m_toRotate.localRotation = Quaternion.Euler(Vector3.forward * d);
			this.m_image.overrideSprite = overrideSprite;
			this.m_image.enabled = true;
		}

		// Token: 0x06004CA4 RID: 19620 RVA: 0x001BD6FC File Offset: 0x001BB8FC
		private ITooltipParameter GetTooltipParameter()
		{
			if (this.m_currentFlankingBonus == null)
			{
				return null;
			}
			string text = this.m_flankingPosition.GetPositionDescription();
			WeaponFlankingBonusType type;
			int arg;
			float value;
			if (this.m_currentFlankingBonus.TryGetWeaponFlankingBonus(this.m_flankingPosition, out type, out arg, out value))
			{
				using (Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder())
				{
					utf16ValueStringBuilder.AppendLine(text);
					utf16ValueStringBuilder.AppendFormat<int, string, string, string, string>("{0} {1} + {2}% <size=80%><color={3}>{4}</color></size>", arg, type.GetAbbreviation(), value.GetAsPercentage(), UIManager.BlueColor.ToHex(), StatType.Flanking.GetStatPanelDisplay());
					text = utf16ValueStringBuilder.ToString();
				}
			}
			return new ObjectTextTooltipParameter(this, text, false);
		}

		// Token: 0x170010EE RID: 4334
		// (get) Token: 0x06004CA5 RID: 19621 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x170010EF RID: 4335
		// (get) Token: 0x06004CA6 RID: 19622 RVA: 0x00073DA9 File Offset: 0x00071FA9
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x170010F0 RID: 4336
		// (get) Token: 0x06004CA7 RID: 19623 RVA: 0x00073DB7 File Offset: 0x00071FB7
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x06004CA9 RID: 19625 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x0400467F RID: 18047
		[SerializeField]
		private NameplateControllerUI m_nameplateControllerUI;

		// Token: 0x04004680 RID: 18048
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x04004681 RID: 18049
		[SerializeField]
		private RectTransform m_toRotate;

		// Token: 0x04004682 RID: 18050
		[SerializeField]
		private Image m_image;

		// Token: 0x04004683 RID: 18051
		[SerializeField]
		private Sprite m_fullArrow;

		// Token: 0x04004684 RID: 18052
		[SerializeField]
		private Sprite m_halfArrow;

		// Token: 0x04004685 RID: 18053
		[SerializeField]
		private Sprite m_emptyArrow;

		// Token: 0x04004686 RID: 18054
		private Color m_defaultColor;

		// Token: 0x04004687 RID: 18055
		private FlankingPosition m_flankingPosition;

		// Token: 0x04004688 RID: 18056
		private WeaponFlankingBonusWithOverride m_currentFlankingBonus;
	}
}

using System;
using Cysharp.Text;
using SoL.Game.Interactives;
using SoL.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SoL.Game.UI
{
	// Token: 0x0200087B RID: 2171
	public class EmberStoneSlotUI : MonoBehaviour, ITooltip, IInteractiveBase, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
	{
		// Token: 0x06003F2B RID: 16171 RVA: 0x0006ABDF File Offset: 0x00068DDF
		private void Start()
		{
			this.InitHighlight();
		}

		// Token: 0x06003F2C RID: 16172 RVA: 0x00187C70 File Offset: 0x00185E70
		private void OnDestroy()
		{
			if (this.m_equipmentUi && LocalPlayer.GameEntity && LocalPlayer.GameEntity.CollectionController != null)
			{
				LocalPlayer.GameEntity.CollectionController.EmberStoneChanged -= this.RefreshEmberStone;
			}
		}

		// Token: 0x06003F2D RID: 16173 RVA: 0x00187CC0 File Offset: 0x00185EC0
		public void Initialize(EquipmentUI equipmentUi)
		{
			this.m_equipmentUi = equipmentUi;
			if (LocalPlayer.GameEntity != null && LocalPlayer.GameEntity.CollectionController != null)
			{
				LocalPlayer.GameEntity.CollectionController.EmberStoneChanged += this.RefreshEmberStone;
				this.RefreshEmberStone();
			}
		}

		// Token: 0x06003F2E RID: 16174 RVA: 0x00187D10 File Offset: 0x00185F10
		private void RefreshEmberStone()
		{
			if (LocalPlayer.GameEntity != null && LocalPlayer.GameEntity.CollectionController != null && LocalPlayer.GameEntity.CollectionController.CurrentEmberStone != null)
			{
				this.m_icon.color = LocalPlayer.GameEntity.CollectionController.CurrentEmberStone.IconTint;
				this.m_icon.overrideSprite = LocalPlayer.GameEntity.CollectionController.CurrentEmberStone.Icon;
				int maxCapacity = LocalPlayer.GameEntity.CollectionController.CurrentEmberStone.MaxCapacity;
				int emberEssenceCount = LocalPlayer.GameEntity.CollectionController.GetEmberEssenceCount();
				this.m_label.text = emberEssenceCount.ToString();
				this.m_progressMeter.fillAmount = (float)emberEssenceCount / (float)maxCapacity;
				this.m_progressMeterParent.SetActive(true);
				int displayValueForTravelEssence = LocalPlayer.GameEntity.CollectionController.GetDisplayValueForTravelEssence();
				this.m_travelLabel.text = displayValueForTravelEssence.ToString();
				this.m_travelProgressMeter.fillAmount = (float)displayValueForTravelEssence / (float)maxCapacity;
				this.m_travelProgressMeterParent.SetActive(true);
				return;
			}
			this.m_icon.color = Color.white;
			this.m_icon.overrideSprite = null;
			this.m_label.text = string.Empty;
			this.m_progressMeterParent.SetActive(false);
			this.m_travelLabel.text = string.Empty;
			this.m_travelProgressMeterParent.SetActive(false);
		}

		// Token: 0x06003F2F RID: 16175 RVA: 0x0006ABE7 File Offset: 0x00068DE7
		private void InitHighlight()
		{
			if (this.m_highlight != null)
			{
				this.m_highlight.enabled = true;
				this.ToggleHighlight(false, true);
			}
		}

		// Token: 0x06003F30 RID: 16176 RVA: 0x0006AC0B File Offset: 0x00068E0B
		public void ToggleHighlight(bool isEnabled, bool instant = false)
		{
			if (this.m_highlight != null)
			{
				this.m_highlight.CrossFadeAlpha(isEnabled ? 1f : 0f, instant ? 0f : 0.1f, true);
			}
		}

		// Token: 0x06003F31 RID: 16177 RVA: 0x0006AC45 File Offset: 0x00068E45
		public void OnPointerEnter(PointerEventData eventData)
		{
			this.ToggleHighlight(true, false);
		}

		// Token: 0x06003F32 RID: 16178 RVA: 0x0006AC4F File Offset: 0x00068E4F
		public void OnPointerExit(PointerEventData eventData)
		{
			this.ToggleHighlight(false, false);
		}

		// Token: 0x06003F33 RID: 16179 RVA: 0x00187E7C File Offset: 0x0018607C
		private ITooltipParameter GetTooltipParameter()
		{
			string txt = string.Empty;
			if (LocalPlayer.GameEntity && LocalPlayer.GameEntity.CollectionController != null && LocalPlayer.GameEntity.CollectionController.CurrentEmberStone != null)
			{
				using (Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder())
				{
					utf16ValueStringBuilder.Append(LocalPlayer.GameEntity.CollectionController.CurrentEmberStone.DisplayName);
					utf16ValueStringBuilder.AppendLine();
					utf16ValueStringBuilder.AppendFormat<int, int>(" Travel: {0}/{1}", LocalPlayer.GameEntity.CollectionController.GetDisplayValueForTravelEssence(), LocalPlayer.GameEntity.CollectionController.CurrentEmberStone.MaxCapacity);
					utf16ValueStringBuilder.AppendLine();
					utf16ValueStringBuilder.AppendFormat<int, int>("Essence: {0}/{1}", LocalPlayer.GameEntity.CollectionController.GetEmberEssenceCount(), LocalPlayer.GameEntity.CollectionController.CurrentEmberStone.MaxCapacity);
					txt = utf16ValueStringBuilder.ToString();
					goto IL_EF;
				}
			}
			txt = "Ember Stone Slot";
			IL_EF:
			return new ObjectTextTooltipParameter(this, txt, false);
		}

		// Token: 0x17000E9B RID: 3739
		// (get) Token: 0x06003F34 RID: 16180 RVA: 0x0006AC59 File Offset: 0x00068E59
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x17000E9C RID: 3740
		// (get) Token: 0x06003F35 RID: 16181 RVA: 0x0006AC67 File Offset: 0x00068E67
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x17000E9D RID: 3741
		// (get) Token: 0x06003F36 RID: 16182 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x06003F38 RID: 16184 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04003D17 RID: 15639
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x04003D18 RID: 15640
		[SerializeField]
		private Image m_icon;

		// Token: 0x04003D19 RID: 15641
		[SerializeField]
		private Image m_highlight;

		// Token: 0x04003D1A RID: 15642
		[SerializeField]
		private TextMeshProUGUI m_label;

		// Token: 0x04003D1B RID: 15643
		[SerializeField]
		private Image m_progressMeter;

		// Token: 0x04003D1C RID: 15644
		[SerializeField]
		private GameObject m_progressMeterParent;

		// Token: 0x04003D1D RID: 15645
		[SerializeField]
		private TextMeshProUGUI m_travelLabel;

		// Token: 0x04003D1E RID: 15646
		[SerializeField]
		private Image m_travelProgressMeter;

		// Token: 0x04003D1F RID: 15647
		[SerializeField]
		private GameObject m_travelProgressMeterParent;

		// Token: 0x04003D20 RID: 15648
		private EquipmentUI m_equipmentUi;
	}
}

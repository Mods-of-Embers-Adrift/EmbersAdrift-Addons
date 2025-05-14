using System;
using Cysharp.Text;
using SoL.Game.Interactives;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Containers;
using SoL.Managers;
using SoL.UI;
using SoL.Utilities.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SoL.Game.UI.Dialog
{
	// Token: 0x0200098E RID: 2446
	public class RewardChoiceItem : MonoBehaviour, ITooltip, IInteractiveBase
	{
		// Token: 0x1700102D RID: 4141
		// (get) Token: 0x060048E5 RID: 18661 RVA: 0x00070FDC File Offset: 0x0006F1DC
		public bool IsSelected
		{
			get
			{
				return this.m_toggle.isOn;
			}
		}

		// Token: 0x1700102E RID: 4142
		// (get) Token: 0x060048E6 RID: 18662 RVA: 0x00070FE9 File Offset: 0x0006F1E9
		public IMerchantInventory Reward
		{
			get
			{
				return this.m_reward;
			}
		}

		// Token: 0x140000DF RID: 223
		// (add) Token: 0x060048E7 RID: 18663 RVA: 0x001ABA54 File Offset: 0x001A9C54
		// (remove) Token: 0x060048E8 RID: 18664 RVA: 0x001ABA8C File Offset: 0x001A9C8C
		public event Action ChoiceChanged;

		// Token: 0x1700102F RID: 4143
		// (get) Token: 0x060048E9 RID: 18665 RVA: 0x00070FF1 File Offset: 0x0006F1F1
		// (set) Token: 0x060048EA RID: 18666 RVA: 0x00070FFE File Offset: 0x0006F1FE
		public bool Interactable
		{
			get
			{
				return this.m_toggle.interactable;
			}
			set
			{
				this.m_toggle.interactable = value;
			}
		}

		// Token: 0x060048EB RID: 18667 RVA: 0x0007100C File Offset: 0x0006F20C
		private void Awake()
		{
			this.m_toggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnToggleChanged));
		}

		// Token: 0x060048EC RID: 18668 RVA: 0x0007102A File Offset: 0x0006F22A
		private void OnDestroy()
		{
			this.m_toggle.onValueChanged.RemoveListener(new UnityAction<bool>(this.OnToggleChanged));
		}

		// Token: 0x060048ED RID: 18669 RVA: 0x00071048 File Offset: 0x0006F248
		public void Init(ToggleGroup toggleGroup)
		{
			this.m_toggle.group = toggleGroup;
			this.m_toggle.SetIsOnWithoutNotify(false);
		}

		// Token: 0x060048EE RID: 18670 RVA: 0x001ABAC4 File Offset: 0x001A9CC4
		public void InitItem(IMerchantInventory thing, uint count = 1U)
		{
			if (!thing.Archetype)
			{
				this.m_reward = null;
				this.m_label.text = "INVALID";
				this.m_toggle.interactable = false;
				return;
			}
			this.m_reward = thing;
			Color color;
			if (thing is LearnableArchetype)
			{
				this.m_label.SetTextFormat("Learn: {0}", thing.Archetype.DisplayName);
			}
			else if (thing.Archetype.TryGetItemCategoryColor(ItemCategory.ColorFlags.DisplayName, out color))
			{
				this.m_label.SetTextFormat("<color={0}>{1}</color>", color.ToHex(), thing.Archetype.DisplayName);
			}
			else
			{
				this.m_label.ZStringSetText(thing.Archetype.DisplayName);
			}
			this.m_countLabel.text = ((count > 1U) ? count.ToString() : null);
			this.m_countGradient.gameObject.SetActive(count > 1U);
			this.m_archetypeIcon.SetIcon(thing.Archetype, new Color?(thing.Archetype.IconTint));
			if (!(this.m_toggle.group != null))
			{
				this.m_toggle.SetIsOnWithoutNotify(false);
				this.m_toggle.interactable = false;
				return;
			}
			if (thing is ItemArchetype)
			{
				this.UpdateItemInteractable();
				return;
			}
			this.m_toggle.interactable = thing.EntityCanAcquire(LocalPlayer.GameEntity, out this.m_errorMessage);
		}

		// Token: 0x060048EF RID: 18671 RVA: 0x001ABC1C File Offset: 0x001A9E1C
		public void UpdateItemInteractable()
		{
			if (this.m_toggle.group == null)
			{
				return;
			}
			IMerchantInventory reward = this.m_reward;
			ItemArchetype itemArchetype = reward as ItemArchetype;
			if (itemArchetype == null)
			{
				LearnableArchetype learnableArchetype = reward as LearnableArchetype;
				if (learnableArchetype == null)
				{
					return;
				}
				this.m_toggle.interactable = learnableArchetype.EntityCanAcquire(LocalPlayer.GameEntity, out this.m_errorMessage);
				return;
			}
			else
			{
				ContainerInstance containerInstance;
				CannotAcquireReason cannotAcquireReason;
				if (LocalPlayer.GameEntity.EntityCanAcquire(itemArchetype, out containerInstance, out cannotAcquireReason, null))
				{
					this.m_toggle.interactable = true;
					this.m_errorMessage = null;
					return;
				}
				this.m_toggle.interactable = false;
				if (cannotAcquireReason == CannotAcquireReason.NoRoom)
				{
					this.m_errorMessage = "Not enough room in inventory";
					return;
				}
				if (cannotAcquireReason == CannotAcquireReason.Dead)
				{
					this.m_errorMessage = "Cannot receive while missing your bag";
					return;
				}
				if (cannotAcquireReason != CannotAcquireReason.Unknown)
				{
					return;
				}
				this.m_errorMessage = "Cannot receive item for unknown reason";
				return;
			}
		}

		// Token: 0x060048F0 RID: 18672 RVA: 0x00071062 File Offset: 0x0006F262
		private void OnToggleChanged(bool value)
		{
			Action choiceChanged = this.ChoiceChanged;
			if (choiceChanged == null)
			{
				return;
			}
			choiceChanged();
		}

		// Token: 0x060048F1 RID: 18673 RVA: 0x001ABCDC File Offset: 0x001A9EDC
		private ITooltipParameter GetTooltipParameter()
		{
			if (this.m_reward == null)
			{
				return null;
			}
			ArchetypeTooltipParameter archetypeTooltipParameter = default(ArchetypeTooltipParameter);
			archetypeTooltipParameter.Archetype = this.m_reward.Archetype;
			archetypeTooltipParameter.AtTrainer = true;
			string errorMessage = this.m_errorMessage;
			archetypeTooltipParameter.AdditionalText = ((errorMessage != null) ? errorMessage.Color(UIManager.RequirementsNotMetColor) : null);
			return archetypeTooltipParameter;
		}

		// Token: 0x17001030 RID: 4144
		// (get) Token: 0x060048F2 RID: 18674 RVA: 0x00071074 File Offset: 0x0006F274
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x17001031 RID: 4145
		// (get) Token: 0x060048F3 RID: 18675 RVA: 0x00071082 File Offset: 0x0006F282
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x17001032 RID: 4146
		// (get) Token: 0x060048F4 RID: 18676 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x060048F6 RID: 18678 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04004412 RID: 17426
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x04004413 RID: 17427
		[SerializeField]
		private ArchetypeIconUI m_archetypeIcon;

		// Token: 0x04004414 RID: 17428
		[SerializeField]
		private TextMeshProUGUI m_label;

		// Token: 0x04004415 RID: 17429
		[SerializeField]
		private TextMeshProUGUI m_countLabel;

		// Token: 0x04004416 RID: 17430
		[SerializeField]
		private Image m_countGradient;

		// Token: 0x04004417 RID: 17431
		[SerializeField]
		private SolToggle m_toggle;

		// Token: 0x04004418 RID: 17432
		private IMerchantInventory m_reward;

		// Token: 0x04004419 RID: 17433
		private string m_errorMessage;
	}
}

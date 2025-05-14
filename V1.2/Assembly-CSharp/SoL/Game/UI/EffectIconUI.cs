using System;
using Cysharp.Text;
using SoL.Game.EffectSystem;
using SoL.Game.Interactives;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Settings;
using SoL.Managers;
using SoL.Networking.Objects;
using SoL.UI;
using SoL.Utilities.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SoL.Game.UI
{
	// Token: 0x02000879 RID: 2169
	public class EffectIconUI : UIWindow, ITooltip, IInteractiveBase, IContextMenu
	{
		// Token: 0x06003F12 RID: 16146 RVA: 0x001872B4 File Offset: 0x001854B4
		public void Init(NetworkEntity entity, EffectSyncData? esd, bool isSelfPanel)
		{
			bool flag = this.m_effectSyncData != null;
			this.m_networkEntity = entity;
			this.m_archetype = null;
			this.m_maxTriggerCount = null;
			this.m_effectSyncData = esd;
			this.m_countPanel.anchoredPosition = (isSelfPanel ? new Vector2(0f, 5f) : Vector2.zero);
			if (this.m_effectSyncData != null)
			{
				EffectSyncData value = this.m_effectSyncData.Value;
				if (value.EffectSource != null)
				{
					IdentifiableScriptableObjectCollection<BaseArchetype> archetypes = InternalGameDatabase.Archetypes;
					value = this.m_effectSyncData.Value;
					if (archetypes.TryGetItem(value.EffectSource.ArchetypeId, out this.m_archetype))
					{
						this.m_icon.overrideSprite = this.m_archetype.Icon;
						goto IL_C4;
					}
				}
				this.m_icon.overrideSprite = null;
				IL_C4:
				Graphic cooldownOverlay = this.m_cooldownOverlay;
				value = this.m_effectSyncData.Value;
				Color color;
				if (value.CombatEffect != null)
				{
					value = this.m_effectSyncData.Value;
					if (value.CombatEffect.Polarity == Polarity.Negative)
					{
						color = UIManager.RedColor;
						goto IL_105;
					}
				}
				color = UIManager.BlueColor;
				IL_105:
				cooldownOverlay.color = color;
				value = this.m_effectSyncData.Value;
				CombatEffect combatEffect = value.CombatEffect;
				int? maxTriggerCount;
				if (((combatEffect != null) ? combatEffect.Expiration : null) != null)
				{
					value = this.m_effectSyncData.Value;
					if (value.CombatEffect.Expiration.HasTriggerCount)
					{
						value = this.m_effectSyncData.Value;
						CombatEffect combatEffect2 = value.CombatEffect;
						maxTriggerCount = ((combatEffect2 != null) ? new int?(combatEffect2.Expiration.TriggerCount) : null);
						goto IL_187;
					}
				}
				maxTriggerCount = null;
				IL_187:
				this.m_maxTriggerCount = maxTriggerCount;
				value = this.m_effectSyncData.Value;
				int lastTriggerCount;
				if (value.TriggerCount == null)
				{
					lastTriggerCount = 0;
				}
				else
				{
					value = this.m_effectSyncData.Value;
					lastTriggerCount = (int)value.TriggerCount.Value;
				}
				this.m_lastTriggerCount = lastTriggerCount;
				value = this.m_effectSyncData.Value;
				int lastStackCount;
				if (value.StackCount == null)
				{
					lastStackCount = 0;
				}
				else
				{
					value = this.m_effectSyncData.Value;
					lastStackCount = (int)value.StackCount.Value;
				}
				this.m_lastStackCount = lastStackCount;
				this.RefreshCountLabel();
				this.RefreshStackLabel();
				this.RefreshTextLabel();
				this.RefreshFrameColor();
				this.RefreshAlchemyIndicator();
				if (!base.Visible || !flag)
				{
					this.Show(false);
					return;
				}
			}
			else
			{
				this.Hide(true);
				this.m_icon.overrideSprite = null;
				this.RefreshCountLabel();
				this.RefreshStackLabel();
				this.RefreshTextLabel();
				this.RefreshFrameColor();
				this.RefreshAlchemyIndicator();
			}
		}

		// Token: 0x06003F13 RID: 16147 RVA: 0x00187528 File Offset: 0x00185728
		private void Update()
		{
			if (this.m_effectSyncData != null)
			{
				EffectSyncData value;
				if (this.m_effectSyncData.Value.Duration >= 2147483647)
				{
					this.m_cooldownOverlay.fillAmount = 0f;
					this.m_text.ZStringSetText("<font=\"Font Awesome 5 Free-Solid-900 SDF\"></font>");
				}
				else
				{
					value = this.m_effectSyncData.Value;
					float timeRemaining = value.GetTimeRemaining();
					float fillAmount = timeRemaining / (float)this.m_effectSyncData.Value.Duration;
					this.m_cooldownOverlay.fillAmount = fillAmount;
					float value2 = Mathf.Clamp(timeRemaining, 0f, float.MaxValue);
					this.m_text.SetFormattedTime(value2, false);
				}
				value = this.m_effectSyncData.Value;
				if (value.TriggerCount != null)
				{
					value = this.m_effectSyncData.Value;
					byte value3 = value.TriggerCount.Value;
					if ((int)value3 != this.m_lastTriggerCount)
					{
						this.RefreshCountLabel();
					}
					this.m_lastTriggerCount = (int)value3;
				}
				value = this.m_effectSyncData.Value;
				if (value.StackCount != null)
				{
					value = this.m_effectSyncData.Value;
					byte value4 = value.StackCount.Value;
					if ((int)value4 != this.m_lastStackCount)
					{
						this.RefreshStackLabel();
					}
					this.m_lastStackCount = (int)value4;
					return;
				}
			}
			else
			{
				this.m_cooldownOverlay.fillAmount = 0f;
				this.m_text.text = null;
			}
		}

		// Token: 0x06003F14 RID: 16148 RVA: 0x00187680 File Offset: 0x00185880
		private ITooltipParameter GetTooltipParameter()
		{
			if (this.m_effectSyncData != null)
			{
				return new EffectSyncDataTooltipParameter
				{
					SyncData = this.m_effectSyncData.Value,
					Archetype = this.m_archetype
				};
			}
			return null;
		}

		// Token: 0x17000E95 RID: 3733
		// (get) Token: 0x06003F15 RID: 16149 RVA: 0x0006AAC4 File Offset: 0x00068CC4
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x17000E96 RID: 3734
		// (get) Token: 0x06003F16 RID: 16150 RVA: 0x0006AAD2 File Offset: 0x00068CD2
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x17000E97 RID: 3735
		// (get) Token: 0x06003F17 RID: 16151 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x06003F18 RID: 16152 RVA: 0x0006AADA File Offset: 0x00068CDA
		private bool IsLocal()
		{
			return this.m_networkEntity && LocalPlayer.NetworkEntity && this.m_networkEntity == LocalPlayer.NetworkEntity;
		}

		// Token: 0x06003F19 RID: 16153 RVA: 0x0006AB07 File Offset: 0x00068D07
		private bool CanDismiss()
		{
			return this.IsLocal() && this.m_effectSyncData != null && this.m_effectSyncData.Value.Dismissible;
		}

		// Token: 0x06003F1A RID: 16154 RVA: 0x0006AB30 File Offset: 0x00068D30
		public string FillActionsGetTitle()
		{
			if (this.CanDismiss())
			{
				ContextMenuUI.AddContextAction("Dismiss", true, new Action(this.DismissEffect), null, null);
				return this.m_archetype.DisplayName;
			}
			return null;
		}

		// Token: 0x06003F1B RID: 16155 RVA: 0x0006AB60 File Offset: 0x00068D60
		private void DismissEffect()
		{
			if (this.CanDismiss())
			{
				LocalPlayer.GameEntity.NetworkEntity.PlayerRpcHandler.Client_DismissEffectRequest(this.m_effectSyncData.Value.InstanceId);
			}
		}

		// Token: 0x06003F1C RID: 16156 RVA: 0x001876CC File Offset: 0x001858CC
		private void RefreshTextLabel()
		{
			if (!this.m_text)
			{
				return;
			}
			if (this.m_defaultTextColor == null)
			{
				this.m_defaultTextColor = new Color?(this.m_text.color);
			}
			if (this.m_effectSyncData != null && this.m_effectSyncData.Value.CombatEffect != null)
			{
				this.m_text.color = ((this.m_effectSyncData.Value.CombatEffect.Polarity == Polarity.Negative) ? UIManager.RedColor : UIManager.BlueColor);
				return;
			}
			this.m_text.color = this.m_defaultTextColor.Value;
		}

		// Token: 0x06003F1D RID: 16157 RVA: 0x00187774 File Offset: 0x00185974
		private void RefreshCountLabel()
		{
			if (!this.m_countLabel || !this.m_countPanel)
			{
				return;
			}
			if (this.m_maxTriggerCount != null && this.m_effectSyncData != null)
			{
				EffectSyncData value = this.m_effectSyncData.Value;
				int num;
				if (value.TriggerCount == null)
				{
					num = 0;
				}
				else
				{
					value = this.m_effectSyncData.Value;
					num = (int)value.TriggerCount.Value;
				}
				int num2 = num;
				int arg = this.m_maxTriggerCount.Value - num2;
				this.m_countLabel.SetTextFormat("{0}x", arg);
				if (!this.m_countPanel.gameObject.activeSelf)
				{
					this.m_countPanel.gameObject.SetActive(true);
					return;
				}
			}
			else
			{
				this.m_countPanel.gameObject.SetActive(false);
			}
		}

		// Token: 0x06003F1E RID: 16158 RVA: 0x00187844 File Offset: 0x00185A44
		private void RefreshStackLabel()
		{
			if (!this.m_stackLabel)
			{
				return;
			}
			if (this.m_effectSyncData != null)
			{
				EffectSyncData value = this.m_effectSyncData.Value;
				if (value.StackCount != null)
				{
					value = this.m_effectSyncData.Value;
					if (value.StackCount.Value > 1)
					{
						TMP_Text stackLabel = this.m_stackLabel;
						value = this.m_effectSyncData.Value;
						stackLabel.ZStringSetText(value.StackCount.Value);
						if (!this.m_stackLabel.gameObject.activeSelf)
						{
							this.m_stackLabel.gameObject.SetActive(true);
							return;
						}
						return;
					}
				}
			}
			this.m_stackLabel.gameObject.SetActive(false);
		}

		// Token: 0x06003F1F RID: 16159 RVA: 0x0006AB8E File Offset: 0x00068D8E
		private void RefreshFrameColor()
		{
			if (!this.m_frame)
			{
				return;
			}
			if (this.m_defaultFrameColor == null)
			{
				this.m_defaultFrameColor = new Color?(this.m_frame.color);
			}
		}

		// Token: 0x06003F20 RID: 16160 RVA: 0x001878FC File Offset: 0x00185AFC
		private void RefreshAlchemyIndicator()
		{
			if (this.m_alchemyGlow)
			{
				bool enabled = false;
				Color color = Color.white;
				if (this.m_effectSyncData != null && this.m_effectSyncData.Value.AlchemyPowerLevel != AlchemyPowerLevel.None)
				{
					enabled = true;
					color = GlobalSettings.Values.Ashen.GetUIHighlightColor(this.m_effectSyncData.Value.AlchemyPowerLevel);
				}
				this.m_alchemyGlow.color = color;
				this.m_alchemyGlow.enabled = enabled;
			}
		}

		// Token: 0x06003F22 RID: 16162 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04003D02 RID: 15618
		private NetworkEntity m_networkEntity;

		// Token: 0x04003D03 RID: 15619
		private BaseArchetype m_archetype;

		// Token: 0x04003D04 RID: 15620
		private EffectSyncData? m_effectSyncData;

		// Token: 0x04003D05 RID: 15621
		private int? m_maxTriggerCount;

		// Token: 0x04003D06 RID: 15622
		private int m_lastTriggerCount;

		// Token: 0x04003D07 RID: 15623
		private int m_lastStackCount;

		// Token: 0x04003D08 RID: 15624
		private Color? m_defaultFrameColor;

		// Token: 0x04003D09 RID: 15625
		private Color? m_defaultTextColor;

		// Token: 0x04003D0A RID: 15626
		[SerializeField]
		private Image m_icon;

		// Token: 0x04003D0B RID: 15627
		[SerializeField]
		private Image m_frame;

		// Token: 0x04003D0C RID: 15628
		[SerializeField]
		private Image m_cooldownOverlay;

		// Token: 0x04003D0D RID: 15629
		[SerializeField]
		private TextMeshProUGUI m_text;

		// Token: 0x04003D0E RID: 15630
		[SerializeField]
		private TextMeshProUGUI m_countLabel;

		// Token: 0x04003D0F RID: 15631
		[SerializeField]
		private TextMeshProUGUI m_stackLabel;

		// Token: 0x04003D10 RID: 15632
		[SerializeField]
		private RectTransform m_countPanel;

		// Token: 0x04003D11 RID: 15633
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x04003D12 RID: 15634
		[SerializeField]
		private Image m_alchemyGlow;
	}
}

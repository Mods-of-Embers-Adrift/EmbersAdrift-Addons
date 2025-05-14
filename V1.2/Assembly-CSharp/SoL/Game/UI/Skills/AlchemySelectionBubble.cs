using System;
using Cysharp.Text;
using SoL.Game.EffectSystem;
using SoL.Game.Interactives;
using SoL.Game.Settings;
using SoL.Managers;
using SoL.UI;
using SoL.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace SoL.Game.UI.Skills
{
	// Token: 0x0200092B RID: 2347
	public class AlchemySelectionBubble : AlchemyUI, ITooltip, IInteractiveBase
	{
		// Token: 0x06004506 RID: 17670 RVA: 0x0006E9AB File Offset: 0x0006CBAB
		private void Start()
		{
			this.m_toggle.onValueChanged.AddListener(new UnityAction<bool>(this.ToggleChanged));
			if (LocalPlayer.IsInitialized)
			{
				this.LocalPlayerOnLocalPlayerInitialized();
				return;
			}
			LocalPlayer.LocalPlayerInitialized += this.LocalPlayerOnLocalPlayerInitialized;
		}

		// Token: 0x06004507 RID: 17671 RVA: 0x0019E504 File Offset: 0x0019C704
		private void OnDestroy()
		{
			this.m_toggle.onValueChanged.RemoveListener(new UnityAction<bool>(this.ToggleChanged));
			if (LocalPlayer.GameEntity && LocalPlayer.GameEntity.CollectionController != null)
			{
				LocalPlayer.GameEntity.CollectionController.EmberStoneChanged -= this.CollectionControllerOnEmberStoneChanged;
			}
		}

		// Token: 0x06004508 RID: 17672 RVA: 0x0019E560 File Offset: 0x0019C760
		private void LocalPlayerOnLocalPlayerInitialized()
		{
			this.m_toggleController.Toggle(false);
			LocalPlayer.LocalPlayerInitialized -= this.LocalPlayerOnLocalPlayerInitialized;
			if (LocalPlayer.GameEntity && LocalPlayer.GameEntity.CollectionController != null)
			{
				LocalPlayer.GameEntity.CollectionController.EmberStoneChanged += this.CollectionControllerOnEmberStoneChanged;
				this.CollectionControllerOnEmberStoneChanged();
			}
		}

		// Token: 0x06004509 RID: 17673 RVA: 0x0006E9E8 File Offset: 0x0006CBE8
		private void CollectionControllerOnEmberStoneChanged()
		{
			this.RefreshLabel();
		}

		// Token: 0x0600450A RID: 17674 RVA: 0x0019E5C4 File Offset: 0x0019C7C4
		private void RefreshLabel()
		{
			int num;
			int num2;
			int arg;
			this.GetCostAndCurrent(out num, out num2, out arg);
			if (this.m_label)
			{
				this.m_label.SetTextFormat("{0}", arg);
			}
		}

		// Token: 0x0600450B RID: 17675 RVA: 0x0019E5FC File Offset: 0x0019C7FC
		private void ToggleChanged(bool arg0)
		{
			AudioClip clip;
			float value;
			float value2;
			if ((arg0 || LocalClientSkillsController.AlchemyExecutionCompleteFrame < Time.frameCount) && ClientGameManager.UIManager && GlobalSettings.Values.Ashen.TryGetAudioClip(this.m_alchemyPowerLevel, arg0, out clip, out value, out value2))
			{
				ClientGameManager.UIManager.PlayClip(clip, new float?(value2), new float?(value));
			}
			this.m_toggleController.State = (arg0 ? ToggleController.ToggleState.ON : ToggleController.ToggleState.OFF);
		}

		// Token: 0x0600450C RID: 17676 RVA: 0x0019E670 File Offset: 0x0019C870
		private void GetCostAndCurrent(out int essenceCost, out int currentEssence, out int count)
		{
			essenceCost = this.m_alchemyPowerLevel.GetRequiredEmberEssence();
			currentEssence = ((LocalPlayer.GameEntity && LocalPlayer.GameEntity.CollectionController != null) ? LocalPlayer.GameEntity.CollectionController.GetEmberEssenceCount() : 0);
			count = Mathf.FloorToInt((float)currentEssence / (float)essenceCost);
		}

		// Token: 0x0600450D RID: 17677 RVA: 0x0019E6C4 File Offset: 0x0019C8C4
		private ITooltipParameter GetTooltipParameter()
		{
			int num;
			int num2;
			int num3;
			this.GetCostAndCurrent(out num, out num2, out num3);
			int keybindActionId = this.m_alchemyPowerLevel.GetKeybindActionId();
			string text = (keybindActionId >= 0) ? SolInput.Mapper.GetPrimaryBindingNameForAction(keybindActionId) : string.Empty;
			string txt = string.Empty;
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder())
			{
				utf16ValueStringBuilder.AppendFormat<string>("{0}", this.m_alchemyPowerLevel.GetAlchemyPowerLevelDescription());
				utf16ValueStringBuilder.AppendLine();
				if (num <= num2)
				{
					utf16ValueStringBuilder.AppendFormat<int>("{0} Ember Essence consumed.", num);
					utf16ValueStringBuilder.AppendLine();
					string arg = (num3 == 1) ? "time" : "times";
					utf16ValueStringBuilder.AppendFormat<int, string>("You can currently perform this action {0} {1}.", num3, arg);
				}
				else
				{
					utf16ValueStringBuilder.AppendFormat<int>("{0} Ember Essence required.", num);
					utf16ValueStringBuilder.AppendLine();
					utf16ValueStringBuilder.AppendFormat<int>("You currently only have {0}.", num2);
				}
				utf16ValueStringBuilder.AppendLine();
				if (string.IsNullOrEmpty(text))
				{
					utf16ValueStringBuilder.Append("<i>Not currently bound.</i>");
				}
				else
				{
					utf16ValueStringBuilder.AppendFormat<string>("<i>{0} to toggle.</i>", text);
				}
				txt = utf16ValueStringBuilder.ToString();
			}
			return new ObjectTextTooltipParameter(this, txt, false);
		}

		// Token: 0x17000F7C RID: 3964
		// (get) Token: 0x0600450E RID: 17678 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000F7D RID: 3965
		// (get) Token: 0x0600450F RID: 17679 RVA: 0x0006E9F0 File Offset: 0x0006CBF0
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x17000F7E RID: 3966
		// (get) Token: 0x06004510 RID: 17680 RVA: 0x0006E9FE File Offset: 0x0006CBFE
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x06004512 RID: 17682 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x0400418E RID: 16782
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x0400418F RID: 16783
		[SerializeField]
		private SolToggle m_toggle;

		// Token: 0x04004190 RID: 16784
		[SerializeField]
		private TextMeshProUGUI m_label;

		// Token: 0x04004191 RID: 16785
		[SerializeField]
		private ToggleController m_toggleController;
	}
}

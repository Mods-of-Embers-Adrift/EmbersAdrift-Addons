using System;
using SoL.Game.Interactives;
using SoL.UI;
using UnityEngine;
using UnityEngine.Events;

namespace SoL.Game.UI
{
	// Token: 0x020008CB RID: 2251
	public class PauseAdventuringExperienceToggle : MonoBehaviour, ITooltip, IInteractiveBase
	{
		// Token: 0x060041CA RID: 16842 RVA: 0x00190888 File Offset: 0x0018EA88
		public void Init()
		{
			if (!this.m_toggle)
			{
				return;
			}
			this.m_toggle.isOn = (LocalPlayer.GameEntity && LocalPlayer.GameEntity.CharacterData && LocalPlayer.GameEntity.CharacterData.PauseAdventuringExperience);
			this.m_toggle.onValueChanged.AddListener(new UnityAction<bool>(this.PauseAdventuringExperienceToggleChanged));
		}

		// Token: 0x060041CB RID: 16843 RVA: 0x0006C712 File Offset: 0x0006A912
		private void OnDestroy()
		{
			if (this.m_toggle)
			{
				this.m_toggle.onValueChanged.RemoveListener(new UnityAction<bool>(this.PauseAdventuringExperienceToggleChanged));
			}
		}

		// Token: 0x060041CC RID: 16844 RVA: 0x0006C73D File Offset: 0x0006A93D
		private void PauseAdventuringExperienceToggleChanged(bool value)
		{
			if (LocalPlayer.GameEntity && LocalPlayer.GameEntity.CharacterData)
			{
				LocalPlayer.GameEntity.CharacterData.PauseAdventuringExperience = value;
			}
		}

		// Token: 0x060041CD RID: 16845 RVA: 0x0006C76C File Offset: 0x0006A96C
		private ITooltipParameter GetTooltipParameter()
		{
			return new ObjectTextTooltipParameter(this, "Pause adventuring experience gain from kills or tasks.", false);
		}

		// Token: 0x17000F00 RID: 3840
		// (get) Token: 0x060041CE RID: 16846 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000F01 RID: 3841
		// (get) Token: 0x060041CF RID: 16847 RVA: 0x0006C77F File Offset: 0x0006A97F
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x17000F02 RID: 3842
		// (get) Token: 0x060041D0 RID: 16848 RVA: 0x0006C78D File Offset: 0x0006A98D
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x060041D2 RID: 16850 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04003F0B RID: 16139
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x04003F0C RID: 16140
		[SerializeField]
		private SolToggle m_toggle;
	}
}

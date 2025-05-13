using System;
using SoL.Game.Interactives;
using SoL.UI;
using UnityEngine;
using UnityEngine.Events;

namespace SoL.Game.UI
{
	// Token: 0x0200089D RID: 2205
	public class InspectToggle : MonoBehaviour, ITooltip, IInteractiveBase
	{
		// Token: 0x0600403A RID: 16442 RVA: 0x0018BC64 File Offset: 0x00189E64
		public void Init()
		{
			if (!this.m_toggle)
			{
				return;
			}
			this.m_toggle.isOn = (LocalPlayer.GameEntity && LocalPlayer.GameEntity.CharacterData && !LocalPlayer.GameEntity.CharacterData.BlockInspections);
			this.m_toggle.onValueChanged.AddListener(new UnityAction<bool>(this.BlockInspectionsChanged));
		}

		// Token: 0x0600403B RID: 16443 RVA: 0x0006B7D9 File Offset: 0x000699D9
		private void OnDestroy()
		{
			if (this.m_toggle)
			{
				this.m_toggle.onValueChanged.RemoveListener(new UnityAction<bool>(this.BlockInspectionsChanged));
			}
		}

		// Token: 0x0600403C RID: 16444 RVA: 0x0006B804 File Offset: 0x00069A04
		private void BlockInspectionsChanged(bool value)
		{
			if (LocalPlayer.GameEntity && LocalPlayer.GameEntity.CharacterData)
			{
				LocalPlayer.GameEntity.CharacterData.BlockInspections = !value;
			}
		}

		// Token: 0x0600403D RID: 16445 RVA: 0x0006B836 File Offset: 0x00069A36
		private ITooltipParameter GetTooltipParameter()
		{
			return new ObjectTextTooltipParameter(this, "Enable others to inspect your gear.", false);
		}

		// Token: 0x17000EC0 RID: 3776
		// (get) Token: 0x0600403E RID: 16446 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000EC1 RID: 3777
		// (get) Token: 0x0600403F RID: 16447 RVA: 0x0006B849 File Offset: 0x00069A49
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x17000EC2 RID: 3778
		// (get) Token: 0x06004040 RID: 16448 RVA: 0x0006B857 File Offset: 0x00069A57
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x06004042 RID: 16450 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04003E1E RID: 15902
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x04003E1F RID: 15903
		[SerializeField]
		private SolToggle m_toggle;
	}
}

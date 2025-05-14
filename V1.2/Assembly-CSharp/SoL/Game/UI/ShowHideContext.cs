using System;
using SoL.Game.Interactives;
using SoL.UI;
using UnityEngine;

namespace SoL.Game.UI
{
	// Token: 0x020008D1 RID: 2257
	public class ShowHideContext : MonoBehaviour, ITooltip, IInteractiveBase, IContextMenu
	{
		// Token: 0x06004206 RID: 16902 RVA: 0x00191780 File Offset: 0x0018F980
		private void ToggleItem()
		{
			ShowHideContext.ToggleType type = this.m_type;
			if (type == ShowHideContext.ToggleType.Helm)
			{
				LocalPlayer.GameEntity.CharacterData.HideHelm = !LocalPlayer.GameEntity.CharacterData.HideHelm;
				return;
			}
			if (type != ShowHideContext.ToggleType.EmberStone)
			{
				return;
			}
			LocalPlayer.GameEntity.CharacterData.HideEmberStone = !LocalPlayer.GameEntity.CharacterData.HideEmberStone;
		}

		// Token: 0x06004207 RID: 16903 RVA: 0x001917E0 File Offset: 0x0018F9E0
		private string GetTitleForType()
		{
			ShowHideContext.ToggleType type = this.m_type;
			if (type == ShowHideContext.ToggleType.Helm)
			{
				return "Show/Hide Helm";
			}
			if (type != ShowHideContext.ToggleType.EmberStone)
			{
				return string.Empty;
			}
			return "Show/Hide Ember Stone";
		}

		// Token: 0x06004208 RID: 16904 RVA: 0x00191810 File Offset: 0x0018FA10
		private string GetOptionTextForType()
		{
			ShowHideContext.ToggleType type = this.m_type;
			if (type != ShowHideContext.ToggleType.Helm)
			{
				if (type != ShowHideContext.ToggleType.EmberStone)
				{
					return string.Empty;
				}
				if (!LocalPlayer.GameEntity.CharacterData.HideEmberStone)
				{
					return "Hide Ember Stone";
				}
				return "Show Ember Stone";
			}
			else
			{
				if (!LocalPlayer.GameEntity.CharacterData.HideHelm)
				{
					return "Hide Helm";
				}
				return "Show Helm";
			}
		}

		// Token: 0x06004209 RID: 16905 RVA: 0x0006C93F File Offset: 0x0006AB3F
		private ITooltipParameter GetTooltipParameter()
		{
			return new ObjectTextTooltipParameter(this, this.GetTitleForType(), false);
		}

		// Token: 0x17000F0A RID: 3850
		// (get) Token: 0x0600420A RID: 16906 RVA: 0x0006C953 File Offset: 0x0006AB53
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return this.m_interactionSettings;
			}
		}

		// Token: 0x17000F0B RID: 3851
		// (get) Token: 0x0600420B RID: 16907 RVA: 0x0006C95B File Offset: 0x0006AB5B
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x17000F0C RID: 3852
		// (get) Token: 0x0600420C RID: 16908 RVA: 0x0006C963 File Offset: 0x0006AB63
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x0600420D RID: 16909 RVA: 0x0019186C File Offset: 0x0018FA6C
		string IContextMenu.FillActionsGetTitle()
		{
			if (LocalPlayer.GameEntity == null || LocalPlayer.GameEntity.CharacterData == null)
			{
				return null;
			}
			ContextMenuUI.AddContextAction(this.GetOptionTextForType(), true, new Action(this.ToggleItem), null, null);
			return this.GetTitleForType();
		}

		// Token: 0x0600420F RID: 16911 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04003F28 RID: 16168
		private const string kHelmTitle = "Show/Hide Helm";

		// Token: 0x04003F29 RID: 16169
		private const string kStoneTitle = "Show/Hide Ember Stone";

		// Token: 0x04003F2A RID: 16170
		[SerializeField]
		private ShowHideContext.ToggleType m_type;

		// Token: 0x04003F2B RID: 16171
		[SerializeField]
		private InteractionSettings m_interactionSettings;

		// Token: 0x04003F2C RID: 16172
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x020008D2 RID: 2258
		private enum ToggleType
		{
			// Token: 0x04003F2E RID: 16174
			Helm,
			// Token: 0x04003F2F RID: 16175
			EmberStone
		}
	}
}

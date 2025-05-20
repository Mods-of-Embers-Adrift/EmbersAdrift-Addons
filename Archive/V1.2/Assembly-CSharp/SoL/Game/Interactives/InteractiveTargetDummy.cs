using System;
using SoL.Game.Targeting;
using SoL.Managers;
using SoL.UI;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Interactives
{
	// Token: 0x02000BA5 RID: 2981
	public class InteractiveTargetDummy : GameEntityComponent, IInteractive, IInteractiveBase, ITooltip, ICursor
	{
		// Token: 0x06005C54 RID: 23636 RVA: 0x0007DF84 File Offset: 0x0007C184
		private bool CanInteract(GameEntity entity)
		{
			return entity && this.m_interactionSettings.IsWithinRange(base.GameEntity, entity);
		}

		// Token: 0x170015C7 RID: 5575
		// (get) Token: 0x06005C55 RID: 23637 RVA: 0x00045BCA File Offset: 0x00043DCA
		bool IInteractive.RequiresLos
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170015C8 RID: 5576
		// (get) Token: 0x06005C56 RID: 23638 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.gameObject
		{
			get
			{
				return base.gameObject;
			}
		}

		// Token: 0x170015C9 RID: 5577
		// (get) Token: 0x06005C57 RID: 23639 RVA: 0x0007DFA2 File Offset: 0x0007C1A2
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return this.m_interactionSettings;
			}
		}

		// Token: 0x06005C58 RID: 23640 RVA: 0x001F11E0 File Offset: 0x001EF3E0
		bool IInteractive.ClientInteraction()
		{
			bool result = false;
			if (!GameManager.IsServer && base.GameEntity && base.GameEntity.Targetable != null && LocalPlayer.GameEntity && LocalPlayer.GameEntity.TargetController)
			{
				LocalPlayer.GameEntity.TargetController.SetTarget(TargetType.Offensive, base.GameEntity.Targetable);
				if (UIManager.AutoAttackButton)
				{
					UIManager.AutoAttackButton.InitiateCombat();
					result = true;
				}
			}
			return result;
		}

		// Token: 0x06005C59 RID: 23641 RVA: 0x0007DFAA File Offset: 0x0007C1AA
		bool IInteractive.CanInteract(GameEntity entity)
		{
			return this.CanInteract(entity);
		}

		// Token: 0x06005C5A RID: 23642 RVA: 0x0004475B File Offset: 0x0004295B
		void IInteractive.BeginInteraction(GameEntity interactionSource)
		{
		}

		// Token: 0x06005C5B RID: 23643 RVA: 0x0004475B File Offset: 0x0004295B
		void IInteractive.EndInteraction(GameEntity interactionSource, bool clientIsEnding)
		{
		}

		// Token: 0x06005C5C RID: 23644 RVA: 0x0004475B File Offset: 0x0004295B
		void IInteractive.EndAllInteractions()
		{
		}

		// Token: 0x06005C5D RID: 23645 RVA: 0x0007DFB3 File Offset: 0x0007C1B3
		private ITooltipParameter GetTooltipParameter()
		{
			if (string.IsNullOrEmpty(this.m_tooltipDescription))
			{
				return null;
			}
			return new ObjectTextTooltipParameter(this, this.m_tooltipDescription, false);
		}

		// Token: 0x170015CA RID: 5578
		// (get) Token: 0x06005C5E RID: 23646 RVA: 0x0007DFD6 File Offset: 0x0007C1D6
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x170015CB RID: 5579
		// (get) Token: 0x06005C5F RID: 23647 RVA: 0x0007DFE4 File Offset: 0x0007C1E4
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x170015CC RID: 5580
		// (get) Token: 0x06005C60 RID: 23648 RVA: 0x0007DFEC File Offset: 0x0007C1EC
		CursorType ICursor.Type
		{
			get
			{
				if (!this.CanInteract(LocalPlayer.GameEntity))
				{
					return CursorType.MainCursor;
				}
				return CursorType.SwordCursor1;
			}
		}

		// Token: 0x04005014 RID: 20500
		[SerializeField]
		private string m_tooltipDescription;

		// Token: 0x04005015 RID: 20501
		[SerializeField]
		private InteractionSettings m_interactionSettings;

		// Token: 0x04005016 RID: 20502
		[SerializeField]
		private TooltipSettings m_tooltipSettings;
	}
}

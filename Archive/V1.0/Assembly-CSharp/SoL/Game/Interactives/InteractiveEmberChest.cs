using System;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Settings;
using SoL.Managers;
using SoL.UI;
using UnityEngine;

namespace SoL.Game.Interactives
{
	// Token: 0x02000B86 RID: 2950
	public class InteractiveEmberChest : BaseInteractive, ITooltip, IInteractiveBase
	{
		// Token: 0x06005AF4 RID: 23284 RVA: 0x001EDBF8 File Offset: 0x001EBDF8
		public override void BeginInteraction(GameEntity interactionSource)
		{
			base.BeginInteraction(interactionSource);
			if (GameManager.IsServer && this.CanInteractInternal(interactionSource))
			{
				EmberStone currentEmberStone = interactionSource.CollectionController.CurrentEmberStone;
				EmberStone nextEmberStone = GlobalSettings.Values.Progression.GetNextEmberStone(currentEmberStone);
				if (nextEmberStone != null)
				{
					interactionSource.CollectionController.CurrentEmberStone = nextEmberStone;
				}
				int delta = interactionSource.CollectionController.CurrentEmberStone ? interactionSource.CollectionController.CurrentEmberStone.MaxCapacity : 0;
				interactionSource.CollectionController.AdjustEmberEssenceCount(delta);
			}
		}

		// Token: 0x06005AF5 RID: 23285 RVA: 0x0007D14C File Offset: 0x0007B34C
		public override bool CanInteract(GameEntity entity)
		{
			return base.CanInteract(entity) && this.CanInteractInternal(entity);
		}

		// Token: 0x06005AF6 RID: 23286 RVA: 0x0007D160 File Offset: 0x0007B360
		private bool CanInteractInternal(GameEntity entity)
		{
			return entity != null && entity.Type == GameEntityType.Player && entity.CollectionController != null;
		}

		// Token: 0x06005AF7 RID: 23287 RVA: 0x0007D17F File Offset: 0x0007B37F
		private ITooltipParameter GetTooltipParameter()
		{
			return new ObjectTextTooltipParameter(this, "Ember Stone Chest", false);
		}

		// Token: 0x17001549 RID: 5449
		// (get) Token: 0x06005AF8 RID: 23288 RVA: 0x0007D192 File Offset: 0x0007B392
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x1700154A RID: 5450
		// (get) Token: 0x06005AF9 RID: 23289 RVA: 0x0007D1A0 File Offset: 0x0007B3A0
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x06005AFB RID: 23291 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04004FA0 RID: 20384
		[SerializeField]
		private TooltipSettings m_tooltipSettings;
	}
}

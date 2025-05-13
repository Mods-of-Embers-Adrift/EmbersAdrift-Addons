using System;
using SoL.Managers;
using SoL.UI;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Interactives
{
	// Token: 0x02000BA7 RID: 2983
	public class InteractiveTeleporterLimitedPopulation : GameEntityComponent, IInteractive, IInteractiveBase, ITooltip, ICursor
	{
		// Token: 0x06005C72 RID: 23666 RVA: 0x001F13B0 File Offset: 0x001EF5B0
		private bool CanInteract(GameEntity entity)
		{
			return entity && this.m_occupantList && this.m_interactionDistance.IsWithinRange(base.GameEntity, entity) && (!this.m_occupantList.IsFull || this.m_occupantList.IsInList(entity));
		}

		// Token: 0x06005C73 RID: 23667 RVA: 0x001F1404 File Offset: 0x001EF604
		private void BeginInteraction(GameEntity interactionSource)
		{
			if (GameManager.IsServer && interactionSource && interactionSource.CharacterData && interactionSource.NetworkEntity && interactionSource.NetworkEntity.PlayerRpcHandler)
			{
				SharedOccupantList.OccupantData occupantData = new SharedOccupantList.OccupantData
				{
					Entity = interactionSource,
					Name = interactionSource.CharacterData.Name.Value
				};
				TargetPosition targetPosition;
				if (this.m_occupantList.IsInList(ref occupantData))
				{
					this.m_occupantList.RemoveFromList(ref occupantData);
					targetPosition = this.m_targetOut;
				}
				else
				{
					this.m_occupantList.AddToList(ref occupantData);
					targetPosition = this.m_targetIn;
				}
				Vector3 position = targetPosition.GetPosition();
				Quaternion rotation = targetPosition.GetRotation();
				interactionSource.NetworkEntity.PlayerRpcHandler.ResetPosition(position, rotation.eulerAngles.y);
			}
		}

		// Token: 0x170015D1 RID: 5585
		// (get) Token: 0x06005C74 RID: 23668 RVA: 0x00070E66 File Offset: 0x0006F066
		private CursorType ActiveCursorType
		{
			get
			{
				return CursorType.GloveCursor;
			}
		}

		// Token: 0x170015D2 RID: 5586
		// (get) Token: 0x06005C75 RID: 23669 RVA: 0x0004479C File Offset: 0x0004299C
		private CursorType InactiveCursorType
		{
			get
			{
				return CursorType.MainCursor;
			}
		}

		// Token: 0x170015D3 RID: 5587
		// (get) Token: 0x06005C76 RID: 23670 RVA: 0x0007E050 File Offset: 0x0007C250
		CursorType ICursor.Type
		{
			get
			{
				if (!this.CanInteract(LocalPlayer.GameEntity))
				{
					return this.InactiveCursorType;
				}
				return this.ActiveCursorType;
			}
		}

		// Token: 0x170015D4 RID: 5588
		// (get) Token: 0x06005C77 RID: 23671 RVA: 0x00045BCA File Offset: 0x00043DCA
		bool IInteractive.RequiresLos
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170015D5 RID: 5589
		// (get) Token: 0x06005C78 RID: 23672 RVA: 0x0007E06C File Offset: 0x0007C26C
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return this.m_interactionDistance;
			}
		}

		// Token: 0x06005C79 RID: 23673 RVA: 0x0007E074 File Offset: 0x0007C274
		bool IInteractive.ClientInteraction()
		{
			if (this.CanInteract(LocalPlayer.GameEntity))
			{
				LocalPlayer.NetworkEntity.PlayerRpcHandler.Client_RequestInteraction(base.GameEntity.NetworkEntity);
				return true;
			}
			return false;
		}

		// Token: 0x06005C7A RID: 23674 RVA: 0x0007E0A0 File Offset: 0x0007C2A0
		bool IInteractive.CanInteract(GameEntity entity)
		{
			return this.CanInteract(entity);
		}

		// Token: 0x06005C7B RID: 23675 RVA: 0x0007E0A9 File Offset: 0x0007C2A9
		void IInteractive.BeginInteraction(GameEntity interactionSource)
		{
			this.BeginInteraction(interactionSource);
		}

		// Token: 0x06005C7C RID: 23676 RVA: 0x0004475B File Offset: 0x0004295B
		void IInteractive.EndInteraction(GameEntity interactionSource, bool clientIsEnding)
		{
		}

		// Token: 0x06005C7D RID: 23677 RVA: 0x0004475B File Offset: 0x0004295B
		void IInteractive.EndAllInteractions()
		{
		}

		// Token: 0x06005C7E RID: 23678 RVA: 0x001F14EC File Offset: 0x001EF6EC
		private ITooltipParameter GetTooltipParameter()
		{
			if (!this.m_occupantList)
			{
				return null;
			}
			string occupantNames = this.m_occupantList.GetOccupantNames();
			if (string.IsNullOrEmpty(occupantNames))
			{
				return null;
			}
			return new ObjectTextTooltipParameter(this, "Occupants: " + occupantNames, false);
		}

		// Token: 0x170015D6 RID: 5590
		// (get) Token: 0x06005C7F RID: 23679 RVA: 0x0007E0B2 File Offset: 0x0007C2B2
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x170015D7 RID: 5591
		// (get) Token: 0x06005C80 RID: 23680 RVA: 0x0007E0C0 File Offset: 0x0007C2C0
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x06005C82 RID: 23682 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x0400501D RID: 20509
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x0400501E RID: 20510
		[SerializeField]
		private InteractionSettings m_interactionDistance;

		// Token: 0x0400501F RID: 20511
		[SerializeField]
		private SharedOccupantList m_occupantList;

		// Token: 0x04005020 RID: 20512
		[SerializeField]
		private TargetPosition m_targetIn;

		// Token: 0x04005021 RID: 20513
		[SerializeField]
		private TargetPosition m_targetOut;
	}
}

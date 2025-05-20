using System;
using SoL.Managers;
using SoL.Networking.Database;
using SoL.UI;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Interactives
{
	// Token: 0x02000BAA RID: 2986
	public class InteractiveZonePointGM : MonoBehaviour, ITooltip, IInteractiveBase, ICursor, IContextMenu
	{
		// Token: 0x06005CB2 RID: 23730 RVA: 0x0007E39C File Offset: 0x0007C59C
		private void Awake()
		{
			this.m_hasPermissions = (GameManager.IsServer || (SessionData.User != null && (AccessFlags.GM & SessionData.User.Flags) > AccessFlags.None));
		}

		// Token: 0x06005CB3 RID: 23731 RVA: 0x0007E3C7 File Offset: 0x0007C5C7
		private bool CanInteract(GameEntity entity)
		{
			return entity != null && this.m_hasPermissions && this.m_interactionDistance.IsWithinRange(base.gameObject, entity);
		}

		// Token: 0x06005CB4 RID: 23732 RVA: 0x001F1C48 File Offset: 0x001EFE48
		string IContextMenu.FillActionsGetTitle()
		{
			if (GameManager.IsServer || !this.CanInteract(LocalPlayer.GameEntity))
			{
				return null;
			}
			ContextMenuUI.ClearContextActions();
			ZoneId[] zoneIds = ZoneIdExtensions.ZoneIds;
			for (int i = 0; i < zoneIds.Length; i++)
			{
				ZoneId zoneId = zoneIds[i];
				if (zoneId.IsAvailable())
				{
					ContextMenuUI.AddContextAction(zoneId.ToString(), zoneId != (ZoneId)LocalZoneManager.ZoneRecord.ZoneId, delegate()
					{
						ZoneId zoneId = zoneId;
						this.ZoneRequest(zoneId);
					}, null, null);
				}
			}
			return "Zone Request";
		}

		// Token: 0x06005CB5 RID: 23733 RVA: 0x001F1CE8 File Offset: 0x001EFEE8
		private void ZoneRequest(ZoneId zoneId)
		{
			if (!GameManager.IsServer && this.CanInteract(LocalPlayer.GameEntity))
			{
				LoginApiManager.PerformZoneCheck(zoneId, delegate(bool response)
				{
					ZoneId zoneId2 = zoneId;
					this.ZoneCheckResponse(response, zoneId2);
				});
			}
		}

		// Token: 0x06005CB6 RID: 23734 RVA: 0x0004475B File Offset: 0x0004295B
		private void ZoneCheckResponse(bool result, ZoneId zoneId)
		{
		}

		// Token: 0x06005CB7 RID: 23735 RVA: 0x0007E3EE File Offset: 0x0007C5EE
		private ITooltipParameter GetTooltipParameter()
		{
			if (!this.m_hasPermissions)
			{
				return null;
			}
			return new ObjectTextTooltipParameter(this, "Teleport to another zone", false);
		}

		// Token: 0x170015E3 RID: 5603
		// (get) Token: 0x06005CB8 RID: 23736 RVA: 0x0007E40B File Offset: 0x0007C60B
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x170015E4 RID: 5604
		// (get) Token: 0x06005CB9 RID: 23737 RVA: 0x0007E419 File Offset: 0x0007C619
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x170015E5 RID: 5605
		// (get) Token: 0x06005CBA RID: 23738 RVA: 0x0007E421 File Offset: 0x0007C621
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return this.m_interactionDistance;
			}
		}

		// Token: 0x170015E6 RID: 5606
		// (get) Token: 0x06005CBB RID: 23739 RVA: 0x00070E66 File Offset: 0x0006F066
		private CursorType ActiveCursorType
		{
			get
			{
				return CursorType.GloveCursor;
			}
		}

		// Token: 0x170015E7 RID: 5607
		// (get) Token: 0x06005CBC RID: 23740 RVA: 0x0004479C File Offset: 0x0004299C
		private CursorType InactiveCursorType
		{
			get
			{
				return CursorType.MainCursor;
			}
		}

		// Token: 0x170015E8 RID: 5608
		// (get) Token: 0x06005CBD RID: 23741 RVA: 0x0007E429 File Offset: 0x0007C629
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

		// Token: 0x06005CBF RID: 23743 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04005034 RID: 20532
		public const int kTargetZonePointIndex = 0;

		// Token: 0x04005035 RID: 20533
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x04005036 RID: 20534
		[SerializeField]
		private InteractionSettings m_interactionDistance;

		// Token: 0x04005037 RID: 20535
		private bool m_hasPermissions;
	}
}

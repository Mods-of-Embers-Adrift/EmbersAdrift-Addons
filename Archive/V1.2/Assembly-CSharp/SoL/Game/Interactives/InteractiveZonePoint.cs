using System;
using Cysharp.Text;
using SoL.Game.Messages;
using SoL.Game.Quests;
using SoL.Managers;
using SoL.Networking.Database;
using SoL.UI;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Interactives
{
	// Token: 0x02000BA9 RID: 2985
	public class InteractiveZonePoint : ZonePoint, IInteractive, IInteractiveBase, ICursor, ITooltip
	{
		// Token: 0x06005C97 RID: 23703 RVA: 0x0007E1C2 File Offset: 0x0007C3C2
		private ZoneRecord GetTargetZoneRecord()
		{
			if (this.m_zoneRecord == null || this.m_zoneRecord.ZoneId != (int)this.m_targetZone)
			{
				this.m_zoneRecord = SessionData.GetZoneRecord(this.m_targetZone);
			}
			return this.m_zoneRecord;
		}

		// Token: 0x06005C98 RID: 23704 RVA: 0x001F1A8C File Offset: 0x001EFC8C
		private string GetTargetZoneName()
		{
			ZoneRecord targetZoneRecord = this.GetTargetZoneRecord();
			return LocalZoneManager.GetFormattedZoneName((targetZoneRecord != null) ? targetZoneRecord.DisplayName : this.m_targetZone.ToString(), this.m_subZoneId);
		}

		// Token: 0x06005C99 RID: 23705 RVA: 0x001F1AC8 File Offset: 0x001EFCC8
		private void Awake()
		{
			this.m_accessFlagRequirement &= ~AccessFlags.Active;
			if (!GameManager.IsServer && this.m_accessFlagRequirement != AccessFlags.None && SessionData.User != null && (this.m_accessFlagRequirement & SessionData.User.Flags) == AccessFlags.None)
			{
				base.gameObject.SetActive(false);
				return;
			}
		}

		// Token: 0x06005C9A RID: 23706 RVA: 0x001F1B1C File Offset: 0x001EFD1C
		private bool DoubleClicked()
		{
			if (GameManager.IsServer || !this.CanInteract(LocalPlayer.GameEntity, true))
			{
				return false;
			}
			if (this.m_confirmationDialog)
			{
				bool flag = LocalZoneManager.ZoneRecord != null && LocalZoneManager.ZoneRecord.ZoneId == (int)this.m_targetZone;
				string title = flag ? "Teleport" : "Zone";
				string text = flag ? ZString.Format<string>("Are you sure you want to teleport to {0}?", this.GetTargetZoneName()) : ZString.Format<string>("Are you sure you want to zone to {0}?", this.GetTargetZoneName());
				DialogOptions opts = new DialogOptions
				{
					Title = title,
					Text = text,
					ConfirmationText = "Yes",
					CancelText = "No",
					Callback = new Action<bool, object>(this.TeleportConfirmation),
					AutoCancel = new Func<bool>(this.AutoCancel)
				};
				ClientGameManager.UIManager.ConfirmationDialog.Init(opts);
			}
			else
			{
				this.PerformZoneCheckAndZone();
			}
			return true;
		}

		// Token: 0x06005C9B RID: 23707 RVA: 0x0007E1F6 File Offset: 0x0007C3F6
		private void TeleportConfirmation(bool answer, object arg2)
		{
			if (answer && !GameManager.IsServer && this.CanInteract(LocalPlayer.GameEntity, true))
			{
				this.PerformZoneCheckAndZone();
			}
		}

		// Token: 0x06005C9C RID: 23708 RVA: 0x0007E216 File Offset: 0x0007C416
		private bool PassesCombatCheck(bool notify)
		{
			if (this.m_disableInCombat && LocalPlayer.GameEntity && LocalPlayer.GameEntity.InCombat)
			{
				if (notify)
				{
					MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "You cannot use this while in combat!");
				}
				return false;
			}
			return true;
		}

		// Token: 0x06005C9D RID: 23709 RVA: 0x0007E24F File Offset: 0x0007C44F
		private bool PassesProgressionRequirementCheck(GameEntity entity)
		{
			return !this.m_requireProgression || this.m_progressionRequirement == null || this.m_progressionRequirement.MeetsAllRequirements(entity);
		}

		// Token: 0x06005C9E RID: 23710 RVA: 0x0007E26F File Offset: 0x0007C46F
		private void PerformZoneCheckAndZone()
		{
			LoginApiManager.PerformZoneCheck(this.m_targetZone, new Action<bool>(this.ZoneCheckResponse));
		}

		// Token: 0x06005C9F RID: 23711 RVA: 0x0007E288 File Offset: 0x0007C488
		private void ZoneCheckResponse(bool isActive)
		{
			if (isActive && this.CanInteract(LocalPlayer.GameEntity, false))
			{
				LocalPlayer.NetworkEntity.PlayerRpcHandler.RequestZone((int)this.m_targetZone, this.m_targetZonePointIndex, ZonePoint.RegisterZoneRequest(this));
			}
		}

		// Token: 0x06005CA0 RID: 23712 RVA: 0x0007E2BC File Offset: 0x0007C4BC
		private bool CanInteract(GameEntity entity, bool notify)
		{
			return entity != null && this.m_interactionDistance.IsWithinRange(base.gameObject, entity) && base.LocalPlayerIsAlive(notify) && this.PassesCombatCheck(notify) && this.PassesProgressionRequirementCheck(entity);
		}

		// Token: 0x06005CA1 RID: 23713 RVA: 0x0007E2F6 File Offset: 0x0007C4F6
		public override bool ServerCanEntityInteract(GameEntity entity)
		{
			return base.ServerCanEntityInteract(entity) && this.PassesProgressionRequirementCheck(entity);
		}

		// Token: 0x06005CA2 RID: 23714 RVA: 0x0007E30A File Offset: 0x0007C50A
		private bool AutoCancel()
		{
			return !LocalPlayer.GameEntity || (this.m_disableInCombat && LocalPlayer.GameEntity.InCombat) || !this.m_interactionDistance.IsWithinRange(base.gameObject, LocalPlayer.GameEntity);
		}

		// Token: 0x170015DC RID: 5596
		// (get) Token: 0x06005CA3 RID: 23715 RVA: 0x00045BCA File Offset: 0x00043DCA
		bool IInteractive.RequiresLos
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06005CA4 RID: 23716 RVA: 0x0007E347 File Offset: 0x0007C547
		bool IInteractive.ClientInteraction()
		{
			return this.DoubleClicked();
		}

		// Token: 0x06005CA5 RID: 23717 RVA: 0x0007E34F File Offset: 0x0007C54F
		bool IInteractive.CanInteract(GameEntity entity)
		{
			return this.CanInteract(entity, false);
		}

		// Token: 0x06005CA6 RID: 23718 RVA: 0x00048A92 File Offset: 0x00046C92
		void IInteractive.BeginInteraction(GameEntity interactionSource)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06005CA7 RID: 23719 RVA: 0x00048A92 File Offset: 0x00046C92
		void IInteractive.EndInteraction(GameEntity interactionSource, bool clientIsEnding)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06005CA8 RID: 23720 RVA: 0x00048A92 File Offset: 0x00046C92
		void IInteractive.EndAllInteractions()
		{
			throw new NotImplementedException();
		}

		// Token: 0x170015DD RID: 5597
		// (get) Token: 0x06005CA9 RID: 23721 RVA: 0x0007E359 File Offset: 0x0007C559
		CursorType ICursor.Type
		{
			get
			{
				if (!this.CanInteract(LocalPlayer.GameEntity, false))
				{
					return this.InactiveCursorType;
				}
				return this.ActiveCursorType;
			}
		}

		// Token: 0x170015DE RID: 5598
		// (get) Token: 0x06005CAA RID: 23722 RVA: 0x00070E66 File Offset: 0x0006F066
		private CursorType ActiveCursorType
		{
			get
			{
				return CursorType.GloveCursor;
			}
		}

		// Token: 0x170015DF RID: 5599
		// (get) Token: 0x06005CAB RID: 23723 RVA: 0x0004479C File Offset: 0x0004299C
		private CursorType InactiveCursorType
		{
			get
			{
				return CursorType.MainCursor;
			}
		}

		// Token: 0x06005CAC RID: 23724 RVA: 0x001F1C10 File Offset: 0x001EFE10
		private ITooltipParameter GetTooltipParameter()
		{
			if (this.m_requireProgression && !this.PassesProgressionRequirementCheck(LocalPlayer.GameEntity))
			{
				return null;
			}
			string txt = string.Empty;
			if (this.m_accessFlagRequirement.HasBitFlag(AccessFlags.FullClient) && SessionData.User != null && SessionData.User.IsTrial())
			{
				txt = ZString.Format<string>("Zone to {0}\n{1}", "(Purchase Required)");
			}
			else if (!this.PassesCombatCheck(false))
			{
				txt = ZString.Format<string>("Zone to {0}\nCannot use while in combat!", this.GetTargetZoneName());
			}
			else
			{
				txt = ZString.Format<string>("Zone to {0}", this.GetTargetZoneName());
			}
			return new ObjectTextTooltipParameter(this, txt, false);
		}

		// Token: 0x170015E0 RID: 5600
		// (get) Token: 0x06005CAD RID: 23725 RVA: 0x0007E376 File Offset: 0x0007C576
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x170015E1 RID: 5601
		// (get) Token: 0x06005CAE RID: 23726 RVA: 0x0007E384 File Offset: 0x0007C584
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x170015E2 RID: 5602
		// (get) Token: 0x06005CAF RID: 23727 RVA: 0x0007E38C File Offset: 0x0007C58C
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return this.m_interactionDistance;
			}
		}

		// Token: 0x06005CB1 RID: 23729 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x0400502A RID: 20522
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x0400502B RID: 20523
		[SerializeField]
		private InteractionSettings m_interactionDistance;

		// Token: 0x0400502C RID: 20524
		[SerializeField]
		private bool m_confirmationDialog;

		// Token: 0x0400502D RID: 20525
		[SerializeField]
		private bool m_disableInCombat;

		// Token: 0x0400502E RID: 20526
		[SerializeField]
		private AccessFlags m_accessFlagRequirement;

		// Token: 0x0400502F RID: 20527
		[SerializeField]
		private string m_zoneNameSuffix;

		// Token: 0x04005030 RID: 20528
		[SerializeField]
		private SubZoneId m_subZoneId;

		// Token: 0x04005031 RID: 20529
		[SerializeField]
		private bool m_requireProgression;

		// Token: 0x04005032 RID: 20530
		[SerializeField]
		private ProgressionRequirement m_progressionRequirement;

		// Token: 0x04005033 RID: 20531
		private ZoneRecord m_zoneRecord;
	}
}

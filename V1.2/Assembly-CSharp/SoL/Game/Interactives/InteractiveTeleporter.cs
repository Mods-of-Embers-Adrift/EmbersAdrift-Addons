using System;
using System.Collections;
using SoL.Game.Audio;
using SoL.Game.Messages;
using SoL.Managers;
using SoL.Networking.Database;
using SoL.UI;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Interactives
{
	// Token: 0x02000BA6 RID: 2982
	public class InteractiveTeleporter : TargetPosition, IInteractive, IInteractiveBase, ICursor
	{
		// Token: 0x06005C62 RID: 23650 RVA: 0x001F1264 File Offset: 0x001EF464
		private bool CanInteract(bool showFailureMessage)
		{
			if (!LocalPlayer.GameEntity || !LocalPlayer.GameEntity.VitalsReplicator || LocalPlayer.GameEntity.VitalsReplicator.CurrentHealthState.Value != HealthState.Alive)
			{
				return false;
			}
			if (!this.m_rewardRequirement.ClientMeetsRewardRequirement(LocalPlayer.GameEntity))
			{
				return false;
			}
			if (LocalPlayer.GameEntity.CharacterData.CharacterFlags.Value.HasBitFlag(PlayerFlags.InCombat))
			{
				if (showFailureMessage)
				{
					MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "You cannot use this while in combat!");
				}
				return false;
			}
			if (this.m_interactionDistance.IsWithinRange(base.gameObject, LocalPlayer.GameEntity))
			{
				Stance stance = LocalPlayer.GameEntity.Vitals.Stance;
				if (stance == Stance.Idle || stance == Stance.Torch)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06005C63 RID: 23651 RVA: 0x001F1320 File Offset: 0x001EF520
		public bool Interact()
		{
			if (this.CanInteract(true))
			{
				if (this.m_confirmationDialog)
				{
					DialogOptions opts = new DialogOptions
					{
						Title = "Teleport",
						Text = this.m_confirmationDialogText,
						ConfirmationText = "Yes",
						CancelText = "No",
						Callback = new Action<bool, object>(this.TeleportConfirmation),
						AutoCancel = new Func<bool>(this.AutoCancel)
					};
					ClientGameManager.UIManager.ConfirmationDialog.Init(opts);
				}
				else
				{
					this.ExecuteTeleport();
				}
				return true;
			}
			return false;
		}

		// Token: 0x06005C64 RID: 23652 RVA: 0x0007DFFF File Offset: 0x0007C1FF
		private bool AutoCancel()
		{
			return !this.CanInteract(false);
		}

		// Token: 0x06005C65 RID: 23653 RVA: 0x0007E00B File Offset: 0x0007C20B
		private void TeleportConfirmation(bool answer, object arg2)
		{
			if (answer && this.CanInteract(false))
			{
				this.ExecuteTeleport();
			}
		}

		// Token: 0x06005C66 RID: 23654 RVA: 0x001F13C0 File Offset: 0x001EF5C0
		private void ExecuteTeleport()
		{
			if (LocalPlayer.Motor)
			{
				ClientGameManager.UIManager.PlayRandomClip(this.m_audioClipCollection, null);
				LocalPlayer.Motor.SetPositionAndRotation(base.GetPosition(), base.GetRotation(), this.m_resetCameraRotation);
			}
		}

		// Token: 0x170015CD RID: 5581
		// (get) Token: 0x06005C67 RID: 23655 RVA: 0x00045BCA File Offset: 0x00043DCA
		bool IInteractive.RequiresLos
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06005C68 RID: 23656 RVA: 0x0007E01F File Offset: 0x0007C21F
		bool IInteractive.ClientInteraction()
		{
			return this.Interact();
		}

		// Token: 0x06005C69 RID: 23657 RVA: 0x00048A92 File Offset: 0x00046C92
		bool IInteractive.CanInteract(GameEntity entity)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06005C6A RID: 23658 RVA: 0x00048A92 File Offset: 0x00046C92
		void IInteractive.BeginInteraction(GameEntity interactionSource)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06005C6B RID: 23659 RVA: 0x00048A92 File Offset: 0x00046C92
		void IInteractive.EndInteraction(GameEntity interactionSource, bool clientIsEnding)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06005C6C RID: 23660 RVA: 0x00048A92 File Offset: 0x00046C92
		void IInteractive.EndAllInteractions()
		{
			throw new NotImplementedException();
		}

		// Token: 0x170015CE RID: 5582
		// (get) Token: 0x06005C6D RID: 23661 RVA: 0x0007E027 File Offset: 0x0007C227
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return this.m_interactionDistance;
			}
		}

		// Token: 0x170015CF RID: 5583
		// (get) Token: 0x06005C6E RID: 23662 RVA: 0x0007E02F File Offset: 0x0007C22F
		CursorType ICursor.Type
		{
			get
			{
				if (!this.CanInteract(false))
				{
					return CursorType.MainCursor;
				}
				return CursorType.GloveCursor;
			}
		}

		// Token: 0x170015D0 RID: 5584
		// (get) Token: 0x06005C6F RID: 23663 RVA: 0x00053971 File Offset: 0x00051B71
		private IEnumerable GetAudioClipCollections
		{
			get
			{
				return SolOdinUtilities.GetDropdownItems<AudioClipCollection>();
			}
		}

		// Token: 0x06005C71 RID: 23665 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04005017 RID: 20503
		[SerializeField]
		private bool m_resetCameraRotation;

		// Token: 0x04005018 RID: 20504
		[SerializeField]
		private InteractionSettings m_interactionDistance;

		// Token: 0x04005019 RID: 20505
		[SerializeField]
		private AudioClipCollection m_audioClipCollection;

		// Token: 0x0400501A RID: 20506
		[SerializeField]
		private ClientRewardRequirement m_rewardRequirement;

		// Token: 0x0400501B RID: 20507
		[SerializeField]
		private bool m_confirmationDialog;

		// Token: 0x0400501C RID: 20508
		[TextArea(2, 4)]
		[SerializeField]
		private string m_confirmationDialogText = string.Empty;
	}
}

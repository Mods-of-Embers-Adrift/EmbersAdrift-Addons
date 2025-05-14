using System;
using SoL.Game.Audio;
using SoL.Game.Messages;
using SoL.Managers;
using SoL.Networking.Database;
using SoL.Utilities;
using Unity.AI.Navigation;
using UnityEngine;

namespace SoL.Game.Interactives
{
	// Token: 0x02000B95 RID: 2965
	public class InteractiveLadder : MonoBehaviour, IInteractive, IInteractiveBase, ICursor
	{
		// Token: 0x06005B49 RID: 23369 RVA: 0x0007D554 File Offset: 0x0007B754
		private void Start()
		{
			if (GameManager.IsServer)
			{
				this.UpdateNavMeshLink(true);
			}
		}

		// Token: 0x06005B4A RID: 23370 RVA: 0x0007D564 File Offset: 0x0007B764
		private bool IsAboveMidPoint()
		{
			return LocalPlayer.GameEntity.gameObject.transform.position.y >= this.m_midPoint.transform.position.y;
		}

		// Token: 0x06005B4B RID: 23371 RVA: 0x001EE69C File Offset: 0x001EC89C
		private bool CanInteract(bool showFailureMessage)
		{
			if (!LocalPlayer.GameEntity || !LocalPlayer.GameEntity.VitalsReplicator || LocalPlayer.GameEntity.VitalsReplicator.CurrentHealthState.Value != HealthState.Alive || !LocalPlayer.Motor || !LocalPlayer.Motor.Motor.GroundingStatus.IsStableOnGround)
			{
				return false;
			}
			if (!this.m_rewardRequirement.ClientMeetsRewardRequirement(LocalPlayer.GameEntity))
			{
				return false;
			}
			if (!this.m_allowUseWhileInCombat && LocalPlayer.GameEntity.InCombat)
			{
				if (showFailureMessage)
				{
					MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "You cannot use this while in combat!");
				}
				return false;
			}
			GameObject obj = this.IsAboveMidPoint() ? this.m_upperPoint : this.m_lowerPoint;
			return this.m_interactionDistance.IsWithinRange(obj, LocalPlayer.GameEntity);
		}

		// Token: 0x06005B4C RID: 23372 RVA: 0x001EE768 File Offset: 0x001EC968
		private bool Interact()
		{
			if (!this.CanInteract(true))
			{
				return false;
			}
			ClientGameManager.UIManager.PlayRandomClip(this.m_audioClipCollection, null);
			TargetPosition targetPosition = this.IsAboveMidPoint() ? this.m_lowerPos : this.m_upperPos;
			LocalPlayer.Motor.SetPositionAndRotation(targetPosition.GetPosition(), targetPosition.GetRotation(), this.m_resetCameraRotation);
			return true;
		}

		// Token: 0x17001560 RID: 5472
		// (get) Token: 0x06005B4D RID: 23373 RVA: 0x00045BCA File Offset: 0x00043DCA
		bool IInteractive.RequiresLos
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17001561 RID: 5473
		// (get) Token: 0x06005B4E RID: 23374 RVA: 0x0007D599 File Offset: 0x0007B799
		GameObject IInteractiveBase.gameObject
		{
			get
			{
				if (!this.IsAboveMidPoint())
				{
					return this.m_lowerPoint;
				}
				return this.m_upperPoint;
			}
		}

		// Token: 0x17001562 RID: 5474
		// (get) Token: 0x06005B4F RID: 23375 RVA: 0x0007D5B0 File Offset: 0x0007B7B0
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return this.m_interactionDistance;
			}
		}

		// Token: 0x06005B50 RID: 23376 RVA: 0x0007D5B8 File Offset: 0x0007B7B8
		bool IInteractive.ClientInteraction()
		{
			return this.Interact();
		}

		// Token: 0x06005B51 RID: 23377 RVA: 0x00048A92 File Offset: 0x00046C92
		bool IInteractive.CanInteract(GameEntity entity)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06005B52 RID: 23378 RVA: 0x00048A92 File Offset: 0x00046C92
		void IInteractive.BeginInteraction(GameEntity interactionSource)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06005B53 RID: 23379 RVA: 0x00048A92 File Offset: 0x00046C92
		void IInteractive.EndInteraction(GameEntity interactionSource, bool clientIsEnding)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06005B54 RID: 23380 RVA: 0x00048A92 File Offset: 0x00046C92
		void IInteractive.EndAllInteractions()
		{
			throw new NotImplementedException();
		}

		// Token: 0x17001563 RID: 5475
		// (get) Token: 0x06005B55 RID: 23381 RVA: 0x0007D5C0 File Offset: 0x0007B7C0
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

		// Token: 0x06005B56 RID: 23382 RVA: 0x001EE7D0 File Offset: 0x001EC9D0
		private void UpdateNavMeshLink(bool isStart)
		{
			if (this.m_navMeshLink == null)
			{
				return;
			}
			this.m_navMeshLink.startPoint = base.gameObject.transform.InverseTransformPoint(this.m_upperPoint.transform.position);
			this.m_navMeshLink.endPoint = base.gameObject.transform.InverseTransformPoint(this.m_lowerPoint.transform.position);
			if (isStart && GameManager.IsServer && Application.isPlaying)
			{
				this.m_navMeshLink.gameObject.AddComponent<NavMeshLink>().CopyFrom(this.m_navMeshLink, true);
			}
		}

		// Token: 0x06005B57 RID: 23383 RVA: 0x0007D5CE File Offset: 0x0007B7CE
		private void OnValidate()
		{
			this.UpdateNavMeshLink(false);
		}

		// Token: 0x04004FBF RID: 20415
		public const string kInCombatMessage = "You cannot use this while in combat!";

		// Token: 0x04004FC0 RID: 20416
		[SerializeField]
		private bool m_allowUseWhileInCombat;

		// Token: 0x04004FC1 RID: 20417
		[SerializeField]
		private TargetPosition m_upperPos;

		// Token: 0x04004FC2 RID: 20418
		[SerializeField]
		private GameObject m_upperPoint;

		// Token: 0x04004FC3 RID: 20419
		[SerializeField]
		private GameObject m_midPoint;

		// Token: 0x04004FC4 RID: 20420
		[SerializeField]
		private GameObject m_lowerPoint;

		// Token: 0x04004FC5 RID: 20421
		[SerializeField]
		private TargetPosition m_lowerPos;

		// Token: 0x04004FC6 RID: 20422
		[SerializeField]
		private NavMeshLink m_navMeshLink;

		// Token: 0x04004FC7 RID: 20423
		[SerializeField]
		private bool m_resetCameraRotation;

		// Token: 0x04004FC8 RID: 20424
		[SerializeField]
		private InteractionSettings m_interactionDistance;

		// Token: 0x04004FC9 RID: 20425
		[SerializeField]
		private AudioClipCollection m_audioClipCollection;

		// Token: 0x04004FCA RID: 20426
		[SerializeField]
		private ClientRewardRequirement m_rewardRequirement;
	}
}

using System;
using SoL.Game.Messages;
using SoL.Game.Objects;
using SoL.Managers;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Interactives
{
	// Token: 0x02000B85 RID: 2949
	public class InteractiveDropLocation : MonoBehaviour, IInteractive, IInteractiveBase, ICursor
	{
		// Token: 0x06005AE3 RID: 23267 RVA: 0x001EDBA8 File Offset: 0x001EBDA8
		private void Awake()
		{
			if (!GameManager.IsServer && GameManager.QuestManager != null)
			{
				GameManager.QuestManager.QuestsUpdated += this.OnQuestsOrTasksUpdated;
				GameManager.QuestManager.BBTasksUpdated += this.OnQuestsOrTasksUpdated;
				LocalPlayer.LocalPlayerInitialized += this.OnLocalPlayerInitialized;
			}
		}

		// Token: 0x06005AE4 RID: 23268 RVA: 0x001EDC08 File Offset: 0x001EBE08
		private void OnDestroy()
		{
			if (!GameManager.IsServer && GameManager.QuestManager != null)
			{
				GameManager.QuestManager.QuestsUpdated -= this.OnQuestsOrTasksUpdated;
				GameManager.QuestManager.BBTasksUpdated -= this.OnQuestsOrTasksUpdated;
			}
		}

		// Token: 0x06005AE5 RID: 23269 RVA: 0x0007D07B File Offset: 0x0007B27B
		private void DoInternalInteraction()
		{
			if (!this.m_dropLocation.TryDrop(LocalPlayer.GameEntity))
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "Objective requirements not met!");
			}
		}

		// Token: 0x06005AE6 RID: 23270 RVA: 0x0007D0A0 File Offset: 0x0007B2A0
		private void OnLocalPlayerInitialized()
		{
			LocalPlayer.LocalPlayerInitialized -= this.OnLocalPlayerInitialized;
			this.m_isObjectiveActiveAndIncomplete = this.m_dropLocation.HasActiveIncompleteObjective(LocalPlayer.GameEntity);
		}

		// Token: 0x06005AE7 RID: 23271 RVA: 0x0007D0C9 File Offset: 0x0007B2C9
		private void OnQuestsOrTasksUpdated()
		{
			this.m_isObjectiveActiveAndIncomplete = this.m_dropLocation.HasActiveIncompleteObjective(LocalPlayer.GameEntity);
		}

		// Token: 0x17001544 RID: 5444
		// (get) Token: 0x06005AE8 RID: 23272 RVA: 0x00045BCA File Offset: 0x00043DCA
		bool IInteractive.RequiresLos
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06005AE9 RID: 23273 RVA: 0x0007D0E1 File Offset: 0x0007B2E1
		public bool ClientInteraction()
		{
			if (this.CanInteract(LocalPlayer.GameEntity) && this.m_isObjectiveActiveAndIncomplete)
			{
				this.DoInternalInteraction();
				return true;
			}
			return false;
		}

		// Token: 0x06005AEA RID: 23274 RVA: 0x0007D101 File Offset: 0x0007B301
		public virtual bool CanInteract(GameEntity entity)
		{
			return entity != null && this.m_interactionDistance.IsWithinRange(base.gameObject, entity);
		}

		// Token: 0x06005AEB RID: 23275 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void BeginInteraction(GameEntity interactionSource)
		{
		}

		// Token: 0x06005AEC RID: 23276 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void EndInteraction(GameEntity interactionSource, bool clientIsEnding)
		{
		}

		// Token: 0x06005AED RID: 23277 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void EndAllInteractions()
		{
		}

		// Token: 0x17001545 RID: 5445
		// (get) Token: 0x06005AEE RID: 23278 RVA: 0x00070E66 File Offset: 0x0006F066
		protected virtual CursorType ActiveCursorType
		{
			get
			{
				return CursorType.GloveCursor;
			}
		}

		// Token: 0x17001546 RID: 5446
		// (get) Token: 0x06005AEF RID: 23279 RVA: 0x0004479C File Offset: 0x0004299C
		protected virtual CursorType InactiveCursorType
		{
			get
			{
				return CursorType.MainCursor;
			}
		}

		// Token: 0x17001547 RID: 5447
		// (get) Token: 0x06005AF0 RID: 23280 RVA: 0x0007D120 File Offset: 0x0007B320
		CursorType ICursor.Type
		{
			get
			{
				if (!this.CanInteract(LocalPlayer.GameEntity) || !this.m_isObjectiveActiveAndIncomplete)
				{
					return this.InactiveCursorType;
				}
				return this.ActiveCursorType;
			}
		}

		// Token: 0x17001548 RID: 5448
		// (get) Token: 0x06005AF1 RID: 23281 RVA: 0x0007D144 File Offset: 0x0007B344
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return this.m_interactionDistance;
			}
		}

		// Token: 0x06005AF3 RID: 23283 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04004F9D RID: 20381
		[SerializeField]
		private InteractionSettings m_interactionDistance;

		// Token: 0x04004F9E RID: 20382
		[SerializeField]
		private DropLocation m_dropLocation;

		// Token: 0x04004F9F RID: 20383
		private bool m_isObjectiveActiveAndIncomplete;
	}
}

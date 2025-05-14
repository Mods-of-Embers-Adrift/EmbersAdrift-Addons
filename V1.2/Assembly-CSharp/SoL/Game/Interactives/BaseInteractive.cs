using System;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Interactives
{
	// Token: 0x02000B77 RID: 2935
	public abstract class BaseInteractive : GameEntityComponent, IInteractive, IInteractiveBase, ICursor
	{
		// Token: 0x06005A50 RID: 23120 RVA: 0x0007CA2C File Offset: 0x0007AC2C
		protected virtual void Awake()
		{
			base.GameEntity.Interactive = this;
		}

		// Token: 0x06005A51 RID: 23121 RVA: 0x0007CA3A File Offset: 0x0007AC3A
		public bool ClientInteraction()
		{
			if (this.CanInteract(LocalPlayer.GameEntity) && this.DoubleClickPreCondition(LocalPlayer.GameEntity))
			{
				LocalPlayer.NetworkEntity.PlayerRpcHandler.Client_RequestInteraction(base.GameEntity.NetworkEntity);
				return true;
			}
			return false;
		}

		// Token: 0x06005A52 RID: 23122 RVA: 0x0004479C File Offset: 0x0004299C
		protected virtual bool DoubleClickPreCondition(GameEntity entity)
		{
			return true;
		}

		// Token: 0x06005A53 RID: 23123 RVA: 0x0007CA73 File Offset: 0x0007AC73
		public virtual bool CanInteract(GameEntity entity)
		{
			return entity != null && entity.IsAlive && this.m_interactionDistance.IsWithinRange(base.GameEntity, entity);
		}

		// Token: 0x06005A54 RID: 23124 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void BeginInteraction(GameEntity interactionSource)
		{
		}

		// Token: 0x06005A55 RID: 23125 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void EndInteraction(GameEntity interactionSource, bool clientIsEnding)
		{
		}

		// Token: 0x06005A56 RID: 23126 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void EndAllInteractions()
		{
		}

		// Token: 0x17001504 RID: 5380
		// (get) Token: 0x06005A57 RID: 23127 RVA: 0x00070E66 File Offset: 0x0006F066
		protected virtual CursorType ActiveCursorType
		{
			get
			{
				return CursorType.GloveCursor;
			}
		}

		// Token: 0x17001505 RID: 5381
		// (get) Token: 0x06005A58 RID: 23128 RVA: 0x0004479C File Offset: 0x0004299C
		protected virtual CursorType InactiveCursorType
		{
			get
			{
				return CursorType.MainCursor;
			}
		}

		// Token: 0x17001506 RID: 5382
		// (get) Token: 0x06005A59 RID: 23129 RVA: 0x0007CA9A File Offset: 0x0007AC9A
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

		// Token: 0x17001507 RID: 5383
		// (get) Token: 0x06005A5A RID: 23130 RVA: 0x0007CAB6 File Offset: 0x0007ACB6
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return this.m_interactionDistance;
			}
		}

		// Token: 0x17001508 RID: 5384
		// (get) Token: 0x06005A5B RID: 23131 RVA: 0x00045BCA File Offset: 0x00043DCA
		bool IInteractive.RequiresLos
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06005A5D RID: 23133 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04004F64 RID: 20324
		[SerializeField]
		private InteractionSettings m_interactionDistance;
	}
}

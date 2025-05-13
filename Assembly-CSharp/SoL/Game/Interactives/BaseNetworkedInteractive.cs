using System;
using System.Collections.Generic;
using SoL.Game.Animation;
using SoL.Managers;
using SoL.Networking.Replication;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Interactives
{
	// Token: 0x02000B78 RID: 2936
	public class BaseNetworkedInteractive : SyncVarReplicator, IInteractive, IInteractiveBase, ICursor
	{
		// Token: 0x17001509 RID: 5385
		// (get) Token: 0x06005A5E RID: 23134 RVA: 0x00045BCA File Offset: 0x00043DCA
		protected virtual bool AllowInteractionWhileMissingBag
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700150A RID: 5386
		// (get) Token: 0x06005A5F RID: 23135 RVA: 0x0007CABE File Offset: 0x0007ACBE
		protected bool PreventInteractionsWhileMissingBag
		{
			get
			{
				return !this.AllowInteractionWhileMissingBag;
			}
		}

		// Token: 0x1700150B RID: 5387
		// (get) Token: 0x06005A60 RID: 23136 RVA: 0x0007CAC9 File Offset: 0x0007ACC9
		public AnimancerAnimationPose InteractionPose
		{
			get
			{
				return this.m_interactionPose;
			}
		}

		// Token: 0x1700150C RID: 5388
		// (get) Token: 0x06005A61 RID: 23137 RVA: 0x0004479C File Offset: 0x0004299C
		protected virtual bool ValidateOnAwake
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700150D RID: 5389
		// (get) Token: 0x06005A62 RID: 23138 RVA: 0x0007CAD1 File Offset: 0x0007ACD1
		protected int CurrentInteractorCount
		{
			get
			{
				if (this.m_currentInteractors == null)
				{
					return 0;
				}
				return this.m_currentInteractors.Count;
			}
		}

		// Token: 0x06005A63 RID: 23139 RVA: 0x0007CAE8 File Offset: 0x0007ACE8
		protected virtual void Awake()
		{
			base.GameEntity.Interactive = this;
			if (this.ValidateOnAwake)
			{
				this.StartValidatingInteractors();
			}
		}

		// Token: 0x06005A64 RID: 23140 RVA: 0x0007CB04 File Offset: 0x0007AD04
		protected override void OnDestroy()
		{
			if (this.m_invoking)
			{
				base.CancelInvoke("ValidateInteractors");
			}
			StaticListPool<GameEntity>.ReturnToPool(this.m_currentInteractors);
			this.m_currentInteractors = null;
			base.OnDestroy();
		}

		// Token: 0x06005A65 RID: 23141 RVA: 0x001ECA7C File Offset: 0x001EAC7C
		protected void StartValidatingInteractors()
		{
			if (GameManager.IsServer && !this.m_invoking)
			{
				this.m_invoking = true;
				base.InvokeRepeating("ValidateInteractors", BaseNetworkedInteractive.kStaggeredOffset, 1f);
				BaseNetworkedInteractive.kStaggeredOffset += 0.1f;
				if (BaseNetworkedInteractive.kStaggeredOffset > 1f)
				{
					BaseNetworkedInteractive.kStaggeredOffset = 0f;
				}
			}
		}

		// Token: 0x06005A66 RID: 23142 RVA: 0x001ECADC File Offset: 0x001EACDC
		public virtual bool ClientInteraction()
		{
			bool result = false;
			this.m_notifyOnCanInteractFail = true;
			if (this.CanInteract(LocalPlayer.GameEntity))
			{
				LocalPlayer.NetworkEntity.PlayerRpcHandler.Client_RequestInteraction(base.GameEntity.NetworkEntity);
				result = true;
			}
			this.m_notifyOnCanInteractFail = false;
			return result;
		}

		// Token: 0x06005A67 RID: 23143 RVA: 0x0007CB31 File Offset: 0x0007AD31
		public virtual bool CanInteract(GameEntity entity)
		{
			return entity != null && entity.IsAlive && this.IsWithinDistance(entity) && (this.AllowInteractionWhileMissingBag || !entity.IsMissingBag);
		}

		// Token: 0x06005A68 RID: 23144 RVA: 0x0007CB62 File Offset: 0x0007AD62
		public bool IsWithinDistance(GameEntity entity)
		{
			return this.m_interactionDistance.IsWithinRange(base.GameEntity, entity);
		}

		// Token: 0x06005A69 RID: 23145 RVA: 0x0007CB76 File Offset: 0x0007AD76
		public virtual void BeginInteraction(GameEntity interactionSource)
		{
			if (GameManager.IsServer)
			{
				if (this.m_currentInteractors == null)
				{
					this.m_currentInteractors = StaticListPool<GameEntity>.GetFromPool();
				}
				this.m_currentInteractors.Add(interactionSource);
			}
		}

		// Token: 0x06005A6A RID: 23146 RVA: 0x0007CB9E File Offset: 0x0007AD9E
		public virtual void EndInteraction(GameEntity interactionSource, bool clientIsEnding)
		{
			if (GameManager.IsServer)
			{
				List<GameEntity> currentInteractors = this.m_currentInteractors;
				if (currentInteractors == null)
				{
					return;
				}
				currentInteractors.Remove(interactionSource);
			}
		}

		// Token: 0x06005A6B RID: 23147 RVA: 0x001ECB24 File Offset: 0x001EAD24
		public virtual void EndAllInteractions()
		{
			if (GameManager.IsServer && this.m_currentInteractors != null)
			{
				for (int i = 0; i < this.m_currentInteractors.Count; i++)
				{
					if (this.m_currentInteractors[i] == null)
					{
						this.m_currentInteractors.RemoveAt(i);
						i--;
					}
					else
					{
						this.EndInteraction(this.m_currentInteractors[i], false);
						i--;
					}
				}
			}
		}

		// Token: 0x06005A6C RID: 23148 RVA: 0x001ECB94 File Offset: 0x001EAD94
		private void ValidateInteractors()
		{
			if (this.m_currentInteractors == null || this.m_currentInteractors.Count <= 0)
			{
				return;
			}
			for (int i = 0; i < this.m_currentInteractors.Count; i++)
			{
				if (this.m_currentInteractors[i] == null)
				{
					this.m_currentInteractors.RemoveAt(i);
					i--;
				}
				else if ((this.m_currentInteractors[i].VitalsReplicator && this.m_currentInteractors[i].VitalsReplicator.CurrentHealthState.Value != HealthState.Alive) || !this.IsWithinDistance(this.m_currentInteractors[i]))
				{
					this.EndInteraction(this.m_currentInteractors[i], false);
					i--;
				}
			}
		}

		// Token: 0x1700150E RID: 5390
		// (get) Token: 0x06005A6D RID: 23149 RVA: 0x00070E66 File Offset: 0x0006F066
		protected virtual CursorType ActiveCursorType
		{
			get
			{
				return CursorType.GloveCursor;
			}
		}

		// Token: 0x1700150F RID: 5391
		// (get) Token: 0x06005A6E RID: 23150 RVA: 0x0004479C File Offset: 0x0004299C
		protected virtual CursorType InactiveCursorType
		{
			get
			{
				return CursorType.MainCursor;
			}
		}

		// Token: 0x17001510 RID: 5392
		// (get) Token: 0x06005A6F RID: 23151 RVA: 0x0007CBB9 File Offset: 0x0007ADB9
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

		// Token: 0x17001511 RID: 5393
		// (get) Token: 0x06005A70 RID: 23152 RVA: 0x00045BCA File Offset: 0x00043DCA
		bool IInteractive.RequiresLos
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17001512 RID: 5394
		// (get) Token: 0x06005A71 RID: 23153 RVA: 0x0007CBD5 File Offset: 0x0007ADD5
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return this.m_interactionDistance;
			}
		}

		// Token: 0x06005A73 RID: 23155 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04004F65 RID: 20325
		private const float kCadence = 1f;

		// Token: 0x04004F66 RID: 20326
		private const float kOffsetDelta = 0.1f;

		// Token: 0x04004F67 RID: 20327
		private static float kStaggeredOffset;

		// Token: 0x04004F68 RID: 20328
		private List<GameEntity> m_currentInteractors;

		// Token: 0x04004F69 RID: 20329
		[SerializeField]
		private InteractionSettings m_interactionDistance;

		// Token: 0x04004F6A RID: 20330
		[SerializeField]
		private AnimancerAnimationPose m_interactionPose;

		// Token: 0x04004F6B RID: 20331
		private bool m_invoking;

		// Token: 0x04004F6C RID: 20332
		protected bool m_notifyOnCanInteractFail;
	}
}

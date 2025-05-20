using System;
using SoL.Game.Quests;
using SoL.Managers;
using SoL.UI;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Interactives
{
	// Token: 0x02000B81 RID: 2945
	public class InteractiveBulletinBoard : MonoBehaviour, IInteractive, IInteractiveBase, ICursor, ITooltip
	{
		// Token: 0x17001538 RID: 5432
		// (get) Token: 0x06005AC0 RID: 23232 RVA: 0x0007CEBF File Offset: 0x0007B0BF
		public bool Enabled
		{
			get
			{
				return this.m_enabled.HasBitFlag(DeploymentBranchFlagsExtensions.GetBranchFlags()) && !this.m_gmOnly;
			}
		}

		// Token: 0x17001539 RID: 5433
		// (get) Token: 0x06005AC1 RID: 23233 RVA: 0x0007CEDE File Offset: 0x0007B0DE
		public BulletinBoard Board
		{
			get
			{
				return this.m_bulletinBoard;
			}
		}

		// Token: 0x1700153A RID: 5434
		// (get) Token: 0x06005AC2 RID: 23234 RVA: 0x00045BCA File Offset: 0x00043DCA
		bool IInteractive.RequiresLos
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06005AC3 RID: 23235 RVA: 0x0007CEE6 File Offset: 0x0007B0E6
		public bool ClientInteraction()
		{
			if (this.CanInteract(LocalPlayer.GameEntity))
			{
				ClientGameManager.UIManager.BulletinBoardUI.Interactive = this;
				ClientGameManager.UIManager.BulletinBoardUI.Show(false);
				return true;
			}
			return false;
		}

		// Token: 0x06005AC4 RID: 23236 RVA: 0x0007CF18 File Offset: 0x0007B118
		public virtual bool CanInteract(GameEntity entity)
		{
			return entity && this.Enabled && this.m_interactionDistance.IsWithinRange(base.gameObject, entity) && this.m_bulletinBoard != null;
		}

		// Token: 0x06005AC5 RID: 23237 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void BeginInteraction(GameEntity interactionSource)
		{
		}

		// Token: 0x06005AC6 RID: 23238 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void EndInteraction(GameEntity interactionSource, bool clientIsEnding)
		{
		}

		// Token: 0x06005AC7 RID: 23239 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void EndAllInteractions()
		{
		}

		// Token: 0x1700153B RID: 5435
		// (get) Token: 0x06005AC8 RID: 23240 RVA: 0x00066DE7 File Offset: 0x00064FE7
		protected virtual CursorType ActiveCursorType
		{
			get
			{
				return CursorType.TextCursor;
			}
		}

		// Token: 0x1700153C RID: 5436
		// (get) Token: 0x06005AC9 RID: 23241 RVA: 0x0004479C File Offset: 0x0004299C
		protected virtual CursorType InactiveCursorType
		{
			get
			{
				return CursorType.MainCursor;
			}
		}

		// Token: 0x1700153D RID: 5437
		// (get) Token: 0x06005ACA RID: 23242 RVA: 0x0007CF4C File Offset: 0x0007B14C
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

		// Token: 0x1700153E RID: 5438
		// (get) Token: 0x06005ACB RID: 23243 RVA: 0x0007CF68 File Offset: 0x0007B168
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return this.m_interactionDistance;
			}
		}

		// Token: 0x06005ACC RID: 23244 RVA: 0x001ED97C File Offset: 0x001EBB7C
		private ITooltipParameter GetParameter()
		{
			if (this.m_bulletinBoard == null)
			{
				return null;
			}
			if (!this.Enabled || string.IsNullOrEmpty(this.m_bulletinBoard.Title))
			{
				return new ObjectTextTooltipParameter(this, "Bulletin Board", false);
			}
			return new ObjectTextTooltipParameter(this, this.m_bulletinBoard.Title, false);
		}

		// Token: 0x1700153F RID: 5439
		// (get) Token: 0x06005ACD RID: 23245 RVA: 0x0007CF70 File Offset: 0x0007B170
		public BaseTooltip.GetTooltipParameter GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetParameter);
			}
		}

		// Token: 0x17001540 RID: 5440
		// (get) Token: 0x06005ACE RID: 23246 RVA: 0x0007CF7E File Offset: 0x0007B17E
		public TooltipSettings TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x06005AD0 RID: 23248 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04004F90 RID: 20368
		[SerializeField]
		private BulletinBoard m_bulletinBoard;

		// Token: 0x04004F91 RID: 20369
		[SerializeField]
		private InteractionSettings m_interactionDistance;

		// Token: 0x04004F92 RID: 20370
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x04004F93 RID: 20371
		[SerializeField]
		private DeploymentBranchFlags m_enabled = DeploymentBranchFlags.DEV | DeploymentBranchFlags.QA | DeploymentBranchFlags.LIVE;

		// Token: 0x04004F94 RID: 20372
		[SerializeField]
		private bool m_gmOnly;
	}
}

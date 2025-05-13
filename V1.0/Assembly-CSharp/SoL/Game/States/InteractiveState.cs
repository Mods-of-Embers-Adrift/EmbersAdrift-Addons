using System;
using Cysharp.Text;
using SoL.Game.Interactives;
using SoL.UI;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.States
{
	// Token: 0x0200065E RID: 1630
	public class InteractiveState : BaseState, IInteractive, IInteractiveBase, ITooltip, ICursor
	{
		// Token: 0x17000ACF RID: 2767
		// (get) Token: 0x060032BD RID: 12989 RVA: 0x00062EC4 File Offset: 0x000610C4
		protected virtual string CanInteractText
		{
			get
			{
				return "Interact";
			}
		}

		// Token: 0x17000AD0 RID: 2768
		// (get) Token: 0x060032BE RID: 12990 RVA: 0x00062ECB File Offset: 0x000610CB
		protected virtual string CannotInteractText
		{
			get
			{
				return "Locked by a contraption";
			}
		}

		// Token: 0x17000AD1 RID: 2769
		// (get) Token: 0x060032BF RID: 12991 RVA: 0x00045BCA File Offset: 0x00043DCA
		protected virtual bool PreventClientFromProgressing
		{
			get
			{
				return false;
			}
		}

		// Token: 0x060032C0 RID: 12992 RVA: 0x00062ED2 File Offset: 0x000610D2
		internal void InjectTooltipText(string txt)
		{
			this.m_injectedTooltipText = txt;
		}

		// Token: 0x060032C1 RID: 12993 RVA: 0x00062EDB File Offset: 0x000610DB
		private bool MinTimeHasElapsed()
		{
			return Time.time - this.m_timeOfLastInteraction >= this.m_minTimeBetweenInteractions;
		}

		// Token: 0x060032C2 RID: 12994 RVA: 0x00062EF4 File Offset: 0x000610F4
		public virtual bool CanInteract(GameEntity entity)
		{
			return entity && entity.IsAlive && this.MinTimeHasElapsed() && this.m_interactionSettings.IsWithinRange(base.GameEntity, entity);
		}

		// Token: 0x060032C3 RID: 12995 RVA: 0x001615F4 File Offset: 0x0015F7F4
		public bool ClientInteraction()
		{
			if (this.CanInteract(LocalPlayer.GameEntity))
			{
				if (LocalPlayer.NetworkEntity && LocalPlayer.NetworkEntity.PlayerRpcHandler)
				{
					LocalPlayer.NetworkEntity.PlayerRpcHandler.Client_RequestStateInteraction(base.Key);
					this.m_timeOfLastInteraction = Time.time;
				}
				return true;
			}
			return false;
		}

		// Token: 0x17000AD2 RID: 2770
		// (get) Token: 0x060032C4 RID: 12996 RVA: 0x00062F22 File Offset: 0x00061122
		GameObject IInteractiveBase.gameObject
		{
			get
			{
				if (!this.m_requireLos || !this.m_losObject)
				{
					return base.gameObject;
				}
				return this.m_losObject;
			}
		}

		// Token: 0x17000AD3 RID: 2771
		// (get) Token: 0x060032C5 RID: 12997 RVA: 0x00062F46 File Offset: 0x00061146
		bool IInteractive.RequiresLos
		{
			get
			{
				return this.m_requireLos;
			}
		}

		// Token: 0x17000AD4 RID: 2772
		// (get) Token: 0x060032C6 RID: 12998 RVA: 0x00062F4E File Offset: 0x0006114E
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return this.m_interactionSettings;
			}
		}

		// Token: 0x060032C7 RID: 12999 RVA: 0x00062F56 File Offset: 0x00061156
		bool IInteractive.ClientInteraction()
		{
			return this.ClientInteraction();
		}

		// Token: 0x060032C8 RID: 13000 RVA: 0x00062F5E File Offset: 0x0006115E
		bool IInteractive.CanInteract(GameEntity entity)
		{
			return this.CanInteract(entity);
		}

		// Token: 0x060032C9 RID: 13001 RVA: 0x00062F67 File Offset: 0x00061167
		void IInteractive.BeginInteraction(GameEntity interactionSource)
		{
			if (this.PreventClientFromProgressing)
			{
				return;
			}
			this.m_clientChanged = true;
			base.ProgressState();
			this.m_clientChanged = false;
		}

		// Token: 0x060032CA RID: 13002 RVA: 0x0004475B File Offset: 0x0004295B
		void IInteractive.EndInteraction(GameEntity interactionSource, bool clientIsEnding)
		{
		}

		// Token: 0x060032CB RID: 13003 RVA: 0x0004475B File Offset: 0x0004295B
		void IInteractive.EndAllInteractions()
		{
		}

		// Token: 0x060032CC RID: 13004 RVA: 0x00161650 File Offset: 0x0015F850
		private ITooltipParameter GetTooltipParameter()
		{
			if (!this.MinTimeHasElapsed())
			{
				return null;
			}
			string text = this.CanInteract(LocalPlayer.GameEntity) ? this.CanInteractText : this.CannotInteractText;
			if (!string.IsNullOrEmpty(this.m_injectedTooltipText))
			{
				text = ZString.Format<string, string>("{0}\n{1}", text, this.m_injectedTooltipText);
			}
			return new ObjectTextTooltipParameter(this, text, false);
		}

		// Token: 0x17000AD5 RID: 2773
		// (get) Token: 0x060032CD RID: 13005 RVA: 0x00062F86 File Offset: 0x00061186
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x17000AD6 RID: 2774
		// (get) Token: 0x060032CE RID: 13006 RVA: 0x00062F94 File Offset: 0x00061194
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x17000AD7 RID: 2775
		// (get) Token: 0x060032CF RID: 13007 RVA: 0x00062F9C File Offset: 0x0006119C
		CursorType ICursor.Type
		{
			get
			{
				if (!this.CanInteract(LocalPlayer.GameEntity))
				{
					return CursorType.GloveCursorInactive;
				}
				return CursorType.GloveCursor;
			}
		}

		// Token: 0x0400311B RID: 12571
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x0400311C RID: 12572
		[SerializeField]
		private InteractionSettings m_interactionSettings;

		// Token: 0x0400311D RID: 12573
		[SerializeField]
		private float m_minTimeBetweenInteractions = 1f;

		// Token: 0x0400311E RID: 12574
		[SerializeField]
		private bool m_requireLos;

		// Token: 0x0400311F RID: 12575
		[SerializeField]
		private GameObject m_losObject;

		// Token: 0x04003120 RID: 12576
		private float m_timeOfLastInteraction = float.MinValue;

		// Token: 0x04003121 RID: 12577
		private string m_injectedTooltipText;
	}
}

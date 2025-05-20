using System;
using SoL.UI;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Interactives
{
	// Token: 0x02000B8D RID: 2957
	public class InteractiveForwarder : GameEntityComponent, IInteractive, IInteractiveBase, IContextMenu, ITooltip, ICursor, IHighlight
	{
		// Token: 0x17001550 RID: 5456
		// (get) Token: 0x06005B20 RID: 23328 RVA: 0x0007D3F5 File Offset: 0x0007B5F5
		public IInteractive Interactive
		{
			get
			{
				return this.m_interactive;
			}
		}

		// Token: 0x06005B21 RID: 23329 RVA: 0x0007D3FD File Offset: 0x0007B5FD
		private void Awake()
		{
			base.gameObject.layer = LayerMap.Interaction.Layer;
		}

		// Token: 0x06005B22 RID: 23330 RVA: 0x001EE414 File Offset: 0x001EC614
		private void Start()
		{
			if (this.m_selfInit)
			{
				if (base.GameEntity != null)
				{
					this.Init(base.GameEntity.gameObject);
					return;
				}
			}
			else if (this.m_target != null)
			{
				this.Init(this.m_target);
			}
		}

		// Token: 0x06005B23 RID: 23331 RVA: 0x001EE464 File Offset: 0x001EC664
		public void Init(GameObject obj)
		{
			if (obj)
			{
				this.m_interactive = obj.GetComponent<IInteractive>();
				this.m_contextMenu = obj.GetComponent<IContextMenu>();
				this.m_highlight = obj.GetComponent<IHighlight>();
				this.m_tooltip = obj.GetComponent<ITooltip>();
				this.m_cursor = obj.GetComponent<ICursor>();
			}
		}

		// Token: 0x17001551 RID: 5457
		// (get) Token: 0x06005B24 RID: 23332 RVA: 0x0007D419 File Offset: 0x0007B619
		bool IInteractive.RequiresLos
		{
			get
			{
				return this.m_interactive != null && this.m_interactive.RequiresLos;
			}
		}

		// Token: 0x06005B25 RID: 23333 RVA: 0x0007D430 File Offset: 0x0007B630
		bool IInteractive.ClientInteraction()
		{
			return this.m_interactive != null && this.m_interactive.ClientInteraction();
		}

		// Token: 0x06005B26 RID: 23334 RVA: 0x0007D447 File Offset: 0x0007B647
		bool IInteractive.CanInteract(GameEntity entity)
		{
			return this.m_interactive != null && this.m_interactive.CanInteract(entity);
		}

		// Token: 0x06005B27 RID: 23335 RVA: 0x0007D45F File Offset: 0x0007B65F
		void IInteractive.BeginInteraction(GameEntity interactionSource)
		{
			IInteractive interactive = this.m_interactive;
			if (interactive == null)
			{
				return;
			}
			interactive.BeginInteraction(interactionSource);
		}

		// Token: 0x06005B28 RID: 23336 RVA: 0x0007D472 File Offset: 0x0007B672
		void IInteractive.EndInteraction(GameEntity interactionSource, bool clientIsEnding)
		{
			IInteractive interactive = this.m_interactive;
			if (interactive == null)
			{
				return;
			}
			interactive.EndInteraction(interactionSource, clientIsEnding);
		}

		// Token: 0x06005B29 RID: 23337 RVA: 0x0007D486 File Offset: 0x0007B686
		void IInteractive.EndAllInteractions()
		{
			IInteractive interactive = this.m_interactive;
			if (interactive == null)
			{
				return;
			}
			interactive.EndAllInteractions();
		}

		// Token: 0x06005B2A RID: 23338 RVA: 0x0007D498 File Offset: 0x0007B698
		string IContextMenu.FillActionsGetTitle()
		{
			IContextMenu contextMenu = this.m_contextMenu;
			if (contextMenu == null)
			{
				return null;
			}
			return contextMenu.FillActionsGetTitle();
		}

		// Token: 0x06005B2B RID: 23339 RVA: 0x0007D4AB File Offset: 0x0007B6AB
		private ITooltipParameter GetTooltipParameter()
		{
			if (this.m_tooltip != null)
			{
				return this.m_tooltip.GetTooltipParameter();
			}
			return null;
		}

		// Token: 0x17001552 RID: 5458
		// (get) Token: 0x06005B2C RID: 23340 RVA: 0x0007D4C7 File Offset: 0x0007B6C7
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x17001553 RID: 5459
		// (get) Token: 0x06005B2D RID: 23341 RVA: 0x001EE4B8 File Offset: 0x001EC6B8
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				if (this.m_tooltip != null)
				{
					return this.m_tooltip.TooltipSettings;
				}
				return default(TooltipSettings);
			}
		}

		// Token: 0x17001554 RID: 5460
		// (get) Token: 0x06005B2E RID: 23342 RVA: 0x0007D4D5 File Offset: 0x0007B6D5
		CursorType ICursor.Type
		{
			get
			{
				if (this.m_cursor != null)
				{
					return this.m_cursor.Type;
				}
				return CursorType.MainCursor;
			}
		}

		// Token: 0x17001555 RID: 5461
		// (get) Token: 0x06005B2F RID: 23343 RVA: 0x0007D4EC File Offset: 0x0007B6EC
		// (set) Token: 0x06005B30 RID: 23344 RVA: 0x0007D503 File Offset: 0x0007B703
		bool IHighlight.HighlightEnabled
		{
			get
			{
				return this.m_highlight != null && this.m_highlight.HighlightEnabled;
			}
			set
			{
				if (this.m_highlight != null)
				{
					this.m_highlight.HighlightEnabled = value;
				}
			}
		}

		// Token: 0x17001556 RID: 5462
		// (get) Token: 0x06005B31 RID: 23345 RVA: 0x0007D519 File Offset: 0x0007B719
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				IInteractiveBase interactiveBase = this.GetInteractiveBase();
				if (interactiveBase == null)
				{
					return null;
				}
				return interactiveBase.Settings;
			}
		}

		// Token: 0x17001557 RID: 5463
		// (get) Token: 0x06005B32 RID: 23346 RVA: 0x0007D52C File Offset: 0x0007B72C
		GameObject IInteractiveBase.gameObject
		{
			get
			{
				IInteractiveBase interactiveBase = this.GetInteractiveBase();
				if (interactiveBase == null)
				{
					return null;
				}
				return interactiveBase.gameObject;
			}
		}

		// Token: 0x06005B33 RID: 23347 RVA: 0x001EE4E4 File Offset: 0x001EC6E4
		private IInteractiveBase GetInteractiveBase()
		{
			IInteractiveBase result = null;
			if (this.m_interactive != null)
			{
				result = this.m_interactive;
			}
			else if (this.m_contextMenu != null)
			{
				result = this.m_contextMenu;
			}
			else if (this.m_highlight != null)
			{
				result = this.m_highlight;
			}
			else if (this.m_tooltip != null)
			{
				result = this.m_tooltip;
			}
			else if (this.m_cursor != null)
			{
				result = this.m_cursor;
			}
			return result;
		}

		// Token: 0x04004FB7 RID: 20407
		private IInteractive m_interactive;

		// Token: 0x04004FB8 RID: 20408
		private IContextMenu m_contextMenu;

		// Token: 0x04004FB9 RID: 20409
		private IHighlight m_highlight;

		// Token: 0x04004FBA RID: 20410
		private ITooltip m_tooltip;

		// Token: 0x04004FBB RID: 20411
		private ICursor m_cursor;

		// Token: 0x04004FBC RID: 20412
		[SerializeField]
		private bool m_selfInit;

		// Token: 0x04004FBD RID: 20413
		[SerializeField]
		private GameObject m_target;
	}
}

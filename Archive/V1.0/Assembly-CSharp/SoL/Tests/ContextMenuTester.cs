using System;
using System.Collections.Generic;
using SoL.Game.Interactives;
using SoL.UI;
using UnityEngine;

namespace SoL.Tests
{
	// Token: 0x02000D97 RID: 3479
	public class ContextMenuTester : MonoBehaviour, IContextMenu, IInteractiveBase
	{
		// Token: 0x1700190E RID: 6414
		// (get) Token: 0x0600687C RID: 26748 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x0600687D RID: 26749 RVA: 0x00214AB8 File Offset: 0x00212CB8
		public string FillActionsGetTitle()
		{
			ContextMenuAction contextMenuAction = new ContextMenuAction();
			contextMenuAction.Text = "Testing sub menu";
			contextMenuAction.Enabled = true;
			contextMenuAction.Callback = delegate()
			{
				Debug.Log("Clicked nested menu");
			};
			contextMenuAction.NestedActions = null;
			ContextMenuAction item = contextMenuAction;
			ContextMenuAction contextMenuAction2 = new ContextMenuAction();
			contextMenuAction2.Text = "Testing 1";
			contextMenuAction2.Enabled = true;
			contextMenuAction2.Callback = delegate()
			{
				Debug.Log("Clicked!!");
			};
			contextMenuAction2.NestedActions = new List<ContextMenuAction>
			{
				item
			};
			ContextMenuAction item2 = contextMenuAction2;
			ContextMenuUI.ActionList.Add(item2);
			ContextMenuUI.ActionList.Add(item2);
			return "TEST FILL";
		}

		// Token: 0x0600687F RID: 26751 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}
	}
}

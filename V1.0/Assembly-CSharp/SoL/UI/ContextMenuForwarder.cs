using System;
using SoL.Game.Interactives;
using UnityEngine;

namespace SoL.UI
{
	// Token: 0x02000345 RID: 837
	public class ContextMenuForwarder : MonoBehaviour, IContextMenu, IInteractiveBase
	{
		// Token: 0x060016DB RID: 5851 RVA: 0x00051FEF File Offset: 0x000501EF
		private void Awake()
		{
			this.m_context = this.m_target.GetComponent<IContextMenu>();
		}

		// Token: 0x060016DC RID: 5852 RVA: 0x00052002 File Offset: 0x00050202
		public string FillActionsGetTitle()
		{
			IContextMenu context = this.m_context;
			if (context == null)
			{
				return null;
			}
			return context.FillActionsGetTitle();
		}

		// Token: 0x17000552 RID: 1362
		// (get) Token: 0x060016DD RID: 5853 RVA: 0x00052015 File Offset: 0x00050215
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				IContextMenu context = this.m_context;
				if (context == null)
				{
					return null;
				}
				return context.Settings;
			}
		}

		// Token: 0x060016DF RID: 5855 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04001EAC RID: 7852
		[SerializeField]
		private GameObject m_target;

		// Token: 0x04001EAD RID: 7853
		private IContextMenu m_context;
	}
}

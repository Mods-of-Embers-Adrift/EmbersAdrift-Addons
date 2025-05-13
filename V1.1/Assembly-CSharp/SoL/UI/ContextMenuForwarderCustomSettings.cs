using System;
using SoL.Game.Interactives;
using UnityEngine;

namespace SoL.UI
{
	// Token: 0x02000346 RID: 838
	public class ContextMenuForwarderCustomSettings : MonoBehaviour, IContextMenu, IInteractiveBase
	{
		// Token: 0x060016E0 RID: 5856 RVA: 0x00052030 File Offset: 0x00050230
		private void Awake()
		{
			this.m_context = this.m_target.GetComponent<IContextMenu>();
		}

		// Token: 0x060016E1 RID: 5857 RVA: 0x00052043 File Offset: 0x00050243
		public string FillActionsGetTitle()
		{
			IContextMenu context = this.m_context;
			if (context == null)
			{
				return null;
			}
			return context.FillActionsGetTitle();
		}

		// Token: 0x17000553 RID: 1363
		// (get) Token: 0x060016E2 RID: 5858 RVA: 0x00052056 File Offset: 0x00050256
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return this.m_interactionSettings;
			}
		}

		// Token: 0x060016E4 RID: 5860 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04001EAE RID: 7854
		[SerializeField]
		private InteractionSettings m_interactionSettings;

		// Token: 0x04001EAF RID: 7855
		[SerializeField]
		private GameObject m_target;

		// Token: 0x04001EB0 RID: 7856
		private IContextMenu m_context;
	}
}

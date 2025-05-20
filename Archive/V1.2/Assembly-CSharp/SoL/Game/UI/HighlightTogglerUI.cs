using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SoL.Game.UI
{
	// Token: 0x0200088D RID: 2189
	public class HighlightTogglerUI : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
	{
		// Token: 0x06003FC4 RID: 16324 RVA: 0x0006B1A0 File Offset: 0x000693A0
		private void Awake()
		{
			if (this.m_highlight)
			{
				this.m_highlight.enabled = false;
			}
		}

		// Token: 0x06003FC5 RID: 16325 RVA: 0x0006B1BB File Offset: 0x000693BB
		void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
		{
			if (this.m_highlight != null)
			{
				this.m_highlight.enabled = true;
			}
		}

		// Token: 0x06003FC6 RID: 16326 RVA: 0x0006B1D7 File Offset: 0x000693D7
		void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
		{
			if (this.m_highlight != null)
			{
				this.m_highlight.enabled = false;
			}
		}

		// Token: 0x04003D68 RID: 15720
		[SerializeField]
		private Image m_highlight;
	}
}

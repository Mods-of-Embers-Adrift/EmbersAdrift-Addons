using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SoL.UI
{
	// Token: 0x0200036E RID: 878
	public class SimpleHighlightToggle : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
	{
		// Token: 0x06001800 RID: 6144 RVA: 0x00052CEB File Offset: 0x00050EEB
		private void Awake()
		{
			if (this.m_image != null)
			{
				this.m_image.enabled = false;
			}
		}

		// Token: 0x06001801 RID: 6145 RVA: 0x00052D07 File Offset: 0x00050F07
		void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
		{
			if (this.m_image != null)
			{
				this.m_image.enabled = (this.m_button == null || this.m_button.interactable);
			}
		}

		// Token: 0x06001802 RID: 6146 RVA: 0x00052CEB File Offset: 0x00050EEB
		void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
		{
			if (this.m_image != null)
			{
				this.m_image.enabled = false;
			}
		}

		// Token: 0x04001F80 RID: 8064
		[SerializeField]
		private Image m_image;

		// Token: 0x04001F81 RID: 8065
		[SerializeField]
		private SolButton m_button;
	}
}

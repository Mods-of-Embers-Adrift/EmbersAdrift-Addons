using System;
using SoL.Managers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SoL.UI
{
	// Token: 0x0200036F RID: 879
	public class SliderHooks : MonoBehaviour, IBeginDragHandler, IEventSystemHandler, IEndDragHandler
	{
		// Token: 0x06001804 RID: 6148 RVA: 0x00052D3E File Offset: 0x00050F3E
		public void OnBeginDrag(PointerEventData eventData)
		{
			if (ClientGameManager.UIManager)
			{
				ClientGameManager.UIManager.IsUsingSlider = true;
			}
		}

		// Token: 0x06001805 RID: 6149 RVA: 0x00052D57 File Offset: 0x00050F57
		public void OnEndDrag(PointerEventData eventData)
		{
			if (ClientGameManager.UIManager)
			{
				ClientGameManager.UIManager.IsUsingSlider = false;
			}
		}

		// Token: 0x06001806 RID: 6150 RVA: 0x0004475B File Offset: 0x0004295B
		private void InfoBox()
		{
		}
	}
}

using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SoL.UI
{
	// Token: 0x02000360 RID: 864
	public class DragAbsorber : MonoBehaviour, IBeginDragHandler, IEventSystemHandler, IEndDragHandler, IDragHandler
	{
		// Token: 0x06001796 RID: 6038 RVA: 0x0004475B File Offset: 0x0004295B
		void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
		{
		}

		// Token: 0x06001797 RID: 6039 RVA: 0x0004475B File Offset: 0x0004295B
		void IEndDragHandler.OnEndDrag(PointerEventData eventData)
		{
		}

		// Token: 0x06001798 RID: 6040 RVA: 0x0004475B File Offset: 0x0004295B
		void IDragHandler.OnDrag(PointerEventData eventData)
		{
		}
	}
}

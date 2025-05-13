using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SoL.Game.UI
{
	// Token: 0x020008D5 RID: 2261
	public class SolStandaloneInputModule : StandaloneInputModule
	{
		// Token: 0x0600421C RID: 16924 RVA: 0x00191978 File Offset: 0x0018FB78
		public GameObject GameObjectUnderPointer(int pointerId)
		{
			PointerEventData lastPointerEventData = base.GetLastPointerEventData(pointerId);
			if (lastPointerEventData == null)
			{
				return null;
			}
			return lastPointerEventData.pointerCurrentRaycast.gameObject;
		}

		// Token: 0x0600421D RID: 16925 RVA: 0x0006CA03 File Offset: 0x0006AC03
		public GameObject GameObjectUnderPointer()
		{
			return this.GameObjectUnderPointer(-1);
		}
	}
}

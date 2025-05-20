using System;
using UnityEngine;

namespace SoL.Utilities.Extensions
{
	// Token: 0x02000328 RID: 808
	public static class CanvasGroupExtensions
	{
		// Token: 0x0600164A RID: 5706 RVA: 0x000518FE File Offset: 0x0004FAFE
		public static void SetActive(this CanvasGroup canvasGroup, bool isActive)
		{
			if (canvasGroup)
			{
				canvasGroup.alpha = (isActive ? 1f : 0f);
				canvasGroup.interactable = isActive;
				canvasGroup.blocksRaycasts = isActive;
			}
		}
	}
}

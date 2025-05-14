using System;
using UnityEngine;
using UnityEngine.UI;

namespace SoL.Utilities.Extensions
{
	// Token: 0x02000332 RID: 818
	public static class SpriteExtensions
	{
		// Token: 0x06001663 RID: 5731 RVA: 0x00100738 File Offset: 0x000FE938
		public static float GetAspectRatio(this Sprite sprite)
		{
			if (!(sprite == null))
			{
				return sprite.rect.width / sprite.rect.height;
			}
			return 1f;
		}

		// Token: 0x06001664 RID: 5732 RVA: 0x000519FC File Offset: 0x0004FBFC
		public static void CustomCrossFadeAlpha(this Image img, float startAlpha, float endAlpha, float duration)
		{
			if (img)
			{
				img.canvasRenderer.SetAlpha(startAlpha);
				img.CrossFadeAlpha(endAlpha, duration, false);
			}
		}
	}
}

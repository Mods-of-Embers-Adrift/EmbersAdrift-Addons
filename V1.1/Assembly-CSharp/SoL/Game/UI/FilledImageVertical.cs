using System;
using UnityEngine;

namespace SoL.Game.UI
{
	// Token: 0x02000884 RID: 2180
	[Serializable]
	public class FilledImageVertical : FilledImage
	{
		// Token: 0x06003F81 RID: 16257 RVA: 0x00189060 File Offset: 0x00187260
		protected override void RefreshEdge(float value)
		{
			if (!this.m_edge)
			{
				return;
			}
			float y = this.m_edge.rectTransform.sizeDelta.y;
			float num = ((this.m_parent.rect.size.y > 0f) ? this.m_parent.rect.size.y : this.m_fallbackWidth) * value;
			if (this.m_fill.fillOrigin == 1)
			{
				num *= -1f;
			}
			this.m_edge.rectTransform.anchoredPosition = new Vector2(0f, num);
			this.m_edge.enabled = ((this.m_fill.fillOrigin == 1) ? (num < -y) : (num > y));
		}
	}
}

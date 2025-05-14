using System;
using UnityEngine;
using UnityEngine.UI;

namespace SoL.Game.UI
{
	// Token: 0x02000883 RID: 2179
	[Serializable]
	public class FilledImage : FilledWithEdge
	{
		// Token: 0x17000EAD RID: 3757
		// (get) Token: 0x06003F7C RID: 16252 RVA: 0x0006AEDA File Offset: 0x000690DA
		// (set) Token: 0x06003F7D RID: 16253 RVA: 0x0006AEFB File Offset: 0x000690FB
		public override float fillAmount
		{
			get
			{
				if (!(this.m_fill != null))
				{
					return 0f;
				}
				return this.m_fill.fillAmount;
			}
			set
			{
				if (this.m_fill == null)
				{
					return;
				}
				this.m_fill.fillAmount = value;
				this.RefreshEdge(value);
			}
		}

		// Token: 0x06003F7E RID: 16254 RVA: 0x00188FF4 File Offset: 0x001871F4
		protected virtual void RefreshEdge(float value)
		{
			if (!this.m_edge)
			{
				return;
			}
			float x = this.m_edge.rectTransform.sizeDelta.x;
			float num = ((this.m_parent.rect.size.x > 0f) ? this.m_parent.rect.size.x : this.m_fallbackWidth) * value;
			if (this.m_fill.fillOrigin == 1)
			{
				num *= -1f;
			}
			this.m_edge.rectTransform.anchoredPosition = new Vector2(num, 0f);
			this.m_edge.enabled = ((this.m_fill.fillOrigin == 1) ? (num < -x) : (num > x));
		}

		// Token: 0x06003F7F RID: 16255 RVA: 0x0006AF1F File Offset: 0x0006911F
		public Color GetFillColor()
		{
			if (!this.m_fill)
			{
				return Color.white;
			}
			return this.m_fill.color;
		}

		// Token: 0x04003D4B RID: 15691
		[SerializeField]
		protected Image m_fill;
	}
}

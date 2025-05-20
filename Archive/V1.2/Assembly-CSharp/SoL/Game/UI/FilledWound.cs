using System;
using UnityEngine;

namespace SoL.Game.UI
{
	// Token: 0x02000885 RID: 2181
	[Serializable]
	public class FilledWound : FilledWithEdge
	{
		// Token: 0x17000EAE RID: 3758
		// (get) Token: 0x06003F83 RID: 16259 RVA: 0x0006AF4F File Offset: 0x0006914F
		// (set) Token: 0x06003F84 RID: 16260 RVA: 0x00189184 File Offset: 0x00187384
		public override float fillAmount
		{
			get
			{
				return this.m_fillAmount;
			}
			set
			{
				this.m_fillAmount = value;
				if (this.m_rect && this.m_parent)
				{
					float num = (this.m_parent.rect.size.x > 0f) ? this.m_parent.sizeDelta.x : this.m_fallbackWidth;
					Vector2 sizeDelta = this.m_rect.sizeDelta;
					sizeDelta.x = num * value;
					this.m_rect.sizeDelta = sizeDelta;
					if (this.m_edge)
					{
						this.m_edge.enabled = (sizeDelta.x > this.m_edge.rectTransform.sizeDelta.x);
					}
				}
			}
		}

		// Token: 0x04003D4C RID: 15692
		[SerializeField]
		private RectTransform m_rect;

		// Token: 0x04003D4D RID: 15693
		private float m_fillAmount;
	}
}
